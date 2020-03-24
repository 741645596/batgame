using UnityEngine;
using System.Collections.Generic;

public enum FindSoldierMode {
    DataID    = 0,
    SceneID   = 1,
};

public class CombatWnd : WndBase
{
    public CombatWnd_h MyHead {
        get
        {
            return (base.BaseHead() as CombatWnd_h);
        }
    }
    
    PortraitItem mPortrait;
    public PortraitItem Portrait {
        get
        {
            if (mPortrait == null)
            {
                mPortrait = MyHead.PortrainItem;
            }
            return mPortrait;
        }
    }
    
    UITablePivot mCombatRoleTable;
    UITablePivot CombatRoleTable {
        get
        {
            if (mCombatRoleTable == null)
            {
                mCombatRoleTable = MyHead.CombatRoleTable;
            }
            return mCombatRoleTable;
        }
    }
    
    /// <summary>
    /// 炮弹兵的 CombatRoleUI
    /// </summary>
    private Dictionary<int, CombatRoleItem> m_SoldierUI = new Dictionary<int, CombatRoleItem>();
    private bool m_bFirstFireSelected = true;
    /// <summary>
    /// 新手引导-出兵手势示意
    /// </summary>
    private GameObject m_GuideFireEffect = null;
    private bool m_bRepositionFinish = false;
    private Transform m_tselFireSoldier = null;
    private Transform m_tReleaseSoldier = null;
    
    private float m_fGuideCounter = 0f;
    private int m_iGuideIndex = 0;
    
    public override void WndStart()
    {
        base.WndStart();
        RegisterEvents();
        
        if (CmCarbon.GetPlayer2Camp(true) == LifeMCamp.DEFENSE) {
            InitSoldierUI();
            CombatRoleTable.padding.y = -95f;
        }
    }
    
    
    public static new string DialogIDD()
    {
        return "CombatWnd";
    }
    /// <summary>
    /// 清除引导表现
    /// </summary>
    public void DestroyGuideFireEffect()
    {
        if (m_GuideFireEffect) {
            Destroy(m_GuideFireEffect);
        }
    }
    
    public void ClearSoldier()
    {
        foreach (CombatRoleItem w in m_SoldierUI.Values) {
            if (w != null) {
                DestroyImmediate(w.gameObject);
            }
        }
        m_SoldierUI.Clear();
    }
    
    void RegisterEvents()
    {
        EventCenter.RegisterHooks(NDEventType.Attr_Anger, SetAnger);
        EventCenter.RegisterHooks(NDEventType.Attr_FullAnger, SetAngerEffect);
        EventCenter.RegisterHooks(NDEventType.Attr_HP, SetHP);
        EventCenter.RegisterHooks(NDEventType.StatusCG, SetStatusEffect);
    }
    
    private void SetStatusEffect(int SceneID, object Param)
    {
        if (Param is StatusInfo) {
            StatusInfo Info = Param as StatusInfo;
            StatusType type = Info.Type;
            CombatRoleItem c = GetSoldierUI(SceneID, FindSoldierMode.SceneID);
            if (c != null) {
                if (Info.State == StatusState.Add) {
                    c.ShowStatusEffect(type, Info.time);
                }
            }
        }
    }
    
    void AntiRegisterEvents()
    {
        EventCenter.AntiRegisterHooks(NDEventType.Attr_Anger, SetAnger);
        EventCenter.AntiRegisterHooks(NDEventType.Attr_FullAnger, SetAngerEffect);
        EventCenter.AntiRegisterHooks(NDEventType.Attr_HP, SetHP);
        EventCenter.AntiRegisterHooks(NDEventType.StatusCG, SetStatusEffect);
    }
    /// <summary>
    /// 设置怒气UI特效（怒气条）
    /// </summary>
    private void SetAngerEffect(int SceneID, object Param)
    {
        int data = (int)Param;
        CombatRoleItem c = GetSoldierUI(SceneID, FindSoldierMode.SceneID);
        if (c != null) {
            if (CmCarbon.IsBorn(c.SoldierDataID)) {
                if (data == 0) {
                    c.DestroyAngryUI();
                } else {
                    c.SpawnAngryUI();
                }
            }
        }
    }
    /// <summary>
    /// 设置怒气可释放特效
    /// </summary>
    private void SetAngerReleaseEffect(int SceneID, object Param)
    {
    
    }
    
