using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 船只建筑数据转换。
/// </summary>
/// 房间的血量 = 本身的血量 + 所有陷阱的血量  + 甲板的血量。
/// 房间的血量 = 本身的血量 +所有陷阱的血量　。处理合并掉。
/// 
public class ShipBuildData  {

	/// <summary>
	/// 保持战斗建筑摆设数据
	/// </summary>
	public static void SaveShipBuildData(battle.ShipBuildInfo  item ,
	                                     ref Dictionary<int,BuildInfo>lBuild)
	{
		if(item == null ||  lBuild == null)
			return;
		if(item.type == (int) ShipBuildType.BuildRoom || item.type == (int) ShipBuildType.BuildStair)
		{
			BuildInfo RoomBuild = buildingM.GetBuildInfo(item);
			if(RoomBuild  != null)
			{
				int RoomBuildID = CmCarbon.GetDataID();
				lBuild.Add(RoomBuildID, RoomBuild);
			}
		}
	}

	/// <summary>
	/// 保持战斗建筑摆设数据
	/// </summary>
	public static void SaveShipBuildData(BuildInfo item , 
	                                     ref Dictionary<int,BuildInfo>lBuild)
	{
		if(item == null ||  lBuild == null)
			return;
		int RoomBuildID = CmCarbon.GetDataID();
		lBuild.Add(RoomBuildID, item);
	}
	/// <summary>
	/// 保持战斗建筑摆设数据
	/// </summary>
	public static void SaveShipBuildData(ShipPutInfo item ,
	                                     ref Dictionary<int,BuildInfo>lBuild)
	{
		if(item == null || lBuild == null)
			return;
		if(item.type == (int) ShipBuildType.BuildRoom || item.type == (int) ShipBuildType.BuildStair)
		{
			BuildInfo RoomBuild = item.GetBuildInfo( );
			if(RoomBuild != null )
			{
				int RoomBuildID = CmCarbon.GetDataID();
				lBuild.Add(RoomBuildID, RoomBuild);
			}
		}
	}
}
