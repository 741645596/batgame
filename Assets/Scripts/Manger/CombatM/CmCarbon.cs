using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using battle;


/// <summary>
/// 战斗副本待加载数据
/// </summary>


/// <summary>
/// 炮弹兵状态
/// </summary>
public enum SoldierState
{
	NoBorn  = 0, //未发射，未出生
	Born    = 1, //已出生
	Dead    = 2, //死亡
}


public class SoldierTypeInfo
{            
	public SoldierState m_State;    
	public SoldierInfo m_Soldier;
	public int mFireQueue;
	public float mFireTime;
	public SoldierTypeInfo(SoldierState State , SoldierInfo Info)
    {
		m_State = State;
		m_Soldier = Info;
    }
	public SoldierTypeInfo(SoldierState State, SoldierInfo Info, int queue, int time)
	{
		m_State = State;
		m_Soldier = Info;
		mFireQueue = queue;
		mFireTime = time;
	}
};

public class CaptainSkillInfo
{            
	public  GodSkill m_Skill = new GodSkill();
	public  int m_Mana;
	public  int m_CaptainID ;    //船长ID
	public CaptainSkillInfo(int CaptainID ,LifeMCamp Camp, int mana)
	{
		m_CaptainID = CaptainID ;
		CaptionInfo God = BlackScienceDC.GetCaption(CaptainID);
		m_Skill.SetSkill(God.GetGodSkillInfo()) ;
		m_Skill.Camp = Camp;
		m_Mana = mana;
	}
};




public class CmCarbon
{
	private static Dictionary<int, SoldierTypeInfo> m_AttackList = new Dictionary<int, SoldierTypeInfo>();
	private static Dictionary<int, EnemyFireInfo> m_EnemyFireInfoList = new Dictionary<int, EnemyFireInfo>();
	//防守方的。
	private static ShipCanvasInfo m_CanvansInfo = new ShipCanvasInfo();

	private static Dictionary<int, SoldierInfo> m_DefenseList = new  Dictionary<int, SoldierInfo>();
	private static Dictionary<int,BuildInfo>m_DefenseBuild = new Dictionary<int,BuildInfo>();
	private static FloorInfo m_Floor = new FloorInfo(); 
	private static UserInfo m_DefenderUserInfo = new UserInfo();
	public static  UserInfo DefenderInfo
	{
		get {
			if(m_DefenderUserInfo == null)
				m_DefenderUserInfo = new UserInfo();
			return m_DefenderUserInfo;}
	}
	private static List<int> m_WallList = new List<int>();

    //进攻方炮弹兵死亡的数量
    private static int m_AttackDeadSoldierNum = 0;

    private static int WinGold = 0;
    private static int WinWood = 0;
	private static int WinItem = 0;
	//船长技，
	private static Dictionary<bool, CaptainSkillInfo> m_CaptainSkill = new  Dictionary<bool, CaptainSkillInfo>();
	//死亡的玩家炮弹兵
	private static Dictionary<int,Int2>m_DiePlayersoldier = new Dictionary<int,Int2>();
	//死亡的非玩家炮弹兵
	private static Dictionary<int,Int2>m_DieNotPlayersoldier = new Dictionary<int,Int2>();
	//被摧毁的建筑
	private static List<int> m_DieBuild = new List<int>();

	private static int g_DataID = 0 ;
	private static int g_CurrentDataID = 0;

	private static Queue<SoldierInfo> m_QueueSoldierInfos = new Queue<SoldierInfo>();
	
	//战役前对白
	private static List<CounterPartDialogUnit> m_StartTalk = new List<CounterPartDialogUnit>();
	private static bool m_StartTalkOver = true;
	public static bool StartTalkOver
	{
		get {return m_StartTalkOver;}
		set{m_StartTalkOver = value;}
	}
	//战役后对白
	private static List<CounterPartDialogUnit> m_EndTalk = new List<CounterPartDialogUnit>();
	private static bool m_EndTalkOver = true;
	public static bool EndTalkOver
	{
		get {return m_EndTalkOver;}
		set{m_EndTalkOver = value;}
	}
	
    //加载副本数据
	public static void LoadCmCombon()
    {
        GetWall();
    }


