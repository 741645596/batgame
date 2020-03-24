using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum LineState
{
	Finish        = 0,    //一完成
	Roading       = 1,    //途中
	UnOPEN        = 2,    //未开启
}

public class StageLine : MonoBehaviour {

	public List<StagePoint> m_StagePoint = new List<StagePoint>();
	public  int m_PrevStage;    //前一节
	public  int m_NextStage;    //下一节

	private StageType m_Type;   //关卡类型
	private int m_Chapter;      //章
	private LineState m_state = LineState.UnOPEN;


	/// <summary>
	/// 设置关卡副本数据
	/// <summary>
	public void SetStageData(StageType Type ,int Chapter )
	{
		m_Type = Type ;
		m_Chapter = Chapter ;
		CalcRoadState();

		SetUI();
	}

	private void CalcRoadState()
	{
		int PrevStageID = StageM.GetStageID(m_Type ,m_Chapter ,m_PrevStage);
		int NextStageID = StageM.GetStageID(m_Type ,m_Chapter ,m_NextStage);

		bool IsPrevOpen = StageDC.CheckOpenStage(m_Type,PrevStageID );
		int  PrevstarNum = StageDC.GetPassStageStar(m_Type,PrevStageID);

		bool IsNextOpen = StageDC.CheckOpenStage(m_Type,NextStageID );
		int  NextstarNum = StageDC.GetPassStageStar(m_Type,NextStageID);
		//
		if(IsPrevOpen == false || PrevstarNum <= 0)
			m_state = LineState.UnOPEN;
		else if(IsPrevOpen == true && PrevstarNum > 0 )
		{
			if(IsNextOpen == true)
			{
				if(NextstarNum > 0)
				{
					m_state = LineState.Finish;
				}
				else 
				{
					m_state = LineState.Roading;
				}
			}
		}
		else m_state = LineState.UnOPEN;
	}


	private void SetUI()
	{
		if(m_StagePoint == null || m_StagePoint.Count ==0)
			return ;
		for(int i = 0; i< m_StagePoint.Count; i++)
		{
			if(m_StagePoint[i] == null)
				continue;
			else m_StagePoint[i].SetState(m_state ,i , m_StagePoint.Count);
		}
	}
}
