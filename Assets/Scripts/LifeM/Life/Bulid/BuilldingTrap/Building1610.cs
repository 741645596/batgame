using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

/// <summary>
/// 对空炮
/// </summary>
/// <author>TXM</author>
/// <Revisor>QFord</Revisor>
public class Building1610 : Building {
	
	protected GameObject m_goEffect1912011;
	private Vector3 m_vDirGreed;
	private Vector3 m_vCurGreed;
	private bool m_bAttack=false;
	private float m_fAttackTime;
	private GameObject m_goPao;
	public override void InitBuildModel()
	{
		base.InitBuildModel ();
		m_goEffect1912011  = GameObjectLoader.LoadPath("effect/prefab/", "1912011", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.help01).transform);
		m_goEffect1912011.SetActive (false);
		m_goPao=m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.help02);
		m_goPao.transform.localRotation = Quaternion.Euler(new Vector3(0,-45f,0));
		m_vDirGreed =  new Vector3 (0, -45f, 0);
		m_vCurGreed = new Vector3 (0, -45f, 0);
		
	}
	public override void InitBuild ()
	{
		base.InitBuild ();


	}
	public override void BuildUpdate()
	{
		base.BuildUpdate ();
		int nFlag = 0;
		m_vCurGreed = m_vDirGreed;
		m_goPao.transform.localRotation = Quaternion.Euler (m_vCurGreed);
		if (m_bAttack && Time.time - m_fAttackTime>0.5f) 
		{
			m_bAttack=false;
			m_goEffect1912011.SetActive(false);
			SetAnimator (Build_AnimatorState.Stand10000);
		}
		
	}
	protected override void TriggerSkillTime(ref List<Life> RoleList,ref float fReleaseDelay)
	{
		fReleaseDelay = 0.25f;
		bool IsRelease = false;
		if (RoleList.Count > 0)
		{
			SetAnimator (Build_AnimatorState.Trigger30000);
			{
				Life w = RoleList[0];
				Vector3 posSrc = m_goPao.transform.position;
				Vector3 posRole = new Vector3(w.GetPos().x,w.GetPos().y,w.GetPos().z);
				float fgreed = NdUtil.V2toAngle(posSrc,posRole,Vector3.right);
				if(fgreed<0f||fgreed>270)
					fgreed=0f;
				else if(fgreed>180f)
					fgreed=180f;
				m_vDirGreed =  new Vector3 (0, -fgreed,0);
			}
			m_bAttack=true;
			m_fAttackTime = Time.time;
			m_goEffect1912011.SetActive(true);
			SoundPlay.Play ("trap_cannon_sky",false,false);
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
			{
				Life w = RoleList[0];
				Vector3 posSrc = m_goPao.transform.position;
				Vector3 posRole = new Vector3(w.GetPos().x,w.GetPos().y,w.GetPos().z);
				float fgreed = NdUtil.V2toAngle(posSrc,posRole,Vector3.right);
				if(fgreed<0f||fgreed>270)
					fgreed=0f;
				else if(fgreed>180f)
					fgreed=180f;
				m_vDirGreed =  new Vector3 (0, -fgreed,0);
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
		m_goEffect1912011.SetActive (false);
	}

	public override void PlayClickAni()
    {
		base.PlayClickAni();
        ShowAttackRangeSemi(true);//对空炮 使用半圆攻击特效
    }
}