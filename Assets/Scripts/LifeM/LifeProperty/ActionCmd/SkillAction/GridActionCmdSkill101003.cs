using UnityEngine;
using DG.Tweening;
/// <summary>
/// 猫女普攻 1056
/// </summary>
/// 
public class GridActionCmd101003Skill1056 :GridActionCmdAttack{
	public GridActionCmd101003Skill1056(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		m_CastTime = 0.6f;
		m_EventTime = 0.6f;
		m_Duration = 1.05f;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		PlayAction(AnimatorState.Attack85100,m_Start);
		SoundPlay.Play("atc_maonv",false,false);
	}
	public override   void DoUpdate () {
		if (m_TimeCount > (m_EventTime - 0.0f) && (m_TimeCount - m_Delatime) < (m_EventTime - 0.0f))
		{
			GameObject posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
			if (posgo != null && (m_LifePrent as Role).RoleSkill.m_AttackTarget.m_thisT != null)
			{
				float fDistance = ((m_LifePrent as Role).m_thisT.position.x-(m_LifePrent as Role).RoleSkill.m_AttackTarget.m_thisT.position.x);
				fDistance=fDistance<0?-fDistance:fDistance;
				GameObjectActionExcute  gae= EffectM.LoadEffect(EffectM.sPath,"1054021",posgo.transform.position,posgo.transform);
				gae.transform.Rotate(new Vector3(0,180,0));
				Vector3 scale = gae.transform.localScale;
				//scale.x= fDistance/1.8f/2;
				gae.transform.localScale= scale;
				GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(0.5f);
				gae.AddAction(ndEffect);
			}

		}
	}
	
	public override void DoEvent()
	{
		base.DoEvent();
	}
	
}


/// <summary>
/// 猫女魅惑 1058
/// </summary>
/// 
public class GridActionCmd101003Skill1058 :GridActionCmdAttack{
	public GridActionCmd101003Skill1058(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		m_CastTime = 1.0f;
		m_EventTime = 1.0f;
		m_Duration = 1.267f;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		PlayAction(AnimatorState.Attack81000,m_Start);
	}
	public override   void DoUpdate () {
		if (m_TimeCount > (m_EventTime - 0.5f) && (m_TimeCount - m_Delatime) < (m_EventTime - 0.5f))
		{
			GameObject posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.RightHandPos);
			if (posgo != null && (m_LifePrent as Role).RoleSkill.m_AttackTarget.m_thisT != null)
			{
				GameObjectActionExcute  gae= EffectM.LoadEffect(EffectM.sPath,"1054031",posgo.transform.position,m_LifePrent.GetLifeProp().transform);
				if (m_LifePrent.WalkDir == WalkDir.WALKRIGHT)
					gae.transform.localEulerAngles += new Vector3(0,180,0);
				GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1.5f);
				gae.AddAction(ndEffect);
			}
			
		}
	}
	
	public override void DoEvent()
	{
		base.DoEvent();
	}
	
}
/// <summary>
/// 猫女闪影 1059
/// </summary>
/// 
public class GridActionCmd101003Skill1059 :GridActionCmdAttack{
	public GridActionCmd101003Skill1059(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		m_CastTime = 0.9f;
		m_EventTime = 0.9f;
		m_Duration = 2.75f;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		PlayAction(AnimatorState.Attack82000,m_Start);

	}
	public override   void DoUpdate () {

		if (m_TimeCount > (m_EventTime + 0.7f) && (m_TimeCount - m_Delatime) < (m_EventTime + 0.7f))
		{
			(m_LifePrent as Role).RoleSkinCom.SetVisable(true);
			 PlayAction(AnimatorState.Attack82001,m_Start);
			
		}
	}
	
	public override void DoEvent()
	{
		GameObject posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectPos);
		if (posgo != null && (m_LifePrent as Role).RoleSkill.m_AttackTarget.m_thisT != null)
		{
			float fDistance = ((m_LifePrent as Role).GetPos().x-(m_LifePrent as Role).RoleSkill.m_AttackTarget.GetPos().x);
			string strEffect = "1054051_01";
			if(m_Dir==WalkDir.WALKRIGHT)
				strEffect = "1054051_02";
			GameObjectActionExcute  gae= EffectM.LoadEffect(EffectM.sPath,strEffect,posgo.transform.position,null);
			gae.transform.parent = null;
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(0.7f);
			gae.AddAction(ndEffect);
			(m_LifePrent as Role).RoleSkinCom.SetVisable(false);
		}
		base.DoEvent();
	}
	public override void Finish ()
	{
		base.Finish ();
		(m_LifePrent as Role).RoleSkinCom.SetVisable(true);
	}
}


