#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#define UNITY_EDITOR_LOG
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 状态叠加管理
/// </summary>
/// <author>zhulin</author>
public enum LifeAction
{
	Attack      = 0,   //攻击行为
	Hit = 1,//受击
	WalkToEnd = 2,//走到尽头
	AngerEmety = 3,//怒气空了
    Die = 4,

};
/// <summary>
/// 状态结束枚举
/// </summary>
[System.Flags]
public enum StatusOverAction
{
	Normal    = 0x00,   //需强制结束
	CD        = 0x01,   //cd时间到结束
	Hurt      = 0x02,   //受伤害结束
	PhyHurt   = 0x04,   //受魔法伤害结束
	MagicHurt = 0x08,   //受魔法伤害结束
	Attack    = 0x10,   //攻击结束
	WalkToEnd    = 0x20,   //走到终点
    AngerEmety = 0x40,   //怒气空了
    Die = 0x80,         // 死亡
	ALL    = CD | Hurt | PhyHurt | MagicHurt | Attack ,
}

/// <summary>
/// 状态叠加规则
/// </summary>
public enum  AddRule
{
	ADD      = 0,  //叠加
	Replace  = 1,  //替换
}

/// <summary>
/// 防打断状态
/// </summary>
[System.Flags]
public enum  AntiInterruptStatus
{
	Normal   = 0x00,  //不打断
	Phy      = 0x01,  //物理防打断
	Magic    = 0x02,  //魔法防打断
}

/// <summary>
/// 状态属性
/// </summary>
public enum EffectType
{
	None         = 0, //none 
	Strength     = 1, //力量
	Agility      = 2, //敏捷
	Intelligence = 3, //智力
	Hp           = 4, //血量
	Anger        = 5, //怒气
	PhyDamage    = 6, //物理伤害
	PhyAttack    = 7, //物理攻击
	PhyDefense   = 8, //物理防御
	MagicDamage  = 9, //魔法伤害
	MagicAttack  =10, //魔法攻击
	MagicDefense =11, //魔法防御
	Hit          =12, //命中 
	Dodge        =13, //闪避 
	PhyCrit      =14, //物理暴击 
	MagicCrit    =15, //魔法暴击
	MoveSpeed    =16, //移动速度 
	AttackTime   =17, //攻击速度
	RecoHp       =18, //生命回复
	RecoAnger    =19, //怒气回复
	Vampire      =20, //吸血属性
	AntiPress = 31, //抗压性
	Shape        =32, //身材
	IcePoint     =33, //结冰点
	CutphyDamage =34, //减免物理伤害
	CutMagDamage =35, //减免魔法伤害
	SelfInterrupt=36, //自我打断属性
	CutPhyDefend =39, //穿透物理护甲
	CutMagDefend =40, //忽视魔法防御
	AddDoctor    =41, //治疗效果提高
	PhysicalCritBonusDamage    =42, //物理暴击伤害加成
	MagicCritBonusDamage    =43, //魔法暴击伤害加成
	SetHpByPercent = 46,			//生命上限百分比
	AddCurAngerAttack = 47,			//根据当前怒气增加攻击力
	Level 		= 48,						//绿巨熊 暴怒手足同情使用 等级判定
	AngryIncreasement = 50,			 // 怒气增加百分比



	FireAttack = 200,				// 火攻击
	AntiFire = 201,					// 火抗性
	FireDamageReduction = 202,		// 火系伤害减免

	WaterAttack = 210,				// 水攻击	
	AntiWater = 211,				// 水抗性
	WaterDamageReduction = 212,		// 水系伤害减免

	ElectricAttack = 220,			// 电攻击	
	AntiElectric = 221,				// 电抗性
	ElectricDamageReduction = 222,	// 电系伤害减免

	PotionAttack = 230,				// 毒攻击	
	AntiPotion = 231,				// 毒抗性
	PotionDamageReduction = 232,	// 毒系伤害减免

	GasAttack = 240,				// 气攻击	
	AntiGas = 241,					// 气抗性
	GasDamageReduction = 242,		// 气系伤害减免
	
};

public class StatusM  {
	

	protected int m_SceneID = 0;
	/// <summary>
	/// 初始化
	/// </summary>
	public virtual void Init(int SceneID,int ResalseSceneID,int SkillType,SkillStatusInfo Info)
	{
		m_SceneID = SceneID;
		AddStatus(ResalseSceneID,SkillType,Info);
	}
	
