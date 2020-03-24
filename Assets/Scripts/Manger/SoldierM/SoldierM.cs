using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using sdata;

/// <summary>
/// 炮弹兵数据
/// </summary>
public class SoldierM
{

    private static List<s_soldierqualityInfo> m_soldierQualityEquips = new List<s_soldierqualityInfo>();
    private static List<s_soldierstar_typeInfo> m_soldierStarType = new List<s_soldierstar_typeInfo>();
    private static List<s_soldier_experienceInfo> m_lexp = new List<s_soldier_experienceInfo>();
    private static List<s_soldier_typeInfo> m_soldierType = new List<s_soldier_typeInfo>();
    private static List<s_skill_costInfo> m_lSkillCost = new List<s_skill_costInfo>();
    
    private static bool _IsLoad = false;
    
    public static void Init(object obj)
    {
        if (_IsLoad == true) {
            return;
        }
        System.Diagnostics.Debug.Assert(obj is sdata.StaticDataResponse);
        StaticDataResponse sdrsp = obj as StaticDataResponse;
        
        m_soldierQualityEquips = sdrsp.s_soldierquality_info;
        m_soldierStarType = sdrsp.s_soldierstar_type_info;
        m_lexp = sdrsp.s_soldier_experience_info;
        m_soldierType = sdrsp.s_soldier_type_info;
        m_lSkillCost = sdrsp.s_skill_cost_info ;
        _IsLoad = true;
    }
    
    
    private static s_soldierqualityInfo GetqualityInfoData(int soldierTypeid, int quality)
    {
        foreach (s_soldierqualityInfo v in m_soldierQualityEquips) {
            if (v.soldiertypeid == soldierTypeid && v.quality == quality) {
                return v;
            }
        }
        return null;
    }
    
    
    
    private static  s_skill_costInfo GetSoldierSkillLevel(int SkillLevel)
    {
        foreach (s_skill_costInfo v in m_lSkillCost) {
            if (v.skill_level == SkillLevel) {
                return v;
            }
        }
        return null;
    }
    /// <summary>
    /// 取得该炮弹兵星级初始星级.
    /// </summary>
    /// <returns>The soldier star level.</returns>
    /// <param name="soldier_id">Soldier_id.</param>
    /// <param name="itemNum">Item number.</param>
    public static int GetSoldierStarLevel(int soldier_id)
    {
        foreach (s_soldier_typeInfo v in m_soldierType) {
            if (v.id == soldier_id) {
                return v.starlevel;
            }
        }
        return 1;
    }
    /// <summary>
    /// 获取物品不是关卡掉落时，其他来源描述.
    /// </summary>
    /// <returns>The hero drop desc.</returns>
    /// <param name="soldier_id">Soldier_id.</param>
    public static string GetHeroDropDesc(int soldier_id)
    {
        foreach (s_soldier_typeInfo v in m_soldierType) {
            if (v.id == soldier_id) {
                return v.dropdes;
            }
        }
        return "";
    }
    
    /// <summary>
    /// QFord 获取炮弹兵在指定星级的静态数据
    /// </summary>
    /// <param name="soldier_id"></param>
    /// <param name="starlevel"></param>
    /// <returns></returns>
    public static  s_soldierstar_typeInfo GetSoldierStarData(int soldier_id, int starlevel)
    {
        foreach (s_soldierstar_typeInfo v in m_soldierStarType) {
            if (v.soldiertypeid == soldier_id && v.starlevel == starlevel) {
                return v;
            }
        }
        return null;
    }
    /// <summary>
    /// 获取炮弹兵最小星级.
    /// </summary>
    /// <returns>The soldier original star.</returns>
    /// <param name="soldier_id">Soldier_id.</param>
    public static int GetSoldierOriginalStar(int soldier_id)
    {
        int star = 5;
        foreach (s_soldierstar_typeInfo v in m_soldierStarType) {
            if (v.soldiertypeid == soldier_id) {
                if (star >= v.starlevel) {
                    star = v.starlevel;
                }
            }
        }
        return star;
    }
    
    /// <summary>
    /// QFord 根据ItemType 获取炮弹兵ID
    /// </summary>
    /// <param name="soldier_id"></param>
    /// <param name="starlevel"></param>
    /// <returns></returns>
    public static  int GetSoldierStarID(int itemType)
    {
        foreach (s_soldier_typeInfo v in m_soldierType) {
            if (v.fragment == itemType) {
                return v.id;
            }
        }
        return 0;
    }
    
    public static s_soldier_experienceInfo GetSoldierExpData(int level)
    {
    
        foreach (sdata.s_soldier_experienceInfo v in m_lexp) {
            if (v.level == level) {
                return v;
            }
        }
        
        return null;
    }
    public static int   GetSoldierExp(int level)
    {
    
        foreach (sdata.s_soldier_experienceInfo v in m_lexp) {
            if (v.level == level) {
                return v.exp;
            }
        }
        
        return 0;
    }
    
    public static int GetSoldierTotalExpAtLevel(int level)
    {
        int nExp = 0;
        
        foreach (sdata.s_soldier_experienceInfo v in m_lexp) {
            if (v.level <= level) {
                nExp += v.exp;
            }
        }
        
        return nExp;
    }
    
    public static s_soldier_typeInfo GetSoldierType(int SoldierTypeID)
    {
        foreach (s_soldier_typeInfo v in m_soldierType) {
            if (v.id == SoldierTypeID) {
                return v;
            }
        }
        return null;
    }
    /// <summary>
    /// 获取所有的炮弹兵
    /// </summary>
    public static List<SoldierInfo> GetAllSoldier()
    {
        List<SoldierInfo> l = new List<SoldierInfo>();
        foreach (s_soldier_typeInfo BaseInfo in m_soldierType) {
            if (BaseInfo == null) {
                continue;
            }
            if (BaseInfo.id >= 900000) {
                continue;
            }
            SoldierInfo Info = new SoldierInfo();
            Info.ID = BaseInfo.id;
            Info.SoldierTypeID = BaseInfo.id;
            Info.Level = 1;
            Info.StarLevel = BaseInfo.starlevel;
            Info.EXP = 0;
            Info.Quality = 10;
            Info.CX = 0;
            Info.CY = 0;
            //装备
            Info.Equipment0 = 0;
            Info.Equipment1 = 0;
            Info.Equipment2 = 0;
            Info.Equipment3 = 0;
            Info.Equipment4 = 0;
            Info.Equipment5 = 0;
            //技能
            List<int> lSkillLevel = new List<int>();
            lSkillLevel.Add(1);
            lSkillLevel.Add(1);
            lSkillLevel.Add(1);
            lSkillLevel.Add(1);
            lSkillLevel.Add(1);
            lSkillLevel.Add(1);
            Info.m_Skill.SetSkillLevel(lSkillLevel);
            
            GetSoldierInfo(ref Info) ;
            
            l.Add(Info);
        }
        return l;
    }
    
