using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 
/// <From> </From>
/// <Author>QFord</Author>
/// </summary>
public class HeroIntroduceItem02 : MonoBehaviour {

    private HeroIntroduceItem02_h MyHead;

    public void SetUI(string text)
    {
        MyHead = GetComponent<HeroIntroduceItem02_h>();
        NGUIUtil.SetLableText<string>(MyHead.LblDesc, text);//设置英雄描述
    }
	
}
