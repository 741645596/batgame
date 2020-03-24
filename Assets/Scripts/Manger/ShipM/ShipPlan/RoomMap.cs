using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// 房间地图
/// </summary>
public class RoomMap  {

	private static List<RoomGrid>  m_Room = new List<RoomGrid>();//可编辑范围的空间
	private static List <RoomGrid>  m_PutRoom = new List<RoomGrid>();//已占领的连体空间
	private static List<RoomGrid> m_DeckRoom = new List<RoomGrid>();//拥有的甲板空间
	private static List<RoomGrid> m_PutDeckRoom = new List<RoomGrid>();//已占领的甲板空间
	private static List<RoomGrid> m_GoldRoom = new List<RoomGrid>();//金库空间
	private static bool m_IsInit = false;

	public  const int MaxUnit = 8;
	protected static Int2 m_Mapv2Start=new Int2 (0, 0);//地图起始格子，只用于创建背景格子
	public static Int2 Mapv2Start 
	{
				get {
					return m_Mapv2Start;
				}
	}
	protected static Int2 m_Mapv2End=new Int2 (7,4);//地图结束格子，只用于创建背景格子
	public static Int2 Mapv2End 
	{
		get {
			return m_Mapv2End;
		}
	}
	protected static Int2 m_RealMapSize=new Int2(MaxUnit,4);//实际格子图大小
	public static Int2 RealMapSize
	{
		get {
			return m_RealMapSize;
		}
		set
		{
			m_RealMapSize = value;
		}
	}

	protected static Int2 m_RealMapBegin=new Int2 (0,0);//实际起始格子
	public static Int2 RealMapBegin 
	{
		get {
			return m_RealMapBegin;
		}
	}
	
	public static int DeckRoomTopLayer//甲板房占的最高层数
	{
		get {
			return 4;
		}
	}
	public static int BuildRoomTopLayer//连接房占的最高层数
	{
		get {
			return 3;
		}
	}

	public static bool CheckHaveMap()
	{
		return m_IsInit;
	}
	/// <summary>
	/// 创建画布矩阵
	/// </summary>
	public static void CreateCanvansArea()
	{
		ClearCanvansArea();
		for(int Layer = 0; Layer <= m_RealMapSize.Layer; Layer ++)
		{
			for(int Unit = 0; Unit < m_RealMapSize.Unit; Unit ++)
			{
				m_Room.Add(new RoomGrid(Layer, Unit));
			}
		}
	
		m_IsInit = true;
	}
	/// <summary>
	/// 清空画布矩阵
	/// </summary>
	public static void ClearCanvansArea()
	{
		m_Room.Clear();
		m_PutRoom.Clear();
		m_DeckRoom.Clear();
		m_GoldRoom.Clear();
		m_PutDeckRoom.Clear();
		m_IsInit = false;
	}
	/// <summary>
	/// 获取所有画布格子
	/// </summary>
	public static List <RoomGrid> GetAllRoomGrid()
	{
		return m_Room;
	}
	/// <summary>
	/// 获取当前船最上层
	/// </summary>
	public static int GetTopmostLayer()
	{
		int layer = 0;
		foreach(RoomGrid R in m_PutRoom)
		{
			if(R.mPosRoomGrid.Layer > layer) layer = R.mPosRoomGrid.Layer;
		}
		return layer;
	}
	/// <summary>
	/// 获取当前船最底层
	/// </summary>
	public static int GetBottommostLayer()
	{
		int layer = RoomMap.RealMapSize.Layer + 1;
		foreach(RoomGrid R in m_PutRoom)
		{
			if(R.mPosRoomGrid.Layer < layer) layer = R.mPosRoomGrid.Layer;
		}
		return layer;
	}

	
	/// <summary>
	/// 根据局部坐标获取RoomGrid
	/// </summary>
	public static RoomGrid FindRoomGrid(Vector3 posLocal )
	{
		List <RoomGrid> l = GetAllRoomGrid();
		foreach (RoomGrid r in l)
		{
			if(r.CheckInArea (posLocal ) == true)
			{
				return r;
			}
		}
		return null;
	}

	/// <summary>
	/// 定位房间格子区域
	/// </summary>
	public static RoomGrid FindRoomGrid(Int2 Pos  ,XYSYS xy)
	{
		int layer = Pos.Layer;
		int unit = Pos.Unit;
		if(xy == XYSYS.MapGrid)
			unit = unit / MapGrid.m_UnitRoomGridNum ;
		List <RoomGrid> l = GetAllRoomGrid();
		foreach (RoomGrid r in l)
		{
			if(r.mPosRoomGrid.Layer == layer && r.mPosRoomGrid.Unit == unit )
				return r;
		}
		return null;
	}
	/// <summary>
	/// 获取相邻区域的房间
	/// </summary>
	public static RoomGrid FindNeighbourRoomGrid(RoomGrid R , DIR dir)
	{
		if(R == null) return null;
		Int2 Pos = R.mPosRoomGrid;
		if(dir == DIR.LEFT)
		{
			Pos.Unit -= 1;
			return FindRoomGrid(Pos ,XYSYS.RoomGrid);
		}
		else if(dir == DIR.RIGHT)
		{
			Pos.Unit += 1;
			return FindRoomGrid(Pos ,XYSYS.RoomGrid);
		}
		else if(dir == DIR.UP)
		{
			Pos.Layer += 1;
			return FindRoomGrid(Pos ,XYSYS.RoomGrid);
		}
		else if(dir == DIR.DOWN)
		{
			Pos.Layer -= 1;
			return FindRoomGrid(Pos ,XYSYS.RoomGrid);
		}
		return null;
	}
	/// <summary>
	/// 获取小格子
	/// </summary>
	/// <param name="roomGrid"></param>
	/// <returns></returns>
	public static Int2 GetMapGrid(Int2 roomGrid)
	{
		return new Int2(roomGrid.Unit * MapGrid.m_UnitRoomGridNum, roomGrid.Layer);
	}
	
