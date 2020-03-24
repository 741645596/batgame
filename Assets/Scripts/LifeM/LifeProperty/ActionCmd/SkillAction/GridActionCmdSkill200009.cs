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
public class GridActionCmd200009Skill1061 :GridActionCmdAttack{
	public GridActionCmd200009Skill1061(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
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
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1401061", pos, posgo.transform);
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
					GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1401071", pos, posgo.transform);
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
public class GridActionCmd200009Skill1062 :GridActionCmdAttack{
	public GridActionCmd200009Skill1062(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
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
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1401031", pos, posgo.transform);
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
					GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1401041", pos, posgo.transform);
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
					GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1401051", pos, posgo.transform);
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
				GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1401121", pos, posgo.transform);
				GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
				gae.AddAction(ndEffect);
				m_effectgo = gae.gameObject;
			}
		}
	}
	
}

public class GridActionCmd200009ConditionSkill1063 :GridActionCmdConditionSkill{
	float m_Effecttime;
	public GridActionCmd200009ConditionSkill1063(Vector3 start,DoAttackFun fun,DoQianyaoFun qianyaofun)
	{
		m_Start = start;
		m_End = start;
		m_CastTime = m_eventtime = 1f;
		m_realDuration = 2.5f;
		m_Duration = m_realDuration;
		m_DoSkill = fun;
		m_QianYaoStatus = qianyaofun;
		m_Effecttime = m_eventtime - 0.5f;
		
	}
	public override void StartWithTarget (Life Parent)
	{
		base.StartWithTarget (Parent);
		SoundPlay.Play("skill_hulk",false,false);
	}
	public override void DoEvent ()
	{
		base.DoEvent ();
		(m_LifePrent as Role).TurnsInto(true);
	}
	public override void UpdatePos()
	{
		if (m_TimeCount < m_eventtime)
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
				string name = "1401101_01";
				if (m_LifePrent.WalkDir == WalkDir.WALKRIGHT)
					name = "1401101_02";
				GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath,name , pos, BattleEnvironmentM.GetLifeMBornNode(true));
				GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1.5f);
				gae.AddAction(ndEffect);
				m_effectgo = gae.gameObject;
			}
		}
	}
}

//boss 技能------------------------------------------------------------
// 回血技能  5013 5014 5015
public class GridActionCmd200009ConditionSkill5013 :GridActionCmdConditionSkill{
	public GridActionCmd200009ConditionSkill5013(Vector3 start,DoAttackFun fun,DoQianyaoFun qianyaofun)
	{
		m_Start = start;
		m_End = start;
		m_CastTime = m_eventtime = 2.3f;
		m_realDuration = 3f;
		m_Duration = m_realDuration;
		m_DoSkill = fun;
		m_QianYaoStatus = qianyaofun;
		
	}
	public override void StartWithTarget (Life Parent)
	{
		base.StartWithTarget (Parent);
		//		SoundPlay.Play("skill_biochemical_boss",false,false);
		m_LifePrent.m_thisT.position += new Vector3(0,0,-1.5f);
		GameObject posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.Bone1Pos);
		if (posgo != null)
		{
			Vector3 pos = posgo.transform.position;
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1401161", pos, posgo.transform);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(m_Duration);
			gae.AddAction(ndEffect);
			m_effectgo = gae.gameObject;
		}
	}
	public override void UpdatePos()
	{
		PlayAction(AnimatorState.Attack84000,m_Start);
	}
	public override void Finish ()
	{
		base.Finish ();
		m_LifePrent.m_thisT.position += new Vector3(0,0,1.5f);
	}
}
// 回血技能 5016 5017 5018
public class GridActionCmd200009ConditionSkill5016 :GridActionCmdConditionSkill{
	public GridActionCmd200009ConditionSkill5016(Vector3 start,DoAttackFun fun,DoQianyaoFun qianyaofun)
	{
		m_Start = start;
		m_End = start;
		m_CastTime = m_eventtime = 2.3f;
		m_realDuration = 3f;
		m_Duration = m_realDuration;
		m_DoSkill = fun;
		m_QianYaoStatus = qianyaofun;
		
	}
	
