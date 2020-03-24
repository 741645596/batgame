#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#define UNITY_EDITOR_LOG
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 绿巨熊技能 编号102003 
/// </summary>
/// 
/// //科学家普攻 1061
public class GridActionCmd102003Skill1061 :GridActionCmdAttack{
	public GridActionCmd102003Skill1061(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		
		m_CastTime = 0.5f;
		m_EventTime = 0.5f;
		m_Duration = 2f;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);

		PlayAction(AnimatorState.Attack85000,m_Start);
		SoundPlay.Play("atc_hulk",false,false);

		GameObject posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
		if (posgo != null)
		{
			Vector3 pos = posgo.transform.position;
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1103061", pos, posgo.transform);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
			gae.AddAction(ndEffect);
			m_effectgo = gae.gameObject;

		}

	}
	public override   void DoUpdate () {
		if (m_TimeCount < 1f)
		{
			PlayAction(AnimatorState.Attack85000,m_Start);
		}
		else
		{
			PlayAction(AnimatorState.Attack85100,m_Start);
			if ((m_TimeCount - m_Delatime) < 1f)
			{
				GameObject posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
				if (posgo != null)
				{
					Vector3 pos = posgo.transform.position;
					GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1103071", pos, posgo.transform);
					GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
					gae.AddAction(ndEffect);
					m_effectgo = gae.gameObject;
				}

			}
		}
	}
	
	public override void DoEvent()
	{
		base.DoEvent();
	}
	
}
/// //科学家普攻 1062
public class GridActionCmd102003Skill1062 :GridActionCmdAttack{
	public GridActionCmd102003Skill1062(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		
		m_CastTime = 0.3f;
		m_EventTime = 0.3f;
		m_Duration = 3.333f;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		
		PlayAction(AnimatorState.Attack85000,m_Start);
		SoundPlay.Play("atc_hulk_02",false,false);
		GameObject posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
		if (posgo != null)
		{
			Vector3 pos = posgo.transform.position;
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1103031", pos, posgo.transform);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
			gae.AddAction(ndEffect);
			m_effectgo = gae.gameObject;
		}
	}
	public override   void DoUpdate () {
		
		if (m_TimeCount < 1f)
		{
			PlayAction(AnimatorState.Attack85000,m_Start);
		}
		else if (m_TimeCount < 2f)
		{
			PlayAction(AnimatorState.Attack85100,m_Start);			
			if ((m_TimeCount - m_Delatime) < 1f)
			{
				GameObject posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
				if (posgo != null)
				{
					Vector3 pos = posgo.transform.position;
					GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1103041", pos, posgo.transform);
					GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
					gae.AddAction(ndEffect);
					m_effectgo = gae.gameObject;
				}
				
			}
		}
		else
		{
			PlayAction(AnimatorState.Attack85200,m_Start);
			
			if ((m_TimeCount - m_Delatime) < 2f)
			{
				GameObject posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
				if (posgo != null)
				{
					Vector3 pos = posgo.transform.position;
					GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1103051", pos, posgo.transform);
					GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
					gae.AddAction(ndEffect);
					m_effectgo = gae.gameObject;
				}

			}
		}
	}
	
	public override void DoEvent()
	{
		base.DoEvent();
	}
	public override void DoAttackEvent()
	{
		base.DoAttackEvent();
		if (m_effectcount == 3)
		{
			GameObject posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.FirePos);
			if (posgo != null)
			{
				Vector3 pos = posgo.transform.position;
				GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1103121", pos, posgo.transform);
				GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
				gae.AddAction(ndEffect);
				m_effectgo = gae.gameObject;
			}
		}
	}
	
}
// 手足之情 1067
public class GridActionCmd102003ConditionSkill1067 :GridActionCmdConditionSkill{
	public GridActionCmd102003ConditionSkill1067(Vector3 start,DoAttackFun fun,DoQianyaoFun qianyaofun)
	{
		m_Start = start;
		m_End = start;
		m_CastTime = m_eventtime = 0.5f;
		m_realDuration = 0.833f;
		m_Duration = m_realDuration;
		m_DoSkill = fun;
		m_QianYaoStatus = qianyaofun;
		
		//Debug.Log("  大斜八块 ");
	}
	public override void UpdatePos()
	{
		PlayAction(AnimatorState.Attack82000,m_Start);
	}
	
}
// 暴怒 1066
public class GridActionCmd102003ConditionSkill1066 :GridActionCmdConditionSkill{
	public GridActionCmd102003ConditionSkill1066(Vector3 start,DoAttackFun fun,DoQianyaoFun qianyaofun)
	{
		m_Start = start;
		m_End = start;
		m_CastTime = m_eventtime = 0.5f;
		m_realDuration = 0.833f;
		m_Duration = m_realDuration;
		m_DoSkill = fun;
		m_QianYaoStatus = qianyaofun;
		
	}

	public override void UpdatePos()
	{
		PlayAction(AnimatorState.Attack83000,m_Start);
		/*Vector3 pos = Vector3.Lerp(m_Start,m_End,(m_TimeCount - m_oldduration) / (m_Duration - m_oldduration));
		if (m_TimeCount <= m_eventtime){
			if (m_Start != m_End)
				PlayAction(AnimatorState.Attack81000,pos,true,AnimatorState.Walk);
			else
				PlayAction(AnimatorState.Attack81000,pos,true,AnimatorState.Stand);
		}
		else{
			if (m_Start != m_End)
				PlayAction(AnimatorState.Attack81010,pos,true,AnimatorState.Walk);
			else
				PlayAction(AnimatorState.Attack81010,pos,true,AnimatorState.Stand);
		}*/
	}



}
// 怒火冲天 1063
public class GridActionCmd102003ActiveSkill :GridActionCmdActiveSkill{
	float m_Effecttime;
	public GridActionCmd102003ActiveSkill(DoQianyaoFun qianyaofun,DoAttackFun fun,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
		:base( qianyaofun, fun,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime)
	{
		m_CastTime = 1f;
		m_EventTime = 1f;
		m_Duration = 2.5f;
		m_Dir = AttackDir;
		m_Effecttime = m_EventTime - 0.5f;
	}
	public override void ActiiveStart()
	{
		SoundPlay.Play("skill_hulk",false,false);
	}
	
	public override void DoAttackEvent()
	{
		
		(m_LifePrent as Role).TurnsInto(true);
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


		if (m_TimeCount > m_Effecttime && (m_TimeCount-m_Delatime) <= m_Effecttime)
		{
			
			GameObject posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
			if (posgo != null)
			{
				Vector3 pos = posgo.transform.position;
				pos.z -=4f;
				string name = "1103101_01";
				if (m_LifePrent.WalkDir == WalkDir.WALKRIGHT)
					name = "1103101_02";
				GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath,name , pos, BattleEnvironmentM.GetLifeMBornNode(true));
				GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1.5f);
				gae.AddAction(ndEffect);
				m_effectgo = gae.gameObject;
			}
		}
	}
}



