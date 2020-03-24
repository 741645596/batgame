#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#define UNITY_EDITOR_LOG
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//




public class PathData
{
	public MapGrid Road;
	public RoleState state;
	public float deltaTime;
	public WalkDir dir;
	public GridSpace gs;

	public PathData( PathData s)
	{
		this.Road = s.Road;
		this.state = s.state;
		this.deltaTime = s.deltaTime;
		this.dir = s.dir;
		this.gs = s.gs;
	}
	
	public PathData( tga.pathroad s)
	{
		this.Road = MapGrid.GetMG(s.road.layer,s.road.unit);
		this.state = (RoleState)s.state;
		this.deltaTime = s.deltaTime * 0.001f;
		this.dir = (WalkDir)s.dir;
		this.gs = (GridSpace)s.gs;
	}
	public PathData()
	{
	}

	/// <summary>
	/// 检测是否有可触发陷阱，判断标准，为陷阱，且不处于cd
	/// </summary>
	/// <returns>false，没有，true 有</returns>
	public bool CheckTrap()
	{
		if(Road == null) return false;
		List<int> l = new List<int>();
		Road.GetBuildList(ref l);
		foreach(int SceneID in l)
		{
			Life life = CM.GetLifeM(SceneID ,LifeMType.BUILD);
			if(life == null) continue;
			if (life is Building)
			{
				Building build = life as Building;
				if ( !build.IsInCDStatus())
					return true;
			}
		}
		return false;
	}

	/// <summary>
	/// 确定是否为障碍，墙体,资源房，陷阱房间
	/// </summary>
	/// <returns>false，没有，true 有</returns>
	public bool HaveObstacles()
	{
		if(Road == null) return false;
		return Road.HaveBuildRoom();
	}
	public tga.pathroad toPathRoad()
	{
		tga.pathroad  tp = new tga.pathroad();
		tp.deltaTime = (int)(deltaTime * 1000);
		tp.dir = (int)dir;
		tp.gs = (int)gs;
		tp.road =new tga.GridPos();
		tp.road.layer = Road.GridPos.Layer;
		tp.road.unit = Road.GridPos.Unit;
		tp.state = (int)state;
		return tp;
	}
}

public class AIPathData  {
	//计算状态所需数据
	private static float jump_distance =3;
	private static float speed =1.0f;
	private static float jumpTime = 0.5f;
	private static float fallTime = 1.0f;
	private static float StairDistent =6.0f;
	private static float jumpStartTime =0.2f;
	private static float jumpEndTime = 0.2f;
	private static float stairInTime =0.05f;
	private static float stairOutTime = 0.2f;
	private static bool  g_IsHandstand = false;   //受否躲避陷阱，倒立行走
	private static int   g_SceneID = 0;           //角色ID
	private static bool  g_KeepDir = false;       //保持寻路方向
	private static bool g_IsInit = false;         //是否有设置寻路参数标识



	/// <summary>
	/// 设置寻路参数
	/// </summary>
	/// <param name="SceneID">寻路对象sceneID 用于对日志的输出</param>
	/// <param name="fspeed">对象的行走速度</param>
	/// <param name="fjump_distance">对象的跳跃距离</param>
	/// <param name="IsHandstand">是否遇障碍倒立行走</param>
	/// <param name="KeepDir">是否按指定方向寻路</param>
	public static void SetPathParam(int SceneID ,float fspeed , float fjump_distance ,bool IsHandstand ,bool KeepDir)
	{
		speed = fspeed;
		jump_distance = fjump_distance;
		g_SceneID = SceneID;
		g_IsHandstand = false;
		g_KeepDir = KeepDir;
		g_IsInit = true;
	}

