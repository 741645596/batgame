using UnityEngine;
using System.Collections;

public class ConstantData
{
    public static string g_version = "5.3.0.400";
    /// <summary>
    /// 内部版本号
    /// </summary>
    public static string g_Ndversion = "400";
    
    /// <summary>
    /// 保证弹出的界面在3dModel前面，不会被遮挡
    /// </summary>
    public static int  iDepBefore3DModel = -1000;
    //错误列表
    public  static int ERR_DIVIDE       = 1000000; // 除数为0的错误
    /// <summary>
    /// 炮弹兵最高星级
    /// </summary>
    public const int MaxStarLevel = 5;
    /// <summary>
    /// 主场景没有遮挡时Wnd窗口数
    /// </summary>
    public const int MainTownClearWndCount = 4;
    
    /// <summary>
    /// 窗体弹出或者消失的动画效果时长
    /// </summary>
    public static float fPopUpAniTime = 0.4f;
    
    public static float fPopDownAniTime = 0.3f;
    
    /// <summary>
    /// 物品图标路径
    /// </summary>
    public static string ItemIconPath = "Textures/item/";
    /// <summary>
    /// 成就图标路径
    /// </summary>
    public static string FrutionIconPath = "Textures/frution/";
    /// <summary>
    /// 英雄图标路径
    /// </summary>
    public static string HeroIconPath = "Textures/role/";
    /// <summary>
    /// 资源图标路径
    /// </summary>
    public static string CurrencyIconPath = "Textures/currency/";
    /// <summary>
    ///陷阱图标路径
    /// </summary>
    public static string TrapIconPath = "Textures/room/";
}
