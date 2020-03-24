using System.Collections.Generic;


public enum StageType {
    Normal     = 1,   //普通副本
    Hard       = 2,   //精英副本
    Team       = 3,   //团队副本
}

public enum PVEMode {

    Attack      = 0,   //进攻模式
    Defense     = 1,   //防守模式
}

/// <summary>
/// 战役数据中心
/// </summary>
///<anthor>zhulin</anthor>
public class StageDC
{

    //普通副本
    private static int m_NormalStage = 0;
    private static int m_FirstNormalStage = 0;
    private static List<GatePassInfo> m_NormalStagePass = new List<GatePassInfo>();
    //精英副本
    private static int m_HardStage = 0;
    private static int m_FirstHardStage = 0;
    private static List<GatePassInfo> m_HardStagePass = new List<GatePassInfo>();
    //团队副本
    private static int m_TeamStage = 0;
    private static int m_FirstTeamStage = 0;
    private static List<GatePassInfo> m_TeamStagePass = new List<GatePassInfo>();
    
    //当前选中的战役关卡
    private static int m_stageid = 1;
    private static StageType m_type = StageType.Normal;
    private static CounterPartInfo  m_CounterPartInfo = new CounterPartInfo();
    private static ShipCanvasInfo  m_CounterPartMap = new ShipCanvasInfo();
    private static List<SoldierInfo>  m_CounterPartPutSoldier = new List<SoldierInfo>();
    private static List<EnemyFireInfo> m_CounterPartEnemys = new List<EnemyFireInfo>();
    private static List<BuildInfo>  m_CounterPartPutBulid = new  List<BuildInfo> ();
    private static List<int> m_CounterPartReward = new List<int>();
    
    private static List<EnemyFireInfo> m_FireEnemyInfos = new List<EnemyFireInfo>();
    /// <summary>
    /// 怪物掉落物品奖励
    /// </summary>
    private static List<MonsterReward> m_lRewardItem = new List<MonsterReward>();
    /// <summary>
    /// 副本奖励领取状态
    /// </summary>
    private static Dictionary<int, int> m_lRewardStatus = new Dictionary<int, int>();
    /// <summary>
    /// 剧情掉落物品
    /// message dropinfo {
    ///	required int32 type = 1;//1-item, 2-soldier, 3-build, 4-captain 5-designPic
    ///	required int32 value = 2;//itemtype, soldiertype, buildtype, captaintype
    ///	required int32 num = 3;
    /// }
    /// </summary>
    public class ScriptDropItem
    {
        public int mType = 0;
        public int mID = 0;
        public int mCount = 0;
        public bool mIsSoul = false;
        public bool mIsBook = false;
    }
    static List<ScriptDropItem> mScriptDropItems = new List<ScriptDropItem>();
    
    // 副本扫荡奖励
    public class StageSweepReward
    {
        public class ItemInfo
        {
            public int mItemTypeID;
            public int mCount;
        }
        public List<ItemInfo> mSweepItems = new List<ItemInfo>();
        public int mCoin;
        public int mExp;
        public List<ItemInfo> mExtraRewards = new List<ItemInfo>();
    }
    static List<StageSweepReward> mStageSweepRewards = new List<StageSweepReward>();
    
    
    private static bool m_bFirstTimeStage = false;
    /// <summary>
    /// 引导使用,用于判定首次战役
    /// </summary>
    public static bool FirstTimeStage {
        get
        {
            return m_bFirstTimeStage;
        }
        set
        {
            m_bFirstTimeStage = value;
        }
    }
    
    
    static stage.StageSettleResponse m_stageResult = new stage.StageSettleResponse();
    /// <summary>
    /// 获取pve 战斗模式
    /// </summary>
    public static PVEMode GetPveMode()
    {
        if (m_CounterPartInfo.mode == 0) {
            return PVEMode.Attack;
        } else {
            return PVEMode.Defense;
        }
    }
    
    
    /// <summary>
    /// 处理事件
    /// </summary>
    public static bool ProcessData(int  CmdID, int nErrorCode, object Info)
    {
        if (nErrorCode == 0) {
            SaveData(CmdID, Info);
        }
        return true;
    }
    
