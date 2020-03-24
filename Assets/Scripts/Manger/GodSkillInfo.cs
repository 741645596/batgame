using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using captain;

public class GodSkillInfo  {
	public int m_id;
	public int m_type;
	public int m_level;
	public string m_name;
	public int m_action;
	public int m_target;
	public int m_multiple;
	public float m_range;
	public int m_power;
	public int m_power2;
    public int m_selectTarget;
	public List<SkillStatusInfo> m_own_status_info = new List<SkillStatusInfo>();
	public List<SkillStatusInfo> m_enemy_status_info = new List<SkillStatusInfo>();
	public List<int> m_mana = new List<int>();
	public int m_times;
	public float m_blackscreentime;
	public int m_crystalneed;
	public int m_coinneed;
	public int m_woodneed;
	public string m_explain;
    /// <summary>
    /// s_itemtype id
    /// </summary>
	public int m_needbook;
	public int m_needbooknum;
	public string m_stardescription;
	public GodSkillInfo()
	{
	}
}

public class CaptionInfo
{
	public int m_id;
    /// <summary>
    /// s_captain id (1,2,3...)
    /// </summary>
	public int m_captionid;
	public int m_godskilltype1;
	public int m_level1;
	public int m_godskilltype2;
	public int m_level2;
	public int m_godskilltype3;
	public int m_level3;
	public int m_godskilltype4;
	public int m_level4;
	public int m_levelneed;
    /// <summary>
    /// 召唤所需碎片数量
    /// </summary>
    public int m_needbooknum;
	public int m_star;

	public Dictionary<int,GodSkillInfo> m_GodSkill = new Dictionary<int, GodSkillInfo>();
	public CaptionInfo()
	{
	}

	public CaptionInfo(CaptionInfo c)
	{
		this.m_id = c.m_id;
		this.m_captionid = c.m_captionid;
		this.m_godskilltype1 = c.m_godskilltype1;
		this.m_godskilltype2 = c.m_godskilltype2;
		this.m_godskilltype3 = c.m_godskilltype3;
		this.m_godskilltype4 = c.m_godskilltype4;
		this.m_level1 = c.m_level1;
		this.m_level2 = c.m_level2;
		this.m_level3 = c.m_level3;
		this.m_level4 = c.m_level4;
		this.m_levelneed = c.m_levelneed;
        this.m_needbooknum = c.m_needbooknum;
		this.m_star = c.m_star;

	}

	public void SetCaption(CaptainInfo info)
	{
		m_id = info.id > 0 ? info.id : m_id;
		m_star = info.star > 0? info.star : m_star;
		m_level1 = info.level1 > 0? info.level1 : m_level1;
		m_godskilltype1 = info.godskilltype1 > 0? info.godskilltype1 : m_godskilltype1;
		GodSkillInfo skill = new GodSkillInfo();
		if (m_godskilltype1 > 0 && m_level1 > 0)
		{
			GodSkillM.GetGodSkill(m_godskilltype1,m_level1,m_star,ref skill);
			m_GodSkill[m_godskilltype1] = skill;
		}
		skill = new GodSkillInfo();
		if (info.godskilltype2 > 0 && info.level2 > 0)
		{
			m_level2 = info.level2;
			m_godskilltype2 = info.godskilltype2;
			GodSkillM.GetGodSkill(info.godskilltype2,info.level2,info.star,ref skill);
			m_GodSkill[info.godskilltype2] = skill;
		}
		skill = new GodSkillInfo();
		if (info.godskilltype3 > 0 && info.level3 > 0)
		{
			m_level3 = info.level3;
			m_godskilltype3 = info.godskilltype3;
			GodSkillM.GetGodSkill(info.godskilltype3,info.level3,info.star,ref skill);
			m_GodSkill[info.godskilltype3] = skill;
		}
		skill = new GodSkillInfo();
		if (info.godskilltype4 > 0 && info.level4 > 0)
		{
			m_level4 = info.level4;
			m_godskilltype4 = info.godskilltype4;
			GodSkillM.GetGodSkill(info.godskilltype4,info.level4,info.star,ref skill);
			m_GodSkill[info.godskilltype4] = skill;
		}
	}


	public void SetCaption(int level)
	{
		GodSkillInfo skill = new GodSkillInfo();
		m_level1 = level;
		m_level2 = level;
		m_level3 = level;
		m_level4 = level;
		if (m_godskilltype1 > 0)
		{
            if (m_star == 0)
                m_star = 1;
            
			GodSkillM.GetGodSkill(m_godskilltype1,level,m_star,ref skill);
			if (m_GodSkill.ContainsKey(m_godskilltype1))
				m_GodSkill[m_godskilltype1] = skill;
			else
				m_GodSkill.Add(m_godskilltype1,skill);
		}
		skill = new GodSkillInfo();
		if (m_godskilltype2 > 0)
		{
			GodSkillM.GetGodSkill(m_godskilltype2,level,m_star,ref skill);
			if (m_GodSkill.ContainsKey(m_godskilltype2))
				m_GodSkill[m_godskilltype2] = skill;
			else
				m_GodSkill.Add(m_godskilltype2,skill);
		}
		skill = new GodSkillInfo();
		if (m_godskilltype3 > 0)
		{
			GodSkillM.GetGodSkill(m_godskilltype3,level,m_star,ref skill);
			if (m_GodSkill.ContainsKey(m_godskilltype3))
				m_GodSkill[m_godskilltype3] = skill;
			else
				m_GodSkill.Add(m_godskilltype3,skill);
		}
		skill = new GodSkillInfo();
		if (m_godskilltype4 > 0)
		{
			GodSkillM.GetGodSkill(m_godskilltype4,level,m_star,ref skill);
			if (m_GodSkill.ContainsKey(m_godskilltype4))
				m_GodSkill[m_godskilltype4] = skill;
			else
				m_GodSkill.Add(m_godskilltype4,skill);
		}
	}

    /// <summary>
    /// 目前只使用 m_godskilltype1
    /// </summary>
	public GodSkillInfo GetGodSkillInfo()
	{
		return m_GodSkill[m_godskilltype1];
	}
}
