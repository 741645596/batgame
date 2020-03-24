using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 查看战役攻船UI
/// </summary>
public class ViewStageWnd_h :  WndBase_h {

	public UILabel ManualLabel;
	public UILabel RewardLabel;
	public UILabel DescLabel ;
	public UILabel TimesLabel;
	public UILabel stagename ;
	public UIButton btnclose ;
	public UIButton btnCombat;
    public Transform BtnPivot;

	public UITable Rewardtable;
	public UITablePivot RoleTable;

	public GameObject Info;
	public GameObject Role;
	public GameObject Plan;

	public UILabel PlanText;
	public UILabel RoleText;

	public UISprite[] m_lstar;
	public UISprite DefenseBg;

	public UISprite TitleSprite;

	public GameObject SweepGroup;
	public UILabel LblSweepTicket;
	public UIButton BtnSweepMultiple;
	public UIButton BtnSweepOnce;
	public UILabel LblSweepMultiple;
	public UILabel LblSweepOnce;
}
