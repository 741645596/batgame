using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FloorM  {

	private static List<FloorData> m_lFooorData = new List<FloorData>(); 
	public static int g_ID = 0 ;
	public static List<FloorData>  GetFloorData()
	{
		return m_lFooorData ;
	}
	public static void  SetFloorData(ShipCanvasInfo Map)
	{
		m_lFooorData.Clear();
	    g_ID = 0 ;
		
		List<List<int>> grid = new List<List<int>>();
		List<List<int>> map = new List<List<int>>();
		
		grid = Map.GetShape();
		map = Map.GetMap();
		Int2 start = new Int2(-1, 0);
		for (int j = 0; j < grid.Count; j++)
		{
			for (int k = 0; k < grid[j].Count; k++)
			{
				if (grid[j][k] == 0)
				{
					if (j < (grid.Count - 1))
					{
						if (grid[j + 1][k] == 1)
						{
							FloorData fd = new FloorData();
							fd.SetFloorID();
							fd.m_FloorPos = new Int2(k * MapGrid.m_UnitRoomGridNum,j + 1);
							fd.m_FloorType = FloorType.bottom;
							m_lFooorData.Add(fd);
						}
					}
					if (k < (grid[j].Count - 1))
					{
						if (grid[j][k + 1] == 1)
						{
							FloorData fd = new FloorData();
							fd.SetFloorID();
							fd.m_FloorPos = new Int2((k+1) * MapGrid.m_UnitRoomGridNum,j);
							fd.m_FloorType = FloorType.left;
							m_lFooorData.Add(fd);
						}
					}
					
				}
				else if (grid[j][k] == 1)
				{
					
					if (j == 0)
					{
						if (start.Unit < 0)
							start.Unit = k;
						FloorData fd = new FloorData();
						fd.SetFloorID();
						fd.m_FloorPos = new Int2(k * MapGrid.m_UnitRoomGridNum,0);
						fd.m_FloorType = FloorType.bottom;
						m_lFooorData.Add(fd);
					}
					if (j == (grid.Count - 1))
					{
						//if (map[j + 1][k] == 1)
						FloorData fd = new FloorData();
						fd.SetFloorID();
						fd.m_FloorPos = new Int2(k * MapGrid.m_UnitRoomGridNum,j+1);
						fd.m_FloorType = FloorType.top;
						m_lFooorData.Add(fd);
					}
					else if (j < (grid.Count - 1))
					{
						if (grid[j + 1][k] == 1)
						{
							if (map[j + 1][k] == 1)
							{
								
								FloorData fd = new FloorData();
								fd.SetFloorID();
								fd.m_FloorPos = new Int2(k * MapGrid.m_UnitRoomGridNum,j+1);
								fd.m_FloorType = FloorType.Normal;
								m_lFooorData.Add(fd);
							}
						}
						else
						{
							
							FloorData fd = new FloorData();
							fd.SetFloorID();
							fd.m_FloorPos = new Int2(k * MapGrid.m_UnitRoomGridNum,j+1);
							fd.m_FloorType = FloorType.top;
							m_lFooorData.Add(fd);
						}
					}

					if (k == 0)
					{
						
						FloorData fd = new FloorData();
						fd.SetFloorID();
						fd.m_FloorPos = new Int2(0,j);
						fd.m_FloorType = FloorType.left;
						m_lFooorData.Add(fd);
					}
					if (k == (grid[j].Count - 1))
					{
						FloorData fd = new FloorData();
						fd.SetFloorID();
						fd.m_FloorPos = new Int2((k+1) * MapGrid.m_UnitRoomGridNum-1,j);
						fd.m_FloorType = FloorType.right;
						m_lFooorData.Add(fd);
					}
					else if (k < (grid[j].Count - 1))
					{
						if (grid[j][k + 1] == 0)
						{
							
							FloorData fd = new FloorData();
							fd.SetFloorID();
							fd.m_FloorPos = new Int2((k+1) * MapGrid.m_UnitRoomGridNum-1,j);
							fd.m_FloorType = FloorType.right;
							m_lFooorData.Add(fd);
						}
					}
				}
				else
				{
					#if UNITY_EDITOR_LOG
					Debug.Log("数据有问题");
					#endif
				}
			}
		}
	}
}


public class FloorData
{
	public int m_FloorID = 0;
	public Int2 m_FloorPos ;
	public FloorType m_FloorType;
	public FloorData(){}
	public void SetFloorID()
	{
		m_FloorID = FloorM.g_ID ;
		FloorM.g_ID ++ ;
	}	
}