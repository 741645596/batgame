using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum RoomType
{
	NormalTrap      = 0, 
	DeckTrap        = 1, 
	ResRoom         = 2, 
	Stair           = 3,
	EmptyTrap       = 4,

};

public class BuildInfo
{
	private int m_BuildID;
	public int ID
	{
		get{return m_BuildID;}
		set{if(value != -1)
			m_BuildID = value;}
	}



	private int m_type;
	public int BuildType
	{
		get{return m_type;}
		set{if(value != -1)
			m_type = value;}
	}

	private int m_level;
	public int Level
	{
		get{return m_level;}
		set{if(value != -1)
			m_level = value;}
	}


	private int m_Quality;
	public int Quality
	{
		get{return m_Quality;}
		set{if(value != -1)
			m_Quality = value;}
	}


	private int m_Star;
	public int StarLevel
	{
		get{return m_Star;}
		set{if(value != -1)
			m_Star = value;}
	}

	private int m_Mana;
	public int Mana
	{
		get { return m_Mana; }
		set
		{
			if (value != -1)
				m_Mana = value;
		}
	}



	public int m_cx;
	public int m_cy;
	/// <summary>
	/// 0 无属性房间, 1.火属性房间, 2.水属性 3.雷属性,4.毒属性，5.气属性
	/// </summary>
	public int m_RoomKind; 
	public int m_levellimit;
	public string m_name;
	public int m_modeltype;
	public int m_damage;
	public string m_Desc;
	public RoomType m_RoomType   = RoomType.NormalTrap;
	public int m_coin;
	public int m_wood;
	public int m_stone;
	public int m_steel;
	public int m_hp;
	public int m_Floorhp; 
	public float m_Solidity;   
	public float m_Intensity; 
	public float m_Tenacity;   
	public int m_phyattack;
	public int m_magicattack;
	public int m_phydefend;
	public int m_magicdefend;
	public int m_ShipPutdata0 = -1;
	public int m_ShipPutdata1 = -1;
	public int m_data0;
	public int m_data1;
	public int m_DefensePower;
	public BuildSkillInfo m_Skill;
	public int m_HideCD;
	public int fragmentTypeID;
	public ShapeType    m_Shape = new ShapeType();


	public void ClearCombatData()
	{
		m_Skill.ClearCombatData();
	}

	//受击能力
	public int m_bear;
	public List<FloorData> m_lFloor = new List<FloorData>();
	public BuildInfo()
	{
	}

	public BuildInfo(int buildType,int buildLevel ,int Quality ,int Star)
	{
		m_type = buildType;
		m_level = buildLevel;
		m_Quality = Quality;
		m_Star = Star;
	}
	



	public bool CheckInBuildMap(Int2 Pos)
	{
		if(m_Shape == null) 
			return false;
		List<Int2> l = new List<Int2>();
		for(int layer = 0; layer < m_Shape.height ; layer++)
		{
			for(int unit = 0; unit < m_Shape.width ; unit ++ )
			{
				if( m_Shape.GetShapeValue (layer , unit) == 1 )
				{
					l.Add(new Int2(m_cx + unit * MapGrid.m_UnitRoomGridNum + 0, m_cy + layer));
					l.Add(new Int2(m_cx + unit * MapGrid.m_UnitRoomGridNum + 1, m_cy + layer));
					l.Add(new Int2(m_cx + unit * MapGrid.m_UnitRoomGridNum + 2, m_cy + layer));
					l.Add(new Int2(m_cx + unit * MapGrid.m_UnitRoomGridNum + 3, m_cy + layer));
					l.Add(new Int2(m_cx + unit * MapGrid.m_UnitRoomGridNum + 4, m_cy + layer));
					l.Add(new Int2(m_cx + unit * MapGrid.m_UnitRoomGridNum + 5, m_cy + layer));
				}
			}
		}
		if(l.Contains(Pos) == true)
			return true;
		else return false;
	}

	public List<Int2 > GetPutRoom()
	{
		List<Int2 > lPutRoom = new List<Int2 >();
		if(m_Shape != null)
		{
			Dictionary<Int2 ,ShapeValue > l = m_Shape.GetShapeData(new Int2 (m_cx,m_cy));
			lPutRoom.AddRange(l.Keys);
		}
		return lPutRoom;
	}

