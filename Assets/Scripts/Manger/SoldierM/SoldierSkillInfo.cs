using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using sdata;

/// <summary>
/// 技能你信息管理
/// </summary>
/// <author>zhulin</author>
/// 
public enum SkillTypeData
{
	Fire     = 0,    //炮战
	General  = 1,    //普攻
	BigSkill = 2,    //大招
	Second   = 3,    //次要技能
}


public class SoldierSkillInfo
{
	public List<SoldierSkill> m_Skillinfo  = new List<SoldierSkill>(); 
	
	public int m_gskilltype ;
	public int m_skill1_type ;
	public int m_skill2_type ;
	public int m_skill3_type ;
	public int m_skill4_type ;
	public int m_skill5_type ;
	public int m_lskilltype;
	public int m_askilltype;
	public int m_skill1_level ;
	public int m_skill2_level ;
	public int m_skill3_level ;
	public int m_skill4_level ;
	public int m_skill5_level ;
	public int m_askill_level ;
	public string attack1;
	public string attack2;
	
	public SoldierSkillInfo()
	{
		
	}
	
	public void Copy( SoldierSkillInfo s)
	{
		this.m_gskilltype = s.m_gskilltype ;
		this.m_skill1_type = s.m_skill1_type ;
		this.m_skill2_type = s.m_skill2_type;
		this.m_skill3_type = s.m_skill3_type;
		this.m_skill4_type = s.m_skill4_type;
		this.m_skill5_type = s.m_skill5_type;
		this.m_lskilltype = s.m_lskilltype;
		this.m_askilltype = s.m_askilltype;
		this.m_skill1_level = s.m_skill1_level;
		this.m_skill2_level = s.m_skill2_level;
		this.m_skill3_level = s.m_skill3_level;
		this.m_skill4_level = s.m_skill4_level;
		this.m_skill5_level = s.m_skill5_level;
		this.m_askill_level = s.m_askill_level;
		this.attack1 = s.attack1;
		this.attack2 = s.attack2;
	}
	/// <summary>
	/// 用于战斗奖励的炮弹兵数据初始化
	/// </summary>
	public void SetSkillType(s_soldier_typeInfo Info)
	{
		if(Info == null) return ;
		m_gskilltype = Info.gskilltype;
		m_lskilltype = Info.lskilltype;
		m_skill1_type = Info.skill1_type;
		m_skill2_type = Info.skill2_type;
		m_skill3_type = Info.skill3_type;
		m_skill4_type = Info.skill4_type;
		m_skill5_type = Info.skill5_type;
		m_askilltype = Info.askilltype;
	}


	public void SetSkillLevel(List<int> l)
	{
		if(l == null || l.Count != 6)
		{
			return ;
		}
        if (l[0]!=-1)
        {
            m_askill_level = l[0];
        }
        if (l[1] != -1)
        {
            m_skill1_level = l[1];
        }
        if (l[2] != -1)
        {
            m_skill2_level = l[2];
        }
        if (l[3] != -1)
        {
            m_skill3_level = l[3];
        }
        if (l[4] != -1)
        {
            m_skill4_level = l[4];
        }
        if (l[5] != -1)
        {
            m_skill5_level = l[5];
        }

	}


	public void UpSkillLevel(int SkillLevel, int SkillIndex)
	{
		if(SkillIndex == 0)
		{
			m_askill_level = SkillLevel;
		}
		else if(SkillIndex == 1)
		{
			m_skill1_level = SkillLevel;
		}
		else if(SkillIndex == 2)
		{
			m_skill2_level = SkillLevel;
		}
		else if(SkillIndex == 3)
		{
			m_skill3_level = SkillLevel;
		}
		else if(SkillIndex == 4)
		{
			m_skill4_level = SkillLevel;
		}
		else if(SkillIndex == 5)
		{
			m_skill5_level = SkillLevel;
		}

		InitSkill();
	}

	public void SetSkill(s_monsterInfo monster)
	{
		if(monster == null) return ;
		SoldierSkill Info = new SoldierSkill();
		//0
		SkillM.GetSkillInfo (monster.gskillid,  ref Info);
		m_Skillinfo.Add(Info);
		//1
		Info = new SoldierSkill();
		SkillM.GetSkillInfo (monster.skill1id, ref Info);
		m_skill1_level = Info.m_level;
		m_Skillinfo.Add(Info);
		//2
		Info = new SoldierSkill();
		SkillM.GetSkillInfo (monster.skill2id, ref Info);
		m_skill2_level = Info.m_level;
		m_Skillinfo.Add(Info);
		//3
		Info = new SoldierSkill();
		SkillM.GetSkillInfo (monster.skill3id, ref Info);
		m_skill3_level = Info.m_level;
		m_Skillinfo.Add(Info);
		//4
		Info = new SoldierSkill();
		SkillM.GetSkillInfo (monster.skill4id, ref Info);
		m_skill4_level = Info.m_level;
		m_Skillinfo.Add(Info);
		//5
		Info = new SoldierSkill();
		SkillM.GetSkillInfo (monster.skill5id, ref Info);
		m_skill5_level = Info.m_level;
		m_Skillinfo.Add(Info);
		//6
		Info = new SoldierSkill();
		SkillM.GetSkillInfo (monster.askillid, ref Info);
		m_Skillinfo.Add(Info);
		//7
		Info = new SoldierSkill();
		SkillM.GetSkillInfo (monster.lskillid, ref Info);
		m_Skillinfo.Add(Info);
	}

	
	
