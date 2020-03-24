using UnityEngine;
using System.Collections;

public class TreasureCanNotRobWnd : WndBase {
	
	public TreasureCanNotRobWnd_h MyHead
	{
		get 
		{
			return (base.BaseHead () as TreasureCanNotRobWnd_h);
		}
	}
	// Use this for initialization
	public override void WndStart () {
		base.WndStart();
		MyHead.btnConfirm.OnClickEventHandler += Confirm;
	}
	
	// Update is called once per frame
	void Confirm (UIButton sender) {
		WndManager.DestoryDialog<TreasureCanNotRobWnd>();
	}
}
