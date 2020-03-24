using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 道具
/// </summary>
/// <author>zhulin</author>
public class SummonPros : Life {

	public Int2 m_Pos
	{
		get{return MapPos;}
		set{MapPos = value;}
	}
	
	private Life m_Parent = null;
	
	private float m_timecount = 0;
	private int m_cout = 0;
	private SummonProsInfo m_info = null;
	private MapGrid m_MapGrid = null;
	
	public RoleSkin m_Skin = null;
	public RoleSkin RoleSkinCom{
		get{return m_Skin;}
	}
	public void CreateSkin(Transform parent,int roleType,string roleName,AnimatorState aniState,bool isplayer)
	{
		m_Skin = new RoleSkin();
		m_Skin.CreateSkin(parent,roleType,roleName,aniState,isplayer);
		
	}
	public override void NDStart()
	{	
		
	}
	/// <summary>
	/// 召唤物
	/// </summary>
	public void SetBorn(Life Parent, int SummonProsID, SummonProsInfo info, MapGrid pos)
	{
		m_Parent = Parent;
		m_info = info;
		m_MapGrid = pos;
		m_Pos = pos.GridPos;
	}

	/// <summary>
	/// FixedUpdate
	/// </summary>
	public override void NDFixedUpdate (float deltaTime)
	{
		
	}
	
	public override void NDUpdate (float deltaTime)
	{
		if (CheckCombatIng())
		{
			if (m_timecount > m_info.m_time)
				return;
			m_timecount += deltaTime;
			if (m_timecount > m_info.m_time)
				GameObject.Destroy(m_thisT.gameObject);
			else
			{
				if (m_timecount > m_cout * m_info.m_timeinterval)
				{
					m_cout ++;
					List<Life> lr = new List<Life>();
					LifeMCamp camp = m_Parent.m_Core.m_Camp == LifeMCamp.ATTACK ? LifeMCamp.DEFENSE : LifeMCamp.ATTACK;
					CM.SearchLifeMListInBoat(ref lr,LifeMType.SOLDIER, camp);
					for(int i = 0; i < lr.Count; i++)
					{
						if (NdUtil.IsSameMapLayer(lr[i].GetMapPos(),m_Pos))
						{
							if (lr[i].m_thisT.localPosition.x  > m_MapGrid.pos.x - m_info.m_range  && lr[i].m_thisT.localPosition.x  < m_MapGrid.pos.x + m_info.m_range  )
							{
								
								SkillReleaseInfo sInfo = new SkillReleaseInfo(); 
								sInfo.m_InterruptSkill = false;
								sInfo.m_MakeStatus = new List<StatusType> ();
								sInfo.m_bImmunity = false;
								sInfo.m_Damage = m_info.m_ReduceAttr.GetAttr(EffectType.RecoHp);
								int fire = m_info.m_ReduceAttr.GetAttr(EffectType.FireAttack);
								if (fire > 0)
									sInfo.m_Damage = Skill.CalcAttributableSkillDamage(AttributeType.Fire, lr[i].m_Attr, sInfo.m_Damage);
								sInfo.Result = AttackResult.Normal;
								lr[i].ApplyDamage(sInfo, null);
							}
						}
					}
				}
			}
		}
	}
	
	
	/// <summary>
	/// 死前干点啥
	/// </summary>
	public override void BeforeDead()
	{
		
	}
	
	//作为寻路目标时，对应的地图格子
	public override MapGrid GetTargetMapGrid()
	{
		return MapGrid.GetMG(m_Pos);
	}
	
	//所在的地图格子
	public override MapGrid GetMapGrid()
	{
		return MapGrid.GetMG(m_Pos);
	}
	
	//所在格子的坐标
	public override Int2 GetMapPos()
	{
		return m_Pos;
	}
	
	/// <summary>
	/// 判断life 是否在视野范围内
	/// </summary>
	/// <param name="life">被观察对象</param>
	/// <returns>在视野范围内 true，否则false</returns>
	public override bool CheckInVision(Life life)
	{
		return true;
	}
	
	public  void OnDestroy() {
		
	}
	
	public override bool ApplyDamage (SkillReleaseInfo Info,Transform attackgo)
	{
		return true;
	}
	
	public override void Dead()
	{
		
	}
	public override void GameOver(bool isWin)
	{
		
	}
}
