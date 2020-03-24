using UnityEngine;
using System.Collections;

public class CombatSchedulerWnd : WndBase
{
	UIButton mBtnExit;
	UIButton mBtnRebattle;
	UIButton mBtnResume;

	UILabel mLblExit;
	UILabel mLblRebattle;
	UILabel mLblResume;

	public override void WndStart()
	{
		Init();
		base.WndStart();
	}

	void Init()
	{
		CombatSchedulerWnd_h head = base.BaseHead() as CombatSchedulerWnd_h;
		mBtnExit = head.BtnExit;
		mBtnRebattle = head.BtnRebattle;
		mBtnResume = head.BtnResume;
		mLblExit = head.LblExit;
		mLblRebattle = head.LblRebattle;
		mLblResume = head.LblResume;

		mLblExit.text = NGUIUtil.GetStringByKey("88800064");
		mLblRebattle.text = NGUIUtil.GetStringByKey("70000194");
		mLblResume.text = NGUIUtil.GetStringByKey("70000195");
	}

	public override void BindEvents()
	{
		mBtnExit.OnClickEventHandler += BtnExit_OnClickEventHandler;
		mBtnRebattle.OnClickEventHandler += BtnRebattle_OnClickEventHandler;
		mBtnResume.OnClickEventHandler += BtnResume_OnClickEventHandler;
	}

	void BtnExit_OnClickEventHandler(UIButton sender)
	{
		Time.timeScale = 1f;
		StageDC.SendStageSettleRequest(StageDC.GetCompaignStageID(), null, null, true, false);
		WndManager.DestoryDialog<CombatSchedulerWnd>();
		BSC.AntiAllRegisterHooks();
		SceneM.Load(MainTownScene.GetSceneName(), false, null, false);
		
	}

	void BtnRebattle_OnClickEventHandler(UIButton sender)
	{ 
		Time.timeScale = 1f;
		WndManager.DestoryDialog<CombatSchedulerWnd>();
		BSC.AntiAllRegisterHooks();
		SceneM.Load(ViewStageScene.GetSceneName(), false, null, false);
	}

	void BtnResume_OnClickEventHandler(UIButton sender)
	{
		CombatScheduler.ResumeCombat();
		WndManager.DestoryDialog<CombatSchedulerWnd>();
	}
}
