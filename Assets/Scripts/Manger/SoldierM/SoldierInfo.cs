using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


//角色定位
public enum CombatLoactionType
{
	NONE    =  1 << 0,  //默认无   
	Tank    =  1 << 1,  //坦克 肉盾
	DPS     =  1 << 2,  //输出
	Assist  =  1 << 3,  //辅助
	ALL     =  NONE | Tank | DPS | Assist,  
};

/// <summary>
/// 炮弹兵信息数据
/// </summary>
public class SoldierInfo  {
	private int m_id;
	public int ID
	{
		get{return m_id;}
		set{if(value != -1)
				m_id = value;}
	}

	private int m_cx;
	public int CX
	{
		get{return m_cx;}
		set{if(value != -1)
			m_cx = value;}
	}

	private int m_cy;
	public int CY
	{
		get{return m_cy;}
		set{if(value != -1)
			m_cy = value;}
	}



	private int m_soldierTypeID ;
	public int SoldierTypeID
	{
		get{return m_soldierTypeID;}
		set{if(value != -1)
			m_soldierTypeID = value;}
	}

	private int m_equipment0;
	public int Equipment0
	{
		get{return m_equipment0;}
		set{if(value != -1)
			m_equipment0 = value;}
	}

	private int m_equipment1;
	public int Equipment1
	{
		get{return m_equipment1;}
		set{if(value != -1)
			m_equipment1 = value;}
	}

	private int m_equipment2;
	public int Equipment2
	{
		get{return m_equipment2;}
		set{if(value != -1)
			m_equipment2 = value;}
	}

	private int m_equipment3;
	public int Equipment3
	{
		get{return m_equipment3;}
		set{if(value != -1)
			m_equipment3 = value;}
	}

	private int m_equipment4;
	public int Equipment4
	{
		get{return m_equipment4;}
		set{if(value != -1)
			m_equipment4 = value;}
	}

	private int m_equipment5;
	public int Equipment5
	{
		get{return m_equipment5;}
		set{if(value != -1)
			m_equipment5 = value;}
	}
	private int m_level;
	public int Level
	{
		get{return m_level;}
		set{if(value != -1)
			m_level = value;}
	}

	private int m_starlevel;
	public int StarLevel
	{
		get{return m_starlevel;}
		set{if(value != -1)
			m_starlevel = value;}
	}

	private int m_exp;
	public int EXP
	{
		get{return m_exp;}
		set{if(value != -1)
			m_exp = value;}
	}

	private int m_quality;
	public int Quality
	{
		get{return m_quality;}
		set{if(value != -1)
			m_quality = value;}
	}
    //
	public int m_soldier_group;
	public int m_soldier_type;
	public int m_main_proerty;
	public int m_attack_like; 
	public int m_attack_type; 
	public float m_speed ;
	public float m_Flyspeed ; 
	public float m_attack_time ;
	public int m_jump_distance ;
	public int m_hp ;
	public int m_mp;
	public int m_strength ;
	public int m_agility ;
	public int m_intelligence ;
    //初始
	public int m_hpstart;
	public int m_phy_attackstart;
	public int m_phy_defendstart;
	public int m_magic_attackstart;
	public int m_magic_defendstart;
	public int m_phy_critstart;
	public int m_magic_critstart;
	public int m_strengthstart ;
	public int m_agilitystart ;
	public int m_intelligencestart ;
    //
	public int m_strength_grow;
	public int m_agility_grow;
	public int m_intelligence_grow;
	public int m_phy_attack ;
	public int m_phy_defend ;
	public int m_magic_attack ;
	public int m_magic_defend ;
	public int m_phy_crit ;
	public int m_magic_crit ;
	public int m_dodge ;
	public int m_shape ;
	public int m_goorder;

	public int m_resist;
	public int m_passageway;
	public string m_name;
	public int m_modeltype;

	public int m_critratio;
	public int m_dodgeratio;

	public int m_CritAgainst;
	public int m_hit;
	public int m_damage;
	public int m_typedes;

