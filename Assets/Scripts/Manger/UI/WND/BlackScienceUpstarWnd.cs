using UnityEngine;
using System.Collections;

public class BlackScienceUpstarWnd : WndBase {
	
	public BlackScienceUpstarWnd_h MyHead
	{
		get 
		{
			return (base.BaseHead () as BlackScienceUpstarWnd_h);
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
		UISprite[] starSprites = U3DUtil.GetComponentsInChildren<UISprite>(MyHead.lblPreStar);
		NGUIUtil.SetStarLevelNum (starSprites,prelevel);
	
		starSprites = U3DUtil.GetComponentsInChildren<UISprite>(MyHead.lblAfterStar);
		NGUIUtil.SetStarLevelNum (starSprites,afterlevel);

		MyHead.lblExplain.text = explain;
	}
	
	void Back(UIButton sender)
	{
		
		WndManager.DestoryDialog<BlackScienceUpstarWnd>();
	}
}