	/// <summary>
	/// 建造建筑物的等级限制
	/// </summary>
	public ShipCanvasInfo GetBuildCanvasInfo()
	{
		ShipCanvasInfo buildMap = new ShipCanvasInfo ();
		//m_Shape
		if(m_Shape != null)
		{
			buildMap.SetMapData(m_Shape.width ,m_Shape.height ,m_Shape.map,m_Shape.shape);
		}
		else Debug.Log("building shape is null");
		return buildMap ;
	}
	/// <summary>
	/// 加入甲板
	/// </summary>
	public void JoinFloor(List<FloorData> lFloor)
	{
		m_lFloor.Clear();
		if( lFloor == null || lFloor.Count == 0)
			return ;
		foreach(FloorData f in lFloor)
		{
			Int2  Pos = f.m_FloorPos;
			FloorType type = f.m_FloorType ;
			if(type == FloorType.Normal || type == FloorType.top)
			{
				if(CheckInBuildMap(Pos) == true)
				{
					m_lFloor.Add(f);
				}
				Pos.Layer = Pos.Layer -1;
				if(Pos.Layer < 0) Pos.Layer = 0;
				if(CheckInBuildMap(Pos) == true)
				{
					m_lFloor.Add(f);
				}
			}
			else if(type == FloorType.left)
			{
				Pos.Unit += 3; 
				if(CheckInBuildMap(Pos) == true)
				{
					m_lFloor.Add(f);
				}
			}
			else if(type == FloorType.right)
			{
				Pos.Unit -= 3;
				if(CheckInBuildMap(Pos) == true)
				{
					m_lFloor.Add(f);
				}
			}
		}
	}

	/// <summary>
	/// 计算加班血量
	/// </summary>
	public void AddFlooHp()
	{
		if(m_lFloor != null || m_lFloor.Count > 0)
		{
			FloorInfo f = CmCarbon.GetFloor();
			if(f != null)
			{
				m_Floorhp = f.m_hp;
			}
		}
	}

	/// <summary>
	/// 确认属于某个系的房间
	/// </summary>
	/// <author>zhulin</author>
	public bool CheckAttributeType(AttributeType KindType)
	{
		int value = (int)KindType;
		AttributeType v = SkillM.GetBuildAttributeType(m_RoomKind);
		int iv = (int) v;
		if((value & iv) == iv)
			return true;
		else return false;
	}

	public bool EqualOf(int buildType ,int level ,int star ,int quality)
	{
        return (this.BuildType == buildType &&
                this.Level == level &&
                this.StarLevel == star &&
                this.Quality == quality);
	}
	
	
	public bool EqualOf(BuildInfo info)
	{
		return (this.BuildType == info.BuildType &&
		        this.Level == info.Level &&
		        this.StarLevel == info.StarLevel &&
		        this.Quality == info.Quality);
	}
	/// <summary>
	/// 判断能否被分解
	/// </summary>
	public bool CheckSplit()
	{
		List<ShipPlan> l = ShipPlanDC.GetAllShipPlan();
		foreach (ShipPlan plan in l)
		{
			if(plan.CheckHaveTrap(ID) == true)
				return false;
		}
		return true;
	}
}




//墙体数据
public class WallInfo
{
	public int m_id;
	public int m_cx;
	public int m_cy;
	public int m_phydefned;
	public int m_magicdefend;
	public int m_hp;
	public int m_w;
	//掉落资源
	public int m_wood ;
	public int m_stone ;
	public int m_steel ;
	
	
	public WallInfo()
	{
		
	}
	
	public WallInfo( WallInfo s)
	{
		swap (s);
	}
	
	public void Copy( WallInfo s)
	{
		swap (s);
	}
	
	private void swap( WallInfo s)
	{
		this.m_id = s.m_id;
		this.m_cx = s.m_cx;
		this.m_cy = s.m_cy;
		this.m_phydefned = s.m_phydefned;
		this.m_magicdefend = s.m_magicdefend;
		this.m_hp = s.m_hp;
		this.m_w = s.m_w;
		
		this.m_wood = s.m_wood;
		this.m_stone = s.m_stone;
		this.m_steel = s.m_steel;
	}
}


//楼板数据
public class FloorInfo
{
	public int m_deck_id = 0;
	public int m_level = 0;
	public int m_material_id = 0;
	public int m_hp = 0;
	public int m_phydefend = 0;
	public int m_magicdefend = 0;
	
	public FloorInfo()
	{
		
	}
	
	public FloorInfo( FloorInfo s)
	{
		swap (s);
	}
	
	public void Copy( FloorInfo s)
	{
		swap (s);
	}
	
	private void swap( FloorInfo s)
	{
		this.m_deck_id = s.m_deck_id;
		this.m_level = s.m_level;
		this.m_material_id = s.m_material_id;
		this.m_hp = s.m_hp;
		this.m_phydefend = s.m_phydefend;
		this.m_magicdefend = s.m_magicdefend;
		this.m_magicdefend = s.m_magicdefend;
	}
}
