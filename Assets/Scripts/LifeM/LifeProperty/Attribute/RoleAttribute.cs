using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// 角色属性
/// </summary>
/// <author>zhulin</author>
///
public class RoleAttribute : NDAttribute {

	private int m_main_proterty;
	private SoldierInfo m_info;
	public override void Init(int SceneID, LifeMCore Core, Life Parent)
	{
		base.Init(SceneID,Core,Parent);
		m_info = CmCarbon.GetSoldierInfo(Core);
		m_AttrType = m_info.m_modeltype;
		m_strength = m_info.m_strength;
		m_agility = m_info.m_agility;
		m_intelligence = m_info.m_intelligence;
		m_phy_defend = m_info.m_phy_defend;
		m_phy_crit = m_info.m_phy_crit;
		m_phy_attack = m_info.m_phy_attack;
		m_magic_defend = m_info.m_magic_defend;
		m_magic_crit = m_info.m_magic_crit;
		m_magic_attack = m_info.m_magic_attack;
		m_DodgeRatio = m_info.m_dodgeratio;
		m_dodge = m_info.m_dodge;
		m_speed = m_info.m_speed;
		m_Flyspeed = m_info.m_Flyspeed;
		m_AntiPress = m_info.m_resist;
		m_main_proterty = m_info.m_main_proerty;
		m_CritRatio = m_info.m_critratio;
		m_Hp = FullHp;
		m_Anger = m_info.m_mp;
		m_Passageway = m_info.m_passageway;
		m_attack_time = m_info.m_attack_time;
		m_attack_like = m_info.m_attack_like;
		m_attack_type = m_info.m_attack_type;
		m_jumpDistance = m_info.m_jump_distance;
		m_loaction = m_info.m_loaction;
		m_shape = m_info.m_shape;
		m_goorder = m_info.m_goorder;
		m_concussion = m_info.m_concussion;
		m_strDieSound = m_info.m_strDieSound;
		SetValue(NDAttrKeyName.DeadMP_Int,m_info.m_dead_mp);
		SetAddAttr(m_info.m_AddAttr);
	}

	/// <summary>
	/// 获取附加属性
	/// </summary>
	protected void SetAddAttr(AddAttrInfo Info)
	{
		if(Info != null)
		{
			m_AntiIce += Info.GetAttr(EffectType.AntiWater);
			m_Hit += Info.GetAttr(EffectType.Hit);
			m_RecoHp += Info.GetAttr(EffectType.RecoHp);
			m_RecoAnger += Info.GetAttr(EffectType.RecoAnger);
			m_Vampire += Info.GetAttr(EffectType.Vampire);
			m_CutPhyDamage += Info.GetAttr(EffectType.CutphyDamage);
			m_CutMagDamage += Info.GetAttr(EffectType.CutMagDamage);
			m_CutPhyDefend += Info.GetAttr(EffectType.CutPhyDefend);
			m_CutMagDefend += Info.GetAttr(EffectType.CutMagDefend);
			m_AddDoctor += Info.GetAttr(EffectType.AddDoctor);
		}
	}

	/// <summary>
	/// 合成属性数据
	/// </summary>
	public override int GetAttrData(EffectType Type)
	{
		int ret = GetBaseAttrData(Type);
		
		if(Gstatus != null && Type != EffectType.RecoHp && Type != EffectType.RecoAnger)
			ret += Gstatus.GetAttrData(Type);

        List<Life> lifeList = new List<Life>();
      /*  Life skillTarget = m_Parent.m_Skill.PropSkillInfo.m_target;*/
        //LifeMCamp camp = skillTarget.m_Core.m_Camp;
        //遍历同一阵营
        CM.SearchLifeMListInBoat(ref lifeList, LifeMType.SOLDIER | LifeMType.SUMMONPET, LifeMCamp.ALL);
        foreach(Life life in lifeList)
        {
            ret += life.m_Skill.GetAuraAffector(Type, m_Parent, Camp);
        }
		return ret ;
	}

	
	/// <summary>
	/// 释放属性数据
	/// </summary>
	public override void Destory()
	{
		
	}


