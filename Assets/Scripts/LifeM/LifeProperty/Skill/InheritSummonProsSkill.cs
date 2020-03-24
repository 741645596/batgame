using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 建筑物技能
/// </summary>
/// <author>QFord</author>
public class InheritSummonProsSkill : Skill {

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


	public InheritSummonProsSkill(){}

	public InheritSummonProsSkill(PetInfo Info, Life skillowner)
	{
		if(Info != null )
		{
			m_Parent = skillowner;
			m_skill = Info.m_skillinfo1;
			if(m_skill != null)
			{
				m_CDDuration = m_skill.m_cd*0.001f;
			}
		}
	}

	public override bool Init(int SceneID, LifeMCore Core)
    {
        m_SceneID = SceneID;
		//m_Parent = CM.GetLifeM(m_SceneID ,LifeMType.INHERITSUMMONPROS);
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
		m_CdTime -= deltaTime;
		if (m_CdTime <= 0 && (m_skill as SoldierSkill).m_condition == 0)
		{
			DoSkill();
			ReSetCDTime();
			
		}
	}

	public void CheckConditionSkill()
	{
		if ( (m_skill as SoldierSkill).m_condition == (int)SkillCondition.BoomDie)
		{
			if (m_Parent.isDead)
			{
				DoSkill();
				ReSetCDTime();
			}
		}
	}
	public void DoSkill()
	{
		List<Life> targets = GetBuildSkillTarget();
		if (targets.Count > 0)
		{
			foreach (Life t in targets)
			{
				
				GridActionCmd action = null;
				if (t is Role)
					action = (t as Role).CurrentAction;
				SkillReleaseInfo Info = SkillRelease(m_Parent, t, action,m_skill);
				t.ApplyDamage(Info, null);
			}
		}
	}




	public List<Life> GetBuildSkillTarget()
	{
		List<Life> RoleList = new List<Life>();
		LifeMCamp camp = GetSkillCamp(m_skill, m_Parent);
		SoldierSkill skill = m_skill as SoldierSkill;
		if ((skill.m_damagetargettype & (int)TargetType.Soldier) == (int)TargetType.Soldier)
		{
			List<Life> lr = new List<Life>();
			CM.SearchLifeMListInBoat(ref lr, LifeMType.SOLDIER, camp);
			foreach (Role r in lr)
			{
				if (CheckCanAttack(r.CurrentAction) && CheckRangeAttackTarget(r, m_Parent.GetMapGrid(), skill.m_sort, skill.m_range, WalkDir.WALKSTOP))
					RoleList.Add(r);
			}
		}
		if ((skill.m_damagetargettype & (int)TargetType.Pet) == (int)TargetType.Pet)
		{
			List<Life> lp = new List<Life>();
			CM.SearchLifeMListInBoat(ref lp, LifeMType.SUMMONPET, camp);
			foreach (Role r in lp)
			{
				if (CheckCanAttack(r.CurrentAction) && CheckRangeAttackTarget(r, m_Parent.GetMapGrid(), skill.m_sort, skill.m_range, WalkDir.WALKSTOP))
					RoleList.Add(r);
			}
		}
		/*int distant = skill.m_distance / MapGrid.m_Pixel;
		if ((skill.m_targettype & (int)TargetType.Soldier) == (int)TargetType.Soldier)
		{
			List<Life> l = new List<Life>();
			///需要排除隐形 。。。。。。。。。。。。。。。。。。
			CM.SearchLifeMListInBoat(ref l, LifeMType.SOLDIER, camp);
			foreach (Role r in l)
			{


				if ( CheckAttackTarget(r, skill.m_sort, distant, skill.m_condition != (int)SkillCondition.SameLayer) )
					RoleList.Add(r);
			}
		}
		if ((skill.m_targettype & (int)TargetType.Pet) == (int)TargetType.Pet)
		{
			List<Life> lp = new List<Life>();
			CM.SearchLifeMListInBoat(ref lp, LifeMType.SUMMONPET, camp);
			foreach (Life p in lp)
			{

				 if (CheckAttackTarget(p, skill.m_sort, distant, skill.m_condition != (int)SkillCondition.SameLayer) )
					 RoleList.Add(p);
			}
		}
		CheckMultiple(ref RoleList, m_skill);*/
		return RoleList;
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
