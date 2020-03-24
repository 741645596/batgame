using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 炮弹兵信息界面
/// <From> 炮弹兵详细信息界面</From>
/// <Author>QFord</Author> 150/150
/// </summary>
public class SoldierInfoWnd_h : WndBase_h
{
    public UISprite SprType;
    public UILabel LblTitle;
    public UISprite[] SprStars;
    public UIButton BtnClose;

    public GameObject EquipmentsParent;

    public UILabel LblLevel;
    public UILabel LblExp;
    public UILabel LblCombatPower;
    /// <summary>
    /// 灵魂石百分比
    /// </summary>
    public UISprite SprSoulPercentage;
    public UILabel LblSoulNum;
    public UIButton BtnUpgradSoulStone;

    public UIButton BtnJinJie;
    public UIButton BtnJinHua;
    public UIButton BtnJiNeng;

    
}
