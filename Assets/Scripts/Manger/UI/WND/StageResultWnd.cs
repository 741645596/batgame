using UnityEngine;
using System.Collections.Generic;


public class ItemInfo
{
    public int mItemtypeid;	// s_itemtype表的id。根据sitemtypeid查询s_itemtype表的gtype值，确定did是哪个表的id。
    public int mSuperpose;	// 获得物品数量。碎片有数量，其他的没有或者为1（比如1个炮弹兵、1个船长）。
    public int mDID;
}

//炮弹兵结算信息
public class SoldierSettlement
{
    public int mSoldierid = 1;
    public int mExp = 2;
}

//关卡结算资源
public class Resouce
{
    public int mCoin;
    public int mWood;
}

public class BattleSettleResponseInfo
{
    public BattleSettleResponseInfo()
    {
        mRes = new Resouce();
        mRewards = new List<ItemInfo>();
        mSoldierSettles = new List<SoldierSettlement>();
    }
    public int mStar;
    public int mReward;
    public int mTeamExp; //战队结算信息
    public bool mWin = false;
    
    public Resouce mRes = null;
    
    public List<ItemInfo> mRewards = null;
    public List<SoldierSettlement> mSoldierSettles = null;   // 炮弹兵经验
}


/// <summary>
/// 战役结算wnd
/// </summary>
public class StageResultWnd : WndBase
{

    public bool m_IsPve = true;
    public int m_nMode = -1;
    
    private List<sdata.s_itemtypeInfo> m_lRewardInfo = new List<sdata.s_itemtypeInfo>();
    
    private TrophiesActionWnd m_Trophies;
    //    {
    //        get
    //        {
    //            return MyHead.Trophies.GetComponent<TrophiesActionWnd>();
    //        }
    //    }
    
    public StageResultWnd_h MyHead {
        get
        {
            return (base.BaseHead() as StageResultWnd_h);
        }
    }
    
    
    
    public override void BindEvents()
    {
        if (MyHead.btnclose) {
            MyHead.btnclose.OnClickEventHandler += ClickClose;
        }
        if (MyHead.btnrecombat) {
            MyHead.btnrecombat.OnClickEventHandler += ClickReCombat;
        }
        if (MyHead.RewardOK) {
            MyHead.RewardOK.OnClickEventHandler += GetNextReward;
        }
        
        MyHead.BtnGotoPdbbb21.OnClickEventHandler += BtnGotoPdbbb_OnClickEventHandler;
        MyHead.BtnGotoPdbbb31.OnClickEventHandler += BtnGotoPdbbb_OnClickEventHandler;
        MyHead.BtnGotoPdbbb41.OnClickEventHandler += BtnGotoPdbbb_OnClickEventHandler;
        MyHead.BtnGotoPdbbb42.OnClickEventHandler += BtnGotoPdbbb_OnClickEventHandler;
        MyHead.BtnGotoTrapList31.OnClickEventHandler += BtnGotoTrapList31_OnClickEventHandler;
        MyHead.BtnGotoShipEdit31.OnClickEventHandler += BtnGotoShipEdit31_OnClickEventHandler;
        
    }
    /// <summary>
    /// 战斗跳转到 船只编辑
    /// </summary>
    void BtnGotoShipEdit31_OnClickEventHandler(UIButton sender)
    {
        SceneM.Load(MainTownScene.GetSceneName(), false, null, false);
        MainTownInit.s_currentState = MainTownState.CanvasEdit;
    }
    /// <summary>
    /// 战斗跳转到 陷阱背包
    /// </summary>
    void BtnGotoTrapList31_OnClickEventHandler(UIButton sender)
    {
        SceneM.Load(MainTownScene.GetSceneName(), false, null, false);
        MainTownInit.s_currentState = MainTownState.MainMenuTrapBb;
    }
    /// <summary>
    /// 战斗跳转到 炮弹兵背包
    /// </summary>
    private void BtnGotoPdbbb_OnClickEventHandler(UIButton sender)
    {
        SceneM.Load(MainTownScene.GetSceneName(), false, null, false);
        MainTownInit.s_currentState = MainTownState.MainMenuPdbbb;
    }
    
    
    // Use this for initialization
    public override void WndStart()
    {
        base.WndStart();
        HideMoneyEffect();
        CombatScheduler.NoTimeCount = false;
        NGUIUtil.UpdatePanelValue(MyHead.PanelMask, 1024, 0.15f);
        NGUIUtil.TweenGameObjectPosX(MyHead.ScrollLeft, -461, 0.15f, gameObject, "HideAniScroll");
        NGUIUtil.TweenGameObjectPosX(MyHead.ScrollRight, 461, 0.15f);
    }
    
