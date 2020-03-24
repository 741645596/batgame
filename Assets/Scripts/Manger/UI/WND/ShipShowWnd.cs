using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 船只显示窗口
/// </summary>
public class ShipShowWnd : WndBase
{
    public ShipShowWnd_h MyHead {
        get
        {
            return (base.BaseHead() as ShipShowWnd_h);
        }
    }
    
    private bool m_Show = false;
    
    
    
    public override bool IsFullWnd()
    {
        return true;
    }
    
    void OnEnable()
    {
        if (MyHead.BtnEdit == null) {
            return;
        }
        
        NGUITools.SetActiveSelf(MyHead.BtnEdit.gameObject, true);
    }
    
    public override void WndStart()
    {
        base.WndStart();
        
        Invoke("DelayGuid", 1f);
        if (MyHead.BtnReturn) {
            MyHead.BtnReturn.OnClickEventHandler += BtnReturn_OnClickEventHandler;
        }
        if (MyHead.BtnEdit) {
            MyHead.BtnEdit.OnClickEventHandler += BtnEdit_OnClickEventHandler;
        }
        AddSoldier();
    }
    void DelayGuid()
    {
        m_Show = true;
        //		AddSoldier();
    }
    public void AddSoldier()
    {
        U3DUtil.DestroyAllChild(MyHead.Parent.gameObject, false);
        List<SoldierInfo> lSoldierInfo = new List<SoldierInfo>();
        List<BuildInfo> lBuildInfo = new List<BuildInfo>();
        List<BuildInfo> lStair = new List<BuildInfo>();
        ShipPlan Plan = ShipPlanDC.GetCurShipPlan();
        Plan.GetShipLifeObj(ref lSoldierInfo, ref lBuildInfo, ref lStair);
        foreach (SoldierInfo item in lSoldierInfo) {
            CreateItem(item);
        }
        NGUIUtil.RepositionTablePivot(MyHead.Parent.gameObject);
        SetCombatPower();
    }
    
    private void CreateItem(SoldierInfo s)
    {
        GameObject go = NDLoad.LoadWndItem("CanvasItem", MyHead.Parent);
        CanvasItem c = go.GetComponent<CanvasItem>();
        if (c != null) {
            c.SetCanvasItem(s, 1);
        }
    }
    
    private void SetCombatPower()
    {
        ShipPlan Plan = ShipPlanDC.GetCurShipPlan();
        int combatPower = Plan.CombatPower;
        MyHead.LblCombatPower.text = combatPower.ToString();
    }
    
    public void BtnEdit_OnClickEventHandler(UIButton sender)
    {
        HideUI();
        Life.Environment = LifeEnvironment.Edit ;
        GenerateShip.BreakUpFallBack(gameObject, "StartBreakUp");
    }
    
    void StartBreakUp()
    {
        SoundPlay.Play("ship_break", false, false);
        GenerateShip.BreakUp(gameObject, "GoToEdit");
    }
    
    void GoToEdit()
    {
        MainCameraM.s_Instance.SetCameraLimitParam(MainCameraM.s_reaLimitPyramidEditView);
        MainCameraM.s_Instance.AutoMoveTo(MainCameraM.s_vEditViewBoatviewCamPos, 0.3f);
        //U3DUtil.DestroyAllChild(BattleEnvironmentM.GetLifeMBornNode(true).gameObject);
        GenerateShip.ResetBuildingToCanvas(0.35f, gameObject, "DoFinish");
    }
    
    void DoFinish()
    {
        //U3DUtil.DestroyAllChild(BattleEnvironmentM.GetLifeMBornNode(true).gameObject,false);
        RoomMap.CreateCanvansArea();
        ShipPlan Plan = ShipPlanDC.GetCurShipPlan();
        Plan.ReCalcShipBuildInfoXY(XYmode.Save2Edit);
        BattleEnvironmentM.SetBattleEnvironmentMode(BattleEnvironmentMode.Edit);
        BattleEnvironmentM.BuildScene();
        WndManager.DestoryDialog<ShipShowWnd>();
        WndManager.DestoryDialog<FangAnMenuWnd>();
        GenerateShip.CreateMiddleBeam();
        WndManager.GetDialog<ShipCanvasWnd>().ShowDialog();
        MainCameraM.s_Instance.EnableOthOn(true);
    }
    
    void BtnReturn_OnClickEventHandler(UIButton sender)
    {
        U3DUtil.DestroyAllChild(BattleEnvironmentM.GetLifeMBornNode(true).gameObject);
        WndManager.DestroyAllDialogs();
        BoatObj.SendGetPlanData();
        
        TreasureScene.OutTreasureScene();
    }
    
    void HideUI()
    {
        U3DUtil.SetChildrenActive(gameObject, false);
    }
    
}
