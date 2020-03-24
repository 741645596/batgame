using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SignAwardDescribeWnd : WndBase
{
    public SignAwardDescribeWnd_h MyHead
    {
        get
        {
            return (base.BaseHead() as SignAwardDescribeWnd_h);
        }
    }

    public override void WndStart()
    {
        base.WndStart();
        MyHead.BtnBg.OnClickEventHandler += BtnBg_OnClickEventHandler;
    }

    void BtnBg_OnClickEventHandler(UIButton sender)
    {
        WndManager.DestoryDialog<SignAwardDescribeWnd>();
    }
}