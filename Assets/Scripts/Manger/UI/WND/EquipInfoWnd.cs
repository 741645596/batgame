using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using sdata;

public class EquipInfoWnd : WndBase 
{
	public EquipInfoWnd_h MyHead 
	{
		get
		{ 
		 return (base.BaseHead () as EquipInfoWnd_h);	
		}	
	}
	private ItemTypeInfo m_ItemInfo;

    public override void WndStart()
	 {
	 	base.WndStart();
		MyHead.BtnClose.OnClickEventHandler += BtnClose_OnClickHander;
		DoWndEffect ();
		MyHead.DragBox.enabled = false;
	 }

	public void SetData(ItemTypeInfo info)
	{
		m_ItemInfo = info;
        SetUI();
	}

	private void SetUI()
	{
		if(MyHead.Spr2dIcon != null )
			NGUIUtil.Set2DSprite(MyHead.Spr2dIcon, "Textures/item/",m_ItemInfo.m_Icon.ToString());
		//设置物品品阶
		if(MyHead.SprQuality != null)
			NGUIUtil.SetSprite(MyHead.SprQuality, ConfigM.GetBigQuality(m_ItemInfo.m_Quality).ToString());

		if(m_ItemInfo.m_type == ItemType.LingHunShi)
		{
			AddGetWay ();
		}
		else if(m_ItemInfo.m_type == ItemType.JuanZhou)
		{
			AddCouldEquip ();
			AddGetWay ();
		}
		else if(m_ItemInfo.m_type == ItemType.Lingjian)
		{
			AddGetWay ();
		}
		else
		{
			AddCouldEquip ();
			AddCouldEquipHero ();
			AddGetWay ();

		}
		StartCoroutine(RepositionFinish(2));
	}

	IEnumerator RepositionFinish(int frameCount)
	{
		yield return StartCoroutine(U3DUtil.WaitForFrames(frameCount));
		ResetPosition();
		MyHead.DragBox.enabled = true;
	}

	public void AddBanner(int WndType)
	{
		string showText = "";
		if(WndType == 0)
		{
			showText = NGUIUtil.GetStringByKey(10000149);
		}
		else if(WndType == 1)
		{
			showText = NGUIUtil.GetStringByKey(10000150);
		}
		else if(WndType == 2)
		{
			showText = NGUIUtil.GetStringByKey(10000151);
		}

        GameObject go = NDLoad.LoadWndItem("PdbbbItem", MyHead.Table.transform);
		PdbbbItem Pitem = go.GetComponent<PdbbbItem>();
		if(Pitem != null && Pitem.MyHead.Table != null)
		{
            GameObject go2 = NDLoad.LoadWndItem("EquipInfoInnerBanner", Pitem.MyHead.Table.transform);
			if (go2 != null)
			{
				EquipInfoInnerBanner item = go2.GetComponent<EquipInfoInnerBanner>();
				item.SetLabelName(showText);
			}
			Pitem.MyHead.Table.Reposition();
			Pitem.MyHead.Table.repositionNow = true;
			UIPanel panel = MyHead.Table.transform.parent.gameObject.GetComponent<UIPanel>();
			float x = 150f;
			if(panel != null)
			{
				UISprite spr = Pitem.GetComponentInChildren<UISprite>();
				if(spr != null)
				{
					x = (panel.width - spr.width)/2 -30;
				}
			}
			Pitem.MyHead.Table.transform.localPosition = new Vector3(x,Pitem.MyHead.Table.transform.localPosition.y,Pitem.MyHead.Table.transform.localPosition.z);
		} 

	}

	/// <summary>
	/// 可合成装备列表.
	/// </summary>
	private void AddCouldEquip()
	{
		List<s_itemtypeInfo> ListInfo = new List<s_itemtypeInfo> ();
		List<KeyValueName> lList = new List<KeyValueName>();
		///获得用途列表
		
		ListInfo = ItemM.GetEquipMaterialUse(m_ItemInfo.itemType);
		if(ListInfo == null || ListInfo.Count < 1) return;
		AddBanner(1);
		PdbbbItem pbbItem = null;
        GameObject go = NDLoad.LoadWndItem("PdbbbItem", MyHead.Table.transform);
		pbbItem = go.GetComponent<PdbbbItem>();


		if(pbbItem != null && pbbItem.MyHead.Table != null)
		{

			pbbItem.MyHead.Table.columns = 2;
			pbbItem.MyHead.Table.padding.y = 10f;

			foreach(s_itemtypeInfo info in ListInfo)
			{
                GameObject go2 = NDLoad.LoadWndItem("EquipInfoInnerItem", pbbItem.MyHead.Table.transform);
				if (go2 != null)
				{
					EquipInfoInnerItem item = go2.GetComponent<EquipInfoInnerItem>();
					item.SetData(info.icon,info.quality,info.name,"Textures/item/");
				}
			}
			pbbItem.MyHead.Table.Reposition();
			pbbItem.MyHead.Table.repositionNow = true;
		}

	}

