using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class StationsInfo
{ 
	public int   m_SceneID; 
	public int   m_StaionsDeep;       // 站位深度  
	public DIR   m_dir;               // 方向，只针对上下楼梯
	public float m_InPutTime;         // 进入时间或预测进入时间
	public LifeMCamp  m_Camp;         // 阵营
	public bool m_IsHide;             // 状态
	public GridSpace m_GridSpace;     // 所处于的格子空间
	public int m_Passageway;          // 通道数
	public int m_Shape;               // 体型大小
	public bool m_IsTop ;
	public float m_Speed;             // 行走速度
	
	public StationsInfo( StationsInfo s)
	{
		this.m_StaionsDeep = s.m_StaionsDeep;
		this.m_Camp = s.m_Camp;
		this.m_dir = s.m_dir;
		this.m_InPutTime = s.m_InPutTime;
		this.m_GridSpace = s.m_GridSpace;
		this.m_IsHide = s.m_IsHide;
		this.m_Passageway = s.m_Passageway;
		this.m_Shape = s.m_Shape;
		this.m_Speed = s.m_Speed;
	}
	
	public StationsInfo()
	{
	}
	
	public StationsInfo(NDAttribute Attr ,int StaionsDeep,LifeMCamp Camp,DIR dir,GridSpace gs)
	{
		if(Attr != null)
		{
			m_IsHide = Attr.IsHide;
			m_Passageway = Attr.Passageway;
			m_Shape = Attr.Shape;
			m_Speed = Attr.Speed;
		}
		else Debug.LogError("Attr is null ");

		this.m_StaionsDeep = StaionsDeep;
		this.m_Camp = Camp;
		this.m_dir = dir;
		this.m_InPutTime = Time.realtimeSinceStartup;
		this.m_GridSpace = gs;
	}


	public bool IsValidStations()
	{
		if(m_IsHide == true || m_GridSpace != GridSpace.Space_DOWN)
			return true;
		return false;
	}
	
	public static bool Equal(StationsInfo a, StationsInfo b)
	{
		if(a == null || b == null)
			return false;
		if(a.m_SceneID == b.m_SceneID)
			return true;
		else return false;
	}

	public static int Compare(StationsInfo a, StationsInfo b)
	{
		if(a == null || b == null)
			return 0;

		float d = a.m_InPutTime- b.m_InPutTime;
		if(Mathf.Abs(d) > 0.005f) //不同一帧。
		{
			if(d < 0) return -1;
			else return 1;
		}
		else
		{
			if(a.m_Speed > b.m_Speed)
				return -1;
			else if(a.m_Speed == b.m_Speed)
				return 0;
			else return 1;
		}
	}
};

public class RoleStations  {

	public List<StationsInfo> RoleList = new List<StationsInfo> ();


	public void AddRole(StationsInfo Rs)
	{
		if(Rs == null) return;
		bool bHave = true;
		for(int i = 0; i < RoleList.Count; i ++)
		{
			if(StationsInfo.Equal(RoleList[i] , Rs) == true)
			{
				RoleList[i].m_IsHide = Rs.m_IsHide;
				bHave = false;
				return  ;
			}
		}
		if(bHave) 
		{
			RoleList.Add(Rs);
		}
	}

	public void AddRole(int SceneID,StationsInfo Rs)
	{
		if(Rs == null)
		{
			return ;
		}
			
		Rs.m_SceneID = SceneID;
		if(CheckHaveRole (SceneID) == false)
		{
			AddRole(Rs);
		}
	}


	public bool RomoveRole(int SceneID)
	{
		for(int i = 0; i < RoleList.Count; i ++)
		{
			if(RoleList[i].m_SceneID == SceneID)
			{
				RoleList.RemoveAt(i);
				return  true;
			}
		}
		return false;
	}

	public bool RomoveRole(StationsInfo Rs)
	{
		if(Rs == null) return false;
		return RomoveRole(Rs.m_SceneID);
	}


	public void ClearRole()
	{
		RoleList.Clear();
	}


	public StationsInfo GetRoleStationsInfo(int SceneID)
	{
		for(int i = 0; i < RoleList.Count; i ++)
		{
			if(RoleList[i].m_SceneID == SceneID)
			{
				return  RoleList[i];
			}
		}
		return null;
	}

	public void GetRoleList(ref List<int> List)
	{
		if(List == null)
			List = new List<int>();
		List.Clear();

		for(int i = 0; i < RoleList.Count; i ++)
		{
			List.Add(RoleList[i].m_SceneID);
		}
	}


	public bool HaveRole()
	{
		if(RoleList == null) return false;
		if(RoleList.Count > 0) return true;
		else return false;
	}

	/// <summary>
	/// 确认是否还有每个角色
	/// </summary>
	/// <returns>true,含有，false 不含有</returns>
	public bool CheckHaveRole(int SceneID)
	{
		for(int i = 0; i < RoleList.Count; i ++)
		{
			if(RoleList[i].m_SceneID == SceneID)
			{
				return true;
			}
		}
		return false;
	}


	/// <summary>
	/// 获取最佳从临时站领，转为占领的角色SceneID
	/// </summary>
	/// <returns>-1,没有，非-1：最佳转正的角色</returns>
	public virtual int GetBestRole()
	{
		return -1;
	}

	/// <summary>
	/// 确认占领攻击位是否已经沾满
	/// </summary>
	/// <returns>true，已满，false未满</returns>
	public virtual bool CheckFullRole()
	{
		return true;
	}



	
	public void SetChannel(Channel channel)
	{
		if(channel == null) return ;

		for(int i = 0; i < RoleList.Count; i ++)
		{
			StationsInfo Info = RoleList[i];
			if(Info == null) continue ;
			int deep = channel.GetChannel(Info.m_SceneID);
			if(deep != -1)
			{
				Info.m_StaionsDeep = deep;
			}
			else Info.m_StaionsDeep = -1;
		}
	}

	
	/// <summary>
	/// 是否包含对立阵营的炮弹兵， 有true 否 flase
	/// </summary>
	public bool AntiCamp(LifeMCamp Camp)
	{
		for(int i = 0; i < RoleList.Count; i ++)
		{
			StationsInfo Info = RoleList[i];
			if(Info == null || Info.IsValidStations() == true) 
				continue ;
			if(Info.m_Camp != Camp)
			{
				return  true;
			}
		}
		return false;
	}

	public void SetTop(int SceneID)
	{
	  StationsInfo Info	= GetRoleStationsInfo(SceneID);
	  if(Info != null)
	  {
			Info.m_IsTop = true;
	  }
	}

	public void ResetTop()
	{
		for(int i = 0; i < RoleList.Count; i ++)
		{
			StationsInfo Info = RoleList[i];
			if(Info == null) 
				continue ;
			Info.m_IsTop = false;
		}
	}

	/// <summary>
	/// 获取角色列表
	/// </summary>
	public void GetCampRoleList(LifeMCamp Camp ,ref List<int> lSameCampRole , ref List<int> lUnSameCampRole)
	{
		if(lSameCampRole == null)
			lSameCampRole = new List<int>();
		lSameCampRole.Clear();

		if(lUnSameCampRole == null)
			lUnSameCampRole = new List<int>();
		lUnSameCampRole.Clear();
		
		for(int i = 0; i < RoleList.Count; i ++)
		{
			if(RoleList[i].m_Camp == Camp)
				lSameCampRole.Add(RoleList[i].m_SceneID);
			else lUnSameCampRole.Add(RoleList[i].m_SceneID);
		}
	}
}
