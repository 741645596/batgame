using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using sdata;

/// <summary>
/// 技能数据管理
/// </summary>
/// <author>zhulin</author>
public class SkillM
{

    private static List<s_skill_typeInfo> m_LSkillType = new List<s_skill_typeInfo>();
    
    private static List<s_skillupInfo> m_LSkillUp = new List<s_skillupInfo>();
    
    private static List<s_skill_orderInfo> m_LPlaySkill = new List<s_skill_orderInfo>();
    
    private static List<s_statusInfo> m_LStatus = new List<s_statusInfo>();
    
    private static List<s_statusreplaceInfo> m_LStatusReplace = new List<s_statusreplaceInfo>();
    
    private static List<s_attackmodeInfo> m_LAttackMode = new List<s_attackmodeInfo>();
    
    private static List<s_skilleffectInfo> m_skilleffect = new List<s_skilleffectInfo>();
    
    private static List<s_buildskilltypeInfo> m_lBuildSkillType = new List<s_buildskilltypeInfo>();
    
    private static List<s_distainInfo> m_lDistainInfo = new List<s_distainInfo>();
    
    private static bool _IsLoad = false;
    
    
    public static void Init(object obj)
    {
        System.Diagnostics.Debug.Assert(obj is sdata.StaticDataResponse);
        
        sdata.StaticDataResponse sdrsp = obj as sdata.StaticDataResponse;
        
        m_LSkillType = sdrsp.s_skill_type_info;
        
        m_LPlaySkill = sdrsp.s_skill_order_info;
        
        m_LStatus = sdrsp.s_status_info;
        
        m_LAttackMode = sdrsp.s_attackmode_info;
        
        m_skilleffect = sdrsp.s_skilleffect_info;
        
        m_lBuildSkillType = sdrsp.s_buildskilltype_info ;
        
        m_lDistainInfo = sdrsp.s_distain_info;
        
        m_LSkillUp = sdrsp.s_skillup_info;
        
        m_LStatusReplace = sdrsp.s_statusreplace_info;
    }
    /// <summary>
    /// 获取技能轮流
    /// </summary>
    private static s_skill_orderInfo GetSkillOrder(int soldier_id)
    {
        foreach (s_skill_orderInfo v in m_LPlaySkill) {
            if (v.soldier_id == soldier_id) {
                return v;
            }
        }
        return null;
    }
    /// <summary>
    /// 获取技能基础数据
    /// </summary>
    private static s_skill_typeInfo GetSkillType(int Skilltype)
    {
        int nCount = m_LSkillType.Count;
        for (int nCnt = 0; nCnt < nCount; nCnt++) {
            s_skill_typeInfo v = m_LSkillType[nCnt];
            if (v.type == Skilltype) {
                return v;
            }
        }
        return null;
    }
    /// <summary>
    /// 获取技能等级相关数据
    /// </summary>
    private static s_skillupInfo GetSkillUp(int Skillid)
    {
        int nCount = m_LSkillUp.Count;
        for (int nCnt = 0; nCnt < nCount; nCnt++) {
            s_skillupInfo v = m_LSkillUp[nCnt];
            if (v.id == Skillid) {
                return v;
            }
        }
        return null;
    }
    /// <summary>
    /// 获取技能
    /// </summary>
    private static s_skillupInfo GetSkillUp(int skilltype, int level)
    {
        int nCount = m_LSkillUp.Count;
        for (int nCnt = 0; nCnt < nCount; nCnt++) {
            s_skillupInfo v = m_LSkillUp[nCnt];
            if (v.type == skilltype && v.level == level) {
                return v;
            }
        }
        return null;
    }
    
