using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum WndType {
    /// <summary>
    /// 船只编辑中选择设计图.
    /// </summary>
    ShipEdit = 1,
    /// <summary>
    /// show in The package 背包中展示.
    /// </summary>
    PackgeShow = 2,
}
public enum ShipModemType {
    ShipAllModel = 0,
    Ship8Model = 1,
    Ship12Model = 2,
    Ship16Model = 3,
    Ship24Model = 4,
    Ship32Model = 5,
}
public class ShipDesignWnd : WndBase
{
    public ShipDesignWnd_h MyHead {
        get
        {
            return (base.BaseHead() as ShipDesignWnd_h);
        }
    }
    
    private WndType m_iWndType = WndType.PackgeShow;
    private ShipDesignItem m_selectItem;
    public override void WndStart()
    {
        base.WndStart();
        BindingEvent();
        NGUIUtil.UpdatePanelValue(MyHead.PanelMask, 1024, 0.15f);
        ShowTogglesAni(0.05f, 0.05f);
        NGUIUtil.TweenGameObjectPosX(MyHead.ScrollLeft, -461, 0.15f, gameObject, "HideAniScroll");
        NGUIUtil.TweenGameObjectPosX(MyHead.ScrollRight, 461, 0.15f);
        
        
    }
    /// <summary>
    /// 隐藏左右两个动画轴
    /// </summary>
    public void HideAniScroll()
    {
        NGUIUtil.SetActive(MyHead.ScrollLeft, false);
        NGUIUtil.SetActive(MyHead.ScrollRight, false);
        NGUIUtil.SetPanelClipping(MyHead.PanelMask, UIDrawCall.Clipping.None);
    }
    
    
    public void SetData(WndType type = WndType.PackgeShow)
    {
        m_iWndType = type;
        SetUI();
    }
    void BindingEvent()
    {
        MyHead.BtnClose.OnClickEventHandler += Close;
        MyHead.BtnEdit.OnClickEventHandler += Edit;
        EventDelegate.Add(MyHead.BtnModelAll.onChange, BtnModelAll_OnClickEventHandler);
        EventDelegate.Add(MyHead.Btn8Model.onChange, Btn8Model_OnClickEventHandler);
        EventDelegate.Add(MyHead.Btn12Model.onChange, Btn12Model_OnClickEventHandler);
        EventDelegate.Add(MyHead.Btn16Model.onChange, Btn16Model_OnClickEventHandler);
        EventDelegate.Add(MyHead.Btn24Model.onChange, Btn24Model_OnClickEventHandler);
        EventDelegate.Add(MyHead.Btn32Model.onChange, Btn32Model_OnClickEventHandler);
    }
    void SetUI()
    {
        MyHead.Lbl8Model.text = string.Format(NGUIUtil.GetStringByKey(70000232), ConfigM.GetShipDesignCellRange(ShipModemType.Ship8Model).Unit);
        MyHead.Lbl12Model.text = string.Format(NGUIUtil.GetStringByKey(70000232), ConfigM.GetShipDesignCellRange(ShipModemType.Ship12Model).Unit);
        MyHead.Lbl16Model.text = string.Format(NGUIUtil.GetStringByKey(70000232), ConfigM.GetShipDesignCellRange(ShipModemType.Ship16Model).Unit);
        MyHead.Lbl24Model.text = string.Format(NGUIUtil.GetStringByKey(70000232), ConfigM.GetShipDesignCellRange(ShipModemType.Ship24Model).Unit);
        MyHead.Lbl32Model.text = string.Format(NGUIUtil.GetStringByKey(70000232), ConfigM.GetShipDesignCellRange(ShipModemType.Ship32Model).Unit);
        
        MyHead.goDesGroup.SetActive(false);
    }
    
