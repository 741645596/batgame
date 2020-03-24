using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 
/// <From> </From>
/// <Author>QFord</Author>
/// </summary>
public class EquipComposeWnd_h : WndBase_h{

    public GameObject WndTweenParent;
    /// <summary>
    /// 合成装备中的线
    /// </summary>
    public GameObject[] SprLines;

    public UIButton BtnWndBg;
    public UIButton BtnCompose;
    public UIButton BtnRootEquip;

    public UILabel LblCoin;
    public UILabel LblName;

    public GameObject ComposeEquipsParent;
    public GameObject ComposeMaterialsParent;
    /// <summary>
    /// 父节点 显示用来合成装备的子项目
    /// </summary>
    public Transform EquipMidTable;
    /// <summary>
    /// 最终要合成的装备
    /// </summary>
    public ECEquipItem m_ComposeEquip;
    /// <summary>
    /// 显示当前被合成的装备
    /// </summary>
    public ECEquipItem m_ComposeMid;
}
