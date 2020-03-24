using UnityEngine;
using System.Collections;
/// <summary>
/// 黑胡子技能 编号103002.
/// </summary>
/// 
/// //1101 普攻.
public class GridActionCmd103002SkillNomalAttack :GridActionCmdAttack{
	public GridActionCmd103002SkillNomalAttack(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{

		m_CastTime = 0.7f;
		m_EventTime = 0.7f;
		m_Duration = 1.333f;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		
		PlayAction(AnimatorState.Attack85000,m_Start);
		GameObject gpos2 = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
		string effectName = m_Dir == WalkDir.WALKLEFT?"1105061_01":"1105061_02";
		if (gpos2 != null)
		{
			GameObjectActionExcute gae2 = EffectM.LoadEffect(EffectM.sPath,effectName,gpos2.transform.position, gpos2.transform);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(m_Duration);
			gae2.AddAction(ndEffect);
			m_effectgo = gae2.gameObject;
		}

	}
	public override   void DoUpdate () {
		
	}
	
	public override void DoEvent()
	{
		base.DoEvent();
	}
	
}
/// <summary>
/// 黑胡子大招 1102.
/// </summary>
public class GridActionCmd103002BigSkill :GridActionCmdActiveSkill{
	
	float m_walktime;
	public GridActionCmd103002BigSkill(DoQianyaoFun qianyaofun,DoAttackFun fun,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
		:base( qianyaofun, fun,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime)
	{
		m_CastTime = 0.667f;
		m_EventTime = 0.667f;
		m_Duration = 4f;
		m_walktime = 0f;
		m_Dir = AttackDir;
	}
	public override void ActiiveStart()
	{
		PlayAction(AnimatorState.Skill01,m_Start);
		
		//string effName = m_Dir == WalkDir.WALKLEFT?"1402051_01":"1405051_02";
		GameObject gpos = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.LeftHandPos);
		if (gpos != null)
		{
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1105041_01", gpos.transform.position, gpos.transform);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(m_Duration);
			gae.AddAction(ndEffect);
			m_effectgo = gae.gameObject;
		}
		GameObject gpos2 = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
		if (gpos2 != null)
		{
			GameObjectActionExcute gae2 = EffectM.LoadEffect(EffectM.sPath,"1105041_02",gpos2.transform.position, gpos2.transform);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(m_Duration);
			gae2.AddAction(ndEffect);
			m_effectgo = gae2.gameObject;
		}
	}
	public override void RangeTarget (System.Collections.Generic.List<Life> llist)
	{
		base.RangeTarget (llist);
		
		/*foreach(Life life in llist)
		{
			GameObject go = life.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
			
			GameObjectActionExcute Tgae = EffectM.LoadEffect(EffectM.sPath,"1402061",go.transform.position, go.transform);
		}*/
	}
	public override void DoAttackEvent()
	{
		
	}
	public override void UpdatePos()
	{
//		if (m_TimeCount < m_EventTime)
//		{
//			PlayAction(AnimatorState.PreSkill01,m_Start);
//		}
//		else
//		{
//			//Vector3	pos = Vector3.Lerp(m_Start,m_End,(m_TimeCount - m_EventTime) / m_walktime );
//
//			//AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_SELFSP),m_LifePrent);
//		}
	}
}

/// <summary>
/// 黑胡子技能1 船绳飞舞1103.
/// </summary>
public class GridActionCmd103002Skill1103 :GridActionCmdAttack{
	public GridActionCmd103002Skill1103(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		m_CastTime = 1.2f;
		m_EventTime = 1.2f;
		m_Duration = 1.667f;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		
		PlayAction(AnimatorState.Attack81000,m_Start);
		
		GameObject gpos = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.FirePos);
		if (gpos != null)
		{
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1105021_01", gpos.transform.position, gpos.transform);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(m_Duration);
			gae.AddAction(ndEffect);
			m_effectgo = gae.gameObject;
		}
		GameObject gpos2 = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
		if (gpos2 != null)
		{
			GameObjectActionExcute gae2 = EffectM.LoadEffect(EffectM.sPath,"1105021_02",gpos2.transform.position, gpos2.transform);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(m_Duration);
			gae2.AddAction(ndEffect);
			m_effectgo = gae2.gameObject;
		}

	}
	public override   void DoUpdate () {
		
	}
	
	public override void DoEvent()
	{
		base.DoEvent();
	}
	
}

/// <summary>
/// 黑胡子技能2 胡子踹 1104.
/// </summary>
public class GridActionCmd103002Skill1104 :GridActionCmdAttack{
	public GridActionCmd103002Skill1104(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		
		m_CastTime = 0.533f;
		m_EventTime = 0.533f;
		m_Duration = 1.5f;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		
		PlayAction(AnimatorState.Attack82000,m_Start);
		
		GameObject gpos2 = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
		if (gpos2 != null)
		{
			GameObjectActionExcute gae2 = EffectM.LoadEffect(EffectM.sPath,"1105011",gpos2.transform.position, gpos2.transform);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(m_Duration);
			gae2.AddAction(ndEffect);
			m_effectgo = gae2.gameObject;
		}
		
	}
	public override   void DoUpdate () {
		
	}
	
	public override void DoEvent()
	{
		base.DoEvent();
	}
}
/// <summary>
/// 黑胡子技能2 胡子踹 1106,.1104击退之后产生的伤害.
/// </summary>
public class GridActionCmd103002Skill1106 :GridActionCmdAttack{
	public GridActionCmd103002Skill1106(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		
		m_CastTime = 0.833f;
		m_EventTime = 0.833f;
		m_Duration = 1.333f;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		
//		PlayAction(AnimatorState.Attack85000,m_Start);
//		
//		GameObject gpos = m_skillinfo.m_LifeTarget.m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectPos);
//		string name = "1104041_01";
//		if (m_LifePrent.WalkDir == WalkDir.WALKRIGHT)
//			name = "1104041_02";
//		GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, name, gpos.transform.position, gpos.transform);
//		GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(180.0f);
//		m_effectgo = gae.gameObject;
		
	}
	public override   void DoUpdate () {
		
	}
	
	public override void DoEvent()
	{
		base.DoEvent();
	}
}

