using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// 战斗副本场景数据管理
/// </summary>
/// <author>zhulin</author>
public class SceneSoldier{ 
	
	public Life m_Life;       //对应着场景中的对象
	public LifeMCore m_Core;
	public int m_SceneID;      //有唯一性。

	public SceneSoldier(int SceneID,Life Life ,LifeMCore Core)
	{
		m_SceneID = SceneID;
		m_Life = Life;
		m_Core = new LifeMCore(Core);
	}
};

/*
指定地图搜索方式
*/
public enum MapSearchStlye
{
	AllWorld   = 0,  //整个世界范围
	InBoat     = 1,  //在船上
	SameLayer  = 2,  //同层进行搜索
	Circle     = 3,  //圆形范围进行搜索
	Ball       = 4,  //球形范围进行搜索
}



/*
搜索方式
*/
public enum SearchStlye
{
	Target       = 1,  //目标搜索  不支持
	Line         = 2,  //线性搜索
	Sector       = 3,  //扇形搜索
	Rectangular  =4,  //矩形搜索
}

public class CM  {
	private static List<SceneSoldier> m_SoldierList= new List<SceneSoldier>();
	private static List<SceneSoldier> m_PetList= new List<SceneSoldier>();
	private static List<SceneSoldier> m_SummonPetList= new List<SceneSoldier>();
	private static List<SceneSoldier> m_BuildList= new List<SceneSoldier>();
	private static List<SceneSoldier> m_WallList= new List<SceneSoldier>();
	private static List<SceneSoldier> m_FoorList= new List<SceneSoldier>();
	private static List<SceneSoldier> m_InheritSummonList= new List<SceneSoldier>();

	public static Life GoldBuild;
	//加入战斗
	public static void JoinCombat(int SceneID,Life m,LifeMCore Core)
	{
		if(m == null)
		{
			NGUIUtil.DebugLog("life is null do not join");
			return ;
		}
		List<SceneSoldier> l = GetSceneSoldierList(Core.m_type);
		if(l != null)
		{
			bool IsHave = false;
			for(int i =0; i < l.Count; i++)
			{
				if(SceneID == l[i].m_SceneID  )
				{
					if(Core.m_MoveState != l[i].m_Core.m_MoveState)
					{
						l [i].m_Life = m;
						l [i].m_Core.Copy(Core);
					}
					IsHave = true;
				}
			}
			if(IsHave == false)
			{
				l.Add(new SceneSoldier(SceneID,m ,Core));
			}
		}
	}

    // 退出战斗，死亡，或被毁灭时。
	public static void ExitCombat(LifeMCore Core,int SceneID)
	{
		List<SceneSoldier> l = GetSceneSoldierList(Core.m_type);
		if(l != null)
		{
			SceneSoldier s = GetSceneSoldier(l,SceneID);
			if(s != null)
			{
				l.Remove(s);
				if(Core.m_type == LifeMType.SOLDIER)
				{
					if (Core.m_Camp == LifeMCamp.ATTACK)
					{
						CmCarbon.DeadAttackSoldier();
					}
				}
				else if(Core.m_type == LifeMType.BUILD)
				{
				}
			}
		}
	}
	
	public static void ExitCm()
	{
		EventCenter.AntiAllRegisterHooks();
		MapM.ClearMapLife();
		m_SoldierList.Clear ();
		m_PetList.Clear ();
		m_BuildList.Clear ();
		m_WallList.Clear ();
		m_FoorList.Clear();
		m_SummonPetList.Clear();
	}

	private static void DestroySceneLife(List<SceneSoldier> l)
	{
		if(l != null)
		{
			foreach (SceneSoldier m in l) 
			{
				if(m != null)
				{
					if(m.m_Life != null)
					{
						m.m_Life.Dead();
					}
				}
			}
			l.Clear ();
		}
	}

	public static void DestroyAllLife()
	{

		List<SceneSoldier> l = GetSceneSoldierList(LifeMType.SOLDIER);
		DestroySceneLife(l);

		l = GetSceneSoldierList(LifeMType.BUILD);
		DestroySceneLife(l);

		l = GetSceneSoldierList(LifeMType.PET);
		DestroySceneLife(l);

		l = GetSceneSoldierList(LifeMType.SUMMONPET);
		DestroySceneLife(l);

		l = GetSceneSoldierList(LifeMType.WALL);
		DestroySceneLife(l);
		l = GetSceneSoldierList(LifeMType.INHERITSUMMONPROS);
		DestroySceneLife(l);

	}

