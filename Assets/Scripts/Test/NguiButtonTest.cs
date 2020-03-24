using UnityEngine;
using System.Collections;

public class NguiButtonTest : MonoBehaviour
{
	//定义单例
	public static NguiButtonTest _wnd;


	public UIButton btn;
	public UIEventTrigger UIEvent;
	
	void Start () {
		if (btn != null) 
		{
			btn.OnClickEventHandler += ButtonClick;
		}
		//
		if (UIEvent != null) 
		{
			UIEvent.onClick += TButtonClick;

		}
		
	}
	
	public void ButtonClick(UIButton sender)
	{
		Debug.Log ("click：ButtonClick " +sender.gameObject.name);
	}
	
	
	public void TButtonClick(UIEventTrigger sender)
	{
		Debug.Log ("click：ButtonClick " +sender.gameObject.name);
	}
	
	
	
}