    /// <summary>
    /// 获取建筑技能
    /// </summary>
    private static s_buildskilltypeInfo GetBuildSkillType(int buildType, int BigQuality)
    {
        foreach (s_buildskilltypeInfo v in m_lBuildSkillType) {
            if (v.buildtype == buildType && v.quality == BigQuality) {
                return v;
            }
        }
        return null;
    }
    /// <summary>
    /// 获取状态
    /// </summary>
    private static s_statusInfo GetSkillStatus(int id)
    {
        foreach (s_statusInfo v in m_LStatus) {
            if (v.id == id) {
                return v;
            }
        }
        
        return null;
    }	/// <summary>
    /// 获取状态替换规则
    /// </summary>
    private static s_statusreplaceInfo GetSkillStatusReplace(int statustype, int effect, int bufftype)
    {
        foreach (s_statusreplaceInfo v in m_LStatusReplace) {
            if (statustype == 0) {
                if (v.statustype == statustype && v.effect == effect && v.bufftype == bufftype) {
                    return v;
                }
            } else {
                if (v.statustype == statustype) {
                    return v;
                }
            }
        }
        
        return null;
    }
    /// <summary>
    /// 获取攻击模式
    /// </summary>
    private static s_attackmodeInfo GetAttackMode(int id)
    {
        foreach (s_attackmodeInfo v in m_LAttackMode) {
            if (v.id == id) {
                return v;
            }
        }
        return null;
    }
    /// <summary>
    /// 获取技能特效
    /// </summary>
    private static s_skilleffectInfo GetSkillEffect(int type)
    {
        foreach (s_skilleffectInfo e in m_skilleffect) {
            if (e.skilltype == type) {
                return e;
            }
        }
        return null;
    }
    /// <summary>
    /// 设置技能特效
    /// </summary>
    private static void  SetSkillEffect(s_skilleffectInfo e, ref SkillEffectInfo Info)
    {
        if (e == null) {
            return ;
        }
        if (Info == null) {
            Info = new SkillEffectInfo();
        }
        Info.m_id = e.id;
        Info.m_skilltype = e.skilltype;
        Info.m_hiteffect = e.hiteffect;
        Info.m_targeteffect = e.targeteffect;
        Info.m_locus = e.locus;
        Info.m_hitaudio = e.hitaudio;
        Info.m_isloop = e.isloop;
        Info.m_postion = e.position;
    }
    /// <summary>
    /// 设置技能
    /// </summary>
    private static void SetSoldierSkill(s_skillupInfo up, s_skill_typeInfo v, ref  SoldierSkill Info)
    {
        if (v == null || Info == null || up == null) {
            return ;
        }
        Info.m_id = up.id;
        Info.m_level = up.level;
        //Info.m_soldierid = v.soldierid;
        Info.m_power1 = up.power1;
        Info.m_power2 = up.power2;
        //Info.m_percent = up.percent;
        SetSkillStatus(ref Info.m_own_status, up.own_status);
        SetSkillStatus(ref Info.m_attack_status_own, up.attack_status_own);
        SetSkillStatus(ref Info.m_attack_status_enemy, up.attack_status_enemy);
        SetSkillStatus(ref Info.m_buildstatus, up.buildstatus);
        //Info.m_attr_type = v.at;
        Info.m_data0 = up.data0;
        Info.m_data1 = up.data1;
        Info.m_data2 = up.data2;
        Info.m_data3 = up.data3;
        //Info.m_ldistance = v.ldistance;
        Info.m_rankslevel = up.ranklevel;
        //Info.m_trapbreach = v.trapbreach;
        Info.m_description2 = up.description;
        Info.m_power3 = up.power3;
        Info.m_power4 = up.power4;
        
        
        
        Info.m_ischangetarget = v.ischangetarget;
        Info.m_step_secs = v.step_secs;
        Info.m_intone_speed = v.intone_speed;
        SetSkillStatus(ref Info.m_releasedown_status, v.releasedown_status);
        SetSkillStatus(ref Info.m_releasedenemy_status, v.releasedenemy_status);
        SetSkillStatus(ref Info.m_releasedself_status, v.releasedself_status);
        Info.m_type = v.type;
        Info.m_name = v.name;
        Info.m_attacktype = v.attacktype;
        Info.m_priority = v.priority;
        Info.m_sort = v.sort;
        Info.m_multiple = v.multiple;
        Info.m_actiontype = v.actiontype;
        Info.m_use_mp = v.use_mp;
        Info.m_target = v.target;
        Info.m_distance = v.distance;
        Info.m_term1 = v.term1;
        Info.m_term2 = v.term2;
        Info.m_term3 = v.term3;
        Info.m_cd = v.cd;
        Info.m_timeinterval = v.timeinterval;
        Info.m_range = (v.range * MapGrid.m_width / MapGrid.m_Pixel);
        Info.m_attckmodeid = v.attackmodeid;
        s_attackmodeInfo I = GetAttackMode(Info.m_attckmodeid);
        if (I != null) {
            SetAttackPower(I, ref Info.m_lAttackPower);
        }
        
        Info.m_interrupt_skill = v.interrupt_skill;
        Info.m_condition = v.condition;
        Info.m_condition_data0 = v.condition_data0;
        Info.m_condition_data1 = v.condition_data1;
        Info.m_ipriority = v.ipriority;
        Info.m_struckeffect = v.struckeffect;
        Info.m_ismove = v.ismove;
        Info.m_blackscreentime = v.blackscreentime * 0.001f;
        s_skilleffectInfo e = GetSkillEffect(Info.m_type);
        if (e != null) {
            SetSkillEffect(e, ref Info.m_skilleffectinfo);
        }
        SetSkillAttributeType(ref Info.m_AttributeType, v.attribute);
        
        Info.m_description1 = v.description1;
        Info.m_status_hitratio = v.status_hitratio * 0.001f;
        Info.m_targettype = v.targettype;
        Info.m_damagetargettype = v.damagetargettype;
        Info.m_Screen = v.screen;
        Info.m_ScreenTime = v.screentime * 0.001f;
    }
    /// <summary>
    /// 设置建筑技能
    /// </summary>
    private static void SetSoldierSkill(s_buildskilltypeInfo v, ref  BuildSkillInfo Info)
    {
        if (v == null || Info == null) {
            return ;
        }
        Info.m_desc = v.description ;
        Info.m_id = v.id;
        Info.m_type = v.type;
        Info.m_name = v.name;
        Info.m_buildtype = v.buildtype;
        Info.m_quality = v.quality;
        Info.m_attacktype = v.attacktype;
        Info.m_target = v.target;
        Info.m_power1 = v.power1;
        Info.m_power2 = v.power2;
        Info.m_cd = v.cd;
        Info.m_multiple = v.multiple;
        Info.m_attckmodeid = v.attackmodeid;
        s_attackmodeInfo I = GetAttackMode(Info.m_attckmodeid);
        if (I != null) {
            SetAttackPower(I, ref Info.m_lAttackPower);
        }
        SetSkillStatus(ref Info.m_attack_status_own, v.attack_status_own);
        SetSkillStatus(ref Info.m_attack_status_enemy, v.attack_status_enemy);
        SetSkillStatus(ref Info.m_releasedown_status, v.releasedown_status);
        SetSkillStatus(ref Info.m_releasedenemy_status, v.releasedenemy_status);
        Info.m_struckeffect = v.struckeffect;
        s_skilleffectInfo e = GetSkillEffect(Info.m_type);
        if (e != null) {
            SetSkillEffect(e, ref Info.m_skilleffectinfo);
        }
        Info.m_interrupt_skill = v.interrupt_skill;
        Info.m_tSearchInfo.SetData(v.tshape, v.tinboat, v.tlayer, v.tparam);
        Info.m_dSearchInfo.SetData(v.dshape, v.dinboat, v.dlayer, v.dparam);
        SetSkillAttributeType(ref Info.m_AttributeType, v.attribute);
    }
    /// <summary>
    /// 设置技能属性
    /// </summary>
    private static void SetSkillAttributeType(ref AttributeType SkillType, string StrValue)
    {
        SkillType = AttributeType.NONE ;
        if (string.IsNullOrEmpty(StrValue) == true) {
            SkillType = AttributeType.NONE;
        } else {
            int length = NdUtil.GetLength(StrValue);
            for (int i = 0 ; i < length ; i++) {
                int value = NdUtil.GetIntValue(StrValue, i);
                AttributeType type = GetBuildAttributeType(value);
                SkillType = SkillType | type ;
            }
        }
    }
    /// <summary>
    /// 设置状态
    /// </summary>
    private static bool SetSkillStatus(s_statusInfo stainfo, ref SkillStatusInfo Info)
    {
        if (stainfo == null || Info == null) {
            return false;
        }
        Info.m_persistence = stainfo.persistence;
        Info.m_id = stainfo.id;
        Info.m_name = stainfo.name;
        Info.m_type = stainfo.type;
        
        if (string.IsNullOrEmpty(stainfo.effect) == false && stainfo.effect  != "0") {
            int length = NdUtil.GetLength(stainfo.effect);
            for (int i = 0; i < length; ++i) {
                Info.m_effect.Add(NdUtil.GetIntValue(stainfo.effect, i));
            }
        }
        Info.m_effectlevel = stainfo.effectlevel;
        Info.m_time = stainfo.time;
        Info.m_timeinterval = stainfo.timeinterval;
        Info.m_effectid = stainfo.effectid;
        Info.m_condition = stainfo.condition;
        Info.m_position = stainfo.position;
        Info.m_data0 = stainfo.data0;
        Info.m_data1 = stainfo.data1;
        Info.m_skill1type = stainfo.skill1type;
        Info.m_level1 = stainfo.level1;
        Info.m_skill2type = stainfo.skill2type;
        Info.m_level2 = stainfo.level2;
        Info.m_bufftype = stainfo.bufftype;
        s_statusreplaceInfo replace = GetSkillStatusReplace(Info.m_type, Info.m_effect.Count > 0 ? Info.m_effect[0] : 0, Info.m_bufftype);
        if (replace != null) {
            Info.m_RelpaceInfo = new SkillStatusReplaceInfo();
            Info.m_RelpaceInfo.m_id = replace.id;
            Info.m_RelpaceInfo.m_statustype = replace.statustype;
            Info.m_RelpaceInfo.m_effect = replace.effect;
            Info.m_RelpaceInfo.m_name = replace.name;
            Info.m_RelpaceInfo.m_type = replace.type;
            Info.m_RelpaceInfo.m_compatible = replace.compatible;
            Info.m_RelpaceInfo.m_reppriority = replace.reppriority;
            Info.m_RelpaceInfo.m_spepriority = replace.spepriority;
            Info.m_RelpaceInfo.m_bufftype = replace.bufftype;
            Info.m_RelpaceInfo.m_isaction = replace.isaction;
        } else {
            Debug.Log("xxxx " + Info.m_name);
        }
        return true;
    }
    