	/// <summary>
	/// 搜寻路径
	/// </summary>
	/// <param name="start">起点</param>
	/// <param name="end">目标</param>
	/// <param name="PathList">对象的跳跃距离</param>
	/// <param name="StartGS">所在的格子空间</param>
	/// <param name="Curdir">当前的方向</param>
	/// <returns>返回搜索列表list, false 寻路失败</returns>
	public static bool SearchPath(MapGrid start ,MapGrid end  ,ref List<PathData> PathList,GridSpace StartGS,WalkDir Curdir)
	{
		#if UNITY_EDITOR_LOG
		if(g_IsInit == false)
		{
			Debug.LogError("寻路之前，请先设置寻路参数，调用SetPathParam");
		}
		#endif
		//搜索路径
		if (PathList == null)
			PathList = new List<PathData> ();
		List<MapGrid> list = new List<MapGrid>();
		if(g_KeepDir == true)
		{
			list = GetFrontPath (start, end,Curdir);
		}
		else list = MapPath.GetPath (start, end);
		
		#if UNITY_EDITOR_LOG
		string str = "SearchPath "+g_SceneID + ",";
		for(int i = 0; i < list.Count; i++)
		{
			str += "{" + list[i].GridPos + "," + list[i].Type + "," + list[i].PropStations + "," + list[i].UpHaveHole + "," + list[i].HaveTrap() + "}  ";
		}
		#endif
		if (list.Count == 0) return true;
		#if UNITY_EDITOR_LOG
		FileLog.write(g_SceneID,str,true);
		#endif
		//分析状态
		AnalysisPath(list ,ref PathList,StartGS);
		list.Clear ();
		g_IsInit = false;
		return true;
	}

	//熊减防一直往前走
	public static List<MapGrid> GetFrontPath(MapGrid start,MapGrid end,WalkDir curdir)
	{
		List<MapGrid> list = new List<MapGrid>();
		if (start == null)
			return list;
		int flag = curdir == WalkDir.WALKLEFT?-1:1;
		int layer = start.GridPos.Layer;
		int unit = start.GridPos.Unit;
		//list.Add(start);
		MapGrid next  = start;
		do
		{
			list.Add(next);
			unit += flag;
			next = MapGrid.GetMG(layer,unit );
		}while (next != null && next.Type != GridType.GRID_HOLE && next.Type != GridType.GRID_WALL && ( !next.IsAttackStations() || next.CheckIdleAttackStation()));
		while(list.Count >0)
		{
			if (!list[list.Count - 1].IsAttackStations())
			{
				list.RemoveAt(list.Count - 1);
			}
			else
			{
				break;
			}
		}
		return list;

	}


	//验证下一个状态不通过角色列表
	//返回值，true 有不通过角色，具体参看list 返回
	public static  bool  GetIllegalList(ref List<int>BackList)
	{
		if(BackList == null) 
			BackList = new List<int>();
		BackList.Clear ();

		
		return BackList.Count > 0 ? true : false;
	}
	
