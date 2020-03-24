using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 静电状态
/// </summary>
/// <author>zhulin</author>
public class StaticElecStatus : StatusM {

	public override void Init(int SceneID,int ResalseSceneID,int SkillID,SkillStatusInfo Info)
	{
		m_OverData = StatusOverAction.CD; 
		m_AddRole = AddRule.Replace;
		m_type = StatusType.StaticElec;
		m_SkillImmune = ImmuneSkill.Normal;
		m_InterruptSkill = true;
		m_SelfInterruptSkill = true;
		base.Init(SceneID,ResalseSceneID,SkillID,Info);
	}	

	/// <summary>
	/// 持续触发麻痹状态
	/// </summary>
	/// <author>zhulin</author>
	public override void DoSkillStatus(float duration)
	{
		Life life = CM.GetAllLifeM(m_SceneID, LifeMType.ALL);
		if(life == null) return ;
		if(m_StatusData == null || m_StatusData.Count ==0)
			return ;
		for(int j = 0; j < m_StatusData.Count; j++ )
		{
			StatusAction skill = m_StatusData[j];
			if(skill != null)
			{
				if(skill.DoSkillStatus(duration)== true )
				{
					life.m_Attr.Paralysis = true;
				}
			}
		}


	}
}