	public static void SetDefenseData(battle.DefInfoResponse defInfo)
	{
		SetDefenseMap(defInfo);
		SetDefenseSoldier(defInfo.soldier_info);
		SetDefenseBuild(defInfo.shipput_info);
		SetDefenseFloor(defInfo);
		SetDefenseUserInfo(defInfo);
	}
	
	/// <summary>
	/// 设置防御方炮弹兵数据。
	/// </summary>
	private static void SetDefenseSoldier(List<battle.SoldierInfo> soldierInfo)
	{
		m_DefenseList.Clear();
		for (int i = 0; i < soldierInfo.Count; i++)
		{
			
			SoldierInfo info = SoldierM.GetSoldierInfo(soldierInfo[i]);
			if(info == null) continue ;
			if (!m_DefenseList.ContainsKey(info.ID))
			{
				m_DefenseList.Add(info.ID, info);
			}
		}
	}

	/// <summary>
	/// 设置防御方炮弹兵数据。
	/// </summary>
	public static void SetPVEMonisterSoldier(List<SoldierInfo> lsoldierInfo)
	{
		m_DefenseList.Clear();
		for (int i = 0; i < lsoldierInfo.Count; i++)
		{
			SoldierInfo info = new SoldierInfo();
			info = lsoldierInfo[i];
			//副本没有dataid
			info.ID = CmCarbon.GetDataID();
			if (!m_DefenseList.ContainsKey(info.ID))
			{
				m_DefenseList.Add(info.ID, info);
			}
		}
	}

	/// <summary>
	/// 设置防御方炮弹兵数据。
	/// </summary>
	public static void SetDefenseSoldier(List<SoldierInfo> lsoldierInfo)
	{
		m_DefenseList.Clear();
		for (int i = 0; i < lsoldierInfo.Count; i++)
		{
			SoldierInfo info = new SoldierInfo();
			info = lsoldierInfo[i];
			//副本没有dataid
			if (!m_DefenseList.ContainsKey(info.ID))
			{
				m_DefenseList.Add(info.ID, info);
			}
		}
	}
	/// <summary>
	/// 设置防御方建筑物数据
	/// </summary>
	private static void SetDefenseBuild(List<battle.ShipBuildInfo> shipBuildInfo)
	{
		m_DefenseBuild.Clear();
		if(shipBuildInfo == null|| shipBuildInfo.Count == 0) return ;
		foreach(battle.ShipBuildInfo ShipBuild in shipBuildInfo)
		{
			ShipBuildData.SaveShipBuildData(ShipBuild ,ref m_DefenseBuild);
		}
	}
	/// <summary>
	/// 设置防御方建筑物数据
	/// </summary>
	public static void SetDefenseBuild(List<BuildInfo> lbuildInfo)
	{
		m_DefenseBuild.Clear();
		if(lbuildInfo == null|| lbuildInfo.Count == 0) return ;
		foreach(BuildInfo Info in lbuildInfo)
		{
			ShipBuildData.SaveShipBuildData(Info ,ref m_DefenseBuild);
		}
	}


	/// <summary>
	/// 设置防御方建筑物数据
	/// </summary>
	public static void SetDefenseBuild(List<ShipPutInfo> lbuildInfo)
	{
		m_DefenseBuild.Clear();
		if(lbuildInfo == null|| lbuildInfo.Count == 0) return ;
		foreach(ShipPutInfo Info in lbuildInfo)
		{
			ShipBuildData.SaveShipBuildData(Info  ,ref m_DefenseBuild);
		}
	}

	/// <summary>
	/// 设置防守方地图数据
	/// </summary>
	private static void SetDefenseMap(battle.DefInfoResponse defInfo)
	{
		m_CanvansInfo.EmptyMapArea();
		if(defInfo == null ) return ;
			
		m_CanvansInfo.SetMapData(defInfo.width,defInfo.height,defInfo.map,defInfo.shape);
	}

	public static void SetDefenseMap(ShipCanvasInfo Info)
	{
		m_CanvansInfo.EmptyMapArea();
		if(Info != null)
		{
			m_CanvansInfo.SetMapData(Info.width ,Info.height,Info.map,Info.shape,Info.skin,Info.environment);
		}
	}


	/// <summary>
	/// 设置防守方甲板数据
	/// </summary>
	private static void SetDefenseFloor(battle.DefInfoResponse defInfo)
	{
		m_Floor = DeckM.GetFloorInfo(defInfo.deck_level);
	}

