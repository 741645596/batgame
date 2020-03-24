using UnityEngine;
using System.Collections;
using System.Collections.Generic ;

public enum TrapState {
    /// <summary>
    /// The exit.
    /// </summary>
    Exit,
    CanSum,
    CanNotSum,
}
public class TrapViewListWnd : WndBase
{
    public TrapViewListWnd_h MyHead {
        get
        {
            return (base.BaseHead() as TrapViewListWnd_h);
        }
    }
    /// <summary>
    /// 玩家存在陷阱.
    /// </summary>
    public List<BuildInfo> m_Build = new List<BuildInfo>();
    public List<BuildInfo> m_CanSumBuild = new List<BuildInfo>();
    public List<BuildInfo> m_CannotSumBuild = new List<BuildInfo>();
    
    private AttributeType m_iBtnTrapTypeIndex = AttributeType.ALL;
    
    private TrapViewItem m_canShengji = null;
    private bool m_bGuide = false;
    private bool m_bRepositionFinish = false;
    
    void Awake()
    {
        //自动关联Toogle
        MyHead.ToggleList = U3DUtil.GetComponentsInChildren<Transform>(MyHead.ToggleParent.gameObject, false);
        EventDelegate.Add(MyHead.BtnTrapsAll.onChange, BtnTrapAll_OnClickEventHandler);
        EventDelegate.Add(MyHead.BtnTrapsFire.onChange, BtnTrapFire_OnClickEventHandler);
        EventDelegate.Add(MyHead.BtnTrapsWater.onChange, BtnTrapWarter_OnClickEventHandler);
        EventDelegate.Add(MyHead.BtnTrapsPosion.onChange, BtnTrapPoison_OnClickEventHandler);
        EventDelegate.Add(MyHead.BtnTrapsGas.onChange, BtnTrapGas_OnClickEventHandler);
        EventDelegate.Add(MyHead.BtnTrapsThur.onChange, BtnTrapThur_OnClickEventHandler);
        //m_canShengji = null;
    }
    public override void WndStart()
    {
        base.WndStart();
        //初始化的时候默认选择全部，全部的索引是5
        if (MyHead.BtnReturn) {
            MyHead.BtnReturn.OnClickEventHandler += BtnClose_OnClickEventHandler;
        }
        
        DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_908, UpdataTrapList_Resp);
        
        
        NGUIUtil.UpdatePanelValue(MyHead.PanelMask, 1024, 0.15f);
        ShowTogglesAni(0.05f, 0.05f);
        NGUIUtil.TweenGameObjectPosX(MyHead.ScrollLeft, -461, 0.15f, gameObject, "HideAniScroll");
        NGUIUtil.TweenGameObjectPosX(MyHead.ScrollRight, 461, 0.15f);
        NGUIUtil.SetMainMenuTop(this);
        UpdateData();
        
    }
    
    /// <summary>
    /// 隐藏左右两个动画轴
    /// </summary>
    public void HideAniScroll()
    {
        NGUIUtil.SetActive(MyHead.ScrollLeft, false);
        NGUIUtil.SetActive(MyHead.ScrollRight, false);
        NGUIUtil.SetPanelClipping(MyHead.PanelMask, UIDrawCall.Clipping.None);
        if (m_bGuide) {
            return;
        }
        
        List<ItemTypeInfo> litem = ItemDC.SearchItemListBystype(44);
        if (litem.Count > 0) {
            ItemUseConfirmWnd wnd = WndManager.GetDialog<ItemUseConfirmWnd>();
            wnd.SetData(44, litem);
        }
    }
    
    private void SetUI()
    {
        CreateWhenSelectChange(m_iBtnTrapTypeIndex);
    }
    
    /// <summary>
    /// 设置建筑数据
    /// </summary>
    public void UpdateData()
    {
        m_Build.Clear();
        m_CannotSumBuild.Clear();
        m_CanSumBuild.Clear();
        List<BuildInfo> s_allBuild = buildingM.GetAllBuildInfo();
        foreach (BuildInfo item in s_allBuild) {
            if (item.BuildType == 1201 || item.BuildType >= 9990) {
                continue ;
            }
            
            BuildInfo exitInfo = BuildDC.SearchBuilding(item.BuildType);
            if (exitInfo == null) {
                bool canSum = buildingM.CheckCanSummon(item.BuildType);
                if (canSum) {
                    m_CanSumBuild.Add(item);
                } else {
                    m_CannotSumBuild.Add(item);
                }
            }
        }
        List<BuildInfo> lWareInfo = BuildDC.GetBuildingList(AttributeType.ALL);
        foreach (BuildInfo Ware in lWareInfo) {
            if (Ware == null || Ware.BuildType == 1201) {
                continue ;
            }
            m_Build.Add(Ware);
        }
        
        BuildDC.SortBuildList(ref m_Build);
        BuildDC.SortBuildList(ref m_CanSumBuild);
        BuildDC.SortCanNotExitBuildList(ref m_CannotSumBuild);
        //更新UI
        SetUI();
    }
    
    /// <summary>
    /// 对建筑数据进行分类
    /// </summary>
    private List<BuildInfo> GetBuildInfo(AttributeType AType, TrapState trapState)
    {
        List<BuildInfo> l = new List<BuildInfo>();
        if (trapState == TrapState.Exit) {
            foreach (BuildInfo Info in m_Build) {
                if (Info == null) {
                    continue ;
                }
                if (AType == AttributeType.ALL) {
                    l.Add(Info);
                }
                AttributeType bAttr = SkillM.GetBuildAttributeType(Info.m_RoomKind);
                if (AType == bAttr) {
                    l.Add(Info);
                }
            }
        } else if (trapState == TrapState.CanSum) {
            foreach (BuildInfo Info in m_CanSumBuild) {
                if (Info == null) {
                    continue ;
                }
                if (AType == AttributeType.ALL) {
                    l.Add(Info);
                }
                AttributeType bAttr = SkillM.GetBuildAttributeType(Info.m_RoomKind);
                if (AType == bAttr) {
                    l.Add(Info);
                }
            }
        } else if (trapState == TrapState.CanNotSum) {
            foreach (BuildInfo Info in m_CannotSumBuild) {
                if (Info == null) {
                    continue ;
                }
                if (AType == AttributeType.ALL) {
                    l.Add(Info);
                }
                AttributeType bAttr = SkillM.GetBuildAttributeType(Info.m_RoomKind);
                if (AType == bAttr) {
                    l.Add(Info);
                }
            }
        }
        
        return l ;
    }
    //	public List<BuildInfo> GetBuildInfo()
    //	{
    //		return GetBuildInfo (m_iBtnTrapTypeIndex);
    //	}
    private void CreateBanner()
    {
        GameObject go = NDLoad.LoadWndItem("PdbbbItem", MyHead.Parent.transform);
        PdbbbItem Pitem = go.GetComponent<PdbbbItem>();
        if (Pitem != null && Pitem.MyHead.Table != null) {
            GameObject Banner = NDLoad.LoadWndItem("PdbbbBanner", Pitem.MyHead.Table.transform);
            PdbbbBanner banner = Banner.GetComponent<PdbbbBanner>();
            if (banner != null) {
                banner.SetLabelText(NGUIUtil.GetStringByKey(10000193));
            }
            
            Pitem.MyHead.Table.Reposition();
            Pitem.MyHead.Table.repositionNow = true;
            UIPanel panel = MyHead.Parent.transform.parent.gameObject.GetComponent<UIPanel>();
            float x = 150f;
            if (panel != null) {
                UISprite spr = Pitem.GetComponentInChildren<UISprite>();
                if (spr != null) {
                    x = (panel.width - spr.width) / 2 - 20f;
                }
            }
            Pitem.MyHead.Table.transform.localPosition = new Vector3(x, Pitem.MyHead.Table.transform.localPosition.y, Pitem.MyHead.Table.transform.localPosition.z);
        }
    }
    
    /// <summary>
    /// 创建已召唤炮弹兵
    /// </summary>
    private void CreateTrap(List<BuildInfo> BuildList, TrapState trapState)
    {
        if (trapState == TrapState.Exit) {
            m_canShengji = null;
        }
        if (BuildList == null || BuildList.Count == 0) {
            return;
        }
        if (MyHead.Parent == null) {
            NGUIUtil.DebugLog("ShipCanvasDialogWnd ListParent!!!");
            return;
        }
        
        if (trapState == TrapState.CanNotSum) {
            CreateBanner();
        }
        
        PdbbbItem pbbItem = null;
        int count = 0;
        
        for (int i = 0; i < BuildList.Count; i++) {
            if (count % 4 == 0) {
                pbbItem = null;
                GameObject go = NDLoad.LoadWndItem("PdbbbItem", MyHead.Parent.transform);
                pbbItem = go.GetComponent<PdbbbItem>();
            }
            count ++;
            if (pbbItem != null && pbbItem.MyHead.Table != null) {
                GameObject viewgo = NDLoad.LoadWndItem("TrapViewItem", pbbItem.MyHead.Table.transform);
                if (viewgo != null) {
                    TrapViewItem item = viewgo.GetComponent<TrapViewItem>();
                    if (item != null) {
                        item.SetBuildInfo(BuildList[i], this, trapState);
                        if (null == m_canShengji && trapState == TrapState.Exit) {
                            CanLevelResult LevResult = buildingM.GetLevelCanUP(BuildList[i]);
                            if (LevResult == CanLevelResult.CanUp) {
                                m_canShengji = item;
                            }
                        }
                    }
                }
            }
            
        }
    }
    public void BtnTrapAll_OnClickEventHandler()
    {
        if (!MyHead.BtnTrapsAll.value) {
            return;
        }
        m_iBtnTrapTypeIndex = AttributeType.ALL;
        CreateWhenSelectChange(m_iBtnTrapTypeIndex);
    }
    public void BtnTrapFire_OnClickEventHandler()
    {
        if (!MyHead.BtnTrapsFire.value) {
            return;
        }
        m_iBtnTrapTypeIndex = AttributeType.Fire;
        CreateWhenSelectChange(m_iBtnTrapTypeIndex);
    }
    public void BtnTrapWarter_OnClickEventHandler()
    {
        if (!MyHead.BtnTrapsWater.value) {
            return;
        }
        m_iBtnTrapTypeIndex = AttributeType.Water;
        CreateWhenSelectChange(m_iBtnTrapTypeIndex);
    }
    public void BtnTrapPoison_OnClickEventHandler()
    {
        if (!MyHead.BtnTrapsPosion.value) {
            return;
        }
        m_iBtnTrapTypeIndex = AttributeType.Poison;
        CreateWhenSelectChange(m_iBtnTrapTypeIndex);
    }
    public void BtnTrapThur_OnClickEventHandler()
    {
        if (!MyHead.BtnTrapsThur.value) {
            return;
        }
        m_iBtnTrapTypeIndex = AttributeType.Electric;
        CreateWhenSelectChange(m_iBtnTrapTypeIndex);
    }
    public void BtnTrapGas_OnClickEventHandler()
    {
        if (!MyHead.BtnTrapsGas.value) {
            return;
        }
        m_iBtnTrapTypeIndex = AttributeType.Gas;
        CreateWhenSelectChange(m_iBtnTrapTypeIndex);
    }
    
    private void CreateWhenSelectChange(AttributeType type)
    {
        m_iBtnTrapTypeIndex = type;
        
        List<BuildInfo> BuildList = new List<BuildInfo> ();
        List<BuildInfo> CanExitBuildList = new List<BuildInfo> ();
        List<BuildInfo> CanNotExitBuildList = new List<BuildInfo> ();
        switch (m_iBtnTrapTypeIndex) {
            case AttributeType.Gas:
                BuildList = GetBuildInfo(AttributeType.Gas, TrapState.Exit);
                CanExitBuildList = GetBuildInfo(AttributeType.Gas, TrapState.CanSum);
                CanNotExitBuildList = GetBuildInfo(AttributeType.Gas, TrapState.CanNotSum);
                break;
            case AttributeType.Electric:
                BuildList = GetBuildInfo(AttributeType.Electric, TrapState.Exit);
                CanExitBuildList = GetBuildInfo(AttributeType.Electric, TrapState.CanSum);
                CanNotExitBuildList = GetBuildInfo(AttributeType.Electric, TrapState.CanNotSum);
                break;
            case AttributeType.Poison:
                BuildList = GetBuildInfo(AttributeType.Poison, TrapState.Exit);
                CanExitBuildList = GetBuildInfo(AttributeType.Poison, TrapState.CanSum);
                CanNotExitBuildList = GetBuildInfo(AttributeType.Poison, TrapState.CanNotSum);
                break;
            case AttributeType.Water:
                BuildList = GetBuildInfo(AttributeType.Water, TrapState.Exit);
                CanExitBuildList = GetBuildInfo(AttributeType.Water, TrapState.CanSum);
                CanNotExitBuildList = GetBuildInfo(AttributeType.Water, TrapState.CanNotSum);
                break;
            case AttributeType.Fire:
                BuildList = GetBuildInfo(AttributeType.Fire, TrapState.Exit);
                CanExitBuildList = GetBuildInfo(AttributeType.Fire, TrapState.CanSum);
                CanNotExitBuildList = GetBuildInfo(AttributeType.Fire, TrapState.CanNotSum);
                break;
            case AttributeType.ALL:
                BuildList = m_Build;
                CanExitBuildList = m_CanSumBuild;
                CanNotExitBuildList = m_CannotSumBuild;
                break;
        }
        
        U3DUtil.DestroyAllChild(MyHead.Parent);
        
        CreateTrap(CanExitBuildList, TrapState.CanSum);
        CreateTrap(BuildList, TrapState.Exit);
        CreateTrap(CanNotExitBuildList, TrapState.CanNotSum);
        
        SetTrapCount(BuildList.Count);
        NGUIUtil.RepositionTable(MyHead.Parent);
        MyHead.Anchor.enabled = true;
        StartCoroutine(RepositionFinish(2));
        //		MyHead.Anchor.runOnlyOnce = true;
    }
    
    IEnumerator RepositionFinish(int frameCount)
    {
        yield return StartCoroutine(U3DUtil.WaitForFrames(frameCount));
        m_bRepositionFinish = true;
    }
    
    private void SetTrapCount(int count)
    {
        NGUIUtil.SetLableText<string>(MyHead.LblCount, string.Format("{0}", count));
    }
    private void BtnClose_OnClickEventHandler(UIButton sender)
    {
        WndManager.DestoryDialog<TrapViewListWnd>();
        WndManager.ShowAllWnds(true);
        MainCameraM.s_Instance.EnableDrag(true);
    }
    
    
    //卷轴动画.
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
    
    
    public void OnDestroy()
    {
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_908, UpdataTrapList_Resp);
    }
    
    /// <summary>
    /// 906 升星
    /// </summary>
    /// <param name="nErrorCode"></param>
    void UpdataTrapList_Resp(int nErrorCode)
    {
        if (nErrorCode == 0) {
            UpdateData();
        }
        
    }
}
