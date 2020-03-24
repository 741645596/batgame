using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using sdata;

public class EnemyFireInfo
{
	public int mFireQueue;
	public int mFireTime;
	public SoldierInfo mEnemy;
}


/// <summary>
/// 战役副本数据
/// </summary>
/// <author>zhulin</author>
public class StageM  {
	//副本关卡
	private static List<s_counterpartInfo> m_lstage = new List<s_counterpartInfo>();
	//副本地图
	private static List<s_countershipcanvasInfo> m_lMap = new List<s_countershipcanvasInfo>();
	//副本摆设
	private static List<s_countershipputInfo> m_lPut = new List<s_countershipputInfo>();
	//怪物
	private static List<s_monsterInfo> m_lmonster = new List<s_monsterInfo>();
	//怪物技能轮询
	private static List<s_monsterskillorderInfo> m_lmonsterskillorder = new List<s_monsterskillorderInfo>();

	//战役副本星级奖励
	private static List<s_stage_rewardInfo> m_lchapterreward = new List<s_stage_rewardInfo>();
	//战前对白
	private static List<s_scriptdialogueInfo> m_lstageDialog = new List<s_scriptdialogueInfo>();
	//战场环境
	private static List<s_environmentInfo> m_lenvironment = new List<s_environmentInfo>();

	private static List<s_bubble_promptInfo> m_lbubble_prompt = new List<s_bubble_promptInfo>();

	private static List<s_extra_stagedropInfo> m_lExtraDropInfo = new List<s_extra_stagedropInfo>();

	private static List<EnemyFireInfo> m_lFireEnemyInfo = new List<EnemyFireInfo>();
	

	private static bool _IsLoad = false;


	
	public static void Init (object obj)
	{
		if (_IsLoad == true)
			return;
		//加载数据
		
		System.Diagnostics.Debug.Assert(obj is StaticDataResponse);
		
		StaticDataResponse sdrsp = obj as StaticDataResponse;

		m_lstage = sdrsp.s_counterpart_info;
		m_lMap = sdrsp.s_countershipcanvas_info;
		m_lPut = sdrsp.s_countershipput_info;
		m_lmonster = sdrsp.s_monster_info;
		m_lmonsterskillorder = sdrsp.s_monsterskillorder_info;
		m_lchapterreward = sdrsp.s_stage_reward_info;
		m_lstageDialog = sdrsp.s_scriptdialogue_info;
		m_lenvironment = sdrsp.s_environment_info;
		m_lbubble_prompt = sdrsp.s_bubble_prompt_info;
		m_lExtraDropInfo = sdrsp.s_extra_stagedrop_info;

		_IsLoad = true;
		
	}
	
	/// <summary>
	/// 获取指定关卡的副本信息。
	/// </summary>
	public static CounterPartInfo GetCounterPartInfo(int StageID)
	{
		foreach(s_counterpartInfo Info in m_lstage)
		{
			if(Info.id == StageID)
			{
				return new CounterPartInfo(Info);
			}
		}
		return null;
	}
	/// <summary>
	/// 根据关卡id获取副本信息.
	/// </summary>
	/// <returns>The counter part info list.</returns>
	/// <param name="lStageID">L stage I.</param>
	public static List<CounterPartInfo> GetCounterPartInfoList(List<int> lStageID)
	{
		if(lStageID == null || lStageID.Count < 1)
		{
			return null;
		}
		List<CounterPartInfo> lInfo = new List<CounterPartInfo>();
		foreach(int stageId in lStageID)
		{
			CounterPartInfo info = GetCounterPartInfo(stageId);
			if(info != null)
			{
				lInfo.Add(info);
			}
		}
		return lInfo;
	}
	/// <summary>
	/// 根据关卡id获取扫荡信息.
	/// </summary>
	/// <returns>The counter part info list.</returns>
	/// <param name="lStageID">L stage I.</param>
	public static List<s_extra_stagedropInfo> GetExtraDropInfoInfoList(int stageID)
	{
		List<s_extra_stagedropInfo> lInfo = new List<s_extra_stagedropInfo>();
		for (int i = 0; i < m_lExtraDropInfo.Count; i++ )
		{
			s_extra_stagedropInfo info = m_lExtraDropInfo[i];
			if (stageID == info.counterpartid)
			{
				lInfo.Add(info);
			}
		}

		return lInfo;
	}
	/// <summary>
	/// 根据战役对白
	/// </summary>
	public static s_scriptdialogueInfo GetScriptdialogueInfo(int ID)
	{
		foreach(s_scriptdialogueInfo Info in m_lstageDialog)
		{
			if(Info.id == ID)
			{
				return Info;
			}
		}
		return null;
	}


