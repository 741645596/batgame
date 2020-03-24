using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 条件技能基类
/// </summary>
/// <author>zhulin</author>
public class GridActionCmdConditionSkill:GridActionSkill{
	
	public float m_realDuration;
	public float m_oldduration;
	public float m_eventtime;
	///public delegate void DoAttackFun(SkillInfo skill, int count);
	public DoAttackFun m_DoSkill;
	public bool Played  = false;
	public int m_effectcount;
	public bool QianYaoPlayed  = false;
	public float m_CastTime = 0;
	public DoQianyaoFun m_QianYaoStatus;
	//产生次数
	public int m_effecttime;
	public bool m_ismove;
	//间隔
	public List<float> m_timeinterval;
	protected SkillInfo m_skillinfo;
	public void SetAttackInfo(int effecttime,List<float> timeinterval)
	{
		m_effecttime = effecttime;
		m_timeinterval = timeinterval;
	}
	public GridActionCmdConditionSkill()
	{
		
		m_effectcount = 0;
		m_effecttime = 1;
		m_timeinterval = new List<float>();
		m_timeinterval.Add(0f);
		m_ismove = false;
	}
	public virtual   void StartWithTarget(Life Parent)
	{
		SetTarget(Parent);
		
		m_skillinfo = (Parent as Role).PropSkillInfo;
		if (m_realDuration <= 0)
		{
			Update();
		}
	}
	public override   void Update () {
		base.Update();
		UpdatePos();
		if (m_TimeCount > m_CastTime && !QianYaoPlayed && m_QianYaoStatus != null)
		{
			m_QianYaoStatus();
			QianYaoPlayed = true;
		}		
		if (m_effectcount < m_effecttime && m_TimeCount > (m_eventtime + m_timeinterval[m_effectcount]/*m_timeinterval * m_effectcount*/))
		{
			if (m_TimeCount - m_Delatime <= (m_eventtime + m_timeinterval[m_effectcount]/*m_timeinterval * m_effectcount*/))
			{
				DoEvent();
				m_effectcount ++;
				DoAttack(m_effectcount);
			}
			//DoEvent();
		}
		/*if (m_TimeCount > m_eventtime && !Played)
		{
			m_DoSkill();
			Played = true;
		}*/
	}
	public virtual void UpdatePos()
	{
	}
	public override void SetDone ()
	{
		base.SetDone ();
		m_realDuration = m_TimeCount;
	}
	public bool RealDone()
	{
		if (m_Duration >= m_realDuration)
		{
			PlayAction(AnimatorState.Stand,m_End,true);
			return true;
		}
		return false;
	}
	
	public void ExtendDuration( float d,Vector3 start,Vector3 end,bool ismove)
	{
		m_Start = start;
		m_End = end;
		m_ismove = ismove;
		if (m_Duration < m_realDuration)
		{
			m_oldduration = m_Duration;
			m_Duration += d;
			if (m_Duration > m_realDuration)
				m_Duration = m_realDuration;
			
		}
	}
	
	public virtual void DoEvent()
	{
	}
	public virtual void DoAttack(int count)
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
			
			if (m_skillinfo.m_type == 1041)
				posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.LeftHandPos);
			else if (m_skillinfo.m_type == 1044)
				posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.bagPos);
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
					go.transform.localScale = new Vector3(-go.transform.localScale.x,go.transform.localScale.y,go.transform.localScale.z);
				Bullet bullet = go.AddComponent<Bullet>();
				if (bullet != null)
				{
					Bullet.BulletType t = Bullet.BulletType.Bullet;
					Vector3 vpos = pos.pos;
					if (m_skillinfo.m_skilleffectinfo.m_postion == 0)
						vpos.y = go.transform.localPosition.y;
					else
						vpos.y += 0.2f;
					bullet.SetInfo(vpos,m_DoSkill,10f,m_LifePrent.WalkDir,t,(m_skillinfo as SoldierSkill),true);
					//vpos,m_DoAttack,10f,m_LifePrent.WalkDir,t,m_skillinfo as SoldierSkill);
				}
			}
		}
		else
		{
			m_DoSkill(m_skillinfo as SoldierSkill,count);
		}
		//m_DoAttack(count);
	}
}






