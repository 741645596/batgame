using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Logic;

/// <summary>
/// 公式参数表  s_config.csv
/// </summary>
/// <author>zhulin</author>

public class ConfigM
{

    private static bool _IsLoad = false;
    
    private static List<sdata.s_configInfo> m_lConfig = new List<sdata.s_configInfo>();
    private static List<sdata.s_resourcedropInfo> m_lConfigDrop = new List<sdata.s_resourcedropInfo>();
    
    public static void Init(object obj)
    {
        if (_IsLoad == true) {
            return;
        }
        
        System.Diagnostics.Debug.Assert(obj is sdata.StaticDataResponse);
        
        sdata.StaticDataResponse sdrsp = obj as sdata.StaticDataResponse;
        
        m_lConfig = sdrsp.s_config_info;
        m_lConfigDrop = sdrsp.s_resourcedrop_info;
        
        _IsLoad = true;
        
#if UNITY_EDITOR
#endif
        
    }
    
    private static int GetParaInt(int id, int index)
    {
        foreach (sdata.s_configInfo sci in m_lConfig) {
            if (sci.id == id) {
                switch (index) {
                    case 1:
                        return sci.data1;
                    case 2:
                        return sci.data2;
                    case 3:
                        return sci.data3;
                    case 4:
                        return sci.data4;
                    case 5:
                        return sci.data5;
                }
            }
        }
        return -1;
    }
    
    private static string GetParaStr(int id, int index)
    {
        foreach (sdata.s_configInfo sci in m_lConfig) {
            if (sci.id == id) {
                switch (index) {
                    case 1:
                        return sci.str1;
                    case 2:
                        return sci.str2;
                    case 3:
                        return sci.str3;
                    case 4:
                        return sci.str4;
                    case 5:
                        return sci.str5;
                }
            }
        }
        return string.Empty;
    }
    
    
    private static string GetParaStrValue(int id, int strIndex, int index)
    {
        string value = GetParaStr(id, index);
        if (string.IsNullOrEmpty(value) == true) {
            return string.Empty;
        }
        return NdUtil.GetStrValue(value, index);
        
    }
    /// <summary>
    /// 注意索引开始顺序（为了和常量表字段名称保持一致）
    /// </summary>
    /// <param name="id">id索引从1开始</param>
    /// <param name="strIndex">strIndex索引从1开始</param>
    /// <param name="index">索引从0开始</param>
    /// <returns></returns>
    private static int GetParaStrToInt(int id, int strIndex, int index)
    {
        string value = GetParaStr(id, strIndex);
        if (string.IsNullOrEmpty(value) == true) {
            return -1;
        }
        return NdUtil.GetIntValue(value, index);
    }
    
    //属性公式参数
    public static int GetAttributeK(int index)
    {
        return GetParaStrToInt(1, 1, index);
    }
    // 炮战公式参数
    public static int GetGunK(int index)
    {
    
        return GetParaInt(2, index);
    }
    //白刃战公式参数
    public static int GetWhiteNinjaK(int index)
    {
    
        return GetParaInt(3, index);
    }
    
    //船破坏参数
    public static int GetBoatCombatK(int index)
    {
    
        return GetParaInt(4, index);
    }
    
    
    //星级判定
    public static int GetStarLevel(int DamageRate)
    {
        if (DamageRate < GetParaStrToInt(5, 1, 0)) {
            return 0;
        } else if (DamageRate < GetParaStrToInt(5, 1, 1)) {
            return 1;
        } else if (DamageRate < GetParaStrToInt(5, 2, 1)) {
            return 2;
        } else if (DamageRate > GetParaStrToInt(5, 3, 0)) {
            return 3;
        }
        
        return 0;
    }
    /// <summary>
    /// 船板破坏程度判定
    /// </summary>
    /// <param name="damageRate"></param>
    /// <returns>1 轻度破坏 ， 2 中度破坏，3完全破坏</returns>
    public static int GetFloorDamageLevel(int damageRate)
    {
        if (NdUtil.IsIntBetween(damageRate, GetParaStrToInt(8, 1, 0), GetParaStrToInt(8, 1, 1))) {
            return 1;
        } else if (NdUtil.IsIntBetween(damageRate, GetParaStrToInt(8, 2, 0), GetParaStrToInt(8, 2, 1))) {
            return 2;
        } else if (NdUtil.IsIntBetween(damageRate, GetParaStrToInt(8, 3, 0), GetParaStrToInt(8, 3, 1))) {
            return 3;
        }
        
        return -1;
    }
    
