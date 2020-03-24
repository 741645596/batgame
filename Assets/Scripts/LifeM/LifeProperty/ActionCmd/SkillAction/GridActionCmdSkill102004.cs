#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#define UNITY_EDITOR_LOG
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 维京熊技能 编号102004
/// </summary>

// 1071 普攻
public class GridActionCmd102004Skill1071 :GridActionCmdAttack{
	public GridActionCmd102004Skill1071(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		
		m_CastTime = 0.4f;
		m_EventTime = 0.4f;
		m_Duration = 1f;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);

		PlayAction(AnimatorState.Attack85000,m_Start);
		SoundPlay.Play("atc_viking",false,false);
		if (m_LifePrent.m_Status.HaveState(StatusType.Berserker))
		{
			GameObject gpos = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1104031", gpos.transform.position, gpos.transform);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(180.0f);

			m_effectgo = gae.gameObject;
		}
		else
		{
			GameObject gpos = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1104021", gpos.transform.position, gpos.transform);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(180.0f);
			m_effectgo = gae.gameObject;
		}
	}
	public override   void DoUpdate () {
		
		PlayAction(AnimatorState.Attack85000,m_Start);
	}
	
	public override void DoEvent()
	{
		base.DoEvent();
	}
	
}

// 1074 盾击
public class GridActionCmd102004Skill1074 :GridActionCmdAttack{
	public GridActionCmd102004Skill1074(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		
		m_CastTime = 0.4f;
		m_EventTime = 0.4f;
		m_Duration = 1f;
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
		if (m_skillinfo.m_LifeTarget is Role)
		{
			GameObject gpos = m_skillinfo.m_LifeTarget.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectPos);
			string name = "1104041_01";
			if (m_LifePrent.WalkDir == WalkDir.WALKRIGHT)
				name = "1104041_02";
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, name, gpos.transform.position, gpos.transform);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(180.0f);
			m_effectgo = gae.gameObject;
		}
	}
	
}
//1073 瘫痪
public class GridActionCmd102004Skill1073 :GridActionCmdConditionSkill{
	public GridActionCmd102004Skill1073(Vector3 start,DoAttackFun fun,DoQianyaoFun qianyaofun)
	{
		m_Start = start;
		m_End = start;
		m_CastTime = m_eventtime = 0.67f;
		m_realDuration = 1.5f;
		m_Duration = m_realDuration;
		m_DoSkill = fun;
		m_QianYaoStatus = qianyaofun;
		
		//Debug.Log("  大斜八块 ");
	}
	public override void StartWithTarget (Life Parent)
	{
		base.StartWithTarget (Parent);
		m_Dir = Parent.WalkDir;
		
		GameObject gpos = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
		string name = "1104081";
		if (Parent.m_Core.m_bTurn)
			name = "1104071";
		GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, name, gpos.transform.position, gpos.transform);
		m_effectgo = gae.gameObject;
	}
	public override void UpdatePos()
	{
		PlayAction(AnimatorState.Attack84000,m_Start);
	}
	public override void DoEvent ()
	{
		base.DoEvent ();
		if (!m_skillinfo.m_LifeTarget.isDead) 
		{
			Vector3 pos = m_skillinfo.m_LifeTarget.m_thisT.position;
			pos += new Vector3(1.5f,1.5f,-2);
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1104051", pos, BattleEnvironmentM.GetLifeMBornNode(true));
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(180.0f);
		}
	}
	
}

// 1072 战斗本能
public class GridActionCmd102004Skill1072 :GridActionCmdAttack{
	public GridActionCmd102004Skill1072(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		
		m_CastTime = 3f;
		m_EventTime = 3f;
		m_Duration = 3f;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		
		PlayAction(AnimatorState.Attack83000,m_Start);
		
		GameObject gpos = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
		GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1104061", gpos.transform.position, gpos.transform);
		GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(180.0f);
		m_effectgo = gae.gameObject;
	}
	public override   void DoUpdate () {
		
	}
	
	public override void DoEvent()
	{
		base.DoEvent();
	}
	
}
public class GridActionCmd102004ActiveSkill :GridActionCmdActiveSkill{
	
	float m_walktime;
	public GridActionCmd102004ActiveSkill(DoQianyaoFun qianyaofun,DoAttackFun fun,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
		:base( qianyaofun, fun,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime)
	{
		m_CastTime = 1.667f;
		m_EventTime =1.667f;
		m_Duration = 1.667f;
		m_walktime = 0f;
		m_Dir = AttackDir;
	}
	public override void ActiiveStart()
	{
		
		SoundPlay.Play("skill_viking", false, false);
		
		GameObject gpos = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
		string name = "1104061_01";
		if (m_LifePrent.WalkDir == WalkDir.WALKRIGHT )
			name = "1104061_02";
		GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, name, gpos.transform.position, gpos.transform);
		GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(180.0f);
		m_effectgo = gae.gameObject;
	}
	
	public override void DoAttackEvent()
	{

	}
	public override void UpdatePos()
	{
		if (m_TimeCount < m_EventTime)
		{
			PlayAction(AnimatorState.Skill01,m_Start);
		}
		else
        {
            
    
			//Vector3	pos = Vector3.Lerp(m_Start,m_End,(m_TimeCount - m_EventTime) / m_walktime );
			PlayAction(AnimatorState.Skill01,m_Start);
			//AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_SELFSP),m_LifePrent);
		}
	}
}
public class TurnBack102004 : GridActionCmd
{
	public TurnBack102004()
	{
		m_Duration = 1f;
	}
	public override void SetTarget (Life Parent)
	{
		base.SetTarget (Parent);
		m_Skin.PlayAnimation(AnimatorState.Attack83000);
	}
	public override void Finish ()
	{
		base.Finish ();
		foreach (GameObject go in m_Skin.ProPerty.roleProperties)
		{
			Animator ani = go.GetComponent<Animator>();
			if (ani != null)
			{
				AnimatorOverrideController  controller = Resources.Load<RuntimeAnimatorController>("AnimaCtrl/Roles/102004@Over") as AnimatorOverrideController;
				ani.runtimeAnimatorController = controller;
				m_Skin.PlayAnimation(AnimatorState.Stand);
			}
		}
	}
}