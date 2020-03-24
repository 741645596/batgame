#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#define UNITY_EDITOR_LOG
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 熊孩子技能 编号102001
/// </summary>
public class GridActionCmd102001Skill01 :GridActionCmdAttack{
	public GridActionCmd102001Skill01(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		
		m_CastTime = 0.4f;
		m_EventTime = 0.4f;
		m_Duration = 0.833f;
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
// 1024 千斤顶
public class GridActionCmd102001Skill1024 :GridActionCmdConditionSkill{
	public GridActionCmd102001Skill1024(Vector3 start,Vector3 end,float eventtime, float totaltime,DoAttackFun fun,DoQianyaoFun qianyaofun)
	{
		m_Start = start;
		m_End = end;
		m_CastTime = m_eventtime = 0;
		m_realDuration = 1.267f;
		m_Duration = m_realDuration;
		m_DoSkill = fun;
		m_QianYaoStatus = qianyaofun;
		
		//Debug.Log("  大斜八块 ");
	}
	public override void StartWithTarget (Life Parent)
	{
		base.StartWithTarget (Parent);
		m_Dir = Parent.WalkDir;
		Transform t = m_Skin.tRoot;
		if (t != null)
		{
			//m_effectgo = SkillEffects._instance.LoadEffect("effect/prefab/", "1001031",t.position,m_Duration);
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath,m_Dir == WalkDir.WALKRIGHT?"1101061_02":"1101061_01", t.position, BattleEnvironmentM.GetLifeMBornNode(true));
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(m_Duration);
			gae.AddAction(ndEffect);
			m_effectgo = gae.gameObject;
			if(m_Dir == WalkDir.WALKRIGHT)
				m_effectgo.transform.Rotate(new Vector3(0,180,0),Space.Self);
		}
	}
	public override void UpdatePos()
	{
		PlayAction(AnimatorState.NoSquashEnd,m_Start);
	}
	
}

public class GridActionCmd102001ConditionSkill01 :GridActionCmdConditionSkill{
	public GridActionCmd102001ConditionSkill01(Vector3 start,Vector3 end,float eventtime, float totaltime,DoAttackFun fun,DoQianyaoFun qianyaofun)
	{
		m_Start = start;
		m_End = end;
		m_CastTime = m_eventtime = 1.2f;
		m_realDuration = 3f;
		m_Duration = m_realDuration;
		m_DoSkill = fun;
		m_QianYaoStatus = qianyaofun;
		
	}

	public override void UpdatePos()
	{
		PlayAction(AnimatorState.Attack81000,m_Start);
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
public class GridActionCmd102001LSkill01 :GridActionCmdAttack{
	
	public GridActionCmd102001LSkill01(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		m_Duration = 0.8333f;
		m_CastTime = 0.3f;
		m_EventTime = 0.3f;
		m_Dir = AttackDir;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		
	}
	public override   void DoUpdate () {
		
		PlayAction(AnimatorState.Attack82000,m_Start);
	}
	
}
public class GridActionCmd102001ActiveSkill :GridActionCmdActiveSkill{
	
	float m_walktime;
	public GridActionCmd102001ActiveSkill(DoQianyaoFun qianyaofun,DoAttackFun fun,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
		:base( qianyaofun, fun,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime)
	{
		m_CastTime = 1.36f;
		m_EventTime = 1.36f;
		m_Duration = 2.133f;
		m_walktime = 0f;
		m_Dir = AttackDir;
	}
	public override void ActiiveStart()
	{
		
		SoundPlay.Play("skill_voice_bear", false, false);
		/*MapGrid gfrom = m_LifePrent.GetMapGrid();
		m_Start = m_Skin.tRoot.localPosition;
        DIR dir = m_Dir == WalkDir.WALKLEFT ? DIR.LEFT : DIR.RIGHT;

		MapGrid gto = gfrom;
		//w.m_Attr.Speed
		while(Mathf.Abs(gto.GridPos.Unit - gfrom.GridPos.Unit) <= (m_LifePrent.m_Attr.Speed * (m_Duration- m_EventTime)))
		{
			MapGrid g = gto.GetNextAttackStation(dir,false);
			if(g != null)
			{
				List<int> lr = new List<int>();
				g.GetRoleList(ref lr);
				if (lr.Count > 1 || g.Type == GridType.GRID_HOLE || g.Type == GridType.GRID_WALL)
				{
					break;
				}
				else
				{
					if (lr.Count == 1 && lr[0] != m_LifePrent.m_SceneID)
						break;
					else
						gto = g;
				}
			}
			else
				break;
		}
		if (gto != null)
		{
			m_End = gto.pos;
			//w.Teleport(gto);
		}
		else
			m_End = m_Start;
		m_walktime = Mathf.Abs(gto.GridPos.Unit - gfrom.GridPos.Unit)/ m_LifePrent.m_Attr.Speed;*/
	/*	MapGrid gl = gfrom.GetNextAttackStation(DIR.LEFT,false);
		MapGrid gr = gfrom.GetNextAttackStation(DIR.RIGHT,false);*/
       
	}
	
	public override void DoAttackEvent()
	{
		if ((m_LifePrent as Role).RoleSkill.m_AttackTarget != null && (m_LifePrent as Role).RoleSkill.m_AttackTarget is Role && (m_LifePrent as Role).RoleSkill.m_AttackTarget.m_Attr.AttrType != 102003 && (m_LifePrent as Role).RoleSkill.m_AttackTarget.m_Status.HaveState (StatusType.ClickFly))
		{
			((m_LifePrent as Role).RoleSkill.m_AttackTarget as Role).HitFly(m_Dir, 2.3f, 0f,true);
			GameObject posgo = (m_LifePrent as Role).RoleSkill.m_AttackTarget .GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectPos);
			if (posgo != null)
			{
				//m_effectgo = SkillEffects._instance.LoadEffect("effect/prefab/", "1003091",
				// posgo.transform.position, m_Duration, posgo);
				Vector3 pos = posgo.transform.position;
				pos.z -=4;
				GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "2000361", pos, null);
				GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(0.6f);
				gae.AddAction(ndEffect);
			}
			(m_LifePrent as Role).ChangeTarget(ChangeTargetReason.HitFlyClearTarget,null);
			//AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_SELFSP),m_LifePrent);
		}
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
