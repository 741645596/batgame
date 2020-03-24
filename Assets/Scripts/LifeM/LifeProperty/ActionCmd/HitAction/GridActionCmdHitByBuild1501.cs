using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 1501(冰窟) 受击
/// </summary>
/// <author>zhulin</author>
public class GridActionCmdHitByBuild1501 :GridActionCmdHitByBuild{


	float m_AppearTime;
	float m_ContinueTime;
	float m_DisapperaTime;
	Vector3 pos ;

	public GridActionCmdHitByBuild1501(float duration)
	{
		m_AppearTime = 0.5f;
		m_Duration = duration;
		m_DisapperaTime = m_Duration - 1f;
		m_ContinueTime = m_DisapperaTime - m_AppearTime;
	}

	public override void StartHitByBuild ()
	{
		pos = m_Skin.tRoot.position;
		pos.z -= 1;
		m_AniSpeed = 0f;
	}

	public override void UpdateHitByBuild()
	{
		PlayAction(AnimatorState.Stand,m_Start);
		if (m_TimeCount < m_AppearTime)
		{
			Step1();
		}
		else if (m_TimeCount < m_DisapperaTime)
		{
			Step2();
		}
		else if (m_TimeCount < m_Duration)
		{
			if ((m_TimeCount - m_Delatime) < m_DisapperaTime)
			{
				Step3();
			}
			
		}
		else if (m_TimeCount >= m_Duration)
		{
			//m_Skin.HitByBuildingEffect(HitbyBuilding.HitByBuild1501,false ,2 ,HitEffectMode.Normal ,null);
			m_AniSpeed = 1f;
		}
		
	}
	
	public override void SetDone ()
	{
		base.SetDone ();
	}
	/// <summary>
	/// 步骤1
	/// </summary>
	private void Step1()
	{
		if ((m_TimeCount - m_Delatime) < 0)
		{
			(m_Skin as RoleSkin).HitByBuildingEffect(HitbyBuilding.HitByBuild1501,true ,0 ,HitEffectMode.Normal ,null);
		}
	}
	/// <summary>
	/// 步骤2
	/// </summary>
	private void Step2()
	{
		if ((m_TimeCount - m_Delatime) < m_AppearTime)
		{
			(m_Skin as RoleSkin).HitByBuildingEffect(HitbyBuilding.HitByBuild1501,false ,0 ,HitEffectMode.Normal ,null);
			(m_Skin as RoleSkin).HitByBuildingEffect(HitbyBuilding.HitByBuild1501,true ,1 ,HitEffectMode.Normal ,null);
		}
	}
	/// <summary>
	/// 步骤3
	/// </summary>
	private void Step3()
	{
		if ((m_TimeCount - m_Delatime) < m_DisapperaTime)
		{
			(m_Skin as RoleSkin).HitByBuildingEffect(HitbyBuilding.HitByBuild1501,false ,1 ,HitEffectMode.Normal ,null);
			(m_Skin as RoleSkin).HitByBuildingEffect(HitbyBuilding.HitByBuild1501,true ,2 ,HitEffectMode.Normal ,
			                           (t) => {
				if(t == null) return ;
				ParticleSystem[] ps = t.GetComponentsInChildren<ParticleSystem>();
				foreach(ParticleSystem p in ps)
				{
					p.Play();
					p.loop = false;
				}
			});
		}
	}
	public override void Finish ()
	{
		base.Finish ();
		(m_Skin as RoleSkin).HitByBuildingEffect(HitbyBuilding.HitByBuild1501,false ,0 ,HitEffectMode.Normal ,null);
		(m_Skin as RoleSkin).HitByBuildingEffect(HitbyBuilding.HitByBuild1501,false ,1 ,HitEffectMode.Normal ,null);
		(m_Skin as RoleSkin).HitByBuildingEffect(HitbyBuilding.HitByBuild1501,false ,2 ,HitEffectMode.Normal ,null);
	}
}
