using UnityEngine;
using System.Collections;

public class SaveFangAnWnd : WndBase {

	public delegate void ButtonOnClickHandle(UIButton sender);
	public ButtonOnClickHandle OKButtonOnClick;
	public ButtonOnClickHandle CancelButtonOnClick;

	public SaveFangAnWnd_h MyHead
	{
		get 
		{
			return (base.BaseHead () as SaveFangAnWnd_h);
		}
	}

    //确  定
    public override void BindEvents()
    {
		if (MyHead.BtnOK)
        {
			MyHead.BtnOK.OnClickEventHandler += BtnOK_OnClickEventHandler;
        }
		if (MyHead.BtnCancel)
        {
			MyHead.BtnCancel.OnClickEventHandler += BtnCancel_OnClickEventHandler;
        }
    }

    public void SetLblTip(string str)
    {
		NGUIUtil.SetLableText<string>(MyHead.LblTip, str);
    }

    void BtnCancel_OnClickEventHandler(UIButton sender)
    {
		if (CancelButtonOnClick!=null)
        {
            WndManager.DestoryDialog<SaveFangAnWnd>();
			CancelButtonOnClick(sender);
        }
    }

    void BtnOK_OnClickEventHandler(UIButton sender)
    {
		if (OKButtonOnClick!=null)
        {
            WndManager.DestoryDialog<SaveFangAnWnd>();
			OKButtonOnClick(sender);
        }
    }

	public override void WndStart()
	{
		base.WndStart();
	}
	
	
}