    /// <summary>
    /// 存储数据，供查询
    /// </summary>
    private static bool SaveData(int  cmdID, object Info)
    {
        switch (cmdID) {
            case (int)gate.Command.CMD.CMD_702:
                RecvStageAttackResponse(Info);
                break;
            case (int)gate.Command.CMD.CMD_704:
                RecvStageSettleResponse(Info);
                break;
            case (int)gate.Command.CMD.CMD_706:
                RecvStageScheduleResponse(Info);
                break;
            case (int)gate.Command.CMD.CMD_708:
                RecvStageRewardGetResponse(Info) ;
                break;
            case (int)gate.Command.CMD.CMD_710:
                RecvStageRewardFlagResponse(Info);
                break;
            case (int)gate.Command.CMD.CMD_712:
                RespStageInfoResponse(Info);
                break;
            case (int)gate.Command.CMD.CMD_714:
                RespStageResetResponse(Info);
                break;
            case (int)gate.Command.CMD.CMD_716:
                RespStageScriptDropResponse(Info);
                break;
            case (int)gate.Command.CMD.CMD_718:
                RespStageSweepResponse(Info);
                break;
        }
        return true;
    }
    
    /// <summary>
    /// 清空数据
    /// </summary>
    public static void ClearDC()
    {
        m_NormalStage = 0;
        m_NormalStagePass.Clear();
        //精英副本
        m_HardStage = 0;
        m_HardStagePass.Clear();
        //团队副本
        m_TeamStage = 0;
        m_TeamStagePass.Clear();
        
        m_lRewardStatus.Clear();
        mScriptDropItems.Clear();
        mStageSweepRewards.Clear();
        
        m_stageid = 1;
        m_type = StageType.Normal;
    }
    
    /// <summary>
    /// 0701 获取攻击关卡
    /// </summary>
    /// <returns></returns>
    public static bool SendStageAttackRequest(int stageid, List<int> lSoldier, int captinID)
    {
        stage.StageAttackRequest Info = new stage.StageAttackRequest();
        Info.stageid = stageid;
        Info.captainid = captinID;
        Info.soldiers.AddRange(lSoldier);
        return true;
    }
    /// <summary>
    /// 0702 奖励数据
    /// </summary>
    /// <param name="Info"></param>
    /// <returns></returns>
    public static bool RecvStageAttackResponse(object obj)
    {
        if (obj == null) {
            return false;
        }
        stage.StageAttackResponse Info = obj as stage.StageAttackResponse;
        
        m_lRewardItem.Clear();
        
        for (int i = 0 ; i < Info.rewards.Count ; i ++) {
            MonsterReward reward = new MonsterReward();
            reward.monsterid = Info.rewards[i].monsterid;
            reward.m_lreward.AddRange(Info.rewards[i].drops);
            m_lRewardItem.Add(reward);
        }
        CmCarbon.SetRewardItew();
        return true;
    }
    /// <summary>
    /// 0703 发送关卡结算
    /// </summary>
    /// <returns></returns>
    public static bool SendStageSettleRequest(int stageid, List<int>lSoldier, List<int> lDestroyBuild, bool exit, bool timeout)
    {
        stage.StageSettleRequest Info = new stage.StageSettleRequest();
        Info.stageid = stageid;
        if (lSoldier != null) {
            Info.soldiers.AddRange(lSoldier);
        }
        if (lDestroyBuild != null) {
            Info.builds.AddRange(lDestroyBuild);
        }
        Info.active_exit = exit;
        Info.timeout = timeout;
        return true;
    }
    /// <summary>
    /// 0704 回应战斗结果数据
    /// </summary>
    /// <param name="Info"></param>
    /// <returns></returns>
    public static bool RecvStageSettleResponse(object obj)
    {
        if (obj == null) {
            return false;
        }
        stage.StageSettleResponse Info = obj as stage.StageSettleResponse;
        
        
        m_stageResult = Info;
        return true;
    }
    /// <summary>
    /// 0705 发送关卡进度
    /// </summary>
    /// <returns></returns>
    public static bool SendStageScheduleRequest()
    {
        stage.StageScheduleRequest Info = new stage.StageScheduleRequest();
        
        return true;
    }
    /// <summary>
    /// 0706 回应炮弹兵数据
    /// </summary>
    /// <param name="Info"></param>
    /// <returns></returns>
    public static bool RecvStageScheduleResponse(object obj)
    {
        ClearDC();
        if (obj == null) {
            return false;
        }
        stage.StageScheduleResponse Info = obj as stage.StageScheduleResponse;
        //普通副本
        m_NormalStage = Info.normal;
        foreach (stage.StageScheduleResponse.ScheduleInfo I in  Info.normal_info) {
            GatePassInfo g = new GatePassInfo();
            g.star = I.star;
            g.times = I.times;
            g.stageid = I.stageid;
            m_NormalStagePass.Add(g);
        }
        //精英副本
        m_HardStage = Info.hard;
        foreach (stage.StageScheduleResponse.ScheduleInfo I in  Info.hard_info) {
            GatePassInfo g = new GatePassInfo();
            g.star = I.star;
            g.times = I.times;
            g.stageid = I.stageid;
            m_HardStagePass.Add(g);
        }
        //团队副本
        //精英副本
        m_TeamStage = Info.team;
        foreach (stage.StageScheduleResponse.ScheduleInfo I in  Info.team_info) {
            GatePassInfo g = new GatePassInfo();
            g.star = I.star;
            g.times = I.times;
            g.stageid = I.stageid;
            m_TeamStagePass.Add(g);
        }
        
        m_FirstNormalStage = StageM.FindFirstCanAttackStage(StageType.Normal, m_NormalStage);
        m_FirstHardStage = StageM.FindFirstCanAttackStage(StageType.Hard, m_HardStage);
        m_FirstTeamStage = StageM.FindFirstCanAttackStage(StageType.Team, m_TeamStage);
        
        return true;
    }
    
    
    
