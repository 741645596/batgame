using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using sdata;

/// <summary>
/// 召唤物数据
/// </summary>
public class SummonM  {

	private static List<s_summonpetInfo> m_lsummonpet = new List<s_summonpetInfo>();
	private static List<s_summonprosInfo> m_lsummonpros = new List<s_summonprosInfo>();
	private static List<sdata.s_summonInfo> m_lPet = new List<sdata.s_summonInfo>();

	
	public static void Init(object obj)
	{

		System.Diagnostics.Debug.Assert(obj is sdata.StaticDataResponse);
		StaticDataResponse sdrsp = obj as StaticDataResponse;
		
		m_lsummonpet = sdrsp.s_summonpet_info;
		m_lsummonpros = sdrsp.s_summonpros_info;
		m_lPet = sdrsp.s_summon_info;
	}
	/// <summary>
	/// 获取召唤物
	/// </summary>
	private static s_summonpetInfo GetSummonpet(int id)
	{
		foreach(s_summonpetInfo I in m_lsummonpet)
		{
			if(I.id == id)
			{
				return I;
			}
		}
		return null;
	}

	/// <summary>
	/// 获取道具
	/// </summary>
	private static s_summonprosInfo GetSummonpros(int id)
	{
		foreach(s_summonprosInfo I in m_lsummonpros)
		{
			if(I.id == id)
			{
				return I;
			}
		}
		return null;
	}

	/// <summary>
	/// 设置召唤物数据
	/// </summary>
	private static void SetSummonPetInfo(ref SummonpetInfo Info ,s_summonpetInfo I)
	{
		if(Info == null || I == null)
			return ;
		Info.m_id = I.id;
		Info.m_name = I.name;
		Info.m_type = I.type;
		Info.m_modeltype = I.modeltype;
		Info.m_isattack = I.isattack;
		Info.m_level = I.level;
		Info.m_shape = I.shape;
		Info.m_goorder = I.goorder;
		Info.m_attack_like = I.attack_like;
		Info.m_attack_type = I.attacktype;
		Info.m_attack_time = I.attack_time * 0.001f;
		Info.m_speed = I.speed * 1.0f /MapGrid.m_Pixel;
		Info.m_jump_distance = I.jump_distance  /MapGrid.m_Pixel;
		Info.m_resist = I.resist;
		Info.m_hp = I.hp;
		Info.m_phy_attack = I.physical_attack;
		Info.m_phy_defend = I.physical_defend;
		Info.m_phy_crit = I.physical_crit;
		Info.m_magic_attack = I.magic_attack;
		Info.m_magic_defend = I.magic_defend;
		Info.m_magic_crit = I.magic_crit;
		Info.m_dodge = I.dodge;
		Info.m_time = I.time;
		Info.m_AddAttr.SetAddAttrInfo(I);


		//爆击及闪避
		s_soldier_experienceInfo ExpInfo = SoldierM.GetSoldierExpData(I.level);
		if(ExpInfo != null)
		{
			Info.m_critratio = ExpInfo.critratio;
			Info.m_dodgeratio = ExpInfo.dodgeratio;
		}
		//
		if(I.gskillid > 0)
		{
			SoldierSkill skill = new SoldierSkill();
			if(SkillM.GetSkillInfo(I.gskillid ,ref skill )== true)
			{
				Info.m_Skillinfo.Add(skill);
			}
		}
		if(I.skill1id > 0)
		{
			SoldierSkill skill = new SoldierSkill();
			if(SkillM.GetSkillInfo(I.skill1id ,ref skill )== true)
			{
				Info.m_Skillinfo.Add(skill);
			}
		}

		if(I.skill2id > 0)
		{
			SoldierSkill skill = new SoldierSkill();
			if(SkillM.GetSkillInfo(I.skill2id ,ref skill )== true)
			{
				Info.m_Skillinfo.Add(skill);
			}
		}

		if(I.skill3id > 0)
		{
			SoldierSkill skill = new SoldierSkill();
			if(SkillM.GetSkillInfo(I.skill3id ,ref skill )== true)
			{
				Info.m_Skillinfo.Add(skill);
			}
		}

		if(I.skill4id > 0)
		{
			SoldierSkill skill = new SoldierSkill();
			if(SkillM.GetSkillInfo(I.skill4id ,ref skill )== true)
			{
				Info.m_Skillinfo.Add(skill);
			}
		}
	}
	/// <summary>
	/// 设置道具数据
	/// </summary>
	private static void SetSummonProsInfo(ref SummonProsInfo Info ,s_summonprosInfo I)
	{
		if(Info == null || I == null)
			return ;
		Info.m_id = I.id;
		Info.m_name = I.name;
		Info.m_modeltype = I.modeltype;
		Info.m_isattack = I.isattack;
		Info.m_taget = I.target;
		Info.m_hp = I.hp;
		Info.m_range = I.range * 1.0f / MapGrid.m_Pixel ;
		Info.m_time = I.time * 0.001f;
		Info.m_timeinterval = I.timeinterval * 0.001f;
		Info.m_ReduceAttr.SetAddAttrInfo(I);
	}
	/// <summary>
	/// 获取道具
	/// </summary>
	public static SummonProsInfo GetSummonProsInfo (int ProsID)
	{
		s_summonprosInfo I = GetSummonpros(ProsID);
		if(I == null) return null;
		else 
		{
			SummonProsInfo Info = new SummonProsInfo();
			SetSummonProsInfo(ref Info,I);
			return Info;
		}
	}
	/// <summary>
	/// 获取召唤物
	/// </summary>
	public static SummonpetInfo GetSummonPetInfo (int PetID)
	{
		s_summonpetInfo I = GetSummonpet(PetID);
		if(I == null) return null;
		else 
		{
			SummonpetInfo Info = new SummonpetInfo();
			SetSummonPetInfo(ref Info,I);
			return Info;
		}
	}




	public static PetInfo GetPetInfo (int PetID)
	{
		foreach(sdata.s_summonInfo I in m_lPet)
		{
			
			if(I.id == PetID)
			{
				PetInfo Info = new PetInfo();
				Info.m_id = I.id;
				Info.m_type = I.type;
				Info.m_mp = I.mp;
				Info.m_damage = I.damage;
				Info.m_name = I.name;
				Info.LifeTime = I.time;
				Info.m_isattack = I.isattack;
				Info.m_attacktype = I.attacktype;
				Info.m_speedpercent = I.speedpercent;
				Info.m_jumppercent = I.jumppercent;
				Info.m_hppercent = I.hppercent;
				Info.m_phydamagepercent = I.phydamagepercent;
				Info.m_phyattackpercent = I.phyattackpercent;
				Info.m_phydefendpercent = I.phydefendpercent;
				Info.m_magicattackpercent = I.magicattackpercent;
				Info.m_magicdamagepercent = I.magicdamagepercent;
				Info.m_magicdefendpercent = I.magicdefendpercent;
				Info.m_phy_critpercent = I.phy_critpercent;
				Info.m_magic_critpercent = I.magic_critpercent;
				Info.m_skill1 = I.skill1;
				
				SoldierSkill skillInfo = new SoldierSkill();
				//0
				SkillM.GetSkillInfo (Info.m_skill1,  ref skillInfo);
				Info.m_skillinfo1 = skillInfo;
				SkillM.SetSkillStatus(ref Info.m_status , I.status);
				return Info;
			}
		}
		return null;
	}




}
