using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GodSkillWnd_h : WndBase_h
{
    public UILabel CurMana;
    public UILabel RequireMana;
    public GameObject Fire;
    //public GameObject Shadow;
    public GameObject m_fulleffect;
    public GameObject m_notfulleffect;
    public UIButton BtnRelease;
    public UI2DSpriteAnimation Vaule;
    public UI2DSprite m_BiaoQingParent;
    public Animator m_BiaoQing;
	public Transform PathParent;
	public GameObject SelectEffect;
	public UILabel lblLevel;
	public GameObject StarListParent;

	public UILabel LblDestroyPts;

    public GameObject GuidePivot;
}
