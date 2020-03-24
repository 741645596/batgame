using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoleViewItem_h : MonoBehaviour {
	
	public UI2DSprite head ;
	public UILabel level ;
	public List<GameObject> m_star = new List<GameObject>();

    public UIButton BtnShowTip;
    public Transform T_Center;
	
	public UILabel LblQualityPlus;
	public UISprite SprQuality;
}