using UnityEngine;
using System.Collections;
//5000企鹅巫师普攻
public class GridActionCmd200002Skill5000 :GridActionCmdAttack{
	public GridActionCmd200002Skill5000(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		
		m_CastTime = 0.43f;
		m_EventTime = 0.43f;
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
	
}

//5001 企鹅巫师呼叫增援
public class GridActionCmd200002Skill5001 :GridActionCmdConditionSkill{
	public GridActionCmd200002Skill5001(Vector3 start,DoAttackFun fun,DoQianyaoFun qianyaofun)
	{
		m_Start = start;
		m_End = start;
		m_CastTime = 0;
		m_eventtime = 0.3f;
		m_realDuration = 0.833f;
		m_Duration = m_realDuration;
		m_DoSkill = fun;
		m_QianYaoStatus = qianyaofun;
		
		//Debug.Log("  企鹅巫师呼叫增援 ");
	}
	public override void UpdatePos()
	{
		PlayAction(AnimatorState.Attack81000,m_Start);
	}
	
}

//5002 企鹅巫师标记
public class GridActionCmd200002Skill5002 :GridActionCmdConditionSkill{
	public GridActionCmd200002Skill5002(Vector3 start,DoAttackFun fun,DoQianyaoFun qianyaofun)
	{
		m_Start = start;
		m_End = start;
		m_CastTime = 0;
		m_eventtime = 0.6f;
		m_realDuration = 1.167f;
		m_Duration = m_realDuration;
		m_DoSkill = fun;
		m_QianYaoStatus = qianyaofun;
		
		//Debug.Log("  企鹅巫师标记 ");
	}
	
	public override void UpdatePos()
	{
		PlayAction(AnimatorState.Attack82000,m_Start);
	}
	
	
	
}
