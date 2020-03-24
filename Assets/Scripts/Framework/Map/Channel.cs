#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#define UNITY_EDITOR_LOG
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 通道数
/// </summary>
/// <author>zhulin</author>
///

public enum CHANNEL {
TOP     = 0,   //顶层通道 
ONE     = 1,   //通道1
TWO     = 2,   //通道2
DEFAULT = 3,   //默认通道
};

public struct RolePower
{
	public int      SceneID ;
	public int      Shape;    // 体型大小，体型大的在后。
	public bool     IsNewJoin ; // 是否为新进入的。
	public LifeMCamp  m_Camp;
	public CHANNEL  Deep;


	public void SetPower(StationsInfo Info)
	{
		if(Info == null)
			return ;
		this.SceneID = Info.m_SceneID;
		this.Shape = Info.m_Shape;
		this.m_Camp = Info.m_Camp ;
		if(Info.m_StaionsDeep == 0)
		{
			this.Deep = CHANNEL.ONE;
		}
		else if(Info.m_StaionsDeep == 1)
		{
			this.Deep = CHANNEL.TWO;
		}
		else
		{
			this.Deep = CHANNEL.DEFAULT;
		}
	}

	public static int Compare(RolePower a, RolePower b)
	{
		if(a.Shape < b.Shape ) return -1;
		else if(a.Shape > b.Shape ) return 1;
		else
		{
			if(a.Deep < b.Deep) return -1;
			else if(a.Deep > b.Deep) return 1;
			else return 0;
		}
		
	}
}

public class Channel  {
	//标记通道是否被占领
	private List<bool>m_ListChannel = new List<bool> ();
	//存储通道中的角色
	private Dictionary<int ,int> m_RoleDeep = new Dictionary<int ,int> ();
	private List<RolePower>m_lBackCamp = new List<RolePower> ();  //后排阵营
	private List<RolePower>m_lFrontCamp = new List<RolePower> (); //前排阵营
	private List<RolePower>m_TLPower = new List<RolePower> ();    //需要强制置顶的
	
	/// <summary>
	/// 通道初始化
	/// </summary>
	public void Init()
	{
		//定义4个通道
		m_ListChannel.Clear();
		m_ListChannel.Add(false);
		m_ListChannel.Add(false);
		m_ListChannel.Add(false);
		m_ListChannel.Add(false);
	}

	/// <summary>
	/// 设置角色的通道权值
	/// </summary>
	/// <param name="HoldRoleList">已在格子的角色</param>
	/// <param name="_NewRolelist"></param>
	public void SetRolePower(RoleStations Role,RoleStations CalcInRole)
	{
		if(Role == null || CalcInRole == null )
			return ;

		m_TLPower.Clear();
		List<int> l = new List<int>();
		Role.GetRoleList(ref l);
		List<RolePower> lold = new List<RolePower> (); 
		for(int i = 0 ; i < l.Count ; i++)
		{
			StationsInfo Info = Role.GetRoleStationsInfo(l[i]);
			if(Info != null)
			{
				RolePower p= new RolePower();
				p.SetPower(Info);
				p.IsNewJoin = false;
				if(Info.m_IsTop == true)
				{
					m_TLPower.Add(p);
				}
				else 
				{
					lold.Add(p);
				}
			}
		}
		//
		l.Clear();
		//
		CalcInRole.GetRoleList(ref l);
		List<RolePower> lNew = new List<RolePower> (); 
		for(int i = 0 ; i < l.Count ; i++)
		{
			StationsInfo Info = CalcInRole.GetRoleStationsInfo(l[i]);
			if(Info != null)
			{
				RolePower p= new RolePower();
				p.IsNewJoin = true;
				p.SetPower(Info);
				if(Info.m_IsTop == true)
				{
					m_TLPower.Add(p);
				}
				else 
				{
					lNew.Add(p);
				}
			}
		}
        //划分中置顶阵营，前阵营，后阵营。
		CalcTopChannel(ref lold ,ref lNew);
		CalcMainCamp(ref lold ,ref lNew);

	}


