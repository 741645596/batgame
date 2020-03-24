using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BeiBaoWnd_h : WndBase_h {
	
	public Transform AnchorTarget;
	
	public UIToggle BtnQuanBu;
	public UIToggle BtnZhuangBei;
	public UIToggle BtnJuanZhou;
	public UIToggle BtnLingHunShi;
	public UIToggle BtnXiaoHaoPin;
	public UIToggle BtnLingJian;
	
	public UIButton BtnClose;
	
	public GameObject Parent;
	public GameObject BeiBaoItem;
	
	public GameObject ItemParent;

	public UIPanel PanelMask;
	public Transform ToggleParent;
	public Transform[] ToggleList;
	
	public GameObject ScrollLeft;
	public GameObject ScrollRight;

	public GameObject Target;


}