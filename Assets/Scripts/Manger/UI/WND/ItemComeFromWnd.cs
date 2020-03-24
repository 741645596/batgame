using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using sdata;


public class ItemComeFromWnd : WndTopBase 
{
	public ItemComeFromWnd_h MyHead 
	{
		get
		{ 
			return (base.BaseHead () as ItemComeFromWnd_h);	
		}	
	}
	private BuildInfo m_buildInfo;
	private SoldierInfo m_soldierInfo;
	private s_itemtypeInfo m_itemInfo;
	/// <summary>
	/// 1:正常获得途径窗口，2：pdbyc界面灵魂石获得途径(已经召唤)，3：未召唤，4trapshowwnd陷阱碎片获得,5陷阱未召唤.
	/// </summary>
	private int m_wndType = 1;
    public override void WndStart()
 	{
		base.WndStart(); 

		WndEffects.DoWndEffect(gameObject);
	}

	public override void BindEvents ()
	{
		base.BindEvents ();
		MyHead.BtnClose.OnClickEventHandler += ClickClose;
	}

	public void SetData(s_itemtypeInfo info,SoldierInfo soldierInfo,BuildInfo BuildInfo,int wndType = 1)
	{
		m_itemInfo = info;
		m_wndType = wndType;
		m_soldierInfo = soldierInfo;
		m_buildInfo = BuildInfo;
		SetUI();
	}

	private void SetUI()
	{
		NGUIUtil.Set2DSprite(MyHead.Spr2DItemIcon, "Textures/item/", m_itemInfo.icon.ToString());
		NGUIUtil.SetLableText<string>(MyHead.LblItemName, m_itemInfo.name);
		NGUIUtil.SetSprite(MyHead.SprItemQuality,ConfigM.GetBigQuality(m_itemInfo.quality).ToString());

		ShowLblItemCount();
		AddItemComeFrom();
	}
	private void ShowLblItemCount()
	{
		if(m_wndType == 1)
		{
			int count = ItemDC.GetItemCount(m_itemInfo.id);
			string countText = string.Format("[FF7E14]" + NGUIUtil.GetStringByKey("88800040") + "[-][9C4314]{0}[-][FF7E14]" + NGUIUtil.GetStringByKey("88800037") + "[-]", count);
			MyHead.LblItemAmount.text = countText;
		}
		else if(m_wndType == 2 && m_soldierInfo != null)
		{
			int NeedNum = 0;
			int NeedCoin = 0;
			SoldierM.GetUpStarNeed(m_soldierInfo.SoldierTypeID ,m_soldierInfo.StarLevel + 1 , ref NeedNum ,ref  NeedCoin);
			int Have = m_soldierInfo.GetHaveFragmentNum();//当前灵魂石
			MyHead.LblItemAmount.text = Have.ToString()+"/" + NeedNum.ToString();

		}
		else if(m_wndType == 3 && m_soldierInfo != null)
		{
			int NeedNum = 0;
			int NeedCoin = 0;
			SoldierM.GetUpStarNeed(m_soldierInfo.SoldierTypeID ,m_soldierInfo.StarLevel , ref NeedNum ,ref  NeedCoin);
			int Have = m_soldierInfo.GetHaveFragmentNum();//当前灵魂石
			MyHead.LblItemAmount.text = Have.ToString()+"/" + NeedNum.ToString();
		}
		else if(m_wndType == 4 && m_buildInfo != null)
		{
			int iCoin = 0;
			int NeedNum = 0;
			
			buildingM.GetUpStarNeed (m_buildInfo.BuildType,m_buildInfo.StarLevel + 1,ref NeedNum,ref iCoin);
			int Have = ItemDC.GetItemCount(m_buildInfo.fragmentTypeID);
			MyHead.LblItemAmount.text = Have.ToString()+"/" + NeedNum.ToString();
		}
		else if(m_wndType == 5 && m_buildInfo != null)
		{
			int iCoin = 0;
			int NeedNum = 0;
			
			buildingM.GetUpStarNeed (m_buildInfo.BuildType,m_buildInfo.StarLevel,ref NeedNum,ref iCoin);
			int Have = ItemDC.GetItemCount(m_buildInfo.fragmentTypeID);
			MyHead.LblItemAmount.text = Have.ToString()+"/" + NeedNum.ToString();
		}
	}
	/// <summary>
	/// 其他获得途径.
	/// </summary>
	/// <param name="show">If set to <c>true</c> show.</param>
	private void ShowAnotherFrom(bool show = true)
	{
		if(show)
		{
			int soldierId = SoldierM.GetSoldierStarID(m_itemInfo.id);
			string DropDesc = SoldierM.GetHeroDropDesc(soldierId);
			MyHead.LblGetDes.text = DropDesc;
		}
		MyHead.LblGetDes.gameObject.SetActive(show);


	}
	private void AddItemComeFrom()
	{
		List<int> lCounpart = StageM.GetRewardFromStage(m_itemInfo.id);
		if(lCounpart == null || lCounpart.Count < 1)
		{
			ShowAnotherFrom();
			return;
		} 
		List<CounterPartInfo> lCounterInfo = StageM.GetCounterPartInfoList(lCounpart);
		if(lCounterInfo == null || lCounterInfo.Count < 1)
		{
			ShowAnotherFrom();
			return;
		}
		ShowAnotherFrom(false);
		Dictionary<int,int> dl = new Dictionary<int, int>();
		foreach( CounterPartInfo info in lCounterInfo)
		{
//			if(info.isboss == 0) continue;
			if(dl.ContainsKey(info.id)) continue;
			dl.Add(info.id,info.id);
			GameObject go = NDLoad.LoadWndItem("ItemComeFromItem", MyHead.Table.transform);
			if(go != null)
			{
				ItemComeFromItem item = go.GetComponent<ItemComeFromItem>();
				if (item)
				{
					item.SetCounterInfoData(info);
				}
			}
			
		}
		MyHead.Table.Reposition();
	}

	void Close(object o)
	{
		WndManager.DestoryDialog<ItemComeFromWnd>();
	}
	void ClickClose(UIButton sender)
	{
		WndEffects.DoCloseWndEffect(gameObject,Close);
	}
}