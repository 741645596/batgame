using UnityEngine;
using System.Collections;

public class GridActionCmdISPApperAction : GridActionCmd {

	public GridActionCmdISPApperAction()
	{
	}
	public override void SetTarget (Life Parent)
	{
		base.SetTarget (Parent);

	}
	public override void Finish ()
	{
		base.Finish();
	}
	public static GridActionCmd Create(Life Parent,int ISPtype,Vector3 end)
	{
	
		if (ISPtype == 1005)
		{
			GameObject go = (Parent as Role).RoleSkinCom.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.FirePos);
			Vector3 start = Parent.m_thisT.parent.InverseTransformPoint(go.transform.position);
			return new GridActionCmdISPApperAction1005 (start,end);
		}
		return null;
	}
}
public class GridActionCmdISPApperAction1005 : GridActionCmd {
	public float m_locus = 0;
	Bezier myBezier;
	Bezier myBezier1;
	Bezier myBezier2;
	public GridActionCmdISPApperAction1005(Vector3 start, Vector3 end)
	{
		m_Duration = 1.5f;
		m_Start = start;
		m_End = end;
		m_locus = -3f;
		float dis = m_End.x - m_Start.x;
		Vector3 pos = m_End;
		pos.x = m_Start.x + dis *0.7f; 
		myBezier = new Bezier( m_Start,  new Vector3(0,-m_locus,0),  new Vector3(0,-m_locus,0), pos );
		Vector3 pos1 = pos;
		pos1.x += dis * 0.2f;
		myBezier1 = new Bezier( pos,  new Vector3(0,-m_locus * 0.4f,0),  new Vector3(0,-m_locus*0.4f,0), pos1 );
		myBezier2 = new Bezier( pos1,  new Vector3(0,-m_locus *0.2f,0),  new Vector3(0,-m_locus*0.2f,0), m_End );
	}
	public override void SetTarget (Life Parent)
	{
		base.SetTarget (Parent);
		
	}

	public override void Update ()
	{
		base.Update ();
		if (m_TimeCount < m_Duration - 1)
		{

			Vector3 pos  = myBezier.GetPointAtTime(m_TimeCount/0.5f);
			PlayAction( AnimatorState.Stand,pos,true);
			//m_LifePrent.m_thisT.localPosition = pos;
		}
		else if (m_TimeCount <= m_Duration - 0.5f)
		{
			Vector3 pos = myBezier1.GetPointAtTime((m_TimeCount - 0.5f)/0.5f);
			PlayAction( AnimatorState.Stand,pos,true);
			//m_LifePrent.m_thisT.localPosition = pos;
		}
		else if (m_TimeCount < m_Duration )
		{
			
			Vector3 pos = myBezier2.GetPointAtTime((m_TimeCount -1f)/0.5f);
			PlayAction( AnimatorState.Stand,pos,true);
			//m_LifePrent.m_thisT.localPosition = pos;
		}
	}
	public override void Finish ()
	{
		base.Finish();
	}
}