	private static bool TerrainAI(List<MapGrid> list , ref List<MapGrid> l ,float JumpDistance)
	{
		if (l == null) l = new List<MapGrid> ();
		l.Clear ();
		//
		TerrainTailAI (list , ref l ,JumpDistance);		
		#if UNITY_EDITOR_LOG
		string str = "SearchPath "+g_SceneID + ",";
		for(int i = 0; i < l.Count; i++)
		{
			str += "{" + l[i].GridPos + "," + l[i].Type + "," + l[i].UpHaveHole + "," + l[i].HaveTrap() + "}  ";
		}
		FileLog.write(g_SceneID,str,true);
		#endif
		if (TerrainPathLegal (g_SceneID,l, JumpDistance) == false) 
		{
			Debug.Log ("TerrainAI路径非法，请调查原因" + g_SceneID);
			return false;
		}
		return true;
	}
	//补充路径结尾段
	//1.正常路径结尾
	//2.最后一个点位hole，需加入hole 最下面的那个点
	//3.洞过长，需进行裁剪，
	//4.最后一个路径点不能为洞 ，算出这种情况需找出原因
	private static void TerrainTailAI(List<MapGrid> list ,ref List<MapGrid> l ,float JumpDistance)
	{
		if (l == null) l = new List<MapGrid> ();
		l.Clear ();
		int nHole = 0;
		int StartDisorder = 0;
		int nUpHole = 0;
		int walkStyle = 0;
		if (list == null || list.Count == 0) return;
		//
		MapGrid FirstHole = null;
		for (int i = 0; i < list.Count; i++) 
		{
			if (g_IsHandstand == true)
			{
				if (list[i].HaveTrap())
				{
					if (StartDisorder == 0 && !list[i].UpHaveHole)
					{
						walkStyle = 1;
					}
					StartDisorder ++;
				}
				else
				{
					StartDisorder = 0;
					walkStyle = 0;
				}
				if (walkStyle == 1 && list[i].UpHaveHole)
				{
					if (i > 0 && list[i-1].GridPos.Layer != list[i].GridPos.Layer)
					{
						
						for(int j = 0 ; j <= nHole;j++)
							l.Add(list[i-j]);
						nHole = 0;
					}
					else
						nHole = nHole + 1;
						//洞的长度大于跳跃距离情况
				}
				else if(walkStyle == 0  &&(list[i].Type == GridType.GRID_HOLE || list[i].Type == GridType.GRID_HOLESTAIR))
				{
					//当起始点是洞的时候
					if ( i == 0 )
					{
						
						l.Add(list[i]);
						if (list.Count == 1 || 
						    (i < list.Count -1 && list[i].GridPos.Layer > list[i+1].GridPos.Layer)
						    ||(i < list.Count -1 && list[i].GridPos.Layer == list[i+1].GridPos.Layer && list[i].Type == GridType.GRID_HOLE)) 
						{
							MapGrid m = MapGrid.GetHoleBottom(list[i]);
							if(m != null) l.Add(m);
							break;
						}
					}
					else
					{
						//不需要跳直接掉落的情况
						if (nHole == 0)
						{
							if (i > 0 && list[i-1].GridPos.Layer != list[i].GridPos.Layer)
							{
								l.Add(list[i]);
								nHole = 0;
							}
							else nHole ++;
							FirstHole = list[i];
							
							if (i < list.Count -1 && list[i].GridPos.Layer > list[i+1].GridPos.Layer)
							{
								l.Add(list[i]);
								MapGrid m = MapGrid.GetHoleBottom(list[i]);
								if(m != null) l.Add(m);
								break;
							}
						}
						if (nHole > 0)
						{
							
							nHole = nHole + 1;
							//连续洞中央掉落情况
							if(i < list.Count -1 && list[i].GridPos.Layer > list[i+1].GridPos.Layer)
							{
								l.Add(FirstHole);
								MapGrid m = MapGrid.GetHoleBottom(list[i]);
								if(m != null) l.Add(m);
								break;
							}
							if(i < list.Count -1 && list[i].GridPos.Layer < list[i+1].GridPos.Layer)
							{
								l.Add(list[i]);
								nHole = 0;
								continue;
							}
							
							//洞的长度大于跳跃距离情况
							if(nHole >= JumpDistance) //路径需结束了 进入fall了
							{
								//加入fall 点
								l.Add(list[i]);
								MapGrid m = MapGrid.GetHoleBottom(list[i]);
								if(m != null) l.Add(m);
								else l.Add(list[list.Count - 1]);
								break;
							}
						}
					}
				}
				else 
				{
					l.Add(list[i]);
					nHole = 0;
				}
			}
			else
			{
				if(list[i].Type == GridType.GRID_HOLE || list[i].Type == GridType.GRID_HOLESTAIR)
				{
					//当起始点是洞的时候
					if ( i == 0 )
					{
						
						l.Add(list[i]);
						if (list.Count == 1 || 
						    (i < list.Count -1 && list[i].GridPos.Layer > list[i+1].GridPos.Layer)
						    ||(i < list.Count -1 && list[i].GridPos.Layer == list[i+1].GridPos.Layer && list[i].Type == GridType.GRID_HOLE)) 
						{
							MapGrid m = MapGrid.GetHoleBottom(list[i]);
							if(m != null) l.Add(m);
							break;
						}
					}
					else
					{
						//不需要跳直接掉落的情况
						if (nHole == 0)
						{
							if (i > 0 && list[i-1].GridPos.Layer != list[i].GridPos.Layer)
							{
								l.Add(list[i]);
								nHole = 0;
							}
							else nHole ++;
							FirstHole = list[i];
							if (i < list.Count -1 && list[i].GridPos.Layer > list[i+1].GridPos.Layer)
							{
								l.Add(list[i]);
								MapGrid m = MapGrid.GetHoleBottom(list[i]);
								if(m != null) l.Add(m);
								break;
							}
						}
						if (nHole > 0)
						{

							nHole = nHole + 1;
							//连续洞中央掉落情况
							if(i < list.Count -1 && list[i].GridPos.Layer > list[i+1].GridPos.Layer)
							{

								/*l.Add(list[i]);
								MapGrid m = MapGrid.GetHoleBottom(list[i]);*/
								l.Add(FirstHole);
								MapGrid m = MapGrid.GetHoleBottom(FirstHole);
								if(m != null) l.Add(m);
								break;
							}
							if(i < list.Count -1 && list[i].GridPos.Layer < list[i+1].GridPos.Layer)
							{
								l.Add(list[i]);
								nHole = 0;
								continue;
							}

							//洞的长度大于跳跃距离情况
							if(nHole >= JumpDistance) //路径需结束了 进入fall了
							{
								//加入fall 点
								l.Add(list[i]);
								MapGrid m = MapGrid.GetHoleBottom(list[i]);
								if(m != null) l.Add(m);
								else l.Add(list[list.Count - 1]);
								break;
							}
						}
					}
				}
				else 
				{
					l.Add(list[i]);
					nHole = 0;
				}
			}
		}
		//加入路径结束点
		if (l.Count > 0) 
		{
			//加入最后一个点目标的目的是 便于寻路更新，保证目标在路径上
			if (list [list.Count - 1] != l [l.Count - 1]) 
			{
				l.Add(list [list.Count - 1]);
			}
		}
		//最后一个点位洞
		if(l [l.Count - 1].Type == GridType.GRID_HOLE || l [l.Count - 1].Type == GridType.GRID_HOLESTAIR)
		{
			MapGrid n = MapGrid.GetHoleBottom(l [l.Count - 1]);
			if(n == null)
			{
				Debug.Log("格子数据异常，请调查原因");
			}
			else l.Add(n);
		}
	}
	
