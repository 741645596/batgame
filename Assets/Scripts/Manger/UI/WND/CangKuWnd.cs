using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 仓库（船只编辑）窗口
/// <From> </From>
/// <Author>QFord</Author>
/// </summary>
public class CangKuWnd : WndBase
{

    public CangKuWnd_h MyHead {
        get
        {
            return (base.BaseHead() as CangKuWnd_h);
        }
    }
    private Transform m_tmask = null;
    
    /// <summary>
    /// 标识仓库菜单是否开启
    /// </summary>
    private bool m_bOpen = true;//默认开启
    /// <summary>
    /// 标识是否选中 陷阱 标签
    /// </summary>
    private bool m_bTrap = true;
    
    private int m_iBtnTrapsTypeIndex = 0;
    private int m_iBtnSoldiersTypeIndex = 0;
    
    private float m_fMinHeight ;
    private float m_fMaxHeight ;
    
    private bool m_bRunOnce = true;
    
    private List<BuildInfo> warehouseBuildList;
    private List<SoldierInfo> soldierList = new List<SoldierInfo>();
    private ShipBuildType m_shipBuildType;
    private List<CanvasItem> m_lItems = new List<CanvasItem>();
    private int m_iCurCaptainID = 0;
    
    public override void WndStart()
    {
        base.WndStart();
        
        MyHead.BtnShow.OnClickEventHandler += BtnShow_OnClickEventHandler;
        MyHead.BtnHide.OnClickEventHandler += BtnHide_OnClickEventHandler;
        MyHead.BtnBlackScience.OnClickEventHandler += BtnBlackScience_OnClickEventHandler;
        MyHead.BtnDeleteAll.OnClickEventHandler += BtnDeleteAll_OnClickEventHandler;
        
        m_shipBuildType = ShipBuildType.BuildRoom;
        RefreshTrapUI(AttributeType.ALL);
        RefreshSoldierUI(CombatLoactionType.ALL);
        RefreshBlackScienceUI();
        CheckCaptionCount();
        
        EventDelegate.Add(MyHead.TogTrap.onChange, TogTrap_OnClickEventHandler);
        EventDelegate.Add(MyHead.TogSoldier.onChange, TogSoldier_OnClickEventHandler);
    }
    
    public void SetCaption(int id)
    {
        m_iCurCaptainID = id;
        CaptionInfo c = BlackScienceDC.GetCaption(id);
        ShipPlan P = ShipPlanDC.GetCurShipPlan();
        P.BlackScienceID = c.m_id;
        SetBlackScienceUI(c);
    }
    /// <summary>
    /// 检测船长技能是否存在，没有则隐藏
    /// </summary>
    void CheckCaptionCount()
    {
        if (BlackScienceDC.CheckHaveCaption() == false) {
            //            MyHead.BtnBlackScience.transform.parent.gameObject.SetActive(false);
            MyHead.SprCaptainHead.gameObject.SetActive(false);
        }
    }
    
    void RefreshBlackScienceUI()
    {
        ShipPlan P = ShipPlanDC.GetCurShipPlan();
        CaptionInfo captionInfo = BlackScienceDC.GetCaptionD(P.BlackScienceID);
        if (captionInfo != null) {
            SetBlackScienceUI(captionInfo);
        } else { //未设置黑科技
            NGUIUtil.Set2DSprite(MyHead.SprCaptainHead, "Textures/role/", "-999");
            NGUIUtil.SetLableText<string>(MyHead.LblSkillName, "");
        }
    }
    
