using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectSoldierwnd_h : WndBase_h {
	public UIToggle TogAll;
	public UIToggle TogTank;
	public UIToggle TogDPS;
	public UIToggle TogAssisst;
	public UIToggle TogMercenary;
	public UITable SelectSoldierTable;
	public UITable SelectCaptionTable;
	public UITable TogglesTable;
	public UITable SelectPosTable;

	public UILabel LblTipInfo;
	public UIButton BtnStart;
	public UIButton BtnBack;
	public UILabel LblCombatForce;
	public GameObject Captiongo;
	public Transform TSelected;

    public UIButton BtnBlackScience;
    public UI2DSprite SprCaptainHead;


	public List<Transform> SelectedPos = new List<Transform>();

	public UIPanel PanelMask;
	public Transform ToggleParent;
	public Transform[] ToggleList;
	
	public GameObject ScrollLeft;
	public GameObject ScrollRight;

    public Transform Guide;
}
