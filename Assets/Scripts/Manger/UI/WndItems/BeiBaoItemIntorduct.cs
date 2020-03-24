using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BeiBaoItemIntorduct : MonoBehaviour 
{
	public BeiBaoItemIntorduct_h MyHead 
	{
		get
		{ 
			return GetComponent<BeiBaoItemIntorduct_h>();;	
		}	
	}
	private ItemTypeInfo m_ItemInfo;
	private BeiBaoWnd m_parent;
	
	void Start ()
	{
	   	if (MyHead.BtnSell)
		{
			MyHead.BtnSell.OnClickEventHandler += BtnSell_OnClickEventHandler;
		}
		if (MyHead.BtnDetail)
		{   
			MyHead.BtnDetail.OnClickEventHandler += BtnDetail_OnClickEventHandler;
		}
		RegisterHooks();
		SetUI ();
	}
	void BtnDetail_OnClickEventHandler(UIButton sender)
	{
		if(m_ItemInfo == null || m_parent == null)
			return;
		if(m_ItemInfo.m_isuser == 1)
		{
			if(m_ItemInfo.m_func == "Item_AddSoldierExp")
			{
				AddExpWnd wnd = WndManager.GetDialog<AddExpWnd>();
				wnd.SetData(m_ItemInfo,m_parent);
			}
			else
			{
				ItemDC.Send_ItemUseRequest (m_ItemInfo.ID,1,UserDC.GetUserID());
			}
		}
		else
		{
			EquipInfoWnd wnd = WndManager.GetDialog<EquipInfoWnd>();
			wnd.SetData (m_ItemInfo);
		}

	}
	void BtnSell_OnClickEventHandler(UIButton sender)
	{
		if(m_ItemInfo == null || m_parent == null)
			return;
		EquipSellWnd wnd = WndManager.GetDialog<EquipSellWnd>();
		wnd.SetData (m_ItemInfo,m_parent);
	}
	public void SetData(ItemTypeInfo info,BeiBaoWnd wnd)
	{
		m_parent = wnd;
		m_ItemInfo = info;
	}
	private void SetUI()
	{
		if(m_ItemInfo == null) return;
		//
		if(m_ItemInfo.m_isuser == 1) 
			MyHead.LblDetail.text = NGUIUtil.GetStringByKey(70000060);
		else MyHead.LblDetail.text = NGUIUtil.GetStringByKey(70000061);

		if(MyHead.Spr2dIcon != null )
			NGUIUtil.Set2DSprite(MyHead.Spr2dIcon, "Textures/item/",m_ItemInfo.m_Icon.ToString());
		//设置物品品阶.
		if(MyHead.SprQuality != null)
			NGUIUtil.SetSprite(MyHead.SprQuality, ConfigM.GetBigQuality(m_ItemInfo.m_Quality).ToString());
		
		
		MyHead.LblItemName.text = m_ItemInfo.m_Name;

        MyHead.LblItemNum.text = string.Format("[552d0a]" + NGUIUtil.GetStringByKey("88800040") + "[-] [FFFFFF]{0}[-] [552d0a]" + NGUIUtil.GetStringByKey("88800037") + "[-]", m_ItemInfo.Num);
		
		MyHead.LblItemSellCoin.text = m_ItemInfo.m_sellemoney.ToString ();
		if(m_ItemInfo.m_message != null || m_ItemInfo.m_message.Length > 0)
		{
			MyHead.LblItemEffDes.text = m_ItemInfo.m_message;
		}
		MyHead.LblItemDes.overflowMethod = UILabel.Overflow.ResizeHeight;

		m_ItemInfo.m_title = m_ItemInfo.m_title.Replace("\\n", System.Environment.NewLine);
		MyHead.LblItemDes.text = string.Format (m_ItemInfo.m_title) +"\n";

		MyHead.Table.Reposition (); 
	}

	public void RefreshUI()
	{
		SetUI ();
	}

	/// <summary>
	/// 注册事件
	/// </summary>
	public   void RegisterHooks()
	{
		DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_804, Recv_ItemUseRespone);
	}
	
	/// <summary>
	/// 反注册事件
	/// </summary>
	public   void AntiRegisterHooks()
	{
		DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_804, Recv_ItemUseRespone);
	}

	void Recv_ItemUseRespone(int iErrorCode)
	{
		if(iErrorCode == 0)
		{
//			if(m_ItemInfo.m_func == "Item_AddWood")
//			{
//				Debug.Log("addwood" + m_ItemInfo.m_args);
//				string show = string.Format(NGUIUtil.GetStringByKey(10000179),NGUIUtil.GetStringByKey(10000178)) + m_ItemInfo.m_args.ToString();
//				m_parent.AddHudText(show);
//			}
//			else if(m_ItemInfo.m_func == "Item_AddCrystal")
//			{
//				Debug.Log("AddCrystal" + m_ItemInfo.m_args);
//				string show = string.Format(NGUIUtil.GetStringByKey(10000179),NGUIUtil.GetStringByKey(10000177)) + m_ItemInfo.m_args.ToString();
//				m_parent.AddHudText(show);
//			}
		}
	}

	public void OnDestroy()
	{
		AntiRegisterHooks();
	}
}