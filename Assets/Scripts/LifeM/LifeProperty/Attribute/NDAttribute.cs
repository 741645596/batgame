#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#define UNITY_EDITOR_LOG
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 属性模块
/// </summary>
/// <author>zhulin</author>
///
/// <summary>
/// 目标免疫
/// </summary>
[System.Flags]
public enum ImmuneTarget
{
	Normal  = 0x00,  //被所有的进行攻击
	Soldier = 0x01,  //不会被炮弹兵选为目标
	Pet     = 0x02,  //不会被宠物选为目标
	Build   = 0x04,  //不会被建筑选为目标
	ALL     = Soldier | Pet | Build,   //免疫所有
}

public enum ReleaseType
{
	Normal       = 0x00, //正常
	NoAttack     = 0x01, //不能对敌使用技能
	NoMagic      = 0x02, //不能使用魔法技能
	NoALL        = 0x03  //所有技能不能释放     
};

/// <summary>
/// 技能免疫
/// </summary>
[System.Flags]
public enum ImmuneSkill
{
	Normal       = 0x00, //正常
	PhySkill     = 0x01, //免疫物理技能
	MagicSkill   = 0x02, //免疫魔法技能
	SacredSkill  = 0x04, //免疫神圣技能
	FireSkill    = 0x08, //免疫炮战技能
	Invincible   = PhySkill | MagicSkill | SacredSkill | FireSkill, //无敌，免疫所有技能
}

public enum NDAttrKeyName
{
	DeadMP_Int , //死亡怒气
}
public class NDAttribute  {

	
	public Dictionary<NDAttrKeyName,int> m_Intdic = new Dictionary<NDAttrKeyName, int>();
	

	public virtual bool TryGetIntValue(NDAttrKeyName key,ref int value)
	{

		return m_Intdic.TryGetValue(key,out value);
	}

	public virtual void SetValue(NDAttrKeyName key, int value)
	{
		//if (m_Intdic.ContainsKey(key))
		{
			m_Intdic[key] = value;
		}
	}
	
	public virtual int GetintValue(NDAttrKeyName key)
	{
		return m_Intdic[key];
	}

	public virtual int GetintValueDefalt(NDAttrKeyName key)
	{
		
		if (m_Intdic.ContainsKey(key))
		{
			return m_Intdic[key];
		}

		return 0;
	}


	//定时器
	protected float m_times = 0.0f;
	protected int m_SceneID = -1;	
	protected Life m_Parent = null;

	public void  SetLifeData(int SceneID, Life Parent)
	{
		m_SceneID = SceneID;
		m_Parent = Parent;
	}
	public bool idDead()
	{
		return m_Parent.isDead;
	}

	public string AttrName 
	{
		get{return m_Parent.GetLifeProp().name;}
	}
	/// <summary>
	/// 核心数据
	/// </summary>
	protected int m_AttrType = 0;
	public int AttrType{
		get{return m_AttrType;}
	}
	/// <summary>
	/// 属性变更，该字段变化会引起效果变化，属于核心字段
	/// </summary>
	protected bool m_AttrChange = false;
	public bool AttrChange{
		get{return m_AttrChange;}
		set{m_AttrChange = value;}
	}
	/// <summary>
	/// 获取status
	/// </summary>
	protected Status Gstatus{
		get{
			if(m_Parent == null) return null;
			else return m_Parent.m_Status;
		}
	}
	
	/// <summary>
	/// 获取是否被雷达搜索，true 能被搜索，false，不能被搜索
	/// </summary>
	private bool m_RadarSearch = true;
	public  bool RadarSearch
	{
		get{return m_RadarSearch;}
		set{m_RadarSearch =value;}
	}

	/// <summary>
	/// 英雄死亡音效
	/// </summary>
	protected string m_strDieSound = "";
	public  string strDieSound
	{
		get{return m_strDieSound;}
		set{m_strDieSound =value;}
	}

	/// <summary>
	/// 目标免疫属性，0 可以作为任何对象的目标
	/// B1111101.形式，为1表示不能被这类对象攻击，为 0表示可以
	/// </summary>
	protected ImmuneTarget m_TargetImmune = ImmuneTarget.Normal;
	public  ImmuneTarget TargetImmune
	{
		get{return m_TargetImmune;}
		set{m_TargetImmune = value;}
	}