    /// <summary>
    /// 获取炮弹兵信息
    /// </summary>
    /// <param name="Info">炮弹兵数据</param>
    public static SoldierInfo GetSoldierInfo(int SoldierID, s_itemtypeInfo I)
    {
        if (I == null) {
            return null;
        }
        SoldierInfo Info = new SoldierInfo();
        Info.ID = SoldierID;
        Info.SoldierTypeID = I.gid;
        Info.Level = I.level;
        Info.StarLevel = 1;
        Info.EXP = 0;
        Info.Quality = I.quality;
        Info.CX = 0;
        Info.CY = 0;
        //装备
        Info.Equipment0 = 0;
        Info.Equipment1 = 0;
        Info.Equipment2 = 0;
        Info.Equipment3 = 0;
        Info.Equipment4 = 0;
        Info.Equipment5 = 0;
        //技能
        List<int> lSkillLevel = new List<int>();
        lSkillLevel.Add(I.level);
        lSkillLevel.Add(I.level);
        lSkillLevel.Add(I.level);
        lSkillLevel.Add(I.level);
        lSkillLevel.Add(I.level);
        lSkillLevel.Add(I.level);
        Info.m_Skill.SetSkillLevel(lSkillLevel);
        
        GetSoldierInfo(ref Info);
        
        return Info;
    }
    public static SoldierInfo GetSoldierInfo(treasure.TreasureObjInfo I)
    {
        if (I == null) {
            return null;
        }
        
        SoldierInfo Info = new SoldierInfo();
        Info.ID = I.objid;
        Info.SoldierTypeID = I.soldiertypeid;
        Info.Level = I.level;
        Info.StarLevel = I.starlevel;
        Info.Quality = I.quality;
        Info.m_typedes = I.type;
        List<int> lSkillLevel = new List<int>();
        Info.m_Skill.SetSkillLevel(lSkillLevel);
        
        GetSoldierInfo(ref Info);
        Info.CX = I.cx;
        Info.CY = I.cy;
        Info.m_hp = I.hp;
        Info.m_mp = I.mp;
        
        return Info;
    }
    /// <summary>
    /// 获取炮弹兵信息
    /// </summary>
    /// <param name="Info">炮弹兵数据</param>
    public static SoldierInfo GetSoldierInfo(soldier.SoldierInfo I)
    {
        if (I == null) {
            return null;
        }
        
        SoldierInfo Info = new SoldierInfo();
        Info.ID = I.id;
        Info.SoldierTypeID = I.soldiertypeid;
        Info.Level = I.level;
        Info.StarLevel = I.starlevel;
        Info.EXP = I.exp;
        Info.Quality = I.quality;
        Info.CX = 0;
        Info.CY = 0;
        //装备
        Info.Equipment0 = I.equipment0;
        Info.Equipment1 = I.equipment1;
        Info.Equipment2 = I.equipment2;
        Info.Equipment3 = I.equipment3;
        Info.Equipment4 = I.equipment4;
        Info.Equipment5 = I.equipment5;
        //技能
        List<int> lSkillLevel = new List<int>();
        lSkillLevel.Add(I.askill_level);
        lSkillLevel.Add(I.skill1_level);
        lSkillLevel.Add(I.skill2_level);
        lSkillLevel.Add(I.skill3_level);
        lSkillLevel.Add(I.skill4_level);
        lSkillLevel.Add(I.skill5_level);
        Info.m_Skill.SetSkillLevel(lSkillLevel);
        
        GetSoldierInfo(ref Info);
        
        return Info;
    }
    
