using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrapShowWnd_h : WndBase_h 
{
	public UILabel LblTrapName;
	public UILabel LblTrapQuality;
	public UILabel LblLv;
	public UILabel LblDefandPower;
	public UILabel LblSoulNum;
	public UILabel LblGoldNum;
	public UILabel LblWoodNum;
	public UISprite[] SprStars;
	public UIButton BtnGetSoulStone;

//	public List<UIButton> BtnList;
	public UIButton BtnShengJi;
	public UIButton BtnShengXing;
	public UIButton BtnChaiJie;
	public UIToggle BtnShuXing;
	public UIToggle BtnShengJie;
	public UIToggle BtnTuJian;
	public UIButton BtnClickTrap;

	public UIButton BtnReturn;
	public UISprite SprSoulPercentage;

	public GameObject SprTrapIntorduct;
	public GameObject EffectParent;

	public HUDText HudText;

	public UIButton BtnPre;
	public UIButton BtnNext;
	public UISprite SprQuality;
	public UISprite SprRoleType;//力 bb_001ic  敏 bb_002ic  智bb_003ic
	public UILabel LblRoleTypeName;

}