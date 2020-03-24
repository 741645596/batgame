using UnityEngine;
using System.Collections;

public class CombatInfoWnd_h : WndBase_h {

	//HourGlass
	public UILabel HourGlassLeftTime;
    public GameObject HourGlass;
    public GameObject BG;

    public Transform OurMoneyPos;
	public Transform OurWoodPos;
	public Transform OurBoxPos;


	public Transform EmemyMoneyPos;
	public Transform EmemyWoodPos;
	public Transform EmemyBoxPos;


    public GameObject EnemyMoneyEffect;

	//OurResource
	public UILabel OurResourceGold ;
	public UILabel OurResourceEnergy ;
	public UILabel OurResourceItem;
    public UILabel LblUserName;

	//EnemyResource
	public UILabel EnemyResourceGold = null;
	public UILabel EnemyResourceEnergy = null;
	public UILabel EnemyResourceTip = null;
    public UILabel LblEnemyName;


    public UIButton BtnPause;

	public GameObject itemBG;
}
