using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using sdata;

/// <summary>
/// 过关信息
/// </summary>
public class GatePassInfo  {
	public int star ; //星级
	public int times; //当天已攻击次数
	public int stageid; //关卡ID

	/// <summary>
	/// 关卡节点
	/// </summary>
	public int CounterNode
	{
		get{return stageid % 1000;}
	}
	
	public int CouterType
	{
		get{return stageid / 100000;}
	}


	public int CouterChapter
	{
		get{return  (stageid %100000) /1000 ;}
	}
}

/// <summary>
/// 副本关卡信息
/// </summary>
public class CounterPartInfo
{
	public int id ;
	public int type ;
	public int mode ;
	public int chapters ;
	public string chaptersname ;
	public string counterpartname ;
	public string description ;
	public int times ;
	public int levellimit ;
	public int soldierlevellimit ;
	public int soldierquality ;
	public int rewardexp ;
	public int rewardglod ;
	public int decklevel ;
	public int win_physical ;
	public int lose_physical ;
	public int data0 ;
	public int countershipcanvasid;
	public int iscaptain;
	public int isboss;
	public string drop;
	//战役前对白
	public List<CounterPartDialogUnit> m_StageStartTalk = new List<CounterPartDialogUnit>();
	//战役后对白
	public List<CounterPartDialogUnit> m_StageEndTalk = new List<CounterPartDialogUnit>();
	//战役冒泡
	public List<CounterBubblePromtInfo> m_lBubblePromt = new List<CounterBubblePromtInfo>();

	public CounterPartInfo()
	{}
	public CounterPartInfo(s_counterpartInfo Info)
	{
		this.id = Info.id;
		this.type = Info.type;
		this.mode = Info.mode;
		this.chapters = Info.chapters;
		this.chaptersname = Info.chaptersname;
		this.counterpartname = Info.counterpartname;
		this.description = Info.description;
		this.times = Info.times;
		this.levellimit = Info.levellimit;
		this.soldierlevellimit = Info.soldierlevellimit;
		this.soldierquality = Info.soldierquality;
		this.rewardexp = Info.rewardexp;
		this.rewardglod = Info.rewardglod;
		this.decklevel = Info.decklevel;
		this.win_physical = Info.win_physical;
		this.lose_physical = Info.lose_physical;
		this.data0 = Info.data0;
		this.countershipcanvasid = Info.countershipcanvasid;
		this.iscaptain = Info.iscaptain;
		this.isboss = Info.isboss;
		//s_scriptdialogueInfo I = GetScriptdialogueInfo(id);
		if (Info.endscript > 0)
		{
			s_scriptdialogueInfo I = StageM.GetScriptdialogueInfo(Info.endscript);
			this.drop = I.drop;
		}
		
		StageM.GetStageTalk(Info.startscript,ref this.m_StageStartTalk);
		StageM.GetStageTalk(Info.endscript,ref this.m_StageEndTalk);
		StageM.GetBubblePromt(this.id,ref this.m_lBubblePromt);
	}

	/// <summary>
	/// 关卡节点
	/// </summary>
	public int CounterNode
	{
		get{return id % 1000;}
	}
}


/// <summary>
/// 副本关卡信息
/// </summary>
public class CounterPartDialogUnit
{
	public int position ;
	public int npcid ;
	public string talk ;
}


/// <summary>
/// 副本关卡信息
/// </summary>
public class CounterBubblePromtInfo
{
	public float x ;
	public float y ;
	public string text ;
}