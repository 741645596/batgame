using UnityEngine;
/// <summary>
/// 战役对象
/// </summary>
/// <author>zhulin</author>
public class StageObj : BlockObj
{

    public GameObject Guide;
    
    public override void InitObj()
    {
        StageDC.SendStageScheduleRequest();
    }
    /// <summary>
    /// 点击事件
    /// </summary>
    public override bool OnClick()
    {
        StageMapWnd wnd = WndManager.FindDialog<StageMapWnd>();
        if (wnd == null) {
            wnd = WndManager.GetDialog<StageMapWnd>();
            if (wnd != null) {
                wnd.GotoChapter(StageDC.GetCompaignStageType(), StageDC.GetStageChapter(StageDC.GetCompaignStageType()), -1);
                MainTownScene.SaveCameraPosToCamPosTemp();
            }
        }
        return true;
    }
    
}
