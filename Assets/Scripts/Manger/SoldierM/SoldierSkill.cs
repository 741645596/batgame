using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum AttributeType
{
	NONE    = 1 << 0,  //钱属性
	Fire    = 1 << 1,  //火属性
	Water   = 1 << 2,  //水属性
	Electric = 1 << 3,  //电属性
	Poison  = 1 << 4,  //毒属性
	Gas     = 1 << 5,  //气属性
	Physical = 1 << 6,  //物理性
	ALL =  NONE | Fire | Water |Electric | Poison | Gas | Physical,
};





/// <summary>
/// 技能数据
/// </summary>
public class SkillInfo
{

	public bool m_Relsease;
	public MapGrid m_TargetPos;
	public Life m_LifeTarget;
	public bool m_enable = true;
	public int  m_enableQuality ;
	public Vector3 m_vTargetPos;
	
	public AttributeType m_AttributeType;

	//type
	public int m_type;
	public string m_name;
	public int m_cd;
	public int m_multiple;
	public List<SkillStatusInfo> m_releasedown_status = new List<SkillStatusInfo>();
	public List<SkillStatusInfo> m_releasedenemy_status = new List<SkillStatusInfo>();
	public int m_interrupt_skill;
	public List<AttackPower> m_lAttackPower = new List<AttackPower>();
	public int m_target ;//0=敌人所有炮弹兵和金库，1=自己，2=对地使用(敌方所有炮弹兵),3=己方所有炮弹兵(不包含自己),4=已方中的一个(multiple 要匹配单体)，5=敌方所有炮弹兵
	public string m_struckeffect;
	public int m_attacktype;
	public int m_attckmodeid;
	public SkillEffectInfo m_skilleffectinfo;


	//up
	public int m_id;
	public int m_level;
	public int m_power1;
	public int m_power2;
	public int m_power3;
	public int m_power4;
	public List<SkillStatusInfo> m_attack_status_own = new List<SkillStatusInfo>();
	public List<SkillStatusInfo> m_attack_status_enemy = new List<SkillStatusInfo>();

	public void SetTarget(Life target,MapGrid pos)
	{
		m_LifeTarget =  target;
		m_TargetPos = pos;
	}
	public void SetTargetV3Pos(Vector3 vPos)
	{
		m_vTargetPos = vPos;
	}
	public void GetAttackTimes(ref List<float>ltimes)
	{
		if(ltimes == null)
			ltimes = new List<float>();
		ltimes.Clear();
		foreach(AttackPower p in m_lAttackPower)
		{
			ltimes.Add(p.time);
		}
	}

	public float GetAttackPower(int index)
	{
		if(index <0 || m_lAttackPower.Count <= index)
			return 0.0f;
		else return m_lAttackPower[index].power;
	}

	public void ClearCombatData()
	{
		m_LifeTarget = null;
		m_TargetPos = null;
		m_vTargetPos = Vector3.zero;
	}
}


/// <summary>
/// 炮弹兵技能数据 s_skilltype
/// </summary>
public class SoldierSkill : SkillInfo
{
	//public int m_soldierid;
	//public int m_percent;
	//public int m_attr_type;
	//public int m_ldistance;
	//public int m_trapbreach;
	public int m_style;

	//type
	public int m_priority;
	public int m_ipriority;
	public int m_ismove;
	public int m_sort;//1=目标攻击，2=线性攻击，3=圆形攻击，4=扇形攻击，5=球形攻击，6=跳跃攻击，7=置换攻击，8=传送，默认填0表示没有攻击性
	public int m_actiontype;//0=一次性伤害技能，1=持续性伤害技能， 3=被动属性技能(被动技能专用)，4=传送技能，5=召唤技能
	public int m_use_mp;
	public int m_distance;
	public int m_term1;
	public int m_term2;
	public int m_term3;
	public int m_intone_speed;
	public int m_step_secs;
	public int m_timeinterval;
	public float m_range;
	public int m_condition;
	public string m_description1 = string.Empty;
	public float m_blackscreentime;
	public int m_ischangetarget;

	public int m_targettype;
	public int m_damagetargettype;
	public float m_status_hitratio;
	public int m_condition_data0;
	public int m_condition_data1;
	public List<SkillStatusInfo> m_releasedself_status = new List<SkillStatusInfo>();
	// up
	public int m_rankslevel;
	public string m_description2 = string.Empty;
	public List<SkillStatusInfo> m_own_status = new List<SkillStatusInfo>();
	public List<SkillStatusInfo> m_buildstatus = new List<SkillStatusInfo>();
	public int m_data0;
	public int m_data1;
	public int m_data2;
	public int m_data3;
	public int m_Screen;
	public float m_ScreenTime;
}

public class SkillEffectInfo {
	public int 		m_id;
	public int 		m_skilltype;
	public int 		m_locus;
	public string 	m_targeteffect;
	public int 		m_hiteffect;
	public string 	m_hitaudio;
	public int 		m_isloop;
	public int 		m_postion;
};
public class SkillStatusInfo {
	public int 		m_id;
	public string 	m_name;
	public int 		m_type;
	public List<int> m_effect = new List<int>();
	public int 		m_effectlevel;
	public int 		m_persistence;
	public int		m_time;
	public int 		m_effecttime;
	public int 		m_timeinterval;
	public int		m_effectid;
	public int      m_bufftype;  // 0 debuff ,1 buff
	public int      m_position;
	public int		m_condition;
	public int		m_data0;
	public int		m_data1;
    public int      m_skill1type;
    public int      m_level1;
    public int      m_skill2type;
    public int      m_level2;
	public int		m_releasesceneid;
	public SkillStatusReplaceInfo m_RelpaceInfo;
};

public class SkillStatusReplaceInfo{
	public int m_id;
	public int m_statustype;
	public int m_effect;
	public string m_name;
	public int m_type;
	public int m_compatible;
	public int m_reppriority;
	public int m_spepriority;
	public int m_bufftype;
	public int m_isaction;
}

public class AttackPower {
	public float 	time;
	public float 	power;
	public AttackPower(){}
	public AttackPower( float time ,float power)
	{
		this.time = time;
		this.power = power;
	}
};

public enum StatusState
{
	Add    = 0, //状态添加
	Remove = 1, //状态移除
	SoonDie = 2, //即将要死
}
//状态信息数据
public class StatusInfo
{
	public StatusType Type;   //状态类型
	public StatusState State; //状态标示
	public float time;        //状态持续时间
	public int effectid;      //状态特效id
	public int position;      //状态挂载位置
	public int SkillType;     //由什么技能产生
    public int exSkill1;      //这个状态将会产生的技能1
    public int exSkillLevel1;        //这个状态将会产生的技能等级1
    public int exSkill2;        //这个状态将会产生的技能2
    public int exSkillLevel2;        //这个状态将会产生的技能等级2
	public int Releasescentid;
	public List<int> effect;
	public int SpePriority; //兼容表现优先级，数值越到越外层
	public bool isAction;
}

