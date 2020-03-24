using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

/// <summary>
/// 查看战役攻船UI
/// </summary>
public class ViewStageWnd :  WndBase
{
    public ViewStageWnd_h MyHead {
        get
        {
            return (base.BaseHead() as ViewStageWnd_h);
        }
    }
    
    private int m_stageid = 0;
    private StageType m_type;
    private int m_AvailableMultipleSweep = 0;
    private int m_ClickSweepTimes = 0;
    /// <summary>
    /// 适配完成标识
    /// </summary>
    private bool m_bAnchorFinish = false;
    
    public override void BindEvents()
    {
        if (MyHead.btnclose) {
            MyHead.btnclose.OnClickEventHandler += ClickClose;
        }
        if (MyHead.btnCombat) {
            MyHead.btnCombat.OnClickEventHandler += ClickCombat;
        }
        if (MyHead.BtnSweepMultiple) {
            MyHead.BtnSweepMultiple.OnClickEventHandler += ClickSweepMultiple;
        }
        if (MyHead.BtnSweepOnce) {
            MyHead.BtnSweepOnce.OnClickEventHandler += ClickSweepOnce;
        }
    }
    
    // Use this for initialization
    public override void WndStart()
    {
        base.WndStart();
        MainCameraM.s_Instance.EnableDrag(false);
        m_bAnchorFinish = true;
        DoStartTalks(CmCarbon.GetStartTalk());
        
    }
    
    public void SetStageid(StageType Type, int stageid)
    {
        m_stageid = stageid;
        m_type = Type;
        
        CounterPartInfo info = StageDC.GetCounterPartInfo();
        StartCoroutine(DoBubble(info.m_lBubblePromt, 0.8f));
        UpdateStageInfo();
        ShowDropInfo();
        ShowTitleByStageType();
        UpdateSweepInfo();
        
        if (StageDC.GetPveMode() == PVEMode.Defense) {
            ShipPlanDC.SetCurShipPlan(PlanType.Default);
            ShipPlan Plan = ShipPlanDC.GetCurShipPlan();
            if (MyHead.RoleText != null) {
                MyHead.RoleText.text  = "[ffff00]" + NGUIUtil.GetStringByKey("88800094") + "[-]";
            }
            if (MyHead.PlanText != null)
                if (Plan != null) {
                    MyHead.PlanText.text  = "[ffff00]" + NGUIUtil.GetStringByKey("88800095") + "[-][ffffff]" + Plan.Name + "[-]";
                } else {
                    MyHead.PlanText.text  = "[ffff00]" + NGUIUtil.GetStringByKey("88800095") + "[-]";
                }
            SetStageSoldier();
            MyHead.DefenseBg.gameObject.SetActive(true);
        } else {
            if (MyHead.Role != null) {
                MyHead.Role.SetActive(false);
            }
            if (MyHead.Plan != null) {
                MyHead.Plan.SetActive(false);
            }
        }
    }
    
    private void UpdateSweepInfo()
    {
        CounterPartInfo info = StageDC.GetCounterPartInfo();
        int times = StageDC.GetPassStageTimes(m_type, m_stageid);
        bool passed = times > 0;
        bool hasBoss = info.isboss > 0;
        bool isPVE = StageDC.GetPveMode() == PVEMode.Attack;
        // 是否需要显示扫荡UI
        bool activeSweepInfo = passed && hasBoss && isPVE;
        MyHead.SweepGroup.SetActive(activeSweepInfo);
        if (!activeSweepInfo) {
            return;
        }
        
        ItemTypeInfo sweepTicketInfo = ItemDC.GetSweepTickets();
        int sweepTicketCount = sweepTicketInfo == null ? 0 : sweepTicketInfo.Num;
        MyHead.LblSweepTicket.text = string.Format("x{0}", sweepTicketCount);
        
        int sweepOnce = 1;
        int currentSweepMultiple = 0;
        if (info.type == (int)StageType.Normal) {
            currentSweepMultiple = 10;
        } else if (info.type == (int)StageType.Hard) {
            currentSweepMultiple = 3;
        }
        MyHead.LblSweepMultiple.text = string.Format(NGUIUtil.GetStringByKey(70000255), currentSweepMultiple);
        MyHead.LblSweepOnce.text = string.Format(NGUIUtil.GetStringByKey(70000256), sweepOnce);
        
    }
    
