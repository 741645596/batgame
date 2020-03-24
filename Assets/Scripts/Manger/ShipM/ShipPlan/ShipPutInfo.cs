using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// 船只放置信息
/// </summary>
public class ShipPutInfo
{
	public int id;
	public int battlemode ;
	public int objid;//根据type决定是谁的ID
	public int type ; //type=1，房间，2=炮弹兵，3=楼梯
	public int cxMapGrid; 
	public int cyMapGrid ;
	public int shipput_data0 ;
	public int shipput_data1 ;

	public int LinkRoomID = -1 ;
	public int OffX ;//偏移值 小格子
	public int OffY ;
	public bool m_DeckRoom = false;   //是否为甲板房间（只能放最上层）
	private static int NewID = 0;


	public ShipPutInfo(){}



	public void Copy(ShipPutInfo Info)
	{
		this.id = Info.id;
		this.battlemode = Info.battlemode;
		this.objid = Info.objid;
		this.type = Info.type;

		this.cxMapGrid = Info.cxMapGrid;
		this.cyMapGrid = Info.cyMapGrid;
		this.shipput_data0 = Info.shipput_data0;
		this.shipput_data1 = Info.shipput_data1;

		this.LinkRoomID = Info.LinkRoomID;
		this.OffX = Info.OffX;
		this.OffY = Info.OffY;
		this.m_DeckRoom = Info.m_DeckRoom;
	}

    public static bool IsEqual(ShipPutInfo a, ShipPutInfo b)
    {
        if (a == null || b == null)
        {
            return false;
        }
        bool isEqual = false;
        if (a.id == b.id && a.cxMapGrid == b.cxMapGrid && a.cyMapGrid == b.cyMapGrid)
        {
            isEqual = true;
        }
        return isEqual;
    }

	/// <summary>
	/// 构造函数，从建筑物仓库中构造
	/// </summary>
	public ShipPutInfo( int ID,BuildInfo Info)
	{
		if(Info != null)
		{
			id = ID;
			battlemode = 1;
			objid = Info.ID;
			type = (int)ShipBuildType.BuildRoom;
			shipput_data0 = Info.m_data0;
			shipput_data1 = Info.m_data1;
			//设置受否为甲板房间
			if(Info.m_RoomType == RoomType.DeckTrap)
				m_DeckRoom = true;
			else m_DeckRoom = false;
			cxMapGrid = 0;
			cyMapGrid = 0;
		}
	}

	/// <summary>
	/// 从炮弹兵进行构造
	/// </summary>
	public ShipPutInfo(int ID ,int SoldierID)
	{
		id = ID;
		battlemode = 1;
		objid = SoldierID;
		type = (int)ShipBuildType.Soldier;
		shipput_data0 = 0;
		shipput_data1 = 0;
		cxMapGrid = 0;
		cyMapGrid = 0;
	}

	public ShapeType GetPutRoomShape()
	{
		if ((int) ShipBuildType.BuildRoom != type) 
			return null;
		BuildInfo  roomBuild = GetBuildInfo();
		if(roomBuild != null)
		{
			return roomBuild.m_Shape;
		}
		return null;
	}
	
	/// <summary>
	/// 设置摆设位置
	/// </summary>
	public void SetBuildPostion(Int2 TargetPos)
	{
		cxMapGrid = TargetPos.Unit;
		cyMapGrid = TargetPos.Layer;
	}


	/// <summary>
	/// 设置参数，第一个参数为关联的建筑Core
	/// </summary>
	public void SetBuildPara(CanvasCore Core,int data0 ,int data1)
	{
		ShipPlan P = ShipPlanDC.GetCurShipPlan ();
		if(P == null) return ;
		shipput_data0 = data0;
		shipput_data1 = data1;
		if(IsTransgateRoom () == true)
		{
			ShipPutInfo Info = P.GetShipBuildInfo(Core);
			if(Info != null)
			{
				OffX = data0 - Info.cxMapGrid;
				OffY = data1 - Info.cyMapGrid;
				LinkRoomID  = Core.m_ID;
			}
			else
			{
				LinkRoomID = -1;
				OffX = 0 ;
				OffY = 0 ;
			}
		}
	}
    /// <summary>
    /// 更新传送门- 传送点
    /// </summary>
    /// <param name="diffX"></param>
    /// <param name="diffY"></param>
	public void UpdateTransgateRoomParam(int diffX, int diffY)
	{
		ShipPlan P = ShipPlanDC.GetCurShipPlan ();
		if(P == null) return ;
		if(IsTransgateRoom () == false)
			return ;
		ShipPutInfo Info = P.GetShipBuildInfo(new CanvasCore(ShipBuildType.BuildRoom ,false, LinkRoomID,LinkRoomID,Vector2.zero));
		if(Info != null)
		{
			shipput_data0 = Info.cxMapGrid + OffX;
			shipput_data1 = Info.cyMapGrid + OffY;
		}
		else
		{
			shipput_data0 += diffX;
			shipput_data1 += diffY;
		}
	}
	
