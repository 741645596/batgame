using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipDesignWnd_h : WndBase_h 
{
	public UITable Table;

	public UIButton BtnEdit;
	public UIButton BtnClose;

	public UILabel LblDescribeLine1;
	public UILabel LblDescribeLine2;
	public UILabel LblDescribeLine3;

	public UIToggle BtnModelAll;
	public UIToggle Btn8Model;
	public UILabel Lbl8Model;

	public UIToggle Btn12Model;
	public UILabel Lbl12Model;

	public UIToggle Btn16Model;
	public UILabel Lbl16Model;

	public UIToggle Btn24Model;
	public UILabel Lbl24Model;

	public UIToggle Btn32Model;
	public UILabel Lbl32Model;
	
	public UIPanel PanelMask;
	public Transform ToggleParent;
	public Transform[] ToggleList;
	
	public GameObject ScrollLeft;
	public GameObject ScrollRight;
	public GameObject goDesGroup;

}