    private void ShowDropInfo()
    {
        SetStageItemReward();
        AddScriptDropItems();
    }
    
    private void ShowTitleByStageType()
    {
        if (m_type == StageType.Hard) {
            if (MyHead.TitleSprite != null) {
                MyHead.TitleSprite.spriteName = "bg108";
            }
        }
    }
    
    // 添加剧情掉落物品
    private void AddScriptDropItems()
    {
        CounterPartInfo info = StageDC.GetCounterPartInfo();
        int Times = StageDC.GetPassStageTimes(m_type, m_stageid);
        if (Times == 0 && info.drop != null) {
            JsonData jd = JsonMapper.ToObject(info.drop);
            if (jd.Keys.Contains("item")) {
                int count = jd["item"].Count;
                
                for (int i = 0; i < count; i++) {
                    if (jd["item"][i].Keys.Contains("id")) {
                        int id = (int)jd["item"][i]["id"];
                        SetStageItemReward(0, id, 0);
                    }
                }
            }
            if (jd.Keys.Contains("captain")) {
                int count = jd["captain"].Count;
                for (int i = 0; i < count; i++) {
                    int id = (int)jd["captain"][i];
                    SetStageItemReward(4, id, 0);
                }
            }
            if (jd.Keys.Contains("soldier")) {
                int count = jd["soldier"].Count;
                for (int i = 0; i < count; i++) {
                    int id = (int)jd["soldier"][i];
                    SetStageItemReward(1, id, 0);
                }
            }
        }
    }
    /// <summary>
    /// 战前NPC对话
    /// </summary>
    private void DoStartTalks(List<CounterPartDialogUnit> ltalk)
    {
        if (ltalk.Count == 0) {
            return;
        }
        MyHead.btnCombat.enabled = false;
        List<int> lNpcID = new List<int>();
        List<string> lStrTalks = new List<string>();
        List<NpcDirection> lWndDirs = new List<NpcDirection>();
        foreach (CounterPartDialogUnit t in ltalk) {
            lNpcID.Add(t.npcid);
            lStrTalks.Add(t.talk);
            lWndDirs.Add((NpcDirection)t.position);
        }
        NpcTalksWnd wnd = WndManager.GetDialog<NpcTalksWnd>();
        wnd.SetData(lNpcID, lStrTalks, lWndDirs);
        ViewStageScene scene = SceneM.GetCurIScene() as ViewStageScene;
        if (scene != null && scene.GoldTransform != null) {
            Transform tGoldRoom = scene.GoldTransform;
            LifeObj obj = tGoldRoom.GetComponent<LifeObj>();
            if (obj != null) {
                Vector3 pos = obj.GetLife().GetLifeProp().HelpPoint.GetVauleByKey(BuildHelpPointName.guidePos).transform.position;
                wnd.SetWndStyle(2, pos);
            }
        }
    }
    /// <summary>
    /// 预览界面泡泡提示(需镜头停止移动后显示)
    /// </summary>
    private IEnumerator DoBubble(List<CounterBubblePromtInfo> lInfo, float delay)
    {
        //NGUIUtil.DebugLog("泡泡有几个："+lInfo.Count);
        yield return new WaitForSeconds(delay);
        int index = 0;
        foreach (CounterBubblePromtInfo info in lInfo) {
            if (info != null) {
                GameObject go = NDLoad.LoadWndItem("ViewStageBubble", transform);
                if (go) {
                    ViewStageBubble bubble = go.GetComponent<ViewStageBubble>();
                    if (bubble) {
                        bubble.SetData(lInfo[index]);
                    }
                }
            }
            index++;
        }
    }
    
