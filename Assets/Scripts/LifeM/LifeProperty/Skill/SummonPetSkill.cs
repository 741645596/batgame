#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#define UNITY_EDITOR_LOG
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SummonPetSkill: RoleSkill  {

	/// <summary>
	/// 初始化技能数据，从数据中心获取所有技能数据
	/// </summary>
	public override bool Init(int SceneID,LifeMCore Core)
	{
		this.m_SceneID = SceneID;
		m_SkillOwner = CM.GetLifeM(m_SceneID,LifeMType.SUMMONPET) as SummonPet;
		SummonpetInfo Info = SummonM.GetSummonPetInfo(Core.m_DataID); 
		if(Info == null)
			return false;
		//SoldierSkillInfo Info = info.m_Skillinfo;
		m_attack1 = "0";
		m_attack2 ="0";
		for(int i = 0; i<= 7; i++)
		{
			if (i < Info.m_Skillinfo.Count)
			{
				if (Info.m_Skillinfo[i].m_attacktype != (int)AttackType.PassiveCondition && Info.m_Skillinfo[i].m_condition != 0 && Info.m_Skillinfo[i].m_enable == true)
				{
					m_ConditionSkill.Add(Info.m_Skillinfo[i]);
					m_ConditionSkillTimeCount.Add(0f);
		
				}
				m_AllSkill.Add(Info.m_Skillinfo[i]);
				Info.m_Skillinfo[i].m_LifeTarget = null;
				Info.m_Skillinfo[i].m_TargetPos  = null;
			}
			else
			{
				m_AllSkill.Add(new SoldierSkill());
			}
		}
		GetNextSkill();
		m_CdTime = m_CDDuration;
		m_CDDuration = m_AllSkill[0].m_cd ;
		m_Release = false;
		return true;
	}
	protected override  int GetNextSkillOrderID()
	{
		return 0;
	}
}