	/// <summary>
	/// 划分前后阵营，前阵营优先在已进入列表中寻找
	/// </summary>
	public void CalcMainCamp(ref List<RolePower> lOld,ref List<RolePower> lNew )
	{
		lOld.Sort((x, y) =>{
			return RolePower.Compare(x , y);
		});
		
		lNew.Sort((x, y) =>{
			return RolePower.Compare(x , y);
		});

		LifeMCamp MainCamp  = LifeMCamp.NONE ;

		if(lOld != null && lOld.Count > 0)
		{
			MainCamp = lOld[0].m_Camp;
		}
		else if(lNew != null && lNew.Count > 0)
		{
			MainCamp = lNew[0].m_Camp;
		}
		//
		m_lFrontCamp.Clear();
		m_lBackCamp.Clear();

		if(lOld != null && lOld.Count > 0)
		{
			foreach(RolePower p in lOld)
			{
				if(p.m_Camp == MainCamp)
				{
					m_lFrontCamp.Add(p);
				}
				else
				{
					m_lBackCamp.Add(p);
				}
			}
		}


		if(lNew != null && lNew.Count > 0)
		{
			foreach(RolePower p in lNew)
			{
				if(p.m_Camp == MainCamp)
				{
					m_lFrontCamp.Add(p);
				}
				else
				{
					m_lBackCamp.Add(p);
				}
			}
		}
		lOld.Clear();
		lNew.Clear();
		
	}


	private void CalcTopChannel(ref List<RolePower> lOld,ref List<RolePower> lNew )
	{
		m_TLPower.Sort((x, y) =>{
			return RolePower.Compare(x , y);
		});

		if(m_TLPower == null || m_TLPower.Count < 2)
			return ;
		for(int i = 1 ;  i < m_TLPower.Count ; )
		{
			if(m_TLPower[i].IsNewJoin )
			{
				lNew.Add(m_TLPower [i]);
			}
			else
			{
				lOld.Add(m_TLPower [i]);
			}
			m_TLPower.RemoveAt(i);
		}

	}

	/// <summary>
	/// 通道排序
	/// </summary>
	public void SortChannel()
	{
		ResetChannel();

		m_lFrontCamp.Sort((x, y) =>{
			return RolePower.Compare(x , y);
		});
		
		m_lBackCamp.Sort((x, y) =>{
			return RolePower.Compare(x , y);
		});
		
		bool HaveFront = false;
		LifeMCamp TopFrontCamp = LifeMCamp.NONE ;
		//置顶通道
		if(m_TLPower.Count >0)
		{
			int sceneID = m_TLPower[0].SceneID;
			HaveFront = true;
			TopFrontCamp = m_TLPower[0].m_Camp ;
			RoleSeatDeep(sceneID  , 0);
		}
		//前置阵营通道
		if(m_lFrontCamp.Count > 0)
		{
			int start = 0;
			if(HaveFront == true && TopFrontCamp == m_lFrontCamp[0].m_Camp)
				start = 1;
			for(int i = 0; i < m_lFrontCamp.Count; i++)
			{
				int sceneID = m_lFrontCamp[i].SceneID;
				RoleSeatDeep(sceneID  , i + start);
			}
		}
		//后置阵营
		if(m_lBackCamp.Count > 0)
		{
			int start = 0;
			if(HaveFront == true && TopFrontCamp == m_lBackCamp[0].m_Camp)
				start = 1;
			for(int i = 0; i < m_lBackCamp.Count; i++)
			{
				HaveFront = true;
				int sceneID = m_lBackCamp[i].SceneID;
				RoleSeatDeep(sceneID  , i + start);
			}
		}
	}

	/// <summary>
	/// 角色占领通道
	/// </summary>
    private void RoleSeatDeep(int SceneID ,int deep)  
	{
		if(m_ListChannel.Count <= deep)
		{
			deep = m_ListChannel.Count -1; //强制插入到最后一个排
		}
		m_ListChannel[deep] = true;
		if(m_RoleDeep.ContainsKey (SceneID) == false)
		{
			m_RoleDeep.Add(SceneID, deep);
		}
		else Debug.LogError("排序出现异常");
	}
	/// <summary>
	/// 通道复位
	/// </summary>
	private void ResetChannel()
	{
		for(int i = 0; i < m_ListChannel.Count ; i++)
		{
			m_ListChannel[i] = false;
		}
		m_RoleDeep.Clear();
	}

	/// <summary>
	/// 获取空闲通道
	/// </summary>
	public bool GetIdleChanel(ref int Deep)
	{
		if(Deep >= 0 && Deep < m_ListChannel.Count -1)
		{
			if(m_ListChannel[Deep] == false)
			{
				return true;
			}
		}

		Deep = -1;
		for(int i = 0; i < m_ListChannel.Count; i++)
		{
			if(m_ListChannel[i] == false)
			{
				Deep = i;
				return true;
			}
		}
		return false;
	}


	public int GetChannel(int SceneID)
	{
		if(m_RoleDeep.ContainsKey (SceneID)== true)
			return m_RoleDeep[SceneID] ;
		return  (int)CHANNEL.DEFAULT;
	}

	public void ClearChannel()
	{
		m_ListChannel.Clear();
		m_lBackCamp.Clear();
		m_lFrontCamp.Clear();
		m_TLPower.Clear();
	}

}
