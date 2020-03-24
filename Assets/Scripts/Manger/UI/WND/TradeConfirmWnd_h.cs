using UnityEngine;
using System.Collections;

public class TradeConfirmWnd_h : WndBase_h {
	public UIButton BtnConfirm;
	public UIButton BtnCancel;
	public UILabel Content;
	public UIButton BtnBG;
	public delegate void CallBack();
	public CallBack CallBackFun;
	// Use this for initialization
}
