using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 
/// <From> </From>
/// <Author>QFord</Author>
/// </summary>
public class HeroIntroduceItem01 : MonoBehaviour {

    private HeroIntroduceItem01_h MyHead;

    public void SetUI(int type)
    {
        MyHead = GetComponent<HeroIntroduceItem01_h>();
        switch (type)//设置 定位
        {
            case 1:
                NGUIUtil.SetLableTextByKey<string>(MyHead.LblDingWei, "30000010");
                break;
            case 2:
                NGUIUtil.SetLableTextByKey<string>(MyHead.LblDingWei, "30000011");
                break;
            case 3:
                NGUIUtil.SetLableTextByKey<string>(MyHead.LblDingWei, "30000012");
                break;
        }
    }
	
}
