using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum UniversalObjState {
    None,
    Treasure,
    MainTown,
    ViewStage,
}
/// <summary>
/// 船只编辑
/// </summary>
/// <author>zhulin</author>
public class UniversalObj : BlockObj
{

    public static UniversalObjState m_currentState = UniversalObjState.None;
    public SceneObj Treasure;
    public SceneObj MainTown;
    public SceneObj ViewStage;
    public static UniversalObj s_instane;
    
    public override void InitObj()
    {
        if (s_instane == null) {
            s_instane = this;
        }
    }
    /// <summary>
    /// 点击事件
    /// </summary>
    public override bool OnClick()
    {
        return true;
    }
    
    public void SetBackGroundByState(UniversalObjState state)
    {
        Treasure.gameObject.SetActive(state == UniversalObjState.Treasure);
        MainTown.gameObject.SetActive(state == UniversalObjState.MainTown);
        ViewStage.gameObject.SetActive(state == UniversalObjState.ViewStage);
    }
    
}