	public int m_loaction;//肉盾 1 输出2 辅助3
	public string m_desc ;
	public int m_FireAI;
	public List<int> m_RewardItem =  new List<int>();
    /// <summary>
    /// 战斗力
    /// </summary>
    public int m_combat_power;

	/// <summary>
	/// 装备穿戴情况是否有变化.
	/// </summary>
	public bool m_EuqipCanPut;
	//冲击力
	public int m_concussion;
	/// <summary>
	/// 死亡音效 从string中随机一个.
	/// </summary>
	public string m_strDieSound = "";

	public SoldierSkillInfo m_Skill = new SoldierSkillInfo();
	public AddAttrInfo      m_AddAttr = new AddAttrInfo();
	//怪物死亡怒气
	public int m_dead_mp;
	public SoldierInfo m_TurnInfo = null;
	//灵魂石碎片
	public int fragmentTypeID;

	/// <summary>
	/// 碎片掉落描述.
	/// </summary>
	public string m_strDropDesc = "";

	public SoldierInfo()
	{

	}
	
    public float GetSoldierAttr(EffectType type)
    {
        switch (type)
        {
            case EffectType.Strength:
                return m_strength;
            case EffectType.Agility:
                return m_agility;
            case EffectType.Intelligence:
                return m_intelligence;
            case EffectType.Hp:
                return m_hp;
            case EffectType.Anger:
                return m_mp;
            case EffectType.PhyDamage://策划说改值没用了(返回0 的都是目前没用的)
                return 0;
            case EffectType.PhyAttack:
                return m_phy_attack;
            case EffectType.PhyDefense:
                return m_phy_defend;
            case EffectType.MagicDamage:
                return 0;
            case EffectType.MagicAttack:
                return m_magic_attack;
            case EffectType.MagicDefense:
                return m_magic_defend;
            case EffectType.Hit:
                return m_hit;
            case EffectType.Dodge:
                return m_dodge;
            case EffectType.PhyCrit:
                return m_phy_crit;
            case EffectType.MagicCrit:
                return m_magic_crit;
            case EffectType.MoveSpeed:
                return m_speed;
            case EffectType.AttackTime:
                return m_attack_time;
            case EffectType.Shape:
                return m_shape;

            default:
                return m_AddAttr.GetAttr(type);

        }
    }

