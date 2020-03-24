using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 免疫状态
/// </summary>
/// <author>zhulin</author>
public class ImmuneState : StatusM {

	public override void Init(int SceneID,int ResalseSceneID,int SkillID,SkillStatusInfo Info)
	{
		m_OverData = StatusOverAction.CD; 
		m_AddRole = AddRule.Replace;
		m_type = StatusType.ImmuneState;
		m_SkillImmune = ImmuneSkill.Normal;
		m_InterruptSkill = false;
		m_SelfInterruptSkill = false;
		m_AntiInterruptStatus = AntiInterruptStatus.Magic | AntiInterruptStatus.Phy;
		base.Init(SceneID,ResalseSceneID,SkillID,Info);
	}

	
	/// <summary>
	/// 技能免疫判断
	/// </summary>
	/// <param name="AttackSkillType">攻击方技能类型</param>
	/// <param name="SkillType">免疫特定技能</param>
	/// <param name="DefanseStatus">防御方状态</param>
	/// <returns> true 免疫该技能， false 不免疫该技能</returns>
	public override bool ImmunitySkill(AttackType AttackSkillType ,int SkillType)
	{
		List<int>lData = new List<int>();
		foreach(StatusAction  skill in m_StatusData)
		{
			if(skill != null)
			{
				skill.GetAttrData(ref lData );
			}
		}
		//包含该技能，免疫
		if(lData.Contains (SkillType))
			return true;


		return base.ImmunitySkill(AttackSkillType ,SkillType);
	}
}
