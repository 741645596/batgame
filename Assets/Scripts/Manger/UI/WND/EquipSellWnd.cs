using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// <From> </From>
/// <Author>QFord</Author>
/// </summary>
public class EquipSellWnd : WndBase {

	public EquipSellWnd_h MyHead
	{
		get 
		{
			return (base.BaseHead () as EquipSellWnd_h);
		}
	}

	private int m_itemcount = 0 ;
	private ItemTypeInfo m_ItemInfo;
	private BeiBaoWnd m_parent;
	private int m_iSelectNum = 0;

	private bool m_bMouseDown = false;
	private bool m_bMouseUp = false;
	private bool m_bAddClick = false;
	/// <summary>
	/// 鼠标按下时间
	/// </summary>
	private float m_nPressDownTime = 0f;

    public override void WndStart()
	{
		base.WndStart();
		RegisterHooks ();
		MyHead.BtnAdd.OnClickEventHandler += BtnAdd_OnClickHander;
		MyHead.BtnAdd.OnPressDownEventHandler += BtnAdd_OnPressDownHander;
		MyHead.BtnAdd.OnPressUpEventHandler += BtnAdd_OnPressUpHander;
		MyHead.BtnAdd.OnDragEventHandler += BtnAdd_OnDragHander;

		MyHead.BtnMax.OnClickEventHandler += BtnMax_OnClickHander;

		MyHead.BtnMinus.OnClickEventHandler += BtnMinus_OnClickHander;
		MyHead.BtnMinus.OnPressDownEventHandler += BtnMinus_OnPressDownHander;
		MyHead.BtnMinus.OnPressUpEventHandler += BtnMinus_OnPressUpHander;
		MyHead.BtnMinus.OnDragEventHandler += BtnMinus_OnDragHander;

		MyHead.BtnClose.OnClickEventHandler += BtnClose_OnClickHander;
		MyHead.BtnSell.OnClickEventHandler += BtnSell_OnClickHander;
		DoWndEffect ();
	}
	void Update()
	{
		float time = Time.realtimeSinceStartup - m_nPressDownTime;
		if(m_bMouseDown)
		{
			if(time > 0.5f)
			{
				if(m_bAddClick)
				{
					BtnAdd_OnClickHander(null);
				}
				else
				{
					BtnMinus_OnClickHander(null);
				}
			}
		}
	}

	void BtnAdd_OnDragHander(UIButton sender)
	{
		if(UICamera.currentTouch !=null && UICamera.currentTouch.dragStarted)
		{
			bool posChanged = UICamera.currentTouch.delta.sqrMagnitude > 0.001f;
			if(posChanged)
			{
				m_bMouseDown = false;
				m_bMouseUp = false;
			}
		}
	}
	void BtnAdd_OnPressDownHander(UIButton sender)
	{
		m_bAddClick = true;
		m_bMouseDown = true;
		m_nPressDownTime = Time.realtimeSinceStartup;
	}
	void BtnAdd_OnPressUpHander(UIButton sender)
	{
		m_bMouseDown = false;
		m_bMouseUp = true;
	}

	void BtnAdd_OnClickHander(UIButton sender)
	{
		m_iSelectNum += 1;
		if(m_iSelectNum >= m_itemcount)
			m_iSelectNum = m_itemcount;
		
		SetSelectNum (m_iSelectNum);
	}
	void BtnMax_OnClickHander(UIButton sender)
	{
		m_iSelectNum = m_itemcount;
		
		SetSelectNum (m_iSelectNum);
	}
	void BtnMinus_OnClickHander(UIButton sender)
	{
		m_iSelectNum -= 1;
		if(m_iSelectNum <= 0)
			m_iSelectNum = 1;

		SetSelectNum (m_iSelectNum);
	}

	void BtnMinus_OnDragHander(UIButton sender)
	{
		if(UICamera.currentTouch !=null && UICamera.currentTouch.dragStarted)
		{
			bool posChanged = UICamera.currentTouch.delta.sqrMagnitude > 0.001f;
			if(posChanged)
			{
				m_bMouseDown = false;
				m_bMouseUp = false;
			}
		}
	}
	void BtnMinus_OnPressDownHander(UIButton sender)
	{
		m_bAddClick = false;
		m_bMouseDown = true;
		m_nPressDownTime = Time.realtimeSinceStartup;
	}
	void BtnMinus_OnPressUpHander(UIButton sender)
	{
		m_bMouseDown = false;
		m_bMouseUp = true;
	}

