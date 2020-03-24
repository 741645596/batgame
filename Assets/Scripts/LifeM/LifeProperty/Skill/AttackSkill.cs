using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 攻击技能
/// </summary>
/// <author>zhulin</author>
public class AttackSkill  : SkillAI {

	/// <summary>
	/// 所有技能
	/// </summary>
	private List<SoldierSkill> m_AllSkill  = new List<SoldierSkill>();
	private string m_attack1 = string.Empty;
	private string m_attack2 = string.Empty;
	/// <summary>
	/// 攻击次数
	/// </summary>
	private int   m_AttackTimes = 1;
	
	
	
	public  override void Update (float deltaTime ,float AttackSpeed) 
	{
		if (m_CdTime > 0)
		{
			m_CdTime -= deltaTime * AttackSpeed;
			if (m_CdTime <= 0)
			{
				//释放技能
				if(m_SkillInfo.m_Relsease == true)
				{
					m_CdTime = m_TimeCount;
					GetNextSkill();
				}
			}
		}
	}


	/// <summary>
	/// 获取下一个技能
	/// </summary>
	/// <returns></returns>
	public  bool GetNextSkill()
	{
		int skillNo = GetNextSkillOrderID();
		m_SkillInfo = GetSkill(skillNo);
		m_SkillInfo.m_Relsease = false;
		return true;
	}


	/// <summary>
	/// 获取下一个轮询技能ID
	/// </summary>
	/// <returns></returns>
	private  int GetNextSkillOrderID()
	{
		int N = m_AttackTimes ++;
		if (string.IsNullOrEmpty(m_attack1) || string.IsNullOrEmpty(m_attack2) || N <= 0) 
		{
			Debug.LogError ("数据非法 " + N);
			return -1;
		} 
		else 
		{
			int M= m_attack1.Length;
			int P = m_attack2.Length;
			if(N <= M)
			{
				return NdUtil.GetNumIndex(m_attack1 , N);
			}
			else
			{
				int Q = (N - M) % P;
				if(Q == 0) Q = P;
				return NdUtil.GetNumIndex(m_attack2 , Q);
			}
		}
	}


	/// <summary>
	/// 获取技能
	/// </summary>
	/// <param name="SkillNo"></param>
	/// <returns></returns>
	public SoldierSkill GetSkill(int SkillNo)
	{
		if(SkillNo >= 0 && SkillNo <= 7)
		{
			return m_AllSkill[SkillNo];
		}
		return null;
	}
}
