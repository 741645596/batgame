using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PetSkill : Skill {
	Pet m_parent;
	public float m_timecount;
	public int m_times;

	/// <summary>
	/// 初始化技能数据，从数据中心获取所有技能数据
	/// </summary>
	public override bool Init(int SceneID,LifeMCore Core)
	{
		this.m_SceneID = SceneID;
		
		m_parent = CM.GetLifeM(m_SceneID,LifeMType.PET) as Pet;
		PetInfo Info = CmCarbon.GetPetInfo(Core);
		m_skill = Info.m_skillinfo1;
		return true;
	}
	/// <summary>
	/// 技能cd时间
	/// </summary>
	public override void Update (float deltaTime) 
	{
		if (m_timecount <= m_times * (m_skill as SoldierSkill).m_timeinterval* 0.001f)
		{
			m_timecount += Time.deltaTime;
			if (m_timecount > m_times * (m_skill as SoldierSkill).m_timeinterval*0.001f)
			{
				m_times ++;
				DoSkill(m_times);
			}
		}
	}

	/// <summary>
	/// 检测攻击目标
	/// </summary>
	/// <param name="target"></param>
	/// <param name="g"></param>
	/// <param name="Sort"></param>
	/// <param name="radus"></param>
	/// <param name="dir">线性搜索需要方向</param>
	/// <returns></returns>
	 public  bool CheckRangeAttackTarget(Life target, MapGrid g,int Sort, float radus)
	{
		Int2 AttackStation2 = target.GetMapPos();
		if (!NdUtil.IsSameMapLayer(g.GridPos, AttackStation2))
			return false;
		
		if (Sort == 3)
		{
			if (target.m_thisT.localPosition.x >= (m_parent.m_thisT.localPosition.x - radus * MapGrid.m_width) &&
			    target.m_thisT.localPosition.x <= (m_parent.m_thisT.localPosition.x + radus * MapGrid.m_width))
				return true;
		}
		
		
		return false;
	}
	public List<Life> GetRangeAttackList(MapGrid g,SkillInfo skill,LifeMCamp camp,Life targetAttack)
	{
		int Sort = (skill as SoldierSkill) .m_sort;
		float radus = (skill as SoldierSkill).m_range/ MapGrid.m_Pixel;
		List<Life> l = new List<Life>();
		List<Life> lr = new List<Life>();
		CM.SearchLifeMListInBoat(ref lr,LifeMType.SOLDIER, camp);
		foreach(Role r in lr)
		{
			if ( CheckRangeAttackTarget(r,g,Sort,radus))
				l.Add(r);
		}
		
		return l;
	}

	public void DoSkill(int times)
	{
		if(m_skill == null ) return ;
		SoldierSkill skill = m_skill as SoldierSkill ;
		List<Life> targetlist = new List<Life>();
		LifeMCamp camp = m_parent.m_Core.m_Camp == LifeMCamp.ATTACK?LifeMCamp.DEFENSE : LifeMCamp.ATTACK;
		targetlist =  GetRangeAttackList(MapGrid.GetMG(m_parent.m_Pos),skill,camp,null);
		if (skill.m_actiontype == 0)
		{
			StatusSelfBuff(m_parent,m_skill);
			for(int i =0; i < targetlist.Count; i++)
			{
				if (!targetlist[i].InBoat)
					continue;
				SkillReleaseInfo info = Life.CalcDamage(m_parent,targetlist[i],null,m_skill);
				if (skill.m_attckmodeid > 0)
				{
					float power = skill.GetAttackPower(times);
					info.m_Damage = (int)(info.m_Damage * power);
				}
				Transform attackT = m_parent.m_thisT;
				targetlist[i].ApplyDamage (info, attackT);
			}
			//Debug.Log(str);
		}
		else if (skill.m_actiontype == 1)
		{
			StatusSelfBuff(m_parent,m_skill);
			for(int i =0; i < targetlist.Count; i++)
			{
				if (!targetlist[i].InBoat)
					continue;
				SkillReleaseInfo info = Life.CalcDamage(m_parent,targetlist[i],null,m_skill);
				Debug.Log("doskill  " + m_skill.m_name + "," + info.m_Damage);
				if (skill.m_attckmodeid > 0)
				{
					float power = skill.GetAttackPower(times);
					info.m_Damage = (int)(info.m_Damage * power);
				}
				Transform attackT = m_parent.m_thisT;
				targetlist[i].ApplyDamage (info, attackT);
			}
			//Debug.Log(str);
		}
	}
}
