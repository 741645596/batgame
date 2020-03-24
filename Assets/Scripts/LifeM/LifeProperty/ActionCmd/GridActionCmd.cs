#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#define UNITY_EDITOR_LOG
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum ActionMode
{
	Set  = 0,     //设置模式
	Delta   = 1,  //增量改变模式
}

public class GridActionCmd  {

	public Vector3 m_Start;
	public RoleState m_State;
	public float m_Duration= 1.0f;
	public int m_AttackSceneID;  //攻击目标 sceneID
	public WalkDir m_Dir;
	public int m_RankDeep;
	public Vector3 m_End;
	public float m_TimeCount = 0f;

	public float  m_Delatime = 0.02f;
	public GameObject m_effectgo = null;
	public float m_Speed = 1f;
	public float m_AniSpeed = 1f;
	//核心数据
	public Skin m_Skin = null;
	public Life m_LifePrent =  null;
	public bool m_bFinish = false;

	public float Speed
	{
		get{return GetSpeed();}
	}
	public virtual float GetSpeed()
	{
		return 1;
	}
	public GridActionCmd()
	{
	}
	public void SetData( Vector3 start,Vector3 end,RoleState state, float duration ,int attactsceneid,WalkDir dir, int deep)
	{
		this.m_Start = start;
		this.m_End = end;
		this.m_State = state;
		this.m_Duration = duration;
		this.m_AttackSceneID = attactsceneid;
		this.m_Dir = dir;
		this.m_RankDeep = deep;
	}

	public virtual void SetTarget(Life Parent)
	{
		if(Parent ==null)
        {
            Debug.Log("lifem is null");
        }
		else
		{
			m_LifePrent = Parent;
			m_Skin = Parent.GetSkin();
			if (m_Skin is RoleSkin)
				m_TimeCount =  - (m_Skin as RoleSkin).BlendTime;
			#if UNITY_EDITOR_LOG
			if (m_LifePrent!=null)
			{
				FileLog.write(m_LifePrent.SceneID, this + "  start" + m_LifePrent.m_thisT.localPosition + "," + Time.realtimeSinceStartup);
			}
			#endif
		}
	}

	// Update is called once per frame
	public virtual void Update () {
		m_Delatime =  Time.deltaTime * Speed;
		m_TimeCount += m_Delatime;
		if(m_Skin != null)
		{
			m_Skin.SetAnimatorSpeed(Speed * m_AniSpeed);
			m_Skin.SetMirror(m_Dir);
		}
	}

	// Update is called once per frame
	public  void RotateAction (Vector3 Angel) 
	{
		if(m_Skin != null)
		{
			m_Skin.Rotate(Angel);
		}
	}

	/// <summary>
	/// 播放动画
	/// </summary>
	public void RolePlayAnimation(AnimatorState state)
	{
		if(m_Skin != null)
		{
			m_Skin.PlayAnimation(state);
		}
	}
	/// <summary>
	/// 位移改变
	/// </summary>
	public void MoveAction(AnimatorState state,Vector3 Pos,ActionMode Mode)
	{
		if(m_Skin != null)
		{
			m_Skin.Move(state ,Pos ,Mode);
		}
	}

	/// <summary>
	/// 开启尾迹
	/// </summary>
	public void EnableTrail(bool isEnable = true)
	{
		if(m_Skin == null) return ;
		(m_Skin as RoleSkin).EnableTrail(isEnable);
	}

	public void EnableFireFx(bool isEnable = true)
	{
		if (m_Skin == null) return;
		(m_Skin as RoleSkin).AttachFxWorldCoord("2001211", "EffectPos", Vector3.zero, Vector3.zero,1.5f);
	}


	public void PlayAction(AnimatorState state,Vector3 pos, bool isMove = false,AnimatorState layerstate = AnimatorState.Empty)

	{
		if(m_Skin != null)
		{
			m_Skin.PlayAction(state ,pos ,m_RankDeep,isMove,layerstate);
			if (!isMove)
				if (m_LifePrent is Role)
					(m_LifePrent as Role).AdjustPosition(m_Delatime);
		}

	}

