using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// 连接性判断算法。
/// </summary>
/// <author>zhulin</author>
public class NDLink  {

	private static List<Int2> m_link = new List<Int2>();
	private static Int2 m_Area;
	private static bool m_putSame = false;

	/// <summary>
	/// 清空算法数据
	/// </summary>
	public static void ClearLink()
	{
		m_link.Clear();
		m_putSame = false;
		m_Area = Int2.zero;
	}


	/// <summary>
	/// 验证连接是否合法
	/// </summary>
	public static bool CheckLinkOK()
	{
		if(m_link.Count == 0
		   || CheckInArea (m_link) == false 
		   || m_putSame == true)
		   return false;

		if(m_link.Count == 1)
			return true;
		//算法开始。
		List<Int2> Have = new List<Int2>();
		List<Int2> Around = new List<Int2>();
		//算法初始化。
		Have.Add(m_link[0]) ;
		m_link.RemoveAt(0);
		//获取周边列表。
		while(m_link.Count > 0)
		{
			//获取周边列表。
			Around = GetRound(Have);
			//裁剪掉相同的。
			List<Int2> Same = GetSameList(Around , m_link);
			//裁剪不下去了
			if(Same.Count == 0 ) return false;
			else
			{
				Have.AddRange(Same);
				SubList(m_link ,Same);
			}
		}
		return true;
	}


	/// <summary>
	/// 清空算法数据
	/// </summary>
	public static void JoinLink(List<Int2> SubList,List<Int2>AddList,List <RoomGrid> PutRoom ,Int2 Area)
	{
		ClearLink();
		//put
		foreach(RoomGrid R in PutRoom)
		{
			if(m_link.Contains (R.mPosRoomGrid) == false)
				m_link.Add(R.mPosRoomGrid);
		}
		//sub
		foreach(Int2 p in SubList)
		{
			if(m_link.Contains (p) == true)
				m_link.Remove(p);
		}
		//add
		foreach(Int2 p in AddList)
		{
			if(m_link.Contains (p) == false)
				m_link.Add(p);
			else m_putSame = true;
		}

		m_Area = Area;
	}

	/// <summary>
	/// 获取可放置区域
	/// </summary>
	public static List<Int2> GetCanPutArea()
	{
		return GetRound(m_link);
	}

	/// <summary>
	/// 获取周边数据
	/// </summary>
	private static List<Int2> GetRound(Int2 Pos)
	{
		List<Int2> l = new List<Int2>();
		l.Add(new Int2(Pos.Unit - 1,  Pos.Layer -1));
		l.Add(new Int2(Pos.Unit - 1, Pos.Layer));
		l.Add(new Int2(Pos.Unit - 1,  Pos.Layer +1));
		l.Add(new Int2(Pos.Unit , Pos.Layer -1));
		l.Add(new Int2(Pos.Unit , Pos.Layer + 1)); 
		l.Add(new Int2(Pos.Unit + 1,  Pos.Layer -1));
		l.Add(new Int2(Pos.Unit + 1, Pos.Layer));
		l.Add(new Int2(Pos.Unit + 1,  Pos.Layer +1));
		return l ;
	}




	/// <summary>
	/// 获取周边数据
	/// </summary>
	private static List<Int2> GetRound(List<Int2> ll)
	{
		List<Int2> l = new List<Int2>();
		foreach(Int2 Pos in ll)
		{
			List<Int2> lll = GetRound(Pos);
			foreach(Int2 p in lll)
			{
				if(l.Contains (p) == false && ll.Contains (p) == false)
				{
					if(p.Unit >= 0&& p.Unit<8 && p.Layer>=0 && p.Layer<4)
						l.Add(p);
				}
			}
		}
		return l;
	}


	/// <summary>
	/// 获取相同的列表
	/// </summary>
	private static List<Int2> GetSameList(List<Int2> l2 ,List <Int2> l3)
	{
		List<Int2> l = new List<Int2>();
		foreach(Int2 Pos in l2)
		{
			if(l3.Contains (Pos) == true )
				l.Add(Pos);
		}
		return l;
	}

	/// <summary>
	/// 裁剪一部分列表
	/// </summary>
	private static void SubList(List<Int2> TargetList ,List <Int2> SubList)
	{
		foreach(Int2 Pos in SubList)
		{
			if(TargetList.Contains (Pos) == true )
				TargetList.Remove(Pos);
		}
	}


