using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using DG.Tweening;
public class MapStart : MonoBehaviour {


	public Transform EndLeft;
	public Transform EndRight;

	void OnEnable() {
		BattleEnvironmentM.JoinBoatData(this);
		TreasureScene.JoinBoatData(this);
	}
	
	void OnDisable() {
		BattleEnvironmentM.ExitBoatData();
	}


	public void CalcPositition(Int2 Size ,Vector3 CenterPos ,bool bSetimmediately)
	{
		if (bSetimmediately)
						transform.localPosition = new Vector3 (- Size.Unit * MapGrid.m_UnitRoomGridNum / 2 * MapGrid.m_width, 3f, 0);
		else {
			Vector3 newPosition = new Vector3 (- Size.Unit *  MapGrid.m_UnitRoomGridNum / 2 * MapGrid.m_width, 3f, 0);
			gameObject.transform.DOMove(newPosition, 0.3f);
		}
	}

	void OnDrawGizmos(){
		if(BattleEnvironmentM.GetBattleEnvironmentMode() == BattleEnvironmentMode.Edit)
		{
			List<RoomGrid> list = new List<RoomGrid>();
			list = RoomMap.GetAllRoomGrid();
			if(list.Count == 0 ) return ;
			foreach (RoomGrid p in list) 
			{
				p.DrawRoomGridInfo();
			}
		}
		else
		{
			List<MapGrid> lg = new List<MapGrid>();
			MapGrid.GetMapGridList (ref lg);
			foreach (MapGrid m in lg) 
			{
				DrawMapGrid(m ,m.GridPos.Unit);
			}
		}
		//GenerateShip.DrawRejectPolygon ();
	}
	
	
	void DrawMapGrid(MapGrid m ,int num)
	{
		if(m == null )
			return;
		MapGrid l = m.Left;
		MapGrid r = m.Right;
		MapGrid u = m.Up;
		MapGrid d = m.Down;
		
		if (m.Type == GridType.GRID_NORMAL) 
		{
			if(m.PropStations == StationsProp.ATTACK)
				Gizmos.color=Color.yellow;
			else Gizmos.color=Color.white;
			Gizmos.DrawSphere(m.WorldPos, 0.25f);
			
			
			Gizmos.color=Color.blue;
			if(l != null )
			{
				Gizmos.DrawLine(m.WorldPos,l.WorldPos);
			}
			if(r != null )
			{
				Gizmos.DrawLine(m.WorldPos,r.WorldPos);
			}
			if(u != null )
			{
				Gizmos.DrawLine(m.WorldPos,u.WorldPos);
			}
			if(d != null )
			{
				Gizmos.DrawLine(m.WorldPos,d.WorldPos);
			}
		} 
		else if (m.Type == GridType.GRID_STAIR) 
		{
			if(m.PropStations == StationsProp.ATTACK)
				Gizmos.color=Color.yellow;
			else Gizmos.color=Color.white;
			Gizmos.DrawSphere(m.WorldPos, 0.25f);
			
			Gizmos.color=Color.blue;
			if(l != null )
			{
				Gizmos.DrawLine(m.WorldPos,l.WorldPos);
			}
			if(r != null )
			{
				Gizmos.DrawLine(m.WorldPos,r.WorldPos);
			}
			if(u != null )
			{
				Gizmos.DrawLine(m.WorldPos,u.WorldPos);
			}
			if(d != null )
			{
				Gizmos.DrawLine(m.WorldPos,d.WorldPos);
			}
		}
		else if (m.Type == GridType.GRID_WALL) 
		{
			Gizmos.color=Color.grey;
			Gizmos.DrawSphere(m.WorldPos, 0.25f);
			Gizmos.color=Color.blue;
			if(l != null )
			{
				Gizmos.DrawLine(m.WorldPos,l.WorldPos);
			}
			if(r != null )
			{
				Gizmos.DrawLine(m.WorldPos,r.WorldPos);
			}
			if(u != null )
			{
				Gizmos.DrawLine(m.WorldPos,u.WorldPos);
			}
			if(d != null )
			{
				Gizmos.DrawLine(m.WorldPos,d.WorldPos);
			}
		} 
		else if (m.Type == GridType.GRID_HOLE) 
		{
			if(m.PropStations == StationsProp.ATTACK)
				Gizmos.color=Color.yellow;
			else Gizmos.color=Color.red;
			Gizmos.DrawSphere(m.WorldPos, 0.25f);
			
			Gizmos.color=Color.blue;
			if(l != null )
			{
				Gizmos.DrawLine(m.WorldPos,l.WorldPos);
			}
			if(r != null )
			{
				Gizmos.DrawLine(m.WorldPos,r.WorldPos);
			}
			if(u != null )
			{
				Gizmos.DrawLine(m.WorldPos,u.WorldPos);
			}
			if(d != null )
			{
				Gizmos.DrawLine(m.WorldPos,d.WorldPos);
			}
		}
		else if (m.Type == GridType.GRID_HOLESTAIR) 
		{
			if(m.PropStations == StationsProp.ATTACK)
				Gizmos.color=Color.yellow;
			else Gizmos.color=Color.green;
			Gizmos.DrawSphere(m.WorldPos, 0.20f);
			
			Gizmos.color=Color.blue;
			if(l != null )
			{
				Gizmos.DrawLine(m.WorldPos,l.WorldPos);
			}
			if(r != null )
			{
				Gizmos.DrawLine(m.WorldPos,r.WorldPos);
			}
			if(u != null )
			{
				Gizmos.DrawLine(m.WorldPos,u.WorldPos);
			}
			if(d != null )
			{
				Gizmos.DrawLine(m.WorldPos,d.WorldPos);
			}
		}
		else 
		{
			
		}
		DrawCombatMode(m,num);
	}
	
	
	
	
	
