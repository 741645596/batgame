using UnityEngine;
using System.Collections;

public class SystemWnd : WndTopBase {
	
	public SystemWnd_h MyHead
	{
		get 
		{
			return (base.BaseHead () as SystemWnd_h);
		}
	}
	public override void WndStart ()
	{
		base.WndStart();
	}

}
