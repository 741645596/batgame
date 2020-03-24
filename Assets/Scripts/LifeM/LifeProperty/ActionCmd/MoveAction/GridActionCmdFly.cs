using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 炮战飞行
/// </summary>
/// <author>zhulin</author>
public struct FlyDataInfo
{
	public Vector3 Pos;     //飞行轨迹点
	public Vector3 FlyDir;  //飞行方向
	public float   FlyTime; //飞行耗时
}

public class GridActionCmdFly: GridActionCmd
{
	/// <summary>
	/// 飞行轨迹数据
	/// </summary>
	private List<FlyDataInfo> m_lFlyData = new List<FlyDataInfo> ();
	/// <summary>
	/// 前期飞行 轨迹点列表的索引
	/// </summary>
	protected int m_iPathIndext = 0;
	/// <summary>
	/// 飞行的速度
	/// </summary>
	protected float m_fSpeed = 10.0f;
	/// <summary>
	/// 记录最后的方向
	/// </summary>
	protected Vector3 m_Lastdir;

    protected  AnimatorState m_flyState;
	/// <summary>
	/// 2个飞行结点间的飞行时间统计
	/// </summary>
	private float m_FlySecTime;
	/// <summary>
	/// 飞行位置
	/// </summary>
	protected Vector3 m_FlyPos;
	/// <summary>
	/// 飞行方向
	/// </summary>
	protected Vector3 m_FlyDir;
	/// <summary>
	/// 角色模型
	/// </summary>
	protected int m_ModelType;

	protected bool m_TraiOver = false;
    /// <summary>
    /// 是否旋转
    /// </summary>
    public bool m_bEnableRotate = true;


	protected bool m_IsTraceFly = false;
	public bool IsTraceFly
	{
		get{return m_IsTraceFly;}
	}

    protected FlyCollisionInfo m_FlyCollisionInfo;

	public GridActionCmdFly(Life Parent,List<Vector3> pos,float speed,int ModelType)
	{
		m_IsTraceFly = true;
		SetTarget(Parent);
		m_ModelType = ModelType;
		m_fSpeed = speed;
        //m_fSpeed =2.5f;
		m_Duration = 0.0f;
		m_FlySecTime = 0.0f;
		CollectFlyPath(pos);
        m_flyState = AnimatorState.Fly00000;
        RolePlayAnimation(m_flyState);
        MoveAction(m_flyState, m_lFlyData[0].Pos, ActionMode.Set);
		m_FlyDir = CalcDir(Vector3.left,m_lFlyData[0].FlyDir,1,1);
		if (m_Dir == WalkDir.WALKLEFT) 
			m_FlyDir.z -= 180;
		SetLineDir(m_FlyDir);
		EnableTrail(true);
		EnableFireFx();
	}

    public void  SetFlyCollisionInfo(FlyCollisionInfo info)
    {
        m_FlyCollisionInfo = info;
    }

	public virtual void SetLineDir(Vector3 FlyDir)
	{
		RotateAction(m_FlyDir);
	}

	public override void Update()
	{
		base.Update();
        DoFly();
	}
    public virtual void DoFly()
    {
        if (m_TraiOver == false)
        {
            //trail 飞行
			m_IsTraceFly = true;
            if (GetFlyData() == true)
            {
                TrailFly();
            }
            else
            {
                ClearFlyData();
                m_Duration = 600;
                m_TraiOver = true;
            }
        }
        else//直线飞行 
        {
            MoveAction(m_flyState, m_Lastdir * m_fSpeed * Time.deltaTime, ActionMode.Delta);
			m_IsTraceFly = false;
        }

        //NGUIUtil.DebugLog("GridActionCmdFly:" + "m_TrailOver=" + m_TraiOver + " ,m_Duration ="+m_Duration,"red");
    }

