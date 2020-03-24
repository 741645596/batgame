using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 
/// <From> </From>
/// <Author>QFord</Author>
/// </summary>
public class ECEquipItem : MonoBehaviour {

    public UI2DSprite Spr2dEquipIcon;
    public UISprite SprEquipQuality;
    public GameObject SprEquipSelect;
    public UIButton BtnClick;

    private int m_itemTypeID;



    public void SetData(int ItemTypeID)
    {
        m_itemTypeID = ItemTypeID;
    }

	void Start () 
	{
        BtnClick.OnClickEventHandler += BtnClick_OnClickEventHandler;
        SelectThisItem(true);
	}

    void BtnClick_OnClickEventHandler(UIButton sender)
    {
        
    }

    public void SelectThisItem(bool isSelect)
    {
        SprEquipSelect.SetActive(isSelect);
    }
	
	
}