	/// <summary>
	/// 按类型取得lifeM列表，不支持组合类型
	/// </summary>
	private static List<SceneSoldier> GetSceneSoldierList(LifeMType Type)
	{
		if(Type == LifeMType.SOLDIER)
		{
			return m_SoldierList;
		}
		else if(Type == LifeMType.PET)
		{
			return m_PetList;
		}
		else if(Type == LifeMType.BUILD)
		{
			return m_BuildList;
		}
		else if(Type == LifeMType.WALL)
		{
			return m_WallList;
		}
		else if(Type == LifeMType.FLOOR)
		{
			return m_FoorList;
		}
		else if(Type == LifeMType.SUMMONPET)
		{
			return m_SummonPetList;
		}
		else if (Type == LifeMType.INHERITSUMMONPROS)
		{
			return m_InheritSummonList;
		}
		return null;
	}
	

	/// <summary>
	/// 在列表中，按SceneID进行匹配得到Scene Lifem信息
	/// </summary>
	private static SceneSoldier GetSceneSoldier(List<SceneSoldier> l,int SceneID )
	{
		if(l == null) return null;
		for(int i =0; i < l.Count; i++)
		{
			if(SceneID == l[i].m_SceneID)
			{
				return l[i];
			}
		}
		return null;
	}

	/// <summary>
	/// 根据scene 及Type 取得LifeM对象, 不支持混合类型
	/// </summary>
	public static Life GetLifeM(int SceneID ,LifeMType Type )
	{
		if(SceneID < 0) return null;
		List<SceneSoldier> l = GetSceneSoldierList( Type );
		SceneSoldier s = GetSceneSoldier (l,SceneID);
		if(s != null)
		{
			if(s.m_Life == null)
				NGUIUtil.DebugLog("s.m_Life is null" + SceneID +"," + Type);
			return s.m_Life;
		}
		return null;
	}

	/// <summary>
	/// 根据scene 及Type 取得LifeM对象, 支持混合类型
	/// </summary>
	public static Life GetAllLifeM(int SceneID ,LifeMType Type )
	{
		if(SceneID < 0) return null;
		List<SceneSoldier> l = GetLifeMList( Type );
		SceneSoldier s = GetSceneSoldier (l,SceneID);
		if(s != null)
		{
			if(s.m_Life == null)
				NGUIUtil.DebugLog("s.m_Life is null" + SceneID +"," + Type);
			return s.m_Life;
		}
		return null;
	}

	/// <summary>
	/// 按类型取得lifeM列表,支持组合类型
	/// </summary>
	private static List<SceneSoldier> GetLifeMList(LifeMType Type )
	{
		List<SceneSoldier> l = new List<SceneSoldier>();
		if ( (Type & LifeMType.SOLDIER) != 0 )
		{
			l.AddRange(m_SoldierList);
		}
		if ( (Type & LifeMType.BUILD) != 0 )
		{
			l.AddRange(m_BuildList);
		}
		if ( (Type & LifeMType.PET) != 0 )
		{
			l.AddRange(m_PetList);
		}
		if ( (Type & LifeMType.SUMMONPET) != 0 )
		{
			l.AddRange(m_SummonPetList);
		}
		if ( (Type & LifeMType.WALL) != 0 )
		{
			l.AddRange(m_WallList);
		}
		if ( (Type & LifeMType.FLOOR) != 0 )
		{
			l.AddRange(m_FoorList);
		}
		if ( (Type & LifeMType.INHERITSUMMONPROS) != 0 )
		{
			l.AddRange(m_InheritSummonList);
		}
		return l;
	}

	/// <summary>
	/// 判断战斗中包含某类对象，不支持组合对象
	/// </summary>
	public static bool HaveIntanseInScene(LifeMType Type , LifeMCamp Camp ,int Data)
	{

		List<SceneSoldier> l = GetSceneSoldierList(Type);
		if(l != null)
		{
			foreach ( SceneSoldier  s in l) 
			{
				LifeMCore Core = s.m_Core;
				if(Core.m_type == Type 
				   && Core.m_Camp == Camp 
				   && Core.m_DataID == Data)
				{
					return true;
				}
			}
		}
		return false;
	}

