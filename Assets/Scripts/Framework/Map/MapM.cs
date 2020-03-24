using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// 地图管理
/// </summary>
/// <author>zhulin</author>
/// 
/// 
public enum DIR{
	UP    =   0x01, 
	DOWN  =   0x02, 
	LEFT  =   0x03,  
	RIGHT =   0x04,
	NONE  =   0x05
};

//搜索攻击位方式
public enum SEARCHAGT{
	SAGTY_POLL = 0x00,  //轮询方式搜索
	SAGTY_HD  =  0x01,  //保持方向一致性搜索
	SAGTY_THDA  = 0x02, //保持方向一致搜索，遇到敌方阵营方向转向 
};

public class MapM  {

	private static bool m_IsLoadMap = false;
	private static List<MapGrid> m_lGrid = new List<MapGrid>();
	private static List<MapStair> m_lStair = new List<MapStair> ();
	private static List<MapStations> m_lUpRoleStation = new List<MapStations>();
	private static Dictionary<int, MapStations >m_RoleStation = new Dictionary<int, MapStations>();

	/// <summary>
	/// 创建地图
	/// </summary>
	public static void CreateMap(ShipCanvasInfo Info)
	{
		ClearMap();
		if(Info != null)
		{
			CreateMap(Info.MapSizeData,
			          Info.HoleData,
			          Info.AttackData);
		}
		else Debug.LogError("ShipCanvansInfo is null 请调查");
	}


	//创建地图
	public static void CreateMap(List<Int3> SizeList,List<Int2> Hole ,List<Int2> AttackPos)
	{
		ClearMap();
		List<StairInfo> lStair = new List<StairInfo>();
		CmCarbon.GetStairInfo(ref lStair);

		//设置地图大小
		MapSize.SetMapSize (SizeList);
		//创建格子
		MapGrid.CreateMapGrid (SizeList);
		//创建洞
		MapGrid.CreateHole (Hole);
		//用户楼梯
		MapGrid.CreateStair (lStair);
		//预设攻击位
		MapGrid.SetAttackGrid(AttackPos);

		GetGridList();
		GetStairsList();

		m_IsLoadMap = true;
	}


	private static void GetGridList()
	{
		m_lGrid.Clear();
		MapGrid.GetMapGridList (ref m_lGrid);
	}

	private static void GetStairsList()
	{
		m_lStair.Clear();
		MapStair.GetStairsList (ref m_lStair);
	}
	

	public static bool IsLoadMap()
	{
		return m_IsLoadMap;
	}


	public static void ClearMap()
	{
		m_RoleStation.Clear();
		m_lUpRoleStation.Clear();
		m_lGrid.Clear();
		m_lStair.Clear();
		MapSize.ClearMapSize ();
		MapGrid.ClearMapGrid ();
		MapStair.ClearStair ();
	}


	public static void ClearMapLife()
	{
		foreach (MapGrid m in m_lGrid) 
		{
			m.ClearLife();
		}
		foreach (MapStair m in m_lStair) 
		{
			m.ClearLife();
		}
	}
	
	//加入角色变更列表
	public static void AddUpRoleStation(MapStations m)
	{
		if(m == null) return;
		if(m_lUpRoleStation.Contains(m) == false)
		{
			m_lUpRoleStation.Add(m);
		}
	}


	public static void ChangeRoleStation(int SceneID,MapStations m)
	{
		if(m == null) return;
		if(m_RoleStation.ContainsKey (SceneID))
		{
			m_RoleStation[SceneID] = m;
		}
		else m_RoleStation.Add(SceneID,m);
	}

	//移除角色变更列表
	public static void RemoveUpRoleStation(int SceneID)
	{
		if(m_RoleStation.ContainsKey (SceneID))
		{
			m_RoleStation.Remove(SceneID);
		}
	}


	public static void ClearUpRoleStation()
	{
		m_lUpRoleStation.Clear();
	}

	
	
	public static  void EmptyRoleStations(int SceneID,LifeMType Type)
	{
		if(Type == LifeMType.SOLDIER)
		{
			if(m_RoleStation.ContainsKey(SceneID)== true)
			{
				MapStations m = m_RoleStation[SceneID];
				if(m != null)
				{
					if(m.EmptyStations(SceneID)== true)
					{
						AddUpRoleStation(m);
					}
				}
				m_RoleStation.Remove(SceneID);
			}
		}
		else if(Type == LifeMType.BUILD)
		{
			foreach (MapGrid m in m_lGrid) 
			{
				m.RemoveBuild(SceneID);
			}
		}
		else if(Type == LifeMType.WALL)
		{
			foreach (MapGrid m in m_lGrid) 
			{
				m.RemoveWall(SceneID);
			}
		}
	}
	
