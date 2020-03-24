using UnityEngine;
using System.Collections;

public class ScaleWndBase : WndBase
{
	private GameObject WndBackground;
	// Use this for initialization
	public override void WndStart()
	{
		base.WndStart();

	}

	// Update is called once per frame
	void Update ()
	{
		WndBackground.transform.localScale = new Vector3 (1.0f,1.0f,1.0f);
	}
	public override void CloseDialog()
	{
		base.CloseDialog ();
	}
	public override void DestroyDialog()
	{
		Destroy (gameObject);
	}
}