	/// <summary>
	/// 获取摆设位置
	/// </summary>
	public Int2 GetBuildMapGridPos()
	{
		return new Int2(cxMapGrid ,cyMapGrid);
	}



	/// <summary>
	/// 获取占领的房间位置
	/// </summary>
	public List<Int2> GetPutRoomGridPos()
	{
		return GetMovetoRoomGridPos(new Int2 (cxMapGrid,cyMapGrid));
	}

	/// <summary>
	/// 确定点在房间里面
	/// </summary>
	public bool CheckInRoom(Int2 GridPos)
	{
		List<Int2> l = GetPutRoomGridPos();
		if(l == null || l.Count == 0)
			return false;
		int Unit = GridPos.Unit / MapGrid.m_UnitRoomGridNum ;
		int Layer = GridPos.Layer ;

		foreach(Int2 Pos in l )
		{
			if(Pos.Layer == Layer && Unit ==  Pos.Unit )
				return true;
		}
		return false ;
	}

	/// <summary>
	/// 传送门关联房间
	/// </summary>
	public void LinkTransgatePointRoom(List<ShipPutInfo> l)
	{
		if(IsTransgateRoom () == false)
			return ;
		foreach(ShipPutInfo Info in l)
		{
			if(Info.CheckInRoom (new Int2(shipput_data0 ,shipput_data1)) == true)
			{
				LinkRoomID = Info.id;
				OffX = shipput_data0 - Info.cxMapGrid;
				OffY = shipput_data1 - Info.cyMapGrid;
				return;
			}
		}
	}

	/// <summary>
	/// 获取移动到目标位置所占领的房间位置
	/// </summary>
	/// <param name="TargetPos">目的位置</param>
	public List<Int2> GetMovetoRoomGridPos(Int2 TargetPos)
	{
		List<Int2> l = new List<Int2>();
		ShapeType shape = GetPutRoomShape();
		if (shape != null) {
						Dictionary<Int2 ,ShapeValue > lshape = shape.GetShapeData (TargetPos);
						foreach (Int2 Pos in lshape.Keys) {
								l.Add (Pos);
						}
		} else {
			//如果没有形状态则本格子就是其形状态
			Int2 posSoldier =  new Int2(TargetPos.Unit/ MapGrid.m_UnitRoomGridNum,TargetPos.Layer);
			l.Add (posSoldier);
				}
		return l;
	}

	/// <summary>
	/// 获取地图数据
	/// </summary>
	public List<Int2> GetPuRoomMapData()
	{
		return GetMovetoRoomMapData(new Int2 (cxMapGrid,cyMapGrid));
	}
	/// <summary>
	/// 获取地图数据
	/// </summary>
	/// <param name="TargetPos">目的位置</param>
	public  List<Int2> GetMovetoRoomMapData(Int2 TargetPos )
	{
		List<Int2> l = new List<Int2>();
		ShapeType shape = GetPutRoomShape();
		if(shape != null) 
		{
			Dictionary<Int2 ,ShapeValue > lshape = shape.GetShapeData(TargetPos);
			foreach(Int2 Pos in lshape.Keys)
			{
				if(lshape[Pos].DownMap == 1)
				{
					if(l.Contains(Pos) == false)
					{
						l.Add(Pos);
					}
				}

				if(lshape[Pos].UpMap == 1)
				{
					Int2 UpPos = new Int2(Pos.Unit ,Pos.Layer +1);
					if(l.Contains(UpPos) == false)
					{
						l.Add(UpPos);
					}
				}
			}
		}
		return l;
	}

	/// <summary>
	/// 获取地形房间及其内部陷阱
	/// </summary>
	/// <author>zhulin</author>
	public BuildInfo  GetBuildInfo()
	{
		BuildInfo bInfo = null;
		if(type == (int)ShipBuildType.BuildRoom)
		{
			bInfo = BuildDC.GetBuilding(objid);
		}
		else if(type == (int)ShipBuildType.BuildStair)
		{
			bInfo = buildingM.GetStartBuildInfo(objid);
		}
	 
		if(bInfo != null)
		{
			bInfo.ID = objid;
			bInfo.m_cx = cxMapGrid;
			bInfo.m_cy = cyMapGrid;
			bInfo.m_ShipPutdata0 = shipput_data0;
			bInfo.m_ShipPutdata1 = shipput_data1;
			if(bInfo.m_RoomType == RoomType.DeckTrap)
				m_DeckRoom = true;
			else m_DeckRoom = false;
		}
		return bInfo ;
	}
	/// <summary>
	/// 获取炮弹兵
	/// </summary>
	/// <author>zhulin</author>
	public bool  GetSoldier(ref SoldierInfo  Soldier)
	{
		Soldier = SoldierDC.GetSoldiers(objid);
		if(Soldier != null)
		{
			Soldier.CX = cxMapGrid;
			Soldier.CY = cyMapGrid;
			Soldier.ID = objid;
			return true;
		}
		return false;
	}
    /// <summary>
    /// 判定是否是金库
    /// </summary>
	public bool IsGoldBuild()
	{
        if ((int) ShipBuildType.BuildRoom == type)
	    {
			BuildInfo roomBuild = GetBuildInfo();
			if (roomBuild.BuildType == 1300) 
			{
				return true;
			}
		}
		return false;
	}



