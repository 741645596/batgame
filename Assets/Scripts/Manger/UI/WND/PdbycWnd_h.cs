using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 炮弹兵养成
/// <From> </From>
/// <Author>QFord</Author>
/// </summary>
public class PdbycWnd_h : WndBase_h {

    public UIButton BtnSoldierPrev;
    public UIButton BtnSoldierNext;
    
    public UIButton BtnClose;

    public Transform SoldierPos;
    public UIButton BtnClickSoldier;
    public HUDText HudText;

    
    // SoldierName
    public UISprite SprType;
    public UISprite[] SprStars;
    public UILabel LblSoldierName;
    //SoldierEquip
    public GameObject[] SoldierEquipList;
    //SoldierLevel
    public UILabel LblLevel;
    public UILabel LblCombatPower;
    public UILabel LblExp;
    public UISprite SprSoulPercentage;

    public UILabel LblSoulNum;
    public UIButton BtnGetSoulStone;

    public UIButton BtnJinSheng;
    public UIButton BtnShengXing;
    //tooggles
    public UIToggle Toggle1;
    public UIToggle Toggle2;
    public UIToggle Toggle3;

	public UILabel LblLocation;
	public UILabel LblSmallQuality;

	public UISprite SprHeadQuality;

	public GameObject GoParent;

    public Transform EquipFlyPoint;

	public UIButton BtnTips;
	public UISprite SprTipsInfo;
}