	public virtual bool IsDone()
	{
		if (m_TimeCount >= m_Duration)
		{
			if (!m_bFinish)
			{
				Finish();
			}
		}
		return (m_TimeCount >= m_Duration);
	}
	public bool TestNextDone()
	{
		return ((m_TimeCount +Time.deltaTime* Speed) >= m_Duration);
	}
	public virtual void SetDone()
	{
		m_Duration = m_TimeCount;
		if (!m_bFinish)
		{
			Finish();
		}

	}
	public void SetSpeed(float speed)
	{
		m_Speed = speed;
	}
	//结束处理
	public virtual void Finish()
	{
		#if UNITY_EDITOR_LOG
        if (m_LifePrent!=null)
        {
			FileLog.write(m_LifePrent.SceneID, this + "  finish" + m_LifePrent.m_thisT.localPosition + "," + Time.realtimeSinceStartup);
        }
		#endif
        if (m_effectgo != null)
            GameObject.Destroy(m_effectgo);
		m_bFinish = true;
	}
}

public class GridActionCmdJumpHoleDown :GridActionCmdFall{
	// Update is called once per frame
	public GridActionCmdJumpHoleDown(Vector3 start,Vector3 mid,Vector3 end, float duration ,WalkDir dir, int deep)
		:base(start,mid,end,duration,dir,deep)
	{
	}
	public override   void DoUpdate () {

		base.DoUpdate();
	}
}
public class GridActionCmdFall :GridActionCmd{
	Vector3 m_Mid;
	// Update is called once per frame
	public GridActionCmdFall(Vector3 start,Vector3 mid, Vector3 end, float duration ,WalkDir dir, int deep)
	{
		base.SetData(start,end,RoleState.JUMP,duration,-1,dir,deep);
		m_Mid = mid;
	}
	
	public override float GetSpeed ()
	{
		if (m_LifePrent != null)
			return m_LifePrent.m_Attr.SpeedPercent;
		return 1;
	}
	public override   void Update () {
		base.Update();
		DoUpdate();
		
	}
	public virtual void DoUpdate()
	{
		/*Vector3 pos = Vector3.Lerp(m_Start,m_End,m_TimeCount / m_Duration);
		pos.z = -0.7f * m_RankDeep;
		PlayAction(AnimatorState.Fly00000,pos,true);*/
		if (m_TimeCount > (m_Duration-0.2f))
		{
			m_End.z = Skin.s_DeepDistint * m_RankDeep;
			m_AniSpeed = 1f;
			PlayAction(AnimatorState.Jump3,m_End,true);
		}
		else if (m_TimeCount < 0.2f)
		{
			
			Vector3 pos = m_Start;
			pos.x = Mathf.Lerp(m_Start.x,m_Mid.x,m_TimeCount/(m_Duration-0.2f));
			//pos.y = Mathf.Lerp(m_Start.y,m_Start.y-0.8f,m_TimeCount/0.2f);
			pos.z = Skin.s_DeepDistint * m_RankDeep;
			PlayAction(AnimatorState.Jump1,pos,true);
		}
		else
		{
			float height = 0.8f;
			float per = (m_TimeCount - 0.2f) / (m_Duration-0.4f);
			Vector3 pos = m_Mid;
			
			pos.x = Mathf.Lerp(m_Mid.x,m_End.x,m_TimeCount/(m_Duration-0.2f));
			pos.y = Mathf.Lerp(m_Mid.y ,m_End.y,per);
			m_AniSpeed = 0.56667f /(m_Duration-0.4f);
			pos.z = Skin.s_DeepDistint * m_RankDeep;
			PlayAction(AnimatorState.Jump2,pos,true);
		}
	}
}
public class GridActionCmdFallDown :GridActionCmd{
	// Update is called once per frame
	public GridActionCmdFallDown(float Duration,int deep,Vector3 pos)
	{
		m_Start = pos;
		m_Duration = Duration;
		m_RankDeep = deep;
	}
	public override   void Update () {
		base.Update();
		PlayAction(AnimatorState.Stand,m_Start);
	}
}


/*public class GridActionCmdJumpStart :GridActionCmd{
	// Update is called once per frame
	
	public override   void Update () {
		base.Update();
		PlayAction(AnimatorState.Jump1,m_Start);
	}
}*/

