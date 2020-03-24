using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 全局计时器
/// <From> </From>
/// <Author>QFord</Author>
/// </summary>
public class GlobalTimer  : MonoBehaviour
{

    public static bool s_HaveServerTime = false;
    /// <summary>
    /// 服务端 当前时间 计时
    /// </summary>
    private static int ServerTimeSeconds = 0;
    /// <summary>
    /// 客户端游戏运行时间
    /// </summary>
    private static float ClientTimeSceonds = 0;
    /// <summary>
    /// 体力恢复计时器
    /// </summary>
    private static int PhysicsResumeCounter;
    /// <summary>
    /// 恢复体力达到时间点
    /// </summary>
    private static int PhysicsResumeEnd;
    private static int PhysicsResumeAllTotal;
    /// <summary>
    /// 海神杯挑战CD倒计时.
    /// </summary>
    private static int AthleticsChallengeCD;
    private static int AthleticsTotal;
    /// <summary>
    /// 技能点恢复计时器
    /// </summary>
    private static int SkillResumeCounter;
    private static int SkillResumeTotal;
    /// <summary>
    /// 地精商店关闭计时器
    /// </summary>
    private static int GnomeShopCloseCounter;
    private static int GnomeShopCloseTotal;
    /// <summary>
    /// 黑市商店关闭计时器
    /// </summary>
    private static int BlackShopCloseCounter;
    private static int BlackShopCloseTotal;
    
    private static bool bInit = false;
    public static GlobalTimer instance;
    
    void Awake()
    {
        instance = this;
    }
    
    
    public static  void SetServerTime(int seconds)
    {
        if (seconds == -1 || bInit == true) {
            return ;
        }
        
        if (bInit == false) {
            ServerTimeSeconds = seconds ;
            ClientTimeSceonds = Time.realtimeSinceStartup;
            bInit = true;
        }
        UserInfo info = UserDC.GetPlayer();
        PhysicsResumeEnd = info.Physical_time + ConfigM.GetResumePhysicsTime() - seconds + GetNowTimeInt();
        SkillResumeTotal = info.SkillPointTime + ConfigM.GetResumeSkillTime() - seconds + GetNowTimeInt();
        
        instance.InvokeRepeating("RefreshCounter", 0f, 1f);
    }
    
    public static void ResetPhysicsResume()
    {
        int interval = ConfigM.GetResumePhysicsTime();
        SetPhysicsResume(interval);
    }
    public static void ResetSkillResume()
    {
        SkillResumeTotal = ConfigM.GetResumeSkillTime() + GetNowTimeInt();
        SkillResumeCounter = ConfigM.GetResumeSkillTime();
    }
    /// <summary>
    /// 刷新计时器变量
    /// </summary>
    private void RefreshCounter()
    {
        PhysicsResumeCounter = PhysicsResumeEnd - GlobalTimer.GetNowTimeInt();
        if (PhysicsResumeCounter < 0) {
            ResetPhysicsResume();
        }
        
        SkillResumeCounter = SkillResumeTotal - GlobalTimer.GetNowTimeInt();
        if (SkillResumeCounter < 0) {
            SkillResumeTotal = ConfigM.GetResumeSkillTime() + GetNowTimeInt();
            SkillResumeCounter = ConfigM.GetResumeSkillTime();
        }
        
        if (AthleticsTotal > 0) {
            AthleticsChallengeCD = AthleticsTotal - GetNowTimeInt();
            if (AthleticsChallengeCD <= 0) {
                AthleticsTotal = 0;
            }
        }
        
        if (GnomeShopCloseTotal > 0) {
            GnomeShopCloseCounter = GnomeShopCloseTotal - GetNowTimeInt();
            if (GnomeShopCloseCounter <= 0) {
                GnomeShopCloseTotal = 0;
            }
        }
        
        if (BlackShopCloseTotal > 0) {
            BlackShopCloseCounter = BlackShopCloseTotal - GetNowTimeInt();
            if (BlackShopCloseCounter <= 0) {
                BlackShopCloseTotal = 0;
            }
        }
        
    }
    
    public int GetSkillResumeCounter()
    {
        return SkillResumeCounter;
    }
    public int GetPhysicsResumeCounter()
    {
        return PhysicsResumeCounter;
    }
    public int GetPhysicsResumeAllCounter()
    {
        int current = UserDC.GetPhysical();
        int total = UserM.GetMaxPhysical(UserDC.GetLevel());
        int temp = (total - current - 1) * ConfigM.GetResumePhysicsTime();
        return PhysicsResumeCounter + temp;
    }
    
    public int GetAthleticsCDCounter()
    {
        return AthleticsChallengeCD;
    }
    public void SetAthleticsCDTotal(int CDSecond)
    {
        AthleticsChallengeCD =  CDSecond;
        AthleticsTotal = CDSecond + GetNowTimeInt();
    }
    /// <summary>
    /// 获取地精商店的当前倒计时
    /// </summary>
    public static int GetGnomeShopCloseCounter()
    {
        return GnomeShopCloseCounter;
    }
    /// <summary>
    /// 设置地精商店的关闭时间点（Timestamp）
    /// </summary>
    public static void SetGnomeShopCloseTotal(int closeTime)
    {
        GnomeShopCloseCounter = closeTime - GetNowTimeInt();
        GnomeShopCloseTotal = closeTime;
    }
    /// <summary>
    /// 获取黑市商店的当前倒计时
    /// </summary>
    public static int GetBlackShopCloseCounter()
    {
        return BlackShopCloseCounter;
    }
    /// <summary>
    /// 设置黑市商店的关闭时间点（Timestamp）
    /// </summary>
    public static void SetBlackhopCloseTotal(int closeTime)
    {
        //NGUIUtil.DebugLog("BlackhopCloseTime ="+closeTime);
        BlackShopCloseCounter = closeTime - GetNowTimeInt();
        BlackShopCloseTotal = closeTime;
    }
    
    /// <summary>
    /// 获取当前时间（1970 时间戳 秒）
    /// </summary>
    public static int GetNowTimeInt()
    {
        float currentTime = Time.realtimeSinceStartup - ClientTimeSceonds;
        return (int)(ServerTimeSeconds + (int) currentTime); //舍弃客户端0.x秒尾数，避免客户端时间领先服务器端
    }
    /// <summary>
    /// 获取当前时间（1970 时间戳 秒）
    /// </summary>
    public static float GetNowTimeFloat()
    {
        float currentTime = Time.realtimeSinceStartup - ClientTimeSceonds;
        return (ServerTimeSeconds + currentTime);
    }
    
    /// <summary>
    /// 设置体力相关时间
    /// </summary>
    private static void SetPhysicsResume(int time)
    {
        PhysicsResumeCounter = time;
        PhysicsResumeEnd = PhysicsResumeCounter + GetNowTimeInt();
    }
    
}