	/// <summary>
	/// 判定点击格子区域是楼梯还是兵 （楼梯是格子上部1/3） UE已经确认
	/// </summary>
	public static ShipBuildType PickupShipBuildType(RoomGrid rGrid,Vector3 v3LocalPos,int layer)
	{
		if (v3LocalPos.y >= rGrid.LocalPos.y+RoomGrid.m_heigth*0.667f)
		{
			return ShipBuildType.BuildStair;
		}
		else
		{
			return ShipBuildType.Soldier;
		}
	}
	
	/// <summary>
	/// 根据小格子获取房间的CanvasCore
	/// </summary>
	/// <param name="posMapGrid"></param>
	/// <returns></returns>
	public static CanvasCore FindCanvasCore(Int2 posMapGrid,ShipBuildType typeBuld)
	{
		RoomGrid roomGrid = FindRoomGrid(posMapGrid, XYSYS.MapGrid);
		if (roomGrid != null)
		{
			if(typeBuld==ShipBuildType.BuildRoom)
				return roomGrid.GetBuildRoom();
			List<CanvasCore> roomChildList = roomGrid.GetOtherBuild();
			for (int i = 0; i < roomChildList.Count; i++)
			{
				if(roomChildList[i].m_type==typeBuld)
					return roomChildList[i];
			}
		}
		return null;
	}
	/// <summary>
	/// 获取可放置区域
	/// </summary>
	public static List<Int2> GetCanPutArea(ShipBuildType type,bool bDeckRoom)
	{
		List<Int2> l = new List<Int2>();
		if(m_PutRoom.Count == 0)  //一个都未摆设，则所有区域都可以摆设了。
		{
			List <RoomGrid> ll = GetAllRoomGrid();
			foreach(RoomGrid g in ll)
			{
				l.Add(g.mPosRoomGrid);
			}
		}
		else  //根据现有的设置摆设区域。
		{


			if(type==ShipBuildType.Soldier)
			{
				List <RoomGrid> lAll = GetAllRoomGrid();
				foreach (RoomGrid r in lAll)
				{
					if(r.CheckPosition((int)PutPosition.Soldier))
						l.Add(r.mPosRoomGrid);
				}
				//List<RoomGrid>listRoomGrid = RoomMap.GetPutRoomGrid();
			}
			else if(type==ShipBuildType.BuildRoom)
			{
				if(bDeckRoom)
				{
					foreach (RoomGrid r in m_DeckRoom)
					{
						if(r.CheckPosition((int)PutPosition.Floor))
							l.Add(r.mPosRoomGrid);
					}

				}
				else
				{
					List<Int2> listOld = new List<Int2>();
					List<Int2> listNew = new List<Int2>();
					List<RoomGrid>listNoLinRoomGrid =  GetNoLinkRoom(listOld,listNew);
					List<RoomGrid>listCanPutRoomGrid= new List<RoomGrid>();;
					int nCount = m_PutRoom.Count;
					for(int nCnt=0;nCnt<nCount;nCnt++)
					{
						RoomGrid roolGrid = m_PutRoom[nCnt];
						if(!listNoLinRoomGrid.Contains(roolGrid)&&roolGrid.mPosRoomGrid.Layer<4)
							listCanPutRoomGrid.Add(roolGrid);
					}
					NDLink.JoinLink(new List<Int2>() , new List<Int2>() , listCanPutRoomGrid ,m_RealMapSize);
					l.AddRange(NDLink.GetCanPutArea());

				}

			}
		}
		return l;
	}
	/// <summary>
	/// 开启地图
	/// </summary>
	public static void OpenCanvans(Dictionary<Int2 ,ShapeValue > l ,int Buildid)
	{
		if(l == null || l.Count == 0)
			return ;
		foreach(Int2 Pos in l.Keys )
		{
			RoomGrid t = FindRoomGrid( Pos ,XYSYS.RoomGrid);
			if(t != null)
			{
				t.SetRoomGrid(l[Pos],Buildid);
				AddPutRoomGrid(t);
			}
		}
	}
	/// <summary>
	/// 关闭地图
	/// </summary>
	public static void CloseCanvans(List<Int2> l)
	{
		if(l == null || l.Count == 0)
			return ;
		foreach(Int2 pos in l )
		{
			RoomGrid t = FindRoomGrid(pos ,XYSYS.RoomGrid);
			if(t != null)
			{
				t.EmptyRoomGrid();
				RemovePutRoomGrid(t);
			}
		}
	}

	/// <summary>
	/// 移动房间
	/// </summary>
	public static void MoveCanvans(List<RoomGrid> OldList,List<RoomGrid> NewList)
	{
		if(OldList == null || NewList == null ) 
			return ;
		if(OldList.Count != NewList.Count)
			return ;

		List<RoomGrid>Rdata = new List<RoomGrid>();
		//先备份数据
		for(int i = 0 ; i < OldList.Count ; i++)
		{
			RoomGrid R = new RoomGrid();
			R.CopyPutData(OldList[i]);
			OldList[i].EmptyRoomGrid();
			Rdata.Add(R);
		}
		//继承数据
		for(int i = 0 ; i < NewList.Count ; i++)
		{
			NewList[i].InheritanceRoomGrid(Rdata[i]);
		}
		//先删除
		for(int i = 0 ; i < OldList.Count ; i++)
		{
			RemovePutRoomGrid(OldList[i]);
		}
		//后添加
		for(int i = 0 ; i < NewList.Count ; i++)
		{
			AddPutRoomGrid(NewList[i]);
		}
	}


