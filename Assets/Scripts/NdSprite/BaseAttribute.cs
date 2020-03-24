using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AttrKeyName
{
	public const string Pause_Bool = "Pause";
	public const string Speed_Float = "Speed";
}

public class BaseAttribute {
	public NdSprite m_Owner;
	public Dictionary<string,object> m_dic = new Dictionary<string, object>();

	public BaseAttribute()
	{
		m_dic[AttrKeyName.Pause_Bool] = false;
		m_dic[AttrKeyName.Speed_Float] = 1f;
	}
	public virtual bool TryGetValue(string key,ref object value)
	{
		return m_dic.TryGetValue(key,out value);
	}

	public virtual void SetValue(string key, object value)
	{
		if (m_dic.ContainsKey(key))
		{
			AttrEventCenter.DoEvent(key,value);
			m_dic[key] = value;
		}
	}

	public virtual object GetValue(string key)
	{
		return m_dic[key];
	}

}
