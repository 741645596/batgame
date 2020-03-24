using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Logic;
using sdata;

public enum ErnieSetType
{
	None = 0,
	CoinErnie = 1,
	DiamonErnie = 2,
	SoulErnie = 3,
}
public class ErnieM
{
	private static bool _IsLoad = false;
	private static List<s_ernie_setInfo> m_lErnieInfo = new List<s_ernie_setInfo> ();

	private static List<s_top_ernie_heroInfo> m_lErnieHeroInfo = new List<s_top_ernie_heroInfo> ();

	public static void Init (object obj)
	{
		if (_IsLoad == true)
			return;
		
		System.Diagnostics.Debug.Assert(obj is StaticDataResponse);
		
		StaticDataResponse sdrsp = obj as StaticDataResponse;
		
		
		m_lErnieInfo = sdrsp.s_ernie_set_info;

		m_lErnieHeroInfo = sdrsp.s_top_ernie_hero_info; 

		_IsLoad = true;
		
		#if UNITY_EDITOR
		#endif
	}

	/// <summary>
	/// 返回类型的设置信息.
	/// </summary>
	/// <returns>The ernie set info.</returns>
	/// <param name="type">Type.</param>
	public static s_ernie_setInfo GetErnieSetInfo(ErnieSetType type)
	{
		foreach(s_ernie_setInfo item in m_lErnieInfo)
		{
			if(item.type == (int)type)
			{
				return item;
			}
		}
		return null;
	}
	/// <summary>
	/// 抽奖次数上限.
	/// </summary>
	/// <returns>The ernie lottery draw free time.</returns>
	/// <param name="type">Type.</param>
	public static int GetErnieFreeTimeLimit(ErnieSetType type)
	{
		foreach(s_ernie_setInfo item in m_lErnieInfo)
		{
			if(item.type == (int)type)
			{
				return item.free_times;
			}
		}
		return 0;
	}

	/// <summary>
	/// 抽奖CD时间.
	/// </summary>
	/// <returns>The ernie lottery draw C.</returns>
	/// <param name="type">Type.</param>
	public static int GetErnieCD(ErnieSetType type)
	{
		foreach(s_ernie_setInfo item in m_lErnieInfo)
		{
			if(item.type == (int)type)
			{
				return item.free_cd;
			}
		}
		return 0;
	}

	/// <summary>
	/// 抽奖CD时间内抽奖所需金币或者钻石数.
	/// </summary>
	/// <returns>The ernie lottery draw C.</returns>
	/// <param name="type">Type.</param>
	public static int GetErnieLotteryDrawNeed(ErnieSetType type)
	{
		foreach(s_ernie_setInfo item in m_lErnieInfo)
		{
			if(item.type == (int)type)
			{
				if(type == ErnieSetType.CoinErnie)
				{
					return item.need_coin;
				}
				else if(type == ErnieSetType.DiamonErnie || type == ErnieSetType.SoulErnie)
				{
					return item.need_diamond;
				}
			}
		}
		return 0;
	}
	/// <summary>
	/// 抽奖十次折扣.
	/// </summary>
	/// <returns>The ernie lottery draw C.</returns>
	/// <param name="type">Type.</param>
	public static int GetErnieTenDisCount(ErnieSetType type)
	{
		foreach(s_ernie_setInfo item in m_lErnieInfo)
		{
			if(item.type == (int)type)
			{
				return item.ten_agio;
			}
		}
		return 100;
	}

	/// <summary>
	/// 获取购买所需金币.
	/// </summary>
	/// <returns>The buy need coin or diamond.</returns>
	/// <param name="type">Type.</param>
	/// <param name="isTenBuy">If set to <c>true</c> is ten buy.</param>
	public static int GetBuyNeedCoinOrDiamond(ErnieSetType type,bool isTenBuy = false)
	{
		if(isTenBuy)
		{
			return ErnieM.GetErnieLotteryDrawNeed (type)* 10 * ErnieM.GetErnieTenDisCount (type) / 100 ;
		}
		else
			return ErnieM.GetErnieLotteryDrawNeed (type);
	}

	/// <summary>
	/// 获取本周热点英雄.
	/// </summary>
	/// <param name="id">Identifier.</param>
	/// <param name="HeroId">Hero identifier.</param>
	public static void GetWeekHotHero(int id,ref int HeroId)
	{
		foreach(s_top_ernie_heroInfo info in m_lErnieHeroInfo)
		{
			if(info.id == id)
			{
				HeroId = info.week_hero_id;
			}
		}
	}

	/// <summary>
	/// 获取本日热点英雄.
	/// </summary>
	/// <param name="id">Identifier.</param>
	/// <param name="lHeroIdList">L hero identifier list.</param>
	public static void GetDayHotHeroList(int id,ref List<int> lHeroIdList)
	{
		if(lHeroIdList == null)
			lHeroIdList = new List<int>();

		lHeroIdList.Clear ();
		foreach(s_top_ernie_heroInfo info in m_lErnieHeroInfo)
		{
			if(info.id == id)
			{
				lHeroIdList.Add( info.day_hero_soul_1);
				lHeroIdList.Add( info.day_hero_soul_2);
				lHeroIdList.Add( info.day_hero_soul_3);
			}
		}
	}

	public static int GetID()
	{
		int now = GlobalTimer.GetNowTimeInt ();
		DateTime start = NDTime.GetServerTime(now);
		return int.Parse(start.ToLocalTime().ToString("yyyyMMdd"));
	}
}

