using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WndManager : MonoBehaviour
{
    public static WndManager s_instance;
    public GameObject  m_goNGUIRoot;
    public Camera  m_cameraNGUI;
    public GameObject m_goWndRoot;
    
    
    
    /// <summary>
    /// 普通窗体深度计数
    /// </summary>
    private static int s_NormalWndDepthCounter = 0;
    public static int NormalWndDepth {
        get{return s_NormalWndDepthCounter;}
        set{s_NormalWndDepthCounter = value;}
    }
    
    /// <summary>
    /// 置顶窗体深度计数
    /// </summary>
    private static int s_TopWndDepthCounter = 5000;
    public static int TopWndDepth {
        get{return s_TopWndDepthCounter;}
        set{s_TopWndDepthCounter = value;}
    }
    /// <summary>
    /// 拥有全屏窗口的数量
    /// </summary>
    public static int FullWndCount {
        get{
            int total = 0 ;
            foreach (WndBase w in m_lwnd)
            {
                if (w != null && w.IsFullWnd() == true) {
                    total ++;
                }
            }
            return total;
        }
    }
    
    private static List<WndBase>  m_lwnd = new List<WndBase>();
    
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        //设置屏幕自动旋转， 并置支持的方向
        Screen.orientation = ScreenOrientation.AutoRotation;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
    }
    
    
    void Start()
    {
        s_instance = this;
    }
    
    public static GameObject GetNGUIRoot()
    {
        return WndManager.s_instance.m_goNGUIRoot;
    }
    public static Camera GetNGUICamera()
    {
        if (WndManager.s_instance) {
            return WndManager.s_instance.m_cameraNGUI;
        } else {
            return NGUITools.FindCameraForLayer(8);
        }
    }
    public static GameObject GetWndRoot()
    {
        if (WndManager.s_instance) {
            return WndManager.s_instance.m_goWndRoot;
        }
        return null;
    }
    /// <summary>
    /// 获取当前窗口数量
    /// </summary>
    public static int GetCurWndsCount()
    {
        GameObject wndRoot = GetWndRoot();
        if (wndRoot) {
            return wndRoot.transform.childCount;
        }
        return 0;
    }
    
    /// <summary>
    /// 获取窗口，从窗口根节点下一层获取
    /// </summary>
    /// <typeparam name="T">获取窗口，从窗口根节点下一层获取</typeparam>
    /// <returns></returns>
public static T FindDialog<T>()where T:
    WndBase {
        foreach (WndBase w in m_lwnd)
        {
            if (w == null) {
                continue ;
            }
            if (w is T) {
                return (T) w;
            }
        }
        return default(T);;
    }
    
public static int GetDialogDepth<T>() where T:
    WndBase {
        T wndDialog = FindDialog<T>();
        UIPanel panel = wndDialog.GetComponent<UIPanel>();
        if (panel)
        {
            return panel.depth;
        }
        return 0;
    }
    
    /// <summary>
    /// 删除窗口根节点下的窗口，如果不存在该窗口返回false
    /// </summary>
    /// <typeparam name="T">窗口实现类</typeparam>
    /// <returns>删除成功返回true</returns>
public static bool DestoryDialog<T>() where T:
    WndBase {
        T wndDialog = FindDialog<T>();
        if (wndDialog != null)
        {
            wndDialog.DestroyDialog();
            return true;
        }
        return false;
    }
    
