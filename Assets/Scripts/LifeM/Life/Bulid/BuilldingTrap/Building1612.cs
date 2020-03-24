using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

/// <summary>
/// 对地炮
/// </summary>
/// <author>TXM</author>
/// <Revisor>QFord</Revisor>
public class Building1612 : Building {
	private bool m_bAttack=false;
	//private bool m_bAttackSound = true;
	private float m_fAttackTime;
	
	protected List<GameObject> m_listEffect1915011= new List<GameObject>();//受击通用特效
	
	
	private float m_fCheckRoleInSmallCirle;
	
	private float m_fSpeenRotate;
	private Vector3 m_vDirGreed;
	private Vector3 m_vCurGreed;

	private GameObject m_goPao;
	private GameObject Pao
	{
		get{
				if (m_goPao == null)
						m_goPao = m_Property.HelpPoint.GetVauleByKey (BuildHelpPointName.help03);
				return m_goPao;
			}

	}
	
	
	
	public override void InitBuildModel()
	{
		base.InitBuildModel ();
		GameObject go  = GameObjectLoader.LoadPath("effect/prefab/", "1915011", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.help01).transform);
		go.SetActive (false);
		m_listEffect1915011.Add (go);
		go  = GameObjectLoader.LoadPath("effect/prefab/", "1915011", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.help02).transform);
		go.SetActive (false);
		m_listEffect1915011.Add (go);
		m_fCheckRoleInSmallCirle = Time.time;
		Pao.transform.localRotation = Quaternion.Euler(new Vector3(45f,90f,0));
		m_vDirGreed = new Vector3(45f,90f,0);
		m_vCurGreed = new Vector3(45f,90f,0);
	}
	public override void InitBuild ()
	{
		base.InitBuild ();
		
	}
	public override void BuildUpdate()
	{
		base.BuildUpdate ();
		m_vCurGreed = m_vDirGreed;
		Pao.transform.localRotation = Quaternion.Euler (m_vCurGreed);
		if (Time.time - m_fCheckRoleInSmallCirle > 0.2f) 
		{
			List<Life> RoleList = new List<Life>();
			List<Life> newRoleList = new List<Life>();
			CM.SearchLifeMList(ref RoleList,null,LifeMType.SOLDIER /*| LifeMType.PET*/,LifeMCamp.ALL,MapSearchStlye.Circle,this,(MapGrid.m_width * 150) / MapGrid.m_Pixel);
			if (RoleList.Count > 0) 
			{
				(m_Property as BuildProperty).SetColor("_Color", new Color (1.0f, 1.0f, 1.0f, 0.196f));
			}
			else
			{
				(m_Property as BuildProperty).SetColor("_Color", new Color (1.0f, 1.0f, 1.0f, 1.0f));
			}
		}
		if (m_bAttack && Time.time - m_fAttackTime>0.5f) 
		{
			m_bAttack=false;
			SoundPlay.Play ("trap_cannon_land",false,false);
			ActiveEffect1915011(false);
			
			SetAnimator (Build_AnimatorState.Stand10000);
			
		}
		
		
	}
	void ActiveEffect1915011(bool bActive)
	{
		int nEffectCount = m_listEffect1915011.Count;
		for (int nEffectCnt=0; nEffectCnt<nEffectCount; nEffectCnt++) 
		{
			
			m_listEffect1915011[nEffectCnt].SetActive(bActive);
			
		}
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
			SetAnimator (Build_AnimatorState.Trigger30000);
			//foreach(Life w in RoleList)
			{
				Life w = RoleList[0];
				Vector3 posSrc = Pao.transform.position;
				Vector3 posRole = new Vector3(w.GetPos().x,w.GetPos().y,w.GetPos().z);
				float fgreed = NdUtil.V2toAngle(posSrc,posRole,Vector3.right);
				m_vDirGreed =  new Vector3 (fgreed,90f,0);
				bool InterruptSkill = false;
				GridActionCmd action = null;
				if (w is Role)
					action = (w as Role).CurrentAction;
				SkillReleaseInfo Info = m_Skill.SkillRelease(this,w,action,m_Skill.PropSkillInfo);
				w.ApplyDamage(Info, m_thisT);
			}
			m_bAttack=true;
			m_fAttackTime = Time.time;
			ActiveEffect1915011(true);
		}
		return IsRelease ;
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
		ActiveEffect1915011(false);
	}
	
	public override void PlayClickAni()
	{
		base.PlayClickAni();
		ShowAttackRange(true);//对地炮 使用 全圆 攻击特效
	}
	
}