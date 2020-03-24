using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 温馨提示
/// </summary>
public class ReminderWnd : WndBase
{
    public ReminderWnd_h MyHead
    {
        get
        {
            return (base.BaseHead() as ReminderWnd_h);
        }
    }

	public override void WndStart()
	{
		base.WndStart();

        MyHead.BtnConfirm.OnClickEventHandler += BtnConfirm_OnClickEventHandler;
    }

    void BtnConfirm_OnClickEventHandler(UIButton sender)
    {
        WndManager.DestoryDialog<ReminderWnd>();
    }

    public void SetData(string tipText)
    {
        NGUIUtil.SetLableText<string>(MyHead.LblTips, tipText);
    }
}
