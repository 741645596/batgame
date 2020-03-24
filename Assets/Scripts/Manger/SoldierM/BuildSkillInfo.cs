using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 建筑技能 
/// </summary>
public class BuildSkillInfo  : SkillInfo
{
	
	public int m_buildtype;
	public int m_quality;
	public string m_desc;
	public SearchTarageInfo m_tSearchInfo  = new SearchTarageInfo();
	public SearchTarageInfo m_dSearchInfo  = new SearchTarageInfo();
}


/// <summary>
/// 建筑技能搜索目标 
/// </summary>
public class SearchTarageInfo 
{
	private int m_shape;//m_shape 0表示无需触发，只需cd状态到后可触发。1=目标攻击，2=圆形攻击，3=环形攻击，4=矩形攻击,5=目标范围圆形攻击
	private int m_inboat;
	private int m_layer;
	private string m_param;
	private List<float> m_lParam = new List<float>();

	public void SetData(int shape ,int inboat, int layer, string param)
	{
		m_shape = shape ;
		m_inboat = inboat ;
		m_layer = layer;
		m_param = param;
		SetParam() ;
	}

	private void SetParam()
	{
		m_lParam.Clear();
		List<int> l = new List<int>();

		if(string.IsNullOrEmpty (m_param) == true)
			return ;
		
		if(m_param != null)
		{
			int length = NdUtil.GetLength(m_param);
			for(int i = 0 ; i < length ; i ++)
			{
				l.Add(NdUtil.GetIntValue(m_param , i));
			}
		}
		//m_shape 0表示无需触发，只需cd状态到后可触发。1=目标攻击，2=圆形攻击，3=环形攻击，4=矩形攻击,5=目标范围圆形攻击
		if(m_shape == 1||m_shape == 5)
		{
			if(l.Count == 1)
			{
				m_lParam.Add(l[0] * MapGrid.m_width / MapGrid.m_Pixel);
			}
		}
		//按圆形
		else if(m_shape == 2)
		{
			if(l.Count == 3)
			{
				m_lParam.Add(l[0] * 1.0f);
				m_lParam.Add(l[1] * 1.0f);
				m_lParam.Add(l[2] * MapGrid.m_width / MapGrid.m_Pixel);
			}
		}
		//按环形
		else if(m_shape == 3)
		{
			if(l.Count == 4)
			{
				m_lParam.Add(l[0] * 1.0f);
				m_lParam.Add(l[1] * 1.0f);
				m_lParam.Add(l[2] * MapGrid.m_width / MapGrid.m_Pixel);
				m_lParam.Add(l[3] * MapGrid.m_width / MapGrid.m_Pixel);
			}
		}
		//按矩形
		else if(m_shape == 4)
		{
			if(l.Count == 2)
			{
				m_lParam.Add(l[0] * MapGrid.m_width / MapGrid.m_Pixel );
				m_lParam.Add(l[1] * MapGrid.m_width / MapGrid.m_Pixel);
			}
		}
	}


	/// <summary>
	/// 2个对象是否处于同一层
	/// </summary>
	/// <param name="Self">发起的对象</param>
	/// <param name="Target">目标对象</param>
	public bool CheckConditions(Life Self ,Life Target)
	{
		if(Self == null || Target == null )
			return false;
		//是否在船内
		if(m_inboat == 1 && Target.InBoat == false)
			return false;
		//是否同层，对异形房间，就算最下层
		if(m_layer == 1 && Life.InSameLayer(Self,Target) == false)
			return false;
		//按距离
		if(m_shape == 1)
		{
			float distance = Life.GetfDistance(Self ,Target);
			if(distance <= m_lParam[0])
				return true;
		}
		//按圆形
		else if(m_shape == 2)
		{
			if(m_lParam.Count != 3)
				return false;
			float distance = Life.GetfDistance(Self ,Target);
			float Angle = Life.GetAngle(Self ,Target);
			if(Angle >= m_lParam[0] 
			   && Angle <= m_lParam[1]
			   && distance <= m_lParam[2]) 
				return true;
		}
		//按环形
		else if(m_shape == 3)
		{
			if(m_lParam.Count != 4)
				return false;
			float distance = Life.GetfDistance(Self ,Target);
			float Angle = Life.GetAngle(Self ,Target);
			if(Angle >= m_lParam[0] 
			   && Angle <= m_lParam[1]
			   && distance >= m_lParam[2]
			   && distance <= m_lParam[3]
			   ) 
				return true;
		}
		//按矩形
		else if(m_shape == 4)
		{
			if(m_lParam.Count != 2) return false;
			Vector2 distance = Life.GetDistance(Self ,Target);
			if(distance.x <= m_lParam[0] 
			   && distance.y <= m_lParam[1])
				return true;
		}
		//目标范围圆形攻击
		else if(m_shape == 5)
		{
			Life liftTrigger = Self.m_Skill.PropSkillInfo.m_LifeTarget;
			if(null!=liftTrigger)
			{
				if(liftTrigger.GetMapPos().Layer==Target.GetMapPos().Layer)
				{
					Vector3 P1 = Self.m_Skill.PropSkillInfo.m_vTargetPos;
					Vector3 p2 = Target.GetPos();
					float distance = Vector2.Distance(P1,p2);
					if(distance <= m_lParam[0])
						return true;
				}
			}
		}
		
		return false;
	}



	

}