	/// <summary>
	/// 状态数据列表
	/// </summary>
	protected List<StatusAction>m_StatusData = new List<StatusAction>();
	public List<StatusAction> StatusData{
		get{return  m_StatusData;}
	}
	/// <summary>
	/// 状态结束类型
	/// </summary>
	protected StatusOverAction m_OverData = StatusOverAction.CD; 
	/// <summary>
	/// 状态叠加规则
	/// </summary>
	protected AddRule m_AddRole = AddRule.ADD;
	/// <summary>
	/// 是否打断技能
	/// </summary>
	protected bool m_InterruptSkill = false;
	/// <summary>
	/// 是否自我打断技能
	/// </summary>
	protected bool m_SelfInterruptSkill = false;
	/// <summary>
	/// 自我打断类型.
	/// </summary>
	public SelfStatusInterrupt m_selfRuptType = SelfStatusInterrupt.None;

	/// <summary>
	/// 免疫技能
	/// </summary>
	protected ImmuneSkill m_SkillImmune = ImmuneSkill.Normal;
	/// <summary>
	/// 防打断状态
	/// </summary>
	protected AntiInterruptStatus m_AntiInterruptStatus = AntiInterruptStatus.Normal;
	/// <summary>
	/// 状态类型
	/// </summary>
	public StatusType m_type = StatusType.None;

	protected StatusInfo m_StatusInfo ;
    public StatusInfo CurrentStatus { get { return m_StatusInfo; } }

	/// <summary>
	/// buff 状态
	/// </summary>
	private int m_bufftype = 0;
	public int BuffType{
		get{return  m_bufftype;}
	}
	private SkillStatusReplaceInfo m_ReplaceInfo = null;
	public SkillStatusReplaceInfo ReplaceInfo
	{
		get{return m_ReplaceInfo;}
	}
	/// <summary>
	/// 叠加一个状态
	/// </summary>
	/// 
	public  bool AddStatus(int ResalseSceneID,int SkillType,SkillStatusInfo Info)
	{
		m_bufftype = Info.m_bufftype;
		//为空 添加
		Info.m_releasesceneid = ResalseSceneID;
		if(m_StatusData.Count == 0)
		{
			StatusAction status = new StatusAction();
			status.Init(m_SceneID,SkillType,ResalseSceneID,Info);
			m_StatusData.Add(status);
			//AddStatusEffect(Info , SkillType);
			DoSkillStatus(0.0f);
			m_ReplaceInfo = Info.m_RelpaceInfo;
			return true;
		}
		else
		{
			//状态叠加新规则，根据技能类型和技能等级做判断
			StatusAction s = FindSameSkillTypeStaus(SkillType);
			if (s != null)
			{
				if(s.StatusInfo.m_effectlevel < Info.m_effectlevel)
				{
					s.SetDone();
					StatusAction status = new StatusAction();
					status.Init(m_SceneID,SkillType,ResalseSceneID,Info);
					m_StatusData.Add(status);
					DoSkillStatus(0.0f);
					return true;
				}
			}
			else
			{
				
				StatusAction status = new StatusAction();
				status.Init(m_SceneID,SkillType,ResalseSceneID,Info);
				m_StatusData.Add(status);
				DoSkillStatus(0.0f);
			}
			/*if(m_AddRole ==AddRule.ADD) //叠加
			{
				StatusAction s = FindSameStaus(ResalseSceneID, SkillType);
				if(s == null)
				{
					StatusAction status = new StatusAction();
					status.Init(m_SceneID,SkillType,ResalseSceneID,Info);
					m_StatusData.Add(status);
					DoSkillStatus(0.0f);
				}
				else
				{
					s.ResetTime(Info.m_time);
				}
			}
			else if(m_AddRole ==AddRule.Replace) //替换
			{
				m_StatusData.Clear();
				StatusAction status = new StatusAction();
				status.Init(m_SceneID,SkillType,ResalseSceneID,Info);
				m_StatusData.Add(status);
				DoSkillStatus(0.0f);
			}
			else
			{
				Debug.LogError("出现第三种规则，异常");
			}*/
		}
		return false;
	}
	/// <summary>
	/// 该状态是否中断技能
	/// </summary>
	public bool InterruptSkill()
	{
		return m_InterruptSkill ;
	}
	/// <summary>
	/// 添加状态，通知表现状态效果
	/// </summary>
	public   void AddStatusEffect(SkillStatusInfo s,int SkillType,bool isaction)
	{
		SetStatusInfo(s,isaction);
		SetStatusEvent(SkillType);
	}
	private void SetStatusInfo(SkillStatusInfo s,bool isaction)
	{
		if(s == null)
		{
			m_StatusInfo = null;
		}
		else
		{
			if(m_StatusInfo == null)
				m_StatusInfo = new StatusInfo();
			m_StatusInfo.State = StatusState.Add;
			m_StatusInfo.Type =  m_type;
			m_StatusInfo.effectid = s.m_effectid;
			m_StatusInfo.position = s.m_position;
			m_StatusInfo.time = s.m_time *0.001f;
            m_StatusInfo.exSkill1 = s.m_skill1type;
            m_StatusInfo.exSkill2 = s.m_skill2type;
            m_StatusInfo.exSkillLevel1 = s.m_level1;
            m_StatusInfo.exSkillLevel2 = s.m_level2;
			m_StatusInfo.Releasescentid = s.m_releasesceneid;
			m_StatusInfo.effect = s.m_effect;
			m_StatusInfo.SpePriority = s.m_RelpaceInfo.m_spepriority;
			m_StatusInfo.isAction = isaction;
			#if UNITY_EDITOR_LOG
			//Debug.Log("添加状态：" + s.m_name + "," + m_StatusInfo.effectid + "," + m_StatusInfo.position + "," + Time.time);
			#endif
		}
	}