	/// <summary>
	/// 检测能否被免疫
	/// </summary>
	/// <param name="AttackType">攻击者身份</param>
	/// <param name="HurtProp">受击者免疫目标属性</param>
	/// <returns> true，被免疫，不能作为目标被选择，false 不被免疫，可做为目标被选择
	public static bool CheckTargetImmune(ImmuneTarget AttackType ,ImmuneTarget HurtProp)
	{
		if((AttackType & HurtProp) == AttackType)
			return true;
		else 
			return false;
	}


	/// <summary>
	/// 技能释放
	/// </summary>
	public ReleaseType SkillReleaseType
	{
		get{
			return GetReleaseType();
		}
	}

	/// <summary>
	/// 触发陷进房间
	/// </summary>
	public bool TriggerTrapBuilding
	{
		get{
			return GetTriggerTrapBuilding();
		}
	}
	

	/// <summary>
	/// 技能免疫
	/// </summary>
	public ImmuneSkill SkillImmune
	{
		get{
			return GetSkillImmune();
		}
	}
	/// <summary>
	/// 是否隐身，true 隐身， false 现身
	/// </summary>
	public  bool IsHide
	{
		get{return GetHide();}
	}

	/// <summary>
	/// 是否遇障碍倒立行走
	/// </summary>
	public bool IsHandstand
	{
		get{
			if(IsHide && AttrType == 101001)return true;
			else return false;
		}
	}

	/// <summary>
	/// 是否按指定方向寻路
	/// </summary>
	public bool IsKeepDir
	{
		get{
			/*if (Gstatus.CheckStateBySkill(1022)) return true;
			else */return false;
		}
	}

	/// <summary>
	/// 获取skill
	/// </summary>
	protected Skill Gskill{
		get{
			if(m_Parent == null) return null;
			else return m_Parent.m_Skill;
		}
	}
	/// <summary>
	/// 力量
	/// </summary>
	protected int m_strength ;
	public int Strength{
		get{return GetStrength();}
	}
	/// <summary>
	/// 敏捷
	/// </summary>
	protected int m_agility ;
	public int Agility{
		get{return GetAgility();}
	}
	/// <summary>
	/// 智力
	/// </summary>
	protected int m_intelligence ;
	public int Intelligence{
		get{return GetIntelligence();}
	}
	/// <summary>
	/// 物理减甲
	/// </summary>
	protected int m_CutPhyDefend = 0;
	public int CutPhyDefend{
		get{return GetCutPhyDefend();}
	}



	/// <summary>
	/// 魔法忽视防御
	/// </summary>
	protected int m_CutMagDefend = 0;
	public int CutMagDefend{
		get{return GetCutMagDefend();}
	}


	/// <summary>
	/// 物理减免伤害
	/// </summary>
	protected int m_CutPhyDamage = 0;
	public int CutPhyDamage{
		get{return GetCutPhyDamage();}
	}
	
	
	
	/// <summary>
	/// 魔法减免伤害
	/// </summary>
	protected int m_CutMagDamage = 0;
	public int CutMagDamage{
		get{return GetCutMagDamage();}
	}

	
	/// <summary>
	/// 物理攻击
	/// </summary>
	protected int m_phy_attack ;
	public int PhyAttack{
		get{return GetPhyAttack();}
	}
	/// <summary>
	/// 物理防御
	/// </summary>
	protected int m_phy_defend ;
	public int PhyDefend{
		get{return GetPhyDefend();}
	}
	/// <summary>
	/// 物理暴击
	/// </summary>
	protected int m_phy_crit ;
	public int PhyCrit{
		get{return GetPhyCrit();}
	}
	/// <summary>
	/// 物理命中率
	/// </summary>
	protected int m_DodgeRatio;
	public int DodgeRatio{
		get{return GetDodgeRatio();}
	}
	/// <summary>
	/// 闪避率
	/// </summary>
	protected int m_dodge;
	public int Dodge{
		get{return GetDodge();}
	}
	/// <summary>
	/// 暴击率
	/// </summary>
	protected int m_CritRatio;
	public int CritRatio{
		get{return GetCritRatio();}
	}
	/// <summary>
	/// 魔法攻击
	/// </summary>
	protected int m_magic_attack ;
	public int MagicAttack{
		get{return GetMagicAttack();}
	}
	/// <summary>
	/// 魔法防御
	/// </summary>
	protected int m_magic_defend ;
	public int MagicDefend{
		get{return GetMagicDefend();}
	}
	/// <summary>
	/// 魔法暴击
	/// </summary>
	protected int m_magic_crit ;
	public int MagicCrit{
		get{return GetMagicCrit();}
	}
	/// <summary>
	/// 移动速度
	/// </summary>
	protected float m_speed ;
	public float Speed{
		get{return GetSpeed();}
	}

