using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 建筑物属性
/// </summary>
/// <author>zhulin</author>
///
public class BuildAttribute : NDAttribute {

	/// <summary>
	/// 死亡Mana
	/// </summary>
	protected int m_DeadMana;
	public int DeadMana
	{
		get { return m_DeadMana; }
		set { m_DeadMana = value; }
	}

	private BuildInfo m_BuildInfo = null;
	public override void Init(int SceneID, LifeMCore Core, Life Parent)
	{
		base.Init(SceneID,Core,Parent);
		m_Broken = false;
		if(m_BuildInfo != null)
		{
			m_Durability = m_BuildInfo.m_data0 ;
			BuildAttributeType = SkillM.GetBuildAttributeType(m_BuildInfo.m_RoomKind);
			JoinMap(SceneID  ,m_StartPos ,m_BuildInfo.m_Shape);
		}
	}

	public BuildAttribute(){}
	
	public BuildAttribute( BuildInfo Info)
	{
		if(Info != null)
		{
			m_BuildInfo = Info;
			m_level = Info.Level;
			m_AttrType = Info.BuildType;
			m_phy_defend = Info.m_phydefend;
			m_magic_defend = Info.m_magicdefend;
			m_phy_attack = Info.m_phyattack;
			m_magic_attack = Info.m_magicattack;
			m_FullHp = Info.m_hp + Info.m_Floorhp;
			m_StartPos.Unit = Info.m_cx;
			m_StartPos.Layer = Info.m_cy;
			//熔炉房如果取中点，得到的点会可能出现再船外，所以特殊处理
			/*if (m_BuildInfo.m_Shape.height > 1)
			{
				for(int unit = 0; unit < m_BuildInfo.m_Shape.width ; unit ++ )
				{
					if( m_BuildInfo.m_Shape.GetShapeValue (0 , unit) == 1 )
					{
						m_Pos.Layer = m_BuildInfo.m_cy ;
						m_Pos.Unit = m_BuildInfo.m_cx + unit * MapGrid.m_UnitRoomGridNum + 3;
						break;
					}
				}
			}
			else*/
			{
				m_Pos.Unit = Info.m_cx + Info.m_Shape.width * MapGrid.m_UnitRoomGridNum /2;
				m_Pos.Layer = Info.m_cy;
			}
			m_Size = Info.m_Shape.width * MapGrid.m_UnitRoomGridNum;
			if(Info.m_damage == 0)
				m_IsDamage = false;
			else m_IsDamage = true;

			
			if(Info.m_RoomType == RoomType.ResRoom)
				m_IsResource = true;
			else m_IsResource = false;
			
			
			m_wood = Info.m_wood;
			m_stone = Info.m_stone;
			m_steel = Info.m_steel;
			m_Type = Info.BuildType;
			m_HideCd = Info.m_HideCD * 0.001f;
			m_ShipPutdata0 = Info.m_ShipPutdata0;
			m_ShipPutdata1 = Info.m_ShipPutdata1;
			m_bear = Info.m_bear;
			m_DeadMana = Info.Mana;
		}
	}

	public List<MapGrid> GetAllMapGrid()
	{
		List<MapGrid> l = new List<MapGrid>();
		for(int layer = 0; layer < m_BuildInfo.m_Shape.height ; layer++)
		{
			for(int unit = 0; unit < m_BuildInfo.m_Shape.width ; unit ++ )
			{
				if( m_BuildInfo.m_Shape.GetShapeValue (layer , unit) == 1 )
				{
					int y = m_BuildInfo.m_cy + layer;
					int x = m_BuildInfo.m_cx + unit * MapGrid.m_UnitRoomGridNum + MapGrid.m_UnitRoomGridNum / 2;
					MapGrid g = MapGrid.GetMG(y,x);
					l.Add(g);
				}
			}
		}
		return l;
	}

	public bool CheckInBuildMap(Int2 Pos)
	{
		if (null == m_BuildInfo)
				return false;
		return m_BuildInfo.CheckInBuildMap (Pos);
	}

