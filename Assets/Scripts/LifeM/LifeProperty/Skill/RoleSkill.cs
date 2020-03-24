#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    #define UNITY_EDITOR_LOG
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class RoleSkill: Skill
{
    /// <summary>
    /// 所有技能
    /// </summary>
    protected List<SoldierSkill> m_AllSkill  = new List<SoldierSkill>();
    /// <summary>
    /// 条件技能
    /// </summary>
    protected List<SoldierSkill> m_ConditionSkill  = new List<SoldierSkill>();
    /// <summary>
    /// 条件技能计时器
    /// </summary>
    protected List<float> m_ConditionSkillTimeCount  = new List<float>();
    protected string m_attack1 = string.Empty;
    protected string m_attack2 = string.Empty;
    private int   m_HitN = 0;       //命中
    private int   m_HitM = 0;
    private int   m_CritN = 0;      //暴击
    private int   m_CritM = 0;
    /// <summary>
    /// 攻击次数
    /// </summary>
    private int   m_AttackTimes = 1;
    private List<Life> m_AttackList = new List<Life>();
    protected Role m_SkillOwner = null;
    // Update is called once per frame
    public override void Update(float deltaTime)
    {
        if (m_SkillOwner != null) {
            if (m_CdTime > 0) {
                m_CdTime -= deltaTime * m_SkillOwner.m_Attr.AttackSpeed;
                if (m_CdTime <= 0) {
                    if (m_AttackSkill.m_Relsease) {
                        GetNextSkill();
                    }
                }
            }
        }
        for (int i = 0; i < m_ConditionSkillTimeCount.Count; i++) {
            m_ConditionSkillTimeCount[i] -= deltaTime * m_SkillOwner.m_Attr.AttackSpeed;
        }
    }
    
    /// <summary>
    /// 获取下一个轮询技能ID
    /// </summary>
    /// <returns></returns>
    protected virtual  int GetNextSkillOrderID()
    {
        int N = m_AttackTimes ++;
        if (string.IsNullOrEmpty(m_attack1) || string.IsNullOrEmpty(m_attack2) || N <= 0) {
            Debug.LogError("数据非法 " + N);
            return -1;
        } else {
            int order = 0;
            int M = m_attack1.Length;
            int P = m_attack2.Length;
            if (N <= M) {
                order =  NdUtil.GetNumIndex(m_attack1, N);
            } else {
                int Q = (N - M) % P;
                if (Q == 0) {
                    Q = P;
                }
                order = NdUtil.GetNumIndex(m_attack2, Q);
            }
            SoldierInfo info =  CmCarbon.GetSoldierInfo(m_SkillOwner.m_Core);
            if (order == 1 && info.m_Skill.m_skill1_level == 0) {
                return GetNextSkillOrderID();
            }
            if (order == 2 && info.m_Skill.m_skill2_level == 0) {
                return GetNextSkillOrderID();
            }
            if (order == 3 && info.m_Skill.m_skill3_level == 0) {
                return GetNextSkillOrderID();
            }
            if (order == 4 && info.m_Skill.m_skill4_level == 0) {
                return GetNextSkillOrderID();
            }
            if (order == 5 && info.m_Skill.m_skill5_level == 0) {
                return GetNextSkillOrderID();
            }
            return order;
            
        }
    }
    /// <summary>
    /// 获取下一个技能
    /// </summary>
    /// <returns></returns>
    public override bool GetNextSkill()
    {
        int skillNo = GetNextSkillOrderID();
        m_AttackSkill = GetSkill(skillNo);
        m_AttackSkill.m_Relsease = false;
        return true;
    }
    
    /// <summary>
    /// 获取技能
    /// </summary>
    /// <param name="SkillNo"></param>
    /// <returns></returns>
    public SoldierSkill GetSkill(int SkillNo)
    {
        if (SkillNo >= 0 && SkillNo <= 7) {
            return m_AllSkill[SkillNo];
        }
        return null;
    }
    /// <summary>
    /// 获取大招
    /// </summary>
    /// <returns></returns>
    public override SoldierSkill GetBigSkill()
    {
        if (m_AllSkill[1].m_type != 0) {
            return m_AllSkill[1];
        } else {
            return  null;
        }
    }
    
    /// <summary>
    /// 获取次要攻击技能
    /// </summary>
    public SoldierSkill GetLSkill()
    {
        if (m_AllSkill[7].m_type != 0) {
            return m_AllSkill[7];
        } else {
            return null;
        }
    }
    
    public override void ResetConditionTime(int id)
    {
        for (int i = 0; i < m_ConditionSkill.Count; i++) {
            if (m_ConditionSkill[i].m_type == id) {
                m_ConditionSkillTimeCount[i] = m_ConditionSkill[i].m_cd * 0.001f;
            }
        }
    }
    /// <summary>
    /// 初始化技能数据，从数据中心获取所有技能数据
    /// </summary>
    public override bool Init(int SceneID, LifeMCore Core)
    {
        this.m_SceneID = SceneID;
        m_SkillOwner = CM.GetLifeM(m_SceneID, LifeMType.SOLDIER) as Role;
        SoldierInfo info = CmCarbon.GetSoldierInfo(Core);
        if (info == null) {
            return false;
        }
        SoldierSkillInfo Info = info.m_Skill;
        m_attack1 = Info.attack1;
        m_attack2 = Info.attack2;
        for (int i = 0; i <= 7; i++) {
            /*if (Info.m_Skillinfo[i].m_attacktype != (int)AttackType.Aura)
            {
            	if (!Skill.AuraSkill.Contains(this))
            		Skill.AuraSkill.Add(this);
            }*/
            if (Info.m_Skillinfo[i].m_attacktype != (int)AttackType.PassiveCondition && Info.m_Skillinfo[i].m_condition != 0 && Info.m_Skillinfo[i].m_enable == true) {
                m_ConditionSkill.Add(Info.m_Skillinfo[i]);
                m_ConditionSkillTimeCount.Add(0f);
                
            }
            m_AllSkill.Add(Info.m_Skillinfo[i]);
            Info.m_Skillinfo[i].m_LifeTarget = null;
            Info.m_Skillinfo[i].m_TargetPos  = null;
        }
        GetNextSkill();
        m_CdTime = m_CDDuration;
        m_CDDuration = info.m_attack_time ;
        m_Release = false;
        return true;
    }
    /// <summary>
    /// 判定技能是否命中
    /// </summary>
    protected override bool SkillHit(NDAttribute Attack, NDAttribute Defense, SkillInfo skill)
    {
        if (skill == null) {
            return false;
        }
        SoldierSkill soldierskill = m_skill as SoldierSkill ;
        if (Attack == null || Defense == null || skill == null) {
            return false;
        }
        //非炮弹兵一定命中
        if ((Defense is RoleAttribute) == false) {
            return true;
        }
        
        AttackType Type = (AttackType)skill.m_attacktype;
        if (Type == AttackType.Physical) { //物理攻击技能
            if (Defense is RoleAttribute) {
                /*if (Defense.Level < soldierskill.m_percent)
                {  //低于必中等级
                	return true;
                }
                else */
                {
                    return SkillPhyHit(Attack, Defense);
                }
            } else {
                return true;
            }
        } else {
            return true;
        }
    }
    
    /// <summary>
    /// 判定物理攻击技能的是否命中
    /// </summary>
    private bool SkillPhyHit(NDAttribute Attack, NDAttribute Defense)
    {
        if (Attack == null || Defense == null) {
            return false;
        }
        double Basehit = ScriptM.Formula<float>("CALC_SOLDIER_HIT", Attack.Hit, Defense.DodgeRatio, Defense.Dodge);
        double realhit = Basehit + m_HitN * Basehit + m_HitM * (1 - Basehit);
        realhit = realhit > 1 ? 1 : realhit;
        realhit = realhit < 0 ? 0 : realhit;
        float value = Random.Range(0.0f, 1.0f);
        if (value <= realhit) {
            m_HitN = 0;
            m_HitM ++;
            return true;
        } else {
            m_HitN ++;
            m_HitM = 0;
        }
        return false;
    }
    
    /// <summary>
    /// 是否技能暴击
    /// </summary>
    protected  override bool SkillCrit(NDAttribute Attack, NDAttribute Defense, SkillInfo skill)
    {
        if (Attack == null || Defense == null || skill == null) {
            return false;
        }
        //非炮弹兵一定暴击
        if ((Defense is RoleAttribute) == false) {
            return false;
        }
        
        AttackType Type = (AttackType)skill.m_attacktype;
        if (Type == AttackType.Physical) { //物理攻击
            double baseCritical = ScriptM.Formula<float>("CALC_SOLDIER_PHYCRITS",
                    Attack.PhyCrit,
                    Defense.CritRatio);
            return CheckCrit(baseCritical);
        } else if (Type == AttackType.Magic) { // 魔法攻击
            double baseCritical = ScriptM.Formula<float>("CALC_SOLDIER_MAGICCRITS", Attack.MagicCrit, Defense.CritRatio);
            return CheckCrit(baseCritical);
        }
        
        return false;
    }
    
    
    private bool CheckCrit(double BaseCritical)
    {
        double realCritical = BaseCritical + m_CritN * BaseCritical - m_CritM * (1 - BaseCritical);
        realCritical = realCritical > 1 ? 1 : realCritical;
        realCritical = realCritical < 0 ? 0 : realCritical;
        float value = Random.Range(0.0f, 1.0f);
        if (value <= realCritical) {
            m_CritN = 0;
            m_CritM ++;
            return true;
        } else {
            m_CritN ++;
            m_CritM = 0;
            return false;
        }
        return false;
    }
    
    public void CheckStart()
    {
    
    }
    
    public static GridActionCmd GetAttackSkillAction(int skillid, int AttackSceneId, WalkDir AttackDir, int deep, int modetype, int sceneID)
    {
        Life l = CM.GetAllLifeM(sceneID, LifeMType.SOLDIER | LifeMType.SUMMONPET);
        if (l == null || !(l is Role)) {
            return null;
        }
        Role w = l as Role;
        RoleSkill rs = w.m_Skill as RoleSkill;
        GridActionCmdAttack action = GridActionCmdAttackFactory.Create(rs.DoQianYaoStatus, rs.DoSkill, rs.DoSkill, sceneID, AttackSceneId, AttackDir, deep, skillid, w.m_Attr.AttrType, (rs.PropSkillInfo as SoldierSkill).m_blackscreentime);
        if (action == null) {
            Debug.Log("GetAttackSkillAction  " + sceneID + "," + AttackSceneId + "," + AttackDir + "," + deep + "," + skillid + "," + w.m_Attr.AttrType);
            Debug.Log("GetAllSkill  " + rs.m_AllSkill[0].m_type + "," + rs.m_AllSkill[1].m_type + "," + rs.m_AllSkill[2].m_type + "," + rs.m_AllSkill[3].m_type + "," +
                rs.m_AllSkill[4].m_type + "," + rs.m_AllSkill[5].m_type + "," + rs.m_AllSkill[6].m_type + "," + w.m_Attr.AttrType);
            //action = GridActionCmdAttackFactory.Create(rs.DoQianYaoStatus,rs.DoAttack,rs.DoReleaseSkill,sceneID, AttackSceneId, AttackDir, deep,skillid ,w.m_Attr.AttrType,(rs.PropSkillInfo as SoldierSkill).m_blackscreentime);
        }
        if (rs.PropSkillInfo.m_attckmodeid > 0) {
            List<float> times = new List<float>();
            rs.PropSkillInfo.GetAttackTimes(ref times);
            int count = times.Count;
            action.SetAttackInfo(count, times);
        } else if ((rs.PropSkillInfo as SoldierSkill).m_actiontype == 1) {
            int count = (rs.PropSkillInfo as SoldierSkill).m_step_secs / (rs.PropSkillInfo as SoldierSkill).m_timeinterval;
            List<float> times = new List<float>();
            
            for (int i = 0; i < count; i++) {
                times.Add(i * (rs.PropSkillInfo as SoldierSkill).m_timeinterval * 0.001f);
            }
            action.SetAttackInfo(count, times);
        }
        return action;
    }
    
    
    public static bool CheckConditionSkillAction(int SceneID, ref SoldierSkill info, bool newaction, bool BigSkill, StatusState state = StatusState.Add)
    {
        Life life = CM.GetAllLifeM(SceneID, LifeMType.SOLDIER | LifeMType.SUMMONPET);
        if (life != null) {
            Role w =  life as Role;
            RoleSkill rs = (w.m_Skill as RoleSkill);
            if (rs == null || !w.m_Attr.CanAttack) {
                return false;
            }
            List<SoldierSkill> l = new List<SoldierSkill>();
            SoldierSkill bskill = rs.GetBigSkill();
            
            for (int i = 0; i < rs.m_ConditionSkill.Count; i++) {
                SoldierSkill skill = rs.m_ConditionSkill[i];
                int skillCondition = skill.m_condition;
                if (rs.m_ConditionSkillTimeCount[i] <= 0) {
                    bool existSkillTarget = rs.GetSkillTarget(skill);
                    if (skillCondition == (int)SkillCondition.Walk
                        && w.CurrentAction is GridActionCmdWalk
                        && newaction
                        && existSkillTarget) {
                        l.Add(skill);
                    } else if (skillCondition == (int)SkillCondition.NotAttack
                        && w.m_NoHurtCount > skill.m_condition_data0
                        && w.m_Attr.Hp < w.m_Attr.FullHp
                        && existSkillTarget) {
                        l.Add(skill);
                    } else if (skillCondition == (int)SkillCondition.Attack
                        && w.CurrentAction is GridActionCmdAttack
                        && existSkillTarget) {
                        l.Add(skill);
                    } else if (skillCondition == (int)SkillCondition.HasTrapInRange
                        && w.m_Core.m_Camp == LifeMCamp.ATTACK
                        && w.run.CheckTrapInRoadSameLayer(skill.m_condition_data0 / MapGrid.m_Pixel)
                        && existSkillTarget) {
                        if (rs.m_ConditionSkill[i].m_type == 1018) {
                            if (!CM.HaveIntanseInScene(LifeMType.PET, w.m_Core.m_Camp, skill.m_data2)) {
                                l.Add(skill);
                            }
                        } else {
                            l.Add(skill);
                        }
                    } else if (skillCondition == (int)SkillCondition.Die) {
                        if (skill.m_condition_data0 == 2) {
                            List<Life> lp = new List<Life>();
                            CM.SearchLifeMListInBoat(ref lp, LifeMType.PET, w.m_Core.m_Camp);
                            bool havepet = false;
                            foreach (Pet p in lp) {
                                if (p.CheckParent(w)) {
                                    havepet = true;
                                }
                            }
                            if (!havepet) {
                                l.Add(skill);
                            }
                        } else if (skill.m_condition_data0 == 4) {
                            if (w.isDead) {
                                if (rs.GetSkillTarget(skill)) {
                                    l.Add(skill);
                                }
                            }
                        }
                    } else if (skillCondition == (int)SkillCondition.HaveDoor) {
                        if (w.m_Core.m_Camp == LifeMCamp.ATTACK && existSkillTarget) {
                            l.Add(skill);
                        }
                    } else if (skillCondition == (int)SkillCondition.NoPet && rs.m_SkillOwner.CurPet == null) {
                        l.Add(skill);
                    } else if (skillCondition == (int)SkillCondition.BigSkillNoPet && rs.m_SkillOwner.CurPet == null) {
                        if (BigSkill && w.m_Core.m_IsPlayer == true) {
                            if (rs.GetBigAttackTarget()) {
                                l.Add(skill);
                            }
                        }
                        if (w.m_Core.m_IsPlayer == false
                            && bskill != null
                            &&  w.Anger >= bskill.m_use_mp
                        ) {
                            if (rs.GetBigAttackTarget()) {
                                l.Add(skill);
                            }
                        }
                    } else if (skillCondition == (int)SkillCondition.LowHp) {
                        if (w.HP  < w.fullHP * skill.m_condition_data0 * 0.001f && existSkillTarget) {
                            l.Add(skill);
                        }
                    } else if (skillCondition == (int)SkillCondition.InTrapRange) {
                        //if (rs.GetSkillTarget(rs.m_ConditionSkill[i]))
                        //l.Add(rs.m_ConditionSkill[i]);
                    } else if (skillCondition == (int)SkillCondition.NoFullHpNoSelf) {
                        if (existSkillTarget) {
                            l.Add(skill);
                        }
                    } else  if (skillCondition == (int)SkillCondition.SameLayer) {
                        if (existSkillTarget) {
                            l.Add(skill);
                        }
                    } else if (skillCondition == (int)SkillCondition.TrapAttack) {
                        if (existSkillTarget) {
                            l.Add(skill);
                        } else {
                            List<Life> lp = new List<Life>();
                            CM.SearchLifeMListInBoat(ref lp, LifeMType.INHERITSUMMONPROS, GetSkillCamp(skill, w));
                            foreach (Life p in lp) {
                            
                                if (w.RoleSkill.CheckAttackTarget(p, skill.m_sort, skill.m_distance / MapGrid.m_Pixel, skill.m_condition != (int)SkillCondition.SameLayer) &&
                                    p.m_Attr.AttrType == skill.m_condition_data1) {
                                    skill.m_LifeTarget = p;
                                    l.Add(skill);
                                }
                            }
                        }
                    } else if (skillCondition == (int)SkillCondition.HaveStatus) {
                        if (w.m_Status.HaveState((StatusType)rs.m_ConditionSkill[i].m_condition_data0)) {
                            int sceneid = w.m_Status.GetResalseSceneID((StatusType)skill.m_condition_data0);
                            
                            skill.m_LifeTarget = CM.GetAllLifeM(sceneid, LifeMType.BUILD | LifeMType.SOLDIER);
                            l.Add(skill);
                        }
                    } else if (skillCondition == (int)SkillCondition.RemoveStatus) {
                        if (state == StatusState.Remove && w.m_Status.HaveState((StatusType)skill.m_condition_data0)) {
                            if (existSkillTarget) {
                                l.Add(skill);
                            }
                        }
                    } else if (skillCondition == (int)SkillCondition.LowHpAndHaveDownLayer) {
                        //Debug.Log((w.HP * 1.0f/w.fullHP) + "," + (rs.m_ConditionSkill[i].m_data0 * 0.001f) + "," + (w.HP * 1.0f/w.fullHP < rs.m_ConditionSkill[i].m_data0 * 0.001f) + "," +w.MapPos.Layer );
                        if (w.HP * 1.0f / w.fullHP < rs.m_ConditionSkill[i].m_condition_data0 * 0.001f && w.MapPos.Layer > 0) {
                            //Debug.Log( rs.m_ConditionSkill[i].m_name );
                            if (existSkillTarget) {
                                l.Add(skill);
                            }
                        }
                    } else if (skillCondition == (int)SkillCondition.near) {
                        if (existSkillTarget) {
                            l.Add(skill);
                        }
                    } else if (skillCondition == (int)SkillCondition.LowHPAndDie) {
                        if ((w.HP  < w.fullHP * skill.m_condition_data0 * 0.001f || w.isDead) && existSkillTarget) {
                            w.NoDie = true;
                            l.Add(skill);
                        }
                        if (state == StatusState.SoonDie && existSkillTarget) {
                            w.NoDie = true;
                            l.Add(skill);
                        }
                    } else if (skillCondition == (int)SkillCondition.AttackBuild) {
                        if (existSkillTarget) {
                            l.Add(skill);
                        }
                    }
                }
            }
            if (newaction) {
                if (rs.CDTime <= 0 && w.run.CheckAttackCondition()) { //&& !w.m_Status.CheckStateBySkill(1022))//熊孩子的大招状态不能攻击
                    l.Add(w.PropAttackSkillInfo);
                }
                
                if (BigSkill && w.m_Core.m_IsPlayer == true) {
                    //if (rs.GetBigAttackTarget() )
                    l.Add(bskill);
                }
                if (w.m_Core.m_IsPlayer == false && bskill != null &&  w.Anger >= ConfigM.GetAngerK(1)) {
                    if (rs.GetBigAttackTarget()) {
                        l.Add(bskill);
                    }
                }
                
            }
            if (l.Count > 0) {
                info = l[0];
            } else {
                return false;
            }
            for (int i = 1; i < l.Count; i++) {
                if (info.m_priority < l[i].m_priority) {
                    info = l[i];
                }
            }
            return true;
        }
        return false;
    }
    
    public static  GridActionCmd GetConditionSkillAction(int SceneID,  SoldierSkill info, Vector3 start, Vector3 end, float d, bool ismove)
    {
        Role w =  CM.GetAllLifeM(SceneID, LifeMType.SOLDIER | LifeMType.SUMMONPET) as Role;
        
        RoleSkill rs = (w.m_Skill as RoleSkill);
        rs.m_SkillTarget = info.m_LifeTarget;
        rs.PropSkillInfo = info;
        
        w.Anger -= (rs.PropSkillInfo as SoldierSkill).m_use_mp;
        GridActionCmd action = null;
        if (info.m_distance > 0 && (info.m_LifeTarget == null || !(info.m_LifeTarget is IggWall)) && info.m_TargetPos != null)
            //rs.m_ConditionSkillTarget[info.m_type] == null || !(rs.m_ConditionSkillTarget[info.m_type] is Wall) ) && rs.m_ConditionSkillTargetPos[info.m_type] != null)
        {
            //门开着直接过。不用打。 更改为都要打。 参考炮弹兵遇墙规则
            /*List<Wall> lw = new List<Wall>();
            MapGrid.GetWallList(ref lw,w.GetMapPos(),rs.m_ConditionSkillTargetPos[info.m_type].GridPos);
            bool havedoor = false;
            foreach(Wall wl in lw)
            {
            	if (!wl.m_Attr.DoorState)
            	{
            		havedoor = true;
            	}
            	wl.OpenDoor(OpenDoorStyle.Attack,w.m_Core.m_Camp,Mathf.Abs(rs.m_ConditionSkillTargetPos[info.m_type].GridPos.Unit - w.GetMapPos().Unit));
            }
            if (havedoor)
            	return null;*/
        }
        if (info.m_type == 1023) {
            action = new GridActionCmd102001ConditionSkill01(start, end, 0f, d, (w.m_Skill as RoleSkill).DoSkill, (w.m_Skill as RoleSkill).DoQianYaoStatus);
            w.m_NoHurtCount = 0;
        }
        
        else if (info.m_type == 1024) {
            action = new GridActionCmd102001Skill1024(start, end, 0f, d, (w.m_Skill as RoleSkill).DoSkill, (w.m_Skill as RoleSkill).DoQianYaoStatus);
        } else if (info.m_type == 1034) {
        
            if (w.CurrentGS == GridSpace.Space_UP) {
                action = new GridActionCmdJumpDown(MapGrid.GetMG(w.MapPos).Uppos, MapGrid.GetMG(w.MapPos).pos, 1f, w.WalkDir, w.RankDeep);
                action.SetTarget(w);
                w.CurrentGS = GridSpace.Space_DOWN;
                AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_SELFSP), w);
                return action;
            }
            action = new GridActionCmd101001ConditionSkill01(start, end, (w.m_Skill as RoleSkill).DoSkill, w.WalkDir, (w.m_Skill as RoleSkill).DoQianYaoStatus);
        }
        /*else if (info.m_type == 1033)
        {
            action = new GridActionCmd100003ConditionSkill02(start, end, (w.m_Skill as RoleSkill).DoConditionSkill, w.WalkDir);
        }*/
        else if (info.m_type == 1043) {
            action = new GridActionCmd102005Skill1043(start, end, (w.m_Skill as RoleSkill).DoSkill, w.WalkDir, (w.m_Skill as RoleSkill).DoQianYaoStatus);
        } else if (info.m_type == 1053) {
            action = new GridActionCmd100004ConditionSkill1053(start, end, (w.m_Skill as RoleSkill).DoSkill, (w.m_Skill as RoleSkill).DoQianYaoStatus);
        } else if (info.m_type == 1054) {
            action = new GridActionCmd100004ConditionSkill1054(start, end, (w.m_Skill as RoleSkill).DoSkill, (w.m_Skill as RoleSkill).DoQianYaoStatus);
        } else if (info.m_type == 1055) {
            action = new GridActionCmd100004ConditionSkill1055(start, end, (w.m_Skill as RoleSkill).DoSkill, (w.m_Skill as RoleSkill).DoQianYaoStatus);
        } else if (info.m_type == 1060) {
            action = new GridActionCmd101003Ski1060(start, end, (w.m_Skill as RoleSkill).DoSkill, w.WalkDir, (w.m_Skill as RoleSkill).DoQianYaoStatus);
        } else if (info.m_type == 5001) {
            action = new GridActionCmd200002Skill5001(start, (w.m_Skill as RoleSkill).DoSkill, (w.m_Skill as RoleSkill).DoQianYaoStatus);
        } else if (info.m_type == 5002) {
            action = new GridActionCmd200002Skill5002(start, (w.m_Skill as RoleSkill).DoSkill, (w.m_Skill as RoleSkill).DoQianYaoStatus);
        } else if (info.m_type == 5003) {
            action = new GridActionCmd200003Skill5003(start, (w.m_Skill as RoleSkill).DoSkill, (w.m_Skill as RoleSkill).DoQianYaoStatus);
        } else if (info.m_type == 5004) {
            action = new GridActionCmd200003Skill5004(start, (w.m_Skill as RoleSkill).DoSkill, (w.m_Skill as RoleSkill).DoQianYaoStatus);
        } else if (info.m_type == 1066) {
            action = new GridActionCmd102003ConditionSkill1066(start, (w.m_Skill as RoleSkill).DoSkill, (w.m_Skill as RoleSkill).DoQianYaoStatus);
        } else if (info.m_type == 1067) {
            action = new GridActionCmd102003ConditionSkill1067(start, (w.m_Skill as RoleSkill).DoSkill, (w.m_Skill as RoleSkill).DoQianYaoStatus);
        } else if (info.m_type == 5006) {
            action = new GridActionCmd200004Skill5006(start, (w.m_Skill as RoleSkill).DoSkill, w.WalkDir, (w.m_Skill as RoleSkill).DoQianYaoStatus);
        } else if (info.m_type == 1073) {
            action = new GridActionCmd102004Skill1073(start, (w.m_Skill as RoleSkill).DoSkill, (w.m_Skill as RoleSkill).DoQianYaoStatus);
        } else if (info.m_type >= 5013 && info.m_type <= 5015) {
            action = new GridActionCmd200009ConditionSkill5013(start, (w.m_Skill as RoleSkill).DoSkill, (w.m_Skill as RoleSkill).DoQianYaoStatus);
        } else if (info.m_type >= 5016 && info.m_type <= 5018) {
            action = new GridActionCmd200009ConditionSkill5016(start, (w.m_Skill as RoleSkill).DoSkill, (w.m_Skill as RoleSkill).DoQianYaoStatus);
        } else if (info.m_type == 5010) {
            action = new GridActionCmd200009ConditionSkill1063(start, (w.m_Skill as RoleSkill).DoSkill, (w.m_Skill as RoleSkill).DoQianYaoStatus);
        }
        //		else if(info.m_type == 5019)
        //		{
        //			action = new GridActionCmd200008BigSkill((w.m_Skill as RoleSkill).DoQianYaoStatus,(w.m_Skill as RoleSkill).DoSkill,SceneID, 0, w.WalkDir,w.RankDeep,info.m_type,0);
        //		}
        else {
            if (rs.IsBigSkill(info)) {
                action = (w.m_Skill as RoleSkill).GetBigAttackAction();
            } else {
                if (w.run.CheckAttackPos()) {
                    action = w.run.GetAttackAction();
                }
            }
        }
        if (action != null && action is GridActionSkill) {
            if (action is GridActionCmdConditionSkill) {
                GridActionCmdConditionSkill conditionskill = action as GridActionCmdConditionSkill;
                conditionskill.StartWithTarget(w);
                w.m_Skill.ResetConditionTime(info.m_type);
                
                if (rs.PropSkillInfo.m_attckmodeid > 0) {
                    List<float> times = new List<float>();
                    rs.PropSkillInfo.GetAttackTimes(ref times);
                    int count = times.Count;
                    conditionskill.SetAttackInfo(count, times);
                }
                conditionskill.m_ismove = ismove;
                conditionskill.SetSpeed(w.m_Attr.AttackSpeed);
            } else if (info.m_condition > 0) {
                rs.ResetConditionTime(info.m_type);
            }
            if ((rs.PropSkillInfo as SoldierSkill).m_ischangetarget == 1) {
#if UNITY_EDITOR_LOG
                FileLog.write(w.SceneID, "GetConditionSkillAction");
#endif
                w.ChangeTarget(ChangeTargetReason.SkillChangeTarget, rs.PropSkillInfo.m_LifeTarget);
            }
        }
        return action;
    }
    
    
    public bool IsBigSkill(SoldierSkill skill)
    {
        return skill.m_type == m_AllSkill[1].m_type;
    }
    public override bool GetSkillTarget(SoldierSkill Skill)
    {
        //获取攻击列表
        if (Skill.m_target == (int)SkillTarget.Self || Skill.m_target == (int)SkillTarget.FriendlyTeam || Skill.m_target == (int)SkillTarget.EnemyTeam) {
            Skill.SetTarget(m_SkillOwner, m_SkillOwner.GetMapGrid());
            return true;
        } else if (Skill.m_target == (int)SkillTarget.EnemyGround  || Skill.m_target == (int)SkillTarget.FriendlyGround) {
            Int2 pos = m_SkillOwner.GetMapPos();
            MapGrid lgrid = MapGrid.GetMG(pos.Layer, pos.Unit - Skill.m_distance / MapGrid.m_Pixel);
            MapGrid rgrid = MapGrid.GetMG(pos.Layer, pos.Unit + Skill.m_distance / MapGrid.m_Pixel);
            
            if (m_SkillOwner.WalkDir == WalkDir.WALKLEFT && lgrid != null) {
                Skill.m_TargetPos = lgrid;
                //m_ConditionSkillTargetPos[Skill.m_type] = lgrid;
                return true;
            } else if (m_SkillOwner.WalkDir == WalkDir.WALKRIGHT && rgrid != null) {
                Skill.m_TargetPos = rgrid;
                //m_ConditionSkillTargetPos[Skill.m_type] = rgrid;
                return true;
            }
        }
        
        if (!GetAttackList(Skill, IsBigSkill(Skill))) {
        
            Skill.SetTarget(null, null);
            //m_ConditionSkillTarget[Skill.m_type] = null;
            //m_ConditionSkillTargetPos[Skill.m_type] = null;
            return false;
        }
        
        //从攻击列表中获取最佳攻击目标
        /*m_ConditionSkillTarget[Skill.m_type]*/ Skill.m_LifeTarget = GetBestAttackTarget(Skill);
        int id = -1;
        if (/*m_ConditionSkillTarget[Skill.m_type]*/Skill.m_LifeTarget != null) {
            Skill.m_TargetPos = Skill.m_LifeTarget.GetMapGrid();
            //m_ConditionSkillTargetPos[Skill.m_type] = m_ConditionSkillTarget[Skill.m_type].GetMapGrid();
            return true;
        }
        return false;
    }
    
    
    //获取攻击目标
    public override bool GetAttackTarget(ref int sceneid)
    {
        if ((m_SkillOwner as Role).CurrentAction is GridActionCmdAttack) {
            if (!((m_SkillOwner as Role).CurrentAction as GridActionCmdAttack).Played) {
                return true;
            }
        }
        if (m_SkillOwner.ForceChangeTarget) {
            m_SkillOwner.ForceChangeTarget = false;
            m_AttackTarget = null;
        }
        bool result = GetSkillTarget(PropAttackSkillInfo);
        if (result && PropAttackSkillInfo.m_LifeTarget != null) {
            sceneid = PropAttackSkillInfo.m_LifeTarget.SceneID;//m_ConditionSkillTarget[PropAttackSkillInfo.m_type].SceneID;
            m_AttackTarget = PropAttackSkillInfo.m_LifeTarget;//m_ConditionSkillTarget[PropAttackSkillInfo.m_type];
        } else {
        
            /*if(!m_parent.run.CheckHaveIdleAttackPosInPath(m_parent.SceneID))
            {
            	m_ConditionSkillTarget[PropAttackSkillInfo.m_type] = null;
            	SkillInfo lskill = GetLSkill();
            	if (lskill != null)
            	{
            		result = GetSkillTarget(lskill);
            		if (result)
            		{
            			sceneid = m_ConditionSkillTarget[lskill.m_type].SceneID;
            			m_AttackTarget = m_ConditionSkillTarget[lskill.m_type];
            			PropAttackSkillInfo = lskill;
            		}
            		else
            		{
            			m_ConditionSkillTarget[lskill.m_type] = null;
            			m_AttackTarget = null;
            			sceneid = 0;
            		}
            	}
            	else
            	{
            		sceneid = 0;
            	}
            }
            else
            {
            	m_AttackTarget = null;
            	sceneid = 0;
            }*/
            
        }
        return result;
    }
    //获取主动攻击目标
    public override bool GetBigAttackTarget()
    {
        SoldierSkill bskill = GetBigSkill();
        if (bskill == null) {
            return false;
        }
        bool result = GetSkillTarget(bskill);
        if (result) {
            m_SkillTarget = bskill.m_LifeTarget;//m_ConditionSkillTarget[bskill.m_type];
        }
        return result;
    }
    
    static public List<Life> GetRangeAttackList(MapGrid pos, SkillInfo skill, LifeMCamp camp, Life targetAttack, Role attacker)
    {
        List<Life> targetlist = new List<Life>();
        SoldierSkill soldierskill = skill as SoldierSkill;
        //Debug.Log (skill.m_target);
        //Debug.Log ((skill as SoldierSkill).m_sort);
        if (skill.m_target == (int)SkillTarget.Self) {
            /*if(skill.m_target == 3)
            {
                CM.SearchLifeMListInBoat(ref targetlist, LifeMType.SOLDIER | LifeMType.SUMMONPET, attacker.m_Core.m_Camp);
            	//if (targetlist.Contains (m_SkillOwner))
            	//	targetlist.Remove (m_SkillOwner);
            }
            else*/
            targetlist.Add(attacker);
        } else if (skill.m_target == (int)SkillTarget.EnemyTeam || skill.m_target == (int)SkillTarget.FriendlyTeam) {
            if ((soldierskill.m_damagetargettype & (int)TargetType.Soldier) == (int)TargetType.Soldier) {
                CM.SearchLifeMListInBoat(ref targetlist, LifeMType.SOLDIER, camp);
            }
            List<Life> l = new List<Life>();
            if ((soldierskill.m_damagetargettype & (int)TargetType.Pet) == (int)TargetType.Pet) {
                CM.SearchLifeMListInBoat(ref l, LifeMType.SUMMONPET, camp);
                targetlist.AddRange(l);
            }
            if ((soldierskill.m_damagetargettype & (int)TargetType.Gold) == (int)TargetType.Gold || (soldierskill.m_damagetargettype & (int)TargetType.Trap) == (int)TargetType.Trap) {
                List<Life> lb = new List<Life>();
                CM.SearchLifeMListInBoat(ref lb, LifeMType.BUILD, camp);
                foreach (Building b in lb) {
                    if (b.m_Attr.IsDamage) {
                        if (b.m_roomType == RoomType.ResRoom && (soldierskill.m_damagetargettype & (int)TargetType.Gold) == (int)TargetType.Gold) {
                            targetlist.Add(b);
                        } else if (b.m_roomType != RoomType.ResRoom && (soldierskill.m_damagetargettype & (int)TargetType.Trap) == (int)TargetType.Trap) {
                            targetlist.Add(b);
                        }
                    }
                }
            }
        }
        //0=敌人所有炮弹兵和金库，2=对地使用,4=已方中的一个(multiple 要匹配单体)，5=敌方所有炮弹兵
        else if (skill.m_target == (int)SkillTarget.Enemy || skill.m_target == (int)SkillTarget.EnemyGround ||
            skill.m_target == (int)SkillTarget.SomeOneFriendly || skill.m_target == (int)SkillTarget.FriendlyGround) {
            int nSort = (skill as SoldierSkill).m_sort;
            
            float radus = (skill as SoldierSkill).m_range;
            if (nSort == 1 || nSort == 0) {
                if (targetAttack != null) {
                    targetlist.Add(targetAttack);
                }
            } else {
                //LifeMCamp camp = GetSkillCamp(skill, m_SkillOwner);
                if ((soldierskill.m_damagetargettype & (int)TargetType.Soldier) == (int)TargetType.Soldier) {
                    List<Life> lr = new List<Life>();
                    CM.SearchLifeMListInBoat(ref lr, LifeMType.SOLDIER, camp);
                    foreach (Role r in lr) {
                        if (CheckCanAttack(r.CurrentAction) && CheckRangeAttackTarget(r, pos, nSort, radus, attacker.WalkDir)) {
                            targetlist.Add(r);
                        }
                    }
                }
                if ((soldierskill.m_damagetargettype & (int)TargetType.Pet) == (int)TargetType.Pet) {
                    List<Life> lp = new List<Life>();
                    CM.SearchLifeMListInBoat(ref lp, LifeMType.SUMMONPET, camp);
                    foreach (Role r in lp) {
                        if (CheckCanAttack(r.CurrentAction) && CheckRangeAttackTarget(r, pos, nSort, radus, attacker.WalkDir)) {
                            targetlist.Add(r);
                        }
                    }
                }
                if ((soldierskill.m_damagetargettype & (int)TargetType.Gold) == (int)TargetType.Gold || (soldierskill.m_damagetargettype & (int)TargetType.Trap) == (int)TargetType.Trap) {
                    List<Life> lb = new List<Life>();
                    CM.SearchLifeMListInBoat(ref lb, LifeMType.BUILD, camp);
                    foreach (Building b in lb) {
                        if (b.m_Attr.IsDamage) {
                            if (b.m_Attr.IsResource) {
                                if ((soldierskill.m_damagetargettype & (int)TargetType.Gold) == (int)TargetType.Gold && CheckRangeAttackTarget(b, pos, nSort, radus, attacker.WalkDir)) {
                                    targetlist.Add(b);
                                }
                            } else {
                            
                                if ((soldierskill.m_damagetargettype & (int)TargetType.Trap) == (int)TargetType.Trap && CheckRangeAttackTarget(b, pos, nSort, radus, attacker.WalkDir)) {
                                    targetlist.Add(b);
                                }
                            }
                        }
                        /*if ((b.m_roomType == RoomType.ResRoom || ((skill as SoldierSkill).m_trapbreach == 1 && (b.m_roomType == RoomType.DeckTrap || b.m_roomType == RoomType.NormalTrap)))
                            && b.m_Attr.IsDamage && CheckRangeAttackTarget(b, pos, nSort, radus, attacker.WalkDir))
                        	targetlist.Add(b);*/
                    }
                }
                /*if (skill.m_target == 0 || skill.m_target == 2 || skill.m_target == 8)
                {
                    List<Life> lb = new List<Life>();
                    CM.SearchLifeMListInBoat(ref lb, LifeMType.BUILD, camp);
                    foreach (Building b in lb)
                    {
                
                        if ((b.m_roomType == RoomType.ResRoom || ((skill as SoldierSkill).m_trapbreach == 1 && (b.m_roomType == RoomType.DeckTrap || b.m_roomType == RoomType.NormalTrap)))
                            && b.m_Attr.IsDamage && CheckRangeAttackTarget(b, pos, nSort, radus, attacker.WalkDir))
                            targetlist.Add(b);
                    }
                }*/
                
                if (nSort == 2 && skill.m_type != 5009) { //线性攻击，如蹦大大招 天雨散弹
                    targetlist = GetNearestAttackList(attacker, targetlist);
                }
                CheckMultiple(ref targetlist, skill);
            }
        }
        return targetlist;
    }
    
    static private  List<Life> GetNearestAttackList(Role attacker, List<Life> l)
    {
        int grid = attacker.GetMapGrid().GridPos.Unit;
        List<Life> nearestL = new List<Life>();
        Life lifeM = null;
        int temp = int.MaxValue;
        foreach (var i in l) {
            if (attacker.WalkDir == WalkDir.WALKLEFT) {
                if (i.GetMapGrid().GridPos.Unit <= grid  && Mathf.Abs(i.GetMapGrid().GridPos.Unit - grid) < temp) {
                    lifeM = i;
                    temp = Mathf.Abs(i.GetMapGrid().GridPos.Unit - grid);
                }
            } else {
                if (i.GetMapGrid().GridPos.Unit >= grid  && Mathf.Abs(i.GetMapGrid().GridPos.Unit - grid) < temp) {
                    lifeM = i;
                    temp = Mathf.Abs(i.GetMapGrid().GridPos.Unit - grid);
                }
            }
        }
        if (lifeM != null) {
            nearestL.Add(lifeM);
        }
        return nearestL;
    }
    
    
    
    private bool GetAttackList(SoldierSkill skill, bool isbig = false)
    {
        m_AttackList.Clear();
        int distant = skill.m_distance / MapGrid.m_Pixel;
        LifeMCamp camp = GetSkillCamp(skill, m_SkillOwner);
        if (skill.m_condition != (int)SkillCondition.HaveDoor) {
            /*if (skill.m_distance > 0 && skill.m_target != 4 && (skill.m_target != 5 || m_parent.Target is Role))
            {
            	if (m_parent.Target != null && !m_parent.Target.isDead)
            	{
            		if (m_parent.m_Core.m_Camp != camp && CheckAttackTarget(m_parent.Target,skill.m_sort,distant))
            			m_AttackList.Add(m_parent.Target);
            		else
            		{
            			if (!m_parent.run.CheckHaveIdleAttackPosInPath(m_parent.SceneID))
            			{
            				//m_parent.run.Pass = false;
            				int ldistant = skill.m_ldistance/MapGrid.m_Pixel;
            				if (ldistant > 0 && CheckAttackTarget(m_parent.Target,skill.m_sort,ldistant))
            					m_AttackList.Add(m_parent.Target);
            			}
            			else
            			{
            				//m_parent.run.Pass = true;
            			}
            		}
            	}
            }
            else*/
            {
                //if (skill.m_condition != (int)SkillCondition.TrapAttack && skill.m_condition != (int)SkillCondition.AttackBuild && skill.m_target != 6)
                if ((skill.m_targettype & (int)TargetType.Soldier) == (int)TargetType.Soldier) {
                    List<Life> l = new List<Life>();
                    ///需要排除隐形 。。。。。。。。。。。。。。。。。。
                    CM.SearchLifeMListInBoat(ref l, LifeMType.SOLDIER, camp);
                    foreach (Role r in l) {
                        if (skill.m_type == 1039 && r.m_Status.HaveState(StatusType.DieMark)) {
                            m_AttackList.Clear();
                            m_AttackList.Add(r);
                            return true;
                        }
                        
                        if (m_SkillOwner.m_Attr.IsHide && !isbig) {
                            if (m_SkillOwner.Target != null && !m_SkillOwner.Target.isDead && CheckAttackTarget(m_SkillOwner.Target, skill.m_sort, distant)) {
                                m_AttackList.Add(m_SkillOwner.Target);
                            }
                        }//因为小白的治愈之光技能，所有注释掉
                        else if (/*!(skill.m_target == 4 && r == m_parent) &&*/ !r.m_Attr.IsHide && CheckCanAttack(r.CurrentAction, skill.m_distance == 0) &&
                            CheckAttackTarget(r, skill.m_sort, distant, skill.m_condition != (int)SkillCondition.SameLayer) &&
                            (skill.m_condition != (int)SkillCondition.NoFullHpNoSelf || (r.HP < r.fullHP && r.SceneID != m_SkillOwner.SceneID))) {
                            m_AttackList.Add(r);
                        }
                    }
                }
                if ((skill.m_targettype & (int)TargetType.Pet) == (int)TargetType.Pet) {
                    List<Life> lp = new List<Life>();
                    CM.SearchLifeMListInBoat(ref lp, LifeMType.SUMMONPET, camp);
                    foreach (Life p in lp) {
                    
                        if (m_SkillOwner.m_Attr.IsHide && !isbig) {
                            if (m_SkillOwner.Target != null && !m_SkillOwner.Target.isDead && CheckAttackTarget(m_SkillOwner.Target, skill.m_sort, distant)) {
                                m_AttackList.Add(m_SkillOwner.Target);
                            }
                        } else if (CheckAttackTarget(p, skill.m_sort, distant, skill.m_condition != (int)SkillCondition.SameLayer) &&
                            (skill.m_condition != (int)SkillCondition.NoFullHpNoSelf || (p.HP < p.fullHP && p.SceneID != m_SkillOwner.SceneID))) {
                            m_AttackList.Add(p);
                        }
                    }
                }
                /*if (skill.m_target == 0 || skill.m_target == 6)
                {
                	if (skill.m_target != 5 && (skill.m_condition != (int)SkillCondition.NoFullHpNoSelf || skill.m_target == 6))
                	{*/
                if ((skill.m_targettype & (int)TargetType.Trap) == (int)TargetType.Trap  || (skill.m_targettype & (int)TargetType.Gold) == (int)TargetType.Gold) {
                    List<Life> lb = new List<Life>();
                    CM.SearchLifeMListInBoat(ref lb, LifeMType.BUILD, camp);
                    foreach (Building b in lb) {
                    
                        if (m_SkillOwner.m_Attr.IsHide && !isbig) {
                            if (m_SkillOwner.Target != null && !m_SkillOwner.Target.isDead && CheckAttackTarget(m_SkillOwner.Target, skill.m_sort, distant)) {
                                m_AttackList.Add(m_SkillOwner.Target);
                            }
                        } else {
                            if (b.m_Attr.IsDamage) {
                                if (b.m_Attr.IsResource) {
                                    if ((skill.m_targettype & (int)TargetType.Gold) == (int)TargetType.Gold  && CheckAttackTarget(b, 1, distant)) {
                                        m_AttackList.Add(b);
                                    }
                                } else if ((skill.m_targettype & (int)TargetType.Trap) == (int)TargetType.Trap) {
                                    if (skill.m_condition == 0) {
                                        if (CheckAttackTarget(b, 1, distant)) {
                                            m_AttackList.Add(b);
                                        }
                                    } else if ((skill.m_condition == (int)SkillCondition.TrapAttack && (skill.m_condition_data0 == b.m_Attr.Type || skill.m_condition_data1 == b.m_Attr.Type) && !b.IsInCDStatus()) ||
                                        (skill.m_condition == (int)SkillCondition.AttackBuild && (skill.m_condition_data0 == b.m_Attr.Type || skill.m_condition_data1 == b.m_Attr.Type))) {
                                        if (CheckAttackTarget(b, 1, distant)) {
                                            m_AttackList.Add(b);
                                        }
                                    }
                                }
                            }
                            /*	if(skill.m_target == 6 )
                            {
                            	if (!b.m_Attr.IsResource&& b.m_Attr.IsDamage && CheckAttackTarget(b,1,distant))
                            		m_AttackList.Add(b);
                            }
                            else if (
                            	(
                            	(skill.m_condition != (int)SkillCondition.TrapAttack && skill.m_condition != (int)SkillCondition.AttackBuild && b.m_roomType == RoomType.ResRoom) ||
                            	(skill.m_condition == (int)SkillCondition.TrapAttack && (skill.m_data0 == b.m_Attr.Type|| skill.m_data1 == b.m_Attr.Type)&&!b.IsInCDStatus()) ||
                            	(skill.m_condition == (int)SkillCondition.AttackBuild && (skill.m_data0 == b.m_Attr.Type|| skill.m_data1 == b.m_Attr.Type))
                            	)
                                     && b.m_Attr.IsDamage && CheckAttackTarget(b,1,distant)
                            	)
                            	m_AttackList.Add(b);
                            }*/
                        }
                    }
                    
                    /*if (skill.m_condition != (int)SkillCondition.TrapAttack)
                    {
                    	List<Life> lr = new List<Life>();
                    	CM.SearchLifeMListInBoat(ref lr,LifeMType.FLOOR, camp);
                    	foreach(Floor w in lr)
                    	{
                    		if (CheckAttackTarget(w,1,distant) )
                    			m_AttackList.Add(w);
                    	}
                    }*/
                }
            }
            if (skill.m_distance > 0) {
                if (m_AttackList.Contains(m_SkillOwner.Target)) {
                    m_AttackList.Clear();
                    m_AttackList.Add(m_SkillOwner.Target);
                }
            }
        }
        
        /*if (skill.m_target != 5 && skill.m_condition != (int)SkillCondition.TrapAttack)
        {
        	List<Life> lw = new List<Life>();
        	CM.SearchLifeMListInBoat(ref lw,LifeMType.WALL, camp);
        	foreach(Wall w in lw)
        	{
        		if (CheckAttackTarget(w,1,distant) && !w.m_Attr.DoorState)
        			m_AttackList.Add(w);
        	}
        }*/
        if (m_SkillOwner.m_Attr.Charmed && skill.m_target != 1) {
            if (m_AttackList.Contains(m_SkillOwner)) {
                m_AttackList.Remove(m_SkillOwner);
            }
        }
        //}
        //CM.SearchAttackLifeMList(ref m_AttackList,m_SkillOwner);
        //parent.write(str + Time.realtimeSinceStartup);
        
        return true;
    }
    
    private Life GetBestAttackTarget(SoldierSkill skill)
    {
        if (m_AttackList == null || m_AttackList.Count == 0) {
            return null;
        }
        
        /*if (m_AttackList.Count == 1)
        	return m_AttackList [0];*/
        
        
        Life m = null;
        List<Life> l = new List<Life>();
        //CM.SearchAttackLifeMList(ref m_AttackList,m_SkillOwner);
        if (m_AttackList.Count > 0) {
        
        
            l = GetSkillTarget(m_AttackList, skill);
            if (l.Count == 1) {
                m = l[0];
            }
            if (m != null) {
                return m;
            }
            if (l.Count == 0) {
                return null;
            }
            //if (l.Count > 1)
            //{
            m_AttackList  = l;
            //}
            /*else
            	return null;*/
            //目标一致优先
            /*m = GetAttack2Target();
            if(m != null) return m;
            
            //目标一致优先
            m = GetAttackTargetLock();
            if(m != null) return m;*/
            //距离最近
            m = GetShortAttackTarget(skill);
            if (m != null) {
                return m;
            }
        }
        
        return null;
    }
    
    
    //目标做为攻击目标
    private Life GetAttack2Target()
    {
    
        foreach (Life  m in m_AttackList) {
            if (m  == m_AttackTarget) {
                return m;
            }
        }
        
        return null;
    }
    
    
    //锁定目标
    private Life GetAttackTargetLock()
    {
        foreach (Life  m in m_AttackList) {
            if (m == m_AttackTarget) {
                return m;
            }
        }
        
        return null;
    }
    
    //获取距离最近的目标
    private Life GetShortAttackTarget(SoldierSkill skill)
    {
        int ii = 0;
        float dis = 0;
        if (skill.m_distance > 0) {
            dis = Life.CalcDistance(m_AttackList[0], m_SkillOwner);
        } else {
            dis = Life.CalcLineDistance(m_AttackList[0], m_SkillOwner);
        }
        string str = "GetShortAttackTarget" + m_AttackList[0]  + "," + dis + "," + m_AttackList[0].m_thisT.localPosition + "," + MapGrid.GetMG(m_AttackList[0].GetMapPos()).pos ;
        for (int i = 1; i < m_AttackList.Count; i++) {
            float d = 0;
            if (skill.m_distance > 0) {
                d = Life.CalcDistance(m_AttackList[i], m_SkillOwner);
            } else {
                d = Life.CalcLineDistance(m_AttackList[i], m_SkillOwner);
            }
            str += "," + m_AttackList[i] + "," + d + "," + m_AttackList[i].m_thisT.localPosition + "," + MapGrid.GetMG(m_AttackList[i].GetMapPos()).pos;
            if (dis > d) {
                dis  = d;
                ii = i;
            }
        }
        
        //parent.write(str );
        return m_AttackList [ii];
    }
    
    public void DoQianYaoStatus()
    {
        Life target = PropSkillInfo.m_LifeTarget;//m_ConditionSkillTarget[PropSkillInfo.m_type];
        //buff 给自己增加状态
        StatusSelfBuff(m_SkillOwner, PropSkillInfo);
        if (GetBigSkill() != null && m_skill.m_type != GetBigSkill().m_type) {
            m_SkillOwner.AddAnger(AngerStyle.SkillLaunch, null);
        }
        if (target != null) {
            //debuff 给对方增加状态
            if (target.m_Core.m_Camp == m_SkillOwner.m_Core.m_Camp)
            
            {
                StatusReleaseBuff(target);
            } else {
                StatusReleaseDeBuff(target);
            }
        }
    }
    
    
    
    
    /*public void DoConditionSkill(int count)
    {
    	Life targetAttack = PropSkillInfo.m_LifeTarget;
    	MapGrid pos = PropSkillInfo.m_TargetPos;
    	if ( targetAttack == null && pos == null)
    		return;
    	bool interrupt = false;
    	List<Life> targetlist = new List<Life>();
    	LifeMCamp camp = GetSkillCamp(PropSkillInfo,m_SkillOwner);
    	targetlist = RoleSkill.GetRangeAttackList(pos,PropSkillInfo,camp,targetAttack, m_SkillOwner);
    	/*
    	 if (PropSkillInfo.m_target == 3)
    	{
    		CM.SearchLifeMListInBoat(ref targetlist ,LifeMType.SOLDIER, m_SkillOwner.m_Core.m_Camp);
    		if (targetlist.Contains(m_SkillOwner))
    			targetlist.Remove(m_SkillOwner);
    	}
    	else if ((PropSkillInfo as SoldierSkill).m_sort != 1 )
    	{
    		LifeMCamp camp = GetSkillCamp(PropSkillInfo,m_SkillOwner);
    		targetlist = GetRangeAttackList(pos,PropSkillInfo,camp,targetAttack);
    	}
    	if (targetAttack != null && !targetlist.Contains(targetAttack))
    		targetlist.Add(targetAttack);
    	if ((PropSkillInfo.m_AttributeType & AttributeType.Electric) == AttributeType.Electric)
    	{
    		GetWetTarget(ref targetlist,PropSkillInfo,m_SkillOwner);
    	}
    
    	(m_SkillOwner.CurrentAction as GridActionSkill).RangeTarget(targetlist);
    	if ((PropSkillInfo as SoldierSkill).m_actiontype == 5)
    	{
    		StatusSelfBuff(m_SkillOwner,PropSkillInfo);
    		PetInfo Info = CmCarbon.GetPetInfo((PropSkillInfo as SoldierSkill).m_data2);
    		if(Info == null) return ;
    		int PetType = Info.m_type;
    		int ModeType = PetType;
    		CreatePet(PetType ,ModeType,Info.m_id,Info);
    	}
    	else if ((PropSkillInfo as SoldierSkill).m_actiontype == 0 || (PropSkillInfo as SoldierSkill).m_actiontype == 1)
    	{
    		//if (PropSkillInfo.m_type != 1053)
    			StatusSelfBuff(m_SkillOwner,PropSkillInfo);
    		for(int i =0; i < targetlist.Count; i++)
    		{
    				StatusSelfBuff(m_SkillOwner,PropSkillInfo);
    
    			if (!targetlist[i].InBoat)
    				continue;
    			if (m_SkillOwner.Target == targetlist[i])
    			{
    				#if UNITY_EDITOR_LOG
    				FileLog.write(m_SkillOwner.SceneID, "TargetAttacked " + m_SkillOwner.TargetAttacked + "," + m_SkillOwner.m_TargetDuration);
    				#endif
    			}
    			SkillReleaseInfo info = Life.CalcDamage(m_SkillOwner,targetlist[i],null,PropSkillInfo);
    			if (PropSkillInfo.m_type == 1030)
    			{
    				NGUIUtil.DebugLog(info.Result.ToString(),"red");
    			}
    			if (PropSkillInfo.m_attckmodeid > 0)
    			{
    				float power = PropSkillInfo.GetAttackPower(0);
    				info.m_Damage = (int)(info.m_Damage * power);
    			}
    			Transform attackT = m_SkillOwner.m_thisT;
    			if (!targetlist[i].ApplyDamage (info, attackT) && !(targetlist[i] is Wall))
    				m_SkillOwner.AddAnger(AngerStyle.TargetDead,targetlist[i]);
    		}
    	}
    
    }*/
    private InheritSummonPros CreateInheritPros(int ID, MapGrid pos)
    {
    
    
        PetInfo info = CmCarbon.GetPetInfo(ID);
        if (info == null) {
            return null;
        }
        InheritSummonPros pros = new InheritSummonPros();
        pros.CreateSkin(BattleEnvironmentM.GetLifeMBornNode(true), info.m_type, info.m_type.ToString(), AnimatorState.Empty, m_SkillOwner.m_Core.m_IsPlayer);
        //IGameRole i = GameRoleFactory.Create(BattleEnvironmentM.GetLifeMBornNode(true), info.m_type, info.m_type.ToString(), AnimatorState.Empty);
        GameObject go = pros.RoleSkinCom.tRoot.gameObject;
        LifeObj lo = go.AddComponent<LifeObj>();
        lo.SetLife(pros, pros.RoleSkinCom.ProPerty);
        pros.SetBorn(m_SkillOwner, ID, info, pos);
        go.transform.localPosition = pos.pos;
        pros.m_AppearAction = GridActionCmdISPApperAction.Create(m_SkillOwner, info.m_type, pos.pos);
        if (pros.m_AppearAction != null) {
            pros.m_AppearAction.SetTarget(pros);
        }
        return pros;
    }
    private SummonPros CreatePros(int ID, MapGrid pos)
    {
        SummonProsInfo info = SummonM.GetSummonProsInfo(ID);
        
        SummonPros pros = new SummonPros();
        pros.CreateSkin(BattleEnvironmentM.GetLifeMBornNode(true), info.m_modeltype, info.m_modeltype.ToString(), AnimatorState.Empty, m_SkillOwner.m_Core.m_IsPlayer);
        //IGameRole i = GameRoleFactory.Create(BattleEnvironmentM.GetLifeMBornNode(true), info.m_modeltype, info.m_modeltype.ToString(), AnimatorState.Empty);
        GameObject go = pros.RoleSkinCom.tRoot.gameObject;
        LifeObj lo = go.AddComponent<LifeObj>();
        lo.SetLife(pros, pros.RoleSkinCom.ProPerty);
        pros.SetBorn(m_SkillOwner, ID, info, pos);
        go.transform.localPosition = pos.pos;
        return pros;
    }
    private SummonPet CreatePet(int PetDataID)
    {
    
        MapGrid StartGrid = m_SkillOwner.m_target.GetMapGrid();
        SummonpetInfo info = SummonM.GetSummonPetInfo(PetDataID);
        if (info.m_type == 3001) {
            MapGrid g = m_SkillOwner.GetMapGrid();
            int dis = 6;
            if (m_SkillOwner.WalkDir == WalkDir.WALKLEFT) {
                for (int n = 0; n < dis; n++) {
                    if (g.Right != null) {
                        g = g.Right;
                    } else {
                        break;
                    }
                }
            } else if (m_SkillOwner.WalkDir == WalkDir.WALKRIGHT) {
                for (int n = 0; n < dis; n++) {
                    if (g.Left != null) {
                        g = g.Left;
                    } else {
                        break;
                    }
                }
            }
            StartGrid = g;
        }
        SummonPet pet = new SummonPet();
        pet.CreateSkin(BattleEnvironmentM.GetLifeMBornNode(true), info.m_modeltype, info.m_modeltype.ToString(), AnimatorState.Empty, m_SkillOwner.m_Core.m_IsPlayer);
        //IGameRole i = GameRoleFactory.Create(BattleEnvironmentM.GetLifeMBornNode(true), info.m_modeltype, info.m_modeltype.ToString(), AnimatorState.Empty);
        GameObject go = pet.RoleSkinCom.tRoot.gameObject;
        Vector3 pos = Vector3.zero;
        string posname = "";
        LifeObj lo = go.AddComponent<LifeObj>();
        pet.SetEnemyType(PetEnemyType.SOLDIER);
        pet.SetSummonPetLife(info, pet.RoleSkinCom.ProPerty, LifeEnvironment.Combat);
        pet.SetLifeCore(new LifeMCore(PetDataID, false, LifeMType.SUMMONPET, m_SkillOwner.m_Core.m_Camp, MoveState.Walk));
        pet.SetSkin();
        pet.SetBornPos(StartGrid.GridPos, 0);
        lo.SetLife(pet, pet.RoleSkinCom.ProPerty);
        go.transform.parent = BattleEnvironmentM.GetLifeMBornNode(true);
        Vector3 start = StartGrid.WorldPos;
        start.z = Camera.main.transform.position.z;
        go.transform.position = start;
        return pet;
    }
    
    
    /// <summary>
    /// 创建宠物
    /// </summary>
    private Pet CreatePet(int PetType, int ModeType, int PetDataID, PetInfo info)
    {
        if (ModeType != 1000 && ModeType != 1001 && ModeType != 1002 && ModeType != 1003) {
            return null;
        }
        Pet pet = new Pet();
        pet.CreateSkin(BattleEnvironmentM.GetLifeMBornNode(true), ModeType, ModeType.ToString(), AnimatorState.Empty, m_SkillOwner.m_Core.m_IsPlayer);
        //IGameRole i = GameRoleFactory.Create(BattleEnvironmentM.GetLifeMBornNode(true), ModeType, ModeType.ToString(), AnimatorState.Empty);
        GameObject go = pet.PetSkinCom.tRoot.gameObject;
        Vector3 pos = Vector3.zero;
        string posname = "";
        Int2 toGrid = m_SkillOwner.GetMapPos();
        
        if (PetType == 1002) {
            if (m_SkillOwner.CurPet == null) {
                LifeObj lo = go.AddComponent<LifeObj>();
                
                lo.SetLife(pet, pet.PetSkinCom.ProPerty);
                m_SkillOwner.CurPet = pet;
                GameObject posgo = m_SkillOwner.m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.petFollowPos);
                pos = posgo.transform.position;
                // pet.SetSkin(i);
                // (pet as Pet1002).m_petState = Pet1002State.Follow;
                
            } else {
                Object.Destroy(go);
            }
        } else if (PetType == 1003) {
            LifeObj lo = go.AddComponent<LifeObj>();
            lo.SetLife(pet, pet.PetSkinCom.ProPerty);
            pet.SetSkin();
            
        }
        
        //go.transform.position = pos;
        if (pet != null) {
            pet.SetBorn(m_SkillOwner, PetDataID, PetType, toGrid, m_SkillOwner.WalkDir);
            pet.SetSkin();
        }
        pet.Info = info;
        //pet.SetMoveState(MoveState.Walk);
        return pet;
    }
    public  void DoSkill(SkillInfo skill, int times)
    {
    
        //if (skill.m_type == 1009 || skill.m_type == 1028)
        //	NGUIUtil.DebugLog( "doskill " + skill.m_type  + "," +  m_ConditionSkillTarget[skill.m_type] + "," + skill.m_name,"red");
        if (m_SkillOwner == null) {
            return ;
        }
        
        Life targetAttack = skill.m_LifeTarget;//m_ConditionSkillTarget[skill.m_type];
        MapGrid pos = skill.m_TargetPos;//m_ConditionSkillTargetPos[skill.m_type];
        if (targetAttack == null && pos == null) {
            return;
        }
        bool interrupt = false;
        List<Life> targetlist = new List<Life>();
        LifeMCamp camp = GetSkillCamp(skill, m_SkillOwner);
        targetlist = RoleSkill.GetRangeAttackList(pos, skill, camp, targetAttack, m_SkillOwner);
        
        if ((skill.m_AttributeType & AttributeType.Electric) == AttributeType.Electric) {
            GetWetTarget(ref targetlist, skill, m_SkillOwner);
        }
        if (m_SkillOwner.CurrentAction is GridActionSkill) {
            (m_SkillOwner.CurrentAction as GridActionSkill).RangeTarget(targetlist);
        }
        
        SkillUse(skill, times, targetlist);
        
    }
    //为了贯穿伤害，贯穿伤害tagert要从外面传
    public void SkillUse(SkillInfo skill, int times, List<Life> targetlist)
    {
        Life targetAttack = skill.m_LifeTarget;
        MapGrid pos = skill.m_TargetPos;
#if UNITY_EDITOR_LOG
        string str = m_SkillOwner.SceneID + "," + skill.m_name + "," + targetlist.Count + ": attack  ";
#endif
        if ((skill as SoldierSkill).m_actiontype == 0) {
            //StatusSelfBuff(m_SkillOwner,skill);
            for (int i = 0; i < targetlist.Count; i++) {
                if (targetlist[i] == null) {
                    Debug.Log("eeeeeeeee");
                }
                if (!targetlist[i].InBoat || targetlist[i].isDead) {
                    continue;
                }
                if (m_SkillOwner.Target == targetlist[i]) {
                    m_SkillOwner.TargetAttacked = true;
#if UNITY_EDITOR_LOG
                    FileLog.write(m_SkillOwner.SceneID, "TargetAttacked " + m_SkillOwner.TargetAttacked + "," + m_SkillOwner.m_TargetDuration);
#endif
                }
                SkillReleaseInfo info = Life.CalcDamage(m_SkillOwner, targetlist[i], null, skill);
                if (skill.m_type == 1030) {
                    NGUIUtil.DebugLog(info.Result.ToString(), "red");
                }
                if (skill.m_attckmodeid > 0) {
                    float power = skill.GetAttackPower(times - 1);
                    info.m_Damage = (int)(info.m_Damage * power);
                }
                Transform attackT = m_SkillOwner.m_thisT;
                int hp =  targetlist[i].HP;
                if (!targetlist[i].ApplyDamage(info, attackT) && !(targetlist[i] is IggWall) && hp > 0) {
                    m_SkillOwner.AddAnger(AngerStyle.TargetDead, targetlist[i]);
                }
#if UNITY_EDITOR_LOG
                str += targetlist[i].SceneID + ",  ";
#endif
            }
            //Debug.Log(str);
        } else if ((skill as SoldierSkill).m_actiontype == 1) {
            if (times > 1) {
                StatusSelfBuff(m_SkillOwner, skill);
            }
            for (int i = 0; i < targetlist.Count; i++) {
                if (!targetlist[i].InBoat || targetlist[i].isDead) {
                    continue;
                }
                if (m_SkillOwner.Target == targetlist[i]) {
                    m_SkillOwner.TargetAttacked = true;
#if UNITY_EDITOR_LOG
                    FileLog.write(m_SkillOwner.SceneID, "TargetAttacked " + m_SkillOwner.TargetAttacked + "," + m_SkillOwner.m_TargetDuration);
#endif
                }
                SkillReleaseInfo info = Life.CalcDamage(m_SkillOwner, targetlist[i], null, skill);
                if (skill.m_attckmodeid > 0) {
                    float power = skill.GetAttackPower(times);
                    info.m_Damage = (int)(info.m_Damage * power);
                }
                Transform attackT = m_SkillOwner.m_thisT;
                if (!targetlist[i].ApplyDamage(info, attackT) && !(targetlist[i] is IggWall)) {
                    m_SkillOwner.AddAnger(AngerStyle.TargetDead, targetlist[i]);
                }
#if UNITY_EDITOR_LOG
                str += targetlist[i].SceneID + ",  ";
#endif
            }
        } else if ((skill as SoldierSkill).m_actiontype == 7) {
            if ((skill as SoldierSkill).m_data0 == 1) {
                int PedNum = (skill as SoldierSkill).m_data2;
                if (PedNum == 0) {
                    PedNum = 1;
                }
                for (int i = 0 ; i < PedNum; i++) {
                    SummonPet pet = CreatePet((skill as SoldierSkill).m_data1);
                    if (pet == null) {
                        continue;
                        //----------------------------
                        //if(m_SkillOwner.CurrentAction is GridActionCmd200008Skill1101)
                        {
                            m_SkillOwner.CurrentAction.SetDone();
                        }
                        break;
                    }
                }
            } else if ((skill as SoldierSkill).m_data0 == 2) {
                int PedNum = (skill as SoldierSkill).m_data2;
                if (PedNum == 0) {
                    PedNum = 1;
                }
                for (int i = 0 ; i < PedNum; i++) {
                    SummonPros pros = CreatePros((skill as SoldierSkill).m_data1, targetAttack.GetMapGrid());
                }
            }
            
        } else if ((skill as SoldierSkill).m_actiontype == 5) {
            StatusSelfBuff(m_SkillOwner, skill);
            for (int i = 0; i < targetlist.Count; i++) {
                if (!targetlist[i].InBoat) {
                    continue;
                }
                if (m_SkillOwner.Target == targetlist[i]) {
                    m_SkillOwner.TargetAttacked = true;
#if UNITY_EDITOR_LOG
                    FileLog.write(m_SkillOwner.SceneID, "TargetAttacked " + m_SkillOwner.TargetAttacked + "," + m_SkillOwner.m_TargetDuration);
#endif
                }
                SkillReleaseInfo info = Life.CalcDamage(m_SkillOwner, targetlist[i], null, skill);
                if (skill.m_type == 1030) {
                    NGUIUtil.DebugLog(info.Result.ToString(), "red");
                }
                if (skill.m_attckmodeid > 0) {
                    float power = skill.GetAttackPower(times);
                    info.m_Damage = (int)(info.m_Damage * power);
                }
                Transform attackT = m_SkillOwner.m_thisT;
                if (!targetlist[i].ApplyDamage(info, attackT) && !(targetlist[i] is IggWall)) {
                    m_SkillOwner.AddAnger(AngerStyle.TargetDead, targetlist[i]);
                }
            }
            
            int PedNum = (skill as SoldierSkill).m_data3;
            if (PedNum == 0) {
                PedNum = 1;
            }
            for (int i = 0 ; i < PedNum; i++) {
                PetInfo Info = CmCarbon.GetPetInfo((skill as SoldierSkill).m_data2);
                if (Info == null) {
                    continue;
                }
                
                int PetType = Info.m_type;
                int ModeType = PetType;
                Debug .Log("CreatePet  " + PetType);
                
                Pet pet = CreatePet(PetType, ModeType, Info.m_id, Info);
                
                pet.m_thisT.localPosition = pos.pos;
                if (targetAttack != null && targetAttack is Role) {
                    pet.m_thisT.localPosition = targetAttack.m_thisT.localPosition;
                }
            }
            
            /*GameObject go = GameObjectLoader.LoadPath("Prefabs/Roles/",skill.m_data2.ToString(),BattleEnvironmentM.GetLifeMBornNode(true));
            go.transform.localPosition = m_parent.transform.localPosition;
            Pet pet = go.GetComponent<Pet>();
            if (pet != null)
            	pet.SetBorn(m_parent,skill.m_data2,m_parent.GetMapPos(),m_parent.WalkDir);*/
        } else if ((skill as SoldierSkill).m_actiontype == 6) {
            for (int i = 0; i < targetlist.Count; i++) {
                if (!targetlist[i].InBoat || targetlist[i].isDead) {
                    continue;
                }
                if (m_SkillOwner.Target == targetlist[i]) {
                    m_SkillOwner.TargetAttacked = true;
#if UNITY_EDITOR_LOG
                    FileLog.write(m_SkillOwner.SceneID, "TargetAttacked " + m_SkillOwner.TargetAttacked + "," + m_SkillOwner.m_TargetDuration);
#endif
                }
                SkillReleaseInfo info = Life.CalcDamage(m_SkillOwner, targetlist[i], null, skill);
                if (!info.m_bImmunity) {
                    if ((skill as SoldierSkill).m_data0 == 19) {
                        m_SkillOwner.Anger += (skill as SoldierSkill).m_data1;
                        m_SkillOwner.StatusUpdateAnger((skill as SoldierSkill).m_data1);
                    }
                }
                if (skill.m_type == 1030) {
                    NGUIUtil.DebugLog(info.Result.ToString(), "red");
                }
                if (skill.m_attckmodeid > 0) {
                    float power = skill.GetAttackPower(times - 1);
                    info.m_Damage = (int)(info.m_Damage * power);
                }
                Transform attackT = m_SkillOwner.m_thisT;
                if (!targetlist[i].ApplyDamage(info, attackT) && !(targetlist[i] is IggWall)) {
                    m_SkillOwner.AddAnger(AngerStyle.TargetDead, targetlist[i]);
                }
#if UNITY_EDITOR_LOG
                str += targetlist[i].SceneID + ",  ";
#endif
            }
        } else if ((skill as SoldierSkill).m_actiontype == 9) {
            StatusSelfBuff(m_SkillOwner, skill);
            for (int i = 0; i < targetlist.Count; i++) {
                if (!targetlist[i].InBoat) {
                    continue;
                }
                if (m_SkillOwner.Target == targetlist[i]) {
                    m_SkillOwner.TargetAttacked = true;
#if UNITY_EDITOR_LOG
                    FileLog.write(m_SkillOwner.SceneID, "TargetAttacked " + m_SkillOwner.TargetAttacked + "," + m_SkillOwner.m_TargetDuration);
#endif
                }
                SkillReleaseInfo info = Life.CalcDamage(m_SkillOwner, targetlist[i], null, skill);
                if (skill.m_type == 1030) {
                    NGUIUtil.DebugLog(info.Result.ToString(), "red");
                }
                if (skill.m_attckmodeid > 0) {
                    float power = skill.GetAttackPower(times);
                    info.m_Damage = (int)(info.m_Damage * power);
                }
                Transform attackT = m_SkillOwner.m_thisT;
                if (!targetlist[i].ApplyDamage(info, attackT) && !(targetlist[i] is IggWall)) {
                    m_SkillOwner.AddAnger(AngerStyle.TargetDead, targetlist[i]);
                }
            }
            
            int PedNum = (skill as SoldierSkill).m_data2;
            if (PedNum == 0) {
                PedNum = 1;
            }
            for (int i = 0; i < PedNum; i++) {
                MapGrid g = pos;//m_SkillOwner.GetMapGrid();
                if (targetAttack != null) {
                    g = targetAttack.GetMapGrid();
                }
                /*int n = (skill as SoldierSkill).m_distance / MapGrid.m_Pixel;
                if (m_SkillOwner.WalkDir == WalkDir.WALKLEFT)
                {
                	for (int k = 0; k < n; k++)
                	{
                		if (g.Left != null)
                			g = g.Left;
                		else
                			break;
                	}
                }
                else
                {
                	for (int k = 0; k < n; k++)
                	{
                		if (g.Right != null)
                			g = g.Right;
                		else
                			break;
                	}
                }*/
                InheritSummonPros pros = CreateInheritPros((skill as SoldierSkill).m_data1, g);
            }
            
            /*GameObject go = GameObjectLoader.LoadPath("Prefabs/Roles/",skill.m_data2.ToString(),BattleEnvironmentM.GetLifeMBornNode(true));
            go.transform.localPosition = m_parent.transform.localPosition;
            Pet pet = go.GetComponent<Pet>();
            if (pet != null)
            	pet.SetBorn(m_parent,skill.m_data2,m_parent.GetMapPos(),m_parent.WalkDir);*/
        }
        EventCenter.DoEvent(NDEventType.StatusInterrupt, m_SceneID, LifeAction.Attack);
    }
    static public void GlobalUseSkill(Role attacker, Role victim, SkillInfo skill, Vector3 worldPosSkill)
    {
        Life targetAttack = victim;//m_ConditionSkillTarget[skill.m_type];
        MapGrid pos = victim.GetMapGrid();//m_ConditionSkillTargetPos[skill.m_type];
        if (targetAttack == null && pos == null) {
            return;
        }
        
        List<Life> targetlist = new List<Life>();
        LifeMCamp camp = RoleSkill.GetSkillCamp(skill, attacker as Life);
        targetlist = RoleSkill.GetRangeAttackList(pos, skill, camp, targetAttack, attacker);
        
        if ((skill.m_AttributeType & AttributeType.Electric) == AttributeType.Electric) {
            GetWetTarget(ref targetlist, skill, attacker);
        }
        if (victim.CurrentAction is GridActionSkill) {
            (victim.CurrentAction as GridActionSkill).RangeTarget(targetlist);
        }
        
        
        //#if UNITY_EDITOR_LOG
        //        string str = m_SkillOwner.SceneID + "," + skill.m_name + "," + targetlist.Count + ": attack  ";
        //#endif
        if ((skill as SoldierSkill).m_actiontype == 0) {
            //StatusSelfBuff(m_SkillOwner,skill);
            for (int i = 0; i < targetlist.Count; i++) {
                if (targetlist[i] == null) {
                    Debug.Log("eeeeeeeee");
                }
                if (!targetlist[i].InBoat || targetlist[i].isDead) {
                    continue;
                }
                if (attacker.Target == targetlist[i]) {
                    attacker.TargetAttacked = true;
                    
                }
                SkillReleaseInfo info = Life.CalcDamage(attacker, targetlist[i], null, skill);
                if (skill.m_type == 1030) {
                    NGUIUtil.DebugLog(info.Result.ToString(), "red");
                }
                if (skill.m_attckmodeid > 0) {
                    float power = skill.GetAttackPower(0);
                    info.m_Damage = (int)(info.m_Damage * power);
                }
                Transform attackT = attacker.m_thisT;
                if (!targetlist[i].ApplyDamage(info, attackT) && !(targetlist[i] is IggWall)) {
                    attacker.AddAnger(AngerStyle.TargetDead, targetlist[i]);
                }
                
            }
            //Debug.Log(str);
        } else if ((skill as SoldierSkill).m_actiontype == 1) {
            for (int i = 0; i < targetlist.Count; i++) {
                if (!targetlist[i].InBoat || targetlist[i].isDead) {
                    continue;
                }
                if (attacker.Target == targetlist[i]) {
                    attacker.TargetAttacked = true;
#if UNITY_EDITOR_LOG
                    FileLog.write(attacker.SceneID, "TargetAttacked " + attacker.TargetAttacked + "," + attacker.m_TargetDuration);
#endif
                }
                SkillReleaseInfo info = Life.CalcDamage(attacker, targetlist[i], null, skill);
                if (skill.m_attckmodeid > 0) {
                    float power = skill.GetAttackPower(1);
                    info.m_Damage = (int)(info.m_Damage * power);
                }
                Transform attackT = attacker.m_thisT;
                if (!targetlist[i].ApplyDamage(info, attackT) && !(targetlist[i] is IggWall)) {
                    attacker.AddAnger(AngerStyle.TargetDead, targetlist[i]);
                }
                
            }
        } else if ((skill as SoldierSkill).m_actiontype == 6) {
            for (int i = 0; i < targetlist.Count; i++) {
                if (!targetlist[i].InBoat || targetlist[i].isDead) {
                    continue;
                }
                if (attacker.Target == targetlist[i]) {
                    attacker.TargetAttacked = true;
#if UNITY_EDITOR_LOG
                    FileLog.write(attacker.SceneID, "TargetAttacked " + attacker.TargetAttacked + "," + attacker.m_TargetDuration);
#endif
                }
                SkillReleaseInfo info = Life.CalcDamage(attacker, targetlist[i], null, skill);
                if (!info.m_bImmunity) {
                    if ((skill as SoldierSkill).m_data0 == 19) {
                        attacker.Anger += (skill as SoldierSkill).m_data1;
                        attacker.StatusUpdateAnger((skill as SoldierSkill).m_data1);
                    }
                }
                if (skill.m_type == 1030) {
                    NGUIUtil.DebugLog(info.Result.ToString(), "red");
                }
                if (skill.m_attckmodeid > 0) {
                    float power = skill.GetAttackPower(0);
                    info.m_Damage = (int)(info.m_Damage * power);
                }
                Transform attackT = attacker.m_thisT;
                if (!targetlist[i].ApplyDamage(info, attackT) && !(targetlist[i] is IggWall)) {
                    attacker.AddAnger(AngerStyle.TargetDead, targetlist[i]);
                }
                
            }
        }
        //EventCenter.DoEvent(NDEventType.StatusInterrupt, m_SceneID, LifeAction.Attack);
    }
    
    public override bool CanAttack()
    {
        //Debug.Log (m_parent.m_Attr.SkillReleaseType);
        if (m_SkillOwner.m_Attr.SkillReleaseType == ReleaseType.Normal) {
            return true;
        } else if (m_SkillOwner.m_Attr.SkillReleaseType == ReleaseType.NoAttack) {
            return false;
        } else if (m_SkillOwner.m_Attr.SkillReleaseType == ReleaseType.NoMagic) {
            if (m_AttackSkill.m_attacktype == (int)AttackType.Sacred) {
                return false;
            } else {
                return true;
            }
        } else {
            return false;
        }
    }
    
    /// <summary>
    /// 获取大招Action
    /// </summary>
    public GridActionCmd GetBigAttackAction()
    {
        GridActionCmd action = null;
        SoldierSkill bskill = GetBigSkill();
        
        int attackid  = -1;
        if (m_SkillTarget != null) {
            attackid = m_SkillTarget.SceneID;
        }
        action = RoleSkill.GetAttackSkillAction(bskill.m_type,
                attackid,
                m_SkillOwner.WalkDir,
                m_SkillOwner.RankDeep,
                m_SkillOwner.m_Attr.ModelType,
                m_SceneID);
        if (action == null) {
            Debug.Log("bigskill " + bskill.m_name + "," + bskill.m_type + "," + m_SkillOwner.m_Attr.ModelType + "," + m_SceneID);
        }
        (action as GridActionCmdActiveSkill).StartWithTarget(m_SkillOwner);
        ReSetBigCDTime();
        
        (m_SkillOwner as Role).RoleWalk.run.Attack = false;
        (m_SkillOwner as Role).RoleWalk.run.Reject = true;
        (m_SkillOwner as Role).RoleWalk.run.m_DoUpatePath = false;
        return action;
        
    }
    
    
    /// <summary>
    /// 产生建筑状态
    /// </summary>
    protected override bool StatusBuildDeBuff(Life Defense)
    {
        if (m_skill == null) {
            return false;
        }
        SoldierSkill skill = m_skill as SoldierSkill ;
        
        if (!(Defense is Building) || Defense == null || Defense.m_Attr == null || Defense.m_Status == null) {
            return  false;
        }
        
        if (skill.m_buildstatus == null) {
            return false;
        }
        
        bool InterruptSkill = false;
        List<SkillStatusInfo>l = skill.m_buildstatus;
        
        for (int i = 0; i < l.Count ; i++) {
            if (CheckCondition(Defense.m_Attr, l[i])) {
                if (Defense is Building) {
                    Building r = Defense as Building;
                    if (r.m_Attr.Level <= skill.m_rankslevel || Random.Range(0, 1f) <= skill.m_status_hitratio) {
                        if (Defense.m_Status.AddStatus(m_SceneID, m_skill.m_type, l[i]) == true) {
                            InterruptSkill = true;
                        }
                    }
                    
                }
            }
        }
        return InterruptSkill;
    }
    
    
    /// <summary>
    /// 产生DeBuff
    /// </summary>
    protected override bool StatusReleaseDeBuff(Life Defense)
    {
        if (m_skill == null) {
            return false;
        }
        SoldierSkill skill = m_skill as SoldierSkill ;
        if (!(Defense is Role) || Defense == null || Defense.m_Attr == null || Defense.m_Status == null) {
            return  false;
        }
        if (skill.m_releasedenemy_status == null) {
            return false;
        }
        
        bool InterruptSkill = false;
        List<SkillStatusInfo>l = skill.m_releasedenemy_status;
        
        for (int i = 0; i < l.Count ; i++) {
            if (CheckCondition(Defense.m_Attr, l[i])) {
                if (Defense.m_Status.AddStatus(m_SceneID, skill.m_type, l[i]) == true) {
                    InterruptSkill = true;
                }
            }
        }
        return InterruptSkill;
    }
    
    /// <summary>
    /// 产生Buff
    /// </summary>
    protected override bool StatusReleaseBuff(Life Defense)
    {
        if (m_skill == null) {
            return false;
        }
        SoldierSkill skill = m_skill as SoldierSkill ;
        if (!(Defense is Role) || Defense == null || Defense.m_Attr == null || Defense.m_Status == null) {
            return false;
        }
        if (m_skill == null || skill.m_releasedown_status == null) {
            return false;
        }
        
        bool InterruptSkill = false;
        List<SkillStatusInfo> l = skill.m_releasedown_status;
        
        for (int i = 0; i < l.Count; i++) {
            if (CheckCondition(Defense.m_Attr, l[i])) {
                if (Defense.m_Status.AddStatus(m_SceneID, skill.m_type, l[i]) == true) {
                    InterruptSkill = true;
                }
            }
        }
        return InterruptSkill;
    }
    
    /// <summary>
    /// 自我产生的产生Buff
    /// </summary>
    public  override void StatusSelfBuff(Life Self, SkillInfo skill)
    {
        if (m_skill == null) {
            return ;
        }
        SoldierSkill soldierskill = m_skill as SoldierSkill ;
        if (Self == null || Self.m_Attr == null || Self.m_Status == null) {
            return ;
        }
        if (soldierskill.m_own_status == null) {
            return ;
        }
        List<SkillStatusInfo>l = soldierskill.m_own_status;
        
        for (int i = 0; i < l.Count ; i++) {
            if (CheckCondition(Self.m_Attr, l[i])) {
                Self.m_Status.AddStatus(m_SceneID, soldierskill.m_type, l[i]);
            }
        }
    }
    
    /// <summary>
    /// 产生DeBuff
    /// </summary>
    protected override bool StatusDeBuff(Life Defense, SkillInfo skill)
    {
        if (m_skill == null) {
            return  false;
        }
        SoldierSkill soldierskill = skill as SoldierSkill ;
        if (!(Defense is Role) || Defense == null || Defense.m_Attr == null || Defense.m_Status == null) {
            return  false;
        }
        if (soldierskill.m_attack_status_enemy == null) {
            return false;
        }
        
        AttackType Type = (AttackType)skill.m_attacktype;
        
        bool InterruptSkill = false;
        List<SkillStatusInfo>l = soldierskill.m_attack_status_enemy;
        
        for (int i = 0; i < l.Count ; i++) {
            if (CheckCondition(Defense.m_Attr, l[i])) {
                if (Defense is Role) {
                    Role r = Defense as Role;
                    float random = Random.Range(0, 1f);
                    //Debug.Log(r + "," + r.m_Attr.Level  + "," + m_skill.m_rankslevel + ","  + random +  "," + ConfigM.GetSkillHitRate() + "," +l[i].m_name);
                    if (r.m_Attr.Level <= soldierskill.m_rankslevel || random <= soldierskill.m_status_hitratio) {
                        if (Defense.m_Status.CheckCanAddStatus(m_SceneID, soldierskill.m_type, Type, l[i]) == true) {
                            if (Defense.m_Status.AddStatus(m_SceneID, soldierskill.m_type, l[i]) == true) {
                                InterruptSkill = true;
                            }
                        }
                        
                    }
                    
                }
            }
        }
        return InterruptSkill;
    }
    
    public void ModifyAttributeByCondition(SkillReleaseInfo info, float delta)
    {
        foreach (SoldierSkill s in m_AllSkill) {
            // 闪电受击
            if (s.m_attacktype == (int)AttackType.PassiveCondition
                && s.m_condition == (int)SkillCondition.HitByElectric
                && ((int)info.HitAttributeType & (int)AttributeType.Electric) > 0
            ) {
                if (s.m_power1 == (int)EffectType.AngryIncreasement) {
                    m_SkillOwner.Anger += (int)((float)delta * (1f + (float)(s.m_power2) / 100f));
                }
                break;
                
            }
            
        }
    }
    
    public bool CheckDie(Life dielife)
    {
        foreach (SoldierSkill s in m_AllSkill) {
            if (s.m_attacktype == (int)AttackType.PassiveCondition && s.m_condition == 5 && s.m_enable) {
                if (s.m_condition_data0 == 1 && dielife.m_Core.m_Camp == m_SkillOwner.m_Core.m_Camp) {
                    if (dielife.m_Attr.Level <= s.m_level) {
                        AddAttr(s.m_power1, s.m_power2);
                    } else {
                        AddAttr(s.m_data0, s.m_data1);
                    }
                    return true;
                } else if (s.m_condition_data0 == 3 && dielife.m_Core.m_Camp != m_SkillOwner.m_Core.m_Camp) {
                    if (dielife.m_Attr.Level <= s.m_level) {
                        AddAttr(s.m_power1, s.m_power2);
                    } else {
                        AddAttr(s.m_data0, s.m_data1);
                    }
                    return true;
                }
            }
        }
        return false;
    }
    
    public void AddAttr(int  type, int value)
    {
        if (type == (int)EffectType.RecoAnger) {
            m_SkillOwner.Anger += value;
            m_SkillOwner.StatusUpdateAnger(value);
        }
    }
    
    public override int GetAuraAffector(EffectType Type, Life l, LifeMCamp camp)
    {
        foreach (SoldierSkill s in m_AllSkill) {
            if (s.m_attacktype == (int)AttackType.Aura && s.m_power1 == (int)Type) {
            
                if (l == m_SkillOwner && s.m_data0 == (int)Type) {
                    return s.m_data1;
                } else {
                    int skillTarget = s.m_target;
                    if (l.m_Core.m_Camp == camp) {
                        switch (skillTarget) {
                            case ((int)SkillTarget.Self):
                            case ((int)SkillTarget.SomeOneFriendly):
                            case ((int)SkillTarget.FriendlyTeam):
                            case ((int)SkillTarget.FriendlyGround): {
                                if (CheckAuraRange(s, l)) {
                                    return s.m_power2;
                                }
                                break;
                            }
                        }
                    } else {
                        switch (skillTarget) {
                            case ((int)SkillTarget.Enemy):
                            case ((int)SkillTarget.EnemyTeam):
                            case ((int)SkillTarget.EnemyGround): {
                                if (CheckAuraRange(s, l)) {
                                    return s.m_power2;
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }
        return 0;
    }
    
    public bool CheckAuraRange(SoldierSkill skill, Life l)
    {
        if (skill.m_range < 0) {
            return true;
        } else {
            return CheckRangeAttackTarget(l, m_SkillOwner.GetMapGrid(), skill.m_sort, skill.m_range, m_SkillOwner.WalkDir);
        }
    }
}


