using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Logic;
using sdata;

public class AthleticsM
{
	private static bool _IsLoad = false;
	private static List<s_monsterInfo> m_lMonsterInfo = new List<s_monsterInfo> ();
    private static List<s_athletics_ranking_rewardInfo> m_lAthleticsRankReward = new List<s_athletics_ranking_rewardInfo>();

	public static void Init (object obj)
	{
		if (_IsLoad == true)
			return;
		System.Diagnostics.Debug.Assert(obj is StaticDataResponse);
		StaticDataResponse sdrsp = obj as StaticDataResponse;
		m_lMonsterInfo = sdrsp.s_monster_info;
        m_lAthleticsRankReward = sdrsp.s_athletics_ranking_reward_info;
        SortAthleticsRankReward(ref m_lAthleticsRankReward);
		_IsLoad = true;
	}

	/// <summary>
	/// 计算怪物炮弹兵战斗力，不包括陷阱.
	/// </summary>
	/// <returns>The monstoner combat power.</returns>
	/// <param name="modeid">Modeid.</param>
	public static int GetMonstonerCombatPower(int modeid)
	{
		int combatPower = 0;
		foreach(s_monsterInfo item in m_lMonsterInfo)
		{
			if(item.modeid == modeid)
			{
				SoldierInfo info = SoldierM.GetSoldierInfo(item);
				combatPower += info.m_combat_power;
			}
		}
		return combatPower;
	}

	/// <summary>
	/// 根据副本计算战斗力.
	/// </summary>
	/// <returns>The counter part combat power.</returns>
	/// <param name="nCounterPartID">N counter part I.</param>
	public static int GetCounterPartCombatPower(int nCounterPartID)
	{
		List<SoldierInfo> lSoldInfoList = new List<SoldierInfo> ();
		List<BuildInfo> lBuildList = new List<BuildInfo> ();
		StageM.GetCounterPartShipPut (nCounterPartID,ref lSoldInfoList,ref lBuildList);

		int combatPower = 0;
		foreach(SoldierInfo info in lSoldInfoList)
		{
			combatPower += info.m_combat_power;
		}
		foreach(BuildInfo info in lBuildList)
		{
			combatPower += info.m_DefensePower;
		}
		return combatPower;
	}
    /// <summary>
    /// 获取排名区间
    /// </summary>
    public static void GetRankZone(int rank,ref int min,ref int max)
    {
        bool bingo = false;
        foreach (s_athletics_ranking_rewardInfo info in m_lAthleticsRankReward)
        {
            if (rank <= info.ranking_interval)
            {
                max = info.ranking_interval;
                if (min == 0)
                {
                    min = max;
                }
                return;
            }
            min = info.ranking_interval;
        }
    }
    /// <summary>
    /// 获取全部排行奖励数据
    /// </summary>
    public static List<s_athletics_ranking_rewardInfo> GetAllRankReward()
    {
        return m_lAthleticsRankReward;
    }
    /// <summary>
    /// 获取指定排行奖励数据
    /// </summary>
    public static s_athletics_ranking_rewardInfo GetAthleticsRankReward(int rank)
    {
        foreach (s_athletics_ranking_rewardInfo info in m_lAthleticsRankReward)
        {
            if ( rank<=info.ranking_interval )
            {
                return info;
            }
        }
        return null;
    }
    /// <summary>
    /// s_athletics_ranking_rewardInfo 表按照ranking_interval字段升序排列
    /// </summary>
    private static void SortAthleticsRankReward(ref List<s_athletics_ranking_rewardInfo> l)
    {
        if (l == null)
            return;
        l.Sort((a, b) =>
        {
            if ( a.ranking_interval >= b.ranking_interval )
            {
                return 1;
            }
            else return -1;
        });
    }
}

