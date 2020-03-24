using UnityEngine;
using System.Collections.Generic;
using sdata;
/// <summary>
///  装备信息界面（界面类型wndType    1 确定 2 合成公式 3 装备 4 购买 5获得途径）
/// <Author>QFord</Author>
/// </summary>

public class EquipmentInfoWnd : WndBase
{
    public EquipmentInfoWnd_h MyHead {
        get
        {
            return (base.BaseHead() as EquipmentInfoWnd_h);
        }
    }
    
    private int m_iItemTypeID;
    private int m_posIndex;
    private int m_iItemID;
    private SoldierInfo m_soldierInfo;
    
    /// <summary>
    /// 装备属性信息
    /// </summary>
    private AddAttrInfo m_EquipAttrInfo = new AddAttrInfo();
    
    /// <summary>
    /// 窗口初始位置
    /// </summary>
    private Vector3 m_vInitPos = Vector3.zero;
    
    /// <summary>
    /// 1 确定 2 合成公式 3 装备 4购买,5获得途径.
    /// </summary>
    private int m_wndType = 1;
    
    //购买使用的参数
    private int m_iShopType;
    private int m_iWareID;
    private int m_iBuyCount;
    private int m_iBuyPrice;
    private int m_iBuyCurrency;
    
    public bool BGuideSelect = false;
    
    public Dictionary<int, int> GetEquipAddAttr()
    {
        return m_EquipAttrInfo.GetEquipAddAttr();
    }
    
    private void AddDataValue(List<Vector2> list, Vector2 v)
    {
        if (v.x != 0 && !list.Contains(v)) {
            list.Add(v);
        }
    }
    /// <summary>
    /// 刷新UI(默认刷新装备拥有数量)
    /// </summary>
    /// <param name="type">0:刷新装备拥有数量</param>
    public void RefreshUI(int type = 0)
    {
        switch (type) {
            case 0:
                SetEquipCountUI();
                break;
        }
    }
    
    public void SetData(int itemTypeID, int itemID, SoldierInfo soldeierInfo, int posIndex, int wndType)
    {
        m_vInitPos = MyHead.WndTweenParent.transform.localPosition;
        
        m_iItemTypeID = itemTypeID;
        m_soldierInfo = soldeierInfo;
        m_posIndex = posIndex;
        m_iItemID = itemID;
        m_wndType = wndType;
        
        //ItemTypeInfo info = ItemDC.FindEquipByItemTypeID(m_iItemTypeID, 0);
        s_itemtypeInfo info = ItemM.GetItemInfo(m_iItemTypeID);
        
        if (info == null) {
            NGUIUtil.DebugLog("s_itemtypinfo 静态表获取数据失败：" + m_iItemTypeID);
            return;
        }
        m_EquipAttrInfo.SetAddAttrInfo(info);
        //        MyHead.LblFuMo.gameObject.SetActive(false);
        NGUIUtil.Set2DSprite(MyHead.SprEquip, "Textures/item/", info.icon.ToString());
        NGUIUtil.SetLableText<string>(MyHead.LblName, info.name);
        string desc = info.message.Replace("\\n", System.Environment.NewLine);
        NGUIUtil.SetLableText<string>(MyHead.LblDescription, desc);
        string title = info.title.Replace("\\n", System.Environment.NewLine);
        NGUIUtil.SetLableText<string>(MyHead.LblTitle, title);
        SetWndType();
        if (soldeierInfo != null) {
            if (soldeierInfo.Level < info.level) {
                NGUIUtil.SetLableText<string>(MyHead.LblRequireLevel, NGUIUtil.GetStringByKey("30000014") + info.level.ToString());
                NGUIUtil.SetActive(MyHead.LblRequireLevel.gameObject, true);
                if (wndType == 3) { //装备 窗口状态下设定
                    MyHead.BtnSure.isEnabled = false;
                }
            }
        }
        
        SetEquipCountUI();
        
        if (wndType == 4) { //商品购买
            //设定购买件数
            NGUIUtil.SetActive(MyHead.LblBuyCount.gameObject, true);
            string buyCountText = string.Format(NGUIUtil.GetStringByKey(70000020), m_iBuyCount);
            NGUIUtil.SetLableText<string>(MyHead.LblBuyCount, buyCountText);
            //设定价格
            NGUIUtil.SetActive(MyHead.LblTotalPrice.gameObject.transform.parent.gameObject, true);
            
            bool currencyEnough = UserDC.CheckCurrencyEnough(m_iBuyCurrency, m_iBuyPrice);
            if (currencyEnough) {
                NGUIUtil.SetLableText<int>(MyHead.LblTotalPrice, m_iBuyPrice);
            } else {
                string priceText = string.Format("[FA3B3B]{0}[-]", m_iBuyPrice);
                NGUIUtil.SetLableText<string>(MyHead.LblTotalPrice, priceText);
            }
            
            //设定代币类型
            NGUIUtil.Set2DSprite(MyHead.Spr2dCurrency, "Textures/currency/", m_iBuyCurrency.ToString());
        }
    }
    /// <summary>
    /// 设置装备数量UI
    /// </summary>
    private void SetEquipCountUI()
    {
        int count = ItemDC.GetItemCount(m_iItemTypeID);
        string countText = string.Format("[FF7E14]" + NGUIUtil.GetStringByKey("88800040") + "[-][9C4314]{0}[-][FF7E14]" + NGUIUtil.GetStringByKey("88800037") + "[-]", count);
        NGUIUtil.SetActive(MyHead.LblFuMo.gameObject, true);
        NGUIUtil.SetLableText<string>(MyHead.LblFuMo, countText);
    }
    
