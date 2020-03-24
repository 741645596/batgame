using UnityEngine;
using System.Collections;
using sdata;


public class ItemComeFromItem : MonoBehaviour {

	public ItemComeFromItem_h MyHead;
	private int m_iQuality;
	private int m_iCon;
	private string m_Name;
	private string m_strIconPath;
	private CounterPartInfo m_counterInfo;

	void Awake ()
	{
		MyHead = GetComponent<ItemComeFromItem_h>();
		MyHead.BtnItemClick.OnClickEventHandler += BtnItemClick_OnClickHander;
	}
	// Use this for initialization
	void Start () 
	{
		if(m_counterInfo != null)
		{
			SetCounterInfoUI();
		}
	}

	void BtnItemClick_OnClickHander(UIButton sender)
	{
		if(m_counterInfo != null)
		{
			bool Open = StageDC.CheckOpenStage((StageType)m_counterInfo.type,m_counterInfo.id);
			if(!Open)
			{
				NGUIUtil.ShowTipWndByKey(10000175);
				return;
			}

			StageMapWnd wnd = WndManager.GetDialog<StageMapWnd>();
			if(wnd != null)
			{
				wnd.SetMainMenuTop(false);
				int stageNode = StageDC.GetStageNode(m_counterInfo.id);
				int chapter = StageDC.GetStageChapter(m_counterInfo.id);
				wnd.GotoChapter((StageType)m_counterInfo.type, chapter ,stageNode);
				ItemComeFromWnd itemComeWnd = WndManager.FindDialog<ItemComeFromWnd>();
				if(itemComeWnd != null)
				{
					WndManager.SetBeforeWnd(wnd,itemComeWnd);
				}

			}
		}

	}
	public void SetCounterInfoData(CounterPartInfo info)
	{
		m_counterInfo = info;
	}

	private void SetCounterInfoUI()
	{
		if(m_counterInfo == null) return;

		if(m_counterInfo.type == 2)
		{
			MyHead.LblElite.text = NGUIUtil.GetStringByKey(70000139);
		}
		else if(m_counterInfo.type == 3)
		{
			MyHead.LblElite.text = NGUIUtil.GetStringByKey(70000140);
		}
		else if(m_counterInfo.type == 1)
		{
			MyHead.LblElite.text = "";//NGUIUtil.GetStringByKey(70000138);
		}
		bool Open = StageDC.CheckOpenStage((StageType)m_counterInfo.type,m_counterInfo.id);
		if(!Open)
		{
			MyHead.LblElite.text += NGUIUtil.GetStringByKey(10000170);
		}
		else 
		{
			MyHead.LblElite.text += "";
		}

		string Chapter = string.Format(NGUIUtil.GetStringByKey(10000169),m_counterInfo.chapters);
		MyHead.LblChapter.text = Chapter;

//		int stage = m_counterInfo.id %100000 % 1000;
//		string strStage = string.Format(NGUIUtil.GetStringByKey(10000171),stage);
		MyHead.LblItemName.text = m_counterInfo.counterpartname;

		NGUIUtil.Set2DSprite(MyHead.SprChapterIco,"Textures/Chapter/",m_counterInfo.id);
	}
}