	/// <summary>
	/// 获取关卡名称
	/// </summary>
	public static string GetChaptersName(int Chapters ,StageType type)
	{
		foreach(s_counterpartInfo Info in m_lstage)
		{
			if(Info.chapters == Chapters && Info.type == (int)type)
			{
				return Info.chaptersname;
			}
		}
		return string.Empty;
	}


	/// <summary>
	/// 确认是否包含该章。
	/// </summary>
	public static bool CheckHaveChapters(int Chapters ,StageType type)
	{
		foreach(s_counterpartInfo Info in m_lstage)
		{
			if(Info.chapters == Chapters && Info.type == (int)type)
			{
				return true;
			}
		}
		return false;
	}



	/// <summary>
	/// 获取该章节下的所有关卡
	/// </summary>
	public static List<CounterPartInfo> GetChaptersGate(int Chapters ,StageType type)
	{
		List<CounterPartInfo> l = new List<CounterPartInfo>();
		foreach(s_counterpartInfo Info in m_lstage)
		{
			if(Info.chapters == Chapters && Info.type == (int)type)
			{
				l.Add(new CounterPartInfo(Info));
			}
		}
		return l;
	}

	/// <summary>
	/// 获取副本ID
	/// </summary>
	public static int GetStageID(StageType type ,int Chapters ,int Stage)
	{
		return 100000 *(int)type + 1000 * Chapters + Stage ;
	}
	/// <summary>
	/// 获取指定副本地图
	/// </summary>
	public static ShipCanvasInfo GetCounterPartMap(int countershipcanvasid)
	{
		foreach(s_countershipcanvasInfo Info in m_lMap)
		{
			if(Info.id == countershipcanvasid)
			{
				return new ShipCanvasInfo(Info);
			}
		}
		return null;
	}

