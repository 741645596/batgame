using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrapSplitWnd : WndTopBase
{
    public TrapSplitWnd_h MyHead {
        get
        {
            return (base.BaseHead() as TrapSplitWnd_h);
        }
    }
    
    private BuildInfo m_build;
    private int m_FragmentNum ;
    private int m_wood = 0;
    private int m_coin = 0;
    
    
    public override void WndStart()
    {
        base.WndStart();
        RegisterHooks();
        
        MyHead.BtnCancel.OnPressDownEventHandler += BtnCancel_OnClickEventHandler;
        MyHead.BtnSplit.OnPressUpEventHandler += BtnSplit_OnClickEventHandler;
    }
    
    void OnDestroy()
    {
        AntiRegisterHooks();
    }
    /// <summary>
    /// 注册事件
    /// </summary>
    public   void RegisterHooks()
    {
        DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_910, BuildAnnalyze);
        
    }
    
    /// <summary>
    /// 反注册事件
    /// </summary>
    public   void AntiRegisterHooks()
    {
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_606, BuildAnnalyze);
    }
    
    public void SetData(BuildInfo Info)
    {
        m_build = Info;
        m_FragmentNum = ConfigM.GetBuildAnanlyzeFragment(m_build.StarLevel);
        buildingM.GetBuildAnanlyze(m_build.BuildType, m_build.Level, ref m_wood, ref m_coin);
        SetUI();
    }
    
    private void SetUI()
    {
        GameObject go = NDLoad.LoadWndItem("TrapViewItem", MyHead.trap);
        if (go != null) {
            TrapViewItem item = go.GetComponent<TrapViewItem>();
            if (item != null) {
                item.SetBuildInfo(m_build, null, TrapState.Exit, 2);
            }
        }
        sdata.s_itemtypeInfo info  = ItemM.GetItemInfo(m_build.fragmentTypeID);
        if (info != null) {
            NGUIUtil.Set2DSprite(MyHead.Spr2DItem, "Textures/item/" + info.icon);
        } else {
            NGUIUtil.DebugLog("m_build.fragmentTypeID =" + m_build.fragmentTypeID + "not found!");
        }
        NGUIUtil.SetLableText(MyHead.LblItemNum, m_FragmentNum);
        NGUIUtil.SetLableText(MyHead.LblWoodNum, m_wood);
        NGUIUtil.SetLableText(MyHead.LblGoldNum, m_coin);
    }
    
    /// <summary>
    /// 更新玩家信息
    /// </summary>
    private void BuildAnnalyze(int nErrorCode)
    {
        if (nErrorCode == 0) {
            //弹出拆解结果
            WndManager.DestoryDialog<TrapShowWnd>();
            WndManager.DestoryDialog<TrapSplitWnd>();
        }
    }
    
    private void BtnCancel_OnClickEventHandler(UIButton sender)
    {
        WndManager.DestoryDialog<TrapSplitWnd>();
    }
    
    private void BtnSplit_OnClickEventHandler(UIButton sender)
    {
        BuildDC.Send_BuildAnalyzeRequest(m_build.ID);
    }
}