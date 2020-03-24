
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    #define UNITY_EDITOR_LOG
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum BreakActionCause {
    SkillRelease, //放大招
    Dead, //死亡
    SkillInterrupt, //技能打断
    InterruptAction, //动作
    Vertigo, // 眩晕打断
    Hit, // 受击
}



public enum AngerStyle {
    SkillLaunch,  //技能发起
    TargetDead,   //目标死亡
}


public class Walk : Move
{

    protected RoleGridRun m_run = null;
    public RadarAI m_RadarAI = null;
    public RoleGridRun run {
        get{return m_run;}
    }
    
    public List<GridActionCmd> m_ActionList = new List<GridActionCmd>();
    public Walk(Life Parent)
        : base(Parent)
    {
        InitAI();
    }
    
    
    public override void SetBornPos(Int2 BornPos, int deep)
    {
        m_Owner.MapPos = BornPos;
        m_run = new RoleGridRun(m_Owner);
        m_run.SetBorn(BornPos, deep);
        m_Owner.InBoat = true;
        m_Owner.WaitServer = true;
        tga.SoldierBornRequest sbr = new tga.SoldierBornRequest();
        sbr.time = new tga.tgaTime();
        sbr.time.Requesttime = Time.realtimeSinceStartup;
        BSC.SoldierBornRequest(m_Owner.m_Core.m_DataID, sbr);
    }
    
    /// <summary>
    /// 初始化AI
    /// </summary>
    public void InitAI()
    {
    
    }
    /// <summary>
    /// FixedUpdate 调用
    /// </summary>
    public override void FixedUpdate()
    {
    }
    
