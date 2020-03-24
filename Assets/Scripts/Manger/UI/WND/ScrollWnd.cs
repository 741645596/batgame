using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 卷轴滚动/Toggle动画模板窗口
/// <Author>QFord</Author>
/// </summary>
public class ScrollWnd : WndBase {

    public ScrollWnd_h MyHead
    {
        get { return base.BaseHead() as ScrollWnd_h; }
    }

    void Awake()
    {
        //自动关联Toogle
        MyHead.ToggleList = U3DUtil.GetComponentsInChildren<Transform>(MyHead.ToggleParent.gameObject, false);
    }

	public override void WndStart()
	{
		base.WndStart();
        MyHead.BtnClose.OnClickEventHandler += BtnClose_OnClickEventHandler;
        
        NGUIUtil.UpdatePanelValue(MyHead.PanelMask, 1024);
        ShowTogglesAni(0.5f, 0.1f);
	}
    /// <summary>
    /// 显示toggle动画
    /// </summary>
    /// <param name="delay">开始显示第一个toggle的延时</param>
    /// <param name="interval">显示下一个toggle的间隔</param>
    void ShowTogglesAni(float delay, float interval)
    {
        GameObjectActionExcute gae = GameObjectActionExcute.CreateExcute(gameObject);
        float waitInterval = delay;
        for (int i = 0; i < MyHead.ToggleList.Length; i++)
        {
            GameObjectActionWait wait = new GameObjectActionWait(waitInterval,WaitFinish);
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
        if (go==null)
        {
		    return;
        }
       GameObjectActionExcute gae = go.GetComponent<GameObjectActionExcute>();
       if (gae)
       {
           GameObjectActionWait wait = gae.GetCurrentAction() as GameObjectActionWait;
           if (wait != null)
           {
               int index = (int)wait.Data1;
               NGUIUtil.SetActive(MyHead.ToggleList[index].gameObject, true);
           }
       }
    }
    /// <summary>
    /// 隐藏左右两个动画轴
    /// </summary>
    public void HideAniScroll()
    {
        NGUIUtil.SetActive(MyHead.ScrollLeft, false);
        NGUIUtil.SetActive(MyHead.ScrollRight, false);
    }

    /// <summary>
    /// 关闭当前窗口
    /// </summary>
    void BtnClose_OnClickEventHandler(UIButton sender)
    {
        //WndManager.DestoryDialog<ScrollWnd>();
    }
	
}
