using UnityEngine;

/// <summary>
/// 船只画布窗口
/// </summary>
public class ShipCanvasWnd : WndBase
{

    private bool m_IsSaving = false;
    
    public ShipCanvasWnd_h MyHead {
        get
        {
            return (base.BaseHead() as ShipCanvasWnd_h);
        }
    }
    
    private bool m_isAthleticeShip = false;
    
    private GameObject m_GoCanvasBox;
    //是否在编辑船只.
    private bool m_CanvasShip = true;
    //是否在编辑船只.
    public bool GetShipCanvasStatus()
    {
        return m_CanvasShip;
    }
    private bool m_bFirstLoad = true;
    
    public override void WndStart()
    {
        base.WndStart();
        
        if (MyHead.BtnReturn) {
            MyHead.BtnReturn.OnClickEventHandler += BtnReturn_OnClickEventHandler;
        }
        if (MyHead.BtnNext) {
            MyHead.BtnNext.OnClickEventHandler += BtnNext_OnClickEventHandler;
        }
        
        MainCameraM.s_Instance.EnableDrag(false);
        m_IsSaving = false;
        PutCanvasM.CanOperate = true;
        //TouchMove.s_CanOperate = true;
        //TouchMoveManager.HideDeckCanvasUnit();
        
        CangKuWnd wnd = WndManager.GetDialog<CangKuWnd>();
        ShipPlan P = ShipPlanDC.GetCurShipPlan();
        P.BackupPlan();
        LoadShipDesign();
        //LoadCanvasBox();
    }
    public void BtnSelectChange()
    {
        BtnDesign_OnClickEventHandler(null);
    }
    public void LoadShipDesign()
    {
        U3DUtil.DestroyAllChild(MyHead.ShipDesignParent, false);
        StaticShipCanvas canvas = ShipPlanDC.GetCurShipDesignInfo();
        if (canvas != null) {
            GameObject go = NDLoad.LoadWndItem("ShipDesignItem", MyHead.ShipDesignParent.transform);
            if (go != null) {
                go.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
                ShipDesignItem item = go.GetComponent<ShipDesignItem>();
                if (item != null) {
                    item.SetData(canvas, false, ShipPlanDC.CurShipDesignID);
                    EventDelegate.Add(item.BtnSelect.onClick, BtnSelectChange);
                }
            }
        }
    }
    
    public void LoadCanvasBox()
    {
        CanvasUnitBG unitBg = TouchMoveManager.GetCanvasUnitBG(Int2.zero);
        if (unitBg) {
            m_GoCanvasBox = GameObjectLoader.LoadPath("Prefabs/Others/", "CanvasBox", WndManager.GetWndRoot().transform);
            Vector3 worldPos = unitBg.transform.position;
            NGUIUtil.Set3DUIPos(m_GoCanvasBox, worldPos);
        }
    }
    
    public  void BtnReturn_OnClickEventHandler(UIButton sender)
    {
        Life.Environment = LifeEnvironment.View ;
        PutCanvasM.PutDownNewBuild();
        ShipPlan P = ShipPlanDC.GetCurShipPlan();
        P.Mode = XYmode.Edit2Save;
        if (P.IsShipEditChange() == false) { //如果未修改，则直接返回
            P.ReCalcShipBuildInfoXY(XYmode.Edit2Save);
            DoReturn();
            return;
        }
        if (!m_IsSaving) {
            WndManager.GetDialog<IsSaveFangAnWnd>();
        }
    }
    
    public void DoReturn()
    {
        RoomMap.ClearCanvansArea();
        TouchMoveManager.ClearShipBuild();
        ShipPlan P = ShipPlanDC.GetCurShipPlan();
        P.ResetPutID();
        U3DUtil.DestroyAllChild(TreasureScene.GetMapStart().gameObject);
        WndManager.DestroyAllDialogs();
        BoatObj.SendGetPlanData();
        
        TreasureScene.BuildUIByTreasureState();
        TreasureScene.OutTreasureScene();
    }
    
    public void DoReturnMainTown()
    {
        U3DUtil.DestroyAllChild(BattleEnvironmentM.GetLifeMBornNode(true).gameObject);
        WndManager.DestroyAllDialogs();
        BoatObj.SendGetPlanData();
        
        
        MainCameraM.s_Instance.EnableOthOn(false);
        MainCameraM.s_Instance.ResetCameraDataByHaven();
        MainCameraM.s_Instance.SetCameraLimitParam(MainCameraM.s_reaLimitPyramidHavenView);
        MainTownScene.RevertFrCamPosTemp();
    }
    
    void CANVANS_BUILDING_INFO_RESP(int nErrorCode)
    {
        if (nErrorCode == 0) {
            //加载画布和建筑
            RoomMap.CreateCanvansArea();
            BattleEnvironmentM.BuildScene();
        }
    }
    
    /// <summary>
    /// 清空方案下的现有船只
    /// </summary>
    void ClearFangAn()
    {
        U3DUtil.DestroyAllChild(BattleEnvironmentM.GetLifeMBornNode(true).gameObject);
    }
    public void BtnDesign_OnClickEventHandler(UIButton sender)
    {
        ShipDesignWnd wnd = WndManager.GetDialog<ShipDesignWnd>();
        if (wnd != null) {
            wnd.SetData(WndType.ShipEdit);
        }
    }
    /// <summary>
    /// 下一步 生成船只
    /// </summary>
    /// <param name="sender"></param>
    public void BtnNext_OnClickEventHandler(UIButton sender)
    {
        if (!PutCanvasM.CheckSave()) {
            return;
        }
        
        if (Input.touchCount > 1 || PutCanvasM.CanOperate == false) { //有其他操作时不得保存
            return;
        }
        // 客户端直接保存。
        ShipPlanDC.SaveCanvansInfo();
        SaveSuccecd();
        
        
    }
    void SaveSuccecd()
    {
        TouchMoveManager.ShowCanvas(false);
        MainCameraM.s_Instance.ResetCameraDataByHaven();
        MainCameraM.s_Instance.EnableOthOn(false);
        MainCameraM.s_Instance.SetCameraLimitParam(MainCameraM.s_reaLimitPyramidHavenView);
        MainCameraM.s_Instance.AutoMoveTo(MainCameraM.s_vHavenViewBoatviewCamPos, 0.3f);
        MainCameraM.s_Instance.EnableDrag(false);
        
        Life.Environment = LifeEnvironment.View ;
        PutCanvasM.PutDownNewBuild();
        TouchMoveManager.DoTransgatePoint();
        m_IsSaving = true;
        
        if (MyHead.BtnReturn) {
            MyHead.BtnReturn.gameObject.SetActive(false);
        }
        if (MyHead.BtnNext) {
            MyHead.BtnNext.gameObject.SetActive(false);
        }
        CangKuWnd wnd = WndManager.FindDialog<CangKuWnd>();
        if (wnd) {
            wnd.gameObject.SetActive(false);
        }
        
        PutCanvasM.CanOperate = false;
        PutCanvasM.ShowRoomGridUI(false);
        TouchMoveManager.SetCurTouchMove(null);
        MainCameraM.s_Instance.EnableDrag(false);
        ShipPlan P = ShipPlanDC.GetCurShipPlan();
        P.CreateCanvans();
        GenerateShip.GenerateShipsWithAni();
    }
    
    void OnDestroy()
    {
        if (m_GoCanvasBox) {
            Destroy(m_GoCanvasBox);
        }
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_510, CANVANS_BUILDING_INFO_RESP);
    }
    
    public override bool IsFullWnd()
    {
        return true;
    }
    
}
