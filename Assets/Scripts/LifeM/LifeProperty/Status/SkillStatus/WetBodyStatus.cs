using UnityEngine;
using System.Collections;

/// <summary>
/// 落水失身状态
/// </summary>
/// <author>zhulin</author>
public class WetBodyStatus : StatusM {
	
	public override void Init(int SceneID,int ResalseSceneID,int SkillID,SkillStatusInfo Info)
	{
		m_OverData = StatusOverAction.CD; 
		m_AddRole = AddRule.Replace;
		m_type = StatusType.WetBody;
		m_SkillImmune = ImmuneSkill.Normal;
		m_InterruptSkill = false;
		m_SelfInterruptSkill = false;
		base.Init(SceneID,ResalseSceneID,SkillID,Info);
	}
	
}