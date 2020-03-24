using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PetEnemyType
{
	None = 0,
	Build = 1,
	SOLDIER = 2,
}
/// <summary>
/// 召唤物
/// </summary>
/// <author>zhulin</author>
public class SummonPet : Role {
	Int2 m_bornpos = Int2.zero;
	bool isAppear = false;
	PetEnemyType m_EnemyType = PetEnemyType.Build;	
	public RoleSkin m_Skin = null;
	public RoleSkin RoleSkinCom{
		get{return m_Skin;}
	}
	public void CreateSkin(Transform parent,int roleType,string roleName,AnimatorState aniState,bool isplayer)
	{
		m_Skin = new RoleSkin();
		m_Skin.CreateSkin(parent,roleType,roleName,aniState,isplayer);
		
	}
	/// <summary>
	/// 召唤物出生
	/// </summary>
	public void SetBorn(Life Parent,int PetID,int pettype,Int2 Grid ,WalkDir dir,List<PathData> Path = null)
	{
		if(Parent == null)
		{
			Dead();
			//Debug.Log("需消费对象");
			return;
		}
		SetLifeCore(new LifeMCore(PetID ,Parent.m_Core.m_IsPlayer,LifeMType.PET,Parent.m_Core.m_Camp,MoveState.Walk));
		SetMoveState(MoveState.Walk);
	}

	public void SetEnemyType(PetEnemyType type)
	{
		m_EnemyType = type;
	}

	public void SetSummonPetLife(SummonpetInfo Info, LifeProperty SkinProperty,LifeEnvironment Environment)
	{
		if(Info == null || SkinProperty == null) return ;
		LifeProperty(SkinProperty);
		if(Environment == LifeEnvironment.Combat)
		{
			
		}
		else
		{
			
		}
	}
	public override void InitSkill()
	{
		if(m_Core.m_MoveState == MoveState.Fly)
		{
			m_Skill = new FireSkill();
			m_Skill.Init(SceneID,m_Core);
		}
		else if(m_Core.m_MoveState == MoveState.Walk)
		{
			m_Skill = new SummonPetSkill();
			m_Skill.Init(SceneID,m_Core);
		}
	}
	public override void DeadEffect ()
	{
		RoleSkin skin = m_Skin as RoleSkin;
		
		GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
		Vector3 pos = posgo.transform.position;
		pos.z = 1.5f;
		GameObject go =null;
		GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath,"2000811" , pos, BattleEnvironmentM.GetLifeMBornNode(true));
		GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(2f);
		gae.AddAction(ndEffect);
		go = gae.gameObject;
	}
	public override void NDFixedUpdate (float deltaTime)
	{
		
		if (isAppear)
		{
			if (CurrentAction.IsDone())
			{
				isAppear = false;
				CurrentAction = null;
				MoveAI.SetBornPos(m_bornpos,0);
			}
			else
				CurrentAction.Update();
		}
	}
	public override void NDUpdate (float deltaTime)
	{
		if (!isAppear)
		{
			CheckDead();
			base.NDUpdate (deltaTime);
		}
	}
	public override void NDLateUpdate (float deltaTime)
	{
		if (!isAppear)
		{
			base.NDLateUpdate (deltaTime);
		}
	}
	public void CheckDead()
	{
		LifeMCamp camp = m_Core.m_Camp == LifeMCamp.ATTACK? LifeMCamp.DEFENSE : LifeMCamp.ATTACK;
		List<Life> lb = new List<Life>();
		int count = 0;
		if(m_EnemyType == PetEnemyType.Build)
		{
			CM.SearchLifeMListInBoat(ref lb,LifeMType.BUILD,camp );
			foreach(Building b in lb)
			{
				if (b.m_Attr.IsDamage && !b.m_Attr.IsResource)
				{
					count ++;
				}
			}
		}
		else if(m_EnemyType == PetEnemyType.SOLDIER)
		{
			CM.SearchLifeMListInBoat(ref lb,LifeMType.SOLDIER,camp );
			foreach(Role b in lb)
			{
				count ++;
			}
		}

		if(count <= 0)
			Dead();
	}

	public  void SetBornPos(Int2 BornPos ,int deep)
	{
		if (m_Attr.AttrType == 3000)
		{
			isAppear = true;
			CurrentAction = new GridActionCmd3000Appear(MapGrid.GetMG(BornPos).WorldPos);
			CurrentAction.SetTarget(this);
		}
		else if(m_Attr.AttrType == 200003)
		{

			isAppear = true;
			m_CurrentAction = new GridActionCmd200003FindTarget();
			m_CurrentAction.SetTarget(this);

		}
		else
		{
			
			isAppear = false;
			CurrentAction = null;
			MoveAI.SetBornPos(m_bornpos,0);
		}
		m_bornpos = BornPos;
	}
}
