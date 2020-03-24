using UnityEngine;
using System.Collections;
/// <summary>
/// 攻击目标倒计时
/// </summary>
public class CombatCountDownWnd : WndBase
{

    public CombatCountDownWnd_h MyHead {
        get
        {
            return (base.BaseHead() as CombatCountDownWnd_h);
        }
    }
    
    private int m_iLimitCounter;
    public override void WndStart()
    {
        base.WndStart();
        int countDown =  ConfigM.GetAttackCountDown(BattleEnvironmentM.GetBattleEnvironmentMode());
        SetAttackTime(countDown);
        AddEvents();
        StartAttackCountDown();
    }
    
    void GotoSelectSoldiers()
    {
        WndManager.DestoryDialog<CombatInfoWnd>();
        WndManager.DestoryDialog<CombatCountDownWnd>();
        WndManager.GetDialog<SelectSoldierwnd>();
    }
    
    void StartAttackCountDown()
    {
        if (MyHead.HourGlassLeftTime == null) {
            NGUIUtil.DebugLog("CombatCountDownWnd.cs->HourGlassLeftTime 未设置");
            return;
        }
        
        int countDown = ConfigM.GetAttackCountDown(BattleEnvironmentM.GetBattleEnvironmentMode());
        if (countDown <= 0) {
            NGUIUtil.DebugLog("CombatCountDownWnd.cs->s_config type=11 数据配置有问题");
            return;
        }
        m_iLimitCounter = countDown;
        InvokeRepeating("SetLeftTime", 1f, 1f);
    }
    
    void SetLeftTime()
    {
        if (m_iLimitCounter < 0) {
            MyHead.BtnAttack.enabled = false;
            WndManager.DestoryDialog<CombatCountDownWnd>();
            GotoSelectSoldiers();
        }
        
        SetAttackTime(m_iLimitCounter);
        m_iLimitCounter--;
        
    }
    void SetAttackTime(int leftTime)
    {
        MyHead.HourGlassLeftTime.text = NdUtil.TimeFormat(leftTime);
    }
    
    void AddEvents()
    {
        if (MyHead.BtnAttack) {
            MyHead.BtnAttack.OnClickEventHandler += BtnAttack_OnClickEventHandler;
        }
        if (MyHead.BtnAbandon) {
            MyHead.BtnAbandon.OnClickEventHandler += BtnAbandon_OnClickEventHandler;
        }
    }
    
    void BtnAbandon_OnClickEventHandler(UIButton sender)
    {
        MainTownInit.s_currentState = MainTownState.StageMap;
        SceneM.Load(MainTownScene.GetSceneName(), false, null, false);
    }
    
    void BtnAttack_OnClickEventHandler(UIButton sender)
    {
        //NGUIUtil.DebugLog("BtnAttack_OnClickEventHandler");
        GotoSelectSoldiers();
    }
    
}
