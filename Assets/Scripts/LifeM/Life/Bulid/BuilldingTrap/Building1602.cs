#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#define UNITY_EDITOR_LOG
#endif

using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

/// <summary>
/// 弹簧板
/// </summary>
/// <author>QFord</author>
public class Building1602 : Building
{
    private AnimationClip m_CurrentClip;
    private float m_fCurrentClipLength;
	private WalkDir m_dir = WalkDir.WALKLEFT;
	private bool m_bCDOver=true;
	public override void InitBuildModel()
	{
		base.InitBuildModel ();
		SetAnimator (Build_AnimatorState.CDOver40100);
		
	}
	public override void InitBuild ()
	{
		base.InitBuild ();

	}
	public override void BuildUpdate()
	{
		base.BuildUpdate ();
		//if(m_bTriggerSkill&&Time.time-m_fTriggerTime>1.0f)
		//	SetAnimator (Build_AnimatorState.Stand10000);
		if (!m_bCDOver&&GetCDTime()<1f) 
		{
			m_bCDOver=true;
			SetAnimator (Build_AnimatorState.CDOver40100);
		}
	}
	public bool TimerCheckAttackOver()
	{
		if (Time.time - m_fTriggerTime < 1.0f)
			return false;
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
			m_bCDOver = false;

			IsRelease = true;
			foreach(Life w in RoleList)
			{
				Role flyRole = w as Role;
				SkillReleaseInfo Info = m_Skill.SkillRelease(this,flyRole,null,m_Skill.PropSkillInfo);
				if (flyRole != null) {
					flyRole.ApplyDamage (Info, m_thisT);
					if (flyRole.m_Status != null&&flyRole.m_Status.HaveState (StatusType.ClickFly) == true) {
						flyRole.HitFly (m_dir, 3f, 0.1f);
					} 
				}
			}
		} 
		return IsRelease ;
	}
	protected override void TriggerSkillTime(ref List<Life> RoleList,ref float fReleaseDelay)
	{
		fReleaseDelay = 0.0f;
		if (RoleList.Count > 0)
		{
			SoundPlay.Play("Trap/trap_ejection",false,false);
			Life lRole = RoleList [0];
			Vector3 end = Vector3.zero;
			m_dir = GetDir (lRole);
			switch (m_dir) {
			case  WalkDir.WALKLEFT:
				SetAnimator (Build_AnimatorState.Stand10000);
				SetAnimator (Build_AnimatorState.Trigger30100);
				break;
				
			case WalkDir.WALKRIGHT:
				SetAnimator (Build_AnimatorState.Stand10000);
				SetAnimator (Build_AnimatorState.Trigger30000);
				break;
			}
		}
	}
	
	
	/// <summary>
	/// 判断炮弹兵在拳击手套的左边还是右边
	/// </summary>
	/// <param name="walk"></param>
	/// <returns></returns>
	private WalkDir GetDir(Life walk)
	{
		//获取拳击房间中心点的位置
		MapGrid boxGrid = GetMapGrid();
		MapGrid walkGrid = walk.GetMapGrid();
		
		if (boxGrid.GridPos.Unit>=walkGrid.GridPos.Unit)
		{
			return WalkDir.WALKLEFT;
		}
		return WalkDir.WALKRIGHT;
	}
	public override void Hit(int damage)
	{
	}
	public override void Shake()
	{
		if(!isDead)
			m_thisT.DOShakePosition(0.5f);
	}
}