    /// <summary>
    /// 获取炮弹兵信息
    /// </summary>
    /// <param name="Info">炮弹兵数据</param>
    public static void UpdateSoldierInfo(ref SoldierInfo Info, soldier.SoldierInfo I)
    {
        if (I == null || Info == null) {
            return ;
        }
        Info.ID = I.id;
        Info.SoldierTypeID = I.soldiertypeid;
        Info.Level = I.level;
        Info.StarLevel = I.starlevel;
        Info.EXP = I.exp;
        Info.Quality = I.quality;
        //装备
        Info.Equipment0 = I.equipment0;
        Info.Equipment1 = I.equipment1;
        Info.Equipment2 = I.equipment2;
        Info.Equipment3 = I.equipment3;
        Info.Equipment4 = I.equipment4;
        Info.Equipment5 = I.equipment5;
        //技能
        if (I.askill_level != -1 || I.skill1_level != -1
            || I.skill2_level != -1 || I.skill3_level != -1
            || I.skill4_level != -1 || I.skill5_level != -1) {
            List<int> lSkillLevel = new List<int>();
            lSkillLevel.Add(I.askill_level);
            lSkillLevel.Add(I.skill1_level);
            lSkillLevel.Add(I.skill2_level);
            lSkillLevel.Add(I.skill3_level);
            lSkillLevel.Add(I.skill4_level);
            lSkillLevel.Add(I.skill5_level);
            Info.m_Skill.SetSkillLevel(lSkillLevel);
        }
        GetSoldierInfo(ref Info);
    }
    
    
    /// <summary>
    /// 获取炮弹兵信息
    /// </summary>
    /// <param name="Info">炮弹兵数据</param>
    public static SoldierInfo GetSoldierInfo(battle.SoldierInfo I)
    {
        if (I == null) {
            return null;
        }
        
        SoldierInfo Info = new SoldierInfo();
        Info.ID = I.id;
        Info.SoldierTypeID = I.soldiertypeid;
        Info.Level = I.level;
        Info.StarLevel = I.starlevel;
        Info.EXP = I.exp;
        Info.Quality = I.quality;
        Info.CX = I.cx;
        Info.CY = I.cy;
        //装备
        Info.Equipment0 = I.equipment0;
        Info.Equipment1 = I.equipment1;
        Info.Equipment2 = I.equipment2;
        Info.Equipment3 = I.equipment3;
        Info.Equipment4 = I.equipment4;
        Info.Equipment5 = I.equipment5;
        //技能
        List<int> lSkillLevel = new List<int>();
        lSkillLevel.Add(I.askill_level);
        lSkillLevel.Add(I.skill1_level);
        lSkillLevel.Add(I.skill2_level);
        lSkillLevel.Add(I.skill3_level);
        lSkillLevel.Add(I.skill4_level);
        lSkillLevel.Add(I.skill5_level);
        Info.m_Skill.SetSkillLevel(lSkillLevel);
        
        GetSoldierInfo(ref Info);
        
        return Info;
    }
    /// <summary>
    /// 获取炮弹兵信息
    /// </summary>
    /// <param name="Info">炮弹兵数据</param>
    public static SoldierInfo GetSoldierInfo(s_monsterInfo monster)
    {
        if (monster == null) {
            return null;
        }
        SoldierInfo I =  new SoldierInfo();
        //
        I.m_name = monster.name;
        I.ID = monster.id;
        I.SoldierTypeID = monster.id;
        I.m_modeltype = monster.modeid;
        I.m_soldier_type = monster.type;
        I.Level = monster.level;
        I.StarLevel = monster.star;
        I.Quality = monster.quality;
        I.m_hp = monster.hp;
        I.m_phy_attack = monster.physical_attack;
        I.m_phy_defend = monster.physical_defend;
        I.m_phy_crit = monster.physical_crit;
        I.m_magic_attack = monster.magic_attack;
        I.m_magic_defend = monster.magic_defend;
        I.m_dodge = monster.dodge;
        I.m_Flyspeed = 1.0f *  monster.flyspeed / MapGrid.m_Pixel;
        I.m_attack_like = monster.attack_like;
        I.m_attack_type = monster.attacktype;
        I.m_speed = 1.0f * monster.speed / MapGrid.m_Pixel;
        I.m_jump_distance = monster.jump_distance / MapGrid.m_Pixel;
        I.m_resist = monster.resist;
        I.m_attack_time = monster.attack_time * 0.001f;
        I.m_desc = monster.description;
        I.m_FireAI = monster.fireai ;
        I.m_shape = monster.shape ;
        I.m_goorder = monster.goorder;
        I.m_concussion = monster.concussion;
        I.m_dead_mp = monster.dead_mp;
        //附加属性
        //主附加属性
        I.m_AddAttr.SetAddAttrInfo(monster);
        //被动技能附加属性
        I.m_Skill.SetSkill(monster);
        I.m_Skill.SetAddrData(ref I.m_AddAttr);
        SetAddAttr(ref I);
        //爆击及闪避
        s_soldier_experienceInfo ExpInfo = SoldierM.GetSoldierExpData(I.Level);
        if (ExpInfo != null) {
            I.m_critratio = ExpInfo.critratio;
            I.m_dodgeratio = ExpInfo.dodgeratio;
        }
        I.m_combat_power = monster.power;
        
        I.m_hpstart = I.m_hp;
        I.m_phy_attackstart =  I.m_phy_attack;
        I.m_phy_defendstart = I.m_phy_defend;
        I.m_phy_critstart = I.m_phy_crit;
        
        I.m_magic_attackstart  = I.m_magic_attack;
        I.m_magic_defendstart  = I.m_magic_defend;
        I.m_magic_critstart  = I.m_magic_crit;
        I.m_strengthstart = I.m_strength;
        I.m_agilitystart = I.m_agility;
        I.m_intelligencestart = I.m_intelligence;
        return I;
    }
    
    
    /// <summary>
    /// 升星
    /// </summary>
    /// <param name="Info">炮弹兵数据</param>
    public static void UpStarSoldier(ref SoldierInfo Info, int StarLevel)
    {
        if (Info == null) {
            return ;
        }
        
        Info.StarLevel = StarLevel;
        
        UpdataSoldier(ref Info);
    }
    
    
    /// <summary>
    /// 升阶
    /// </summary>
    /// <param name="Info">炮弹兵数据</param>
    public static void UpQualitySoldier(ref SoldierInfo Info, int Quality)
    {
        if (Info == null) {
            return ;
        }
        
        Info.Quality = Quality;
        
        UpdataSoldier(ref Info);
    }
    
    
    
    /// <summary>
    /// 获取炮弹兵数据
    /// </summary>
    /// <param name="Info">炮弹兵类型</param>
    /// <returns>建筑数据,null 获取失败</returns>
    public static void GetSoldierInfo(ref SoldierInfo Info)
    {
        if (Info == null) {
            return ;
        }
        s_soldier_typeInfo BaseInfo = GetSoldierType(Info.SoldierTypeID);
        if (BaseInfo == null) {
            NGUIUtil.DebugLog("获取不到该类型的炮弹兵数据：" + Info.SoldierTypeID);
            return ;
        }
        FillBaseSoldierInfo(BaseInfo, ref Info);
        //设置炮弹兵技能
        SetSoldierSkill(BaseInfo, ref Info);
        //计算升级，升星，升阶 数据。
        CalcSoldierInfo(BaseInfo, ref Info) ;
        if (Info.SoldierTypeID == 102003) {
            Info.m_TurnInfo = new SoldierInfo();
            Info.m_TurnInfo.Copy(Info);
            Info.m_TurnInfo.SoldierTypeID = 902003;
            BaseInfo = GetSoldierType(Info.m_TurnInfo.SoldierTypeID);
            if (BaseInfo == null) {
                NGUIUtil.DebugLog("获取不到该类型的炮弹兵数据：" + Info.m_TurnInfo.SoldierTypeID);
                return ;
            }
            FillBaseSoldierInfo(BaseInfo, ref Info.m_TurnInfo);
            //设置炮弹兵技能
            SetSoldierSkill(BaseInfo, ref Info.m_TurnInfo);
            //计算升级，升星，升阶 数据。
            CalcSoldierInfo(BaseInfo, ref Info.m_TurnInfo) ;
        }
    }
    
    public static SoldierInfo GetSoldierInfo(int soldierTypeID)
    {
        SoldierInfo info = new SoldierInfo();
        info.SoldierTypeID = soldierTypeID;
        GetSoldierInfo(ref info);
        return info;
    }
    