	public void Copy( SoldierInfo s)
	{
		this.m_id = s.m_id;
		this.m_cx = s.m_cx;
		this.m_cy = s.m_cy;
		this.m_soldierTypeID = s.m_soldierTypeID;
		this.m_soldier_group = s.m_soldier_group;
		this.m_soldier_type = s.m_soldier_type;
		this.m_main_proerty = s.m_main_proerty;
		this.m_level = s.m_level;
		this.m_starlevel = s.m_starlevel;
		this.m_exp = s.m_exp;
		this.m_quality = s.m_quality;
		this.m_attack_like = s.m_attack_like;
		this.m_attack_type = s.m_attack_type;
		this.m_speed = s.m_speed;
		this.m_Flyspeed = s.m_Flyspeed;
		this.m_attack_time = s.m_attack_time;
		this.m_jump_distance = s.m_jump_distance;
		this.m_hp = s.m_hp;
		this.m_mp = s.m_mp;
		this.m_strength = s.m_strength;
		this.m_agility = s.m_agility;
		this.m_intelligence = s.m_intelligence;
		this.m_strengthstart = s.m_strengthstart;
		this.m_agilitystart = s.m_agilitystart;
		this.m_intelligencestart = s.m_intelligencestart;
		this.m_strength_grow = s.m_strength_grow;
		this.m_agility_grow = s.m_agility_grow;
		this.m_intelligence_grow = s.m_intelligence_grow;
		this.m_phy_attack = s.m_phy_attack;
		this.m_phy_defend = s.m_phy_defend;
		this.m_magic_attack = s.m_magic_attack;
		this.m_magic_defend = s.m_magic_defend;
		this.m_phy_crit = s.m_phy_crit;
		this.m_magic_crit = s.m_magic_crit;
		this.m_dodge = s.m_dodge;
		this.m_critratio = s.m_critratio;
		this.m_dodgeratio = s.m_dodgeratio;
		this.m_equipment0 = s.m_equipment0;
		this.m_equipment1 = s.m_equipment1;
		this.m_equipment2 = s.m_equipment2;
		this.m_equipment3 = s.m_equipment3;
		this.m_equipment4 = s.m_equipment4;
		this.m_equipment5 = s.m_equipment5;
		this.m_name = s.m_name;
		this.m_modeltype = s.m_modeltype;
		this.m_resist = s.m_resist;
		this.m_loaction = s.m_loaction;
		this.m_desc = s.m_desc;
		this.m_combat_power = s.m_combat_power;
		this.m_FireAI = s.m_FireAI;
		this.m_RewardItem = s.m_RewardItem;
		this.m_shape = s.m_shape;
		this.m_goorder = s.m_goorder;
		this.m_Skill.Copy(s.m_Skill);
		this.m_EuqipCanPut = s.m_EuqipCanPut;


		this.m_hpstart = s.m_hpstart;
		this.m_phy_attackstart = s.m_phy_attackstart;
		this.m_phy_defendstart = s.m_phy_defendstart;
		this.m_magic_attackstart = s.m_magic_attackstart;
		this.m_magic_defendstart = s.m_magic_defendstart;
		this.m_phy_critstart = s.m_phy_critstart;
		this.m_magic_critstart = s.m_magic_critstart;
		this.m_strDieSound = s.m_strDieSound;
		this.m_dead_mp = s.m_dead_mp;
		this.m_strDropDesc = s.m_strDropDesc;
	}


