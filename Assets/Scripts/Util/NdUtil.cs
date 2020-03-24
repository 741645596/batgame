 using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
#else 
	#if UNITY_IPHONE
	using System.Runtime.InteropServices;
	#endif
#endif


/// <summary>
/// 打印
/// </summary>
/// <author>zhulin</author>
public enum PRINT{
	ZLN = 0,
	LQF = 1, 
	TXM = 2,
	LF  = 3,
	LHF = 4,
};
/// <summary>
/// 通用的算法接口
/// </summary>
/// <author>zhulin</author>
public enum WalkDir{
	WALKLEFT  = 0x01,
	WALKRIGHT = 0x02, 
	WALKSTOP  = 0x03
};

/// <summary>
/// LifeM 类型
/// </summary>
[System.Flags]
public enum LifeMType
{
	None          = 0,
	SOLDIER       = 1 << 0,
	PET           = 1 << 1,
	BUILD         = 1 << 2,
	WALL          = 1 << 3,
	FLOOR         = 1 << 4,
	SUMMONPET     = 1 << 5, //召唤物
	SUMMONPROS = 1 << 6, //道具
	INHERITSUMMONPROS = 1 << 7, //道具
	ALL = SOLDIER | PET | BUILD | WALL | FLOOR | SUMMONPET | SUMMONPROS | INHERITSUMMONPROS,
};

/// <summary>
/// LifeM 阵营
/// </summary>
[System.Flags]
public enum LifeMCamp
{
	NONE    = 0,
	ATTACK  = 1 << 0,
	DEFENSE = 1 << 1,
	ALL     = ATTACK | DEFENSE,
};



/// <summary>
/// 运动状态
/// </summary>
public enum MoveState
{
	Static     = 0,   //静止（一般用于静态物体）
	Fly        = 1,   //飞行
	Walk       = 2,   //行走
	BackWalk   = 3,   //倒退，击退
};



/// <summary>
/// 建筑物主分类
/// </summary>
public enum BuildType
{
	None              = 0, //非法类型
	ResBuild          = 1, //资源房间 
	Disorder          = 2, //障碍房间，包含陷阱与宿舍陷阱
	RoomTrap          = 3, //宿舍陷阱
	Trap              = 4, //非宿舍陷阱
	WallRoom          = 5, //带墙建筑
	Stair             = 6, //楼梯
};

public enum AttackResult
{
	Normal     = 0,  //正常攻击
	Miss       = 1,  //被miss掉，未命中
	Immunity   = 2,  //被免疫掉
	Crit       = 3,  //暴击
	Shield     = 4,  //被盾防御
	Fire	   = 5,	 //炮战
};

/// <summary>
/// 技能释放信息
/// </summary>
public struct SkillReleaseInfo
{
	public int m_Damage;                   //技能产生的伤害
	public List<StatusType> m_MakeStatus;  //技能产生的状态
	public bool m_bImmunity;               //是否被免疫
	public bool m_InterruptSkill;          //技能是否打断
	public string  m_struckeffect;         //受击特效id
	public AttackResult Result;            //攻击结果
	public AttackType HitType;             //受到伤害类型
    public AttributeType HitAttributeType;
	public int mOwnerLevel;
}




public class NdUtil  {

	#region 公式除法
	private static int g_sceneID = 0;





	public static float IDivide(int a,int b)
	{
		if(b==0)
		{
			return ConstantData.ERR_DIVIDE;
		}
		return a *1.0f/b;
	}

	public static float IDivide(int a,float b)
	{
		if(b==0.0f)
		{
			return ConstantData.ERR_DIVIDE;
		}
		return a *1.0f/b;
	}

	public static float IDivide(float a,int b)
	{
		if(b==0)
		{
			return ConstantData.ERR_DIVIDE;
		}
		return a *1.0f/b;
	}

	public static float IDivide(float a,float b)
	{
		if(b==0.0f)
		{
			return ConstantData.ERR_DIVIDE;
		}
		return a *1.0f/b;
	}

	//获取一个整数的位数
	public static int GetNumLength(int num)
	{
		if (num < 0)  num = -num;
		if (num < 10) return 1;
		else return 1 + GetNumLength (num/10);
	}


	//获取一个整数，从左---》右，第N位的数字
	public static int GetNumIndex(string num , int index)
	{;
		if (num.Length >= index) 
		{
			char[] myChars = num.ToCharArray ();
			return (Convert.ToInt32(myChars [index - 1]) -Convert.ToInt32('0'));
		} 
		else return 0;
		//str
	}


	#endregion

	#region 位置和向量运算方法


