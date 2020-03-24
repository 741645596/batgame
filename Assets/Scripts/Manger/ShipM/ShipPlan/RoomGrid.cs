using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Flags]
public enum PutPosition{
	None        = 0x00,  //未占有空间
	Stair       = 0x01,  //占有楼梯
	Background  = 0x02,  //占有背景
	Ceiling     = 0x04,  //占有天花板
	Floor       = 0x08,  //占有地板
	Soldier     = 0x10,  //占有兵位	
	All         = Stair | Background | Ceiling | Floor | Soldier,
};

public enum XYSYS{
	MapGrid        = 0x00,  //格子坐标
	RoomGrid       = 0x01,  //房间坐标
};
/// <summary>
/// 画布单元，为 包含6 * 1个格子的 最小房间单元组成。 
/// </summary>
public class RoomGrid  {
	
	public const int EMPTYGRIDID = -int.MaxValue;
	public Int2  mPosRoomGrid = Int2.zero; //位置
	public int  buildingid = RoomGrid.EMPTYGRIDID;       //
	public int upMap;          //上地图格子
	public int downMap;        //下地图格子
	public string Name;
	public const float m_width = 3.0f;
	public const float m_heigth = 3.0f;
	//占有的摆设
	private int m_CanPosition = (int)PutPosition.None;  
	private int m_Position = (int)PutPosition.None;
	//拥有的子对象，房间，陷阱，楼梯，炮弹兵
	private  List<CanvasCore> m_Child = new List<CanvasCore>();
	public Vector3   LocalPos = Vector3.zero;
	private CanvasCore m_Core = new CanvasCore();
	//建筑摆设位置
	public Int2 BuildPos
	{
		get{return new Int2(mPosRoomGrid.Unit * MapGrid.m_UnitRoomGridNum ,mPosRoomGrid.Layer);}
	}

	//炮弹兵摆设位置
	public Int2 SoldierPos
	{
		get{return new Int2(mPosRoomGrid.Unit * MapGrid.m_UnitRoomGridNum + 4 ,mPosRoomGrid.Layer);}
	}

	public RoomGrid(){}
	public RoomGrid( int Layer, int Unit)
	{
		this.mPosRoomGrid = new Int2 (Unit , Layer);
		SetPosValue();
	}

	/// <summary>
	/// 拷贝摆设数据
	/// </summary>
	public void CopyPutData( RoomGrid R)
	{
		this.upMap = R.upMap;
		this.downMap = R.downMap;
		this.buildingid = R.buildingid;
		this.m_CanPosition = GetCanPosition();
		this.m_Position = R.m_Position;
		this.m_Child.Clear();
		this.m_Child.AddRange(R.GetOtherBuild());
	}
	
	private void SetPosValue()
	{
		m_Child.Clear();
		LocalPos.x = m_width * this.mPosRoomGrid.Unit;
		LocalPos.y =  (m_heigth+0.3f) * this.mPosRoomGrid.Layer;
		LocalPos.z = 0;
		Name = string.Format("[{0},{1}]", mPosRoomGrid.Layer, mPosRoomGrid.Unit);
	}
	
	
	public void SetRoomGrid(ShapeValue S ,int Buildid)
	{
		this.upMap = S.UpMap;
		this.downMap = S.DownMap;
		this.buildingid = Buildid;
		this.m_CanPosition = GetCanPosition();
		this.m_Position = (this.m_Position | S.Position );

	}
	//继承地图数据，
	public void InheritanceRoomGrid(RoomGrid R)
	{
		this.upMap = R.upMap;
		this.downMap = R.downMap;
		this.buildingid = R.buildingid;
		this.m_CanPosition = GetCanPosition();
		this.m_Position = (this.m_Position | R.m_Position);
		this.m_Child.AddRange(R.GetOtherBuild());
	}
	
	private int GetCanPosition()
	{
		int value  = (int) PutPosition.Floor  + (int) PutPosition.Background + (int) PutPosition.Stair  + (int) PutPosition.Ceiling;
		if(downMap == 1) 
			value += (int) PutPosition.Soldier;
		return value;
	}

