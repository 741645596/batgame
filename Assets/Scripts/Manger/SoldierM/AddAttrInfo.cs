using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using sdata;

/// <summary>
/// 附加属性信息
/// </summary>
/// <author>zhulin</author>
public enum AddAttrType{
	Main  = 0 , //主附加属性
	Skill = 1 , //被动技能加属性
	Equip = 2 , //装备附加属性 
}
public class AddAttrInfo
{
	/// <summary>
	/// 主附加属性
	/// </summary>
	private Dictionary<int,int> m_MainAttr  = new Dictionary<int,int>(); 
	/// <summary>
	/// 被动技能附加属性
	/// </summary>
	private Dictionary<int,int> m_SkillAttr  = new Dictionary<int,int>(); 
	/// <summary>
	/// 装备附加属性
	/// </summary>
	private Dictionary<int,int> m_EquipAttr  = new Dictionary<int,int>(); 
	/// <summary>
	/// 获取装备附加属性
	/// </summary>
	public Dictionary<int,int> GetEquipAddAttr()
	{
		return m_EquipAttr;
	}
	/// <summary>
	/// 获取附加属性
	/// </summary>
	public Dictionary<int,int> GetAddAttr()
	{
		Dictionary<int,int> l = new Dictionary<int, int>();
		//先添加主属性
		foreach(int key in m_MainAttr.Keys)
		{
			if(l.ContainsKey (key) == false)
			{
				l.Add(key,m_MainAttr[key]);
			}
			else
			{
				l[key] = l[key] +  m_MainAttr[key];
			}
		}

		//添加被动技能附加属性
		foreach(int key in m_SkillAttr.Keys)
		{
			if(l.ContainsKey (key) == false)
			{
				l.Add(key,m_SkillAttr[key]);
			}
			else
			{
				l[key] = l[key] +  m_SkillAttr[key];
			}
		}
		//再添加装备属性
		foreach(int key in m_EquipAttr.Keys)
		{
			if(l.ContainsKey (key) == false)
			{
				l.Add(key,m_EquipAttr[key]);
			}
			else
			{
				l[key] = l[key] +  m_EquipAttr[key];
			}
		}
		return l;
	}



	/// <summary>
	/// 获取指定的附加属性
	/// </summary>
	public int GetAttr(EffectType Type)
	{
		int type = (int)Type ;
		int value = 0 ;
		if(m_MainAttr.ContainsKey (type) == true)
		{
			value += m_MainAttr[type];
		}

		if(m_SkillAttr.ContainsKey (type) == true)
		{
			value += m_SkillAttr[type];
		}

		if(m_EquipAttr.ContainsKey (type) == true)
		{
			value += m_EquipAttr[type];
		}

		return value ;
	}

	/// <summary>
	/// 从附加属性中移除某类属性
	/// </summary>
	public void RemoveAttr(EffectType Type)
	{
		int type = (int)Type ;
		if(m_MainAttr.ContainsKey (type) == true)
		{
			m_MainAttr.Remove(type);
		}

		if(m_SkillAttr.ContainsKey (type) == true)
		{
			m_SkillAttr.Remove(type);
		}
		
		if(m_EquipAttr.ContainsKey (type) == true)
		{
			m_EquipAttr.Remove(type);
		}

	}


	/// <summary>
	/// 设置附加属性
	/// </summary>
	public  void SetAddAttrInfo(s_soldier_typeInfo Info)
	{
		if(Info == null) 
		{
			Debug.LogError("SetAddAttrInfo is null");
			return ;
		}
		AddAttr(AddAttrType.Main,Info.add_attr1,Info.value1);
		AddAttr(AddAttrType.Main,Info.add_attr2,Info.value2);
		AddAttr(AddAttrType.Main,Info.add_attr3,Info.value3);
		AddAttr(AddAttrType.Main,Info.add_attr4,Info.value4);
		AddAttr(AddAttrType.Main,Info.add_attr5,Info.value5);
	}

