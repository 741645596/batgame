using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using sdata;

/// <summary>
/// s_armyexp
/// </summary>
/// <author>zhulin</author>
/// <Fill>QFord</Fill>
///

public enum PortraitPartType {
    None = 0,
    Portrait = 0x01,
    Frame = 0x02,
    Background = 0x04,
}

public class UserM
{

    private static List<s_armyexpInfo> m_lArmyExp = new List<s_armyexpInfo>();
    private static List<s_deblockingInfo>  m_lDeblock = new List<s_deblockingInfo>();
    private static List<s_reviseheadInfo> m_lRevisehead = new List<s_reviseheadInfo>();
    
    public static void Init(object obj)
    {
        StaticDataResponse sdrsp = obj as StaticDataResponse;
        m_lArmyExp = sdrsp.s_armyexp_info;
        m_lDeblock = sdrsp.s_deblocking_info;
        m_lRevisehead = sdrsp.s_revisehead_info;
    }
    
    /// <summary>
    /// s_armyexp静态数据表
    /// </summary>
    /// <param name="level">userLevel</param>
    /// <returns></returns>
    private static s_armyexpInfo GetArmyexpInfo(int level)
    {
        foreach (s_armyexpInfo v in m_lArmyExp) {
            if (v.level == level) {
                return v;
            }
        }
        return null;
    }
    /// <summary>
    /// s_armyexp静态数据表 level下能升级到最高等级
    /// </summary>
    /// <returns>The user max hero level.</returns>
    /// <param name="level">Level.</param>
    public static int GetUserMaxHeroLevel(int level)
    {
        s_armyexpInfo  Info = GetArmyexpInfo(level);
        if (Info != null) {
            return Info.maxherolevel ;
        } else {
            return 0;
        }
    }
    /// <summary>
    /// 玩家当前等级体力值恢复上限值
    /// </summary>
    public static int GetMaxPhysical(int level)
    {
        s_armyexpInfo  Info = GetArmyexpInfo(level);
        if (Info != null) {
            return Info.maxphysical ;
        } else {
            return 0;
        }
    }
    
    
    
    /// <summary>
    /// 获取战队等级上限
    /// </summary>
    /// <returns></returns>
    public static int GetTeamMaxLevel()
    {
        for (int i = m_lArmyExp.Count - 1; i >= 0; i--) {
            if (m_lArmyExp[i].exp == 0) {
                return m_lArmyExp[i].maxherolevel;
            }
        }
        return 0;
    }
    
    /// <summary>
    /// 获取舰队等级所需经验
    /// </summary>
    public static int GetTeamExp(int level)
    {
        s_armyexpInfo  Info = GetArmyexpInfo(level);
        if (Info != null) {
            return Info.exp ;
        } else {
            return 0;
        }
    }
    /// <summary>
    /// 获取解锁模块描述
    /// </summary>
    public static string GetDeblockDesc(DEBLOCKINGFLAG flag)
    {
        int iflag = (int)flag;
        foreach (s_deblockingInfo v in m_lDeblock) {
            if (v.function_id == iflag) {
                return v.description;
            }
        }
        return string.Empty;
    }
    /// <summary>
    /// 获取普通头像
    /// </summary>
    public static List<int> GetOrdinaryPortraits(PortraitPartType portraitPartType)
    {
        List<int> portraits = new List<int>();
        for (int i = 0; i < m_lRevisehead.Count; i++) {
            int id = m_lRevisehead[i].id;
            if (id / 1000000 == (int)portraitPartType) {
                int achievement = m_lRevisehead[i].achieve;
                if (achievement > 0) {
                    continue;
                }
                //int playerAchievement = FruitionDC.GetCurFruition();
                //if (playerAchievement >= achievement)
                {
                    id = id - 1000000 * (int)portraitPartType;
                    portraits.Add(id);
                }
            }
        }
        return portraits;
    }
    /// <summary>
    /// 获取成就头像
    /// </summary>
    public static List<int> GetAchievementPortraits(PortraitPartType portraitPartType)
    {
        List<int> portraits = new List<int>();
        for (int i = 0; i < m_lRevisehead.Count; i++) {
            int id = m_lRevisehead[i].id;
            if (id / 1000000 == (int)portraitPartType) {
            }
        }
        return portraits;
    }
}
