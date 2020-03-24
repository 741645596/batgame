using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// 战斗服务中心
/// </summary>
/// <author>zhulin</author>
public delegate void BSCEventHook(object Info);
public enum BSCEventType{
	BSC_Born      = 0,    //对象出生
	BSC_Dead      = 1,    //对象挂了
	BSC_RunRoad   = 2,    //行走线路
};


public class BscCmd
{
	//life 的数据id
	public int DataID;
	//时间类型
	public BSCEventType Type;

	public BscCmd(){}


	public BscCmd( int DataID,BSCEventType Type)
	{
		this.DataID = DataID;
		this.Type = Type;
	}
}

public class BSC  {
	/// <summary>
	///BSC事件
	/// </summary>
	private static Dictionary<BscCmd,BSCEventHook> m_EventHook = new Dictionary<BscCmd,BSCEventHook>();
	/// <summary>
	/// 注册事件
	/// </summary>
	public static void RegisterHooks(BscCmd  cmd , BSCEventHook evt )
	{
		if(cmd == null || evt == null) return;
		BscCmd fcmd = FindBscEventCmd(cmd);
		if(fcmd == null)
		{
			m_EventHook.Add(new BscCmd (cmd.DataID ,cmd.Type) ,evt);
		}
		else
		{
			m_EventHook[fcmd] = evt;
		}
	}
	/// <summary>
	/// 反注册事件
	/// </summary>
	public static void AntiRegisterHooks(BscCmd  cmd , BSCEventHook evt)
	{
		if(cmd == null || evt == null) return;
		BscCmd fcmd = FindBscEventCmd(cmd);
		if(fcmd != null)
		{
			m_EventHook.Remove(fcmd);
		}
	}
	/// <summary>
	/// 反注册事件
	/// </summary>
	public static void AntiAllRegisterHooks()
	{
		m_EventHook.Clear();
		BSsyncD.Clear();
	}
	/// <summary>
	/// 查找事件
	/// </summary>
	private static BscCmd FindBscEventCmd(BscCmd  cmd)
	{
		if(cmd == null ) return null;
		foreach (BscCmd key in m_EventHook.Keys)
		{
			if(key == null) continue ;
			if(key.DataID == cmd.DataID && key.Type == cmd.Type)
			{
				return key;
			}
		}
		return null;
	}
	/// <summary>
	/// 查找事件
	/// </summary>
	private static BSCEventHook FindBscEvent(BscCmd  cmd)
	{
		if(cmd == null ) return null;
		BscCmd fcmd = FindBscEventCmd(cmd);
		if(fcmd != null)
		{
			return m_EventHook[fcmd];
		}
		return null;
	}
	/// <summary>
	/// 客户端模拟模块使用
	/// </summary>
	public static bool ProcessData(BscCmd cmd, object Info)
	{
		BSCEventHook evt = FindBscEvent(cmd);
		if(evt != null)
		{
			evt(Info);
		}
		return true;
	}
	/// <summary>
	/// 真实服务端使用的接口
	/// </summary>
	public static bool ProcessData(int  CmdID,int nErrorCode, object Info)
	{
		return true;
	}
	/// <summary>
	/// 炮弹兵出生请求
	/// </summary>
	public static void SoldierBornRequest(int DataID ,tga.SoldierBornRequest Info)
	{
		BSsyncD.DoBornRequest(DataID ,Info);
	}	
	/// <summary>
	/// 炮弹兵死亡请求
	/// </summary>
	public static void SoldierDeadRequest(int DataID ,tga.SoldierDeadRequest Info )
	{
		BSsyncD.DoDeadRequest(DataID ,Info);
	}

	/// <summary>
	/// 炮弹兵请求寻路
	/// </summary>
	public static void RunRoadRequest(int DataID ,tga.SoldierRunRoadRequest Info)
	{
		BSsyncD.DoRunRoadRequest(DataID ,Info);
	}

	
	/// <summary>
	/// 炮弹兵请求技能攻击
	/// </summary>
	public static void SkillAttackRequest(int DataID ,tga.SoldierSkillAttackRequest Info)
	{
		BSsyncD.DoSkillAttackRequest(DataID ,Info);
	}

}
