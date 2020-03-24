using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 1504(桑拿) 受击
/// </summary>
/// <author>zhulin</author>
public class GridActionCmdHitByBuild1504 :GridActionCmdHitByBuild{


	float m_JumpTime;
	float m_statusTime;
	float m_walktime;
	float m_JumpinTime;
	float m_jumpouttime;
	Vector3 m_des;
	List<MapGrid> m_mgs;

	public GridActionCmdHitByBuild1504(Vector3 start, Vector3 to,float duration,List<MapGrid> l)
	{
		m_Start = start;
		m_End = to;
		float distent = Mathf.Abs(m_Start.x - m_End.x);
		
		m_walktime = distent /3f;
		m_JumpTime = m_walktime + 0.5f;
		m_statusTime = duration;
		m_jumpouttime = m_statusTime + 0.25f;
		m_Duration = m_jumpouttime + 0.5f;
		m_mgs = l;
	}

	public override void StartHitByBuild ()
	{
		int count = Mathf.CeilToInt(m_mgs.Count/2f);
		int index = count-1;
		//	{
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
			Role w = m_LifePrent as Role;
			List<int> role = new List<int>();
			gto.GetRoleList(ref role);
			if (role.Count > 0)
			{
				foreach(int id in role)
				{
					Life l = CM.GetLifeM(id,LifeMType.SOLDIER);
					if (l is Role)
					{
						Role ww = l as Role;
						if (ww.CurrentAction is GridActionCmdHitByBuild1504)
							continue;
						else
						{
							w.RoleWalk.Teleport(gto);
							m_des = gto.pos;
							return true;
						}
					}
				}
			}
			else
			{
				w.RoleWalk.Teleport(gto);
				m_des = gto.pos;
				return true;
			}
		}
		m_LifePrent.InBoat = false;
		return false;
	}
	
	public override void UpdateHitByBuild()
	{
		
		if (m_TimeCount < m_walktime)
		{
			Vector3 topos = m_End;
			topos.z = m_Start.z;
			Vector3 pos = Vector3.Lerp(m_Start,topos,m_TimeCount / m_walktime);
			//m_AniSpeed = 2f;
			PlayAction(AnimatorState.Walk,pos,true);
		}
		else if (m_TimeCount < m_JumpTime)
		{
			Vector3 topos = m_End;
			topos.z = m_Start.z;
			Vector3 pos = Vector3.Lerp(topos,m_End,(m_TimeCount-m_walktime) / (m_JumpTime-m_walktime));
			PlayAction(AnimatorState.SanNa,pos,true);
			if ((m_TimeCount - m_Delatime) < (m_walktime + 0.3f) && m_TimeCount > (m_walktime +0.3f))
			{
				
				Vector3 effectpos = m_Skin.tRoot.position;
				effectpos.y +=0.8f;
				//m_effectgo = SkillEffects._instance.LoadEffect("effect/prefab/", "1906031",effectpos,0.75f);
				GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1906031", effectpos, BattleEnvironmentM.GetLifeMBornNode(true));
				GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(0.75f);
				gae.AddAction(ndEffect);
                m_effectgo = gae.gameObject;

				SoundPlay.Play("Trap/trap_sauna",false ,false);
			}
		}
		else if (m_TimeCount < m_statusTime)
		{
			Transform t = m_Skin.tRoot;
			
			Vector3 topos = m_des;
			topos.z = m_End.z;
			t.localPosition = topos;
			if ((m_TimeCount - m_Delatime) < m_JumpTime)
			{
				(m_Skin as RoleSkin).HitByBuildingEffect(HitbyBuilding.HitByBuild1504,true,0 ,HitEffectMode.CoverBody,null);
				PlayAction(AnimatorState.Stand,m_Start);
			}
		}
		else if (m_TimeCount < m_jumpouttime)
		{
			Vector3 frompos = m_des;
			frompos.z = m_End.z;
			Vector3 topos = frompos;
			topos.y += 1f;
			Vector3 pos = Vector3.Lerp(frompos,topos,(m_TimeCount-m_statusTime) / (m_jumpouttime-m_statusTime));
			PlayAction(AnimatorState.Stand,pos,true);
		}
		else if (m_TimeCount < m_Duration)
		{
			(m_Skin as RoleSkin).HitByBuildingEffect(HitbyBuilding.HitByBuild1504,false ,0 ,HitEffectMode.CoverBody,null);
			m_Skin.tBody.localScale = new Vector3(1,1,-1);			
			Vector3 topos = m_des;
			topos.z = m_End.z;
			Vector3 pos = Vector3.Lerp(topos,m_des,(m_TimeCount-m_jumpouttime) / (m_Duration-m_jumpouttime));
			PlayAction(AnimatorState.SanNa,pos,true);
			if ((m_TimeCount - m_Delatime) < (m_jumpouttime ) && m_TimeCount > (m_jumpouttime))
			{
				
				Vector3 effectpos = m_Skin.tRoot.position;
				effectpos.y +=0.8f;
				//m_effectgo = SkillEffects._instance.LoadEffect("effect/prefab/", "1906031",effectpos,0.75f);
				GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1906031", effectpos, BattleEnvironmentM.GetLifeMBornNode(true));
				GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(0.75f);
				gae.AddAction(ndEffect);
                m_effectgo = gae.gameObject;
			}
		}
		else if (m_TimeCount >= m_Duration)
		{
			m_Skin.tBody.localScale = new Vector3(1,1,1);
			m_LifePrent.InBoat = true;
		}
	}
	
	public override void SetDone ()
	{
		base.SetDone ();
	}
	
}
