using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemUseConfirmWnd : WndBase 
{
	public ItemUseConfirmWnd_h MyHead 
	{
		get
		{ 
			return (base.BaseHead () as ItemUseConfirmWnd_h);	
		}	
	}

	private List<ItemTypeInfo> m_litem = new List<ItemTypeInfo>(); 
	private int m_stype;
	private bool m_send = false;
		
	public override void  WndStart()
 	{
		base.WndStart(); 
		MyHead.BtnConfirm.OnClickEventHandler += BtnConfirm_OnClickHander;
    }

	/// <summary>
	/// 按品阶排序
	/// </summary>
	public void SetData(int stype,List<ItemTypeInfo> litem)
	{
		m_stype = stype;
		m_litem = litem;

		m_litem.Sort((a, b) =>{
			
			if(a.m_Quality > b.m_Quality)
				return -1;
			else if(a.m_Quality < b.m_Quality)
				return 1;
			else return 0;});

		SetUI();
	}

	private void SetUI()
	{
		int total = 0;
		foreach (ItemTypeInfo item in m_litem) 
		{
			GameObject go = NDLoad.LoadWndItem("ItemUseConfirmItem", MyHead.ItemList.transform);
			if (go != null)
			{
				ItemUseConfirmItem itemgo = go.GetComponent<ItemUseConfirmItem>();
				if(itemgo != null)
				{
					itemgo.SetItem(item);

				}
			}
			total += int.Parse(item.m_args) * item.Num;
		}
		if(m_stype == 44)
		{
			MyHead.SprRes1.spriteName = "icon011";
		}
		else if(m_stype == 45)
		{
			MyHead.SprRes1.spriteName = "icon013";
		}
		MyHead.LblRes1Num.text = "X" + total;
	}

	void BtnConfirm_OnClickHander(UIButton sender)
	{
		if(m_send == false)
		{
			m_send = true;
			List<KeyValue> litem = new List<KeyValue>();
			foreach (ItemTypeInfo item in m_litem) 
			{
				KeyValue v = new KeyValue();
				v.key = item.ID;
				v.value = item.Num;
				litem.Add(v);
			}
			ItemDC.SendItemBatchUseRequest(litem,UserDC.GetUserID());
			Invoke("closewnd" ,0.1f);
		}
	}

	private void closewnd()
	{
		WndManager.DestoryDialog<ItemUseConfirmWnd>();
	}
}