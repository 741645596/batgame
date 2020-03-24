using UnityEngine;
using System;
using System.Collections;

/// <summary>
///  装备信息界面
/// <From> </From>
/// <Author>QFord</Author>
/// </summary>
public class EquipmentInfoWnd_h : WndBase_h
{
    public GameObject WndTweenParent;

    public UI2DSprite SprEquip;
    public UILabel LblName;
    public UILabel LblFuMo;
    public GameObject AttributesParent;
    public UIButton BtnSure;
    public UIButton BtnWndBg;
    public UILabel LblDescription;
    public UILabel LblRequireLevel;
    public UIButton BtnClose;
    public UISprite SprWndType;
    public UILabel LblTitle;
	public UILabel LblButtonType;
	public UISprite SprCurrency;

    public UILabel LblBuyCount;
    public UILabel LblTotalPrice;
    public UI2DSprite Spr2dCurrency;

	public GameObject GoShop;

}