	public virtual void GetStatusOverAction()
	{
		if(m_StatusData.Count > 0)
		{
			// 非cd 模式，攻击，被攻击都会被打断
			if(m_StatusData[0].GetCdMode() == false)
			{
				m_OverData = StatusOverAction.Attack | StatusOverAction.Hurt ;
			}
		}
	}

	/// <summary>
	/// 清理所有状态
	/// </summary>
	public void ClearAllStatusAction()
	{
		m_StatusData.Clear();
	}

	/// <summary>
	/// 设置效果通知事件
	/// </summary>
	protected void SetStatusEvent(int SkillType)
	{
		if(m_StatusInfo == null) return ;
		m_StatusInfo.State = StatusState.Add;
		m_StatusInfo.SkillType = SkillType;
		EventCenter.DoEvent(NDEventType.StatusCG,m_SceneID,m_StatusInfo);
	}

	public void TriggerStatus()
	{
		if(m_StatusInfo == null) return ;
		m_StatusInfo.State = StatusState.Add;
		EventCenter.DoEvent(NDEventType.StatusCG,m_SceneID,m_StatusInfo);
	}
	/// <summary>
	/// 查找相同技能状态，相同施法者及相同技能
	/// </summary>
	private StatusAction FindSameSkillTypeStaus(int SkillID)
	{
		if(m_StatusData == null || m_StatusData.Count == 0)
			return null;
		for(int i = 0; i < m_StatusData.Count ;i ++)
		{
			if(m_StatusData[i] != null && 
			   m_StatusData[i].CheckSameSkillTypeStaus(SkillID) == true)
			{
				return m_StatusData[i];
			}
		}
		return null;
	}
	/// <summary>
	/// 查找相同技能状态，相同施法者及相同技能
	/// </summary>
	private StatusAction FindSameStaus(int ResalseSceneID,int SkillID)
	{
		if(m_StatusData == null || m_StatusData.Count == 0)
			return null;
		for(int i = 0; i < m_StatusData.Count ;i ++)
		{
			if(m_StatusData[i] != null && 
			   m_StatusData[i].CheckSameStaus(ResalseSceneID,SkillID) == true)
			{
				return m_StatusData[i];
			}
		}
		return null;
	}

	/// <summary>
	/// 获取技能释放者
	/// </summary>
	public int GetResalseSceneID()
	{
		for(int i = 0 ; i < m_StatusData.Count; i++ )
		{
			StatusAction  skill = m_StatusData[i];
			if(skill != null)
				return skill.GetResalseSceneID();
		}
		return -1;
	}

	/// <summary>
	/// 获取属性数据
	/// </summary>
	public virtual int GetAttrData(EffectType type)
	{		
		return 0 ;
	}

