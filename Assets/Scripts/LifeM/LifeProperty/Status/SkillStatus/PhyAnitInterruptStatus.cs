using UnityEngine;
using System.Collections;

/// <summary>
/// 物理防打断状态
/// </summary>
/// <author>zhulin</author>
public class PhyAnitInterruptStatus : StatusM {

	public override void Init(int SceneID,int ResalseSceneID,int SkillID,SkillStatusInfo Info)
	{
		m_OverData = StatusOverAction.CD | StatusOverAction.PhyHurt; 
		m_AddRole = AddRule.Replace;
		m_type = StatusType.PhyAnitInterrupt;
		m_SkillImmune = ImmuneSkill.Normal;
		m_InterruptSkill = false;
		m_SelfInterruptSkill = false;
		m_AntiInterruptStatus = AntiInterruptStatus.Phy;
		base.Init(SceneID,ResalseSceneID,SkillID,Info);
	}
}
