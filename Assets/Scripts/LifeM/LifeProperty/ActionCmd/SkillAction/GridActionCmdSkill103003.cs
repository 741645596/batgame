using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 火狐普攻 1111
/// </summary>
/// 
public class GridActionCmd103003Skill1111 :GridActionCmdAttack
{
	public GridActionCmd103003Skill1111(DoQianyaoFun qianyaofun, DoAttackFun fun, int AttackSceneId, WalkDir AttackDir, int deep, int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		m_CastTime = 0.42f;
		m_EventTime = 0.42f;
		m_Duration = 1f;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		PlayAction(AnimatorState.Attack85000,m_Start);
        Transform t = null;
        //RolePropertyM rpm = m_Skin.iGameRole.GetRolePropertyM();
		GameObject goLeftHand = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.LeftHandPos);
		GameObject goRightHand = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.LeftHandPos);
		
		if (goLeftHand != null && goRightHand != null)
        {
            t = goLeftHand.transform;
			GameObjectActionExcute gaeLeftHand = EffectM.LoadEffect(EffectM.sPath, "1202031_01", t.position, t);
            GameObjectActionDelayDestory ndEffectL = new GameObjectActionDelayDestory(1.5f);
			gaeLeftHand.AddAction(ndEffectL);

			t = goRightHand.transform;
			GameObjectActionExcute gaeRightHand = EffectM.LoadEffect(EffectM.sPath, "1202031_01", t.position, t);
			GameObjectActionDelayDestory ndEffectR = new GameObjectActionDelayDestory(1.5f);
			gaeRightHand.AddAction(ndEffectR);
			m_effectgo = gaeRightHand.gameObject;

			SoundPlay.JoinPlayQueue("atc_thor", 1.5f);
        }
		GameObject goRoot = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
		if (goRoot != null)
		{
			t = goRoot.transform;
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1202031_02", t.position, t);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(0.5f);
			gae.AddAction(ndEffect);
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
				string bulletLeft = "1202071";
				string bulletRight = "1202071";

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


				if (m_LifePrent.WalkDir == WalkDir.WALKLEFT)
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

// 火焰圈
public class GridActionCmd103003Skill1113 : GridActionCmdAttack
{
	public GridActionCmd103003Skill1113(DoQianyaoFun qianyaofun, DoAttackFun fun, int AttackSceneId, WalkDir AttackDir, int deep, int skillid)
		: base(qianyaofun, fun, AttackSceneId, AttackDir, deep, skillid)
	{
		m_CastTime = 1f;
		m_EventTime = 1f;
		m_Duration = 1.5f;
	}
	public override void StartWithTarget(Life Parent, StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent, StartAttack);
		PlayAction(AnimatorState.Attack82000, m_Start);

		Transform t = null;
		//RolePropertyM rpm = m_Skin.iGameRole.GetRolePropertyM();

		GameObject go = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
		if (go != null)
		{
			t = go.transform;
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1202051", t.position, t);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
			gae.AddAction(ndEffect);
		}
	}

	public override void DoEvent()
	{
		base.DoEvent();
	}

}

// 火焰盾
public class GridActionCmd103003Skill1114 : GridActionCmdAttack
{
	public GridActionCmd103003Skill1114(DoQianyaoFun qianyaofun, DoAttackFun fun, int AttackSceneId, WalkDir AttackDir, int deep, int skillid)
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

		Transform t = null;
		//RolePropertyM rpm = m_Skin.iGameRole.GetRolePropertyM();

		GameObject go = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
		if (go != null)
		{
			t = go.transform;
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1202041", t.position, t);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
			gae.AddAction(ndEffect);
		}
	}

	public override void DoEvent()
	{
		base.DoEvent();
	}

}

// 火狐大招 火尾舞
public class GridActionCmd103003ActiveSkill : GridActionCmdActiveSkill
{
	private bool bEffect = false;
	public GridActionCmd103003ActiveSkill(DoQianyaoFun qianyaofun, DoAttackFun fun, int sceneID, int AttackSceneId, WalkDir AttackDir, int deep, int skillid, float blackscreentime)
        : base(qianyaofun, fun, sceneID, AttackSceneId, AttackDir, deep, skillid, blackscreentime)
    {
        m_CastTime = 0.3f;
        m_EventTime = 0.9f;
        m_Duration = 2.167f;
    }
    public override void ActiiveStart()
    {
		SoundPlay.Play("skill_thor", false, false);
        Transform t = null;
        //RolePropertyM rpm = m_Skin.iGameRole.GetRolePropertyM();
		// 尾巴
        GameObject goTail = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.Bone1Pos);
		if (goTail != null)
        {
			t = goTail.transform;
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1202061_01", t.position, m_LifePrent.GetLifeProp().transform);
            GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1.5f);
            gae.AddAction(ndEffect);
        }

		GameObject posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
		if (posgo != null && (m_LifePrent as Role).RoleSkill.m_AttackTarget.m_thisT != null)
		{

			Vector3 pos = posgo.transform.position;
			//pos.z  = -1.5f;
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1202061_02", posgo.transform.position, posgo.transform);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(2f);
			gae.AddAction(ndEffect);
			/*Life ttagrt = (m_LifePrent as Role).RoleSkill.m_AttackTarget;
			Vector3 tpos = ttagrt.GetCenterPos();

			float dis = Mathf.Abs(posgo.transform.position.x - tpos.x);
			gae.transform.localScale = new Vector3(-dis * 0.25f, 1, 1);*/

		}

		PlayAction(AnimatorState.Skill01, m_Start);
    }

}

