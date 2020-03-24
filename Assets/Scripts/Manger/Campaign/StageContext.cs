using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using sdata;
/// <summary>
/// 战役副本章节内容
/// <summary>
/// <Author>zhulin</Author>
public class StageContext : MonoBehaviour {

	public  List<StageNode> m_StageNode = new List<StageNode>();
	public  List<StageLine> m_StageLine = new List<StageLine>();
	public  UILabel m_lbStageName;
	private List<CounterPartInfo> m_CounterInfo = new List<CounterPartInfo>();
	
	/// <summary>
	/// 初始化副本数据
	/// </summary>
	public void SetStageContext(StageType Type ,int Chapter)
	{
		m_CounterInfo = StageDC.GetChaptersGate(Chapter,Type) ;

		if(m_CounterInfo == null || m_CounterInfo.Count == 0)
		{
			Debug.LogError("s_counterpartInfo is error");
			return ;
		}
		if(m_StageNode == null || m_StageNode.Count == 0)
		{
			Debug.LogError("m_StageNode is error");
			return ;
		}

		if(m_StageNode.Count < m_CounterInfo.Count)
		{
			NGUIUtil.ShowFreeSizeTipWnd(20000001, null, 30);
			return ;
		}

		foreach(StageNode Node in m_StageNode)
		{
			if(Node == null)
			{
				Debug.LogError("Node is error");
				continue ;
			}
			else
			{
				CounterPartInfo Info = FindCounterpartInfo(Type , Chapter , Node.m_Stage);
				if(Info == null)
				{
					Node.gameObject.SetActive(false);
					continue ;
				}
				else Node.SetStageData(Type ,Chapter , Info);
			}
		}

		if(m_lbStageName != null)
		{
			m_lbStageName.text = "[ffffff]"+NGUIUtil.GetStringByKey("88800092")+ m_CounterInfo[0].chapters.ToString() + NGUIUtil.GetStringByKey("88800093")
				+ " " + m_CounterInfo[0].chaptersname ;
		}
		else
		{
			Debug.LogError("m_lbStageName is null");
		}


		foreach(StageLine line in m_StageLine)
		{
			if(line == null)
			{
				Debug.LogError("Node is error");
				continue ;
			}
			else
			{
				line.SetStageData(Type , Chapter);
			}
		}
	}
	/// <summary>
	/// 查找对应的副本数据
	/// <summary>
	private CounterPartInfo FindCounterpartInfo(StageType Type ,int Chapter,int StageID)
	{
		foreach(CounterPartInfo Info in m_CounterInfo)
		{
			if(Info.id == StageM.GetStageID (Type , Chapter ,StageID ))
			{
				return Info ;
			}
		}
		return null;
	}

	/// <summary>
	/// 查找章节对象
	/// </summary>
	public StageNode FindStageNode(int Stage)
	{
		foreach (StageNode n in m_StageNode)
		{
			if(n != null && n.m_Stage == Stage)
			{
				return n;
			}
		}
		return null;
	}
}
