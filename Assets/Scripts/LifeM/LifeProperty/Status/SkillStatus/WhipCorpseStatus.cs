using UnityEngine;
using System.Collections;

/// <summary>
/// 鞭尸状态
/// </summary>
/// <author>huangbufu</author>
public class WhipCorpseStatus : StatusM
{
	
	public override void Init(int SceneID,int ResalseSceneID,int SkillID,SkillStatusInfo Info)
	{
		m_OverData = StatusOverAction.CD; 
		m_AddRole = AddRule.Replace;
        m_type = StatusType.WhipCorpse;
		m_SkillImmune = ImmuneSkill.Normal;
		m_InterruptSkill = false;
		m_SelfInterruptSkill = false;
		base.Init(SceneID,ResalseSceneID,SkillID,Info);
	}
	
}