	//路径合法性判断
	private static bool TerrainPathLegal(int SceneID,List<MapGrid> l ,float JumpDistance)
	{
		if (l == null || l.Count == 0) return true;
		if (l.Count == 1) 
		{
			if (l [0] == null || l [0].Type == GridType.GRID_HOLE) 
			{
				Debug.Log ("路径非法，请调查原因");
				return false;
			} 
			else return true;
		} 
		else if (l.Count == 2) 
		{
			if (l [0] == null 
			    ||l [1] == null 
			    || l [1].Type == GridType.GRID_HOLE) 
			{
				Debug.Log ("路径非法，请调查原因");
				return false;
			} 
			if(l [0].Type == GridType.GRID_HOLE)
			{
				MapGrid m = MapGrid.GetHoleBottom(l [0]);
				if(m == l [1]) return true;
				return false;
			}
			return true;
		}
		else 
		{
			for (int i = 0; i < l.Count -2; i++) 
			{
				if(MapGrid.PathGridLegal(l[i] , l[i+1] ,JumpDistance) == false)
				{
					Debug.Log ("PathGridLegal路径非法，请调查原因" + l[i].GetStationInfo() +"XXX" + l[i+1].GetStationInfo() + SceneID);
					return false;
				}
			}
			//
			if (l [l.Count -2] == null 
			    ||l [l.Count -1] == null 
			    || l [l.Count -1].Type == GridType.GRID_HOLE) 
			{
				return false;
			} 
			if(l [l.Count -2].Type == GridType.GRID_HOLE)
			{
				MapGrid m = MapGrid.GetHoleBottom(l [l.Count -2]);
				if(m == l [l.Count -1]) return true;
				return false;
			}
			return true;
		}
	}
	
	
	
