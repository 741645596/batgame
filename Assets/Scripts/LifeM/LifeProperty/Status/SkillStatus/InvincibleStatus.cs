using UnityEngine;
using System.Collections;

/// <summary>
/// 无敌状态
/// </summary>
/// <author>zhulin</author>
public class InvincibleStatus : StatusM {

	public override void Init(int SceneID,int ResalseSceneID,int SkillID,SkillStatusInfo Info)
	{
		m_OverData = StatusOverAction.CD; 
		m_AddRole = AddRule.Replace;
		m_type = StatusType.Invincible;
		m_SkillImmune = ImmuneSkill.Invincible;
		m_InterruptSkill = false;
		m_SelfInterruptSkill = false;
		m_AntiInterruptStatus = AntiInterruptStatus.Magic | AntiInterruptStatus.Phy;
		base.Init(SceneID,ResalseSceneID,SkillID,Info);
	}
}