    /// <summary>
    /// 隐藏左右两个动画轴
    /// </summary>
    public void HideAniScroll()
    {
        NGUIUtil.SetActive(MyHead.ScrollLeft, false);
        NGUIUtil.SetActive(MyHead.ScrollRight, false);
    }
    
    
    void HideMoneyEffect()
    {
        CombatInfoWnd wnd = WndManager.FindDialog<CombatInfoWnd>();
        if (wnd) {
            wnd.HideMoneyEffect();
        }
    }
    
    /// <summary>
    /// 设置战役表现
    /// </summary>
    public void SetStageResult(stage.StageSettleResponse Info)
    {
        m_IsPve = true;
        if (Info == null) {
            return ;
        }
        if (MyHead.m_pve != null) {
            MyHead.m_pve.SetActive(true);
        }
        //需要先隐藏起来。
        SetStageWin(Info.win, Info.star);
        SetteamReward(Info.userinfo, Info.resource);
        if (Info.win) {
            SetSoldierReward(Info.soldierinfos);
        } else {
            SetSoldierReward();
        }
        SetItemReward(Info.resource);
        
        if (Info.win) {
            GetTrophies(Info.resource) ;
            if (m_Trophies != null) {
                m_Trophies.gameObject.SetActive(false);
            }
        }
        DoFailJumb();
    }
    
    public void SetSettlementResult(BattleSettleResponseInfo Info)
    {
        if (Info == null) {
            return;
        }
        if (MyHead.m_pve != null) {
            MyHead.m_pve.SetActive(true);
        }
        //需要先隐藏起来。
        SetStageWin(Info.mWin, Info.mStar);
        SetteamReward(Info.mTeamExp, Info.mRes.mCoin);
        
        if (Info.mStar > 0) {
            SetSoldierReward(Info.mSoldierSettles);
        } else {
            SetSoldierReward();
        }
        SetItemReward(Info.mRewards);
        
        if (Info.mStar > 0) {
            GetTrophies(Info.mRewards);
            if (m_Trophies != null) {
                m_Trophies.gameObject.SetActive(false);
            }
            
        }
        DoFailJumb();
    }
    
    /// <summary>
    /// 设置海神杯结算表现
    /// </summary>
    public void SetAthleticsResult(athletics.AthleticsSettleResponse athleticsSettleInfo)
    {
        BattleSettleResponseInfo settleInfo = new BattleSettleResponseInfo();
        settleInfo.mStar = athleticsSettleInfo.star;
        settleInfo.mReward = athleticsSettleInfo.reward;
        settleInfo.mRes.mCoin = athleticsSettleInfo.resource.coin;
        settleInfo.mRes.mWood = athleticsSettleInfo.resource.wood;
        settleInfo.mWin = athleticsSettleInfo.win;
        settleInfo.mSoldierSettles = new List<SoldierSettlement>();
        global::System.Collections.Generic.List<athletics.StageSoldierSettle> soldierinfos = athleticsSettleInfo.soldierinfos;
        for (int i = 0; i < soldierinfos.Count; i++) {
            athletics.StageSoldierSettle stageSoldierInfo = soldierinfos[i];
            SoldierSettlement soldierSettlement = new SoldierSettlement();
            soldierSettlement.mExp = stageSoldierInfo.exp;
            soldierSettlement.mSoldierid = stageSoldierInfo.soldierid;
            settleInfo.mSoldierSettles.Add(soldierSettlement);
        }
        m_IsPve = false;
        SetSettlementResult(settleInfo);
        DoFailJumb();
    }
    