	//分析路径状态 核心算法
	private static void AnalysisPath(List<MapGrid> list  ,ref List<PathData> PathList,GridSpace StartGS)
	{
		if (PathList == null)
			PathList = new List<PathData> ();
		PathList.Clear ();
		RoleState PreState =RoleState.WALK;
		RoleState state = RoleState.WALK;
		RoleState next = RoleState.WALK;
		WalkDir dir = WalkDir.WALKSTOP; 
		WalkDir ndir = WalkDir.WALKSTOP; 
		WalkDir PreDir = WalkDir.WALKSTOP;
		int TimeCycle = 1;
		int nTimeCycle = 1;
		//
		GridSpace currentgs = StartGS;
		if (list == null || list.Count == 0) return;
		List<MapGrid> l = new List<MapGrid> ();
		TerrainAI (list, ref  l, jump_distance);
		/*if (TerrainAI (list, ref  l, jump_distance) == false)
			return;*/
		#if UNITY_EDITOR_LOG
		string str = "SearchPath "+g_SceneID + ",";
		if (l.Count > 0  )
		{
			for(int i = 0; i < l.Count; i++)
			{
				str += l[i].GridPos + "," + l[i].Type +",";
			}
			//Debug.Log(str);
		}
		FileLog.write(g_SceneID,str,true);
		#endif
		if (l == null || l.Count == 0) 
		{
			return ;
		} 
		else if (l.Count == 1) 
		{
			AddPathRoad(ref PathList,l[0],WalkDir.WALKSTOP,WalkDir.WALKSTOP,RoleState.WALK,RoleState.WALK, RoleState.WALK,1,StartGS);
			PathRoadStateEx(ref PathList,l[0] ,l[0]);
			return;
		} 
		else if (l.Count == 2) 
		{
			state = MapGrid.GetRoleState(l[0] ,l[1],ref dir,ref TimeCycle,g_IsHandstand,ref currentgs);
			AddPathRoad(ref PathList,l[0],dir,WalkDir.WALKSTOP,RoleState.WALK,state, RoleState.WALK ,TimeCycle,StartGS);
			AddPathRoad(ref PathList,l[1],WalkDir.WALKSTOP,dir,state,RoleState.WALK, RoleState.WALK,1,StartGS);

			PathRoadStateEx(ref PathList,l[0] ,l[1]);

			return;
		} 
		else  // > 3
		{
			for (int i = 0; i<l.Count -2; i ++) 
			{
				state	= MapGrid.GetRoleState(l[i] ,l[i +1],ref dir,ref TimeCycle,g_IsHandstand,ref currentgs);
				GridSpace temp = currentgs;
				next =MapGrid.GetRoleState(l[i + 1] ,l[i + 2],ref ndir,ref nTimeCycle,g_IsHandstand,ref currentgs);
				currentgs = temp;
				AddPathRoad(ref PathList,l[i],dir,PreDir,PreState,state, next ,TimeCycle,StartGS);
				PreState = state;
				PreDir = dir;
			}
			
			//最后2个点
			
			if (state == RoleState.FALL)
				state = RoleState.FALLDOWN;
			else
				state = MapGrid.GetRoleState(l[l.Count -2] ,l[l.Count -1],ref dir,ref TimeCycle,g_IsHandstand,ref currentgs);
			AddPathRoad(ref PathList,l[l.Count -2],dir,PreDir,PreState,state, RoleState.WALK ,TimeCycle,StartGS);
			AddPathRoad(ref PathList,l[l.Count -1],WalkDir.WALKSTOP,dir,state,RoleState.WALK, RoleState.WALK ,1,StartGS);

			PathRoadStateEx(ref PathList,l[0] ,l[1]);

			return;
		}
		
	}


