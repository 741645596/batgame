using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 方案按钮项
/// <From> </From>
/// <Author>QFord</Author>
/// </summary>
public class FangAnBtnItem : WndBase {

	public FangAnBtnItem_h MyHead
	{
		get 
		{
			return (base.BaseHead () as FangAnBtnItem_h);
		}
	}
    public ShipPlan m_ShipPlan;

	public void SetData(ShipPlan Plan)
    {
		m_ShipPlan = Plan;
		if (m_ShipPlan!=null)
        {
            SetUI();
        }
    }

	void Start () 
	{
        if (MyHead.BtnSelectFangAn)
        {
            MyHead.BtnSelectFangAn.OnClickEventHandler += BtnSelectFangAn_OnClickEventHandler;
        }
	}

    void SetUI()
    {
        if (MyHead.LblFangAnName)
        {
			MyHead.LblFangAnName.text = m_ShipPlan.Name;
        }
        if (MyHead.SprBg)
        {
			ShipPlan CurPlan = ShipPlanDC.GetCurShipPlan();
			if (CurPlan.ID == m_ShipPlan.ID)
            {
                MyHead.SprBg.spriteName = "bj_bg005";
            }
            else
            {
                MyHead.SprBg.spriteName = "bj_bg006";
            }
        }
        
    }

    void BtnSelectFangAn_OnClickEventHandler(UIButton sender)
    {
        //NGUIUtil.DebugLog("Name = "+m_shipCanvasInfo.name);
        FangAnMenuWnd wnd = WndManager.FindDialog<FangAnMenuWnd>();
        if (wnd)
        {
            wnd.BtnClick_OnClickEventHandler(null);
        }

    }
	
}
