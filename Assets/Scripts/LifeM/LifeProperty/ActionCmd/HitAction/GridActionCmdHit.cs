using UnityEngine;
using System.Collections;

public class GridActionCmdHitParalysis : GridActionCmd {
	public GridActionCmdHitParalysis(float duration)
	{
		m_Duration = duration;
	}
	public override void SetTarget (Life Parent)
	{
		base.SetTarget (Parent);
		m_AniSpeed = 0;
		
	}
	public override void Finish ()
	{
		base.Finish();
		m_AniSpeed = 1;
	}
}

public class GridActionCmdHitKicked : GridActionCmd {
	public GridActionCmdHitKicked(Vector3 end, float duration)
	{
		m_Duration = duration;
		m_End = end;
	}
	public override void SetTarget (Life Parent)
	{
		base.SetTarget (Parent);
		m_Start = Parent.m_thisT.localPosition;
		
	}
	public override void Update ()
	{
		base.Update ();
		Vector3 pos = Vector3.Lerp(m_Start,m_End,m_TimeCount/m_Duration);
		PlayAction(AnimatorState.Stand,pos,true);
	}
	public override void Finish ()
	{
		base.Finish();
	}
}
