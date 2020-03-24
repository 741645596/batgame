using UnityEngine;
using System.Collections;
/// <summary>
/// KTV状态
/// </summary>
/// <author>zhulin</author>
public class KTVStatus : StatusM {

	public override void Init(int SceneID,int ResalseSceneID,int SkillID,SkillStatusInfo Info)
	{
		m_OverData = StatusOverAction.CD; 
		m_AddRole = AddRule.Replace;
		m_type = StatusType.KTV;
		m_SkillImmune = ImmuneSkill.Normal;
		m_InterruptSkill = true;
		m_SelfInterruptSkill = false;
		base.Init(SceneID,ResalseSceneID,SkillID,Info);
	}
}