	/// <summary>
	/// 飞行速度
	/// </summary>
	protected float m_Flyspeed ;
	public float Flyspeed{
		get{return GetFlyspeed();}
	}
	/// <summary>
	/// 移动速度百分比
	/// </summary>
	public float SpeedPercent{
		get{return GetSpeedercent();}
	}
	/// <summary>
	/// 攻击速度
	/// </summary>
	protected float m_Attackspeed ;
	public float AttackSpeed{
		get{return GetAttackSpeed();}
	}

	/// <summary>
	/// 攻击频率
	/// </summary>
	protected float m_attack_time ;
	public float AttackTime{
		get{return GetAttackTime();}
	}

	/// <summary>
	/// 攻击喜好
	/// </summary>
	protected int m_attack_like;
	public int AttackLike{
		get{return m_attack_like;}
	}
	/// <summary>
	/// 攻击喜好
	/// </summary>
	protected int m_attack_type;
	public int AttackType{
		get{return m_attack_type;}
	}
	/// <summary>
	/// 攻击喜好
	/// </summary>
	protected bool m_Attacked;
	public bool Attacked{
		get{return m_Attacked;}
		set{m_Attacked = value;}
	}
	/// <summary>
	/// 吸血属性
	/// </summary>
	protected int m_Vampire = 0;
	public int Vampire{
		get{return GetVampire();}
	}


	/// <summary>
	/// 属性
	/// </summary>
	protected int m_Hit = 0;
	public int Hit{
		get{return GetHit();}
	}

	/// <summary>
	/// 生命恢复
	/// </summary>
	protected int m_RecoHp = 0;
	public int RecoHp{
		get{return GetRecoHp();}
	}

	/// <summary>
	/// 落水湿身属性
	/// </summary>
	protected bool m_IsWetBody = false;
	public bool IsWetBody{
		set{m_IsWetBody = value;}
		get{return m_IsWetBody;}
	}


	/// <summary>
	/// 麻痹属性
	/// </summary>
	protected bool m_Paralysis = false;
	public bool Paralysis{
		get{return m_Paralysis;}
		set{m_Paralysis = value;
			if(m_Paralysis == true)
			{EventCenter.DoEvent(NDEventType.Attr_Paralysis,m_SceneID,m_Paralysis);}
		}
		}


	/// <summary>
	/// 怒气恢复
	/// </summary>
	protected int m_RecoAnger = 0;
	public int RecoAnger{
		get{return GetRecoAnger();}
	}

	/// <summary>
	/// 治疗效果提高
	/// </summary>
	protected int m_AddDoctor = 0;
	public int AddDoctor{
		get{return GetAddDoctor();}
	}

	/// <summary>
	/// 等级
	/// </summary>
	protected int m_level ;
	public int Level{
		get{return GetLevel();}
	}

	/// <summary>
	/// 抗压
	/// </summary>
	protected int m_AntiPress;
	public int AntiPress{
		get{return GetAntiPress();}
	}


	/// <summary>
	/// 冰点
	/// </summary>
	protected float m_fIcePointUpdate;
	protected float m_fIcePointSlowDown;
	protected int m_IcePoint;
	public int IcePoint
	{
		get{
			if(m_IcePoint>0&&Time.time-m_fIcePointUpdate > 1f)
			{
				m_fIcePointSlowDown+=Time.deltaTime;
				if(m_fIcePointSlowDown>0.5f)
				{
					m_fIcePointSlowDown=0f;
					m_IcePoint-=50;
					if(m_IcePoint<0)
						m_IcePoint=0;
				}
			}
			return m_IcePoint;
		}
		set{
			m_fIcePointUpdate=Time.time;
			m_IcePoint = value;
			m_fIcePointSlowDown=0f;
		}
	}

