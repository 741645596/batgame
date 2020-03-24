using UnityEngine;
using System.Collections;
/// <summary>
/// 魔法防打断状态
/// </summary>
/// <author>zhulin</author>
public class MagAnitInterruptStatus : StatusM {

	public override void Init(int SceneID,int ResalseSceneID,int SkillID,SkillStatusInfo Info)
	{
		m_OverData = StatusOverAction.CD | StatusOverAction.MagicHurt; 
		m_AddRole = AddRule.Replace;
		m_type = StatusType.MagAnitInterrupt;
		m_SkillImmune = ImmuneSkill.Normal;
		m_InterruptSkill = false;
		m_SelfInterruptSkill = false;
		m_AntiInterruptStatus = AntiInterruptStatus.Magic;
		base.Init(SceneID,ResalseSceneID,SkillID,Info);
	}
}
