using UnityEngine;
using System.Collections;

public class NeedItemNumItem : MonoBehaviour {

	/// <summary>
	/// 1默认横着，2竖着
	/// </summary>
	public int m_wndType = 1;
	public NeedItemNumItem_h MyHead;
	private int m_itemType = 0;

	void Awake()
	{
		MyHead = GetComponent<NeedItemNumItem_h>();
		MyHead.btnClick.OnClickEventHandler += BtnClick_OnClickHander;
	}
	// Use this for initialization
	void Start () 
	{

	}
	public void SetHorItem()
	{
		MyHead.LblNeedNum.gameObject.transform.localPosition = new Vector3(0f,-27f,0);
		MyHead.LblNeedNum.fontSize = 20;
		MyHead.LblNeedNum.width = 80;
		MyHead.LblNeedNum.alignment = NGUIText.Alignment.Center;
		MyHead.LblNeedNum.effectStyle = UILabel.Effect.Outline;
		m_wndType = 2;
	}
	void BtnClick_OnClickHander(UIButton sender)
	{
//		ItemComeFromWnd wnd = WndManager.GetDialog<ItemComeFromWnd>();
		sdata.s_itemtypeInfo itemInfo = ItemM.GetItemInfo(m_itemType);//当前灵魂石
		ItemComeFromWnd wnd = WndManager.GetDialog<ItemComeFromWnd>();
		wnd.SetData(itemInfo,null,null,1);
	}
	public void SetData(int iHave ,int iNeed,int itemType)
	{
		string color;
		if(iNeed > iHave)
		{
			color = "[FF0000]";
		}
		else
		{
			color = "[FFFFFF]";
		}
		if(m_wndType == 1)
		{
			MyHead.LblNeedNum.text = string.Format(color + "{0}[-]",iNeed);
		}
		else if(m_wndType == 2)
		{
			MyHead.LblNeedNum.text = string.Format(color + "{0}/{1}[-]",iHave,iNeed);
		}
		m_itemType = itemType;
		string iCon = ItemM.GetItemIcon (itemType);
		NGUIUtil.Set2DSprite(MyHead.sprIcon, "Textures/item/",iCon);
	}

}