	/// <summary>
	/// 检测是否 
	/// </summary>
	/// <param name="info"></param>
	/// <returns></returns>
	public bool CheckHaveSkillUp()
	{
		List<SoldierSkill> lSkill = m_Skill.GetUpdateSkills();
		int i = 0;
		foreach (SoldierSkill skill in lSkill)
		{
			int m_iSkillNo = i ++;
			if(skill == null) continue ;

			
			int skillUpCoin = SoldierM.GetUpSkillLevelNeed(skill.m_level + 1, m_iSkillNo);
			int userCoin = UserDC.GetCoin();
			if (skillUpCoin > userCoin)
			{
				continue;
			}

			int teamMaxLevel = SoldierM.GetSoldierMaxLevel();
			int skillMaxLevel = ConfigM.GetSkillMaxLevel(m_iSkillNo);
			int q = teamMaxLevel - skillMaxLevel; //战队等级上限 - 技能等级上限
			int m = skill.m_level - Level - q;
			
			if (Level == skillMaxLevel)//技能等级未达到上限
			{
				continue;
			}
			
			if (m >= 1 && UserDC.GetLeftSkillPoints() >= 1)
				return true;
		}
		return false;
	}
	/// <summary>
	/// 检测炮弹兵是否满足进阶条件
	/// </summary>
	public bool CheckSoldierJinJie()
	{
		Dictionary<int ,int> l = new Dictionary<int, int>();
		// 是否最高阶
		if (ConfigM.GetNextQuality(Quality) == 0)
			return false;
		int levelLimit = 0;
		int equipID1 = 0;
		int equipID2 = 0;
		int equipID3 = 0;
		int equipID4 = 0;
		int equipID5 = 0;
		int equipID6 = 0;

		SoldierM.GetUpQualityNeed(SoldierTypeID, Quality, ref levelLimit,
		                          ref equipID1, ref equipID2, ref equipID3,
		                          ref equipID4, ref equipID5, ref equipID6);
		// 等级限制
		if (Level < levelLimit) return false;

		//需要的装备
		List<int> lEquipsID = new List<int>();
		if (Equipment0 <= 0) lEquipsID.Add(equipID1);
		if (Equipment1<= 0) lEquipsID.Add(equipID2);
		if (Equipment2 <= 0) lEquipsID.Add(equipID3);
		if (Equipment3 <= 0) lEquipsID.Add(equipID4);
		if (Equipment4 <= 0) lEquipsID.Add(equipID5);
		if (Equipment5 <= 0) lEquipsID.Add(equipID6);
		// 不缺装备
		if(lEquipsID.Count == 0) return true;
		//统计需要的装备
		foreach(int itemid in lEquipsID)
		{
			ItemTypeInfo ItemInfo = ItemDC.SearchItem(itemid);//检测装备道具是否存在
			//检测装备是否存在
			if (ItemInfo == null) return false;
			//检测等级是否满足装备要求
			if (Level < ItemInfo.m_level) return false;
			
			if(l.ContainsKey(itemid) == false)
			{
				l.Add(itemid,1);
			}
			else l[itemid] = l[itemid] + 1;
		}
		//判断装备数量是否够
		foreach(int itemid in l.Keys)
		{
			int num = ItemDC.CheckItem(itemid);
			if(num < l[itemid]) return false;
		}
		return true;
	}
	/// <summary>
	/// 检测是否存在这样的炮弹兵：存在空闲装备位，且装备可以通过合成得到（金币足够）或直接关卡掉落得到
	/// </summary>
	public  bool CheckExistEquipPosNoEquip()
	{
		List<int> listEquipID = new List<int>();
		listEquipID.Add(Equipment0);
		listEquipID.Add(Equipment1);
		listEquipID.Add(Equipment2);
		listEquipID.Add(Equipment3);
		listEquipID.Add(Equipment4);
		listEquipID.Add(Equipment5);
		
		for (int i = 0; i < listEquipID.Count; i++)
		{
			int itemTypeID = 0;
			EquipmentPutType type = SoldierM.CheckCanPutEquip(this, listEquipID[i], i, ref itemTypeID);
			if (type == EquipmentPutType.NoCanCombine || type== EquipmentPutType.ReadyCombine)
			{
				return true;
			}
		}
		return false;
	}
	/// <summary>
	/// 装备是否可穿戴.
	/// </summary>
	/// <returns><c>true</c>, if equip can put was calculated, <c>false</c> otherwise.</returns>
	/// <param name="Info">Info.</param>
	public bool CheckEquipCanPut()
	{
		bool bRedPot = false;
		
		List<int> listEquipID = new List<int>();
		listEquipID.Add(Equipment0);
		listEquipID.Add(Equipment1);
		listEquipID.Add(Equipment2);
		listEquipID.Add(Equipment3);
		listEquipID.Add(Equipment4);
		listEquipID.Add(Equipment5);
		
		for (int i = 0; i < listEquipID.Count; i++)
		{
			int itemTypeID = 0;
			EquipmentPutType type = SoldierM.CheckCanPutEquip(this,listEquipID[i],i,ref itemTypeID);
			
			if(!bRedPot)
			{
				bRedPot = (type == EquipmentPutType.CanPut) || (type == EquipmentPutType.CanCombinePut);
				if(bRedPot)
				{
					break;
				}
				
			}
		}
		
		return bRedPot;
	}
	/// <summary>
	/// 确认能被召唤
	/// </summary>
	/// <returns><c>true</c></returns>
	/// <param name="Info"></param>
	public  bool CheckCanSummon()
	{
		return SoldierM.CheckCanSummon(this.SoldierTypeID);
	}
	/// <summary>
	/// 获取碎片数量
	/// </summary>
	public int GetHaveFragmentNum()
	{
		return ItemDC.GetItemCount(fragmentTypeID);
	}
	/// <summary>
	/// 添加掉落物品奖励
	/// </summary>
	public void SetRewardItems(List<int> lReward)
	{
		if(m_RewardItem == null)
			m_RewardItem = new List<int>();
		m_RewardItem.Clear();
		if(lReward == null || lReward.Count == 0)
			return ;
		else m_RewardItem.AddRange(lReward);
	}
	
