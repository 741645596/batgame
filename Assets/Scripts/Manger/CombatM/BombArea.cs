using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 爆炸区块
/// </summary>
public class BombArea  {
	private BombProcessType m_ProcessType = BombProcessType.Start;
	public BombProcessType ProcessType{
		get{return m_ProcessType;}
		set{m_ProcessType = value;}
	}
	//区块起爆点房间
	private List<BombPoint> m_start = new List<BombPoint>();
	//区块临近爆炸点
	private List<BombPoint> m_linkPoint = new List<BombPoint>();
	//中心包围圈，只有最后一次爆炸才有
	private List<List<BombPoint>> m_linkCenterPoint = new List<List<BombPoint>>();

	private float m_TimeCount = 0;
	private float m_CenterBornTimeCount = 0;
	private float m_GoldResTimeCount = 0;
	private float m_GoldResTimes = 0;
	private float m_dt1 = 0.0f;  //小爆到邻接爆炸间隔时间
	private float m_dt2 = 0.0f;  //邻接爆到中爆（大爆）间隔时间
	private float m_dt3 = 0.0f;  //邻接到中心的一次小爆炸时间 
	private int m_step = 0;


	public void SetBombAreaTime(float dt1, float dt2 ,float dt3)
	{
		m_step = 0;
		m_TimeCount = 0;
		m_dt1 = dt1;
		m_dt2 = dt2;
		m_dt3 = dt3;
		m_GoldResTimeCount = 0;
		m_CenterBornTimeCount = 0 ;
		PlayEffect(m_step);
		m_step ++;
	}
	/// <summary>
	/// 清空爆炸区块
	/// </summary>
	public void ClearBombArea()
	{
		m_start.Clear();
		m_linkPoint.Clear();
	}
	/// <summary>
	/// 设置爆炸区块
	/// </summary>
	/// <param name="StartPos">引导区</param>
	/// <param name="lLinkPos">邻接区</param>
	public void SetArea(Int2 StartPos, List<Int2> lLinkPos)
	{
		ClearBombArea();
		m_start.Add(new BombPoint(StartPos));
		if(lLinkPos != null && lLinkPos.Count > 0)
		{
			foreach(Int2 p in lLinkPos)
			{
				m_linkPoint.Add(new BombPoint(p));
			}
		}
	}


	/// <summary>
	/// 设置中心邻接区域
	/// </summary>
	/// <param name="lLinkCenterPos">中心邻接区域</param>
	public void SetCenterArea(List<List<Int2>> lLinkCenterPos)
	{
		m_linkCenterPoint.Clear();
		if(lLinkCenterPos != null && lLinkCenterPos.Count > 0)
		{
			foreach(List<Int2> lp in lLinkCenterPos)
			{
				if(lp != null && lp.Count > 0)
				{
					List<BombPoint> lBomp = new List<BombPoint>();
					foreach(Int2 p in lp)
					{
						lBomp.Add(new BombPoint(p));
					}
					m_linkCenterPoint.Add(lBomp);
				}
			}
		}
	}

	/// <summary>
	/// 设置爆炸区块
	/// </summary>
	/// <param name="lStartPos">引导区</param>
	/// <param name="lLinkPos">邻接区</param>
	public void SetArea(List<Int2> lStartPos, List<Int2> lLinkPos)
	{
		ClearBombArea();
		if(lStartPos != null && lStartPos.Count > 0)
		{
			foreach(Int2 p in lStartPos)
			{
				m_start.Add(new BombPoint(p));
			}
		}
		
		if(lLinkPos != null && lLinkPos.Count > 0)
		{
			foreach(Int2 p in lLinkPos)
			{
				m_linkPoint.Add(new BombPoint(p));
			}
		}
	}
	/// <summary>
	/// 添加爆炸区块（甲板房间）
	/// </summary>
	public void AddArea(List<Int2> lDeckPos ,Building room)
	{
		if(lDeckPos != null && lDeckPos.Count > 0)
		{
			foreach(Int2 p in lDeckPos)
			{
				m_start.Add(new BombPoint(p ,room));
			}
		}
	}
	
