using UnityEngine;
using System.Collections;
/// <summary>
/// 吸血状态
/// </summary>
/// <author>zhulin</author>
public class VampireStatus : StatusM {
	
	public override void Init(int SceneID,int ResalseSceneID,int SkillID,SkillStatusInfo Info)
	{
		m_OverData = StatusOverAction.CD; 
		m_AddRole = AddRule.ADD;
		m_type = StatusType.Vampire;
		m_SkillImmune = ImmuneSkill.Normal;
		m_InterruptSkill = false;
		m_SelfInterruptSkill = false;
		base.Init(SceneID,ResalseSceneID,SkillID,Info);
	}
}