    /// <summary>
    /// 获取魔方伤害减免百分比
    /// </summary>
    /// <param name="s2_magicdefend">魔法防御</param>
    public static float  GetReduceMagicPercent(int s2_magicdefend)
    {
        int Value = 0;
        int min = 0;
        for (int i = 1; i <= 5; i++) {
            int l = GetParaStrToInt(9, i, 0);
            int r = GetParaStrToInt(9, i, 1);
            if (s2_magicdefend > l && s2_magicdefend <= r) {
                int para = i;
                min = l;
                Value = i;
                break;
            }
        }
        float  k8 = GetAttributeK(8) * 0.01f;
        float  k9 = GetAttributeK(9) * 0.01f;
        float  k10 = GetAttributeK(10) * 0.01f;
        float  k11 = GetAttributeK(10) * 0.01f;
        float  k12 = GetAttributeK(11) * 0.01f;
        
        int def = s2_magicdefend - min;
        
        if (Value == 1) {
            return  def / 60.0f * k8 ;
        } else if (Value == 2) {
            return 0.05f + def / 50.0f * k9;
        } else if (Value == 3) {
            return 0.25f + def / 90.0f * k10;
        } else if (Value == 4) {
            return 0.40f + def / 75.0f * k11;
        } else if (Value == 5) {
            return 0.45f + def / 80.0f * k12;
        }
        return 0.0f;
    }
    
    /// <summary>
    ///  怒气累计参数
    /// </summary>
    /// <param name="para">k 满怒气的值</param>
    /// <returns></returns>
    public static int GetAngerK(int index)
    {
        return GetParaInt(6, index);
    }
    
    /// <summary>
    /// 怒气累计参数
    /// </summary>
    public static int GetSkillData0()
    {
        return GetParaInt(7, 1);
    }
    /// <summary>
    /// 获取攻击目标倒计时-排行榜搜索目标
    /// </summary>
    public static int GetAttackCountDown(BattleEnvironmentMode Mode)
    {
        int result = 30;
        
        switch (Mode) {
            case BattleEnvironmentMode.CombatPVE:
                result = GetParaInt(11, 2);
                break;
            default:
                break;
        }
        
        return result;
    }
    
    //获取建筑物额外信息
    public static sdata.s_resourcedropInfo GetBuildSourceDropIRow(int type, int num)
    {
        foreach (sdata.s_resourcedropInfo I in m_lConfigDrop) {
            if (I.type == type && (I.data2 <= num && I.data3 >= num)) {
                return I;
            }
        }
        return null;
    }
    
    /// <summary>
    /// 获取金币房类型
    /// </summary>
    /// <param name="para">金币量</param>
    public static int GetGoldType(int g)
    {
        for (int i = 1; i <= 5; i++) {
            int l = GetParaStrToInt(10, i, 0);
            int r = GetParaStrToInt(10, i, 1);
            if (g >= l && g <= r) {
                return i;
            }
        }
        return 5;
    }
    
    /// <summary>
    /// 血量百分比
    /// </summary>
    /// <param name="para">血量百分比</param>
    public static int GetBuildHPType()
    {
        return GetParaInt(19, 1);
    }
    public static int GetChangeNameDiamond()
    {
        return GetParaInt(13, 1);
    }
    /// <summary>
    /// 获取 购买体力所消费的钻石
    /// </summary>
    /// <returns></returns>
    public static int GetAddPhysicsDiamond()
    {
        return 50;
    }
    /// <summary>
    /// 获取 玩家总体力值 上限
    /// </summary>
    /// <returns></returns>
    public static int GetPhysicsMax()
    {
        return 500;
    }
    
    public static int GetInitMana()
    {
        return GetParaInt(14, 3);
    }
    public static int GetSoldierDeadMana()
    {
        return GetParaInt(14, 1);
    }
    public static int GetBuildDeadMana()
    {
        return GetParaInt(14, 2);
    }
    
    //获取防御模式开战表现停顿参数
    public static float GetCombatWaringWaitTime()
    {
    
        return GetParaInt(3, 16) / 1000.0f;
    }
    
    public static float GetBigSkillDelatime()
    {
    
        return GetParaInt(17, 1) * 0.001f;
    }
    public static float GetSkillHitRate()
    {
    
        return GetParaInt(18, 1) * 0.001f;
    }
    