	/// <summary>
	/// 查找包含该房间的区块
	/// </summary>
	public List<BombPoint> FindAreaRoom(Int2  Pos)
	{
		List<BombPoint> l = new List<BombPoint>();
		//start
		foreach(BombPoint p in m_start)
		{
			if(p.m_BombPos == Pos)
				l.Add(p);
		}
		//link
		foreach(BombPoint p in m_linkPoint)
		{
			if(p.m_BombPos == Pos)
				l.Add(p);
		}
		//centerLink
		foreach(List<BombPoint> lp in m_linkCenterPoint)
		{
			foreach(BombPoint p in lp)
			{
				if(p.m_BombPos == Pos)
					l.Add(p);
			}
		}

		
		return l;
	}


	public void Update(float deltatime)
	{
		m_TimeCount += deltatime;
		if(m_step == 1)
		{
			if(m_TimeCount >= m_dt1 )
			{
				PlayEffect(m_step );
				m_TimeCount = 0;
				m_step ++;
			}
		}
		else if(m_step == 2)
		{
			if(m_TimeCount >= m_dt2 )
			{
				PlayEffect(m_step);
				m_TimeCount = 0;
				m_step ++;
			}
		}
		//播放爆炸小区域
		if(ProcessType == BombProcessType.End && m_step > 0)
		{
			m_CenterBornTimeCount += deltatime ;
			if(m_CenterBornTimeCount >= m_dt3 )
			{
				m_CenterBornTimeCount = 0;
				if(m_linkCenterPoint != null && m_linkCenterPoint.Count > 0)
				{
					PlayLinkCenter(m_linkCenterPoint[0]);
					m_linkCenterPoint.RemoveAt(0);
				}
			}
		}


		//金库资源
		if(ProcessType == BombProcessType.Start)
		{
			m_GoldResTimeCount += deltatime ;
			if(m_GoldResTimeCount > 0.001f)
			{
				m_GoldResTimeCount =0.0f;
				if(m_GoldResTimes < 2)
				{
					//PlayerGoldRes();
					m_GoldResTimes ++ ;
				}
			}
		}
	}

	//播放起爆点爆炸效果
	public void PlayEffect(int step)
	{
		if(step == 0)
			PlayStep1();
		else if(step == 1)
			PlayStep2();
		else if(step == 2)
		{
			if(ProcessType == BombProcessType.End)
			{
				PlayStep3("2000271_03" ,2.0f );
				SoundPlay.Play("bigboom",false,false);
			}
			else 
			{
				PlayStep3("2000271_02" ,2.0f );
				SoundPlay.Play("middleboom",false,false);
			}
		}
	}
	//播放起爆点爆炸效果
	private void PlayStep1()
	{
		foreach (BombPoint B in m_start)
		{
			if(B != null)
			{
				B.PlayEffect("2000271_01" ,2.0f);
			}

		}
	}
	
	//播放邻接点爆炸
	private void PlayStep2()
	{
		foreach (BombPoint B in m_linkPoint)
		{
			if(B != null)
			{
				B.PlayEffect("2000271_01" ,2.0f);
			}
		}
	}
	
	//播放区块中心爆炸效果
	private void PlayStep3(string effectName ,float deadtime  )
	{
		Vector3 pos = Vector3.zero;
		foreach (BombPoint B in m_start)
		{
			if(B != null)
				pos += B.GetWorldPos();
		}
		foreach (BombPoint B in m_linkPoint)
		{
			if(B != null)
				pos += B.GetWorldPos();
		}
		int total = m_start.Count + m_linkPoint.Count ;
		if(total != 0)
		{
			pos = pos /total;
			GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", effectName,
			                                                pos, BattleEnvironmentM.GetLifeMBornNode(true));
			if (gae != null)
			{
				GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(deadtime);
				gae.AddAction(ndEffect);
			}
		}
		//房间爆炸掉
		DestroyRoom();