	/// <summary>
	/// 添加持续属性
	/// </summary>
	protected void AddrContinuedAttr(List<int> lAttrData)
	{
		if(lAttrData == null || lAttrData.Count == 0)
			return ;
		Life life = CM.GetAllLifeM(m_SceneID, LifeMType.ALL);
		if(life == null) return ;

		for(int i = 0; i <lAttrData.Count -1; i += 2 )
		{
			EffectType Type = (EffectType)lAttrData[i];
			if(Type == EffectType.RecoHp)
			{
				life.HP += lAttrData[i + 1];
				life.StatusUpdateHp(lAttrData[i + 1]);
			}
			else if(Type == EffectType.RecoAnger)
			{
					life.m_Attr.Anger += lAttrData[i + 1];
                    life.StatusUpdateAnger(lAttrData[i + 1]);
			}
			else if(Type == EffectType.IcePoint)
			{
				if(lAttrData[i + 1]<0)
					life.m_Attr.IcePoint=lAttrData[i + 1];
				else
					life.m_Attr.IcePoint += lAttrData[i + 1];
			}
		}
	}

	/// <summary>
	/// 获取属性数据
	/// </summary>
	protected  int GetAttrData(List<int> lAttrData,EffectType Type)
	{
		int value = 0;
		if(lAttrData == null || lAttrData.Count == 0)
			return value;
		int type = (int)Type;
		for(int i = 0; i <lAttrData.Count -1; i += 2 )
		{
			if(lAttrData[i] == type)
			{
				value += lAttrData[i+1] ;
			}
			if (Type == EffectType.PhyAttack && lAttrData[i] == (int) EffectType.AddCurAngerAttack)
			{
				Life l = CM.GetLifeM(m_SceneID,LifeMType.SOLDIER);
				value += (int) (l.Anger * 1.0f/l.m_Attr.FullAnger * lAttrData[i+1]) ;
			}
		}
		return value;
	}

	/// <summary>
	/// 持续时间到，移除状态
	/// </summary>
	public virtual void RemoveStatus(float duration)
	{
		if(m_StatusData == null || m_StatusData.Count ==0)
			return ;
		for(int i = 0; i<m_StatusData.Count; i++ )
		{
			if(m_StatusData[i].IsStatusOver(duration))
			{
				#if UNITY_EDITOR_LOG
				//Debug.Log("移除状态： " + m_StatusData[i].m_Name + "," + Time.time);
				#endif
				//结束前做点啥。
				m_StatusData[i].DoEventOver();
				m_StatusData.RemoveAt(i);
				i--;
			}
		}
		DoSkillStatus(duration);
	}
	/// <summary>
	/// 清理状态
	/// </summary>
	public void ClearStatus()
	{
		if(m_StatusData == null || m_StatusData.Count ==0)
			return ;
		for(int i = 0; i<m_StatusData.Count; i++ )
		{
			m_StatusData[i].DoEventOver();
			m_StatusData.RemoveAt(i);
			i--;
		}
	}


	/// <summary>
	/// 移除状态特效
	/// </summary>
	public void RemoveStatusEffect()
	{
		if(m_StatusInfo == null) return ;
		m_StatusInfo.State = StatusState.Remove;
		EventCenter.DoEvent(NDEventType.StatusCG,m_SceneID,m_StatusInfo);
	}

	/// <summary>
	/// 持续状态属性
	/// </summary>
	/// <author>zhulin</author>
	public virtual void DoSkillStatus(float duration)
	{

	}
	/// <summary>
	/// 根据行为打断状态
	/// </summary>
	public virtual void InterruptStatus(LifeAction Action)
	{
		GetStatusOverAction();
		if( (m_OverData & StatusOverAction.Attack)  == StatusOverAction.Attack)
		{
			if(Action == LifeAction.Attack)
			{
				m_StatusData.Clear();
			}
		}
		if( (m_OverData & StatusOverAction.Hurt) == StatusOverAction.Hurt)
		{
			if(Action == LifeAction.Hit)
			{
				m_StatusData.Clear();
			}
		}
		if( (m_OverData & StatusOverAction.WalkToEnd) == StatusOverAction.WalkToEnd)
		{
			if(Action == LifeAction.WalkToEnd)
			{
				m_StatusData.Clear();
			}
		}
		if( (m_OverData & StatusOverAction.AngerEmety) == StatusOverAction.AngerEmety)
		{
			if(Action == LifeAction.AngerEmety)
			{
				m_StatusData.Clear();
			}
		}
	}


