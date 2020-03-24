using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 战斗姿态同步数据中心
/// </summary>
/// <author>zhulin</author>
public class BSsyncD  {

	private static Dictionary<int,LifeInfo> m_lLifeInfo = new Dictionary<int,LifeInfo>();
	private static List<BSsyncDTask> m_lTask = new List<BSsyncDTask>();
	
	/// <summary>
	/// 数据同步中心帧执行
	/// </summary>
	public static void Update(float deltaTime)
	{
		if(m_lTask == null || m_lTask.Count ==0)
			return ;
		for(int i = m_lTask.Count -1; i >=0 ; i --)
		{
			if(m_lTask[i] != null)
			{
				if(m_lTask[i].Update(deltaTime) == true)
				{
					m_lTask.RemoveAt(i);
				}
			}
		}
	}	
	/// <summary>
	/// 炮弹兵请求寻路
	/// </summary>
	public static void DoBornRequest(int DataID ,tga.SoldierBornRequest Info)
	{
		if(Info == null) return ;

		AddLifeInfo(DataID ,Info.info);


		BSsyncDTask task = new BSsyncDTask();
		task.cmd = new BscCmd(DataID,BSCEventType.BSC_Born);
		task.RemainingTime = Random.Range(0.01f,0.1f);
		tga.SoldierBornResponse sresponse = new tga.SoldierBornResponse();
		sresponse.time = Info.time;
		sresponse.time.servertime = Time.realtimeSinceStartup ;
		sresponse.time.Responsetime = sresponse.time.servertime + task.RemainingTime;
		task.Info = sresponse;
		m_lTask.Add(task);
	}
	/// <summary>
	/// 炮弹兵请求寻路
	/// </summary>
	public static void DoDeadRequest(int DataID ,tga.SoldierDeadRequest Info)
	{
		if(Info == null) return ;

		DelLifeInfo(DataID);

		BSsyncDTask task = new BSsyncDTask();
		task.cmd = new BscCmd(DataID,BSCEventType.BSC_Dead);
		task.RemainingTime = Random.Range(0.01f,0.1f);
		tga.SoldierDeadResponse sresponse = new tga.SoldierDeadResponse();
		sresponse.time = Info.time;
		sresponse.time.servertime = Time.realtimeSinceStartup ;
		sresponse.time.Responsetime = sresponse.time.servertime + task.RemainingTime;
		task.Info = sresponse;
		m_lTask.Add(task);
	}
	/// <summary>
	/// 炮弹兵请求寻路
	/// </summary>
	public static void DoRunRoadRequest(int DataID ,tga.SoldierRunRoadRequest Info)
	{
		if(Info == null) return ;
		UpdataLifeInfo(DataID,Info.info);
		BSsyncDTask task = new BSsyncDTask();
		task.cmd = new BscCmd(DataID,BSCEventType.BSC_RunRoad);
		task.RemainingTime = Random.Range(0.01f,0.1f);
		tga.SoldierRunRoadResponse sresponse = new tga.SoldierRunRoadResponse();
		sresponse.end = Info.end;
		sresponse.start = Info.start;
		for(int i = 0; i < Info.roadlist.Count; i++)
			sresponse.roadlist.Add(Info.roadlist[i]);
		sresponse.time = Info.time;
		sresponse.time.servertime = Time.time ;
		sresponse.time.Responsetime = sresponse.time.servertime + task.RemainingTime;
		task.Info = sresponse;
		m_lTask.Add(task);
	}
	/// <summary>
	/// 炮弹兵请求技能攻击
	/// </summary>
	public static void DoSkillAttackRequest(int DataID ,tga.SoldierSkillAttackRequest Info)
	{
		if(Info == null) return ;
		UpdataLifeInfo(DataID,Info.info);
		BSsyncDTask task = new BSsyncDTask();
		task.cmd = new BscCmd(DataID,BSCEventType.BSC_RunRoad);
		task.RemainingTime = Random.Range(0.01f,0.1f);
		tga.SoldierSkillAttackResponse sresponse = new tga.SoldierSkillAttackResponse();
		for(int i = 0; i < Info.SkillObjlist.Count; i++)
			sresponse.SkillObjlist.Add(Info.SkillObjlist[i]);
		sresponse.time = Info.time;
		sresponse.time.servertime = Time.realtimeSinceStartup ;
		sresponse.time.Responsetime = sresponse.time.servertime + task.RemainingTime;
		task.Info = sresponse;
		m_lTask.Add(task);
	}

	/// <summary>
	/// 添加lifeinfo信息
	/// </summary>
	private static void AddLifeInfo(int DataID ,tga.lifestateinfo Info)
	{
		if(Info == null) return ;
		if(m_lLifeInfo.ContainsKey (DataID) == false)
		{
			LifeInfo I = new LifeInfo();
			I.SetLifeInfo(Info);
			m_lLifeInfo.Add(DataID, I);
		}
	}

	/// <summary>
	/// 更新lifeinfo信息
	/// </summary>
	private static void UpdataLifeInfo(int DataID ,tga.lifestateinfo Info)
	{
		if(Info == null) return ;
		if(m_lLifeInfo.ContainsKey (DataID) == true)
		{
			LifeInfo I = m_lLifeInfo[DataID];
			if(I != null)
				I.SetLifeInfo(Info);
		}
	}



	/// <summary>
	/// 删除
	/// </summary>
	private static void DelLifeInfo(int DataID )
	{
		if(m_lLifeInfo.ContainsKey (DataID) == true)
		{
			m_lLifeInfo.Remove(DataID);
		}
	}

	/// <summary>
	/// 清空数据
	/// </summary>
	public static void Clear()
	{
		m_lLifeInfo.Clear();
		m_lTask.Clear();
	}

}





/// <summary>
/// life信息数据
/// </summary>
public class LifeInfo  {
	public int   hp;
	public Int2  MapPos; 

	/// <summary>
	/// 设置玩家用户信息
	/// </summary>
	public void SetLifeInfo(tga.lifestateinfo Info)
	{
		if(Info == null)
			return ;
		this.hp = Info.hp;
		if(Info.CurPos != null)
		{
			this.MapPos.Layer = Info.CurPos.layer;
			this.MapPos.Unit = Info.CurPos.unit;
		}
	}
}


public class BSsyncDTask  {
	public float  RemainingTime ;
	public BscCmd cmd;
	public object Info; 
	/// <summary>
	/// 数据同步中心帧执行
	/// </summary>
	public bool Update(float deltaTime)
	{
		RemainingTime -= deltaTime;
		if(RemainingTime <= 0)
		{
			BSC.ProcessData(cmd , Info);
			return true;
		}
		return false;
	}
}