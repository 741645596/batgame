using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using sdata;

/// <summary>
/// 评星奖励 是否获取到物品
/// </summary>
public enum StarAwardType
{
    /// <summary>
    /// 获取到物品
    /// </summary>
    GetItems = 0,
    /// <summary>
    /// 啥都没有
    /// </summary>
    GetNothing = 1,
}

/// <summary>
/// 评星奖励
/// </summary>
public class StarAwardWnd : WndBase 
{
    public StarAwardWnd_h MyHead
    {
        get
        {
            return (base.BaseHead() as StarAwardWnd_h);
        }
    }
    /// <summary>
    /// 当前窗口类型 获得物品/啥都没有
    /// </summary>
    private StarAwardType m_type;
    /// <summary>
    /// 奖励物品列表
    /// </summary>
	private Dictionary<s_itemtypeInfo ,int>  m_itemList = new Dictionary<s_itemtypeInfo ,int>();
	private int m_RewardCoin = 0;
	private int m_RewardDiamond = 0;

    private int m_starRewardID;
	private StageMapWnd m_wnd = null;

	public override void WndStart()
	{
		base.WndStart();
		RegisterHooks();
        MyHead.BtnConfirm.OnClickEventHandler += BtnConfirm_OnClickEventHandler;
        MyHead.BtnGetItem.OnClickEventHandler += BtnGetItem_OnClickEventHandler;


		WndEffects.DoWndEffect(gameObject);
    }

    private void SetUI()
    {
        foreach (var item in m_itemList.Keys)
        {
			CreateItem(item ,m_itemList[item]);
       }

		if (m_type == StarAwardType.GetItems)
		{
			MyHead.BtnGetItem.gameObject.SetActive(true);
			MyHead.BtnConfirm.gameObject.SetActive(false);
		}
		else
		{
			MyHead.BtnConfirm.gameObject.SetActive(true);
			MyHead.BtnGetItem.gameObject.SetActive(false);
		}
	}
	
	private void CreateItem(s_itemtypeInfo info ,int num)
    {
		GameObject go = NDLoad.LoadWndItem("BeiBaoItem", MyHead.TabItem.transform);
        if (go)
        {
            BeiBaoItem item = go.GetComponent<BeiBaoItem>();
			if(item != null)
				item.SetData(ItemM.GetItemInfo(info),num ,null);
        }
		MyHead.TabItem.Reposition();
    }

    void BtnGetItem_OnClickEventHandler(UIButton sender)
    {
        StageDC.SendStageRewardGetRequest(m_starRewardID);
    }
    /// <summary>
    /// 确认 啥都没得到
    /// </summary>
    void BtnConfirm_OnClickEventHandler(UIButton sender)
    {
		WndEffects.DoCloseWndEffect(gameObject,Close);
    }

	void Close(object o)
	{
		WndManager.DestoryDialog<StarAwardWnd>();
	}
    /// <summary>
    /// 设置奖励界面数据
    /// </summary>
    /// <param name="type">是否获取到物品</param>
    /// <param name="StarRewardID">s_stage_reward表的id</param>
    /// <param name="starNumText">星星数量，如  8/15 </param>
    /// <param name="itemList">奖励物品列表</param>
	public void SetData(StageMapWnd Wnd,StarAwardType type,int StarRewardID ,int HaveStar,int TotalStar)
    {
		m_wnd = Wnd;
        m_type = type;
        m_starRewardID = StarRewardID;
		m_itemList =  StageM.GetChapterRewardItem(m_starRewardID, ref m_RewardCoin, ref m_RewardDiamond);
        MyHead.LblCoin.text = m_RewardCoin.ToString();
        MyHead.LblDiamond.text = m_RewardDiamond.ToString();
		if(MyHead != null && MyHead.LblStarNum != null)
			MyHead.LblStarNum.text = "[ffffff]"+ HaveStar.ToString() + "[-][c7945b]" + "/" +  TotalStar.ToString() +"[-]";

		
		SetUI();
    }

	/// <summary>
	/// 注册事件
	/// </summary>
	public   void RegisterHooks()
	{
		DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_708, BoxReward);
	}
	
	/// <summary>
	/// 反注册事件
	/// </summary>
	public   void AntiRegisterHooks()
	{
		DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_708, BoxReward);
	}

	void BoxReward(int nErrorCode)
	{

		if (nErrorCode==0)
		{

		}
		else
		{
			
		}
		if(m_wnd != null)
		{
			m_wnd.GetBoxDataStage();
		}

		BtnConfirm_OnClickEventHandler(null);
	}

	public void OnDestroy()
	{
		AntiRegisterHooks();
	}
}
