using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 二白普攻 1051
/// </summary>
/// 
public class GridActionCmd100004Skill01 :GridActionCmdAttack{
	public GridActionCmd100004Skill01(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		//Debug.Log("二白普攻 1051");
		m_CastTime = 0.6f;
		m_EventTime = 0.6f;
		m_Duration = 1f;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		
		PlayAction(AnimatorState.Attack85000,m_Start);
		SoundPlay.Play("atc_xiaobai",false,false);
	}
	public override   void DoUpdate () {
		if (m_TimeCount > (m_EventTime - 0.3f) && (m_TimeCount - m_Delatime) < (m_EventTime - 0.3f))
		{
			GameObject posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.FirePos);
			if (posgo != null && (m_LifePrent as Role).RoleSkill.m_AttackTarget.m_thisT != null)
			{
				
				Vector3 pos = posgo.transform.position;
				//pos.z  = -1.5f;
				GameObjectActionExcute  gae= EffectM.LoadEffect(EffectM.sPath,"1004011",posgo.transform.position,m_LifePrent.GetLifeProp().transform);
				GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
				gae.AddAction(ndEffect);
				Life ttagrt =  (m_LifePrent as Role).RoleSkill.m_AttackTarget;
				Vector3 tpos = ttagrt.GetCenterPos();
				
				float dis = Mathf.Abs(posgo.transform.position.x - tpos.x);
				gae.transform.localScale = new Vector3(-dis * 0.25f,1,1);
				pos.x = tpos.x;
				gae= EffectM.LoadEffect(EffectM.sPath,"1004021",pos,m_LifePrent.GetLifeProp().transform);
				ndEffect = new GameObjectActionDelayDestory(1f);
				gae.AddAction(ndEffect);
			}

		}
	}
	
	public override void DoEvent()
	{
		base.DoEvent();
	}
	
}
//负能量转移 1053
public class GridActionCmd100004ConditionSkill1053 :GridActionCmdConditionSkill{
	public GridActionCmd100004ConditionSkill1053(Vector3 start,Vector3 end,DoAttackFun fun,DoQianyaoFun qianyaofun)
	{
		m_Start = start;
		m_End = end;
		m_CastTime = 0.73f;
		m_eventtime = 0.73f;
		m_realDuration = 1f;
		m_Duration = m_realDuration;
		m_DoSkill = fun;
		m_QianYaoStatus = qianyaofun;
		
	}
	
	public override void StartWithTarget (Life Parent)
	{
		base.StartWithTarget (Parent);
		if (m_LifePrent.m_thisT.localPosition.x - (m_LifePrent as Role).RoleSkill.PropSkillInfo.m_LifeTarget.m_thisT.localPosition.x > 0)
		{
			m_Dir = WalkDir.WALKLEFT;
			m_LifePrent.WalkDir = WalkDir.WALKLEFT;
		}
		else
		{
			m_Dir = WalkDir.WALKRIGHT;
			m_LifePrent.WalkDir = WalkDir.WALKRIGHT;
		}
		//Debug.Log("负能量转移 1053 "  + (m_LifePrent as Role).RoleSkill.PropSkillInfo.m_LifeTarget + ", " +(m_LifePrent as Role).RoleSkill.PropSkillInfo.m_LifeTarget.m_thisT.localPosition.x);
		
		/*GameObject posgo = m_LifePrent.m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectPos);
		if (posgo != null)
		{
			Vector3 pos = posgo.transform.position;
			pos.z  = -1.5f;
			GameObjectActionExcute  gae= EffectM.LoadEffect(EffectM.sPath,"1004061",posgo.transform.position,posgo.transform);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
			gae.AddAction(ndEffect);
		}*/
	}
	public override void UpdatePos()
	{
		PlayAction(AnimatorState.Attack82000,m_Start);
	}
	
