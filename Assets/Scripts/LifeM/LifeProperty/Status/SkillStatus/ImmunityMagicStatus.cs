using UnityEngine;
using System.Collections;
/// <summary>
/// 魔法免疫状态
/// </summary>
/// <author>zhulin</author>
public class ImmunityMagicStatus : StatusM {

	public override void Init(int SceneID,int ResalseSceneID,int SkillID,SkillStatusInfo Info)
	{
		m_OverData = StatusOverAction.CD; 
		m_AddRole = AddRule.Replace;
		m_type = StatusType.ImmunityMagic;
		m_SkillImmune = ImmuneSkill.MagicSkill;
		m_InterruptSkill = false;
		m_SelfInterruptSkill = false;
		m_AntiInterruptStatus = AntiInterruptStatus.Magic;
		base.Init(SceneID,ResalseSceneID,SkillID,Info);
	}
}
