#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#define UNITY_EDITOR_LOG
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 状态属性
/// </summary>
/// <author>zhulin</author>
///

public enum StatusType
{
	AddAttr         = 0,   //加属性
	Vertigo         = 1,   //眩晕
	Silence         = 2,   //沉默
	Vampire         = 3,   //吸血
	Rebound         = 4,   //反弹
	Invisible       = 5,   //隐身
	Invincible      = 6,   //无敌
	Transfiguration = 7,   //变形术
	Summon          = 8,   //召唤术
	ClickFly        = 9,   //击飞
	AbsorbDamage    = 10,  //吸收伤害
	Virtual         = 11,  //虚化
	Shield          = 12,  //防护盾
	Sputtering      = 13,  //溅射
	Damnation       = 14,  //诅咒
	ImmunePhysical  = 15,  //物理免疫
	ImmunityMagic   = 16,  //魔法免疫
	IceBuild        = 17,  //结冰/冰冻
	Ring            = 18,  //光环
	Taunt           = 19,  //嘲讽.
	StaticElec      = 20,  //静电
	Still           = 21,  //静止状态 ，技能前摇阶段。
	Squash          = 22,  //压扁状态
	Sauna           = 23,  //桑拿
	KTV             = 24,  //KTV唱歌
	ImmuneState     = 25,  //免疫状态
	NoSquash        = 26,  //不被压扁状态
	paralysis       = 27,  //麻痹状态
	PhyAnitInterrupt= 28,  //物理防打断
	MagAnitInterrupt= 29,  //魔法防打断
	Cleanse         = 30,  //净化
	Charm           = 31,  //魅惑
	BuildDestory    = 33,  //压扁房毁坏
	Mark			= 34,	//标记
	RunAway			= 35,	//逃跑
	FakeTaunt		= 36,	//伪嘲讽，受到这个状态的会以被标记的人为目标
	Turn			= 37,	//变身
	WetBody         = 38,   //落水上身
	Paralyze      	= 40,   //陷阱瘫痪
	Berserker       = 41,   //狂暴变身
	DieMark			= 42,	//死神狙击标记
    Sleep           = 43,	//zzzZZZ
	Trapped			= 44,   //束缚状态
    TearOfMermaid   = 45,	//美人鱼之泪	
	KickedBack      = 46,   //黑胡子胡子踹踢退
    WhipCorpse      = 47,   //暴雨梨花鞭鞭尸
	FireShield		= 48,   //火焰盾
    Die             = 99,  //死亡状态 临时添加 by lqf
	None            = 100, //无效
};

/// <summary>
/// 防打断状态
/// </summary>
public enum  StatusInterrupt
{
	Phy      = 0,  //物理打断
	Magic    = 1,  //魔法打断
}
/// <summary>
/// 自我打断.
/// </summary>
public enum SelfStatusInterrupt
{
	PhyRupt = 0,//自我打断物理攻击
	MagicRupt = 1,//自我打断魔法攻击
	None = 10,//无
}


public class Status  {

	private int m_SceneID = 0;
	/// <summary>
	/// 状态数据列表
	/// </summary>
	private List<StatusM>m_RoleStatus = new List<StatusM>();
	/// <summary>
	/// 待移除列表，主要用于内存优化
	/// </summary>
	private List<StatusType>m_lRemoveStatus = new List<StatusType>();
	
