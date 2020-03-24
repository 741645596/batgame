using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class Building1619 : Building {
	
	private bool m_bAttack=false;
	//private bool m_bAttackSound = true;
	private float m_fAttackTime;

	
	
	private float m_fCheckRoleInSmallCirle;

	private GameObject m_goEffect1916011;
	private GameObject m_goEffect1916021_01;
	private GameObject m_goEffect1916021_02;
	
	public override void InitBuildModel()
	{
		base.InitBuildModel ();
		m_goEffect1916011  = GameObjectLoader.LoadPath("effect/prefab/", "1916011", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.skinroot).transform);
		m_goEffect1916011.SetActive (true);
		
		m_goEffect1916021_01  = GameObjectLoader.LoadPath("effect/prefab/", "1916021", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.help01).transform);
		m_goEffect1916021_01.SetActive (true);
		m_goEffect1916021_02  = GameObjectLoader.LoadPath("effect/prefab/", "1916021", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.help02).transform);
		m_goEffect1916021_02.SetActive (true);
	}
	public override void InitBuild ()
	{
		base.InitBuild ();
		
	}
	public override void BuildUpdate()
	{
		base.BuildUpdate ();
		
	}

	protected override void TriggerSkillTime(ref List<Life> RoleList,ref float fReleaseDelay)
	{
		
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
				GridActionCmd action = null;
				if (w is Role)
					action = (w as Role).CurrentAction;
				SkillReleaseInfo Info = m_Skill.SkillRelease(this,w,action,m_Skill.PropSkillInfo);
				w.ApplyDamage(Info, m_thisT);
			}
			m_bAttack=true;
			m_fAttackTime = Time.time;
		}
		return IsRelease ;
	}
	public override void Hit(int damage)
	{
	}
	
	public override void Shake()
	{
		if(!isDead)
			m_thisT.DOShakePosition(0.5f);
	}
	public override void Destroy()
	{
		base.Destroy();
	}
	
	public override void PlayClickAni()
	{
		base.PlayClickAni();
	}
}
