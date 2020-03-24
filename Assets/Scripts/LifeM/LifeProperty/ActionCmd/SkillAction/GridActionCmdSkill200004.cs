using UnityEngine;
using System.Collections;
/// <summary>
/// 企鹅医师技能 编号200004
/// </summary>
/// 
/// //5005 普攻
public class GridActionCmd200004Skill5005 :GridActionCmdAttack{
	public GridActionCmd200004Skill5005(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		
		m_CastTime = 0.833f;
		m_EventTime = 0.833f;
		m_Duration = 1.367f;
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
	
}
public class GridActionCmd200004Skill5006 : GridActionCmdConditionSkill {
	public GridActionCmd200004Skill5006(Vector3 start, DoAttackFun fun, WalkDir dir,DoQianyaoFun qianyaofun)
	{
		
		m_CastTime = m_eventtime = 0.66f;
		m_Duration = 1.333f;
		m_Start = start;
		m_End = start;
		m_Dir = dir;
		m_DoSkill = fun;
		m_QianYaoStatus = qianyaofun;
	}
	
	public override void UpdatePos ()
	{
		base.UpdatePos ();
		PlayAction(AnimatorState.Attack81000,m_Start);
	}
	
	public override void DoEvent()
	{
		base.DoEvent();
	}
	
}