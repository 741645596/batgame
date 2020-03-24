using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// 属性变更事件中心
/// </summary>
/// <author>zhulin</author>
/// 
/// <summary>
/// 通知事件
/// </summary>
/// <param name="SceneID">对象SceneID</param>
/// <param name="Param">核心参数</param>
/// <param name="list">参数列表</param>
public delegate void AttrEventHook(string attr,object value);

public class AttrEventCenter  {

	private static Dictionary<string,List<AttrEventHook>> m_EventHook = new Dictionary<string,List<AttrEventHook>>();



	public static void DoEvent(string evt,object value)
	{
		CheckValid();
		if(m_EventHook.ContainsKey(evt))
		{
			List<AttrEventHook> l = m_EventHook[evt];
			if(l== null || l.Count == 0)
				return;
			foreach(AttrEventHook f in l)
			{
				if(f != null)
				{
					f(evt,value);
				}
			}
		} 
	}
	/// <summary>
	/// 注册事件
	/// </summary>
	public static void RegisterHooks(string  evt,AttrEventHook evf )
	{
		if(evf == null) return;
		CheckValid();
		if(m_EventHook.ContainsKey(evt))
		{
			List<AttrEventHook> l = m_EventHook[evt];
			l.Add(evf);
		}
		else
		{
			List<AttrEventHook> l = new List<AttrEventHook>();
			l.Add(evf);
			m_EventHook.Add(evt,l);
		}
	}

	/// <summary>
	/// 反注册事件
	/// </summary>
	public static void AntiRegisterHooks(string  evt,AttrEventHook evf)
	{
		if(evf == null) return;
		CheckValid();
		if(m_EventHook.ContainsKey(evt))
		{
			List<AttrEventHook> l = m_EventHook[evt];
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

	private static void CheckValid()
	{
		if(m_EventHook== null)
			m_EventHook = new Dictionary<string,List<AttrEventHook>>();
	}
}