	/// <summary>
	/// 获取基础属性数据
	/// </summary>
	protected override int GetBaseAttrData(EffectType Type)
	{
		//
		if(Type == EffectType.Strength)
			return m_strength;
		else if(Type == EffectType.Agility)
			return m_agility;
		else if(Type == EffectType.Intelligence)
			return m_intelligence;
		else if(Type == EffectType.Hp)
		{
			//int AddStrength = Strength - m_info.m_strengthstart;
			//return ScriptM.Formula<int>("CALC_HP",m_info.m_hpstart,AddStrength);
			return m_info.m_hp;
		}
		else if(Type == EffectType.PhyAttack)
		{
			//int AddStrength = Strength - m_info.m_strengthstart;
			//int AddAgility = Agility - m_info.m_agilitystart;
			//int AddIntelligence = Intelligence - m_info.m_intelligencestart;
			//return ScriptM.Formula<int>("CALC_SOLDIER_PHYATTACK",m_info.m_phy_attackstart,m_main_proterty,AddStrength ,AddAgility,AddIntelligence);
			return m_phy_attack;
		}
		else if(Type == EffectType.PhyDefense)
		{
			//int AddStrength = Strength - m_info.m_strengthstart;
			//int AddAgility = Agility - m_info.m_agilitystart;
			//return ScriptM.Formula<int>("CALC_SOLDIER_PHYDEFEND",m_info.m_phy_defendstart,AddStrength,AddAgility);
			return m_phy_defend;
		}
		else if(Type ==EffectType.MagicAttack)
		{
			//int AddIntelligence = Intelligence - m_info.m_intelligencestart;
			//return ScriptM.Formula<int>("CALC_SOLDIER_MAGICATTACK",m_info.m_magic_attackstart,AddIntelligence);
			return m_magic_attack;
		}
		else if(Type ==EffectType.MagicDefense)
		{
			//int AddIntelligence = Intelligence - m_info.m_intelligencestart;
			//return ScriptM.Formula<int>("CALC_SOLDIER_MAGICDEFEND",m_info.m_magic_defendstart,AddIntelligence);
			return m_magic_defend;
		}
		else if(Type ==EffectType.PhyCrit)
		{
			//int AddAgility = Agility - m_info.m_agilitystart;
			//return ScriptM.Formula<int>("CALC_SOLDIER_PHYCRIT",m_info.m_phy_critstart,AddAgility);
			return m_phy_crit;
		}
		else if(Type ==EffectType.MagicCrit)
		{
			//int AddIntelligence = Intelligence - m_info.m_intelligencestart;
			//return ScriptM.Formula<int>("CALC_SOLDIER_MAGICCRIT",m_info.m_magic_critstart,AddIntelligence);
			return m_magic_crit;
		}
		else if (Type == EffectType.CutPhyDefend)
			return m_CutPhyDefend;
		else if(Type == EffectType.CutMagDefend)
			return m_CutMagDefend;
		else if(Type == EffectType.CutphyDamage)
			return m_CutPhyDamage;
		else if(Type == EffectType.CutMagDamage)
			return m_CutMagDamage;
		else if(Type == EffectType.Dodge)
			return m_dodge;
		else if(Type == EffectType.Vampire)
			return m_Vampire;
		else if(Type == EffectType.Hit)
			return m_Hit;
		else if(Type == EffectType.RecoHp)
			return m_RecoHp;
		else if(Type == EffectType.RecoAnger)
			return m_RecoAnger;
		else if(Type == EffectType.AddDoctor)
			return m_AddDoctor;
		else if(Type == EffectType.AntiPress)
			return m_AntiPress;
		else if(Type == EffectType.IcePoint)
			return IcePoint;
		else if (Type == EffectType.AntiFire
			|| Type == EffectType.FireDamageReduction
			|| Type == EffectType.AntiWater
			|| Type == EffectType.WaterDamageReduction
			|| Type == EffectType.AntiPotion
			|| Type == EffectType.PotionDamageReduction
			|| Type == EffectType.AntiGas
			|| Type == EffectType.GasDamageReduction
			|| Type == EffectType.AntiElectric
			|| Type == EffectType.ElectricDamageReduction
			)
			return m_info.m_AddAttr.GetAttr(Type);
	
		return 0;
	}
	
