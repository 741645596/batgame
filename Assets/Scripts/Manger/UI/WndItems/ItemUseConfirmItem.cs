using UnityEngine;
using System.Collections;

public class ItemUseConfirmItem : MonoBehaviour {

	public UISprite SprItemQuality;
	public UI2DSprite Spr2DItem;
	public UILabel LblItemName;



	public void SetItem(ItemTypeInfo item)
	{
		SprItemQuality.spriteName = ConfigM.GetBigQuality(item.m_Quality).ToString();
		NGUIUtil.Set2DSprite(Spr2DItem, "Textures/item/", item.m_Icon.ToString());
		if(item.m_func == "Item_AddWood" )
		{
			LblItemName.text = string.Format(NGUIUtil.GetStringByKey(10000178)+ NGUIUtil.GetStringByKey(70000182), item.m_args, item.Num);
		}
		else if(item.m_func == "Item_AddCrystal" )
		{
			LblItemName.text = string.Format(NGUIUtil.GetStringByKey(10000177)  + NGUIUtil.GetStringByKey(70000182), item.m_args, item.Num);
		}
	}
}