    private static void SetAttackPower(s_attackmodeInfo Info, ref List<AttackPower> lAttackPower)
    {
        if (Info == null || lAttackPower == null) {
            return ;
        }
        lAttackPower.Clear();
        int count = Info.times;
        if (count >= 1) {
            lAttackPower.Add(new AttackPower(Info.time1 * 0.001f, Info.time1_power * 0.001f));
        }
        if (count >= 2) {
            lAttackPower.Add(new AttackPower(Info.time2 * 0.001f, Info.time2_power * 0.001f));
        }
        if (count >= 3) {
            lAttackPower.Add(new AttackPower(Info.time3 * 0.001f, Info.time3_power * 0.001f));
        }
        if (count >= 4) {
            lAttackPower.Add(new AttackPower(Info.time4 * 0.001f, Info.time4_power * 0.001f));
        }
        if (count >= 5) {
            lAttackPower.Add(new AttackPower(Info.time5 * 0.001f, Info.time5_power * 0.001f));
        }
        if (count >= 6) {
            lAttackPower.Add(new AttackPower(Info.time6 * 0.001f, Info.time6_power * 0.001f));
        }
        if (count >= 7) {
            lAttackPower.Add(new AttackPower(Info.time7 * 0.001f, Info.time7_power * 0.001f));
        }
    }
    /// <summary>
    /// 设置技能轮流信息
    /// </summary>
    public static void SetSkillOrder(ref SoldierSkillInfo Info, int soldier_id)
    {
        if (Info == null) {
            return ;
        }
        s_skill_orderInfo orderinfo = GetSkillOrder(soldier_id);
        if (orderinfo != null) {
            Info.attack1 = orderinfo.attack1;
            Info.attack2 = orderinfo.attack2;
        }
    }
    /// <summary>
    /// 获取建筑物技能
    /// </summary>
    public static BuildSkillInfo GetBuildSkill(int buildType, int BigQuality)
    {
        s_buildskilltypeInfo v = GetBuildSkillType(buildType, BigQuality);
        if (v == null) {
            return null;
        } else {
            BuildSkillInfo Info = new BuildSkillInfo();
            SetSoldierSkill(v, ref Info);
            return Info;
        }
    }
    /// <summary>
    /// 获取建筑物技能属性
    /// </summary>
    public static AttributeType GetBuildAttributeType(int Kind)
    {
        if (Kind == 0) {
            return  AttributeType.NONE;
        } else if (Kind == 1) {
            return  AttributeType.Fire;
        } else if (Kind == 2) {
            return AttributeType.Water;
        } else if (Kind == 3) {
            return AttributeType.Electric;
        } else if (Kind == 4) {
            return AttributeType.Poison;
        } else if (Kind == 5) {
            return AttributeType.Gas;
        } else if (Kind == 6) {
            return AttributeType.Physical;
        } else {
            return AttributeType.NONE;
        }
    }
    /// <summary>
    /// 获取技能
    /// </summary>
    public static bool GetSkillInfo(int id, ref SoldierSkill Info)
    {
        s_skillupInfo up = GetSkillUp(id);
        if (up == null) {
            return false;
        }
        s_skill_typeInfo v = GetSkillType(up.type);
        if (v == null) {
            return false;
        }
        SetSoldierSkill(up, v, ref Info);
        return true;
    }
    /// <summary>
    /// 获取技能
    /// </summary>
    public static bool  GetSkillInfo(int type, int level, ref SoldierSkill Info)
    {
        s_skillupInfo up = GetSkillUp(type, level);
        s_skill_typeInfo v = GetSkillType(type);
        if (v == null || up == null) {
            return false;
        }
        SetSoldierSkill(up, v, ref Info);
        return true;
    }
    /// <summary>
    /// 获取设置状态
    /// </summary>
    public static void SetSkillStatus(ref List<SkillStatusInfo> lstatus, string strStatus)
    {
        if (lstatus == null) {
            lstatus = new List<SkillStatusInfo>();
        }
        if (string.IsNullOrEmpty(strStatus) == true || strStatus == "0") {
            return ;
        }
        
        int length = NdUtil.GetLength(strStatus);
        for (int i = 0; i < length; ++i) {
            int statusid = NdUtil.GetIntValue(strStatus, i);
            s_statusInfo stainfo = GetSkillStatus(statusid);
            if (stainfo != null) {
                SkillStatusInfo ss = new SkillStatusInfo();
                if (SetSkillStatus(stainfo, ref ss) == true) {
                    lstatus.Add(ss);
                }
            }
        }
    }
    /// <summary>
    /// 获取湿身状态数据
    /// </summary>
    public static SkillStatusInfo GetWetBodyStatusInfo()
    {
        s_statusInfo stainfo = GetSkillStatus(999999);
        if (stainfo != null) {
            SkillStatusInfo Info = new SkillStatusInfo();
            if (SetSkillStatus(stainfo, ref Info) == true) {
                return Info;
            }
        }
        return null;
    }
    
    // 星级鄙视
    public static void GetDistainFactor(int diff, ref float starFactor, ref float levelFactor)
    {
        for (int i = 0; i < m_lDistainInfo.Count; i++) {
            s_distainInfo info = m_lDistainInfo[i];
            if (diff == info.delta_value) {
                if (info.type == 1) {
                    starFactor = (float)info.data * 0.001f;
                    levelFactor = 0;
                    return;
                }
                if (info.type == 2) {
                    starFactor = 0;
                    levelFactor = (float)info.data * 0.001f;
                    return;
                }
#if UNITY_EDITOR
                Debug.LogWarning("s_distain表配的有问题，delta_value = " + info.delta_value + "， type值不对");
#endif
            }
        }
    }
    
    
}
