using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrophiesActionWnd_h : WndBase_h {

	public Transform Drop ;
    public Transform DropHero;

	public UILabel m_lblTroptype;
	public UILabel m_TropName;
	public GameObject Content;

    public UIButton BtnOK;

	public UILabel LblDes;
    /// <summary>
    /// 炮弹兵技能
    /// </summary>
    public GameObject SkillInfo;
    public BigSkillItem bigSkillItem;
    public Transform SkillTable;

    public Transform MoveParent;
    public Transform MoveLeft;
    public GameObject HeroInfo;
    public UISprite SprRoleType;
    public UISprite[] StarSprites;

    public GameObject BlackScience;
    public UILabel LblBlackScienceDesc;
    public UI2DSprite Spr2dBSIcon;

    public GameObject Building;
    public UILabel LblBuildDesc;
    public UILabel LblBuildEffect;

    public GameObject WndBg;

    public GameObject DesignPic;
    public UISprite SprDesignQuality;
    public UISprite SprDesignQuality1;
    public UILabel LblDesignCellCount;
    public ShipDesignUnitItem mShipDesignUnitItem;


}
