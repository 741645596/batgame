using UnityEngine;
using System.Collections;
/// <summary>
/// 豆豆技能 编号100001
/// </summary>
/// 
public class GridActionCmd100001Skill01 :GridActionCmdAttack{
	public GridActionCmd100001Skill01(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		
		m_CastTime = 0.53f;
		m_EventTime = 0.53f;
		m_Duration = 1.0f;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		
		GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
		Transform t = posgo.transform;
		if (t != null)
		{
			//m_effectgo = SkillEffects._instance.LoadEffect("effect/prefab/", "1001031",t.position,m_Duration);
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, m_LifePrent.WalkDir == WalkDir.WALKLEFT? "1001131_01" : "1001131_02", t.position, t);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(m_Duration);
			gae.AddAction(ndEffect);
            m_effectgo = gae.gameObject;
		}
		PlayAction(AnimatorState.Attack85000,m_Start);
	}
	public override   void DoUpdate () {
		
	}
	
	public override void DoEvent()
	{
		base.DoEvent();
        SoundPlay.Play("broadsword_atc", false, false);
	}
	
}
//雪球
public class GridActionCmd100001Skill02 :GridActionCmdAttack{
	public GridActionCmd100001Skill02(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		
		m_CastTime = 0.6f;
		m_EventTime = 0.6f;
		m_Duration = 1.267f;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);

		PlayAction(AnimatorState.Attack81000,m_Start);
	}
	public override   void DoUpdate () {
		
	}
	
	public override void DoEvent()
	{
		base.DoEvent();
	}
	
}
//挑击
public class GridActionCmd100001Skill03 :GridActionCmdAttack{
	public GridActionCmd100001Skill03(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		
		m_CastTime = 0.9f;
		m_EventTime = 0.9f;
		m_Duration = 1.5f;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
		Transform t = posgo.transform;
		if (t != null)
		{
			//m_effectgo = SkillEffects._instance.LoadEffect("effect/prefab/", "1001031",t.position,m_Duration);
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, m_LifePrent.WalkDir == WalkDir.WALKLEFT? "1001121_01" : "1001121_02", t.position, t);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(m_Duration);
			gae.AddAction(ndEffect);
			m_effectgo = gae.gameObject;
		}
		PlayAction(AnimatorState.Attack82000,m_Start);
	}
	public override   void DoUpdate () {
		
	}
	
	public override void DoEvent()
	{
		base.DoEvent();
	}
	
}

//单体砸晕 1037
public class GridActionCmd100001Skill1037 :GridActionCmdAttack{
	public GridActionCmd100001Skill1037(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		
		m_CastTime = 0f;
		m_EventTime = 0.6f;
		m_Duration = 1.333f;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		/*Transform t = m_Skin.tRoot;
		if (t != null)
		{
			//m_effectgo = SkillEffects._instance.LoadEffect("effect/prefab/", "1001031",t.position,m_Duration);
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1001201", t.position, BattleEnvironmentM.GetLifeMBornNode(true));
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(m_Duration);
			gae.AddAction(ndEffect);
			m_effectgo = gae.gameObject;
			if(m_Dir == WalkDir.WALKRIGHT)
				m_effectgo.transform.Rotate(new Vector3(0,180,0),Space.Self);
		}*/
		PlayAction(AnimatorState.Attack84000,m_Start);
//		SoundPlay.Play("skill_duang",false,false);
	}
	public override   void DoUpdate () {
		
	}
	
	public override void DoEvent()
	{
		base.DoEvent();
	}
	
}
//远程普攻
public class GridActionCmd100001LSkill01 :GridActionCmdAttack{
	public GridActionCmd100001LSkill01(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		
		m_CastTime = 0.5f;
		m_EventTime = 0.5f;
		m_Duration = 1f;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		
		PlayAction(AnimatorState.Attack83000,m_Start);
	}
	public override   void DoUpdate () {
		
	}

	
}
public class GridActionCmd100001ActiveSkill :GridActionCmdActiveSkill{
	

	
	public GridActionCmd100001ActiveSkill(DoQianyaoFun qianyaofun,DoAttackFun fun,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
		:base( qianyaofun, fun,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime)
	{
		m_CastTime = 1.2f;
		m_EventTime = 1.2f;
		m_Duration = 1.933f;
	}
	public override void ActiiveStart()
	{		
		SoundPlay.Play("skill_duang", false, false);	
		Transform t = null;
		RolePropertyM rpm = (m_Skin as RoleSkin).GetRolePropertyM();		
		GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
		if (posgo != null)
		{
			t = posgo.transform;
			
			//m_effectgo = SkillEffects._instance.LoadEffect("effect/prefab/", "1001081",t.position,0.75f,true);
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, m_LifePrent.WalkDir == WalkDir.WALKLEFT? "1001111_01" : "1001111_02", t.position, t);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(m_Duration);
			gae.AddAction(ndEffect);
			m_effectgo = gae.gameObject;
		}
	}
	public override void UpdatePos()
	{
		if (m_TimeCount < m_CastTime)
		{
			
			PlayAction(AnimatorState.Skill01,m_Start);
		}
		else if (m_TimeCount < m_Duration)
		{
			PlayAction(AnimatorState.Skill01,m_Start);
			//2015.4.22 by txm replce
			//分支start
			//if((m_TimeCount - m_Delatime) < (m_EventTime +0.4f) && m_TimeCount > (m_EventTime +0.4f))
			/*if((m_TimeCount - m_Delatime) < (m_EventTime ) && m_TimeCount > (m_EventTime ))
			//分支end
			{
				Transform t = null;
				RolePropertyM rpm = m_Skin.iGameRole.GetRolePropertyM();		
				GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.bagPos);
				if (posgo != null)
				{
					t = posgo.transform;

					//m_effectgo = SkillEffects._instance.LoadEffect("effect/prefab/", "1001051",t.position,1f,true);
					GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1001051", t.position, BattleEnvironmentM.GetLifeMBornNode(true));
					GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
					gae.AddAction(ndEffect);
                    m_effectgo = gae.gameObject;
					SoundPlay.JoinPlayQueue("hammer_hit", 1.5f);
				}
			}*/
			
			/*if((m_TimeCount - m_Delatime) < (m_EventTime +0.3f) && m_TimeCount > (m_EventTime +0.3f))
			{
				Transform t = null;
				RolePropertyM rpm = m_Skin.iGameRole.GetRolePropertyM();		
				GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectTopPos);
				if (posgo != null)
				{
					t = posgo.transform;
					
					//m_effectgo = SkillEffects._instance.LoadEffect("effect/prefab/", "1001081",t.position,0.75f,true);
					GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1001081", t.position, BattleEnvironmentM.GetLifeMBornNode(true));
					GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(0.75f);
					gae.AddAction(ndEffect);
                    m_effectgo = gae.gameObject;
				}
			}*/
		}
	}
	public override void DoEvent()
	{

	}

	
}