	/// <summary>
	/// 房间置换数据
	/// </summary>
	public static void ExchangeCanvans(List<RoomGrid> l1,List<RoomGrid> l1MoveTo,List<RoomGrid> l2,List<RoomGrid> l2MoveTo)
	{
		if(l1 == null || l2 == null ) 
			return ;
		//if(l1.Count != l2.Count)
		//	return ;
		List<RoomGrid> lTemp1 = new List<RoomGrid>();
		//交换数据
		int nRoomGrid1Count = l1.Count;
		for(int nRoomGrid1Cnt = 0 ; nRoomGrid1Cnt < nRoomGrid1Count ; nRoomGrid1Cnt++)
		{
			RoomGrid R = l1[nRoomGrid1Cnt];
			if(l2MoveTo.Contains(R))
			{
				RoomGrid rTemp = l1[nRoomGrid1Cnt];
			    R = new RoomGrid();
				R.CopyPutData(rTemp);
				rTemp.EmptyRoomGrid();
				RemovePutRoomGrid(rTemp);
			}
			lTemp1.Add(R);
		}
		
		List<RoomGrid> lTemp2 = new List<RoomGrid>();
		nRoomGrid1Count = l2.Count;
		for(int nRoomGrid1Cnt = 0 ; nRoomGrid1Cnt < nRoomGrid1Count ; nRoomGrid1Cnt++)
		{
			RoomGrid R = l2[nRoomGrid1Cnt];
			if(l1MoveTo.Contains(R))
			{
				RoomGrid rTemp = l2[nRoomGrid1Cnt];
				R = new RoomGrid();
				R.CopyPutData(rTemp);
				rTemp.EmptyRoomGrid();
				RemovePutRoomGrid(rTemp);
			}
			lTemp2.Add(R);
		}
		
		MoveCanvans(lTemp1,l1MoveTo);
		MoveCanvans(lTemp2,l2MoveTo);
		lTemp1.Clear ();
		lTemp2.Clear ();
	}


	public static void AddPutRoomGrid(RoomGrid r)
	{
		if(m_PutRoom.Contains (r) == false)
		{
			m_PutRoom.Add(r);
		}
	}

	public static void RemovePutRoomGrid(RoomGrid R)
	{
		if(m_PutRoom.Contains(R))
		{
			m_PutRoom.Remove(R);
		}
	}
	
	/// <summary>
	/// 获取每列最顶层连接房
	/// </summary>
	public static List<RoomGrid> GetTopmostLayerPutRoom()
	{
		List<RoomGrid> l = new List<RoomGrid>();
		Dictionary<int ,int> ll = new Dictionary<int, int>();
		foreach(RoomGrid m in m_PutRoom)
		{
			Int2 Pos = m.mPosRoomGrid;
			if(ll.ContainsKey(Pos.Unit) == false)
			{
				ll.Add(Pos.Unit,Pos.Layer);
			}
			else
			{
				if(Pos.Layer > ll[Pos.Unit])
				{
					ll[Pos.Unit] = Pos.Layer;
				}
			}
		}
		foreach(int unit in ll.Keys)
		{
			RoomGrid m = FindRoomGrid(new Int2(unit,ll[unit]) ,XYSYS.RoomGrid);
			if(m != null) l.Add(m);
		}
		return l ;
	}
	/// <summary>
	/// 更新甲板数据
	/// </summary>
	public static void UpdateDeckRoomGrid()
	{
		
		List<RoomGrid> deckList = new List<RoomGrid>();
		foreach(RoomGrid m in m_PutRoom)
		{
			RoomGrid Deck =FindNeighbourRoomGrid(m,DIR.UP);
			if(Deck!=null&& m_PutRoom.Contains(Deck) == false)
			{
				Deck.SetDeckRoomValue(m.upMap);
				deckList.Add(Deck);
			}
		}
		/*
		List<RoomGrid> deckList = new List<RoomGrid>();
		//List<RoomGrid> l =  GetTopmostLayerPutRoom();
		foreach(RoomGrid R in l)
		{
			RoomGrid Deck =FindNeighbourRoomGrid(R,DIR.UP);
			if(Deck != null)
			{
				Deck.SetDeckRoomValue(R.upMap);
				deckList.Add(Deck);
			}
		}
		*/
		//获取新增的甲板
		for(int i = 0 ; i < m_DeckRoom.Count;  )
		{
			RoomGrid R = m_DeckRoom[i];
			if(deckList.Contains (R) == true || m_PutRoom.Contains(R) == true)
			{
				m_DeckRoom.Remove(R);
			}
			else 
			{
				/*Int2 nMapDownGrid = new Int2(R.mPosRoomGrid.Unit,R.mPosRoomGrid.Layer-1);
				RoomGrid rDown = RoomMap.FindRoomGrid(nMapDownGrid,XYSYS.MapGrid);
				if(rDown!=null&& m_PutRoom.Contains(rDown) == true)
				{
					m_DeckRoom.Remove(R);
					deckList.Add(R);
				}
				else*/
					i ++;
			}
		}

		//
		//清空新增的甲板数据
		foreach(RoomGrid R in m_DeckRoom)
		{
			if(m_PutDeckRoom.Contains(R) == false)
			{
				R.SetDeckRoomValue(0);
			}

		}

		m_DeckRoom.Clear();
		m_DeckRoom.AddRange(deckList);
	}

