using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// 事件中心
/// </summary>
/// <author>zhulin</author>
/// 
/// <summary>
/// 通知事件
/// </summary>
/// <param name="SceneID">对象SceneID</param>
/// <param name="Param">核心参数</param>
/// <param name="list">参数列表</param>
public delegate void EventHook(int SceneID,object Param);
public enum NDEventType{
	Attr_Anger      = 0,    //怒气更新
	Attr_FullAnger  = 1,    //怒气满了
	StatusCG        = 2,    //状态变更
	StatusInterrupt = 3,    //打断状态
	Attr_HP			= 4,	//血量更新
    Attr_AngerRelease =5,   //怒气可以释放（技能可以释放）
	Attr_Paralysis  = 6,    //麻痹属性
	Attr_Wake       = 7     //
};

public class EventCenter  {

	private static Dictionary<NDEventType,List<EventHook>> m_EventHook = new Dictionary<NDEventType,List<EventHook>>();



	public static void DoEvent(NDEventType evt, 
	                           int SceneID,
	                           object Param)
	{
		CheckValid();
		if(m_EventHook.ContainsKey(evt))
		{
			List<EventHook> l = m_EventHook[evt];
			if(l== null || l.Count == 0)
				return;
			//foreach(EventHook f in l)
            for (int i = 0; i < l.Count; i++ )
            {
                EventHook f = l[i];
                if (f != null)
                {
                    f(SceneID, Param);
                }
            }
		} 
	}
	/// <summary>
	/// 注册事件
	/// </summary>
	public static void RegisterHooks(NDEventType  evt,EventHook evf )
	{
		if(evf == null) return;
		CheckValid();
		if(m_EventHook.ContainsKey(evt))
		{
			List<EventHook> l = m_EventHook[evt];
			l.Add(evf);
		}
		else
		{
			List<EventHook> l = new List<EventHook>();
			l.Add(evf);
			m_EventHook.Add(evt,l);
		}
	}

	/// <summary>
	/// 反注册事件
	/// </summary>
	public static void AntiRegisterHooks(NDEventType  evt,EventHook evf)
	{
		if(evf == null) return;
		CheckValid();
		if(m_EventHook.ContainsKey(evt))
		{
			List<EventHook> l = m_EventHook[evt];
			if(l== null || l.Count == 0)
			{
				m_EventHook.Remove(evt);
				return;
			}
			if(l.Contains(evf))
			{
				l.Remove(evf);
			}
			if(l.Count == 0)
			{
				m_EventHook.Remove(evt);
			}
		}
	}

	/// <summary>
	/// 反注册事件
	/// </summary>
	public static void AntiAllRegisterHooks()
	{
		m_EventHook.Clear();
	}

	private static void CheckValid()
	{
		if(m_EventHook== null)
			m_EventHook = new Dictionary<NDEventType,List<EventHook>>();
	}
}
