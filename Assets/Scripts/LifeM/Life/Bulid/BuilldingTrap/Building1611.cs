using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

/// <summary>
/// 火焰
/// </summary>
/// <author>TXM</author>
/// <Revisor>QFord</Revisor>
public class Building1611 : Building {
	
	protected List<GameObject> m_listEffect1913011= new List<GameObject>();//喷火特效
	protected List<GameObject> m_listEffect1913021= new List<GameObject>();//火苗特效
	protected GameObject m_goEffect1913031= null;//受击通用特效
	bool m_bAttck=false;
	float m_fAttackTime;
	public override void InitBuildModel()
	{
		base.InitBuildModel ();
		GameObject go  = GameObjectLoader.LoadPath("effect/prefab/", "1913011", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.help01).transform);
		go.SetActive (false);
		m_listEffect1913011.Add (go);
		go  = GameObjectLoader.LoadPath("effect/prefab/", "1913011", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.help02).transform);
		go.SetActive (false);
		m_listEffect1913011.Add (go);
		go  = GameObjectLoader.LoadPath("effect/prefab/", "1913011", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.help03).transform);
		go.SetActive (false);
		m_listEffect1913011.Add (go);
		go  = GameObjectLoader.LoadPath("effect/prefab/", "1913011", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.help04).transform);
		go.SetActive (false);
		m_listEffect1913011.Add (go);
		go  = GameObjectLoader.LoadPath("effect/prefab/", "1913011", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.help05).transform);
		go.SetActive (false);
		m_listEffect1913011.Add (go);
		go  = GameObjectLoader.LoadPath("effect/prefab/", "1913011", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.help06).transform);
		go.SetActive (false);
		m_listEffect1913011.Add (go);
		
		
		go  = GameObjectLoader.LoadPath("effect/prefab/", "1913021", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.help01).transform);
		m_listEffect1913021.Add (go);
		go  = GameObjectLoader.LoadPath("effect/prefab/", "1913021", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.help02).transform);
		m_listEffect1913021.Add (go);
		go  = GameObjectLoader.LoadPath("effect/prefab/", "1913021", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.help03).transform);
		m_listEffect1913021.Add (go);
		go  = GameObjectLoader.LoadPath("effect/prefab/", "1913021", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.help04).transform);
		m_listEffect1913021.Add (go);
		go  = GameObjectLoader.LoadPath("effect/prefab/", "1913021", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.help05).transform);
		m_listEffect1913021.Add (go);
		go  = GameObjectLoader.LoadPath("effect/prefab/", "1913021", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.help06).transform);
		m_listEffect1913021.Add (go);
		
		m_goEffect1913031= GameObjectLoader.LoadPath("effect/prefab/", "1913031", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.help07).transform);
		
	}
	public override void InitBuild ()
	{
		base.InitBuild ();
	}
	void ActiveEffect1913011(bool bActive)
	{
		int nEffectCount = m_listEffect1913011.Count;
		for (int nEffectCnt=0; nEffectCnt<nEffectCount; nEffectCnt++) 
		{
			m_listEffect1913011[nEffectCnt].SetActive(bActive);
		}
	}
	void ActiveEffect1913021(bool bActive)
	{
		int nEffectCount = m_listEffect1913021.Count;
		for (int nEffectCnt=0; nEffectCnt<nEffectCount; nEffectCnt++) 
		{
			m_listEffect1913021[nEffectCnt].SetActive(bActive);
		}
	}
	public override void BuildUpdate()
	{
		base.BuildUpdate ();
		if (m_bAttck&&Time.time - m_fAttackTime > 0.9f) 
		{
			m_bAttck=false;
			ActiveEffect1913011(false);
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
		fReleaseDelay = 0.25f;
		ActiveEffect1913011(true);
		m_bAttck = true;
		m_fAttackTime = Time.time;
		
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
		ActiveEffect1913011(false);
		ActiveEffect1913021(false);
	}
     
}