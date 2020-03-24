using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquipInfoInnerHeroItem : MonoBehaviour 
{
	public EquipInfoInnerHeroItem_h MyHead ;

	private SoldierInfo m_soldierInfo;
	void Awake()
	{
		MyHead = GetComponent<EquipInfoInnerHeroItem_h>();
	}
	public void SetData(SoldierInfo info)
	{
		m_soldierInfo = info;
		SetUI ();
	}

	private void SetUI()
	{
		MyHead.LblItemName.text = m_soldierInfo.m_name;

		int bigLevel = ConfigM.GetBigQuality(m_soldierInfo.Quality);
	
		MyHead.LblQualityPlus.text = NGUIUtil.GetSmallQualityStr(m_soldierInfo.Quality);

		NGUIUtil.Set2DSprite(MyHead.SprItemIco, "Textures/role/", m_soldierInfo.SoldierTypeID.ToString());

		NGUIUtil.SetSprite(MyHead.SprItemQuality, bigLevel.ToString());
		NGUIUtil.SetSprite(MyHead.SprQualityBg, bigLevel.ToString());
	}

}