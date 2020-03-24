using UnityEngine;
using System.Collections;
// 拆解机器人攻击 7000
public class GridActionCmd3000Skill7000 :GridActionCmdAttack{
	public GridActionCmd3000Skill7000(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		
		m_CastTime = 0.5f;
		m_EventTime = 0.5f;
		m_Duration = 1f;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		
		PlayAction(AnimatorState.Attack85000,m_Start);
	}
	public override   void DoUpdate () {
		
	}
	
	public override void DoEvent()
	{
		base.DoEvent();
	}
	public override void Finish ()
	{
		base.Finish ();
	}
}
// 拆解机器人攻击 7000
public class GridActionCmd3000Appear :GridActionCmd{
	Vector3 start = Vector3.zero;
	Vector3 end = Vector3.zero;
	public GridActionCmd3000Appear( Vector3 e )
	{
		start = e;
		start.z = Camera.main.transform.position.z;
		end = e;
		m_Duration = 2.5f;
	}

	public override void Update ()
	{
		base.Update ();
		if (m_TimeCount < 1.0f)
		{
			m_LifePrent.m_thisT.position = Vector3.Lerp(start,end,m_TimeCount/1.0f);
			PlayAction(AnimatorState.FlyToAttack00010,m_Start,false);
		}
		else
			PlayAction(AnimatorState.FlyFallStand00210,m_Start,false);
	}
	

	public override void Finish ()
	{
		base.Finish ();
	}
}