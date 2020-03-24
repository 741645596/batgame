using UnityEngine;
using System.Collections;

public class TreasureBgWnd : WndBase {
	public TreasureBgWnd_h MyHead
	{
		get 
		{
			return (base.BaseHead () as TreasureBgWnd_h);
		}
	}
	// Use this for initialization
	public override void WndStart () {
		base.WndStart();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void SetMode(int mode)
	{
		if (mode == 1)
		{
			MyHead.sprJyd.gameObject.SetActive(true);
			MyHead.sprFsjl.gameObject.SetActive(false);
		}
		else if (mode == 2)
		{
			MyHead.sprJyd.gameObject.SetActive(false);
			MyHead.sprFsjl.gameObject.SetActive(true);
		}
	}
}