    /// <summary>
    /// 更新炮弹兵数据，星级，等级，阶数的变更。
    /// </summary>
    /// <param name="Info">炮弹兵数据</param>
    public static void UpdataSoldier(ref SoldierInfo Info)
    {
        if (Info == null) {
            return ;
        }
        s_soldier_typeInfo BaseInfo = GetSoldierType(Info.SoldierTypeID);
        if (BaseInfo == null) {
            NGUIUtil.DebugLog("获取不到该类型的炮弹兵数据：" + Info.SoldierTypeID);
            return ;
        }
        //计算升级，升星，升阶 数据。
        CalcSoldierInfo(BaseInfo, ref Info) ;
    }
    
    
    
    
    /// <summary>
    /// 获取初级建筑数据
    /// </summary>
    /// <param name="Type">炮弹兵类型</param>
    /// <returns>炮弹兵数据,null 获取失败</returns>
    public static SoldierInfo GetStartSoldierInfo(int Type)
    {
        s_soldier_typeInfo BaseInfo = GetSoldierType(Type);
        if (BaseInfo == null) {
            NGUIUtil.DebugLog("获取不到该类型的建筑数据：" + Type);
            return null;
        }
        SoldierInfo Info = new SoldierInfo();
        Info.ID = 0;
        Info.SoldierTypeID = Type;
        Info.Level = 1;
        Info.StarLevel = BaseInfo.starlevel;
        Info.EXP = 0;
        Info.Quality = 10;
        Info.CX = 0;
        Info.CY = 0;
        //装备
        Info.Equipment0 = 0;
        Info.Equipment1 = 0;
        Info.Equipment2 = 0;
        Info.Equipment3 = 0;
        Info.Equipment4 = 0;
        Info.Equipment5 = 0;
        //技能
        List<int> lSkillLevel = new List<int>();
        lSkillLevel.Add(1);
        lSkillLevel.Add(1);
        lSkillLevel.Add(1);
        lSkillLevel.Add(1);
        lSkillLevel.Add(1);
        lSkillLevel.Add(1);
        Info.m_Skill.SetSkillLevel(lSkillLevel);
        
        GetSoldierInfo(ref Info) ;
        
        return Info;
    }
    /// <summary>
    /// 填充基础炮弹兵数据
    /// </summary>
    private  static void FillBaseSoldierInfo(s_soldier_typeInfo I, ref SoldierInfo Info)
    {
        if (I == null || Info == null) {
            return;
        }
        
        Info.m_resist = I.resist;
        Info.m_attack_like = I.attack_like;
        Info.m_attack_type = I.attacktype;
        Info.m_speed = 1.0f * I.speed / MapGrid.m_Pixel;
        Info.m_Flyspeed = 1.0f * I.flyspeed / MapGrid.m_Pixel;
        Info.m_attack_time = I.attack_time * 0.01f;
        Info.m_jump_distance = I.jump_distance / MapGrid.m_Pixel;
        
        Info.m_hpstart = I.hp;
        Info.m_phy_attackstart =  I.phy_attack;
        Info.m_phy_defendstart = I.phy_defend;
        Info.m_phy_critstart = I.phy_crit;
        
        Info.m_magic_attackstart  = I.magic_attack;
        Info.m_magic_defendstart  = I.magic_defend;
        Info.m_magic_critstart  = I.magic_crit;
        Info.m_strengthstart = I.strength;
        Info.m_agilitystart = I.agility;
        Info.m_intelligencestart = I.intelligence;
        
        Info.m_dodge = I.dodge;
        Info.m_soldier_group = I.group;
        Info.m_soldier_type = I.type;
        Info.m_main_proerty = I.main_proerty;
        Info.m_loaction = I.location;
        Info.m_modeltype = I.modeltype;
        Info.m_name = I.name;
        Info.m_mp = I.mp;
        Info.m_passageway  = I.passageway;
        Info.m_concussion = I.concussion;
        Info.m_desc = I.description;
        Info.m_shape = I.shape;
        Info.m_goorder = I.goorder;
        Info.fragmentTypeID = I.fragment;
        //附加属性
        Info.m_AddAttr.SetAddAttrInfo(I);
        
        
        Info.m_hp = I.hp;
        Info.m_phy_attack =  I.phy_attack;
        Info.m_phy_defend = I.phy_defend;
        Info.m_phy_crit = I.phy_crit;
        
        Info.m_magic_attack  = I.magic_attack;
        Info.m_magic_defend  = I.magic_defend;
        Info.m_magic_crit  = I.magic_crit;
        Info.m_strength = I.strength;
        Info.m_agility = I.agility;
        Info.m_intelligence = I.intelligence;
        Info.m_strDieSound = I.dead_sound;
        Info.m_strDropDesc = I.dropdes;
    }
    
    /// <summary>
    /// 填充基础建筑信息
    /// </summary>
    private static void CalcSoldierInfo(s_soldier_typeInfo I, ref SoldierInfo Info)
    {
        if (I == null || Info == null) {
            return ;
        }
        //获取三围
        //初始值
        s_soldierstar_typeInfo StarInfo = GetSoldierStarData(Info.SoldierTypeID, Info.StarLevel);
        if (StarInfo != null) {
            Info.m_strength_grow = StarInfo.strength_growth;
            Info.m_agility_grow = StarInfo.agility_growth;
            Info.m_intelligence_grow = StarInfo.intelligence_growth;
            Info.m_concussion += StarInfo.concussion;
        }
        
        
        //当前-初始.
        int oriStrength = 0;
        int oriAgility = 0;
        int oriIntell = 0;
        
        int oriStar = GetSoldierOriginalStar(Info.SoldierTypeID);
        s_soldierstar_typeInfo OriInfo = GetSoldierStarData(Info.SoldierTypeID, oriStar);
        if (OriInfo != null && StarInfo != null) {
            oriStrength = OriInfo.strength_growth;
            oriAgility = OriInfo.agility_growth;
            oriIntell = OriInfo.intelligence_growth;
        }
        //被动技能附加属性
        Info.m_Skill.SetAddrData(ref Info.m_AddAttr);
        
        Info.m_strength = ScriptM.Formula<int>("CALC_STRENGTH", Info.m_strength_grow, oriStrength, Info.Level, I.strength);
        Info.m_agility = ScriptM.Formula<int>("CALC_AGILITY", Info.m_agility_grow, oriAgility, Info.Level, I.agility);
        Info.m_intelligence = ScriptM.Formula<int>("CALC_INTELLIGENCE", Info.m_intelligence_grow, oriIntell, Info.Level, I.intelligence);
        //增加进阶属性,目前只有冲击力
        s_soldierqualityInfo qualityinfo = GetqualityInfoData(Info.SoldierTypeID, Info.Quality);
        if (qualityinfo != null) {
            Info.m_concussion += qualityinfo.concussion;
        }
        //爆击及闪避
        s_soldier_experienceInfo ExpInfo = SoldierM.GetSoldierExpData(Info.Level);
        if (ExpInfo != null) {
            Info.m_critratio = ExpInfo.critratio;
            Info.m_dodgeratio = ExpInfo.dodgeratio;
        }
        //阶数，添加装备附加属性
        AddEquipAttr(ref Info);
        CalcSoldierAttr(ref Info);
    }
    
