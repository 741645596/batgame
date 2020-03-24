using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// life 处于的环境
/// </summary>
public enum LifeEnvironment
{
	Combat    = 0,   //战斗环境
	Edit      = 1,   //编辑环境
	View      = 2,   //查看环境
};

public class Life  {
	public static bool ServerMode = false;
	// 是否在等待服务器
	public bool WaitServer = false;
	// 是否是本地AI还是服务器广播
	public bool islocal = true;
	//是否需要计算AI
	public bool isRequairAi = true;
	protected static LifeEnvironment m_Environment = LifeEnvironment.Combat; 
	public static LifeEnvironment Environment
	{
		get{return m_Environment;}
		set{m_Environment = value;}
	}

	public int m_SceneID = 0;

	public uint m_nBattleTagID=0;//战斗实例标识
	public bool m_isDead;
	public Transform m_thisT;
	public LifeMCore m_Core = new LifeMCore();
	public Skill m_Skill = null;
	public NDAttribute m_Attr = null;
	public Status m_Status = null;
	//public Skin m_Skin = null;
	protected bool  m_Pause = false;

	public virtual HPAciton MyHPAction()
	{
		return GetSkin().MyHPAction();
	}
	
	//	public LifeProperty m_Property;
	public virtual LifeProperty GetLifeProp()
    {
		return null;
	}
	public virtual void SetLifeProp(LifeProperty lifeObj)
	{
	}
	
	//public Skin m_Skin = null;
	public virtual Skin GetSkin()
	{
		return null;
	}
	/// <summary>
	/// 设置Skin
	/// </summary>
	/// <param name="iGameRole">iGameRole</param>
	/// <returns></returns>
	public virtual bool SetSkin()
	{
		return true;
	}

    protected Move m_MoveAI = null;
	public Move MoveAI
	{
		get{return m_MoveAI;}
		set{m_MoveAI = value;}
	}


	public int SceneID
	{
		get{return m_SceneID;}
		set{m_SceneID = value;}
	}
	public uint BattleTagID
	{
		get{return m_nBattleTagID;}
		set{m_nBattleTagID = value;}
	}
	public bool isDead
	{
		get{return m_isDead;}
		set{m_isDead = value;}
	}

	public int fullHP
	{
		get{
            if (m_Attr!=null)
            {
                return m_Attr.FullHp;
            }
            return 0;
        }
	}
	public int HP
	{
        get
        {
            if (m_Attr != null)
            {
                return m_Attr.Hp;
            }
            return 0;
        }
            
		set
        {
            if (m_Attr != null)
            {
                m_Attr.Hp = value;
            }
        }
	}
	//临时放这边的属性
	protected Int2 m_MapPos = Int2.zero;
	public Int2 MapPos
	{
		get { return m_MapPos; }
		set { m_MapPos = value;}
	}
	//所在通道
	public int m_RankDeep = 0;
	public int RankDeep
	{
		get { return m_RankDeep; }
		set { m_RankDeep = value;}
	}
	//所在空间
	private GridSpace m_CurrentGS = GridSpace.Space_DOWN;
	public GridSpace CurrentGS
	{
		get{return m_CurrentGS;}
		set{m_CurrentGS = value;}
	}
	
	private WalkDir  m_WalkDir = WalkDir.WALKLEFT;
	public WalkDir WalkDir
	{
		get{return m_WalkDir;}
		set{
			if (value != WalkDir.WALKSTOP)
			{
				if (m_WalkDir != value && RankDeep > 0 && this is Role)
				{
					m_WalkDir = value;
					(this as Role).RoleWalk.run.ChangeDeep(RankDeep);
				}
				
				m_WalkDir = value;
			}

		}
	}

	
	public void InitHP()
	{
		m_isDead = false;
	}

