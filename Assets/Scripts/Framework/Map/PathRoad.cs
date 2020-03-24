#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#define UNITY_EDITOR_LOG
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum PathAccess
{
	Prev   = 0, //访问前一个路径节点
	Cur    = 1, //访问当前路径节点
	Next   = 2, //访问下一个路径节点
}


public enum ErrReason
{
	RePathPoint    = 0, //提前遇到重新寻路路径点
	NoAttackPos    = 1, //剩余路线都没有攻击位路径节点
}

public struct ErrorInfo
{
	public ErrReason Err;       //错误原因
	public RoleState state;     //重新寻路点的状态        
	public DIR dir;             //重新寻路点的状态     
	public Int2 GridPos;        //重新寻路点格子坐标     
}

/// <summary>
/// 运动线路管理
/// </summary>
/// <author>zhulin</author>
public class PathRoad  {

	//角色SceneID
	private int m_SceneID = -1;
	public bool islocal = true;
	//格子单元位置
	private List<PathData>  m_Path = new List<PathData>();
	private List<PathData>  m_SearchPath = new List<PathData>();
	public List<PathData> Path
	{
		get{return m_Path;}
		set{m_Path = value;}
	}
	public List<PathData> SearchPath
	{
		get{return m_SearchPath;}
		set{m_SearchPath = value;}
	}

	public int PathCount
	{
		get{return Path.Count;}
	}
	/// <summary>
	/// 当前行走路径
	/// </summary>
	private int  m_Index = 0;
	public  int  Indext 
	{
		get{return m_Index;}
	}
	/// <summary>
	/// 清空路径
	/// </summary>
	public  void ClearPath()
	{
		Path.Clear();
		m_Index = 0;
	}
	public void SetIndex(int i)
	{
		m_Index = i;
	}
	/// <summary>
	/// 清空路径
	/// </summary>
	public  void ClearSeachPath()
	{
		SearchPath.Clear();
	}
	/// <summary>
	/// check 是有有路径
	/// </summary>
	public bool CheckHavePath()
	{
		if(Path.Count == 0) 
			return false;
		else return true;
	}


	/// <summary>
	/// check 是否刚开始运动
	/// </summary>
	public bool CheckRunFirstRoad()
	{
		if(m_Index == 0) 
			return true;
		else return false;
	}
	

	/// <summary>
	/// 路线运行完成
	/// </summary>
	public bool CheckRunFinishRoad()
	{
		if(m_Index >= PathCount-1) 
			return true;
		else return false;
	}


	/// <summary>
	/// 获取下一个攻击位（包含当前点）
	/// </summary>
	/// <returns>Error 错误哦原因</returns>
	public PathData GetNextAttackPosCurInPath(ref ErrorInfo Error)
	{
		Error.Err = ErrReason.NoAttackPos;
		for(int i = m_Index; i < PathCount; i++)
		{
			if (Path[i].state == RoleState.FALL || 
			    Path[i].state == RoleState.FALLDOWN || 
			    Path[i].state == RoleState.STAIR || 
			    Path[i].state == RoleState.JUMP)
			{
				Error.Err = ErrReason.RePathPoint;
				Error.GridPos = Path[i].Road.GridPos;
				Error.state = Path[i].state;
				//这个方向不对
				Error.dir = DIR.LEFT;
				return null;
			}
			if (Path[i].Road.PropStations == StationsProp.ATTACK)
			{
				return Path[i];
			}
		}
		return null;
	}


	/// <summary>
	/// 获取下一个攻击位(不包含当前点)
	/// </summary>
	/// <returns>Error 错误哦原因</returns>
	public PathData GetNextAttackPosNoCurInPath(ref ErrorInfo Error)
	{
		Error.Err = ErrReason.NoAttackPos;
		for(int i = m_Index + 1; i < PathCount; i++)
		{
			if (Path[i- 1].state == RoleState.FALL || 
			    Path[i- 1].state == RoleState.FALLDOWN || 
			    Path[i- 1].state == RoleState.STAIR || 
			    Path[i -1].state == RoleState.JUMP)
			{
				Error.Err = ErrReason.RePathPoint;
				Error.GridPos = Path[i].Road.GridPos;
				Error.state = Path[i].state;
				//这个方向不对
				int PrevLayer =Path[i- 1].Road.GridPos.Layer;
				int CurLayer =Path[i].Road.GridPos.Layer;
				if(PrevLayer < CurLayer)
				{
					Error.dir = DIR.UP;
				}
				else if(PrevLayer > CurLayer)
				{
					Error.dir = DIR.DOWN;
				}
				else Error.dir = DIR.LEFT;
				return null;
			}
			if (Path[i].Road.PropStations == StationsProp.ATTACK)
			{
				return Path[i];
			}
		}
		return null;
	}