    /// <summary>
    /// 0707 发送关卡星级奖励
    /// </summary>
    /// <returns></returns>
    public static bool SendStageRewardGetRequest(int StarRewardID)
    {
        stage.StageRewardGetRequest Info = new stage.StageRewardGetRequest();
        Info.stage_reward_id = StarRewardID ;
        
        return true;
    }
    
    
    /// <summary>
    /// 0708 发送关卡星级回应
    /// </summary>
    /// <returns></returns>
    public static bool RecvStageRewardGetResponse(object obj)
    {
        if (obj == null) {
            return false;
        }
        stage.StageRewardGetResponse  Info = obj as stage.StageRewardGetResponse;
        return true;
    }
    
    /// <summary>
    /// 0709 发送关卡查询奖励状态
    /// </summary>
    /// <returns></returns>
    public static bool SendStageRewardFlagRequest(List<int> lStarRewardID)
    {
        if (lStarRewardID == null || lStarRewardID.Count == 0) {
            return false;
        }
        stage.StageRewardFlagRequest Info = new stage.StageRewardFlagRequest();
        Info.stage_reward_ids.AddRange(lStarRewardID);
        return true;
    }
    
    
    /// <summary>
    /// 0710 关卡查询奖励状态
    /// </summary>
    /// <returns></returns>
    public static bool RecvStageRewardFlagResponse(object obj)
    {
        if (obj == null) {
            return false;
        }
        stage.StageRewardFlagResponse  Info = obj as stage.StageRewardFlagResponse;
        
        
        foreach (stage.StageRewardFlagResponse.StageRewardFlag Item in Info.flags) {
            if (m_lRewardStatus.ContainsKey(Item.stage_reward_id) == true) {
                m_lRewardStatus[Item.stage_reward_id] = Item.flag ;
            } else {
                m_lRewardStatus.Add(Item.stage_reward_id, Item.flag);
            }
        }
        
        return true;
    }
    /// <summary>
    /// 0711 关卡信息
    /// </summary>
    /// <returns></returns>
    public static bool SendStageStageInfoRequest()
    {
        stage.StageInfoRequest Info = new stage.StageInfoRequest();
        return true;
    }
    /// <summary>
    /// 0712 关卡信息
    /// </summary>
    /// <returns></returns>
    public static bool RespStageInfoResponse(object obj)
    {
        if (obj == null) {
            return false;
        }
        stage.StageInfoResponse  Info = obj as stage.StageInfoResponse;
        return true;
    }
    /// <summary>
    /// 0713 关卡信息
    /// </summary>
    /// <returns></returns>
    public static bool SendStageResetRequest(int stageID)
    {
        stage.StageResetRequest Info = new stage.StageResetRequest();
        Info.stageid = stageID;
        return true;
    }
    /// <summary>
    /// 0714 关卡信息
    /// </summary>
    /// <returns></returns>
    public static bool RespStageResetResponse(object obj)
    {
        if (obj == null) {
            return false;
        }
        return true;
    }
    