	private static void PathRoadStateEx(ref List<PathData> PathList, MapGrid start , MapGrid end)
	{
		if (PathList != null && PathList.Count > 0) 
		{
			if(PathList[0].state == RoleState.FALLDOWN)
			{
				PathList[0].state =RoleState.WALK;
				if(start.GridPos.Unit > end.GridPos.Unit)
					 PathList[0].dir =WalkDir.WALKLEFT;
				else PathList[0].dir =WalkDir.WALKRIGHT;
				PathList[0].deltaTime = 1.0f/speed;
				Debug.Log("出现第一个路径点为falldown 状态，请调查原因");
			}
		}
	}
	
	
	private static void AddPathRoad(ref List<PathData> PathList,MapGrid cur,WalkDir Dir ,WalkDir PreDir ,RoleState PreState,RoleState state ,RoleState NextState,int TimeCycle,GridSpace GS)
	{
		if (PathList == null || cur == null ) return;
		PathData p = null;

		if (state == RoleState.WALK) 
		{

				p = new PathData();
				p.Road = cur;
				p.dir = Dir;
				p.state = RoleState.WALK;
				p.deltaTime = 1.0f/speed;
			if (PathList.Count == 0  )
				p.gs = GS;
			else /*if (PathList[PathList.Count - 1].state == RoleState.JUMPDOWN)
				p.gs = GridSpace.Space_DOWN;
			else*/
				p.gs = PathList[PathList.Count - 1].gs;
				PathList.Add (p);
			//}
		} 
		else if (state == RoleState.JUMPUP)
		{
			p = new PathData ();
			p.Road = cur;
			p.state =state;
			p.deltaTime = 0.8f;
			p.dir = Dir;
			p.gs = GridSpace.Space_UP;
			PathList.Add (p);
		}
		else if (state == RoleState.JUMPDOWN)
		{
			p = new PathData ();
			p.Road = cur;
			p.state = state;
			p.deltaTime = 0.8f;
			p.dir = Dir;
			p.gs = GridSpace.Space_DOWN;
			PathList.Add (p);
		}
		else if (state == RoleState.REVERSEJUMP)
		{
			p = new PathData ();
			p.Road = cur;
			p.state = state;
			p.deltaTime = jumpTime;
			p.dir = Dir;
			p.gs = GridSpace.Space_UP;
			PathList.Add (p);
		}
		else if (state == RoleState.JUMP) 
		{
			p = new PathData ();
			p.Road = cur;
			p.state = RoleState.JUMP;
			p.deltaTime = jumpTime;
			p.dir = Dir;
			if (PathList.Count == 0  )
				p.gs = GS;
			else 
				p.gs = PathList[PathList.Count - 1].gs;
			PathList.Add (p);
		} 
		else if (state == RoleState.FALL) 
		{
			if (PathList.Count > 0 && PathList[PathList.Count - 1].gs == GridSpace.Space_UP)
			{
				p = new PathData ();
				p.Road = cur;
				p.state = RoleState.JUMPDOWN;
				p.deltaTime = 0.8f;
				p.dir = Dir;
				p.gs = GridSpace.Space_DOWN;
				PathList.Add (p);
			}
			p = new PathData ();
			p.Road = cur;
			p.state = state;
			p.dir = PreDir;
			p.gs = GridSpace.Space_DOWN;
			p.deltaTime = fallTime * TimeCycle;
			PathList.Add (p);
		} 
		
		else if (state == RoleState.STAIR) 
		{
			if (PathList.Count > 0 && PathList[PathList.Count - 1].gs == GridSpace.Space_UP)
			{
				p = new PathData ();
				p.Road = cur;
				p.state = RoleState.JUMPDOWN;
				p.deltaTime = jumpTime;
				p.dir = Dir;
				p.gs = GridSpace.Space_DOWN;
				PathList.Add (p);
			}
			p = new PathData ();
			p.Road = cur;
			p.state = RoleState.STAIR;
			p.deltaTime = StairDistent / speed;
			p.dir = Dir;
			p.gs = GridSpace.Space_DOWN;
			PathList.Add (p);
		} 
		else 
		{
			p = new PathData ();
			p.Road = cur;
			p.dir = Dir;
			p.state = state;
			p.gs = GridSpace.Space_DOWN;
			p.deltaTime = 1.0f/speed;
			PathList.Add (p);
		}
	}





}
