using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 炮战技能
/// </summary>
/// <author>zhulin</author>
/// <editor>QFord</editor>
public class FireSkill : Skill
{

    private FlyCollisionInfo m_FlyInfo = null;
    
    /// <summary>
    /// 炮战作用对象计数
    /// </summary>
    private int m_iFireHitCount = 0;
    /// <summary>
    /// 是否启用对目标的伤害
    /// </summary>
    public bool m_bApplyDamage = true;
    /// <summary>
    /// 初始化数据，从数据中心获取所有技能数据
    /// </summary>
    public override bool Init(int SceneID, LifeMCore Core)
    {
        this.m_SceneID = SceneID;
        SoldierInfo info = CmCarbon.GetSoldierInfo(Core);
        if (info == null) {
            return false;
        }
        m_skill = info.GetFireSkill();
        return true;
    }
    
    public FlyCollisionInfo FlyInfo {
        set
        {
            m_FlyInfo = value;
        }
    }
    
    /// <summary>
    /// 触发炮战
    /// </summary>
    /// <param name="collision">技能释放者</param>
    /// <param name="HitDir">受击者</param>
    /// <returns>技能伤害值</returns>
    public FlyCollisionInfo FireTrigger(Collision collision, Life life, Life attatckLifeM, bool bReleaseSkill, bool bApplyDamage, int hitCount, FlyDir flydir)
    {
        //获取撞击对象
        //LifeM  life = GetCollisionGo(collision);
        if (life == null) {
            return null;
        }
        //炮战技能伤害
        return FireCombat(collision, life, attatckLifeM, bReleaseSkill, bApplyDamage, hitCount, flydir);
    }
    
    
    /// <summary>
    /// 炮战计算
    /// </summary>
    private FlyCollisionInfo FireCombat(Collision collision, Life Defense, Life Attacker, bool bReleaseSkill, bool bApplyDamage, int hitCount, FlyDir flydir)
    {
        if (Defense == null || Defense.isDead) {
            return null;
        }
        if (Defense.m_Core == null
            || Defense.m_Core.m_Camp == LifeMCamp.ATTACK) {
            return null;
        }
        
        int Damage = 0;
        if (m_bApplyDamage && bReleaseSkill == true) { //判断是否造成伤害
            Int2 Pos = Defense.GetMapPos();
            StatusSelfBuff(CM.GetLifeM(m_SceneID, Attacker.m_Core.m_type), m_skill);
            Damage = FireSkillRelease(CM.GetLifeM(m_SceneID, Attacker.m_Core.m_type), Defense, Pos);
            //Damage = ScriptM.Formula<int>("CALC_FIREDAMAGE_LOSS", Damage, m_iFireHitCount);
            m_iFireHitCount = hitCount;
            
        }
        //撞击伤害表现。
        //if(bApplyDamage == false) Damage = 0;
        {
            m_FlyInfo.damage = Damage;
            m_FlyInfo.bReleaseSkill = bReleaseSkill;
            m_FlyInfo.DamageTimeInterval = (m_skill as SoldierSkill).m_timeinterval * 0.001f;
            Defense.CollisionProcByFire(collision, Damage, ref m_FlyInfo, flydir);
        }
        
        if (Attacker is Pet) {
            m_bApplyDamage = m_FlyInfo.bApplyDamage = true;
        } else {
            //m_bApplyDamage = false;
        }
        SoldierSkill skill = m_skill as SoldierSkill ;
        if (!Defense.isDead && Defense is IggFloor) {
            if (skill.m_actiontype == 7) {
                if (skill.m_data0 == 2) {
                    int PedNum = skill.m_data2;
                    if (PedNum == 0) {
                        PedNum = 1;
                    }
                    for (int i = 0; i < PedNum; i++) {
                        //Role r = CM.GetLifeM(m_SceneID, LifeMType.SOLDIER) as Role;
                        
                        Int2 BornPos = Defense.GetMapPos();
                        Vector3 v3 = collision.contacts[0].point;
                        float fGridDisatance = Vector3.Distance(v3, Defense.GetMapGrid().WorldPos);
                        // Info.HitDir = Info.bLeft ? WalkDir.WALKRIGHT : WalkDir.WALKLEFT;
                        
                        if (fGridDisatance > 0) {
                            BornPos.Unit += (int)(fGridDisatance / MapGrid.m_width);
                            MapGrid mg = MapGrid.GetMG(BornPos);
                            if (mg == null) {
                                BornPos = Defense.GetMapPos();
                            }
                        }
                        //Int2 Pos = Defense.GetMapPos();
                        SummonPros pros = CreatePros(skill.m_data1, MapGrid.GetMG(BornPos));
                    }
                }
            }
        }
        
        return m_FlyInfo;
    }
    
