using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 炮弹兵背包
/// <From> </From>
/// <Author>QFord</Author>
/// </summary>
public class PdbbbWnd_h : WndBase_h {

    public UIToggle TogAll;
    public UIToggle TogTank;
    public UIToggle TogDPS;
    public UIToggle TogAssisst;

    public UIButton BtnReturn;
    public UITable Parent;

    public UILabel LblCount;

	public UIPanel PanelMask;
	public UIPanel PanelPdbbbWnd;
	public Transform ToggleParent;
	public Transform[] ToggleList;
	
	public UISprite ScrollLeft;
	public UISprite ScrollRight;
}
