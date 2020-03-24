using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// 1041咸鱼干
public class GridActionCmd102005Skill1041 : GridActionCmdAttack {
	public GridActionCmd102005Skill1041(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		
		m_CastTime = 0.3f;
		m_EventTime = 0.3f;
		m_Duration = 0.667f;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		
		PlayAction(AnimatorState.Attack82000,m_Start);
		SoundPlay.Play("atc_naixiong",false,false);
	}
	public override   void DoUpdate () {
		
	}
	
	public override void DoEvent()
	{
		base.DoEvent();
	}

}
//1043 饥肠辘辘
public class GridActionCmd102005Skill1043 : GridActionCmdConditionSkill {
	public GridActionCmd102005Skill1043(Vector3 start, Vector3 end, DoAttackFun fun, WalkDir dir,DoQianyaoFun qianyaofun)
	{

		m_CastTime = m_eventtime = 0.3f;
		m_Duration =0.7f;
		m_Start = start;
		m_End = end;
		m_Dir = dir;
		m_DoSkill = fun;
		m_QianYaoStatus = qianyaofun;
	}
	public override void StartWithTarget (Life Parent)
	{
		base.StartWithTarget (Parent);
		
		GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
		if (posgo != null)
		{
			//m_effectgo = SkillEffects._instance.LoadEffect("effect/prefab/", "1003091",
			// posgo.transform.position, m_Duration, posgo);
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1106031", posgo.transform.position, posgo.transform);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
			gae.AddAction(ndEffect);
			Vector3 pos = gae.transform.localPosition;
			pos.z = 1.5f;
			gae.transform.localPosition = pos;
			m_effectgo = gae.gameObject;
		}
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
// 1044 奶熊普攻
public class GridActionCmd102005Skill1044 : GridActionCmdAttack {
	public GridActionCmd102005Skill1044(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		
		m_CastTime = 0.33f;
		m_EventTime = 0.33f;
		m_Duration = 0.667f;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		
		PlayAction(AnimatorState.Attack85000,m_Start);
		SoundPlay.Play("skill_naixiong",false,false);
	}
	public override   void DoUpdate () {
		
	}
	
	public override void DoEvent()
	{
		base.DoEvent();
	}
	
}
//1042 满汉全席
public class GridActionCmd102005ActiveSkill :GridActionCmdActiveSkill{

	public GridActionCmd102005ActiveSkill(DoQianyaoFun qianyaofun,DoAttackFun fun,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
		:base( qianyaofun, fun,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime)
	{
		m_CastTime = 1.23f;
		m_EventTime = 1.23f;
		m_Duration = 3.5f;
		m_Dir = AttackDir;
	}
	public override void ActiiveStart()
	{
		#if UNITY_EDITOR_LOG
		App.log.PrintSelf(PRINT.TXM,m_AttackSceneID.ToString() );
		#endif
		GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
		if (posgo != null)
		{
			//m_effectgo = SkillEffects._instance.LoadEffect("effect/prefab/", "1003091",
			// posgo.transform.position, m_Duration, posgo);
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1106041", posgo.transform.position, posgo.transform);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
			gae.AddAction(ndEffect);
			Vector3 pos = gae.transform.localPosition;
			pos.z = 1.5f;
			gae.transform.localPosition = pos;
			m_effectgo = gae.gameObject;
		}

		
	}
	
	public override void DoEvent()
	{
		
	}
	public override void UpdatePos()
	{
		if (m_TimeCount < m_CastTime)
		{
			
			PlayAction(AnimatorState.Skill01,m_Start);
		}
		else if (m_TimeCount < (m_Duration - 0.5f))
		{
			if ((m_TimeCount - m_Delatime) < m_CastTime)
			{
				GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.GuoPos);
				if (posgo != null)
				{
					//m_effectgo = SkillEffects._instance.LoadEffect("effect/prefab/", "1003091",
					// posgo.transform.position, m_Duration, posgo);
					GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1102011", posgo.transform.position, posgo.transform);
					GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(m_Duration - m_TimeCount);
					gae.AddAction(ndEffect);
					m_effectgo = gae.gameObject;
				}
			}
			PlayAction(AnimatorState.Skill01,m_Start);

		}
		else
		{
			if ((m_TimeCount - m_Delatime) < (m_Duration - 0.5f))
			{
				GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectPos);
				if (posgo != null)
				{
					//m_effectgo = SkillEffects._instance.LoadEffect("effect/prefab/", "1003091",
					// posgo.transform.position, m_Duration, posgo);
					Vector3 pos = posgo.transform.position;
					pos.z -= 2f;
					GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1051161", posgo.transform.position, posgo.transform);
					GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
					gae.AddAction(ndEffect);
					m_effectgo = gae.gameObject;
				}
			}
			PlayAction(AnimatorState.Attack80100,m_Start);
		}
	}
}