    /// <summary>
    /// 判定是否是某种陷阱
    /// </summary>
	public bool IsTransgateRoom()
    {
        if ((int)ShipBuildType.BuildRoom == type)
        {
			BuildInfo roomBuild = GetBuildInfo();
			if (roomBuild.BuildType == 1605)
				return true;
        }
        return false;
    }


	/// <summary>
	/// 可置换的房间
	/// </summary>
	public static bool CheckExchangeRoom (ShipPutInfo S1 ,ShipPutInfo S2)
	{
		if(S1 == null || S2 == null ) return false;
		if(S1.type != S1.type) 
			return false; 
		if (S1.type == (int)ShipBuildType.Soldier)
				return true;
		if(S1.type == (int) ShipBuildType.BuildStair)  
			return false;
		if(S1.m_DeckRoom != S2.m_DeckRoom) 
			return false;
		ShapeType t1 = S1.GetPutRoomShape();
		ShapeType t2 = S2.GetPutRoomShape();
		return ShapeType.CheckShapeSame(t1 ,t2);
	}
   
	/// <summary>
	/// 获取新增楼梯ID
	/// </summary>
	public static int GetNewShipPutId ()
	{
		return NewID  -- ;
	}
	
	public static void ClearNewShipPutId ()
	{
		NewID  = 0;
	}
}


/// <summary>
/// 建筑形状表
/// </summary>
/// <author>zhulin</author>
public class ShapeType  {
	
	public int id;        //编号
	public int width;     //长度  300个像素为一个单位，也就是6个格子的长度 
	public int height;    //层数
	public string shape;  //形状
	public string map;    //地图



	public static bool CheckShapeSame(ShapeType S1 ,ShapeType S2)
	{
		if(S1 == null || S2 == null) 
			return false;
		if(S1.width != S2.width) 
			return false;
		if(S1.height != S2.height) 
			return false;
		if(S1.shape != S2.shape) 
			return false;
		return true;
	}

	/// <summary>
	/// 获取形状数据
	/// </summary>
	public Dictionary<Int2 ,ShapeValue > GetShapeData()
	{
		Dictionary<Int2 ,ShapeValue > l = new Dictionary<Int2 ,ShapeValue >();
		if(width > 0 && height > 0)
		{
			for(int layer = 0; layer < height ; layer++)
			{
				for(int unit = 0; unit < width ; unit ++ )
				{
					int shape = GetShapeValue (layer , unit);
					if(shape == 1)
					{
						ShapeValue value = new ShapeValue();
						value.UpMap = GetUpMapValue (layer , unit);
						value.DownMap = GetDownMapValue (layer , unit);
						l.Add(new Int2 (unit ,layer	),value);
					}
				}
			}
		}
		return l;
	}
	/// <summary>
	/// 获取形状数据
	/// </summary>
	public Dictionary<Int2 ,ShapeValue > GetShapeData(Int2 Roomstart)
	{
		Dictionary<Int2 ,ShapeValue > l = new Dictionary<Int2 ,ShapeValue >();
		if(Roomstart.Unit % MapGrid.m_UnitRoomGridNum != 0)
		{
			Debug.LogError("房间起始位置不对:" + Roomstart) ;
			return l;
		}
		Dictionary<Int2 ,ShapeValue > ll = GetShapeData();

		foreach(Int2 Pos in ll.Keys)
		{
			Int2 P = new Int2(Pos.Unit + Roomstart.Unit / MapGrid.m_UnitRoomGridNum ,Pos.Layer + Roomstart.Layer);
			l.Add(P, ll[Pos] );
		}
		return l;
	}
	/// <summary>
	/// 获取指定的房间地图数据
	/// </summary>
	public int GetShapeValue(int layer, int Unit)
	{
		string strlayer = NdUtil.GetStrValue(shape ,layer );
		return int.Parse(strlayer.Substring(Unit ,1));	
	}
	
	/// <summary>
	/// 获取指定的房间地图数据
	/// </summary>
	public int GetUpMapValue(int layer, int Unit)
	{
		string strlayer = NdUtil.GetStrValue(map ,layer + 1);
		return int.Parse(strlayer.Substring(Unit ,1));	
	}
	
	/// <summary>
	/// 获取指定的房间地图数据
	/// </summary>
	public int GetDownMapValue(int layer, int Unit)
	{
		string strlayer = NdUtil.GetStrValue(map ,layer );
		return int.Parse(strlayer.Substring(Unit ,1));	
	}
}

public class ShapeValue  {
	public int UpMap;     //地图
	public int DownMap;   //地图
	public int Position;  //position
}