    /// <summary>
    /// 计算属性
    /// </summary>
    private static void CalcSoldierAttr(ref SoldierInfo Info)
    {
        if (Info == null) {
            return ;
        }
        //添加附加三围属性
        if (Info.m_AddAttr != null) {
            Info.m_strength += Info.m_AddAttr.GetAttr(EffectType.Strength);
            Info.m_agility += Info.m_AddAttr.GetAttr(EffectType.Agility);
            Info.m_intelligence += Info.m_AddAttr.GetAttr(EffectType.Intelligence);
            
            Info.m_AddAttr.RemoveAttr(EffectType.Strength);
            Info.m_AddAttr.RemoveAttr(EffectType.Agility);
            Info.m_AddAttr.RemoveAttr(EffectType.Intelligence);
        }
        
        
        int AddStrength = Info.m_strength - Info.m_strengthstart;
        int AddAgility = Info.m_agility - Info.m_agilitystart;
        int AddIntelligence = Info.m_intelligence - Info.m_intelligencestart;
        Info.m_hp =  ScriptM.Formula<int>("CALC_HP", Info.m_hpstart, AddStrength);
        Info.m_phy_attack = ScriptM.Formula<int>("CALC_SOLDIER_PHYATTACK", Info.m_phy_attackstart, Info.m_main_proerty, AddStrength, AddAgility, AddIntelligence);
        Info.m_phy_defend = ScriptM.Formula<int>("CALC_SOLDIER_PHYDEFEND", Info.m_phy_defendstart, AddStrength, AddAgility);
        Info.m_magic_attack = ScriptM.Formula<int>("CALC_SOLDIER_MAGICATTACK", Info.m_magic_attackstart, AddIntelligence);
        Info.m_magic_defend = ScriptM.Formula<int>("CALC_SOLDIER_MAGICDEFEND", Info.m_magic_defendstart, AddIntelligence);
        Info.m_phy_crit = ScriptM.Formula<int>("CALC_SOLDIER_PHYCRIT", Info.m_phy_critstart, AddAgility);
        Info.m_magic_crit = ScriptM.Formula<int>("CALC_SOLDIER_MAGICCRIT", Info.m_magic_critstart, AddIntelligence);
        //添加非附加三围属性
        if (Info.m_AddAttr != null) {
            Info.m_hp += Info.m_AddAttr.GetAttr(EffectType.Hp);
            Info.m_mp += Info.m_AddAttr.GetAttr(EffectType.Anger);
            Info.m_phy_attack += Info.m_AddAttr.GetAttr(EffectType.PhyAttack);
            Info.m_phy_defend += Info.m_AddAttr.GetAttr(EffectType.PhyDefense);
            Info.m_magic_attack += Info.m_AddAttr.GetAttr(EffectType.MagicAttack);
            Info.m_magic_defend += Info.m_AddAttr.GetAttr(EffectType.MagicDefense);
            Info.m_hit += Info.m_AddAttr.GetAttr(EffectType.Hit);
            Info.m_dodge += Info.m_AddAttr.GetAttr(EffectType.Dodge);
            Info.m_phy_crit += Info.m_AddAttr.GetAttr(EffectType.PhyCrit);
            Info.m_magic_crit += Info.m_AddAttr.GetAttr(EffectType.MagicCrit);
            
            Info.m_AddAttr.RemoveAttr(EffectType.Hp);
            Info.m_AddAttr.RemoveAttr(EffectType.Anger);
            Info.m_AddAttr.RemoveAttr(EffectType.PhyAttack);
            Info.m_AddAttr.RemoveAttr(EffectType.PhyDefense);
            Info.m_AddAttr.RemoveAttr(EffectType.MagicAttack);
            Info.m_AddAttr.RemoveAttr(EffectType.MagicDefense);
            Info.m_AddAttr.RemoveAttr(EffectType.Hit);
            Info.m_AddAttr.RemoveAttr(EffectType.Dodge);
            Info.m_AddAttr.RemoveAttr(EffectType.PhyCrit);
            Info.m_AddAttr.RemoveAttr(EffectType.MagicCrit);
        }
        //计算战斗力
        CalcSoldierPower(ref Info);
    }
    /// <summary>
    /// 设置附加属性
    /// </summary>
    /// <param name="Info">Info.</param>
    private static void SetAddAttr(ref SoldierInfo Info)
    {
        if (Info == null || Info.m_AddAttr == null) {
            return ;
        }
        Info.m_strength += Info.m_AddAttr.GetAttr(EffectType.Strength);
        Info.m_agility += Info.m_AddAttr.GetAttr(EffectType.Agility);
        Info.m_intelligence += Info.m_AddAttr.GetAttr(EffectType.Intelligence);
        Info.m_hp += Info.m_AddAttr.GetAttr(EffectType.Hp);
        Info.m_mp += Info.m_AddAttr.GetAttr(EffectType.Anger);
        Info.m_phy_attack += Info.m_AddAttr.GetAttr(EffectType.PhyAttack);
        Info.m_phy_defend += Info.m_AddAttr.GetAttr(EffectType.PhyDefense);
        Info.m_magic_attack += Info.m_AddAttr.GetAttr(EffectType.MagicAttack);
        Info.m_magic_defend += Info.m_AddAttr.GetAttr(EffectType.MagicDefense);
        Info.m_hit += Info.m_AddAttr.GetAttr(EffectType.Hit);
        Info.m_dodge += Info.m_AddAttr.GetAttr(EffectType.Dodge);
        Info.m_phy_crit += Info.m_AddAttr.GetAttr(EffectType.PhyCrit);
        Info.m_magic_crit += Info.m_AddAttr.GetAttr(EffectType.MagicCrit);
        
        Info.m_AddAttr.RemoveAttr(EffectType.Strength);
        Info.m_AddAttr.RemoveAttr(EffectType.Agility);
        Info.m_AddAttr.RemoveAttr(EffectType.Intelligence);
        Info.m_AddAttr.RemoveAttr(EffectType.Hp);
        Info.m_AddAttr.RemoveAttr(EffectType.Anger);
        Info.m_AddAttr.RemoveAttr(EffectType.PhyAttack);
        Info.m_AddAttr.RemoveAttr(EffectType.PhyDefense);
        Info.m_AddAttr.RemoveAttr(EffectType.MagicAttack);
        Info.m_AddAttr.RemoveAttr(EffectType.MagicDefense);
        Info.m_AddAttr.RemoveAttr(EffectType.Hit);
        Info.m_AddAttr.RemoveAttr(EffectType.Dodge);
        Info.m_AddAttr.RemoveAttr(EffectType.PhyCrit);
        Info.m_AddAttr.RemoveAttr(EffectType.MagicCrit);
    }
    /// <summary>
    /// 检测是否满足召唤条件（钱够，灵魂石够）
    /// </summary>
    public static bool CheckCanSummon(int SoldierTypeID)
    {
        foreach (s_soldier_typeInfo BaseInfo in m_soldierType) {
            if (BaseInfo == null) {
                continue;
            }
            if (BaseInfo.id == SoldierTypeID) {
                int NeedCoin = 0;
                int fragment = 0;
                int NeedNum = 0;
                SoldierM.GetUpStarNeed(SoldierTypeID, BaseInfo.starlevel, ref NeedNum, ref  NeedCoin);
                int Have = ItemDC.GetItemCount(BaseInfo.fragment);//当前灵魂石
                if (Have >= NeedNum) {
                    return true;
                }
                return false;
            }
        }
        return false;
    }
    public static bool CheckCanSummonMoney(int SoldierTypeID)
    {
        foreach (s_soldier_typeInfo BaseInfo in m_soldierType) {
            if (BaseInfo == null) {
                continue;
            }
            if (BaseInfo.id == SoldierTypeID) {
                int NeedCoin = 0;
                int fragment = 0;
                int NeedNum = 0;
                SoldierM.GetUpStarNeed(SoldierTypeID, BaseInfo.starlevel, ref NeedNum, ref  NeedCoin);
                int Have = ItemDC.GetItemCount(BaseInfo.fragment);//当前灵魂石
                if (Have >= NeedNum && UserDC.GetCoin() >= NeedCoin) {
                    return true;
                }
                return false;
            }
        }
        return false;
    }
    /// <summary>
    /// 计算炮弹兵战斗力
    /// </summary>
    private static void CalcSoldierPower(ref SoldierInfo Info)
    {
        if (Info == null) {
            return ;
        }
        int k1 = 0;
        int k2 = 0;
        int k3 = 0;
        int k4 = 0;
        int k5 = 0;
        int LimitLevel = 0;
        int equipID1 = 0;
        int equipID2 = 0;
        int equipID3 = 0;
        int equipID4 = 0;
        int equipID5 = 0;
        int equipID6 = 0;
        //
        ConfigM.GetSoldierPower(ref  k1, ref  k2, ref  k3, ref  k4, ref  k5);
        int totalSkillLevel = Info.m_Skill.GetTotalSkillLevel();
        int Power = totalSkillLevel * k1;
        Power = Power + (k2 + ConfigM.GetQualityIndex(Info.Quality) * k3 + Info.StarLevel * k4) * Info.Level;
        
        List<int> lQuality = ConfigM.GetPrevQuality(Info.Quality);
        
        List<int> lEquipType = new List<int>();
        foreach (int quality in lQuality) {
            GetUpQualityNeed(Info.SoldierTypeID, quality, ref  LimitLevel,
                ref  equipID1, ref  equipID2, ref  equipID3,
                ref  equipID4, ref  equipID5, ref  equipID6);
            lEquipType.Add(equipID1);
            lEquipType.Add(equipID2);
            lEquipType.Add(equipID3);
            lEquipType.Add(equipID4);
            lEquipType.Add(equipID5);
            lEquipType.Add(equipID6);
        }
        //当前阶装备类型
        GetUpQualityNeed(Info.SoldierTypeID, Info.Quality, ref  LimitLevel,
            ref  equipID1, ref  equipID2, ref  equipID3,
            ref  equipID4, ref  equipID5, ref  equipID6);
        //
        if (Info.Equipment0 > 0) {
            lEquipType.Add(equipID1);
        }
        if (Info.Equipment1 > 0) {
            lEquipType.Add(equipID2);
        }
        if (Info.Equipment2 > 0) {
            lEquipType.Add(equipID3);
        }
        if (Info.Equipment3 > 0) {
            lEquipType.Add(equipID4);
        }
        if (Info.Equipment4 > 0) {
            lEquipType.Add(equipID5);
        }
        if (Info.Equipment5 > 0) {
            lEquipType.Add(equipID6);
        }
        
        int EquipTotalLevel = 0;
        
        foreach (int EquipType in lEquipType) {
            s_itemtypeInfo item =  ItemM.GetItemInfo(EquipType);
            if (item != null) {
                EquipTotalLevel += item.level;
            }
        }
        //添加装备部分产生的战斗力
        Power = Power + EquipTotalLevel * k5;
        
        Info.m_combat_power = Power;
    }
    /// <summary>
    /// 获取炮弹兵数据
    /// </summary>
    /// <param name="I">炮弹兵类型静态数据</param>
    /// <param name="Info">炮弹兵数据</param>
    private static void SetSoldierSkill(s_soldier_typeInfo I, ref SoldierInfo Info)
    {
        if (I == null || Info == null || Info.m_Skill == null) {
            return ;
        }
        Info.m_Skill.SetSkillType(I);
        Info.m_Skill.InitSkill();
        SkillM.SetSkillOrder(ref Info.m_Skill, I.id);
    }
    /// <summary>
    /// 获取升星所需资源
    /// </summary>
    /// <param name="SoldierType">炮弹兵类型</param>
    /// <param name="Star">星级</param>
    /// <param name="NeedMaterial">需要材料，key item id,value 数量</param>
    /// <param name="Star">建筑星级</param>
    /// <returns>建筑数据,null 获取失败</returns>
    public static void GetUpStarNeed(int SoldierType, int Star, ref int fragmentNum, ref int Coin)
    {
        Coin = 0 ;
        fragmentNum = 0;
        
        s_soldierstar_typeInfo Info  = GetSoldierStarData(SoldierType, Star);
        if (Info != null) {
            Coin = Info.price ;
            fragmentNum = Info.item_num;
        }
    }
    
