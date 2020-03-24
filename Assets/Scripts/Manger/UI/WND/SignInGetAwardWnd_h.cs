using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SignInGetAwardWnd_h : WndBase_h 
{
	//普通奖励

	public GameObject NormalAwardGroup;
	public UISprite SprQualityBg;
	public UI2DSprite Spr2DItem;
	public UILabel LblItem;

	//VIP奖励
	public GameObject VipAwardGroup;
	public UISprite SprVipQualityBg;
	public UI2DSprite Spr2DVipItem;
	public UILabel LblVipItem;

	public UIButton BtnConfirm;
}