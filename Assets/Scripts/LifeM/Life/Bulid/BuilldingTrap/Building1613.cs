using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

/// <summary>
/// 对地炮
/// </summary>
/// <author>TXM</author>
/// <Revisor>QFord</Revisor>
public class Building1613 : Building {
	private bool m_bAttack=false;//发射标识
	private bool m_bTriggle=false;//触发标识
	private float m_fAttackTime;//发射已过时间
	
	protected List<GameObject> m_listEffect1919011= new List<GameObject>();//受击通用特效


	private float m_fCheckRoleInSmallCirle;

	private Vector3 m_vDirGreed;
	private Vector3 m_vCurGreed;
	private Vector3 m_vStartGreed;

	private GameObject m_goPao;
	private GameObject Pao
	{
		get{
				if (m_goPao == null)
					m_goPao = m_Property.HelpPoint.GetVauleByKey (BuildHelpPointName.help01);
				return m_goPao;
			}

	}
	Vector3 m_posRole; //计录目标位置
	private float m_TimeCountRotate = 0;//瞄准计数器
	private float m_DurationRotate = 0;//瞄准周期
	private float  m_fdistance = 0;//与目标的距离
	private float m_fBombSpeed = 12f;//炸弹移动速度
	private float m_fGunRotateSpeed = 270f;//炸弹移动速度

	
	public override void InitBuildModel()
	{
		base.InitBuildModel ();
		GameObject go  = GameObjectLoader.LoadPath("effect/prefab/", "1919011", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.help01).transform);
		go.SetActive (false);
		m_listEffect1919011.Add (go);
		m_fCheckRoleInSmallCirle = Time.time;
		Pao.transform.localRotation = Quaternion.Euler(new Vector3(180f,90f,0));
		m_vDirGreed = new Vector3(180f,90f,0);
		m_vCurGreed = new Vector3(180f,90f,0);
	}
	
	public override void InitBuild ()
	{
		base.InitBuild ();
		
	}
	public override void BuildUpdate()
	{
		base.BuildUpdate ();
		if (m_Skill.PropSkillInfo.m_LifeTarget != null) 
		{
			m_TimeCountRotate+=Time.deltaTime;
			if(m_TimeCountRotate>=m_DurationRotate&&m_DurationRotate>=0)
			{
				m_TimeCountRotate=0;
				m_DurationRotate=0;
				m_vCurGreed = m_vDirGreed;
				if(m_bTriggle)
				{
					
					Vector3 posSrc = m_Property.HelpPoint.GetVauleByKey (BuildHelpPointName.help01).transform.position;
					Vector3 vTargetPos =  m_Skill.PropSkillInfo.m_vTargetPos;
					m_posRole = new Vector3 (vTargetPos.x, vTargetPos.y + 0.5f, -2);
					m_Skill.PropSkillInfo.SetTargetV3Pos(vTargetPos);
					GameObjectActionExcute gaeTarget = EffectM.LoadEffect(EffectM.sPath, "1919021", posSrc, m_Property.transform);
					GameObjectActionMove move = new GameObjectActionMove (posSrc, m_posRole, m_fdistance/m_fBombSpeed);
					move.m_complete = OnBomb;
					gaeTarget.AddAction (move);
					ActiveEffect1919011 (true);
					m_fAttackTime = Time.time;
					m_bAttack = true;
					m_bTriggle=false;
				}
				Pao.transform.localRotation = Quaternion.Euler (m_vCurGreed);
				if(!m_bAttack)
				{
					UpdateAimAt(m_bTriggle);
				}
			}
			else
			{
				m_vCurGreed = Vector3.Lerp (m_vStartGreed,m_vDirGreed,m_TimeCountRotate/m_DurationRotate);
				Pao.transform.localRotation = Quaternion.Euler (m_vCurGreed);
			}
		}

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
			SoundPlay.Play ("trap_cannon_sky",false,false);
			ActiveEffect1919011(false);
			SetAnimator (Build_AnimatorState.Stand10000);
			
		}
		
		
	}
	float UpdateAimAt(bool bTriggle)
	{
		if (m_Skill.PropSkillInfo.m_LifeTarget!=null&&m_DurationRotate==0) {
			Vector3 posSrc = new Vector3 (Pao.transform.position.x, Pao.transform.position.y, -2);
			Vector3 vTargetPos = m_Skill.PropSkillInfo.m_vTargetPos;
			if(!bTriggle)
			{
				if(m_Skill.PropSkillInfo.m_LifeTarget.isDead)
				{
					vTargetPos = m_Skill.PropSkillInfo.m_vTargetPos;
				}
				else {
					vTargetPos = m_Skill.PropSkillInfo.m_LifeTarget.GetPos();
					
				}
			}
			Vector3 posRole = new Vector3 (vTargetPos.x, vTargetPos.y + 0.5f, -2);
			m_fdistance = Vector3.Distance (posSrc, posRole);
			float fgreed = NdUtil.V2toAngle (posSrc, posRole, Vector3.right);

			if(m_vCurGreed.x<0)
				m_vCurGreed.x = 360f + m_vCurGreed.x;
			else if (m_vCurGreed.x > 360f)
				m_vCurGreed.x = m_vCurGreed.x-360f;
			m_vStartGreed = m_vCurGreed;
			m_vDirGreed = new Vector3 (fgreed, 90f, 0);

			float fGreeddistance = m_vStartGreed.x - m_vDirGreed.x;
			fGreeddistance = fGreeddistance > 0 ? fGreeddistance : -fGreeddistance;
			if (fGreeddistance > 180f)
			{
				if (m_vStartGreed.x > m_vDirGreed.x)
					m_vDirGreed.x += 360f;
				else if (m_vStartGreed.x < m_vDirGreed.x)
					m_vDirGreed.x -= 360f;
				fGreeddistance = 360f - fGreeddistance;
			}

			m_DurationRotate = fGreeddistance / m_fGunRotateSpeed;
			m_bTriggle = bTriggle;
		}
		return m_DurationRotate;
	}
	void ActiveEffect1919011(bool bActive)
	{
		int nEffectCount = m_listEffect1919011.Count;
		for (int nEffectCnt=0; nEffectCnt<nEffectCount; nEffectCnt++) 
		{
			
			m_listEffect1919011[nEffectCnt].SetActive(bActive);
			
		}
	}

	protected override void TriggerSkillTime(ref List<Life> RoleList,ref float fReleaseDelay)
	{
		if (m_Skill.PropSkillInfo.m_LifeTarget!=null) {
					m_DurationRotate=0;
			fReleaseDelay=UpdateAimAt(true)+m_fdistance/m_fBombSpeed;

				}
		
	}
	protected void OnBomb(object o)
	{
		GameObject.Destroy(o as GameObject);
		Transform tTarget = m_Skill.PropSkillInfo.m_LifeTarget.m_thisT;
		GameObjectActionExcute gaeTarget1919031 = EffectM.LoadEffect(EffectM.sPath, "1919031", m_posRole, tTarget);
		GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1.0f);
		gaeTarget1919031.AddAction(ndEffect);
		
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
			foreach(Life w in RoleList)
			{
				//Life w = RoleList[0];
				GridActionCmd action = null;
				if (w is Role)
					action = (w as Role).CurrentAction;
				SkillReleaseInfo Info = m_Skill.SkillRelease(this,w,action,m_Skill.PropSkillInfo);
				w.ApplyDamage(Info, m_thisT);
			}
			//m_Skill.PropSkillInfo.m_LifeTarget=null;
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
		ActiveEffect1919011(false);
	}
	
	public override void PlayClickAni()
	{
		base.PlayClickAni();
		ShowAttackRange(true);//对地炮 使用 全圆 攻击特效
	}
	
}