public class GridActionCmdJump :GridActionCmd{
	// Update is called once per frame
	public delegate bool JumpAttackFun(Life target,MapGrid m);
	protected float m_EventTime;
	protected JumpAttackFun m_Fun;
	protected Life m_AttackWall;
	protected MapGrid m_FallGrid;
	float m_StartTime = 0.2f;
	float m_EndTime = 0.2f;
	float m_JumpTime;
	public GridActionCmdJump(Vector3 start,Vector3 end, float duration ,WalkDir dir, int deep, float starttime, float endtime,
	                         float eventtime,JumpAttackFun fun, Life w,MapGrid m)
	{
		base.SetData(start,end,RoleState.JUMP,duration,-1,dir,deep);
		m_StartTime = starttime;
		m_EndTime = endtime;
		m_JumpTime = duration;
		m_Duration = duration + starttime + endtime;
		m_EventTime = eventtime + starttime;
		m_Fun = fun;
		m_AttackWall = w;
		m_FallGrid = m;
	}
	
	public void CheckAttack()
	{
		if (m_Fun != null)
		{
			if (m_TimeCount > m_EventTime)
				if(m_Fun(m_AttackWall,m_FallGrid))
					m_Duration = m_TimeCount;
		}
	}
	public override  void Update () {
		base.Update();
		if (m_TimeCount > (m_Duration-m_EndTime))
		{
			m_End.z = Skin.s_DeepDistint * m_RankDeep;
			m_AniSpeed = 1f;
			PlayAction(AnimatorState.Jump3,m_End,true);
		}
		else if (m_TimeCount < m_StartTime)
		{
			PlayAction(AnimatorState.Jump1,m_Start);
		}
		else
		{
			
			float height = -0.8f;
			float per = (m_TimeCount-m_StartTime) / m_JumpTime;
			Vector3 pos = Vector3.Lerp(m_Start,m_End,per);
			pos.y =m_Start.y + height * (  Mathf.Pow((per - 0.5f) / 0.5f,2) - 1);
			m_AniSpeed = 0.56667f / m_JumpTime;
			pos.z = Skin.s_DeepDistint * m_RankDeep;
			PlayAction(AnimatorState.Jump2,pos,true);
			CheckAttack();
		}
	}
	
	public override float GetSpeed ()
	{
		if (m_LifePrent != null)
			return m_LifePrent.m_Attr.SpeedPercent;
		return 1;
	}
}
public class GridActionCmdSpecialJump :GridActionCmd{
	// Update is called once per frame
	float m_StartTime = 0.2f;
	float m_EndTime = 0.2f;
	float m_JumpTime = 0;
	public GridActionCmdSpecialJump(float Duration,Vector3 vStart,Vector3 vEnd,WalkDir dir,int deep)
	{
		m_JumpTime = Duration;
		m_Duration = m_StartTime + Duration + m_EndTime;
		m_Start = vStart;
		m_End = vEnd;
		m_Dir = dir;
		m_RankDeep = deep;
	}

	public override  void Update () {
		base.Update();
		
		Vector3 pos = Vector3.Lerp(m_Start,m_End,m_TimeCount / m_Duration);
		PlayAction(AnimatorState.Walk,pos,true);
	}
}



public class GridActionCmdHit :GridActionCmdInterrupt{
	// Update is called once per frame
	public bool Played  = false;
	public GridActionCmdHit(float Duration,Vector3 pos,int deep,WalkDir dir)
	{

		m_Duration = Duration;
		m_Start = pos;
		m_End = pos;
		m_RankDeep = deep;
		m_Dir = dir;
	}

	public override   void Update () {
		base.Update();
		if (!Played)
		{
			PlayAction(AnimatorState.Hit,m_Start);
			Played = true;
		}
	}

}

