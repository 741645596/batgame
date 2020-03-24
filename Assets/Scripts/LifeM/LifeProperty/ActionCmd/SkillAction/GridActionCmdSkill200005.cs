using UnityEngine;
using System.Collections;
/// <summary>
/// 刀盾企鹅技能 编号200005.
/// </summary>
/// 
/// //5008 普攻.
public class GridActionCmd200005Skill5008 :GridActionCmdAttack{
	public GridActionCmd200005Skill5008(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
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