    public void InitSoldierUI()
    {
        ClearSoldier();
        CreateList();
    }
    /// <summary>
    /// 创建发兵按钮列表
    /// </summary>
    private void CreateList()
    {
        float factor = Screen.width / 960f;
        List<int> PlayerList = new List<int>();
        CmCarbon.GetPlayerList(ref PlayerList);
        
        SoldierInfo soldierInfo = new SoldierInfo();
        
        foreach (int key in PlayerList) {
            soldierInfo = CmCarbon.GetPlayerSoldierInfo(key);
            GameObject go = NDLoad.LoadWndItem("CombatRoleItem", MyHead.RoleParent);
            
            //加载点击事件
            UIButton u = go.GetComponentInChildren<UIButton>();
            if (u) {
                u.OnClickEventHandler += FireSoldier;
                u.gameObject.name = key.ToString();
            }
            //根据类型切换头像
            CombatRoleItem c = go.GetComponent<CombatRoleItem>();
            if (c != null) {
                m_SoldierUI.Add(key, c);
                c.SoldierDataID = key;
                c.Set3DHeadStandby();
                c.SetSoldierUI(soldierInfo);
                float anger = soldierInfo.m_mp * 1.0f / ConfigM.GetAngerK(1);
                c.SetAnger(anger);
                if (CmCarbon.GetPlayer2Camp(true) == LifeMCamp.DEFENSE) {
                    c.AddFireBtnState(FIRE_BTN_STATE.FireBtnFired);
                    c.SetPowerActive(false);
                }
            }
        }
        
        NGUIUtil.RepositionTablePivot(MyHead.RoleParent.gameObject);
        Invoke("RepositionFinish", Time.deltaTime);
    }
    
    void RepositionFinish()
    {
        m_bRepositionFinish = true;
    }
    
    public  void FireSoldier(UIButton sender)
    {
        int DataID = System.Convert.ToInt32(sender.name);
        List<int> PlayerList = new List<int>();
        CmCarbon.GetPlayerList(ref PlayerList);
        
        CombatRoleItem c = GetSoldierUI(DataID, FindSoldierMode.DataID);
        if (c == null) {
            return;
        } else {
            if (c.IsInFireBtnState(FIRE_BTN_STATE.FireBtnDied)) {
                return;
            }
            //c.soldierItem.PlayBtnAnimation();//点击炮弹兵UI播放动画
            if (c.IsInFireBtnState(FIRE_BTN_STATE.FireBtnFired) &&
                !c.IsInFireBtnState(FIRE_BTN_STATE.FireBtnAngerFull) && !c.IsInFireBtnState(FIRE_BTN_STATE.FireBtnSkill9001)) {
                return ;
            } else if (!c.IsInFireBtnState(FIRE_BTN_STATE.FireBtnSelected)) {
                ReadyToFire(DataID);
            } else {
                CancelFire(DataID);
                Portrait.Hide();
            }
        }
        //处理其他按钮
        foreach (int key in PlayerList) {
            if (key == DataID) {
                continue;
            }
            CombatRoleItem c1 = GetSoldierUI(key, FindSoldierMode.DataID);
            if (c1 == null) {
                continue;
            }
            if (c1.IsInFireBtnState(FIRE_BTN_STATE.FireBtnSelected)
                && !c.IsInFireBtnState(FIRE_BTN_STATE.FireBtnFired)) {
                
                c1.SetCancelToFire();
            }
            c1.DelFireBtnState(FIRE_BTN_STATE.FireBtnSelected);
        }
        
    }
    
    public void ReadyToFire(int soldierDataID)
    {
        int SceneID = CM.DataID2SceneIDInSoldier(soldierDataID);
        Life life = CM.GetLifeM(SceneID, LifeMType.SOLDIER);
        CombatRoleItem c = GetSoldierUI(soldierDataID, FindSoldierMode.DataID);
        if (c != null) {
            c.AddFireBtnState(FIRE_BTN_STATE.FireBtnSelected);
            if (!c.IsInFireBtnState(FIRE_BTN_STATE.FireBtnFired)) {
                PlayerSoldierFire.isTrace = true;
                PlayerSoldierFire.soldierDataID = soldierDataID;
                c.SetReadyToFire();
                Portrait.ShowSoldierPortrait(c.m_soldierItem.m_info.SoldierTypeID);
                GodSkillWnd Wnd = WndManager.FindDialog<GodSkillWnd>();
                if (Wnd != null) {
                    Wnd.CancelSelect();
                }
                //
                Building.ShowAllHp(true);
                if (m_bFirstFireSelected) {
                    m_bFirstFireSelected = true;
                    MainCameraM.s_Instance.AutoMoveTo(MainCameraM.s_vBattleFarthestCamPos);
                }
            }
            
            if (c.IsInFireBtnState(FIRE_BTN_STATE.FireBtnSkill9001)) {
                GodSkillWnd wnd = WndManager.FindDialog<GodSkillWnd>();
                if (wnd) {
                    NGUIUtil.DebugLog("FireBtnSkill9001");
                    wnd.SelectRole(SceneID);
                }
                return;
            }
            
            if (c.IsInFireBtnState(FIRE_BTN_STATE.FireBtnAngerFull) && !c.IsInFireBtnState(FIRE_BTN_STATE.FireBtnDied) && !c.IsInFireBtnState(FIRE_BTN_STATE.FireBtnSkill9001)) {
                if (life != null && life.GetLifeProp().enabled == true) {
                    c.ReleaseAnger();
                    c.DelFireBtnState(FIRE_BTN_STATE.FireBtnSelected);
                }
            }
        }
    }
    