public class GridActionCmdSkillStand :GridActionCmdInterrupt{
	// Update is called once per frame
	public bool m_beffect;
	public GridActionCmdSkillStand(float Duration,Vector3 pos,int deep)
	{
		m_Duration = Duration;
		m_Start = pos;
		m_End = pos;
		m_RankDeep = deep;
		m_beffect = false;

	}
	public override   void Update () {
		base.Update();
		/*if(!m_beffect)
		{
			Transform t = U3DUtil.FindChildDeep(m_Skin.tRoot.gameObject,"Vertigopos").transform;
			if (t != null)
			{
				m_effectgo = SkillEffects._instance.LoadEffect("effect/prefab/", "2000031",t.position,m_Duration);
			}
			m_beffect = true;
		}*/
		PlayAction(AnimatorState.dizziness,m_Start);
	}
	


}
public class GridActionCmdCDStand :GridActionCmdInterrupt{
	// Update is called once per frame
	public GridActionCmdCDStand(float Duration,Vector3 pos,int deep,WalkDir dir)
	{
		m_Duration = Duration;
		m_Start = pos;
		m_End = pos;
		m_RankDeep = deep;
		m_Dir = dir;
	}
	public override   void Update () {
		base.Update();

		/*if(m_LifePrent != null && m_LifePrent.m_Status != null)
		{
			if (m_LifePrent.m_Status.CheckStateBySkill(1022))
			{
				PlayAction(AnimatorState.Skill01,m_Start);
				//m_Speed = w.m_Attr.SpeedPercent;
			}
			else PlayAction(AnimatorState.Stand,m_Start);
		}
		else*/ PlayAction(AnimatorState.Stand,m_Start);
	}
	

	public void SetDuration(float duration)
	{
		m_Duration = duration;
	}
}
public class GridActionCmdStand :GridActionCmdInterrupt{
	// Update is called once per frame
	public override   void Update () {
		base.Update();	
		if(m_LifePrent != null && m_LifePrent.m_Status != null)
		{
			/*if (m_LifePrent.m_Status.CheckStateBySkill(1022))
			{
				PlayAction(AnimatorState.Skill01,m_Start);
				//m_Speed = w.m_Attr.SpeedPercent;
			}
			else */if (m_LifePrent != null && m_LifePrent is Role && (m_LifePrent as Role).Target != null)
				PlayAction(AnimatorState.Walk,m_Start);
			else
				PlayAction(AnimatorState.Stand,m_Start);
		}
		else if (m_LifePrent != null && m_LifePrent is Role && (m_LifePrent as Role).Target != null)
			PlayAction(AnimatorState.Walk,m_Start);
		else
			PlayAction(AnimatorState.Stand,m_Start);
	}


	public void SetDuration(float duration)
	{
		m_Duration = duration;
	}
}

public class GridActionCmdStair :GridActionCmd{
	// Update is called once per frame
	float m_inTime;
	float m_outTime;
	float m_StairTime;
	Vector3 m_inpos;
	Vector3 m_outpos;
	public GridActionCmdStair(Vector3 start,Vector3 end, float duration ,WalkDir dir, int deep,float intime, float outtime,Vector3 inpos, Vector3 outpos)
	{
		m_StairTime = duration; 
		m_inTime = intime;
		m_outTime = outtime;
		m_inpos = inpos;
		m_inpos.z = 2.5f;
		m_outpos = outpos;
		m_outpos.z = 2.5f;

		base.SetData(start,end,RoleState.JUMP,duration,-1,dir,deep);
		m_Duration = duration + intime + outtime;
	}

	public override   void Update () {
		base.Update();
		if (m_TimeCount > (m_Duration - m_outTime))
		{
			/*Vector3 start = m_End;
			start.z = -1.8f;
			m_End.z = -0.7f * m_RankDeep;*/
			m_AniSpeed = 1;
			Vector3 pos = Vector3.Lerp(m_outpos,m_End,(m_TimeCount - m_StairTime -m_inTime) / m_outTime);
			PlayAction(AnimatorState.Jump3,pos,true);
		}
		else if (m_TimeCount < m_inTime)
		{
			/*Vector3 end = m_Start;
			end.z = -2.5f;
			m_Start.z =  -2.5f;*/
			Vector3 pos = Vector3.Lerp(m_Start,m_inpos,m_TimeCount / m_inTime);
			PlayAction(AnimatorState.Jump1,pos,true);
		}
		else
		{
			m_Start.z = 1.8f;
			m_End.z = 1.8f;
			m_AniSpeed = 0.56667f /(m_Duration-0.4f);
			Vector3 pos = Vector3.Lerp(m_inpos,m_outpos,(m_TimeCount-m_inTime) / m_StairTime);
			PlayAction(AnimatorState.Jump2,pos,true);

		}
	}