	public static void SetDefenseFloor(int deck_level)
	{
		m_Floor = DeckM.GetFloorInfo(deck_level);
	}

	/// <summary>
	/// 设置防守方玩家信息
	/// </summary>
	private static void SetDefenseUserInfo(battle.DefInfoResponse defInfo)
	{
		m_DefenderUserInfo.Name  = defInfo.user_info.name;
		m_DefenderUserInfo.Level = defInfo.user_info.level;
		m_DefenderUserInfo.Coin  = defInfo.resource_info.coin;
		m_DefenderUserInfo.Wood  = defInfo.resource_info.wood;
	}


	/// <summary>
	/// 设置防守方玩家信息
	/// </summary>
	public static void SetDefenseUserInfo(CounterPartInfo Info,List<BuildInfo> lb)
	{
		m_DefenderUserInfo.Name  = Info.counterpartname;
		m_DefenderUserInfo.Level = Info.CounterNode;
		m_DefenderUserInfo.Coin  = Info.rewardglod;

		int wood = 0;
		if (lb != null)
		{
			foreach(BuildInfo b in lb)
				wood += b.m_wood;
		}
		m_DefenderUserInfo.Wood  = wood;
	}
	
	/// <summary>
    /// 设置攻击方炮弹兵
    /// </summary>
	public static void SetAttackSoldier(List<SoldierInfo> Soldierlist)
    {
        if (m_AttackList == null)
			m_AttackList = new Dictionary<int, SoldierTypeInfo>();
        m_AttackList.Clear();
        //船员
		foreach (SoldierInfo Info in Soldierlist)
        {
			m_AttackList[GetDataID()] = new SoldierTypeInfo(SoldierState.NoBorn ,Info);
        }
    }

	/// <summary>
	/// 设置攻击方炮弹兵
	/// </summary>
	public static void SetAttackSoldier(List<EnemyFireInfo> Soldierlist)
	{
		if (m_AttackList == null)
			m_AttackList = new Dictionary<int, SoldierTypeInfo>();
		m_AttackList.Clear();
		Soldierlist.Sort(SortEnemys);
		//船员
		foreach (EnemyFireInfo Info in Soldierlist)
		{
			m_AttackList[GetDataID()] = new SoldierTypeInfo(SoldierState.NoBorn, Info.mEnemy, Info.mFireQueue, Info.mFireTime);
		}
	}

	static int SortEnemys(EnemyFireInfo a, EnemyFireInfo b)
	{
		if (a.mFireTime > b.mFireTime)
		{
			return 1;
		}
		if (a.mFireTime < b.mFireTime)
		{
			return -1;
		}
		return 0;
	}

    /// <summary>
    /// 获取攻击方炮弹兵
    /// </summary>
    public static void GetAttackList(ref List<int> Soldierlist)
    {
        if (Soldierlist == null)
            Soldierlist = new List<int>();
        Soldierlist.Clear();
		
        if (m_AttackList != null)
        {
			foreach (int key  in m_AttackList.Keys)
            {
				Soldierlist.Add(key);
            }
        }
    }
    /// <summary>
    /// 判断炮弹兵是否已经发射。
    /// true 已经发射，false 未发射
    /// SoildierID 炮弹兵ID
    /// </summary>
	public static bool IsBorn(int DataID)
    {
		if (m_AttackList.ContainsKey(DataID))
        {
			if(m_AttackList[DataID].m_State == SoldierState.NoBorn)
				return false;
			else return true;
        }
        return true;
    }

    /// <summary>
    /// 设置炮弹兵发射完成
    /// SoildierID 炮弹兵ID
    /// </summary>
	public static void SetBorn(int DataID)
    {
		if (m_AttackList.ContainsKey(DataID))
        {
			m_AttackList[DataID].m_State = SoldierState.Born;
        }
    }


	/// <summary>
	/// 设置炮弹兵发射完成
	/// SoildierID 炮弹兵ID
	/// </summary>
	public static void SetUnBorn(int DataID)
	{
		if (m_AttackList.ContainsKey(DataID))
		{
			m_AttackList[DataID].m_State = SoldierState.NoBorn;
		}
	}

    public static int GetFireOutCount()
    {
        int count = 0;
        foreach (var a in m_AttackList)
        {
			if (a.Value.m_State != SoldierState.NoBorn)
            {
                count++;
            }
        }
        return count;
    }

