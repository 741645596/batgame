using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Accessories : MonoBehaviour {
	
	List<Building> m_rooms = new List<Building>();
	//大格
	public Int2 start;
	public Int2 end;
	// Use this for initialization
	void Start () {
	}
	public void Init(Int2 s,Int2 e)
	{
		start = s;
		end = e;
		for(int i = start.Unit; i <= end.Unit; i++ )
		{
			
			MapGrid m = MapGrid.GetMG(start.Layer, i * MapGrid.m_UnitRoomGridNum + 3);
			if(m != null)
			{
				int BuildRoomSceneID =  -1;
				if(m.GetBuildRoom (ref BuildRoomSceneID) == true)
				{
					Life buildRoom = CM.GetLifeM(BuildRoomSceneID ,LifeMType.BUILD);
					if(buildRoom != null) 
					{
						m_rooms.Add(buildRoom as Building);
						(buildRoom as Building).AddAccessories(this);
					}
				}
			}
		}

	}

	public void DestroyRoom(Building room)
	{
		if(m_rooms.Contains(room))
			m_rooms.Remove(room);
		if (m_rooms.Count == 0)
		{
			Destroy(gameObject);
		}
	}
}
