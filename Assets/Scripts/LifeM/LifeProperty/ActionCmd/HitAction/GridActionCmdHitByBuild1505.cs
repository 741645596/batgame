using UnityEngine;
using System.Collections;

/// <summary>
/// 1505(压扁) 受击
/// </summary>
/// <author>zhulin</author>
public class GridActionCmdHitByBuild1505 :GridActionCmdHitByBuild{
	float m_PressTime;
	float m_delayTime;
	float m_recoverTime;
	public GridActionCmdHitByBuild1505(float duration)
	{
        //NGUIUtil.DebugLog("压扁动作");
		m_PressTime = 0.1f;
		m_delayTime = duration;
		m_Duration = m_delayTime + 1.067f;
	}
	public override void StartHitByBuild ()
	{
        RoleSkin skin = m_Skin as RoleSkin;
        if (skin != null)
        {
            skin.ShowAngerEffect(false);
        }
	}
	public override void UpdateHitByBuild()
	{
		if (m_TimeCount < m_PressTime)
		{
			Transform t = m_Skin.tRoot;
			t.localScale = Vector3.Lerp(new Vector3(1,1,1),new Vector3(1,0.2f,1),m_TimeCount / m_PressTime);
		}
		else if (m_TimeCount < m_delayTime)
		{
			Transform t = m_Skin.tRoot;
			t.localScale = new Vector3(1,0.2f,1);
			m_AniSpeed = 0;
		}
		else if (m_TimeCount < m_Duration)
		{
			m_AniSpeed = 1f;
			Transform t = m_Skin.tRoot;
			t.localScale = new Vector3(1,1,1);
			PlayAction(AnimatorState.Squash,m_Start);
		}
	}
	

	public override void Finish ()
	{
		base.Finish ();
        RoleSkin skin = m_Skin as RoleSkin;
        if (skin != null)
        {
            skin.ShowAngerEffect(true);
        }
		Transform t = m_Skin.tRoot;
		t.localScale = new Vector3(1,1,1);
	}
}
//压扁受击
public class GridActionCmdNoHitByBuild1505 :GridActionCmdHitByBuild{
	float m_StartTime;
	float m_IngTime;
	float m_endTime;
	public GridActionCmdNoHitByBuild1505(float duration)
	{
		m_StartTime = 0.733f;
		m_IngTime = duration-1.267f;
		m_endTime = duration;
		m_Duration = 1.267f;
	}
	public override void StartHitByBuild ()
	{
	}
	public override void UpdateHitByBuild()
	{
		//if (m_TimeCount < m_StartTime)
		//	PlayAction(AnimatorState.NoSquashStart,m_Start);
		/*else if (m_TimeCount < m_IngTime)
			PlayAction(AnimatorState.NoSquashing,m_Start);*/
		//else if (m_TimeCount < m_Duration)
			PlayAction(AnimatorState.NoSquashEnd,m_Start);
	}
	
	public override void Finish ()
	{
		base.Finish ();
		
		Transform t = m_Skin.tRoot;
		t.localScale = new Vector3(1,1,1);
	}
	
}
