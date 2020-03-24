using UnityEngine;
using System.Collections;
/// <summary>
/// 击飞状态
/// </summary>
/// <author>zhulin</author>
public class ClickFlyStatus : StatusM {

	public override void Init(int SceneID,int ResalseSceneID,int SkillID,SkillStatusInfo Info)
	{
		m_OverData = StatusOverAction.CD; 
		m_AddRole = AddRule.Replace;
		m_type = StatusType.ClickFly;
		m_SkillImmune = ImmuneSkill.Invincible;
		m_InterruptSkill = true;
		m_SelfInterruptSkill = false;
		base.Init(SceneID,ResalseSceneID,SkillID,Info);
	}
}
