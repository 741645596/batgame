using UnityEngine;


/// <summary>
/// 主菜单（侧边按钮栏）
/// </summary>
public class MainMenuWnd : WndBase
{

    private int m_wndType = 0;
    public MainMenuWnd_h MyHead {
        get
        {
            return (base.BaseHead() as MainMenuWnd_h);
        }
    }
    
    private float m_fMinHeight;
    private float m_fMaxHeight ;
    
    private bool m_bOpenMainMenu = true;
    
    private bool m_bAniFinish = true;
    /// <summary>
    /// 是否该窗口显示时就被外部关闭
    /// </summary>
    private bool m_bBeClosed = false;
    private GameObject m_preWnd = null;
    public override void WndStart()
    {
        base.WndStart();
        MainMenuInit();
        RegisterHooks();
    }
    
    public void OpenMenu()
    {
        if (!m_bOpenMainMenu) {
            ClickMenuBtn();
        }
    }
    
    public void CloseMenu()
    {
        if (m_bOpenMainMenu) {
            m_bBeClosed = true;
            ClickMenuBtn();
        }
    }
    /// <summary>
    /// 关闭下拉栏的5个模块
    /// </summary>
    public void CloseOtherWnds()
    {
        ReSetMainDataType(0);
        WndManager.DestoryDialog<PdbbbWnd>();
        WndManager.DestoryDialog<TrapViewListWnd>();
        WndManager.DestoryDialog<BeiBaoWnd>();
        WndManager.DestoryDialog<BlackSciencdWnd>();
    }
    
    /// <summary>
    /// 设置窗口类型
    /// </summary>
    /// <param name="wndType">0：普通  1：窗口置前</param>
    public void SetWndType(int wndType)
    {
        m_wndType = wndType;
    }
    
    /// <summary>
    /// 主菜单初始设定
    /// </summary>
    void MainMenuInit()
    {
        m_bOpenMainMenu = true;//默认是开启主菜单
        MyHead.WidgetTweenSprite.ResetAndUpdateAnchors(); //适配设定
        Vector3 vPos = MyHead.WidgetTweenSprite.transform.localPosition;
        m_fMaxHeight = vPos.y;
        m_fMinHeight = m_fMaxHeight + 625f;
        MyHead.WidgetTweenSprite.transform.localPosition = new Vector3(vPos.x, m_fMinHeight, vPos.z);
        CheckMenuPoint();
        
        MyHead.BtnAdd.OnClickEventHandler += BtnMainMenu_OnClickEventHandler;
        MyHead.BtnMinus.OnClickEventHandler += BtnMainMenu_OnClickEventHandler;
        
        if (MyHead.BtnYingXiong) {
            MyHead.BtnYingXiong.OnClickEventHandler += BtnYingXiong_OnClickEventHandler;
        }
        if (MyHead.BtnBeiBao) {
            MyHead.BtnBeiBao.OnClickEventHandler += BtnBeiBao_OnClickEventHandler;
        }
        if (MyHead.BtnTrapWnd) {
            MyHead.BtnTrapWnd.OnClickEventHandler += BtnTrapWnd_OnClickEventHandler;
        }
        if (MyHead.BtnRw) {
            MyHead.BtnRw.OnClickEventHandler += BtnRw_OnClickEventHandler;
        }
        if (MyHead.BtnHkj) {
            MyHead.BtnHkj.OnClickEventHandler += BtnHkj_OnClickEventHandler;
        }
    }
    private void ReSetMainDataType(int type)
    {
    }
    /// <summary>
    ///  背包
    /// </summary>
    void BtnBeiBao_OnClickEventHandler(UIButton sender)
    {
        CloseOtherWnds();
        CloseShop();
        
        BeiBaoWnd wnd = WndManager.GetDialog<BeiBaoWnd>();
    }
    /// <summary>
    ///  陷阱
    /// </summary>
    public void BtnTrapWnd_OnClickEventHandler(UIButton sender)
    {
        CloseOtherWnds();
        CloseShop();
        WndManager.GetDialog<TrapViewListWnd>();
    }
    /// <summary>
    /// 点击 英雄 按钮(炮弹兵背包)
    /// </summary>
    void BtnYingXiong_OnClickEventHandler(UIButton sender)
    {
        CloseOtherWnds();
        CloseShop();
        PdbbbWnd wnd = WndManager.GetDialog<PdbbbWnd>();
        
    }
    public void BtnRw_OnClickEventHandler(UIButton sender)
    {
        CloseOtherWnds();
        //		ShipDesignWnd wnd = WndManager.GetDialog<ShipDesignWnd>();
        //		if(wnd != null)
        //		{
        //			wnd.SetData();
        //		}
        
    }
    void BtnHkj_OnClickEventHandler(UIButton sender)
    {
        CloseOtherWnds();
        BlackSciencdWnd Bwnd = WndManager.GetDialog<BlackSciencdWnd>();
    }
    /// <summary>
    /// 关闭商店
    /// </summary>
    void CloseShop()
    {
        m_wndType = 0;
        NGUIUtil.SetActive(MyHead.WndBg, false);
    }
    /// <summary>
    /// 点击 主菜单 按钮
    /// </summary>
    void BtnMainMenu_OnClickEventHandler(UIButton sender)
    {
        ClickMenuBtn();
    }
    /// <summary>
    /// 点击主菜单处理
    /// </summary>
    public void ClickMenuBtn()
    {
        CheckMenuPoint();
        GameObject go = MyHead.WidgetTweenSprite.gameObject;
        if (m_bOpenMainMenu == false) { //打开主菜单
            m_bOpenMainMenu = true;
            m_bAniFinish = false;
            
            NGUIUtil.TweenGameObjectPosY(go, m_fMinHeight, m_fMaxHeight, 0.2f, 0f, gameObject, "AnimationFinish");
            WndEffects.PlayWndAnimation(gameObject, "MainMenuAni01");
        } else {
            m_bOpenMainMenu = false;
            m_bAniFinish = false;
            NGUIUtil.TweenGameObjectPosY(go, m_fMaxHeight, m_fMinHeight, 0.2f, 0f, gameObject, "AnimationFinish");
            WndEffects.PlayWndAnimation(gameObject, "MainMenuAni02");
        }
        
        if (m_wndType == 1) {
            NGUIUtil.SetActive(MyHead.WndBg, m_bOpenMainMenu);
        }
    }
    
    
    /// <summary>
    /// 菜单红点设定
    /// </summary>
    public void CheckMenuPoint()
    {
        bool YxRed = SoldierDC.CheckHaveCanEquip();
        MyHead.SprHeroPoint.gameObject.SetActive(YxRed);
        
        bool TrapRed = BuildDC.CheckCanUp();
        MyHead.SprTrapPoint.gameObject.SetActive(TrapRed);
        
        bool BlackTel = BlackScienceDC.CheckCanUp();
        MyHead.SprHeiKeJiPoint.gameObject.SetActive(BlackTel);
        
        
        bool all = YxRed || TrapRed || BlackTel ;
        MyHead.SprAllPoint.gameObject.SetActive(all);
    }
    
