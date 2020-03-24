using System;
using System.Reflection;

/// <summary>
/// 脚本管理 
/// </summary>
/// <author>zhulin</author>
public partial class ScriptM
{
	private static ScriptM instance = new ScriptM();
	
	/// <summary>
	/// 根据脚本编号调用脚本 
	/// </summary>
	public static object Invoke(int scriptNo, params object[] args)
	{
		// 0 - 100为特殊内置脚本
		if (scriptNo == 0)
			return false;
		if (scriptNo <= 100)
			return scriptNo;
		
		string funcName = string.Format("SCRIPT_{0}", scriptNo);
		MethodInfo f = instance.GetType().GetMethod(funcName, BindingFlags.Static | BindingFlags.NonPublic);
		if (f == null)
		{
			App.log.Error("ScriptM.cs", "脚本{0}处理函数不存在。", scriptNo);
			return false;
		}
		
		return f.Invoke(instance, args);
	}
	
	/// <summary>
	/// 调用公式
	/// </summary>
	public static T Formula<T>(string formuleName, params object[] args)
	{
		MethodInfo f = instance.GetType().GetMethod(formuleName, BindingFlags.Static | BindingFlags.NonPublic);
		if (f == null)
		{
			App.log.Error("ScriptM.cs", "公式{0}处理函数不存在。", formuleName);
			return (T) (object) false;
		}

		object o = f.Invoke(instance, args);
		return (T) o;
	}
	
	/// <summary>
	/// 判断某个脚本是否存在
	/// </summary>
	public static bool ContainsScript(int scriptNo)
	{
		string funcName = string.Format("SCRIPT_{0}", scriptNo);
		MethodInfo f = instance.GetType().GetMethod(funcName, BindingFlags.Static | BindingFlags.NonPublic);
		if (f == null)
		{
			App.log.Error("ScriptM.cs", "脚本{0}处理函数不存在。", scriptNo);
			return false;
		}
		return true;
	}	
	public static bool ContainsScript(string scriptName)
	{
		MethodInfo f = instance.GetType().GetMethod(scriptName, BindingFlags.Static | BindingFlags.NonPublic);
		if (f == null)
		{
			App.log.Error("ScriptM.cs", "脚本{0}处理函数不存在。", scriptName);
			return false;
		}
		return true;
	}
}
