using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 公用提示窗（1商店购买 ，2与服务器断线，3购买体力，4 重置竞技场CD，5重置竞技场刷新时间，6改名界面）
/// <Author>QFord</Author>
/// </summary>
public class PublicSmallTipsWnd : WndTopBase {

    private int m_iWndType = 0;

    public PublicSmallTipsWnd_h MyHead
    {
        get { return base.BaseHead() as PublicSmallTipsWnd_h; }
    }

    public delegate void BtnConfirmClickHandler(UIButton sender);
    public BtnConfirmClickHandler BtnClickEventHandler;

	public override void WndStart()
	{
		base.WndStart();
        MyHead.BtnCancel.OnClickEventHandler += BtnCancel_OnClickEventHandler;
        MyHead.BtnConfirm.OnClickEventHandler += BtnConfirm_OnClickEventHandler;

		WndEffects.DoWndEffect(gameObject);
	}

    void BtnConfirm_OnClickEventHandler(UIButton sender)
    {
        //窗口类型：1商店购买 ，2与服务器断线，3购买体力，4 重置竞技场CD，5重置竞技场刷新时间，6改名界面
        switch (m_iWndType)
        {
            case 1:
                
                break;
            case 2:
                
                break;
            case 3:
                UserDC.Send_BuyPhysicalRequest();
                break;
            case 4:
                
                break;
            case 5:
                
                break;
            case 6:
                
                break;
        }

        if (BtnClickEventHandler!=null)
        {
            BtnClickEventHandler(sender);
        }

		BtnCancel_OnClickEventHandler(sender);
    }

	void Close(object o)
	{
		WndManager.DestoryDialog<PublicSmallTipsWnd>();
	}
    void BtnCancel_OnClickEventHandler(UIButton sender)
    {
		WndEffects.DoCloseWndEffect(gameObject,Close);
    }
	/// <summary>
    /// 窗口类型：1商店购买 ，2与服务器断线，3购买体力，4 重置竞技场CD，5重置竞技场刷新时间，6改名界面
	/// </summary>
	/// <param name="wndType"></param>
    public void SetData(int wndType)
    {
        m_iWndType = wndType;
        switch (m_iWndType)
        {
            case 1:
                NGUIUtil.SetActive(MyHead.ShopBuyGroup, true);
                break;
            case 2:
                NGUIUtil.SetActive(MyHead.LoginErrorGroup, true);
                break;
            case 3:
                NGUIUtil.SetActive(MyHead.BuySPGroup, true);
                break;
            case 4:
                NGUIUtil.SetActive(MyHead.ArenaCDGroup, true);
                break;
            case 5:
                NGUIUtil.SetActive(MyHead.ArenaTimeGroup, true);
                break;
            case 6:
                NGUIUtil.SetActive(MyHead.ChangeNameGroup, true);
                MyHead.BtnRandom.OnClickEventHandler += BtnRandom_OnClickEventHandler;
                break;
        }
    }
    /// <summary>
    /// 随机改名
    /// </summary>
    /// <param name="sender"></param>
    void BtnRandom_OnClickEventHandler(UIButton sender)
    {
        
    }
    /// <summary>
    /// 商店购买UI设定
    /// </summary>
    public void SetShopBuyUI(int needDiamod,int refreshTime)
    {
        NGUIUtil.SetLableText(MyHead.LblDiamondNum, needDiamod);
        NGUIUtil.SetLableText(MyHead.LblRefreshTime, refreshTime);
    }
    /// <summary>
    /// 购买体力UI
    /// </summary>
    public void SetBuySpUI(int needDiamod)
    {
        NGUIUtil.SetLableText(MyHead.LblCostDiamondSP, needDiamod);
    }

    /// <summary>
    /// 重置竞技场CD UI
    /// </summary>
    public void SetArenaCDUI(int needDiamod)
    {
        NGUIUtil.SetLableText(MyHead.LblCostDiamondCD, needDiamod);
    }
    
    /// <summary>
    /// 重置竞技场刷新时间 UI
    /// </summary>
    /// <param name="buyTime">第n次购买</param>
    /// <param name="needDiamod">消耗钻石</param>
    /// <param name="plusTime">增加n次</param>
    public void SetArenaTimeUI(int buyTime,int needDiamod,int plusTime)
    {
        NGUIUtil.SetLableText(MyHead.LblBuyTime, buyTime);
        NGUIUtil.SetLableText(MyHead.LblCostDiamondTime, needDiamod);
        NGUIUtil.SetLableText(MyHead.LblPlusTime, plusTime);
    }
	
}