	/// <summary>
	/// 设置附加属性
	/// </summary>
	public  void SetAddAttrInfo(s_monsterInfo monster)
	{
		if(monster == null) 
		{
			Debug.LogError("SetAddAttrInfo is null");
			return ;
		}
		AddAttr(AddAttrType.Main,monster.data1,monster.value1);
		AddAttr(AddAttrType.Main,monster.data2,monster.value2);
		AddAttr(AddAttrType.Main,monster.data3,monster.value3);
		AddAttr(AddAttrType.Main,monster.data4,monster.value4);
		AddAttr(AddAttrType.Main,monster.data5,monster.value5);
	}
	/// <summary>
	/// 设置技能附加属性
	/// </summary>
	public  void SetAddAttrInfo(List<SoldierSkill> lSkill)
	{
		if(lSkill == null || lSkill.Count == 0) 
		{
			return ;
		}

		foreach(SoldierSkill s in lSkill)
		{
			if (s.m_actiontype == 3 && s.m_enable)
			{
				AddAttr(AddAttrType.Skill,s.m_power1,s.m_power2);
			}
		}
	}

	/// <summary>
	/// 设置附加属性
	/// </summary>
	public  void SetAddAttrInfo(s_summonprosInfo prosinfo)
	{
		if(prosinfo == null) 
		{
			Debug.LogError("SetAddAttrInfo is null");
			return ;
		}

		int datalength = NdUtil.GetLength(prosinfo.data);
		int valuelength = NdUtil.GetLength(prosinfo.value);
		if(datalength != valuelength) 
			return ;
		for(int i= 0; i < datalength; i ++)
		{
			int data = NdUtil.GetIntValue(prosinfo.data ,i);
			int value = NdUtil.GetIntValue(prosinfo.value ,i);
			AddAttr(AddAttrType.Main,data,value);
		}
	}


	/// <summary>
	/// 设置附加属性
	/// </summary>
	public  void SetAddAttrInfo(s_summonpetInfo petinfo)
	{
		if(petinfo == null) 
		{
			Debug.LogError("SetAddAttrInfo is null");
			return ;
		}
		AddAttr(AddAttrType.Main,petinfo.data1,petinfo.value1);
		AddAttr(AddAttrType.Main,petinfo.data2,petinfo.value2);
		AddAttr(AddAttrType.Main,petinfo.data3,petinfo.value3);
		AddAttr(AddAttrType.Main,petinfo.data4,petinfo.value4);
		AddAttr(AddAttrType.Main,petinfo.data5,petinfo.value5);
	}


	/// <summary>
	/// 设置装备附加属性
	/// </summary>
	public  void SetAddAttrInfo(s_itemtypeInfo Info)
	{
		if(Info == null) 
		{
			Debug.LogError("SetAddAttrInfo is null");
			return ;
		}
		AddAttr(AddAttrType.Equip,Info.data0,Info.values0);
		AddAttr(AddAttrType.Equip,Info.data1,Info.values1);
		AddAttr(AddAttrType.Equip,Info.data2,Info.values2);
		AddAttr(AddAttrType.Equip,Info.data3,Info.values3);
		AddAttr(AddAttrType.Equip,Info.data3,Info.values4);
		AddAttr(AddAttrType.Equip,Info.data5,Info.values5);
	}


	/// <summary>
	/// 添加附加属性信息
	/// </summary>
	private void AddAttr(AddAttrType AddType,int type , int Value)
	{
		if(AddType == AddAttrType.Main)
		{
			AddAttr(ref m_MainAttr , type , Value) ;
		}
		else if(AddType == AddAttrType.Skill)
		{
			AddAttr(ref m_SkillAttr , type , Value) ;
		}
		else 
		{
			AddAttr(ref m_EquipAttr , type , Value) ;
		}
	}
	
	
	/// <summary>
	/// 添加附加属性
	/// </summary>
	private void AddAttr(ref Dictionary<int,int> lAttr , int Type , int Value)
	{
		if(lAttr == null)
			return ;
		if(Type <= 0 || Value == 0)
			return ;
		
		if(lAttr.ContainsKey (Type) == false)
		{
			lAttr.Add(Type,Value);
		}
		else
		{
			lAttr[Type] = lAttr[Type] + Value ;
		}
	}
	/// <summary>
	/// 清理附加属性
	/// </summary>
	public void Clear(AddAttrType Type)
	{
		if(Type == AddAttrType.Equip)
			m_EquipAttr.Clear();
	}
}