	/// <summary>
	/// 更新金库数据
	/// </summary>
	public static void UpdateGoldRoomGrid(List<Int2> GoldPosList)
	{
		m_GoldRoom.Clear();
		foreach(Int2 P in GoldPosList)
		{
			RoomGrid r = FindRoomGrid(P ,XYSYS.RoomGrid);
			if(r != null)
				m_GoldRoom.Add(r);
		}
	}

	/// <summary>
	/// 获取甲板房间
	/// </summary>
	public static List<RoomGrid> GetDeckRoomGrid()
	{
		return m_DeckRoom;
	}

	/// <summary>
	/// 获取甲板房间空间
	/// </summary>
	public static List<Int2> GetDeckRoomGridXY()
	{
		List<Int2> l = new List<Int2>();
		foreach(RoomGrid R  in m_DeckRoom)
		{
			l.Add(R.mPosRoomGrid);
		}
		return l;
	}
	/// <summary>
	/// 获取指定层连体房占领单元
	/// </summary>
	public static List<RoomGrid> GetPutRoom(int layer)
	{
		List<RoomGrid> l = new List<RoomGrid>();
		foreach(RoomGrid m in m_PutRoom)
		{
			if(m.mPosRoomGrid.Layer == layer)
			{
				l.Add(m);
			}
		}
		return l ;
	}
	/// <summary>
	/// 获取指定层连体房占领情况说明
	/// </summary>
	/// <returns>list 值为1 标识摆设了</returns>
	private static List<int> GetColumnPutRoom(int layer)
	{
		List<int> l = new List<int>();
		if (layer > BuildRoomTopLayer)
				return l;
		Int2 Area = m_RealMapSize;
		for(int i = 0; i < Area.Unit ; i++)
		{
			l.Add(0);
		}
		foreach(RoomGrid m in m_PutRoom)
		{
			Int2 Pos = m.mPosRoomGrid;
			if(Pos.Layer == layer && Pos.Unit < Area.Unit)
			{
				l[Pos.Unit] = 1;
			}
		}
		return l ;
	}


	/// <summary>
	/// 获取楼梯活动区域
	/// </summary>
	/// <param name="layer">格子坐标</param>
	/// <param name="unit">格子坐标</param>
	/// <returns>返回的为地图格子坐标范围Int2.Layer为区间开始 Int2.Unit区间结束</returns>
	public static Int2  GetStairActiveArea(int layer,int unit)
	{
		Int2 Area = Int2.zero;
		int u = unit / MapGrid.m_UnitRoomGridNum;
		//获取一层区组
		List<int> l = new List<int>();
		l = GetColumnPutRoom(layer);
		//对一层区组进行连续性分割
		List<Int2> lArea = NDLink.GetContinuousRange(l);
		
		foreach(Int2 Pos in lArea)
		{
			//Int2.Layer为区间开始 Int2.Unit区间结束
			if(Pos.Layer <= u && u <= Pos.Unit)
			{
				Area.Layer = Pos.Layer * MapGrid.m_UnitRoomGridNum ; 
				Area.Unit =  (Pos.Unit + 1) * MapGrid.m_UnitRoomGridNum ;
				break;
			}
		}
		return Area;
	}
	/// <summary>
	/// 获取改层需要创造的楼梯位置点。
	/// </summary>
	private static List<Int2> GetStairCreatePoint(int layer)
	{
		List<Int2> lTarget = new List<Int2>();
		//获取一层区组
		List<int> l = new List<int>();
		l = GetColumnPutRoom(layer);
		//对一层区组进行连续性分割
		List<Int2> lArea = NDLink.GetContinuousRange(l);
		//
		foreach(Int2 P in lArea)
		{
			Int2 StairPos = Int2.zero;
			if(CheckStairPoint(layer,P.Layer,P.Unit, ref StairPos) == true)
			{
				lTarget.Add(StairPos);
			}
		}
		return lTarget;
	}
	/// <summary>
	/// 确认该区间是否需要楼梯，需要则计算楼梯的位置
	/// </summary>
	public static bool CheckStairPoint(int Layer,int Start,int End ,ref Int2 StairPoint)
	{
		StairPoint = Int2.zero;
		for(int unit = Start ; unit <= End ; unit++)
		{
			RoomGrid R = FindRoomGrid(new Int2(unit,Layer) ,XYSYS.RoomGrid);
			if(R != null && R.HaveStair ()) return false;
		}
		//计算需要楼梯的位置
		StairPoint = GetBestStairPoint(Layer,Start,End);
		return true;
	}
	/// <summary>
	/// 获取最佳楼梯位置
	/// </summary>
	private static Int2 GetBestStairPoint(int Layer,int Start,int End )
	{
		Int2 BestPoint = new Int2(Start * MapGrid.m_UnitRoomGridNum ,Layer);
		int distance = 0;
		for(int unit = Start ; unit <= End ; unit++)
		{
			RoomGrid R = FindRoomGrid(new Int2(unit,Layer) ,XYSYS.RoomGrid);
			if(R != null)
			{
				int d = DistanceGold(R);
				if(d > distance)
				{
					distance = d;
					BestPoint = new Int2(unit * MapGrid.m_UnitRoomGridNum ,Layer);
				}
			}
		}
		return BestPoint;
	}
	/// <summary>
	/// 获取需要创建楼梯的点
	/// </summary>
	public static List<Int2> GetStairCreatePoint()
	{
		List<Int2> lTarget = new List<Int2>();
		int minLayer = GetBottommostLayer();
		int maxLayer = GetTopmostLayer();
		for(int layer = minLayer; layer <= maxLayer ; layer++)
		{
			lTarget.AddRange(GetStairCreatePoint(layer));
		}
		return lTarget;
	}