	public void SetDeckRoomValue(int UpMap )
	{
		int value = 0;
		if(UpMap == 1)
		{
			value  = (int) PutPosition.Floor  + (int) PutPosition.Soldier;
		}
		this.m_CanPosition = value ;
		//Debug.Log("SetDeckRoomValue" + Pos );
	}

	
	public void EmptyRoomGrid()
	{
		this.upMap = 0;
		this.downMap = 0;
		this.buildingid = RoomGrid.EMPTYGRIDID;
		this.m_CanPosition = 0;
		this.m_Position = 0;
		this.m_Child.Clear();
		//Debug.Log("EmptyRoomGrid");
	}	
	/// <summary>
	/// 设置占有空间
	/// </summary>
	/// <param name="position">占有空间</param>
	public void AddPosition(CanvasCore Core ,int position)
	{
		int value = (int)PutPosition.All;
		position &= value;
		m_Position |= position;
		m_Child.Add(new CanvasCore(Core));
		//Debug.Log("AddPosition:" + m_Position + "," + Pos);
	}
	/// <summary>
	/// 移除占有空间
	/// </summary>
	/// <param name="position">占有空间</param>
	public void RemovePosition(CanvasCore Core,int position)
	{
		int value = (int)PutPosition.All;
		position &= value;
		m_Position &= ~position;
		m_Position &= value;
		//Debug.Log("RemovePosition:" + m_Position + "," + Pos);
		foreach(CanvasCore c in m_Child)
		{
			if(c.m_ID == Core.m_ID && c.m_type == Core.m_type)
			{
				m_Child.Remove(c);
				return;
			}
		}
	}
	/// <summary>
	/// 确认能否摆设
	/// </summary>
	/// <param name="position">占有空间</param>
	public bool CheckPosition(int position)
	{
		int value = (int)PutPosition.All;
		position &= value;
		//在能力允许范围内，
		int LeavePosition = m_CanPosition - m_Position;
		int v= LeavePosition & position ;
		if(v != position) return false;
		//没被占
		return (m_Position & position) == 0;
	}
	/// <summary>
	/// 确认房间能否放置，用于移动房间的检测
	/// </summary>
	/// <param name="position">占有空间</param>
	public bool CheckRoomGridPosition(int RoomPosition )
	{
		int value = (int)PutPosition.All;
		RoomPosition &= value;
		//在能力允许范围内，
		int LeavePosition = m_CanPosition - m_Position;
		int v= LeavePosition & RoomPosition ;
		if(v != RoomPosition) return false;
		//没被占
		return (m_Position & RoomPosition) == 0;
	}
	/// <summary>
	/// 获取房间的Position
	/// </summary>
	public int GetRoomPosition()
	{
		return m_Position;
	}
	/// <summary>
	/// 是否拥有楼梯
	/// </summary>
	public bool HaveStair()
	{
		foreach(CanvasCore c in m_Child)
		{
			if( c.m_type == ShipBuildType.BuildStair)
				return true;
		}
		return false;
	}

	/// <summary>
	/// 确认是否在区域里面。
	/// </summary>
	/// <param name="pos">局部坐标</param>
	public bool CheckInArea( Vector3 pos )
	{
		float LimY = 1.65f;
		float limX = 1.5f;
        float midX = LocalPos.x + m_width * 0.5f;
        float midY = LocalPos.y + m_heigth * 0.5f;
		float TargetMidX = pos.x ;
		float TargetMidY = pos.y ;

		float dx = Mathf.Abs(TargetMidX - midX);
		float dy = Mathf.Abs(TargetMidY - midY);


		if(  dx < limX &&  dy < LimY)
			return true;
		else return false;
	}

	public void  DrawRoomGridInfo()
	{
#if UNITY_EDITOR
		Vector3 pos = BattleEnvironmentM.Local2WorldPos(LocalPos);
		pos.x += m_width / 2;
		pos.y -= 0.3f ;
		GUIStyle style = new GUIStyle();
		style.normal.textColor = Color.green;
		int value = m_CanPosition - m_Position;
		Handles.Label(pos, value.ToString()  ,style);
        pos.y += 1.5f;
        pos.x -= 0.5f;
        Vector2 v = new Vector2(LocalPos.x, LocalPos.y);
        style.normal.textColor = Color.red;
        Handles.Label(pos, v.ToString(), style);
		/*
		if(IsDeckRoom == true)
		{
			pos.y -= 0.5f;
			style.normal.textColor = Color.yellow;
			Handles.Label(pos, "DeckRoom", style);
		}*/
#endif
	}
	/// <summary>
    /// 获取出地形房间外其他的对象
	/// </summary>
	public List<CanvasCore> GetOtherBuild()
	{
		List<CanvasCore> l = new List<CanvasCore>();
		l.AddRange(m_Child);
		return l;
	}
    /// <summary>
    /// 获取房间地形对象，未摆设地形（其中包含甲板房间）的房间获取不到
    /// </summary>
	public CanvasCore GetBuildRoom()
	{
		m_Core.m_type = ShipBuildType.BuildRoom;
		m_Core.IsNewCreate = false;
		m_Core.m_ID = buildingid;
		m_Core.m_DataID = buildingid ;
		return m_Core;
	}

	public void RemovStair()
	{
		foreach(CanvasCore c in m_Child)
		{
			if(c.m_type == ShipBuildType.BuildStair)
			{
				RemovePosition(c,(int)PutPosition.Stair);
				c.RemoveSelf();
				m_Child.Remove(c);
				return;
			}
		}
	}
}