	void  DrawCombatMode(MapGrid m ,int num)
	{
		if(m == null ) return;
		
		List<string> Attacklist = new List<string>();
		List<string> Defenselist = new List<string>();
		m.HasRole (ref Attacklist ,ref Defenselist);
		
		if(Attacklist.Count > 0 && Defenselist.Count == 0)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere (m.WorldPos, 0.30f);
		}
		else if(Attacklist.Count == 0 && Defenselist.Count > 0)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere (m.WorldPos, 0.30f);
		}
		else if(Attacklist.Count > 0 && Defenselist.Count > 0)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere (m.WorldPos, 0.30f);
		}
		
		
		
		#if UNITY_EDITOR
		Vector3 pos = m.WorldPos;
		GUIStyle style = new GUIStyle();
		style.normal.textColor = Color.red;
		pos.y -=0.25f;
		Handles.Label(pos, num.ToString(),style);
		pos.y -= 0.5f;
		//
		if(Attacklist.Count > 0 || Defenselist.Count > 0)
		{
			if(DrawRole(Attacklist,pos,Color.blue) == true)
			{
				pos.y -= 0.25f;
			}
			DrawRole(Defenselist,pos,Color.red);
		}
		#endif
		
	}
	
	bool DrawRole(List<string> Text, Vector3 Pos,Color color)
	{
		if(Text == null || Text.Count == 0)
			return false;
		GUIStyle style = new GUIStyle();
		style.normal.textColor = color;
		style.fontSize = 16;
		string str = "{" + Text.Count +",";
		foreach(string s in Text)
		{
			Pos.y -= 0.5f;	
			#if UNITY_EDITOR
			Handles.Label(Pos, s,style);
			#endif
			//Pos.x -= 0.5f;
			//str += s+",";
		}
		str += "}";
		#if UNITY_EDITOR
		//Handles.Label(Pos, str,style);
		#endif
		return true;
	}
	

	public Vector3 GetDolphineLeftPoint()
	{
		int height = MapSize.GetMapLayer();
		int width = MapSize.GetLayerSize(0) /MapGrid.m_UnitRoomGridNum;
		int start = 1;
		for (int i = (height - 1); i >= 0; i--)
		{
			for(int j = 0; j < width; j ++)
			{
				MapGrid g = MapGrid.GetMG(i,j * MapGrid.m_UnitRoomGridNum +start);
				if (g!= null && g.Type != GridType.GRID_HOLE && g.Type != GridType.GRID_HOLESTAIR)
				{
					return g.WorldPos;
				}
			}
		}
		return Vector3.zero;
	}
	public Vector3 GetDolphineRightPoint()
	{
		int height = MapSize.GetMapLayer();
		int width = MapSize.GetLayerSize(0)/MapGrid.m_UnitRoomGridNum;
		int start = 4;
		for (int i = (height - 1); i >= 0; i--)
		{
			for(int j = (width -1); j >= 0; j --)
			{
				MapGrid g = MapGrid.GetMG(i,j * MapGrid.m_UnitRoomGridNum +start);
				if (g!= null && g.Type != GridType.GRID_HOLE && g.Type != GridType.GRID_HOLESTAIR)
				{
					return g.WorldPos;
				}
			}
		}
		return Vector3.zero;
	}
}
