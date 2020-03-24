using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AddExpWnd : WndBase 
{
	public AddExpWnd_h MyHead 
	{
		get
		{ 
		 return (base.BaseHead () as AddExpWnd_h);	
		}	
	}
	/// <summary>
	/// 要使用的物品ID
	/// </summary>
	private ItemTypeInfo m_ItemInfo;
	private BeiBaoWnd m_parent;

    public override void WndStart()
    {
 		base.WndStart(); 
		MyHead.BtnClose.OnClickEventHandler += BtnClose_OnClickHander;
		WndEffects.DoWndEffect(gameObject);
	}

	public void SetData(ItemTypeInfo ItemInfo,BeiBaoWnd wnd)
	{
		m_ItemInfo = ItemInfo;
		m_parent = wnd;
        SetUI();
	}



	private void SetUI()
	{
		List<SoldierInfo> lList = new List<SoldierInfo> ();
		SoldierDC.GetSoldiers (ref lList,CombatLoactionType.ALL);
		SoldierM.SortSoldierList(ref lList);

		for(int i = 0;i < lList.Count; i++)
		{
            GameObject go = NDLoad.LoadWndItem("AddExpItem", MyHead.Table.transform);
			if (go != null)
			{
				AddExpItem item = go.GetComponent<AddExpItem>();
				if(item != null)
				{
					item.SetData(lList[i],m_ItemInfo,this);
				}
			}
		}
		MyHead.Table.Reposition ();
	}

	public int GetItemNum()
	{
		return m_ItemInfo.Num;
	}
	public void SetItemNum()
	{
		m_ItemInfo.Num -= 1;
	}

	/// <summary>
	/// 发送物品使用申请，使用结果在BeiBaoWnd中监听处理.
	/// </summary>
	/// <param name="ItemId">Item identifier.</param>
	/// <param name="ItemNum">Item number.</param>
	/// <param name="SoldierId">Soldier identifier.</param>
	public void SendUseItem(int ItemId,int ItemNum,int SoldierId)
	{
		ItemDC.Send_ItemUseRequest (ItemId,ItemNum,SoldierId);
	}

	void BtnClose_OnClickHander(UIButton sener)
	{
		WndEffects.DoCloseWndEffect(gameObject,DestoryDialogCallBack);
	}
	void DestoryDialogCallBack(object o)
	{
		WndManager.DestoryDialog<AddExpWnd>();		
	}


}