using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class StageWipeWnd : WndBase 
{
	int mSweepCount;

	List<SweepResultItem> mSweepResultItems = new List<SweepResultItem>();

	public void Init(int sweepCount)
	{
		mSweepCount = sweepCount;
	}

	public StageWipeWnd_h MyHead
	{
		get
		{
			return (base.BaseHead() as StageWipeWnd_h);
		}
	}

	public override void BindEvents()
	{
		if (MyHead.BtnConfirm)
			MyHead.BtnConfirm.OnClickEventHandler += ClickClose;
		if (MyHead.BtnRewipe)
			MyHead.BtnRewipe.OnClickEventHandler += Resweep;
	}

	void ClickClose(UIButton sender)
	{
		WndManager.DestoryDialog<StageWipeWnd>();
	}

	void Resweep(UIButton sender)
	{
		ItemTypeInfo sweepTicketInfo = ItemDC.GetSweepTickets();
		if (sweepTicketInfo == null || sweepTicketInfo.Num < mSweepCount)
		{			
			WndManager.DestoryDialog<StageWipeWnd>();
			NGUIUtil.ShowFreeSizeTipWnd(NGUIUtil.GetStringByKey("70000229"));
			return;
		}
		CounterPartInfo info = StageDC.GetCounterPartInfo();
		int playerPhysical = UserDC.GetPhysical();

		// ÌåÁ¦²»×ã
		if (playerPhysical < info.win_physical * mSweepCount)
		{
			WndManager.DestoryDialog<StageWipeWnd>();
			NGUIUtil.ShowFreeSizeTipWnd(NGUIUtil.GetStringByKey("99904008"));
			return;
		}

		StageDC.SendStageSweepRequest(StageDC.GetCompaignStageID(), mSweepCount);
		Clear();
		//DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_718, ShowSweepResult);		
	}

	void ShowSweepResult(int nErrorCode)
	{		
		List<StageDC.StageSweepReward> sweepRewards = StageDC.GetStageSweepRewards();
		SetWipeResult(sweepRewards);
		//DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_718, ShowSweepResult);
	}

	public void SetWipeResult(List<StageDC.StageSweepReward> sweepRewards)
	{
		CounterPartInfo info = StageDC.GetCounterPartInfo();
		if (info.type == (int)StageType.Normal)
		{
			MyHead.LblTitle.text = NGUIUtil.GetStringByKey(70000213);
		}
		else if (info.type == (int)StageType.Hard)
		{
			MyHead.LblTitle.text = NGUIUtil.GetStringByKey(70000214);
		}

		List<StageDC.StageSweepReward.ItemInfo> extraRewardItems = new List<StageDC.StageSweepReward.ItemInfo>();
		for (int i = 0; i<sweepRewards.Count; i++ )
		{
			StageDC.StageSweepReward sweepReward = sweepRewards[i];
			GameObject go = Create(MyHead.SweepResultInfoTemplate, MyHead.StageWipeParent.transform);
			go.SetActive(true);
			SweepResultItem sweepResultItem = go.GetComponent<SweepResultItem>();
			sweepResultItem.SetSweepResult(sweepReward, i);
			mSweepResultItems.Add(sweepResultItem);
			extraRewardItems.AddRange(sweepReward.mExtraRewards);
		}

		GameObject goExtra = Create(MyHead.SweepResultInfoTemplate, MyHead.StageWipeParent.transform);
		goExtra.SetActive(true);
		SweepResultItem sweepExtraRewardItem = goExtra.GetComponent<SweepResultItem>();
		sweepExtraRewardItem.SetExtraRewardItems(extraRewardItems);
		mSweepResultItems.Add(sweepExtraRewardItem);
		MyHead.StageWipeParent.Reposition();
	}

	void Clear()
	{
		for (int i = 0; i < mSweepResultItems.Count; i++ )
		{
			SweepResultItem sweepResultItem = mSweepResultItems[i];
			GameObject.DestroyImmediate(sweepResultItem.gameObject);
		}
		mSweepResultItems.Clear();
	}

	GameObject Create(GameObject obj, Transform parent)
	{
		GameObject go = GameObject.Instantiate(MyHead.SweepResultInfoTemplate) as GameObject;
		go.transform.parent = parent;
		go.transform.localPosition = Vector3.zero;
		go.transform.localScale = Vector3.one;
		go.transform.localRotation = Quaternion.Euler(Vector3.zero);
		return go;
	}

}