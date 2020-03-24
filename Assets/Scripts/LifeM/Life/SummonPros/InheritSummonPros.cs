using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 道具
/// </summary>
/// <author>zhulin</author>
public class InheritSummonPros : Life
{

	public Int2 m_Pos
	{
		get{return MapPos;}
		set{MapPos = value;}
	}
	
	private Life m_Parent = null;
	
	private float m_timecount = 0;
	private int m_cout = 0;
	private PetInfo m_info = null;
	private MapGrid m_MapGrid = null;	
	public GridActionCmd m_AppearAction;
	public RoleSkin m_Skin = null;
	public RoleSkin RoleSkinCom{
		get{return m_Skin;}
	}
	public InheritSummonProsSkill InheritSummonProsSkillCom{
		get{return m_Skill as InheritSummonProsSkill;}
	}
	public override Skin GetSkin()
	{
		return m_Skin;
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
	public void SetBorn(Life Parent, int SummonProsID, PetInfo info, MapGrid pos)
	{
		m_Parent = Parent;
		m_info = info;
		m_MapGrid = pos;
		m_Pos = pos.GridPos;
		m_Skill = new InheritSummonProsSkill(info,this);
		m_Attr = new InheritSummonProsAttribute();
		m_SceneID = NdUtil.GetSceneID();
		m_Core = new LifeMCore(info.m_id, Parent.m_Core.m_IsPlayer, LifeMType.INHERITSUMMONPROS, Parent.m_Core.m_Camp, MoveState.Walk);
		m_Skill.Init(m_SceneID, m_Core);
		(m_Attr as InheritSummonProsAttribute).InheritInit(m_SceneID, m_Core, this, Parent);
		SetLifeCore(m_Core);
		InBoat = false;
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
			if (m_AppearAction == null || m_AppearAction.IsDone())
			{
				InBoat = true;
				if (m_timecount > m_info.LifeTime * 0.001f)
					return;
				m_timecount += deltaTime;
				if (m_timecount > m_info.LifeTime * 0.001f)
				{
					Dead();
					InheritSummonProsSkillCom.CheckConditionSkill();
				}
				m_Skill.Update(deltaTime);
			}
			else
				m_AppearAction.Update();

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
	

	
	public override bool ApplyDamage (SkillReleaseInfo Info,Transform attackgo)
	{
		return true;
	}
	
	public override void Dead()
	{
		base.Dead();

	}
	public override void GameOver(bool isWin)
	{
		
	}


	public void CreateAppearAction()
	{

	}

	public  void HitFly(WalkDir Dir,float duration,float delay, bool bearhit = false)
	{
		InBoat = false;
		Vector3 pos =GetPos();
		GameObjectActionExcute gaeTarget = EffectM.LoadEffect(EffectM.sPath, "1917011", pos, null);
		GameObjectActionSpawn newSpawn = new GameObjectActionSpawn();
		gaeTarget.AddAction(newSpawn);
		GameObjectActionDelayDestory ndEffectTarget = new GameObjectActionDelayDestory(duration);
		GameObjectActionBezierMove nBezoerMove = new GameObjectActionBezierMove(pos,duration,Dir);
		newSpawn.AddAction(ndEffectTarget);
		newSpawn.AddAction(nBezoerMove);
		Dead();
		//SetAnimator (Build_AnimatorState.Trigger30000);
		//m_Skill.ReSetCDTime();
		//m_bHitFly = true;
	}
}
