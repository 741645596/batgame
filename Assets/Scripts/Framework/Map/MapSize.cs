using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 地图大小
/// 船的map size 由有制作船时产生，
/// </summary>
/// <author>zhulin</author>

public class MapSize  {

	private static List<Int3>m_Size = new List<Int3>();

	/// <summary>
	/// 设置地图大小
	/// </summary>
	public static void SetMapSize(List<Int3> SizeList)
	{
		ClearMapSize ();
		if (SizeList == null 
		    || SizeList.Count == 0)
				return;

		for (int i = 0; i< SizeList.Count; i++) 
		{
			m_Size.Add(SizeList[i]);
		}
	}

	/// <summary>
	/// 清空地图大小
	/// </summary>
	public static void ClearMapSize()
	{
		if(m_Size == null)
			m_Size = new List<Int3>();
		m_Size.Clear ();
	}

	/// <summary>
	/// 获取指定层的大小
	/// </summary>
	public static Int3 GetMapLayerSize(int layer)
	{
		if(m_Size == null || m_Size.Count == 0 )
			return Int3.zero;

		if (layer < 0 || m_Size.Count <= layer)
			return Int3.zero;

		return m_Size [layer];
	}


	/// <summary>
	/// 获取一层的格子起点
	/// </summary>
	public static int GetGridStart(int layer)
	{
		return GetMapLayerSize (layer).GridStart;
	}

	/// <summary>
	/// 获取一层的房间内部格子起点
	/// </summary>
	public static int GetRoomStart(int layer)
	{
		return GetMapLayerSize (layer).RoomStart;
	}


	/// <summary>
	/// 获取一层格子的数量
	/// </summary>
	public static int GetLayerSize(int layer)
	{
		return GetMapLayerSize (layer).GridLength;
	}



	/// <summary>
	/// 获取一层索引最大的格子
	/// </summary>
	public static int GetLayerMaxGrid(int layer)
	{
		return  GetGridStart(layer)+ GetLayerSize (layer) -1;
	}


	/// <summary>
	/// 获取地图层数
	/// </summary>
	public static int GetMapLayer()
	{
		if(m_Size == null || m_Size.Count == 0 )
			return 0;
		return m_Size.Count;
	}

	/// <summary>
	/// 判断格子是否在地图中
	/// </summary>
	public static bool IsInMap(Int2 Pos)
	{
		Int3 s= GetMapLayerSize (Pos.Layer);
		if (s == Int3.zero)
			return false;

		if (Pos.Unit < s.GridStart || Pos.Unit >= s.GridStart + s.GridLength)
						return false;
		return true;
	}


	public static bool IsBorder(int layer ,int unit)
	{
		int ld  =Mathf.Abs(GetRoomStart(layer) - unit);
		int rd  =Mathf.Abs(GetLayerMaxGrid(layer) - unit);
		if (ld <= 1 || rd <= 1) return true;
		else return false;
	}
	
	
	
	public static bool IsBorder(int layer ,int unit ,bool IsLeft)
	{
		int ld  =Mathf.Abs(GetRoomStart(layer) - unit);
		int rd  =Mathf.Abs(GetLayerMaxGrid(layer) - unit);
		
		if (ld <= 1 && IsLeft == true) return true;
		else if (rd <= 1 && IsLeft == false) return true;
		else return false;
	}

	/// <summary>
	/// 获取邻居位
	/// </summary>
	public static bool Getbrothers(Int2 p,DIR dir, ref Int2 pos)
	{
		if (dir == DIR.UP) 
		{
			pos.Layer = p.Layer + 1;
			pos.Unit = p.Unit;
		} 
		else if (dir == DIR.DOWN) 
		{
			pos.Layer = p.Layer - 1;
			pos.Unit = p.Unit;
		} 
		else if (dir == DIR.LEFT) 
		{
			pos.Layer = p.Layer ;
			pos.Unit = p.Unit -1;
		} 
		else if (dir == DIR.RIGHT) 
		{
			pos.Layer = p.Layer ;
			pos.Unit = p.Unit  +1;
		} 
		else 
		{
			return false;
		}

		if (IsInMap (pos)) 
		{
			return true;
		}
		return false;

	}



}
