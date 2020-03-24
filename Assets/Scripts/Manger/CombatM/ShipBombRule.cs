using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 爆炸规则
/// </summary>
/// <author>zhulin</author>

/// <summary>
/// 爆炸路径
/// </summary>
public enum BombRoad
{
	GoldStart   = 0,  //金库起点
	LeftUp      = 1,  //左上区块
	RightDown   = 2,  //右下区块
	LeftDown    = 3,  //左下区块
	RightUp     = 4,  //右上区块
	LeftMiddle  = 5,  //左中
	RightMiddle = 6,  //右中
	Middle      = 7,  //正中
    Over        = 8,  //over	
}


/// <summary>
/// 爆炸过程类型，起始 ，中间，结束
/// </summary>
public enum BombProcessType
{
	Start      = 0,  //起始区块
	Middle     = 1,  //中间爆炸区块
	End        = 2,  //结束爆炸区块
}


public class ShipBombRule  {
	//爆炸区块路径
	private static Dictionary <BombRoad ,BombArea> m_BombRoad = new Dictionary <BombRoad ,BombArea>();
	//当前播放区块
	private static BombRoad m_CurBombRoad = BombRoad.GoldStart;
	/// <summary>
	/// 清理爆炸相关数据
	/// </summary>
	public static void ClearBombData()
	{
		m_BombRoad.Clear();
	}
	//设置爆炸地图
	public static void SetBombMap(ShipCanvasInfo Info)
	{
		ClearBombData();
		if(Info == null) return ;
		Int2 StartPos  = Int2.zero;
		List<Int2> lLinkPos = new List<Int2>();
		//LeftUp
		Info.GetMapAreaRoom(MapArea.LeftUp ,ref StartPos , ref lLinkPos);
		BombArea bombarea = new BombArea();
		bombarea.ProcessType = BombProcessType.Middle;
		bombarea.SetArea(StartPos ,lLinkPos);
		m_BombRoad.Add(BombRoad.LeftUp ,bombarea);
		//RightDown
		Info.GetMapAreaRoom(MapArea.RightDown ,ref StartPos , ref lLinkPos);
		bombarea = new BombArea();
		bombarea.ProcessType = BombProcessType.Middle;
		bombarea.SetArea(StartPos ,lLinkPos);
		m_BombRoad.Add(BombRoad.RightDown ,bombarea);
		//LeftDown
		Info.GetMapAreaRoom(MapArea.LeftDown ,ref StartPos , ref lLinkPos);
		bombarea = new BombArea();
		bombarea.ProcessType = BombProcessType.Middle;
		bombarea.SetArea(StartPos ,lLinkPos);
		m_BombRoad.Add(BombRoad.LeftDown ,bombarea);
		//RightUp
		Info.GetMapAreaRoom(MapArea.RightUp ,ref StartPos , ref lLinkPos);
		bombarea = new BombArea();
		bombarea.ProcessType = BombProcessType.Middle;
		bombarea.SetArea(StartPos ,lLinkPos);
		m_BombRoad.Add(BombRoad.RightUp ,bombarea);
		//LeftMiddle
		Info.GetMapAreaRoom(MapArea.LeftMiddle ,ref StartPos , ref lLinkPos);
		bombarea = new BombArea();
		bombarea.ProcessType = BombProcessType.Middle;
		bombarea.SetArea(StartPos ,lLinkPos);
		m_BombRoad.Add(BombRoad.LeftMiddle ,bombarea);
		//RightMiddle
		Info.GetMapAreaRoom(MapArea.RightMiddle ,ref StartPos , ref lLinkPos);
		bombarea = new BombArea();
		bombarea.ProcessType = BombProcessType.Middle;
		bombarea.SetArea(StartPos ,lLinkPos);
		m_BombRoad.Add(BombRoad.RightMiddle ,bombarea);
		//Middle
		Info.GetMapAreaRoom(MapArea.Middle ,ref StartPos , ref lLinkPos);
		List<List<Int2>> lCenterPos = new List<List<Int2>>();
		Info.GetMapCenterAreaRoom(ref lCenterPos);
		bombarea = new BombArea();
		bombarea.ProcessType = BombProcessType.End;
		bombarea.SetArea(StartPos ,lLinkPos);
		bombarea.SetCenterArea(lCenterPos);
		m_BombRoad.Add(BombRoad.Middle ,bombarea);
	}
	/// <summary>
	/// 添加普通建筑
	/// </summary>
	/// <param name="lRoomPos">建筑所在房间区域</param>
	/// <param name="room">建筑对象</param>
	/// <returns></returns>
	public static void JoinBuildRoom(List<Int2> lRoomPos ,Building room)
	{
		if(lRoomPos == null || lRoomPos.Count == 0 || room == null)
			return ;
		foreach(Int2 Pos in lRoomPos)
		{
			List<BombPoint> l = FindAreaRoom(Pos);
			foreach(BombPoint P in l)
			{
				P.SetBuild(room);
			}
		}

	}
	/// <summary>
	/// 添加金库建筑
	/// </summary>
	/// <param name="lRoomPos">建筑所在房间区域</param>
	/// <param name="room">建筑对象</param>
	/// <returns></returns>
	public static void JoinGoldBuildRoom(List<Int2> lRoomPos ,Building room)
	{
		if(lRoomPos == null || lRoomPos.Count == 0 || room == null)
			return ;
		BombArea bombarea = new BombArea();
		bombarea.ProcessType = BombProcessType.Start;
		bombarea.SetArea(lRoomPos ,new List<Int2> ());
		m_BombRoad.Add(BombRoad.GoldStart ,bombarea);
		//
		foreach(Int2 Pos in lRoomPos)
		{
			List<BombPoint> l = FindAreaRoom(Pos);
			foreach(BombPoint P in l)
			{
				P.SetBuild(room);
			}
		}
	}
	/// <summary>
	/// 添加甲板建筑
	/// </summary>
	/// <param name="lRoomPos">建筑所在房间区域</param>
	/// <param name="room">建筑对象</param>
	/// <returns></returns>
	public static void JoinDeckBuildRoom(List<Int2> lRoomPos ,Building room)
	{
		if(lRoomPos == null || lRoomPos.Count == 0 || room == null)
			return ;
		if(m_BombRoad.ContainsKey (BombRoad.Middle) == true)
		{
			BombArea bombarea = m_BombRoad[BombRoad.Middle];
			if(bombarea == null) return ;
			bombarea.AddArea(lRoomPos ,room);
		}
	}


	/// <summary>
	/// 查找包含该房间的区块
	/// </summary>
	private static List<BombPoint> FindAreaRoom(Int2  Pos)
	{
		List<BombPoint> list = new List<BombPoint>();
		foreach(BombArea p in m_BombRoad.Values)
		{
			List<BombPoint> l = new List<BombPoint>();
			l = p.FindAreaRoom(Pos);
			if(l != null  && l.Count > 0)
			{
				list.AddRange(l);
			}
		}
		return list;
	}


	//获取开始爆炸区块
	public static BombArea GetStartBombArea()
	{
		m_CurBombRoad = BombRoad.GoldStart ;
		if(m_BombRoad.ContainsKey (m_CurBombRoad) == true)
		{
			return m_BombRoad [m_CurBombRoad];
		}
		else return null;
	}


	//获取下一个爆炸区块
	public static BombArea GetNextBombArea()
	{
		int value = (int)m_CurBombRoad ;
		value ++;
		m_CurBombRoad = (BombRoad) value;
		if(m_BombRoad.ContainsKey (m_CurBombRoad) == true)
		{
			return m_BombRoad [m_CurBombRoad];
		}
		else return null;
	}
}








