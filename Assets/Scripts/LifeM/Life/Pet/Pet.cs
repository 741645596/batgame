
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#define UNITY_EDITOR_LOG
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum PetAnimatorState{
	
	PetAppear = 90000,
	PetDown = 90100,
	PetWalk = 90200,
}
/// <summary>
/// 召唤物
/// </summary>
/// <author>zhulin</author>
public class Pet : Life {
	/// <summary>
	/// 召唤物老大
	/// </summary>
	public PetInfo m_Info;
	public int m_PetType = 0;
	public Life m_Parent = null;
	public Int2 m_Pos
	{
		get{return MapPos;}
		set{MapPos = value;}
	}
	private float  m_CoutTime = 0.0f;
	protected float  m_TotalTime = 0.0f;
	public WalkDir m_dir;
    public GridActionCmd m_CurrentAction = null;
	public PetMove m_move;
	public PetMove PetMoveAI
	{
		get{return m_move;}
	}
    public Life m_target;
    /// <summary>
    /// 记录炮战作用对象的ID
    /// </summary>
    public List<int> m_listCollisionGoID = new List<int>();
	
	public PetInfo Info
	{
		get{return m_Info;}
		set{m_Info = value;}
	}

	public void SetPetLife(PetInfo Info,LifeEnvironment Environment)
	{
		if(Info == null)
			return ;
	}


	public override void NDStart()
	{	
		if(Environment == LifeEnvironment.Combat)
		{
			CombatInit();
		}
	}
	
	public RolePropertyM m_Property;	
	public override LifeProperty GetLifeProp()
	{
		return m_Property;
	}
	public override void SetLifeProp(LifeProperty lifeObj)
	{
		m_Property = lifeObj as RolePropertyM;
	}
	public PetSkin m_Skin = null;
	public override Skin GetSkin()
	{
		return m_Skin;
	}
	
	public PetSkin PetSkinCom{
		get{return m_Skin;}
	}
	
	public void CreateSkin(Transform parent,int roleType,string roleName,AnimatorState aniState,bool isplayer = true)
	{
		m_Skin = new PetSkin();
		m_Skin.CreateSkin(parent,roleType,roleName,aniState,isplayer);
		
	}
	public override bool SetSkin()
	{
		//m_Skin = new PetSkin();
		if(m_Skin != null)
		{
			m_Skin.SetGameRole(this, MoveState.Walk ,m_Core.m_Camp, m_Core.m_IsPlayer);
		}
		
		return true;
	}	

	public  void CombatInit()
	{
		SetLifeData();
		if (m_Attr!=null)
		{
			m_TotalTime = m_Attr.LifeTime * 0.001f;
		}
		
		RoleSkin();
		EventCenter.RegisterHooks(NDEventType.StatusCG, SetStatus);
		
		if (m_PetType == 1002)
		{
			InitHP();
			HP = fullHP;
			m_TotalTime = float.MaxValue;
		}
		if (m_PetType == 1003)
		{
			
		}
	}

	public void SetMoveState(MoveState state)
	{

		m_Core.m_MoveState = state;
		if (m_Core.m_MoveState == MoveState.Fly)
		{
			
			if (m_PetType == 1002)
			{
				m_move = new PetFly1002();
				m_move.Init(this);
				m_Skill = new FireSkill();
				m_Skill.Init(m_SceneID,m_Parent.m_Core);
			}
		}
		else
		{
			
			if (m_PetType == 1002)
			{
				m_move = new PetWalk1002();
				m_move.Init(this);
				
				(PetMoveAI as PetWalk1002).m_petState = Pet1002State.Follow;
			}
			
			PetInfo Info = CmCarbon.GetPetInfo(m_Core);
			if (Info.m_skill1 > 0)
			{
				m_Skill = new PetSkill();
				m_Skill.Init(m_SceneID,m_Core);
			}
		}
	}
	public void SendToGrid(Vector3 start,Vector3 startTo,Vector3 endFr,Vector3 end,float duration,MapGrid m)
	{
		if (null != m_CurrentAction) 
		{
			m_CurrentAction.SetDone ();
		}
		m_CurrentAction = new GridActionCmdSendToPos(this,m_thisT.localPosition,startTo,endFr, end, duration, m_dir,1,m);
		//MapM.EmptyRoleStations(m_SceneID);
		
	}