	/// <summary>
	/// 确定整个路线上，有空闲攻击位
	/// </summary>
	/// <returns>true： 有，false 没有</returns>
	public bool CheckHaveIdleAttackPosInPath(int sceneID)
	{
		if(CheckHavePath () == false) 
			return false;
		int layer = Path[0].Road.GridPos.Layer;

		for(int i = m_Index+1 ; i < PathCount; i++)
		{
			if (Path[i].Road.CheckIdle(sceneID) == true || (Path[i].Road.GridPos.Layer != layer))
			{
				return true;
			}
		}
		return false;
	}
	
	/// <summary>
	/// 获取路径节点
	/// </summary>
	/// <param name="IsSavePrev">ture 当前点为第一个节点时，当前点就为第一个节点</param>
	public PathData GetPathData(PathAccess Access ,bool IsSavePrev = false)
	{
		int index = 0;
		if(Access == PathAccess.Prev)
		{
			index = m_Index -1;
			if(index < 0 && IsSavePrev == true) 
				index = 0;
			return GetPathData(index);
		}
		else if(Access == PathAccess.Cur)
		{
			index = m_Index;
			return GetPathData(index);
		}
		else if(Access == PathAccess.Next)
		{
			index = m_Index +1;
			return GetPathData(index);
		}
		return null;
	}

	/// <summary>
	/// 运动到下一个结点
	/// </summary>
	public void RunNextPath()
	{
		m_Index ++;
	}

	/// <summary>
	/// 回滚到前一个格子
	/// </summary>
	public void ResetPath()
	{
		m_Index --;
		if(m_Index < 0)
			m_Index = 0;
	}


	/// <summary>
	/// 获取路径节点
	/// </summary>
	public PathData GetPathData(PathAccess Access ,int index)
	{
		if(Access == PathAccess.Cur)
		{
			return GetPathData(index);
		}
		else if(Access == PathAccess.Prev)
		{
			return GetPathData(index - 1);
		}
		else if(Access == PathAccess.Next)
		{
			return GetPathData(index + 1);
		}
		return null;
	}

	/// <summary>
	/// 获取路径节点
	/// </summary>
	public PathData GetPathData(int index)
	{
		if(CheckHavePath () == false) 
			return null;
		if(index >= 0 && index < PathCount)
		{
			return Path[index];
		}
		return null;
	}

	/// <summary>
	/// 是否在路径上
	/// </summary>
	public bool InRoad(Int2 pos)
	{
		for(int i = m_Index; i < PathCount; i++)
		{
			if(NdUtil.IsSameMapPos(pos ,Path[i].Road.GridPos) == true)
				return true;
		}
		if (PathCount <= 0)
			return true;
		return false;
	}


	public void PathAI(int SceneID,MapGrid start ,MapGrid end,NDAttribute Attr,GridSpace gs,WalkDir dir)
	{
		m_SceneID = SceneID;
		if(Attr == null)
		{
			Debug.LogError("Attr == null");
			return ;
		}
		#if UNITY_EDITOR_LOG
		FileLog.write(SceneID,"PathAI :"   + start.GridPos + "," + end.GridPos);
		
		#endif
		AIPathData.SetPathParam(SceneID ,Attr.Speed,Attr.JumpDistance,Attr.IsHandstand ,Attr.IsKeepDir);
		AIPathData.SearchPath (start,end,ref m_SearchPath,gs,dir);
		//m_Index = 0;

		#if UNITY_EDITOR_LOG
	    //打印整合路径
		if (PathCount > 0  )
		{
			string str = SceneID + ",";
			for(int i = 0; i <  SearchPath.Count; i++)
			{
				str +=  "{" + SearchPath[i].state + "," +  SearchPath[i].Road.GridPos + "," +  SearchPath[i].deltaTime + "," +  SearchPath[i].dir +"," + SearchPath[i].gs+ "},";
			}
			FileLog.write(SceneID,str,true);
		}
       #endif
	}


	//获取当前层的路径，小黄鸭用
	public List<PathData>  GetCurrentLayerPathList(Int2 MapPos)
	{
		List<PathData> layerpath = new List<PathData>();
		int layer = MapPos.Layer;
		for (int i = m_Index; i < Path.Count; i++)
		{
			if (layer != Path[i].Road.GridPos.Layer)
				break;
			layerpath.Add(Path[i]);
		}
		return layerpath;
	}




	public void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		if (PathCount > 1)
		{
			for(int i = 1; i < Path.Count; i ++)
			{
				MapGrid s  = Path[i-1].Road;
				MapGrid e = Path[i].Road;
				Gizmos.DrawLine(s.WorldPos,e.WorldPos);
			}
			
		}
	}

	public void RemoveRange(int index,int count)
	{
		Path.RemoveRange(index,count);
	}
}
