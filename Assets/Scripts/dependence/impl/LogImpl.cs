using System.Collections.Generic;
using UnityEngine;
using Logic;

/// <summary>
/// 日志的实现
/// </summary>
public class LogImpl : Log
{
    private Dictionary<string, bool> traceOn = new Dictionary<string, bool>();
    
    // debug用，所有的日志信息
    public List<string> debugLogList = new List<string>();
    
    public void Init()
    {
    }
    
    /// <summary>
    /// 普通日志接口
    /// </summary>
    /// <param name="path"> 日志的来源文件 </param>
    /// <param name="content"> 日志的内容 </param>
    public void To(string path, string content, params object[] args)
    {
        To(path, content, LogLevel.NORMAL, args);
    }
    
    /// <summary>
    /// 错误日志接口
    /// </summary>
    /// <param name="path"> 日志的来源文件 </param>
    /// <param name="content"> 日志的内容 </param>
    public void Error(string path, string content, params object[] args)
    {
        To(path, content, LogLevel.ERROR, args);
    }
    
    /// <summary>
    ///  Trace信息
    /// </summary>
    public void Trace(string path, string content, params object[] args)
    {
        if (traceOn.ContainsKey(path) && ! traceOn[path])
            // 此路径不输出
        {
            return;
        }
        
        To(path, content, LogLevel.TRACE, args);
    }
    
    /// <summary>
    /// 是否打开trace开关
    /// </summary>
    public void EnableTrace(string path, bool enable)
    {
        traceOn[path] = enable;
    }
    
    /// <summary>
    /// 日志接口
    /// </summary>
    /// <param name="path"> 日志的来源文件 </param>
    /// <param name="content"> 日志的内容 </param>
    /// <param name="level"> 日志的严重等级 </param>
    public void To(string path, string content, LogLevel level, params object[] args)
    {
    }
    
    /// <summary>
    /// 直接打印接口
    /// </summary>
    public void Print(string content, params object[] args)
    {
    
    }
    
    
    
    private void DebugLog(string log)
    {
        debugLogList.Add(log);
        if (debugLogList.Count > 50) {
            debugLogList.RemoveAt(0);
        }
    }
}
