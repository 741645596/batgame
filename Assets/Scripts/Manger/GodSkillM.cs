using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using sdata;

public class GodSkillM {
	
	private static List<sdata.s_godskilltypeInfo> m_lgodskill = new List<sdata.s_godskilltypeInfo>();
    private static List<sdata.s_captainInfo> m_lcaption = new List<sdata.s_captainInfo>();
	private static List<sdata.s_captainexpressionInfo> m_lcaptionexpress = new List<sdata.s_captainexpressionInfo>();
	private static List<sdata.s_godskillstarInfo> m_lGodSkillStar = new List<sdata.s_godskillstarInfo>();
	private static bool _IsLoad = false;
	// Use this for initialization
	public static void Init (object obj)
	{
		if (_IsLoad == true)
			return;
		System.Diagnostics.Debug.Assert(obj is sdata.StaticDataResponse);
		
		sdata.StaticDataResponse sdrsp = obj as sdata.StaticDataResponse;
		
		m_lgodskill = sdrsp.s_godskilltype_info;
		m_lcaption = sdrsp.s_captain_info;
		m_lcaptionexpress = sdrsp.s_captainexpression_info;
		m_lGodSkillStar = sdrsp.s_godskillstar_info;
		_IsLoad = true;
		
	}
	public static void GetGodSkill(int type, int level,int star, ref GodSkillInfo info)
	{
		GetGodSkill(type,level,ref info);
		foreach(sdata.s_godskillstarInfo gs in m_lGodSkillStar)
		{
			if (gs.godskilltype == type && gs.godskillstar == star)
			{
				info.m_needbook = gs.needbook;
				info.m_needbooknum = gs.needbooknum;
				info.m_stardescription = gs.description;
				SetGrowData(gs.growtype0,gs.grownum0,ref info);
				SetGrowData(gs.growtype1,gs.grownum1,ref info);
				SetGrowData(gs.growtype2,gs.grownum2,ref info);
				int lentime = NdUtil.GetLength(gs.statustime);
				int lentype = NdUtil.GetLength(gs.statustype);
				for(int i = 0; i < info.m_own_status_info.Count; i++)
				{
					if (gs.statustime != "" && i < lentime)
						info.m_own_status_info[i].m_time += NdUtil.GetIntValue(gs.statustime,i);
					if (info.m_own_status_info[i].m_effect.Count > 1 && gs.statustype != "")
					{
						for(int j = 0; j < info.m_own_status_info[i].m_effect.Count; j+=2)
						{
							for(int k = 0; k < lentype; k++)
							{
								if (NdUtil.GetIntValue(gs.statustype,i) == info.m_own_status_info[i].m_effect[j])
									info.m_own_status_info[i].m_effect[j+1] += NdUtil.GetIntValue(gs.statusvalue,i);
							}
						}
					}
				}
				break;
				/*info.m_statustime = gs.statustime;
				info.m_statustype = gs.statustype;
				info.m_statusvalue = gs.statusvalue;*/
			}
		}
	}
	public static void SetGrowData(int type, int num, ref GodSkillInfo info)
	{
		if (type == 1)
			info.m_power += num;
		else if(type == 2)
			info.m_range += num * 0.01f;
		else if (type == 4)
			info.m_power2 += num;
	}
	public static void GetGodSkill(int type, int level, ref GodSkillInfo info)
	{
		foreach(sdata.s_godskilltypeInfo gs in m_lgodskill)
		{
			if (gs.type == type && gs.level == level)
			{
				info.m_id = gs.id;
				info.m_type = gs.type;
				info.m_level = gs.level;
				info.m_name = gs.name;
				info.m_action = gs.action;
				info.m_target = gs.target;
				info.m_multiple = gs.multiple;
				info.m_range = gs.range * 0.01f;
				info.m_power = gs.power;
				info.m_power2 = gs.power2;
                info.m_selectTarget = gs.selecttarget;
				info.m_own_status_info = new List<SkillStatusInfo>();
				info.m_enemy_status_info = new List<SkillStatusInfo>();
				info.m_mana = new List<int>();

				SkillM.SetSkillStatus(ref info.m_own_status_info ,gs.own_status);
				SkillM.SetSkillStatus(ref info.m_enemy_status_info ,gs.enemy_status);
				int length = NdUtil.GetLength(gs.mana);
				if(length > 0)
				{
					for(int i = 0; i < length; ++i)
					{
						info.m_mana.Add(NdUtil.GetIntValue(gs.mana ,i));
					}
				}
				info.m_times = gs.times;
				info.m_blackscreentime = gs.blackscreentime * 0.001f;
				info.m_crystalneed = gs.crystalneed;
				info.m_coinneed = gs.coinneed;
				info.m_woodneed = gs.woodneed;
				info.m_explain = gs.explain;
				break;
			}
		}
	}
    /// <summary>
    /// 获取黑科技
    /// </summary>
    /// <param name="id">s_captian id</param>
	public static void GetCaption(int id,ref CaptionInfo info)
	{
		foreach(sdata.s_captainInfo c in m_lcaption)
		{
			if (c.id == id)
			{
				info.m_captionid = c.id;
				info.m_godskilltype1 = c.godskilltype1;
				info.m_godskilltype2 = c.godskilltype2;
				info.m_godskilltype3 = c.godskilltype3;
				info.m_godskilltype4 = c.godskilltype4;
				info.m_levelneed = c.levelneed;
                info.m_needbooknum = c.needbooknum;
				break;
			}
		}
	}
    /// <summary>
    /// 获取黑科技召唤所需的数量
    /// </summary>
    public static int GetCaptainNeedBookNum(int sCaptainID)
    {
        CaptionInfo info = new CaptionInfo();
        GetCaption(sCaptainID, ref info);
        return info.m_needbooknum;
    }
    /// <summary>
    /// 获取所有黑科技（包括未召唤）
    /// </summary>
	public static List<CaptionInfo> GetCaptions()
	{
		List<CaptionInfo> lcap = new List<CaptionInfo>();
		foreach(sdata.s_captainInfo c in m_lcaption)
		{
			CaptionInfo info = new CaptionInfo();
			info.m_captionid = c.id;
			info.m_godskilltype1 = c.godskilltype1;
			info.m_godskilltype2 = c.godskilltype2;
			info.m_godskilltype3 = c.godskilltype3;
			info.m_godskilltype4 = c.godskilltype4;
			info.m_levelneed = c.levelneed;
			info.SetCaption(1);
			lcap.Add(info);

		}
		return lcap;
	}

