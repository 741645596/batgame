using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrapViewListWnd_h : WndBase_h 
{
	public UIButton BtnReturn;
	public GameObject Parent;
	
	public UILabel LblCount;
	public UIToggle BtnTrapsAll;
	public UIToggle BtnTrapsFire;
	public UIToggle BtnTrapsWater;
	public UIToggle BtnTrapsPosion;
	public UIToggle BtnTrapsThur;
	public UIToggle BtnTrapsGas;

	public UIPanel PanelMask;
	public Transform ToggleParent;
	public Transform[] ToggleList;
	
	public GameObject ScrollLeft;
	public GameObject ScrollRight;
	public UIAnchor Anchor;
}