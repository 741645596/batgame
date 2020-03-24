using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PublicSmallTipsWnd_h : WndBase_h 
{
	//	public UILabel LblTip;
	//	public UILabel LblRefreshNum;
	public UIButton BtnCancel;
	public UIButton BtnConfirm;
	//	public UILabel LblTimes;.
    /// <summary>
    ///  1 商店购买（次数提示）
    /// </summary>
	public GameObject ShopBuyGroup;
    public UILabel LblDiamondNum;
    public UILabel LblRefreshTime;
    /// <summary>
    /// 2 与服务器断线
    /// </summary>
	public GameObject LoginErrorGroup;
    /// <summary>
    /// 3 购买体力
    /// </summary>
	public GameObject BuySPGroup;
    public UILabel LblCostDiamondSP;
    /// <summary>
    /// 4 重置竞技场CD
    /// </summary>
	public GameObject ArenaCDGroup;
    public UILabel LblCostDiamondCD;
    /// <summary>
    ///  5 重置竞技场刷新时间
    /// </summary>
	public GameObject ArenaTimeGroup;
    public UILabel LblBuyTime;
    public UILabel LblCostDiamondTime;
    public UILabel LblPlusTime;

    /// <summary>
    /// 6 改名界面
    /// </summary>
	public GameObject ChangeNameGroup;
	public UILabel LblNameImput;
    public UIButton BtnRandom;




}