		if(ProcessType == BombProcessType.End)
		{
			//if(m_linkCenterPoint == null || m_linkCenterPoint.Count == 0)
			{
				List<Life> l = new List<Life>();
				CM.SearchLifeMListInBoat(ref l, LifeMType.BUILD ,LifeMCamp.DEFENSE);
				foreach(Life b in l)
				{
					if(b == null) continue ;
					if(b is Building  && (b as Building).m_roomType == RoomType.NormalTrap)
					{
						(b as Building).KillSelf(0);
					}
				}
			}
		}
	}

	//播放邻接点爆炸
	private void PlayLinkCenter(List<BombPoint> lCenter)
	{
		if(lCenter == null || lCenter.Count == 0)
			return ;
		foreach (BombPoint B in lCenter)
		{
			if(B != null )
			{
				B.PlayTrapEffect("2000271_01" ,2.0f);
				B.DestroyRoom();
			}
		}
	}


	//房间爆炸，变暗
	private void DestroyRoom()
	{
		foreach (BombPoint B in m_start)
		{
			if(B != null)
			{
				B.DestroyRoom();
			}
		}
		foreach (BombPoint B in m_linkPoint)
		{
			if(B != null)
			{
				B.DestroyRoom();
			}
		}
	}
	
	/// <summary>
	/// 喷金库资源
	/// </summary>
	private void PlayerGoldRes()
	{
		foreach (BombPoint B in m_start)
		{
			if(B != null)
			{
				B.PlayEffect("2000651" ,3.0f);
			}
		}
	}
}
/// <summary>
/// 爆炸点
/// </summary>
public class BombPoint  {
	public BombPoint(){}
	public BombPoint(Int2 Pos)
	{
		m_BombPos = Pos ;
		m_building = null;
	}
	public BombPoint(Int2 Pos, Building room)
	{
		m_BombPos = Pos ;
		m_building = room;
	}
	
	public void SetBuild(Building room )
	{
		m_building = room;
	}
	
	//爆炸格子,room grid 大房间格子
	public Int2 m_BombPos;
	//爆炸的房间
	public Building m_building;

	/// <summary>
	/// 播放特效
	/// </summary>
	public void PlayEffect(string effectName ,float deadtime)
	{
		Vector3 Pos = GetWorldPos() ;
		if(effectName == "2000651")
		{
			Pos.z -= 10; 			
			Pos.x -= MapGrid.m_width / 2 ;
			Pos.y  = Pos.y - MapGrid.m_heigth / 2 + 0.3f;
		}
		else 
		{
			Pos.z -= 1; 	
		}
			
		GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", effectName,
		                                                Pos, BattleEnvironmentM.GetLifeMBornNode(true));
		if (gae != null)
		{
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(deadtime);
			gae.AddAction(ndEffect);
		}
	}


	/// <summary>
	/// 播放特效
	/// </summary>
	public void PlayTrapEffect(string effectName ,float deadtime)
	{
		if(m_building == null ) return ;
		if(m_building.m_roomType == RoomType.ResRoom)
			return ;
		Vector3 Pos = GetWorldPos() ;
		if(effectName == "2000651")
		{
			Pos.z -= 10; 			
			Pos.x -= MapGrid.m_width / 2 ;
			Pos.y  = Pos.y - MapGrid.m_heigth / 2 + 0.3f;
		}
		else 
		{
			Pos.z -= 1; 	
		}
		
		GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", effectName,
		                                                Pos, BattleEnvironmentM.GetLifeMBornNode(true));
		if (gae != null)
		{
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(deadtime);
			gae.AddAction(ndEffect);
		}
	}


	public Vector3 GetWorldPos()
	{
		Vector3 Pos = Vector3.zero ;
		MapGrid mg = MapGrid.GetMG(new Int2 (m_BombPos.Unit * MapGrid.m_UnitRoomGridNum + 3 ,m_BombPos.Layer));
		if(mg != null)
		{
			Pos = mg.WorldPos;
			Pos.y += MapGrid.m_heigth /2;
			return Pos;
		}
		return Vector3.zero;
	}
	/// <summary>
	/// 房间爆炸
	/// </summary>
	public void DestroyRoom()
	{
		if(m_building != null )
		{ 
			//金库延时爆掉
			if(m_building.m_roomType == RoomType.ResRoom && m_building.m_thisT != null)
			{
				GameObjectActionExcute gae =m_building.m_thisT.gameObject.AddComponent<GameObjectActionExcute>();
				if(gae != null )
				{
					GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(4.0f);
					gae.AddAction(ndEffect);
				}
			}
			else if(m_building.isDead == false)
			{
				m_building.KillSelf(0);
				m_building = null;
			}
		}
	}
}