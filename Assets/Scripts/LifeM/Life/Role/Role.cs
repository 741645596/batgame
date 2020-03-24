#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    #define UNITY_EDITOR_LOG
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum AttackWeight {
    like = 5,
    Soldier = 4,
    build = 3,
    wall = 2,
    gold = 1,
}

public enum ChangeTargetReason {
    ClearOtherTarget,		//清除其他以这个兵为目标的炮弹兵，并重新寻路
    ClearTarget, 			//清楚目标
    ClearTargetAndPath, 	//清楚目标并寻路
    HitChangetTarget, 		//受击的时候改变目标
    TimeOut,				//时间超时
    HitFlyClearTarget,		//熊大击飞敌方的时候需要清目标
    SkillChangeTarget,		//技能改变目标，根据条件m_ischangetarget==1
    Taunt,					//因为嘲讽改变目标
    RadarSearchTarget,		//雷达寻路目标
    Teleport,				//瞬移清理目标
    InGoldPath,				//在金库路线上的目标
}
public class Role : Life
{
    public RolePropertyM m_Property;
    public override LifeProperty GetLifeProp()
    {
        return m_Property;
    }
    public override void SetLifeProp(LifeProperty lifeObj)
    {
        m_Property = lifeObj as RolePropertyM;
    }
    public RoleSkin m_Skin = null;
    public RoleSkin RoleSkinCom{
        get{return m_Skin;}
    }
    public void CreateSkin(Transform parent, int roleType, string roleName, AnimatorState aniState, bool isplayer = true)
    {
        m_Skin = new RoleSkin();
        m_Skin.CreateSkin(parent, roleType, roleName, aniState, isplayer);
        LifeProperty(m_Skin.ProPerty);
    }
    public override Skin GetSkin()
    {
        return m_Skin;
    }
    public override bool SetSkin()
    {
        //m_Skin = new RoleSkin();
        if (m_Skin != null) {
            m_Skin.SetGameRole(this, m_Core.m_MoveState, m_Core.m_Camp, m_Core.m_IsPlayer);
        }
        return true;
    }
    // 行走方向
    //[HideInInspector]
    //protected WalkDir  m_WalkDIR =WalkDir.WALKSTOP ;
    //玩家不受伤害，用来保证某些怪物至少触发一次技能
    public bool NoDie = false;
    //当前Action
    protected GridActionCmd m_CurrentAction = null;
    public GridActionCmd CurrentAction {
        get{return m_CurrentAction;}
        set{m_CurrentAction = value;}
    }
    //walk
    /**********************************************************/
    public bool m_bReachGold = false;
    
    //至少对目标攻击过一次
    public bool m_TargetAttacked;
    /// <summary>
    /// 当前炮弹兵的z值
    /// </summary>
    private float m_fCurrentZ = 0;
    //search 目标
    public Life m_target = null;
    public bool m_Hit = false;
    
    
    public bool m_SkillRelease = false;
    public float m_NoHurtCount;
    public Pet m_Pet;
    public Pet CurPet {
        get{return m_Pet;}
        set{m_Pet = value;}
    }
    //预测标识
    public static bool s_bCalcNext;
    public RoleSkill RoleSkill {
        get{return m_Skill as RoleSkill;}
    }
    public float CDTime {
        get {return m_Skill.CDTime;}
    }
    public SoldierSkill PropAttackSkillInfo {
        get { return m_Skill.PropAttackSkillInfo; }
        set { m_Skill.PropAttackSkillInfo = value;}
    }
    public SoldierSkill PropSkillInfo {
        get { return m_Skill.PropSkillInfo as SoldierSkill; }
        set { m_Skill.PropSkillInfo = value;}
    }
    //攻击自己的目标
    public bool m_TargetisAttack;
    //目标的有效时间
    public float m_TargetDuration;
    //判断是否强制更改目标
    public bool m_ForceChangeTarget = false;
    public bool ForceChangeTarget {
        get { return m_ForceChangeTarget; }
        set { m_ForceChangeTarget = value;}
    }
    //调整位置的时间
    public float m_ajustduration = 0.3f;
    //调整位置的技术
    public float m_ajustcount = 0.3f;
    //调整位置
    public Vector3 m_ajusttopostion;
    //调整位置
    public Vector3 m_ajustfrompostion;
    //
    public bool m_bReBorn;
    public float m_changetargettime = 30f;
    public static float s_deepoffset = 0.25f;
    public Life Target {
        get{return m_target;}
        set{
            if (m_target == null || m_Attr.AttackLike != 4)
            {
                if (m_target == null && value != null) {
                    FindTarget();
                }
                if (m_target != value) {
                    m_TargetAttacked = false;
                }
                m_target = value;
                string str = "";
                if (m_target != null && m_target.isDead == false) {
                    m_TargetDuration = m_changetargettime;
                    str += "," + m_target.GetMapPos() + "," + m_target.m_thisT.localPosition;
                }
#if UNITY_EDITOR_LOG
                FileLog.write(SceneID, "设置目标: " + m_target + ",sceneid:" + (m_target != null ? m_target.SceneID.ToString() : "") + "," + m_TargetAttacked + "," + m_TargetisAttack + "," + m_TargetDuration
                    + str);
#endif
            }
        }
    }
    public bool TargetAttacked {
        get{return m_TargetAttacked;}
        set{
            m_TargetAttacked = value;
            if (m_TargetAttacked)
            {
                m_TargetDuration = m_changetargettime;
            }
        }
    }
    
    public RoleGridRun run {
        get{
            if (m_MoveAI is Walk)
            {
                return (m_MoveAI as Walk).run;
            } else
#if UNITY_EDITOR_LOG
                FileLog.write(SceneID, "当前状态不是walk，不能访问run");
#endif
            return null;
        }
    }
    
    public Walk RoleWalk {
        get{
            if (m_MoveAI is Walk)
            {
                return m_MoveAI as Walk;
            }
            return null;
        }
    }
    
    public Fly RoleFly {
        get{
            if (m_MoveAI is Fly)
            {
                return m_MoveAI as Fly;
            }
            return null;
        }
    }
    //逃跑点
    public MapGrid RunPos;
    public bool m_bTrunBack = false;
    /*********************************************************************************/
    
    /// <summary>
    /// 切换到ActionWalk
    /// </summary>
    public  void ActionFlyToWalk()
    {
        CombatWnd wnd = WndManager.FindDialog<CombatWnd>();
        if (wnd != null) {
            wnd.Set3DHeadState(SceneID, AnimatorState.UIAttack);
        }
        m_Skin.tRoot.parent = BattleEnvironmentM.GetLifeMBornNode(true);
        SetMoveState(MoveState.Walk);
        //m_Skin.EnableColider(ColiderType.Click ,true);
        m_Skin.EnableColider(false);
        //MoveCameraMid();
    }
    /// <summary>
    /// 相机移近
    /// 1：全部发兵后，且切换到Walk状态时
    /// 2：全部发兵后，最后一只兵飞行死亡时
    /// </summary>
    void MoveCameraMid()
    {
        if (CmCarbon.IsAllFireOut()) {
            MainCameraM.s_Instance.AutoMoveTo(MainCameraM.s_vBattleBoatviewCamPos);
        }
    }
    /****************************************************************************************************************************************/
    
    public void SetRoleLife(SoldierInfo Info, LifeProperty SkinProperty, LifeEnvironment Environment)
    {
        if (Info == null || SkinProperty == null) {
            return ;
        }
        LifeProperty(SkinProperty);
        if (Environment == LifeEnvironment.Combat) {
        
        } else {
        
        }
    }
    
    
    
    public override bool CheckInVision(Life life)
    {
        if (life == null || life.m_Attr == null) {
            return false;
        }
        if (life.m_Attr.RadarSearch == false || life.m_Attr.IsHide == true) {
            return false;
        }
        if (!CheckTarget(life)) {
            return false;
        }
        if (life is Pet) {
            if (CanAttackTarget(life) == false) {
                return false;
            }
        }
        
        if (m_Core.m_Camp == LifeMCamp.DEFENSE) {
            if (m_Attr.Attacked == true) {
                return true;
            } else {
                if (CheckSoldier(GetMapPos(), life.GetMapPos()) == true) {
                    return true;
                }
            }
            return false;
        } else {
            return true;
        }
    }
    
