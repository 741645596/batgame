using UnityEngine;
using System.Collections;
/// <summary>
/// 魅惑状态
/// </summary>
/// <author>zhulin</author>
public class CharmStatus : StatusM {
	
	public override void Init(int SceneID,int ResalseSceneID,int SkillID,SkillStatusInfo Info)
	{
		m_OverData = StatusOverAction.CD; 
		m_AddRole = AddRule.Replace;
		m_type = StatusType.Charm;
		m_SkillImmune = ImmuneSkill.Normal;
		m_InterruptSkill = false;
		m_SelfInterruptSkill = false;
		m_AntiInterruptStatus = AntiInterruptStatus.Normal;
		base.Init(SceneID,ResalseSceneID,SkillID,Info);
	}
	
}