    public void UpdateStageInfo()
    {
        CounterPartInfo Info = StageDC.GetCounterPartInfo();
        if (Info != null) {
            if (MyHead.stagename != null) {
                int Stage = Info.id  % 1000;
                MyHead.stagename.text = "[ffffff]"   +   Info.chapters  + "-" + Stage + " " +  Info.chaptersname + "·" + Info.counterpartname + "[-]";
            }
            
            if (MyHead.ManualLabel != null) {
                MyHead.ManualLabel.text = "[ffff00]" + NGUIUtil.GetStringByKey("88800096") + "[-]" + "[ffffff]" + Info.win_physical + "[-]";
            }
            if (MyHead.RewardLabel != null) {
                MyHead.RewardLabel.text = "[ffff00]" + NGUIUtil.GetStringByKey("88800097") + "[-]";
            }
            if (MyHead.DescLabel != null) {
                MyHead.DescLabel.text = "[ffffff]" + Info.description + "[-]";
            }
            if (MyHead.TimesLabel != null) {
                if (m_type == StageType.Normal) {
                    MyHead.TimesLabel.text = "" ;
                    m_AvailableMultipleSweep = 10;
                } else {
                    int Times = StageDC.GetPassStageTimes(m_type, m_stageid);
                    m_AvailableMultipleSweep = Info.times - Times;
                    MyHead.TimesLabel.text = "[ffff00]" + NGUIUtil.GetStringByKey("88800098") + "[-]" + "[ffffff]" + m_AvailableMultipleSweep + "/" + Info.times + "[-]";
                }
            }
            SetStageStar(StageDC.GetPassStageStar(m_type, m_stageid));
        }
    }
    
    public void SetStageSoldier()
    {
        if (MyHead.RoleTable == null) {
            return ;
        }
        List<int> list = new List<int>();
        CmCarbon.GetAttackList(ref list);
        foreach (int DataID in  list) {
            SoldierInfo s = CmCarbon.GetSoldierInfo(LifeMCamp.ATTACK, DataID);
            if (s == null) {
                continue;
            }
            GameObject go = NDLoad.LoadWndItem("RoleViewItem", MyHead.RoleTable.transform);
            if (go != null) {
                RoleViewItem item = go.GetComponent<RoleViewItem>();
                if (item != null) {
                    item.SetRoleRewardItem(s);
                }
            }
            
        }
        MyHead.Rewardtable.enabled = true;
        MyHead.Rewardtable.Reposition();
        
    }
    
    /// <summary>
    /// 设置战役星级
    /// </summary>
    private void SetStageStar(int star)
    {
    
        if (star <= 3 && MyHead.m_lstar.Length == 3) {
            NGUIUtil.SetStarLevelNum(MyHead.m_lstar, star);
        }
        
    }
    
    private void SetStageItemReward()
    {
        List<int> lItem = StageDC.GetStageReward();
        if (lItem != null && lItem.Count > 0) {
            if (MyHead.Rewardtable == null) {
                return ;
            }
            
            for (int i = 0 ; i < lItem.Count ; i++) {
                GameObject go = NDLoad.LoadWndItem("RewardItem", MyHead.Rewardtable.transform);
                if (go != null) {
                    RewardItem item = go.GetComponent<RewardItem>();
                    if (item != null) {
                        item.SetRewardItem(lItem[i], 0);
                    }
                }
            }
            MyHead.Rewardtable.enabled = true;
            MyHead.Rewardtable.Reposition();
        }
    }
    
    private void SetStageItemReward(int itemType, int itemID, int itemCount)
    {
        GameObject go = NDLoad.LoadWndItem("RewardItem", MyHead.Rewardtable.transform);
        if (go != null) {
            RewardItem item = go.GetComponent<RewardItem>();
            if (item != null) {
                item.SetRewardItem(itemType, itemID, itemCount);
            }
        }
    }
    
    
    void ClickClose(UIButton sender)
    {
        SceneM.Load(MainTownScene.GetSceneName(), false, false);
        MainTownInit.s_currentState = MainTownState.StageMap;
    }
    
    void ClickCombat(UIButton sender)
    {
        CounterPartInfo Info = StageDC.GetCounterPartInfo();
        if (Info == null) {
            return ;
        }
        if (m_type != StageType.Normal) {
            int Times = StageDC.GetPassStageTimes(m_type, m_stageid);
            if (Times >= Info.times) {
                NGUIUtil.ShowTipWndByKey("88800031", 2);
                return ;
            }
        }
        if (StageDC.GetPveMode() == PVEMode.Defense) {
            SoundPlay.PlayBackGroundSound("bgm_battle_loop", true, false);
            ShipCanvasInfo Canvas = new ShipCanvasInfo();
            List<SoldierInfo> lSoldier = new List<SoldierInfo>();
            List<ShipPutInfo> lBuild = new List<ShipPutInfo>();
            ShipPlanDC.SetCurShipPlan(PlanType.Default);
            ShipPlan Plan = ShipPlanDC.GetCurShipPlan();
            Plan.GetShipCansInfoPlan(ref Canvas, ref lSoldier, ref lBuild);
            CmCarbon.SetDefenseMap(Canvas);
            CmCarbon.SetDefenseBuild(lBuild);
            CmCarbon.SetDefenseSoldier(lSoldier);
            CmCarbon.SetDefenseFloor(UserDC.GetDeckLevel());
            CmCarbon.SetDefenseUserInfo(Info, null);
            List<int> l = new List<int>();
            foreach (SoldierInfo s in lSoldier) {
                l.Add(s.ID);
            }
            sender.enabled = false;
            //DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_702, CanCombat);
            //StageDC.SendStageAttackRequest(m_stageid, l, 0);
            SceneM.Load(CombatScene.GetSceneName(), false, false);
        } else {
            sender.enabled = false;
            //SoldierDC.Send_SoldierBattleListRequest(0);
            //DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_214, LoadStage);
            SceneM.Load(CombatScene.GetSceneName(), false, false);
        }
    }
    