    public void SetTreasureResult(treasure.TreasureRobSettleResponse Info)
    {
        bool win = Info.win;
        
        battle.SettleResponse pvpInfo = Info.pvpinfo;
        
        m_IsPve = false;
        if (Info == null) {
            return;
        }
        //		if(MyHead.m_pvp != null)
        //			MyHead.m_pvp.SetActive(true);
        if (MyHead.btnrecombat != null) {
            MyHead.btnrecombat.gameObject.SetActive(false);
        }
        if (win) {
            SoundPlay.Play("win", false, false);
            SetteamReward(pvpInfo);
            SetStageWin(win, pvpInfo.star);
            List<SoldierInfo> lSoldier = new List<SoldierInfo>();
            CmCarbon.GetPlayerSoldier(ref lSoldier);
            SetSoldierReward(lSoldier);
        } else {
            SoundPlay.Play("battle_lose", false, false);
            SetteamReward(pvpInfo);
            if (MyHead.PvpPveFail != null) {
                MyHead.PvpPveFail.SetActive(true);
                MyHead.fail.gameObject.SetActive(true);
                MyHead.Data.SetActive(false);
                MyHead.succ.gameObject.SetActive(false);
                MyHead.Rewardtable.transform.parent.gameObject.SetActive(false);
            }
        }
        DoFailJumb();
    }
    
    /// <summary>
    /// 战斗失败结算界面表现
    /// </summary>
    private void DoFailJumb()
    {
        if (m_IsPve) { //副本战斗
            if (CmCarbon.GetCamp2Player(LifeMCamp.ATTACK) == false) { //防御战
                NGUIUtil.SetActive(MyHead.Help2, false);
                NGUIUtil.SetActive(MyHead.Help4, false);
                NGUIUtil.SetActive(MyHead.Help3, true);
            } else {
                NGUIUtil.SetActive(MyHead.Help3, false);
                NGUIUtil.SetActive(MyHead.Help2, false);
                NGUIUtil.SetActive(MyHead.Help4, true);
            }
        } else {
            NGUIUtil.SetActive(MyHead.Help3, false);
            NGUIUtil.SetActive(MyHead.Help2, false);
            NGUIUtil.SetActive(MyHead.Help4, true);
        }
    }
    
    private void SetteamReward(battle.SettleResponse  Info)
    {
        if (Info == null) {
            return ;
        }
        if (MyHead.lv != null) {
            MyHead.lv.text = "[ffffff]LV:" + UserDC.GetLevel() + "[-]";
        }
        if (MyHead.gold != null) {
            MyHead.gold.text = "[ffffff]+" + Info.resource.coin + "[-]";
        }
        if (MyHead.Wood != null) {
            MyHead.Wood.text = "[ffffff]+" + Info.resource.wood + "[-]";
        }
        if (MyHead.Cup != null) {
            MyHead.Cup.text = "[ffffff]+" + Info.reward + "[-]";
        }
    }
    
    
    
    public void GetTrophies(stage.StageResource Res)
    {
        if (Res != null && Res.rewards != null) {
            for (int i = 0 ; i < Res.rewards.Count ; i++) {
                sdata.s_itemtypeInfo Info = ItemM.GetItemInfo(Res.rewards[i].itemtypeid);
                if (Info == null) {
                    continue ;
                }
                if (Info.gtype == 1 ||  Info.gtype == 2 || Info.gtype == 3) {
                    m_lRewardInfo.Add(Info);
                }
            }
        }
    }
    
    public void GetTrophies(List<ItemInfo> rewards)
    {
        if (rewards != null) {
            for (int i = 0; i < rewards.Count; i++) {
                sdata.s_itemtypeInfo Info = ItemM.GetItemInfo(rewards[i].mItemtypeid);
                if (Info == null) {
                    continue;
                }
                if (Info.gtype == 1 || Info.gtype == 2 || Info.gtype == 3) {
                    m_lRewardInfo.Add(Info);
                }
            }
        }
    }
    