	/// <summary>
	/// 获取力量
	/// </summary>
	protected override int GetStrength()
	{
		return GetAttrData(EffectType.Strength) ;
	}
	/// <summary>
	/// 获取敏捷
	/// </summary>
	protected override int GetAgility()
	{
		return GetAttrData(EffectType.Agility) ;
	}
	/// <summary>
	/// 获取智力
	/// </summary>
	protected override int GetIntelligence()
	{
		return GetAttrData(EffectType.Intelligence) ;
	}
	/// <summary>
	/// 获取透视物理护甲
	/// </summary>
	protected override int GetCutPhyDefend()
	{
		return GetAttrData(EffectType.CutPhyDefend) ;
	}
	/// <summary>
	/// 获取魔法忽视防御
	/// </summary>
	protected override int GetCutMagDefend()
	{
		return GetAttrData(EffectType.CutMagDefend) ;
	}	
	/// <summary>
	/// 获取物理减免伤害
	/// </summary>
	protected override int GetCutPhyDamage()
	{
		return GetAttrData(EffectType.CutphyDamage) ;
	}
	/// <summary>
	/// 获取魔法减免伤害
	/// </summary>
	protected override int GetCutMagDamage()
	{
		return GetAttrData(EffectType.CutMagDamage) ;
	}
	
	/// <summary>
	/// 获取物理攻击
	/// </summary>
	protected override int GetPhyAttack()
	{
		return GetAttrData(EffectType.PhyAttack) ;
	}
	/// <summary>
	/// 获取物理防御
	/// </summary>
	protected override int GetPhyDefend()
	{
		return GetAttrData(EffectType.PhyDefense) ;
	}
	/// <summary>
	/// 获取物理暴击
	/// </summary>
	protected override int GetPhyCrit()
	{
		return GetAttrData(EffectType.PhyCrit) ;
	}

	/// <summary>
	/// 获取魔法攻击
	/// </summary>
	protected override int GetMagicAttack()
	{
		return GetAttrData(EffectType.MagicAttack) ;
	}
	/// <summary>
	/// 获取魔法防御
	/// </summary>
	protected override int GetMagicDefend()
	{
		return GetAttrData(EffectType.MagicDefense) ;
	}
	/// <summary>
	/// 获取魔法暴击
	/// </summary>
	protected override int GetMagicCrit()
	{
		return GetAttrData(EffectType.MagicCrit) ;
	}
	/// <summary>
	/// 获取吸血
	/// </summary>
	protected override int GetVampire ()
	{
		return GetAttrData(EffectType.Vampire) ;
	}	
	/// <summary>
	/// 获取命中
	/// </summary>
	protected override int GetHit ()
	{
		return GetAttrData(EffectType.Hit) ;
	}

	/// <summary>
	/// 生命恢复
	/// </summary>
	protected override int GetRecoHp ()
	{
		return GetAttrData(EffectType.RecoHp) ;
	}
	
	/// <summary>
	/// 怒气恢复
	/// </summary>
	protected override int GetRecoAnger ()
	{
		return GetAttrData(EffectType.RecoAnger) ;
	}	

	/// <summary>
	/// 治疗效果提高
	/// </summary>
	protected override int GetAddDoctor ()
	{
		return GetAttrData(EffectType.AddDoctor) ;
	}	

