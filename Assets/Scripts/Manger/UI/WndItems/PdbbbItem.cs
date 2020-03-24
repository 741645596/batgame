using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PdbbbItem : MonoBehaviour 
{
	public PdbbbItem_h MyHead ;
	void Awake()
	{
		MyHead = GetComponent<PdbbbItem_h>();
	}
}