	/// <summary>
	/// 按条件搜索lifeM
	/// </summary>
	/// <param name="HaveList">不为空，则在这个列表中进行搜索，为空在所有CM管理的列表中进行搜索</param>
	/// <param name="Type">lifem 类型，支持位组合</param>
	/// <param name="Camp">按指定的阵营，支持组合</param>
	/// <param name="Target">参考目标,同层，圆形，球形搜索方式需要</param>
	/// <param name="Radius">搜索半径，圆形，球形需要</param>
	/// <returns>返回搜索列表list</returns>
	public static void SearchLifeMList(ref List<Life> list,
	                                   List<Life>HaveList,
	                                   LifeMType Type ,
	                                   LifeMCamp Camp,
	                                   MapSearchStlye SearchStype,
	                                   Life Self ,
	                                   float Radius)
	{
		if (list == null)
			list = new List<Life> ();
		else list.Clear ();
		//cm 中获取列表
		if(HaveList == null || HaveList.Count == 0)
		{
			List<SceneSoldier> l = GetLifeMList(Type);
			if(l != null)
			{
				foreach ( SceneSoldier  s in l) 
				{
					if(s == null) continue;
					LifeMCore Core = s.m_Core;
					Life life = s.m_Life;
					if(CheckCoreConditions(Core,Camp) == false)
						continue;
					if(CheckMapConditions(SearchStype,Self,life,Radius) == false)
						continue;
					list.Add(life);
				}
			}
		}
		else //have list 中进行过滤
		{
			foreach ( Life life in HaveList) 
			{
				if(life == null) continue ;
				LifeMCore Core = life.m_Core;
				if(CheckCoreConditions(Core,Type,Camp) == false)
					continue;
				if(CheckMapConditions(SearchStype,Self,life,Radius) == false)
					continue;
				list.Add(life);
			}
		}
	}	



	/// <summary>
	/// 按条件搜索lifeM
	/// </summary>
	/// <param name="HaveList">不为空，则在这个列表中进行搜索，为空在所有CM管理的列表中进行搜索</param>
	/// <param name="Type">lifem 类型，支持位组合</param>
	/// <param name="Camp">按指定的阵营，支持组合</param>
	/// <param name="Target">参考目标,同层，圆形，球形搜索方式需要</param>
	/// <param name="SearchInfo">搜索参数</param>
	/// <returns>返回搜索列表list</returns>
	public static void SearchLifeMList(ref List<Life> list,
	                                   List<Life>HaveList,
	                                   LifeMType Type ,
	                                   LifeMCamp Camp,
	                                   Life Self ,
	                                   SearchTarageInfo SearchInfo)
	{
		if (list == null)
			list = new List<Life> ();
		else list.Clear ();
		//cm 中获取列表
		if(HaveList == null || HaveList.Count == 0)
		{
			List<SceneSoldier> l = GetLifeMList(Type);
			if(l != null)
			{
				foreach ( SceneSoldier  s in l) 
				{
					if(s == null) continue;
					LifeMCore Core = s.m_Core;
					Life life = s.m_Life;
					if(CheckCoreConditions(Core,Camp) == false)
						continue;
					if(CheckMapConditions(SearchInfo,Self,life) == false)
						continue;
					list.Add(life);
				}
			}
		}
		else //have list 中进行过滤
		{
			foreach ( Life life in HaveList) 
			{
				if(life == null) continue ;
				LifeMCore Core = life.m_Core;
				if(CheckCoreConditions(Core,Type,Camp) == false)
					continue;
				if(CheckMapConditions(SearchInfo,Self,life) == false)
					continue;
				list.Add(life);
			}
		}
	}
	/// <summary>
	/// 满足地图空间条件判断
	/// </summary>
	/// <param name="SearchInfo">搜索条件</param>
	/// <param name="Self">目标对象</param>
	/// <param name="Target">对象</param>
	/// <returns>true 满足条件，false 不满足条件</returns>
	private static bool CheckMapConditions(SearchTarageInfo SearchInfo,Life Self,Life Target)
	{
		if(Target == null ) return false;
		if(Self == null ) return false;
		if(SearchInfo == null ) return false;

		return SearchInfo.CheckConditions(Self ,Target);
	}