	/// <summary>
	/// 哪些炮弹兵可以装备
	/// </summary>
	private void AddCouldEquipHero()
	{
		List<SoldierInfo> lSoldierInfo = new List<SoldierInfo>();
		List<KeyValueName> lList = new List<KeyValueName>();
		
		lSoldierInfo = SoldierM.GetAllSoldier ();
		if(lSoldierInfo == null || lSoldierInfo.Count < 1) return;
		AddBanner(2);
		PdbbbItem pbbItem = null;
        GameObject go2 = NDLoad.LoadWndItem("PdbbbItem", MyHead.Table.transform);
		pbbItem = go2.GetComponent<PdbbbItem>();
		if(pbbItem != null && pbbItem.MyHead.Table != null)
		{

			pbbItem.MyHead.Table.columns = 3;
			pbbItem.MyHead.Table.padding.y = 10f;
			pbbItem.MyHead.Table.padding.x = 20f;
			foreach(SoldierInfo info in lSoldierInfo)
			{
				bool can = SoldierM.CheckCanPutEquip(info.SoldierTypeID,m_ItemInfo.itemType);
				if(can)
				{
                    GameObject go = NDLoad.LoadWndItem("EquipInfoInnerHeroItem", pbbItem.MyHead.Table.transform);
					if (go != null)
					{
						EquipInfoInnerHeroItem item = go.GetComponent<EquipInfoInnerHeroItem>();
						item.SetData(info);
					}
				}
			}
			pbbItem.MyHead.Table.Reposition();
			pbbItem.MyHead.Table.repositionNow = true;
		}

	}
	/// <summary>
	/// 获得途径.
	/// </summary>
	private void AddGetWay()
	{
		AddBanner(0);
		List<int> lCounpart = StageM.GetRewardFromStage(m_ItemInfo.itemType);
		if(lCounpart == null || lCounpart.Count < 1)
		{
			return;
		} 
		List<CounterPartInfo> lCounterInfo = StageM.GetCounterPartInfoList(lCounpart);
		if(lCounterInfo == null || lCounterInfo.Count < 1)
		{
			return;
		}

		Dictionary<int,int> dl = new Dictionary<int, int>();
		PdbbbItem pbbItem = null;
        GameObject go2 = NDLoad.LoadWndItem("PdbbbItem", MyHead.Table.transform);
		pbbItem = go2.GetComponent<PdbbbItem>();
		if(pbbItem != null && pbbItem.MyHead.Table != null)
		{

			pbbItem.MyHead.Table.columns = 2;
			pbbItem.MyHead.Table.padding.y = 10f;
			foreach( CounterPartInfo info in lCounterInfo)
			{
//				if(info.isboss == 0) continue;
				if(dl.ContainsKey(info.id)) continue;
				dl.Add(info.id,info.id);
				GameObject go = NDLoad.LoadWndItem("EquipComeFromItem", pbbItem.MyHead.Table.transform);
				if(go != null)
				{
					EquipComeFromItem item = go.GetComponent<EquipComeFromItem>();
					if (item)
					{
						item.SetCounterInfoData(info);
						item.MyHead.BtnItemClick.OnClickEventHandler += (UIButton sender) => 
						{
							bool Open = StageDC.CheckOpenStage((StageType)info.type,info.id);
							if(!Open)
							{
								NGUIUtil.ShowTipWndByKey(10000175);
								return;
							}
							
							StageMapWnd wnd = WndManager.GetDialog<StageMapWnd>();
							if(wnd != null)
							{
								wnd.SetMainMenuTop(false);
								
								int stageNode = StageDC.GetStageNode(info.id);
								int chapter = StageDC.GetStageChapter(info.id);
								wnd.GotoChapter((StageType)info.type, chapter ,stageNode);
							}
							EquipInfoWnd InfoWnd = WndManager.FindDialog<EquipInfoWnd>();
							if(InfoWnd != null)
							{
								WndManager.SetBeforeWnd(wnd,InfoWnd);
							}
						};;
					}
				}	
				pbbItem.MyHead.Table.Reposition();
				pbbItem.MyHead.Table.repositionNow = true;
			}
		}
	}
	public void ResetPosition()
	{
		MyHead.Table.Reposition ();
		MyHead.Table.repositionNow = true;
	}

	void BtnClose_OnClickHander(UIButton sender)
	{
		WndEffects.DoCloseWndEffect(gameObject,DestoryDialogCallBack);
	}

	void DestoryDialogCallBack(object o)
	{
		WndManager.DestoryDialog<EquipInfoWnd>();		
	}
	
	void DoWndEffect()
	{
		
		WndEffects.DoWndEffect(gameObject);
		
	}

}