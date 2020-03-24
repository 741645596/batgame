using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 人鱼术士 1091
/// </summary>
/// 

// 普通攻击
public class GridActionCmd103001Skill1091 :GridActionCmdAttack
{
	public GridActionCmd103001Skill1091(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		m_CastTime = 0.6f;
		m_EventTime = 0.6f;
		m_Duration = 1.0f;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		PlayAction(AnimatorState.Attack85000,m_Start);
		SoundPlay.Play(m_LifePrent.m_Attr.ModelType.ToString() +"_atc", false, false);
        Transform t = null;
        //RolePropertyM rpm = m_Skin.iGameRole.GetRolePropertyM();
        GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.FirePos);
        if (posgo != null)
        {
            t = posgo.transform;
            GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1201021", t.position, t);
            GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
            gae.AddAction(ndEffect);
            m_effectgo = gae.gameObject;
        }
	}
	
	public override void DoEvent()
	{
		base.DoEvent();
	}
    public override void DoAttack(int count)
    {
        Life target = m_skillinfo.m_LifeTarget;//m_ConditionSkillTarget[PropSkillInfo.m_type];
        MapGrid pos = m_skillinfo.m_TargetPos;//m_ConditionSkillTargetPos[PropSkillInfo.m_type];
        if (m_skillinfo.m_target != 2 && target != null)
            pos = target.GetMapGrid();
        //if (PropSkillInfo.m_type == 1009 || PropSkillInfo.m_type == 1028)
        //	NGUIUtil.DebugLog( "doskill " + PropSkillInfo.m_type  + "," +  m_ConditionSkillTarget[PropSkillInfo.m_type] + "," + PropSkillInfo.m_name,"red");
        //播放攻击动画
        //受击掉血
        m_LifePrent.m_Attr.Attacked = true;

        if (m_skillinfo.m_skilleffectinfo != null)//RoleModelsM.GetSkillResourcesData(PropSkillInfo.m_type,"HasBullet") == 1)
        {
			GameObject posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.FirePos);

            if (m_skillinfo.m_type == 1041)
				posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.LeftHandPos);
            else if (m_skillinfo.m_type == 1044)
				posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.bagPos);
            Transform fireplace = posgo.transform;
            if (fireplace != null)
            {
                string bulletname = m_skillinfo.m_skilleffectinfo.m_targeteffect;
                string bulletLeft = "1201031_01";
                string bulletRight = "1201031_02";

                if (BattleEnvironmentM.GetLifeMBornNode(true) == null)
                {
                    return;
                }
                GameObject go = null;
                if (m_LifePrent.WalkDir == WalkDir.WALKLEFT)
                {
                    go = GameObjectLoader.LoadPath("effect/prefab/", bulletLeft, fireplace.position, BattleEnvironmentM.GetLifeMBornNode(true));
                }
                else
                {
                    go = GameObjectLoader.LoadPath("effect/prefab/", bulletRight, fireplace.position, BattleEnvironmentM.GetLifeMBornNode(true));
                }
                
                
                if (m_LifePrent.WalkDir == WalkDir.WALKRIGHT)
                    go.transform.localScale = new Vector3(-go.transform.localScale.x, go.transform.localScale.y, go.transform.localScale.z);
                Bullet bullet = go.AddComponent<Bullet>();
                if (bullet != null)
                {
                    Bullet.BulletType t = Bullet.BulletType.Bullet;
                    Vector3 vpos = pos.pos;
                    if (m_skillinfo.m_skilleffectinfo.m_postion == 0)
                        vpos.y = go.transform.localPosition.y;
                    else
                        vpos.y += 0.2f;
                    bullet.SetInfo(vpos, m_DoAttack, 10f, m_LifePrent.WalkDir, t, (m_skillinfo as SoldierSkill), true);
                    //vpos,m_DoAttack,10f,m_LifePrent.WalkDir,t,m_skillinfo as SoldierSkill);
                }
            }
        }
        else
        {
            m_DoAttack(m_skillinfo as SoldierSkill, count);
        }
    }
}

// 美人鱼之吻
public class GridActionCmd103001Skill1093 : GridActionCmdAttack
{
    public GridActionCmd103001Skill1093(DoQianyaoFun qianyaofun, DoAttackFun fun, int AttackSceneId, WalkDir AttackDir, int deep, int skillid)
        : base(qianyaofun, fun, AttackSceneId, AttackDir, deep, skillid)
    {
        m_CastTime = 0.5f;
        m_EventTime = 0.5f;
        m_Duration = 1.667f;
    }
    public override void StartWithTarget(Life Parent, StartAttackFun StartAttack)
    {
        base.StartWithTarget(Parent, StartAttack);
        PlayAction(AnimatorState.Attack81000, m_Start);
        //Transform t = null;
        //RolePropertyM rpm = m_Skin.iGameRole.GetRolePropertyM();
        //GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
        //if (posgo != null)
        //{
        //    t = posgo.transform;
        //    GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1055031", t.position, t);
        //    GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1.5f);
        //    gae.AddAction(ndEffect);
        //    m_effectgo = gae.gameObject;
        //    SoundPlay.JoinPlayQueue("hammer_hit", 1.5f);
        //}
    }

    public override void DoEvent()
    {
        base.DoEvent();
    }
}

// 美人鱼之歌 1092
public class GridActionCmd103001ActiveSkill : GridActionCmdActiveSkill
{
    public GridActionCmd103001ActiveSkill(DoQianyaoFun qianyaofun, DoAttackFun fun, int sceneID, int AttackSceneId, WalkDir AttackDir, int deep, int skillid, float blackscreentime)
        : base(qianyaofun, fun, sceneID, AttackSceneId, AttackDir, deep, skillid, blackscreentime)
    {
        m_CastTime = 0.95f;
        m_EventTime = 0.95f;
        m_Duration = 1.967f;
        PlayAction(AnimatorState.Skill01, m_Start);
    }
    public override void ActiiveStart()
    {
		SoundPlay.Play(m_LifePrent.m_Attr.ModelType.ToString() +"_skill", false, false);
        Transform t = null;
        //RolePropertyM rpm = m_Skin.iGameRole.GetRolePropertyM();
		GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
        if (posgo != null)
        {
            t = posgo.transform;

            //m_effectgo = SkillEffects._instance.LoadEffect("effect/prefab/", "1001081",t.position,0.75f,true);
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1201041", t.position, m_LifePrent.GetLifeProp().transform);
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
}

// 美人鱼之泪
public class GridActionCmd103001Skill1094 : GridActionCmdAttack
{
    public GridActionCmd103001Skill1094(DoQianyaoFun qianyaofun, DoAttackFun fun, int AttackSceneId, WalkDir AttackDir, int deep, int skillid)
        : base(qianyaofun, fun, AttackSceneId, AttackDir, deep, skillid)
    {
        m_CastTime = 0.66f;
        m_EventTime = 0.66f;
        m_Duration = 2f;
    }
    public override void StartWithTarget(Life Parent, StartAttackFun StartAttack)
    {
        base.StartWithTarget(Parent, StartAttack);
        PlayAction(AnimatorState.Attack82000, m_Start);

    }

    public override void DoEvent()
    {
        base.DoEvent();
    }
}