    /// <summary>
    /// 抗冰
    /// </summary>
    protected int m_AntiIce;
    public int AntiIce
    {
        get { return GetAntiIce(); }
    }
	/// <summary>
	/// 类型
	/// </summary>
	protected int m_Type;
	public int Type
	{
		get { return GetType(); }
	}
	/// <summary>
	/// 定位
	/// </summary>
	protected int m_loaction;
	public int Loaction
	{
		get { return m_loaction; }
	}

	protected virtual int GetType()
	{
		#if UNITY_EDITOR_LOG
		Debug.Log("忘记重载了"+ this);
		#endif
		return m_Type;
	}
   protected virtual int GetAntiIce()
	{
		#if UNITY_EDITOR_LOG
		Debug.Log("忘记重载了"+ this);
		#endif
        return m_AntiIce;
    }

	/// <summary>
	/// 满血量
	/// </summary>
	protected int m_FullHp;
	public int FullHp{
		get{return GetFullHp() ;}
		set{m_FullHp = value ;}
	}
	/// <summary>
	/// 当前血量
	/// </summary>
	protected int m_Hp;
	public int Hp{
		get{
			m_Hp = m_Hp > FullHp?FullHp:m_Hp;
			return m_Hp;
		}
		set{
			SetHp(value);
		}
	}	
	/// <summary>
	/// 满怒气
	/// </summary>
	protected int m_FullAnger;
	public int FullAnger{
		get{return GetFullAnger();}
	}
	/// <summary>
	/// 当前怒气
	/// </summary>
	protected int m_Anger;
	public int Anger{
		get{
			m_Anger = m_Anger > FullAnger? FullAnger:m_Anger;
			return m_Anger;
		}
		set{
			SetAnger(value);
		}
	}
	/// <summary>
	/// 开启怒气技能
	/// </summary>
	protected int m_IsAngerSkill;
	public int IsAngerSkill
	{
		get{return m_IsAngerSkill;}
		set{
			if(m_IsAngerSkill != value)
			{
				m_IsAngerSkill = value;
				EventCenter.DoEvent(NDEventType.Attr_FullAnger,m_SceneID,m_IsAngerSkill);
			}
		}
	}

	
	/// <summary>
	/// 建筑start位置
	/// </summary>
	protected Int2 m_StartPos;
	public Int2 StartPos{
		get{return GetStartPos();}
		set{m_StartPos = value;}
	}

	/// <summary>
	/// 建筑Hidecd
	/// </summary>
	protected float  m_HideCd = 0.0f;
	public float HideCd{
		get{return m_HideCd;}
	}

	/// <summary>
	/// 建筑物耐久度
	/// </summary>
	protected int  m_Durability = 100000;
	public int Durability{
		get{return m_Durability;}
		set{m_Durability = value;}
	}


	/// <summary>
	/// 建筑物kind属性
	/// </summary>
	protected AttributeType  m_BuildAttributeType = AttributeType.NONE;
	public AttributeType BuildAttributeType{
		get{return m_BuildAttributeType;}
		set{m_BuildAttributeType = value;}
	}
	
	/// <summary>
	/// 体型，体型大的站后面，体型小的战前面
	/// </summary>
	protected int m_shape = 1;
	public int Shape
	{
		get{return m_shape ;}
	}


	/// <summary>
	/// 行走优先级
	/// </summary>
	protected int m_goorder = 0;
	public int Goorder
	{
		get{return m_goorder ;}
	}

	/// <summary>
	/// 位置
	/// </summary>
	protected Int2 m_Pos;
	public Int2 Pos{
		get{return GetPos();}
		set{m_Pos = value;}
	}
	/// <summary>
	/// 建筑大小，体型
	/// </summary>
	protected int m_Size;
	public int Size{
		get{return GetSize();}
	}

	/// <summary>
	/// 是否可被攻击，破坏
	/// </summary>
	protected bool m_IsDamage = true;
	public bool IsDamage{
		get{return m_IsDamage;}
	}


	/// <summary>
	/// 是否资源
	/// </summary>
	protected bool m_IsResource = true;
	public bool IsResource{
		get{return m_IsResource;}
	}

	/// <summary>
	/// 木材数量
	/// </summary>
	protected int m_wood;
	public int Wood{
		get{return GetWood();}
	}


	/// <summary>
	/// 石头数量
	/// </summary>
	protected int m_stone;
	public int Stone{
		get{return GetStone();}
	}


