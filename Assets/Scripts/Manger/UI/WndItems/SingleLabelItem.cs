using UnityEngine;
using System.Collections;

public class SingleLabelItem : MonoBehaviour {

	public SingleLabelItem_h MyHead
	{
		get
		{
			return GetComponent<SingleLabelItem_h>();
		}
	}

	public void SetData(string text,int iFontSize = 0)
	{
		if(text == null) return;
		if(iFontSize > 0 && iFontSize < MyHead.label01.fontSize)
		{
			MyHead.label01.fontSize = iFontSize;
		}
		MyHead.label01.text = text;
	}

}
