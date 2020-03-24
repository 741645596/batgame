using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 表示金银岛所处状态
/// </summary>
public enum TreasureState {
    None,
    /// <summary>
    /// 船只编辑
    /// </summary>
    CanvasEdit,
}

/// <summary>
/// 游戏入口场景
/// </summary>
/// <author>weism</author>
public class TreasureScene : IScene
{
    /// <summary>
    /// 船体数据
    /// </summary>
    private static MapStart m_MapStart = null;
    public static TreasureState m_SceneState = TreasureState.CanvasEdit;
    public static Vector3 m_MainCameraPos = new Vector3(0f, 9.27f, -220.5f);
    
    public new static string GetSceneName()
    {
        return "Treasure";
    }
    /// <summary>
    /// 资源载入入口
    /// </summary>
    //private AsyncOperation async;
    public override IEnumerator Load()
    {
        async = Application.LoadLevelAsync(TreasureScene.GetSceneName());
        return null;
    }
    
    /// <summary>
    /// 加入船体数据
    /// </summary>
    public static void JoinBoatData(MapStart start)
    {
        m_MapStart = start;
    }
    public static Transform GetMapStart()
    {
        return m_MapStart.transform;
    }
    public static void SetTreasureState(TreasureState state = TreasureState.None)
    {
        m_SceneState = state;
    }
    /// <summary>
    /// 准备载入场景
    /// </summary>
    public override void PrepareLoad()
    {
    }
    /// <summary>
    /// 是否已经载入完成
    /// </summary>
    public override bool IsEnd()
    {
        if (async != null) {
            return async.isDone;
        } else {
            return false;
        }
    }
    
    /// <summary>
    /// 资源卸载
    /// </summary>
    public override void Clear()
    {
        WndManager.DestroyAllDialogs();
    }
    
    
    
    
    public override void BuildUI()
    {
        BuildUIByTreasureState();
    }
    
    public static void BuildUIByTreasureState()
    {
        switch (m_SceneState) {
            case TreasureState.CanvasEdit: {
                WndManager.GetDialog<ShipShowWnd>().ShowDialogWithAction();
                WndManager.GetDialog<FangAnMenuWnd>().ShowDialogWithAction();
                UniversalObj.s_instane.SetBackGroundByState(UniversalObjState.MainTown);
                //			ShipPlan P = ShipPlanDC.GetCurShipPlan();
                //			BattleEnvironmentM.ResetStartPos(new Int2(P.Canvans.width, P.Canvans.height), false);
                break;
            }
        }
        MainCameraM.s_Instance.EnableDrag(false);
    }
    /// <summary>
    /// 退出金银岛场景.
    /// </summary>
    public static void OutTreasureScene()
    {
        if (TreasureScene.m_SceneState == TreasureState.CanvasEdit) {
            MainTownInit.s_currentState = MainTownState.None;
        }
        SceneM.Load(MainTownScene.GetSceneName(), false, false);
        //		UniversalObj.s_instane.SetBackGroundByState(UniversalObjState.None);
        MainCameraM.s_Instance.EnableDrag(true);
        TreasureScene.m_SceneState = TreasureState.None;
    }
    
    public override void BuildWorld()
    {
        ReBuildShip();
    }
    
    public static void ReBuildShip()
    {
        U3DUtil.DestroyAllChild(m_MapStart.gameObject);
        Int2 roomGrid = new Int2();
        switch (m_SceneState) {
            case TreasureState.None:
            case TreasureState.CanvasEdit: {
                ShipPlan plan = ShipPlanDC.GetCurShipPlan();
                if (plan != null) {
                    roomGrid = plan.Canvans.GetMapSize();
                    BattleEnvironmentM.ResetStartPos(plan.Canvans.GetMapSize(), true);
                }
                
                GenerateShip.GenerateShips(m_MapStart);
                
                break;
            }
        }
        Vector3 pos = MainCameraM.GetMainCameraPos(roomGrid);
        MainCameraM.s_Instance.SetCameraLimitParam(MainCameraM.s_reaLimitPyramidBoatView);
        MainCameraM.s_Instance.AutoMoveTo(pos, 0f);
        
    }
    
    
}