	/// <summary>
	/// 确认包含防打断状态
	/// </summary>
	public bool CheckInterruptStatus(StatusInterrupt status)
	{
		if(status == StatusInterrupt.Phy)
		{
			if((m_AntiInterruptStatus & AntiInterruptStatus.Phy ) == AntiInterruptStatus.Phy)
				return true;
			else return false;
		}
		else if(status == StatusInterrupt.Magic)
		{
			if((m_AntiInterruptStatus & AntiInterruptStatus.Magic ) == AntiInterruptStatus.Magic)
				return true;
			else return false;
		}
		return  false;
	}
	


	//InterruptStatus
	
	/// <summary>
	/// 判断状态是否已经结束
	/// </summary>
	public bool IsStatusOver()
	{
		if(m_StatusData == null || m_StatusData.Count ==0)
			return true;
		else return false;
	}


	/// <summary>
	/// 是否拥有自我打断的状态
	/// </summary>
	/// <returns> true 免疫该技能， false 不免疫该技能</returns>
	public bool SelfInterrupt()
	{
		return m_SelfInterruptSkill;
	}
	/// <summary>
	/// 技能免疫判断
	/// </summary>
	/// <param name="AttackSkillType">攻击方技能类型</param>
	/// <param name="SkillType">免疫特定技能</param>
	/// <param name="DefanseStatus">防御方状态</param>
	/// <returns> true 免疫该技能， false 不免疫该技能</returns>
	public virtual bool ImmunitySkill(AttackType AttackSkillType ,int SkillType)
	{
		if(AttackSkillType == AttackType.Physical)
		{
			if((m_SkillImmune & ImmuneSkill.PhySkill) != 0 )
				return true;
			else return false;
		}
		else if(AttackSkillType == AttackType.Magic )
		{
			if((m_SkillImmune & ImmuneSkill.MagicSkill ) != 0 )
				return true;
			else return false;
		}
		else if(AttackSkillType == AttackType.Sacred  )
		{
			if((m_SkillImmune & ImmuneSkill.SacredSkill) != 0 )
				return true;
			else return false;
		}
		else if(AttackSkillType == AttackType.Fire  )
		{
			if((m_SkillImmune & ImmuneSkill.FireSkill ) != 0)
				return true;
			else return false;
		}
		return false;
	}

	/// <summary>
	/// check 是否包含某个技能产生的状态
	/// </summary>
	/// <returns> true 包含 ，false 不包含</returns>
	public bool CheckStateBySkill(int SkillType)
	{
		for(int i = 0 ; i < m_StatusData.Count; i++ )
		{
			StatusAction  skill = m_StatusData[i];
			if(skill != null && skill.CheckActionBySkill(SkillType) == true)
				return true;
		}
		return false;
	}

	public int ReduceHpbyShield(AttackType type ,int damage)
	{
		//物理盾
		int reduceHp = 0; 
		if(type == AttackType.Physical && m_type == StatusType.PhyAnitInterrupt)
		{
			for(int i = 0 ; i < m_StatusData.Count ; i++)
			{
				int hp = 0; 
				if(m_StatusData[i].ReduceShieldHp(damage ,ref hp) == true)
				{
					m_StatusData.RemoveAt(i);
					i -- ;
				}
				damage = damage -hp ;
				reduceHp += hp ;
				if(damage  <= 0) break;
			}
		}
		//魔法盾
		if(type == AttackType.Magic && m_type == StatusType.MagAnitInterrupt)
		{
			for(int i = 0 ; i < m_StatusData.Count ; i++)
			{
				int hp = 0; 
				if(m_StatusData[i].ReduceShieldHp(damage ,ref hp) == true)
				{
					m_StatusData.RemoveAt(i);
					i -- ;
				}
				damage = damage -hp ;
				reduceHp += hp ;
				if(damage  <= 0) break;
			}
		}
		return reduceHp ;
	}

	public bool CheckHaveStatus()
	{
		if (m_StatusData.Count > 0)
			return true;
		return false;
	}
	public float GetLongDuration()
	{
		float duration = 0;
		if (m_StatusData.Count > 0)
		{
			duration = m_StatusData[0].Duration;
			for (int i = 1; i < m_StatusData.Count; i++)
			{
				if (duration < m_StatusData[i].Duration)
					duration = m_StatusData[i].Duration;
			}
		}
		return duration;
	}
}

