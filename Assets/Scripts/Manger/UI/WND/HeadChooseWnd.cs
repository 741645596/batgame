using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeadChooseWnd : WndBase 
{
	public enum PortratiType
	{ 
		Ordinary = 0,
		Achivement = 1,
	}

	List<HeadItem> mHeadItem = new List<HeadItem>();
	List<UIGrid> mAttachRoot = new List<UIGrid>();

	public HeadChooseWnd_h MyHead
	{
		get
		{
			return (base.BaseHead() as HeadChooseWnd_h);
		}
	}

	public override void BindEvents()
	{
		if (MyHead.BtnClose)
			MyHead.BtnClose.OnClickEventHandler += ClickClose;
		if (MyHead.TogglePortrait)
			MyHead.TogglePortrait.OnClickEventHandler += TogglePortrait;
		if (MyHead.TogglePortraitFrame)
			MyHead.TogglePortraitFrame.OnClickEventHandler += TogglePortraitFrame;
		if (MyHead.TogglePortraitBG)
			MyHead.TogglePortraitBG.OnClickEventHandler += TogglePortraitBG;

	}

	void ClickClose(UIButton sender)
	{
		WndManager.DestoryDialog<HeadChooseWnd>();
	}

	public override void WndStart()
	{
		base.WndStart();

		mAttachRoot.Add(MyHead.HeadParents1);
		mAttachRoot.Add(MyHead.HeadParents2);

		MyHead.LblTogglePortrait.text = NGUIUtil.GetStringByKey("70000262");
		MyHead.LblTogglePortraitFrame.text = NGUIUtil.GetStringByKey("70000263");
		MyHead.LblTogglePortraitBG.text = NGUIUtil.GetStringByKey("70000263");
		MainCameraM.s_Instance.EnableDrag(false);
		ShowList(PortraitPartType.Portrait);
	}

	void ShowList(PortraitPartType portraitPartType)
	{
		if ((portraitPartType & PortraitPartType.Portrait )== PortraitPartType.Portrait)
		{
			ShowOrdinaryPortrait(PortraitPartType.Portrait);
			ShowAchievementPortraits(PortraitPartType.Portrait);
		}
		if ((portraitPartType & PortraitPartType.Frame) == PortraitPartType.Frame)
		{
			ShowOrdinaryPortrait(PortraitPartType.Frame);
			ShowAchievementPortraits(PortraitPartType.Frame);
		}
	}

	void ShowOrdinaryPortrait(PortraitPartType portraitPartType)
	{
		List<int> portraits = UserM.GetOrdinaryPortraits(portraitPartType);
		for (int i = 0; i < portraits.Count; i++ )
		{
			GameObject go = NDLoad.LoadWndItem("HeadItem", mAttachRoot[(int)PortratiType.Ordinary].transform);
			HeadItem headItem = go.GetComponent<HeadItem>();
			headItem.Init(this);
			headItem.ShowPortrait(portraitPartType, portraits[i]);
			mHeadItem.Add(headItem);
		}
		mAttachRoot[(int)PortratiType.Ordinary].Reposition();
	}

	void ShowAchievementPortraits(PortraitPartType portraitPartType)
	{
		List<int> portraits = UserM.GetAchievementPortraits(portraitPartType);
		for (int i = 0; i < portraits.Count; i++)
		{
			GameObject go = NDLoad.LoadWndItem("HeadItem", mAttachRoot[(int)PortratiType.Achivement].transform);
			HeadItem headItem = go.GetComponent<HeadItem>();
			headItem.Init(this);
			headItem.ShowPortrait(portraitPartType, portraits[i]);
			mHeadItem.Add(headItem);
		}
		mAttachRoot[(int)PortratiType.Achivement].Reposition();
	}

	void Clear()
	{
		for (int i = 0; i < mHeadItem.Count; i++ )
		{
			HeadItem headItem = mHeadItem[i];
			GameObject.DestroyImmediate(headItem.gameObject);
		}
		mHeadItem.Clear();
	}

	void TogglePortrait(UIButton sender)
	{
		Clear();
		ShowList(PortraitPartType.Portrait);
	}

	void TogglePortraitFrame(UIButton sender)
	{
		Clear();
		ShowList(PortraitPartType.Frame);
	}

	void TogglePortraitBG(UIButton sender)
	{
		Clear();
		ShowList(PortraitPartType.Background);
	}


	public void SelectItem(HeadItem item)
	{
		for (int i = 0; i < mHeadItem.Count; i++ )
		{
			HeadItem headItem = mHeadItem[i];
			bool selected = headItem == item;
			headItem.SetSelectState(selected);
		}
	}
}