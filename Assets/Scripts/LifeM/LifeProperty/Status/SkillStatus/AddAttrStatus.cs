using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 加属性状态
/// </summary>
/// <author>zhulin</author>
public class AddAttrStatus : StatusM {

	public override void Init(int SceneID,int ResalseSceneID,int SkillID,SkillStatusInfo Info)
	{
		m_OverData = StatusOverAction.CD; 
		m_AddRole = AddRule.ADD;
		m_type = StatusType.AddAttr;
		m_SkillImmune = ImmuneSkill.Normal;
		m_InterruptSkill = false;
		m_SelfInterruptSkill = false;
		base.Init(SceneID,ResalseSceneID,SkillID,Info);
	}

	/// <summary>
	/// 获取属性数据
	/// </summary>
	public override int GetAttrData(EffectType type)
	{		
		List<int>lData = new List<int>();
		for(int i = 0; i < m_StatusData.Count ; i++)
		{
			StatusAction  skill = m_StatusData[i];
			if(skill != null && skill.GetPersistence () == false)
			{
				skill.GetAttrData(ref lData );
			}
		}
		return GetAttrData(lData , type);
	}
	
	/// <summary>
	/// 持续状态属性
	/// </summary>
	/// <author>zhulin</author>
	public override void DoSkillStatus(float duration)
	{
		if(m_StatusData == null || m_StatusData.Count ==0)
			return ;
		for(int j = 0; j < m_StatusData.Count; j++ )
		{
			StatusAction skill = m_StatusData[j];
			if(skill != null)
			{
				List<int>lData = new List<int>();
				if(skill.GetPersistence () == true)
				{
					skill.GetAttrData(ref lData);
				}
				else
				{
					List<int>l = new List<int>();
					skill.GetAttrData(ref l);
					for(int i = 0; i <l.Count -1; i += 2 )
					{
						EffectType Type = (EffectType)l[i];
						if(Type == EffectType.RecoHp || Type == EffectType.RecoAnger || Type == EffectType.IcePoint )
						{
							//是否要调用，可能会产生添加怒气
							lData.Add(l[i]);
							lData.Add(l[i+1]);
						}
					}
				}
				if(skill.DoSkillStatus(duration)== true && lData.Count > 0)
				{
					//Debug.Log(" 加血  " + duration + "," + skill.Duration);
					AddrContinuedAttr(lData);
				}
			}
		}
	}
}