    /// <summary>
    /// 设置为购买窗口类型时的数据（ShopBuyRequest）
    /// </summary>
    public void SetBuyData(int shopType, int wareID, int buyCount, int buyPrice, int buyCurrency, int itemTypeID)
    {
        m_iShopType = shopType;
        m_iWareID = wareID;
        m_iBuyCount = buyCount;
        m_iBuyPrice = buyPrice;
        m_iBuyCurrency = buyCurrency;
        m_iItemTypeID = itemTypeID;
    }
    
    public void EnableBtn(bool bEnable)
    {
        MyHead.BtnSure.enabled = bEnable;
    }
    
    /// <summary>
    /// 设置窗口在不同类型发下的表现(按钮文字不一样)
    /// </summary>
    /// <param name="type">1 确定 2 合成公式 3 装备 4购买 5获得途径</param>
    private void SetWndType()
    {
        int keyCode = 0;
        switch (m_wndType) {
            case 1:
                keyCode = 10000045;
                break;
            case 2:
                keyCode = 10000133;
                break;
            case 3:
                keyCode = 70000055;
                break;
            case 4:
                keyCode = 30000020;
                break;
            case 5:
                keyCode = 30000001;
                break;
        }
        if (m_wndType == 4) {
            MyHead.GoShop.SetActive(true);
        } else {
            MyHead.GoShop.SetActive(false);
        }
        MyHead.LblButtonType.text = NGUIUtil.GetStringByKey(keyCode);
    }
    
    public override void WndStart()
    {
        base.WndStart();
        
        MyHead.BtnSure.OnClickEventHandler += BtnSure_OnClickEventHandler;
        MyHead.BtnWndBg.OnClickEventHandler += BtnWndBg_OnClickEventHandler;
        MyHead.BtnClose.OnClickEventHandler += BtnWndBg_OnClickEventHandler;
        transform.localPosition = transform.localPosition = U3DUtil.SetZ(transform.localPosition, ConstantData.iDepBefore3DModel);
        
        WndEffects.DoWndEffect(gameObject);
    }
    void Close(object o)
    {
        WndManager.DestoryDialog<EquipmentInfoWnd>();
        WndManager.DestoryDialog<EquipComposeWnd>();
    }
    void BtnWndBg_OnClickEventHandler(UIButton sender)
    {
        WndEffects.DoCloseWndEffect(gameObject, Close);
    }
    
    void BtnSure_OnClickEventHandler(UIButton sender)
    {
        switch (m_wndType) {
            case 1://确定
                BtnWndBg_OnClickEventHandler(null);
                break;
                
            case 2://合成公式
                TweenLeft();
                EquipComposeWnd wnd = WndManager.GetDialog<EquipComposeWnd>();
                if (wnd) {
                    EnableBtn(false);
                    wnd.SetData(m_iItemTypeID);
                }
                break;
                
            case 3://穿装备
                PdbycWnd wnd1 = WndManager.FindDialog<PdbycWnd>();
                if (wnd1) {
                    wnd1.SetEquipDataNoReady();
                }
                MyHead.BtnSure.isEnabled = false;
                SoldierDC.Send_SoldierEquipRequest(m_soldierInfo.ID, m_posIndex);
                WndManager.DestoryDialog<EquipmentInfoWnd>();
                break;
                
            case 4://购买
                //堆叠数量上限判定
                bool overLimit = ItemDC.CheckItemOverLimit(m_iItemTypeID, m_iBuyCount);
                if (overLimit) {
                    NGUIUtil.ShowTipWndByKey(30000030);
                    break;
                }
                //持有代币是否足够
                bool currencyEnough = UserDC.CheckCurrencyEnough(m_iBuyCurrency, m_iBuyPrice);
                if (currencyEnough) {
                
                } else {
                    string currencyName = NGUIUtil.GetStringByKey(99700000 + m_iBuyCurrency);
                    string tipText = string.Format(NGUIUtil.GetStringByKey(30000029), currencyName);
                    //NGUIUtil.ShowTipWnd(tipText);
                    NGUIUtil.ShowFreeSizeTipWnd(tipText);
                }
                break;
                
            case 5://获得途径
                ItemComeFromWnd Formwnd = WndManager.GetDialog<ItemComeFromWnd>();
                s_itemtypeInfo info = ItemM.GetItemInfo(m_iItemTypeID);
                Formwnd.SetData(info, m_soldierInfo, null);
                break;
        }
    }
    
    /// <summary>
    /// 检测装备是否存在且符合穿的条件
    /// </summary>
    /// <returns></returns>
    public bool CheckEquipExist()
    {
        m_wndType = 3;
        SetWndType();
        s_itemtypeInfo info = ItemM.GetItemInfo(m_iItemTypeID);
        if (ItemDC.GetItemCount(m_iItemTypeID) > 0 && m_soldierInfo.Level >= info.level) {
            MyHead.BtnSure.isEnabled = true;
            return true;
        }
        MyHead.BtnSure.isEnabled = false;
        return false;
    }
    
    private void TweenLeft()
    {
        Vector3  toPos = U3DUtil.AddX(m_vInitPos, -200f);
        TweenPosition.Begin(MyHead.WndTweenParent, 0.2f, toPos, false);
    }
    
    public void TweenReset()
    {
        TweenPosition.Begin(MyHead.WndTweenParent, 0.2f, m_vInitPos);
    }
    
}
