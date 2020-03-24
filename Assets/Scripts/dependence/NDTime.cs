using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 时间管理
/// </summary>
public class NDTime  {

	/// <summary>
	/// 判断两个时间是不是同一天
	/// </summary>
	public static bool IsSameDay(int sec1, int sec2)
	{
		DateTime start = new DateTime(1970, 1, 1);
		DateTime dt1 = start.AddSeconds(sec1);
		DateTime dt2 = start.AddSeconds(sec2);
		
		return dt1.ToLongDateString() == dt2.ToLongDateString();
	}
	/// <summary>
	/// 根据服务器的时间计算本地时间
	/// </summary>
	public static DateTime GetServerTime(int serverTime)
	{
		DateTime start = DateTime.Parse("1970-1-1");
		start = start.AddHours(8f);
		
		DateTime now = start.AddSeconds(serverTime);
		return now;
	}


	/// <summary>
	/// 由秒转换成时间
	/// </summary>
	public static DateTime GetTime(int sec1)
	{
		DateTime start = new DateTime(1970, 1, 1);
		DateTime dt1 = start.AddSeconds(sec1);
		return dt1;
	}
	
	
	/// <summary>
	/// 毫秒值
	/// </summary>
	private static DateTime _start = DateTime.Now;
	public static int Tick
	{
		get
		{
			TimeSpan span = DateTime.Now - _start;
			return (int) span.TotalMilliseconds;
		}
	}
}
