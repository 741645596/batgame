using UnityEngine;
using System.Collections;
using sdata;


public class EquipComeFromItem : MonoBehaviour {

	public EquipComeFromItem_h MyHead;
	private int m_iQuality;
	private int m_iCon;
	private string m_Name;
	private string m_strIconPath;
	private CounterPartInfo m_counterInfo;

	void Awake ()
	{
		MyHead = GetComponent<EquipComeFromItem_h>();
	}
	// Use this for initialization
	void Start () 
	{
		if(m_counterInfo != null)
		{
			SetCounterInfoUI();
		}
	}

	public void SetCounterInfoData(CounterPartInfo info)
	{
		m_counterInfo = info;
	}

	private void SetCounterInfoUI()
	{
		if(m_counterInfo == null) return;

		string showText = "";

		if(m_counterInfo.type == 2)
		{
			showText = NGUIUtil.GetStringByKey(70000139) + System.Environment.NewLine;
		}
		else if(m_counterInfo.type == 3)
		{
			showText = NGUIUtil.GetStringByKey(70000140) + System.Environment.NewLine;
		}
		else if(m_counterInfo.type == 1)
		{
			showText = "";//NGUIUtil.GetStringByKey(70000138);
		}
//		bool Open = StageDC.CheckOpenStage((StageType)m_counterInfo.type,m_counterInfo.id);
//		if(!Open)
//		{
//			MyHead.LblElite.text += NGUIUtil.GetStringByKey(10000170);
//		}
//		else 
//		{
//			MyHead.LblElite.text += "";
//		}

//		string Chapter = string.Format(NGUIUtil.GetStringByKey(10000169),m_counterInfo.chapters);
//		MyHead.LblChapter.text = Chapter;

//		int stage = m_counterInfo.id %100000 % 1000;
//		string strStage = string.Format(NGUIUtil.GetStringByKey(10000171),stage);

		MyHead.LblItemName.text = showText + m_counterInfo.counterpartname;

		NGUIUtil.Set2DSprite(MyHead.SprChapterIco,"Textures/Chapter/",m_counterInfo.id);
	}
}