    /// <summary>
    /// 判断所有炮弹兵是否已经发射。
    /// true 全部已经发射完成，false 未发射完成
    /// </summary>
    public static bool IsAllFireOut()
    {
		foreach (SoldierTypeInfo s in m_AttackList.Values)
        {
			if (s.m_State == SoldierState.NoBorn)
                return false;
        }
        return true;
    }
	
    public static void GetDefenseList(ref List<int> list)
    {
        if (list == null)
            list = new List<int>();

        foreach (int key in m_DefenseList.Keys)
        {
            list.Add(key);
        }
    }
	/// <summary>
	/// 获取玩家炮弹兵
	/// true 全部已经发射完成，false 未发射完成
	/// </summary>
	public static void GetPlayerSoldier(ref List<SoldierInfo> list)
	{
		if(list == null)
			list = new List<SoldierInfo>();
		list.Clear();
		LifeMCamp Camp = GetPlayer2Camp(true);
		if(Camp == LifeMCamp.DEFENSE )
		{
			foreach(SoldierInfo Info in m_DefenseList.Values)
			{
				list.Add(Info);
			}
		}
		else 
		{
			foreach(SoldierTypeInfo Info in m_AttackList.Values)
			{
				list.Add(Info.m_Soldier);
			}
		}
	}

	public static void GetPlayerList(ref List<int> list)
	{
		if(list == null)
			list = new List<int>();
		list.Clear();
		LifeMCamp Camp = GetPlayer2Camp(true);
		if(Camp == LifeMCamp.DEFENSE )
		{
			foreach(int key in m_DefenseList.Keys)
			{
				list.Add(key);
			}
		}
		else 
		{
			foreach(int key in m_AttackList.Keys)
			{
				list.Add(key);
			}
		}
	}


    public static int GetDenfenseCount()
    {
        if (m_DefenseList != null)
            return m_DefenseList.Count;
        else return 0;
    }


    public static int GetAttackCount()
    {
        if (m_AttackList != null)
            return m_AttackList.Count;
        else return 0;
    }

    public static int GetWinGold()
    {
        return WinGold;
    }
	/// <summary>
	/// 只有pve ，怪物才会掉宝箱，所以一定是赢
	/// </summary>
	public static int GetWinItem()
	{
		return WinItem;
	}

    public static void SetAddWinGold(int Gold)
    {
		if(GetCamp2Player(LifeMCamp.ATTACK) == true)
		{
			WinGold += Gold;
		}
		else
		{
			WinGold -= Gold;
		}
    }

	public static void SetAddWinItem(int Item)
	{
		WinItem += Item;
	}


    public static int GetWinWood()
    {
        return WinWood;
    }

    public static void SetAddWinWood(int wood)
    {
		if(GetCamp2Player(LifeMCamp.ATTACK) == true)
		{
			WinWood += wood;
		}
		else
		{
			WinWood -= wood;
		}
    }



    public static void ResetFireState()
    {
		foreach (SoldierTypeInfo Info  in m_AttackList.Values)
		{
			Info.m_State = SoldierState.NoBorn;
		}
    }
	
	public static Dictionary<int ,BuildInfo> GetBuildData()
	{
		FloorM.SetFloorData(m_CanvansInfo);
		foreach(BuildInfo Info in m_DefenseBuild.Values)
		{
			Info.JoinFloor(FloorM.GetFloorData());
			Info.AddFlooHp();
		}
		return m_DefenseBuild;
	}

	public static int GetDestoryBuildCount()
	{
		int cout = 0;
		foreach(BuildInfo Info in m_DefenseBuild.Values)
		{
			if(Info.m_damage == 1 || Info.BuildType == 9999)
				cout ++;
		}
		return cout;
	}


    private static void GetWall()
    {
        WallM.GetWall();
    }

	public static FloorInfo GetFloor()
	{
		return m_Floor;
	}


    public static void GetWallList(ref List<int> list)
    {
        WallM.GetWallList(ref list);
    }

    public static int GetiAttackDeadSolid()
    {
        return m_AttackDeadSoldierNum;
    }
    public static void DeadAttackSoldier()
    {
        m_AttackDeadSoldierNum++;
		if(m_AttackDeadSoldierNum > 0 && m_AttackDeadSoldierNum == GetAttackCount())
		{
			CombatScheduler.AttackAllDead = true;
		}
    }


