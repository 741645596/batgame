using UnityEngine;
using System.Collections;
//击退
public class GridActionCmdRepulse : GridActionCmd {
	
	protected Vector3 m_Start;
	protected Vector3 m_End;
	protected MapGrid desgrid;
	protected float	m_GetDownTime = 0.2f;
	protected float	m_GetUpTime = 0.2f;
	//protected float m_speed
}

public class GridActionCmdRepulseJump : GridActionCmdRepulse{

	public GridActionCmdRepulseJump( MapGrid g,WalkDir dir)
	{
		m_Duration = m_GetDownTime + m_GetUpTime + 0.5f;
		m_End = g.pos;
		desgrid = g;
		//m_Dir = dir == WalkDir.WALKLEFT? WalkDir.WALKRIGHT : WalkDir.WALKLEFT;
	}
	public override void SetTarget (Life Parent)
	{
		base.SetTarget (Parent);
		m_Start = Parent.m_thisT.localPosition;
		//Debug.Log("GridActionCmdRepulseJump  :" + desgrid.GridPos);
		m_Dir  = m_LifePrent.WalkDir;// == WalkDir.WALKLEFT? WalkDir.WALKRIGHT : WalkDir.WALKLEFT;
	}
	public override void Update ()
	{
		base.Update ();
		if (m_TimeCount < m_GetDownTime)
		{
			PlayAction(AnimatorState.Jump1,m_Start);
		}
		else if (m_TimeCount < m_Duration - m_GetUpTime)
		{
			Vector3 pos = m_Start;
			
			float height = -0.8f;
			pos.y =m_Start.y + height * (  Mathf.Pow(((m_TimeCount - m_GetDownTime)  /0.5f - 0.5f) / 0.5f,2) - 1);
			PlayAction(AnimatorState.Jump2,pos,true);
		}
		else
		{
			PlayAction(AnimatorState.Jump2,m_Start);
		}
	}
	public override void Finish ()
	{
		base.Finish();
		//(m_LifePrent as Role).RoleWalk.Teleport(desgrid);
		m_LifePrent.m_thisT.localPosition = m_Start;
		(m_LifePrent as Role).ChangeTarget(ChangeTargetReason.ClearTargetAndPath,null);
	}
}
//击退一个攻击位
public class GridActionCmdRepulseOneGrid : GridActionCmdRepulse{
	public GridActionCmdRepulseOneGrid( MapGrid g,WalkDir dir)
	{
		m_Duration = m_GetDownTime + m_GetUpTime + 0.5f;
		m_End = g.pos;
		desgrid = g;
		//m_Dir  = dir == WalkDir.WALKLEFT? WalkDir.WALKRIGHT : WalkDir.WALKLEFT;
	}	
	public override void SetTarget (Life Parent)
	{
		base.SetTarget (Parent);
		m_Start = Parent.m_thisT.localPosition;//m_LifePrent.WalkDir == WalkDir.WALKLEFT? WalkDir.WALKRIGHT : WalkDir.WALKLEFT;
		m_Dir = m_LifePrent.WalkDir;
		//Debug.Log("GridActionCmdRepulseOneGrid  :" + m_LifePrent.GetMapPos() + "," + desgrid.GridPos + ", " + m_Dir);
		(m_LifePrent as Role).RoleWalk.Teleport(desgrid);
		
	}
	public override void Update ()
	{
		base.Update ();
		if (m_TimeCount < m_GetDownTime)
		{
			PlayAction(AnimatorState.Jump1,m_Start);
		}
		else if (m_TimeCount < m_Duration - m_GetUpTime)
		{

			Vector3 pos = m_Start;
			pos = Vector3.Lerp(m_Start,m_End,(m_TimeCount - m_GetDownTime)  /0.5f);
			float height = -0.8f;
			pos.y =m_Start.y + height * (  Mathf.Pow(((m_TimeCount - m_GetDownTime)  /0.5f - 0.5f) / 0.5f,2) - 1);
			PlayAction(AnimatorState.Jump2,pos,true);
		}
		else
		{
			PlayAction(AnimatorState.Jump3,m_End);
		}
	}
	public override void Finish ()
	{
		base.Finish();
		m_LifePrent.m_thisT.localPosition = m_End;
		//(m_LifePrent as Role).ChangeTarget(ChangeTargetReason.ClearTargetAndPath,null);
	}
}
//击退掉船里
public class GridActionCmdRepulseDownLayer : GridActionCmdRepulse{
	protected MapGrid m_down;
	private float m_downtime;
	private float m_speed = 2.5f;
	public GridActionCmdRepulseDownLayer( MapGrid g,MapGrid down,WalkDir dir)
	{
		m_GetDownTime = 0.2f;
		m_GetUpTime = 0.2f;
		m_Duration = m_GetDownTime + m_GetUpTime;
		m_End = g.pos;
		desgrid = g;
		m_down = down;
		m_downtime = (g.GridPos.Layer - down.GridPos.Layer )  / m_speed;
		m_Duration += m_downtime;
		//m_Dir = dir == WalkDir.WALKLEFT? WalkDir.WALKRIGHT : WalkDir.WALKLEFT;
	}
	public override void SetTarget (Life Parent)
	{
		base.SetTarget (Parent);
		m_Start = Parent.m_thisT.localPosition;
		m_LifePrent.InBoat = false;
		m_Dir  = m_LifePrent.WalkDir;// == WalkDir.WALKLEFT? WalkDir.WALKRIGHT : WalkDir.WALKLEFT;
		
		(m_LifePrent as Role).RoleWalk.Teleport(m_down);
		//Debug.Log("GridActionCmdRepulseDownLayer  :" + desgrid.GridPos + "," + m_down.GridPos);
	}
	public override void Update ()
	{
		base.Update ();
		if (m_TimeCount < m_GetDownTime)
		{
			Vector3 pos = Vector3.Lerp(m_Start,m_End,m_TimeCount/m_GetDownTime);
			
			float height = -0.8f;
			pos.y =m_Start.y + height * (  Mathf.Pow((m_TimeCount  /m_GetDownTime - 0.5f) / 0.5f,2) - 1);
			PlayAction(AnimatorState.Jump1,pos,true);
		}
		else if (m_TimeCount < m_Duration - m_GetUpTime)
		{
			
			m_AniSpeed = 0.56667f / m_downtime;
			Vector3 pos = Vector3.Lerp(m_End,m_down.pos,(m_TimeCount - m_GetDownTime)/m_downtime);

			PlayAction(AnimatorState.Jump2,pos,true);
		}
		else if (m_TimeCount < m_Duration)
			PlayAction(AnimatorState.Jump3,m_down.pos,true);
	}
	public override void Finish ()
	{
		base.Finish();
		m_LifePrent.InBoat = true;
		m_LifePrent.m_thisT.localPosition = m_down.pos;
		//(m_LifePrent as Role).ChangeTarget(ChangeTargetReason.ClearTargetAndPath,null);
	}
}
//击退掉船外
public class GridActionCmdRepulseOutBoat : GridActionCmdRepulse{