    public void SetUnBorn(int soldierDataID)
    {
        CombatRoleItem c = GetSoldierUI(soldierDataID, FindSoldierMode.DataID);
        c.Set3DHeadStandby();
        c.ResetFireBtnState();
        c.ReleaseAngerUI();
        c.ShowHpAnger(false);
    }
    
    public void CancelFire(int soldierDataID)
    {
        Building.ShowAllHp(false);
        PlayerSoldierFire.soldierDataID = -1;
        CombatRoleItem c = GetSoldierUI(soldierDataID, FindSoldierMode.DataID);
        if (c != null) {
            if (!c.IsInFireBtnState(FIRE_BTN_STATE.FireBtnFired)) {
                PlayerSoldierFire.isTrace = false;
                c.DelFireBtnState(FIRE_BTN_STATE.FireBtnSelected);
                c.SetCancelToFire();
            }
        }
    }
    
    /// <summary>
    /// 取消所有选择的炮弹兵
    /// </summary>
    public void CancelAllFire()
    {
        PlayerSoldierFire.soldierDataID = -1;
        foreach (CombatRoleItem c in m_SoldierUI.Values) {
            if (c != null) {
                if (c.IsInFireBtnState(FIRE_BTN_STATE.FireBtnSelected)) {
                    PlayerSoldierFire.isTrace = false;
                    c.DelFireBtnState(FIRE_BTN_STATE.FireBtnSelected);
                    c.SetCancelToFire();
                }
            }
        }
    }
    
    public  void FireLater(int soldierDataID)
    {
        Building.ShowAllHp(false);
        ResetGuideCounter();
        CombatRoleItem UI = GetSoldierUI(soldierDataID, FindSoldierMode.DataID);
        if (UI != null) {
            UI.ShowHpAnger(true);
            UI.AddFireBtnState(FIRE_BTN_STATE.FireBtnFired);
            UI.DelFireBtnState(FIRE_BTN_STATE.FireBtnSelected);
            UI.Set3DHeadState(AnimatorState.UIAttack);
            Portrait.Fire();
        }
        
        if (CmCarbon.GetFireOutCount() == 1 && CSState.Start == CombatScheduler.State) {
            CombatScheduler.SetCSState(CSState.Combat);
        }
    }
    
    public  void Set3DHeadState(int SceneID, AnimatorState eState)
    {
        CombatRoleItem c = GetSoldierUI(SceneID, FindSoldierMode.SceneID);
        if (c != null) {
            c.Set3DHeadState(eState);
        }
    }
    
