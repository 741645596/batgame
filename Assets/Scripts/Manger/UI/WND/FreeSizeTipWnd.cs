using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FreeSizeTipWnd : WndTopBase 
{
	public FreeSizeTipWnd_h MyHead 
	{
		get
		{ 
			return (base.BaseHead () as FreeSizeTipWnd_h);	
		}	
	}
	/// <summary>
	/// 标题显示的持续时间
	/// </summary>
	public float ShowDuration = 1.0f;
	/// <summary>
	/// 窗体的深度
	/// </summary>
	public int iDep = 0;

	public int SprType;

    public CallBack FinishCallBack;


    public override void WndStart()
	{
		base.WndStart ();
		if(SprType == 2)
		{
            //临时 这个还是得替换掉，放置到同一图集
			NGUIUtil.SetSprite(MyHead.SprBg,"Textures/UI/Altases","pdbyc","gong_oo7bg");
		}
	}
	// Update is called once per frame
	void Update () 
	{
		if(iDep != 0 && iDep != gameObject.transform.localPosition.z)
		{
			gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x,gameObject.transform.localPosition.y,iDep);
		}
		if (ShowDuration <= 0)
		{
            if (FinishCallBack!=null)
            {
                FinishCallBack();
            }
			WndManager.DestoryDialog<FreeSizeTipWnd>();
		}
		else
		{
			ShowDuration -= Time.deltaTime;
		}
	}
}