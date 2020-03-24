using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 
/// <From> </From>
/// <Author>QFord</Author>
/// </summary>
public class HeroIntroduceItem04 : MonoBehaviour {

    private HeroIntroduceItem04_h MyHead;

    void Awake()
    {
        MyHead = GetComponent<HeroIntroduceItem04_h>();
    }
	public void SetUI(string str,string int1,string agi)
	{
		SetOutLine (true);
		NGUIUtil.SetLableText(MyHead.LblStrength, str);
        NGUIUtil.SetLableText(MyHead.LblInt, int1);
        NGUIUtil.SetLableText(MyHead.LblAGI, agi);
	}
	
	public void SetLblName(string strStr,string strInt1,string strAgi)
	{
		if(strStr != null)
		{
			MyHead.LblStrengthName.text = strStr;
		}
		if(strInt1 != null)
		{
			MyHead.LblIntName.text = strInt1;
		}
		if(strAgi != null)
		{
			MyHead.LblAGIName.text = strAgi;
		}
	}
	public void SetOutLine(bool isOutLine)
	{
		MyHead.LblAGIName.effectStyle = UILabel.Effect.Outline;
		MyHead.LblIntName.effectStyle = UILabel.Effect.Outline;
		MyHead.LblStrengthName.effectStyle = UILabel.Effect.Outline;

		MyHead.LblStrength.effectStyle = UILabel.Effect.Outline;
		MyHead.LblInt.effectStyle = UILabel.Effect.Outline;
		MyHead.LblAGI.effectStyle = UILabel.Effect.Outline;

	}
	
}