    public static void ResetDead()
    {
        m_AttackDeadSoldierNum = 0;
    }
	/// <summary>
	/// 获取防守方楼梯 pos 指楼梯下节点数据
	/// </summary>
	public static void GetStairInfo(ref List<StairInfo> lStair)
	{
		if(lStair == null)
			lStair = new List<StairInfo>();
		lStair.Clear();

		foreach (BuildInfo stairBuild in m_DefenseBuild.Values ) 
		{
			if(stairBuild == null )
				continue ;

			if(stairBuild.m_RoomType != RoomType.Stair)
				continue ;

			StairInfo stair = new StairInfo(new Int2(stairBuild.m_cx,stairBuild.m_cy));
			lStair .Add(stair);
		}
	}

	/// <summary>
	/// 获取战斗副本中的炮弹并信息
	/// IsAttack true 攻击方，false 防守方 
	/// id 炮弹并的id
	/// SoildierID 炮弹兵ID
	/// </summary>
	public static SoldierInfo GetSoldierInfo(LifeMCore Core)
	{
		if (Core.m_bTurn)
			return GetTrunSoldierInfo(Core.m_Camp,Core.m_DataID);
		else
			return GetSoldierInfo(Core.m_Camp,Core.m_DataID);
	}

	public static SoldierInfo GetTrunSoldierInfo(LifeMCamp Camp, int DataID)
	{
		if(Camp == LifeMCamp.ATTACK)
		{
			if(m_AttackList.ContainsKey (DataID) == true)
			{
				return m_AttackList[DataID].m_Soldier.m_TurnInfo;
			}
		}
		else
		{
			if(m_DefenseList.ContainsKey(DataID) == true)
			{
				return m_DefenseList[DataID].m_TurnInfo;
			}
		}
		return null;
	}

	public static SoldierInfo GetSoldierInfo(LifeMCamp Camp, int DataID)
	{
		if(Camp == LifeMCamp.ATTACK)
		{
			if(m_AttackList.ContainsKey (DataID) == true)
			{
				return m_AttackList[DataID].m_Soldier;
			}
		}
		else
		{
			if(m_DefenseList.ContainsKey(DataID) == true)
			{
				return m_DefenseList[DataID];
			}
		}
		return null;
	}

	public static SoldierInfo GetPlayerSoldierInfo(int DataID)
	{
		LifeMCamp Camp = GetPlayer2Camp(true);
		if(Camp == LifeMCamp.DEFENSE )
		{
			if(m_DefenseList.ContainsKey(DataID) == true)
			{
				return m_DefenseList[DataID];
			}
		}
		else 
		{
			if(m_AttackList.ContainsKey (DataID) == true)
			{
				return m_AttackList[DataID].m_Soldier;
			}
		}
		return null;
	}

	/// <summary>
	/// 获取战斗副本中的宠物信息
	/// id 宠物的id
	/// </summary>
	public static PetInfo GetPetInfo(LifeMCore Core)
	{
		if(Core == null)
			return null;
		return GetPetInfo(Core.m_DataID);
	}
	/// <summary>
	/// 获取战斗副本中的宠物信息
	/// </summary>
	public static PetInfo GetPetInfo(int ID)
	{
		return SummonM.GetPetInfo(ID);
	}
	/// <summary>
	/// 获取战斗副本中的召唤物 
	/// DataID 召唤物的id
	/// </summary>
	public static SummonpetInfo GetSummonPetInfo(LifeMCore Core)
	{
		if(Core == null)
			return null;
		return GetSummonPetInfo(Core.m_DataID);
	}
	/// <summary>
	/// 获取战斗副本中的召唤物 
	/// DataID 召唤物的id
	/// </summary>
	public static SummonpetInfo GetSummonPetInfo(int ID)
	{
		return SummonM.GetSummonPetInfo(ID);
	}
	/// <summary>
	/// 获取战斗副本中的道具
	/// DataID 道具的id
	/// </summary>
	public static SummonProsInfo GetSummonProsInfo(LifeMCore Core)
	{
		if(Core == null)
			return null;
		return GetSummonProsInfo(Core);
	}
	/// <summary>
	/// 获取战斗副本中的道具
	/// DataID 道具的id
	/// </summary>
	public static SummonProsInfo GetSummonProsInfo(int ID)
	{
		return SummonM.GetSummonProsInfo(ID);
	}

