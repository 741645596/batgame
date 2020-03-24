using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 炮弹兵滚动选择窗口
/// <From> </From>
/// <Author>QFord</Author>
/// </summary>
public class SoldierScrollWnd : WndBase 
{
	public SoldierInfo m_currentSoldierInfo;
	//public UICenterOnChild m_centerChild;
	public static int CurrentIndex = 0;

	public SoldierScrollWnd_h MyHead
	{
		get 
		{
			return (base.BaseHead () as SoldierScrollWnd_h);
		}
	}

    /// <summary>
    /// 当前已召唤的炮弹兵
    /// </summary>
    List<SoldierInfo> m_allExistSoldier = new List<SoldierInfo>();


    List<Transform> m_soldierItems = new List<Transform>();

    private float m_fScrollOffset=1f;

	public override void WndStart()
	{
		base.WndStart();

        SoldierDC.GetSoldiers(ref m_allExistSoldier, CombatLoactionType.ALL);
		SoldierM.SortSoldierList(ref m_allExistSoldier);

		if (MyHead.BtnLeft)
        {
			MyHead.BtnLeft.OnClickEventHandler += BtnLeft_OnClickEventHandler;
        }
		if (MyHead.BtnRight)
        {
			MyHead.BtnRight.OnClickEventHandler += BtnRight_OnClickEventHandler;
        }
		CreateItems();

	}

    void CreateItems()
    {
		if (MyHead.Parent == null)
        {
            return;
        }
        int count = m_allExistSoldier.Capacity;
        if (count > 1)
        {
            m_fScrollOffset = 1.0f / (count - 1);
			MyHead.m_scrollView.scrollWheelFactor = m_fScrollOffset;
        }
        int itemIndex = 0;
        foreach (SoldierInfo info in m_allExistSoldier)
        {
			GameObject go = NDLoad.LoadWndItem("SoldierScrollItem", MyHead.Parent.transform);
            if (go != null)
            {
                SoldierScrollItem item = go.GetComponent<SoldierScrollItem>();
                if (item)
                {

                    m_soldierItems.Add(go.transform);
                    item.SetUI(info);
					item.ItemIndex = itemIndex++;
                    go.name = info.m_name;
					NGUIUtil.SetItemPanelDepth(go, MyHead.Parent.GetComponentInParent<UIPanel>());
                }
            }
        }
        SetCurrentIndex();
		DoSelectItem(CurrentIndex);
		NGUIUtil.RepositionTablePivot(MyHead.Parent);
    }

    void SetCurrentIndex()
    {
        for (int i = 0; i < m_soldierItems.Count;i++ )
        {
            SoldierScrollItem ssi = m_soldierItems[i].GetComponent<SoldierScrollItem>();
            if (ssi == null)
            {
                continue;
            }
			if (ssi.m_info.ID == m_currentSoldierInfo.ID)
            {
				CurrentIndex = i;
                break;
            }
        }
        //CurrentIndex = 0;
    }

	public SoldierScrollWnd_h GetHead()
	{
		return MyHead;
	}

    void BtnRight_OnClickEventHandler(UIButton sender)
    {
        //if (m_scrollbar)
        //{
        //    float value = m_scrollbar.value + m_fScrollOffset;
        //    m_scrollbar.value = Mathf.Clamp01(value);
        //}
		if (CurrentIndex == (m_soldierItems.Count -1))
        {
            return;
        }
		DoSelectItem(++CurrentIndex);
		ViewSelectItem(m_soldierItems[CurrentIndex]);
    }

    void BtnLeft_OnClickEventHandler(UIButton sender)
    {
        //if (m_scrollbar)
        //{
        //    float value = m_scrollbar.value - m_fScrollOffset;
        //    m_scrollbar.value = Mathf.Clamp01(value);
        //}
		if (CurrentIndex == 0)
        {
            return;
        }
		DoSelectItem(--CurrentIndex);
		ViewSelectItem(m_soldierItems[CurrentIndex]);
    }

	public void ViewSelectItem(Transform selectT)
	{
		Bounds b = NGUIMath.CalculateRelativeWidgetBounds(selectT, false);
		Vector3 scale = selectT.localScale;
		b.min = Vector3.Scale(b.min, scale);
		b.max = Vector3.Scale(b.max, scale);
		UIPanel panel = MyHead.m_scrollView.GetComponent<UIPanel>();
		Vector3 min = b.min + selectT.localPosition;
		Vector3 max = b.max + selectT.localPosition;
		UITablePivot table = selectT.parent.GetComponent<UITablePivot>();
		//Debug.Log(panel.finalClipRegion + "," +panel.baseClipRegion);
		
		Vector3 worldmin = selectT.parent.TransformPoint(min);
		Vector3 worldmax = selectT.parent.TransformPoint(max);
		if (!panel.IsVisible(worldmin) || !panel.IsVisible(worldmax))
		{
			
			//Vector3 localmin = selectT.parent.parent.InverseTransformPoint(worldmin);
				//Vector3 localmax = selectT.parent.parent.InverseTransformPoint(worldmax);
			//Vector3 scrollpos = m_scrollView.transform.localPosition  + table.transform.localPosition;
			if ((min.x - table.padding.x) < ( -MyHead.m_scrollView.transform.localPosition.x))
				{
					Vector3 pos = Vector3.zero;
				pos.x =    /*(panel.baseClipRegion.x - m_scrollView.transform.localPosition.x) - */-(min.x- table.padding.x);
					//m_scrollView.MoveRelative(pos);
				SpringPanel.Begin(MyHead.m_scrollView.gameObject, pos, 13f).strength = 8f;
				}
			if ((max.x + table.padding.x) > ( -MyHead.m_scrollView.transform.localPosition.x + panel.finalClipRegion.z))
				{
					//panel.clip
					Vector3 pos = Vector3.zero;
				pos.x =   ( -MyHead.m_scrollView.transform.localPosition.x+ panel.finalClipRegion.z) - (max.x + table.padding.x);
					//m_scrollView.MoveRelative(pos);
				SpringPanel.Begin(MyHead.m_scrollView.gameObject, MyHead.m_scrollView.transform.localPosition + pos, 13f).strength = 8f;
				}
			/*Vector3 pos = Vector3.zero;
				pos.x = min.x -  m_scrollView.transform.localPosition.x;
				m_scrollView.MoveRelative(pos);*/
		}
	}
    public void DoSelectItem(Transform selectT)
    {
        foreach (var item in m_soldierItems)
        {
            item.localScale = Vector3.one;
        }
        if (selectT!=null)
        {
            selectT.localScale = new Vector3(1.2f, 1.2f, 1.2f);
			NGUIUtil.RepositionTablePivot(MyHead.Parent);
            SoldierInfoWnd wnd = WndManager.FindDialog<SoldierInfoWnd>();
            if (wnd)
            {
                SoldierScrollItem ssi = selectT.GetComponent<SoldierScrollItem>();
                if (ssi)
                {
					CurrentIndex = ssi.ItemIndex;
					wnd.SetData(ssi.m_info);
                }
            }
        }
    }

    void DoSelectItem(int index)
    {
        DoSelectItem(m_soldierItems[index]);
    }

   
	
}
