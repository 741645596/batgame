using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NpcTalksWnd_h : WndBase_h 
{
    public Transform Parent;
    public Transform MainBg;
	public UI2DSprite Spr2dNpcHead;
    //public UILabel LblNpcName;
    public UILabel LblTalkDesc;

    public UIButton BtnStyle0;
    public UIButton BtnStyle1;
    public GameObject Style2;

    public Transform From;
    public Transform End;

    public TweenPosition TpMainBg;
    public TweenPosition TpMainBg1;
    public TweenPosition TpScrollBar;

    public UIPanel PanelMask;
    public UIWidget Style0;


}
