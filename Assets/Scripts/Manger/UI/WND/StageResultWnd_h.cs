using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 战役结算wnd
/// </summary>
public class StageResultWnd_h : WndBase_h {

	public Transform succ ;
	public Transform fail ;

	public UITablePivot Datatable;
	public UITable Rewardtable;

	public UISprite[] m_lstar;

	public UILabel lv;
	public UILabel exp;
	public UILabel gold;
	public UILabel Cup;
	public UILabel Wood;

	public UILabel Faillv;
	public UILabel Failexp;
	public UILabel Failgold;
	
	public UIButton btnclose ;
	public UIButton btnrecombat;

	public UIButton BtnUpSoldier;
	public UIButton BtnUpSkill;

	public UIButton RewardOK ;

    public GameObject m_ResultWnd ;
	public GameObject m_pve;
	public GameObject m_pvp;
	public GameObject PvpPveFail;

	public GameObject Failpve;
	public GameObject Failpvp;

	public GameObject Trophies ;

    public GameObject Data;
	public GameObject FailData;

	public UIPanel PanelMask;
	
	public GameObject ScrollLeft;
	public GameObject ScrollRight;

	public GameObject LblVictory1;
	public GameObject LblVictory2;
    //战斗失败跳转UI组
    public GameObject Help2;
    public GameObject Help3;
    public GameObject Help4;

    //战斗失败跳转按钮
    public UIButton BtnGotoPdbbb21;

    public UIButton BtnGotoPdbbb31;
    public UIButton BtnGotoTrapList31;
    public UIButton BtnGotoShipEdit31;

    public UIButton BtnGotoPdbbb41;
    public UIButton BtnGotoPdbbb42; 
}
