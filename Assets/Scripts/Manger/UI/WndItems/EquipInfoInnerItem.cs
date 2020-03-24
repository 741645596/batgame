using UnityEngine;
using System.Collections;
using sdata;


public class EquipInfoInnerItem : MonoBehaviour {

	public EquipInfoInnerItem_h MyHead;
	private int m_iQuality;
	private int m_iCon;
	private string m_Name;
	private string m_strIconPath;

	void Awake ()
	{
		MyHead = GetComponent<EquipInfoInnerItem_h>();
	}
	// Use this for initialization
	void Start () 
	{


	}

	public void SetData(int icon,int quality,string name,string iconPath)
	{
		m_iCon = icon;
		m_iQuality = quality;
		m_Name = name;
		m_strIconPath = iconPath;
		SetUI ();
	}
	private void SetUI()
	{

		if(MyHead.SprItemIco != null )
			NGUIUtil.Set2DSprite(MyHead.SprItemIco, m_strIconPath,m_iCon.ToString());
		//设置物品品阶
		if(MyHead.SprItemQuality != null)
			NGUIUtil.SetSprite(MyHead.SprItemQuality, m_iQuality.ToString());

		MyHead.LblItemName.text = string.Format(m_Name);
	}
}
