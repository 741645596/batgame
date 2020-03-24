using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// 地刺房
/// </summary>
/// <author>TXM</author>
/// <Revisor>QFord</Revisor>
public class Building1601 : Building {

	bool m_bAttack=false;
	float m_fAttackTime=0;
	protected GameObject m_goEffect1918011;//地刺毒气
	public override void InitBuildModel()
	{
		base.InitBuildModel ();
		m_goEffect1918011 = GameObjectLoader.LoadPath("effect/prefab/", "1918011",m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.skinroot).transform);

	}
	public override void InitBuild ()
	{
		base.InitBuild ();
	}
	public override void BuildUpdate ()
	{
		base.BuildUpdate ();
		if (m_bAttack && Time.time - m_fAttackTime > 1.0f) 
		{
			SetAnimator (Build_AnimatorState.Stand10000);
			m_bAttack=false;
		}
	}
	/// <summary>	
	/// 释放技能主逻辑
	/// </summary>
	protected override bool ReleaseSkill(ref List<Life> RoleList,ref int nAttackIndex)
	{
		bool IsRelease = false;
		if (RoleList.Count > 0)
		{
			IsRelease=true;
			SoundPlay.Play("Trap/trap_sunkens",false,false);
			foreach(Life w in RoleList)
			{
				bool InterruptSkill = false;
				GridActionCmd action = null;
				if (w is Role)
					action = (w as Role).CurrentAction;
				SkillReleaseInfo Info = m_Skill.SkillRelease(this,w,action,m_Skill.PropSkillInfo);
				w.ApplyDamage(Info, m_thisT);
			}
		}
		return IsRelease ;
	}
	protected override void TriggerSkillTime(ref List<Life> RoleList,ref float fReleaseDelay)
	{
		m_bAttack = true;
		fReleaseDelay = 0.25f;
		m_fAttackTime = Time.time;
		if (RoleList.Count > 0)
		{
			SetAnimator (Build_AnimatorState.Trigger30000);
			//SoundPlay.Play("Trap/trap_sunkens",false,false);
		}
		
	}
	public override void Hit(int damage)
	{
	}

	public override void Shake()
	{
		if(!isDead)
		 m_thisT.DOShakePosition(0.5f);
	}
     
}