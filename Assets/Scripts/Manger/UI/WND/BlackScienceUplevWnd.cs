using UnityEngine;
using System.Collections;

public class BlackScienceUplevWnd : WndBase {
	public BlackScienceUplevWnd_h MyHead
	{
		get 
		{
			return (base.BaseHead () as BlackScienceUplevWnd_h);
		}
	}
	// Use this for initialization
    public override void WndStart()
    {
		base.WndStart();
		MyHead.btnBg.OnClickEventHandler += Back;
	}
	
	public void SetData(int prelevel,int afterlevel,string explain)
	{
		MyHead.lblPreLevel.text = NGUIUtil.GetStringByKey("60000005") + prelevel.ToString();
		MyHead.lblAfterLevle.text = NGUIUtil.GetStringByKey("60000005") +afterlevel.ToString();
		MyHead.lblExplain.text = explain;
	}

	void Back(UIButton sender)
	{

		WndManager.DestoryDialog<BlackScienceUplevWnd>();
	}
}