/// <summary>
/// 猫女飞吧 1060
/// </summary>
/// 
public class GridActionCmd101003Ski1060 : GridActionCmdConditionSkill {
	Bezier bezier;
	Bezier bezierspeed;
	float m_fDelay;
	bool m_bearhit;
	public GridActionCmd101003Ski1060(Vector3 start, Vector3 end, DoAttackFun fun, WalkDir dir,DoQianyaoFun qianyaofun)
	{
		m_CastTime = m_eventtime = 0.66f;
		m_Duration = 1.05f;
		m_Start = start;
		m_End = end;
		m_Dir = dir;
		m_DoSkill = fun;
		m_QianYaoStatus = qianyaofun;
	}
	public override  void StartWithTarget(Life Parent)
	{
		base.StartWithTarget(Parent);
		PlayAction(AnimatorState.Attack85000,m_Start);
	}
	public override   void UpdatePos () {
		if (m_TimeCount > (m_eventtime - 0.0f) && (m_TimeCount - m_Delatime) < (m_eventtime - 0.0f))
		{
			GameObject posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
			Life lifeBuildTarget = (m_LifePrent as Role).RoleSkill.PropSkillInfo.m_LifeTarget;
				if (posgo != null && lifeBuildTarget.m_thisT != null)
			{
				float fDistance = ((m_LifePrent as Role).m_thisT.position.x-lifeBuildTarget.m_thisT.position.x);
			fDistance=fDistance<0?-fDistance:fDistance;
				GameObjectActionExcute  gae= EffectM.LoadEffect(EffectM.sPath,"1054021",posgo.transform.position,m_LifePrent.GetLifeProp().transform);
				Vector3 scale = gae.transform.localScale;
				scale.x= fDistance/1.8f;
				gae.transform.localScale= scale;
				GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(0.5f);
				gae.AddAction(ndEffect);
			}
			if (lifeBuildTarget is Building)
			{

				{
					Building building = lifeBuildTarget as Building;
					if (building!=null) {
						building.HitFly (m_LifePrent.WalkDir,3f,0f,false);
					}
				}
			}
			else if (lifeBuildTarget is InheritSummonPros)
			{
				{
					InheritSummonPros building = lifeBuildTarget as InheritSummonPros;
					if (building!=null) {
						building.HitFly (m_LifePrent.WalkDir,3f,0f,false);
					}
				}
			}
		}



	}
	
	public override void DoEvent()
	{
		base.DoEvent();
		Life lifeBuildTarget = (m_LifePrent as Role).RoleSkill.PropSkillInfo.m_LifeTarget;
		if (lifeBuildTarget != null)
		{
			GameObjectActionExcute  gae= EffectM.LoadEffect(EffectM.sPath,"1054091",lifeBuildTarget.m_thisT.position,BattleEnvironmentM.GetLifeMBornNode(true));
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(0.5f);
			gae.AddAction(ndEffect);
		}
	}
	
}


//猫女鞭刑 1057
public class GridActionCmd101003ActiveSkill :GridActionCmdActiveSkill{
	
	
	public GridActionCmd101003ActiveSkill(DoQianyaoFun qianyaofun,DoAttackFun fun,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
		:base( qianyaofun, fun,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime)
	{
		//Debug.Log("猫女鞭刑 1052");
		m_Duration = 4.06f;///3.667f;
		m_EventTime = 1.2f;//2.367f;
		m_CastTime = 1.2f;//2.267f;
	}
	
	public override void ActiiveStart()
	{
		Life  w = m_LifePrent.m_Skill.m_SkillTarget ;
		if (w != null) 
		{
			GameObject posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
			if (posgo != null)
			{
				float fDistance = ((m_LifePrent as Role).m_thisT.position.x-w.m_thisT.position.x);
				fDistance=fDistance<0?-fDistance:fDistance;
				Vector3 pos = posgo.transform.position;
				GameObjectActionExcute  gae= EffectM.LoadEffect(EffectM.sPath,"1054071",posgo.transform.position,posgo.transform);
				Vector3 scale = gae.transform.localScale;
				//scale.x= fDistance/2.4f;
				gae.transform.localScale= scale;
				GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(4f);
				gae.AddAction(ndEffect);

			}
			Vector3 pos1 = w.m_thisT.position;
			if (w is Role)
			{
				posgo = w.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectPos);
				pos1 = posgo.transform.position;
			}
			else
				pos1 += new Vector3(0,2,0);
			//if (posgo != null)
			{
				GameObjectActionExcute gaeTarget = EffectM.LoadEffect(EffectM.sPath, "1054081", pos1, BattleEnvironmentM.GetLifeMBornNode(true));
				GameObjectActionDelayDestory ndEffectTarget = new GameObjectActionDelayDestory(4f);
				gaeTarget.AddAction(ndEffectTarget);

				SoundPlay.Play("skill_maonv",false,false);
			}
				
		}

	}
	public override void DoEvent()
	{
	}
	public override void UpdatePos()
	{
		PlayAction(AnimatorState.Skill01,m_Start);
	}
	
}