using UnityEngine;
using System.Collections;

public class ShengJieConsume : WndBase {

	public ShengJieConsume_h MyHead
	{
		get 
		{
			return (base.BaseHead () as ShengJieConsume_h);
		}
	}
	// Use this for initialization
	public override void WndStart()
	{
		base.WndStart();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