	/// <summary>
	/// 刚才数量
	/// </summary>
	protected int m_steel;
	public int Steel{
		get{return GetSteel();}
	}

	/// <summary>
	/// 模型类型
	/// </summary>
	public int ModelType{
		get{return GetModelType();}
	}

	/// <summary>
	///是否可以行走
	/// </summary>
	public bool CanMove{
		get{return GetCanMove();}
	}
	/// <summary>
	///是否可以攻击
	/// </summary>
	public bool CanAttack{
		get{return GetCanAttack();}
	}
	/// <summary>
	///通道数
	/// </summary>
	protected int m_Passageway;
	public int Passageway{
		get{return GetPassageway();}
	}	/// <summary>
	///冲击力
	/// </summary>
	protected int m_concussion;
	public int Concussion{
		get{return m_concussion;}
	}
	/// <summary>
	///当前冲击力
	/// </summary>
	protected int m_curconcussion;
	public int CurConcussion{
		get{return m_curconcussion;}
		set{m_curconcussion = value;}
	}
	/// <summary>
	/// 生命时间
	/// </summary>
	protected int m_LifeTime;
	public int LifeTime{
		get{return GetLifeTime();}
	}

	/// <summary>
	/// 是否被魅惑
	/// </summary>
	protected bool m_Charmed = false;
	public bool Charmed{
		get{return m_Charmed;}
		set{m_Charmed = value;}
	}

	/// <summary>
	/// pet 攻击类型
	/// </summary>
	protected int m_PetAttackType ;
	public int PetAttackType{
		get{return m_PetAttackType;}
	}


	/// <summary>
	/// pet 伤害
	/// </summary>
	protected int m_PetDamage ;
	public int PetDamage{
		get{return m_PetDamage;}
	}

	/// <summary>
	/// pet 攻击类型
	/// </summary>
	protected int m_jumpDistance ;
	public int JumpDistance{
		get{return GetJumpDistance();}
	}


	/// <summary>
	/// 建筑摆设shipputut_data0
	/// </summary>
	protected int m_ShipPutdata0 ;
	public int ShipPutdata0{
		get{return m_ShipPutdata0;}
	}
	
	/// <summary>
	/// 建筑摆设shipputut_data1
	/// </summary>
	protected int m_ShipPutdata1 ;
	public int ShipPutdata1{
		get{return m_ShipPutdata1;}
	}
	//承受力
	protected int m_bear;
	public int Bear{
		get{return GetBear();}
	}
	public virtual int GetBear()
	{
		return m_bear;
	}	
	public int CurBear{
		get{return GetCurBear();}
	}
	public virtual int GetCurBear()
	{
		float curbear = Hp * 1.0f/ FullHp;
		int curb = Mathf.CeilToInt(Bear * curbear);
		curb = curb < 0 ? 0 : curb;
		return curb;
	}
	/// <summary>
	/// 门开关属性 true 开 false 关
	/// </summary>
	protected bool m_DoorState = false;
	public bool DoorState{
		set{m_DoorState = value;}
		get{return m_DoorState;}
	}	
	/// <summary>
	/// 物理暴击伤害加成属性 
	/// </summary>
	public int PhysicalCritBonusDamage{
		get{return GetPhysicalCritBonusDamage();}
	}
	/// <summary>
	/// 获取物理暴击伤害加成属性
	/// </summary>
	protected virtual int GetPhysicalCritBonusDamage()
	{
		return 0;
	}
	/// <summary>
	/// 物理暴击伤害加成属性 
	/// </summary>
	public int MagicCritBonusDamage{
		get{return GetMagicCritBonusDamage();}
	}
	/// <summary>
	/// 获取物理暴击伤害加成属性
	/// </summary>
	protected virtual int GetMagicCritBonusDamage()
	{
		return 0;
	}

	/// <summary>
	/// 获取冰系伤害减免值属性 
	/// </summary>
	public int WaterDamageReduction
	{
		get { return GetWaterDamageReduction(); }
	}
	/// <summary>
	/// 获取冰系伤害减免值
	/// </summary>
	protected virtual int GetWaterDamageReduction()
	{
		return 0;
	}
	//建筑瘫痪
	public bool  m_Broken;
	public bool Broken{
		get{return m_Broken;}
		set{m_Broken = value;}
	}
	/// <summary>
	/// 初始化属性数据
	/// </summary>
	public virtual void Init(int SceneID, LifeMCore Core, Life Parent)
	{
		m_Parent = Parent;
		m_SceneID = SceneID;

        
	}