	/// <summary>
	/// 获取移动速度
	/// </summary>
	protected override float GetSpeed()
	{
		float ret = m_info.m_speed;
		/*if(Gstatus != null)
			ret *= (1+Gstatus.GetAttrData(EffectType.MoveSpeed)*0.01f);*/
		return ret ;
	}
	/// <summary>
	/// 获取移动速度
	/// </summary>
	protected override float GetSpeedercent()
	{
		float speed = 1f;
		if(Gstatus != null)
			speed = (1+Gstatus.GetAttrData(EffectType.MoveSpeed)*0.01f);
		return speed < 0.1f?0.1f:speed;
	}
	/// <summary>
	/// 获取攻击速度
	/// </summary>
	protected override float GetAttackSpeed()
	{
		
		float speed = 1f;
		if(Gstatus != null)
			speed = (1+Gstatus.GetAttrData(EffectType.AttackTime)*0.01f);
		return speed < 0.1f?0.1f:speed;
	}
	/// <summary>
	/// 获取闪避调整系数
	/// </summary>
	protected override int GetDodgeRatio()
	{
		int ret = m_DodgeRatio;
		if(Gstatus != null)
			ret += Gstatus.GetAttrData(EffectType.Hit)/MapGrid.m_Pixel;
		return ret ;
	}
	
	/// <summary>
	/// 获取闪避值
	/// </summary>
	protected override int GetDodge()
	{
		return GetAttrData(EffectType.Dodge) ; 
	}


	/// <summary>
	/// 获取是否隐形
	/// </summary>
	protected override bool GetHide()
	{
		return Gstatus.HaveState(StatusType.Invisible);
	}

	/// <summary>
	/// 获取抗压系统
	/// </summary>
	protected override int GetAntiPress()
	{
		return m_AntiPress;
	}
    protected override int GetAntiIce()
    {
        return m_AntiIce;
    }
	protected override int GetLevel()
	{
		return m_info.Level;
	}
	protected override int GetCritRatio()
	{
		return m_info.m_critratio;
	}
	protected override int GetModelType()
	{
		return m_info.m_modeltype;
	}
	/// <summary>
	/// 获取满血量
	/// </summary>
	protected override int GetFullHp()
	{
		return GetAttrData(EffectType.Hp)  ;
	}
	/// <summary>
	/// 获取满怒气
	/// </summary>
	protected override int GetFullAnger()
	{
		if(Gstatus != null)
			return ConfigM.GetAngerK(1) + Gstatus.GetAttrData(EffectType.Anger);
		return ConfigM.GetAngerK(1);
	}

	/// <summary>
	/// 跳跃距离
	/// </summary>
	protected override int GetJumpDistance()
	{
		return m_jumpDistance;
	}


	/// <summary>
	/// 建筑位置
	/// </summary>
	protected override Int2 GetPos()
	{
		return m_Pos;
	}

	/// <summary>
	/// 获取通道数
	/// </summary>
	protected override int GetPassageway()
	{
		return m_Passageway;
	}

	/// <summary>
	/// 获取攻击频率
	/// </summary>
	protected override float GetAttackTime()
	{
		return m_attack_time;
	}

	/// <summary>
	/// 设置HP
	/// </summary>
	protected override void SetHp(int Hp)
	{
		base.SetHp(Hp);
		EventCenter.DoEvent(NDEventType.Attr_HP, m_SceneID, Hp * 1.0f / GetFullHp());

		if (m_Hp<=0)
		{
			Anger = 0;
		}
	}

	/// <summary>
	/// 设置怒气
	/// </summary>
	protected override void SetAnger(int anger)
	{
		if(anger >= FullAnger)
		{
			m_Anger = FullAnger;
			IsAngerSkill = 1;
		}
		else 
		{
			if (m_Anger > 0 &&  anger <= 0)
				EventCenter.DoEvent(NDEventType.StatusInterrupt,m_SceneID,LifeAction.AngerEmety);
			m_Anger = anger;
			IsAngerSkill = 0;
		}
		if(m_Anger <= 0) m_Anger = 0;
		AttrChange = true;
		EventCenter.DoEvent(NDEventType.Attr_Anger,m_SceneID,m_Anger);
	}