	/// <summary>
	/// 获取防守方地图画板数据
	/// </summary>
	public static ShipCanvasInfo GetDefenseMap()
	{
		return m_CanvansInfo;
	}

	public static BuildInfo GetBuildInfo(int id)
	{
		if(m_DefenseBuild == null )
			return null;
		if(m_DefenseBuild.ContainsKey(id))
		{
			return m_DefenseBuild[id];
		}
		return null;
	}

	public static int GetDataID ()
	{
		return g_DataID ++;
	}
	
	public static void ResetDataID()
	{
		g_DataID = 0;
	}

	/// <summary>
	/// 添加船长技能 ，Ispalyer ： true 玩家自己 flase 非玩家
	/// </summary>
	public static void AddGodSkill(bool IsPlayer,int CaptainID, int mana)
	{
		LifeMCamp Camp = GetPlayer2Camp(IsPlayer);
		if(m_CaptainSkill == null)
			m_CaptainSkill = new Dictionary<bool, CaptainSkillInfo>();
		if(m_CaptainSkill.ContainsKey (IsPlayer ) == true)
		{
			m_CaptainSkill.Remove(IsPlayer);
		}
		m_CaptainSkill.Add(IsPlayer, new CaptainSkillInfo (CaptainID ,Camp,mana));
	}
	
	/// <summary>
	/// 添加船长技能点数 Ispalyer ： true 玩家自己 flase 非玩家
	/// </summary>
	public static void AddGodSkillMana(bool IsPlayer ,int mana)
	{
		if(m_CaptainSkill.ContainsKey (IsPlayer ) == true)
		{
			m_CaptainSkill[IsPlayer].m_Mana += mana;
		}
	}
	/// <summary>
	/// 释放船长技能点数 Ispalyer ： true 玩家自己 flase 非玩家
	/// </summary>
	public static void SubGodSkillMana(bool IsPlayer ,int mana)
	{
		if(m_CaptainSkill.ContainsKey (IsPlayer ) == true)
		{
			m_CaptainSkill[IsPlayer].m_Mana -= mana;
		}
	}

	/// <summary>
	/// 获取船长技能
	/// </summary>
	public static GodSkill GetGodSkill(bool IsPlayer )
	{
		if(m_CaptainSkill.ContainsKey (IsPlayer ) == true)
		{
			return m_CaptainSkill[IsPlayer].m_Skill ;
		}
		return null;
	}

	/// <summary>
	/// 获取技能点数
	/// </summary>
	public static int GetGodSkillMana(bool IsPlayer )
	{
		if(m_CaptainSkill.ContainsKey (IsPlayer ) == true)
		{
			return m_CaptainSkill[IsPlayer].m_Mana ;
		}
		return 0;
	}


	/// <summary>
	/// 获取船长ID
	/// </summary>
	public static int GetCaptainID(bool IsPlayer )
	{
		if(m_CaptainSkill.ContainsKey (IsPlayer ) == true)
		{
			return m_CaptainSkill[IsPlayer].m_CaptainID ;
		}
		return -1;
	}

	/// <summary>
	/// 准备战斗
	/// </summary>
	public static void ReadyCombat()
	{
		WinGold = 0;
		WinWood = 0;
		WinItem = 0;
		ResetDataID();
		g_CurrentDataID = 0;
		ResetDead();
		m_CaptainSkill.Clear();
		m_DiePlayersoldier.Clear();
		m_DieNotPlayersoldier.Clear();
		m_DieBuild.Clear();
		m_StartTalk.Clear();
		m_EndTalk.Clear();
		m_StartTalkOver = true;
		m_EndTalkOver = true;
		BSC.AntiAllRegisterHooks();
	}

	/// <summary>
	/// 添加死亡的玩家炮弹信息
	/// </summary>
	public static void AddDiePlayerSoldier(LifeMCore Core,Int2 Pos)
	{
		if(Core == null )//|| Core.m_IsPlayer == false)
			return ;
		if(Core.m_type != LifeMType.SOLDIER) return ;

		SoldierInfo Info = GetSoldierInfo(Core);
		if(Info == null) return ;

		if(Core.m_IsPlayer && m_DiePlayersoldier.ContainsKey (Info.ID) == false)
		{
			m_DiePlayersoldier.Add(Info.ID ,Pos);
		}
		
		if(!Core.m_IsPlayer && m_DieNotPlayersoldier.ContainsKey (Info.ID) == false)
		{
			m_DieNotPlayersoldier.Add(Info.ID ,Pos);
		}
	}
	
