using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreasureRuleWnd : WndBase 
{
	public TreasureRuleWnd_h MyHead 
	{
		get
		{ 
			return (base.BaseHead () as TreasureRuleWnd_h);	
		}	
	}
	public override void WndStart() 
	{
		base.WndStart(); 
		MyHead.BtnClose.OnClickEventHandler += Close;
		WndEffects.DoWndEffect(gameObject);

		if(MyHead.LblRuleContent != null)
			MyHead.LblRuleContent.text = NGUIUtil.GetStringByKey(70000205);
	}
	void Close(UIButton sender)
	{
		WndEffects.DoCloseWndEffect(gameObject,CloseCallBack);
	}
	void CloseCallBack(object obj)
	{
		WndManager.DestoryDialog<TreasureRuleWnd>();
	}
}