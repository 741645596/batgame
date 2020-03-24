using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 甲板数据
/// </summary>
/// <author>zhulin</author>


public class DeckM  {
    private static List<sdata.s_decktypeInfo> m_DeckType = new List<sdata.s_decktypeInfo>();
	private static bool _IsLoad = false;

	public static void Init (object obj)
	{
		if (_IsLoad == true)
			return;

        System.Diagnostics.Debug.Assert(obj is sdata.StaticDataResponse);

        sdata.StaticDataResponse sdrsp = obj as sdata.StaticDataResponse;

        m_DeckType = sdrsp.s_decktype_info;
	
		_IsLoad = true;

	}
	/// <summary>
	/// 根据等级获取甲板数据。
	/// </summary>
	public static  FloorInfo GetFloorInfo(int level)
	{
		FloorInfo Floor = new FloorInfo(); 
		foreach (sdata.s_decktypeInfo I in m_DeckType)
		{
			if(I.level == level)
			{
				Floor.m_level = I.level;
				Floor.m_hp = I.hp;
				Floor.m_phydefend = I.phydefend;
				Floor.m_magicdefend = I.magicdefend;
				break;
			}
		}
		return Floor;
	}
}
