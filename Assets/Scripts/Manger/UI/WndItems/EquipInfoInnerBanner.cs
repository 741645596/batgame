using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using sdata;

public class EquipInfoInnerBanner : MonoBehaviour {


	public UILabel LblTitle;

	public void SetLabelName(string text)
	{
		if(LblTitle != null)
			LblTitle.text = text;
	}



}
