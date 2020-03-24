using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 召唤物属性
/// </summary>
/// <author>zhulin</author>
public class PetAttribute : NDAttribute {

	private List<SkillStatusInfo>m_SelfStatus = new List<SkillStatusInfo>();

	public override void Init(int SceneID, LifeMCore Core , Life Parent)
	{
		base.Init(SceneID,Core,Parent);
		PetInfo Info = CmCarbon.GetPetInfo(Core);
		m_LifeTime = Info.LifeTime;
		if (m_LifeTime == 0)
			m_LifeTime = int.MaxValue;
	}


	
	public void GetAttrData(NDAttribute RoleAttr, LifeMCore Core)
	{
		if(RoleAttr == null ) 
			return ;
		
		PetInfo Info = CmCarbon.GetPetInfo(Core);
		m_AttrType = Info.m_type;
		m_speed = RoleAttr.Speed * Info.m_speedpercent * 0.01f;
		m_jumpDistance = (int)(RoleAttr.JumpDistance * Info.m_jumppercent * 0.01f);
		m_Hp = (int)(RoleAttr.Hp  * Info.m_hppercent *0.01f);
		m_FullHp = m_Hp;
		m_phy_attack = (int)(RoleAttr.PhyAttack  * Info.m_phyattackpercent *0.01f);
		m_phy_defend = (int)(RoleAttr.PhyDefend  * Info.m_phydefendpercent *0.01f);
		m_phy_crit   = (int)(RoleAttr.PhyCrit  * Info.m_phy_critpercent *0.01f);
		m_magic_attack = (int)(RoleAttr.MagicAttack  * Info.m_magicattackpercent *0.01f);
		m_magic_defend = (int)(RoleAttr.MagicDefend  * Info.m_phydefendpercent *0.01f);
		m_magic_crit   = (int)(RoleAttr.MagicCrit  * Info.m_magic_critpercent *0.01f);
		m_PetAttackType = Info.m_attacktype;
		m_PetDamage = Info.m_damage;
		m_Anger = Info.m_mp;
		m_FullAnger = 1000;

		m_TargetImmune = (ImmuneTarget)Info.m_isattack;
		//出生产生的状态
		m_SelfStatus = Info.m_status;
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


	/// <summary>
	/// 获取宠物产生的状态
	/// </summary>
	public List<SkillStatusInfo> GetStatusInfo()
	{
		return m_SelfStatus;
	}

	/// <summary>
	/// 获取生命时间
	/// </summary>
	protected override int GetLifeTime()
	{
		return m_LifeTime;
	}


	/// <summary>
	/// 获取满血量
	/// </summary>
	protected override int GetFullHp()
	{
		return m_FullHp;
	}
	/// <summary>
	/// 获取物理伤害
	/// </summary>
	protected override int GetCutPhyDefend()
	{
		return m_CutPhyDefend;
	}

	/// <summary>
	/// 获取物理攻击
	/// </summary>
	protected override int GetPhyAttack()
	{
		return m_phy_attack ;
	}
	/// <summary>
	/// 获取物理防御
	/// </summary>
	protected override int GetPhyDefend()
	{
		return m_phy_defend ;
	}
	/// <summary>
	/// 获取物理暴击
	/// </summary>
	protected override int GetPhyCrit()
	{
		return m_phy_crit ;
	}
	/// <summary>
	/// 获取魔法攻击
	/// </summary>
	protected override int GetMagicAttack()
	{
		return m_magic_attack ;
	}
	/// <summary>
	/// 获取魔法防御
	/// </summary>
	protected override int GetMagicDefend()
	{
		return m_magic_defend ;
	}
	/// <summary>
	/// 获取魔法暴击
	/// </summary>
	protected override int GetMagicCrit()
	{
		return m_magic_crit ;
	}

	/// <summary>
	/// 移动速度
	/// </summary>
	protected override float GetSpeed ()
	{
		return m_speed;
	}

	/// <summary>
	/// 获取移动速度
	/// </summary>
	protected override float GetSpeedercent()
	{
		return 1;
	}
	/// <summary>
	/// 获取攻击速度
	/// </summary>
	protected override float GetAttackSpeed()
	{
		return 1;
	}
	/// <summary>
	/// 跳跃距离
	/// </summary>
	protected override int GetJumpDistance()
	{
		return m_jumpDistance;
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
		return m_FullAnger;
	}
}
