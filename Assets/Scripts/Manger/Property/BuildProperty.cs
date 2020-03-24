using UnityEngine;
using DG.Tweening;
using System.Collections.Generic ;


public class BuildHelpPointName{
	public const string skinroot = "skinroot";
	public const string boneroot = "boneroot";
	public const string attackpos = "attackpos";
	public const string help01 = "help01";
	public const string help02 = "help02";
	public const string help03 = "help03";
	public const string help04 = "help04";
	public const string help05 = "help05";
	public const string help06 = "help06";
	public const string help07 = "help07";
	public const string body01 = "body01";
	public const string body02 = "body02";
	public const string body03 = "body03";
	public const string body04 = "body04";
	public const string body05 = "body05";
	public const string guidePos = "guidePos";
	public const string help_hp = "help_hp";
}
public class BuildProperty : LifeProperty {

	public List<Animator> m_listanimator = new List<Animator>();
	public List<GameObject> m_goAllBodySkin = new List<GameObject>();
	public int ModleType=0;
	public virtual void Start () {

	}
	
	public virtual void Update(){

	}


	void OnDestroy()
	{
	}
	
	public void TweenPingponeColor(Color color,float duration)
	{
		int nBodyCount = m_goAllBodySkin.Count;
		for (int nBodyCnt=0; nBodyCnt<nBodyCount; nBodyCnt++) 
		{
			GameObject obj = m_goAllBodySkin[nBodyCnt];
			if(obj)
			{
			}

		}
	}
	
	public void SetColor(string strProperty,Color clr)
	{
		int nBodyCount = m_goAllBodySkin.Count;
		for (int nBodyCnt=0; nBodyCnt<nBodyCount; nBodyCnt++) 
		{
			GameObject obj = m_goAllBodySkin[nBodyCnt];
			if(obj)
			{
				obj.GetComponent<Renderer>().material.SetColor(strProperty, clr);
			}
			
		}
	}
	public void SetAllBodyActive(bool bActive)
	{
		int nBodyCount = m_goAllBodySkin.Count;
		for (int nBodyCnt=0; nBodyCnt<nBodyCount; nBodyCnt++) 
		{
			GameObject obj = m_goAllBodySkin[nBodyCnt];
			if(obj)
			{
				obj.SetActive(bActive);
			}
			
		}
	}
	public void EnableAllBodyRender(bool bEnable)
	{
		int nBodyCount = m_goAllBodySkin.Count;
		for (int nBodyCnt=0; nBodyCnt<nBodyCount; nBodyCnt++) 
		{
			GameObject obj = m_goAllBodySkin[nBodyCnt];
			if(obj)
			{

				Renderer meshRender = obj.GetComponent<Renderer>();;
				if(meshRender!=null)
					meshRender.enabled = bEnable;
			}
			
		}
	}

	public int GetModeType()
	{
		return ModleType;
	}
}
