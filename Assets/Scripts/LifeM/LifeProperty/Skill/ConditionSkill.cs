using UnityEngine;
using System.Collections;

/// <summary>
/// 条件技能
/// </summary>
/// <author>zhulin</author>
public class ConditionSkill : SkillAI {




	public  override void Update (float deltaTime ,float AttackSpeed) 
	{
		if (m_CdTime > 0)
		{
			m_CdTime -= deltaTime * AttackSpeed;
			if (m_CdTime <= 0)
			{
				//释放技能
				m_CdTime = m_TimeCount;
				m_Relsease = true;
			}
		}
	}
	
}
