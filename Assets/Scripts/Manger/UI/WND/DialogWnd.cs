using UnityEngine;
using System.Collections;
/// <summary>
/// 双按钮对话框
/// </summary>
/// <author>QFord</author>
public class DialogWnd : WndTopBase
{
	public DialogWnd_h MyHead
	{
		get 
		{
			return (base.BaseHead () as DialogWnd_h);
		}
	}


    public NGUIText.Alignment Align;
    public delegate void ButtonOnClickHandle(UIButton sender);
    public ButtonOnClickHandle YESButtonOnClick;
    public ButtonOnClickHandle NOButtonOnClick;

    public override void WndStart()
    {
        base.WndStart();
        if (MyHead.BtnRight)
        {
            MyHead.BtnRight.OnClickEventHandler += BtnYES_OnClickEventHandler;
        }
        if (MyHead.BtnLeft)
        {
            MyHead.BtnLeft.OnClickEventHandler += BtnNO_OnClickEventHandler;
        }
        MyHead.LblTitle.alignment = Align;
        transform.localPosition = U3DUtil.SetZ(transform.localPosition, ConstantData.iDepBefore3DModel);

		WndEffects.DoWndEffect(gameObject);
	}
	void Close(object o)
	{
		WndManager.DestoryDialog<DialogWnd>();
	}
    void BtnNO_OnClickEventHandler(UIButton sender)
    {
        if (NOButtonOnClick != null)
        {
            NOButtonOnClick(sender);
        }
        
		WndEffects.DoCloseWndEffect(gameObject,Close);
    }

    void BtnYES_OnClickEventHandler(UIButton sender)
    {
        if (YESButtonOnClick != null)
        {
            YESButtonOnClick(sender);
        }
		WndEffects.DoCloseWndEffect(gameObject,Close);
    }

    /// <summary>
    /// 设置窗体    提示/右侧按钮/左侧按钮   的文本
    /// </summary>
    public void SetDialogLable(string titleStr, string yesStr, string noStr)
    {
        if (MyHead.LblTitle)
        {
            MyHead.LblTitle.text = titleStr;
        }
        if (MyHead.LblYES)
        {
            MyHead.LblYES.text = yesStr;
        }
        if (MyHead.LblNO)
        {
            MyHead.LblNO.text = noStr;
        }
    }
	
	
}