	public static List<CaptionInfo> GetCaptionGreatleval(int level)
	{
		List<CaptionInfo> lcap = new List<CaptionInfo>();
		foreach(sdata.s_captainInfo c in m_lcaption)
		{
			if (c.levelneed > level)
			{
				CaptionInfo info = new CaptionInfo();
				info.m_captionid = c.id;
				info.m_godskilltype1 = c.godskilltype1;
				info.m_godskilltype2 = c.godskilltype2;
				info.m_godskilltype3 = c.godskilltype3;
				info.m_godskilltype4 = c.godskilltype4;
				info.m_levelneed = c.levelneed;
				info.SetCaption(1);
				lcap.Add(info);

			}
		}
		return lcap;
	}

	public static List<CaptionInfo> GetCaptionGreatleval(int level,List<CaptionInfo> l)
	{
		List<CaptionInfo> lcap = new List<CaptionInfo>();
		foreach(sdata.s_captainInfo c in m_lcaption)
		{
			bool contain = false;
			foreach(CaptionInfo d in l)
			{
				if(c.id == d.m_captionid)
				{
					contain = true;
					break;
				}
			}
			if (c.levelneed > level && !contain)
			{
				CaptionInfo info = new CaptionInfo();
				info.m_captionid = c.id;
				info.m_godskilltype1 = c.godskilltype1;
				info.m_godskilltype2 = c.godskilltype2;
				info.m_godskilltype3 = c.godskilltype3;
				info.m_godskilltype4 = c.godskilltype4;
				info.m_levelneed = c.levelneed;
				info.SetCaption(1);
				lcap.Add(info);
			}
		}
		return lcap;
	}

	/// <summary>
	/// 获取解锁黑科技需要最低等级.
	/// </summary>
	/// <returns>The caption need level.</returns>
	public static int GetCaptionNeedLevel()
	{
		int level = 0;
		foreach(sdata.s_captainInfo c in m_lcaption)
		{
			if(level == 0) 
			{
				level = c.levelneed;
				continue;
			}
			if(c.levelneed < level)
			{
				level = c.levelneed;
			}
		}
		return level;
	}
    /// <summary>
    /// 获取黑科技最低星级（目前都为1）
    /// </summary>
    public static int GetCaptionMinStarLevel(int godskilltype)
    {
        return 1;
    }

    public static int GetCaptainID(int godskillType)
    {
        foreach(s_captainInfo info in m_lcaption)
        {
            if (info.godskilltype1 == godskillType)
            {
                return info.id;
            }
        }
        return 0;
    }

    public static int GetGodSkillType(int needbook)
    {
        foreach (s_godskillstarInfo info in m_lGodSkillStar)
        {
            if (info.needbook == needbook)
            {
                return info.godskilltype;
            }
        }
        return 0;
    }

	public static int GetCaptionExpress(int id,int type)
	{		
		foreach(sdata.s_captainexpressionInfo c in m_lcaptionexpress)
		{
			if (c.id == id && c.type == type)
			{
				return c.expression;
			}
		}
		return  -1;
	}
}
