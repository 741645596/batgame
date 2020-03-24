using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using sdata;
/// <summary>
/// 装备合成
/// <Author>QFord</Author>
/// </summary>
public class EquipComposeWnd : WndBase
{

    public EquipComposeWnd_h MyHead {
        get
        {
            return (base.BaseHead() as EquipComposeWnd_h);
        }
    }
    
    private int m_itemTypeID;
    /// <summary>
    /// 装备合成所需金币
    /// </summary>
    private int m_coin;
    private Vector3 m_vInitPos;
    /// <summary>
    /// 用来合成装备的物品列表
    /// </summary>
    private List<KeyValue> m_composeEquipList = new List<KeyValue>();
    private List<GameObject> EquipItemList = new List<GameObject>();
    
    private List<ECEquipComposeItem> m_lCurEquipItems = new List<ECEquipComposeItem>();
    
    public override void WndStart()
    {
        base.WndStart();
        TweenRight();
        DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_808, EquipComposeResponse);
        
        MyHead.BtnCompose.OnClickEventHandler += BtnCompose_OnClickEventHandler;
        MyHead.BtnWndBg.OnClickEventHandler += BtnWndBg_OnClickEventHandler;
        MyHead.BtnRootEquip.OnClickEventHandler += BtnRootEquip_OnClickEventHandler;
    }
    
    private void EquipComposeResponse(int nErrorCode)
    {
        if (nErrorCode == 0) {
            NGUIUtil.ShowTipWndByKey(30000036);
            RefreshEquipItems();
            PdbycWnd wnd1 = WndManager.FindDialog<PdbycWnd>();
            if (wnd1) {
                wnd1.RefreshEquipItems();
            }
            EquipmentInfoWnd wnd2 = WndManager.FindDialog<EquipmentInfoWnd>();
            if (wnd2) {
                wnd2.RefreshUI();
            }
            
            WndManager.DestoryDialog<EquipComposeWnd>();
        }
        
        EquipmentInfoWnd wnd = WndManager.FindDialog<EquipmentInfoWnd>();
        if (wnd) {
            wnd.CheckEquipExist();
            wnd.TweenReset();
        }
        
    }
    
    void BtnRootEquip_OnClickEventHandler(UIButton sender)
    {
        U3DUtil.DestroyAllChild(MyHead.ComposeMaterialsParent, true);
        U3DUtil.DestroyAllChild(MyHead.ComposeEquipsParent, true);
    }
    
    void BtnWndBg_OnClickEventHandler(UIButton sender)
    {
        WndManager.DestoryDialog<EquipComposeWnd>();
    }
    
    void BtnCompose_OnClickEventHandler(UIButton sender)
    {
        if (UserDC.GetCoin() < m_coin) {
            NGUIUtil.ShowTipWndByKey("88800108");
            return;
        }
        
        if (ItemM.CheckEquipCompose(m_itemTypeID)) {
            ItemDC.SendEquipComposeRequest(m_itemTypeID);
        } else {
            NGUIUtil.ShowTipWndByKey("88800110");
        }
    }
    
    public  void SetData(int ItemTypeID)
    {
        ClearUI();
        m_vInitPos = MyHead.WndTweenParent.transform.localPosition;
        m_itemTypeID = ItemTypeID;
        ItemM.GetCombineEquipNeed(m_itemTypeID, ref m_composeEquipList, ref m_coin);
        SetUI();
    }
    
    private void ClearUI()
    {
        U3DUtil.DestroyAllChild(MyHead.EquipMidTable.gameObject, true);
        
    }
    
    private void SetUI()
    {
        //设置要合成装备的名称和所需的金币
        s_itemtypeInfo info = ItemM.GetItemInfo(m_itemTypeID);
        if (info != null) {
            NGUIUtil.SetLableText<string>(MyHead.LblName, info.name);
            
            string coinText = "";
            if (m_coin > UserDC.GetCoin()) { //金币不够时红色
                coinText = string.Format("{0}{1}", "[B05659]", m_coin);
            } else {
                coinText = m_coin.ToString();
            }
            NGUIUtil.SetLableText<string>(MyHead.LblCoin, coinText);
            
            NGUIUtil.Set2DSprite(MyHead.m_ComposeMid.Spr2dEquipIcon, "Textures/item/", info.icon.ToString());
            int quality = ConfigM.GetBigQuality(info.quality);
            NGUIUtil.SetSprite(MyHead.m_ComposeMid.SprEquipQuality, quality.ToString());
            
            NGUIUtil.Set2DSprite(MyHead.m_ComposeEquip.Spr2dEquipIcon, "Textures/item/", info.icon.ToString());
            NGUIUtil.Set2DSprite(MyHead.m_ComposeMid.Spr2dEquipIcon, "Textures/item/", info.icon.ToString());
            NGUIUtil.SetSprite(MyHead.m_ComposeEquip.SprEquipQuality, quality.ToString());
            NGUIUtil.SetSprite(MyHead.m_ComposeMid.SprEquipQuality, quality.ToString());
            
        } else {
            NGUIUtil.DebugLog(" ItemM.GetItemInfo " + m_itemTypeID.ToString() + " not found!");
        }
        m_lCurEquipItems.Clear();
        //创建子装备
        List<int> listUsedItem = new List<int>();
        foreach (var item in m_composeEquipList) {
            CreateEquipComposeItem(item.key, item.value, ref listUsedItem);
        }
        listUsedItem.Clear();
        NGUIUtil.RepositionTablePivot(MyHead.EquipMidTable.gameObject);
        ShowSprLines(m_composeEquipList.Count - 1);
    }
    /// <summary>
    /// 创建用来装备合成的 子装备或材料
    /// </summary>
    /// <param name="ItemTypeID"></param>
    /// <param name="needNum">需要的数量</param>
    private void CreateEquipComposeItem(int ItemTypeID, int needNum, ref List<int> listUsedItem)
    {
        GameObject equipComposeItem = NDLoad.LoadWndItem("ECEquipComposeItem", MyHead.EquipMidTable);
        if (equipComposeItem) {
            ECEquipComposeItem item = equipComposeItem.GetComponent<ECEquipComposeItem>();
            if (item) {
                item.SetData(ItemTypeID, needNum, ref listUsedItem);
                m_lCurEquipItems.Add(item);
            }
        }
    }
    
    private void TweenRight()
    {
        Vector3 toPos = U3DUtil.AddX(m_vInitPos, 200f);
        TweenPosition.Begin(MyHead.WndTweenParent, 0.2f, toPos, false);
    }
    /// <summary>
    /// 设置合成线条表现
    /// </summary>
    /// <param name="index">0表示需要一个物品来合成，以此内推</param>
    private void ShowSprLines(int index)
    {
        for (int i = 0; i < MyHead.SprLines.Length; i++) {
            if (i == index) {
                MyHead.SprLines[i].SetActive(true);
            } else {
                MyHead.SprLines[i].SetActive(false);
            }
        }
    }
    /// <summary>
    /// 刷新用来合成的材料数量
    /// </summary>
    private void RefreshEquipItems()
    {
        List<int> listUsedItem = new List<int>();
        foreach (ECEquipComposeItem item in m_lCurEquipItems) {
            item.RefreshUI(ref listUsedItem);
        }
        listUsedItem.Clear();
    }
    
    public void OnDestroy()
    {
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_808, EquipComposeResponse);
    }
    
}
