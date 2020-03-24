using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class StairInfo 
{
	public Int2 Up = Int2.zero;
	public Int2 Down = Int2.zero;
	//public List<Int2> m_NearHole = new List<Int2>();

	public StairInfo (Int2 StairBuildPos) 
	{
		Up.Layer = StairBuildPos.Layer + 1;
		Up.Unit = StairBuildPos.Unit + MapGrid.m_UnitRoomGridNum / 2;
		Down.Layer = StairBuildPos.Layer ;
		Down.Unit = StairBuildPos.Unit + MapGrid.m_UnitRoomGridNum / 2;

		//m_NearHole.Add(new Int2 (StairBuildPos.Unit + 1 ,StairBuildPos.Layer + 1));
		//m_NearHole.Add(new Int2 (StairBuildPos.Unit + 3 ,StairBuildPos.Layer + 1));
		//m_NearHole.Add(new Int2 (StairBuildPos.Unit + 4 ,StairBuildPos.Layer + 1));
	}
};
/// 
public class MapStair : MapStations  {

	private static List<MapStair> m_Stair = new  List<MapStair>();
	private int m_StairID = 0;
	private Int2 m_Up = Int2.zero;
	private Int2 m_Down = Int2.zero;
	private Building1201 m_stair = null;


	public int StairID
	{
		get { return m_StairID; }
		set{
			m_StairID = value;
		}
	}


	public override void Init()
	{
		m_HoldRole = new RoleStairStations();
		m_TempRole = new RoleStairStations();
	}


	public static void AddStair(MapStair stair)
	{
		if(m_Stair == null)
			m_Stair = new  List<MapStair>();
		m_Stair.Add (stair);
	}

	public static void ClearStair()
	{
		if(m_Stair == null)
			m_Stair = new  List<MapStair>();
		m_Stair.Clear ();
	}


	public static void GetStairsList(ref List<MapStair> list)
	{
		if (list == null)
			list = new List<MapStair> ();
		else list.Clear ();
		
		foreach (MapStair m in m_Stair) 
		{
			list.Add(m);
		}
	}


	public static MapStair GetStair(Int2 Grid, bool IsUp)
	{
		if (m_Stair == null || m_Stair.Count == 0)
						return null;
		foreach(MapStair m in m_Stair)
		{
			if(m.IsStair(Grid ,IsUp) == true)
				return m;
		}
		return null;
	}

	public Int2 GetStairUp()
	{
		return m_Up;
	}

	public Int2 GetStairDown()
	{
		return m_Down;
	}
	//
	public void  SetStair(Int2 Up ,Int2 Down)
	{
		m_Up = Up;
		m_Down = Down;
		StairID = NdUtil.GetSceneID();
	}



	public bool IsStair(Int2 pos , bool IsUp)
	{
		if(IsUp == true && pos == m_Up)
			return true;
		else if(IsUp == false && pos == m_Down)
			return true;
		 else return false;
	}

		
	//获取最大拥挤数
	public  override int GetMaxJam()
	{
		return 1;
	}

	public override string GetStationInfo()
	{
		return  "("+m_Up.ToString() +","+ m_Down.ToString() + ")";
	}


	//获取深度
	public override int GetRankDeep(int SceneID)
	{
		StationsInfo Info =m_HoldRole.GetRoleStationsInfo(SceneID);
		if(Info != null)
		{
			return Info.m_StaionsDeep;
		}
		
		Info =m_TempRole.GetRoleStationsInfo(SceneID);
		if(Info != null)
		{
			return Info.m_StaionsDeep;
		}
		return  -1;

	}
	/// <summary>
	/// 加入楼梯对象
	/// </summary>
	public void JoinStairLife(Building1201 stair)
	{
		m_stair = stair ;
	}

	/// <summary>
	/// 获取楼梯对象
	/// </summary>
	public Building1201 GetStairLife()
	{
		return m_stair;
	}


	/// <summary>
	/// 清除格子数据中的life相关数据
	/// </summary>
	/// <returns></returns>
	public void ClearLife()
	{
		ClearRoleStations();
	}
}
