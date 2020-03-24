using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// 技能属性
/// </summary>
/// <author>zhulin</author>
///
public enum AttackType {
    Physical 	= 0,			//物理技能
    Magic 		= 1,			//魔法技能
    Sacred 		= 2,			//神圣技能
    Passive 	= 3,			//被动技能
    Fire        = 4,			//炮战技能
    Aura        = 5,            //光环技能
    PassiveCondition = 6,		//条件被动技能
};
public enum TargetType {
    Gold = 0x01,			//金库
    Pet = 0x02,				//召唤物
    Trap = 0x04,			//陷阱
    Soldier = 0x08,			//炮弹兵
    
}

public enum SkillTarget {
    Enemy = 0,          // 敌人
    Self = 1,           // 自己
    EnemyGround = 2,    // 对地（敌人）
    FriendlyTeam = 3,    // 己方所有（包含自己）
    SomeOneFriendly = 4, // 己方某一炮兵
    EnemyTeam = 5,      // 敌方所有炮弹兵
    FriendlyGround = 6, // 对地己方使用
}

public class Skill
{
    protected SkillInfo m_skill = null;
    protected SoldierSkill m_AttackSkill = null;
    protected int m_SceneID = -1;
    public   float m_CdTime;          //CD即使时间
    public float m_CDDuration = 0;     // CD总时间
    protected bool m_Release;
    public Life m_AttackTarget = null;//攻击目标
    public MapGrid m_TargetGrid = null;
    public Life m_SkillTarget = null;//当前技能目标
    //public static List<Skill> AuraSkill = new List<Skill>();
    
    public float CDTime {
        get{return m_CdTime;}
    }
    
    public bool Release {
        get{return m_Release;}
        set{m_Release = value;}
    }
    
    public SoldierSkill PropAttackSkillInfo {
        get{return m_AttackSkill;}
        set{m_AttackSkill = value;}
    }
    public SkillInfo PropSkillInfo {
        get{return m_skill ;}
        set{m_skill = value;}
    }
    public void ReSetCDTime()
    {
        m_skill.m_Relsease = true;
        m_CdTime = m_skill.m_cd * 0.001f;
    }
    public void ReSetBigCDTime()
    {
        SoldierSkill bigskill = GetBigSkill();
        m_CdTime = bigskill.m_cd * 0.001f;
    }
    public void ReleaseSkill()
    {
        if (!m_Release) {
            m_CdTime = m_CDDuration;
            m_Release = true;
        }
    }
    
    
    public virtual bool GetNextSkill()
    {
        return false;
    }
    
    
    public virtual SoldierSkill GetBigSkill()
    {
        return null;
    }
    /// <summary>
    /// 初始化技能数据，从数据中心获取所有技能数据
    /// </summary>
    public virtual bool Init(int SceneID, LifeMCore Core)
    {
        return true;
    }
    
    /// <summary>
    /// 技能cd时间
    /// </summary>
    public virtual void Update(float deltaTime)
    {
    }
    
    /// <summary>
    /// 对象死亡，调用释放技能数据
    /// </summary>
    public virtual void Destory()
    {
    
    }
    