	/// <summary>
	/// FixedUpdate
	/// </summary>
	public override void NDFixedUpdate (float deltaTime)
	{
		if(CheckCombatIng () == true)
		{
			if (PetMoveAI != null)
				PetMoveAI.FixUpdate();
		}
	}

	public override void NDUpdate (float deltaTime)
	{
		if(CheckCombatIng () == true)
		{
			m_CoutTime += deltaTime;
			if(m_CoutTime > m_TotalTime)
			{
				Dead();
			}
			//更新宠物效果
			m_Skin.UpdataSkinEffect();
			//PetUpdate();
			if (PetMoveAI != null)
				PetMoveAI.Update();
			if (m_Skill != null)
				m_Skill.Update(deltaTime);
		}
	}
	

	/// <summary>
	/// 召唤物出生
	/// </summary>
	public void SetBorn(Life Parent,int PetID,int pettype,Int2 Grid ,WalkDir dir,List<PathData> Path = null)
	{
		if(Parent == null)
		{
			Dead();
			Debug.Log("需消费对象");
			return;
		}
		m_PetType = pettype;
		m_dir = dir;
		m_Parent = Parent;
		m_Pos = Grid;
		SetPath(Path);
		SetLifeCore(new LifeMCore(PetID ,Parent.m_Core.m_IsPlayer,LifeMType.PET,Parent.m_Core.m_Camp,MoveState.Walk));
		SetMoveState(MoveState.Walk);
	}

    /// <summary>
    /// 飞行召唤物出生
    /// </summary>
	public void SetBorn(Life Parent, int PetID,int pettype, Vector3 pos)
    {
        if (Parent == null)
        {
            Dead();
            Debug.Log("需消费对象");
            return;
		}
		m_PetType = pettype;
        m_Parent = Parent;
		SetLifeCore(new LifeMCore(PetID,Parent.m_Core.m_IsPlayer, LifeMType.PET, Parent.m_Core.m_Camp,MoveState.Fly));
		
		SetMoveState(MoveState.Fly);
    }