    void SetBlackScienceUI(CaptionInfo c)
    {
        NGUIUtil.Set2DSprite(MyHead.SprCaptainHead, "Textures/role/", c.m_captionid.ToString());
        NGUIUtil.SetLableText<string>(MyHead.LblSkillName, c.GetGodSkillInfo().m_name);
    }
    /// <summary>
    /// 黑科技
    /// </summary>
    void BtnBlackScience_OnClickEventHandler(UIButton sender)
    {
        if (BlackScienceDC.CheckHaveCaption() == false) {
            NGUIUtil.ShowTipWndByKey(30000050);
            return;
        }
        BlackScienceChoWnd bsWnd = WndManager.GetDialog<BlackScienceChoWnd>();
        ShipPlan P = ShipPlanDC.GetCurShipPlan();
        CaptionInfo captionInfo = BlackScienceDC.GetCaptionD(P.BlackScienceID);
        bsWnd.SetSelectCaptain(captionInfo);
    }
    /// <summary>
    ///  一键解体（全部清除）
    /// </summary>
    void BtnDeleteAll_OnClickEventHandler(UIButton sender)
    {
        DialogWnd dialogWnd = WndManager.GetDialog<DialogWnd>();
        if (dialogWnd) {
            dialogWnd.SetDialogLable(NGUIUtil.GetStringByKey("30000039"), NGUIUtil.GetStringByKey("88800063"), NGUIUtil.GetStringByKey("88800064"));
            dialogWnd.YESButtonOnClick = YesReAuth;
            dialogWnd.ShowDialog();
        }
    }
    
    private void YesReAuth(UIButton sender)
    {
        TouchMoveManager.DeleteAll();
        PutCanvasM.ShowRoomGridUI(false);
    }
    
    void BtnShow_OnClickEventHandler(UIButton sender)
    {
        PutCanvasM.PutDownNewBuild();
        PutCanvasM.ShowRoomGridUI(false);
        ClickMenuBtn();
    }
    
    private void CreateList()
    {
        if (MyHead.ListParent == null) {
            NGUIUtil.DebugLog("ShipCanvasDialogWnd ListParent!!!");
            return;
        }
        
        //先删除
        foreach (CanvasItem item in m_lItems) {
            if (item != null) {
                GameObject.DestroyImmediate(item.gameObject);
            }
        };
        m_lItems.Clear();
        //重新添加。
        if (m_shipBuildType == ShipBuildType.BuildRoom) {
            foreach (var a in warehouseBuildList) {
                if (TouchMoveManager.CheckHaveExist(m_shipBuildType, a.ID) == false) {
                    AddBuild(a);
                }
            }
        }
        if (m_shipBuildType == ShipBuildType.Soldier) {
            foreach (var a in soldierList) {
                if (TouchMoveManager.CheckHaveExist(m_shipBuildType, a.ID) == false) {
                    AddSoldier(a);
                }
            }
        }
        NGUIUtil.RepositionTable(MyHead.ListParent);
    }
    
    private void AddBuild(BuildInfo WareHouse)
    {
        //判断是否已有该类型的建筑
        foreach (CanvasItem item in m_lItems) {
            if (item != null) {
                // 已有
                if (item.Equalof(WareHouse) == true) {
                    item.SetCanvasItem(WareHouse);
                    item.ShowBearDestroy(true);
                    return ;
                }
            }
        };
        //新的建筑
        GameObject go = NDLoad.LoadWndItem("CanvasItem", MyHead.ListParent.transform);
        if (go) {
            CanvasItem item = go.GetComponent<CanvasItem>();
            if (item) {
                m_lItems.Add(item);
                item.SetCanvasItem(WareHouse);
                item.ShowBearDestroy(true);
            }
        }
    }
    
    private void AddSoldier(SoldierInfo Soldier)
    {
        GameObject go = NDLoad.LoadWndItem("CanvasItem", MyHead.ListParent.transform);
        if (go) {
            CanvasItem item = go.GetComponent<CanvasItem>();
            if (item) {
                m_lItems.Add(item);
                item.SetCanvasItem(Soldier);
                item.ShowBearDestroy(true);
            }
        }
    }
    