    public override void Finish()
    {
        Role role = m_LifePrent as Role;
        if (role!=null)
        {
			if (role.MoveAI is Walk)
				(role.MoveAI as Walk).run.SetPet1002StairState(false);
        }
        base.Finish();
    }
	
	public override float GetSpeed ()
	{
		if (m_LifePrent != null)
			return m_LifePrent.m_Attr.SpeedPercent;
		return 1;
	}
}
/*public class GridActionCmdStairOut :GridActionCmd{
	// Update is called once per frame
	
	public override   void Update () {
		base.Update();
		m_Start.z = -1.8f;
		m_End.z = -0.7f * m_RankDeep;
		Vector3 pos = Vector3.Lerp(m_Start,m_End,m_TimeCount / m_Duration);
		PlayAction(AnimatorState.Walk,pos,true);
	}
}*/
public class GridActionCmdWalk :GridActionCmdInterrupt{
	// Update is called once per frame
	//Role w = null;
	public GridActionCmdWalk(Vector3 start,Vector3 end, float duration ,WalkDir dir, int deep)
	{
        //Debug.Log("start=" + start+",end="+end);
		base.SetData(start,end,RoleState.JUMP,duration,-1,dir,deep);
	}
	public override   void Update () {
		base.Update();
		Vector3 pos = Vector3.Lerp(m_Start,m_End,m_TimeCount / m_Duration);
		if (m_RankDeep >= 2)
			m_RankDeep = 1;
		pos.z = Skin.s_DeepDistint * m_RankDeep;
		if (m_Dir == WalkDir.WALKSTOP)
			PlayAction(AnimatorState.Stand,pos,true);
		else
		{
			/*if(m_LifePrent != null && m_LifePrent.m_Status != null
			   && m_LifePrent.m_Status.CheckStateBySkill(1022))
			{
				PlayAction(AnimatorState.Skill01,pos,true);
				//m_Speed = w.m_Attr.SpeedPercent;
			}
			else*/
				PlayAction(AnimatorState.Walk,pos,true);
		}

	}
	public override void SetTarget (Life Parent)
	{
		base.SetTarget (Parent);
		
		/*if (m_LifePrent.m_Attr.ModelType == 100004)
			m_AniSpeed = 2;
		else
			m_AniSpeed = 1;*/
	}
	public override void Finish ()
	{
		base.Finish ();
		//m_AniSpeed = 1;
	}
	public override float GetSpeed ()
	{
		if (m_LifePrent != null)
		{
				return m_LifePrent.m_Attr.SpeedPercent;
		}
		return 1;
	}

}
public class GridActionCmdUnderWalk :GridActionCmdInterrupt{
	// Update is called once per frame
	public GridActionCmdUnderWalk(Vector3 start,Vector3 end, float duration ,WalkDir dir, int deep)
	{
		//Debug.Log("start=" + start+",end="+end);
		base.SetData(start,end,RoleState.JUMP,duration,-1,dir,deep);
	}
	public override   void Update () {
		base.Update();
		Vector3 pos = Vector3.Lerp(m_Start,m_End,m_TimeCount / m_Duration);

		pos.z = Skin.s_DeepDistint * m_RankDeep;
		if (m_Dir == WalkDir.WALKSTOP)
			PlayAction(AnimatorState.Stand,pos,true);
		else
			PlayAction(AnimatorState.UnderWalk,pos,true);
		
	}
	
}
public class GridActionCmdDie :GridActionCmd{
	// Update is called once per frame
	private bool m_FadeOut;
    private bool m_bRunOnce = true;
	public GridActionCmdDie(Vector3 pos, float duration ,WalkDir dir, int deep)
	{
		m_Start = pos;
		m_Duration = duration;
		m_Dir = dir;
		m_RankDeep = deep;
		m_FadeOut = false;
	}
	public override   void Update () {
        if (m_bRunOnce)
        {
            m_bRunOnce = false;
            base.Update();
            if (m_LifePrent != null)
            {
				(m_LifePrent as Role).SetBottomMostPos();
            }
		}
		PlayAction(AnimatorState.Die, m_Start);
	}