	public  void InitSkill()
	{
		m_Skillinfo.Clear();
		SoldierSkill Info = new SoldierSkill();
		//0
		SkillM.GetSkillInfo (m_gskilltype, 1, ref Info);
		m_Skillinfo.Add(Info);
		//1
		Info = new SoldierSkill();
		if(m_skill1_level == 0)
		{
			SkillM.GetSkillInfo (m_skill1_type, 1, ref Info);
			Info.m_enable = false;
			Info.m_enableQuality = ConfigM.GetEnableSkillQuatlity(2);
		}
		else SkillM.GetSkillInfo (m_skill1_type, m_skill1_level, ref Info);
		m_Skillinfo.Add(Info);
		//2
		Info = new SoldierSkill();
		if(m_skill2_level == 0)
		{
			SkillM.GetSkillInfo (m_skill2_type, 1, ref Info);
			Info.m_enable = false;
			Info.m_enableQuality = ConfigM.GetEnableSkillQuatlity(3);
		}
		else SkillM.GetSkillInfo (m_skill2_type, m_skill2_level, ref Info);
		m_Skillinfo.Add(Info);
		//3
		Info = new SoldierSkill();
		if(m_skill3_level == 0)
		{
			SkillM.GetSkillInfo (m_skill3_type, 1, ref Info);
			Info.m_enable = false;
			Info.m_enableQuality = ConfigM.GetEnableSkillQuatlity(4);
		}
		else SkillM.GetSkillInfo (m_skill3_type, m_skill3_level, ref Info);
		m_Skillinfo.Add(Info);
		//4
		Info = new SoldierSkill();
		if(m_skill4_level == 0)
		{
			SkillM.GetSkillInfo (m_skill4_type, 1, ref Info);
			Info.m_enable = false;
			Info.m_enableQuality = ConfigM.GetEnableSkillQuatlity(5);
		}
		else SkillM.GetSkillInfo (m_skill4_type, m_skill4_level, ref Info);
		m_Skillinfo.Add(Info);
		//5
		Info = new SoldierSkill();
		if(m_skill5_level == 0)
		{
			SkillM.GetSkillInfo (m_skill5_type, 1, ref Info);
			Info.m_enable = false;
			Info.m_enableQuality = ConfigM.GetEnableSkillQuatlity(6);
		}
		else SkillM.GetSkillInfo (m_skill5_type, m_skill5_level, ref Info);
		m_Skillinfo.Add(Info);
		//6
		Info = new SoldierSkill();
		if(m_askill_level == 0)
		{
			SkillM.GetSkillInfo (m_askilltype, 1, ref Info);
			Info.m_enable = false;
			Info.m_enableQuality = ConfigM.GetEnableSkillQuatlity(1);
		}
		else SkillM.GetSkillInfo (m_askilltype, m_askill_level, ref Info);
		m_Skillinfo.Add(Info);
		//7
		Info = new SoldierSkill();
		SkillM.GetSkillInfo (m_lskilltype, 1, ref Info);
		m_Skillinfo.Add(Info);
	}
    /// <summary>
    /// 获取可以升级的技能
    /// </summary>
    /// <returns></returns>
    public List<SoldierSkill> GetUpdateSkills()
    {
        List<SoldierSkill> skills = new List<SoldierSkill>();
        skills.Add(m_Skillinfo[6]);
        skills.Add(m_Skillinfo[1]);
        skills.Add(m_Skillinfo[2]);
        skills.Add(m_Skillinfo[3]);
        skills.Add(m_Skillinfo[4]);
        skills.Add(m_Skillinfo[5]);
        return skills;
    }

	/// <summary>
	/// 获取技能
	/// </summary>
	public SoldierSkill GetSkill(int SkillNo)
	{
		if(SkillNo >= 0 && SkillNo <= 6)
		{
			SoldierSkill s = m_Skillinfo[SkillNo];
			return s;
		}
		
		return null;
		
	}
	
	/// <summary>
	/// 获取大招
	/// </summary>
	public SoldierSkill GetBigSkill()
	{
		
		return m_Skillinfo[1];
	}
	
	/// <summary>
	/// 获取炮战技能
	/// </summary>
	public SoldierSkill GetFireSkill()
	{
		return m_Skillinfo[6];
	}
	/// <summary>
	/// 获取次要攻击技能
	/// </summary>
	public SoldierSkill GetLSkill()
	{
		if(m_Skillinfo.Count >= 7)
			return m_Skillinfo[7];
		else return null;
	}

	/// <summary>
	/// 添加被动技能附加属性
	/// </summary>
	public void SetAddrData(ref AddAttrInfo AddAttr)
	{
		if(AddAttr == null) return ;
		AddAttr.SetAddAttrInfo(m_Skillinfo);
	}

	/// <summary>
	/// 计算技能等级之和
	/// </summary>
	public int GetTotalSkillLevel()
	{
		return  m_skill1_level  
			   + m_skill2_level 
		       + m_skill3_level 
			   + m_skill4_level 
			   + m_skill5_level 
			   + m_askill_level ;
	}
}

