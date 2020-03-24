using UnityEngine;
using System.Collections;

public class DoubleLabelItem : MonoBehaviour 
{

	public DoubleLabelItem_h MyHead
	{
		get
		{
			return GetComponent<DoubleLabelItem_h>();
		}
	}
	
	
	public void SetData(string text1,string text1Name,string text2,string text2Name,int iFontSize = 0)
	{

		if(iFontSize > 0 && iFontSize < MyHead.lbl01.fontSize)
		{
			MyHead.lbl01.fontSize = iFontSize;
		}
		if(iFontSize > 0 && iFontSize < MyHead.lbl02.fontSize)
		{
			MyHead.lbl02.fontSize = iFontSize;
		}
		NGUIUtil.SetLableText<string> (MyHead.lbl01,text1);
		NGUIUtil.SetLableText<string> (MyHead.lbl02,text2);

		NGUIUtil.SetLableText<string> (MyHead.lbl01Name,text1Name);
		NGUIUtil.SetLableText<string> (MyHead.lbl02Name,text2Name);

//		MyHead.lbl01Name.overflowMethod = UILabel.Overflow.ResizeFreely;
//		MyHead.lbl02Name.overflowMethod = UILabel.Overflow.ResizeFreely;
	}
}