	public override void StartWithTarget (Life Parent)
	{
		base.StartWithTarget (Parent);
		SoundPlay.Play("skill_biochemical_boss",false,false);
		m_LifePrent.m_thisT.position += new Vector3(0,0,-1.5f);
		GameObject posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.Bone1Pos);
		if (posgo != null)
		{
			Vector3 pos = posgo.transform.position;
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1401141", pos, posgo.transform);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(m_Duration);
			gae.AddAction(ndEffect);
			m_effectgo = gae.gameObject;
		}
	}
	public override void UpdatePos()
	{
		PlayAction(AnimatorState.Attack84000,m_Start);
	}
	public override void Finish ()
	{
		base.Finish ();
		m_LifePrent.m_thisT.position += new Vector3(0,0,1.5f);
	}
}
//怒吼5011
public class GridActionCmd200009Skill5011 :GridActionCmdAttack{
	public GridActionCmd200009Skill5011(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		
		m_CastTime = 1.3f;
		m_EventTime = 1.3f;
		m_Duration = 2.167f;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		
		PlayAction(AnimatorState.Attack86000,m_Start);
		
		GameObject posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
		if (posgo != null)
		{
			Vector3 pos = posgo.transform.position;
			string name = "1401181_01";
			if (m_LifePrent.WalkDir == WalkDir.WALKRIGHT)
				name = "1401181_02";
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, name, pos, posgo.transform.GetChild(0));
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(m_Duration);
			gae.AddAction(ndEffect);
			m_effectgo = gae.gameObject;
		}
		
	}
	public override   void DoUpdate () {
		/*if (m_TimeCount < 1f)
		{
			PlayAction(AnimatorState.Attack85000,m_Start);
		}
		else
		{
			PlayAction(AnimatorState.Attack85100,m_Start);
			if ((m_TimeCount - m_Delatime) < 1f)
			{
				GameObject posgo = m_LifePrent.m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
				if (posgo != null)
				{
					Vector3 pos = posgo.transform.position;
					GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1103071", pos, posgo.transform);
					GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
					gae.AddAction(ndEffect);
					m_effectgo = gae.gameObject;
				}
				
			}
		}*/
	}
	
	public override void DoEvent()
	{
		base.DoEvent();
	}
	
}
//怒吼5011 5012
public class GridActionCmd200009Skill5012 :GridActionCmdAttack{
	public GridActionCmd200009Skill5012(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		
		m_CastTime = 1.3f;
		m_EventTime = 1.3f;
		m_Duration = 2.333f;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		
		PlayAction(AnimatorState.Attack86000,m_Start);
		
		GameObject posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
		if (posgo != null)
		{
			Vector3 pos = posgo.transform.position;
			string name = "1401171_01";
			if (m_LifePrent.WalkDir == WalkDir.WALKRIGHT)
				name = "1401171_02";
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, name, pos, posgo.transform.GetChild(0));
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(m_Duration);
			gae.AddAction(ndEffect);
			m_effectgo = gae.gameObject;
		}
		
	}
	public override   void DoUpdate () {
		/*if (m_TimeCount < 1f)
		{
			PlayAction(AnimatorState.Attack85000,m_Start);
		}
		else
		{
			PlayAction(AnimatorState.Attack85100,m_Start);
			if ((m_TimeCount - m_Delatime) < 1f)
			{
				GameObject posgo = m_LifePrent.m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
				if (posgo != null)
				{
					Vector3 pos = posgo.transform.position;
					GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1103071", pos, posgo.transform);
					GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
					gae.AddAction(ndEffect);
					m_effectgo = gae.gameObject;
				}
				
			}
		}*/
	}
	
	public override void DoEvent()
	{
		base.DoEvent();
	}
	
}


