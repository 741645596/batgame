using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 船只编辑核心数据结构
/// </summary>
public class CanvasCore  {
	
	public ShipBuildType m_type = ShipBuildType.BuildRoom;
	//public ShipBuildStyle m_style = ShipBuildStyle.InShip; 
    /// <summary>
    /// ShipPutInfo.ID
    /// </summary>
	public int m_ID = 0;
    /// <summary>
    ///  ShipPutInfo.objid
    /// </summary>
	public int m_DataID = 0;


	public ShipPutInfo Data{
		get{
			ShipPlan P = ShipPlanDC.GetCurShipPlan();
			return P.GetShipBuildInfo(this);
		}
	}
	
	private bool m_IsNewCreate=false; 
	public bool IsNewCreate{
		get{
			return m_IsNewCreate;
		}
		set{
			 m_IsNewCreate = value;
		}
	}

	public Int2 GetSize()
	{
		ShapeType shape = Data.GetPutRoomShape();
		if(shape != null)
		{
			return new Int2(shape.width ,shape.height) ;
		}
		else return  new Int2(1, 1);
	}


	public bool IsDeckRoom
	{
		get
		{
			if(m_type != ShipBuildType.BuildRoom)
				return false ;
			if(Data == null) return false;
			return Data.m_DeckRoom;
		}
	}

	public void RemoveSelf()
	{
		TouchMove tm = TouchMoveManager.GetShipBuild(this);
		if(tm != null)
		{
			tm.DestroyShipBuild();
		}
	}
	
	public CanvasCore()
	{
		m_type = ShipBuildType.BuildRoom;
		IsNewCreate = false; 
		m_ID = 0;
		m_DataID = 0;
		
	}
	
	public CanvasCore(ShipBuildType Type , bool IsNewCreate, int ID,int DataID,Vector2 size)
	{
		this.m_type = Type;
		this.IsNewCreate = IsNewCreate;
		this.m_ID = ID;
		this.m_DataID = DataID ;
		
	}

	public CanvasCore(CanvasCore Core)
	{
		Copy(Core);
	}
	
	public void Copy(CanvasCore Core)
	{
		this.m_type = Core.m_type;
		this.IsNewCreate = Core.IsNewCreate;
		this.m_ID = Core.m_ID;
		this.m_DataID = Core.m_DataID ;
	}
	
	public bool Equal(CanvasCore  Core)
	{
		if(m_ID == Core.m_ID 
		   && IsNewCreate == Core.IsNewCreate 
		   && m_type == Core.m_type)
			return true;
		else return false;
	}

	/// <summary>
	/// 获取占领的房间位置
	/// </summary>
	public List<Int2> GetPutRoomGridPos()
	{
		if(Data != null) return Data.GetPutRoomGridPos();
		return new List<Int2>();
	}
	/// <summary>
	/// 获取移动到目标位置所占领的房间位置
	/// </summary>
	/// <param name="TargetPos">目的位置 小格子</param>
	public List<Int2> GetMovetoRoomGridPos(Int2 TargetPos)
	{
		if(Data != null) return Data.GetMovetoRoomGridPos(TargetPos);
		return new List<Int2>();
	}
	
	public ShapeType GetPutRoomShape()
	{
		if(Data == null)
			return null;
		if ((int) ShipBuildType.BuildRoom != Data.type) 
			return null;
		BuildInfo  roomBuild = Data.GetBuildInfo();
		if(roomBuild != null)
		{
			return roomBuild.m_Shape;
		}
		return null;
	}

	
}

public enum ShipBuildType
{
	All         = 0,  //所有
	BuildRoom   = 1,  //房间
	Soldier     = 2,   //炮弹兵
	BuildStair  = 3,  //楼梯
}

/*public enum ShipBuildStyle
{
	InWarehouse    = 0, //还在仓库的(未能与金库连接上的)。
	InShip         = 1, //已在船上的。
}
*/
