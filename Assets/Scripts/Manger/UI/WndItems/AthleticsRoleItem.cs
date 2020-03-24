using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AthleticsRoleItem : MonoBehaviour 
{
	public GameObject[] SprStarList;
	public UISprite SprQuality;
	public UI2DSprite Spr2DHeadIco;
	public UILabel LblLevel;

	private SoldierInfo m_Info;

	public void SetData(SoldierInfo info)
	{
		m_Info = info;
	}
	void Start ()
 	{
		for(int i = 0;i <SprStarList.Length;i++)
		{
			if(i < m_Info.StarLevel)
			{
				SprStarList[i].SetActive(true);
			}
			else
			{
				SprStarList[i].SetActive(false);
			}
		}

		NGUIUtil.Set2DSprite (Spr2DHeadIco, "Textures/role/", m_Info.SoldierTypeID.ToString());
		LblLevel.text = m_Info.Level.ToString ();
		NGUIUtil.SetSprite(SprQuality,ConfigM.GetBigQuality(m_Info.Quality).ToString());
	}
}