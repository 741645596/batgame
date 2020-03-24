using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 状态目标雷达
/// </summary>
/// <author>zhulin</author>
public class StatusRadar  {

	public static void GetStatusTarget(ref List<Life> list,Life Target,int Sort ,float Range , LifeMCamp Camp)
	{
		if(list == null)
			list = new List<Life>();
		list.Clear();
		float Radius = Range/ MapGrid.m_Pixel * 0.5f;
		//
		LifeMCamp camp = (Camp == LifeMCamp.ATTACK) ? LifeMCamp.DEFENSE :LifeMCamp.ATTACK;
		if(Sort == 1)
		{
			list.Add(Target);
		}
		else if(Sort == 3)
		{
			CM.SearchLifeMList(ref list,null,LifeMType.SOLDIER,camp,MapSearchStlye.Circle,Target,Range);
		}
		else if(Sort == 5)
		{
			CM.SearchLifeMList(ref list,null,LifeMType.SOLDIER,camp,MapSearchStlye.Ball,Target,Range);
		}
	}

}
