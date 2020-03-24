using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

/// <summary>
/// 输送炸弹
/// </summary>
/// <author>LHH</author>
/// <Revisor>QFord</Revisor>
public class Building1609 : Building {

	private float m_fAttackTrigger=-1;
	private bool m_bBomb = false;
	private bool m_bHitFly = false;
	private bool m_playSound = true;
	protected GameObject m_goEffect1003031;//闪光特效
	protected List<Life> m_RoleList=new List<Life>();
	public override void InitBuildModel()
	{
		base.InitBuildModel ();
		m_goEffect1003031 = GameObjectLoader.LoadPath("effect/prefab/", "1003031",m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.help01).transform);
		//m_goEffect1003031.transform.position = GetPos ();
		m_goEffect1003031.SetActive (false);
		SetAnimator (Build_AnimatorState.Stand10000);
		
	}
	public override void InitBuild ()
	{
		base.InitBuild ();
	}
	public override void BuildUpdate ()
	{
		base.BuildUpdate ();
	
		if (m_bBomb==true && Time.time-m_fAttackTrigger > 0.5f)
		{
			if(m_playSound == true)
			{
				m_playSound = false;
				SoundPlay.Play("trap_boomroom",false,false);
			}
			//SetAnimator (Build_AnimatorState.Trigger30000);

		}
		if (m_bBomb==true && GetCDTime()<0.6f)
		{
			
			(m_Property as BuildProperty).EnableAllBodyRender (true);
			SetAnimator (Build_AnimatorState.Stand10000);

			m_goEffect1003031.SetActive(false);
			m_playSound = true;
			m_bBomb=false;
		}
		if (m_bHitFly==true && GetCDTime()<0.6f)
		{
			
			(m_Property as BuildProperty).EnableAllBodyRender (true);
			SetAnimator (Build_AnimatorState.Stand10000);
			m_goEffect1003031.SetActive(false);
			m_bHitFly=false;
		}
	}
	/// <summary>	
	/// 释放技能主逻辑
	/// </summary>
	protected override bool ReleaseSkill(ref List<Life> RoleList,ref int nAttackIndex)
	{
		m_RoleList.Clear ();
		bool IsRelease = false;
		if (RoleList.Count > 0)
		{
			IsRelease=true;
			foreach(Life w in RoleList)
			{
				m_RoleList.Add(w);
				GridActionCmd action = null;
				if (w is Role)
					action = (w as Role).CurrentAction;
				SkillReleaseInfo Info = m_Skill.SkillRelease(this,w,action,m_Skill.PropSkillInfo);
				w.ApplyDamage(Info, m_thisT);
			}
			m_fAttackTrigger = Time.time;
			m_bBomb = true;
			SoundPlay.Play("littleboom",false,false);
			SetAnimator (Build_AnimatorState.Trigger30000);
			m_goEffect1003031.SetActive(true);
		}
		return IsRelease ;
	}
	protected override void TriggerSkillTime(ref List<Life> RoleList,ref float fReleaseDelay)
	{

	}
	public override void  Destroy()
	{
		base.Destroy ();
		m_goEffect1003031.SetActive(false);
		
	}
	public override void Shake()
	{
		if(!isDead)
			m_thisT.DOShakePosition(0.5f);
	}
	public override void HitFly(WalkDir Dir,float duration,float delay, bool bearhit = false)
	{
		Vector3 pos =GetPos();
		GameObjectActionExcute gaeTarget = EffectM.LoadEffect(EffectM.sPath, "1917011", pos, null);
		GameObjectActionSpawn newSpawn = new GameObjectActionSpawn();
		gaeTarget.AddAction(newSpawn);
		GameObjectActionDelayDestory ndEffectTarget = new GameObjectActionDelayDestory(duration);
		GameObjectActionBezierMove nBezoerMove = new GameObjectActionBezierMove(pos,duration,Dir);
		newSpawn.AddAction(ndEffectTarget);
		newSpawn.AddAction(nBezoerMove);
		SetAnimator (Build_AnimatorState.Trigger30000);
		m_Skill.ReSetCDTime();
		m_bHitFly = true;
	}
     
}