    public  void ReleaseSkillUI(int SceneID)
    {
        CombatRoleItem c = GetSoldierUI(SceneID, FindSoldierMode.SceneID);
        if (c != null) {
            c.ReleaseAngerUI();
        }
    }
    public void SetAngerSpriteColor(int SceneID, bool red = false)
    {
        CombatRoleItem c = GetSoldierUI(SceneID, FindSoldierMode.SceneID);
        if (c != null) {
            c.SetAngerSpriteColor(red);
        }
    }
    /// <summary>
    /// 获取下个未发射炮弹兵的ID,规则按照炮弹兵UI排序 从左到右
    /// </summary>
    /// <returns></returns>
    int GetNextUnFiredSoldierID()
    {
        int result = -1;
        List<int> PlayerList = new List<int>();
        CmCarbon.GetPlayerList(ref PlayerList);
        foreach (int key in PlayerList) {
            if (!CmCarbon.IsBorn(key)) {
                result = key;
                break;
            }
        }
        return result;
    }
    /// <summary>
    /// 自动选择下一个未发射炮弹兵
    /// </summary>
    public void AutoSelectNextSoldier()
    {
        int nextSoldierID = GetNextUnFiredSoldierID();
        if (nextSoldierID == -1) {
            //Debug.Log("获取未发射炮弹兵失败或者已经全部发射！");
            return;
        }
        
        CombatRoleItem c = m_SoldierUI[nextSoldierID];
        UIButton nextBtn = c.GetComponentInChildren<UIButton>();
        if (nextBtn) {
            FireSoldier(nextBtn);
        } else {
            Debug.Log("CombatWnd.cs->AutoSelectNextSoldier()  nextBtn UIButton not found!!!");
        }
    }
    /// <summary>
    /// 显示所有可被使命召唤的
    /// </summary>
    public void Show9001(bool isShow)
    {
        foreach (CombatRoleItem key in m_SoldierUI.Values) {
            CombatRoleItem c = key;
            if (isShow) {
                if (c.IsInFireBtnState(FIRE_BTN_STATE.FireBtnFired)) {
                    c.ShowSKill9001Effect0(true);
                }
            } else {
                c.ShowSKill9001Effect0(false);
            }
        }
    }
    /// <summary>
    /// 清空UI特效
    /// </summary>
    public void ClearUIEffect()
    {
        foreach (CombatRoleItem c in m_SoldierUI.Values) {
            c.ClearUIEffect();
        }
    }
    
    CombatRoleItem GetSoldierUI(int id, FindSoldierMode Mode)
    {
        if (m_SoldierUI == null) {
            return null;
        }
        if (Mode == FindSoldierMode.SceneID) {
            id = CM.SceneID2DataIDInSoldier(id);
        }
        if (m_SoldierUI.ContainsKey(id)) {
            return m_SoldierUI[id];
        }
        return null;
    }
    
    public  void  ShowSkill9001Effect(int id)
    {
        CombatRoleItem c = GetSoldierUI(id, FindSoldierMode.SceneID);
        c.ShowSkill9001Effect();
    }
    
    
    
    public  void SetHP(int sceneID, float hpScale)
    {
        CombatRoleItem c = GetSoldierUI(sceneID, FindSoldierMode.SceneID);
        if (c != null) {
            if (CmCarbon.IsBorn(c.SoldierDataID)) {
                c.SetHp(hpScale);
            } else {
                c.SetHp(1.0f);
            }
        }
    }
    public  void SetHP(int sceneID, object Param)
    {
        float hpScale = (float)Param;
        CombatRoleItem c = GetSoldierUI(sceneID, FindSoldierMode.SceneID);
        if (c != null) {
            c.SetHp(hpScale);
        }
    }
    public  void SetAnger(int sceneID, object Param)
    {
        int ANGER = (int)Param;
        CombatRoleItem c = GetSoldierUI(sceneID, FindSoldierMode.SceneID);
        if (c != null) {
            if (CmCarbon.IsBorn(c.SoldierDataID)) {
                float anger = ANGER * 1.0f / ConfigM.GetAngerK(1);
                if (!c.m_bEmptyAngerBar) {
                    c.SetAnger(anger);
                }
                if (ANGER >= ConfigM.GetAngerK(1)) {
                    if (!c.IsInFireBtnState(FIRE_BTN_STATE.FireBtnAngerFull)) {
                        c.AddFireBtnState(FIRE_BTN_STATE.FireBtnAngerFull);
                    }
                } else {
                    c.DelFireBtnState(FIRE_BTN_STATE.FireBtnAngerFull);
                }
            }
        }
    }
    
    public  void SetDied(int sceneID, int FullHp)
    {
        CombatRoleItem c = GetSoldierUI(sceneID, FindSoldierMode.SceneID);
        if (c != null) {
            //NGUIUtil.DebugLog("sceneID "+ sceneID + "  Died");
            c.ResetFireBtnState();
            c.AddFireBtnState(FIRE_BTN_STATE.FireBtnDied);
            SetHP(sceneID, 0.0f);
            c.SetDied(c.gameObject);
            c.DestroyAngryUI();
            c.ShowSKill9001Effect0(false);
            c.ShowStatusEffect(StatusType.Die, 0);
        }
    }
    
    public void ResetUIScale()
    {
        foreach (CombatRoleItem c in m_SoldierUI.Values) {
            c.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }
    
    void OnDestroy()
    {
        AntiRegisterEvents();
        m_SoldierUI.Clear();
    }
    
    
    
    
    public void ResetGuideCounter()
    {
        m_fGuideCounter = 0;
    }
    
}
