using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BeiBaoWnd : WndBase
{

    public BeiBaoWnd_h MyHead {
        get
        {
            return (base.BaseHead() as BeiBaoWnd_h);
        }
    }
    
    /// <summary>
    /// 当前选定 物品类型.
    /// </summary>
    private ItemType m_itemType;
    private int m_iNumCount = 0;
    
    private List<ItemTypeInfo> m_listAllTypeInfo = new List<ItemTypeInfo> ();
    private GameObject m_SeletItem;
    /// <summary>
    /// 默认选中第一个
    /// </summary>
    private int nSelectIndex = -1;
    
    private HUDText m_hudText;
    private GameObject GoHudText = null;
    
    public override void WndStart()
    {
        base.WndStart();
        NGUIUtil.SetMainMenuTop(this);
        m_itemType = ItemType.All;
        m_listAllTypeInfo =  ItemDC.GetItemList(ItemType.All);
        
        BindEvents();
        SetUI();
        if (MainCameraM.s_Instance) {
            MainCameraM.s_Instance.EnableDrag(false);
        }
        RegisterHooks();
        
        NGUIUtil.UpdatePanelValue(MyHead.PanelMask, 1024, 0.15f);
        ShowTogglesAni(0.05f, 0.05f);
        NGUIUtil.TweenGameObjectPosX(MyHead.ScrollLeft, -461, 0.15f, gameObject, "HideAniScroll");
        NGUIUtil.TweenGameObjectPosX(MyHead.ScrollRight, 461, 0.15f);
        InitHudText();
    }
    
    private void InitHudText()
    {
        GoHudText = NDLoad.LoadWndItem("AddExpHUDText", MyHead.Target.transform);
        GoHudText.gameObject.name = "GetWood";
        GameObject child = NGUITools.AddChild(WndManager.GetWndRoot(), GoHudText);
        if (GoHudText != null) {
            m_hudText = child.GetComponentInChildren<HUDText>();
            m_hudText.bFollowTarget = true;
            child.AddComponent<UIFollowTarget>().target = MyHead.Target.transform;
            child.GetComponent<UIFollowTarget>().disableIfInvisible = false;
            m_hudText.fontSize = 30;
        }
    }
    /// <summary>
    /// 隐藏左右两个动画轴.
    /// </summary>
    public void HideAniScroll()
    {
        NGUIUtil.SetActive(MyHead.ScrollLeft, false);
        NGUIUtil.SetActive(MyHead.ScrollRight, false);
    }
    private void SetUI()
    {
        CreateAllItemsList();
        
        if (nSelectIndex >= 0) {
            AddIntorductUI(nSelectIndex);
        }
        
    }
    
    public void ReSetUI()
    {
        m_listAllTypeInfo = ItemDC.GetItemList(m_itemType);
        
        CreateItems();
        //最后一个卖掉，显示其上一个内容.
        if (nSelectIndex == m_listAllTypeInfo.Count) {
            nSelectIndex = m_listAllTypeInfo.Count - 1;
        }
        
        if (nSelectIndex >= 0 && nSelectIndex < m_listAllTypeInfo.Count) {
            ItemTypeInfo info = m_listAllTypeInfo[nSelectIndex];
            if (info != null && info.Num > 0) {
                RefreshIntorduct(info);
                return;
            }
        }
        
        ItemTypeInfo defaultInfo =  GetDefaultInfo();
        if (defaultInfo == null) {
            U3DUtil.DestroyAllChild(MyHead.ItemParent);
            return;
        }
        RefreshIntorduct(defaultInfo);
        
    }
    /// <summary>
    /// 根据选取的背包类型创建物品.
    /// </summary>
    private void CreateItems()
    {
        //		nSelectIndex = -1;
        U3DUtil.DestroyAllChild(MyHead.Parent);
        CreateAllItemsList();//临时 等待其他物品数据和资源
        NGUIUtil.RepositionTable(MyHead.Parent);
        
    }
    
    private void CreateAllItemsList()
    {
        m_iNumCount = 0;
        for (int i = 0; i < m_listAllTypeInfo.Count; i++) {
            ItemTypeInfo item = m_listAllTypeInfo[i];
            if (item.Num < 1) {
                continue;
            }
            GameObject go = NGUITools.AddChild(MyHead.Parent, MyHead.BeiBaoItem);
            if (go) {
                BeiBaoItem beiBaoItem = go.GetComponent<BeiBaoItem>();
                if (beiBaoItem) {
                    beiBaoItem.SetData(item, item.Num, this);
                    //                     NGUIUtil.SetItemPanelDepth(go, MyHead.Parent.transform.parent.gameObject);
                }
                if (m_iNumCount == 0 && nSelectIndex == -1) {
                    nSelectIndex = i;
                    beiBaoItem.MyHead.TogSelect.value = true;
                } else if (nSelectIndex == i) {
                    beiBaoItem.MyHead.TogSelect.value = true;
                }
            }
            
            m_iNumCount ++;
        }
        
        
    }
    private ItemTypeInfo GetDefaultInfo()
    {
        foreach (ItemTypeInfo item in m_listAllTypeInfo) {
            if (m_itemType != ItemType.All) {
                if (item.m_type != m_itemType || item.Num < 1) {
                    continue;
                }
            }
            return item;
        }
        return null;
    }
    public void ItemClickCallBack(ItemTypeInfo info)
    {
        int index = m_listAllTypeInfo.IndexOf(info);
        
        if (index >= 0 && index < m_listAllTypeInfo.Count) {
            nSelectIndex = index;
            RefreshIntorduct(info);
        }
    }
    public override void BindEvents()
    {
        if (MyHead.BtnClose) {
            MyHead.BtnClose.OnClickEventHandler += BtnClose_OnClickEventHandler;
        }
        EventDelegate.Add(MyHead.BtnQuanBu.onChange, BtnQuanBu_OnClickEventHandler);
        EventDelegate.Add(MyHead.BtnZhuangBei.onChange, BtnZhuangBei_OnClickEventHandler);
        EventDelegate.Add(MyHead.BtnJuanZhou.onChange, BtnJuanZhou_OnClickEventHandler);
        EventDelegate.Add(MyHead.BtnLingHunShi.onChange, BtnLingHunShi_OnClickEventHandler);
        EventDelegate.Add(MyHead.BtnXiaoHaoPin.onChange, BtnXiaoHaoPin_OnClickEventHandler);
        EventDelegate.Add(MyHead.BtnLingJian.onChange, BtnBtnLingJian_OnClickEventHandler);
        
        
    }
    void BtnClose_OnClickEventHandler(UIButton sender)
    {
        GameObjectActionWait gad = new GameObjectActionWait(ConstantData.fPopDownAniTime);
        gad.m_complete = DestoryDialogCallBack;
        WndEffects.PlayWndAnimation(gameObject, gad, "popupEnd");
    }
    public static void DestoryDialogCallBack(object g)
    {
        WndManager.DestoryDialog<BeiBaoWnd>();
        WndManager.DestoryDialog<ItemInfoWnd>();
        //		WndManager.ShowAllWnds(true);
        MainCameraM.s_Instance.EnableDrag(true);
    }
    /// <summary>
    /// 零件.
    /// </summary>
    public void BtnBtnLingJian_OnClickEventHandler()
    {
        if (!MyHead.BtnLingJian.value) {
            return;
        }
        m_itemType = ItemType.Lingjian;
        nSelectIndex = -1;
        ReSetUI();
    }
    /// <summary>
    /// 消耗品.
    /// </summary>
    public void BtnXiaoHaoPin_OnClickEventHandler()
    {
        if (!MyHead.BtnXiaoHaoPin.value) {
            return;
        }
        m_itemType = ItemType.XiaoHaoPin;
        nSelectIndex = -1;
        ReSetUI();
    }
    /// <summary>
    /// 灵魂石.
    /// </summary>
    public void BtnLingHunShi_OnClickEventHandler()
    {
        if (!MyHead.BtnLingHunShi.value) {
            return;
        }
        m_itemType = ItemType.LingHunShi;
        nSelectIndex = -1;
        ReSetUI();
    }
    /// <summary>
    /// 卷轴.
    /// </summary>
    public void BtnJuanZhou_OnClickEventHandler()
    {
        if (!MyHead.BtnJuanZhou.value) {
            return;
        }
        m_itemType = ItemType.JuanZhou;
        nSelectIndex = -1;
        ReSetUI();
    }
    /// <summary>
    /// 装备.
    /// </summary>
    public void BtnZhuangBei_OnClickEventHandler()
    {
        if (!MyHead.BtnZhuangBei.value) {
            return;
        }
        m_itemType = ItemType.ZhuangBei;
        nSelectIndex = -1;
        ReSetUI();
    }
    /// <summary>
    /// 全部.
    /// </summary>
    public void BtnQuanBu_OnClickEventHandler()
    {
        if (!MyHead.BtnQuanBu.value) {
            return;
        }
        m_itemType = ItemType.All;
        nSelectIndex = -1;
        ReSetUI();
    }
    
    void ShowItemsByType(ItemType type)
    {
    
    }
    
    private void RefreshIntorduct(ItemTypeInfo info)
    {
        if (info == null) {
            return;
        }
        
        BeiBaoItemIntorduct item = gameObject.GetComponentInChildren<BeiBaoItemIntorduct>();
        if (item != null) {
            item.SetData(info, this);
            item.RefreshUI();
        } else {
            int index = m_listAllTypeInfo.IndexOf(info);
            if (index >= 0 && index < m_listAllTypeInfo.Count) {
                AddIntorductUI(index);
            }
            
        }
    }
    
    private void AddIntorductUI(int iSelectIndex)
    {
        if (iSelectIndex < 0 || iSelectIndex >= m_listAllTypeInfo.Count) {
            return;
        }
        
        ItemTypeInfo info = m_listAllTypeInfo[iSelectIndex];
        GameObject go = NDLoad.LoadWndItem("BeiBaoItemIntorduct", MyHead.ItemParent.transform);
        if (go != null) {
            BeiBaoItemIntorduct item = go.GetComponent<BeiBaoItemIntorduct>();
            if (item != null) {
                item.SetData(info, this);
            }
            
        }
    }
    public void AddHudText(string text)
    {
        if (m_hudText != null) {
            m_hudText.Add(text, new Color(1.0f, 1.0f, 98.0f / 255.0f, 1.0f), 0.5f);
        }
    }
    
    
    public void ItemUsed()
    {
        m_listAllTypeInfo =  ItemDC.GetItemList(ItemType.All);
        ReSetUI();
    }
    void Recv_ItemUseRespone(int iErrorCode)
    {
        //success
        if (iErrorCode == 0) {
            if (nSelectIndex >= 0 && nSelectIndex < m_listAllTypeInfo.Count) {
                ItemTypeInfo info = m_listAllTypeInfo[nSelectIndex];
                if (info.m_func == "Item_AddWood") {
                    Debug.Log("addwood" + info.m_args);
                    string show = string.Format(NGUIUtil.GetStringByKey(10000179), NGUIUtil.GetStringByKey(10000178)) + info.m_args.ToString();
                    AddHudText(show);
                } else if (info.m_func == "Item_AddCrystal") {
                    Debug.Log("AddCrystal" + info.m_args);
                    string show = string.Format(NGUIUtil.GetStringByKey(10000179), NGUIUtil.GetStringByKey(10000177)) + info.m_args.ToString();
                    AddHudText(show);
                }
            }
            
            ItemUsed();
        }
    }
    
    public void OnDestroy()
    {
        AntiRegisterHooks();
    }
    
    /// <summary>
    /// 注册事件
    /// </summary>
    public   void RegisterHooks()
    {
        DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_804, Recv_ItemUseRespone);
    }
    
    /// <summary>
    /// 反注册事件
    /// </summary>
    public   void AntiRegisterHooks()
    {
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_804, Recv_ItemUseRespone);
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
    /// <summary>
    ///  是否为全屏窗口
    /// </summary>
    public override bool IsFullWnd()
    {
        return true ;
    }
}