	protected void JoinMap(int SceneID, Int2 Start,ShapeType Info )
	{
		if(Info == null) return ;
		List<Int2> l = new List<Int2>();
		for(int layer = 0; layer < Info.height ; layer++)
		{
			for(int unit = 0; unit < Info.width ; unit ++ )
			{
				if( Info.GetShapeValue (layer , unit) == 1 )
				{
					for(int i = 0; i <= MapGrid.m_UnitRoomGridNum; i ++)
					{
						l.Add(new Int2(Start.Unit + unit * MapGrid.m_UnitRoomGridNum + i, Start.Layer + layer));
					}
					/*l.Add(new Int2(Start.Unit + unit * MapGrid.m_UnitRoomGridNum + 0, Start.Layer + layer));
					l.Add(new Int2(Start.Unit + unit * MapGrid.m_UnitRoomGridNum + 1, Start.Layer + layer));
					l.Add(new Int2(Start.Unit + unit * MapGrid.m_UnitRoomGridNum + 2, Start.Layer + layer));
					l.Add(new Int2(Start.Unit + unit * MapGrid.m_UnitRoomGridNum + 3, Start.Layer + layer));
					l.Add(new Int2(Start.Unit + unit * MapGrid.m_UnitRoomGridNum + 4, Start.Layer + layer));
					l.Add(new Int2(Start.Unit + unit * MapGrid.m_UnitRoomGridNum + 5, Start.Layer + layer));*/
				}
			}
		}
		//
		foreach (Int2 Pos in l )
		{
			MapGrid Gird = MapGrid.GetMG(Pos);
			if(Gird != null)
			{
				Gird.JoinBuild(m_SceneID);
			}
		}
	}


	protected override int GetBaseAttrData(EffectType Type)
	{
		if(Type == EffectType.Strength)
			return m_strength;
		else if(Type == EffectType.Agility)
			return m_agility;
		else if(Type == EffectType.Intelligence)
			return m_intelligence;
		else if(Type == EffectType.Hp)
			return m_FullHp;
		else if(Type == EffectType.PhyAttack)
			return m_phy_attack;
		else if(Type == EffectType.PhyDefense)
			return m_phy_defend;
		else if(Type ==EffectType.MagicAttack)
			return m_magic_attack;
		else if(Type ==EffectType.MagicDefense)
			return m_magic_defend;
		else if(Type ==EffectType.PhyCrit)
			return 1;
		else if(Type ==EffectType.MagicCrit)
			return 1;
		else if (Type == EffectType.CutPhyDefend)
			return m_CutPhyDefend;
		else if(Type == EffectType.CutMagDefend)
			return m_CutMagDefend;
		else if(Type == EffectType.CutphyDamage)
			return m_CutPhyDamage;
		else if(Type == EffectType.CutMagDamage)
			return m_CutMagDamage;
		else if(Type == EffectType.Dodge)
			return m_dodge;
		else if(Type == EffectType.Vampire)
			return m_Vampire;
		else if(Type == EffectType.Hit)
			return m_Hit;
		else if(Type == EffectType.RecoHp)
			return m_RecoHp;
		else if(Type == EffectType.RecoAnger)
			return m_RecoAnger;
		else if(Type == EffectType.AddDoctor)
			return m_AddDoctor;
		
		return 0;
	}
	

	protected override int GetType()
	{
		return m_Type;
	}	/// <summary>
	/// 获取力量
	/// </summary>
	protected virtual int GetStrength()
	{
		return m_strength;
	}
	
	/// <summary>
	/// 获取敏捷
	/// </summary>
	protected virtual int GetAgility()
	{
		return m_agility;
	}
	/// <summary>
	/// 获取智力
	/// </summary>
	protected virtual int GetIntelligence()
	{
		return m_intelligence;
	}
	/// <summary>
	/// 获取魔法防御
	/// </summary>
	protected override int GetMagicDefend()
	{
	     return m_magic_defend;
	}

	/// <summary>
	/// 获取物理防御
	/// </summary>
	protected override int GetPhyDefend()
	{
		return m_phy_defend;
	}

	/// <summary>
	/// 获取满血
	/// </summary>
	protected override int GetFullHp()
	{
		return m_FullHp;
	}
	
	
	/// <summary>
	/// 获取建筑start位置
	/// </summary>
	protected override Int2 GetStartPos()
	{
		return m_StartPos;
	}
	
	/// <summary>
	/// 获取建筑大小，体型
	/// </summary>
	protected override int GetSize()
	{
		return m_Size;
	}


	/// <summary>
	/// 获取木材数量
	/// </summary>
	protected override int GetWood()
	{
		return m_wood;
	}
	
	
	
	/// <summary>
	/// 获取石头数量
	/// </summary>
	protected override int GetStone()
	{
		return m_stone;
	}
	
	
	/// <summary>
	/// 获取刚才数量
	/// </summary>
	protected override int GetSteel()
	{
		return m_steel;
	}


	/// <summary>
	/// 建筑位置
	/// </summary>
	protected override Int2 GetPos()
	{
		return m_Pos;
	}
	protected override int GetCritRatio()
	{
		return m_CritRatio;
	}
	/// <summary>
	/// 获取满怒气
	/// </summary>
	protected override int GetFullAnger()
	{
		return 0;
	}
}
