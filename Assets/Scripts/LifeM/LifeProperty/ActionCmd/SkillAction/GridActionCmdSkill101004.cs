using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 雷神普攻 1081
/// </summary>
/// 
public class GridActionCmd101004Skill1081 :GridActionCmdAttack
{
	public GridActionCmd101004Skill1081(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		m_CastTime = 0.6f;
		m_EventTime = 0.6f;
		m_Duration = 1.05f;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		PlayAction(AnimatorState.Attack85000,m_Start);
        Transform t = null;
        //RolePropertyM rpm = m_Skin.iGameRole.GetRolePropertyM();
        GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
        if (posgo != null)
        {
            t = posgo.transform;
            GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1055031", t.position, t);
            GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1.5f);
            gae.AddAction(ndEffect);
            m_effectgo = gae.gameObject;
			SoundPlay.JoinPlayQueue("atc_thor", 1.5f);
        }
	}
	
	public override void DoEvent()
	{
		base.DoEvent();
	}
	
}


public class GridActionCmd101004Skill1084 : GridActionCmdAttack
{
    public GridActionCmd101004Skill1084(DoQianyaoFun qianyaofun, DoAttackFun fun, int AttackSceneId, WalkDir AttackDir, int deep, int skillid)
        : base(qianyaofun, fun, AttackSceneId, AttackDir, deep, skillid)
    {
        m_CastTime = 1f;
        m_EventTime = 1f;
        m_Duration = 1.5f;
    }
    public override void StartWithTarget(Life Parent, StartAttackFun StartAttack)
    {
        base.StartWithTarget(Parent, StartAttack);
        PlayAction(AnimatorState.Attack81000, m_Start);

        //RolePropertyM rpm = m_Skin.iGameRole.GetRolePropertyM();
        GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
        if (posgo != null)
        {

            Vector3 pos = posgo.transform.position; 
            GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1055041", pos, posgo.transform);
            Life ttagrt = (m_LifePrent as Role).RoleSkill.m_AttackTarget;
            Vector3 tpos = ttagrt.GetCenterPos();
            float dis = Mathf.Abs(posgo.transform.position.x - tpos.x);
            pos.x = tpos.x;
            gae.transform.localScale = new Vector3(dis * 0.25f, 1, 1);
            GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(2f);
            gae.AddAction(ndEffect);
			m_effectgo = gae.gameObject;
        }
    }

    public override void DoEvent()
    {
        base.DoEvent();
    }

}


public class GridActionCmd101004ActiveSkill : GridActionCmdActiveSkill
{
	private bool bEffect = false;
    public GridActionCmd101004ActiveSkill(DoQianyaoFun qianyaofun, DoAttackFun fun, int sceneID, int AttackSceneId, WalkDir AttackDir, int deep, int skillid, float blackscreentime)
        : base(qianyaofun, fun, sceneID, AttackSceneId, AttackDir, deep, skillid, blackscreentime)
    {
        m_CastTime = 1f;
        m_EventTime = 1f;
        m_Duration = 1.5f;
    }
    public override void ActiiveStart()
    {
		SoundPlay.Play("skill_thor", false, false);
        Transform t = null;
        //RolePropertyM rpm = m_Skin.iGameRole.GetRolePropertyM();
        GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.Bone1Pos);
        if (posgo != null)
        {
            t = posgo.transform;

            //m_effectgo = SkillEffects._instance.LoadEffect("effect/prefab/", "1001081",t.position,0.75f,true);
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1055071_01", t.position, m_LifePrent.GetLifeProp().transform);
            GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1.5f);
            gae.AddAction(ndEffect);
            //m_effectgo = gae.gameObject;
        }

        GameObject posgo2 = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.Bone2Pos);
        if (posgo2 != null)
        {
            t = posgo2.transform;

            //m_effectgo = SkillEffects._instance.LoadEffect("effect/prefab/", "1001081",t.position,0.75f,true);
            GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1055071_02", t.position, t);
            GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1.5f);
            gae.AddAction(ndEffect);
            //m_effectgo = gae.gameObject;
        }

        GameObject posgo3 = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.Bone3Pos);
        if (posgo3 != null)
        {
            t = posgo3.transform;
            Vector3 r = new Vector3(0, 180f, 0);

            //m_effectgo = SkillEffects._instance.LoadEffect("effect/prefab/", "1001081",t.position,0.75f,true);
            GameObjectActionExcute gae = EffectM.LoadEffect("1055071_02", Vector3.zero, r, t);
            GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1.5f);
            gae.AddAction(ndEffect);
            //m_effectgo = gae.gameObject;
        }
    }


    public override void UpdatePos()
    {
        if (m_TimeCount < m_CastTime)
        {

            PlayAction(AnimatorState.Skill01, m_Start);
        }
        else if (m_TimeCount < m_Duration)
        {
            PlayAction(AnimatorState.Skill01, m_Start);
        }
    }
    public override void DoEvent()
    {

    }
    public override void RangeTarget(List<Life> llist)
    {        
		if (!bEffect)
		{
			bEffect = true;
	        base.RangeTarget(llist);
	        Transform tBoat = BattleEnvironmentM.GetLifeMBornNode(true);

	        foreach(Life life in llist)
	        {
	            GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1055081", life.GetPos(), tBoat);
	            GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(2f);
	            gae.AddAction(ndEffect);
	        }
		}
    }

}