	public FlyDir GetDir()
	{
		Vector3 dir = Vector3.zero;
		if (m_TraiOver == false)
		{
			dir = m_FlyDir;
		}
		else//直线飞行 
		{
			dir = m_Lastdir;
		}
		if (dir.x > 0 && dir.y >0)
			return FlyDir.RightTop;
		if (dir.x < 0 && dir.y >0)
			return FlyDir.LeftTop;
		if (dir.x < 0 && dir.y <0)
			return FlyDir.LeftBottom;
		if (dir.x > 0 && dir.y <0)
			return FlyDir.RightBottom;
		if (dir.x == 0 && dir.y > 0)
			return FlyDir.Top;
		if (dir.x == 0 && dir.y < 0)
			return FlyDir.Bottom;
		if (dir.y == 0 && dir.x > 0)
			return FlyDir.Right;
		if (dir.y == 0 && dir.x < 0)
			return FlyDir.Left;

		return FlyDir.none;

	}
	/// <summary>
	/// 轨迹飞行
	/// </summary>
	public virtual void TrailFly()
	{
		RotateAction(m_FlyDir);

        //m_flyState = AnimatorState.Fly00000;
        PlayAction(m_flyState, m_FlyPos, true);
	}
	/// <summary>
	/// 收集飞行轨迹
	/// </summary>
	private bool CollectFlyPath(List<Vector3> pos)
	{
		if (pos.Count < 2) return false;
		//收集飞行数据点。
		for(int i = 0; i < pos.Count; i++)
		{
			if(NdUtil.V3Equal(pos[i],Vector3.zero))
				continue;
			FlyDataInfo Info = new FlyDataInfo();
			Info.Pos = new Vector3(pos[i].x,pos[i].y,0);
			m_lFlyData.Add(Info);
		}
		//
		for(int i = 0; i < m_lFlyData.Count -1; i++)
		{
			float Angel = NdUtil.V2toAngle(m_lFlyData[i].Pos, m_lFlyData[i + 1].Pos,Vector3.right);
			FlyDataInfo Info = m_lFlyData[i];
			Info.FlyDir = new Vector3(0,0,Angel);
			float Distance = Vector3.Distance(m_lFlyData[i].Pos  , m_lFlyData[i + 1].Pos);
			Info.FlyTime = Distance / m_fSpeed;
			m_Duration += Info.FlyTime;
			m_lFlyData[i] = Info;
			m_Lastdir =  (m_lFlyData[i + 1].Pos - m_lFlyData[i].Pos).normalized;
		}
		//
		FlyDataInfo Info1 = m_lFlyData[m_lFlyData.Count -1];
		Info1.FlyDir = m_lFlyData[m_lFlyData.Count -2].FlyDir;
		m_lFlyData[m_lFlyData.Count -1] = Info1;
		m_Dir = (m_lFlyData[1].Pos.x - m_lFlyData[0].Pos.x > 0)? WalkDir.WALKRIGHT : WalkDir.WALKLEFT;

		return true;
	}
    /// <summary>
    /// 切换两种飞行动作
    /// </summary>
    public  int CalculateHiteEdge()
    {
        int pointInIndex = -1;
        int index = -1;
        Vector3 returnPos = Vector3.zero;
        int lastIndex = m_lFlyData.Count-1;
       
        for (int i = 0; i < m_lFlyData.Count - 1; i++)
        {
			if (GenerateShip.pointInRejectPolygon(m_lFlyData[i].Pos,GenerateShip.GetRejectPolygon()) && i!=0)
            {
                pointInIndex = i;
                break;
            }
        }

        if (pointInIndex != -1)
        {
			index = GenerateShip.RayToRejectPolygon(m_lFlyData[pointInIndex - 1].Pos, m_lFlyData[pointInIndex].Pos, ref returnPos,GenerateShip.GetRejectPolygon());
        }
        else
        {
			index = GenerateShip.RayToRejectPolygon(m_lFlyData[lastIndex - 1].Pos, m_lFlyData[lastIndex].Pos, ref returnPos,GenerateShip.GetRejectPolygon());
        }
		return index;

        //NGUIUtil.DebugLog("hitIndex =" + index,"brown");
    }