    /// <summary>
    /// 0715 领取剧情对白掉落
    /// </summary>
    /// <returns></returns>
    public static bool SendStageScriptDropRequest(int stageID)
    {
        stage.StageGetScriptDropRequest Info = new stage.StageGetScriptDropRequest();
        Info.stageid = stageID;
        return true;
    }
    /// <summary>
    /// 0716 领取剧情对白掉落
    /// </summary>
    /// <returns></returns>
    public static bool RespStageScriptDropResponse(object obj)
    {
        if (obj == null) {
            return false;
        }
        mScriptDropItems.Clear();
        stage.StageGetScriptDropResponse Info = obj as stage.StageGetScriptDropResponse;
        
        foreach (stage.StageGetScriptDropResponse.dropinfo itemInfos in Info.infos) {
            ScriptDropItem item = new ScriptDropItem();
            item.mType = itemInfos.type;
            item.mID = itemInfos.value;
            item.mCount = itemInfos.num;
            item.mIsSoul = itemInfos.issoul;
            item.mIsBook = itemInfos.isbook;
            mScriptDropItems.Add(item);
        }
        
        return true;
    }
    
    /// <summary>
    /// 0717 发送扫荡次数
    /// </summary>
    /// <returns></returns>
    public static bool SendStageSweepRequest(int stageID, int sweepTimes)
    {
        stage.StageSweepRequest Info = new stage.StageSweepRequest();
        Info.stageid = stageID;
        Info.times = sweepTimes;
        return true;
    }
    /// <summary>
    /// 0718 领取副本扫荡奖励
    /// </summary>
    /// <returns></returns>
    public static bool RespStageSweepResponse(object obj)
    {
        if (obj == null) {
            return false;
        }
        mStageSweepRewards.Clear();
        stage.StageSweepResponse sweepResponse = obj as stage.StageSweepResponse;
        
        foreach (stage.StageSweepResponse.StageSweepReward sweepReward in sweepResponse.sweep_reward) {
            StageSweepReward stageSweepReward = new StageSweepReward();
            stage.StageResource stageResource = sweepReward.resource;
            foreach (stage.StageResource.ItemInfo itemInfo in stageResource.rewards) {
                StageSweepReward.ItemInfo item = new StageSweepReward.ItemInfo();
                item.mItemTypeID = itemInfo.itemtypeid;
                item.mCount = itemInfo.superpose;
                stageSweepReward.mSweepItems.Add(item);
            }
            foreach (stage.StageSoldierSettle soldierinfo in sweepReward.soldierinfos) {
                stageSweepReward.mExp += soldierinfo.exp;
            }
            stageSweepReward.mCoin = stageResource.coin;
            
            foreach (stage.StageSweepResponse.StageSweepReward.SweepExtraReward sweepExtraReward in sweepReward.extra_reward) {
                StageSweepReward.ItemInfo item = new StageSweepReward.ItemInfo();
                item.mItemTypeID = sweepExtraReward.itemid;
                item.mCount = sweepExtraReward.number;
                stageSweepReward.mExtraRewards.Add(item);
            }
            mStageSweepRewards.Add(stageSweepReward);
        }
        
        return true;
    }
    
    
    public static List<StageSweepReward> GetStageSweepRewards()
    {
        return mStageSweepRewards;
    }
    
    public static List<ScriptDropItem> GetStageScriptDrops()
    {
        return mScriptDropItems;
    }
    
