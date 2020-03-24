using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrapViewItem_h : MonoBehaviour 
{
	public GameObject BG;
	public UIButton BtnSelect;
	
	//角色名称
	public UISprite SprRoleType;//力 bb_001ic  敏 bb_002ic  智bb_003ic
	public UILabel LblNameLevel;//角色名称 + 彩色品质等级
	public UILabel LblLevel;
	public UILabel LblDefanAt;//防御力
	/// <summary>
	/// 小红点(表示是否有可以穿上的装备)
	/// </summary>
	public UI2DSprite Shape;
	//角色头像
	public UI2DSprite SprTrapPhoto;
	public UISprite SprQuality;
	public UISprite SprQualityBg;
	public GameObject StarListParent;
	public UISprite[] StarSprites;
	public GameObject SprRedSpot;

	public UILabel LblSamllQuality;
	public UISprite SprBigQuality;

	public UISprite SprTrapPhotoBg;
	public GameObject GoUnDeder;

	public GameObject GoNotExit;
	public UISprite SprNumPercentage;
	public UILabel LblNumPercentage;
//	

}