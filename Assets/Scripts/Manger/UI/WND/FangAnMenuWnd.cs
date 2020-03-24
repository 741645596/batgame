using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 方案菜单
/// </summary>
public class FangAnMenuWnd : WndBase
{
	public FangAnMenuWnd_h MyHead
	{
		get 
		{
			return (base.BaseHead () as FangAnMenuWnd_h);
		}
	}
    private float m_fMinHeight = 500;
    private float m_fMaxHeight = 229;
    private List<FangAnBtnItem> m_listFangAnBtnItem;
    private bool m_bOpenMainMenu;

    public override void WndStart()
    {
        base.WndStart();
        MainMenuInit();
        CreateItems();
	}
    /// <summary>
    /// 加载方案项
    /// </summary>
    private void CreateItems()
    {
        if (MyHead.FangAnTable == null)
        {
            return;
        }
        MyHead.FangAnTable.gameObject.SetActive(false);

        List<ShipPlan> infos = ShipPlanDC.GetAllShipPlan();
        m_listFangAnBtnItem = new List<FangAnBtnItem>(infos.Count);
		foreach (ShipPlan info in infos)
        {
            GameObject go = NDLoad.LoadWndItem("FangAnBtnItem", MyHead.FangAnTable);
            if (go)
            {
                FangAnBtnItem item = go.GetComponent<FangAnBtnItem>();
                if (item)
                {
                    item.SetData(info);
                    m_listFangAnBtnItem.Add(item);
                }
                NGUIUtil.SetItemPanelDepth(go, MyHead.FangAnTable.parent.gameObject);
            }
        }
        NGUIUtil.RepositionTable(MyHead.FangAnTable.gameObject);
        
    }
    /// <summary>
    /// 主菜单初始设定
    /// </summary>
    void MainMenuInit()
    {
        m_bOpenMainMenu = false;//默认是关闭主菜单

        if (MyHead.BtnClick)
        {
            MyHead.BtnClick.OnClickEventHandler += BtnClick_OnClickEventHandler;
        }

        if (MyHead.BtnWndBackground)
        {
            MyHead.BtnWndBackground.OnClickEventHandler += BtnWndBackground_OnClickEventHandler;
        }

        if (MyHead.GoTweenSprite)
        {
            Vector3 pos = MyHead.GoTweenSprite.transform.localPosition;
            if (m_bOpenMainMenu == true)
            {
                MyHead.GoTweenSprite.transform.localPosition = U3DUtil.SetY(pos, m_fMaxHeight);
            }
            else
            {
                MyHead.GoTweenSprite.transform.localPosition = U3DUtil.SetY(pos, m_fMinHeight);
            }
        }

        if (MyHead.LblFangAnName != null)
        {
			ShipPlan P = ShipPlanDC.GetCurShipPlan();
            MyHead.LblFangAnName.text = P.Name;
        }
          
    }

    void BtnWndBackground_OnClickEventHandler(UIButton sender)
    {
        ClickMenuBtn();
    }

    public void BtnClick_OnClickEventHandler(UIButton sender)
    {
        ClickMenuBtn();
    }

    /// <summary>
    /// 点击主菜单处理
    /// </summary>
    void ClickMenuBtn()
    {
        if (m_bOpenMainMenu == false)//打开主菜单
        {
            m_bOpenMainMenu = true;
            MyHead.BtnClick.gameObject.SetActive(false);
            NGUIUtil.TweenGameObjectPosY(MyHead.GoTweenSprite, m_fMinHeight, m_fMaxHeight, 0.2f, 0, gameObject, "EnableBtn");
            MyHead.WndBackground.SetActive(true);
        }
        else
        {
            MyHead.FangAnTable.gameObject.SetActive(false);
            m_bOpenMainMenu = false;
            NGUIUtil.TweenGameObjectPosY(MyHead.GoTweenSprite, m_fMaxHeight, m_fMinHeight, 0.2f);
            MyHead.WndBackground.SetActive(false);
        }
    }

     public void EnableBtn()
    {
        if (MyHead.BtnClick)
        {
            MyHead.BtnClick.gameObject.SetActive(true);
        }
        if (MyHead.FangAnTable)
        {
            MyHead.FangAnTable.gameObject.SetActive(true);
        }
    }

     public void RefreshFangAnName(ShipPlan P,string name)
     {
         if (MyHead.LblFangAnName)
         {
             MyHead.LblFangAnName.text = name;
         }
         foreach (FangAnBtnItem item in m_listFangAnBtnItem)
         {
			if (item.m_ShipPlan.ID == P.ID)
             {
                 item.SetData(P);
             }
         }
     }

    
	
}
