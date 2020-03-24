using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// 船只编辑
/// </summary>
/// <author>zhulin</author>
public class BoatObj : BlockObj
{
    public static void SendGetPlanData()
    {
        ShipPlanDC.SetCurShipPlan(PlanType.Default);
        
        List<PlanType> l = new List<PlanType>();
        l.Add(PlanType.Default);
        ShipPlanDC.SendCanvasBuildRequest(l);
        
    }
    
    public override void InitObj()
    {
        SendGetPlanData();
    }
    
    
    /// <summary>
    /// 注册事件
    /// </summary>
    public override  void RegisterHooks()
    {
    }
    
    /// <summary>
    /// 反注册事件
    /// </summary>
    public override  void AntiRegisterHooks()
    {
    }
    
    
    
    
    /// <summary>
    /// 点击事件
    /// </summary>
    public override bool OnClick()
    {
        TreasureScene.SetTreasureState(TreasureState.CanvasEdit);
        SceneM.Load(TreasureScene.GetSceneName(), false, false);
        
        return true;
    }
    
}