	/// <summary>
	/// 获取副本地图的摆设数据
	/// </summary>
	public static void GetCounterPartShipPut(int StageID,ref List<SoldierInfo> lSoldier,ref List<BuildInfo> lBuild)
	{
		if(lSoldier == null)
			lSoldier = new List<SoldierInfo>();
		lSoldier.Clear();
		if(lBuild == null)
			lBuild = new List<BuildInfo>();
		lBuild.Clear();

		foreach(s_countershipputInfo Info in m_lPut)
		{
			if(Info.counterpartid == StageID)
			{
				if(Info.type == (int) ShipBuildType.Soldier)
				{
					SoldierInfo I = Getmonster(Info);
					if(I != null)
						lSoldier.Add(I);
				}
				else
				{
					// BuildInfo b = buildingM.GetBuildInfo(Info);
					BuildInfo b = buildingM.GetStageBuildInfo(Info);
					if(b != null)
					lBuild.Add(b);
				}
			}
		}
	}
	/// <summary>
	/// 获取副本地图的摆设数据
	/// </summary>
	public static void GetCounterPartShipPut(int StageID, ref List<EnemyFireInfo> lSoldier, ref List<BuildInfo> lBuild)
	{
		if (lSoldier == null)
			lSoldier = new List<EnemyFireInfo>();
		lSoldier.Clear();
		if (lBuild == null)
			lBuild = new List<BuildInfo>();
		lBuild.Clear();

		foreach (s_countershipputInfo Info in m_lPut)
		{
			if (Info.counterpartid == StageID)
			{
				if (Info.type == (int)ShipBuildType.Soldier)
				{
					if(Info.type == 2)
					{
						SoldierInfo I = Getmonster(Info);
						if (I != null)
						{
							EnemyFireInfo enemy = new EnemyFireInfo();
							enemy.mEnemy = I;
							enemy.mFireQueue = Info.data0;
							enemy.mFireTime = Info.data1;
							lSoldier.Add(enemy);
						}
					}
				}
				else
				{
					// BuildInfo b = buildingM.GetBuildInfo(Info);
					BuildInfo b = buildingM.GetStageBuildInfo(Info);
					if (b != null)
						lBuild.Add(b);
				    }
			}
		}
	}
	/// <summary>
	/// 获取怪物数据
	/// </summary>
	public static SoldierInfo Getmonster(s_countershipputInfo Info)
	{
		if(Info == null) return null;
		s_monsterInfo monster = GetMonsterInfo(Info.objid);
		if(monster == null) return null;
		SoldierInfo I =  SoldierM.GetSoldierInfo(monster);
		if (I.m_modeltype == 200009 || I.m_modeltype == 102003)
		{
			s_monsterInfo monsterturn = GetMonsterInfo(Info.objid + 1);
			I.m_TurnInfo =  SoldierM.GetSoldierInfo(monsterturn);
			
			if(I.m_TurnInfo != null)
			{
				I.m_TurnInfo.CX = Info.cx;
				I.m_TurnInfo.CY = Info.cy;
				s_monsterskillorderInfo order = GetSkillOrder(Info.objid +1);
				if(order != null && I.m_TurnInfo.m_Skill != null)
				{
					I.m_TurnInfo.m_Skill.attack1 = order.attack;
					I.m_TurnInfo.m_Skill.attack2 = order.attack1;
				}
			}
		}
		if(I != null)
		{
			I.CX = Info.cx;
			I.CY = Info.cy;
			s_monsterskillorderInfo order = GetSkillOrder(Info.objid);
			if(order != null && I.m_Skill != null)
			{
				I.m_Skill.attack1 = order.attack;
				I.m_Skill.attack2 = order.attack1;
			}
		}
		return I;
	}
	
	/// <summary>
	/// 根据怪物ID 获取怪物数据
	/// </summary>
	private static sdata.s_monsterInfo GetMonsterInfo(int monsterid)
	{
		foreach(s_monsterInfo Info in m_lmonster)
		{
			if(Info.id == monsterid)
			{
				return Info;
			}
		}
		return null;
	}

	private static s_monsterskillorderInfo GetSkillOrder(int monsterid)
	{
		foreach(s_monsterskillorderInfo v in m_lmonsterskillorder)
		{
			if (v.id == monsterid)
			{
				return v;
			}
			
		}
		return null;
	}
	/// <summary>
	/// 获取一个关卡的奖励
	/// </summary>
	public static List<int> GetStageReward(int StageID)
	{
		List<int> lReward = new List<int>();

		foreach(s_countershipputInfo Info in m_lPut)
		{
			if(Info.counterpartid == StageID)
			{
				if(Info.type == (int) ShipBuildType.Soldier)
				{
					List<int> l	= GetMonsterReward(Info.objid);
					lReward.AddRange(l);
				}
			}
		}
		return lReward;
	}
	/// <summary>
	/// 物品可以从哪些关卡中获得.
	/// </summary>
	/// <returns>The reward from stage.</returns>
	/// <param name="itemTypeID">Item type I.</param>
	public static List<int> GetRewardFromStage(int itemTypeID)
	{
		List<int> lCounterpartid = new List<int>();
		foreach(s_countershipputInfo Info in m_lPut)
		{
			if(Info.type == (int) ShipBuildType.Soldier)
			{
				List<int> l	= GetMonsterReward(Info.objid);
				foreach(int itemId in l)
				{
					if(itemId == itemTypeID)
					{
						lCounterpartid.Add(Info.counterpartid);
						break;
					}
				}
			}
		}
		return lCounterpartid;
	}