	/// <summary>
	/// 满足地图空间条件判断
	/// </summary>
	/// <param name="HaveList">不为空，则在这个列表中进行搜索，为空在所有CM管理的列表中进行搜索</param>
	/// <param name="lType">按指定包含的类型进行搜索</param>
	/// <param name="lCamp">按指定的阵营</param>
	/// <returns>true 满足条件，false 不满足条件</returns>
	private static bool CheckMapConditions(MapSearchStlye SearchStype,Life Self,Life Target,float Radius)
	{
		if(Target == null ) return false;
		if(SearchStype == MapSearchStlye.AllWorld)
			return true;
		else if(SearchStype == MapSearchStlye.InBoat)
		{
			return Target.InBoat;
		}
		else if(SearchStype == MapSearchStlye.SameLayer)
		{
			return Life.InSameLayer(Self,Target);
		}
		else if(SearchStype == MapSearchStlye.Circle)
		{
			return Life.IsInCircle(Self,Target,Radius);
		}
		else if(SearchStype == MapSearchStlye.Ball)
		{
			return Life.IsInBall(Self,Target,Radius);
		}
		else return false;
	}


	/// <summary>
	/// 满足地图空间条件判断
	/// </summary>
	/// <param name="SearchStype">搜索类型</param>
	/// <param name="Target">按指定包含的类型进行搜索</param>
	/// <param name="l">按指定的阵营</param>
	/// <param name="Radius">半径</param>
	/// <returns>true 满足条件，false 不满足条件</returns>
	private static bool CheckMapConditions(MapSearchStlye SearchStype,MapGrid Self,Life Target,float Radius)
	{
		if(Target == null ) return false;
		if(SearchStype == MapSearchStlye.AllWorld)
			return true;
		else if(SearchStype == MapSearchStlye.InBoat)
		{
			return Target.InBoat;
		}
		else if(SearchStype == MapSearchStlye.SameLayer)
		{
			return Life.InSameLayer(Self,Target);
		}
		else if(SearchStype == MapSearchStlye.Circle)
		{
			return Life.IsInCircle(Self,Target,Radius);
		}
		else if(SearchStype == MapSearchStlye.Ball)
		{
			return Life.IsInBall(Self,Target,Radius);
		}
		else return false;
	}


	/// <summary>
	/// 满足Core条件判断
	/// </summary>
	/// <param name="Core">不为空，则在这个列表中进行搜索，为空在所有CM管理的列表中进行搜索</param>
	/// <param name="Type">按指定包含的类型进行搜索</param>
	/// <param name="Camp">按指定的阵营</param>
	/// <returns>true 满足条件，false 不满足条件</returns>
	private static bool CheckCoreConditions(LifeMCore Core,LifeMType Type,LifeMCamp Camp)
	{
		if(Core == null )  return false;

		if ( (Type & Core.m_type) == 0 )
			return false;

		if ( (Camp & Core.m_Camp) == 0 )
			return false;

		else return true;
	}


	/// <summary>
	/// 满足Core条件判断
	/// </summary>
	/// <param name="HaveList">不为空，则在这个列表中进行搜索，为空在所有CM管理的列表中进行搜索</param>
	/// <param name="Camp">按指定的阵营</param>
	/// <returns>true 满足条件，false 不满足条件</returns>
	private static bool CheckCoreConditions(LifeMCore Core,LifeMCamp Camp)
	{
		if(Core == null )  return false;

		
		if ( (Camp & Core.m_Camp) == 0 )
			return false;
		
		else return true;
	}

	/// <summary>
	/// 搜索船上，单个类型,某个阵营
	/// </summary>
	/// <param name="Type">按指定包含的类型进行搜索</param>
	/// <param name="Camp">按指定的阵营</param>
	/// <returns>返回搜索列表list</returns>
	public static void SearchLifeMListInBoat(ref List<Life> list, LifeMType Type ,LifeMCamp Camp)
	{
		SearchLifeMList(ref list, null,Type,Camp,MapSearchStlye.InBoat, null,0.0f);
	}
	/// <summary>
	/// 搜索船上，单个类型,所有阵营
	/// </summary>
	/// <param name="Type">按指定包含的类型进行搜索</param>
	/// <param name="Camp">按指定的阵营</param>
	/// <returns>返回搜索列表list</returns>
	public static void SearchLifeMListInBoat(ref List<Life> list, LifeMType Type )
	{
		SearchLifeMList(ref list, null,Type,LifeMCamp.ALL,MapSearchStlye.InBoat, null,0.0f);
	}