	//恢复变更列表
	public static void ResolveStations()
	{
		foreach (MapStations m in m_lUpRoleStation) 
		{
			if(m != null)
				m.ResolveStations();
		}
	}


	//需调整，获取不到。不改变原有的深度
	public static  int GetRankDeep(int SceneID)
	{
		int deep = -1;

		if(m_RoleStation.ContainsKey(SceneID))
		{
			MapStations m = m_RoleStation[SceneID];
			if(m != null)
				return m.GetRankDeep(SceneID);
		}
		return -1;
	}


	public static  void SortGridRank()
	{
		foreach (MapStations m in m_lUpRoleStation) 
		{
			if(m != null)
			    m.SortRank();
		}
	}



	public static void ResetGraph()
	{
		foreach (MapGrid m in m_lGrid) 
		{
			m.ResetGraph();
		}
	}

	//申请格子，防止表现途中跟墙穿帮。
	//如诺都不满足，则使用2墙中间的格子。
	public static bool AskForMapGrid(ref MapStations  m  ,DIR dir)
	{
		if (m == null || m is MapStair) 
		{
			Debug.Log("MapStations is 非法");
			return false;
		}
		
		MapGrid n = m as MapGrid;
		if (n.PropStations == StationsProp.ATTACK) 
		{
			return true;
		}				
		else 
		{
			m = n.GetNextAttackStation (dir);
			if(m != null) return true;
			else return false;
		}
	}

	private static bool AskForMapGridDeep(int SceneID,
	                                     MapGrid m ,
	                                     int StaionsDeep,
	                                     LifeMCamp Camp ,
	                                     ref Int2 Pos, 
	                                     ref int Deep,
	                                     ref int ret,
	                                     ref MapGrid n)
	{
		ret = 0;
		if (m == null ) 
		{
			ret = 2;
			return false;
		}

		int d = m.AskForStaionsDeep (StaionsDeep,Camp,ref ret);
		if(d != -1)
		{
			Deep = d;
			Pos = m.GetStationsPos();
			return true;
		}
		if (m.PropStations == StationsProp.ATTACK && m.GetGridCamp(Camp) && n == null)
			n = m;
		return false;
	}


	public static bool AskForMapGridDeep(int SceneID,
		                                 MapStations m ,
	                                     int StaionsDeep,
	                                     LifeMCamp Camp ,
	                                     DIR dir,
	                                     SEARCHAGT ST,
	                                     ref Int2 Pos, 
	                                     ref int Deep )
	{

		if (ST == SEARCHAGT.SAGTY_HD) {
			    return AskForMapGridDeepHD (SceneID, m, StaionsDeep, Camp, dir, ref Pos, ref Deep);
				} else if (ST == SEARCHAGT.SAGTY_THDA) {
			    return AskForMapGridDeepAttack (SceneID, m, StaionsDeep, Camp, dir, ref Pos, ref Deep);
				} else if (ST == SEARCHAGT.SAGTY_POLL) {
			    return AskForMapGridDeepPoll (SceneID, m, StaionsDeep, Camp, dir, ref Pos, ref Deep);
				} else
						return false;
	}
	
	
	
	public static bool AskForMapGridDeepPoll(int SceneID,
	                                     MapStations m ,
	                                     int StaionsDeep,
	                                     LifeMCamp Camp ,
	                                     DIR dir,
	                                     ref Int2 Pos, 
	                                     ref int Deep )
	{
		MapGrid l = null;
		MapGrid r = null;
		MapGrid c = null;
		MapGrid n = null; //最近的同一阵营格子
		DIR Dir = DIR.LEFT;
		int ret = 0;
		if (GetNAS (ref l, ref r, ref c, ref Dir, m, dir) == false)
			return false;

		if (AskForMapGridDeep (SceneID, c, StaionsDeep, Camp, ref Pos, ref Deep, ref ret,ref n) == true) 
		{
			return true;
		}
		    
		
		int total = 0;
		while (total < 8) 
		{
			GetNASPoll(ref l, ref r, ref Dir,ref c);
			if (AskForMapGridDeep (SceneID, c, StaionsDeep, Camp, ref Pos, ref Deep,ref ret,ref n) == true)
				return true;
			total ++;
		}
		
		Deep = 2; //使用最里面的通道
		if (n != null)
			Pos = n.GetStationsPos();
		else
		{
			Debug.Log("获取不到附近格子" + m.GetStationsPos());
			Pos = m.GetStationsPos();
		}
		return true;
	}
	
	/// <summary>
	/// 轮询获取下一个给子
	/// </summary>
	private static void GetNASPoll(ref MapGrid l,ref MapGrid r,ref DIR dir,ref MapGrid m)
	{
		if(dir == DIR.LEFT)
		{
			if(l != null)
				l = l.GetNextAttackStation(dir);
			m = l;
			dir = DIR.RIGHT;
		}
		else if(dir == DIR.RIGHT)
		{
			if(r != null)
				r = r.GetNextAttackStation(dir);
			m = r;
			dir = DIR.LEFT;
		}
		else
		{
			m = null;
		}
	}
	
