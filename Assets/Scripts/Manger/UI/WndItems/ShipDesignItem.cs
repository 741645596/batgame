using UnityEngine;
using System.Collections;
using sdata;

public class ShipDesignItem : MonoBehaviour {

	public UISprite SprQualityBg;
	public UISprite SprQualityWzBg;
	public UISprite SprQualityIco;

	public UILabel LblShipDesignName;
	public UILabel LblRoomCount;

	public GameObject ShipModelParent;
	public UISprite[] Star;
	public UISprite SprShipBg;
	public UISprite SprDesignChosen;

	public UIToggle BtnSelect;

	public delegate void SelectCallBack(ShipDesignItem item);
	public SelectCallBack m_SelectCallBack;

	private bool m_showSelect = false;
	private int m_ShipDesignID;
//	private ShipPlan m_shipInfo;
	StaticShipCanvas m_s_shipInfo;

	// Use this for initialization
	void Start () 
	{
		BtnSelect.bColorChange = false;
		if(m_showSelect)
			EventDelegate.Add(BtnSelect.onChange,BtnSelectChange);
	}
	public void SetData(StaticShipCanvas s_shipInfo,bool showSelect,int ShipDesignID)
	{
		m_s_shipInfo = s_shipInfo;
		m_showSelect = showSelect;
		m_ShipDesignID = ShipDesignID;
		SetUI();
	}
	public StaticShipCanvas GetInfo()
	{
		return m_s_shipInfo;
	}
	public int GetShipDesignID()
	{
		return m_ShipDesignID;
	}
	public void SetUI()
	{
//		s_shipcanvasInfo info = ShipPlanM.GetShipCanvasInfo(m_shipInfo.ShipCanvasTypeId);
		StaticShipCanvas info = m_s_shipInfo;

		if(info != null)
		{
			int bigLevel = ConfigM.GetBigQuality(info.Quality);
			NGUIUtil.SetStarLevelNum(Star,info.StarLevel);
			LblShipDesignName.text = info.Name;//NGUIUtil.GetBigQualityName(m_shipInfo.Name,info.quality);
			LblRoomCount.text = info.Cell.ToString();
			NGUIUtil.SetSprite(SprQualityBg, bigLevel.ToString());
			NGUIUtil.SetSprite(SprQualityWzBg, bigLevel.ToString());
			NGUIUtil.SetSprite(SprQualityIco, bigLevel.ToString());
			SetChosenEnable(false);
			AddShipModel(info);
		}
//		
	}

	private void AddShipModel(StaticShipCanvas info)
	{
		GameObject go = NDLoad.LoadWndItem("ShipDesignMapItem", ShipModelParent.transform);
		if(go != null)
		{
			ShipDesignMapItem item = go.GetComponent<ShipDesignMapItem>();
			if(item != null)
			{
				KeyValue size = new KeyValue();
				size.key = SprShipBg.height;
				size.value = SprShipBg.width;

				item.SetData(info.Shape,info.Width,size);
			}
		}
	}
	public void SetChosenEnable(bool enable)
	{
		SprDesignChosen.gameObject.SetActive(enable && m_showSelect);
	}

	public void BtnSelectChange()
	{
		SetChosenEnable(BtnSelect.value);
		SetMaskActive(BtnSelect.value && m_showSelect);
		if(BtnSelect.value && m_showSelect)
		{
			if(m_SelectCallBack != null)
			{
				m_SelectCallBack(this);
			}
		}
	}

	/// <summary>
	/// 选兵 设定遮罩显示
	/// </summary>
	/// <param name="isactive"></param>
	public void SetMaskActive(bool isactive)
	{
		// 选定设定颜色为RGB=150
		Color gray = ColorUtils.FromArgb(255,150,150,150);
		Color white = ColorUtils.FromArgb(255,255,255,255);
		Color green = ColorUtils.FromArgb(255, 148, 255, 43);

		UISprite[] sprList = GetComponentsInChildren<UISprite>();
		if(sprList != null )
		{
			foreach (UISprite s in sprList)
			{
				if(s == SprDesignChosen) continue;
				NGUIUtil.SetSpriteColor(s, isactive == true ? gray : white);
			}
		}
//		SprDesignChosen.color = green;
	}

}
