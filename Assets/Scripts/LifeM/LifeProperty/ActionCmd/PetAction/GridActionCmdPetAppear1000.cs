using UnityEngine;
using System.Collections;

/// <summary>
/// 宠物1000菱出生，
/// </summary>
/// <author>zhulin</author>
public class GridActionCmdPetAppear1000 :GridActionCmd{
	// Update is called once per frame
	private bool m_FadeOut;
	private bool m_bRunOnce = true;
	Bezier myBezier;
	public GridActionCmdPetAppear1000(Vector3 start, Vector3 end, float duration ,WalkDir dir, int deep)
	{
		m_Start = start;
		m_End = end;
		m_Duration = duration;
		m_Dir = dir;
		m_RankDeep = deep;
		m_FadeOut = false;
		float r = Random.Range(-0.5f,-2f);
		myBezier = new Bezier( start,  new Vector3(0,r,0),  new Vector3(0,r,0), end );
	}
	public override   void Update () {
		base.Update();
		if (m_TimeCount < m_Duration/2)
		{
			Vector3 pos = myBezier.GetPointAtTime(m_TimeCount/(m_Duration/2));
			PlayAction(AnimatorState.PetAppear, pos,true);
		}
		else
			PlayAction(AnimatorState.PetDown, m_End,true);
		
		
	}
	
	public override bool IsDone()
	{
		return (m_TimeCount >= m_Duration);
	}
	
}
