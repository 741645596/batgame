using UnityEngine;
using System.Collections;


//5003自爆企鹅自爆
public class GridActionCmd200003Skill5003 :GridActionCmdConditionSkill{
	public GridActionCmd200003Skill5003(Vector3 start,DoAttackFun fun,DoQianyaoFun qianyaofun)
	{
		m_Start = start;
		m_End = start;
		m_CastTime = m_eventtime = 0f;
		m_realDuration = 0.5f;
		m_Duration = m_realDuration;
		m_DoSkill = fun;
		m_QianYaoStatus = qianyaofun;

	}
	public override void UpdatePos()
	{
		PlayAction(AnimatorState.Attack81000,m_Start);
	}
	
}

//5004 死亡自爆
public class GridActionCmd200003Skill5004 :GridActionCmdConditionSkill{
	public GridActionCmd200003Skill5004(Vector3 start,DoAttackFun fun,DoQianyaoFun qianyaofun)
	{
		m_Start = start;
		m_End = start;
		m_CastTime = m_eventtime = 0f;
		m_realDuration = 0f;
		m_Duration = m_realDuration;
		m_DoSkill = fun;
		m_QianYaoStatus = qianyaofun;

	}
	public override void StartWithTarget (Life Parent)
	{
		base.StartWithTarget (Parent);
		
		GameObject posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectPos);
		Transform t = posgo.transform;
		if (t != null)
		{
			//m_effectgo = SkillEffects._instance.LoadEffect("effect/prefab/", "1001031",t.position,m_Duration);
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1003031", t.position, BattleEnvironmentM.GetLifeMBornNode(true));
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1.0f);
			gae.AddAction(ndEffect);
		}
	}
	public override void UpdatePos()
	{
		PlayAction(AnimatorState.Attack82000,m_Start);
	}

}

public class GridActionCmd200003FindTarget : GridActionCmd{
	public GridActionCmd200003FindTarget()
	{
		m_Duration = 1.167f;
	}
	public override void Update ()
	{
		base.Update ();
		PlayAction(AnimatorState.Attack85000,m_Start);
	}
}