	/// <summary>
	/// 保持一个方向获取格子
	/// </summary>
	public static bool AskForMapGridDeepHD(int SceneID,
	                                     MapStations m ,
	                                     int StaionsDeep,
	                                     LifeMCamp Camp ,
	                                     DIR dir,
	                                     ref Int2 Pos, 
	                                     ref int Deep )
	{
		MapGrid l = null;
		MapGrid r = null;
		MapGrid c = null;
		MapGrid n = null; //最近的同一阵营格子
		DIR Dir = DIR.LEFT;
		int ret = 0;
		if (GetNAS (ref l, ref r, ref c, ref Dir, m, dir) == false)
						return false;

		if (AskForMapGridDeep (SceneID, c, StaionsDeep, Camp, ref Pos, ref Deep,ref ret,ref n) == true)
			return true;
		
		int total = 0;
		while (total < 8) 
		{
			GetNASHoldDir(ref l, ref r, Dir,ref c);
			if (AskForMapGridDeep (SceneID, c, StaionsDeep, Camp, ref Pos, ref Deep,ref ret,ref n) == true)
				return true;
			total ++;
		}
		
		Deep = 2; 
		if (n != null)
			Pos = n.GetStationsPos();
		else
		{
			Debug.LogError("获取不到附近件格子" + m.GetStationsPos());
			Pos = m.GetStationsPos();
		}
		return true;
		
	}
	/// <summary>
	/// 保持方向一致
	/// </summary>
	private static void GetNASHoldDir(ref MapGrid l,ref MapGrid r, DIR dir,ref MapGrid m)
	{
		if(dir == DIR.LEFT)
		{
			if(l != null)
				l = l.GetNextAttackStation(dir);
			m = l;
		}
		else if(dir == DIR.RIGHT)
		{
			if(r != null)
				r = r.GetNextAttackStation(dir);
			m = r;
		}
		else
		{
			m = null;
		}
	}


	/// <summary>
	/// 遇到不同阵营改变方向
	/// </summary>
	public static bool AskForMapGridDeepAttack(int SceneID,
	                                       MapStations m ,
	                                       int StaionsDeep,
	                                       LifeMCamp Camp ,
	                                       DIR dir,
	                                       ref Int2 Pos, 
	                                       ref int Deep )
	{
		
		MapGrid l = null;
		MapGrid r = null;
		MapGrid c = null;
		MapGrid n = null; //最近的同一阵营格子
		DIR Dir = DIR.LEFT;
		int ret = 0;
		if (GetNAS (ref l, ref r, ref c, ref Dir, m, dir) == false)
			return false;
		
		if (AskForMapGridDeep (SceneID, c, StaionsDeep, Camp, ref Pos, ref Deep,ref ret,ref n) == true)
			return true;
		
		int total = 0;
		while (total < 8) 
		{
			GetNASAttack(ref l, ref r, ref Dir,ref c,ret);
			if (AskForMapGridDeep (SceneID, c, StaionsDeep, Camp, ref Pos, ref Deep,ref ret,ref n) == true)
				return true;
			total ++;
		}
		
		Deep = 2; 
		if (n != null)
			Pos = n.GetStationsPos();
		else
		{
			Debug.Log("获取不到附近格子" + m.GetStationsPos());
			Pos = m.GetStationsPos();
		}
		return true;
		
	}
	/// <summary>
	/// 遇到不同阵营改变方向
	/// </summary>
	private static void GetNASAttack(ref MapGrid l,ref MapGrid r,ref DIR dir,ref MapGrid m, int ret )
	{
		if (ret == 2) 
		{
			if(dir == DIR.LEFT)
			{
				//先跳过
				//GetNASHoldDir (ref l ,ref r, dir, ref m);
				dir = DIR.RIGHT;
			}
			else if(dir == DIR.RIGHT)
			{
				//先跳过
				//GetNASHoldDir (ref l ,ref r, dir, ref m);
				dir = DIR.LEFT;
			}
		}
		GetNASHoldDir (ref l ,ref r, dir, ref m);
	}


	private static bool GetNAS(ref MapGrid l,ref MapGrid r,ref MapGrid m ,ref DIR Dir,MapStations s, DIR dd)
	{
		if (s == null 
		    || (s is MapGrid) == false) 
		{
			Debug.Log("MapStations 非法");
			return false;
		}

		if (dd != DIR.LEFT && dd != DIR.RIGHT) 
		{
			Debug.Log("方向非法");
			return false;
		}

		Dir = dd;
		m = s as MapGrid;
		l = m;
		r = m;
		return true;
	}
	
	
}