	public void Init(int SceneID)
	{
		m_SceneID = SceneID;
		EventCenter.RegisterHooks(NDEventType.StatusInterrupt,InterruptStatus);
	}
	/// <summary>
	/// 添加状态,SceneID 释放技能对象的SceneID
	/// </summary>
	/// <returns>true :该状态会打断技能 false ： 该状态不打断技能</returns>
	public bool AddStatus(int ResalseSceneID,int SkillID,SkillStatusInfo Info)
	{
		StatusType s = (StatusType)Info.m_type;
		SetAttrChange();

		//状态叠加规则
		if (Info.m_RelpaceInfo.m_type == 1)
		{
		}
		else if (Info.m_RelpaceInfo.m_type == 2) //控制类状态
		{
			if (Info.m_RelpaceInfo.m_compatible == 0)
			{
				foreach (StatusM status  in m_RoleStatus)
				{
					if (status.CheckHaveStatus() && status.ReplaceInfo.m_type == 2)
					{
						if (status.ReplaceInfo.m_compatible == 0)
						{
							if (status.ReplaceInfo.m_reppriority > Info.m_RelpaceInfo.m_reppriority)
								return false;
						}
					}
				}
				foreach (StatusM status  in m_RoleStatus)
				{
					
					if (status.CheckHaveStatus() && status.ReplaceInfo.m_type == 2)
					{
						status.ClearStatus();
					}
				}
			}
			else
			{
				
				foreach (StatusM status  in m_RoleStatus)
				{
					if (status.ReplaceInfo.m_type == 2)
					{
						if (status.CheckHaveStatus() && status.ReplaceInfo.m_compatible  == 0)
							return false;
					}
				}
			}
		}
		else
		{
		}
		//
		StatusM Data = FindStatusM(s);
		if(Data != null)
		{
			if (Data.AddStatus(ResalseSceneID,SkillID,Info))
			{
				Data.AddStatusEffect(Info,SkillID,CheckCreateAction(Info));
				return Data.InterruptSkill();
			}
		}
		else
		{
			Data = StatusFactory.Create(m_SceneID,ResalseSceneID,SkillID,Info);
			if(Data != null)
			{
				Data.Init(m_SceneID,ResalseSceneID,SkillID,Info);
				m_RoleStatus.Add(Data);
				Info.m_releasesceneid = ResalseSceneID;
				Data.AddStatusEffect(Info,SkillID,CheckCreateAction(Info));

				Life life = CM.GetAllLifeM(m_SceneID, LifeMType.ALL);
				if (life != null)
				{
					if (Info.m_bufftype == 1)
					{
						life.ShowBuff(Info.m_name);
					}
					else
					{
						life.ShowDebuff(Info.m_name);
					}
				}

				return Data.InterruptSkill();
			}
		}
		return false;
	}
	public bool CheckCreateAction(SkillStatusInfo Info)
	{
		foreach (StatusM status  in m_RoleStatus)
		{
			if (status.CheckHaveStatus() && status.ReplaceInfo.m_type == 2)
			{
				if (status.ReplaceInfo.m_statustype != Info.m_type && status.ReplaceInfo.m_compatible  == 1 && status.ReplaceInfo.m_isaction == 1)
				{
					float longduratio = status.GetLongDuration();
					if (longduratio > Info.m_time * 0.001f)
					{
						return false;
					}
				}
			}
		}
		return true;
	}
	/// <summary>
	/// 确认能否添加状态。（状态放打断机制）
	/// </summary>
	/// <returns>true :该状态会打断技能 false ： 该状态不打断技能</returns>
	public bool CheckCanAddStatus(int ResalseSceneID,int SkillID, AttackType SkillType,SkillStatusInfo Info)
	{
		StatusInterrupt interrputtype = StatusInterrupt.Phy ;
		if(SkillType == AttackType.Physical)
			interrputtype = StatusInterrupt.Phy;
		else if(SkillType == AttackType.Magic)
			interrputtype = StatusInterrupt.Magic;
		else return true ;

		StatusM Data = StatusFactory.Create(m_SceneID,ResalseSceneID,SkillID,Info);
		//打断的技能
		if(Data != null /*&& Data.InterruptSkill() == true*/)
		{
			for(int i = 0 ; i < m_RoleStatus.Count; i++)
			{
				StatusM status = m_RoleStatus[i];
				if(status == null) continue;
				if(status.CheckInterruptStatus (interrputtype ) == true)
					return false;
			}
		}
		return true ;
	}


	public StatusM FindStatusM(StatusType type)
	{
		for(int i = 0 ; i < m_RoleStatus.Count; i++)
		{
			StatusM Data = m_RoleStatus[i];
			if(Data == null) continue;
			if(Data.m_type == type)
				return Data;
		}
		return null;
	}