    /// <summary>
    /// 获取爆炸相关的时间参数
    /// </summary>
    public static void  GetShipBombPara(ref float st1, ref float dt, ref float dt1, ref float dt2, ref float dt3)
    {
        st1 =  GetParaInt(20, 1) * 0.001f;
        dt =   GetParaInt(20, 2) * 0.001f;
        dt1 =  GetParaInt(20, 3) * 0.001f;
        dt2 =  GetParaInt(20, 4) * 0.001f;
        dt3 =  GetParaInt(20, 5) * 0.001f;
    }
    
    /// <summary>
    /// 获取陷阱升级参数
    /// </summary>
    public static float GetBuildUpParam(int k)
    {
        return   GetParaInt(22, k) * 0.001f;
    }
    
    /// <summary>
    /// 获取下一个品阶
    /// </summary>
    public static int GetNextQuality(int Quality)
    {
        List<int> l = new List<int>();
        string value =  GetParaStr(24, 1) ;
        if (string.IsNullOrEmpty(value) == true) {
            return 10 ;
        }
        int length = NdUtil.GetLength(value);
        for (int i = 0 ; i < length ; i++) {
            l.Add(NdUtil.GetIntValue(value, i));
        }
        
        for (int i = 0 ; i < l.Count ; i ++) {
            if (l[i] == Quality) {
                if (i == l.Count - 1) {
                    return l[l.Count - 1];
                } else {
                    return l[i  + 1 ];
                }
            }
        }
        return 0;
    }
    
    
    /// <summary>
    /// 是否为最高品阶
    /// </summary>
    public static bool CheckTopQuality(int Quality)
    {
        List<int> l = new List<int>();
        string value =  GetParaStr(24, 1) ;
        if (string.IsNullOrEmpty(value) == true) {
            return false ;
        }
        int length = NdUtil.GetLength(value);
        for (int i = 0 ; i < length ; i++) {
            l.Add(NdUtil.GetIntValue(value, i));
        }
        
        if (l.Count == 0) {
            return false;
        }
        
        if (l[l.Count - 1] == Quality) {
            return true;
        } else {
            return false;
        }
    }
    
    
    /// <summary>
    /// 获取品质属于第几个顺位 第一个为1
    /// </summary>
    public static int GetQualityIndex(int Quality)
    {
        string value =  GetParaStr(24, 1) ;
        List<int> l = new List<int>();
        if (string.IsNullOrEmpty(value) == true) {
            return 0 ;
        }
        int length = NdUtil.GetLength(value);
        for (int i = 0 ; i < length ; i++) {
            int q = NdUtil.GetIntValue(value, i);
            if (q == Quality) {
                return i + 1;
            }
        }
        return 0 ;
    }
    
    
    /// <summary>
    /// 获取大品阶
    /// </summary>
    public static int GetBigQuality(int Quality)
    {
        return Quality / 10 ;
    }
    
    /// <summary>
    /// 获取小品阶
    /// </summary>
    public static int GetSmallQuality(int Quality)
    {
        return Quality % 10 ;
    }
    /// <summary>
    /// 确认是否为初始品阶
    /// </summary>
    public static bool CheckStartQuality(int Quality)
    {
        List<int> l = new List<int>();
        string value =  GetParaStr(24, 1) ;
        if (string.IsNullOrEmpty(value) == true) {
            return false ;
        }
        int length = NdUtil.GetLength(value);
        for (int i = 0 ; i < length ; i++) {
            l.Add(NdUtil.GetIntValue(value, i));
        }
        
        if (l[0] == Quality) {
            return true;
        } else {
            return false ;
        }
    }
    
    
    /// <summary>
    /// 获取所有的阶
    /// </summary>
    public static List<int> GetQualityList()
    {
        List<int> l = new List<int>();
        string value =  GetParaStr(24, 1) ;
        if (string.IsNullOrEmpty(value) == true) {
            return l ;
        }
        int length = NdUtil.GetLength(value);
        for (int i = 0 ; i < length ; i++) {
            l.Add(NdUtil.GetIntValue(value, i));
        }
        
        return l;
    }
    
    
    /// <summary>
    /// 获取之前更小的阶数列表
    /// </summary>
    public static List<int> GetPrevQuality(int Quality)
    {
        List<int> lResult = new List<int>();
        string value =  GetParaStr(24, 1) ;
        if (string.IsNullOrEmpty(value) == true) {
            return lResult ;
        }
        int length = NdUtil.GetLength(value);
        for (int i = 0 ; i < length ; i++) {
            int v = NdUtil.GetIntValue(value, i);
            if (v < Quality) {
                lResult.Add(v);
            }
        }
        
        return lResult ;
    }
    
    
    
    
    /// <summary>
    /// 确认战役点闪烁时间
    /// </summary>
    public static float GetStagePointFlashTime()
    {
        return GetParaInt(31, 1) * 0.001f;
    }
    