    private void Rec_TaskInfoResponse(int nErrorCode)
    {
        if (nErrorCode == 0) {
            CheckMenuPoint();
        }
    }
    private void Rec_BlackScienceRespone(int nErrorCode)
    {
        if (nErrorCode == 0) {
            CheckMenuPoint();
        }
    }
    
    private void Rec_SoldierResponse(int nErrorCode)
    {
        if (nErrorCode == 0) {
            CheckMenuPoint();
        }
    }
    public void OnDestroy()
    {
        AntiRegisterHooks();
    }
    
    /// <summary>
    /// 注册事件
    /// </summary>
    public   void RegisterHooks()
    {
        //黑科技升星升级
        DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_1102, Rec_BlackScienceRespone);
        DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_1104, Rec_BlackScienceRespone);
        DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_1106, Rec_BlackScienceRespone);
        
        //陷阱升星升阶升级
        DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_902, Rec_BlackScienceRespone);
        DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_904, Rec_BlackScienceRespone);
        DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_906, Rec_BlackScienceRespone);
        
        //炮弹兵升星升级
        DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_204, Rec_SoldierResponse);
        DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_210, Rec_SoldierResponse);
        DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_208, Rec_SoldierResponse);
        
        //任务
        DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_1602, Rec_TaskInfoResponse);
        
    }
    
    /// <summary>
    /// 反注册事件
    /// </summary>
    public   void AntiRegisterHooks()
    {
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_1102, Rec_BlackScienceRespone);
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_1104, Rec_BlackScienceRespone);
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_1106, Rec_BlackScienceRespone);
        
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_902, Rec_BlackScienceRespone);
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_904, Rec_BlackScienceRespone);
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_906, Rec_BlackScienceRespone);
        
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_204, Rec_SoldierResponse);
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_210, Rec_SoldierResponse);
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_208, Rec_SoldierResponse);
        
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_1602, Rec_TaskInfoResponse);
        
    }
}