    private SummonPros CreatePros(int ID, MapGrid pos)
    {
        SummonProsInfo info = SummonM.GetSummonProsInfo(ID);
        
        SummonPros pros = new SummonPros();
        pros.CreateSkin(BattleEnvironmentM.GetLifeMBornNode(true), info.m_modeltype, info.m_modeltype.ToString(), AnimatorState.Empty, true);
        //IGameRole i = GameRoleFactory.Create(BattleEnvironmentM.GetLifeMBornNode(true), info.m_modeltype, info.m_modeltype.ToString(), AnimatorState.Empty);
        GameObject go = pros.RoleSkinCom.tRoot.gameObject;
        LifeObj lo = go.AddComponent<LifeObj>();
        lo.SetLife(pros, pros.RoleSkinCom.ProPerty);
        
        Role r = CM.GetLifeM(m_SceneID, LifeMType.SOLDIER) as Role;
        pros.SetBorn(r, ID, info, pos);
        go.transform.localPosition = pos.pos;
        return pros;
    }
    /// <summary>
    /// 炮战技能释放
    /// </summary>
    public  int FireSkillRelease(Life Attack, Life Defense, Int2 Pos)
    {
        AttackResult Result = AttackResult.Normal;
        if (Attack == null || Defense == null || m_skill == null) {
            return 0;
        }
        SoldierSkill skill = m_skill as SoldierSkill ;
        
        
        if (Attack.m_Attr == null || Defense.m_Attr == null || Attack.m_Attr.CurConcussion == 0) {
            return 0;
        }
        if (Attack.m_Core == null) {
            return 0;
        }
        
        LifeMCamp Camp = Attack.m_Core.m_Camp;
        //炮战伤害改成使用冲击力和承受力
        int Damage = Mathf.CeilToInt(Attack.m_Attr.CurConcussion * Defense.fullHP * 1.0f / Defense.m_Attr.Bear);
        if (Attack.m_Attr.CurConcussion < Defense.m_Attr.CurBear) {
            Attack.m_Attr.CurConcussion = 0;
        } else {
            Attack.m_Attr.CurConcussion = Attack.m_Attr.CurConcussion - Defense.m_Attr.CurBear;
        }
        //计算技能伤害
        //int Damage = CalcSkillDamage(Attack.m_Attr ,Defense.m_Attr ,ref Result,skill);
        if (Defense is IggFloor && (Defense as IggFloor).Room != null) {
            foreach (Building r in (Defense as IggFloor).m_rooms) {
                StatusBuildDeBuff(r);
            }
        }
        if (Defense is IggWall && (Defense as IggWall).Room != null)
            foreach (Building r in (Defense as IggWall).m_rooms) {
                StatusBuildDeBuff(r);
            }
        //给目标参数状态。
        List<Life> l = new List<Life>();
        StatusRadar.GetStatusTarget(ref l, Defense, skill.m_sort, skill.m_range, Camp);
        
        for (int i = 0; i < l.Count; i ++) {
            if (l[i] != null) {
                StatusDeBuff(l[i], m_skill);
            }
        }
        
        return -Damage;
    }
    
    /// <summary>
    /// 技能命中判断
    /// </summary>
    protected override bool SkillHit(NDAttribute Attack, NDAttribute Defense, SkillInfo skill)
    {
        return true;
    }
    
    /// <summary>
    /// 技能暴击判断
    /// </summary>
    protected  override bool SkillCrit(NDAttribute Attack, NDAttribute Defense, SkillInfo skill)
    {
        return false;
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
    protected override bool StatusDeBuff(Life Defense, SkillInfo skill)
    {
        if (m_skill == null) {
            return  false;
        }
        SoldierSkill soldierskill = m_skill as SoldierSkill ;
        if (!(Defense is Role) || Defense == null || Defense.m_Attr == null || Defense.m_Status == null) {
            return  false;
        }
        if (soldierskill.m_attack_status_enemy == null) {
            return false;
        }
        
        bool InterruptSkill = false;
        List<SkillStatusInfo>l = soldierskill.m_attack_status_enemy;
        
        for (int i = 0; i < l.Count ; i++) {
            if (CheckCondition(Defense.m_Attr, l[i])) {
                if (Defense is Role) {
                    Role r = Defense as Role;
                    float random = Random.Range(0, 1f);
                    //Debug.Log(r + "," + r.m_Attr.Level  + "," + m_skill.m_rankslevel + ","  + random +  "," + ConfigM.GetSkillHitRate() + "," +l[i].m_name);
                    if (r.m_Attr.Level <= soldierskill.m_rankslevel || random <= soldierskill.m_status_hitratio) {
                        if (Defense.m_Status.AddStatus(m_SceneID, soldierskill.m_type, l[i]) == true) {
                            InterruptSkill = true;
                        }
                    }
                    
                }
            }
        }
        return InterruptSkill;
    }
}