	public SoldierSkill GetSkill(int SkillNo)
	{
		return m_Skill.GetSkill(SkillNo);

	}
	
	public SoldierSkill GetBigSkill()
	{
		return m_Skill.GetBigSkill();
	}

	public SoldierSkill GetFireSkill()
	{
		return m_Skill.GetFireSkill();
	}
	/// <summary>
	/// 获取次要攻击技能
	/// </summary>
	public SoldierSkill GetLSkill()
	{
		return m_Skill.GetLSkill();
	}

}

/// <summary>
/// 召唤物信息
/// </summary>
public class SummonpetInfo 
{
	public int 		m_id;
	public string   m_name;
	public int		m_type;
	public int      m_modeltype;
	public int 		m_isattack;
	public int      m_attack_like; 
	public int      m_attack_type; 
	public int      m_level;
	public int      m_shape;
	public int      m_goorder;
	public float    m_speed ;
	public float    m_attack_time ;
	public int      m_jump_distance ;
	public int      m_resist;
	public int      m_hp ;
	public int      m_phy_attack ;
	public int      m_phy_defend ;
	public int      m_magic_attack ;
	public int      m_magic_defend ;
	public int      m_phy_crit ;
	public int      m_magic_crit ;
	public int      m_dodge ;
	public int      m_time;
	public AddAttrInfo      m_AddAttr = new AddAttrInfo();
	public List<SoldierSkill> m_Skillinfo  = new List<SoldierSkill>();
	public int m_critratio;
	public int m_dodgeratio;
}

/// <summary>
/// 道具信息
/// </summary>
public class SummonProsInfo 
{
	public int 		m_id;
	public int		m_type;
	public string   m_name;
	public int      m_modeltype;
	public int 		m_isattack;
	public int      m_taget ;
	public int      m_hp ;
	public float    m_range;
	public float    m_time;
	public float    m_timeinterval;
	/// <summary>
	/// 道具伤害属性
	/// </summary>
	public AddAttrInfo  m_ReduceAttr = new AddAttrInfo();
}



public class PetInfo {
	public int 		m_id;
	public int      m_type;
	public int 		LifeTime;
	public int      m_mp ;
	public int      m_damage ;
	public string 	m_name;
	public int 		m_isattack;
	public int 		m_attacktype;
	public int 		m_speedpercent;
	public int 		m_jumppercent;
	public int 		m_hppercent;
	public int 		m_phydamagepercent;
	public int 		m_phyattackpercent;
	public int 		m_phydefendpercent;
	public int 		m_magicattackpercent;
	public int 		m_magicdamagepercent;
	public int 		m_magicdefendpercent;
	public int 		m_phy_critpercent;
	public int 		m_magic_critpercent;
	public List<SkillStatusInfo> m_status = new List<SkillStatusInfo>();
	public int		m_skill1;
	public SoldierSkill m_skillinfo1;
	public PetInfo() {}
};



//lifeM核心数据
public class LifeMCore
{
	public int        m_DataID ;
	public bool       m_IsPlayer;   //是否为玩家
	public LifeMType  m_type;
	public LifeMCamp  m_Camp;
	public MoveState  m_MoveState;
	public bool		  m_bTurn = false;		//是否变身

	public LifeMCore()
	{
	}
	

	public LifeMCore(int Data ,bool IsPlayer, LifeMType Type, LifeMCamp Camp ,MoveState State)
	{
		this.m_DataID = Data;
		this.m_IsPlayer = IsPlayer;
		this.m_type = Type;
		this.m_Camp = Camp;
		this.m_MoveState = State;
	}

	public LifeMCore( LifeMCore s)
	{
		Copy(s);
	}
	
	public void Copy( LifeMCore s)
	{
		this.m_DataID = s.m_DataID;
		this.m_type = s.m_type;
		this.m_Camp = s.m_Camp;
		this.m_MoveState = s.m_MoveState;
		this.m_IsPlayer = s.m_IsPlayer;
	}
}