	private static bool CheckInArea(List<Int2> TargetList)
	{
		List<Int2> l = new List<Int2>();
		foreach(Int2 Pos in TargetList )
		{
			if(Pos.Layer < 0 || Pos.Layer >= m_Area.Layer 
			   || Pos.Unit < 0 || Pos.Unit >= m_Area.Unit)
				return false;
		}
		return true;
	}

	/// <summary>
	/// 获取连续区间  List<int> Line 值为1 标识摆设了
	/// </summary>
	/// 返回的为地图格子坐标范围I  nt2.Layer为区间开始 Int2.Unit区间结束
	public static List<Int2> GetContinuousRange(List<int> Line)
	{
		List<Int2> l = new List<Int2>();
		if(Line == null || Line.Count == 0)
			return l;
		Int2 start = Int2.zero;
		bool bFirst = false;
		for(int i = 0; i < Line.Count ; i++)
		{
			if(bFirst == false)
			{
				if(Line[i] == 1 )
				{
					bFirst = true;
					start.Layer = i;
					start.Unit = i;
				}
			}
			else
			{
				if(Line[i] == 1 )
				{
					start.Unit = i;
				}
				else 
				{
					l.Add(start);
					bFirst = false;
				}
			}
		}
		//最后一个点。
		if(bFirst == true)
		{
			start.Unit = Line.Count -1;
			l.Add(start);
		}
		return l;
	}


	/// <summary>
	/// 获取未与金库连接的地图格子
	/// </summary>
	public static List<Int2> GetNoLinkGrid(List<Int2> Removelist,List<Int2>AddList,List<Int2> GoldList,List <RoomGrid> PutRoom ,Int2 Area)
	{
		List<Int2> lNoLink = new List<Int2>();
		ClearLink();
		//put
		foreach(RoomGrid R in PutRoom)
		{
			if(m_link.Contains (R.mPosRoomGrid) == false)
				m_link.Add(R.mPosRoomGrid);
		}
		//sub
		foreach(Int2 p in Removelist)
		{
			if(m_link.Contains (p) == true)
				m_link.Remove(p);
		}
		//add
		foreach(Int2 p in AddList)
		{
			if(m_link.Contains (p) == false)
				m_link.Add(p);
			else m_putSame = true;
		}

		m_Area = Area;
		//判断移动的是否为金库
		if(SameGridList (Removelist , GoldList) == true)
		{
			lNoLink = GetNoLinkGrid(AddList);
		}
		else lNoLink = GetNoLinkGrid(GoldList);

		return lNoLink;
	}

	/// <summary>
	/// 判断是否为相同的列表
	/// </summary>
	public static bool SameGridList(List<Int2> l1,List<Int2>l2)
	{
		if(l1 == null || l2 == null)
			return false;
		if(l1.Count == 0 || l2.Count == 0 )
			return false;
		if(l1.Count != l2.Count)
			return false;
		//l2 是否全包含 l1 的元素
		foreach(Int2 Pos in l1)
		{
			if(l2.Contains(Pos) == false)
				return false;
		}
		//l1 是否全包含 l2 的元素
		foreach(Int2 Pos in l2)
		{
			if(l1.Contains(Pos) == false)
				return false;
		}
		return true;
	}

	/// <summary>
	/// 获取未连接的地图格子
	/// </summary>
	public static List<Int2> GetNoLinkGrid(List<Int2> GoldList)
	{
		//检测非法性
		List<Int2> lNoLink = new List<Int2>();
		if(m_link.Count == 0
		   || CheckInArea (m_link) == false 
		   || m_putSame == true)
			return lNoLink;
		//检测非法性
		if(GoldList.Count == 0
		   || CheckInArea (GoldList) == false 
		   || m_putSame == true)
			return lNoLink;
		
		if(m_link.Count == 1)
			return lNoLink;
		//从金库开始出发。
		List<Int2> Have = new List<Int2>();
		List<Int2> Around = new List<Int2>();
		//算法初始化。
		Have.Add(GoldList[0]) ;
		m_link.Remove(GoldList[0]);
		//获取周边列表。
		while(m_link.Count > 0)
		{
			//获取周边列表。
			Around = GetRound(Have);
			//裁剪掉相同的。
			List<Int2> Same = GetSameList(Around , m_link);
			//裁剪不下去了
			if(Same.Count == 0 )
			{
				foreach(Int2 Pos in m_link)
				{
					lNoLink.Add(Pos);
				}
				return lNoLink;
			}
			else
			{
				Have.AddRange(Same);
				SubList(m_link ,Same);
			}
		}
		return lNoLink;
	}
}
