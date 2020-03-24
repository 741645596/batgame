using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SweepResultItem : WndBase
{
	static List<string> ChineseNumerals = new List<string>() 
	{
		// 一到十
		NGUIUtil.GetStringByKey("70000216"),
		NGUIUtil.GetStringByKey("70000217"),
		NGUIUtil.GetStringByKey("70000218"),
		NGUIUtil.GetStringByKey("70000219"),
		NGUIUtil.GetStringByKey("70000220"),
		NGUIUtil.GetStringByKey("70000221"),
		NGUIUtil.GetStringByKey("70000222"),
		NGUIUtil.GetStringByKey("70000223"),
		NGUIUtil.GetStringByKey("70000224"),
		NGUIUtil.GetStringByKey("70000225"),
	};


	public SweepResultItem_h MyHead
	{
		get
		{
			return (base.BaseHead() as SweepResultItem_h);
		}
	}

	public void SetSweepResult(StageDC.StageSweepReward sweepReward, int stageIndex)
	{
		MyHead.Session.text = string.Format(NGUIUtil.GetStringByKey("70000215"), ChineseNumerals[stageIndex]);
		for (int i = 0; i < sweepReward.mSweepItems.Count; i++ )
		{	
			StageDC.StageSweepReward.ItemInfo sweepRewardItem = sweepReward.mSweepItems[i];
			GameObject go = NDLoad.LoadWndItem("RewardItem", MyHead.SweepRewardItems.transform);
			if (go != null)
			{
				RewardItem item = go.GetComponent<RewardItem>();
				if (item != null)
					item.SetRewardItem(sweepRewardItem.mItemTypeID, sweepRewardItem.mCount);
			}
		}
		MyHead.RewardExp.text = string.Format("+{0}", sweepReward.mExp);
		MyHead.RewardCoin.text = string.Format("+{0}", sweepReward.mCoin);
	}

	public void SetExtraRewardItems(List<StageDC.StageSweepReward.ItemInfo> extraRewards)
	{
		//额外获得
		MyHead.Session.text = NGUIUtil.GetStringByKey("70000226");
		foreach (StageDC.StageSweepReward.ItemInfo sweepRewardItem in extraRewards)
		{
			GameObject go = NDLoad.LoadWndItem("RewardItem", MyHead.SweepRewardItems.transform);
			if (go != null)
			{
				RewardItem item = go.GetComponent<RewardItem>();

				if (item != null)
					item.SetRewardItem(sweepRewardItem.mItemTypeID, sweepRewardItem.mCount);
			}
		}
	}
}