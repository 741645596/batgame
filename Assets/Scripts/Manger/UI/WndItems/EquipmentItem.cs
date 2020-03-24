using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using sdata;
/// <summary>
///  装备项
/// <From>炮弹兵详细信息界面</From>
/// <Author>QFord</Author>
/// </summary>
public class EquipmentItem : MonoBehaviour {

	public EquipmentItem_h MyHead
	{
		get 
		{
            return (GetComponent < EquipmentItem_h>());
		}
	}

    private EquipmentPutType m_equipmentPutType;
    private int m_icon;//装备的icon
    private int m_iEquipQuality;
    private int m_iItemID;//d_item 的ID
    private int m_itemTypeID;
    private int m_iPosIndex;
    private SoldierInfo m_soldierInfo;

    //void Start () 
    //{
        
    //}
   public bool bGuideSelect = false;

    public EquipmentPutType EquipPutType
    {
        get { return m_equipmentPutType; }
    }

    void SetUI()
    {
        if (MyHead.BtnItem)
        {
            MyHead.BtnItem.OnClickEventHandler += BtnItem_OnClickEventHandler;
        }

        switch (m_equipmentPutType)
        {
            case EquipmentPutType.NoCanCombine:
				MyHead.LblTip.text = string.Format("[FFFFFF]{0}[-]", NGUIUtil.GetStringByKey("88800059"));
                NGUIUtil.Set2DSpriteGray(MyHead.SprEquip, "Textures/item/", m_icon.ToString());
            break;

            case EquipmentPutType.None:
                NGUIUtil.Set2DSpriteGray(MyHead.SprEquip, "Textures/item/", m_icon.ToString());
            break;

            case EquipmentPutType.CanPut:
                MyHead.SprCanEquip.gameObject.SetActive(true);
				MyHead.LblTip.text = string.Format("[00FF00]{0}[-]", NGUIUtil.GetStringByKey("88800060"));
                NGUIUtil.Set2DSpriteGray(MyHead.SprEquip, "Textures/item/", m_icon.ToString());
            break;

            case EquipmentPutType.HavePut:
                MyHead.LblTip.gameObject.SetActive(false);
                NGUIUtil.Set2DSprite(MyHead.SprEquip, "Textures/item/", m_icon.ToString());
                NGUIUtil.SetSprite(MyHead.SprQuality, m_iEquipQuality.ToString());
            break;

            case EquipmentPutType.HaveCannot:
                MyHead.LblTip.text = string.Format("[FFFFFF]{0}[-]", NGUIUtil.GetStringByKey("88800111"));
                MyHead.SprCanEquip.gameObject.SetActive(true);
                NGUIUtil.SetSprite(MyHead.SprCanEquip,"pdbxx_003ic");
                NGUIUtil.Set2DSpriteGray(MyHead.SprEquip, "Textures/item/", m_icon.ToString());
            break;

            case EquipmentPutType.CanCombine:
            case EquipmentPutType.CanCombinePut:
                MyHead.LblTip.text = string.Format("[00FF00]{0}[-]", NGUIUtil.GetStringByKey("88800109"));
                NGUIUtil.Set2DSpriteGray(MyHead.SprEquip, "Textures/item/", m_icon.ToString());
			break;
			case EquipmentPutType.ReadyCombine:
			MyHead.LblTip.text = string.Format("[FFFFFF]{0}[-]", NGUIUtil.GetStringByKey("88800059"));
				NGUIUtil.Set2DSpriteGray(MyHead.SprEquip, "Textures/item/", m_icon.ToString());
            break;
        }
    }

