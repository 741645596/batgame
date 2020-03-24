using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using sdata;

[System.Flags]
/// <summary>
/// 物品类型
/// </summary>
public enum ItemType
{
	ZhuangBei = 1 << 1,
	JuanZhou = 1 << 2,
	LingHunShi = 1 << 3,
	XiaoHaoPin = 1 << 4,
	Lingjian   = 1 << 5,
	All = ZhuangBei | JuanZhou | LingHunShi | XiaoHaoPin | Lingjian,
}

public class  ItemTypeInfo 
{
	private int m_itemID ;
	public int ID
	{
		get{return m_itemID;}
		set{if(value != -1)
			m_itemID = value;}
	}


	private int m_itemTypeID ;
	public int itemType
	{
		get{return m_itemTypeID;}
		set{if(value != -1)
			m_itemTypeID = value;}
	}

	private int m_Num ;
	public int Num
	{
		get{return m_Num;}
		set{if(value != -1)
			m_Num = value;}
	}

	private int m_Position ;
	public int Positon
	{
		get{return m_Position;}
		set{if(value != -1)
			m_Position = value;}
	}


	public void SetType(int Type)
	{
		if(Type == 1)
		{
			m_type = ItemType.ZhuangBei ;
		}
		else if(Type == 2)
		{
			m_type = ItemType.JuanZhou ;
		}
		else if(Type == 3)
		{
			m_type = ItemType.LingHunShi ;
		}
		else if(Type == 4)
		{
			m_type = ItemType.XiaoHaoPin ;
		}
		else if(Type == 6)
		{
			m_type = ItemType.Lingjian ;
		}
	}
	

	public ItemType m_type;
	public int m_stype;
	public int m_Icon;
	public int m_Quality;
	public string m_Name;
	public int m_Supperpose_limit;
	public int m_isuser;
	public int m_level;
	public int m_money;
	public int m_emoney;
	public int m_sellemoney;
	public string m_title;
	public string m_message;
	public int m_gtype;
	public int m_gid;
	public string m_func ;
	public string m_args ;



	public AddAttrInfo m_AddAttr = new AddAttrInfo ();
	

	public ItemTypeInfo()
	{
		
	}
}
