using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 建筑物技能
/// </summary>
/// <author>QFord</author>
public class BuildSkill : Skill {

	protected Life m_Parent = null;
	/// <summary>
	/// 技能触发所执行的方法
	/// </summary>
	/// <returns></returns>
	public  delegate bool TriggerSkillFunc();
	/// <summary>
	/// 技能触发所执行的方法
	/// </summary>
	public  TriggerSkillFunc SkillTrrigerHandler;


	public BuildSkill(){}
	
	public BuildSkill( BuildInfo Info)
	{
		if(Info != null )
		{
			Info.ClearCombatData();
			m_skill = Info.m_Skill;
			if(m_skill != null)
			{
				m_CDDuration = m_skill.m_cd*0.001f;
			}
		}
	}

	public override bool Init(int SceneID, LifeMCore Core)
    {
        m_SceneID = SceneID;
		m_Parent = CM.GetLifeM(m_SceneID ,LifeMType.BUILD);
		// by zhulin 屏蔽
		/*BuildInfo Info = CmCarbon.GetBuildInfo(Core.m_DataID);
		if(Info == null || Info.m_Skill == null) 
			return false;
		m_skill = Info.m_Skill;

        m_CDDuration = m_skill.m_cd*0.001f;*/
        m_CdTime = 0;
		return true;
    }


	protected override bool SkillHit(NDAttribute Attack, NDAttribute Defense,SkillInfo skill)
	{
		return true;
	}
	
	/// <summary>
	/// cd技能攻击，第一次攻击处于等待状态，
	/// </summary>
    public override void Update(float deltaTime)
    {
        if (CM.GetLifeM(m_SceneID,LifeMType.BUILD) == null)
			return ;
		m_CdTime -= deltaTime;
		if (m_CdTime <= 0)
		{
			if(SkillTrrigerHandler())
			{
				ReSetCDTime();
			}
			
		}
	}
	
	public bool CheckSkillTrigger(Life target)
	{
		bool bRelease=false;
		List<Life> RoleList = new List<Life>();
		CM.SearchLifeMList(ref RoleList,null,LifeMType.SOLDIER | LifeMType.SUMMONPET ,LifeMCamp.ATTACK,m_Parent, (m_skill as BuildSkillInfo).m_tSearchInfo);

		if (RoleList.Count > 0) 
		{
			foreach(Life l in RoleList)
			{
				if (l is  Role && !CheckCanAttack((l as Role).CurrentAction))
				{
					
				}
				else 
				{
					bRelease=true;
					m_skill.SetTarget(l,l.GetMapGrid());
					m_skill.SetTargetV3Pos(l.GetPos());
					break;
				}
				
			}
		}
		return bRelease;
	}



	public List<Life> GetBuildSkillTarget(Life target)
	{
		List<Life> RoleList = new List<Life>();
		List<Life> newRoleList = new List<Life>();
		CM.SearchLifeMList(ref RoleList,null,LifeMType.SOLDIER | LifeMType.SUMMONPET ,LifeMCamp.ATTACK,m_Parent, (m_skill as BuildSkillInfo).m_dSearchInfo);

		if (RoleList.Count >0)
		{
			CM.SearchAttackLifeMList(ref RoleList,target);
			int nRoleCnt =0;
			int nRoleMaxCount = m_skill.m_multiple;
			if(nRoleMaxCount==0)
				nRoleMaxCount = RoleList.Count;
			for(int i = 0; i < RoleList.Count; i++)
			{
				if (RoleList[i] is  Role && !CheckCanAttack((RoleList[i]  as Role).CurrentAction))
				{
					
				}
				else 
				{
					newRoleList.Add(RoleList[i]);
					nRoleCnt++;
				}
				if(nRoleCnt>=nRoleMaxCount)
					break;
				
			}
		}
		CheckMultiple(ref newRoleList,m_skill);
		return newRoleList;
	}

	/// <summary>
	/// 是否处于CD
	/// </summary>
	public bool IsInCDStatus()
	{
		return (m_CdTime>0);
	}
	public float GetCDTime()
	{
		return m_CdTime;
	}

	/// <summary>
	/// 产生DeBuff
	/// </summary>
	protected override bool StatusDeBuff(Life Defense,SkillInfo skill)
	{
		if(!(Defense is Role) || Defense == null || Defense.m_Attr == null || Defense.m_Status == null)
			return  false;
		if(skill == null || skill.m_attack_status_enemy == null)
			return false;
		
		bool InterruptSkill = false;
		List<SkillStatusInfo>l = skill.m_attack_status_enemy;
		AttackType Type = (AttackType)skill.m_attacktype;
		for(int i = 0; i <l.Count ; i++)
		{
			if (CheckCondition(Defense.m_Attr, l[i]))
			{ 
				if (Defense is Role)
				{
					Role r = Defense as Role;
					
					if(Defense.m_Status.CheckCanAddStatus(m_SceneID ,skill.m_type ,Type,l[i]) == true)
					{
						if(Defense.m_Status.AddStatus(m_SceneID,skill.m_type,l[i]) == true)
						{
							InterruptSkill = true;
						}
					}
				}
			}
		}
		return InterruptSkill;
	}
	public void DoQianYaoStatus(Life target,LifeMCamp camp)
	{
		if (target != null)
		{
			//debuff 给对方增加状态
			if (target.m_Core.m_Camp == camp)
				StatusReleaseBuff(target);
			else
				StatusReleaseDeBuff(target);
		}
	}

	/// <summary>
	/// 产生DeBuff
	/// </summary>
	protected override bool StatusReleaseDeBuff(Life Defense)
	{
		if(m_skill == null ) return false;
		BuildSkillInfo skill = m_skill as BuildSkillInfo ;
		if(!(Defense is Role) || Defense == null || Defense.m_Attr == null || Defense.m_Status == null)
			return  false;
		if( skill.m_releasedenemy_status == null)
			return false;
		
		bool InterruptSkill = false;
		List<SkillStatusInfo>l = skill.m_releasedenemy_status;
		
		for(int i = 0; i <l.Count ; i++)
		{
			if (CheckCondition(Defense.m_Attr, l[i]))
			{
				if(Defense.m_Status.AddStatus(m_SceneID,skill.m_type,l[i]) == true)
				{
					InterruptSkill = true;
				}
			}
		}
		return InterruptSkill;
	}
	
	/// <summary>
	/// 产生Buff
	/// </summary>
	protected override bool StatusReleaseBuff(Life Defense)
	{
		if(m_skill == null ) return false;
		BuildSkillInfo skill = m_skill as BuildSkillInfo ;
		if (!(Defense is Role) || Defense == null || Defense.m_Attr == null || Defense.m_Status == null)
			return false;
		if (m_skill == null || skill.m_releasedown_status == null)
			return false;
		
		bool InterruptSkill = false;
		List<SkillStatusInfo> l = skill.m_releasedown_status;
		
		for (int i = 0; i < l.Count; i++)
		{
			if (CheckCondition(Defense.m_Attr, l[i]))
			{
				if (Defense.m_Status.AddStatus(m_SceneID, skill.m_type, l[i]) == true)
				{
					InterruptSkill = true;
				}
			}
		}
		return InterruptSkill;
	}
}