    public void BtnItem_OnClickEventHandler(UIButton sender)
    {
        EquipmentInfoWnd wnd = WndManager.GetDialog<EquipmentInfoWnd>();
        if (wnd== null)
            return;

        if (m_equipmentPutType == EquipmentPutType.CanPut)
		{
            if (bGuideSelect)
            {
                wnd.BGuideSelect = true;
            }
			wnd.SetData(m_itemTypeID, m_iItemID, m_soldierInfo, m_iPosIndex,3);
		}
        else if (m_equipmentPutType == EquipmentPutType.HaveCannot)
            wnd.SetData(m_itemTypeID, 0, m_soldierInfo, m_iPosIndex, 1);

        else if (m_equipmentPutType == EquipmentPutType.CanCombine)
            wnd.SetData(m_itemTypeID, 0, m_soldierInfo, m_iPosIndex, 2);

        else if (m_equipmentPutType == EquipmentPutType.NoCanCombine)
        {
            wnd.SetData(m_itemTypeID, 0, m_soldierInfo, m_iPosIndex, 5);
        }

        else if (m_equipmentPutType == EquipmentPutType.HavePut)
            wnd.SetData(m_itemTypeID, 0, m_soldierInfo, m_iPosIndex, 1);
        else if (m_equipmentPutType == EquipmentPutType.ReadyCombine)
        {
            wnd.SetData(m_itemTypeID, 0, m_soldierInfo, m_iPosIndex, 2);
        }
        else
            wnd.SetData(m_itemTypeID, 0, m_soldierInfo, m_iPosIndex, 2);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dItemID"></param>
    /// <param name="posIndex">装备索引 0~5</param>
    /// <param name="solderierInfo"></param>
    public void SetData(int dItemID,int posIndex,SoldierInfo solderierInfo)
    {
        m_iItemID = dItemID;
        m_soldierInfo = solderierInfo;
        m_iPosIndex = posIndex;
        m_itemTypeID = SoldierM.GetSoldierEquip(solderierInfo.SoldierTypeID, solderierInfo.Quality, posIndex);
        if ( m_itemTypeID == 0)
        {
            NGUIUtil.DebugLog(string.Format(" SoldierTypeID={0},Quality={1},EquipIndex={2} 装备静态数据未配置", solderierInfo.SoldierTypeID, solderierInfo.Quality, posIndex));
            return;
        }
        if (m_iItemID == 0)//当未穿装备时
        {
			ItemTypeInfo info = ItemDC.SearchItem(m_itemTypeID);
            if (info!=null)
            {
                m_iItemID = info.ID;
                m_itemTypeID = info.itemType;
                m_icon = info.m_Icon;
            }
            else
            {
                s_itemtypeInfo sInfo = ItemM.GetItemInfo(m_itemTypeID);
                if (sInfo != null)
                    m_icon = sInfo.icon;
                else
                    NGUIUtil.DebugLog("itemTypeID =" + m_itemTypeID + " 未配置");
            }
        }
        else//已装备
        {
			ItemTypeInfo info1 = ItemDC.GetItem(m_iItemID);
			if (info1 == null || info1.Positon == 0)
            {
                NGUIUtil.DebugLog("获取已装备的信息失败 itemID =" + m_iItemID);
                return;
            }
			m_icon = info1.m_Icon;
            if (info1!=null)
            {
                m_iEquipQuality = info1.m_Quality;
            }
        }
		int itemTypeid = 0;
		m_equipmentPutType = SoldierM.CheckCanPutEquip (solderierInfo,dItemID,posIndex,ref itemTypeid);
        SetUI();
    }

}


/// <summary>
/// 装备穿戴类型
/// </summary>
public enum EquipmentPutType
{
    /// <summary>
    /// 无装备
    /// </summary>
    None,
    /// <summary>
    /// 可装备
    /// </summary>
    CanPut,

    /// <summary>
    /// 可合成（可通过合成得到的装备,且材料足够）
    /// </summary>
    CanCombine,
	/// <summary>
	/// 可合成（可通过合成得到的装备,且材料足够）可装备.
	/// </summary>
	CanCombinePut,
    /// <summary>
    /// 可合成（可通过合成得到装备且材料不够）
    /// </summary>
    ReadyCombine,
    /// <summary>
    /// 不可合成（直接通过掉落得到的装备）
    /// </summary>
    NoCanCombine,
    /// <summary>
    /// 已装备
    /// </summary>
    HavePut,
    /// <summary>
    /// 有装备但不可装备
    /// </summary>
    HaveCannot,
}