	/// <summary>
	/// 添加死亡的玩家炮弹信息
	/// </summary>
	public static void AddDieBuild(LifeMCore Core)
	{
		if(Core.m_type != LifeMType.BUILD) return ;

		BuildInfo Info = CmCarbon.GetBuildInfo(Core.m_DataID);

		if(Info == null) return ;

		if(m_DieBuild.Contains (Info.ID) == false)
		{
			m_DieBuild.Add(Info.ID );
		}
	}


	/// <summary>
	/// 获取生存的玩家炮弹兵
	/// </summary>
	public static List<int> GetDiePlayerSoldier()
	{
		List<int> l = new List<int>();
		l.AddRange(m_DiePlayersoldier.Keys);
		return l;
	}


	/// <summary>
	/// 获取生存的玩家炮弹兵
	/// </summary>
	public static Dictionary<int,Int2> GetDiePlayerSoldierInfo()
	{
		return m_DiePlayersoldier;
	}
	
	public static Dictionary<int,Int2> GetDieNotPlayerSoldierInfo()
	{
		return m_DieNotPlayersoldier;
	}
	/// <summary>
	/// 获取被摧毁的建筑物。
	/// </summary>
	public static List<int> GetDestroyBuild()
	{
		return m_DieBuild;
	}

	/// <summary>
	/// 设置物品掉落奖励
	/// </summary>
	public static void SetRewardItew()
	{
		if(BattleEnvironmentM.GetBattleEnvironmentMode () != BattleEnvironmentMode.CombatPVE)
			return ;
		if(StageDC.GetPveMode () == PVEMode.Attack)
		{
			foreach(SoldierInfo Info in m_DefenseList.Values)
			{
				if(Info != null)
				{
					Info.SetRewardItems(StageDC.GetMonsterReward(Info.SoldierTypeID));
				}
			}
		}
		else
		{
			foreach(SoldierTypeInfo s in m_AttackList.Values)
			{
				if(s != null && s.m_Soldier != null)
				{
					s.m_Soldier.SetRewardItems(StageDC.GetMonsterReward(s.m_Soldier.SoldierTypeID));
				}
			}
		}
	}
	/// <summary>
	/// 获取掉落物品奖励
	/// </summary>
	public static List<int> GetRewardItem(LifeMCamp Camp, int DataID)
	{
		if(Camp == LifeMCamp.ATTACK)
		{
			if(m_AttackList.ContainsKey (DataID) == true)
			{
				return m_AttackList[DataID].m_Soldier.m_RewardItem;
			}
		}
		else
		{
			if(m_DefenseList.ContainsKey(DataID) == true)
			{
				return m_DefenseList[DataID].m_RewardItem;
			}
		}
		return new List<int>();
	}
	/// <summary>
	/// 进攻防守阵营转是否玩家
	/// </summary>
	public static bool GetCamp2Player(LifeMCamp Camp)
	{
		if(BattleEnvironmentM.GetBattleEnvironmentMode() == BattleEnvironmentMode.CombatPVE 
		   && StageDC.GetPveMode() == PVEMode.Defense)
		{
			if(Camp == LifeMCamp.ATTACK)
				return false;
			else return true;
		}
		else 
		{
			if(Camp == LifeMCamp.ATTACK)
				return true;
			else return false;
		}
	}

	/// <summary>
	/// 是否玩家转进攻防守阵营
	/// </summary>
	public static LifeMCamp GetPlayer2Camp(bool IsPlayer)
	{
		if(BattleEnvironmentM.GetBattleEnvironmentMode() == BattleEnvironmentMode.CombatPVE 
		   && StageDC.GetPveMode() == PVEMode.Defense)
		{
			if(IsPlayer == true)
				return LifeMCamp.DEFENSE;
			else return LifeMCamp.ATTACK;
		}
		else 
		{
			if(IsPlayer == true)
				return LifeMCamp.ATTACK;
			else return LifeMCamp.DEFENSE;
		}
	}

