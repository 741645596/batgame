using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 已召唤英雄项
/// </summary>
public class ExistHeroItem_h : MonoBehaviour
{
    public GameObject BG;
    public UIButton BtnSelect;

    //角色名称
    public UISprite SprRoleType;//力 bb_001ic  敏 bb_002ic  智bb_003ic
    public UILabel LblNameLevel;//角色名称 + 彩色品质等级
    /// <summary>
    /// 小红点(表示是否有可以穿上的装备)
    /// </summary>
    public UISprite SprLittleRed;
    //角色头像
    public UI2DSprite SprRolePhoto;
    public UISprite SprQuality;
	/// <summary>
	/// 名字下横幅背景.
	/// </summary>
	public UISprite SprHeadBigQualityBg;

    public UISprite SprQualityBg;
    public GameObject StarListParent;
	public UISprite[] StarSprites;
    //角色装备
    public UISprite[] SprsZhuangBei;
    //角色等级 战斗力
    public UILabel LblLevel;
    public UILabel LblZhanDouLi;
	public UILabel LblBigQuality;
	public UI2DSprite[] SprZhuangBeiList;

}
