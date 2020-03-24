using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

/// <summary>
/// 牢笼
/// </summary>
/// <author>zhulin</author>
public class Building1603 : Building {
	
	public float m_fAttackTimer;//上一次技能触发时间
	private float m_fAttackStep_Secs=0f;
	bool m_TriggerIng;
	//private List<Life> m_roleListTarget = new List<Life>();
	private BuildSkillInfo m_infoSkil;
	public override void InitBuildModel()
	{
		base.InitBuildModel ();
	}
	// Use this for initialization
	public override void  InitBuild(){
		base.InitBuild();
		m_TriggerIng = false;
		m_infoSkil = m_Skill.PropSkillInfo as BuildSkillInfo;
		//m_fAttackStep_Secs = m_infoSkil.m_step_secs / 1000.0f;
	}
	public override void BuildUpdate()
	{
		if(m_fAttackTimer>0)
		{
			if (m_TriggerIng) 
			{
				float fTimePass = Time.time-m_fAttackTimer;
				if(fTimePass>=m_fAttackStep_Secs)
				{
					SetAnimator (Build_AnimatorState.Die20100);
					m_TriggerIng=false;
				}
			}
			if(Time.time-m_fAttackTimer>m_fAttackStep_Secs+1.0f&&!isDead)
			{
				m_fAttackTimer=0.0f;
				SetAnimator (Build_AnimatorState.Stand10000);
			}
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
			m_fAttackTimer = Time.time;
			IsRelease=true;
			m_TriggerIng=true;
			SetAnimator (Build_AnimatorState.Trigger30000);
			SoundPlay.Play("Trap/trap_prison",false,false);
			foreach(Life w in RoleList)
			{
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
		
	}
	public override void Hit(int damage)
	{
		if (!m_TriggerIng)
				return;
		//base.Hit(damage);
	}
	public override void Shake()
	{
		if(!isDead)
			m_thisT.DOShakePosition(0.5f);
	}
}
