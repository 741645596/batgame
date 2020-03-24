/// <summary>
/// 屏幕管理类
/// </summary>
/// <Author>QFord</Author>
/// <Data>2014-11-17   9:36</Data>
/// <Path>E:\Projs\SVN_Root\trunk\SeizeTheShip\Assets\Scripts\Client\Action</Path>

using UnityEngine;
using System.Collections;

public delegate void OnScreenSizeChange(Vector2 newScreenSize);

public class ScreenManager : MonoBehaviour
{
    /// <summary>
    /// 当分辨率改变时触发OnScreenSizeChange委托
    /// </summary>
    public static OnScreenSizeChange onScreenSizeChanged;
    public static bool EnableScreenSizeChangeEvent = false;
    Vector2 lastScreenSize;
    
    void Awake()
    {
        lastScreenSize = new Vector2(Screen.width, Screen.height);
    }
    
    void Update()
    {
        if (EnableScreenSizeChangeEvent) {
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            if (this.lastScreenSize != screenSize) {
                this.lastScreenSize = screenSize;
                if (onScreenSizeChanged != null) {
                    onScreenSizeChanged(screenSize);
                }
            }
        }
    }
}
