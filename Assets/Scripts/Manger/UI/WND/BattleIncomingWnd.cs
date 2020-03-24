using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleIncomingWnd : WndBase
{
	
	Vector3[] mCorners;
	Dictionary<int, HeadItem> mHeadItems = null;
	int mMonsterCount;
	public int MonsterCount
	{
		set
		{
			mMonsterCount = value;
			MyHead.LblFireProgress.text = string.Format("{0}/{1}", mMonsterCount, mMonsterCount);
		}
	}
	int mCurrentMonster;

	// Use this for initialization
	public override void WndStart()
	{
		base.WndStart();
		Init();
	}

	void Init()
	{
		mCorners = MyHead.ProgressBG.localCorners;
		Vector4 br = MyHead.ProgressBG.border;
		mCorners[0].x += br.x;
		mCorners[1].x += br.x;
		mCorners[2].x -= br.z;
		mCorners[3].x -= br.z;

		mCorners[0].y += br.y;
		mCorners[1].y -= br.w;
		mCorners[2].y -= br.w;
		mCorners[3].y += br.y;

		mHeadItems = new Dictionary<int, HeadItem>();
	}

	public BattleIncomingWnd_h MyHead
	{
		get
		{
			return (base.BaseHead() as BattleIncomingWnd_h);
		}
	}

	public void CreateEnemySoldier(int soldierID)
	{
		GameObject go = NDLoad.LoadWndItem("HeadItem", MyHead.transform);
		HeadItem headItem = go.GetComponent<HeadItem>();
		SoldierInfo soldier = CmCarbon.GetSoldierInfo(LifeMCamp.ATTACK, soldierID);
		if (soldier.m_soldier_type == 0)	// ะกนึ
		{
			headItem.Scale = 0.3f;
			headItem.IsBoss = false;
		}
		else if (soldier.m_soldier_type == 1)	// BOSS
		{
			headItem.Scale = 0.5f;
			headItem.IsBoss = true;
			MyHead.FxBossComming.gameObject.SetActive(false);
			MyHead.FxBossComming.gameObject.SetActive(true);
		}
		headItem.ShowPortrait(PortraitPartType.Portrait, soldier.m_modeltype);
		mHeadItems[soldierID] = headItem;
		mCurrentMonster++;
		MyHead.LblFireProgress.text = string.Format("{0}/{1}", mCurrentMonster, mMonsterCount);
	}


	public void DestroyEnemySoldier(int soldierID)
	{
		HeadItem item = mHeadItems[soldierID];
		Animation animFadedOut = item.gameObject.GetComponent<Animation>();
		//animFadedOut.AddClip(MyHead.BossCmoingUIClip, MyHead.BossCmoingUIClip.name);
		//animFadedOut.clip = MyHead.BossCmoingUIClip;
		animFadedOut.enabled = true;
		animFadedOut.Play();

	}

	public void SetEnemyProgress(int soldierID, float value)
	{
		HeadItem headItem;
		mHeadItems.TryGetValue(soldierID, out headItem);
		Transform t = mHeadItems[soldierID].transform;
		SetProgress(t, value);
	}

	void SetProgress(Transform thumb, float value)
	{
		Vector3 v0 = Vector3.Lerp(mCorners[0], mCorners[1], 0.5f);
		Vector3 v1 = Vector3.Lerp(mCorners[2], mCorners[3], 0.5f);
		Vector3 pos = Vector3.Lerp(v0, v1, 1 - value);
		SetThumbPosition(thumb, MyHead.ProgressBG.cachedTransform.TransformPoint(pos));
	}

	void SetThumbPosition(Transform thumb, Vector3 worldPos)
	{
		Transform t = thumb.parent;

		if (t != null)
		{
			worldPos = t.InverseTransformPoint(worldPos);
			worldPos.x = Mathf.Round(worldPos.x);
			worldPos.y = Mathf.Round(worldPos.y);
			worldPos.z = 0f;

			if (Vector3.Distance(thumb.localPosition, worldPos) > 0.001f)
				thumb.localPosition = worldPos;
		}
		else if (Vector3.Distance(thumb.position, worldPos) > 0.00001f)
			thumb.position = worldPos;
	}

}