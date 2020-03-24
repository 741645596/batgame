using UnityEngine;
using System;
using System.Collections;

/// <summary>
///  卷轴滚动表现模板窗口
/// <Author>QFord</Author>
/// </summary>
public class ScrollWnd_h : WndBase_h {
    /// <summary>
    /// 遮罩 Panel
    /// </summary>
    public UIPanel PanelMask;
    public Transform ToggleParent;
    public Transform[] ToggleList;

    public GameObject ScrollLeft;
    public GameObject ScrollRight;

    public UIButton BtnClose;
	
}
