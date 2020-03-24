using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 角色在格子地图上运行控制
/// </summary>
/// <author>zhulin</author>
/// 名称解释
/// MT = min time 炮弹兵中，改变到下一个格子的最小时间
/// CGT change grid time 改变到下一个格子所需的时间
/// PST path state tree 炮弹兵的路径状态树
/// NMTS next min time state 下一个mt时，炮弹兵的状态数据
public enum RoleState{
	//JUMPSTART,
	JUMP,//跳跃状态
	//JUMPEND,
	FALL, //掉落状态
	FALLDOWN, //掉落落地状态
	ATTACK, // 攻击状态
	//STAIRIN, 
	STAIR, //楼梯状态
	//STAIROUT,
	STAND, //暂停状态
	JUMPUP,
	JUMPDOWN,
	REVERSEJUMP,
	WALK, //走路在状态,
	HITFLY//被击飞状态
}
public class GridRun  {
	//格子单元位置
	public WalkDir  m_AttackDir = WalkDir.WALKSTOP;
	// Use this for initialization

	private PathRoad m_Path = new PathRoad();
	public PathRoad Path
	{
		get { return m_Path; }
	}
	
	protected Int2  m_PreGrid = Int2.zero;
	protected Int2  m_PreAttackStation = Int2.zero;
	public Int2 PreGrid
	{
		get{return m_PreGrid;}
		set{m_PreGrid = value;}
	}

    //预测拒绝
    protected bool m_Reject = false;
    protected bool m_UpdataPathFlag = true;
    protected AIEventData m_AIEventData = null;

	protected Int2  m_NGrid = Int2.zero;
	public Int2 NGrid
	{
		get{return m_NGrid;}
		set{m_NGrid = value;}
	}

	protected Int2  m_AttackStation = Int2.zero;
	public Int2 AttackStation
	{
		get{return m_AttackStation;}
		set{m_AttackStation = value;}
	}

	protected Int2  m_NAttackStation = Int2.zero;
	public Int2 NAttackStation
	{
		get{return m_NAttackStation;}
		set{m_NAttackStation = value;}
	}
	

	protected Life m_parent = null;
	protected int m_SceneID = 1;
	
	public GridRun(Life role)
	{
		this.m_parent = role;
		m_SceneID = role.m_SceneID;
	}


	public int RankDeep
	{
		get { return m_parent.RankDeep; }
		set { m_parent.RankDeep = value;}
	}
	
	
	public GridSpace CurrentGS
	{
		get{return m_parent.CurrentGS;}
		set{m_parent.CurrentGS = value;}
	}
	
	public Int2 MapPos
	{
		get { return m_parent.MapPos; }
		set { m_parent.MapPos = value;}
	}
	
	
	public WalkDir WalkDIR
	{
		get{return m_parent.WalkDir;}
		set{
			if (value != WalkDir.WALKSTOP)
				m_parent.WalkDir = value;
		}
	}



	public void SetUpdataPath(AIEventData data)
	{
		m_UpdataPathFlag = true;
		if ( m_AIEventData == null)
		{
			m_AIEventData = data;
			if (this is RoleGridRun)
			{
				(this as RoleGridRun).RoleParent.InterruptAction(data);
			}
		}
		else
		{
			if (AIPathConditions.ComareEvent(data, m_AIEventData))
			{
				m_AIEventData = data;
				if (this is RoleGridRun)
				{
					(this as RoleGridRun).RoleParent.InterruptAction(data);
				}
			}
		}
	}

}
