using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 1502(Ktv) 受击
/// </summary>
/// <author>zhulin</author>
public class GridActionCmdHitByBuild1502 :GridActionCmdHitByBuild{
	int m_totalcount;
	int m_count;
	Transform m_parent;
	float m_oncetime;
	List<MapGrid> m_mgs;
	//Vector3 m_des;
	public GridActionCmdHitByBuild1502(float duration,List<MapGrid> l)
	{
		m_Duration = duration;
		m_count = 0;
		m_totalcount = 4;
		m_oncetime = m_Duration / m_totalcount;
		m_mgs = l;
	}
	public override void StartHitByBuild ()
	{
		m_parent = m_Skin.tRoot;
		m_Start = m_parent.localPosition;
		int count = Mathf.CeilToInt(m_mgs.Count/2f);
		int index = count-1;
		for(int i = 0; i < count; i++)
		{
			if (CalcPos(index + i))
				break;
			if (CalcPos(index - i))
				break;
		}
	}
	public bool CalcPos(int i)
	{
		if (i < 0 || i >= m_mgs.Count)
			return false;
		MapGrid gto = m_mgs[i];
		if (gto != null)
		{
			List<int> role = new List<int>();
			gto.GetRoleList(ref role);
			if (role.Count > 0)
			{
				foreach(int id in role)
				{
					Life l = CM.GetLifeM(id ,LifeMType.SOLDIER);
					if (l is Role)
					{
						Role ww = l as Role;
						if (ww.CurrentAction is GridActionCmdHitByBuild1504)
							continue;
						else
						{
							(m_LifePrent as Role).RoleWalk.Teleport(gto);
							m_End = gto.pos;
							return true;
						}
					}
				}
			}
			else
			{
				(m_LifePrent as Role).RoleWalk.Teleport(gto);
				m_End = gto.pos;
				return true;
			}
		}
		return false;
	}
	public override void UpdateHitByBuild()
	{
		Vector3 pos = Vector3.Lerp(m_Start,m_End,m_TimeCount  / m_Duration);
		PlayAction(AnimatorState.KTV,pos,true);
	}

}