    void ClickSweepMultiple(UIButton sender)
    {
        // 判定是否有扫荡券
        ItemTypeInfo sweepTicketInfo = ItemDC.GetSweepTickets();
        if (sweepTicketInfo == null || sweepTicketInfo.Num < m_AvailableMultipleSweep) {
            NGUIUtil.ShowFreeSizeTipWnd(NGUIUtil.GetStringByKey("70000229"));
            return;
        }
        
        CounterPartInfo info = StageDC.GetCounterPartInfo();
        
        // 体力不足
        int playerPhysical = UserDC.GetPhysical();
        if (playerPhysical < info.win_physical * m_AvailableMultipleSweep) {
            NGUIUtil.ShowFreeSizeTipWnd(NGUIUtil.GetStringByKey("99904008"));
            return;
        }
        
        // VIP, 解锁判定
        //......
        int vipLevel = UserDC.GetVIPLevel();
        if (vipLevel < 4) {
            string tip = string.Format(NGUIUtil.GetStringByKey("70000228"), 4);
            NGUIUtil.ShowFreeSizeTipWnd(tip);
            //return;
        }
        
        m_ClickSweepTimes = m_AvailableMultipleSweep;
        StageDC.SendStageSweepRequest(StageDC.GetCompaignStageID(), m_ClickSweepTimes);
        DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_718, ShowSweepResult);
    }
    
    void ClickSweepOnce(UIButton sender)
    {
        // 判定是否有扫荡券
        ItemTypeInfo sweepTicketInfo = ItemDC.GetSweepTickets();
        if (sweepTicketInfo == null || sweepTicketInfo.Num == 0) {
            NGUIUtil.ShowFreeSizeTipWnd(NGUIUtil.GetStringByKey("70000229"));
            return;
        }
        CounterPartInfo info = StageDC.GetCounterPartInfo();
        int playerPhysical = UserDC.GetPhysical();
        
        // 体力不足
        if (playerPhysical < info.win_physical) {
            NGUIUtil.ShowFreeSizeTipWnd(NGUIUtil.GetStringByKey("99904008"));
            return;
        }
        
        m_ClickSweepTimes = 1;
        StageDC.SendStageSweepRequest(StageDC.GetCompaignStageID(), m_ClickSweepTimes);
        DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_718, ShowSweepResult);
    }
    
    void ShowSweepResult(int nErrorCode)
    {
        UpdateStageInfo();
        UpdateSweepInfo();
        
        List<StageDC.StageSweepReward> sweepRewards = StageDC.GetStageSweepRewards();
        StageWipeWnd wnd = WndManager.GetDialog<StageWipeWnd>();
        wnd.Init(m_ClickSweepTimes);
        wnd.SetWipeResult(sweepRewards);
        //DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_718, ShowSweepResult);
    }
    
    void LoadStage(int nErrorCode)
    {
        SceneM.Load(CombatScene.GetSceneName(), false, false);
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_214, LoadStage);
    }
    
    /// <summary>
    /// 0702 获取所有已造建筑回应
    /// </summary>
    /// <param name="Info"></param>
    void CanCombat(int nErrorCode)
    {
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_702, CanCombat);
        if (nErrorCode == 0) {
            NGUIUtil.DebugLog("2：收到702 获取所有建筑请求 回应" + Time.time, "yellow");
            SceneM.Load(CombatScene.GetSceneName(), false, null, false);
        } else {
            Debug.Log("获取数据失败：" + nErrorCode.ToString());
        }
    }
    
}