    public void TogSoldier_OnClickEventHandler()
    {
        if (!MyHead.TogSoldier.value) {
            return;
        }
        if (m_bTrap == false) {
            return;
        }
        m_bTrap = false;
        ClickTrapSoldierBtn();
    }
    
    public void TogTrap_OnClickEventHandler()
    {
        if (!MyHead.TogTrap.value) {
            return;
        }
        if (m_bTrap == true) {
            return;
        }
        m_bTrap = true;
        ClickTrapSoldierBtn();
    }
    
    void ClickTrapSoldierBtn()
    {
        if (m_bTrap) {
            m_shipBuildType = ShipBuildType.BuildRoom;
        } else {
            m_shipBuildType = ShipBuildType.Soldier;
        }
        CreateList();
    }
    
    void BtnHide_OnClickEventHandler(UIButton sender)
    {
        ClickMenuBtn();
    }
    TweenPosition tw;
    /// <summary>
    /// 点击主菜单处理
    /// </summary>
    public void ClickMenuBtn()
    {
        if (m_bOpen == false) { //打开主菜单
            //NGUIUtil.DebugLog("open "+Time.time);
            MainCameraM.s_Instance.EnableDrag(false);
            m_bOpen = true;
            MyHead.BtnShow.gameObject.SetActive(false);
            MyHead.BtnHide.gameObject.SetActive(true);
            //NGUIUtil.TweenGameObjectPosY(TweenGo, m_fMinHeight, m_fMaxHeight, 0.2f, 0,gameObject,"RepositionTable");
            Vector3 pos = MyHead.TweenGo.transform.localPosition;
            pos.y = m_fMaxHeight;
            TweenPosition.Begin(MyHead.TweenGo, 0.2f, pos, false);
        } else {
            m_bOpen = false;
            MyHead.BtnHide.gameObject.SetActive(false);
            //NGUIUtil.TweenGameObjectPosY(TweenGo, m_fMaxHeight, m_fMinHeight, 0.2f,0, gameObject, "ShowBtnShow");
            Vector3 pos = MyHead.TweenGo.transform.localPosition;
            pos.y = m_fMinHeight - 300;
            tw = TweenPosition.Begin(MyHead.TweenGo, 0.2f, pos, false);
            tw.AddOnFinished(ShowBtnShow);
        }
    }
    /// <summary>
    /// 关闭主菜单，供外部调用
    /// </summary>
    public void CloseMenu()
    {
        if (m_bOpen) {
            ClickMenuBtn();
        }
    }
    /// <summary>
    /// 打开主菜单
    /// </summary>
    public void OpenMenu()
    {
        if (m_bOpen == false) {
            ClickMenuBtn();
        }
    }
    
    /// <summary>
    /// 关闭主菜单动画执行完成后的回调
    /// </summary>
    public void ShowBtnShow()
    {
        if (m_bOpen) {
            return;
        }
        MyHead.BtnShow.gameObject.SetActive(true);
    }
    
    public void RefreshTrapUI(AttributeType type = AttributeType.ALL)
    {
        FillTrapData(type);
        CreateList();
    }
    public void RefreshSoldierUI(CombatLoactionType type)
    {
        FillSoldierData(type);
        CreateList();
    }
    
    /// <summary>
    /// 填充陷阱和兵数据
    /// </summary>
    private void FillTrapData(AttributeType type)
    {
        warehouseBuildList = BuildDC.GetBuildingList(type);
    }
    
    private void FillSoldierData(CombatLoactionType type)
    {
        SoldierDC.GetSoldiers(ref soldierList, type);
    }
    
    public void RepositionUI()
    {
        NGUIUtil.RepositionTable(MyHead.ListParent);
    }
    
    void Update()
    {
        if (m_bRunOnce) {
            m_bRunOnce = false;
            m_fMinHeight = MyHead.TweenGo.transform.localPosition.y;
            m_fMaxHeight = m_fMinHeight;//适配菜单移动位置
        }
    }
}
