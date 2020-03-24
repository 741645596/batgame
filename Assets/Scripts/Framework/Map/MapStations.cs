using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// 地图站位基类
/// </summary>
/// <author>zhulin</author>
/// 
 
/// <summary>
/// 格子空间
/// </summary>
public enum GridSpace {
	Space_UP     = 0,   //格子上部空间
	Space_CENTER = 1,   //格子中部空间
	Space_DOWN   = 2,   //格子底部空间
};

/// <summary>
/// 角色在格子中的状态
/// </summary>
public enum RoleStationState{
	NONE          = 0,   //根本不在
	HoldStation   = 1,   //占领中
	TempStation   = 2,   //临时路过中
}


public class MapStations  { 
	//进入地图单元角色列表
	//占领攻击位的角色
	protected RoleStations m_HoldRole = null;
	//临时占领攻击位的角色
	protected RoleStations m_TempRole = null;
	protected Channel  m_Channel = null;

	public virtual void Init()
	{

	}

	//获取最大拥挤数
	public virtual int GetMaxJam()
	{
		return 1;
	}
	

	public void GetRoleList(ref List<int> list)
	{
		//占领攻击位上的角色
		m_HoldRole.GetRoleList(ref list);
		List<int> l = new List<int>();
		//临时攻击位的角色
		m_TempRole.GetRoleList(ref l);
		//
		list.AddRange(l);
	}


	public void GetRoleList(ref List<int> Holdlist ,ref List<int> TempList)
	{
		//占领攻击位上的角色
		m_HoldRole.GetRoleList(ref Holdlist);
		m_TempRole.GetRoleList(ref TempList);
	}

	public  bool IsAttackStations()
	{
		if (this is MapGrid) 
		{
			if((this as MapGrid).PropStations != StationsProp.ATTACK)
			{
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// 角色预报占领攻击位动作
	/// </summary>
	/// <returns></returns>
	public  void InputStations(int SceneID ,StationsInfo Rs)
	{
		//检查是否为攻击位
		/*if(IsAttackStations () == false)
			return ;*/
		//检查是否已经包含该角色
		if(m_HoldRole.CheckHaveRole (SceneID) == true)
			return ;

		//先清理，前任所在的攻击位
		MapM.EmptyRoleStations(SceneID,LifeMType.SOLDIER);
		m_TempRole.AddRole(SceneID ,Rs);
		MapM.AddUpRoleStation(this);
		MapM.ChangeRoleStation(SceneID,this);
	}

	/// <summary>
	/// 清空角色在格子数据
	/// </summary>
	public bool EmptyStations(int SceneID)
	{
		bool ret1 = m_HoldRole.RomoveRole(SceneID);
		bool ret2 = m_TempRole.RomoveRole(SceneID);

		if(ret1 == true || ret2 == true )
			return true;
		else return false;
	}



	/// <summary>
	/// 核心算法，选出，最佳的角色从临时占领，转为占领攻击。
	/// </summary>
	/// <returns></returns>
	public void ResolveStations()
	{
		//未满
		if(m_HoldRole.CheckFullRole () == false)
		{
			int SceneID = m_TempRole.GetBestRole();
			if(SceneID != -1)
			{
				ResolveStations(SceneID);
			}
		}
	}

	/// <summary>
	/// 角色执行占领动作
	/// </summary>
	/// <param name="SceneID">角色sceneID 执行占领动作</param>
	/// <returns></returns>
	public void ResolveStations(int SceneID)
	{
		StationsInfo Rs = m_TempRole.GetRoleStationsInfo(SceneID);
		m_HoldRole.AddRole(Rs);
		m_TempRole.RomoveRole(SceneID);
	}

	/// <summary>
	/// 角色站位信息
	/// </summary>
	/// <returns></returns>
	public void ClearRoleStations()
	{
		if(m_HoldRole != null)
			m_HoldRole.ClearRole();
		if(m_TempRole != null)
			m_TempRole.ClearRole();
		if(m_Channel != null)
			m_Channel.ClearChannel();
	}


	/// <summary>
	/// 确认除指定对象外该位置是否被占领
	/// </summary>
	/// <param name="sceneID">被排除的对象</param>
	/// <returns>true:空闲，false：被占领</returns>
	public bool  CheckIdle(int sceneID)
	{
		//排除非攻击位
		if(IsAttackStations() == false)
			return false;

		List<int> l = new List<int>();
		//临时攻击位的角色
		m_HoldRole.GetRoleList(ref l);

		if(l.Count > 0 ) 
		{
			if(l.Contains (sceneID) && l.Count == 1)
			{
				return true;
			}
			else return false;
		}
		else return true;
	}

	

	public  virtual void SortRank()
	{
	}

		

	

	//获取深度
	public virtual int GetRankDeep(int SceneID)
	{
		 return 0; //默认通道
	}
	
	public virtual string GetStationInfo()
	{
		return "";
	}

	
	//检测排位表
	public virtual Int2 GetStationsPos()
	{
		return Int2.zero;
	}

	//申请通道
	public virtual int AskForStaionsDeep(int StaionsDeep,LifeMCamp Camp,ref int ret)
	{
		return StaionsDeep;
	}

	public void SetTop(int SceneID)
	{
		m_HoldRole.SetTop(SceneID);
	}
	


	/// <summary>
	/// 获取角色在Station中的状态
	/// </summary>
	public RoleStationState GetRoleStateInStation(int SceneID)
	{
		List<int> l = new List<int>();
		m_HoldRole.GetRoleList(ref l);
		if(l.Contains (SceneID) == true)
			return RoleStationState.HoldStation;

		m_TempRole.GetRoleList(ref l);
		if(l.Contains (SceneID) == true)
			return RoleStationState.TempStation;

		return RoleStationState.NONE;
	}

	/// <summary>
	/// 获取角色列表
	/// </summary>
	public void GetCampRoleList(LifeMCamp Camp ,ref List<int> lSameCampRole , ref List<int> lUnSameCampRole)
	{
		m_HoldRole.GetCampRoleList(Camp ,ref lSameCampRole ,ref lUnSameCampRole);
	}


}