    /// <summary>
    /// 检测指定的RoomGrid 是否是甲板Room
    /// </summary>
    public static bool IsDeckRoom(RoomGrid roomGrid)
    {
        if (roomGrid != null)
        {
            return m_DeckRoom.Contains(roomGrid);
        }
        return false;
    }
    /// <summary>
    /// 检测指定的RoomGrid 是否是连接房
    /// </summary>
    public static bool IsPutRoom(RoomGrid roomGrid)
    {
        if (roomGrid != null)
        {
            return m_PutRoom.Contains(roomGrid);
        }
        return false;
    }

    static bool IsIncludeSoldier(RoomGrid roomGrid)
    {
        List<CanvasCore> childCores = roomGrid.GetOtherBuild();
        if (childCores.Count == 0)
        {
            return false;
        }
        for (int i = 0; i < childCores.Count; i++)
        {
            if (childCores[i].m_type == ShipBuildType.Soldier)
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsPutDeckRoom(RoomGrid roomGrid)
    {
        if (roomGrid != null)
        {
            return m_PutDeckRoom.Contains(roomGrid);
        }
        return false;
    }
    

	/// <summary>
	/// 距离金库的距离
	/// </summary>
	private static int DistanceGold(RoomGrid R)
	{
		int distance = 10000;
		foreach(RoomGrid r in m_GoldRoom)
		{
			int d = Mathf.Abs(R.mPosRoomGrid.Layer - r.mPosRoomGrid.Layer) + Mathf.Abs(R.mPosRoomGrid.Unit - r.mPosRoomGrid.Unit);
			if(d < distance)
				distance = d;
		}
		return distance;
	}

	//只用于炮弹兵和楼梯		
	public static void RemoveMapPosition(CanvasCore Core ,Dictionary<RoomGrid ,int> l)
	{
		foreach(RoomGrid Grid in l.Keys)
		{
			if(Grid != null) Grid.RemovePosition(Core,l[Grid]);
		}
	}
	//只用于炮弹兵和楼梯
	public static void AddMapPosition(CanvasCore Core ,Dictionary<RoomGrid ,int> l)
	{
		foreach(RoomGrid Grid in l.Keys)
		{
			if(Grid != null) Grid.AddPosition(Core,l[Grid]);
		}
	}
	//只用于炮弹兵和楼梯
	public static bool CheckMapPosition(Dictionary<RoomGrid ,int> l)
	{
		foreach(RoomGrid Grid in l.Keys)
		{
			if(Grid != null 
			   && Grid.CheckPosition(l[Grid]) == false)
				return false;
		}
		return true;
	}

	/// <summary>
	/// 获取局部坐标
	/// </summary>
	public static Vector3 GetRoomGridLocalPos(Int2 posMapGrid)
	{
		RoomGrid r = RoomMap.FindRoomGrid(posMapGrid ,XYSYS.MapGrid);
		if(r != null)
		{
			Vector3 v = r.LocalPos;
			v.x += (posMapGrid.Unit % MapGrid.m_UnitRoomGridNum) * MapGrid.m_width;
			return v;
		}
		else
		{
			return Vector3.zero;
		}
	}

	/// <summary>
	/// 获取具体对象的局部坐标
	/// </summary>
	public static Vector3 GetShipBuildLocalPos(Int2 posMapGrid ,ShipBuildType Type)
	{
		RoomGrid r = RoomMap.FindRoomGrid(posMapGrid ,XYSYS.MapGrid);
		if(r != null)
		{
			Vector3 v = r.LocalPos;
			if(Type == ShipBuildType.Soldier)
			{
				v.x += 3 * MapGrid.m_width;
			}
			return v;
		}
		else
		{
			return Vector3.zero;
		}
	}

	/// <summary>
	/// 获取未连接的连接房
	/// </summary>
	public static List<RoomGrid> GetNoLinkRoom(List<Int2> lOld ,List<Int2> lNew)
	{
		List<Int2> lGold = new List<Int2>();
		int nRoomCount = m_GoldRoom.Count;
		int nRoomCnt = 0;
		for(nRoomCnt=0;nRoomCnt<nRoomCount;nRoomCnt++)
		{
			RoomGrid rRoomGrid = m_GoldRoom[nRoomCnt];
			if(rRoomGrid != null)
			{
				lGold.Add(rRoomGrid.mPosRoomGrid);
			}
		}
		
		List<Int2> lGrid = new List<Int2>();
		List<RoomGrid> lNoLink = new List<RoomGrid>();
		lGrid = NDLink.GetNoLinkGrid(lOld , lNew,lGold ,m_PutRoom ,m_RealMapSize);
		foreach(Int2 Pos in lGrid )
		{
			RoomGrid R = FindRoomGrid(Pos,XYSYS.RoomGrid);
			if(R != null)
				lNoLink.Add(R);
		}
		return lNoLink;
	}
	/// <summary>
	/// 是否可以正常连通到金库
	/// </summary>
	public static bool CheckLinkToGoldRoom(List<Int2> RemoveList ,List<Int2> AddList ,bool IsNewCreate)
	{
		if(IsNewCreate)
		{
			if(!NDLink.SameGridList(RemoveList,AddList))
			{
				//确认是否产生无效的甲板房间
				if(CheckHaveInvalidRoomGrid (RemoveList ,AddList) == true)
					return false;
				
				//确认能否吸收新房间的
				if(CheckCanPutRoom (RemoveList ,AddList) == false)
				{
					return false;
				}
			}
		}
		//确认是否覆盖了甲板房间或在甲板房间上面
		if(ChencUpCoverDeckRoom (AddList) == true)
			return false;
		//联通性测试
		NDLink.JoinLink(RemoveList , AddList , m_PutRoom ,m_RealMapSize);

		bool bLinkOK = true;
		List<Int2> listOld = new List<Int2>();
		List<Int2> listNew = new List<Int2>();
		List<RoomGrid>listNoLinRoomGrid =  GetNoLinkRoom(listOld,listNew);
		int nNewPosCount = AddList.Count;
		int nNewPosCnt = 0;
		for(nNewPosCnt=0;nNewPosCnt<nNewPosCount;nNewPosCnt++)
		{
			RoomGrid R = FindRoomGrid(AddList[nNewPosCnt],XYSYS.RoomGrid);
			if(listNoLinRoomGrid.Contains(R))
			{
				bLinkOK=false;
				break;
			}
		}
		return bLinkOK;
	}

	/// <summary>
	/// 判断能否吸收连接房间
	/// </summary>
	private static bool CheckCanPutRoom(List<Int2> OldList ,List<Int2> NewList)
	{
		if(OldList == null || NewList == null)
			return false;
		if(OldList.Count != NewList.Count)
			return false;

		for(int i = 0 ; i< OldList.Count ; i++)
		{
			RoomGrid oldRoom = FindRoomGrid(OldList[i] ,XYSYS.RoomGrid);
			RoomGrid NRoom = FindRoomGrid(NewList[i] ,XYSYS.RoomGrid);
			if(oldRoom == null || NRoom == null)
				return false;
			if(m_PutRoom.Contains (NRoom) == true)
			{
				if(!OldList.Contains(NewList[i]))
					return false;
				else
					continue;
			}

			if(oldRoom.CheckRoomGridPosition(NRoom.GetRoomPosition()) == false)
				return false;
		}
		return true;
	}

	/// <summary>
	/// 判断是否有连接房覆盖或在甲板房间上面
	/// </summary>
	private static bool ChencUpCoverDeckRoom(List<Int2> lPos)
	{
		if(lPos == null || lPos.Count == 0)
			return false;
		foreach(Int2 Pos in lPos)
		{

			foreach (RoomGrid R in m_PutDeckRoom)
			{
				if(R.mPosRoomGrid.Unit == Pos.Unit && Pos.Layer >= R.mPosRoomGrid.Layer)
				{
					return true;
				}
			}
		}
		return false;
	}


	/// <summary>
	/// 删除多余的楼梯
	/// </summary>
	public static void RemoveExcessStair()
	{
		int MinLayer = GetBottommostLayer();
		int MaxLayer = GetTopmostLayer();
		for(int Layer = MinLayer; Layer <= MaxLayer; Layer++)
		{
			DeleteStairInLayer(Layer);
		}
	}

	/// <summary>
	/// 删除一层的楼梯
	/// </summary>
	private static void DeleteStairInLayer(int layer)
	{
		//获取一层区组
		List<int> l = new List<int>();
		l = GetColumnPutRoom(layer);
		//对一层区组进行连续性分割
		List<Int2> lArea = NDLink.GetContinuousRange(l);
		//
		foreach(Int2 P in lArea)
		{
			DeleteStairAreaStair(layer ,P.Layer,P.Unit);
		}
	}
	/// <summary>
	/// 删除一个楼梯区间的楼梯
	/// </summary>
	private static void DeleteStairAreaStair(int Layer,int Start,int End)
	{
		bool haveStair = false;
		int distance = 0;
		RoomGrid deleteStairRoomGrid = null;
		for(int unit = Start ; unit <= End ; unit ++)
		{
			RoomGrid R = FindRoomGrid(new Int2(unit ,Layer) ,XYSYS.RoomGrid);
			if(R != null && R.HaveStair ())
			{
				if(haveStair == false)
				{
					distance = DistanceGold (R);
					haveStair = true;
					deleteStairRoomGrid = R;
				}
				else if(deleteStairRoomGrid != null)
				{
					int d = DistanceGold (R);
					//删除距离近的楼梯
					if(d <=  distance)
					{
						R.RemovStair();
					}
					else
					{
						deleteStairRoomGrid.RemovStair();
						deleteStairRoomGrid = R;
						distance = d;
					}
				}
			}
		}
	}
    /// <summary>
    /// 检测格子是否是房间或者甲板
    /// </summary>
    public static bool CheckBuildOrDeckRoom(Vector3 MousePos)
    {
        Vector3 pos = U3DUtil.SetZ(MousePos, -Camera.main.transform.position.z);
        Vector3 m_v3TouchWorldPos = Camera.main.ScreenToWorldPoint(pos);
        Vector3 gridPos = BattleEnvironmentM.World2LocalPos(m_v3TouchWorldPos);
        RoomGrid roomGrid = RoomMap.FindRoomGrid(gridPos);
        if (roomGrid == null)
        {
            return false;
        }
        Int2 grid = roomGrid.mPosRoomGrid;
        return CheckBuildOrDeckRoom(grid);
    }

    /// <summary>
    /// 检测格子是否是房间或者甲板
    /// </summary>
    public static bool CheckBuildOrDeckRoom(Int2 Pos)
    {
        foreach (RoomGrid R in m_DeckRoom)
        {
            if (R.mPosRoomGrid == Pos )
            {
                return true;
            }
        }
        foreach (RoomGrid R in m_PutRoom)
        {
            if (R.mPosRoomGrid == Pos )
            {
                return true;
            }
        }
        foreach (RoomGrid R in m_GoldRoom)
        {
            if (R.mPosRoomGrid == Pos )
            {
                return true;
            }
        }

        return false;
    }

	/// <summary>
	/// 判断是否为甲板房间
	/// </summary>
	public static bool CheckDeckRoom(RoomGrid R)
	{
		if(R == null) return false;
		if(m_DeckRoom.Contains (R) == true)
			return true;
		else return false;
	}

    public static bool CheckRoomGridInBoat(RoomGrid roomGrid)
    {
        if (RoomMap.IsDeckRoom(roomGrid) || RoomMap.IsPutRoom(roomGrid))
        {
            return true;
        }
        return false;
    }

	/// <summary>
	/// 判断是否为带建筑的甲板或甲板房间
	/// </summary>
	private static bool CheckHaveBuildDeckRoom(Int2 Pos)
	{
		Pos.Layer += 1;
		foreach (RoomGrid R in m_DeckRoom)
		{
			if(R.mPosRoomGrid == Pos && (R.GetOtherBuild().Count > 0 || R.buildingid != RoomGrid.EMPTYGRIDID))
			{
				return true;
			}
		}

		foreach (RoomGrid R in m_PutDeckRoom)
		{
			if(R.mPosRoomGrid == Pos )
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// 判断是否会产生未开启的房间，有建筑
	/// </summary>
	private static bool CheckHaveInvalidRoomGrid(List<Int2> OldList ,List<Int2> NewList)
	{
		if(OldList == null || NewList == null)
			return false;
		List<Int2> l = new List<Int2>();
		l.AddRange(OldList);
		foreach(Int2 P in NewList)
		{
			l.Remove(P);
		}

		foreach(Int2 P in l)
		{
			if(CheckHaveBuildDeckRoom(P)== true)
				return true;
		}
		return false;
	}

	/*
	/// <summary>
	/// 获取传送点
	/// </summary>
	private static bool GetTransPoint(Vector3 LocalPos,ref Int2 TransPointPoint)
	{
		TransPointPoint = Int2.zero;
		//已摆设房间
		foreach (RoomGrid r in m_PutRoom)
		{
			if(r.CheckInArea (LocalPos ) == true)
			{
				TransPointPoint = r.SoldierPos;
				return true;
			}
		}
		//甲板房间
		foreach (RoomGrid r in m_DeckRoom)
		{
			if(r.CheckInArea (LocalPos ) == true)
			{
				TransPointPoint = r.SoldierPos;
				return true;
			}
		}
		return false;
	}
	*/
	/// <summary>
	/// 检查甲板房间,能放放置
	/// </summary>
	public static bool CheckCanPutDeckBuild(int buildingid, List<Int2> lPos)
	{
		//判断放置在甲板上面
		foreach(Int2 Pos in lPos)
		{
			RoomGrid R = FindRoomGrid(Pos ,XYSYS.RoomGrid);
			if(R == null) return false;
			if(CheckDeckRoom(R) == false)
				return false;
		}
		//判断是否被占领
		foreach(Int2 Pos in lPos)
		{
			RoomGrid R = FindRoomGrid(Pos ,XYSYS.RoomGrid);
			if(R == null) return false;
			if(m_PutDeckRoom.Contains(R) == true)
			{
				if(R.buildingid != buildingid)
					return false;
			}
		}
		return true;
	}


	/// <summary>
	/// 添加甲板房间
	/// </summary>
	public static void AddDeckBuild(int buildingid, List<Int2> lPos)
	{
		foreach(Int2 Pos in lPos)
		{
			RoomGrid R = FindRoomGrid(Pos ,XYSYS.RoomGrid);
			if(R != null) 
			{
				R.buildingid = buildingid ;
				if(m_PutDeckRoom.Contains(R) == false)
				{
					m_PutDeckRoom.Add(R);
				}
				/*
				if(m_DeckRoom.Contains (R ) ==  false)
				{
					m_DeckRoom.Add(R);
				}*/
			}

		}
	}

	/// <summary>
	/// 移除甲板房间
	/// </summary>
	public static void RemoveDeckBuild(List<Int2> lPos)
	{
		foreach(Int2 Pos in lPos)
		{
			RoomGrid R = FindRoomGrid(Pos ,XYSYS.RoomGrid);
			if(R != null) 
			{
				if(m_PutDeckRoom.Contains(R) == true)
				{
					R.buildingid = RoomGrid.EMPTYGRIDID ;
					m_PutDeckRoom.Remove(R);
				}
			}
			
		}
	}
	/// <summary>
	/// 获取能放置房间的空闲位置
	/// </summary>
	public static List<RoomGrid> GetCanPutBuildIdleRoom()
	{
		List<RoomGrid> l = new List<RoomGrid>();
		List <RoomGrid> ll = GetAllRoomGrid();
		foreach(RoomGrid g in ll)
		{
			if(m_PutRoom.Contains(g) == false && m_PutDeckRoom.Contains(g) == false)
			{
				l.Add(g);
			}
		}
		return l;
	}

	/// <summary>
	/// 获取能放置房间的空闲位置
	/// </summary>
	public static List<RoomGrid> GetCanPutSoldierIdleRoom()
	{
		//List<Int2> l = RoomMap.GetCanPutArea(ShipBuildType.Soldier,false);
		List<RoomGrid> l = new List<RoomGrid>();
		l.AddRange(m_PutRoom);
		l.AddRange(m_DeckRoom);
		return l;
	}
	
	/// <summary>
	/// 获取未连接的房间
	/// </summary>
	public static List<RoomGrid> GetUnLinkDeckRoom()
	{
		List<RoomGrid> lNoLink = new List<RoomGrid>();
		List<Int2> lGold = new List<Int2>();
		int nRoomCount = m_PutDeckRoom.Count;
		int nRoomCnt = 0;
		for(nRoomCnt=0;nRoomCnt<nRoomCount;nRoomCnt++)
		{
			RoomGrid rRoomGrid = m_PutDeckRoom[nRoomCnt];
			if(rRoomGrid != null&&!CheckDeckRoom(rRoomGrid))
			{
				lNoLink.Add(rRoomGrid);
			}
		}
		return lNoLink;
	}
	/// <summary>
	/// 获取未连接的兵
	/// </summary>
	public static List<CanvasCore> GetUnLinkSoldiers()
	{
		List<CanvasCore> lNoLinkCore = new List<CanvasCore>();
		List<Int2> lGold = new List<Int2>();
		int nRoomCount = m_Room.Count;
		int nRoomCnt = 0;
		for(nRoomCnt=0;nRoomCnt<nRoomCount;nRoomCnt++)
		{
			RoomGrid rRoomGrid = m_Room[nRoomCnt];
			if(rRoomGrid != null&&!CheckDeckRoom(rRoomGrid))
			{
				CanvasCore cavasCore = rRoomGrid.GetBuildRoom();
				if(cavasCore!=null&&cavasCore.m_ID == RoomGrid.EMPTYGRIDID)
				{
					List<CanvasCore> listOther = rRoomGrid.GetOtherBuild();
					int nCanvasCoreCount = listOther.Count;
					int nCanvasCoreCnt = 0;
					for(nCanvasCoreCnt=0;nCanvasCoreCnt<nCanvasCoreCount;nCanvasCoreCnt++)
					{
						CanvasCore coreOther = listOther[nCanvasCoreCnt];
						if(coreOther!=null&&coreOther.m_type == ShipBuildType.Soldier)
						{
							lNoLinkCore.Add(coreOther);
						}
					}
				}
			}
		}
		return lNoLinkCore;
	}
	/// <summary>
	/// 获取已经占领的房间
	/// </summary>
	public static List<RoomGrid> GetPutRoomGrid(CanvasCore core)
	{
		if (core.Data != null) 
		{
			return RoomMap.GetMovetoRoomGrid(core,new Int2 (core.Data.cxMapGrid,core.Data.cyMapGrid));
		}
		/// <summary>
		/// 获取占领的房间
		/// </summary>
		return new List<RoomGrid>();
	}
	/// <summary>
	/// 获取移动到目标位置所占领的房间
	/// </summary>
	/// <param name="TargetPos">目的位置</param>
	public static List<RoomGrid> GetMovetoRoomGrid(CanvasCore core,Int2 TargetPos)
	{
		if (core.Data != null) 
		{
			List<RoomGrid> l = new List<RoomGrid>();
			ShapeType shape = core.GetPutRoomShape();
			if (shape != null)
			{
				Dictionary<Int2, ShapeValue> lshape = shape.GetShapeData(TargetPos);
				foreach (Int2 Pos in lshape.Keys)
				{
					RoomGrid r = RoomMap.FindRoomGrid(Pos, XYSYS.RoomGrid);
					if (r != null) l.Add(r);
				}
			}
			else
			{
				RoomGrid r = RoomMap.FindRoomGrid(TargetPos, XYSYS.MapGrid);
				if (r != null) l.Add(r);
				
			}
			return l;
		}
		return new List<RoomGrid>();
	}
	
	/// <summary>
	/// 获取占领的房间位置
	/// </summary>
	public static Dictionary<RoomGrid ,int> GetPutRoomGridPosition(CanvasCore core)
	{
		Dictionary<RoomGrid ,int> l = new Dictionary<RoomGrid ,int>();
		if(core.Data != null && core.m_type != ShipBuildType.BuildRoom)
		{
			return RoomMap.GetMovetoRoomGridPosition(core,new Int2 (core.Data.cxMapGrid,core.Data.cyMapGrid));
		}
		return l;
	}
	/// <summary>
	/// 获取移动到目标位置所占领的房间位置
	/// </summary>
	/// <param name="posMapGrid">目的位置</param>
	public static Dictionary<RoomGrid ,int> GetMovetoRoomGridPosition(CanvasCore core,Int2 posMapGrid)
	{   
		Dictionary<RoomGrid ,int> l = new Dictionary<RoomGrid ,int>();
		if (core.Data == null)
			return l;
		ShapeType shape = core.GetPutRoomShape();
		if(core.Data.type == (int)ShipBuildType.Soldier)
		{
			RoomGrid  r = RoomMap.FindRoomGrid(posMapGrid ,XYSYS.MapGrid);
			if(r != null)
			{
				l.Add(r , (int)PutPosition.Soldier);
			}
		}
		else if (core.Data.type == (int)ShipBuildType.BuildStair)
		{
			RoomGrid r = RoomMap.FindRoomGrid(posMapGrid, XYSYS.MapGrid);
			if (r != null)
			{
				l.Add(r, (int)PutPosition.Stair);
			}
		}
		else if(shape != null) 
		{
			Dictionary<Int2 ,ShapeValue > lshape = shape.GetShapeData(posMapGrid);
			foreach(Int2 Pos in lshape.Keys)
			{
				int position = lshape[Pos].Position;
				RoomGrid  r = RoomMap.FindRoomGrid(Pos ,XYSYS.RoomGrid);
				if(r != null)
				{
					l.Add(r , position);
				}
			}
		}
		return l;
	}
	/// <summary>
	/// 判断移动到目标位置，是否有出界
	/// </summary>
	/// <param name="posMapGrid">目的位置</param>
	public static bool  CheckAllInMap(CanvasCore core,Int2 posMapGrid)
	{
		if (core.Data != null) 
		{
			ShapeType shape = core.GetPutRoomShape();
			if (shape != null)
			{
				Dictionary<Int2, ShapeValue> lshape = shape.GetShapeData(posMapGrid);
				foreach (Int2 posRoomGrid in lshape.Keys)
				{
					RoomGrid r = RoomMap.FindRoomGrid(posRoomGrid, XYSYS.RoomGrid);
					if (r == null)
						return false;
				}
			}
		}
		return true;
	}
}
