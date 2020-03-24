using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// 普通窗体基类
/// </summary>
/// <author>zhulin</author>
public class WndBase : MonoBehaviour
{
    WndBase_h thisHead;
    public virtual WndBase_h BaseHead()
    {
        if (thisHead == null) {
            thisHead  = GetComponent<WndBase_h>();
        }
        return thisHead;
    }
    
    
    public virtual void WndStart()
    {
        BindEvents();
        SetWndDepth();
    }
    
    
    /// <summary>
    ///  是否为全屏窗口
    /// </summary>
    public virtual bool IsFullWnd()
    {
        return false ;
    }
    
    public virtual void SetWndDepth()
    {
        if (BaseHead().m_listUIpanel.Count == 0) {
            NGUIUtil.DebugLog("请给窗口添加UIPanel" + gameObject.name);
        }
        CheckListUIPanel(BaseHead().m_listUIpanel);
        int MaxDepth =  SetDepth(WndManager.NormalWndDepth);
        WndManager.NormalWndDepth = MaxDepth;
    }
    
    
    protected void CheckListUIPanel(List<UIPanel> listUIpanel)
    {
        foreach (UIPanel item in listUIpanel) {
            if (item == null) {
                NGUIUtil.DebugLog("_h文件中的listUIpanel 有未设定的");
            }
        }
    }
    
    /// <summary>
    /// 设置窗口的深度（控制窗口表现的前后）
    /// </summary>
    /// <param name="depth"></param>
    protected  int SetDepth(int depth)
    {
        int maxdepth = 0;
        for (int i = 0 ; i < BaseHead().m_listUIpanel.Count; i++) {
            if (BaseHead().m_listUIpanel[i] == null) {
                NGUIUtil.DebugLog("UI预置ListUIPanel有未设定项");
                return depth + 1;
            }
            BaseHead().m_listUIpanel[i].depth += depth;
            if (BaseHead().m_listUIpanel[i].depth > maxdepth) {
                maxdepth = BaseHead().m_listUIpanel[i].depth;
            }
        }
        maxdepth ++ ;
        return maxdepth;
    }
    
    
    public virtual void BindEvents()
    {
    
    }
    
    /// <summary>
    ///  设置窗口ID，应和prefab、类名保持一致
    /// </summary>
    /// <returns></returns>
    public static string DialogIDD()
    {
        //Debug.Log("派生类要覆盖DialogName方法");
        return "WndBase";
    }
    
    public virtual void ShowDialog()
    {
        gameObject.SetActive(true);
    }
    public virtual void ShowDialogWithAction()
    {
        if (gameObject.GetComponent<Animation>() != null) {
            gameObject.GetComponent<Animation>().Play("Show");
        }
    }
    public virtual void CloseDialog()
    {
        gameObject.SetActive(false);
    }
    public virtual void DestroyDialog()
    {
        GameObject.DestroyImmediate(gameObject);
    }
    public virtual void DestroyDialogNow()
    {
        GameObject.DestroyImmediate(gameObject);
    }
}


