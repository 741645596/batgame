using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

/// <summary>
/// 压扁房
/// </summary>
/// <author>zhulin</author>
public class Building1607 : Building {
	
	public float m_fAttackTimer;
	public bool m_bAttackedToDie=false;
	public bool m_bAttacke = false;
	//protected GameObject m_goEffect1914011;
	protected GameObject m_goEffect1914021;
	
	private bool m_bCDOver=true;
	public override void InitBuildModel()
	{
		base.InitBuildModel ();
		/*m_goEffect1914011  = GameObjectLoader.LoadPath("effect/prefab/", "1914011", m_thisT);
		m_goEffect1914011.SetActive (true);*/
		
		m_goEffect1914021  = GameObjectLoader.LoadPath("effect/prefab/", "1914021", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.boneroot).transform);
		m_goEffect1914021.SetActive (false);
		m_eventtime = 0.2f;
		
	}
	public override void  InitBuild() {
		base.InitBuild();

		
	}
	public override void BuildUpdate()
	{
		base.BuildUpdate ();
		if (m_bAttackedToDie && Time.time - m_fAttackTimer > 0.7f) 
		{
			Debug.Log("add m_bAttackedToDie .........................." + Time.time);
			m_bAttackedToDie=false;
			SkillReleaseInfo sInfo = new SkillReleaseInfo(); 
			sInfo.m_Damage = -fullHP * 2 ;
			sInfo.Result = AttackResult.Normal;
			ApplyDamage(sInfo,null);
		}
		if (m_bAttacke &&TimerCheckAttackOver()) 
		{
			m_bAttacke=false;
			m_goEffect1914021.SetActive(false);
		}
		if (!m_bCDOver&&GetCDTime()<2f) 
		{
			m_bCDOver=true;
			SetAnimator (Build_AnimatorState.CDOver40100);
		}



	}
	public bool TimerCheckAttackOver()
	{
		if (Time.time - m_fAttackTimer < 1.5f)
						return false;
		return true;

	}
	public bool TimeCheckHitEffectTimerOver()
	{
		return false;
	}
	public override void Hit(int damage)
	{
	}
	protected override bool QianYaoSkill (ref List<Life> RoleList, ref int nAttackIndex)
	{
		foreach(Life lifepress in RoleList)
		{
			(m_Skill as BuildSkill).DoQianYaoStatus(lifepress,m_Core.m_Camp);
		}
		return true;
	}
	/// <summary>	
	/// 释放技能主逻辑
	/// </summary>
	protected override bool ReleaseSkill(ref List<Life> RoleList,ref int nAttackIndex)
	{
		bool IsRelease = false;
		if (RoleList.Count > 0)
		{
			m_bCDOver=false;
			
			m_fAttackTimer = Time.time;

			IsRelease=true;
			bool ispress = true;//是否可以压扁
			/*{
				foreach(Life life in RoleList)
				{
					if (life.m_Attr.AntiPress > 100)
					{
						ispress = false;
						GridActionCmd action = null;
						if (life is Role)
							action = (life as Role).CurrentAction;
						SkillReleaseInfo Info = m_Skill.SkillRelease(this,life,action,m_Skill.PropSkillInfo);
						//life.ApplyDamage(Info, transform);
						SetAnimator (Build_AnimatorState.Trigger30100);
						m_bAttackedToDie=true;
					}
				}
			}*/
			if (ispress)
			{
				foreach(Life lifepress in RoleList)
				{
					GridActionCmd action = null;
					if (lifepress is Role)
						action = (lifepress as Role).CurrentAction;
					SkillReleaseInfo Info = m_Skill.SkillRelease(this,lifepress,action,m_Skill.PropSkillInfo);
					lifepress.ApplyDamage(Info, m_thisT);
				}
				SetAnimator (Build_AnimatorState.Trigger30000);
				m_bAttacke=true;

				SoundPlay.Play("Trap/trap_flattening",false,false);
				m_goEffect1914021.SetActive(true);
			}
		}
		return IsRelease ;
	}
	protected override void TriggerSkillTime(ref List<Life> RoleList,ref float fReleaseDelay)
	{
		
	}
	public bool TimeCheckAttackOver()
	{
		if (Time.time - m_fAttackTimer <= 1.5f)
			return false;
		return true;
	}
	public override void  Destroy()
	{
		base.Destroy ();
		//m_goEffect1914011.SetActive (false);
		m_goEffect1914021.SetActive (false);
		
	}
	
	public override void Shake()
	{
		if(!isDead  && m_thisT != null)
			m_thisT.DOShakePosition(0.5f);
	}

	protected override void SetSkillStatus(StatusInfo Info )
	{
		base.SetSkillStatus(Info);
		if(Info == null) return ;

		if (Info.Type == StatusType.BuildDestory)
		{
			
			//Debug.Log("add BuildDestory .........................." + Time.time);
			SetAnimator (Build_AnimatorState.Trigger30100);
			m_bAttackedToDie=true;
			m_fAttackTimer = Time.time;
			m_bTriggerSkill = false;
		}
		
	}

}