    /// <summary>
    /// 获取升级所需经验
    /// </summary>
    /// <param name="SoldierType">炮弹兵类型</param>
    /// <param name="Star">星级</param>
    /// <param name="NeedMaterial">需要材料，key item id,value 数量</param>
    /// <param name="Star">建筑星级</param>
    /// <returns>建筑数据,null 获取失败</returns>
    public static int GetUpLevelNeedExp(int Level)
    {
        s_soldier_experienceInfo Info  = GetSoldierExpData(Level);
        if (Info != null) {
            return Info.exp;
        }
        return 0;
    }
    
    
    /// <summary>
    /// 获取升阶所需资源
    /// </summary>
    /// <param name="SoldierType">炮弹兵类型</param>
    /// <param name="Quality">当前阶</param>
    /// <param name="NeedMaterial">需要材料，key item id,value 数量</param>
    /// <param name="Star">建筑星级</param>
    /// <returns>建筑数据,null 获取失败</returns>
    public static void GetUpQualityNeed(int SoldierType, int CurQuality, ref int LimitLevel,
        ref int equipID1, ref int equipID2, ref int equipID3,
        ref int equipID4, ref int equipID5, ref int equipID6)
    {
        s_soldierqualityInfo Info  = GetqualityInfoData(SoldierType, CurQuality);
        if (Info != null) {
            equipID1 = Info.equipment0;
            equipID2 = Info.equipment1;
            equipID3 = Info.equipment2;
            equipID4 = Info.equipment3;
            equipID5 = Info.equipment4;
            equipID6 = Info.equipment5;
        }
        //
        s_soldierqualityInfo Info1  = GetqualityInfoData(SoldierType, ConfigM.GetNextQuality(CurQuality));
        if (Info1 != null) {
            LimitLevel = Info1.levellimit ;
        }
    }
    /// <summary>
    /// 获取升级技能花费
    /// </summary>
    /// <param name="SkillLevel">技能等级</param>
    /// <param name="SkillIndex">技能序号  0 为炮战 </param>
    public static int GetUpSkillLevelNeed(int SkillLevel, int SkillIndex)
    {
        s_skill_costInfo Info =  GetSoldierSkillLevel(SkillLevel);
        if (Info == null) {
            return 0;
        }
        if (SkillIndex == 0) {
            return Info.askill;
        } else if (SkillIndex == 1) {
            return Info.skill1;
        } else if (SkillIndex == 2) {
            return Info.skill2;
        } else if (SkillIndex == 3) {
            return Info.skill3;
        } else if (SkillIndex == 4) {
            return Info.skill4;
        } else if (SkillIndex == 5) {
            return Info.skill5;
        } else {
            return 0;
        }
    }
    /// <summary>
    /// 升级技能
    /// </summary>
    /// <param name="SkillLevle">技能等级</param>
    /// <param name="SkillIndex">技能序号  0 为炮战 </param>
    public static void UpSkillLevel(ref SoldierInfo Info, int SkillLevel, int SkillIndex)
    {
        if (Info == null || Info.m_Skill == null) {
            return ;
        }
        
        Info.m_Skill.UpSkillLevel(SkillLevel, SkillIndex);
    }
    