	public override void SetLifeData()
	{
		if(m_Core == null) return ;
		if(m_Parent != null)
		{
			PetAttribute Attr = new PetAttribute();
			Attr.GetAttrData(m_Parent.m_Attr,m_Core);
			m_Attr = Attr;
			m_Attr.Init(SceneID,m_Core,this);
			//
			m_Status = new Status();
			m_Status.Init(SceneID);
			//宠物出生产生状态。
			List<SkillStatusInfo> l = (m_Attr as PetAttribute).GetStatusInfo();
			if(l != null && l.Count > 0)
			{
				for(int i = 0; i < l.Count ; i++)
				{
					if(l[i] != null)
					{
						m_Status.AddStatus(SceneID,0,l[i]);
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
		DestroyHp();
		Destroy();
	}


	public virtual void SetPath(List<PathData> Path)
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
	/// 计算宠物伤害
	/// </summary>
	protected int PetDamage(NDAttribute TargetAttr)
	{
		if(TargetAttr == null || m_Attr == null )
			return 0;
		if(m_Attr.PetAttackType == 1)     //物理攻击
		{
			return ScriptM.Formula<int>("CALC_SOLDIER_PHYDAMAGE",
			                            1,
			                            m_Attr.PhyAttack,
			                            m_Attr.CutPhyDefend,
			                            TargetAttr.PhyDefend,
			                            TargetAttr.CutPhyDamage,
			                            0);
		}
		else if(m_Attr.PetAttackType ==  2) // 魔法攻击
		{
			return ScriptM.Formula<int>("CALC_SOLDIER_MAGICDAMAGE",
			                            1,
			                            m_Attr.MagicAttack,
			                            m_Attr.CutMagDefend,
			                            TargetAttr.MagicDefend,
			                            TargetAttr.CutMagDamage,
			                            0);
		}
		else if(m_Attr.PetAttackType ==  3) // 神圣技能
		{
			return m_Attr.PetDamage;
		}
		else
		{
			return 0;
		}
	}

	/// <summary>
	/// 判断life 是否在视野范围内
	/// </summary>
	/// <param name="life">被观察对象</param>
	/// <returns>在视野范围内 true，否则false</returns>
	public override bool CheckInVision(Life life)
	{
		if(life == null || life.m_Attr == null)
			return false;
		if(life.m_Attr.IsHide == true)
			return false;
		return true;
	}

	public virtual void OnDestroy() {
		EventCenter.AntiRegisterHooks(NDEventType.StatusCG, SetStatus);
	}
	/// <summary>
	/// 设置宠物状态效果
	/// </summary>
	/// <param name="Param">状态信息结构</param>
	public void SetStatus(int SceneID,object Param)
	{
		if(SceneID != this.SceneID ) return ;
		
		if(Param is StatusInfo)
		{
			StatusInfo Info = Param as StatusInfo;
			if(Info.State == StatusState.Add)
			{
				SetSkillStatus(Info);
			}
			else if(Info.State == StatusState.Remove)
			{
				RemoveStatus(Info);
			}
			
		}
	}
	/// <summary>
	/// 添加宠物效果
	/// </summary>
	/// <param name="Info">状态信息结构</param>
	protected virtual void SetSkillStatus(StatusInfo Info )
	{
		if(Info == null) return ;
		#if UNITY_EDITOR_LOG
		Debug.Log("add status: " + Info.Type + "," + Info.effectid + "," + Info.position + "," + Info.time);
		#endif
	}

	/// <summary>
	/// 移除宠物信息
	/// </summary>
	/// <param name="Info">状态信息结构</param>
	protected virtual void RemoveStatus(StatusInfo Info)
	{
		if(Info == null) return ;
		#if UNITY_EDITOR_LOG
		Debug.Log("remove status: " + Info.Type + "," + Info.effectid + "," + Info.position + "," + Info.time);
		#endif
	}
	/// <summary>
	/// 检测是否为宠物的主人
	/// </summary>
	/// <param name="Parent">待判断宠物主人对象</param>
	public bool CheckParent(Life Parent)
	{
		if(m_Parent == null || Parent == null)
			return false;
		if(m_Parent == Parent) return true;
		else return false;
	}

	/// <summary>
	/// 角色皮肤设置 （碰撞体、颜色、血条等）
	/// </summary>
	public  void RoleSkin()
	{
		if (m_isDead ) 
			return ;
		if(m_Skin != null)
		{
			m_Skin.ResetSkin();
		}
	}

	public override void CollisionProcByFire(Collision collision,int Damage/*,Vector2 dir*/, ref FlyCollisionInfo Info,FlyDir flydir)
    {
    }


    /// <summary>
    /// 获取炮战对象
    /// </summary>
    public Life GetCollisionGo(Collision collision)
    {
        Transform T = collision.transform;
        int collisionGoID = T.gameObject.GetInstanceID();
        if (m_listCollisionGoID.Contains(collisionGoID))
            return null;
        //m_listCollisionGoID.Add(collisionGoID);
        //墙板，建筑等对象
		LifeProperty lifeobj = T.GetComponent<LifeProperty>();
        //角色对象
		if (lifeobj == null)
        {
            Transform t = T.parent;
            if (t == null) return null;
			lifeobj = t.GetComponent<LifeProperty>();
			if (lifeobj == null) return null;
			Life life = lifeobj.GetLife() as Life;
			if (life.m_Core.m_Camp == LifeMCamp.DEFENSE)
            {
                m_listCollisionGoID.Add(collisionGoID);
            }
			return lifeobj.GetLife() as Life;
        }
        else
		{
			Life life = lifeobj.GetLife() as Life;
			if (life.m_Core.m_Camp == LifeMCamp.DEFENSE)
            {
                m_listCollisionGoID.Add(collisionGoID);
            }
			return lifeobj.GetLife() as Life;
        }
    }

	public override bool ApplyDamage (SkillReleaseInfo Info,Transform attackgo)
	{
		//base.ApplyDamage (Info, attackgo);
		return true;
	}
	
	public override void Dead()
	{
		base.Dead();
		if (m_PetType == 1002)
			(m_Parent as Role).CurPet = null;
	}
	public override void GameOver(bool isWin)
	{
		base.GameOver(isWin);
		if (m_CurrentAction != null)
			m_CurrentAction.SetDone();
		if (isWin)
		{
			m_CurrentAction = new GridActionCmdWin();
			
		}
		else
		{
			m_CurrentAction = new GridActionCmdFaile();
		}
		
		m_CurrentAction.SetTarget(this);
		m_CurrentAction.Update();
	}
	/// <summary>
	/// 炮战碰撞处理
	/// </summary>
	public  override void ColliderProc (Collision collision)
	{
		
		if(PetMoveAI != null)
			PetMoveAI.ColliderProc(collision);

	}
}
