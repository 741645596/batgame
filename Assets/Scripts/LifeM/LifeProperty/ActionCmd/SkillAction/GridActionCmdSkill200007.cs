using UnityEngine;
using System.Collections;
/// <summary>
/// 弓箭企鹅技能 编号200007.
/// </summary>
/// 
/// //5007 普攻.
public class GridActionCmd200007Skill5007 :GridActionCmdAttack{
	public GridActionCmd200007Skill5007(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		
		m_CastTime = 0.833f;
		m_EventTime = 0.833f;
		m_Duration = 1.0f;
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