    /// <summary>
    /// 获取陷阱防御力参数
    /// </summary>
    public static void  GetTrapDefensePower(ref int k1, ref int k2, ref int k3)
    {
        k1 =  GetParaInt(32, 1) ;
        k2 =   GetParaInt(32, 2) ;
        k3 =  GetParaInt(32, 3) ;
    }
    
    
    
    /// <summary>
    /// 获取陷阱防御力参数
    /// </summary>
    public static void  GetSoldierPower(ref int k1, ref int k2, ref int k3, ref int k4, ref int k5)
    {
        k1 =  GetParaInt(35, 1) ;
        k2 =   GetParaInt(35, 2) ;
        k3 =  GetParaInt(35, 3) ;
        k4 =   GetParaInt(35, 4) ;
        k5 =  GetParaInt(35, 5) ;
    }
    
    //获取重新掠夺消耗金币值
    public static int GetReRobCoin()
    {
        return GetParaInt(23, 1);
    }
    //获取更换多少消耗金币值
    public static int GetChangRobCoin()
    {
        return GetParaInt(23, 2);
    }
    //获取雷达消耗
    public static int GetRadarDiamond(int times)
    {
        string value = GetParaStr(23, 1);
        int length = NdUtil.GetLength(value);
        if (times < length) {
            return NdUtil.GetIntValue(value, times);
        } else {
            return NdUtil.GetIntValue(value, length - 1);
        }
    }
    /// <summary>
    /// 获取炮弹兵技能等级上限
    /// </summary>
    /// <param name="skillNo">0:炮战、1:技能1 ... </param>
    /// <returns></returns>
    public static int GetSkillMaxLevel(int skillNo)
    {
        return GetParaStrToInt(29, 1, skillNo);
    }
    /// <summary>
    /// 金银岛船消失时间
    /// </summary>
    /// <returns>The treasure ship time.</returns>
    public static int GetTreasureShipTime()
    {
        return GetParaInt(25, 2)  * 60;
    }
    /// <summary>
    /// 体力点恢复间隔（秒）
    /// </summary>
    /// <returns></returns>
    public static int GetResumePhysicsTime()
    {
        return GetParaInt(33, 2);
    }
    /// <summary>
    /// 获取体力值上限
    /// </summary>
    /// <returns></returns>
    public static int GetMaxPhysics()
    {
        return GetParaInt(33, 1);
    }
    
    /// <summary>
    /// 技能点恢复间隔（秒）
    /// </summary>
    public static int GetResumeSkillTime()
    {
        return GetParaInt(27, 1) * 60;
    }
    /// <summary>
    ///  购买10个技能点费用（钻石）
    /// </summary>
    /// <param name="time">第几次购买</param>
    public static int GetBuyResumeSkill(int time)
    {
        string value = GetParaStr(27, 1);
        if (string.IsNullOrEmpty(value) == true) {
            return 0;
        }
        int length = NdUtil.GetLength(value);
        if (time >= length) {
            time = length - 1;
        }
        return GetParaStrToInt(27, 1, time);
    }
    /// <summary>
    /// 玩家技能点累积的上限(暂不考虑VIP)
    /// </summary>
    public static int GetMaxLeftSkillPoints(int vipLevel)
    {
        if (vipLevel == 0) {
            return GetParaInt(30, 1);
        } else {
            return GetParaInt(30, 2);
        }
    }
    /// <summary>
    /// 海神杯竞技设定:每天可挑战次数.
    /// </summary>
    /// <returns>The arena changle limit time.</returns>
    public static int GetArenaChallengeLimitTime()
    {
        return GetParaInt(34, 1);
    }
    /// <summary>
    /// 海神杯竞技设定 挑战cd.
    /// </summary>
    /// <returns>The arena changle C.</returns>
    public static int GetArenaChallengeCD()
    {
        return GetParaInt(34, 2);
    }
    /// <summary>
    /// 海神杯竞技设定:每天可挑战次数、重置cd消耗钻石.
    /// </summary>
    /// <returns>The reset arena changle time cost.</returns>
    public static int GetResetArenaChallengeTimeCost()
    {
        return GetParaInt(34, 3);
    }
    /// <summary>
    /// 海神杯购买门票次数对应的钻石花费.
    /// </summary>
    /// <returns>The arena add times cost.</returns>
    /// <param name="times">Times.</param>
    public static int GetArenaAddTimesCost(int times)
    {
        string value =  GetParaStr(34, 1) ;
        if (string.IsNullOrEmpty(value) == true) {
            return 0 ;
        }
        
        int length = NdUtil.GetLength(value);
        if (times < length && times > 0) {
            return NdUtil.GetIntValue(value, times - 1);
        } else if (length > 0  && times == 0) {
            return NdUtil.GetIntValue(value, 0);
        } else {
            return NdUtil.GetIntValue(value, length - 1);
        }
        
    }
    /// <summary>
    /// 获取技能开启的阶数
    /// </summary>
    public static int GetEnableSkillQuatlity(int index)
    {
        string value =  GetParaStr(28, 1) ;
        if (string.IsNullOrEmpty(value) == true) {
            return 0 ;
        }
        
        int length = NdUtil.GetLength(value);
        if (index <= length && index > 0) {
            return NdUtil.GetIntValue(value, index - 1);
        }
        return 10;
    }
    /// <summary>
    /// 获取当前阶开启的技能.
    /// </summary>
    public static int GetEnableSkill(int quality)
    {
        string value =  GetParaStr(28, 1) ;
        if (string.IsNullOrEmpty(value) == true) {
            return 0 ;
        }
        
        int length = NdUtil.GetLength(value);
        for (int i = 0 ; i < length ; i++) {
            int v = NdUtil.GetIntValue(value, i);
            if (v == quality) {
                return i;
            }
        }
        return -1;
    }
    