	/// <summary>
	/// 技能释放方式
	/// </summary>
	protected override ReleaseType GetReleaseType()
	{
		if(Gstatus == null) 
			return ReleaseType.Normal;
		//眩晕状态
		if(Gstatus.HaveState(StatusType.Vertigo) == true)
			return ReleaseType.NoALL;
		//击飞状态
		if(Gstatus.HaveState(StatusType.ClickFly))
			return ReleaseType.NoALL;
		//变形状态
		if(Gstatus.HaveState(StatusType.Transfiguration))
			return ReleaseType.NoALL;
		//沉默状态
		if(Gstatus.HaveState(StatusType.Silence))
			return ReleaseType.NoMagic;
		//束缚状态
		if(Gstatus.HaveState(StatusType.Trapped))
			return ReleaseType.NoAttack;
		//击退状态
		if(Gstatus.HaveState(StatusType.KickedBack))
			return ReleaseType.NoALL;

		return ReleaseType.Normal;
	}


	/// <summary>
	/// 技能免疫
	/// </summary>
	protected override ImmuneSkill GetSkillImmune()
	{
		if(Gstatus == null) 
			return ImmuneSkill.Normal;
		//无敌状态
		if(Gstatus.HaveState(StatusType.Invincible) == true)
			return ImmuneSkill.Invincible;
		//击飞状态
		if(Gstatus.HaveState(StatusType.ClickFly) == true)
			return ImmuneSkill.Invincible;
		//虚化状态
		if(Gstatus.HaveState(StatusType.Virtual) == true)
			return ImmuneSkill.PhySkill;
		//物理免疫
		if(Gstatus.HaveState(StatusType.ImmunePhysical) == true)
			return ImmuneSkill.PhySkill;
		//魔法免疫
		if(Gstatus.HaveState(StatusType.ImmunityMagic) == true)
			return ImmuneSkill.MagicSkill;

		return ImmuneSkill.Normal;
	}

	/// <summary>
	/// 移动
	/// </summary>
	protected override bool GetCanMove()
	{
		if(Gstatus == null) 
			return true;

		//眩晕状态
		if(Gstatus.HaveState(StatusType.Vertigo) == true)
			return false;
		//击飞状态
		if(Gstatus.HaveState(StatusType.ClickFly))
			return false;

		//静止状态
		if(Gstatus.HaveState(StatusType.Still))
			return false;

		//束缚状态
		if(Gstatus.HaveState(StatusType.Trapped) == true)
			return false;

		return true;
	}
	/// <summary>
	/// 移动
	/// </summary>
	protected override bool GetCanAttack()
	{
		if(Gstatus == null) 
			return true;
		
		//眩晕状态
		if(Gstatus.HaveState(StatusType.Vertigo) == true)
			return false;
		//击飞状态
		if(Gstatus.HaveState(StatusType.ClickFly))
			return false;
		
		return true;
	}

	/// <summary>
	/// 获取能否触发陷阱房间
	/// </summary>
	protected override bool GetTriggerTrapBuilding()
	{
		if(Gstatus == null) 
			return true;
		//眩晕状态
		if(Gstatus.HaveState(StatusType.Invisible) == true)
			return false;
		//击飞状态
		if(Gstatus.HaveState(StatusType.ClickFly) == true)
			return false;

		return true;
	}
	/// <summary>
	/// 获取物理暴击伤害加成属性
	/// </summary>
	protected override int GetPhysicalCritBonusDamage()
	{
		return GetAttrData(EffectType.PhysicalCritBonusDamage);
	}
	/// <summary>
	/// 获取物理暴击伤害加成属性
	/// </summary>
	protected override int GetMagicCritBonusDamage()
	{
		return GetAttrData(EffectType.MagicCritBonusDamage);
	}

	/// <summary>
	/// 获取冰系伤害减免值
	/// </summary>
	protected override int GetWaterDamageReduction()
	{
		return GetAttrData(EffectType.WaterDamageReduction);
	}

}
