using UnityEngine;
using System.Collections;
/// <summary>
/// 虚化状态
/// </summary>
/// <author>zhulin</author>
public class VirtualStatus : StatusM {

	public override void Init(int SceneID,int ResalseSceneID,int SkillID,SkillStatusInfo Info)
	{
		m_OverData = StatusOverAction.CD; 
		m_AddRole = AddRule.Replace;
		m_type = StatusType.Virtual;
		m_SkillImmune = ImmuneSkill.PhySkill;
		m_InterruptSkill = true;
		m_SelfInterruptSkill = false;
		m_AntiInterruptStatus = AntiInterruptStatus.Phy;
		base.Init(SceneID,ResalseSceneID,SkillID,Info);
	}
}
