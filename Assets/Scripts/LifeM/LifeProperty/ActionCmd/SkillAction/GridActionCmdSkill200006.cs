using UnityEngine;
using System.Collections;
/// <summary>
/// 弩炮熊技能 编号200006.
/// </summary>
/// 
/// //5009 普攻.
public class GridActionCmd200006Skill5009 :GridActionCmdActiveSkill{
	public GridActionCmd200006Skill5009(DoQianyaoFun qianyaofun,DoAttackFun fun,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
		:base(qianyaofun, fun,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime)
	{
		
		m_CastTime = 0.833f;
		m_EventTime = 0.833f;
		m_Duration = 1.4f;
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
		
		if (m_skillinfo.m_skilleffectinfo != null )//RoleModelsM.GetSkillResourcesData(PropSkillInfo.m_type,"HasBullet") == 1)
		{
			GameObject posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.FirePos);

			Transform fireplace = posgo.transform;
			if (fireplace != null)
			{
				string bulletname = m_skillinfo.m_skilleffectinfo.m_targeteffect;
				
				if (BattleEnvironmentM.GetLifeMBornNode(true) == null)
				{
					return;
				}
				GameObject go = GameObjectLoader.LoadPath("effect/prefab/",bulletname,fireplace.position,BattleEnvironmentM.GetLifeMBornNode(true));
				if (m_LifePrent.WalkDir == WalkDir.WALKRIGHT)
					go.transform.Rotate(0,180,0);
					//go.transform.localScale = new Vector3(-go.transform.localScale.x,go.transform.localScale.y,go.transform.localScale.z);

				ThroughBullet bullet = go.AddComponent<ThroughBullet>();
				if (bullet != null)
				{
					Bullet.BulletType t = Bullet.BulletType.Bullet;
					MapGrid mg;

					if (m_Dir == WalkDir.WALKLEFT)
					{
						Int2 posStart = new Int2(MapSize.GetGridStart(pos.GridPos.Layer), pos.GridPos.Layer);
						mg = MapGrid.GetMG(posStart);
					}
					else
					{
						Int2 posEnd = new Int2(MapSize.GetLayerMaxGrid(pos.GridPos.Layer), pos.GridPos.Layer);
						mg = MapGrid.GetMG(posEnd);
					}

					Vector3 vpos = mg.pos;
					if (m_skillinfo.m_skilleffectinfo.m_postion == 0)
						vpos.y = go.transform.localPosition.y;
					else
						vpos.y += 0.2f;

					bullet.SetInfo(vpos,(m_LifePrent as Role).RoleSkill.SkillUse,10f,m_LifePrent.WalkDir,(m_skillinfo as SoldierSkill),
					               Skill.GetSkillCamp(m_skillinfo,m_LifePrent),m_LifePrent.GetMapPos().Layer,true);
				}
			}
		}
		else
		{
			m_DoAttack(m_skillinfo as SoldierSkill,count);
		}
		//m_DoAttack(count);
	}
	
}