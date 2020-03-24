using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AddExpItem_h : MonoBehaviour 
{
	public UIButton BtnSelect;
	//角色框UI
	public UI2DSprite SprRolePhoto;
	public UISprite SprQuality;
	public UISprite SprQualityBg;
	
	//角色名称UI
	public UISprite SprRoleType;//力 bb_001ic  敏 bb_002ic  智bb_003ic
	public UILabel LblNameLevel;//角色名称 + 彩色品质等级
	public GameObject StarListParent;
	public UISprite[] StarSprites;
	
	//灵魂石

	public UILabel LblNumPercentage;//灵魂石数量

	public UISprite SprNumPercentage;//灵魂石进度条
	public UILabel LblLv;//dengji
	public UILabel LblSmallQuality;
	public UILabel LblPower;

	public UILabel LblNumUsed;
	public GameObject Target;

	public UISprite SprRolePhotoBg;


}