using UnityEngine;
using System;
using System.Collections;

/// <summary>
///  背包物品信息界面
/// <From> </From>
/// <Author>QFord</Author>
/// </summary> 
public class ItemInfoWnd : WndBase
{
	public ItemInfoWnd_h MyHead
	{
		get 
		{
			return (base.BaseHead () as ItemInfoWnd_h);
		}
	}

    private ItemTypeInfo m_info;

    public void SetData(ItemTypeInfo info)
    {
        m_info = info;

        SetUI();
    }
    private void SetUI()
    {
		NGUIUtil.Set2DSprite(MyHead.SprType, "Textures/item/", m_info.m_Icon.ToString());
		if (MyHead.LblName)
        {
			MyHead.LblName.text = m_info.m_Name;
        }
		if (MyHead.LblCount)
        {
			int Count = ItemDC.GetItemCount(m_info.itemType);
			MyHead.LblCount.text = string.Format("[552d0a]"+NGUIUtil.GetStringByKey("88800040")+"[-][0000FF]{0}[-][552d0a]"+NGUIUtil.GetStringByKey("88800037")+"[-]", Count);
        }

		if (MyHead.LblDescription1)
        {
			MyHead.LblDescription1.text = m_info.m_title;
        }
		if (MyHead.LblDescription2)
        {
            if (m_info.m_message !="0")
            {
				MyHead.LblDescription2.text = m_info.m_message;
            }
            else
            {
				MyHead.LblDescription2.text = "";
            }
        }
		if (MyHead.LblPrice)
        {
			MyHead.LblPrice.text = m_info.m_sellemoney.ToString();
        }
        //http://jingyan.baidu.com/article/5552ef47d82a90518ffbc914.html
		if (MyHead.AnchorBg != null)
        {
            BeiBaoWnd wnd = WndManager.FindDialog<BeiBaoWnd>();
            if (wnd!=null)
            {
                Transform target = wnd.MyHead.AnchorTarget;
                if (target!=null)
                {
					MyHead.AnchorBg.SetAnchor(target);
					MyHead.AnchorBg.leftAnchor.absolute = -390;
					MyHead.AnchorBg.rightAnchor.relative = 0;
					MyHead.AnchorBg.rightAnchor.absolute = 6;
					MyHead.AnchorBg.bottomAnchor.absolute = -7;
					MyHead.AnchorBg.topAnchor.absolute = 9;
                }
            }
        }

    }

    public override void WndStart()
	{
        base.WndStart();
		if (MyHead.BtnSell)
        {
			MyHead.BtnSell.OnClickEventHandler += BtnSell_OnClickEventHandler;
        }
		if (MyHead.BtnUse)
        {
			MyHead.BtnUse.OnClickEventHandler += BtnUse_OnClickEventHandler;
        }
		if (MyHead.BtnDetail)
        {
			MyHead.BtnDetail.OnClickEventHandler += BtnDetail_OnClickEventHandler;
        }
        //SetUI();
	}

    void BtnDetail_OnClickEventHandler(UIButton sender)
    {
        
    }

    void BtnUse_OnClickEventHandler(UIButton sender)
    {
        
    }

    void BtnSell_OnClickEventHandler(UIButton sender)
    {
        
    }


    
	
	
	
}