public static bool DestoryDialogNow<T>() where T :
    WndBase {
        T wndDialog = FindDialog<T>();
        if (wndDialog != null)
        {
            wndDialog.DestroyDialogNow();
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 获取窗口，从窗口根节点下一层获取，获取失败则创建窗口
    /// </summary>
    /// <typeparam name="T">窗口实现类</typeparam>
    /// <param name="strDialogIDD">strDialogIDD 窗口ID</param>
    /// <param name="parent">parent 父节点</param>
    /// <returns></returns>
public static T GetDialog<T>()where T:
    WndBase {
        T  Wnd = FindDialog<T>();
        if (Wnd != null)
        {
            return Wnd;
        } else
        {
            Wnd =  WndManager.CreateDialog<T>(WndManager.GetWndRoot().transform);
            return Wnd;
        }
    }
    
    /// <summary>
    /// 创建子窗口（非WndRoot(这下面挂载UI界面)下）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="strDialogIDD">窗口ID</param>
    /// <param name="parent">父节点</param>
    /// <returns></returns>
public static T CreateDialog<T>(Transform parent)where T:
    WndBase {
        string strDialogIDD = typeof(T).ToString();
        
        if (strDialogIDD == WndBase.DialogIDD())
        {
            return default(T);
        }
        
        GameObject go = NDLoad.LoadWnd(strDialogIDD, parent) as GameObject;
        if (go == null)
        {
            Debug.Log("WndManager CreateDialog " + strDialogIDD + "  not found ! ");
            return default(T);
        } else
        {
            T wndDialog = go.GetComponent<T> ();
            if (wndDialog != null) {
                wndDialog.WndStart();
                m_lwnd.Add(wndDialog);
                return wndDialog;
            } else {
                wndDialog = go.AddComponent<T> ();
                if (wndDialog != null) {
                    wndDialog.WndStart();
                    m_lwnd.Add(wndDialog);
                }
                
                return wndDialog;
            }
            
        }
    }
    public static bool IsHitNGUI(out RaycastHit hit)
    {
        if (null == WndManager.GetNGUICamera()) {
            hit = new RaycastHit();
            return false;
        }
        Camera camera = WndManager.GetNGUICamera();
        Ray ray = WndManager.GetNGUICamera().ScreenPointToRay(Input.mousePosition);
        
        if (!Physics.Raycast(ray, out hit, camera.farClipPlane - camera.nearClipPlane, 1 << LayerMask.NameToLayer("NGUICamera"))) {
            return false;
        }
        return true;
    }
    /// <summary>
    /// 销毁WndRoot下的所有窗口
    /// </summary>
    public static void DestroyAllDialogs()
    {
        foreach (WndBase w in m_lwnd) {
            if (w != null) {
                GameObject.DestroyImmediate(w.gameObject);
            }
        }
        m_lwnd.Clear();
        
        GameObject root = GetWndRoot();
        if (root != null) {
            for (int i = root.transform.childCount - 1; i >= 0; i--) {
                GameObject.DestroyImmediate(root.transform.GetChild(i).gameObject);
            }
        }
        ResetWndDepth();
    }
    
public static T ShowDialog<T>(bool isShow)where T:
    WndBase {
        T   wnd = FindDialog<T>();
        if (wnd != null)
        {
            wnd.gameObject.SetActive(isShow);
            return wnd;
        } else
        {
            return default(T);
        }
    }
    /// <summary>
    /// 显示或隐藏WndRoot下的所有窗口
    /// </summary>
    /// <param name="isShow"></param>
    public static void ShowAllWnds(bool isShow)
    {
        foreach (WndBase w in m_lwnd) {
            if (w != null) {
                w.gameObject.SetActive(isShow);
            }
        }
        
    }
    /// <summary>
    /// 恢复窗体深度
    /// </summary>
    private static void ResetWndDepth()
    {
        s_NormalWndDepthCounter = 0;
        s_TopWndDepthCounter = 5000;
    }
    
    /// <summary>
    /// 设置wnd1 在wnd2前
    /// </summary>
    public static void SetBeforeWnd(WndBase wnd1, WndBase wnd2, int zDepth = 0)
    {
        int index = 0;
        if (wnd2 is WndTopBase) {
            index = TopWndDepth;
        } else {
            index = NormalWndDepth;
        }
        foreach (UIPanel panel in wnd1.BaseHead().m_listUIpanel) {
            panel.depth = ++ index;
        }
        if (wnd2 is WndTopBase) {
            TopWndDepth = index;
        } else {
            NormalWndDepth = index;
        }
        
        if (wnd1.transform.localPosition.z > wnd2.transform.localPosition.z) {
            wnd1.transform.localPosition = new Vector3(wnd1.transform.localPosition.x, wnd1.transform.localPosition.y, wnd2.transform.localPosition.z);
        }
    }
    
    public static void DestroyWndItem(string name)
    {
        Transform t = GetWndRoot().transform.Find(name);
        if (t != null) {
            Destroy(t.gameObject);
        }
    }
}
