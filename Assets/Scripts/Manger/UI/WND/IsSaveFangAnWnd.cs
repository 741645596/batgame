using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 提示：退出后是否要保存方案窗口
/// <From> 新船只生成 - 船只编辑调整</From>
/// <Author>QFord</Author>
/// </summary>
public class IsSaveFangAnWnd : WndBase
{

    public IsSaveFangAnWnd_h MyHead {
        get
        {
            return (base.BaseHead() as IsSaveFangAnWnd_h);
        }
    }
    
    public override void WndStart()
    {
        base.WndStart();
        if (MyHead.BtnYES) {
            MyHead.BtnYES.OnClickEventHandler += BtnYES_OnClickEventHandler;
        }
        if (MyHead.BtnNO) {
            MyHead.BtnNO.OnClickEventHandler += BtnNO_OnClickEventHandler;
        }
        PutCanvasM.ShowRoomGridUI(false);
    }
    
    void BtnNO_OnClickEventHandler(UIButton sender)
    {
        ShipPlan P = ShipPlanDC.GetCurShipPlan();
        P.ResetPlan();
        WndManager.DestoryDialog<IsSaveFangAnWnd>();
        ShipCanvasWnd wnd = WndManager.FindDialog<ShipCanvasWnd>();
        if (wnd) {
            wnd.DoReturn();
        }
    }
    
    void BtnYES_OnClickEventHandler(UIButton sender)
    {
        WndManager.DestoryDialog<IsSaveFangAnWnd>();
        ShipCanvasWnd wnd = WndManager.FindDialog<ShipCanvasWnd>();
        if (wnd) {
            wnd.BtnNext_OnClickEventHandler(null);
        }
    }
    
    
    
}