	/// <summary>
	/// 选择范围内伤害目标列表
	/// </summary>
	/// <param name="list">搜素列表</param>
	/// <param name="Attack">攻击者</param>
	/// <returns>返回搜索列表list</returns>
	public static void SearchHurtLifeMList(ref List<Life> list,Life Attack )
	{
		if (list == null || list.Count == 0 )
			return ;
		if(Attack == null) list.Clear();
		for(int i = 0; i < list.Count ; i++)
		{
			if(Attack.CheckInVision(list[i]) == false)
			{
				list.RemoveAt(i);
				i --;
			}
		}
	}	

	/// <summary>
	/// 选择可以作为攻击目标列表
	/// </summary>
	/// <param name="list">搜素列表</param>
	/// <param name="Attack">攻击者</param>
	/// <returns>返回搜索列表list</returns>
	public static void SearchAttackLifeMList(ref List<Life> list,Life Attack )
	{
		if (list == null || list.Count == 0 )
			return ;
		if(Attack == null) list.Clear();
		for(int i = 0; i < list.Count ; i++)
		{
			if(Attack.CheckInVision(list[i]) == false|| Attack.CanAttackTarget(list[i]) == false )
			{
				list.RemoveAt(i);
				i --;
			}
		}
	}

	/// <summary>
	/// 炮弹兵SceneID 转dataID
	/// </summary>
	public static int SceneID2DataIDInSoldier(int sceneID)
	{
		foreach(SceneSoldier Info in m_SoldierList )
		{
			if(Info != null && Info.m_SceneID == sceneID)
				return Info.m_Core.m_DataID;
		}
		return -1;
	}
	/// <summary>
	/// dataID 转SceneID
	/// </summary>
	public static int DataID2SceneIDInSoldier(int DataID)
	{
		foreach(SceneSoldier Info in m_SoldierList )
		{
			if(Info != null && Info.m_Core.m_DataID == DataID)
				return Info.m_SceneID;
		}
		return -1;
	}

	/// <summary>
	/// 按条件搜索lifeM
	/// </summary>
	/// <param name="HaveList">不为空，则在这个列表中进行搜索，为空在所有CM管理的列表中进行搜索</param>
	/// <param name="Type">lifem 类型，支持位组合</param>
	/// <param name="Camp">按指定的阵营，支持组合</param>
	/// <param name="TargetMg">格子的位置</param>
	/// <param name="Radius">搜索半径，圆形，球形需要</param>
	/// <returns>返回搜索列表list</returns>
	public static void SearchLifeMListByGrid(ref List<Life> list,
	                                   List<Life>HaveList,
	                                   LifeMType Type ,
	                                   LifeMCamp Camp,
	                                   MapSearchStlye SearchStype,
	                                   MapGrid TargetMg ,
	                                   float Radius)
	{
		if (list == null)
			list = new List<Life> ();
		else list.Clear ();
		//cm 中获取列表
		if(HaveList == null || HaveList.Count == 0)
		{
			List<SceneSoldier> l = GetLifeMList(Type);
			if(l != null)
			{
				foreach ( SceneSoldier  s in l) 
				{
					if(s == null) continue;
					LifeMCore Core = s.m_Core;
					Life life = s.m_Life;
					if(CheckCoreConditions(Core,Camp) == false)
						continue;
					if(CheckMapConditions(SearchStype,TargetMg,life,Radius) == false)
						continue;
					list.Add(life);
				}
			}
		}
		else //have list 中进行过滤
		{
			foreach ( Life life in HaveList) 
			{
				if(life == null) continue ;
				LifeMCore Core = life.m_Core;
				if(CheckCoreConditions(Core,Type,Camp) == false)
					continue;
				if(CheckMapConditions(SearchStype,TargetMg,life,Radius) == false)
					continue;
				list.Add(life);
			}
		}
	}	

}
