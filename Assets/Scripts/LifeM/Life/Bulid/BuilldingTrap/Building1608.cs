using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

/// <summary>
/// 熔炉
/// </summary>
/// <author>TXM</author>
/// <Revisor>QFord</Revisor>
public class Building1608 : Building {
	
	public float m_fAttackTimer;
	private bool m_bAttack=false;
	public GameObject m_goHelp_01;
	protected GameObject m_goEffect1910011;//红色陷阱背景光效  熔炉光晕
	protected GameObject m_goEffect1910021;//红色陷阱背景光效  熔炉光晕
	protected GameObject m_goEffect1911021;//熔浆喷嘴漏岩浆
	protected GameObject m_goEffect1911031;//熔炉背景光晕
	protected GameObject m_goEffect1911011=null;//闪光特效


	
	public override void InitBuildModel()
	{
		base.InitBuildModel ();
		m_goEffect1910011  = GameObjectLoader.LoadPath("effect/prefab/", "1910011", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.skinroot).transform);
		m_goEffect1910021  = GameObjectLoader.LoadPath("effect/prefab/", "1910021", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.skinroot).transform);
		m_goEffect1911021  = GameObjectLoader.LoadPath("effect/prefab/", "1911021", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.skinroot).transform);
		m_goEffect1911031  = GameObjectLoader.LoadPath("effect/prefab/", "1911031", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.skinroot).transform);
		
	}

	public override void InitBuild ()
	{
		base.InitBuild ();
		m_goEffect1910011  = GameObjectLoader.LoadPath("effect/prefab/", "1910011", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.skinroot).transform);
		m_goEffect1910021  = GameObjectLoader.LoadPath("effect/prefab/", "1910021", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.skinroot).transform);
		m_goEffect1911021  = GameObjectLoader.LoadPath("effect/prefab/", "1911021", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.skinroot).transform);
	}
	public override void BuildUpdate()
	{
		base.BuildUpdate ();
		if (m_bAttack&&Time.time - m_fAttackTimer > 3.6f) 
		{
			SetAnimator (Build_AnimatorState.Stand10000);
			m_bAttack=false;
			if (null != m_goEffect1911011)
			{
				GameObject.Destroy (m_goEffect1911011);
				m_goEffect1911011=null;
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
			IsRelease=true;
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
		m_fAttackTimer = Time.time;
		fReleaseDelay = 0.25f;
		m_bAttack=true;
		if (null != m_goEffect1911011)
						GameObject.Destroy (m_goEffect1911011);
		m_goEffect1911011  = GameObjectLoader.LoadPath("effect/prefab/", "1911011", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.skinroot).transform);
		m_goEffect1911011.SetActive (true);
		SetAnimator (Build_AnimatorState.Trigger30000);
		
	}
	public override void Hit(int damage)
	{
	}
	public override void Shake()
	{
		if(!isDead)
			m_thisT.DOShakePosition( 0.5f);
	}
	public override void Destroy()
	{
		base.Destroy();
		m_goEffect1910011.SetActive (false);
		m_goEffect1910021.SetActive (false);
		if (null != m_goEffect1911011)
		{
			GameObject.Destroy (m_goEffect1911011);
			m_goEffect1911011=null;
		}
	}

	public override void SetDark(bool bDark)
	{
		base.SetDark(bDark);
		if (m_bDark) 
		{
			m_goEffect1910011.SetActive(false);
			m_goEffect1910021.SetActive(false);
			m_goEffect1911021.SetActive(false);
		} 
		else 
		{
			m_goEffect1910011.SetActive(true);
			m_goEffect1910021.SetActive(true);
			m_goEffect1911021.SetActive(true);
			
		}
	}
}