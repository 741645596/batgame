using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 召唤物属性
/// </summary>
/// <author>zhulin</author>
public class InheritSummonProsAttribute : NDAttribute
{

	private List<SkillStatusInfo>m_SelfStatus = new List<SkillStatusInfo>();
	private PetInfo m_Info;
	private Life m_Summoner;

	public override void Init(int SceneID, LifeMCore Core , Life Parent)
	{
		base.Init(SceneID,Core,Parent);
		m_Info = CmCarbon.GetPetInfo(Core);
		m_AttrType = m_Info.m_type;
		m_LifeTime = m_Info.LifeTime;
		if (m_LifeTime == 0)
			m_LifeTime = int.MaxValue;
	}

	public void InheritInit(int SceneID, LifeMCore Core, Life Parent, Life Summoner)
	{
		m_Summoner = Summoner;
		Init(SceneID,Core,Parent);
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
		return (int)(m_Summoner.m_Attr.FullHp * m_Info.m_hppercent * 0.01f);
	}
	/// <summary>
	/// 获取物理伤害
	/// </summary>
	protected override int GetCutPhyDefend()
	{
		return (int)(m_Summoner.m_Attr.CutPhyDefend * m_Info.m_phydamagepercent * 0.01f);
	}

	/// <summary>
	/// 获取物理攻击
	/// </summary>
	protected override int GetPhyAttack()
	{
		return (int)(m_Summoner.m_Attr.PhyAttack * m_Info.m_phyattackpercent * 0.01f);
	}
	/// <summary>
	/// 获取物理防御
	/// </summary>
	protected override int GetPhyDefend()
	{
		return (int)(m_Summoner.m_Attr.PhyDefend * m_Info.m_phydefendpercent * 0.01f);
	}
	/// <summary>
	/// 获取物理暴击
	/// </summary>
	protected override int GetPhyCrit()
	{
		return (int)(m_Summoner.m_Attr.PhyCrit * m_Info.m_phy_critpercent * 0.01f);
	}
	/// <summary>
	/// 获取魔法攻击
	/// </summary>
	protected override int GetMagicAttack()
	{
		return (int)(m_Summoner.m_Attr.MagicAttack * m_Info.m_magicattackpercent * 0.01f);
	}
	/// <summary>
	/// 获取魔法防御
	/// </summary>
	protected override int GetMagicDefend()
	{
		return (int)(m_Summoner.m_Attr.CutMagDefend * m_Info.m_magicdefendpercent * 0.01f);
	}
	/// <summary>
	/// 获取魔法暴击
	/// </summary>
	protected override int GetMagicCrit()
	{
		return (int)(m_Summoner.m_Attr.MagicCrit * m_Info.m_magic_critpercent * 0.01f);
	}


}