    public override bool IsDone()
    {
        return (m_TimeCount >= m_Duration);
    }
	
}





public class GridActionCmdInterrupt :GridActionCmd{
	// Update is called once per frame

	public void SetDone()
	{
		m_Duration = m_TimeCount;
	}
}
public class GridActionCmdJumpDown :GridActionCmd{
	// Update is called once per frame
	public GridActionCmdJumpDown(Vector3 start,Vector3 end, float duration ,WalkDir dir, int deep)
	{
		//Debug.Log("start=" + start+",end="+end);
		base.SetData(start,end,RoleState.JUMP,duration,-1,dir,deep);
	}
	public override   void Update () {
		base.Update();
		Vector3 pos = Vector3.Lerp(m_Start,m_End,m_TimeCount / 0.5f);

		pos.z = Skin.s_DeepDistint * m_RankDeep;
		PlayAction(AnimatorState.JumpDown,pos,true);
		
	}

}
public class GridActionCmdJumpUp :GridActionCmd{
	// Update is called once per frame
	
	public GridActionCmdJumpUp(Vector3 start,Vector3 end, float duration ,WalkDir dir, int deep)
	{
		//Debug.Log("start=" + start+",end="+end);
		base.SetData(start,end,RoleState.JUMP,duration,-1,dir,deep);
	}
	public override   void Update () {
		base.Update();
		Vector3 pos = Vector3.Lerp(m_Start,m_End,m_TimeCount / 0.4f);

		pos.z = Skin.s_DeepDistint* m_RankDeep;
		PlayAction(AnimatorState.JumpUp,pos,true);
		
	}
}
public class GridActionCmdReverseJump :GridActionCmd{
	// Update is called once per frame
	public delegate bool JumpAttackFun(Life target,MapGrid m);
	protected float m_EventTime;
	protected JumpAttackFun m_Fun;
	protected Life m_AttackWall;
	protected MapGrid m_FallGrid;
	float m_StartTime = 0.2f;
	float m_EndTime = 0.2f;
	float m_JumpTime;
	public GridActionCmdReverseJump(Vector3 start,Vector3 end, float duration ,WalkDir dir, int deep, float starttime, float endtime,
	                         float eventtime,JumpAttackFun fun, Life w,MapGrid m)
	{
		base.SetData(start,end,RoleState.JUMP,duration,-1,dir,deep);
		m_StartTime = starttime;
		m_EndTime = endtime;
		m_JumpTime = duration;
		m_Duration = duration + starttime + endtime;
		m_EventTime = eventtime + starttime;
		m_Fun = fun;
		m_AttackWall = w;
		m_FallGrid = m;
	}
	
	public void CheckAttack()
	{
		if (m_Fun != null)
		{
			if (m_TimeCount > m_EventTime)
				if(m_Fun(m_AttackWall,m_FallGrid))
					m_Duration = m_TimeCount;
		}
	}
	public override  void Update () {
		base.Update();
		if (m_TimeCount > (m_Duration-m_EndTime))
		{
			m_End.z = Skin.s_DeepDistint * m_RankDeep;
			m_AniSpeed = 1f;
			PlayAction(AnimatorState.UnderJump3,m_End,true);
		}
		else if (m_TimeCount < m_StartTime)
		{
			PlayAction(AnimatorState.UnderJump1,m_Start);
		}
		else
		{
			
			float height = 0.8f;
			float per = (m_TimeCount-m_StartTime) / m_JumpTime;
			Vector3 pos = Vector3.Lerp(m_Start,m_End,per);
			pos.y =m_Start.y - height * (  Mathf.Pow((per - 0.5f) / 0.5f,2) - 1);
			m_AniSpeed = 0.56667f / m_JumpTime;
			pos.z = Skin.s_DeepDistint * m_RankDeep;
			PlayAction(AnimatorState.UnderJump2,pos,true);
			CheckAttack();
		}
	}
}