    private bool CheckSoldier(Int2 start, Int2 end)
    {
        int dis = (start.Layer - end.Layer);
        if (dis < 0) {
            dis = -dis;
        }
        if (dis < 1) {
            return true;
        } else {
            return false;
        }
    }
    // 初始化位置相关处理
    private void StarPosProc()
    {
        //掉下就进行寻路 友方兵出生的时候
        if (m_Core.m_Camp == LifeMCamp.ATTACK) {
            LifeMCamp Camp = (m_Core.m_Camp == LifeMCamp.ATTACK) ? LifeMCamp.DEFENSE : LifeMCamp.ATTACK;
            List<Life> RoleList = new List<Life>();
            CM.SearchLifeMListInBoat(ref RoleList, LifeMType.SOLDIER | LifeMType.SUMMONPET, Camp);
            AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_ADD), RoleList);
        }
        
    }
    
    public  bool IsInLayer(int layer)
    {
        if (GetMapPos().Layer == layer) {
            return true;
        } else {
            return false;
        }
    }
    
    
    /// <summary>
    /// 炮弹兵置顶显示
    /// </summary>
    public void SetTopMostPos(int layer = 0)
    {
        m_thisT.localPosition = new Vector3(m_thisT.localPosition.x, m_thisT.localPosition.y, -2.5f + layer * -1f);
        //NGUIUtil.PauseGame();
    }
    /// <summary>
    /// 炮弹兵置后显示
    /// </summary>
    public void SetBottomMostPos()
    {
        m_fCurrentZ = m_thisT.localPosition.z;
        m_thisT.localPosition = new Vector3(m_thisT.localPosition.x, m_thisT.localPosition.y, 1.8f);
    }
    
    /// <summary>
    /// 恢复炮弹兵位置
    /// </summary>
    public void ResetPos()
    {
        m_thisT.localPosition = new Vector3(m_thisT.localPosition.x, m_thisT.localPosition.y, m_fCurrentZ);
    }
    
    public void SavePosZ()
    {
        m_fCurrentZ = m_thisT.localPosition.z;
    }
    
    
    public void InterruptAction(AIEventData data)
    {
        if (data.Event == AIEvent.EVENT_MAP) {
            if (MapGrid.GetMG(MapPos).Type == GridType.GRID_HOLE) {
                //m_InterruptAction = true;
                BreakAction(BreakActionCause.InterruptAction);
            }
        }
    }
    public bool CheckTarget(Life target)
    {
        if ((m_Attr.AttackType & (int)TargetType.Gold) == (int)TargetType.Gold) {
            if (target == CM.GoldBuild) {
                return true;
            }
        }
        if ((m_Attr.AttackType & (int)TargetType.Pet) == (int)TargetType.Pet) {
            if (target is SummonPet) {
                return true;
            }
        }
        if ((m_Attr.AttackType & (int)TargetType.Soldier) == (int)TargetType.Soldier) {
            if (target is Role) {
                return true;
            }
        }
        if ((m_Attr.AttackType & (int)TargetType.Trap) == (int)TargetType.Trap) {
            if (target is Building && !target.m_Attr.IsResource) {
                return true;
            }
        }
        return false;
    }
    
    public void ChangeTarget(ChangeTargetReason r, Life target)
    {
        if (target != null && !CheckTarget(target)) {
            return ;
        }
        switch (r) {
            case ChangeTargetReason.ClearOtherTarget:
                if (target == Target) {
                    Target = null;
                    AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_SELFSP), this);
                }
                break;
            case ChangeTargetReason.ClearTargetAndPath:
                Target = null;
                AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_SELFSP), this);
                break;
            case ChangeTargetReason.HitChangetTarget:
                if (target != null &&  m_Core.m_MoveState == MoveState.Walk) {
                    bool IsTault = false;
                    int tSceneID = m_Status.GetResalseSceneID(StatusType.Taunt);
                    Life t = CM.GetLifeM(tSceneID, LifeMType.SOLDIER);
                    if (t != null && t is Role) {
                        IsTault = true;
                        
#if UNITY_EDITOR_LOG
                        FileLog.write(SceneID, "ApplyDamage   IsTault");
#endif
                        Target = t;
                    }
                    if (!IsTault && target.m_Core.m_Camp != m_Core.m_Camp && target is Role && target.InBoat) {
                        if (Target == null) {
                        
#if UNITY_EDITOR_LOG
                            FileLog.write(SceneID, "ApplyDamage  Target = null");
#endif
                            Target = target;
                            m_TargetisAttack = true;
                            AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_CHANGETARGET), this);
                        } else {
                        
                            if (target == Target) {
                                m_TargetDuration = m_changetargettime;
                                m_TargetisAttack = true;
                            } else if (NdUtil.IsLifeSampMapLayer(target, GetMapPos())) {
                                Life targetl = GetAttackTarget(target);
                                if (targetl != Target) {
                                    m_TargetisAttack = true;
#if UNITY_EDITOR_LOG
                                    FileLog.write(SceneID, "ApplyDamage  Target = targetl");
#endif
                                    Target = targetl;
                                    
                                    AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_CHANGETARGET), this);
                                    ForceChangeTarget = true;
                                }
                            }
                        }
                    }
                }
                break;
            case ChangeTargetReason.HitFlyClearTarget:
                Target = null;
                AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_SELFSP), this);
                break;
            /*case ChangeTargetReason.TimeOut:
            	break;
            
            case ChangeTargetReason.ToFlyClearTarget:
            	Target = target;
            	break;*/
            default:
                Target = target;
                break;
        }
    }
    
    
    
    public static void CheckAttack()
    {
        List<Life> listRAW = new List<Life>();
        CM.SearchLifeMListInBoat(ref listRAW, LifeMType.SOLDIER | LifeMType.SUMMONPET);
        foreach (Life item in listRAW) {
            Role raw = item as Role;
            RoleGridRun r = raw.run;
            r.CheckAttack();
        }
    }
    
    /// <summary>
    /// 给自己添加怒气
    /// </summary>
    /// <param name="Style">添加怒气的方式</param>
    /// <returns></returns>
    public void AddAnger(AngerStyle Style, Life target)
    {
        //大招不给自己加怒气
        if (Style == AngerStyle.SkillLaunch) {
            Anger  = ScriptM.Formula<int>("CACL_SOLDIER_ANGER", AngerAddWay.Attack, Anger, 0, 0);
        } else if (Style == AngerStyle.TargetDead) {
            if (BattleEnvironmentM.GetBattleEnvironmentMode() == BattleEnvironmentMode.CombatPVE && m_Core.m_IsPlayer) {
                if (target is Role) {
                    Anger  += target.m_Attr.GetintValueDefalt(NDAttrKeyName.DeadMP_Int);
                    m_Skin.ShowKillAnget(target.m_Attr.GetintValueDefalt(NDAttrKeyName.DeadMP_Int));
                } else if (target is Building) {
                    Anger  = ScriptM.Formula<int>("CACL_SOLDIER_ANGER", AngerAddWay.KillBuild, Anger, 0, 0);
                }
                
            } else {
                if (target is Role) {
                    Anger  = ScriptM.Formula<int>("CACL_SOLDIER_ANGER", AngerAddWay.KillSoldier, Anger, 0, 0);
                } else if (target is Building) {
                    Anger  = ScriptM.Formula<int>("CACL_SOLDIER_ANGER", AngerAddWay.KillBuild, Anger, 0, 0);
                }
            }
        }
    }
    
    public bool StartAttack(int ID)
    {
        return true;
    }
    public void FindTarget()
    {
        if (m_Attr.ModelType == 200003) {
        
            run.RegisterNextAction();
            if (m_CurrentAction != null) {
                m_CurrentAction.SetDone();
            }
            m_CurrentAction = new GridActionCmd200003FindTarget();
            m_CurrentAction.SetTarget(this);
        }
    }
    
    public void BreakAction(BreakActionCause bac)
    {
        if (bac == BreakActionCause.SkillRelease) {
            //if (m_CurrentAction is GridActionCmdAttack)
            {
                run.RegisterNextAction();
                m_CurrentAction.SetDone();
            }
        } else if (bac == BreakActionCause.Dead) {
        
            if (m_CurrentAction is GridActionCmdAttack || m_CurrentAction is GridActionCmdInterrupt) {
                //m_CurrentAction.SetDone();
            }
        } else if (bac == BreakActionCause.SkillInterrupt) {
            if (m_CurrentAction is GridActionSkill) {
                //if (!(m_CurrentAction as GridActionCmdAttack).IsPlayed())
                //{
                run.RegisterNextAction();
                m_CurrentAction.SetDone();
                m_Hit = true;
                //}
            }
        } else if (bac == BreakActionCause.InterruptAction) {
            if (m_CurrentAction is GridActionCmdInterrupt) {
                run.RegisterNextAction();
                m_CurrentAction.SetDone();
            }
        } else if (bac == BreakActionCause.Vertigo) {
        
            if (m_CurrentAction is GridActionCmdCDStand) { // || m_CurrentAction is GridActionCmdSkillStand)
                run.RegisterNextAction();
                m_CurrentAction.SetDone();
            }
        } else if (bac == BreakActionCause.Hit) {
        
            if (m_CurrentAction is GridActionCmdCDStand || m_CurrentAction is GridActionCmdStand) {
                //熊的技能不要受击
                //if (!m_Status.CheckStateBySkill(1022))
                //{
                GameObjectActionExcute gae = m_Skin.ProPerty.gameObject.GetComponent<GameObjectActionExcute>();
                if (gae != null) {
                    Object.Destroy(gae);
                }
                
                run.RegisterNextAction();
                m_CurrentAction.SetDone();
                
                m_Hit = true;
                //}
            } else if (m_CurrentAction is GridActionCmdAttack) {
                if (PropSkillInfo.m_actiontype != 1 && (m_CurrentAction as GridActionCmdAttack).IsPlayed()) {
                    GameObjectActionExcute gae = m_Skin.ProPerty.gameObject.GetComponent<GameObjectActionExcute>();
                    if (gae != null) {
                        Object.Destroy(gae);
                    }
                    run.RegisterNextAction();
                    m_CurrentAction.SetDone();
                    m_Hit = true;
                }
            }
            
        }
    }
    
    public bool JumpAttack(Life target, MapGrid grid)
    {
        if (target == null || target.isDead) {
            return true;
        }
        bool interrput = false;
        SkillReleaseInfo info = Life.CalcDamage(this, target, null, m_Skill.PropSkillInfo);
        bool result = target.ApplyDamage(info, m_thisT);
        if (result) {
            AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_SELFSP), this);
            MapPos = grid.GetStationsPos();
        }
        
        return result;
    }
    
    
    
    
    
    //绘制路径线
    void OnDrawGizmos()
    {
        if (run != null) {
            run.OnDrawGizmos();
        }
    }
    //作为寻路目标时，对应的地图格子
    public override MapGrid GetTargetMapGrid()
    {
    
        if (m_CurrentAction != null && (m_CurrentAction is GridActionCmdJump || m_CurrentAction is GridActionCmdStair)) {
            //if(run != null) return run.GetAttackStationMapGrid();
            //else
            return GetMapGrid();
        }
        if (run != null) {
            return run.GetTargetMapGrid();
        } else {
            return null;
        }
    }
    
    //所在的地图格子
    public override MapGrid GetMapGrid()
    {
        return MapGrid.GetMG(MapPos);
    }
    
    public override Int2 GetMapPos()
    {
        return MapPos;
    }
    //检测是否能释放大招
    public bool CheckReleaseSkill(ref bool bnottarget)
    {
        if (RoleSkill == null) {
            return false;
        }
        SoldierSkill skill = RoleSkill.GetBigSkill();
        if (skill == null) {
            return false;
        }
        bnottarget = false;
        if (CurrentAction is GridActionCmdSkillStand || CurrentAction is GridActionCmdHitParalysis || CurrentAction is GridActionCmdHitByBuild
            || CurrentAction is GridActionCmdHitFly || CurrentAction is GridActionCmdFall || CurrentAction is GridActionDophineFly //|| CurrentAction is GridActionCmdHitByBuild1503
            || !m_Attr.CanMove) {
            return false;
        }
        if (!(skill.m_distance == 0 || skill.m_target != 0 || RoleSkill.GetBigAttackTarget())) {
            bnottarget = true;
            return false;
        }
        return true;
        /*
        if ((m_CurrentAction is GridActionCmdWalk || m_CurrentAction is GridActionCmdStand || m_CurrentAction is GridActionCmdCDStand || m_CurrentAction is GridActionCmdUnderWalk
             || m_CurrentAction is GridActionSkill || m_CurrentAction is GridActionCmdHit ) && m_Attr.SkillReleaseType != ReleaseType.NoAttack && m_Attr.CanMove)
        {
        	return true;
        }
        return false;*/
    }
    //释放技能
    public bool  ReleaseSkillEffect()
    {
        if (((m_CurrentAction is GridActionCmdWalk || m_CurrentAction is GridActionCmdStand || m_CurrentAction is GridActionCmdCDStand || m_CurrentAction is GridActionCmdUnderWalk
                    || m_CurrentAction is GridActionSkill || m_CurrentAction is GridActionCmdHit) && m_Attr.SkillReleaseType != ReleaseType.NoAttack && m_Attr.CanMove)
            && RoleSkill.GetBigAttackTarget()) {
            m_NoHurtCount = 0;
            CombatWnd wnd = WndManager.FindDialog<CombatWnd>();
            if (wnd != null) {
                wnd.ReleaseSkillUI(m_SceneID);
            }
            //Anger = 0;
            m_SkillRelease = true;
            BreakAction(BreakActionCause.SkillRelease);
            m_Hit = false;
            //m_AttackInterrupt = false;
            if (m_Pause) {
                Contiune();
            }
            return true;
        }
        Debug.Log("无法触发主动技能，原因:" + m_CurrentAction);
        return false;
    }
    
    
    public void HitFly(WalkDir Dir, float duration, float delay, bool bearhit = false)
    {
    
        MapM.EmptyRoleStations(m_SceneID, LifeMType.SOLDIER);
        m_Skin.PlayAnimation(AnimatorState.Fly00000);
        m_Skin.tRoot.parent = BattleEnvironmentM.GetLifeMBornNode(false);
        SetMoveState(MoveState.Fly);
        if (MoveAI != null) {
            (MoveAI as Fly).SetHitFly(Dir, duration, delay, bearhit);
        }
        if (CurPet != null && CurPet.PetMoveAI is PetWalk1002) {
            (CurPet.PetMoveAI as PetWalk1002).m_petState = Pet1002State.Follow;
        }
    }
    
    public void KickedBack(WalkDir Dir, float duration, float delay, int range)
    {
        if (null != m_CurrentAction) {
            m_CurrentAction.SetDone();
            RoleWalk.m_ActionList.Add(m_CurrentAction);
        }
        Int2 nextGrid = new Int2();
        
        nextGrid.Layer = MapPos.Layer;
        
        if (Dir == WalkDir.WALKLEFT) {
        
            nextGrid.Unit = MapPos.Unit + range;
            int Max = MapSize.GetLayerMaxGrid(MapPos.Layer);
            nextGrid.Unit = nextGrid.Unit > Max ? Max : nextGrid.Unit;
        } else {
            nextGrid.Unit = MapPos.Unit - range;
            int min = MapSize.GetGridStart(MapPos.Layer);
            nextGrid.Unit = nextGrid.Unit < min ? min : nextGrid.Unit;
        }
        RoleWalk.Teleport(MapGrid.GetMG(nextGrid), false);
        m_CurrentAction = new GridActionCmdHitKicked(MapGrid.GetMG(nextGrid).pos, 0.5f);
        m_CurrentAction.SetTarget(this);
        //		m_CurrentAction = new GridActionCmdSpecialJump(0.5f,MapGrid.GetMG( MapPos).pos,MapGrid.GetMG(nextGrid).pos, Dir, RankDeep);
        //		m_CurrentAction.SetTarget(this);
        //		MapPos = nextGrid;
        
        MapM.EmptyRoleStations(m_SceneID, LifeMType.SOLDIER);
        
    }
    
    public void SendToGrid(Vector3 start, Vector3 startTo, Vector3 endFr, Vector3 end, float duration, MapGrid m)
    {
        if (null != m_CurrentAction) {
            m_CurrentAction.SetDone();
            RoleWalk.m_ActionList.Add(m_CurrentAction);
        }
        m_CurrentAction = new GridActionCmdSendToPos(this, m_thisT.localPosition, startTo, endFr, end, duration, WalkDir, 1, m);
        MapM.EmptyRoleStations(m_SceneID, LifeMType.SOLDIER);
        
    }
    
    
    public void SetStatus(int SceneID, object Param)
    {
        if (SceneID != this.SceneID) {
            return ;
        }
        
        if (Param is StatusInfo) {
            StatusInfo Info = Param as StatusInfo;
            if (Info.State == StatusState.Add) {
                SetSkillStatus(Info);
            } else if (Info.State == StatusState.Remove) {
                RemoveStatus(Info);
            }
            
        }
    }
    
    
    public  void RoleWake(int SceneID, object Param)
    {
        if (SceneID != this.SceneID) {
            return ;
        }
        if (m_Attr != null) {
            m_Attr.Attacked = true;
        }
    }
    
    
    
    public void HitSanNa(Vector3 end, float duration, List<MapGrid> l)
    {
        m_CurrentAction.SetDone();
        RoleWalk.m_ActionList.Add(m_CurrentAction);
        m_CurrentAction = new GridActionCmdHitByBuild1504(m_thisT.localPosition, end, duration, l);
        m_CurrentAction.SetTarget(this);
        
#if UNITY_EDITOR_LOG
        FileLog.write(SceneID, m_CurrentAction  + "," + Time.realtimeSinceStartup);
#endif
    }
    public void HitKTV(float duration, List<MapGrid> l)
    {
        m_CurrentAction.SetDone();
        RoleWalk.m_ActionList.Add(m_CurrentAction);
        m_CurrentAction = new GridActionCmdHitByBuild1502(duration, l);
        m_CurrentAction.SetTarget(this);
#if UNITY_EDITOR_LOG
        FileLog.write(SceneID, m_CurrentAction  + "," + Time.realtimeSinceStartup);
#endif
    }
    public void HitFirePower(float duration, WalkDir dir, MapGrid centrum)
    {
        if (!(CurrentAction is GridActionCmdFall || CurrentAction is GridActionCmdStair || CurrentAction is GridActionCmdJump || CurrentAction is GridActionCmdRepulse)) {
            if (m_CurrentAction != null) {
                m_CurrentAction.SetDone();
                RoleWalk.m_ActionList.Add(m_CurrentAction);
            } else {
                Debug.Log("m_CurrentAction  null  " + m_thisT.gameObject + "," + SceneID);
            }
            if (dir == WalkDir.WALKLEFT) {
                MapGrid toleft = centrum;
                for (int i = 0; i < 4; i++) {
                    /*if (toleft.Left != null && toleft.Left.Type == GridType.GRID_NORMAL)
                    {
                    	toleft = toleft.Left;
                    }
                    else break;*/
                    
                    MapGrid g = MapGrid.GetMG(centrum.GridPos.Layer, centrum.GridPos.Unit - i);
                    if (CreateHitFirePowerAction(toleft.Left, dir, centrum)) {
                        m_CurrentAction.SetTarget(this);
#if UNITY_EDITOR_LOG
                        FileLog.write(SceneID, m_CurrentAction  + "," + Time.realtimeSinceStartup);
#endif
                        return ;
                    }
                    toleft = g;
                }
                
                if (toleft == null) {
                    Debug.Log("HitFirePower :" + centrum.GridPos + "," + dir);
                }
                m_CurrentAction = new GridActionCmdRepulseOneGrid(toleft, dir);
            } else {
                MapGrid toright = centrum;
                for (int i = 0; i < 4; i++) {
                    /*if (toright.Right != null && toright.Right.Type == GridType.GRID_NORMAL)
                    {
                    	toright = toright.Right;
                    }
                    else break;*/
                    MapGrid g = MapGrid.GetMG(centrum.GridPos.Layer, centrum.GridPos.Unit + i);
                    if (CreateHitFirePowerAction(g, dir, centrum)) {
                        m_CurrentAction.SetTarget(this);
#if UNITY_EDITOR_LOG
                        FileLog.write(SceneID, m_CurrentAction  + "," + Time.realtimeSinceStartup);
#endif
                        return ;
                    }
                    toright = g;
                }
                if (toright == null) {
                    Debug.Log("HitFirePower :" + centrum.GridPos + "," + dir);
                }
                m_CurrentAction = new GridActionCmdRepulseOneGrid(toright, dir);
            }
            m_CurrentAction.SetTarget(this);
#if UNITY_EDITOR_LOG
            FileLog.write(SceneID, m_CurrentAction  + "," + Time.realtimeSinceStartup);
#endif
        }
    }
    public bool CreateHitFirePowerAction(MapGrid g, WalkDir dir, MapGrid centrum)
    {
    
        if (g == null) {
            Vector3 pos = Vector3.zero;
            MapM.EmptyRoleStations(m_SceneID, LifeMType.SOLDIER);
            m_Skin.tRoot.parent = BattleEnvironmentM.GetLifeMBornNode(false);
            if (dir == WalkDir.WALKLEFT) {
                pos = m_thisT.localPosition - new Vector3(1.5f, 0, 0);
            } else {
                pos = m_thisT.localPosition + new Vector3(1.5f, 0, 0);
            }
            m_CurrentAction = new GridActionCmdRepulseOutBoat(pos, dir);
            SetMoveState(MoveState.Fly);
            RoleFly.m_FlyInfo.flyCollisionAction = FlyCollisionAction.DropOutBoat;
            return true;
        } else if (g.Type == GridType.GRID_NORMAL) {
            return false;
        } else if (g.Type == GridType.GRID_WALL) {
            m_CurrentAction = new GridActionCmdRepulseJump(g, dir);
            return true;
        } else if (g.Type == GridType.GRID_HOLE || g.Type == GridType.GRID_HOLESTAIR) {
        
            MapGrid down = g.Down;
            if (dir == WalkDir.WALKLEFT) {
                down = MapGrid.GetMG(centrum.GridPos.Layer, centrum.GridPos.Unit - 3);
                down = down.Down;
            } else {
                down = MapGrid.GetMG(centrum.GridPos.Layer, centrum.GridPos.Unit + 3);
                down = down.Down;
            }
            while (down != null && down.Type != GridType.GRID_NORMAL && down.Down != null) {
                down = down.Down;
            }
            if (down == null) {
                down = g.Down;
            }
            if (down.Type == GridType.GRID_HOLE && down.GridPos.Layer == 0) {
            
                Vector3 pos = Vector3.zero;
                MapM.EmptyRoleStations(m_SceneID, LifeMType.SOLDIER);
                m_Skin.tRoot.parent = BattleEnvironmentM.GetLifeMBornNode(false);
                if (dir == WalkDir.WALKLEFT) {
                    pos = m_thisT.localPosition - new Vector3(1.5f, 0, 0);
                } else {
                    pos = m_thisT.localPosition + new Vector3(1.5f, 0, 0);
                }
                m_CurrentAction = new GridActionCmdRepulseOutBoat(pos, dir);
                SetMoveState(MoveState.Fly);
                RoleFly.m_FlyInfo.flyCollisionAction = FlyCollisionAction.DropOutBoat;
                return true;
            } else {
                m_CurrentAction = new GridActionCmdRepulseDownLayer(g, down, dir);
            }
            return true;
        }
        return false;
    }
    public void CheckConditionSkillEvent(StatusState s = StatusState.Add)
    {
        SoldierSkill conditionskillinfo = new SoldierSkill();
        if (RoleSkill.CheckConditionSkillAction(m_SceneID, ref conditionskillinfo, false, m_SkillRelease, s)) {
            if (PropSkillInfo == null || conditionskillinfo.m_ipriority > PropSkillInfo.m_ipriority) {
                if (CurrentAction != null) {
                    CurrentAction.SetDone();
                }
                RoleWalk.m_ActionList.Add(CurrentAction);
                CurrentAction = RoleSkill.GetConditionSkillAction(m_SceneID, conditionskillinfo, m_thisT.localPosition, m_thisT.localPosition, 0f, false);
            }
            
        }
    }
    private void SetSkillStatus(StatusInfo Info)
    {
        if (Info == null || m_CurrentAction == null) {
            return ;
        }
#if UNITY_EDITOR_LOG
        //if (Info.Type == StatusType.StaticElec)
        //	Debug.Log(Info.Type + "," + Info.effectid + ","  + Info.time);
#endif
        m_Skin.AddStatusEffect(Info, WalkDir);
        
        
        if (Info.Type == StatusType.Vertigo && Info.isAction) {
            //Debug.Log("add Vertigo ..........................");
            if (m_CurrentAction is GridActionCmdSkillStand) {
                if (m_CurrentAction.m_Duration < Info.time) {
                    m_CurrentAction.m_Duration = Info.time;
                }
            } else {
                m_CurrentAction.SetDone();
                RoleWalk.m_ActionList.Add(m_CurrentAction);
                m_CurrentAction = new GridActionCmdSkillStand(Info.time, m_thisT.localPosition, RankDeep);
                m_CurrentAction.SetTarget(this);
            }
        } else if (Info.Type == StatusType.Invisible) {
            SetState(true);
        } else if (Info.Type == StatusType.Taunt) {
            m_Attr.Attacked = true;
#if UNITY_EDITOR_LOG
            //string text = name  + "被触发嘲讽状态，跑去攻击熊孩子";
            //NGUIUtil.DebugLog(text ,"red");
#endif
        } else if (Info.Type == StatusType.ClickFly) {
#if UNITY_EDITOR_LOG
            //string text = name  + "被拳击手套击飞了";
            //NGUIUtil.DebugLog(text ,"red");
#endif
        } else if (Info.Type == StatusType.StaticElec) {
#if UNITY_EDITOR_LOG
            //Debug.Log("add StaticElec ..........................");
#endif
            
        } else if (Info.Type == StatusType.Squash && Info.isAction) {
            //Debug.Log("add Squash .........................." + Info.time);
            m_CurrentAction.SetDone();
            RoleWalk.m_ActionList.Add(m_CurrentAction);
            m_CurrentAction = new GridActionCmdHitByBuild1505(Info.time);
            m_CurrentAction.SetTarget(this);
        } else if (Info.Type == StatusType.NoSquash) {
            //Debug.Log("add NoSquash .........................." + Info.time);
            CheckConditionSkillEvent(StatusState.Add);
            /*m_CurrentAction.SetDone();
            RoleWalk.m_ActionList.Add(m_CurrentAction);
            m_CurrentAction = new GridActionCmdNoHitByBuild1505(Info.time);
            m_CurrentAction.SetTarget(this);*/
        } else if (Info.Type == StatusType.Sleep && Info.isAction) {
            m_CurrentAction.SetDone();
            RoleWalk.m_ActionList.Add(m_CurrentAction);
            m_CurrentAction = new GridActionCmdSleep(Info.time);
            m_CurrentAction.SetTarget(this);
        } else if (Info.Type == StatusType.FireShield && Info.isAction) {
            /*m_CurrentAction.SetDone();
            RoleWalk.m_ActionList.Add(m_CurrentAction);
            m_CurrentAction = new GridActionCmdFireShield(Info.time);
            m_CurrentAction.SetTarget(this);*/
        } else if (Info.Type == StatusType.TearOfMermaid) {
        
        } else if (Info.Type == StatusType.IceBuild && Info.isAction) {
            //Debug.Log("add IceBuild .........................." + Info.time);
            m_CurrentAction.SetDone();
            RoleWalk.m_ActionList.Add(m_CurrentAction);
            m_CurrentAction = new GridActionCmdHitByBuild1501(Info.time);
            m_CurrentAction.SetTarget(this);
            
        } else if (Info.Type == StatusType.paralysis && Info.isAction) {
            //Debug.Log("add paralysis .........................." + Info.time);
            m_CurrentAction.SetDone();
            RoleWalk.m_ActionList.Add(m_CurrentAction);
            m_CurrentAction = new GridActionCmdHitParalysis(Info.time);
            m_CurrentAction.SetTarget(this);
        } else if (Info.Type == StatusType.Cleanse) {
            m_Status.ClearDebuffStatus();
        } else if (Info.Type == StatusType.Charm) {
            m_Attr.Charmed = true;
        } else if (Info.Type == StatusType.KickedBack) {
            int range = 2;
            if (Info.effect.Count > 0) {
                range = Info.effect[0] / MapGrid.m_Pixel;
            }
            
            KickedBack(WalkDir, 0.5f, 0f, range);
            
        } else if (Info.Type == StatusType.RunAway) {
            //Debug.Log("add RunAway .........................." + Info.time + "," + Time.time);
            MapGrid left = GetMapGrid().Left;
            MapGrid right = GetMapGrid().Right;
            while (left != null || right != null) {
                if (left != null) {
                    if ((left.Type == GridType.GRID_STAIR || left.Type == GridType.GRID_HOLESTAIR)
                        && left.Down != null && (left.Down.Type == GridType.GRID_STAIR || left.Down.Type == GridType.GRID_HOLESTAIR)) {
                        RunPos = left.Down.Left;
                        AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_SELFSP), this);
                        break;
                    } else {
                        left = left.Left;
                    }
                }
                
                if (right != null) {
                    if ((right.Type == GridType.GRID_STAIR || right.Type == GridType.GRID_HOLESTAIR)
                        && right.Down != null && (right.Down.Type == GridType.GRID_STAIR || right.Down.Type == GridType.GRID_HOLESTAIR)) {
                        RunPos = right.Down.Left;
                        AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_SELFSP), this);
                        break;
                    } else {
                        right = right.Right;
                    }
                }
            }
            //RunPos = GetMapGrid().r
        } else if (Info.Type == StatusType.FakeTaunt) {
            //Debug.Log("add FakeTaunt .........................." + Info.time + "," + Time.time);
            m_Attr.Attacked = true;
            AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_SELFSP), this);
        } else if (Info.Type == StatusType.Mark) {
            //Debug.Log("add Mark .........................." + Info.time + "," + Time.time);
        } else if (Info.Type == StatusType.Die) {
            /*SkillReleaseInfo info = new SkillReleaseInfo();
            info.Result = AttackResult.Normal;
            info.m_Damage = - fullHP *2;
            ApplyDamage(info,null);*/
        } else if (Info.Type == StatusType.Turn) {
        
            //Debug.Log("add Turn .........................." + Info.time + "," + Time.time);
        } else if (Info.Type == StatusType.WetBody) {
            m_Attr.IsWetBody = true;
            //Debug.Log("add WetBody .........................." + Info.time + "," + Time.time);
        } else if (Info.Type == StatusType.Berserker) {
#if UNITY_EDITOR_LOG
            Debug.Log("add Berserker .........................." + Info.time + "," + Time.time);
#endif
            TurnsInto(true);
        }