    /// <summary>
    ///  确认奖励是否已经领取励状态
    /// </summary>
    /// <returns></returns>
    public static int GetStarRewardState(int RewardID)
    {
        if (m_lRewardStatus == null || m_lRewardStatus.Count == 0) {
            return 2;
        }
        
        if (m_lRewardStatus.ContainsKey(RewardID) == false) {
            return 2;
        }
        
        return m_lRewardStatus[RewardID];
    }
    
    
    
    /// <summary>
    /// 更新章节进度
    /// </summary>
    public static void UpdataStageScheduler(GatePassInfo Info)
    {
        if (Info == null) {
            return ;
        }
        
        int type = Info.CouterType ;
        if (type == (int)StageType.Normal) {
            if (Info.stageid > m_NormalStage) {
                m_NormalStage = Info.stageid;
                m_NormalStagePass.Add(Info);
                m_FirstNormalStage = StageM.FindFirstCanAttackStage(StageType.Normal, m_NormalStage);
            } else {
                bool IsHave = false;
                for (int i = 0 ; i < m_NormalStagePass.Count; i++) {
                    if (m_NormalStagePass[i].stageid == Info.stageid) {
                        if (m_NormalStagePass[i].star < Info.star) {
                            m_NormalStagePass[i].star = Info.star;
                        }
                        m_NormalStagePass[i].times ++ ;
                        IsHave = true;
                        break;
                    }
                }
                if (IsHave == false) {
                    Info.times = 1;
                    m_NormalStagePass.Add(Info);
                }
            }
            
        } else if (type == (int)StageType.Hard) {
            if (Info.stageid > m_HardStage) {
                m_HardStage = Info.stageid;
                m_HardStagePass.Add(Info);
                m_FirstHardStage = StageM.FindFirstCanAttackStage(StageType.Hard, m_HardStage);
            } else {
                bool IsHave = false;
                for (int i = 0 ; i < m_HardStagePass.Count; i++) {
                    if (m_HardStagePass[i].stageid == Info.stageid) {
                        if (m_HardStagePass[i].star < Info.star) {
                            m_HardStagePass[i].star = Info.star;
                        }
                        m_HardStagePass[i].times ++ ;
                        IsHave = true;
                        break;
                    }
                }
                if (IsHave == false) {
                    Info.times = 1;
                    m_HardStagePass.Add(Info);
                }
            }
        } else {
            if (Info.stageid > m_TeamStage) {
                m_TeamStage = Info.stageid;
                m_TeamStagePass.Add(Info);
                m_FirstTeamStage = StageM.FindFirstCanAttackStage(StageType.Team, m_TeamStage);
            } else {
                bool IsHave = false;
                for (int i = 0 ; i < m_TeamStagePass.Count; i++) {
                    if (m_TeamStagePass[i].stageid == Info.stageid) {
                        if (m_TeamStagePass[i].star < Info.star) {
                            m_TeamStagePass[i].star = Info.star;
                        }
                        m_TeamStagePass[i].times ++ ;
                        IsHave = true;
                        break;
                    }
                }
                if (IsHave == false) {
                    Info.times = 1;
                    m_TeamStagePass.Add(Info);
                }
            }
        }
        
    }
    /// <summary>
    /// 获取当前关卡进度.
    /// </summary>
    /// <returns>The now stage node.</returns>
    /// <param name="Type">Type.</param>
    public static int GetNowStageNode(StageType Type)
    {
        if (Type == StageType.Normal) {
            return (m_FirstNormalStage % 100000) % 1000 ;
        }
        
        else if (Type == StageType.Hard) {
            return (m_FirstHardStage % 100000) % 1000 ;
        } else {
            return (m_FirstTeamStage % 100000) % 1000 ;
        }
    }
    
    public static int GetStageNode(int ChapterID)
    {
        return (ChapterID % 100000) % 1000 ;
    }
    
    public static int GetStageChapter(int ChapterID)
    {
        return (ChapterID % 100000) / 1000 ;
    }
    
