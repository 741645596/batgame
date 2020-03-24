using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate bool InVisionHook(Life life);

public class RadarAI  {

	//雷达视野
	protected InVisionHook m_fCheckInVision = null;
	//搜查目标列表，用于缓存。
	protected List<Life> mTargetlist = new List<Life>();

	/// <summary>
	/// 设置雷达的视野
	/// </summary>
	/// <param name="TargetSceneID">优先考虑的寻路目标</param>
	public  void SetVision(InVisionHook f)
	{ 
		m_fCheckInVision = f;
	}

	


	/// <summary>
	/// 获取寻路目标
	/// </summary>
	/// <param name="TargetSceneID">优先考虑的寻路目标</param>
	/// <param name="AttackLike">攻击喜好</param>
	/// <param name="Attacked">是否已经惊醒</param>
	/// <returns>寻路对象SceneID ,-1 表示没有寻路目标</returns>
	public  int GetSearchTarget(int TargetSceneID,MapGrid Pos,int AttackLike)
	{
		//优先选择指定目标
		if(PriorityTarget (TargetSceneID) == true)
			return TargetSceneID;
		//根据喜好获取目标列表
		GetTargetList(AttackLike);
		//获取距离最短的目标
		return GetShortTarget(Pos);
	}
	public Life GetTarget(PathRoad path,int AttackLike)
	{
		GetTargetList(AttackLike);
		for(int i = 0; i <  path.SearchPath.Count; i++)
		{
			foreach(Life l in mTargetlist)
			{
				List<MapGrid> allg = l.GetAllMapGrid();
				foreach (MapGrid g in allg)
				{
					if (g == path.SearchPath[i].Road)
					{
						path.SearchPath.RemoveRange(i+1,path.SearchPath.Count - i-1);
						return l;
					}
				}
			}
		}
		return CM.GoldBuild;
	}


	/// <summary>
	/// 根据喜好，获取目标列表，裁剪掉不在视野范围的目标
	/// </summary>
	protected virtual void GetTargetList(int AttackLike)
	{

	}

	/// <summary>
	/// 选择优先目标
	/// </summary>
	protected bool PriorityTarget(int TargetSceneID)
	{
		if(TargetSceneID != -1)
		{
			return true;
		}
		return false;
	}
	/// <summary>
	/// 选择路径最短的目标
	/// </summary>
	protected int GetShortTarget(MapGrid Pos)
	{
		if (mTargetlist.Count == 0) 
			return -1 ;
		if (mTargetlist.Count == 1) 
			return mTargetlist[0].SceneID ;
		
		List<MapGrid> list = new List<MapGrid> ();
		for(int i = 0; i <  mTargetlist.Count ; i ++)
		{
			MapGrid m = mTargetlist[i].GetMapGrid();
			if(m == null)
			{
				Debug.Log("lifeM 获取格子为NULL 请调查");
				continue;
			}
			list.Add(m);
		}
		
		int Shortest = MapPath.FindShortestTarget (list , Pos);
		if (Shortest >= 0)
			return  mTargetlist[Shortest].SceneID ;
		return -1;
	}

	/// <summary>
	/// 获取在视野范围内的对象
	/// </summary>
	protected void CheckInVisionAttack(List<Life> list)
	{
		mTargetlist.Clear();
		if(list == null || list.Count == 0)
			return ;

		foreach( Life l in list)
		{
			if(m_fCheckInVision == null )
				continue;
			if(m_fCheckInVision(l)== true )
			{
				mTargetlist.Add(l);
			}
		}
	}
	

}


public class RadarAIFactory
{
	/// <summary>
	/// 创建Radar AI
	/// </summary>
	/// <param name="Core">雷达拥有者的核心信息</param>
	/// <param name="f">雷达拥有视野</param>
	public static RadarAI Create(LifeMCore Core,InVisionHook f )
	{
		if(f == null || Core == null)
			return null;

		if(Core.m_type == LifeMType.SOLDIER || Core.m_type == LifeMType.SUMMONPET)
		{
			if(Core.m_Camp == LifeMCamp.ATTACK)
			{

				AttackRadarAI AI = new AttackRadarAI();
				AI.SetVision(f);
				return AI;
			}
			else if(Core.m_Camp == LifeMCamp.DEFENSE)
			{
				DefenseRadarAI AI = new DefenseRadarAI();
				AI.SetVision(f);
				return AI;
			}
			else return null;
		}
		return null;
	}

}
