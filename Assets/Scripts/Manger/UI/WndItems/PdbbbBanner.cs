using UnityEngine;
using System.Collections;

public class PdbbbBanner : MonoBehaviour {

	public UILabel LblTitile;

	public void SetLabelText(string text)
	{
		if(LblTitile != null)
			LblTitile.text = text;
	}
}