	void BtnSell_OnClickHander(UIButton sender)
	{
		List<KeyValue> l = new List<KeyValue>();
		KeyValue v=  new KeyValue();
		v.key = m_ItemInfo.ID;
		v.value = m_iSelectNum ;
		l.Add(v);

		ItemDC.Send_ItemSellRequest (l);
	}
	public void SetData(ItemTypeInfo info,BeiBaoWnd wnd)
	{
		m_parent = wnd;
		m_ItemInfo = info;
        m_itemcount = ItemDC.GetItemCount(m_ItemInfo.itemType);
        SetUI();
	}

	private void SetUI()
	{
		if(MyHead.Spr2dIcon != null )
			NGUIUtil.Set2DSprite(MyHead.Spr2dIcon, "Textures/item/",m_ItemInfo.m_Icon.ToString());
		//设置物品品阶
		if(MyHead.SprQuality != null)
			NGUIUtil.SetSprite(MyHead.SprQuality, m_ItemInfo.m_Quality.ToString());

		MyHead.LblEqName.text = m_ItemInfo.m_Name;
		MyHead.LblEqNum.text = string.Format("[FF7E00]"+NGUIUtil.GetStringByKey("88800040")+"[-][9C4314]{0}[-][FF7E00]"+NGUIUtil.GetStringByKey("88800037")+"[-]",m_itemcount);

		MyHead.LblSellCoin.text = m_ItemInfo.m_sellemoney.ToString ();
		m_iSelectNum = 1;
		SetSelectNum (m_iSelectNum);

	}
	private void SetSelectNum(int iNum)
	{
		MyHead.LblSelectNum.text = string.Format ("{0}/{1}", iNum, m_itemcount);
		MyHead.LblGetCoin.text = (iNum * m_ItemInfo.m_sellemoney).ToString();
	}
	public void OnDestroy()
	{
		AntiRegisterHooks();
	}
	
	/// <summary>
	/// 注册事件
	/// </summary>
	public   void RegisterHooks()
	{
		DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_806, Recv_ItemSellRespone);
	}
	
	/// <summary>
	/// 反注册事件
	/// </summary>
	public   void AntiRegisterHooks()
	{
		DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_806, Recv_ItemSellRespone);
	}

	private void Recv_ItemSellRespone(int nErrorCode)
	{
		if(nErrorCode == 0)
		{
			BtnClose_OnClickHander(null);
			m_parent.ItemUsed();

		}
		else
		{
			NGUIUtil.DebugLog("道具出售错误码：" + nErrorCode.ToString());
		}
	}

	void BtnClose_OnClickHander(UIButton sender)
	{
		GameObject WndBack = WndEffects.FineChildGameObject(gameObject,"WndBackground");
		WndEffects.PlayWndAnimation (WndBack,"wndBackOver");
		
		GameObject Control =  WndEffects.FineChildGameObject(gameObject,"Control");
		if(Control == null) DestoryDialogCallBack(null);
		GameObjectActionWait gadBack = new  GameObjectActionWait(ConstantData.fPopDownAniTime);;
		gadBack.m_complete = DestoryDialogCallBack;
		WndEffects.PlayWndAnimation (Control,gadBack,"popupEnd");
	}

	////窗口动画处理
	void DestoryDialogCallBack(object o)
	{
		WndManager.DestoryDialog<EquipSellWnd>();		
	}
	
	void DoWndEffect()
	{
		
		GameObject Control = WndEffects.FineChildGameObject(gameObject,"Control");
		
		GameObject WndBack = WndEffects.FineChildGameObject(gameObject,"WndBackground");
		
		WndEffects.PlayWndAnimation (Control,"popupStart");
		WndEffects.PlayWndAnimation (WndBack,"wndBackStart");
		
	}
}