#if UNITY_EDITOR_LOG
        FileLog.write(SceneID, m_CurrentAction + "," + Info.Type + "," + Time.realtimeSinceStartup);
#endif
    }
    
    
    private void RemoveStatus(StatusInfo Info)
    {
        if (Info == null) {
            return ;
        }
        m_Skin.RemoveStatusEffect(Info);
        
        if (Info.Type == StatusType.Invisible) {
            SetState(false);
        } else if (Info.Type == StatusType.Vertigo) {
            //Debug.Log("remove Vertigo ..........................");
        } else if (Info.Type == StatusType.StaticElec) {
            //Debug.Log("remove StaticElec ..........................");
        } else if (Info.Type == StatusType.Charm) {
            m_Attr.Charmed = false;
        } else if (Info.Type == StatusType.RunAway) {
        
            //Debug.Log("remove RunAway .........................." + Info.time + "," + Time.time);
            m_Attr.Attacked = false;
            CheckConditionSkillEvent(StatusState.Remove);
        } else if (Info.Type == StatusType.Die) {
        
            //Debug.Log("remove Die .........................." + Info.time + "," + Time.time);
        } else if (Info.Type == StatusType.Turn) {
        
            Vector3 pos = m_Skin.tRoot.position;
            pos.z -= 2f;
            string name = "1103111_01";
            if (WalkDir == WalkDir.WALKRIGHT) {
                name = "1103111_02";
            }
            GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, name, pos, BattleEnvironmentM.GetLifeMBornNode(true));
            GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1.0f);
            gae.AddAction(ndEffect);
            TurnsInto(false);
            //Debug.Log("remove Turn .........................." + Info.time + "," + Time.time);
        } else if (Info.Type == StatusType.WetBody) {
            m_Attr.IsWetBody = false;
            //Debug.Log("remove WetBody .........................." + Info.time + "," + Time.time);
        } else if (Info.Type == StatusType.Berserker) {
            //Debug.Log("remove Berserker .........................." + Info.time + "," + Time.time);
            m_bTrunBack = true;
            //TurnsInto(false);
        } else if (Info.Type == StatusType.TearOfMermaid) {
        
        } else if (Info.Type == StatusType.KickedBack) {
            Info.Type = StatusType.None;
            SoldierSkill sourceSkill = new SoldierSkill();
            SkillM.GetSkillInfo(Info.exSkill1, Info.exSkillLevel1, ref sourceSkill);
            
            Role attacker = CM.GetLifeM(Info.Releasescentid, LifeMType.SOLDIER) as Role;
            //			sourceSkill.SetTarget(this,this.GetMapGrid());
            
            RoleSkill.GlobalUseSkill(attacker, this, sourceSkill, Vector3.zero);
        } else if (Info.Type == StatusType.WhipCorpse) {
            //Debug.Log("remove WhipCorpse .........................." + SceneID + "," + Time.time + "," + HP);
            if (HP <= 0) {
                BSCEventDeadHook(null);
            }
        }
    }
    
    
    
    public override void Dead()
    {
        if (NeedDelayDie()) {
            return;
        }
        if (ServerMode) {
            if (!WaitServer) {
                WaitServer = true;
                tga.SoldierDeadRequest li = new tga.SoldierDeadRequest();
                li.time = new tga.tgaTime();
                li.time.Requesttime = Time.realtimeSinceStartup;
                BSC.SoldierDeadRequest(m_Core.m_DataID, li);
            }
        } else {
            BSCEventDeadHook(null);
        }
        
        
    }
    public void BSCEventDeadHook(object Info)
    {
        if (m_Pause) {
            Contiune();
        }
        WaitServer = false;
        if (m_CurrentAction != null) {
            m_CurrentAction.SetDone();
        }
        
        base.Dead();
        if (CurPet != null) {
            CurPet.Dead();
        }
        
        List<Life> ll = new List<Life>();
        CM.SearchLifeMListInBoat(ref ll, LifeMType.SOLDIER, LifeMCamp.ALL);
        foreach (Role r in ll) {
            r.SomeBodyDie(this);
        }
        GodSkillWnd gsw = WndManager.FindDialog<GodSkillWnd>();
        if (gsw != null) {
            if (m_Core.m_IsPlayer) {
                gsw.ChangeBiaoqing((int)CaptionExpress.playerdead);
            } else {
                gsw.ChangeBiaoqing((int)CaptionExpress.enemydead);
            }
        }
        List<int> l = CmCarbon.GetRewardItem(m_Core.m_Camp, m_Core.m_DataID);
        if (l != null && l.Count > 0) {
            int i = 0;
            Vector3 pos = m_thisT.localPosition;
            pos += new Vector3(0, 0.45f, 1.2f);
            foreach (int itemid in l) {
                //pos.y += 0.5f * i;
                pos.x += 0.25f * i;
                sdata.s_itemtypeInfo item = ItemM.GetItemInfo(itemid);
                DropResource.Drop(item, pos);
                i ++;
            }
            CmCarbon.SetAddWinItem(l.Count);
        }
        m_Skin.RemoveAllStatusEffect();
        m_Status.ClearAllStatus();
        
        EventCenter.AntiRegisterHooks(NDEventType.Attr_Paralysis, HitDianJiParalysis);
        EventCenter.AntiRegisterHooks(NDEventType.StatusCG, SetStatus);
        
        
        
        
        BSC.AntiRegisterHooks(new BscCmd(m_Core.m_DataID, BSCEventType.BSC_Born), BSCEventBornHook);
        BSC.AntiRegisterHooks(new BscCmd(m_Core.m_DataID, BSCEventType.BSC_Dead), BSCEventDeadHook);
        BSC.AntiRegisterHooks(new BscCmd(m_Core.m_DataID, BSCEventType.BSC_RunRoad), BSCEventRunHook);
        
        EventCenter.AntiRegisterHooks(NDEventType.Attr_Wake, RoleWake);
    }
    
    bool NeedDelayDie()
    {
        if (m_Status.HaveState(StatusType.WhipCorpse)) {
            return true;
        }
        return false;
    }
    
    public Life GetAttackTarget(Life attacktarget)
    {
        int newWeight = GetAttackWeight(attacktarget);
        int oldWeight = GetAttackWeight(Target);
        //遇墙规则，需要先打完墙，才可以打躲在墙体后面的目标。
        if (RoleSkill.m_AttackTarget != null && !RoleSkill.m_AttackTarget.isDead && RoleSkill.m_AttackTarget is IggWall) {
            //判断攻击你的人是否在墙体后面
            int d1 = attacktarget.GetMapPos().Unit  - GetMapPos().Unit;
            int d2 = RoleSkill.m_AttackTarget.GetMapPos().Unit - GetMapPos().Unit;
            int dir = d1 * d2 ;
            if (dir > 0) { //在同一侧
                if (Mathf.Abs(d1)  > Mathf.Abs(d2)) { // 在墙体后面。
                    return RoleSkill.m_AttackTarget;
                }
            } else { // 不在同一侧
                List<IggWall> lw = new List<IggWall>();
                MapGrid.GetWallList(ref lw, GetMapPos(), attacktarget.GetMapPos());
                if (lw.Count > 0) { //跟攻击者之间有墙，保持目标不改变
                    return RoleSkill.m_AttackTarget;
                }
            }
        }
        if (newWeight > oldWeight) {
            return attacktarget;
        } else if (newWeight == oldWeight) {
            float olddis = CalcDistance(Target, this);
            float newdis = CalcDistance(attacktarget, this);
            return newdis < olddis ? attacktarget : Target;
        } else {
            return Target;
        }
        //return newWeight > oldWeight ? attacktarget : RoleSkill.m_AttackTarget;
    }
    //获取攻击目标权值
    public int GetAttackWeight(Life l)
    {
        //炮弹兵
        if (m_Attr.AttackLike == 1 && l is Role) {
            return (int) AttackWeight.like;
        }
        //建筑物
        if (m_Attr.AttackLike == 2 && l is Building) {
            Building build = l as Building;
            if (l.m_Attr.IsDamage && !l.m_Attr.IsResource) {
                return (int) AttackWeight.like;
            }
        }
        //资源建筑物
        if (m_Attr.AttackLike == 3 && l is Building) {
            if (l.m_Attr.IsDamage && l.m_Attr.IsResource) {
                return (int) AttackWeight.like;
            }
        }
        
        if (l is Role) {
            return (int) AttackWeight.Soldier;
        }
        
        if (l is Building) {
            Building build = l as Building;
            if (l.m_Attr.IsDamage && !l.m_Attr.IsResource) {
                return (int) AttackWeight.build;
            }
        }
        
        if (l is IggWall) {
            return (int) AttackWeight.wall;
        }
        
        if (l is Building) {
            if (l.m_Attr.IsDamage && l.m_Attr.IsResource) {
                return (int) AttackWeight.gold;
            }
        }
        
        return 0;
    }
    public void HitDianJi()
    {
    
#if UNITY_EDITOR_LOG
        FileLog.write(SceneID, m_CurrentAction  + ", dianji " + InBoat + "," + Time.realtimeSinceStartup);
#endif
        if (m_CurrentAction != null) {
            m_CurrentAction.SetDone();
            RoleWalk.m_ActionList.Add(m_CurrentAction);
        }
        m_CurrentAction = new GridActionCmdHitByBuild1503();
        m_CurrentAction.SetTarget(this);
        /*	#if UNITY_EDITOR_LOG
        	FileLog.write(SceneID,m_CurrentAction  + "," + Time.realtimeSinceStartup);
        	#endif*/
    }
    public void HitDianJiParalysis(int SceneID, object Param)
    {
    
#if UNITY_EDITOR_LOG
        //Debug.Log("HitDianJiParalysis" + Time.time);
        FileLog.write(SceneID, m_CurrentAction  + ", dianji " + InBoat + "," + Time.realtimeSinceStartup);
#endif
        /*if (MoveState == MoveState.Fly)
        	return ;*/
        if (m_CurrentAction != null) {
            m_CurrentAction.SetDone();
            RoleWalk.m_ActionList.Add(m_CurrentAction);
        }
        /*m_CurrentAction = new GridActionCmdHitParalysis(0.5f);
        m_CurrentAction.SetTarget(this);*/
    }
    public override bool ApplyDamage(SkillReleaseInfo Info, Transform attackgo)
    {
        if (Info.m_Damage < 0) {
            m_NoHurtCount = 0;
        }
        if (HP + Info.m_Damage <= 0) {
            CheckConditionSkillEvent(StatusState.SoonDie);
        }
        if (NoDie) {
            return true;
        }
        //Debug.Log(string.Format("Damage={0},attckGo={1}",damage,attackgo.name));
        bool result = true;
        //Debug.Log(gameObject  + "," + m_SceneID + "," + Info.Result + "," + Info.m_Damage);
        if ((Info.Result == AttackResult.Normal && Info.m_Damage != 0) || Info.Result == AttackResult.Miss || Info.Result == AttackResult.Crit) {
            m_Attr.Attacked = true;
            result = base.ApplyDamage(Info, attackgo);
        }
        if (!result) {
            BreakAction(BreakActionCause.Dead);
#if UNITY_EDITOR_LOG
            FileLog.write(SceneID, "Dead");
#endif
            if (run != null) {
                run.Remove();
            }
        } else {
            if (attackgo != null) {
                Life target = attackgo.GetComponent<LifeObj>().GetLife();
                ChangeTarget(ChangeTargetReason.HitChangetTarget, target);
            }
            
            /*#if UNITY_EDITOR_LOG
            else
            	Debug.LogError("攻击方没有传进来");
            #endif*/
            EventCenter.DoEvent(NDEventType.StatusInterrupt, m_SceneID, LifeAction.Hit);
            if ((Info.Result == AttackResult.Normal || Info.Result == AttackResult.Crit) && Info.m_Damage != 0) {
            
                m_Attr.Attacked = true;
                GameObject go = m_Skin.ProPerty.gameObject;
                GameObjectActionExcute i = go.GetComponent<GameObjectActionExcute>();
                //float f = 0.2f;
                if (i == null) {
                
                    i = go.AddComponent<GameObjectActionExcute>();
                    GameObjectActionShakePostion sp = new GameObjectActionShakePostion();
                    i.AddAction(sp);
                } else {
                    GameObjectAction ga = i.GetCurrentAction();
                    if (ga != null &&  ga is GameObjectActionShakePostion) {
                        /*i.FinishCurAction();
                        GameObjectActionShakePostion sp = new GameObjectActionShakePostion();
                        i.AddAction(sp);*/
                    } else {
                        GameObjectActionShakePostion sp = new GameObjectActionShakePostion();
                        i.AddAction(sp);
                    }
                }
                GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectPos);
                if (posgo != null) {
                    Transform t = posgo.transform;
                    if (Info.m_struckeffect != null && Info.m_struckeffect != "" && Info.m_struckeffect != "null") {
                        if (Info.m_struckeffect.Contains("1905031-")) {
                            //雷神喵不打断
                            if (m_Attr.AttrType == 101004 && Info.mOwnerLevel <= m_Attr.Level) {
                                posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
                                GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1055091", posgo.transform.position, BattleEnvironmentM.GetLifeMBornNode(true));
                                GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1.5f);
                                gae.AddAction(ndEffect);
                            } else {
                                // SkillEffects._instance.LoadEffect("effect/prefab/", Info.m_struckeffect + m_Attr.AttrType.ToString().Substring(0, 3), t.position, 1.5f, true);
                                GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, Info.m_struckeffect + m_Attr.AttrType.ToString().Substring(0, 3), new Vector3(t.position.x, t.position.y, -1), BattleEnvironmentM.GetLifeMBornNode(true));
                                GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1.5f);
                                gae.AddAction(ndEffect);
                                
                                HitDianJi();
                            }
                        } else {
                            //SkillEffects._instance.LoadEffect("effect/prefab/", Info.m_struckeffect, t.position, 0.5f, true);
                            
                            GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, Info.m_struckeffect, new Vector3(t.position.x, t.position.y, -1), m_thisT.parent);
                            if (gae == null && WalkDir == WalkDir.WALKLEFT) {
                                gae = EffectM.LoadEffect(EffectM.sPath, Info.m_struckeffect + "_01", new Vector3(t.position.x, t.position.y, -1), m_thisT.parent);
                            } else if (gae == null && WalkDir == WalkDir.WALKRIGHT) {
                                gae = EffectM.LoadEffect(EffectM.sPath, Info.m_struckeffect + "_02", new Vector3(t.position.x, t.position.y, -1), m_thisT.parent);
                            }
                            if (Info.m_struckeffect == "1051181" && WalkDir == WalkDir.WALKLEFT) {
                                GameObjectActionEffectInit effectinit = new GameObjectActionEffectInit();
                                effectinit.SetRotation(new Vector3(0, 180, 0));
                                gae.AddAction(effectinit);
                            }
                            if (Info.m_struckeffect == "1002131" && WalkDir == WalkDir.WALKRIGHT) {
                                GameObjectActionEffectInit effectinit = new GameObjectActionEffectInit();
                                effectinit.SetRotation(new Vector3(0, 180, 0));
                                gae.AddAction(effectinit);
                            }
                            GameObjectActionDelayDestory ndEffect = null;
                            if (gae.GetComponent<ParticleSystem>() != null) {
                                ndEffect = new GameObjectActionDelayDestory(gae.GetComponent<ParticleSystem>().duration);
                            } else {
                                ndEffect = new GameObjectActionDelayDestory(0.5f);
                            }
                            
                            gae.AddAction(ndEffect);
                        }
                    }
                    //}
                    
                }
            } else {
                //Debug.Log("没加载受击光效" + Info.Result + ", " + Info.m_struckeffect + "," + Info.m_Damage + "," + attackgo);
            }
            
            if (Info.m_InterruptSkill) {
                BreakAction(BreakActionCause.SkillInterrupt);
                if ((((Info.Result == AttackResult.Normal || Info.Result == AttackResult.Crit) && Info.m_Damage != 0))
                    && !Info.m_bImmunity) {
                    //m_Hit = true;
                    BreakAction(BreakActionCause.Hit);
                }
                
            }
            
            int oldAnger = Anger;
            
            Anger = ScriptM.Formula<int>("CACL_SOLDIER_ANGER",
                    AngerAddWay.BeAttacked,
                    this.Anger,
                    Mathf.Abs(Info.m_Damage), fullHP);
                    
            float delta = Anger - oldAnger;
            if (RoleSkill != null) {
                RoleSkill.ModifyAttributeByCondition(Info, delta);
            }
        }
        return result;
    }
    
    
    
    public void SetState(bool IsHide)
    {
        if (IsHide == true) {
            m_Skin.StealthMode(true);
            m_Attr.RadarSearch = false;
        } else {
            m_Skin.StealthMode(false);
            m_Attr.RadarSearch = true;
        }
        LifeMCamp Camp = (m_Core.m_Camp == LifeMCamp.ATTACK) ? LifeMCamp.DEFENSE : LifeMCamp.ATTACK;
        List<Life> RoleList = new List<Life>();
        CM.SearchLifeMListInBoat(ref RoleList, LifeMType.SOLDIER | LifeMType.SUMMONPET, Camp);
        AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_CS), RoleList);
    }
    
    
    
    /*****************************************************************************************************************************************/
    /*公共部分*/
    /*****************************************************************************************************************************************/
    public override void NDStart()
    {
        if (Environment == LifeEnvironment.Combat) {
            CombatInit();
            if (HP == 0) {
                Dead();
            }
            /*if (m_Core.m_Camp == LifeMCamp.ATTACK)
            {
            	islocal = true;
            	isRequairAi = true;
            }
            else
            {
            	islocal = false;
            	isRequairAi = true;
            }*/
        }
    }
    public void BSCEventBornHook(object Info)
    {
        WaitServer = false;
    }
    public void BSCEventRunHook(object Info)
    {
        WaitServer = false;
        if (Info == null || !Life.ServerMode) {
            return;
        }
        tga.SoldierRunRoadResponse srrr = Info as tga.SoldierRunRoadResponse;
        
        RoleWalk.run.ServerPathInit(srrr);
    }
    public void CombatInit()
    {
        RoleSkin();
        InitAI();
#if UNITY_EDITOR_LOG
        FileLog.write(SceneID, "开始  " + m_thisT, false);
#endif
        EventCenter.RegisterHooks(NDEventType.StatusCG, SetStatus);
        if (m_Core.m_MoveState == MoveState.Walk) {
            DelHandPet();
            CreatePet();
            EventCenter.RegisterHooks(NDEventType.Attr_Wake, RoleWake);
            InitHP();
            TargetAttacked = false;
            StarPosProc();
            
        } else if (m_Core.m_MoveState == MoveState.Fly) {
            if (m_Attr.AttrType != 100003 && m_Attr.AttrType != 101002) {
                m_Skin.EnableColider(ColiderType.Click, false);
            }
        }
        
        /*if(m_Core.m_Camp == LifeMCamp.DEFENSE)
        {
        	m_Skin.EnableColider(ColiderType.Click ,true);
        }*/
    }
    
    /// <summary>
    /// FixedUpdate
    /// </summary>
    public override void NDFixedUpdate(float deltaTime)
    {
        if (CheckCombatIng() == true) {
            if (MoveAI != null) {
                MoveAI.FixedUpdate();
            }
        }
    }
    
    public override void NDUpdate(float deltaTime)
    {
        if (CheckCombatIng() == true) {
            m_Status.Update(deltaTime);
            m_Attr.Updata(deltaTime);
            if (m_Core.m_MoveState != MoveState.Walk) {
                return ;
            }
            
            if (m_TargetDuration > 0) {
                m_TargetDuration -= deltaTime;
            } else {
                ChangeTarget(ChangeTargetReason.TimeOut, null);
                AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_SELFSP), this);
            }
            m_NoHurtCount += deltaTime;
            if (CDTime < 0) {
                m_Hit = false;
            }
            s_bCalcNext = false;
            MoveAI.Update();
            m_Skill.Update(deltaTime);
        }
    }
    
    
    
    // walk
    public override void NDLateUpdate(float deltaTime)
    {
        //临时方案
        if (CombatScheduler.State == CSState.Combat) {
            m_Skin.UpdataSkinEffect();
            if (m_Core.m_MoveState != MoveState.Walk) {
                return ;
            }
            MoveAI.LateUpdate();
        }
        
    }
    //用来调整位置
    public void AdjustPosition(float delattime)
    {
        if (m_ajustcount < m_ajustduration) {
            m_ajustcount += delattime;
            m_thisT.localPosition = Vector3.Lerp(m_ajustfrompostion, m_ajusttopostion, m_ajustcount / m_ajustduration);
        }
    }
    public static void WakeEnemy(Life life)
    {
        List<Life> RoleList = new List<Life>();
        CM.SearchLifeMList(ref RoleList, null, LifeMType.SOLDIER, LifeMCamp.DEFENSE, MapSearchStlye.SameLayer, life, 0.0f);
        foreach (Life m in RoleList) {
            EventCenter.DoEvent(NDEventType.Attr_Wake, m.m_SceneID, null);
        }
    }
    
    
    /// <summary>
    /// 设置lifeM 核心结构
    /// </summary>
    public override int SetLifeCore(LifeMCore Core)
    {
        int ID = base.SetLifeCore(Core);
        //
        m_Status = new Status();
        m_Status.Init(SceneID);
        //
        if (this is SummonPet) {
            m_Attr = new SummonPetAttribute();
        } else {
            m_Attr = new RoleAttribute();
        }
        m_Attr.Init(SceneID, m_Core, this);
        InitSkill();
        InitMoveAI();
        InitAI();
        if (Core.m_Camp == LifeMCamp.ATTACK) {
            m_Attr.Attacked = true;
        }
        
        BSC.RegisterHooks(new BscCmd(m_Core.m_DataID, BSCEventType.BSC_Born), BSCEventBornHook);
        BSC.RegisterHooks(new BscCmd(m_Core.m_DataID, BSCEventType.BSC_Dead), BSCEventDeadHook);
        BSC.RegisterHooks(new BscCmd(m_Core.m_DataID, BSCEventType.BSC_RunRoad), BSCEventRunHook);
        return ID;
    }
    
    
    
    public virtual void InitSkill()
    {
        if (m_Core.m_MoveState == MoveState.Fly) {
            m_Skill = new FireSkill();
            m_Skill.Init(SceneID, m_Core);
        } else if (m_Core.m_MoveState == MoveState.Walk) {
            m_Skill = new RoleSkill();
            m_Skill.Init(SceneID, m_Core);
        }
    }
    
    
    public void InitMoveAI()
    {
        MoveAI = MoveAIFactory.Create(this, m_Core);
    }
    
    public void InitAI()
    {
        if (m_Core.m_MoveState == MoveState.Fly) {
            //RoleWalk.run = null;
            //RoleWalk.m_RadarAI = null;
        } else if (m_Core.m_MoveState == MoveState.Walk) {
            //if(m_Core.m_Camp == LifeMCamp.DEFENSE && m_Attr != null)
            //	m_Attr.Attacked = false;
            RoleWalk.m_RadarAI = RadarAIFactory.Create(m_Core, CheckInVision);
        }
    }
    
    
    
    public   void ColliderProc(Collision collision)
    {
        if (MoveAI != null) {
            MoveAI.ColliderProc(collision);
        }
    }
    
    
    /// <summary>
    /// 触发炮战
    /// </summary>
    /// <param name="collision">技能释放者</param>
    /// <param name="HitDir">受击者</param>
    /// <returns>技能伤害值</returns>
    public  FlyCollisionInfo FireTrigger(Collision collision, Life lifeM, Life Attacker, bool bReleaseSkill, bool bApplyDamage, int hitCount, FlyDir flydir)
    {
        if (m_Skill == null) {
            return null;
        }
        return (m_Skill as FireSkill).FireTrigger(collision, lifeM, Attacker, bReleaseSkill, bApplyDamage, hitCount, flydir);
    }
    
    
    
    public override void BeforeDead()
    {
        base.BeforeDead();
        CheckConditionSkillEvent();
        
        StatusM st = m_Status.FindStatusM(StatusType.TearOfMermaid);
        if (st != null) {
            EventCenter.DoEvent(NDEventType.StatusInterrupt, m_SceneID, LifeAction.Die);
            //StatusInfo tearStatus = st.CurrentStatus;
            
            //SoldierSkill sourceSkill = new SoldierSkill();
            //SkillM.GetSkillInfo(tearStatus.exSkill1, tearStatus.exSkillLevel1, ref sourceSkill);
            
            //Role attacker = CM.GetLifeM(tearStatus.Releasescentid, LifeMType.SOLDIER) as Role;
            //RoleSkill.GlobalUseSkill(attacker, this, sourceSkill, Vector3.zero);
        }
        
        if (this is Role) {
            if ((this as Role).run != null) {
                (this as Role).run.Remove();
            }
            if (m_Core.m_Camp == LifeMCamp.ATTACK) {
                CombatWnd Wnd = WndManager.FindDialog<CombatWnd>();
                if (Wnd != null) {
                    Wnd.SetDied(SceneID, m_Attr.FullHp);
                }
            }
        }
    }
    
    
    public static void RefreshSkin()
    {
        List<Life> listRAW = new List<Life>();
        CM.SearchLifeMListInBoat(ref listRAW, LifeMType.SOLDIER);
        foreach (Life item in listRAW) {
            Role raw = item as Role;
            raw.RoleSkin();
        }
    }
    /// <summary>
    /// 角色皮肤设置 （碰撞体、颜色、血条等）
    /// </summary>
    public  void RoleSkin()
    {
        if (m_isDead) {
            return ;
        }
        if (m_Skin != null) {
            m_Skin.ResetSkin();
        }
    }
    
    
    
    private void DelHandPet()
    {
        GameObject go = U3DUtil.FindChildDeep(m_Skin.tRoot.gameObject, "1002@skin");
        Vector3 pos = Vector3.zero;
        if (go) {
            pos = go.transform.position;
            pos.z = -0.5f;
            //go.SetActive(false);
            Object.Destroy(go, 0.5f);
        }
    }
    public void CreatePet()
    {
        if (m_Attr.AttrType == 100003 && CurPet == null) {
            //IGameRole i = GameRoleFactory.Create(BattleEnvironmentM.GetLifeMBornNode(true), 1002, "1002", AnimatorState.Empty);
            Pet pet = new Pet();
            pet.CreateSkin(BattleEnvironmentM.GetLifeMBornNode(true), 1002, "1002", AnimatorState.Empty, m_Core.m_IsPlayer);
            GameObject go = pet.PetSkinCom.tRoot.gameObject;
            Vector3 pos = Vector3.zero;
            string posname = "";
            Int2 toGrid = GetMapPos();
            
            LifeObj lobj = go.AddComponent<LifeObj>();
            
            CurPet = pet;
            GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.petFollowPos);
            pos = posgo.transform.position;
            
            go.transform.position = pos;
            pet.SetBorn(this, 1002001, 1002, toGrid, WalkDir);
            pet.SetSkin();
            lobj.SetLife(pet as Life, pet.PetSkinCom.ProPerty);
        } else if (CurPet != null) {
            CurPet.SetMoveState(MoveState.Walk);
        }
    }
    public override void SetMoveState(MoveState state)
    {
        m_Core.m_MoveState = state;
        InitSkill();
        InitMoveAI();
        InitAI();
        if (m_Core.m_MoveState == MoveState.Fly) {
            m_Skin.EnableRigidbody(true);
            InBoat = false;
            //EventCenter.AntiRegisterHooks(NDEventType.StatusCG, SetStatus);
            List<Life> list = new List<Life>();
            CM.SearchLifeMListInBoat(ref list, LifeMType.SOLDIER);
            foreach (Role lifeM in list) {
                lifeM.ChangeTarget(ChangeTargetReason.ClearOtherTarget, this);
            }
            ChangeTarget(ChangeTargetReason.ClearTarget, null);
            
            RoleColider col = m_Skin.ProPerty.gameObject.GetComponent<RoleColider>();
            col.EnableColider(ColiderType.Fire, true);
            EventCenter.AntiRegisterHooks(NDEventType.Attr_Paralysis, HitDianJiParalysis);
        } else {
            CreatePet();
#if UNITY_EDITOR_LOG
            FileLog.write(SceneID, "SetMoveState  " +  MapPos);
#endif
            m_Skin.EnableRigidbody(false);
            //EventCenter.RegisterHooks(NDEventType.StatusCG, SetStatus);
            InitHP();
            TargetAttacked = false;
            m_MoveAI.SetBornPos(MapPos, 0);
            StarPosProc();
            m_Status.TriggerStatus();
            InBoat = true;
            if (m_Attr != null) {
                m_Attr.IsAngerSkill = 0;
                m_Attr.Anger = m_Attr.Anger;
            }
            RoleColider col = m_Skin.ProPerty.gameObject.GetComponent<RoleColider>();
            col.EnableColider(ColiderType.Click, true);
            EventCenter.RegisterHooks(NDEventType.Attr_Paralysis, HitDianJiParalysis);
        }
        base.SetMoveState(state);
    }
    
    
    public override void SetDark(bool bDark)
    {
    
        RoleSkin roleSkin = m_Skin as RoleSkin;
        roleSkin.SetDark(bDark);
        // 把相关的特效暂停
        /*GameObjectActionExcute[] ps = m_thisT.GetComponentsInChildren<GameObjectActionExcute>();
        foreach(GameObjectActionExcute p in ps)
        {
        	if (bDark)
        		p.Pause();
        	else
        		p.Play();
        }*/
    }
    
    public override void GameOver(bool isWin)
    {
        base.GameOver(isWin);
        m_Skin.ResetBodySkin();
        if (m_CurrentAction != null) {
            m_CurrentAction.SetDone();
        }
        if (CurPet != null) {
            CurPet.GameOver(isWin);
        }
        if (isWin) {
            m_CurrentAction = new GridActionCmdWin();
        } else {
            m_CurrentAction = new GridActionCmdFaile();
        }
        
        m_CurrentAction.SetTarget(this);
        m_CurrentAction.Update();
    }
    public override void DeadEffect()
    {
    
        RoleSkin skin = m_Skin as RoleSkin;
        if (m_Attr != null && m_Attr.strDieSound.Length > 0) {
            string[] l = m_Attr.strDieSound.Split(',');
            if (l != null && l.Length > 0) {
                Random.seed = GlobalTimer.GetNowTimeInt();
                int index = Random.Range(0, l.Length);
                int Type = int.Parse(l[index]);
                string strSoundName = "";
                if (Type == 1) {
                    strSoundName = "male_death_0";
                } else if (Type == 2) {
                    strSoundName = "female_death_0";
                } else if (Type == 3) {
                    strSoundName = "bear_death_0";
                }
                Random.seed = GlobalTimer.GetNowTimeInt();
                int nSoundIndex = Random.Range(1, 4);
                SoundPlay.Play(strSoundName + nSoundIndex.ToString(), false, false);
            }
        }
        GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
        Vector3 pos = posgo.transform.position;
        pos.z = 1.5f;
        GameObject go = null;
        if (m_Core.m_IsPlayer == true) {
            /*string effectname = "2000231_02";
            if (MoveAI is Fly)
            	effectname = "2000231_04";*/
            string effectname = "2000811";
            GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, effectname, pos, BattleEnvironmentM.GetLifeMBornNode(true));
            GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(2f);
            gae.AddAction(ndEffect);
            go = gae.gameObject;
        } else {
            /*string effectname = "2000231_01";
            if (MoveAI is Fly)
            	effectname = "2000231_03";*/
            string effectname = "2000811";
            GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, effectname, pos, BattleEnvironmentM.GetLifeMBornNode(true));
            GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(2f);
            gae.AddAction(ndEffect);
            go = gae.gameObject;
        }
        
        /*if (go != null)
        {
        	iTween.MoveBy(go,iTween.Hash("amount",new Vector3(0,0,3f),"time",0.1f,"delay",1.5f));
        }*/
    }
    
    
    public void ReBorn()
    {
        //设置可战斗数据中心可发射状态。
        CmCarbon.SetUnBorn(m_Core.m_DataID);
        //设置窗口重新可发射状态。
        CombatWnd wnd = WndManager.FindDialog<CombatWnd>();
        if (wnd) {
            wnd.SetUnBorn(m_Core.m_DataID);
        }
        CurrentAction.SetDone();
        if (m_Pet != null) {
            GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.LeftHandPos);
            if (posgo != null) {
                m_Pet.m_thisT.transform.position = posgo.transform.position;
            }
        }
        //先转换为Fly状态。并置于屏幕外面。
        m_Skin.PlayAnimation(AnimatorState.Fly00000);
        m_Skin.tRoot.parent = BattleEnvironmentM.GetLifeMBornNode(false);
        SetMoveState(MoveState.Fly);
        //设置位置去屏幕外面不可见。
        m_bReBorn = true;
        
        Vector3 pos = m_thisT.localPosition;
        m_thisT.localPosition = U3DUtil.SetY(pos, 100000f);
        //NGUIUtil.PauseGame();
        m_Property.enabled = false;
    }
    // 变身 bStoB是否是从小变大
    public void  TurnsInto(bool bStoB)
    {
        Vector3 pos = m_thisT.transform.position;
        if (bStoB) {
            if (m_Attr.ModelType == 102003 || m_Attr.ModelType == 200009) {
                NoDie = false;
                GameObject.Destroy(m_thisT.gameObject);
                DestroyHp();
                SoldierSkill temp = PropSkillInfo;
                int type = m_Attr.ModelType == 102003 ? 1020032 : 2000092;
                //IGameRole i = GameRoleFactory.Create(BattleEnvironmentM.GetLifeMBornNode(true), type, m_Attr.AttrName, AnimatorState.Stand);
                CreateSkin(BattleEnvironmentM.GetLifeMBornNode(true), type, m_Attr.AttrName, AnimatorState.Stand, m_Core.m_IsPlayer);
                GameObject go = RoleSkinCom.tRoot.gameObject;
                go.transform.position = pos;
                LifeObj r = go.AddComponent<LifeObj>();
                r.ChangeLife(this as Life, RoleSkinCom.ProPerty);
                m_Core.m_bTurn = true;
                SetSkin();
                RoleSkin();
                //if(m_Core.m_MoveState == MoveState.Walk)
                InitSkill();
                PropSkillInfo = temp;
                if (CurrentAction != null) {
                    CurrentAction.m_LifePrent = this;
                    CurrentAction.m_Skin = m_Skin;
                }
                CombatWnd  cw = WndManager.FindDialog<CombatWnd>();
                if (cw != null) {
                    cw.SetAngerSpriteColor(m_SceneID, true);
                }
                GameObject gpos = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.Bone1Pos);
                GameObjectActionExcute gae = gpos.GetComponentInChildren<GameObjectActionExcute>();
                if (gae == null) {
                    gae = EffectM.LoadEffect(EffectM.sPath, m_Attr.ModelType == 102003 ? "1103011" : "1401011", gpos.transform.position, gpos.transform);
                    gae.transform.Rotate(new Vector3(0, 0, 180));
                }
                gpos = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.Bone2Pos);
                gae = gpos.GetComponentInChildren<GameObjectActionExcute>();
                if (gae == null) {
                    gae = EffectM.LoadEffect(EffectM.sPath, m_Attr.ModelType == 102003 ? "1103011" : "1401011", gpos.transform.position, gpos.transform);
                }
                gpos = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.Bone3Pos);
                gae = gpos.GetComponentInChildren<GameObjectActionExcute>();
                if (gae == null) {
                    gae = EffectM.LoadEffect(EffectM.sPath, m_Attr.ModelType == 102003 ? "1103021" : "1401021", gpos.transform.position, gpos.transform);
                }
            } else if (m_Attr.ModelType == 102004) {
                foreach (GameObject go in m_Skin.ProPerty.roleProperties) {
                    Animator ani = go.GetComponent<Animator>();
                    if (ani != null) {
                    
                        GameObject gpos = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.Bone1Pos);
                        GameObjectActionExcute gae = gpos.GetComponentInChildren<GameObjectActionExcute>();
                        if (gae == null) {
                        
                            gae = EffectM.LoadEffect(EffectM.sPath, "1104011_01", gpos.transform.position, gpos.transform);
                        }
                        
                        gpos = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.Bone4Pos);
                        gae = gpos.GetComponentInChildren<GameObjectActionExcute>();
                        if (gae == null) {
                            gae = EffectM.LoadEffect(EffectM.sPath, "1104011_02", gpos.transform.position, gpos.transform);
                        }
                        gpos = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.Bone2Pos);
                        gae = gpos.GetComponentInChildren<GameObjectActionExcute>();
                        if (gae == null) {
                            gae = EffectM.LoadEffect(EffectM.sPath, "1104011_03", gpos.transform.position, gpos.transform);
                            gae.transform.localEulerAngles += new Vector3(0, 0, 180);
                        }
                        gpos = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.Bone3Pos);
                        gae = gpos.GetComponentInChildren<GameObjectActionExcute>();
                        if (gae == null) {
                            gae = EffectM.LoadEffect(EffectM.sPath, "1104011_03", gpos.transform.position, gpos.transform);
                        }
                        
                        
                        AnimatorOverrideController  controller = Resources.Load<RuntimeAnimatorController>("AnimaCtrl/Roles/1020041@Over") as AnimatorOverrideController;
                        ani.runtimeAnimatorController = controller;
                        m_Skin.PlayAnimation(AnimatorState.Stand);
                    }
                }
                
            }
        } else {
        
            if (m_Attr.ModelType == 102003 || m_Attr.ModelType == 200009) {
                GameObject.Destroy(m_thisT.gameObject);
                DestroyHp();
                //IGameRole i = GameRoleFactory.Create(BattleEnvironmentM.GetLifeMBornNode(true), m_Attr.ModelType, m_Attr.AttrName, AnimatorState.Stand);
                CreateSkin(BattleEnvironmentM.GetLifeMBornNode(true), m_Attr.ModelType, m_Attr.AttrName, AnimatorState.Stand, m_Core.m_IsPlayer);
                GameObject go = RoleSkinCom.tRoot.gameObject;
                go.transform.position = pos;
                LifeObj r = go.AddComponent<LifeObj>();
                r.ChangeLife(this as Life, RoleSkinCom.ProPerty);
                m_Core.m_bTurn = false;
                SetSkin();
                RoleSkin();
                //if(m_Core.m_MoveState == MoveState.Walk)
                InitSkill();
                if (CurrentAction != null) {
                    CurrentAction.SetDone();
                }
                CombatWnd  cw = WndManager.FindDialog<CombatWnd>();
                if (cw != null) {
                    cw.SetAngerSpriteColor(m_SceneID, false);
                }
            } else if (m_Attr.ModelType == 102004) {
                m_bTrunBack = false;
                if (m_Status.HaveState(StatusType.Berserker)) {
                    return;
                }
                foreach (GameObject go in m_Skin.ProPerty.roleProperties) {
                    Animator ani = go.GetComponent<Animator>();
                    if (ani != null) {
                        GameObject gpos = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.Bone1Pos);
                        GameObjectActionExcute gae = gpos.GetComponentInChildren<GameObjectActionExcute>();
                        GameObject.Destroy(gae.gameObject);
                        gpos = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.Bone4Pos);
                        gae = gpos.GetComponentInChildren<GameObjectActionExcute>();
                        GameObject.Destroy(gae.gameObject);
                        gpos = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.Bone2Pos);
                        gae = gpos.GetComponentInChildren<GameObjectActionExcute>();
                        GameObject.Destroy(gae.gameObject);
                        gpos = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.Bone3Pos);
                        gae = gpos.GetComponentInChildren<GameObjectActionExcute>();
                        GameObject.Destroy(gae.gameObject);
                        
                    }
                }
                CurrentAction = new TurnBack102004();
                CurrentAction.SetTarget(this);
            }
        }
    }
    public void SomeBodyDie(Life dielife)
    {
        if (dielife != this && !isDead) {
            if (RoleSkill.CheckDie(dielife)) {
                if (m_Attr.ModelType == 102003 || m_Attr.ModelType == 200009) {
                
                    GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectPos);
                    if (posgo != null) {
                        Vector3 pos = posgo.transform.position;
                        //pos.z -=4f;
                        string name = m_Attr.ModelType == 102003 ? "1103091" : "1401091";
                        
                        if (m_Core.m_bTurn) {
                            name = m_Attr.ModelType == 102003 ? "1103081" : "1401081";
                        }
                        GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, name, pos, posgo.transform);
                        GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
                        gae.AddAction(ndEffect);
                    }
                }
            }
        }
    }
}



