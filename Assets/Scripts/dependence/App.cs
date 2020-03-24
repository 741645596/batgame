// 一些编译开关配置
using UnityEngine;
using Logic;
using System.Collections;


/// <summary>
/// 存放客户端应用程序的一些相关信息
/// </summary>
public class App
{
    /// <summary>
    /// 日志管理
    /// </summary>
    public static Log log = new LogImpl();
    /// <summary>
    /// 私有日志控制输入
    /// </summary>
    public static PRINT m_print = PRINT.TXM;
    
    /// <summary>
    /// 客户端版本
    /// </summary>
    public static string ver = string.Empty;
    
    public static bool debug {
        get
        {
#if UNITY_EDITOR
            // 编辑版本中使用debug
            return true;
#else
            return false;
#endif
        }
    }
    
    // 是否已经初始化完成
    private static bool bInit = false;
    
    // 初始化处理
    public static void Init()
    {
        if (bInit)
            // 已经初始化过了
        {
            return;
        }
        log.To("App.cs", "开始初始化游戏。");
        
        Localization.language = "SimplifiedChinese";
        
        // 不锁屏
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        
        
        // 日志初始化
        (log as LogImpl).Init();
        
        // 登记所有的场景处理器
        RegisterScene();
        // TODO
        // 逻辑待补充
        
        log.To("App.cs", "游戏初始化完毕。");
        // 标记初始化完成
        bInit = true;
    }
    
    /// <summary>
    /// 判断是否已经初始化完成
    /// </summary>
    public static bool IsInit {
        get { return bInit; }
    }
    /// <summary>
    /// 清空数据
    /// </summary>
    public static void Empty()
    {
    }
    
    
    // 登记所有的场景处理器
    private static void RegisterScene()
    {
        SceneM.RegisterScene(CombatScene.GetSceneName(), new CombatScene());
        SceneM.RegisterScene(MainTownScene.GetSceneName(), new MainTownScene());
        SceneM.RegisterScene(ViewStageScene.GetSceneName(), new ViewStageScene());
        SceneM.RegisterScene(TreasureScene.GetSceneName(), new TreasureScene());
        
        SceneM.RegisterLoadingAnim(CombatScene.GetSceneName(), new LoadingMainTown());
        SceneM.RegisterLoadingAnim(MainTownScene.GetSceneName(), new LoadingMainTown());
        SceneM.RegisterLoadingAnim(ViewStageScene.GetSceneName(), new LoadingMainTown());
        SceneM.RegisterLoadingAnim(TreasureScene.GetSceneName(), new LoadingMainTown());
    }
}
