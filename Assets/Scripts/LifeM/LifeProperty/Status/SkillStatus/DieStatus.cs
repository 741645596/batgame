using UnityEngine;
using System.Collections;

public class DieStatus : StatusM {

	
	public override void Init(int SceneID,int ResalseSceneID,int SkillID,SkillStatusInfo Info)
	{
		m_OverData = StatusOverAction.CD; 
		m_AddRole = AddRule.Replace;
		m_type = StatusType.Die;
		m_SkillImmune = ImmuneSkill.Normal;
		m_InterruptSkill = false;
		m_SelfInterruptSkill = false;
		m_AntiInterruptStatus = AntiInterruptStatus.Normal;
		base.Init(SceneID,ResalseSceneID,SkillID,Info);
	}

	/// <summary>
	/// 持续状态属性
	/// </summary>
	/// <author>zhulin</author>
	public override void DoSkillStatus(float duration)
	{
		if (duration > 0)
		{
			Life life = CM.GetAllLifeM(m_SceneID, LifeMType.ALL);
			
			life.Dead();
		}
	}
}
