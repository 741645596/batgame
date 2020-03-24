using UnityEngine;
using System.Collections;
/// <summary>
/// 功能对象
/// </summary>
/// <author>zhulin</author>
public class BlockObj : SceneObj
{

    public override void Start()
    {
    
        base.Start();
        RegisterHooks();
        InitObj();
    }
    
    
    public override void OnDestroy()
    {
        AntiRegisterHooks();
        base.OnDestroy();
    }
    
    
    public virtual void InitObj()
    {
    
    }
    /// <summary>
    /// 注册事件
    /// </summary>
    public  virtual void RegisterHooks()
    {
    
    }
    
    /// <summary>
    /// 反注册事件
    /// </summary>
    public  virtual void AntiRegisterHooks()
    {
    
    }
    /// <summary>
    /// 点击事件
    /// </summary>
    public virtual bool OnClick()
    {
        return true;
    }
    
    /// <summary>
    /// 刷新
    /// </summary>
    public virtual void Refresh()
    {
    
    }
    
    
}
