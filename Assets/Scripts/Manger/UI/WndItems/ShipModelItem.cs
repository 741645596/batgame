using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipModelItem : MonoBehaviour 
{
	public UISprite sprModel;
	public void SetSize(int width,int height,bool isDeck = false)
	{
		if(sprModel != null)
		{
			sprModel.width = width;

			sprModel.height = height;
			sprModel.spriteName = isDeck?"cm_bg014":"cm_bg011";
		}
	}
}