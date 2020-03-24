using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireAI  {
	public static Vector3 GetPos(int layer,int unit)
	{
		return new Vector3(unit * MapGrid.m_width,layer*MapGrid.m_heigth,0);
	}

	/// <summary>
	/// 获取飞行轨迹，及出生点
	/// </summary>
	/// <param name="fireai">飞行AI</param>
	/// <param name="FlyLine">返回飞行路线</param>
	/// <param name="BornPos">返回出生点</param>
	public static void GetFlyBorn(int  fireai,ref List<Vector3>FlyLine ,ref Vector3 BornPos)
	{
		Vector3 end = Vector3.zero;
		LocationType t = LocationType.Top;
		Life deck = FireAI.GetHitDeck(fireai ,ref end,ref t);

		CalcFlyData(deck , end , t , ref FlyLine , ref BornPos);
	}


	private static void CalcFlyData(Life deck ,Vector3 end , LocationType t ,ref List<Vector3>FlyLine ,ref Vector3 BornPos)
	{
		float height = 4.0f ;
		float width = 2.5f ; 
		if(FlyLine == null)
			FlyLine = new List<Vector3>();
		FlyLine.Clear();

		if (t == LocationType.Top)
		{
			if (deck != null) end = deck.GetMapGrid().pos;
			end.x += 1.5f;
			Vector3 start = end;
			start.y += height * 3;
			Vector3 Pos = BattleEnvironmentM.Local2WorldPos(start);
			FlyLine.Add(Pos);  
			Pos = BattleEnvironmentM.Local2WorldPos(new Vector3(end.x,end.y + height * 2,end.z));
			FlyLine.Add(Pos);
			Pos = BattleEnvironmentM.Local2WorldPos(new Vector3(end.x,end.y + height,end.z));
			FlyLine.Add(Pos);
			Pos = BattleEnvironmentM.Local2WorldPos(end);
			FlyLine.Add(Pos);
			BornPos = Pos;
		}
		else if (t == LocationType.Left)
		{
			if (deck != null) end = deck.GetMapGrid().pos;
			end.y += 1.5f;
			Vector3 start = end;
			start.x -= width * 3;
			Vector3 Pos = BattleEnvironmentM.Local2WorldPos(start);
			FlyLine.Add(Pos);  
			Pos = BattleEnvironmentM.Local2WorldPos(new Vector3(end.x -width * 2,end.y ,end.z));
			FlyLine.Add(Pos);
			Pos = BattleEnvironmentM.Local2WorldPos(new Vector3(end.x - width,end.y ,end.z));
			FlyLine.Add(Pos);
			Pos = BattleEnvironmentM.Local2WorldPos(end);
			FlyLine.Add(Pos);
			BornPos = Pos;
		}
		else if (t == LocationType.Right)
		{
			if (deck != null)
				end = deck.GetMapGrid().pos;
			end.y += 1.5f;
			Vector3 start = end;
			start.x += width * 3;

			Vector3 Pos = BattleEnvironmentM.Local2WorldPos(start);
			FlyLine.Add(Pos);  
			Pos = BattleEnvironmentM.Local2WorldPos(new Vector3(end.x  + width,end.y ,end.z));
			FlyLine.Add(Pos);
			Pos = BattleEnvironmentM.Local2WorldPos(new Vector3(end.x + width,end.y ,end.z));
			FlyLine.Add(Pos);
			Pos = BattleEnvironmentM.Local2WorldPos(end);
			FlyLine.Add(Pos);
			BornPos = Pos;
		}
	}
	

	private static Life GetHitDeck(int  fireai,ref Vector3 Firepos,ref LocationType t)
	{
		GenerateShip.CalcHitDeck();
		t = LocationType.Top;
		if  (GenerateShip.LHitDeck.Count  <= 0 )
			return null;
		if (fireai == 5)
		{
			Life l = null;
			MapGrid g = CM.GoldBuild.GetMapGrid();
			Vector3 pos = Vector3.zero;
			float mindistant = int.MaxValue;
			Firepos = Vector3.zero;
			for(int i = 1; i < GenerateShip.LHitDeck.Count; i++)
			{
				if (GenerateShip.LHitDeck[i].m_type != LocationType.Top)
				{
					pos = GetPos(GenerateShip.LHitDeck[i].m_layer,GenerateShip.LHitDeck[i].m_unit);
					float distant =  Vector3.Distance(g.pos,pos);
					if (mindistant > distant)
					{
						mindistant = distant;
						Firepos = pos;
						t = GenerateShip.LHitDeck[i].m_type;
						l = GenerateShip.LHitDeck[i].m_target;
					}
				}
			}
			for(int i = 0; i < GenerateShip.LHitBrokeDeck.Count; i++)
			{
				if (GenerateShip.LHitDeck[i].m_type != LocationType.Top)
				{
					pos = GetPos(GenerateShip.LHitBrokeDeck[i].m_layer,GenerateShip.LHitBrokeDeck[i].m_unit);
					float distant =  Vector3.Distance(g.pos,pos);
					if (mindistant > distant)
					{
						mindistant = distant;
						Firepos = pos;
						t = GenerateShip.LHitBrokeDeck[i].m_type;
						l = GenerateShip.LHitBrokeDeck[i].m_target;
					}
				}
			}
			return null;
		}
		if (fireai == 4)
		{
			Firepos = new Vector3(0,0,0);
			t= LocationType.Left;
			return null;
			/*
			List<HitDeckInfo> lVertical = new List<HitDeckInfo>();
			List<HitDeckInfo> ldeck = new List<HitDeckInfo>();

			foreach(HitDeckInfo k in GenerateShip.LHitBrokeDeck)
			{
				if (k.m_type == LocationType.Left || k.m_type == LocationType.Right)
					lVertical.Add(k);
				else ldeck.Add(k);
			}
			if (lVertical.Count > 0)
			{
				MapGrid g = CM.GoldBuild.GetMapGrid();
				Vector3 pos = GetPos(lVertical[0].m_layer,lVertical[0].m_unit);
				float mindistant = Vector3.Distance(g.pos,pos);
				Firepos = pos;
				t = lVertical[0].m_type;
				for(int i = 1; i < lVertical.Count; i++)
				{
					pos = GetPos(lVertical[i].m_layer,lVertical[i].m_unit);
					float distant =  Vector3.Distance(g.pos,pos);
					if (mindistant > distant)
					{
						mindistant = distant;
						Firepos = pos;
						t = lVertical[i].m_type;
					}
				}
				return null;
			}
			else
			{
				if (ldeck.Count > 0)
				{
					MapGrid g = CM.GoldBuild.GetMapGrid();
					Vector3 pos = GetPos(ldeck[0].m_layer,ldeck[0].m_unit);
					float mindistant = Vector3.Distance(g.pos,pos);
					Firepos = pos;
					t = ldeck[0].m_type;
					for(int i = 1; i < ldeck.Count; i++)
					{
						pos = GetPos(ldeck[i].m_layer,ldeck[i].m_unit);
						float distant =  Vector3.Distance(g.pos,pos);
						if (mindistant > distant)
						{
							mindistant = distant;
							Firepos = pos;
							t = ldeck[i].m_type;
						}
					}
					return null;
				}
				else
					fireai = 2;
			}*/
		}
		if (fireai == 3)
		{
			List<Building> ltrap = new List<Building>();
			List<Life> lb = new List<Life>();
			List<Life> ld = new List<Life>();
			CM.SearchLifeMListInBoat(ref lb,LifeMType.BUILD, LifeMCamp.DEFENSE);
			foreach(Building b in lb)
			{
				BuildInfo info = CmCarbon.GetBuildInfo(b.m_Core.m_DataID);
				if (info.m_RoomType == RoomType.NormalTrap)
				{
					for (int i = 1; i < GenerateShip.LHitDeck.Count; i++)
					{
						if(GenerateShip.LHitDeck[i].m_type == LocationType.Top && GenerateShip.LHitDeck[i].m_layer == (info.m_cy+1) && GenerateShip.LHitDeck[i].m_unit == info.m_cx)
						{
							ltrap.Add(b);
							ld.Add(GenerateShip.LHitDeck[i].m_target);
						}
					}
				}
			}
			if (ltrap.Count > 0)
			{
				Life l = ld[0];
				int minhp = ltrap[0].m_Attr.Hp;
				for(int i = 1; i < ltrap.Count; i++)
				{
					if (minhp > ltrap[i].m_Attr.Hp)
					{
						minhp = ltrap[i].m_Attr.Hp;
						l = ltrap[i];
					}
				}
				return l;
			}
			else
			{
				fireai = 1;
			}
		}
		if (fireai == 2)
		{
			bool allsame = true;
			Life l = GenerateShip.LHitDeck[0].m_target;
			t = GenerateShip.LHitDeck[0].m_type;
			int minhp = GenerateShip.LHitDeck[0].m_target.m_Attr.Hp;
			for (int i = 1; i < GenerateShip.LHitDeck.Count; i++)
			{
				if (minhp != GenerateShip.LHitDeck[i].m_target.m_Attr.Hp)
					allsame = false;
				if (minhp > GenerateShip.LHitDeck[i].m_target.m_Attr.Hp)
				{
					minhp = GenerateShip.LHitDeck[i].m_target.m_Attr.Hp;
					l = GenerateShip.LHitDeck[i].m_target;
					t = GenerateShip.LHitDeck[i].m_type;
				}
			}
			if (allsame)
				fireai = 1;
			else
			{
				return l;
			}
		}
		if (fireai == 1)
		{
			while(true)
			{
				int i = Random.Range(0,GenerateShip.LHitDeck.Count);
				if (GenerateShip.LHitDeck[i].m_type == LocationType.Top)
					return GenerateShip.LHitDeck[i].m_target;
			}
		}
		return null;
	}
}