    /// <summary>
    /// 设置战役胜负
    /// </summary>
    private void SetStageWin(bool outcome, int star)
    {
        bool boss = false;
        CounterPartInfo countInfo = StageM.GetCounterPartInfo(StageDC.GetCompaignStageID());
        if (countInfo != null) {
            boss = countInfo.isboss == 1;
        }
        if (outcome) {
            MyHead.succ.gameObject.SetActive(true);
            MyHead.fail.gameObject.SetActive(false);
            SetStageStar(star, boss);
            GatePassInfo Info = new GatePassInfo();
            Info.star = star;
            Info.times = 1;
            Info.stageid = StageDC.GetCompaignStageID();
            StageDC.UpdataStageScheduler(Info);
            
            
            
        } else {
            MyHead.succ.gameObject.SetActive(false);
            MyHead.fail.gameObject.SetActive(true);
            MyHead.PvpPveFail.SetActive(true);
            MyHead.Datatable.gameObject.SetActive(false);
            //			MyHead.PvpPveFail.transform.FindChild("PvpFailText").gameObject.SetActive(false);
            MyHead.Rewardtable.transform.parent.gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// 设置战役星级
    /// </summary>
    private void SetStageStar(int star, bool isBoss)
    {
        if (isBoss) {
            if (star <= 3 && MyHead.m_lstar.Length == 3) {
                NGUIUtil.SetStarLevelNum(MyHead.m_lstar, star, true);
            }
            
            //			MyHead.LblVictory1.SetActive(false);
            //			MyHead.LblVictory2.SetActive(false);
        } else {
            NGUIUtil.SetStarHidden(MyHead.m_lstar, 0);
        }
        
    }
    
    /// <summary>
    /// 设置战役奖励
    /// </summary>
    private void SetteamReward(stage.StageUserSettle user, stage.StageResource Res)
    {
        if (user == null || Res == null) {
            return ;
        }
        if (MyHead.lv != null) {
            MyHead.lv.text = "[ffffff]LV:" + UserDC.GetLevel() + "[-]";
        }
        if (MyHead.exp != null) {
            MyHead.exp.text = "[ffffff]+" + user.exp + "[-]";
        }
        if (MyHead.gold != null) {
            MyHead.gold.text = "[ffffff]+" + Res.coin + "[-]";
        }
    }
    
    /// <summary>
    /// 设置战役奖励
    /// </summary>
    private void SetteamReward(int exp, int coin)
    {
        if (MyHead.lv != null) {
            MyHead.lv.text = "[ffffff]LV:" + UserDC.GetLevel() + "[-]";
        }
        if (MyHead.exp != null) {
            MyHead.exp.text = "[ffffff]+" + exp + "[-]";
        }
        if (MyHead.gold != null) {
            MyHead.gold.text = "[ffffff]+" + coin + "[-]";
        }
    }
    /// <summary>
    /// 设置炮弹兵详细数据
    /// </summary>
    private void SetSoldierReward(List<stage.StageSoldierSettle> lsoldier)
    {
        if (lsoldier == null || lsoldier.Count == 0) {
            return ;
        }
        if (MyHead.Datatable == null) {
            return ;
        }
        
        for (int i = 0 ; i < lsoldier.Count ; i ++) {
            GameObject go = NDLoad.LoadWndItem("RoleRewardItem", MyHead.Datatable.transform);
            if (go != null) {
                RoleRewardItem item = go.GetComponent<RoleRewardItem>();
                if (item != null) {
                    item.SetRoleRewardItem(lsoldier[i]);
                }
            }
        }
        MyHead.Datatable.enabled = true;
        NGUIUtil.RepositionTablePivot(MyHead.Datatable.gameObject);
    }
    
    /// <summary>
    /// 设置炮弹兵详细数据
    /// </summary>
    private void SetSoldierReward(List<SoldierSettlement> soldiers)
    {
        if (soldiers == null || soldiers.Count == 0) {
            return;
        }
        if (MyHead.Datatable == null) {
            return;
        }
        
        for (int i = 0; i < soldiers.Count; i++) {
            GameObject go = NDLoad.LoadWndItem("RoleRewardItem", MyHead.Datatable.transform);
            if (go != null) {
                RoleRewardItem item = go.GetComponent<RoleRewardItem>();
                if (item != null) {
                    item.SetRoleRewardItem(soldiers[i]);
                }
            }
        }
        MyHead.Datatable.enabled = true;
        NGUIUtil.RepositionTablePivot(MyHead.Datatable.gameObject);
    }
    
    private void SetSoldierReward(List<SoldierInfo> lsoldier)
    {
        if (lsoldier == null || lsoldier.Count == 0) {
            return ;
        }
        if (MyHead.Datatable == null) {
            return ;
        }
        
        for (int i = 0 ; i < lsoldier.Count ; i ++) {
            GameObject go = NDLoad.LoadWndItem("RoleRewardItem", MyHead.Datatable.transform);
            if (go != null) {
                RoleRewardItem item = go.GetComponent<RoleRewardItem>();
                if (item != null) {
                    item.SetRoleRewardItem(lsoldier[i]);
                }
            }
        }
        MyHead.Datatable.enabled = true;
        NGUIUtil.RepositionTablePivot(MyHead.Datatable.gameObject);
    }
    
    
    
    
    /// <summary>
    /// 设置炮弹兵详细数据
    /// </summary>
    private void SetSoldierReward()
    {
        List<SoldierInfo> l = new List<SoldierInfo>();
        CmCarbon.GetPlayerSoldier(ref l);
        if (MyHead.Datatable == null) {
            return ;
        }
        
        for (int i = 0 ; i < l.Count ; i ++) {
            GameObject go = NDLoad.LoadWndItem("RoleRewardItem", MyHead.Datatable.transform);
            if (go != null) {
                RoleRewardItem item = go.GetComponent<RoleRewardItem>();
                if (item != null) {
                    item.SetRoleRewardItem(l[i]);
                }
            }
        }
        MyHead.Datatable.enabled = true;
        NGUIUtil.RepositionTablePivot(MyHead.Datatable.gameObject);
    }
    /// <summary>
    /// 添加剧情掉落物品
    /// </summary>
    public void SetScriptDropItem(int itemType, int id, int num)
    {
    
        GameObject go = NDLoad.LoadWndItem("RewardItem", MyHead.Rewardtable.transform);
        if (go != null) {
            RewardItem item = go.GetComponent<RewardItem>();
            if (item != null) {
                item.SetRewardItem(itemType, id, num);
            }
        }
        MyHead.Rewardtable.enabled = true;
        MyHead.Rewardtable.Reposition();
    }
    
    private void SetItemReward(stage.StageResource Res)
    {
        if (Res == null) {
            return ;
        }
        if (Res.rewards == null || Res.rewards.Count == 0) {
            return ;
        }
        if (MyHead.Rewardtable == null) {
            return ;
        }
        
        
        Dictionary <int, int > l = new Dictionary <int, int >();
        for (int i = 0 ; i < Res.rewards.Count ; i++) {
            sdata.s_itemtypeInfo Info = ItemM.GetItemInfo(Res.rewards[i].itemtypeid);
            if (Info == null) {
                continue ;
            }
            if (l.ContainsKey(Res.rewards[i].itemtypeid) == false) {
                l.Add(Res.rewards[i].itemtypeid, Res.rewards[i].superpose);
            } else {
                l[Res.rewards[i].itemtypeid] = l[Res.rewards[i].itemtypeid] + Res.rewards[i].superpose;
            }
        }
        //
        foreach (int key in l.Keys) {
            GameObject go = NDLoad.LoadWndItem("RewardItem", MyHead.Rewardtable.transform);
            if (go != null) {
                RewardItem item = go.GetComponent<RewardItem>();
                if (item != null) {
                    item.SetRewardItem(key, l[key]);
                }
            }
        }
        MyHead.Rewardtable.enabled = true;
        MyHead.Rewardtable.Reposition();
    }
    
    private void SetItemReward(List<ItemInfo> rewardItems)
    {
        if (rewardItems == null || rewardItems.Count == 0) {
            return;
        }
        if (MyHead.Rewardtable == null) {
            return;
        }
        
        
        Dictionary<int, int> l = new Dictionary<int, int>();
        for (int i = 0; i < rewardItems.Count; i++) {
            sdata.s_itemtypeInfo Info = ItemM.GetItemInfo(rewardItems[i].mItemtypeid);
            if (Info == null) {
                continue;
            }
            if (l.ContainsKey(rewardItems[i].mItemtypeid) == false) {
                l.Add(rewardItems[i].mItemtypeid, rewardItems[i].mSuperpose);
            } else {
                l[rewardItems[i].mItemtypeid] = l[rewardItems[i].mItemtypeid] + rewardItems[i].mSuperpose;
            }
        }
        //
        foreach (int key in l.Keys) {
            GameObject go = NDLoad.LoadWndItem("RewardItem", MyHead.Rewardtable.transform);
            if (go != null) {
                RewardItem item = go.GetComponent<RewardItem>();
                if (item != null) {
                    item.SetRewardItem(key, l[key]);
                }
            }
        }
        MyHead.Rewardtable.enabled = true;
        MyHead.Rewardtable.Reposition();
    }
    
    void ClickUpSoldier(UIButton sender)
    {
        ShowStageResult(0);
    }
    void ClickUpSkill(UIButton sender)
    {
        ShowStageResult(0);
    }
    void ClickClose(UIButton sender)
    {
        if (MyHead.m_ResultWnd != null) {
            MyHead.m_ResultWnd.SetActive(false);
        }
        m_nMode = 0;
        bool show = ShowTropAction();
        if (!show) {
            ShowStageResult(m_nMode);
        }
    }
    
    void ClickReCombat(UIButton sender)
    {
        if (MyHead.m_ResultWnd != null) {
            MyHead.m_ResultWnd.SetActive(false);
        }
        m_nMode = 1;
        bool show = ShowTropAction();
        if (!show) {
            ShowStageResult(m_nMode);
        }
    }
    
    public bool ShowTropAction()
    {
        List<StageDC.ScriptDropItem> items = StageDC.GetStageScriptDrops();
        if ((m_lRewardInfo != null && m_lRewardInfo.Count > 0) || items.Count > 0) {
            TrophiesActionWnd TropWnd = WndManager.GetDialog<TrophiesActionWnd>();
            if (TropWnd) {
                TropWnd.ClearTropiesData();
                TropWnd.AddTropiesData(m_lRewardInfo);
                //List<StageDC.ScriptDropItem> items = StageDC.GetStageScriptDrops();
                foreach (StageDC.ScriptDropItem item in items) {
                    ItemUIInterface.ItemIconInfo iconInfo = ItemUIInterface.GetIconInfo((ItemUIInterface.IconType)item.mType, item.mID, item.mIsSoul, item.mIsBook);
                    iconInfo.mCount = item.mCount;
                    TropWnd.AddTropiesData(iconInfo.mName, (int)iconInfo.mType, iconInfo.mID, iconInfo.mIsSoul, iconInfo.mIsBook, true, item.mCount);
                }
                TropWnd.SetWndType(3);
                TropWnd.GetTropies(1, ShowStageResult);
                TropWnd.MyHead.LblDes.gameObject.SetActive(false);
                TropWnd.FinalEventClikHandler += BackMainScence;
                return true;
            }
        }
        return false;
    }
    public  bool ShowCaptionUpWnd()
    {
        CombatScene combat = SceneM.GetCurIScene() as CombatScene;
        if (combat != null) {
            UserInfo old = combat.m_oldUserInfo;
            if (old.Level < UserDC.GetLevel()) {
                CaptionUpgradeWnd cuw = WndManager.GetDialog<CaptionUpgradeWnd>();
                int oldMaxPhysical = UserM.GetMaxPhysical(old.Level);
                int newMaxPhysical = UserM.GetMaxPhysical(UserDC.GetLevel());
                int oldMaxherolevel = UserM.GetUserMaxHeroLevel(old.Level);
                int newMaxherolevel = UserM.GetUserMaxHeroLevel(UserDC.GetLevel());
                cuw.SetData(old.Level, UserDC.GetLevel(),
                    StageDC.GetStageResult().win ? old.Physical - StageDC.GetCounterPartInfo().win_physical : old.Physical - StageDC.GetCounterPartInfo().lose_physical,
                    UserDC.GetPhysical(),
                    oldMaxPhysical, newMaxPhysical, oldMaxherolevel, newMaxherolevel);
                    
                cuw.MyHead.BtnBg.OnClickEventHandler += BackMainScence;
                cuw.MyHead.BtnClose.OnClickEventHandler += BackMainScence;
                
                return true;
            }
        }
        
        return false;
    }
    
    private void BackMainScence(UIButton sender)
    {
        //下一个战役
        if (m_IsPve == true) {
            if (m_nMode == 0) {
                SceneM.Load(MainTownScene.GetSceneName(), false, null, false);
                MainTownInit.s_currentState = MainTownState.StageMap;
            }
            //再来一次
            else {
                SceneM.Load(ViewStageScene.GetSceneName(), false, null, false);
            }
        }
    }
    
    public void ShowStageResult(int mode)
    {
        m_nMode = mode;
        bool show = ShowCaptionUpWnd();
        if (!show) {
            BackMainScence(null);
        }
        
        
    }
    
    
    void GetNextReward(UIButton sender)
    {
        if (m_Trophies != null) {
            m_Trophies.GetNextReward();
        }
    }
}
