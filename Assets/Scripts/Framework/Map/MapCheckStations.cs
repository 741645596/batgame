using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class MapCheckStations  {
	

	

	
	//占领一个攻击位，在updata 调用
	public static void RoleStation(int SceneID,MapStations station,StationsInfo info)
	{
		//拆解机器人不需要占攻击位
		Life l = CM.GetAllLifeM(SceneID,LifeMType.SOLDIER | LifeMType.SUMMONPET);
		if (l.m_Attr.AttrType == 3000)
			return;
		if (station == null) 
		{
			Debug.LogError("station is null 请调查原因");
			return ;
		}
		station.InputStations(SceneID,info);
	}

	//占领格子完成，在lateupdata 中调用
	public static  void  FinishStations()
	{
		//完成格子占领分析
		MapM.ResolveStations();
		//对格子通道进行排序
		MapM.SortGridRank ();
		MapM.ClearUpRoleStation();
	}
}