	/// <summary>
	/// 属性变更
	/// </summary>
	public void SetAttrChange()
	{
		Life life = CM.GetAllLifeM(m_SceneID,LifeMType.ALL);
		if(life != null && life.m_Attr != null)
		{
			life.m_Attr.AttrChange = true;
		}
	}

	~Status()
	{
		EventCenter.AntiRegisterHooks(NDEventType.StatusInterrupt,InterruptStatus);
	}
	
	/// <summary>
	/// 持续时间到，移除状态
	/// </summary>
	public void RemoveStatus(float duration)
	{
		if(m_RoleStatus == null || m_RoleStatus.Count == 0)
			return ;
		for(int i = 0 ; i < m_RoleStatus.Count; i++)
		{
			StatusM Data = m_RoleStatus[i];
			if(Data == null) continue;
			Data.RemoveStatus(duration);
			if(Data.IsStatusOver())
			{
				Data.RemoveStatusEffect();
				SetAttrChange();
				m_lRemoveStatus.Add(Data.m_type);
			}
		}
		
		if(m_lRemoveStatus.Count > 0)
		{
			for(int i = 0; i< m_lRemoveStatus.Count; i++)
			{
				Remove(m_lRemoveStatus[i]);
			}
			m_lRemoveStatus.Clear();
		}
	}

	private void Remove(StatusType type)
	{
		for(int i = 0 ; i < m_RoleStatus.Count; i++)
		{
			StatusM Data = m_RoleStatus[i];
			if(Data == null) continue;
			if(Data.m_type == type)
			{
				m_RoleStatus.RemoveAt(i);
				return ;
			}
		}
	}


	/// <summary>
	/// 清理所有状态
	/// </summary>
	public void ClearAllStatus()
	{
		for(int i = 0 ; i < m_RoleStatus.Count; i++)
		{
			StatusM Data = m_RoleStatus[i];
			if(Data == null) continue;
			Data.ClearAllStatusAction();
		}
		m_RoleStatus.Clear();
	}

	// Update is called once per frame
	public void Update (float deltaTime) 
	{
		RemoveStatus(deltaTime);
	}



	public int GetAttrData(EffectType type)
	{		
		int value = 0;
		for(int i = 0 ; i < m_RoleStatus.Count; i++)
		{
			StatusM Data = m_RoleStatus[i];
			if(Data == null) continue;
			value += Data.GetAttrData(type);
		}
		return value;
	}

	public int GetResalseSceneID(StatusType type)
	{
		for(int i = 0 ; i < m_RoleStatus.Count; i++)
		{
			StatusM Data = m_RoleStatus[i];
			if(Data == null) continue;
			if(Data.m_type == type)
			{
				return Data.GetResalseSceneID();;
			}
		}
		return -1;
	}


	public bool HaveState(StatusType type)
	{
		StatusM Data = FindStatusM(type);
		if(Data != null) return true;
		else return false;
	}

	public float GetStateDuration(StatusType type)
	{
		float t = 0;
		StatusM Data = FindStatusM(type);
		if(Data != null)
		{
			foreach(StatusAction sa in Data.StatusData)
			{
				if (t == 0)
					t = sa.Duration;
				else if (t < sa.Duration)
					t = sa.Duration;
			}
		}
		return t;
	}

	public void TriggerStatus()
	{
		for(int i = 0 ; i < m_RoleStatus.Count; i++)
		{
			StatusM Data = m_RoleStatus[i];
			if(Data == null) continue;
			if(Data.m_type == StatusType.Invisible
			   || Data.m_type == StatusType.Vertigo)
			{
				Data.TriggerStatus();
			}
		}
	}

	private void InterruptStatus(int SceneID,object Param)
	{
		if(m_SceneID != SceneID) return ;
		LifeAction Action = (LifeAction)Param;

		for(int i = 0 ; i < m_RoleStatus.Count; i++)
		{
			StatusM Data = m_RoleStatus[i];
			if(Data == null) continue;
			Data.InterruptStatus(Action);
			if(Data.IsStatusOver())
			{
				Data.RemoveStatusEffect();
				m_lRemoveStatus.Add(Data.m_type);
			}
		}
		
		if(m_lRemoveStatus.Count > 0)
		{
			for(int i = 0; i< m_lRemoveStatus.Count; i++)
			{
				Remove(m_lRemoveStatus[i]);
			}
			m_lRemoveStatus.Clear();
		}

	}
	