    /// <summary>
    /// 获取章节
    /// </summary>
    public static int GetStageChapter(StageType Type)
    {
        if (Type == StageType.Normal) {
            return (m_FirstNormalStage % 100000) / 1000 ;
        }
        
        else if (Type == StageType.Hard) {
            return (m_FirstHardStage % 100000) / 1000 ;
        } else {
            return (m_FirstTeamStage % 100000) / 1000 ;
        }
    }
    /// <summary>
    /// 确定该关卡是否已经开启了
    /// </summary>
    public static bool  CheckOpenStage(StageType Type, int stage)
    {
        int maxstage = 0;
        if (Type == StageType.Normal) {
            maxstage = m_FirstNormalStage;
        } else if (Type == StageType.Hard) {
            maxstage = m_FirstHardStage;
        } else {
            maxstage = m_FirstTeamStage;
        }
        
        if (stage > maxstage) {
            return false;
        }
        return true;
    }
    
    /// <summary>
    /// 获取攻击关卡的星级
    /// </summary>
    public static int GetPassStageStar(StageType Type, int stage)
    {
        if (Type == StageType.Normal) {
            for (int i = 0; i < m_NormalStagePass.Count ; i++) {
                if (m_NormalStagePass[i].stageid == stage) {
                    return m_NormalStagePass[i].star ;
                }
            }
        } else if (Type == StageType.Hard) {
            for (int i = 0; i < m_HardStagePass.Count ; i++) {
                if (m_HardStagePass[i].stageid == stage) {
                    return m_HardStagePass[i].star ;
                }
            }
        } else {
            for (int i = 0; i < m_TeamStagePass.Count ; i++) {
                if (m_TeamStagePass[i].stageid == stage) {
                    return m_TeamStagePass[i].star ;
                }
            }
        }
        return 0;
    }
    
    /// <summary>
    /// 获取攻击关卡的星级
    /// </summary>
    public static int GetPassStageTimes(StageType Type, int stage)
    {
        if (Type == StageType.Normal) {
            for (int i = 0; i < m_NormalStagePass.Count ; i++) {
                if (m_NormalStagePass[i].stageid == stage) {
                    return m_NormalStagePass[i].times ;
                }
            }
        } else if (Type == StageType.Hard) {
            for (int i = 0; i < m_HardStagePass.Count ; i++) {
                if (m_HardStagePass[i].stageid == stage) {
                    return m_HardStagePass[i].times ;
                }
            }
        } else {
            for (int i = 0; i < m_TeamStagePass.Count ; i++) {
                if (m_TeamStagePass[i].stageid == stage) {
                    return m_TeamStagePass[i].times ;
                }
            }
        }
        return 0;
    }
    
