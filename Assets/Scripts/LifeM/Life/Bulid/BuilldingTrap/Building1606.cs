using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

/// <summary>
/// 红外电击房
/// </summary>
/// <author>TXM</author>
public class Building1606 : Building
{
	private GameObject m_goEffectLeft1905021_03;
	private GameObject m_goEffectRight1905021_03;
	private GameObject m_goEffect1905011;
	public float m_fAttackTimer;
	public float m_fHitEffectTimer;//上一次受击时间

	private List<Life> m_AttackRoleList = new List<Life>();
	private List<SkillReleaseInfo> m_SkillReleaseInfoList = new List<SkillReleaseInfo>();
	private bool m_bAttack=false;

	
	
	public override void InitBuildModel()
	{
		base.InitBuildModel ();
		m_goEffect1905011 =  GameObjectLoader.LoadPath ("effect/prefab/", "1905011", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.help01).transform);
		
		m_goEffectLeft1905021_03 =  GameObjectLoader.LoadPath ("effect/prefab/", "1905021_03", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.help02).transform);
		m_goEffectLeft1905021_03.transform.localScale = new Vector3 (0.1f, 1.0f, 1.0f);
		m_goEffectRight1905021_03 =  GameObjectLoader.LoadPath ("effect/prefab/", "1905021_03", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.help03).transform);
		m_goEffectRight1905021_03.transform.localScale = new Vector3 (0.1f, 1.0f, 1.0f);
		m_fHitEffectTimer = 0;
		
	}

	public override void  InitBuild() {
		base.InitBuild();
	}
	public override void BuildUpdate()
	{
		base.BuildUpdate ();

		if (m_bAttack&&TimeCheckAttackOver ()) {
			m_goEffectLeft1905021_03.transform.localScale = new Vector3 (0.1f, 1.0f, 1.0f);
			m_goEffectRight1905021_03.transform.localScale = new Vector3 (0.1f, 1.0f, 1.0f);
			m_bAttack=false;
		}
	}
	public override void Hit(int damage)
	{
		base.Hit(damage);
		m_fHitEffectTimer = Time.time;
	}
	/// <summary>
	/// 释放技能主逻辑
	/// </summary>
	protected override bool ReleaseSkill(ref List<Life> RoleList,ref int nAttackIndex)
	{

		bool IsRelease = false;
		m_AttackRoleList.Clear();
		m_SkillReleaseInfoList.Clear ();
		if (RoleList.Count > 0)
		{
			if(nAttackIndex==0)
			{
				IsRelease=true;
				m_fAttackTimer = Time.time;
				Vector3 posSrc = m_goEffectLeft1905021_03.transform.position;
				Vector3 posRole = new Vector3(RoleList[0].GetPos().x,RoleList[0].GetPos().y+1.5f,RoleList[0].GetPos().z);
				float fRate = Vector2.Distance(posSrc,posRole)/3.8f;
				m_goEffectLeft1905021_03.transform.localScale = new Vector3 (fRate, 1.0f, 1.0f);
				float fAngle = NdUtil.V2toAngle(posSrc,posRole,Vector2.right);
				m_goEffectLeft1905021_03.transform.rotation=Quaternion.Euler(new Vector3(0,180f,fAngle));
				
				posSrc = m_goEffectRight1905021_03.transform.position;
				fRate = Vector2.Distance(posSrc,posRole)/3.8f;
				m_goEffectRight1905021_03.transform.localScale = new Vector3 (fRate, 1.0f, 1.0f);
				fAngle = NdUtil.V2toAngle(posSrc,posRole,Vector2.right);
				m_goEffectRight1905021_03.transform.rotation=Quaternion.Euler(new Vector3(0,180f,fAngle));


				foreach(Life w in RoleList)
				{
					
					IsRelease = true;
					GridActionCmd action = null;
					if (w is Role)
						action = (w as Role).CurrentAction;
					SkillReleaseInfo Info = m_Skill.SkillRelease(this,w,action,m_Skill.PropSkillInfo);
					m_SkillReleaseInfoList.Add(Info);
					m_AttackRoleList.Add(w);
					
				}
				m_bAttack=true;
			}
			int nRoleCount = m_AttackRoleList.Count;
			for(int nCnt=0;nCnt<nRoleCount;nCnt++)
			{
				Life w = m_AttackRoleList[nCnt];
				
				if (!w.InBoat || w.isDead)
					continue;
				SkillReleaseInfo info = m_SkillReleaseInfoList[nCnt];
				BuildSkill rs = m_Skill as BuildSkill;
				float power = rs.PropSkillInfo.GetAttackPower( nAttackIndex + 1);
				info.m_Damage = (int)(info.m_Damage * power);
				info.mOwnerLevel = m_Attr.Level;
				w.ApplyDamage(info, m_thisT);
				
			}
			if (nRoleCount > 0)
				SoundPlay.Play("Trap/trap_electricshock",false,false);
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

	public override void Destroy()
	{
		base.Destroy();
		m_goEffectLeft1905021_03.SetActive (false);
		m_goEffectRight1905021_03.SetActive (false);
	}
	public override void Shake()
	{
		if(!isDead)
			m_thisT.DOShakePosition(0.5f);
	}

}
