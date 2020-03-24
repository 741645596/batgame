using UnityEngine;
using System.Collections;

/// <summary>
/// 眩晕状态
/// </summary>
/// <author>zhulin</author>
public class VertigoStatus : StatusM {

	public override void Init(int SceneID,int ResalseSceneID,int SkillID,SkillStatusInfo Info)
	{
		m_OverData = StatusOverAction.CD; 
		m_AddRole = AddRule.ADD;
		m_type = StatusType.Vertigo;
		m_SkillImmune = ImmuneSkill.Normal;
		m_InterruptSkill = true;
		m_SelfInterruptSkill = false;
		base.Init(SceneID,ResalseSceneID,SkillID,Info);
	}

}
