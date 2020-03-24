using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// 炸弹房
/// </summary>
/// <author>zhulin</author>
public class Building1604 : Building {
	 
	protected GameObject m_goEffect1003031;//闪光特效
	private float m_fAttackTrigger=-1;
	public override void InitBuildModel()
	{
		base.InitBuildModel ();
		m_goEffect1003031 = GameObjectLoader.LoadPath("effect/prefab/", "1003031", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.skinroot).transform);
		m_goEffect1003031.transform.position = GetPos ();
		m_goEffect1003031.SetActive (false);
		
	}
	public override void InitBuild ()
	{
		base.InitBuild ();
	}
	public override void BuildUpdate()
	{
		base.BuildUpdate ();
		if(Time.time-m_fAttackTrigger>1.5f&&m_fAttackTrigger>0)
		{
			m_goEffect1003031.SetActive(false);
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
				GridActionCmd action = null;
				if (w is Role)
					action = (w as Role).CurrentAction;
				SkillReleaseInfo Info = m_Skill.SkillRelease(this,w,action,m_Skill.PropSkillInfo);
				w.ApplyDamage(Info, m_thisT);
			}
			m_fAttackTrigger = Time.time;
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
}
