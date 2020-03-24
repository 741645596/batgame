using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using sdata;

/// <summary>
/// 用来合成装备的  装备或材料
/// <Author>QFord</Author>
/// </summary>
public class ECEquipComposeItem : MonoBehaviour {

    public UI2DSprite Spr2dEquipIcon;
    public UISprite SprEquipQuality;
    public UILabel LblNum;

    public UIButton BtnClick;

    private int m_itemTypeID;
    private int m_iNeedNum;



	public void SetData(int ItemTypeID,int needNum,ref List<int> listUsedItem)
    {
        m_itemTypeID = ItemTypeID;
        m_iNeedNum = needNum;

		SetUI(ref listUsedItem);
    }
	
	void Start () 
	{
        BtnClick.OnClickEventHandler += BtnClick_OnClickEventHandler;
	}

    void BtnClick_OnClickEventHandler(UIButton sender)
    {

    }
    /// <summary>
    /// 合成装备后刷新UI（刷新数量）
    /// </summary>
	public void RefreshUI(ref List<int> listUsedItem)
    {
		SetUI(ref listUsedItem);
    }

	void SetUI(ref List<int> listUsedItem)
    {
        s_itemtypeInfo info = ItemM.GetItemInfo(m_itemTypeID);
        if (info!=null)
        {
            NGUIUtil.Set2DSprite(Spr2dEquipIcon, "Textures/item/", info.icon.ToString());
            int quality = ConfigM.GetBigQuality(info.quality);
            NGUIUtil.SetSprite(SprEquipQuality, quality.ToString());
        }
        else
        {
            NGUIUtil.DebugLog(" ItemM.GetItemInfo " + m_itemTypeID.ToString() + " not found!");
            return;
        }
        //设置装备总数量和合成所需的数量
        int totalCount = ItemDC.GetItemCount(m_itemTypeID);
		int countUsed = listUsedItem.Count;
		if (totalCount > 0) 
		{
			int nCountTemp=totalCount;
			int nCnt =0;
			for(nCnt =0;nCnt<countUsed;nCnt++)
			{
				if(listUsedItem[nCnt]==m_itemTypeID)
				{
					nCountTemp--;
					if (nCountTemp == 0) 
						break;
				}
			}
			for(nCnt =0;nCnt<m_iNeedNum;nCnt++)
			{
				listUsedItem.Add(m_itemTypeID);
			}
			if(nCountTemp<m_iNeedNum)
				totalCount = nCountTemp;

		}
		string numText = "";
        if (totalCount >= m_iNeedNum) 
		{
			numText = string.Format ("[FFFFFF]{0}/{1}[-]", totalCount, m_iNeedNum);
		}
        else
            numText = string.Format("[FF0000]{0}/{1}[-]", totalCount, m_iNeedNum);
        NGUIUtil.SetLableText<string>(LblNum, numText);
    }


}