    /// <summary>
    /// 获取炮弹兵当前品阶下穿的装备（itemtypeid）
    /// </summary>
    /// <param name="SoldierType">炮弹兵类型</param>
    /// <param name="Quality">炮弹兵拼接 </param>
    /// <param name="Position">装备插槽 </param>
    public static int GetSoldierEquip(int SoldierType, int Quality, int Position)
    {
        s_soldierqualityInfo Info  = GetqualityInfoData(SoldierType, Quality);
        if (Info != null) {
            if (Position == 0) {
                return Info.equipment0;
            } else if (Position == 1) {
                return Info.equipment1;
            } else if (Position == 2) {
                return Info.equipment2;
            } else if (Position == 3) {
                return Info.equipment3;
            } else if (Position == 4) {
                return Info.equipment4;
            } else if (Position == 5) {
                return Info.equipment5;
            } else {
                return 0 ;
            }
        }
        return 0;
    }
    /// <summary>
    /// 获取炮弹兵适合穿的装备
    /// </summary>
    private static List<int> GetAllEquipType(SoldierInfo Info)
    {
        List<int> l = new List<int>();
        if (Info == null) {
            return l;
        }
        List<int> lQuality = new List<int>();
        lQuality = ConfigM. GetPrevQuality(Info.Quality);
        //早阶的装备
        foreach (int Q in lQuality) {
            s_soldierqualityInfo qualityinfo = GetqualityInfoData(Info.SoldierTypeID, Q);
            if (qualityinfo != null) {
                l.Add(qualityinfo.equipment0);
                l.Add(qualityinfo.equipment1);
                l.Add(qualityinfo.equipment2);
                l.Add(qualityinfo.equipment3);
                l.Add(qualityinfo.equipment4);
                l.Add(qualityinfo.equipment5);
            }
        }
        //当前穿的装备
        s_soldierqualityInfo Nowqualityinfo = GetqualityInfoData(Info.SoldierTypeID, Info.Quality);
        if (Nowqualityinfo != null) {
            if (Info.Equipment0 != 0) {
                l.Add(Nowqualityinfo.equipment0);
            }
            
            if (Info.Equipment1 != 0) {
                l.Add(Nowqualityinfo.equipment1);
            }
            
            if (Info.Equipment2 != 0) {
                l.Add(Nowqualityinfo.equipment2);
            }
            
            if (Info.Equipment3 != 0) {
                l.Add(Nowqualityinfo.equipment3);
            }
            
            
            if (Info.Equipment4 != 0) {
                l.Add(Nowqualityinfo.equipment4);
            }
            
            
            if (Info.Equipment5 != 0) {
                l.Add(Nowqualityinfo.equipment5);
            }
        }
        return l;
    }
    
    
    /// <summary>
    /// 获取炮弹兵适合穿的装备数据
    /// </summary>
    private static void AddEquipAttr(ref SoldierInfo Info)
    {
        if (Info == null || Info.m_AddAttr == null) {
            return ;
        }
        Info.m_AddAttr.Clear(AddAttrType.Equip);
        
        List<int> lEquipType = GetAllEquipType(Info);
        
        foreach (int ItemID in lEquipType) {
            s_itemtypeInfo  I = ItemM.GetItemInfo(ItemID);
            if (I != null) {
                Info.m_AddAttr.SetAddAttrInfo(I);
            }
        }
    }
    
