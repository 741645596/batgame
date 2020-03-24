using UnityEngine;
using System.Collections;

/// <summary>
/// 宠物1001鸭子出生，
/// </summary>
/// <author>zhulin</author>
public class GridActionCmdPetAppear1001 :GridActionCmd{
	// Update is called once per frame
	private bool m_FadeOut;
	private bool m_bRunOnce = true;
	Bezier myBezier;
	public GridActionCmdPetAppear1001(Vector3 start, Vector3 end, float duration ,WalkDir dir, int deep)
	{
		m_Start = start;
		m_End = end;
		m_Duration = duration;
		m_Dir = dir;
		m_RankDeep = deep;
		m_FadeOut = false;
		
		//myBezier = new Bezier( start,  new Vector3(0,-10,0),  new Vector3(0,-10,0), end );
	}
	public override   void Update () {
		base.Update();
		
		Vector3 pos = Vector3.Lerp(m_Start,m_End,m_TimeCount/m_Duration);
		PlayAction(AnimatorState.PetAppear, pos,true);
		
		
		
	}
	
	public override bool IsDone()
	{
		return (m_TimeCount >= m_Duration);
	}
	
}