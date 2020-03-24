/// <summary>
/// 日志的接口
/// </summary>
public interface Log
{
    /// <summary>
    /// 普通日志接口
    /// </summary>
    /// <param name="path"> 日志的来源文件 </param>
    /// <param name="content"> 日志的内容 </param>
    void To(string path, string content, params object[] args);
    
    /// <summary>
    /// 错误日志接口
    /// </summary>
    /// <param name="path"> 日志的来源文件 </param>
    /// <param name="content"> 日志的内容 </param>
    void Error(string path, string content, params object[] args);
    
    /// <summary>
    /// 日志接口
    /// </summary>
    /// <param name="path"> 日志的来源文件 </param>
    /// <param name="content"> 日志的内容 </param>
    /// <param name="level"> 日志的严重等级 </param>
    void To(string path, string content, LogLevel level, params object[] args);
    
    /// <summary>
    /// Trace信息
    /// </summary>
    void Trace(string path, string content, params object[] args);
    
    /// <summary>
    /// 是否打开trace开关
    /// </summary>
    void EnableTrace(string path, bool enable);
    
    /// <summary>
    /// 直接打印接口
    /// </summary>
    void Print(string content, params object[] args);
    
    
}

/// <summary>
/// 日志的等级
/// </summary>
public enum LogLevel {
    NORMAL,
    WARNING,
    TRACE,
    ERROR,
    CRITICAL
}