    /// <summary>
    /// 改炮弹兵类型能穿该装备
    /// </summary>
    /// <param name="SoldireTypeID">炮弹兵类型</param>
    /// <param name="ItemTypeID">装备类型 </param>
    public static bool CheckCanPutEquip(int SoldireTypeID, int ItemTypeID)
    {
        //炮弹兵所有的阶数
        List<int> lQuality = new List<int> ();
        lQuality = ConfigM.GetQualityList();
        
        foreach (int Quality in lQuality) {
            s_soldierqualityInfo  Info = GetqualityInfoData(SoldireTypeID, Quality);
            if (Info == null) {
                continue ;
            } else if (Info.equipment0 == ItemTypeID || Info.equipment1 == ItemTypeID
                || Info.equipment2 == ItemTypeID || Info.equipment3 == ItemTypeID
                || Info.equipment4 == ItemTypeID || Info.equipment5 == ItemTypeID) {
                return true;
            }
        }
        
        return false;
    }
    /// <summary>
    /// 炮弹兵装备格的状态
    /// </summary>
    /// <returns>The can put equip.</returns>
    /// <param name="info">Info.</param>
    /// <param name="EquipID">Equip I.</param>
    /// <param name="index">Index.</param>
    /// <param name="itemTypeID">Item type I.</param>
    public static EquipmentPutType CheckCanPutEquip(SoldierInfo info, int EquipID, int index, ref int itemTypeID)
    {
        itemTypeID = SoldierM.GetSoldierEquip(info.SoldierTypeID, info.Quality, index);
        
        if (EquipID == 0) {
            ItemTypeInfo ItemInfo = ItemDC.SearchItem(itemTypeID);
            if (ItemInfo != null) {
                if (info.Level >= ItemInfo.m_level) {
                    return EquipmentPutType.CanPut;
                } else {
                    return EquipmentPutType.HaveCannot;
                }
            } else {
                s_itemtypeInfo sInfo = ItemM.GetItemInfo(itemTypeID);
                
                List<KeyValue> needSubEquip = new List<KeyValue>();
                int needCoin = 0;
                bool canCombine =  ItemM.GetCombineEquipNeed(itemTypeID, ref needSubEquip, ref needCoin);
                if (canCombine) {
                    if (ItemDC.CheckItemsEnough(needSubEquip)) {
                        if (needCoin < UserDC.GetCoin()) {
                            if (info.Level >= sInfo.level) {
                                return EquipmentPutType.CanCombinePut;
                            } else {
                                return EquipmentPutType.CanCombine;
                            }
                        } else {
                            return EquipmentPutType.CanCombine;
                        }
                    } else if (!ItemDC.CheckItemsEnough(needSubEquip)) {
                        return EquipmentPutType.ReadyCombine;
                    } else {
                        return EquipmentPutType.None;
                    }
                } else {
                    return EquipmentPutType.NoCanCombine;
                }
            }
            
        } else {
            return EquipmentPutType.HavePut;
        }
        
    }
    
    
    /// <summary>
    /// 炮弹兵是否能增加经验 0:可以增加，1：已经满级不能增加
    /// </summary>
    /// <returns><c>true</c>, if can add exp was checked, <c>false</c> otherwise.</returns>
    /// <param name="SoldireTypeID">炮弹兵ID.</param>
    /// <param name="AddExpNum">炮弹兵增加的经验值</param>
    public static int CheckCanAddExp(int SoldireTypeID, int AddExpNum)
    {
        SoldierInfo info = SoldierDC.GetSoldiers(SoldireTypeID);
        if (info == null) {
            return 0;
        }
        
        s_soldier_experienceInfo expInfo = GetSoldierExpData(info.Level);
        if (expInfo.exp == 0) {
            return 1;
        }
        
        int MaxLvl = UserM.GetUserMaxHeroLevel(info.Level);
        int MaxExp = GetSoldierTotalExpAtLevel(MaxLvl);
        
        
        if (info.EXP >= MaxExp) {
            return 1;
        } else if (info.EXP + AddExpNum < MaxExp) {
            return 0;
        } else if (info.EXP + AddExpNum > MaxExp && info.EXP < MaxExp) {
            return 0;
        }
        return 0;
    }
    
    /// <summary>
    /// 获取炮弹兵最高等级
    /// </summary>
    /// <returns></returns>
    public static int GetSoldierMaxLevel()
    {
        for (int i = m_lexp.Count - 1; i >= 0; i--) {
            if (m_lexp[i].exp == 0) {
                return m_lexp[i].level;
            }
        }
        return 0;
    }
    
    public static string GetSoldierDialogue(int soldierID)
    {
        foreach (s_soldier_typeInfo v in m_soldierType) {
            if (v.id == soldierID) {
                return v.text;
            }
        }
        return null;
    }
    public static void SortSoldierList(ref List<SoldierInfo> lSoldier)
    {
        if (lSoldier == null) {
            return ;
        }
        lSoldier.Sort((a, b) => {
        
            if (a.Level > b.Level) {
                return -1;
            } else if (a.Level < b.Level) {
                return 1;
            }
            
            if (a.Quality > b.Quality) {
                return -1;
            } else if (a.Quality < b.Quality) {
                return 1;
            }
            
            if (a.StarLevel > b.StarLevel) {
                return -1;
            } else if (a.StarLevel < b.StarLevel) {
                return 1;
            }
            
            if (a.m_combat_power > b.m_combat_power) {
                return -1;
            } else if (a.m_combat_power < b.m_combat_power) {
                return 1;
            }
            
            if (a.SoldierTypeID > b.SoldierTypeID) {
                return 1;
            } else if (a.SoldierTypeID < b.SoldierTypeID) {
                return -1;
            } else {
                return 0;
            }
        });
    }
    
}