	private float m_downtime;
	private float m_speed = 2f;
	public GridActionCmdRepulseOutBoat(Vector3 pos,WalkDir dir)
	{
		m_Duration = 180f;
		m_End = pos;
		m_Dir = dir == WalkDir.WALKLEFT? WalkDir.WALKRIGHT : WalkDir.WALKLEFT;
	}
	public override void SetTarget (Life Parent)
	{
		base.SetTarget (Parent);
		m_Start = Parent.m_thisT.localPosition;
		//m_Dir  = m_LifePrent.WalkDir == WalkDir.WALKLEFT? WalkDir.WALKRIGHT : WalkDir.WALKLEFT;
		//Debug.Log("GridActionCmdRepulseOutBoat  :" + m_Start + "," + m_End);
	}
	public override void Update ()
	{
		base.Update ();
		if (m_TimeCount < m_GetDownTime)
		{
			Vector3 pos = Vector3.Lerp(m_Start,m_End,m_TimeCount/m_GetDownTime);

			float height = -0.8f;
			pos.y =m_Start.y + height * (  Mathf.Pow((m_TimeCount  /m_GetDownTime - 0.5f) / 0.5f,2) - 1);
			PlayAction(AnimatorState.Fly00000,pos,true);
		}
		else
		{
			m_LifePrent.m_thisT.localPosition += new Vector3(0,-2,0) * 2 * m_Delatime ;
			
			PlayAction(AnimatorState.Fly00000,m_End);
		}
	}
	public override void Finish ()
	{
		base.Finish();
		(m_LifePrent as Role).ChangeTarget(ChangeTargetReason.ClearTargetAndPath,null);
		//(m_LifePrent as Role).RoleWalk.Teleport(desgrid);
		//m_LifePrent.m_thisT.localPosition = m_Start;
	}
}