    void Edit(UIButton sender)
    {
        if (m_selectItem != null) {
            BattleEnvironmentM.ReLoadShipCanvans(m_selectItem.GetShipDesignID());
            ShipCanvasWnd wnd = WndManager.FindDialog<ShipCanvasWnd>();
            if (wnd != null) {
                wnd.LoadShipDesign();
            }
        }
        Close(null);
    }
    void Close(UIButton sender)
    {
        WndManager.DestoryDialog<ShipDesignWnd>();
    }
    //卷轴动画.
    /// <summary>
    /// 显示toggle动画
    /// </summary>
    /// <param name="delay">开始显示第一个toggle的延时</param>
    /// <param name="interval">显示下一个toggle的间隔</param>
    void ShowTogglesAni(float delay, float interval)
    {
        GameObjectActionExcute gae = GameObjectActionExcute.CreateExcute(gameObject);
        float waitInterval = delay;
        for (int i = 0; i < MyHead.ToggleList.Length; i++) {
            GameObjectActionWait wait = new GameObjectActionWait(waitInterval, WaitFinish);
            wait.Data1 = i;
            gae.AddAction(wait);
            waitInterval = interval;
        }
    }
    /// <summary>
    /// 顺序显示toggle
    /// </summary>
    private void WaitFinish(object o)
    {
        GameObject go = o as GameObject;
        if (go == null) {
            return;
        }
        GameObjectActionExcute gae = go.GetComponent<GameObjectActionExcute>();
        if (gae) {
            GameObjectActionWait wait = gae.GetCurrentAction() as GameObjectActionWait;
            if (wait != null) {
                int index = (int)wait.Data1;
                NGUIUtil.SetActive(MyHead.ToggleList[index].gameObject, true);
            }
        }
    }
    
    #region toggle
    public void BtnModelAll_OnClickEventHandler()
    {
        if (!MyHead.BtnModelAll.value) {
            return;
        }
        CreateWhenSelectChange(ShipModemType.ShipAllModel);
    }
    public void Btn8Model_OnClickEventHandler()
    {
        if (!MyHead.Btn8Model.value) {
            return;
        }
        CreateWhenSelectChange(ShipModemType.Ship8Model);
    }
    public void Btn12Model_OnClickEventHandler()
    {
        if (!MyHead.Btn12Model.value) {
            return;
        }
        CreateWhenSelectChange(ShipModemType.Ship12Model);
    }
    public void Btn16Model_OnClickEventHandler()
    {
        if (!MyHead.Btn16Model.value) {
            return;
        }
        CreateWhenSelectChange(ShipModemType.Ship16Model);
    }
    public void Btn24Model_OnClickEventHandler()
    {
        if (!MyHead.Btn24Model.value) {
            return;
        }
        CreateWhenSelectChange(ShipModemType.Ship24Model);
    }
    public void Btn32Model_OnClickEventHandler()
    {
        if (!MyHead.Btn32Model.value) {
            return;
        }
        CreateWhenSelectChange(ShipModemType.Ship32Model);
    }
    #endregion
    
    
    void CreateWhenSelectChange(ShipModemType type)
    {
        U3DUtil.DestroyAllChild(MyHead.Table.gameObject);
        List<StaticShipCanvas> l = ShipPlanDC.GetAllShipDesignList(type);
        SortCanvasList(ref l);
        for (int i = 0; i < l.Count; i++) {
            StaticShipCanvas Info = l[i];
            
            GameObject go = NDLoad.LoadWndItem("ShipDesignItem", MyHead.Table.transform);
            if (go != null) {
                ShipDesignItem item = go.GetComponent<ShipDesignItem>();
                if (item != null) {
                    item.SetData(Info, m_iWndType == WndType.ShipEdit, Info.ShipDesignID);
                    item.m_SelectCallBack = ShipDesignItemCallBack;
                }
            }
        }
        StartCoroutine(RepositionTable(1));
        
    }
    void ShipDesignItemCallBack(ShipDesignItem item)
    {
        m_selectItem = item;
        MyHead.goDesGroup.SetActive(true);
    }
    IEnumerator  RepositionTable(int frameCount)
    {
        yield return StartCoroutine(U3DUtil.WaitForFrames(frameCount));
        MyHead.Table.Reposition();
        MyHead.Table.repositionNow = true;
    }
    
    public static void SortCanvasList(ref List<StaticShipCanvas> list)
    {
        list.Sort((obj1, obj2) => {
            int ret = obj1.Cell.CompareTo(obj2.Cell);
            if (ret != 0) {
                return ret;
            }
            
            ret = obj1.Quality.CompareTo(obj2.Quality);
            if (ret != 0) {
                return ret;
            }
            
            ret = obj1.StarLevel.CompareTo(obj2.StarLevel);
            if (ret != 0) {
                return ret;
            }
            
            return obj1.Id.CompareTo(obj2.Id);
        }
        );
        
        list.Reverse();
    }
}