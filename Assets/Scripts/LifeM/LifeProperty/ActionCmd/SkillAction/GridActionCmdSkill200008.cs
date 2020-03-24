using UnityEngine;
using System.Collections;
/// <summary>
/// 黑胡子Boss技能 编号200008.
/// </summary>
/// 
/// //1101 普攻.
public class GridActionCmd200008SkillNomalAttack :GridActionCmdAttack{
	public GridActionCmd200008SkillNomalAttack(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		
		m_CastTime = 0.1f;
		m_EventTime = 0.1f;
		m_Duration = 1.333f;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		
		PlayAction(AnimatorState.Attack85000,m_Start);
		string effectName = m_Dir == WalkDir.WALKLEFT?"1402011_01":"1402011_02";
		GameObject gpos = m_Skin.ProPerty.gameObject;
		GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, effectName, gpos.transform.position, gpos.transform);
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
/// <summary>
/// 黑胡子Boss大招 5019.
/// </summary>
public class GridActionCmd200008BigSkill :GridActionCmdActiveSkill{
	
	float m_walktime;
	public GridActionCmd200008BigSkill(DoQianyaoFun qianyaofun,DoAttackFun fun,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
		:base( qianyaofun, fun,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime)
	{
		m_CastTime = 2f;
		m_EventTime = 2f;
		m_Duration = 3.0f;
		m_walktime = 0f;
		m_Dir = AttackDir;
	}
	public override void ActiiveStart()
	{
		PlayAction(AnimatorState.Attack87000,m_Start);

		string effName = m_Dir == WalkDir.WALKLEFT?"1402051_01":"1405051_02";
		GameObject gpos = m_Skin.ProPerty.gameObject;
		GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, effName, gpos.transform.position, gpos.transform);
		GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(3f);
		gae.AddAction(ndEffect);
		m_effectgo = gae.gameObject;
		GameObject gpos2 = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.LeftHandPos);

		GameObjectActionExcute gae2 = EffectM.LoadEffect(EffectM.sPath,"1402041",gpos2.transform.position, gpos2.transform);
		m_effectgo = gae.gameObject;
		GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectPos);
		if (posgo != null)
		{
			//m_effectgo = SkillEffects._instance.LoadEffect("effect/prefab/", "1000011",posgo.transform.position,1.5f);
			 gae = EffectM.LoadEffect(EffectM.sPath, "1000011", posgo.transform.position, BattleEnvironmentM.GetLifeMBornNode(true));
			 ndEffect = new GameObjectActionDelayDestory(1.5f);
			gae.AddAction(ndEffect);
			m_effectgo = gae.gameObject;
			m_effectgo.transform.position += new Vector3(0,0,-2);
		}
	}
	
	public override void DoAttackEvent()
	{

	}
	public override void UpdatePos()
	{

	}

	public override void RangeTarget (System.Collections.Generic.List<Life> llist)
	{
		base.RangeTarget (llist);

		foreach(Life life in llist)
		{
			GameObject go = life.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
			
			GameObjectActionExcute Tgae = EffectM.LoadEffect(EffectM.sPath,"1402061",go.transform.position + new Vector3(0,0, -1), go.transform);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(0.5f);
			Tgae.AddAction(ndEffect);
		}
	}
}

/// <summary>
/// 黑胡子技能1 自爆出动5020.
/// </summary>
public class GridActionCmd200008Skill1101 :GridActionCmdAttack{
	public GridActionCmd200008Skill1101(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		
		m_CastTime = 1.1f;
		m_EventTime = 1.1f;
		m_Duration = 3.0f;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		
		PlayAction(AnimatorState.Attack86000,m_Start);

		GameObject gpos = m_Skin.ProPerty.gameObject;
		GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1402021_01", gpos.transform.position, gpos.transform);
		
		GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(3f);
		gae.AddAction(ndEffect);
		m_effectgo = gae.gameObject;
		GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectPos);
		if (posgo != null)
		{
			//m_effectgo = SkillEffects._instance.LoadEffect("effect/prefab/", "1000011",posgo.transform.position,1.5f);
			 gae = EffectM.LoadEffect(EffectM.sPath, "1000011", posgo.transform.position, BattleEnvironmentM.GetLifeMBornNode(true));
			 ndEffect = new GameObjectActionDelayDestory(1.5f);
			gae.AddAction(ndEffect);
			m_effectgo = gae.gameObject;
			m_effectgo.transform.position += new Vector3(0,0,-2);
		}
	}
	public override   void DoUpdate () 
	{


	}
	
	public override void DoEvent()
	{
		base.DoEvent();
		GameObject gpos2 = m_Skin.ProPerty.gameObject;
		MapGrid g = m_LifePrent.GetMapGrid();
		int dis = 6;
		if (m_LifePrent.WalkDir == WalkDir.WALKLEFT)
		{
			for(int n = 0; n < dis; n++)
			{
				if (g.Right != null)
					g = g.Right;
				else
					break;
			}
		}
		else if (m_LifePrent.WalkDir == WalkDir.WALKRIGHT)
		{
			for(int n = 0; n < dis; n++)
			{
				if (g.Left != null)
					g = g.Left;
				else
					break;
			}
		}
		GameObjectActionExcute gae2 = EffectM.LoadEffect(EffectM.sPath, "1402031", g.WorldPos, gpos2.transform);
		GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(3f);
		m_effectgo = gae2.gameObject;
	}
	
}