//传送的动作
public class GridActionCmdSendToPos:GridActionCmdInterrupt{
	Vector3 m_startTo;
	Vector3 m_endFr;
	Vector3 m_scaleOrg;
	Vector3 m_scaleTemp;
	MapGrid m_mgSendTo;
	Transform tfRoot;
	// Update is called once per frame
	public GridActionCmdSendToPos(Life w,Vector3 start,Vector3 startTo,Vector3 endFr,Vector3 end, float duration ,WalkDir dir, int deep,MapGrid m)
	{
		SetTarget (w);
		m_mgSendTo = m;
		m_startTo = startTo;
		m_endFr = endFr;
		Transform t = m_Skin.tRoot;
		m_scaleOrg = new Vector3(t.localScale.x,t.localScale.y,t.localScale.x);
		m_scaleTemp= new Vector3(t.localScale.x/10.0f,t.localScale.y/10.0f,t.localScale.x/10.0f);
		base.SetData(start,end,RoleState.STAND,duration,-1,dir,deep);
		if (m_LifePrent is Role)
		{
			(m_LifePrent as Role).run.ClearPath();
			(m_LifePrent as Role).RoleWalk.Teleport (m_mgSendTo);
		}
		tfRoot = m_Skin.tRoot;
	}
	public override   void Update () {
		base.Update();
		//m_RankDeep = 1;
		Vector3 vpos;
		Vector3 vLocal;
		if (m_TimeCount < (m_Duration - 0.5f) / 2.0f) 
		{
				float t = m_TimeCount / ((m_Duration - 0.5f) / 2.0f);
				vpos = Vector3.Lerp (m_Start, m_startTo, t);
				vLocal = Vector3.Lerp (m_scaleOrg, m_scaleTemp, t);
			    RolePlayAnimation(AnimatorState.Stand);
		} 
		else if (m_TimeCount < (m_Duration - 0.5f)) 
		{
				float t = m_TimeCount / ((m_Duration - 0.5f) / 2.0f) - 1.0f;
				vpos = Vector3.Lerp (m_endFr, m_End, t);
				vLocal = Vector3.Lerp (m_scaleTemp, m_scaleOrg, t);
			    RolePlayAnimation(AnimatorState.Stand);
		} 
		else 
		{
			vpos = new Vector3(m_End.x,m_End.y,m_End.z);
			vLocal = new Vector3( m_scaleOrg.x,m_scaleOrg.y,m_scaleOrg.z);
			RolePlayAnimation(AnimatorState.FlyFallStand00200);
		}
		vpos.z = Skin.s_DeepDistint * m_RankDeep;
		tfRoot.localScale = vLocal;
		tfRoot.localPosition = vpos;
		
	}
	public override bool IsDone()
	{
		bool ret = base.IsDone ();
	//	if(ret)
	//		roleActionWalk.Teleport (m_mgSendTo);
		return ret;
	}
	public void SetDone()
	{
		//roleActionWalk.Teleport (m_mgSendTo);
		base.SetDone ();
		Vector3 vpos = new Vector3(m_End.x,m_End.y,m_End.z);
		Vector3 vLocal = new Vector3( m_scaleOrg.x,m_scaleOrg.y,m_scaleOrg.z);
		tfRoot.localScale = vLocal;
		tfRoot.localPosition = vpos;
	}
}
public class GridActionCmdWin :GridActionCmd{
	// Update is called once per frame
	public GridActionCmdWin()
	{
		//Debug.Log("start=" + start+",end="+end);
		m_Duration = 1f;
	}
    public override void Update()
    {
        RolePlayAnimation(AnimatorState.WinStart);		
	}

    public override void Finish()
    {
        base.Finish();
       // RolePlayAnimation(AnimatorState.WinLoop);
    }
}
public class GridActionCmdFaile :GridActionCmd{
	// Update is called once per frame
	public GridActionCmdFaile()
	{
		//Debug.Log("start=" + start+",end="+end);
		
		m_Duration = 99999f;
	}
    public override void Update()
    {
		RolePlayAnimation(AnimatorState.LoseStart);
		
		
	}
}