	public int Anger
	{
		get{return m_Attr.Anger;}
		set{m_Attr.Anger = value;}
	}
    /// <summary>
    /// 是否在船上
    /// </summary>
	private bool m_InBoat = false;
	public bool InBoat
	{
		get{return m_InBoat;}
		set{m_InBoat = value;
			if (value)
			{
				AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_SELFSP),this);
			}
		}
	}

	public  virtual void Pause()
	{
		m_Pause = true;
		if (GetSkin() != null && GetSkin() is RoleSkin && m_thisT != null)
		{
			(GetSkin() as RoleSkin).SetVisable(true);
			(GetSkin() as RoleSkin).PauseAnimator();
			GameObjectActionExcute[] ps = m_thisT.GetComponentsInChildren<GameObjectActionExcute>();
			foreach(GameObjectActionExcute p in ps)
			{
				p.Pause();
			}
		}
	}
	
	public void Contiune()
	{
		m_Pause = false; 
		if (GetSkin() != null && GetSkin() is RoleSkin && m_thisT != null)
		{
			(GetSkin() as RoleSkin).ContiuneAnimator();
			
			GameObjectActionExcute[] ps = m_thisT.GetComponentsInChildren<GameObjectActionExcute>();
			foreach(GameObjectActionExcute p in ps)
			{
				p.Play();
			}
		}
	}

    public virtual void ColliderProc(Collision collision)
    {

	}	
		
	public virtual void NDUpdate (float deltaTime)  
	{
	}


	public bool CheckCombatIng()
	{
		if(Environment != LifeEnvironment.Combat)
			return false;
		if(CombatScheduler.State != CSState.Combat)
			return false;
		if(isDead == true)
			return false ;
		if (m_Pause)
			return false;
		return true;
	}
	
    /// <summary>
    /// 伤害后死亡返回false，否则true
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="attackgo"></param>
	/// <param name="InterruptSkill">是否打断技能</param>
    /// <returns>伤害后死亡返回false，否则true</returns>
	public virtual bool ApplyDamage(SkillReleaseInfo Info,Transform attackgo)
	{
		if (m_isDead)
			return false;
		HP += Info.m_Damage;
		if (HP >= fullHP)
			HP = fullHP;
		if ((Info.Result == AttackResult.Normal && Info.m_Damage != 0) || Info.Result == AttackResult.Miss || Info.Result == AttackResult.Crit|| Info.Result == AttackResult.Fire)
        {
			ShowHp(HP ,fullHP,Info.m_Damage,Info.Result);
        }
		if (HP <= 0)
		{
			if (attackgo != null)
			{
				Life target = attackgo.GetComponent<LifeObj>().GetLife();				
				if (m_Core.m_type == LifeMType.SOLDIER && target.m_Core.m_type == LifeMType.SOLDIER)
				{
					BattleKillLogWnd wnd = WndManager.GetDialog<BattleKillLogWnd>();
					wnd.Show(target.m_SceneID, m_SceneID);
				}				
			}

			Dead();
			return false;
		}
		return true;
	}

	public virtual void StatusUpdateHp(int deltahp)
	{
		ShowHp(HP ,fullHP,deltahp,AttackResult.Normal);
		if (HP <= 0)
		{
			Dead();
		}
	}
    public virtual void StatusUpdateAnger(int deltaanger)
    {
		ShowAnger(deltaanger);
    }
    
	/// <summary>
	/// 死前干点啥
	/// </summary>
   public virtual void BeforeDead()
	{
		/*if (m_hpa != null)
			m_hpa.DestroyHP();*/
	}

	public virtual  void Dead ()
	{
        if (m_isDead)
        {
            return;
        }
		//Debug.Log(this + " dead" + "," + SceneID);
		//炮弹兵死亡
		if(m_Core.m_type == LifeMType.SOLDIER )
		{
			CmCarbon.AddDiePlayerSoldier(m_Core,GetMapPos());
			//Debug.Log("add mana" + m_SceneID + gameObject);
			CmCarbon.AddGodSkillMana(!m_Core.m_IsPlayer ,ConfigM.GetSoldierDeadMana());
			if(m_Core.m_IsPlayer == false)
			{
				GodSkillWnd gsw = WndManager.FindDialog<GodSkillWnd>();
				if (gsw != null) gsw.SetCurMana(CmCarbon.GetGodSkillMana(true));
			}
		}
		//建筑物死亡
		if(m_Core.m_type == LifeMType.BUILD )
		{
			CmCarbon.AddDieBuild(m_Core);
			BattleEnvironmentM.ShowSmoke();
			//Debug.Log("add mana" + m_SceneID + gameObject);
			if (BattleEnvironmentM.GetBattleEnvironmentMode() == BattleEnvironmentMode.CombatPVE)
			{
				int mana = (m_Attr as BuildAttribute).DeadMana;
				CmCarbon.AddGodSkillMana(!m_Core.m_IsPlayer, mana);
			}
			else
			{
				CmCarbon.AddGodSkillMana(!m_Core.m_IsPlayer, ConfigM.GetBuildDeadMana());
			}
			
			if(m_Core.m_IsPlayer == false)
			{
				GodSkillWnd gsw = WndManager.FindDialog<GodSkillWnd>();
				if (gsw != null) gsw.SetCurMana(CmCarbon.GetGodSkillMana(true));
			}
		}
		m_isDead = true;
        BeforeDead();
		//SkillEffects._instance.LoadEffect("effect/prefab/", "1052061",transform.position,1f);
		LifeMCamp Camp = (m_Core.m_Camp == LifeMCamp.ATTACK) ? LifeMCamp.DEFENSE :LifeMCamp.ATTACK;
		List<Life> RoleList = new List<Life>();
		CM.SearchLifeMListInBoat(ref RoleList,LifeMType.SOLDIER | LifeMType.SUMMONPET,Camp);
		AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_TD),RoleList);
		MapM.EmptyRoleStations(m_SceneID,m_Core.m_type);
		CM.ExitCombat(m_Core,m_SceneID);
		

		DeadEffect();

		if( this is Building1300)
		{

		}
		else Destroy();
	}



	public virtual void DeadEffect()
	{
		if (this is Role)
		{
			RoleSkin skin = GetSkin() as RoleSkin;
			Vector3 pos = Vector3.zero;
			GameObject posgo = GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectPos);
			if (posgo != null)
			{
				pos = posgo.transform.position;
			}
			pos.z = 1.5f;
			//SkillEffects._instance.LoadEffect("effect/prefab/", "2000231",pos,1f);
            GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "2000231", pos, BattleEnvironmentM.GetLifeMBornNode(true));
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
			gae.AddAction(ndEffect);
		}
		else if (this is Building )
		{
			Vector3 pos = GetPos();
			pos.y += 1.5f;
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "2000271", pos, BattleEnvironmentM.GetLifeMBornNode(true));
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
			gae.AddAction(ndEffect);
		}
	}
	//作为寻路目标时，对应的地图格子
	public virtual MapGrid GetTargetMapGrid()
	{
		return null;
	}

	//所在的地图格子
	public virtual MapGrid GetMapGrid()
	{
		return null;
	}

	public virtual List<MapGrid> GetAllMapGrid()
	{
		List<MapGrid> l = new List<MapGrid>();
		l.Add(GetMapGrid());
		return l;
	}
	//所在格子的坐标
	public virtual Int2 GetMapPos()
	{
		return Int2.zero;
	}

	public virtual Vector3 GetPos()
	{
		return m_thisT.position;
	}

	/// <summary>
	/// 判断life 是否在视野范围内
	/// </summary>
	/// <param name="life">被观察对象</param>
	/// <returns>在视野范围内 true，否则false</returns>
	public virtual bool CheckInVision(Life life)
	{
		if(life == null )
			return false;
		return true;
	}


	public virtual void SetMoveState(MoveState state)
	{
		m_Core.m_MoveState = state;
		CM.JoinCombat(SceneID,this,m_Core);
        if (state == MoveState.Walk)
        {
            InBoat = true;
        }
        else if (state == MoveState.Fly)
        {
            InBoat = false;
        }
	}
	

	/// <summary>
	/// 设置lifeM 核心结构
	/// </summary>
	/// <returns></returns>
	public virtual int SetLifeCore(LifeMCore Core)
	{
		m_SceneID = NdUtil.GetSceneID();
		m_Core.Copy(Core);
		CM.JoinCombat(m_SceneID, this,Core );
		return m_SceneID;
	}

	

	public static SkillReleaseInfo CalcDamage(Life Attack, Life Defense ,GridActionCmd DefenseAction,SkillInfo skill)
	{
		return Attack.m_Skill.SkillRelease(Attack, Defense ,DefenseAction,skill);
	}




	public static float CalcDistance(Life a , Life b)
	{
		if(a.m_thisT == null || b.m_thisT == null) 
			return 0;
		if (a is Role)
			return Mathf.Abs(a.m_thisT.localPosition.x - b.m_thisT.localPosition.x) + Mathf.Abs(a.GetMapPos().Layer - b.GetMapPos().Layer)*100 ;
		else
		{
			float d = int.MaxValue;
			List<MapGrid> lg = a.GetAllMapGrid();
			foreach(MapGrid g in lg)
			{
				float temp = Mathf.Abs(g.pos.x - b.m_thisT.localPosition.x) +  Mathf.Abs(g.GridPos.Layer - b.GetMapPos().Layer)*100 ;
				if (d < temp)
					d = temp;
			}
			return d;
		}
	}

	public static float CalcLineDistance(Life a , Life b)
	{
		if(a.m_thisT == null || b.m_thisT == null) 
			return 0;
		if (a is Role)
			return Vector3.Distance(a.m_thisT.localPosition, b.m_thisT.localPosition);
		else
		{
			float d = int.MaxValue;
			List<MapGrid> lg = a.GetAllMapGrid();
			foreach(MapGrid g in lg)
			{
				float temp = Vector3.Distance(g.pos , b.m_thisT.localPosition);
				if (d < temp)
					d = temp;
			}
			return d;
		}
	}	
	/// <summary>
	/// 撞击处理
	/// </summary>
	/// <param name="Damage">撞击伤害</param>
	/// <param name="dir">撞击方向</param>
	/// <param name="Info">返回撞击信息</param>
	/// <returns></returns>
	public virtual void CollisionProcByFire(Collision collision,int Damage/*,Vector2 dir*/,ref FlyCollisionInfo Info,FlyDir flydir)
	{ 
	}
		
	public virtual void KillSelf(float delay = 0)
    {
		Dead();
		isDead = true;
    }

	public virtual void SetLifeData()
	{

	}
	
	public virtual void SetDark(bool bDark)
	{
	}

	/// <summary>
	/// 能否作为攻击目标
	/// </summary>
	/// <param name="Target">返回撞击信息</param>
	/// <returns>true 可以做为攻击目标，false 不能做为攻击目标</returns>
	public bool CanAttackTarget(Life Target)
	{
		if(Target == null || Target.m_Attr == null)
			return false;
		if(m_Core == null) return false;

		if(m_Core.m_type == LifeMType.SOLDIER)
			return !NDAttribute.CheckTargetImmune(ImmuneTarget.Soldier,Target.m_Attr.TargetImmune);
		else if(m_Core.m_type == LifeMType.BUILD)
			return !NDAttribute.CheckTargetImmune(ImmuneTarget.Build,Target.m_Attr.TargetImmune);
		else if(m_Core.m_type == LifeMType.PET)
			return !NDAttribute.CheckTargetImmune(ImmuneTarget.Pet,Target.m_Attr.TargetImmune);
		else 
			return false;
	}


	/// <summary>
	/// 2个对象是否处于同一层
	/// </summary>
	public static bool InSameLayer(Life Self ,Life Target)
	{
		if(Self == null || Target == null )
			return false;
		if (!Target.InBoat)
			return false;
		Int2 p1 = Self.GetMapPos();
		Int2 p2 = Target.GetMapPos();
		return (p1.Layer == p2.Layer) ? true : false;
	}


	/// <summary>
	/// 2个对象是否处于同一层
	/// </summary>
	public static bool InSameLayer(MapGrid TargetMg ,Life l)
	{
		if(TargetMg == null || l == null )
			return false;
		if (!l.InBoat)
			return false;
		Int2 p1 = TargetMg.GridPos;
		Int2 p2 = l.GetMapPos();
		return (p1.Layer == p2.Layer) ? true : false;
	}

	/// <summary>
	/// 在圆形范围内 
	/// </summary>
	public static bool IsInCircle(Life Self ,Life Target,float Radius)
	{
		if(Self == null || Target == null )
			return false;
		if (!Target.InBoat)
			return false;
		if(InSameLayer (Self,Target) == false)
			return false;

		Vector3 P1 = Self.GetPos();
		Vector3 p2 = Target.GetPos(); 


		float distance = Vector2.Distance(P1,p2);

		if(distance <= Radius) 
			return true;
		else return false;
	}

	/// <summary>
	/// 在圆形范围内 
	/// </summary>
	public static bool IsInCircle(MapGrid TargetMg ,Life l,float Radius)
	{
		if(TargetMg == null || l == null )
			return false;
		if (!l.InBoat)
			return false;
		if(InSameLayer (TargetMg,l) == false)
			return false;
		
		Vector3 P1 = TargetMg.WorldPos;
		Vector3 p2 = l.GetPos(); 
		
		
		float distance = Vector2.Distance(P1,p2);
		
		if(distance <= Radius) 
			return true;
		else return false;
	}
	
	/// <summary>
	/// 在球形范围内 
	/// </summary>
	public static bool IsInBall(Life Self ,Life Target,float Radius)
	{
		if(Self == null || Target == null )
			return false;
		
		if (!Target.InBoat)
			return false;
		Vector3 P1 = Self.GetPos();
		Vector3 p2 = Target.GetPos();
		
		float distance = Vector2.Distance(P1,p2);
		
		if(distance <= Radius) 
			return true;
		else return false;
	}


	/// <summary>
	/// 获取2个对象的距离 
	/// </summary>
	public static float GetfDistance(Life Self ,Life Target)
	{
		if(Self == null || Target == null )
			return 0.0f;
		Vector3 P1 = Self.GetPos();
		Vector3 p2 = Target.GetPos();
		
		return Vector2.Distance(P1,p2);
	}
	/// <summary>
	/// 获取2个对象的距离 
	/// </summary>
	public static Vector2 GetDistance(Life Self ,Life Target)
	{
		if(Self == null || Target == null )
			return Vector2.zero;
		Vector3 P1 = Self.GetPos();
		Vector3 p2 = Target.GetPos();

		float x = Mathf.Abs(P1.x - p2.x);
		float y = Mathf.Abs(P1.y - p2.y);
		
		return new Vector2(x ,y);
	}


	/// <summary>
	/// 获取2个对象之间的夹角 
	/// </summary>
	public static float GetAngle(Life Self ,Life Target)
	{
		Vector3 from = Self.GetCenterPos();
		Vector3 to = Target.GetCenterPos();
		return NdUtil.V2toAngle(from, to,Vector3.right);
	}


	/// <summary>
	/// 在球形范围内 
	/// </summary>
	public static bool IsInBall(MapGrid TargetMg ,Life l,float Radius)
	{
		if(TargetMg == null || l == null )
			return false;
		
		if (!l.InBoat)
			return false;
		Vector3 P1 = TargetMg.WorldPos;
		Vector3 p2 = l.GetPos();
		
		float distance = Vector2.Distance(P1,p2);
		
		if(distance <= Radius) 
			return true;
		else return false;
	}

	public virtual void GameOver(bool isWin)
	{
	}
	/// <summary>
	/// 获取对象中心点，对于建筑物，在最下层的中心点
	/// </summary>
	public virtual Vector3 GetCenterPos()
	{
		return GetPos();
	}

	/// <summary>
	/// 关联表现transform
	/// </summary>
	public void LifeProperty(LifeProperty obj)
	{
		SetLifeProp (null);
		//m_Property = null;
		m_thisT = null;
		if(obj != null)
		{
			SetLifeProp (obj);
			//m_Property = obj;
			if(this is Role || this is Pet || this is Building)
			{
				m_thisT = obj.transform.parent;
			}
			else m_thisT = obj.transform;
			obj.SetLife(this);
		}
	}

	


	public void ShowHp(int Hp ,int FullHp, int demage ,AttackResult result)
	{
		if(GetSkin() != null)
		{
			GetSkin().ShowHP(Hp ,FullHp ,demage,result);
		}
	}

	public void ShowBuff(string StatusName)
	{
		if (GetSkin() != null)
		{
			GetSkin().ShowBuff(StatusName);
		}
	}

	public void ShowDebuff(string StatusName)
	{
		if (GetSkin() != null)
		{
			GetSkin().ShowDebuff(StatusName);
		}
	}

	public void ShowAnger(int deltaanger)
	{
		if(GetSkin() != null)
		{
			GetSkin().ShowAnger(deltaanger);
		}
	}

	public  virtual void NDStart ()
	{
	}

	/// <summary>
	/// FixedUpdate
	/// </summary>
	public virtual  void NDFixedUpdate (float deltaTime)
	{

	}
	/// <summary>
	/// LateUpdate
	/// </summary>
	public  virtual void NDLateUpdate(float deltaTime) 
	{

	}

	public void Destroy()
	{
		if(m_thisT != null)
		{
			GameObject.Destroy(m_thisT.gameObject);
		}
	}
    

	/// <summary>
	/// 定义自己的获取脚本的接口
	/// bWantActiveself 是否有显示有要求
	/// </summary>
	public  T GetComponent<T>()
	{
		if(GetLifeProp() != null)
		{
			return (T) (object) GetLifeProp().gameObject.GetComponent(typeof(T));
		}
		return (T) (object)null;
	}


	/// <summary>
	/// 定义自己的获取脚本的接口
	/// bWantActiveself 是否有显示有要求
	/// </summary>
	public  T AddComponent<T>()
	{
		if(GetLifeProp() != null)
		{
			return (T) (object) GetLifeProp().gameObject.AddComponent(typeof(T));
		}
		return (T) (object)null;
	}


	public  T GetComponentInChildren<T>()
	{
		if(GetLifeProp() != null)
		{
			return (T) (object) GetLifeProp().gameObject.GetComponentInChildren(typeof(T));
		}
		return (T) (object)null;
	}

	

	public void ShowHP(int RoomID,int bear)
	{
		if(GetSkin() != null)
		{
			GetSkin().ShowHP(RoomID,HP,fullHP,bear);
		}
	}

	public void DestroyHp()
	{
		if(GetSkin() != null)
		{
			GetSkin().DestroyHp();
		}
	}

	public  void SetActive(bool active)
	{
		if(GetLifeProp() != null)
		{
			GetLifeProp().SetActive(active);
		}
	}
}