	/// <summary>
	/// 轨迹飞行插值
	/// </summary>
	protected bool GetFlyData()
	{
		float d = Time.deltaTime;
		m_FlySecTime += d;
		if(m_iPathIndext< m_lFlyData.Count -1)
		{
			if(m_FlySecTime  < m_lFlyData[m_iPathIndext].FlyTime)
			{
				float t = m_FlySecTime / m_lFlyData[m_iPathIndext].FlyTime;
				m_FlyPos = Vector3.Lerp(m_lFlyData[m_iPathIndext].Pos ,m_lFlyData[m_iPathIndext + 1].Pos ,t);
               
                if (m_bEnableRotate)
                    m_FlyDir = CalcDir(m_lFlyData[m_iPathIndext].FlyDir, m_lFlyData[m_iPathIndext + 1].FlyDir, d, m_lFlyData[m_iPathIndext].FlyTime);
                else
				{
					m_Dir = (m_lFlyData[m_iPathIndext + 1].Pos.x - m_lFlyData[m_iPathIndext].Pos.x > 0)? WalkDir.WALKRIGHT : WalkDir.WALKLEFT;
                    m_FlyDir = new Vector3(0.0f, 0.0f, 0.0f);
				}
			}
			else
			{
				m_iPathIndext ++;
				if(m_iPathIndext < m_lFlyData.Count -1)
				{
					m_FlySecTime = m_FlySecTime - m_lFlyData[m_iPathIndext -1].FlyTime;
					//
					float t = m_FlySecTime / m_lFlyData[m_iPathIndext].FlyTime;
                    m_FlyPos = Vector3.Lerp(m_lFlyData[m_iPathIndext].Pos, m_lFlyData[m_iPathIndext + 1].Pos, t);
                   
                    if (m_bEnableRotate)
					        m_FlyDir = CalcDir(m_lFlyData[m_iPathIndext].FlyDir ,m_lFlyData[m_iPathIndext + 1].FlyDir,d,m_lFlyData[m_iPathIndext].FlyTime);
					else
					{
						m_Dir = (m_lFlyData[m_iPathIndext + 1].Pos.x - m_lFlyData[m_iPathIndext].Pos.x > 0)? WalkDir.WALKRIGHT : WalkDir.WALKLEFT;
						m_FlyDir = new Vector3(0.0f, 0.0f, 0.0f);
					}
                }
				else return false;
			}
			return true;
		}
		return false;
	}

	/// <summary>
	/// 清理飞行数据
	/// </summary>
	protected void ClearFlyData()
	{
		m_lFlyData.Clear();
	}

	public override bool IsDone()
	{
		if(base.IsDone())
		{
			EnableTrail(false);
			return true;
		}
		return false;
	}

	/// <summary>
	/// 获取最后的飞行方向
	/// </summary>
	public Vector3 GetLastFlyDir()
	{
		return m_Lastdir;
	}

	private  Vector3 CalcDir(Vector3 StartDir, Vector3 EndDir, float deltaTime , float TotalTime)
	{
		float fa = (EndDir-StartDir).z;
		if (fa > 180) fa = fa - 360;
		else if(fa < -180) fa = 360+ fa;

		float Angel = fa * deltaTime /TotalTime;
		return new Vector3(0,0,Angel);
	}

	/// <summary>
	/// 获取最后的飞行方向
	/// </summary>
	public float GetFlyDirAngel()
	{
		float angle = Vector3.Angle (m_FlyDir, Vector3.left);
		angle = Vector3.Dot (m_FlyDir, Vector3.up) < 0 ? angle : -angle;
		angle = NdUtil.ClampAngle(angle, 0, 360f);
		return angle;
	}
	/// <summary>
	/// 是否为直线飞行
	/// </summary>
	public bool CheckLineFly()
	{
		if(m_TraiOver == true)
			return true;
		else return false;
	}	
}


public class FlyActionCmdFactory
{
	/// <summary>
    /// 创建GridActionCmdFly
	/// </summary>
	public static GridActionCmdFly Create(Life Parent,List<Vector3>Pos,float speed,int ModelType)
	{
		 if(ModelType == 100003)
		{
			return new GridActionCmdFly100003(Parent,Pos,speed,ModelType);
		}
        else if (ModelType == 101004)
        {
            return new GridActionCmdFly101004(Parent, Pos, speed, ModelType);
        }
		 else if (ModelType == 103003)
		 {
			 return new GridActionCmdFly103003(Parent, Pos, speed, ModelType);
		 }
		else return new GridActionCmdFly(Parent,Pos,speed,ModelType);
	}
}