    public virtual void ResetConditionTime(int id)
    {
    
    }
    /// <summary>
    /// 自我产生的产生Buff
    /// </summary>
    public  virtual void StatusSelfBuff(Life Self, SkillInfo skill)
    {
    
    }
    /// <summary>
    /// 产生建筑状态
    /// </summary>
    protected virtual bool StatusBuildDeBuff(Life Defense)
    {
        return false;
    }
    /// <summary>
    /// 产生DeBuff
    /// </summary>
    protected virtual bool StatusDeBuff(Life Defense, SkillInfo skill)
    {
        return false;
    }
    /// <summary>
    /// 产生Buff
    /// </summary>
    protected bool StatusBuff(Life Defense, SkillInfo skill)
    {
        if (!(Defense is Role) || Defense == null || Defense.m_Attr == null || Defense.m_Status == null) {
            return false;
        }
        if (skill == null || skill.m_attack_status_own == null) {
            return false;
        }
        
        bool InterruptSkill = false;
        List<SkillStatusInfo> l = skill.m_attack_status_own;
        
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
    /// 产生DeBuff
    /// </summary>
    protected virtual bool StatusReleaseDeBuff(Life Defense)
    {
        return false;
    }
    /// <summary>
    /// 产生Buff
    /// </summary>
    protected virtual bool StatusReleaseBuff(Life Defense)
    {
        return false;
    }
    /// <summary>
    /// 状态产生判断
    /// </summary>
    protected bool CheckCondition(NDAttribute attr, SkillStatusInfo StatusInfo)
    {
        if (attr == null || StatusInfo == null) {
            return false;
        }
        //为0 直接产生状态。
        EffectType s = (EffectType)StatusInfo.m_condition;
        if (s == EffectType.None) {
            return true;
        }
        int data0 = StatusInfo.m_data0;
        int data1 = StatusInfo.m_data1;
        int value = attr.GetAttrData(s);
        if (value > data0 && value <= data1) {
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 计算技能伤害
    /// </summary>
    protected  int CalcSkillDamage(NDAttribute Attack, NDAttribute Defense, ref AttackResult Result, SkillInfo skill)
    {
        //未命中
        if (SkillHit(Attack, Defense, skill) == false) {
            Result = AttackResult.Miss;
            return 0;
        }
        
        //计算伤害
        int Damage = SkillDamage(Attack, Defense, skill);
        //是否暴击
        if (SkillCrit(Attack, Defense, skill) == true) {
            Result = AttackResult.Crit;
            Damage = SkillCritDamage(Damage, Attack.PhysicalCritBonusDamage, Attack.MagicCritBonusDamage, skill);
        }
        return Damage;
    }
    
    protected int CalcDistanInfo(NDAttribute attrAttacter, NDAttribute attrDefence, int damage)
    {
        int diffLevel = attrAttacter.Level - attrDefence.Level;
        float factorStar = 0;
        float factorLevel = 0;
        SkillM.GetDistainFactor(diffLevel, ref factorStar, ref factorLevel);
        return ScriptM.Formula<int>("CALC_DISTAIN_DAMAGE", damage, factorLevel, factorStar);
    }
    
    /// <summary>
    /// 计算具有属性的技能的攻击伤害值
    /// </summary>
    ///
    public static int CalcAttributableSkillDamage(AttributeType attributeType, NDAttribute attrDefence, int damage)
    {
        if (attributeType == AttributeType.NONE) {
            return damage;
        }
        
        int resistance = 0;		// 抗性
        int reduction = 0;		// 减免
        int attack = 0;		// 属性攻击
        
        if ((attributeType & AttributeType.Fire) == AttributeType.Fire) {
            resistance = attrDefence.GetAttrData(EffectType.AntiFire);
            attack = attrDefence.GetAttrData(EffectType.FireAttack);
            reduction = attrDefence.GetAttrData(EffectType.FireDamageReduction);
        } else if ((attributeType & AttributeType.Poison) == AttributeType.Poison) {
            resistance = attrDefence.GetAttrData(EffectType.AntiPotion);
            attack = attrDefence.GetAttrData(EffectType.PotionAttack);
            reduction = attrDefence.GetAttrData(EffectType.PotionDamageReduction);
        } else if ((attributeType & AttributeType.Water) == AttributeType.Water) {
            resistance = attrDefence.GetAttrData(EffectType.AntiWater);
            attack = attrDefence.GetAttrData(EffectType.WaterAttack);
            reduction = attrDefence.GetAttrData(EffectType.WaterDamageReduction);
        } else if ((attributeType & AttributeType.Gas) == AttributeType.Gas) {
            resistance = attrDefence.GetAttrData(EffectType.AntiGas);
            attack = attrDefence.GetAttrData(EffectType.GasAttack);
            reduction = attrDefence.GetAttrData(EffectType.GasDamageReduction);
        } else if ((attributeType & AttributeType.Electric) == AttributeType.Electric) {
            resistance = attrDefence.GetAttrData(EffectType.AntiElectric);
            attack = attrDefence.GetAttrData(EffectType.ElectricAttack);
            reduction = attrDefence.GetAttrData(EffectType.ElectricDamageReduction);
        }
        
        return ScriptM.Formula<int>("CALC_ATTRIBUTABLE_SKILL_DAMAGE",
                damage,
                attack,
                resistance,
                reduction);
    }
    
    /// <summary>
    /// 计算技能治疗量
    /// </summary>
    protected   int CalcSkillRegenHP(NDAttribute Attack, NDAttribute Defense, ref AttackResult Result, SkillInfo skill)
    {
        //计算伤害
        int Damage = SkillRegenHP(Attack, Defense, skill);
        return Damage;
    }
    // 计算吸血回复生命值
    protected int CalcVampireHP(NDAttribute Attack, int damage)
    {
        return ScriptM.Formula<int>("CACL_VAMPIRE_HP", Attack.Vampire, damage);
    }
    /// <summary>
    /// 技能暴击判断
    /// </summary>
    protected  virtual bool SkillCrit(NDAttribute Attack, NDAttribute Defense, SkillInfo skill)
    {
        return false;
    }
    /// <summary>
    /// 技能命中判断
    /// </summary>
    protected virtual bool SkillHit(NDAttribute Attack, NDAttribute Defense, SkillInfo skill)
    {
        return true;
    }
    
    
    /// <summary>
    /// 计算技能伤害
    /// </summary>
    protected int SkillDamage(NDAttribute Attack, NDAttribute Defense, SkillInfo skill)
    {
        if (Attack == null || Defense == null || skill == null) {
            return 0;
        }
        AttackType Type = (AttackType)skill.m_attacktype;
        int power1 = skill.m_power1;
        int power2 = skill.m_power2;
        if (Type == AttackType.Physical) {  //物理攻击
            return ScriptM.Formula<int>("CALC_SOLDIER_PHYDAMAGE",
                    power1,
                    Attack.PhyAttack,
                    Attack.CutPhyDefend,
                    Defense.PhyDefend,
                    Defense.CutPhyDamage,
                    power2);
        } else if (Type ==  AttackType.Magic) { // 魔法攻击
            return ScriptM.Formula<int>("CALC_SOLDIER_MAGICDAMAGE",
                    power1,
                    Attack.MagicAttack,
                    Attack.CutMagDefend,
                    Defense.MagicDefend,
                    Defense.CutMagDamage,
                    power2);
        } else if (Type ==  AttackType.Sacred) { // 神圣技能
            return power2;
        } else if (Type == AttackType.Passive) { //被动技能
            return 0;
        } else if (Type == AttackType.Fire) { //炮战技能
            return power2;
        } else {
            Debug.Log("不存在的仅能作用类型：" + skill.m_attacktype);
            return 0;
        }
    }	/// <summary>
    /// 计算技能治疗量
    /// </summary>
    protected int SkillRegenHP(NDAttribute Attack, NDAttribute Defense, SkillInfo skill)
    {
        if (Attack == null || Defense == null || skill == null) {
            return 0;
        }
        AttackType Type = (AttackType)skill.m_attacktype;
        int power3 = skill.m_power3;
        int power4 = skill.m_power4;
        int cout  = 1;
        if ((skill as SoldierSkill).m_timeinterval >  0) {
            cout = (skill as SoldierSkill).m_step_secs / (skill as SoldierSkill).m_timeinterval;
        }
        return ScriptM.Formula<int>("CACL_GENGEN_HP",
                Attack.MagicAttack,
                power3,
                power4,
                cout,
                Attack.AddDoctor);
    }
    /// <summary>
    /// 技能暴击伤害
    /// </summary>
    protected int SkillCritDamage(int Damage, int PhysicalCritBonusDamage, int MagicCritBonusDamage, SkillInfo skill)
    {
        if (skill == null) {
            return Damage;
        }
        AttackType Type = (AttackType)skill.m_attacktype;
        if (Type == AttackType.Physical) {
            return ScriptM.Formula<int>("CALC_SOLDIER_PHYCRITDAMAGE", Damage, PhysicalCritBonusDamage);
        } else if (Type == AttackType.Magic) { // 魔法攻击
            return ScriptM.Formula<int>("CALC_SOLDIER_MAGICCRITDAMAGE", Damage, MagicCritBonusDamage);
        } else {
            return Damage;
        }
    }
    /// <summary>
    /// 检测攻击目标
    /// </summary>
    /// <param name="target"></param>
    /// <param name="g"></param>
    /// <param name="Sort"></param>
    /// <param name="radus"></param>
    /// <param name="dir">线性搜索需要方向</param>
    /// <returns></returns>
    static public  bool CheckRangeAttackTarget(Life target, MapGrid g, int Sort, float radus, WalkDir dir)
    {
        Building buildlife = target as Building;
        int startUnit = g.GridPos.Unit;
        int r = (int)Mathf.Round(radus);
        if (buildlife != null) {
            if (Sort == 3) {
                int nBegine = startUnit - r;
                int nEnd = startUnit + r;
                for (int nCnt = nBegine; nCnt <= nEnd; nCnt++) {
                    if (buildlife.CheckInBuildMap(new Int2(nCnt, g.GridPos.Layer))) {
                        return true;
                    }
                }
            } else if (Sort == 2) {
                int nBegine = g.GridPos.Unit;
                int nEnd = startUnit + r;
                if (dir == WalkDir.WALKLEFT) {
                    nBegine = startUnit - r;
                    nEnd = g.GridPos.Unit;
                }
                for (int nCnt = nBegine; nCnt <= nEnd; nCnt++) {
                    if (buildlife.CheckInBuildMap(new Int2(nCnt, g.GridPos.Layer))) {
                        return true;
                    }
                }
            }
            return false;
        }
        
        MapGrid AttackStation2 = target.GetMapGrid();
        if (!NdUtil.IsSameMapLayer(AttackStation2.GridPos, g.GridPos)) {
            return false;
        }
        
        
        if (Sort == 3) {
            if (AttackStation2.pos.x >= (g.pos.x - radus) && AttackStation2.pos.x <= (g.pos.x + radus)) {
                return true;
            }
        } else  if (Sort == 2) {
            if (dir == WalkDir.WALKLEFT) {
                if (AttackStation2.pos.x >= (g.pos.x - radus)) {
                    return true;
                }
            } else {
                if (AttackStation2.pos.x < (g.pos.x + radus)) {
                    return true;
                }
            }
        } else if (Sort == 4) {
            if (dir == WalkDir.WALKLEFT) {
                if (AttackStation2.pos.x >= (g.pos.x - radus)) {
                    return true;
                }
            } else {
                if (AttackStation2.pos.x < (g.pos.x + radus)) {
                    return true;
                }
            }
        }
        
        return false;
    }
    //Sort行为分类，详细内容在技能系统——炮弹兵技能数据结构说明
    public virtual bool CheckAttackTarget(Life target, int Sort, int distant, bool isusedir = true)
    {
        return CheckDoAttackTarget(target, Sort, distant, isusedir);
        
    }
    
    public WalkDir GetHideDir(Life target)
    {
        Life life = CM.GetLifeM(m_SceneID, LifeMType.SOLDIER);
        
        if (life is Role) {
            Role parent = life as Role;
            
            if (parent.m_Attr.IsHide && target is Role) {
                if (target.WalkDir == WalkDir.WALKLEFT) {
                    MapGrid lg = target.GetMapGrid().Right;
                    while (lg != null) {
                        if (lg.Type == GridType.GRID_NORMAL) {
                            if (lg.IsAttackStations()) {
                                List<int> samelist = new List<int>();
                                List<int> unsamelist = new List<int>();
                                lg.GetCampRoleList(parent.m_Core.m_Camp, ref samelist, ref unsamelist);
                                if (unsamelist.Count <= 0) {
                                    return  target.WalkDir;
                                }
                                break;
                            } else {
                                lg = lg.Right;
                            }
                        } else {
                            break;
                        }
                    }
                } else if (target.WalkDir == WalkDir.WALKRIGHT) {
                    MapGrid rg = target.GetMapGrid().Left;
                    while (rg != null) {
                        if (rg.Type == GridType.GRID_NORMAL) {
                            if (rg.IsAttackStations()) {
                                List<int> samelist = new List<int>();
                                List<int> unsamelist = new List<int>();
                                rg.GetCampRoleList(parent.m_Core.m_Camp, ref samelist, ref unsamelist);
                                if (unsamelist.Count <= 0) {
                                    return target.WalkDir;
                                }
                                break;
                            } else {
                                rg = rg.Left;
                            }
                        } else {
                            break;
                        }
                    }
                }
            }
        }
        return life.WalkDir;
    }
    public virtual bool CheckDoAttackTarget(Life target, int Sort, int distant, bool isusedir = true)
    {
        if (target == null || target.isDead) {
            return false;
        }
        //确认目标在行走路线上。
        Life life = CM.GetAllLifeM(m_SceneID, LifeMType.SOLDIER | LifeMType.SUMMONPET) as Role;
        if (life == null) {
            return false;
        }
        
        float radius = distant;//PropSkillInfo.m_distance/MapGrid.m_Pixel;
        //攻击距离为0的时候是全屏攻击范围
        if (radius <= 0) {
            return true;
        }
        if (!life.m_Attr.IsHide &&  isusedir && (life as Role).run.CheckInRoad(target.GetMapPos()) == false) {
            return false;
        }
        //单人
        //if (Sort == 1)
        {
            Life l = CM.GetAllLifeM(m_SceneID, LifeMType.SOLDIER | LifeMType.SUMMONPET);
            if (l is Role) {
                Role parent = l as Role;
                WalkDir pdir = parent.WalkDir;
                
                List<MapGrid> allg = target.GetAllMapGrid();
                if (!NdUtil.IsLifeSampMapLayer(target, parent.GetMapPos())) {
                    bool link = false;
                    foreach (MapGrid g in allg) {
                        if (MapGrid.CheckLink(parent.GetMapGrid(), g)) {
                            link = true;
                        }
                        
                    }
                    if (!link) {
                        return false;
                    }
                }
                List<IggWall> lw = new List<IggWall>();
                MapGrid.GetWallList(ref lw, parent.GetMapPos(), target.GetMapPos());
                foreach (IggWall w in lw) {
                    if (!w.Open) {
                        return false;
                    }
                }
                //radius += parent.m_Attr.Radius;
                float x1 = parent.m_thisT.localPosition.x;
                /*MapGrid g = MapGrid.GetMG(target.GetMapPos());
                if (g == null)
                {
                	//Debug.LogError(target + "," + target.GetMapPos() + "," + target.InBoat);
                	return false;
                }*/
                //float x2 = 0;//g.pos.x;
                List<float> lx2 = new List<float>();
                foreach (MapGrid g in allg) {
                    lx2.Add(g.pos.x);
                }
                if (target is Role) {
                    //radius += (int)(target as Role).m_Attr.Radius;
                    lx2[0] = target.m_thisT.localPosition.x;
                    /*if (target.WalkDir == WalkDir.WALKLEFT)
                    	x2 -= target.RankDeep * Role.s_deepoffset;
                    else
                    	x2 += target.RankDeep * Role.s_deepoffset;*/
                    
                    if (target.RankDeep > 0) {
                        lx2[0] = target.GetMapGrid().pos.x;
                    }
                    if (distant == 3 && target.WalkDir != l.WalkDir) {
                        if ((target as Role).RoleWalk != null) {
                            lx2[0] = (target as Role).RoleWalk.run.GetAttackStationMapGrid().pos.x;
                        }
                    }
                }
                if (parent.m_Attr.IsHide && target is Role) {
                    if (target.WalkDir == WalkDir.WALKLEFT) {
                        MapGrid lg = target.GetMapGrid().Right;
                        while (lg != null) {
                            if (lg.Type == GridType.GRID_NORMAL) {
                                if (lg.IsAttackStations()) {
                                    List<int> samelist = new List<int>();
                                    List<int> unsamelist = new List<int>();
                                    lg.GetCampRoleList(parent.m_Core.m_Camp, ref samelist, ref unsamelist);
                                    if (unsamelist.Count <= 0) {
                                        pdir = target.WalkDir;
                                        lx2[0] += 1f;
                                    }
                                    break;
                                } else {
                                    lg = lg.Right;
                                }
                            } else {
                                break;
                            }
                        }
                    } else if (target.WalkDir == WalkDir.WALKRIGHT) {
                        MapGrid rg = target.GetMapGrid().Left;
                        while (rg != null) {
                            if (rg.Type == GridType.GRID_NORMAL) {
                                if (rg.IsAttackStations()) {
                                    List<int> samelist = new List<int>();
                                    List<int> unsamelist = new List<int>();
                                    rg.GetCampRoleList(parent.m_Core.m_Camp, ref samelist, ref unsamelist);
                                    if (unsamelist.Count <= 0) {
                                        pdir = target.WalkDir;
                                        lx2[0] -= 1f;
                                    }
                                    break;
                                } else {
                                    rg = rg.Left;
                                }
                            } else {
                                break;
                            }
                        }
                    }
                }
                if (l.RankDeep > 0) {
                    x1 = l.GetMapGrid().pos.x;
                }
                radius += target.RankDeep * Role.s_deepoffset + l.RankDeep * Role.s_deepoffset;
                if (target is IggWall) {
                    radius += 1;
                }
                radius += 0.5f;
                if (isusedir) {
                    if (pdir == WalkDir.WALKLEFT) {
                        foreach (float x2 in lx2) {
                            if (x1 >= x2 && (x1 - x2) <= (radius * MapGrid.m_width)) {
                                return true;
                            }
                        }
                    } else if (pdir == WalkDir.WALKRIGHT) {
                    
                        foreach (float x2 in lx2) {
                            if (x1 <= x2  && (x2 - x1) <= (radius * MapGrid.m_width)) {
                                return true;
                            }
                        }
                    } else {
                    
                        foreach (float x2 in lx2) {
                            if (Mathf.Abs(x1 - x2) < radius * MapGrid.m_width) {
                                return true;
                            }
                        }
                    }
                } else {
                    foreach (float x2 in lx2) {
                        if (Mathf.Abs(x1 - x2) < radius * MapGrid.m_width) {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    //如果是全图的技能，不过滤
    static public bool CheckCanAttack(GridActionCmd action, bool ignoredistance = false)
    {
        if (ignoredistance) {
            return true;
        }
        if (action is GridActionCmdStair || action is GridActionCmdJump || action is GridActionCmdSpecialJump || action is GridActionCmdFall || action is GridActionCmdSendToPos) {
            return false;
        }
        return true;
    }
    
    //目标做为攻击目标
    public static List<Life> GetSkillTarget(List<Life> Attacklist, SoldierSkill info)
    {
        int term1 = info.m_term1;
        int term2 = info.m_term2;
        int term3 = info.m_term3;
        int index = -1;
        int value  = -1;
        List<Life> l = new List<Life>();
        List<Life> temp = new List<Life>();
        if (term1 == 0) {
            return Attacklist;
        }
        //2015.4.23 by txm
        //分支start
        List<Life> lbuild = new List<Life>();
        List<Life> lRandom = new List<Life> ();
        
        for (int i = 0; i < Attacklist.Count; i++) {
            if (Attacklist[i] is Building) {
                lbuild.Add(Attacklist[i]);
            } else if (!(Attacklist[i] is IggWall)) {
                //Role w = Attacklist[i] as Role;
                int newvalue = GetTerm1Vaule(Attacklist[i], term1);
                if (term1 > 0) {
                    bool ret = CheckCondition1(newvalue, term2, term3);
                    if (ret) {
                        temp.Add(Attacklist[i]);
                    }
                    
                }
                /*				if ((term1 >= 1 && term1 <= 5) || term1 == 99)
                				{
                					if (i == 0)
                					{
                						value = newvalue;
                						index = i;
                						l.Add(Attacklist[i]);
                					}
                					else
                					{
                						if (CheckCondition(value,newvalue,term2,term3))
                						{
                							value = newvalue;
                							index = i;
                							l[0] = Attacklist[i];
                						}
                					}
                				}
                				else if(term1 == 8)
                				{
                					bool ret = CheckCondition(newvalue,0,term2,term3);
                					if (ret)
                					{
                						lRandom.Add(Attacklist[i]);
                					}
                
                					if(i == Attacklist.Count -1 && lRandom.Count > 0)
                					{
                						int random = Random.Range(0,lRandom.Count -1);
                						l.Add(lRandom[random]);
                					}
                				}
                				else if (term1 >= 5)
                				{
                					bool ret = CheckCondition(newvalue,0,term2,term3);
                					if (ret)
                					{
                						l.Add(Attacklist[i]);
                					}
                
                				}*/
                
                else if (term1 == 0) {
                    temp.Add(Attacklist[i]);
                }
            }
        }
        if (temp.Count > 0) {
            if (term1 == 99 || term1 == 8) {
                Random.seed = GlobalTimer.GetNowTimeInt();
                int i = Random.Range(0, temp.Count);
                l.Add(temp[i]);
            } else if (term1 == 0 || term2 < term3) {
                l.AddRange(temp);
            } else {
                for (int i = 0; i < temp.Count; i++) {
                    int newvalue = GetTerm1Vaule(temp[i], term1);
                    if (i == 0) {
                        value = newvalue;
                        index = i;
                        l.Add(temp[i]);
                    } else {
                        if (CheckCondition2(value, newvalue, term2, term3)) {
                            value = newvalue;
                            index = i;
                            l[0] = temp[i];
                        }
                    }
                }
            }
        }
        temp.Clear();
        if (l.Count == 0) {
            for (int i = 0; i < lbuild.Count; i++) {
                if (!(lbuild[i] is IggWall)) {
                    //Role w = Attacklist[i] as Role;
                    int newvalue = GetTerm1Vaule(lbuild[i], term1);
                    if (term1 > 0) {
                        bool ret = CheckCondition1(newvalue, term2, term3);
                        if (ret) {
                            temp.Add(lbuild[i]);
                        }
                        
                    }
                    /*					if ((term1 >= 1 && term1 <= 5) || term1 == 99)
                    					{
                    						if (i == 0)
                    						{
                    							value = newvalue;
                    							index = i;
                    							l.Add(lbuild[i]);
                    						}
                    						else
                    						{
                    							if (CheckCondition(value,newvalue,term2,term3))
                    							{
                    								value = newvalue;
                    								index = i;
                    								l[0] = lbuild[i];
                    							}
                    						}
                    					}
                    					else if(term1 == 8)
                    					{
                    						bool ret = CheckCondition(newvalue,0,term2,term3);
                    						if (ret)
                    						{
                    							lRandom.Add(lbuild[i]);
                    						}
                    						if(i == lbuild.Count -1 && lRandom.Count > 0)
                    						{
                    							int random = Random.Range(0,lRandom.Count -1);
                    							l.Add(lRandom[random]);
                    						}
                    					}
                    					else if (term1 > 5)
                    					{
                    						bool ret = CheckCondition(newvalue,0,term2,term3);
                    						if (ret)
                    						{
                    							l.Add(lbuild[i]);
                    						}
                    
                    					}*/
                    else if (term1 == 0) {
                        l.Add(lbuild[i]);
                    }
                }
            }
            
            if (temp.Count > 0) {
                if (term1 == 0 || term2 < term3) {
                    l.AddRange(temp);
                } else if (term1 == 99 || term1 == 8) {
                    l.Add(temp[Random.Range(0, temp.Count)]);
                } else {
                    for (int i = 0; i < temp.Count; i++) {
                        int newvalue = GetTerm1Vaule(temp[i], term1);
                        if (i == 0) {
                            value = newvalue;
                            index = i;
                            l.Add(temp[i]);
                        } else {
                            if (CheckCondition2(value, newvalue, term2, term3)) {
                                value = newvalue;
                                index = i;
                                l[0] = temp[i];
                            }
                        }
                    }
                }
            }
        }
        
        return l;
    }
    public static  int GetTerm1Vaule(Life w, int term1)
    {
        switch (term1) {
            case 1:
                return w.m_Attr.Strength;
            //break;
            case 2:
                return w.m_Attr.Agility;
            //break;
            case 3:
                return w.m_Attr.Intelligence;
            //break;
            case 4:
            
                //2015.4.23 by txm
                //分支start
                //用-1区分有没有满
                return w.m_Attr.Hp >= w.m_Attr.FullHp ? -w.m_Attr.Hp : w.m_Attr.Hp;
            //break;
            case 5:
                //用-1区分有没有满
                return w.m_Attr.Anger >= w.m_Attr.FullAnger ? -w.m_Attr.Anger : w.m_Attr.FullAnger;
            //end
            //break;
            case 6:
                return (int)((float)w.m_Attr.Hp / w.m_Attr.FullHp * 100);
            //break;
            case 7:
                return (int)((float)w.m_Attr.Anger / w.m_Attr.FullAnger * 100);
            //			break;
            case 8:
            
                return (int)((float)w.m_Attr.Hp / w.m_Attr.FullHp * 100);
            //选取随机目标
            case 99:
                int value =  Random.Range(0, 10);
                //Debug.Log("Random  " + value + "," + Time.time);
                return value;
            default:
                break;
        }
        return -1;
    }
    public static  bool CheckCondition1(int left, int term2, int term3)
    {
        //2015.4.23 by txm
        //分支start
        if (term2 == 0 && term3 == 0) {
            return true;
        } else if (term2 == 100 && term3 == 100) {
            return true;
        }
        //分支end
        if (left >= term2 && left <= term3) {
            return true;
        }
        
        return false;
    }
    
    public static  bool CheckCondition2(int left, int right, int term2, int term3)
    {
        //2015.4.23 by txm
        //分支start
        if (term2 == 0 && term3 == 0) {
            if (Mathf.Abs(right) < Mathf.Abs(left)) {
                return true;
            }
            return false;
        } else if (term2 == 100 && term3 == 100) {
            if (right > left) {
                return true;
            }
            return false;
        }
        //分支end
        if (left >= term2 && left <= term3) {
            return true;
        }
        
        return false;
    }
    public virtual  bool GetSkillTarget(SoldierSkill Skill)
    {
        return false;
    }
    //获取攻击目标
    public virtual bool GetAttackTarget(ref int sceneid)
    {
    
        return false;
    }
    
    public virtual  bool GetBigAttackTarget()
    {
        return false;
    }
    public bool CheckAttackTarget()
    {
        return false;
    }
    /*public bool CheckDoAttackTarget(int id)
    {
    	return CheckDoAttackTarget(CM.GetAllLifeM(id ,LifeMType.ALL),PropAttackSkillInfo.m_sort,PropAttackSkillInfo.m_distance/MapGrid.m_Pixel);
    }*/
    
    public virtual bool CanAttack()
    {
        return false;
    }
    
    /// <summary>
    /// 技能免疫判断
    /// </summary>
    /// <param name="AttackSkillType">攻击方技能类型</param>
    /// <param name="DefanseStatus">防御方状态</param>
    /// <returns></returns>
    public bool ImmunitySkill(AttackType AttackSkillType, int SkillType, Status DefanseStatus)
    {
        if (DefanseStatus == null) {
            return false;
        }
        
        //是否免疫某类技能
        return DefanseStatus.ImmunitySkill(AttackSkillType, SkillType);
    }
    
    /// <summary>
    /// 技能释放
    /// </summary>
    /// <param name="Attack">技能释放者</param>
    /// <param name="Defense">受击者</param>
    /// <param name="DefenseAction">防守方，攻击处于的状态</param>
    /// <returns>技能伤害值</returns>
    public  SkillReleaseInfo SkillRelease(Life Attack, Life Defense, GridActionCmd DefenseAction, SkillInfo skill)
    {
        //异常处理
        AttackResult Result = AttackResult.Normal;
        SkillReleaseInfo Info = new SkillReleaseInfo();
        Info.m_InterruptSkill = false;
        Info.m_MakeStatus = new List<StatusType> ();
        Info.m_bImmunity = false;
        Info.m_Damage = 0;
        Info.m_struckeffect = "";
        Info.Result = AttackResult.Normal;
        Info.HitAttributeType = skill.m_AttributeType;
        if (Attack == null || Defense == null || skill == null) {
            Debug.Log(Attack + "," + Defense + "," + skill.m_name);
            return Info;
        }
        if (Defense is Role && (Defense as Role).NoDie) {
            return Info;
        }
        //处于攻击中（过了前要阶段）
        /*if(DefenseAction != null && DefenseAction is GridActionCmdAttack)
        {
        	if((DefenseAction as GridActionCmdAttack).IsPlayed() == true)
        	return Info;
        }*/
        Info.HitType = (AttackType) skill.m_attacktype;
        Info.m_struckeffect = skill.m_struckeffect;
        //判断是否技能免疫 ，免疫技能不受到伤害及状态。
        AttackType Type = (AttackType)skill.m_attacktype;
        bool ret = ImmunitySkill(Type, skill.m_type, Defense.m_Status);
        if (ret == true) {
            Info.m_bImmunity = true;
            Info.Result = AttackResult.Immunity;
            return Info;
        }
        //根据m_interrupt_skill 判断受击方技能是否打断
        if (skill.m_interrupt_skill == 1) {
            if (skill is SoldierSkill) {
                float random = Random.Range(0, 1f);
                if (Defense.m_Attr.Level <= (skill as SoldierSkill).m_rankslevel || random <= (skill as SoldierSkill).m_status_hitratio) {
                    Info.m_InterruptSkill = true;
                }
            } else {
                Info.m_InterruptSkill = true;
            }
        }
        
        if (Attack.m_Attr == null || Defense.m_Attr == null) {
            return Info;
        }
        //计算技能伤害
        int Damage = 0;
        int regenhp = 0;
        if (Attack.m_Core.m_Camp == Defense.m_Core.m_Camp) {
            regenhp = CalcSkillRegenHP(Attack.m_Attr, Defense.m_Attr, ref  Result, skill);
        } else {
            Damage = CalcSkillDamage(Attack.m_Attr, Defense.m_Attr, ref  Result, skill);
        }
        Damage = CalcDistanInfo(Attack.m_Attr, Defense.m_Attr, Damage);
        Damage = CalcAttributableSkillDamage(skill.m_AttributeType, Defense.m_Attr, Damage);
        //计算技能治疗量
        //盾抵抗伤害
        if (Defense.m_Status != null) {
            Damage = Damage - Defense.m_Status.ReduceHpbyShield((AttackType)skill.m_attacktype, Damage) ;
        }
        //miss 不产生状态
        if (Result == AttackResult.Miss) {
            Info.Result = AttackResult.Miss;
            return Info;
        } else if (Result == AttackResult.Crit) {
            Info.Result = AttackResult.Crit;
        }
        
        if ((Defense.m_Attr.Hp - Damage) > 0) {
            //产生debuff，根据ememy_status,判断debuff中是包含打断技能状态
            if (StatusDeBuff(Defense, skill) == true) {
                Info.m_InterruptSkill = true;
            }
            //魅惑状态下的BUFF效果无效
            if (!Attack.m_Attr.Charmed && StatusBuff(Defense, skill) == true) {
                Info.m_InterruptSkill = true;
            }
            if (Defense is Building) {
                StatusBuildDeBuff(Defense);
            }
        }
        
        
        
        
        //else Damage = Defense.HP;
        //判断受击方受到的伤害值，是否大于可被打断伤害值。
        int LimitHp = Defense.m_Attr.Hp * ConfigM.GetSkillData0() / 100;
        if (Damage > LimitHp) {
            Info.m_InterruptSkill = true;
        }
        if (Attack.m_Attr.Vampire > 0) {
            int vampriehp = CalcVampireHP(Attack.m_Attr, Damage);
            Attack.HP += vampriehp;
            Attack.StatusUpdateHp(vampriehp);
        }
        
        if (Attack.m_Core.m_Camp == Defense.m_Core.m_Camp) {
            Info.m_Damage = regenhp;
        } else {
            Info.m_Damage = -Damage;
        }
        
        return Info;
    }
    
    
    //确认是否群攻
    static public void CheckMultiple(ref List<Life> rolelist, SkillInfo skill)
    {
        if (skill.m_multiple <= 0) {
            return;
        } else {
            while (rolelist.Count > skill.m_multiple) {
                rolelist.RemoveAt(rolelist.Count - 1);
            }
        }
    }
    
    
    
    
    static public LifeMCamp GetSkillCamp(SkillInfo skill, Life owner)
    {
        LifeMCamp camp = owner.m_Core.m_Camp == LifeMCamp.ATTACK ? LifeMCamp.DEFENSE : LifeMCamp.ATTACK;
        if (skill.m_target == (int)SkillTarget.SomeOneFriendly || skill.m_target == (int)SkillTarget.Self ||
            skill.m_target == (int)SkillTarget.FriendlyGround || skill.m_target == (int)SkillTarget.FriendlyTeam) {
            camp = owner.m_Core.m_Camp;
        }
        if (owner.m_Attr.Charmed && skill.m_target != 1) {
            camp = camp == LifeMCamp.ATTACK ? LifeMCamp.DEFENSE : LifeMCamp.ATTACK;
        }
        return camp;
    }
    
    static public void GetWetTarget(ref List<Life> targetlist, SkillInfo skill, Life own)
    {
    
        float radius = ConfigM.GetWetBodyRange() * 0.01f;
        List<Life> lr = new List<Life>();
        LifeMCamp camp = GetSkillCamp(skill, own);
        CM.SearchLifeMListInBoat(ref lr, LifeMType.SOLDIER, camp);
        for (int i = 0; i < targetlist.Count; i++) {
            Life l = targetlist[i];
            foreach (Role r in lr) {
                if (NdUtil.IsSameMapLayer(l.MapPos, r.MapPos)) {
                    if (r.m_thisT.localPosition.x >= (l.m_thisT.localPosition.x - radius) && r.m_thisT.localPosition.x <= (l.m_thisT.localPosition.x + radius)) {
                        if (r.m_Attr.IsWetBody) {
                            if (!targetlist.Contains(r)) {
                                targetlist.Add(r);
                            }
                        }
                    }
                }
            }
        }
    }
    
    public virtual int GetAuraAffector(EffectType Type, Life l, LifeMCamp camp)
    {
        return 0;
    }
    /*public static int GetAllAuraAffector(EffectType Type, Life l, LifeMCamp camp)
    {
    	int ret = 0;
    	for (int i = 0; i < AuraSkill.Count;)
    	{
    		if (AuraSkill[i] is RoleSkill)
    		{
    			RoleSkill roleskill = AuraSkill[i] as RoleSkill;
    			ret += roleskill.GetAuraAffector(Type,l,camp);
    		}
    		else
    			AuraSkill.RemoveAt(i);
    	}
    	return ret;
    }*/
}