	/// <summary>
	/// 获取怪物奖励
	/// </summary>
	private static List<int> GetMonsterReward(int monsterid)
	{
		List<int> lReward = new List<int>();
		s_monsterInfo  Info = GetMonsterInfo( monsterid);

		if(Info != null)
		{
			if(Info.drop1 > 0)
				lReward.Add(Info.drop1);
			if(Info.drop2 > 0)
				lReward.Add(Info.drop2);
			if(Info.drop3 > 0)
				lReward.Add(Info.drop3);
			if(Info.drop4 > 0)
				lReward.Add(Info.drop4);
			if(Info.drop5 > 0)
				lReward.Add(Info.drop5);
			if(Info.drop6 > 0)
				lReward.Add(Info.drop6);
			if(Info.drop7 > 0)
				lReward.Add(Info.drop7);
			if(Info.drop8 > 0)
				lReward.Add(Info.drop8);
			if(Info.drop9 > 0)
				lReward.Add(Info.drop9);
			if(Info.drop10 > 0)
				lReward.Add(Info.drop10);
		}
		return lReward;
	}
	/// <summary>
	/// 找到能攻击的第一个关卡
	/// </summary>
	public static int FindFirstCanAttackStage(StageType Type ,int stage)
	{
		int minStage = 0;
		bool bFirst = true;
		foreach(s_counterpartInfo Info in m_lstage)
		{
			if( Info.type == (int)Type && Info.id > stage)
			{
				if(bFirst == true)
				{
					minStage = Info.id;
					bFirst = false;
				}
				else if(Info.id < minStage)
				{
					minStage = Info.id;
				}
			}
		}
		if(minStage == 0 )
			minStage = stage;
		return minStage ;
	}

	//
	/// <summary>
	/// 找到能攻击的第一个关卡
	/// </summary>
	public static List<s_stage_rewardInfo> GetChapterReward(StageType Type ,int Chapter)
	{
		List<s_stage_rewardInfo> l = new List<s_stage_rewardInfo>();
		foreach(s_stage_rewardInfo Info in m_lchapterreward)
		{
			if( Info.type == (int)Type && Info.chapter == Chapter)
			{
				l.Add(Info);
			}
		}
		if(l != null && l.Count > 1)
		{
			l.Sort ((a, b)=>{
				if(a.star > b.star)
					return 1;
				else if(a.star == b.star)
					return 0; 
				else return -1;});
		}
		return l;
	}
	/// <summary>
	/// 获取章节星级奖励
	/// </summary>
	public static Dictionary<s_itemtypeInfo ,int> GetChapterRewardItem(int ChapterRewardID ,ref int Coin, ref int Diamond)
	{
		Dictionary<s_itemtypeInfo ,int> l = new Dictionary<s_itemtypeInfo ,int>();
		foreach(s_stage_rewardInfo Info in m_lchapterreward)
		{
			if( Info.id == ChapterRewardID)
			{
				Coin = Info.reward_coin;
				Diamond = Info.reward_diamond;
				if(Info.num1 > 0 && Info.item1id > 0)
				{
					s_itemtypeInfo I = new s_itemtypeInfo();
					I =ItemM.GetItemInfo(Info.item1id);
					l.Add(I ,Info.num1);
				}
				if(Info.num2 > 0 && Info.item2id > 0)
				{
					s_itemtypeInfo I = new s_itemtypeInfo();
					I =ItemM.GetItemInfo(Info.item2id);
					l.Add(I ,Info.num2);
				}
				if(Info.num3 > 0 && Info.item3id > 0)
				{
					s_itemtypeInfo I = new s_itemtypeInfo();
					I =ItemM.GetItemInfo(Info.item3id);
					l.Add(I ,Info.num3);
				}
				if(Info.num4 > 0 && Info.item4id > 0)
				{
					s_itemtypeInfo I = new s_itemtypeInfo();
					I =ItemM.GetItemInfo(Info.item4id);
					l.Add(I ,Info.num4);
				}
				if(Info.num5 > 0 && Info.item5id > 0)
				{
					s_itemtypeInfo I = new s_itemtypeInfo();
					I =ItemM.GetItemInfo(Info.item5id);
					l.Add(I ,Info.num5);
				}
			}
		}
		return l;
	}

