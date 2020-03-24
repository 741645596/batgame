
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CombatRoleItem_h : MonoBehaviour
{
    public Transform head3D = null;
    public UISprite hpSprite;
    public UISprite angerSprite;
    public GameObject SelectEffect;
    public GameObject LongUnFireEffect;
    public GameObject StatusEffectParent;
    public UISprite hpBG;
    public UISprite AngerBG;

	public GameObject soldierItem;
	public UISprite sprDestructivePower;
	public UILabel lblDestructivePower;

    public Transform PivotHitNum;
    public Transform PivotFired;

    public GameObject HeroHitPower;
}