    /// <summary>
    /// 获取关卡名称
    /// </summary>
    public static string GetChaptersName(int Chapters, StageType type)
    {
        return StageM.GetChaptersName(Chapters, type);
    }
    /// <summary>
    /// 获取该章节下的所有关卡
    /// </summary>
    public static List<CounterPartInfo> GetChaptersGate(int Chapters, StageType type)
    {
        List<CounterPartInfo> l = StageM.GetChaptersGate(Chapters, type);
        if (l != null && l.Count > 1) {
            l.Sort((a, b) => {
                if (a.CounterNode > b.CounterNode) {
                    return 1;
                } else if (a.CounterNode == b.CounterNode) {
                    return 0;
                } else {
                    return -1;
                }
            });
        }
        return l;
    }
    /// <summary>
    /// 设置当前战斗关卡
    /// </summary>
    public static void SetCompaignStage(StageType type, int stageid)
    {
        m_stageid = stageid;
        m_type = type ;
    }
    /// <summary>
    /// 加入战役战斗，填充战役相关数据。
    /// </summary>
    public static bool JoinCompaignBattle()
    {
        m_CounterPartPutSoldier.Clear();
        m_CounterPartEnemys.Clear();
        m_CounterPartPutBulid.Clear();
        m_CounterPartInfo = StageM.GetCounterPartInfo(m_stageid);
        if (m_CounterPartInfo != null) {
            SetStageReward(m_stageid);
            //战斗模式
            if (m_CounterPartInfo.mode == 0) {
                m_CounterPartMap = StageM.GetCounterPartMap(m_CounterPartInfo.countershipcanvasid);
                StageM.GetCounterPartShipPut(m_stageid, ref m_CounterPartPutSoldier, ref m_CounterPartPutBulid);
                //加入战斗
                CmCarbon.ReadyCombat();
                CmCarbon.SetDefenseMap(m_CounterPartMap);
                CmCarbon.SetDefenseBuild(m_CounterPartPutBulid);
                CmCarbon.SetPVEMonisterSoldier(m_CounterPartPutSoldier);
                CmCarbon.SetDefenseFloor(m_CounterPartInfo.decklevel);
                CmCarbon.SetDefenseUserInfo(m_CounterPartInfo, m_CounterPartPutBulid);
            } else { //防守模式
                //StageM.GetCounterPartShipPut(m_stageid , ref m_CounterPartPutSoldier , ref m_CounterPartPutBulid);
                StageM.GetCounterPartShipPut(m_stageid, ref m_CounterPartEnemys, ref m_CounterPartPutBulid);
                //加入战斗
                CmCarbon.ReadyCombat();
                CmCarbon.SetAttackSoldier(m_CounterPartEnemys);
                
            }
            
            if (GetPassStageStar(m_type, m_stageid) == 0 /*&& GetPassStageTimes (m_type ,m_stageid) == 0*/) {
                CmCarbon.SetStartTalk(m_CounterPartInfo.m_StageStartTalk);
                CmCarbon.SetEndTalk(m_CounterPartInfo.m_StageEndTalk);
            }
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 获取当前选择战斗关卡
    /// </summary>
    public static int GetCompaignStageID()
    {
        return m_stageid ;
    }
    
    /// <summary>
    /// 获取当前选择战斗关卡类型
    /// </summary>
    public static StageType GetCompaignStageType()
    {
        return m_type ;
    }
    /// <summary>
    /// 获取当前副本信息
    /// </summary>
    public static CounterPartInfo GetCounterPartInfo()
    {
        return m_CounterPartInfo;
    }
    
    /// <summary>
    /// 获取战役结果
    /// </summary>
    public static stage.StageSettleResponse GetStageResult()
    {
        m_stageResult = new stage.StageSettleResponse();
        m_stageResult.win = true;
        m_stageResult.resource = new stage.StageResource();
        foreach (int itemid in m_CounterPartReward) {
            stage.StageResource.ItemInfo  v =  new stage.StageResource.ItemInfo { };
            v.itemtypeid = itemid;
            m_stageResult.resource.rewards.Add(v);
        }
        return m_stageResult;
    }
    /// <summary>
    /// 获取副本奖励
    /// </summary>
    public static List<int> GetStageReward()
    {
        return m_CounterPartReward;
    }
    
    /// <summary>
    /// 获取一个关卡的奖励
    /// </summary>
    private static void SetStageReward(int StageID)
    {
        m_CounterPartReward.Clear();
        List<int> l = StageM.GetStageReward(StageID);
        
        for (int i = 0; i < l.Count ; i++) {
            if (m_CounterPartReward.Contains(l[i]) == false) {
                m_CounterPartReward.Add(l[i]);
            }
        }
    }
    
    /// <summary>
    /// 获取怪物掉落物品奖励
    /// </summary>
    public static List<int> GetMonsterReward(int monsterid)
    {
        List<int> l = new List<int>();
        if (m_lRewardItem == null) {
            return l;
        }
        
        for (int i = 0; i < m_lRewardItem.Count ; i ++) {
            if (m_lRewardItem [i] != null && m_lRewardItem [i].monsterid == monsterid) {
                l.AddRange(m_lRewardItem [i].m_lreward);
                m_lRewardItem.RemoveAt(i);
                break;
            }
        }
        return l;
    }
    
    /// <summary>
    /// 模拟数据
    /// </summary>
    public static void SimulationData()
    {
        m_NormalStage = 0;
        m_FirstNormalStage = 101001;
        //精英副本
        m_HardStage = 0;
        m_FirstHardStage = 101001;
        //团队副本
        m_TeamStage = 0;
        m_FirstTeamStage = 0;
    }
}
/// <summary>
/// 怪物掉落资源
/// </summary>
public class MonsterReward
{
    public int monsterid ; //怪物id
    public List<int> m_lreward = new List<int>(); //怪物掉落物品
}
