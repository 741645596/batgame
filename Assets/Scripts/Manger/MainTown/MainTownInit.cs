using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 表示主城所处状态
/// </summary>
public enum MainTownState
{
	None,
	/// <summary>
	/// 船只编辑
	/// </summary>
	CanvasEdit,
	
	Treasure,
	
	TreasureCanvasEdit,
	
	StageMap,
	
	/// <summary>
	/// 海神杯.
	/// </summary>
	Athletics,
	/// <summary>
	/// 主菜单-任务
	/// </summary>
	MainMenuMission,
	/// <summary>
	/// 主菜单-炮弹兵背包
	/// </summary>
	MainMenuPdbbb,
	/// <summary>
	/// 主菜单-陷阱背包
	/// </summary>
	MainMenuTrapBb,
	/// <summary>
	/// 主场景金银岛可复仇日志.
	/// </summary>
	TreasureFailLog,
	
}
/// <summary>
/// 主城现场上下文
/// </summary>
public enum MainTownContext
{
	None,
}
/// <summary>
/// 表示主城所处状态
/// </summary>
public enum MainTownFunc
{
	None,
}



/// <summary>
/// 主城环境
/// </summary>
/// <author>zhulin</author>
public class MainTownInit  {

	/// <summary>
	/// 当前所在窗口状态
	/// </summary>
	public static  MainTownState s_currentState = MainTownState.None;
	private static MainTownFunc m_FuncData = MainTownFunc.None;
	private static MainTownContext m_Context = MainTownContext.None ;
	/// <summary>
	/// 保护主城现场
	/// </summary>
	public static void SaveMainTownContext(MainTownContext Context)
	{
		m_Context = Context;
	}
	/// <summary>
	/// 清空主城环境
	/// </summary>
	public static void EmptyMainTownContext()
	{
		m_Context = MainTownContext.None;
	}
	/// <summary>
	/// 获取主城现场
	/// </summary>
	public static MainTownContext GetMainTownContext()
	{
		return m_Context ;
	}
	/// <summary>
	/// 清空主城现场
	/// </summary>
	public static void SetOpenFuncData(MainTownFunc data)
	{
		m_FuncData = data;
	}
	/// <summary>
	/// 清空主城现场
	/// </summary>
	public static MainTownFunc GetOpenFuncData()
	{
		return m_FuncData;
	}
}