/// <summary>
/// 状态工厂
/// </summary>
/// <author>zhulin</author>
public class StatusFactory
{
	/// <summary>
	/// 创建状态
	/// </summary>
	/// <param name="Core">雷达拥有者的核心信息</param>
	/// <param name="f">雷达拥有视野</param>
	public static StatusM Create(int SceneID,int ResalseSceneID,int SkillID,SkillStatusInfo Info)
	{
		if(Info == null) return null;
		StatusType s = (StatusType)Info.m_type;
		StatusM  status = null;
		switch(s)
		{
		case StatusType.AddAttr:
			status = new AddAttrStatus();
			break;
		case StatusType.Vertigo:
			status = new VertigoStatus();
			break;
		case StatusType.Silence:
			status = new SilenceStatus();
			break;
		case StatusType.Vampire:
			status = new VampireStatus();
			break;
		case StatusType.Rebound:
			status = new ReboundStatus();
			break;
		case StatusType.Invisible:
			status = new InvisibleStatus();
			break;
		case StatusType.Invincible:
			status = new InvincibleStatus();
			break;
		case StatusType.Transfiguration:
			status = new TransfigurationStatus();
			break;
		case StatusType.Summon:
			status = new SummonStatus();
			break;
		case StatusType.ClickFly:
			status = new ClickFlyStatus();
			break;
		case StatusType.AbsorbDamage:
			status = new AbsorbDamageStatus();
			break;
		case StatusType.Virtual:
			status = new VirtualStatus();
			break;
		case StatusType.Shield:
			status = new ShieldStatus();
			break;
		case StatusType.Sputtering:
			status = new SputteringStatus();
			break;
		case StatusType.Damnation:
			status = new DamnationStatus();
			break;
		case StatusType.ImmunePhysical:
			status = new ImmunePhysicalStatus();
			break;
		case StatusType.ImmunityMagic:
			status = new ImmunityMagicStatus();
			break;
		case StatusType.IceBuild:
			status = new IceBuildStatus();
			break;
		case StatusType.Ring:
			status = new RingStatus();
			break;
		case StatusType.Taunt:
			status = new TauntStatus();
			break;
		case StatusType.StaticElec:
			status = new StaticElecStatus();
			break;
		case StatusType.Still:
			status = new StillStatus();
			break;
		case StatusType.Squash:
			status = new SquashStatus();
			break;
		case StatusType.Sauna:
			status = new SaunaStatus();
			break;
		case StatusType.KTV:
			status = new KTVStatus();
			break;
		case StatusType.ImmuneState:
			status = new ImmuneState();
			break;
		case StatusType.NoSquash:
			status = new NoSquashStatus();
			break;
		case StatusType.paralysis:
			status = new ParalysisStatus();
			break;
		case StatusType.PhyAnitInterrupt:
			status = new PhyAnitInterruptStatus();
			break;
		case StatusType.MagAnitInterrupt:
			status = new MagAnitInterruptStatus();
			break;
		case StatusType.Cleanse:
			status = new CleanseStatus();
			break;
		case StatusType.Charm:
			status = new CharmStatus();
			break;
		case StatusType.BuildDestory:
			status = new BuildDestoryStatus();
			break;
		case StatusType.Mark:
			status = new MarkStatus();
			break;
		case StatusType.RunAway:
			status = new RunAwayStaus();
			break;
		case StatusType.FakeTaunt:
			status = new FakeTauntStatus();
			break;
		case StatusType.Die:
			status = new DieStatus();
			break;
		case StatusType.Turn:
			status = new TurnStatus();
			break;
		case StatusType.WetBody:
			status = new WetBodyStatus();
			break;
		case StatusType.Berserker:
			status = new BerserkerStatus();
			break;
		case StatusType.Paralyze:
			status = new ParalyzeStatus();
			break;
		case StatusType.DieMark:
			status = new DieMarkStatus();
			break;
        case StatusType.Sleep:
            status = new SleepStatus();
            break;
        case StatusType.TearOfMermaid:
            status = new TearOfMermaidStatus();
            break;
		case StatusType.Trapped:
			status = new TrapedStatus();
			break;
		case StatusType.KickedBack:
			status = new KickedBackStatus();
			break;
        case StatusType.WhipCorpse:
            status = new WhipCorpseStatus();
            break;
		case StatusType.FireShield:
			status = new FireShieldStatus();
			break;
		default:
			break;
	     }
		return status;
   }
	
}