    /// <summary>
    /// 金银岛金库等级加成.
    /// </summary>
    /// <returns>The treasure vault grow.</returns>
    /// <param name="VaultLevel">Vault level.</param>
    public static int GetTreasureVaultGrow(int VaultLevel)
    {
        string value = GetParaStr(37, 1);
        if (string.IsNullOrEmpty(value) == true) {
            return 0 ;
        }
        
        int length = NdUtil.GetLength(value);
        if (VaultLevel <= length && VaultLevel > 0) {
            return NdUtil.GetIntValue(value, VaultLevel - 1);
        }
        return NdUtil.GetIntValue(value, length - 1);
    }
    // 湿身状态影响范围
    public static int GetWetBodyRange()
    {
        return GetParaInt(43, 1);
    }
    /// <summary>
    /// 泡泡被点击后隐藏时间
    /// </summary>
    public static float GetBubblePromtTime()
    {
        int time = GetParaInt(49, 1);
        return time * 0.001f;
    }
    /// <summary>
    /// 冶金需要钻石.
    /// </summary>
    /// <returns>The E money buy need diamond.</returns>
    public static int GetEMoneyBuyNeedDiamond()
    {
        return GetParaInt(52, 1);
    }
    
    /// <summary>
    /// 每日冶金次数.
    /// </summary>
    /// <returns>The day E money time.</returns>
    public static int GetDayEMoneyTime()
    {
        return GetParaInt(52, 5);
    }
    /// <summary>
    /// 拆解陷阱拆解获取碎片
    /// </summary>
    /// <param name="BuildType">陷阱星级</param>
    public static int GetBuildAnanlyzeFragment(int BuildStar)
    {
        return GetParaInt(51, BuildStar);
    }
    /// <summary>
    /// 通过身价值获取对应的身价等级
    /// </summary>
    /// <returns></returns>
    public static int GetFrutionLevel(int frutionValue)
    {
        int data1 = GetParaStrToInt(54, 1, 0);
        int data2 = GetParaStrToInt(54, 1, 1);
        int data3 = GetParaStrToInt(54, 1, 2);
        if ((frutionValue >= data1) && (frutionValue < data2)) {
            return 1;
        } else if ((frutionValue >= data2) && (frutionValue < data3)) {
            return 2;
        } else if (frutionValue > data3) {
            return 3;
        } else {
            return 0;
        }
    }
    /// <summary>
    /// 扫荡券道具ID
    /// </summary>
    /// <returns></returns>
    public static int GetSweepTicketID()
    {
        return GetParaInt(56, 1);
    }
    /// <summary>
    /// 获取船只设计图格子数区间.
    /// </summary>
    /// <returns>The ship design cell number.</returns>
    /// <param name="type">Type.</param>
    public static Int2 GetShipDesignCellRange(ShipModemType type)
    {
        int Index = (int) type;
        Int2 range = new Int2();
        if (Index == 0) {
            range.Layer = 0;
            range.Unit = 100;
        } else {
            range.Layer = GetParaStrToInt(59, Index, 0);
            range.Unit = GetParaStrToInt(59, Index, 1);;
        }
        return range;
    }
}