    public LifeMCamp Camp
    {
        get
        {
            return m_Parent.m_Core.m_Camp;
        }
    }

	/// <summary>
	/// 释放属性数据
	/// </summary>
	public virtual void Destory()
	{
		
	}
	
	/// <summary>
	/// 获取力量
	/// </summary>
	protected virtual int GetStrength()
	{
		return m_strength;
	}

	/// <summary>
	/// 获取敏捷
	/// </summary>
	protected virtual int GetAgility()
	{
		return m_agility;
	}
	/// <summary>
	/// 获取智力
	/// </summary>
	protected virtual int GetIntelligence()
	{
		return m_intelligence;
	}
	/// <summary>
	/// 获取物理物理减甲
	/// </summary>
	protected virtual int GetCutPhyDefend()
	{
		return m_CutPhyDefend;
	}


	/// <summary>
	/// 获取魔法忽视防御
	/// </summary>
	protected virtual int GetCutMagDefend()
	{
		return m_CutMagDefend;
	}


	/// <summary>
	/// 获取物理减免伤害
	/// </summary>
	protected virtual int GetCutPhyDamage()
	{
		return m_CutPhyDamage;
	}
	
	
	/// <summary>
	/// 获取魔法减免伤害
	/// </summary>
	protected virtual int GetCutMagDamage()
	{
		return m_CutMagDamage;
	}



	/// <summary>
	/// 获取物理攻击
	/// </summary>
	protected virtual int GetPhyAttack()
	{
		return m_phy_attack;
	}
	/// <summary>
	/// 获取物理防御
	/// </summary>
	protected virtual int GetPhyDefend()
	{
		return m_phy_defend;
	}
	/// <summary>
	/// 获取物理暴击
	/// </summary>
	protected virtual int GetPhyCrit()
	{
		return m_phy_crit;
	}

	/// <summary>
	/// 获取闪避率
	/// </summary>
	protected virtual int GetDodge()
	{
		return m_dodge;
	}
	/// <summary>
	/// 获取暴击率
	/// </summary>
	protected virtual int GetCritRatio()
	{
		return m_CritRatio;
	}
	/// <summary>
	/// 获取魔法攻击
	/// </summary>
	protected virtual int GetMagicAttack()
	{
		return m_magic_attack;
	}
	/// <summary>
	/// 获取魔法防御
	/// </summary>
	protected virtual int GetMagicDefend()
	{
		return m_magic_defend;
	}
	/// <summary>
	/// 获取魔法暴击
	/// </summary>
	protected virtual int GetMagicCrit()
	{
		return m_magic_crit;
	}

	/// <summary>
	/// 获取移动速度
	/// </summary>
	protected virtual float GetSpeed()
	{
		return m_speed;
	}
	/// <summary>
	/// 获取移动速度
	/// </summary>
	protected virtual float GetSpeedercent()
	{
		return 1;
	}

	/// <summary>
	/// 获取攻击速度
	/// </summary>
	protected virtual float GetAttackSpeed()
	{
		return m_speed;
	}
	/// <summary>
	/// 获取闪避调整系数
	/// </summary>
	protected virtual int GetDodgeRatio()
	{
		return m_DodgeRatio;
	}
	/// <summary>
	/// 获取等级
	/// </summary>
	protected virtual int GetLevel()
	{
		return m_level;
	}

	/// <summary>
	/// 获取抗压系统
	/// </summary>
	protected virtual int GetAntiPress()
	{
		return m_AntiPress;
	}



	/// <summary>
	/// 获取满血量
	/// </summary>
	protected virtual int GetFullHp()
	{
		return m_FullHp;
	}
	/// <summary>
	/// 获取满怒气
	/// </summary>
	protected virtual int GetFullAnger()
	{
		return 0;
	}
	
	/// <summary>
	/// 获取建筑start位置
	/// </summary>
	protected virtual Int2 GetStartPos()
	{
		return m_StartPos;
	}

	/// <summary>
	/// 建筑位置
	/// </summary>
	protected virtual Int2 GetPos()
	{
		return m_StartPos;
	}

