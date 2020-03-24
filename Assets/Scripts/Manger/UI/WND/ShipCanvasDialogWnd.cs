using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipCanvasDialogWnd : WndBase {

	public ShipCanvasDialogWnd_h MyHead
	{
		get 
		{
			return (base.BaseHead () as ShipCanvasDialogWnd_h);
		}
	}

    private List<BuildInfo> warehouseBuildList;
    private List<SoldierInfo> soldierList;
    private ShipBuildType m_shipBuildType;

    private List<CanvasItem> m_items = new List<CanvasItem>();
    public override void BindEvents()
    {
        base.BindEvents();

		if (MyHead.BtnClose)
        {
			MyHead.BtnClose.OnClickEventHandler += BtnClose_OnClickEventHandler;
        }
		if (MyHead.SprCover)
        {
			MyHead.SprCover.width = Screen.width;
			MyHead.SprCover.height = Screen.height;
        }
    }

    void BtnClose_OnClickEventHandler(UIButton sender)
    {
        //TouchMove.s_CanOperate = true;
        MainCameraM.s_Instance.EnableDrag(true);
        WndManager.DestoryDialog<ShipCanvasDialogWnd>();
    }

	// Use this for initialization
	public override void WndStart()
	{
		base.WndStart();
       // TouchMove.s_CanOperate = false;
        CreateList();
		NGUIUtil.RepositionTable(MyHead.ListParent);
	}

    private void CreateList()
    {
        if (m_shipBuildType == ShipBuildType.BuildRoom)
        {
            foreach (var a in warehouseBuildList)
            {
                if(TouchMoveManager.CheckHaveExist(m_shipBuildType,a.ID) == false)
				{
					AddBuild(a);
				}
                   
            }
        }
        if (m_shipBuildType == ShipBuildType.Soldier)
        {
            foreach (var a in soldierList)
            {
                if (TouchMoveManager.CheckHaveExist(m_shipBuildType, a.ID) == false)
                    AddSoldier(a);
            }
        }
    }
    /// <summary>
    /// 填充建造类型数据
    /// </summary>
    public void FillData(ShipBuildType type)
    {
        m_shipBuildType = type;
        switch (type)
        {
            case ShipBuildType.BuildRoom:
			warehouseBuildList = BuildDC.GetBuildingList(AttributeType.ALL);
            break;
            case ShipBuildType.Soldier:
			    SoldierDC.GetSoldiers(ref soldierList, CombatLoactionType.ALL);
            break;

            default:
                break;
        }
    }

	private void AddBuild(BuildInfo WareHouse)
    {
		if (MyHead.ListParent == null || MyHead.CanvasItem == null)
        {
            NGUIUtil.DebugLog("ShipCanvasDialogWnd ListParent || CanvasItem is null!!!");
            return;
        }
		GameObject go = NGUITools.AddChild(MyHead.ListParent, MyHead.CanvasItem);
        CanvasItem item = go.GetComponent<CanvasItem>();
        if (item)
        {
			item.SetCanvasItem(WareHouse);
        }
    }

    private void AddSoldier(SoldierInfo Soldier)
    {
		if (MyHead.ListParent == null || MyHead.CanvasItem == null)
        {
            NGUIUtil.DebugLog("ShipCanvasDialogWnd ListParent || CanvasItem is null!!!");
            return;
        }
		GameObject go = NGUITools.AddChild(MyHead.ListParent, MyHead.CanvasItem);
        CanvasItem item = go.GetComponent<CanvasItem>();
        if (item)
        {
			item.SetCanvasItem(Soldier);
        }
    }


	
	
}
