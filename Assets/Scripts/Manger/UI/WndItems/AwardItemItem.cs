using UnityEngine;
using System.Collections;

public class AwardItemItem : MonoBehaviour {

	public UI2DSprite Spr2DItemIco;
	public UISprite SprQuality;
	public UILabel LblItemNum;

	public void SetData(string iconPathName,int quality,int num)
    {
        NGUIUtil.Set2DSprite(Spr2DItemIco, iconPathName);
        //NGUIUtil.SetSprite(SprQuality, quality);
        string text = "x" + num;
        NGUIUtil.SetLableText(LblItemNum, text);
    }
}