	/// <summary>
	/// 获取建筑大小，体型
	/// </summary>
	protected virtual int GetSize()
	{
		return m_Size;
	}

	/// <summary>
	/// 获取木材数量
	/// </summary>
	protected virtual int GetWood()
	{
		return m_wood;
	}



	/// <summary>
	/// 获取石头数量
	/// </summary>
	protected virtual int GetStone()
	{
		return m_stone;
	}
	

	/// <summary>
	/// 获取刚才数量
	/// </summary>
	protected virtual int GetSteel()
	{
		return m_steel;
	}
	protected virtual int GetModelType()
	{
		return 0;
	}
	protected virtual bool GetCanMove()
	{
		return true;
	}
	protected virtual bool GetCanAttack()
	{
		return true;
	}

	/// <summary>
	/// 获取通道数
	/// </summary>
	protected virtual int GetPassageway()
	{
		return 1;
	}
	

	/// <summary>
	///体型半径
	/// </summary>
	protected virtual int GetRadius()
	{
		return 1;
	}

	/// <summary>
	/// 获取攻击频率
	/// </summary>
	protected virtual float GetAttackTime()
	{
		return 1.0f;
	}


	/// <summary>
	/// 获取生命时间
	/// </summary>
	protected virtual int GetLifeTime()
	{
		return int.MaxValue;
	}

	/// <summary>
	/// 设置HP
	/// </summary>
	protected virtual void SetHp(int hp)
	{
		m_Hp = hp > FullHp? FullHp:hp;
		if (m_Hp<=0)
		{
			Anger = 0;
		}
	}

	/// <summary>
	/// 设置怒气
	/// </summary>
	protected virtual void SetAnger(int anger)
	{

	}

	/// <summary>
	/// 获取是否隐形
	/// </summary>
	protected virtual bool GetHide()
	{
		return false;
	}

	/// <summary>
	/// 获取技能释放方式
	/// </summary>
	protected virtual ReleaseType GetReleaseType()
	{
		return ReleaseType.Normal;
	}

	/// <summary>
	/// 获取技能免疫方式
	/// </summary>
	protected virtual ImmuneSkill GetSkillImmune()
	{
		return ImmuneSkill.Normal;
	}

	/// <summary>
	/// 触发陷阱房间
	/// </summary>
	protected virtual bool GetTriggerTrapBuilding()
	{
		return true;
	}

	/// <summary>
	/// 跳跃距离
	/// </summary>
	protected virtual int GetJumpDistance()
	{
		return m_jumpDistance;
	}

	/// <summary>
	/// 飞行速度
	/// </summary>
	protected virtual float GetFlyspeed ()
	{
		return m_Flyspeed;
	}

	/// <summary>
	/// 获取吸血
	/// </summary>
	protected virtual int GetVampire ()
	{
		return m_Vampire;
	}

	/// <summary>
	/// 获取命中
	/// </summary>
	protected virtual int GetHit ()
	{
		return m_Hit;
	}

	/// <summary>
	/// 生命恢复
	/// </summary>
	protected virtual int GetRecoHp ()
	{
		return m_RecoHp;
	}
	
	/// <summary>
	/// 怒气恢复
	/// </summary>
	protected virtual int GetRecoAnger ()
	{
		return m_RecoAnger;
	}	
	/// <summary>
	/// 治疗效果提高
	/// </summary>
	protected virtual int GetAddDoctor ()
	{
		return m_AddDoctor;
	}	
	/// <summary>
	/// 更新属性，主要用于恢复血量更怒气这2个属性。
	/// </summary>
	public  void Updata (float deltaTime)
	{
		m_times += deltaTime;
		if(m_times >= 1.0f)
		{
			m_times= 0.0f;
			if(m_RecoHp > 0)
			{
				Hp +=  m_RecoHp;
			}

			if(m_RecoAnger > 0)
			{
				Anger += m_RecoAnger;
			}
		}
	}	


	/// <summary>
	/// 获取基础属性数据
	/// </summary>
	protected virtual int GetBaseAttrData(EffectType Type)
	{
		return 0;
	}


	/// <summary>
	/// 合成属性数据
	/// </summary>
	public virtual int GetAttrData(EffectType Type)
	{
		int ret = GetBaseAttrData(Type);
		return ret ;
	}
}