    /// <summary>
    /// LateUpdate 调用
    /// </summary>
    public override void LateUpdate()
    {
    
        if (!Role.s_bCalcNext) {
            CalcNextAction();
            Role.s_bCalcNext = true;
        }
        if (!Life.ServerMode || m_Owner.isRequairAi) {
            run.UpdataPath();
        }
    }
    //预测下一个动作
    public static void CalcNextAction()
    {
        MapCheckStations.FinishStations();
        
        //检测攻击
        Role.CheckAttack();
    }
    /// <summary>
    /// Update 调用
    /// </summary>
    public override void Update()
    {
        SoldierSkill conditionskillinfo = new SoldierSkill();
        bool isnewaction = false;
        if (CurrentAction == null) {
            CurrentAction =  run.GetNextAction();
            DoPathEvent();
#if UNITY_EDITOR_LOG
            FileLog.write(m_Owner.SceneID, m_Owner.SceneID + ",CurrentAction:" + CurrentAction + ",mStart:" + CurrentAction.m_Start + ",mEnd:" + CurrentAction.m_End
                + ",dir:" + CurrentAction.m_Dir
                + ",mDuration:" + CurrentAction.m_Duration + ",mattackid:" + CurrentAction.m_AttackSceneID + ",mGrid:"  + m_Owner.MapPos
                + ",mNGrid:" + m_run.NGrid + ",deep:" + m_run.RankDeep + ",reject" + m_run.Reject);
#endif
        } else if (CurrentAction.IsDone() /*&& !m_Owner.WaitServer*/) {
            if (m_Owner.m_bTrunBack) {
                m_Owner.TurnsInto(false);
            } else {
#if UNITY_EDITOR_LOG
                string str = "skill:" + m_Owner.PropAttackSkillInfo.m_type +  "  ,pos" + m_Owner.m_thisT.localPosition + ",mGrid:"  + m_Owner.MapPos + ",mNGrid:" + m_run.NGrid + ",deep:" + m_run.RankDeep + ",reject" + m_run.Reject
                    + ", attackstation:" + run.AttackStation + ", Nattackstation:" + run.NAttackStation + ",  olddir:" + m_Owner.WalkDir;
#endif
                isnewaction = true;
                m_ActionList.Add(CurrentAction);
                CurrentAction =  run.GetNextAction();
#if UNITY_EDITOR_LOG
                if (CurrentAction != null)
                    FileLog.write(m_Owner.SceneID, m_Owner.SceneID + ",CurrentAction1111:" + CurrentAction  + ",mStart:" + CurrentAction.m_Start + ",mEnd:" + CurrentAction.m_End
                        + ",dir:" + CurrentAction.m_Dir + ", new dir:" + m_Owner.WalkDir + ",mDuration:" + CurrentAction.m_Duration + ",mattackid:" + CurrentAction.m_AttackSceneID + str  + "newGrid" + m_Owner.MapPos + " time:" + Time.realtimeSinceStartup + ", curpos :" + m_Owner.m_thisT.localPosition);
#endif
                        
                m_Owner.m_Hit = false;
                if (!m_Owner.m_isDead) {
                    if (m_ActionList[m_ActionList.Count - 1 ] is GridActionCmdConditionSkill && !(m_ActionList[m_ActionList.Count - 1 ] as GridActionCmdConditionSkill).RealDone()) {
                        GridActionCmdConditionSkill  cs = m_ActionList[m_ActionList.Count - 1 ] as GridActionCmdConditionSkill;
                        
                        if (!(CurrentAction is GridActionCmdJump
                                || CurrentAction is GridActionCmdStair
                                || CurrentAction is GridActionCmdFall
                                || CurrentAction is GridActionCmdSpecialJump
                                || CurrentAction is GridActionCmdHitByBuild
                                || CurrentAction is GridActionCmdJumpDown
                                || CurrentAction is GridActionCmdJumpUp)
                        ) {
                            bool ismove = false;
                            if (CurrentAction is GridActionCmdWalk) {
                                ismove = true;
                            }
                            cs.ExtendDuration(CurrentAction.m_Duration, CurrentAction.m_Start, CurrentAction.m_End, ismove);
#if UNITY_EDITOR_LOG
                            FileLog.write(m_Owner.SceneID, m_Owner.SceneID + ",CurrentAction4444:" + m_Owner.m_thisT.localPosition);
#endif
                            CurrentAction = cs;
                        }
                    } else if (RoleSkill.CheckConditionSkillAction(m_Owner.m_SceneID, ref conditionskillinfo, true, m_Owner.m_SkillRelease)) {
#if UNITY_EDITOR_LOG
                        FileLog.write(m_Owner.SceneID, m_Owner.SceneID + ",CurrentAction3333:" + m_Owner.m_thisT.localPosition  + "," + conditionskillinfo.m_type + "," + conditionskillinfo.m_name);
#endif
                        if (!(CurrentAction is GridActionCmdJump || CurrentAction is GridActionCmdStair || CurrentAction is GridActionCmdFall || CurrentAction is GridActionCmdSpecialJump)) {
                            bool ismove = false;
                            if (CurrentAction is GridActionCmdWalk) {
                                ismove = true;
                            }
                            //Debug.Log(m_CurrentAction + "," + conditionskillinfo.m_type);
                            GridActionCmd action = null;
                            if (conditionskillinfo.m_ismove == 1) {
                                action = RoleSkill.GetConditionSkillAction(m_Owner.m_SceneID, conditionskillinfo, CurrentAction.m_Start, CurrentAction.m_End, CurrentAction.m_Duration, ismove);
                            } else {
                                action = RoleSkill.GetConditionSkillAction(m_Owner.m_SceneID, conditionskillinfo, CurrentAction.m_Start, CurrentAction.m_Start, CurrentAction.m_Duration, ismove);
                            }
                            
                            if (action != null && action is GridActionSkill) {
                                if (m_Owner.m_Status.SeleStatusInterRupt(conditionskillinfo) == false) {
                                    CurrentAction = action;
                                    run.ResetIndex(conditionskillinfo.m_ismove == 1);
                                } else {
                                    action.SetDone();
                                }
                                
                            }
                        }
                    }
                    DoPathEvent();
                    m_Owner.m_SkillRelease = false;
                    if (CurrentAction != null) {
                        List<IggWall> lw = new List<IggWall>();
                        MapGrid.GetWallList(ref lw, m_Owner.GetMapPos(), run.AttackStation);
                        foreach (IggWall w in lw) {
                            w.OpenDoor(OpenDoorStyle.passDoor, m_Owner.m_Core.m_Camp, Mathf.Abs(m_Owner.GetMapPos().Unit - w.GetMapPos().Unit));
                        }
#if UNITY_EDITOR_LOG
                        FileLog.write(m_Owner.SceneID, run.PreGrid + "," + m_Owner.SceneID + ",CurrentAction2222:" + CurrentAction  + ",mStart:" + CurrentAction.m_Start + ",mEnd:" + CurrentAction.m_End
                            + ",dir:" + CurrentAction.m_Dir  + "new dir" + m_Owner.WalkDir + ",mDuration:" + CurrentAction.m_Duration + ",mattackid:" + CurrentAction.m_AttackSceneID + str  + "newGrid" + m_Owner.MapPos + " time:" + Time.realtimeSinceStartup + ", curpos :" + m_Owner.m_thisT.localPosition);
#endif
                    }
                }
            }
        } else {
            if (RoleSkill.CheckConditionSkillAction(m_Owner.m_SceneID, ref conditionskillinfo, false, m_Owner.m_SkillRelease)) {
                if (CurrentAction is GridActionCmdAttack || CurrentAction is GridActionCmdConditionSkill) {
                    if (conditionskillinfo.m_ipriority > m_Owner.PropSkillInfo.m_ipriority) {
                        CurrentAction.SetDone();
                        m_ActionList.Add(CurrentAction);
                        CurrentAction = RoleSkill.GetConditionSkillAction(m_Owner.m_SceneID, conditionskillinfo, m_Owner.m_thisT.localPosition, m_Owner.m_thisT.localPosition, 0f, false);
                    }
                }
                
            }
            
        }
        if (CurrentAction != null) {
            if (CurrentAction is GridActionCmdWalk) {
                List<int> rolelist = new List<int>();
                MapGrid g = MapGrid.GetMG(run.PreGrid);
                g.GetRoleList(ref rolelist);
                int bwalk = 0;
                if (rolelist.Count > 0) {
                    for (int i = 0; i < rolelist.Count; i++) {
                        Role l = CM.GetAllLifeM(rolelist[i], LifeMType.SOLDIER | LifeMType.SUMMONPET) as Role;
                        if (l == null) {
                            continue;
                        }
                        if (l != m_Owner && l.m_Attr.Goorder >= m_Owner.m_Attr.Goorder) {
                            if (l.CurrentAction is GridActionCmdWalk) {
                                bwalk ++;
                                if (l.m_Attr.Goorder == m_Owner.m_Attr.Goorder && l.SceneID < m_Owner.SceneID) {
                                    bwalk --;
                                }
                            }
                        }
                    }
                }
                if (!isnewaction) {
                    if (bwalk == 0) {
                        if (CurrentAction.m_TimeCount == 0 && m_Owner.m_ajustcount < m_Owner.m_ajustduration) {
                            m_Owner.AdjustPosition(Time.deltaTime);
                        } else {
                            CurrentAction.Update();
                        }
                    } else if (bwalk == 1 && CurrentAction.m_TimeCount == 0) {
                        m_Owner.AdjustPosition(Time.deltaTime);
                    }
                    
                } else {
                    if (bwalk > 0) {
                        run.ChangeDeep(0);
                        //m_Owner.m_ajustcount = m_Owner.m_ajustduration;
                    } else {
                        m_Owner.m_ajustcount += m_Owner.m_ajustduration;
                    }
                }
            } else {
                CurrentAction.Update();
            }
            if (CurrentAction.TestNextDone()) {
                if (CurrentAction is GridActionCmdAttack || CurrentAction is GridActionCmdCDStand
                    || CurrentAction is GridActionCmdHit || CurrentAction is GridActionCmdSkillStand
                    || CurrentAction is GridActionCmdFallDown || (CurrentAction is GridActionCmdConditionSkill && m_Owner.PropSkillInfo.m_ismove == 0)) {
                    run.RegisterNextAction(false);
                } else {
                    run.RegisterNextAction();
                }
                
            }
        } else {
#if UNITY_EDITOR_LOG
            FileLog.write(m_Owner.m_SceneID, m_Owner.m_SceneID + "创建action出错" + ",技能类型：" + m_Owner.PropSkillInfo.m_type);
            Debug.LogError(m_Owner.m_thisT.gameObject.name + m_Owner.m_SceneID + "创建action出错" + ",技能类型：" + m_Owner.PropSkillInfo.m_type);
#endif
        }
    }
    public bool DoPathCheck(AIEventData data)
    {
    
        if (data.Event == AIEvent.EVENT_MAP) {
            if (MapGrid.GetMG(m_Owner.MapPos).Type == GridType.GRID_HOLE || MapGrid.GetMG(run.NGrid).Type == GridType.GRID_HOLE) {
                return true;
            }
        }
        
        if (run.m_FindAttackpos) {
            return false;
        }
        if (data.Event == AIEvent.EVENT_SELFSP) {
            return true;
        }
        if (m_Owner.m_Skill.CheckAttackTarget()) {
            return false;
        }
        if (CurrentAction is GridActionCmdFall) {
            return false;
        }
        
        return true;
    }
    public void DoPathEvent()
    {
        LifeMCamp Camp = (m_Owner.m_Core.m_Camp == LifeMCamp.ATTACK) ? LifeMCamp.DEFENSE : LifeMCamp.ATTACK;
        List<Life> RoleList = new List<Life>();
        CM.SearchLifeMListInBoat(ref RoleList, LifeMType.SOLDIER | LifeMType.SUMMONPET, Camp);
        if (CurrentAction is GridActionCmdStair || CurrentAction is GridActionCmdJump || CurrentAction is GridActionCmdFall) {
            AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_INA), RoleList);
        } else if ((m_ActionList.Count > 0 && (m_ActionList[m_ActionList.Count - 1] is GridActionCmdStair || m_ActionList[m_ActionList.Count - 1] is GridActionCmdJump || m_ActionList[m_ActionList.Count - 1] is GridActionCmdSpecialJump))
            || CurrentAction is GridActionCmdFallDown) {
            AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_FMA), RoleList);
        } else if (CurrentAction is GridActionCmdWalk) {
            AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_TM), RoleList);
        }
    }
    public  void SeachTarget()
    {
        if (m_Owner.m_Status.HaveState(StatusType.RunAway)) {
            m_run.PathAI(m_Owner.GetMapGrid(), m_Owner.RunPos, m_Owner.m_Attr);
            return;
        }
        bool IsTault = false;
        int tSceneID = m_Owner.m_Status.GetResalseSceneID(StatusType.Taunt);
        Life t = CM.GetLifeM(tSceneID, LifeMType.SOLDIER);
        if (t != null && t is Role && t.InBoat && m_Owner.CheckTarget(t)) {
            IsTault = true;
        } else {
            tSceneID = -1;
        }
        if (m_Owner.m_Status.HaveState(StatusType.FakeTaunt)) {
        
            LifeMCamp camp = m_Owner.m_Core.m_Camp == LifeMCamp.ATTACK ? LifeMCamp.DEFENSE : LifeMCamp.ATTACK;
            List<Life> l = new List<Life>();
            ///需要排除隐形 。。。。。。。。。。。。。。。。。。
            CM.SearchLifeMListInBoat(ref l, LifeMType.SOLDIER, camp);
            foreach (Role r in l) {
                if (r.m_Status.HaveState(StatusType.Mark)) {
                    t = r;
                    IsTault = true;
                }
            }
        }
        
        //
        if (m_Owner.m_isDead) {
            return;
        }
        if (m_Owner.m_Core.m_Camp == LifeMCamp.ATTACK && (m_Owner.m_Attr.AttrType != 3000 || !m_Owner.m_bReachGold)) {
            if (IsTault) {
                m_Owner.m_TargetisAttack = false;
                m_Owner.ChangeTarget(ChangeTargetReason.Taunt, t);
                
#if UNITY_EDITOR_LOG
                FileLog.write(m_Owner.SceneID, "SeachTarget  + m_Owner.Target = t;");
#endif
                SeachPath(t);
            }
            if (m_Owner.Target != null && (m_Owner.Target != null && !m_Owner.Target.isDead && m_Owner.m_TargetisAttack)) {
                SeachPath(m_Owner.Target);
            } else if (m_Owner.m_Attr.IsHide) {
                if (m_Owner.m_target == null || (m_Owner.m_target != null && m_Owner.m_target.isDead)) {
                    int TargetSceneID = m_RadarAI.GetSearchTarget(tSceneID, m_Owner.GetMapGrid(), m_Owner.m_Attr.AttackLike);
                    m_Owner.m_TargetisAttack = false;
                    
#if UNITY_EDITOR_LOG
                    FileLog.write(m_Owner.SceneID, "SeachTarget  +  CM.GetAllLifeM(TargetSceneID,LifeMType.ALL)");
#endif
                    m_Owner.ChangeTarget(ChangeTargetReason.RadarSearchTarget, CM.GetAllLifeM(TargetSceneID, LifeMType.ALL));
                    //m_Owner.Target =  CM.GetAllLifeM(TargetSceneID,LifeMType.ALL);
                }
                if (m_Owner.Target != null && !m_Owner.Target.isDead) {
                    MapGrid End = m_Owner.Target.GetTargetMapGrid();
                    if (m_Owner.Target is Role) {
                        MapGrid g = End;
                        if (m_Owner.Target.WalkDir == WalkDir.WALKRIGHT) {
                            g = g.Left;
                            while (g != null) {
                                if (g.Type == GridType.GRID_NORMAL) {
                                    if (g.IsAttackStations()) {
                                        End = g;
                                        break;
                                    } else {
                                        g = g.Left;
                                    }
                                } else {
                                    break;
                                }
                            }
                        } else if (m_Owner.Target.WalkDir == WalkDir.WALKLEFT) {
                            g = g.Right;
                            while (g != null) {
                                if (g.Type == GridType.GRID_NORMAL) {
                                    if (g.IsAttackStations()) {
                                        End = g;
                                        break;
                                    } else {
                                        g = g.Right;
                                    }
                                } else {
                                    break;
                                }
                            }
                        }
                    }
                    if (End != null) {
                        m_run.PathAI(m_Owner.GetMapGrid(), End, m_Owner.m_Attr);
                    }
                } else {
                    m_run.CheackIdlePath();
                }
            } else if (m_Owner.Target != null && (m_Owner.Target != null && m_Owner.Target is Role && !m_Owner.Target.isDead && m_Owner.TargetAttacked)) {
                Role roletarget = m_Owner.Target as Role;
                //Debug.Log(m_Owner.SceneID + "," +m_Owner.GetMapPos() + "," + roletarget.GetMapPos() + "," + roletarget.transform.localPosition + "," + m_Owner.transform.localPosition + "," +m_Owner.PropAttackSkillInfo.m_distance * MapGrid.m_width / MapGrid.m_Pixel);
                if (NdUtil.IsLifeSampMapLayer(roletarget, m_Owner.GetMapPos()) &&
                    Mathf.Abs(roletarget.m_thisT.localPosition.x - m_Owner.m_thisT.localPosition.x) - 0.25f < m_Owner.PropAttackSkillInfo.m_distance * MapGrid.m_width / MapGrid.m_Pixel) {
                    
                    SeachPath(m_Owner.Target);
                } else {
                    m_Owner.m_TargetisAttack = false;
                    SeachPath(CM.GoldBuild, true);
                }
            } else {
                m_Owner.m_TargetisAttack = false;
                SeachPath(CM.GoldBuild, true);
            }
        } else {
            if (m_Owner.m_target == null || (m_Owner.m_target != null && m_Owner.m_target.isDead)  || (IsTault && m_Owner.m_target != t)) {
#if UNITY_EDITOR_LOG
                string str = "SeachTarget new  old:" + m_Owner.m_target  + "," + m_Owner.m_TargetAttacked + m_Owner.GetMapPos();
                if (m_Owner.m_target != null) {
                    str += "," + m_Owner.m_target.isDead;
                }
#endif
                int TargetSceneID = m_RadarAI.GetSearchTarget(tSceneID, m_Owner.GetMapGrid(), m_Owner.m_Attr.AttackLike);
                m_Owner.m_TargetisAttack = false;
                m_Owner.ChangeTarget(ChangeTargetReason.RadarSearchTarget, CM.GetAllLifeM(TargetSceneID, LifeMType.ALL));
#if UNITY_EDITOR_LOG
                FileLog.write(m_Owner.SceneID, str +  "   new  " + m_Owner.m_target);
#endif
            }
#if UNITY_EDITOR_LOG
            FileLog.write(m_Owner.SceneID, "SeachTarget old  " + m_Owner.m_target);
#endif
            SeachPath();
        }
        
        
    }
    public Life GetAttacklikeTarget()
    {
    
        int TargetSceneID = m_RadarAI.GetSearchTarget(-1, m_Owner.GetMapGrid(), m_Owner.m_Attr.AttackLike);
        m_Owner.m_TargetisAttack = false;
        Life target = CM.GetAllLifeM(TargetSceneID, LifeMType.ALL);
        m_Owner.ChangeTarget(ChangeTargetReason.RadarSearchTarget, target);
        return target;
    }
    public  void SeachPath(Life t, bool CutPath = false)
    {
        if (t != null && !t.isDead) {
            MapGrid End = t.GetTargetMapGrid();
            if (End != null) {
#if UNITY_EDITOR_LOG
                FileLog.write(m_Owner.SceneID, "SeachPath :"   + m_Owner.GetMapPos());
                
#endif
                m_run.PathAI(m_Owner.GetMapGrid(), End, m_Owner.m_Attr, CutPath);
            }
        } else {
            m_run.CheackIdlePath();
        }
    }
    public  void SeachPath()
    {
        if (m_Owner.Target != null && !m_Owner.Target.isDead) {
            MapGrid End = m_Owner.Target.GetTargetMapGrid();
            if (End != null) {
                m_run.PathAI(m_Owner.GetMapGrid(), End, m_Owner.m_Attr);
            }
        } else {
            m_run.CheackIdlePath();
        }
    }
    
    public void Teleport(MapGrid g, bool ischange = true)
    {
#if UNITY_EDITOR_LOG
        FileLog.write(m_Owner.SceneID, "Teleport " + g.GridPos + "," + m_Owner.m_thisT.localPosition);
#endif
        LifeMCamp Camp = (m_Owner.m_Core.m_Camp == LifeMCamp.ATTACK) ? LifeMCamp.DEFENSE : LifeMCamp.ATTACK;
        List<Life> RoleList = new List<Life>();
        CM.SearchLifeMListInBoat(ref RoleList, LifeMType.SOLDIER | LifeMType.SUMMONPET, Camp);
        AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_TM), RoleList);
        AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_SELFSP), m_Owner);
        if (ischange) {
            m_Owner.ChangeTarget(ChangeTargetReason.Teleport, null);
        }
        run.ClearPath();
        run.Teleport(g);
    }
}

