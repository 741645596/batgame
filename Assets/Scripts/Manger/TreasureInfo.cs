using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TreasureInfo  {
	public int m_treasureid;
	//public int m_treasuretype;
	public int m_canvasid;
	public int m_scale;
	public int m_endtime;
	public int m_profit_coin;
	public int m_profit_diamond;
	public int m_rob_coin;
	public int m_rob_diamond;
	public TreasureInfo()
	{
	}
	public TreasureInfo(treasure.TreasureInfoResponse.TreasureInfo info)
	{
		m_treasureid = info.treasure_id;
		//m_treasuretype = info.treasure_type;
		m_canvasid = info.canvas_id;
		m_scale = info.scale;
		m_endtime = info.endtime;
		m_rob_coin = info.rob_coin;
		m_rob_diamond = info.rob_diamond;
		m_profit_coin = info.profit_coin;
		m_profit_diamond = info.profit_diamond;
	}
}


public class RobTreasureInfo{
	public int pirate_id;
	public battle.DefInfoResponse definfo;
	public int surplus_coin;
	public int surplus_diamond;
	public List<treasure.TreasureObjInfo> def_status;
	public List<treasure.TreasureObjInfo> self_status;
	public int m_state;
	public int endtime;
	public int rob_count;
	public int power;
	/// <summary>
	/// 船只起点编号.
	/// </summary>
	public int origialNum;
	//是否复仇
	public int avenge;

	public void SetInfo(treasure.TreasureSearchInfo info)
	{
		pirate_id= info.pirate_id;
		definfo = info.definfo;
		surplus_coin = info.surplus_coin;
		surplus_diamond = info.surplus_diamond;
		def_status = info.def_status;
		endtime = info.end_time;
		rob_count = info.rob_count;
		self_status = info.self_status;
		power = info.power;
		origialNum = info.origin;
		avenge = info.avenge;
	}
}

public class BattleListInfo
{
	public string battleindex;
	public int win;
	public string level;
	public string name;
	public string servername;
	public int rob_time;
	public int rob_coin;
	public int rob_diamond;
	public int flag;
	/// <summary>
	/// 0为复仇，1已复仇，2，不可复仇.
	/// </summary>
	public int avenge;
	public BattleListInfo(treasure.TreasureBattleListResponse.BattleListInfo info)
	{
		battleindex = info.battleindex;
		rob_time = info.rob_time;
		rob_coin = info.rob_coin;
		rob_diamond = info.rob_diamond;
		flag = info.flag;
		level = info.level;
		name = info.name;
		servername = info.servername;
		win = info.win;
		avenge = info.avenge;
	}
	public BattleListInfo()
	{
	}
}
public class ReportDetail
{
	
	public string battleindex;
	public int report_id;
	
	public battle.DefInfoResponse definfo;
	public List<treasure.TreasureObjInfo> defstatus;
	public List<treasure.TreasureObjInfo> robinfo;
	
	public string level;
	public string name;
	public string servername;
	public int win;
	public int power;
	
	public ReportDetail(treasure.TreasureBattleReportDetailResponse.ReportDetail info)
	{
		battleindex = info.battleindex;
		report_id = info.report_id;
		definfo = info.definfo;
		defstatus = info.defstatus;
		robinfo = info.robinfo;
		level = info.level;
		name = info.name;
		servername = info.servername;
		win = info.win;
		power = info.power;
	}
}




