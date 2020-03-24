using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*
墙体数据
*/
public class WallM  {

	private static  List<WallInfo> m_WallList = new List<WallInfo>();
	public static void GetWall()
	{
		m_WallList.Clear();
	}
	
	private static void SetWall(BuildInfo Info)
	{

	}


	public static bool IsWall(int ID)
	{
		WallInfo Info= GetWallData(ID);
	    if (Info == null)
				return false;
		else    return true;
	}
	
	
	public static void GetWallList(ref List<int> list)
	{
		if(list == null)
			list = new List<int>();
		list.Clear ();
		
		for(int i = 0; i < m_WallList.Count; i++)
		{
			if(IsBorderWall(m_WallList[i]) == false)
				list.Add(m_WallList[i].m_id);
		}
	}


	public static WallInfo GetWallData(WallType  type,int WallID)
	{
		WallInfo Info =new WallInfo();
		
		if (type == WallType.LeftTop) 
		{
			FloorInfo floor = CmCarbon.GetFloor();
			Info.m_hp = floor.m_hp;
			Info.m_magicdefend = floor.m_magicdefend;
			Info.m_phydefned = floor.m_phydefend;
			return Info;
		}
		else 
		{
			WallInfo w = GetWallData(WallID);
			return w;
		}
	}

	
	private static WallInfo GetWallData(int WallID )
	{
		if (m_WallList == null)
			return null;
		for(int i = 0; i < m_WallList.Count ; i++)
		{
			if(WallID == m_WallList[i].m_id)
			{
				return m_WallList[i];
			}
		}
		return null;
	}


	//是否为边界强
	private static bool IsBorderWall(WallInfo Info)
	{
		if(Info == null) return false;
		return MapSize.IsBorder (Info.m_cy,Info.m_cx);
	}
	

	
	private static void AddWall(WallInfo Info)
	{
		bool HasCombin = false;
		foreach(WallInfo v in m_WallList)
		{
			if(v == null || Info == null) continue;
			CombinWall( v , Info ,ref HasCombin);

			if(HasCombin ==  true)
			{
				v.m_hp = v.m_hp + Info.m_hp;
				//物理防御合并 之和 四舍五入
				v.m_phydefned = (v.m_phydefned + Info.m_phydefned)/2;
				//魔法防御合并 之和 四舍五入
				v.m_magicdefend = (v.m_magicdefend + Info.m_magicdefend)/2;
				//墙体材料合并 策划暂时未处理
				
				
				//cx 合并
				v.m_cx = (v.m_cx <= Info.m_cx)? v.m_cx:Info.m_cx;
				v.m_wood += v.m_wood;
				v.m_stone += v.m_stone;
				v.m_steel += v.m_steel;

				v.m_id  = 100* v.m_cy + v.m_cx;

				return;
			}
		}
		
		if (HasCombin == false) 
		{
			m_WallList.Add (Info);
		}			
	}
	
	private static void CombinWall(WallInfo Wall1 ,WallInfo Wall2 ,ref bool ret)
	{
		ret = false;
		//合并规则。相邻或重叠的墙进行合并。
		int dis = Wall1.m_cx - Wall2.m_cx;
		dis = Mathf.Abs(dis);
		//合并墙体
		if (Wall1.m_cy == Wall2.m_cy && dis <= 1) 
		{
			ret = true;
		}
		else ret = false;
	}
	//获取左墙
	private static WallInfo GetWallData(BuildInfo Info, bool Isleft)
	{
		WallInfo w = new WallInfo();
		w.m_cy = Info.m_cy;

		if(Isleft == true)
		{
			w.m_cx = Info.m_cx;
		}  
		else 
		{
			w.m_cx = Info.m_cx + Info.m_Shape.width * MapGrid.m_UnitRoomGridNum;
		}
		w.m_w = 2;
		w.m_id = 100* w.m_cy + w.m_cx;
	    return w;
	}

}
