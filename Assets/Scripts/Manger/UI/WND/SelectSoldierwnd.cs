using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class SelectSoldierwnd : WndBase
{

    public int maxSelect = 5;
    //已选中的炮弹兵
    public List<CanvasItem> m_SelectedSoldier = new List<CanvasItem>();
    List<int> m_SelectedSoldierIDs = new List<int>();
    
    public bool bStart = false;
    public bool hasCaption = false;
    //海神杯查看是否正在战斗.
    public int m_athleticsCheckRank;
    public int m_athleticsCheckUid;
    
    public SelectSoldierwnd_h MyHead {
        get
        {
            return (base.BaseHead() as SelectSoldierwnd_h);
        }
    }
    /// <summary>
    /// 总战力
    /// </summary>
    private  int m_iTotalCombatForce;
    
    private int m_iSoldiersCount;
    /// <summary>
    /// 控制提示信息的透明度
    /// </summary>
    private TweenAlpha m_tweenAlpha;
    
    /// <summary>
    /// 保存所有船员的RoleButton
    /// </summary>
    private Dictionary<int, CanvasItem> m_CrewList = new Dictionary<int, CanvasItem>();
    
    private int m_SelectedBlackScienceCaptainID = 0;
    private int m_SelectedBlackScienceDataID = 0;
    
    private bool m_bFlyFinish = false;
    
    private Transform m_tmask = null;
    
    bool m_InitSoldiers = false;
    
    void Awake()
    {
        //自动关联Toogle
        MyHead.ToggleList = U3DUtil.GetComponentsInChildren<Transform>(MyHead.ToggleParent.gameObject, false);
    }
    
    // Use this for initialization
    public override void WndStart()
    {
        base.WndStart();
        if (MyHead.BtnStart) {
            MyHead.BtnStart.OnClickEventHandler += StartCombat;
        }
        if (MyHead.BtnBack) {
            MyHead.BtnBack.OnClickEventHandler += Back;
        }
        MyHead.BtnBlackScience.OnClickEventHandler += BtnBlackScience_OnClickEventHandler;
        
        EventDelegate.Add(MyHead.TogAll.onChange, TogAll_OnChangeEventHandler);
        EventDelegate.Add(MyHead.TogAssisst.onChange, TogAssisst_OnChangeEventHandler);
        EventDelegate.Add(MyHead.TogDPS.onChange, TogDPS_OnChangeEventHandler);
        EventDelegate.Add(MyHead.TogTank.onChange, TogTank_OnChangeEventHandler);
        EventDelegate.Add(MyHead.TogMercenary.onChange, TogMercenary_OnChangeEventHandler);
        
        hasCaption = true;
        if (BattleEnvironmentM.GetBattleEnvironmentMode() == BattleEnvironmentMode.CombatPVE) {
            hasCaption = StageDC.GetCounterPartInfo().iscaptain == 1;
        }
        
        NGUIUtil.SetActive(MyHead.BtnBlackScience.transform.parent.gameObject, hasCaption);
        
        if (MainCameraM.s_Instance) {
            MainCameraM.s_Instance.EnableDrag(false);
        }
        
        NGUIUtil.UpdatePanelValue(MyHead.PanelMask, 1024, 0.15f);
        NGUIUtil.TweenGameObjectPosX(MyHead.ScrollLeft, -394, 0.15f, gameObject, "HideAniScroll");
        NGUIUtil.TweenGameObjectPosX(MyHead.ScrollRight, 394, 0.15f);
        ShowTogglesAni(0.1f, 0.1f);
    }
    
    /// <summary>
    /// 显示toggle动画
    /// </summary>
    /// <param name="delay">开始显示第一个toggle的延时</param>
    /// <param name="interval">显示下一个toggle的间隔</param>
    void ShowTogglesAni(float delay, float interval)
    {
        GameObjectActionExcute gae = GameObjectActionExcute.CreateExcute(gameObject);
        float waitInterval = delay;
        for (int i = 0; i < MyHead.ToggleList.Length; i++) {
            GameObjectActionWait wait = new GameObjectActionWait(waitInterval, WaitFinish);
            wait.Data1 = i;
            gae.AddAction(wait);
            waitInterval = interval;
        }
    }
    /// <summary>
    /// 顺序显示toggle
    /// </summary>
    private void WaitFinish(object o)
    {
        GameObject go = o as GameObject;
        if (go == null) {
            return;
        }
        GameObjectActionExcute gae = go.GetComponent<GameObjectActionExcute>();
        if (gae) {
            GameObjectActionWait wait = gae.GetCurrentAction() as GameObjectActionWait;
            if (wait != null) {
                int index = (int)wait.Data1;
                NGUIUtil.SetActive(MyHead.ToggleList[index].gameObject, true);
            }
        }
    }
    /// <summary>
    /// 隐藏左右两个动画轴
    /// </summary>
    public void HideAniScroll()
    {
        NGUIUtil.SetActive(MyHead.ScrollLeft, false);
        NGUIUtil.SetActive(MyHead.ScrollRight, false);
    }
    
    void BtnBlackScience_OnClickEventHandler(UIButton sender)
    {
        bool isHave = BlackScienceDC.CheckHaveCaption();
        if (!isHave) {
            NGUIUtil.ShowFreeSizeTipWnd(30000050);
            return;
        }
        
        BlackScienceChoWnd bsWnd = WndManager.GetDialog<BlackScienceChoWnd>();
        ShipPlan P = ShipPlanDC.GetCurShipPlan();
        CaptionInfo captionInfo = BlackScienceDC.GetCaptionD(P.BlackScienceID);
        bsWnd.SetSelectCaptain(captionInfo);
    }
    
    public void Back(UIButton sender)
    {
        BSC.AntiAllRegisterHooks();
        if (BattleEnvironmentM.GetBattleEnvironmentMode() == BattleEnvironmentMode.CombatPVE) {
            SceneM.Load(ViewStageScene.GetSceneName(), false, null, false);
        }  else {
            WndManager.DestoryDialog<SelectSoldierwnd>();
            WndManager.DestoryDialog<CombatWnd>();
            CombatInfoWnd wnd = WndManager.GetDialog<CombatInfoWnd>();
            if (wnd != null) {
                wnd.SetWndMode(CombatInfoMode.view);
            }
            WndManager.GetDialog<CombatCountDownWnd>();
            MainCameraM.s_Instance.EnableDrag(true);
        }
    }
    /// <summary>
    /// 智力筛选
    /// </summary>
    /// <param name="sender"></param>
    public void TogMercenary_OnChangeEventHandler()
    {
    
        if (!MyHead.TogMercenary.value) {
            return ;
        }
        List<SoldierInfo> crewList = new List<SoldierInfo>();
        SoldierDC.GetSoldiers(ref crewList, CombatLoactionType.NONE);
        SoldierM.SortSoldierList(ref crewList);
        LoadHeadList(crewList, MyHead.SelectSoldierTable, 1);
    }
    /// <summary>
    /// 智力筛选
    /// </summary>
    /// <param name="sender"></param>
    public void TogAssisst_OnChangeEventHandler()
    {
        if (!MyHead.TogAssisst.value) {
            return ;
        }
        List<SoldierInfo> crewList = new List<SoldierInfo>();
        SoldierDC.GetSoldiers(ref crewList, CombatLoactionType.Assist);
        SoldierM.SortSoldierList(ref crewList);
        LoadHeadList(crewList, MyHead.SelectSoldierTable, 1);
    }
    /// <summary>
    /// 敏捷筛选
    /// </summary>
    /// <param name="sender"></param>
    public void TogDPS_OnChangeEventHandler()
    {
        if (!MyHead.TogDPS.value) {
            return ;
        }
        List<SoldierInfo> crewList = new List<SoldierInfo>();
        SoldierDC.GetSoldiers(ref crewList, CombatLoactionType.DPS);
        SoldierM.SortSoldierList(ref crewList);
        LoadHeadList(crewList, MyHead.SelectSoldierTable, 1);
    }
    /// <summary>
    /// 力量筛选
    /// </summary>
    /// <param name="sender"></param>
    public void TogTank_OnChangeEventHandler()
    {
        if (!MyHead.TogTank.value) {
            return ;
        }
        List<SoldierInfo> crewList = new List<SoldierInfo>();
        SoldierDC.GetSoldiers(ref crewList, CombatLoactionType.Tank);
        SoldierM.SortSoldierList(ref crewList);
        LoadHeadList(crewList, MyHead.SelectSoldierTable, 1);
    }
    /// <summary>
    /// 全部
    /// </summary>
    /// <param name="sender"></param>
    public void TogAll_OnChangeEventHandler()
    {
        if (!MyHead.TogAll.value) {
            return ;
        }
        List<SoldierInfo> crewList = new List<SoldierInfo>();
        SoldierDC.GetSoldiers(ref crewList, CombatLoactionType.ALL);
        m_iSoldiersCount = crewList.Count;
        SoldierM.SortSoldierList(ref crewList);
        LoadHeadList(crewList, MyHead.SelectSoldierTable, 1);
        
        if (!m_InitSoldiers) {
            List<int> soldiersLastCombat = SoldierDC.GetSoldiersLastCombat();
            for (int i = 0; i < soldiersLastCombat.Count; i++) {
                int soldierID = soldiersLastCombat[i];
                if (soldierID != 0) {
                    SelectSoldier(soldierID);
                }
            }
            int captinLastCombat = SoldierDC.GetCaptainLastCombat();
            if (captinLastCombat != 0) {
                SetBlackScienceDataID(captinLastCombat);
            }
            
            m_InitSoldiers = true;
        }
    }
    /// <summary>
    /// 控制5个底板阴影头像的显示和隐藏
    /// </summary>
    private void ShowShadow()
    {
        int count = m_SelectedSoldier.Count;
        for (int i = 0; i < MyHead.SelectedPos.Count; i++) {
            if (i < count) {
                NGUIUtil.SetActive(MyHead.SelectedPos[i].gameObject, false);
            } else {
                NGUIUtil.SetActive(MyHead.SelectedPos[i].gameObject, true);
            }
        }
        m_bFlyFinish = true;
    }
    
    /// <summary>
    /// 加载头像列表
    /// </summary>
    private void LoadHeadList(List<SoldierInfo> l, UITable Parent, int soldierType)
    {
        //异常判断
        if (Parent == null) {
            return;
        }
        //清空原有列表
        EmptyHeadList(Parent.gameObject, soldierType);
        
        if (l == null || l.Count == 0) {
            return;
        }
        foreach (SoldierInfo  soldierInfo in l) {
            GameObject go = NDLoad.LoadWndItem("CanvasItem", Parent.transform);
            if (go != null) {
                CanvasItem item = go.GetComponent<CanvasItem>();
                if (item) {
                    m_CrewList.Add(soldierInfo.ID, item);
                    item.SetCanvasItem(soldierInfo, 3);
                    //item.ItemIndex = itemIndex++;
                    go.name = soldierInfo.m_name;
                    //NGUIUtil.SetItemPanelDepth(go, Parent.GetComponentInParent<UIPanel>());
                    if (IsContains(soldierInfo.ID)) {
                        item.SetMaskActive(true);
                    }
                }
            }
            
        }
        
        //RestoreGoCombatData();
        
        //排列，避免堆叠
        NGUIUtil.RepositionTable(Parent.gameObject);
    }
    
    /// <summary>
    /// 清空头像列表
    /// </summary>
    private void EmptyHeadList(GameObject Parent, int soldierType)
    {
        //异常判断
        if (Parent == null) {
            return;
        }
        
        m_CrewList.Clear();
        //清空原有列表
        U3DUtil.DestroyAllChild(Parent);
    }
    void oncomplete()
    {
        int f = 0;
    }
    public void SelectSoldier(int id)
    {
        CanvasItem item = null;
        m_CrewList.TryGetValue(id, out item);
        
        CanvasItem selecteditem = null;
        if (TryGetSelectedSoldier(id, ref selecteditem)) {
        
            if (item == null) {
                //NGUIUtil.DebugLog("item=null " + selecteditem.gameObject.name);
                selecteditem.GetComponent<Collider>().enabled = false;
                selecteditem.transform.DOMove(Vector3.zero, 0.3f);
            } else {
                selecteditem.GetComponent<Collider>().enabled = false;
                item.GetComponent<Collider>().enabled = false;
                //NGUIUtil.DebugLog(selecteditem.gameObject.name);
                selecteditem.transform.DOMove(item.transform.position, 0.3f);
            }
            
            
            SetCombatForece(id, false);
            m_SelectedSoldier.Remove(selecteditem);
            m_SelectedSoldierIDs.Remove(selecteditem.Soldier.ID);
            Reposition();
        } else if (m_SelectedSoldier.Count >= maxSelect) {
            item.m_bSelect = false;
            
            string showText = string.Format(NGUIUtil.GetStringByKey(10000157), maxSelect);
            NGUIUtil.ShowFreeSizeTipWnd(showText);
            return;
        } else if (m_SelectedSoldier.Count < maxSelect) {
            Transform parent = MyHead.SelectedPos[m_SelectedSoldier.Count ];
            GameObject go = NDLoad.LoadWndItem("CanvasItem", MyHead.TSelected);
            if (go != null) {
                SetCombatForece(id, true);
                item.SetMaskActive(true);
                go.transform.position = item.transform.position;
                selecteditem = go.GetComponent<CanvasItem>();
                if (selecteditem) {
                    selecteditem.SetCanvasItem(item.Soldier, 3);
                    //item.ItemIndex = itemIndex++;
                    go.name = item.Soldier.m_name;
                    //NGUIUtil.SetItemPanelDepth(go,MyHead.TSelected.GetComponentInParent<UIPanel>());
                    m_SelectedSoldier.Add(selecteditem);
                    m_SelectedSoldierIDs.Add(selecteditem.Soldier.ID);
                }
                Vector3 toPos = parent.position;
                
                item.GetComponent<Collider>().enabled = false;
                go.transform.DOMove(toPos, 0.1f);
            }
        } else {
        
        }
    }
    public void Reposition()
    {
        for (int i = 0; i < m_SelectedSoldier.Count; i++) {
            Vector3 toPos = MyHead.SelectedPos[i].position;
            m_SelectedSoldier[i].transform.DOMove(toPos, 0.1f);
        }
        ShowShadow();
    }
    
    public void DelayCallBack(object o)
    {
        GameObjectActionExcute gae = GetComponent<GameObjectActionExcute>();
        if (gae) {
            GameObjectActionWait wait = gae.GetCurrentAction() as GameObjectActionWait;
            if (wait != null) {
                CanvasItem selecteditem = wait.Data1 as CanvasItem;
                selecteditem.m_bSelect = false;
            }
        }
    }
    public void FlyFinishCall(CanvasItem i)
    {
        i.GetComponent<Collider>().enabled = true;
        i.m_bSelect = false;
        ShowShadow();
    }
    
    public void MoveBackCall(CanvasItem i)
    {
    
        CanvasItem item = null;
        i.m_bSelect = false;
        if (m_CrewList.TryGetValue(i.Soldier.ID, out item)) {
            item.GetComponent<Collider>().enabled = true;
            item.m_bSelect = false;
            item.SetMaskActive(false);
        }
        Destroy(i.gameObject);
    }
    public bool TryGetSelectedSoldier(int id, ref CanvasItem item)
    {
        for (int i = 0; i < m_SelectedSoldier.Count; i++) {
            if (m_SelectedSoldier[i].Soldier.ID == id) {
                item = m_SelectedSoldier[i];
                return true;
            }
        }
        return false;
    }
    public bool IsContains(int id)
    {
        for (int i = 0; i < m_SelectedSoldier.Count; i++) {
            if (m_SelectedSoldier[i].Soldier.ID == id) {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 是否全员出战
    /// </summary>
    /// <returns></returns>
    private bool IsSoldierAllCombat()
    {
        return !(m_SelectedSoldier.Count < Mathf.Min(maxSelect, m_CrewList.Count) && m_SelectedSoldier.Count < m_iSoldiersCount);
    }
    
    /// <summary>
    /// 开始出战
    /// </summary>
    /// <param name="sender"></param>
    void StartCombat(UIButton sender)
    {
        if (m_SelectedSoldier.Count == 0) {
            NGUIUtil.ShowTipWndByKey(88800090);
            return;
        }
        
        //出战人员是否满员判定 （玩家所有兵都参战时不做提示）
        if (m_SelectedSoldier.Count < Mathf.Min(maxSelect, m_CrewList.Count) && m_SelectedSoldier.Count < m_iSoldiersCount) { //&& m_iTotalSoldiersCount != m_BtnGoSelectedList.Count)
            DialogWnd dialogWnd = WndManager.GetDialog<DialogWnd>();
            if (dialogWnd) {
                dialogWnd.SetDialogLable(NGUIUtil.GetStringByKey(88800067), NGUIUtil.GetStringByKey(88800068), NGUIUtil.GetStringByKey(10000044));
                dialogWnd.YESButtonOnClick = YESToCombat;
            } else {
                Debug.Log("dialogWnd fail");
            }
        } else {
            GoCombat();
        }
    }
    
    void YESToCombat(UIButton sender)
    {
        GoCombat();
    }
    
    void GoCombat()
    {
        SoundPlay.PlayBackGroundSound("bgm_battle_loop", true, false);
        List<int> l = new List<int>();
        List<int> lDefense = new List<int>();
        List<SoldierInfo> soldierList = new List<SoldierInfo>();
        foreach (var i in m_SelectedSoldier) {
            soldierList.Add(SoldierDC.GetSoldiers(i.Soldier.ID));
            l.Add(i.Soldier.ID);
            lDefense.Add(i.Soldier.SoldierTypeID);
        }
        
        CmCarbon.SetAttackSoldier(soldierList);
        if (BattleEnvironmentM.GetBattleEnvironmentMode() == BattleEnvironmentMode.CombatPVE) {
            /*if (StageDC.GetPveMode() == PVEMode.Defense) {
                StageDC.SendStageAttackRequest(StageDC.GetCompaignStageID(), lDefense, 0);
            } else {
                StageDC.SendStageAttackRequest(StageDC.GetCompaignStageID(), l, m_SelectedBlackScienceDataID);
            }
            DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_702, CanCombat);*/
            JoinCombat();
        }   else {
            JoinCombat();
        }
    }
    
    void Rec_IsFightingRespone(int nErrorCode)
    {
        if (nErrorCode == 0) {
            DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_1414, AtheticsCombat);
        }
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_1416, Rec_IsFightingRespone);
    }
    
    void BackToAthleticMainWnd()
    {
        SceneM.Load(TreasureScene.GetSceneName(), false, null, false);
    }
    /// <summary>
    /// 海神杯挑战请求返回处理.
    /// </summary>
    /// <param name="nErrorCode">N error code.</param>
    void AtheticsCombat(int nErrorCode)
    {
        if (nErrorCode == 0) {
            JoinCombat();
        } else if (nErrorCode == 273) {
            NGUIUtil.ShowTipWndByKey(10000118);
        }
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_1414, AtheticsCombat);
    }
    
    /// <summary>
    /// 1003 金银岛掠夺
    /// </summary>
    /// <param name="Info"></param>
    void CanRob(int nErrorCode)
    {
    
        if (nErrorCode == 0) {
        
            CmCarbon.ReadyCombat();
            BattleEnvironmentM.BuildScene();
            JoinCombat();
        } else {
            WndManager.GetDialog<TreasureCanNotRobWnd>();
        }
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_1004, CanRob);
    }
    /// <summary>
    /// 0702 获取所有已造建筑回应
    /// </summary>
    /// <param name="Info"></param>
    void CanCombat(int nErrorCode)
    {
        if (nErrorCode == 0) {
            NGUIUtil.DebugLog("2：收到702 获取所有建筑请求 回应" + Time.time, "yellow");
            JoinCombat();
        } else {
            Debug.Log("获取数据失败：" + nErrorCode.ToString());
        }
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_702, CanCombat);
    }
    
    void JoinCombat()
    {
        MainCameraM.s_Instance.EnableDrag(true);
        WndManager.DestoryDialog<SelectSoldierwnd>();
        
        CombatInfoWnd wndInfo = WndManager.GetDialog<CombatInfoWnd>();
        if (wndInfo != null) {
            wndInfo.SetWndMode(CombatInfoMode.combat);
        }
        
        CombatWnd wnd = WndManager.GetDialog<CombatWnd>();
        if (wnd != null) {
            wnd.InitSoldierUI();
        }
        
        if (m_SelectedBlackScienceCaptainID != 0) {	//黑科技设置
            CmCarbon.AddGodSkill(true, m_SelectedBlackScienceCaptainID, ConfigM.GetInitMana());
            
            GodSkillWnd gsw = WndManager.GetDialog<GodSkillWnd>();
            if (gsw != null) {
                gsw.SetCurMana(CmCarbon.GetGodSkillMana(true));
                gsw.SetRequireMana(CmCarbon.GetGodSkill(true).GetRequireMana());
            }
        }
        
        CombatScheduler.SetCSState(CSState.Combat);
    }
    // 传的是UI控件的顺序ID
    public void SetBlackScienceID(int itemID)
    {
        m_SelectedBlackScienceCaptainID = itemID;
        CaptionInfo c = BlackScienceDC.GetCaption(itemID);
        m_SelectedBlackScienceDataID = c.m_id;
        if (c != null) {
            NGUIUtil.Set2DSprite(MyHead.SprCaptainHead, "Textures/role/", c.m_captionid.ToString());
        } else {
            NGUIUtil.DebugLog("BlackScienceID = " + itemID + " 未设置");
        }
        
    }
    // 传的是数据表里的ID
    public void SetBlackScienceDataID(int id)
    {
        CaptionInfo c = BlackScienceDC.GetCaptionD(id);
        m_SelectedBlackScienceDataID = id;
        m_SelectedBlackScienceCaptainID = c.m_captionid;
        if (c != null) {
            NGUIUtil.Set2DSprite(MyHead.SprCaptainHead, "Textures/role/", c.m_captionid.ToString());
        } else {
            NGUIUtil.DebugLog("BlackScienceID = " + id + " 未设置");
        }
        
    }
    
    public void ShowTipInfo(string str)
    {
        if (MyHead.LblTipInfo == null || str == "") {
            return;
        }
        MyHead.LblTipInfo.transform.parent.gameObject.SetActive(true);
        MyHead.LblTipInfo.text = str;
        m_tweenAlpha = TweenAlpha.Begin(MyHead.LblTipInfo.transform.parent.gameObject, 1.0f, 0);
        m_tweenAlpha.from = 1.0f;
        m_tweenAlpha.to = 0f;
        m_tweenAlpha.delay = 0.5f;
        m_tweenAlpha.AddOnFinished(ShowTipInfoEnd);
    }
    void ShowTipInfoEnd()
    {
        if (MyHead.LblTipInfo) {
            MyHead.LblTipInfo.transform.parent.gameObject.SetActive(false);
            m_tweenAlpha.ResetToBeginning();
        }
    }
    /// <summary>
    /// 设定总战斗力
    /// </summary>
    /// <param name="force"></param>
    public void SetCombatForece(int soldierID, bool isAdd)
    {
        SoldierInfo info = new SoldierInfo();
        info = SoldierDC.GetSoldiers(soldierID);
        
        int combatForece = info.m_combat_power;
        if (!isAdd) {
            combatForece = -combatForece;
        }
        if (MyHead.LblCombatForce) {
            m_iTotalCombatForce += combatForece;
            MyHead.LblCombatForce.text = m_iTotalCombatForce.ToString();
        }
    }
}
