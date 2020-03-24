using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 战斗进程枚举
/// </summary>
public enum CSState {
    /// <summary>
    /// 进入战场
    /// </summary>
    Ready            = 0,
    /// <summary>
    /// 战斗开始
    /// </summary>
    Start            = 1,
    /// <summary>
    /// 战斗中
    /// </summary>
    Combat           = 2,
    /// <summary>
    /// 战斗结束
    /// </summary>
    End              = 3,
    /// <summary>
    /// 战斗暂停
    /// </summary>
    Pause			 = 4,
}

/// <summary>
/// 战斗进度管理
/// </summary>
public class CombatScheduler
{
    private static bool m_bNoTimeCount = false;
    public static bool NoTimeCount {
        get{return m_bNoTimeCount;}
        set{m_bNoTimeCount = value;}
    }
    /// <summary>
    /// 战斗进度
    /// </summary>
    private static CSState m_State = CSState.Ready;
    public  static CSState State {
        get{return m_State;}
        set{m_State = value;}
    }
    /// <summary>
    /// 战斗超时
    /// </summary>
    private static bool m_CombatTimeOut = false;
    public  static bool CombatTimeOut {
        get{return m_CombatTimeOut;}
        set
        {
            m_CombatTimeOut = value;
            if (value == true && State == CSState.Combat)
            {
                SetCSState(CSState.End) ;
            }
        }
    }
    /// <summary>
    /// 攻击方全死亡
    /// </summary>
    private static bool m_AttackAllDead = false;
    public  static bool AttackAllDead {
        get{return m_AttackAllDead;}
        set
        {
            m_AttackAllDead = value;
            if (value == true && State == CSState.Combat)
            {
                SetCSState(CSState.End) ;
            }
        }
    }
    /// <summary>
    /// 金库爆掉
    /// </summary>
    private static bool m_GoldDestroy = false;
    public  static bool GoldDestroy {
        get{return m_GoldDestroy;}
        set
        {
            m_GoldDestroy = value;
            if (value == true && State == CSState.Combat)
            {
                SetCSState(CSState.End) ;
            }
        }
    }
    
    /// <summary>
    /// 游戏战斗计时
    /// </summary>
    private static float m_CombatTime = 0.0f;
    public static int CombatTime {
        get{return (int)m_CombatTime;}
    }
    private const  int CombatMaxTime = 180;// 每场战斗的限定时间
    /// <summary>
    /// 游戏暂停
    /// </summary>
    private static bool m_pause = false;
    
    
    public static void  SetCSState(CSState state)
    {
        m_State = state;
        if (m_State == CSState.Ready) {
            CombatReady();
            SetCSState(CSState.Start);
        } else if (m_State == CSState.Start) {
            m_CombatTime = CombatMaxTime * 1.0f;
        } else if (m_State == CSState.Combat) {
            //m_pause = false;
        } else if (m_State == CSState.End) {
            m_CombatTime = 0 ;
            (SceneM.GetCurIScene() as CombatScene).DoCombatFinish();
        }
        
    }
    /// <summary>
    /// 游戏准备阶段
    /// </summary>
    private static void CombatReady()
    {
        NdUtil.ResetSceneID();
        CmCarbon.ResetDead();
        CmCarbon.ResetFireState();
        PlayerSoldierFire.soldierDataID = -1;
        PlayerSoldierFire.isTrace = false;
        m_CombatTime = CombatMaxTime * 1.0f;
        m_CombatTimeOut = false;
        m_AttackAllDead = false;
        m_GoldDestroy = false;
    }
    /// <summary>
    /// 游戏进程控制。
    /// </summary>
    public static void Run(float deltaTime)
    {
        if (State != CSState.Combat) {
            return ;
        }
        if (m_pause == true) {
            return ;
        }
        if (!NoTimeCount) {
            m_CombatTime -=  deltaTime;
        }
        
        if (m_CombatTime <= 0) {
            m_CombatTime = 0;
            CombatTimeOut = true;
        }
    }
    /// <summary>
    /// 恢复游戏
    /// </summary>
    public  static void ResumeCombat()
    {
        if (State != CSState.Pause) {
            return ;
        }
        Time.timeScale = 1.0f;
        State = CSState.Combat;
    }
    /// <summary>
    /// 暂停游戏
    /// </summary>
    public  static void PauseCombat()
    {
        if (State != CSState.Combat) {
            return ;
        }
        State = CSState.Pause;
        Time.timeScale = 0.0f;
        //m_pause = true;
    }
    /// <summary>
    /// 判断是否处于战斗中
    /// </summary>
    public static bool CheckCombatIng()
    {
        if (CombatScheduler.State == CSState.Combat) {
            return true;
        } else {
            return false;
        }
    }
}