	public override void RangeTarget(List<Life> llist)
	{
		GameObject posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectPos);
		if (posgo != null)
		{
			Vector3 pos = posgo.transform.position;
			pos.z  = -1.5f;
			
			GameObjectActionExcute  gae= EffectM.LoadEffect(EffectM.sPath,"1004061",posgo.transform.position,posgo.transform);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
			gae.AddAction(ndEffect);
			for(int i = 0; i < llist.Count; i++)
			{
				if (llist[i].m_thisT != null)
				{
					gae= EffectM.LoadEffect(EffectM.sPath,"1004051",posgo.transform.position,m_LifePrent.GetLifeProp().transform.parent);
					ndEffect = new GameObjectActionDelayDestory(1f);
					gae.AddAction(ndEffect);
					float dis = m_LifePrent.m_thisT.localPosition.x - llist[i].m_thisT.localPosition.x;
					gae.transform.localScale = new Vector3(dis * 0.23f,1,1);
				}
			}

		}
	}
}
//疗伤 1054
public class GridActionCmd100004ConditionSkill1054 :GridActionCmdConditionSkill{
	public GridActionCmd100004ConditionSkill1054(Vector3 start,Vector3 end,DoAttackFun fun,DoQianyaoFun qianyaofun)
	{
		//Debug.Log("疗伤 1054");
		m_Start = start;
		m_End = end;
		m_CastTime = 0.73f;
		m_eventtime = 0.73f;
		m_realDuration = 1f;
		m_Duration = m_realDuration;
		m_DoSkill = fun;
		m_QianYaoStatus = qianyaofun;
		
	}
	
	public override void UpdatePos()
	{
		PlayAction(AnimatorState.Attack81000,m_Start);
	}
}
//给予正能量 1055
public class GridActionCmd100004ConditionSkill1055 :GridActionCmdConditionSkill{
	public GridActionCmd100004ConditionSkill1055(Vector3 start,Vector3 end,DoAttackFun fun,DoQianyaoFun qianyaofun)
	{
		//Debug.Log("给予正能量 1055");
		m_Start = start;
		m_End = end;
		m_CastTime = m_eventtime = 0.73f;
		m_realDuration = 1f;
		m_Duration = m_realDuration;
		m_DoSkill = fun;
		m_QianYaoStatus = qianyaofun;
		
	}
	public override void StartWithTarget (Life Parent)
	{
		base.StartWithTarget (Parent);
		
		GameObject posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
		if (posgo != null)
		{
			Vector3 pos = posgo.transform.position;
			pos.z  = -1.5f;
			GameObjectActionExcute  gae= EffectM.LoadEffect(EffectM.sPath,"1004071",posgo.transform.position,posgo.transform);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
			gae.AddAction(ndEffect);
		}
	}
	public override void UpdatePos()
	{
		PlayAction(AnimatorState.Attack83000,m_Start);
	}
}

//健康护盾 1052
public class GridActionCmd100004ActiveSkill :GridActionCmdActiveSkill{

	
	public GridActionCmd100004ActiveSkill(DoQianyaoFun qianyaofun,DoAttackFun fun,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
		:base( qianyaofun, fun,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime)
	{
		//Debug.Log("健康护盾 1052");
		m_Duration = 2.5f;///3.667f;
		m_EventTime = 0.933f;//2.367f;
		m_CastTime = 0.933f;//2.267f;
	}

	public override void ActiiveStart()
	{
		
		GameObject posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectPos);
		if (posgo != null)
		{
			Vector3 pos = posgo.transform.position;
			pos.z  = -1.5f;
			GameObjectActionExcute  gae= EffectM.LoadEffect(EffectM.sPath,"1004031_01",posgo.transform.position,posgo.transform);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1.5f);
			gae.AddAction(ndEffect);

			SoundPlay.Play("skill_xiaobai",false,false);

		}
	}
	public override void DoEvent()
	{
		//Transform t = m_Skin.ProPerty.m_EffectPos;		
		GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
		if (posgo != null)
		{
			//m_effectgo = SkillEffects._instance.LoadEffect("effect/prefab/", "1002031",posgo.transform.position,m_Duration);
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1004031_02", posgo.transform.position, posgo.transform);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1.5f);
			gae.AddAction(ndEffect);
		}
	}
	public override void UpdatePos()
	{
		if (m_TimeCount < m_CastTime)
		{

			PlayAction(AnimatorState.Skill01,m_Start);
		}
		else //if ()
		{
			PlayAction(AnimatorState.Skill01,m_Start);
		}
	}

}