	public static void CreateQueuedSoilders()
	{
		foreach (int key in m_AttackList.Keys)
		{
			if (m_AttackList[key].m_State == SoldierState.NoBorn)
			{
				//m_QueueSoldierInfos.Enqueue();
				//l.Add(key);
			}
		}
	}

	/// <summary>
	/// 随机选择一个发射的炮弹兵
	/// </summary>
	/// <param name="DataID">DataID</param>
	/// <param name="Soldier">返回炮弹兵</param>
	/// <returns>false 没有可发射的炮弹兵</returns>
	public static bool GetRandomFireSoldier(ref int DataID ,ref SoldierInfo  Soldier)
	{
		DataID = -1;
		if(Soldier == null)
			Soldier = new SoldierInfo ();
		if(m_AttackList == null) return false;
		List<int> l = new List<int>();
		foreach (int key  in m_AttackList.Keys)
		{
			if(m_AttackList[key].m_State == SoldierState.NoBorn)
			{
				l.Add(key);
			}
		}
		if(l.Count > 0)
		{
			int index = Random.Range(0,l.Count);
			DataID = l[index];
			Soldier = GetSoldierInfo(LifeMCamp.ATTACK,DataID);
			return true;
		}
		return false;
	}

	/// <summary>
	/// 按顺序发射炮弹兵
	/// </summary>
	/// <param name="DataID">DataID</param>
	/// <param name="Soldier">返回炮弹兵</param>
	/// <returns>false 没有可发射的炮弹兵</returns>
	public static bool GetQueuedFireSoldier(ref int DataID)
	{
		DataID = -1;

		if (m_AttackList == null) return false;
		List<int> l = new List<int>();
		while ( g_CurrentDataID < m_AttackList.Count )
		{
			SoldierTypeInfo soldierTypeInfo;
			m_AttackList.TryGetValue(g_CurrentDataID, out soldierTypeInfo);
			if (soldierTypeInfo != null && soldierTypeInfo.m_State == SoldierState.NoBorn)
			{
				DataID = g_CurrentDataID;
				return true;
			}			
			g_CurrentDataID++;
		}
		
		return false;
	}

	/// <summary>
	/// 获取发射的炮弹兵
	/// </summary>
	/// <param name="timeElapse">经过的时间</param>
	/// <param name="watingTime">需要等待的时间，填0就是马上就要发射</param>
	/// <returns>false 没有可发射的炮弹兵</returns>
	public static List<int> GetFireSoldiers(float timeElapse, float watingTime)
	{
		if (m_AttackList == null) return null;

		List<int> soldiers = new List<int>();
		foreach (int key in m_AttackList.Keys)
		{
			SoldierTypeInfo soldier = m_AttackList[key];
			if (soldier.m_State == SoldierState.NoBorn &&  timeElapse > soldier.mFireTime - watingTime)
			{
				soldiers.Add(key);
			}
		}
		return soldiers;
	}

	public static int GetFireSoldierCount()
	{
		return m_AttackList.Count;
	}


	/// <summary>
	/// 设置战前对话
	/// </summary>
	public static void SetStartTalk(List<CounterPartDialogUnit> lStartTalk)
	{
		//CounterPartDialogUnit
		m_StartTalk.Clear();
		if(lStartTalk == null || lStartTalk.Count == 0)
		{
			m_StartTalkOver = true;
			return ;
		}
		m_StartTalkOver = false;
		m_StartTalk.AddRange(lStartTalk);
	}
	/// <summary>
	/// 设置战后对话
	/// </summary>
	public static void SetEndTalk(List<CounterPartDialogUnit> lEndTalk)
	{
		m_EndTalk.Clear();
		if(lEndTalk == null || lEndTalk.Count == 0)
		{
			m_EndTalkOver = true;
			return ;
		}
		m_EndTalkOver = false;
		m_EndTalk.AddRange(lEndTalk);
	}
	/// <summary>
	/// 获取战前对话
	/// </summary>
	public static List<CounterPartDialogUnit> GetStartTalk()
	{
		return m_StartTalk;
	}
	/// <summary>
	/// 获取战后对话
	/// </summary>
	public static List<CounterPartDialogUnit> GetEndTalk()
	{
		return m_EndTalk;
	}
}
