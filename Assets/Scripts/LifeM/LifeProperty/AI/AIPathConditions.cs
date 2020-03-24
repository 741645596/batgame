using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 重新寻路条件
/// </summary>
/// <author>zhulin</author>
/// 
public enum AIEvent{
	//优先级由高到低定义宏值
	EVENT_SELFSP = 0,  //只关联个体强制寻路 
    //群体刷新路径
	EVENT_MAP =10, //地图更新
	EVENT_CHANGETARGET = 11,
	EVENT_ADD =12, //新角色加入战ming场
	EVENT_TD  =13, //目标死亡
	EVENT_CS  =14, //目标状态发生变更
	EVENT_FMA =15, //目标不可攻击状态完成
	EVENT_INA =16, //目标进入不可攻击状态
	EVENT_TM  =17, //目标移动


};

public  class AIEventData{
	public AIEventData(AIEvent e)
	{
		Event = e;
	}
	public AIEvent Event;
	public Int2 para1;
	public Int2 para2;
	//其他参数根据业务需要待定
};

public class AIPathConditions  {
	
	//Event 事件类型
	/// <summary>
	/// 群体寻路事件
	/// </summary>
	public static void AIPathEvent(AIEventData data , List<Life> list)
	{
		SetUpdataPathFlag(list,data);
	}

	/// <summary>
	/// 个体寻路事件
	/// </summary>
	public static void AIPathEvent(AIEventData data , Life life)
	{
		switch(data.Event)
		{
		case AIEvent.EVENT_SELFSP:
			SetSelfPathFlag(life,data);
			break;
		case AIEvent.EVENT_CHANGETARGET:
			SetSelfPathFlag(life,data);
			break;
		default:
			break;
		}
	}

	// true data1 大。
	public static bool ComareEvent(AIEventData data1,AIEventData data2)
	{
		if (data1.Event > data2.Event)
			 return false;
		else return true;
	}

	
	private static void SetUpdataPathFlag(List<Life> RoleList ,AIEventData data)
	{
		if (RoleList == null || RoleList.Count == 0)
						return;

		foreach (Life life in RoleList) 
		{
			SetSelfPathFlag(life,data);
		}
	}

	private static void SetSelfPathFlag(Life life ,AIEventData data)
	{
		if(life == null ) return ;
		if(life is Role)
		{
			Role w = life as Role;
			RoleGridRun m= w.run;
			if(m != null)
			{
				m.SetUpdataPath(data);
			}
		}
		if(life is Pet)
		{
			Pet w = life as Pet;
			if(w.PetMoveAI != null && w.PetMoveAI is PetWalk1002 )
			{
				PetGridRun m= (w.PetMoveAI as PetWalk1002).m_run;
				m.SetUpdataPath(data);
			}
		}
	}

}
