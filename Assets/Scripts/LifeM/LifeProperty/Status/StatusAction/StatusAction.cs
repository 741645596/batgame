#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#define UNITY_EDITOR_LOG
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 技能状态
/// </summary>
/// <author>zhulin</author> 

public class StatusAction
{
	//状态
	private List<int>m_AttrData = new List<int>();
	/// <summary>
	/// 产生该状态的，技能释放对象
	/// </summary>
	private int m_SkillType = 0;
	private int m_ResalseSceneID = 0;
	protected int m_SceneID = 0;
	public string m_Name = "";
	//状态持续时间
	private float m_Duration = 0.0f;
	private float m_CountTime = 0.0f;
	private float m_Timeinterval = 0.0f;
	private bool m_NcdOver = false;
	private bool m_Bfirst = false;
	private int m_persistence = 0;
	protected StatusType m_StatusType ;

	private SkillStatusInfo m_info;

	public SkillStatusInfo  StatusInfo
	{
		get {return m_info;}
	}
	public float Duration{
		get{return m_Duration;}
	}

	public void Init(int SceneID,int SkillType,int ResalseSceneID,SkillStatusInfo Info)
	{
		m_StatusType = (StatusType)Info.m_type;
		m_ResalseSceneID = ResalseSceneID;
		m_SceneID = SceneID;
		m_SkillType = SkillType;
		m_Name = Info.m_name;
		m_Bfirst = false;
		//
		#if UNITY_EDITOR_LOG
		//Debug.Log("添加状态: " + m_Name + "," + Info.m_effectid);
		#endif
		m_NcdOver = (Info.m_time == 0) ? true: false;
		m_CountTime = 0.0f;
		m_Duration = Info.m_time * 0.001f;
		m_Timeinterval = Info.m_timeinterval * 0.001f;
		m_persistence = Info.m_persistence ;
		m_info = Info;
		if(Info.m_effect != null && Info.m_effect.Count > 0)
		{
			m_AttrData.AddRange(Info.m_effect);
		}
	}
	
	/// <summary>
	/// 状态结束前做点啥。
	/// </summary>
	public void DoEventOver()
	{
		/*if(m_StatusType == StatusType.AddAttr && m_SkillType == 1022)
		{
			#if UNITY_EDITOR_LOG
			Debug.Log("熊儿子大招状态结束寻路");
			#endif
			AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_SELFSP),CM.GetLifeM(m_SceneID,LifeMType.SOLDIER));
		}*/
	}

	public bool CheckSameStaus(int ResalseSceneID,int SkillType)
	{
		if(ResalseSceneID == m_ResalseSceneID && SkillType == m_SkillType)
		{
			return true;
		}
		else return false;
	}
	
	public bool CheckSameSkillTypeStaus(int SkillType)
	{
		if( SkillType == m_SkillType)
		{
			return true;
		}
		else return false;
	}
	/// <summary>
	/// check 是否包含某个技能产生的状态
	/// </summary>
	/// <returns> true 包含 ，false 不包含</returns>
	public bool CheckActionBySkill(int SkillType)
	{
		if(SkillType == m_SkillType)
		{
			return true;
		}
		return false;
	}


	/// <summary>
	/// 获取属性数据列表
	/// </summary>
	/// <param name="persistence">0 跟状态有关的属性，状态没了，属性跟着没， 1 跟状态不绑定</param>
	/// <param name="lAttr">属性列表</param>
	/// <returns> 返回属性数据列表 </returns>
	public void GetAttrData(ref List<int> lAttr )
	{	
		if(lAttr == null)
			lAttr = new List<int>();
		//不需清空的。
		for(int i= 0; i < m_AttrData.Count; i++)
		{
			lAttr.Add(m_AttrData[i]);
		}
	}

	public int GetResalseSceneID()
	{
	    return	m_ResalseSceneID ;
	}
	
	/// <summary>
	/// 时间打断，重载可以根据释放对象，及周边环境进行打断
	/// </summary>
	public virtual bool IsStatusOver(float duration)
	{
		if(m_NcdOver == true)
			return false;

		m_Duration -=  duration;
		if (m_Duration <= 0)
		{
			return true;
		}
		else return false;

	}

	public bool GetPersistence()
	{
		return (m_persistence == 1) ? true : false;
	}
	public void SetDone()
	{
		m_Duration = 0;
	}

	public void ResetTime(int time)
	{
		m_Duration = time * 0.001f;
	}

	/// <summary>
	/// 持续状态属性
	/// </summary>
	/// <author>zhulin</author>
	public  bool DoSkillStatus(float duration)
	{
		if(m_Bfirst == false)
		{
			m_Bfirst = true;
			return true;
		}
		if(m_Timeinterval == 0.0f) return false;
		m_CountTime += duration; 
		if(m_CountTime >= m_Timeinterval)
		{
			m_CountTime = 0.0f;
			return true;
		}
		else return false;
	}

	
	/// <summary>
	/// 获取cd模式, true 为cd 模式，false 为非cd模式
	/// </summary>
	public bool GetCdMode()
	{
		return !m_NcdOver;
	}
	/// <summary>
	/// 防御盾防御值。
	/// </summary>
	public bool ReduceShieldHp(int Hp ,ref int reduceHp)
	{
		reduceHp = 0;
		if(m_AttrData.Count == 1)
		{
			if(m_AttrData[0] >  Hp)
			{
				m_AttrData[0] -= Hp ;
				reduceHp = Hp;
				return false;
			}
			else 
			{
				reduceHp = m_AttrData[0];
				m_AttrData[0] = 0 ;
				return true;
			}
		}
		return true;
	}
}