	/// <summary>
	/// 获取战役对白
	/// </summary>
	public static void GetStageTalk(int id,ref List<CounterPartDialogUnit> lDialog)
	{
		if(lDialog == null)
			lDialog = new List<CounterPartDialogUnit>();
		lDialog.Clear();

		s_scriptdialogueInfo I = GetScriptdialogueInfo(id) ;
		if(I == null) return ;
		//1
		if(I.data1 != null && I.data1 != "" && I.data1 != "0")
		{
			CounterPartDialogUnit v = new CounterPartDialogUnit();
			v.position = I.scriptposition1;
			v.npcid = I.object1;
			v.talk = I.data1;
			lDialog.Add(v);
		}
		//2
		if(I.data2 != null && I.data2 != "" && I.data2 != "0")
		{
			CounterPartDialogUnit v = new CounterPartDialogUnit();
			v.position = I.scriptposition2;
			v.npcid = I.object2;
			v.talk = I.data2;
			lDialog.Add(v);
		}
		//3
		if(I.data3 != null && I.data3 != "" && I.data3 != "0")
		{
			CounterPartDialogUnit v = new CounterPartDialogUnit();
			v.position = I.scriptposition3;
			v.npcid = I.object3;
			v.talk = I.data3;
			lDialog.Add(v);
		}
		//4
		if(I.data4 != null && I.data4 != "" && I.data4 != "0")
		{
			CounterPartDialogUnit v = new CounterPartDialogUnit();
			v.position = I.scriptposition4;
			v.npcid = I.object4;
			v.talk = I.data4;
			lDialog.Add(v);
		}
		//5
		if(I.data5 != null && I.data5 != "" && I.data5 != "0")
		{
			CounterPartDialogUnit v = new CounterPartDialogUnit();
			v.position = I.scriptposition5;
			v.npcid = I.object5;
			v.talk = I.data5;
			lDialog.Add(v);
		}
		//6
		if(I.data6 != null && I.data6 != "" && I.data6 != "0")
		{
			CounterPartDialogUnit v = new CounterPartDialogUnit();
			v.position = I.scriptposition6;
			v.npcid = I.object6;
			v.talk = I.data6;
			lDialog.Add(v);
		}
	}
	/// <summary>
	/// 获取战役冒泡
	/// </summary>
	public static void GetBubblePromt(int stageid,ref List<CounterBubblePromtInfo> lBubblePromt)
	{
		if(lBubblePromt == null)
			lBubblePromt = new List<CounterBubblePromtInfo>();
		lBubblePromt.Clear();
		//m_lbubble_prompt
		foreach(s_bubble_promptInfo Info in m_lbubble_prompt)
		{
			if(Info.stageid == stageid)
			{
				CounterBubblePromtInfo V = new CounterBubblePromtInfo();
				V.x =(Info.cx *0.1f);
				V.y = (Info.cy *0.1f);
				V.text = Info.description ;

				lBubblePromt.Add(V);
			}
		}
	}

	/// <summary>
	/// 获取战场环境信息 .
	/// </summary>
	/// <returns>The environment info.</returns>
	/// <param name="id">Identifier.</param>
	public static sdata.s_environmentInfo GetEnvironmentInfo(int id)
	{
		foreach(s_environmentInfo Info in m_lenvironment)
		{
			if(Info.id == id)
			{
				return Info;
			}
		}
		return null;
	}
}