	public static bool V3Equal (Vector3 a, Vector3 b)
	{
		return (Mathf.Abs (a.x - b.x) < 0.001) && (Mathf.Abs (a.y - b.y) < 0.001) && (Mathf.Abs (a.z - b.z) < 0.001);
	}


    /// <summary>
    /// 限制角度
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < 0)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
	
	public static float V2toAngle(Vector2 prev, Vector2 cur,Vector2 referenceAxes)
	{
		float angle = Vector2.Angle (referenceAxes,cur-prev);
		angle = Vector2.Dot (cur-prev, Vector2.up) < 0 ? -angle : angle;
		angle = ClampAngle(angle, 0, 360f);
		return angle;
	}
	public static float V3toAngle(Vector3 prev, Vector3 cur,Vector3 referenceAxes)
	{
		float angle = Vector3.Angle (referenceAxes,cur-prev);
		angle = Vector3.Dot (cur-prev, Vector3.up) < 0 ? -angle : angle;
		angle = ClampAngle(angle, 0, 360f);
		return angle;
	}
	#endregion

	/// <summary>
	/// 是否处于地图的同一个地图单元
	/// </summary>
	///


	
	public static bool IsSameMapLayer(Int2 p1 ,Int2 p2)
	{
		return (p1.Layer == p2.Layer) ? true : false;
	}
	
	public static bool IsLifeSampMapLayer(Life a,Int2 p)
	{
		List<MapGrid> allg = a.GetAllMapGrid();
		foreach (MapGrid g in allg)
		{
			if (IsSameMapLayer(g.GridPos,p))
				return true;
		}
		return false;
	}
	
	public static bool IsSameMapUnit(Int2 p1 ,Int2 p2)
	{
		return (p1.Unit == p2.Unit) ? true : false;
	}


	public static bool IsInArea(Int2 Area ,Int2 pos ,int distance)
	{
		if(Area.Layer != pos.Layer)
			return false;
		int d = Math.Abs(Area.Unit  - pos.Unit);
		if(d > distance) return false;
		else return true;
	}


	//同位置判断	
	public static bool IsSameMapPos(Int2 p1 ,Int2 p2)
	{
		return (IsSameMapLayer (p1, p2) && IsSameMapUnit (p1, p2)) ? true : false;
	}



	/// <summary>
	/// 在圆形范围内 
	/// </summary>
	public static bool IsInCircle(Int2 CirclePt,int Radius, Int2 Pt)
	{
		if(CirclePt.Layer != Pt.Layer)
			return false;
		int d = Math.Abs(CirclePt.Unit  - Pt.Unit);
		if(d > Radius) return false;
		else return true;
	}
	
	/// <summary>
	/// 在球形范围内 
	/// </summary>
	public static bool IsInBall(Int2 BallPt,int xRadius,int yRadius, Int2 Pt)
	{
		int y = Math.Abs(BallPt.Layer  - Pt.Layer);
		int x = Math.Abs(BallPt.Unit  - Pt.Unit);
		if(x > xRadius || y > yRadius) return false;
		else return true;
	}



	public static bool IsHaveValueInList(List<int> l , int Value)
	{
		if (l == null || l.Count == 0)
						return false;

		for (int i = 0; i< l.Count; i++) 
		{
			if(l[i] == Value) return true;
		}
		return false;
	}

	public static void SubList(ref List<int>list ,List<int >l)
	{
		if (list == null || list.Count == 0)
						return;

		if (l == null || l.Count == 0)
			return;

		for (int i = 0; i< l.Count; i++) 
		{
			SubList(ref list , l[i]);
		}
	}


	public static void SubList(ref List<int>List ,int value)
	{
		if (List == null || List.Count == 0)
			return;
		if(List.Contains(value) == true)
		{
			List.Remove(value);
		}
	}
	
	
	public static int GetSceneID ()
	{
		return g_sceneID ++;
	}

	public static void ResetSceneID()
	{
		g_sceneID = 0;
	}

    /// <summary>
    /// int抽取
    /// </summary>
    /// <param name="Value"></param>
    /// <param name="From">从低位开始 index从1开始</param>
    /// <param name="To"></param>
    /// <returns></returns>
	public static int Bit32Extract(int Value, int From, int To)
	{
		int ret = 0;
		for (int i = From,b = 0; i <= To; i++,b++) 
		{
			ret += ((Value / Convert.ToInt32(Math.Pow (10,i-1))) % 10 * Convert.ToInt32(Math.Pow (10,b))) ;
		}

		return ret;
	}

	/// <summary>
	/// 从串中取第index个值字符串。index 序号从 1开始。
	/// </summary>
	public static string GetStrValue(string strTarget ,int index)
	{
		string v = ",";
		char[] cutChar = v.ToCharArray();
		string[] sArray = strTarget.Split(cutChar);
		if(sArray.Length <= index || index < 0) 
			return string.Empty;
		return sArray[index];
	}

	/// <summary>
	/// 从串中取第index个值整形值，使用 英文 逗号 分隔
	/// </summary>
	public static int GetIntValue(string strTarget ,int index)
	{
		string v = ",";
		char[] cutChar = v.ToCharArray();
		string[] sArray = strTarget.Split(cutChar);
		if(sArray.Length <= index || index < 0) 
			return -1;
		return int.Parse(sArray[index]);
	}


	/// <summary>
	/// 获取字符串的单元长度
	/// </summary>
	public static int GetLength(string strTarget )
	{
		string v = ",";
		char[] cutChar = v.ToCharArray();
		string[] sArray = strTarget.Split(cutChar);
		return sArray.Length;
	}

    /// <summary>
    /// 判定i是否在(minExclude,maxInclude]之间
    /// </summary>
    public static bool IsIntBetween(int i, int minExclude, int maxInclude)
    {
        if (i>minExclude && i<=maxInclude)
        {
            return true;
        }
        return false;
    }
	
	#if UNITY_EDITOR
	#else 
		#if UNITY_IPHONE
		[DllImport("__Internal")]
		public static extern string getUDID();
		#endif
	#endif
	public static string GetUdid()
	{
		string strUdid = SystemInfo.deviceUniqueIdentifier ;
		#if UNITY_EDITOR
		#else 
			#if UNITY_IPHONE
			try
			{
				strUdid = getUDID();
			}
			catch (System.Exception e)
			{
			}
			#endif
		#endif
		return strUdid;
	}
    /// <summary>
    /// 把服务器1970时间转换成当前时间（hh:mm:ss）
    /// </summary>
    /// <param name="count">Unix时间戳</param>
    /// <returns></returns>
    public static string ConvertServerTime(int count)
    {
        DateTime dtBase = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime convertTime = dtBase.Add(new TimeSpan(count * TimeSpan.TicksPerSecond));
       return ( convertTime.ToLocalTime().ToString("HH:mm:ss"));
    }
    /// <summary>
    /// 把服务器1970时间转换成当前时间（HH:mm）
    /// </summary>
    public static string ConvertServerTime1(int count)
    {
        DateTime dtBase = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime convertTime = dtBase.Add(new TimeSpan(count * TimeSpan.TicksPerSecond));
        return (convertTime.ToLocalTime().ToString("HH:mm"));
    }
    /// <summary>
    /// 把服务器1970时间转换成当前时间（yyyy-MM-dd）
    /// </summary>
    public static string ConvertServerTime2(int count)
    {
        DateTime dtBase = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime convertTime = dtBase.Add(new TimeSpan(count * TimeSpan.TicksPerSecond));
        return (convertTime.ToLocalTime().ToString("yyyy-MM-dd"));
    }

    public static string ConvertServerTime3(int count)
    {
        DateTime dtBase = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime convertTime = dtBase.Add(new TimeSpan(count * TimeSpan.TicksPerSecond));
        return (convertTime.ToLocalTime().ToString("mm:ss"));
    }
    /// <summary>
    /// 返回当前时间格式：201601 (2016年1月)
    /// </summary>
    public static string ConvertServerTimeYYMM()
    {
        DateTime dtBase = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        int nowTime = GlobalTimer.GetNowTimeInt();
        DateTime convertTime = dtBase.Add(new TimeSpan(nowTime * TimeSpan.TicksPerSecond));
        string str = convertTime.ToLocalTime().ToString("yyyyMMdd");
        return str.Substring(0, 6);
    }
    /// <summary>
    /// 返回当前时间格式：20160102 (2016年1月2日)
    /// </summary>
    public static string ConvertServerTimeYYMMDD()
    {
        DateTime dtBase = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        int nowTime = GlobalTimer.GetNowTimeInt();
        DateTime convertTime = dtBase.Add(new TimeSpan(nowTime * TimeSpan.TicksPerSecond));
        return (convertTime.ToLocalTime().ToString("yyyyMMdd"));
    }
    /// <summary>
    /// 获取当前DateTime
    /// </summary>
    public static DateTime GetNowDataTime()
    {
        DateTime dtBase = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        int nowTime = GlobalTimer.GetNowTimeInt();
        DateTime convertTime = dtBase.Add(new TimeSpan(nowTime * TimeSpan.TicksPerSecond));
        return convertTime;
    }
    /// <summary>
    /// DateTime转Unix时间戳
    /// </summary>
    public static long DateTimeToUnixTimestamp(DateTime dateTime)
    {
        return (dateTime.ToUniversalTime().Ticks-621355968000000000)/10000000;
    }

	/// <summary>
	/// 酒馆钻石免费购买倒计时.
	/// </summary>
	/// <returns>The ernie diamond time.</returns>
	/// <param name="count">Count.</param>
	public static string ConverErnieDiamondTime(int count)
	{
		DateTime dtBase = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		DateTime convertTime = dtBase.Add(new TimeSpan(count * TimeSpan.TicksPerSecond));
		return ((convertTime.Day-1) * 24 + convertTime.Hour).ToString ("00") + ":" + convertTime.Minute.ToString ("00") + ":" + convertTime.Second.ToString ("00");
	}
    /// <summary>
    /// 判定是否是同一天
    /// </summary>
    /// <param name="count1">Unix时间戳1</param>
    /// <param name="count2">Unix时间戳2</param>
    public static bool IsSameDay(int count1,int count2)
    {
        DateTime dtBase = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime convertTime1 = dtBase.Add(new TimeSpan(count1 * TimeSpan.TicksPerSecond));
        DateTime convertTime2 = dtBase.Add(new TimeSpan(count2 * TimeSpan.TicksPerSecond));
        int day1 = convertTime1.ToLocalTime().Day;
        int day2 = convertTime2.ToLocalTime().Day;
        return day1 == day2;
    }

    /// <summary>
    /// 格式化倒计时文本
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public static string TimeFormat(int second)
    {
        int ss = second % 60;
        int mm = (second / 60) % 60;
        int hh = 0;
        if (second >= 3600)
            hh = second / 3600;

        System.TimeSpan interval = new System.TimeSpan(hh, mm, ss);
        string result = interval.ToString();
        if (second<3600)
            result = result.Substring(3);

        return result;
    }

    /// <summary>
    /// 格式化输出 111，222，333
    /// </summary>
    public static string IntFormat(int i)
    {
        if (i == 0)
        {
            return "0";
        }
        return i.ToString("#,###");
    }
    /// <summary>
    /// 111,222,333 to 111222333
    /// </summary>
    /// <param name="text">逗号分割的数值串</param>
    /// <returns></returns>
    public static int IntFormat(string text)
    {
        string [] strArray = text.Split(',');
        string str = "";
        for (int i = 0; i < strArray.Length; i++)
        {
            str += strArray[i];
        }
        return Convert.ToInt32(str);
    }
    /// <summary>
    /// 转换百分比（保留小数点后两位）
    /// </summary>
    public static string ConvertPercent(float f)
    {
        return (f / 100).ToString("#.##%");
    }
    /// <summary>
    /// 转换百分比（保留整数）
    /// </summary>
    public static string ConvertPercentInt(float f)
    {
        string str = string.Format("{0}", f * 100);
        if (str.Length>5)
        {
            return str.Substring(0, 4) + "%";
        }
        else
            return str+ "%";
    }

    /// <summary>
    /// 转换仟分比（保留小数点后两位）
    /// </summary>
    public static string ConvertPermillage(float f)
    {
        return (f / 1000).ToString("#.##%");
    }
    /// <summary>
    /// 小于10000时返回具体数值，超过时显示x万（策划案修改后暂时未用到）
    /// </summary>
    public static string Convert10000(int i)
    {
        if ( i < 10000 )
            return i.ToString();
        else
        {
            int j = i / 10000;
            return string.Format("{0}{1}", j, "W");
        }
    }

}
/// <summary>
/// List 扩展方法
/// </summary>
public static class ListExtensions
{
    /// <summary>
    /// 获取下一个元素
    /// </summary>
    public static T NextOf<T>(this IList<T> list, T item)
    {
        return list[(list.IndexOf(item) + 1) == list.Count ? 0 : (list.IndexOf(item) + 1)];
    }
    public static T PrevOf<T>(this IList<T> list, T item)
    {
        return list[(list.IndexOf(item) - 1) < 0 ? list.Count -1 : (list.IndexOf(item) - 1)];
    }

    /// <summary>
    /// 添加元素到第一个
    /// </summary>
    public static void AddToFront<T>(this List<T> list, T item)
    {
        if (list.Contains(item) == false)
        {
            list.Insert(0, item);
        }
    }
    /// <summary>
    /// 随机排序
    /// </summary>
    public static List<T> Shuffle<T>(this List<T> list)
    {
        System.Random random = new System.Random();
        List<T> newList = new List<T>();

        foreach (T item in list)
        {
            newList.Insert(random.Next(newList.Count), item);
        }

        return newList;
    }


}