	/// <summary>
	/// 技能免疫判断
	/// </summary>
	/// <param name="AttackSkillType">攻击方技能类型</param>
	/// <param name="DefanseStatus">防御方状态</param>
	/// <returns> true 免疫该技能， false 不免疫该技能</returns>
	public bool ImmunitySkill(AttackType AttackSkillType ,int SkillType )
	{
		if(m_RoleStatus.Count == 0)
			return false;
		for(int i = 0 ; i < m_RoleStatus.Count; i++)
		{
			StatusM Data = m_RoleStatus[i];
			if(Data == null) continue;
			if(Data.ImmunitySkill(AttackSkillType ,SkillType) == true)
				return true;
		}
		return false;
	}

	/// <summary>
	/// 是否拥有自我打断的状态
	/// </summary>
	/// <returns> true 免疫该技能， false 不免疫该技能</returns>
	public bool SelfInterrupt()
	{
		if(m_RoleStatus.Count == 0)
			return false;
		for(int i = 0 ; i < m_RoleStatus.Count; i++)
		{
			StatusM Data = m_RoleStatus[i];
			if(Data == null) continue;
			if(Data.SelfInterrupt() == true)
				return true;
		}
		return false;
	}

	/// <summary>
	/// 技能释放能被自身状态打断.
	/// </summary>
	/// <returns><c>true</c>, if status inter rupt was seled, <c>false</c> otherwise.</returns>
	/// <param name="skill">Skill.</param>
	public bool SeleStatusInterRupt(SoldierSkill skill)
	{
		for(int i = 0 ; i < m_RoleStatus.Count; i++)
		{
			StatusM Data = m_RoleStatus[i];
			if(Data == null) continue;
			if(Data.m_selfRuptType == SelfStatusInterrupt.None) continue;

			if(Data.m_selfRuptType == SelfStatusInterrupt.MagicRupt && skill.m_attacktype == (int)AttackType.Magic)
				return true;
			else if(Data.m_selfRuptType == SelfStatusInterrupt.PhyRupt && skill.m_attacktype == (int)AttackType.Physical)
				return true;
		}
		return false;
	}
	/// <summary>
	/// check 是否包含某个技能产生的状态
	/// </summary>
	/// <returns> true 包含 ，false 不包含</returns>
	public bool CheckStateBySkill(int SkillType)
	{
		for(int i = 0 ; i < m_RoleStatus.Count; i++)
		{
			StatusM Data = m_RoleStatus[i];
			if(Data == null) continue;
			if(Data.CheckStateBySkill(SkillType) == true)
				return true;
		}

		return false;
	}
	/// <summary>
	/// 盾减免血量
	/// </summary>
	/// <returns> true 包含 ，false 不包含</returns>
	public int ReduceHpbyShield(AttackType type ,int damage)
	{
		int reduceHp = 0;
		for(int i = 0 ; i < m_RoleStatus.Count; i++)
		{
			int hp = 0;
			StatusM Data = m_RoleStatus[i];
			if(Data == null) continue;
			hp = Data.ReduceHpbyShield(type ,damage);
			damage = damage - hp ;
			reduceHp += hp ;
			if(damage <= 0)
				break ;
		}
		/*if(reduceHp > 0)
		{
			NGUIUtil.DebugLog("XXXX" + reduceHp);
		}*/
		return reduceHp ;
	}
	/// <summary>
	/// 清理debuff状态
	/// </summary>
	public void ClearDebuffStatus()
	{
		for(int i = 0 ; i < m_RoleStatus.Count; i++)
		{
			StatusM Data = m_RoleStatus[i];
			if(Data == null) continue;
			if(Data.BuffType == 0)
			{
				Data.ClearStatus();
			}
		}
	}
}
