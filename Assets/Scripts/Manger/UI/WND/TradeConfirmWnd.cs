using UnityEngine;
using System.Collections;

public class TradeConfirmWnd : WndTopBase 
{
	public delegate void CallBack();
	public CallBack CallBackFun;
	public TradeConfirmWnd_h MyHead
	{
		get 
		{
			return (base.BaseHead () as TradeConfirmWnd_h);
		}
	}
	// Use this for initialization
	public override void WndStart ()
	{
		base.WndStart ();
		MyHead.BtnConfirm.OnClickEventHandler += BtnConfirm_OnClickEventHandler;
		MyHead.BtnCancel.OnClickEventHandler += BtnCancel_OnClickEventHandler;
		MyHead.BtnBG.OnClickEventHandler += BtnCancel_OnClickEventHandler;
	}

	void BtnConfirm_OnClickEventHandler(UIButton sender)
	{
		if (CallBackFun != null)
			CallBackFun();
		WndManager.DestoryDialog<TradeConfirmWnd>();
	}
	
	void BtnCancel_OnClickEventHandler(UIButton sender)
	{
		WndManager.DestoryDialog<TradeConfirmWnd>();
		//UserDC.Send_RoleInitRequest(TextRoleName.text);
	}
}
