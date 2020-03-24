using System;
using System.Collections.Generic;

/// <summary>
/// 策划公式 
/// 要求策划公式函数名称 使用全部大写字母，
/// 函数名称方式为CALC_XXX_XXX 形式，XXX 为英文单词（太长使用缩写，保证XXX长度小于等于8）
/// 请参照下面的列子，为计算升级所需要的经验
/// 每个公式需注释说明
/// </summary>
/// <author>zhulin</author>

//	怒气累计方式
public enum AngerAddWay
{
	Attack, // 攻击时
	BeAttacked, // 被攻击时
	KillSoldier, // 杀死敌方兵种时
	KillHero, // 杀死敌方英雄时
	KillBuild
}

public partial class ScriptM
{
	/// <summary>
	/// 计算当前力量
	/// </summary>
	/// <param name="strength_growth">力量成长</param>
	/// <param name="level">等级</param>
	/// <param name="strength">初始力量</param>
	/// <returns>力量</returns>
	private static int CALC_STRENGTH(int strength_growth,int original_growth,int level,int strength) 
	{
		float value =strength_growth * 0.01f * (level -1) + (strength_growth - original_growth)*0.01f;
		value += strength;
		return (int)value;
	}

	/// <summary>
	/// 计算当前敏捷
	/// </summary>
	/// <param name="agility_growth">敏捷成长</param>
	/// <param name="level">等级</param>
	/// <param name="agility">初始敏捷</param>
	/// <returns>力量</returns>
	private static int CALC_AGILITY(int agility_growth,int original_growth,int level,int agility) 
	{
		float value =agility_growth * 0.01f * (level -1)+ (agility_growth - original_growth)*0.01f;
		value += agility;
		return (int)value;
	}

	/// <summary>
	/// 计算当前智力
	/// </summary>
	/// <param name="intelligence_growth">智力成长</param>
	/// <param name="level">等级</param>
	/// <param name="intelligence">初始智力</param>
	/// <returns>力量</returns>
	private static int CALC_INTELLIGENCE(int intelligence_growth,int original_growth,int level,int intelligence)
	{
		float value =intelligence_growth * 0.01f * (level -1)+ (intelligence_growth - original_growth)*0.01f;
		value += intelligence;
		return (int)value;
	}

	/// <summary>
	/// 计算当前Hp
	/// </summary>
	/// <param name="hp">初始HP</param>
	/// <param name="Addstrength">增长力量</param>
	/// <returns>Hp</returns>
	private static int CALC_HP(int hp,int Addstrength )
	{
		float k1 = ConfigM.GetAttributeK(1) * 1.0f;
		float value = hp + Addstrength * k1;

		return (int)value;
	}
	/// <summary>
	/// 计算当前物理攻击力
	/// </summary>
	/// <param name="phyattack">初始物理攻击</param>
	/// <param name="main_proterty">主属性</param>
	/// <param name="Addstrength">增长力量</param>
	/// <param name="Addagility">增长敏捷</param>
	/// <param name="Addintelligence">增长智力</param>
	/// <returns>物理伤害</returns>
	private static int CALC_SOLDIER_PHYATTACK(int phyattack,
	                                          int main_proterty,
	                                          int Addstrength,
	                                          int Addagility,
	                                          int Addintelligence)
	{
		float k2 = ConfigM.GetAttributeK(2) * 0.01f;
		float value = phyattack + Addagility * k2;

		if (main_proterty == 0)
		{
			value +=  Addstrength ;
		}
		else if (main_proterty == 1)
		{
			value += Addagility;
		}
		else if (main_proterty == 2)
		{
			value += Addintelligence;
		}
		return (int)value;
	}	
	/// <summary>
	/// 计算当前物理防御
	/// </summary>
	/// <param name="phydefend">初始物理防御</param>
	/// <param name="Addstrength">增长力量</param>
	/// <param name="Addagility">增长敏捷</param>
	/// <returns>物理防御</returns>
	private static int CALC_SOLDIER_PHYDEFEND(int phydefend,
	                                          int Addstrength,
	                                          int Addagility)
	{
		float k3 = ConfigM.GetAttributeK(3) * 1.0f;
		float k4 = ConfigM.GetAttributeK(4) * 1.0f;
		float value = phydefend + Addstrength / k3 + Addagility / k4;

		return (int)value;
	}

	/// <summary>
	/// 计算当前魔法攻击力
	/// </summary>
	/// <param name="magicattack">初始魔法攻击</param>
	/// <param name="Addintelligence">增长智力</param>
	/// <returns>魔法攻击</returns>
	private static int CALC_SOLDIER_MAGICATTACK(int magicattack,
	                                            int Addintelligence)
	{
		float k5 = ConfigM.GetAttributeK(5) *0.01f;
		float value = magicattack + Addintelligence * k5;
		return (int)value;
	}

	/// <summary>
	/// 计算当前魔法防御
	/// </summary>
	/// <param name="magicdefend">初始魔法防御</param>
	/// <param name="Addintelligence">增长智力</param>
	/// <returns>魔法防御</returns>
	private static int 	CALC_SOLDIER_MAGICDEFEND(int magicdefend,
	                                             int Addintelligence)
	{
		float k6 = ConfigM.GetAttributeK(6) *0.01f;
		float value = magicdefend + Addintelligence * k6;
		return (int)value;
	}

	/// <summary>
	/// 计算当前物理暴击
	/// </summary>
	/// <param name="phycrit">初始物理暴击</param>
	/// <param name="Addagility">增长敏捷</param>
	/// <returns>物理暴击</returns>
	private static int CALC_SOLDIER_PHYCRIT(int phycrit,
	                                        int Addagility)
	{
		float k7 = ConfigM.GetAttributeK(7) * 0.01f;
		float value = phycrit + Addagility * k7;
		return (int)value;
	}

	/// <summary>
	/// 计算当前魔法暴击
	/// </summary>
	/// <param name="magiccrit">初始魔法暴击</param>
	/// <param name="Addintelligence">增长智力</param>
	/// <returns>魔法暴击</returns>
	private static int CALC_SOLDIER_MAGICCRIT(int magiccrit,
	                                          int Addintelligence)
	{
		float k7 = ConfigM.GetAttributeK(7) * 0.01f;
		float value = magiccrit + Addintelligence * k7;
		return (int)value;
	}



	//游戏中需要的策划数值计算公式
	//计算炮战时候伤害
	//flydamage表示攻击方炮弹兵炮战伤害
	//k1为常量，表示飞行伤害衰减系数
	//N表示炮弹兵击碎的建筑物个数
	//策划文档：战斗系统——炮弹兵炮战伤害计算说明
	private static int CALC_FIREDAMAGE_LOSS(int flydamage,
	                                        int N)
	{
		//计算炮弹兵击碎N个建筑物后的最终飞行伤害系数
		int k1 = ConfigM.GetGunK(1);
		float k = 1.0f;

		while ( N > 0)
		{
			k = k /k1;
			N --;
		}
		int lossHp = (int)(flydamage * k);
		return lossHp;
	}

	/// <summary>
	/// 物理伤害公式
	/// </summary>
	/// <param name="s1_skillpercent">技能伤害千分比</param>
	/// <param name="s1_phyattack">物理攻击</param>
	/// <param name="ReducePhyDefend">物理穿透护甲</param>
	/// <param name="s2_phydefend">防御方护甲</param>
	/// <param name="ReducePhyDamage">物理减免伤害</param>
	/// <param name="s1_skilldamage">技能伤害固定值</param>
	/// <returns>物理伤害</returns>
	private static int CALC_SOLDIER_PHYDAMAGE(int s1_skillpercent,
	                                          int s1_phyattack,
	                                          int ReducePhyDefend,
	                                          int s2_phydefend,
	                                          int ReducePhyDamage,
	                                          int s1_skilldamage)
	{
		float k13 = ConfigM.GetWhiteNinjaK(1) *0.001f;
		float k14 = ConfigM.GetWhiteNinjaK(2) *0.001f;
		float SkillPercent = s1_skillpercent *0.001f;
		//绝对防御值。扣除穿透护甲
		int Defense = s2_phydefend - ReducePhyDefend; 
		if(Defense < 0) Defense = 0;

		float f = Defense * k13 ;
		float g = 1.0f + Defense * k14;

		float ff = 1.0f - (f / g);

		float value = SkillPercent * (s1_phyattack * ff - ReducePhyDamage) + s1_skilldamage ;

		return (int)value;
	}

	/// <summary>
	/// 魔法伤害公式
	/// </summary>
	/// <param name="s1_skillpercent">技能伤害千分比</param>
	/// <param name="s1_magicattack">魔法攻击</param>
	/// <param name="ReduceMagicDefend">魔法忽视防御</param>
	/// <param name="s2_magicdefend">防御方护甲</param>
	/// <param name="ReduceMagicDamage">魔法减免伤害</param>
	/// <param name="s1_skilldamage">技能伤害固定值</param>
	/// <returns>物理伤害</returns>
	private static int CALC_SOLDIER_MAGICDAMAGE(int s1_skillpercent,
	                                            int s1_magicattack,
	                                            int ReduceMagicDefend,
	                                            int s2_magicdefend,
	                                            int ReduceMagicDamage,
	                                            int s1_skilldamage)
	{
		float k15 = ConfigM.GetWhiteNinjaK(3) *0.001f;
		float SkillPercent = s1_skillpercent *0.001f;
		//魔方伤害减免百分比
		float ReducePercent =  ConfigM.GetReduceMagicPercent(s2_magicdefend);

		float ff = 1.0f + ReduceMagicDefend * k15 - ReducePercent;
		if(ff < 0.0f)
		 UnityEngine.Debug.LogError("出现负值");


		float value = SkillPercent * (s1_magicattack * ff- ReduceMagicDamage) + s1_skilldamage;

		return (int)value;
	}
	
	/// <summary>
	/// 物理暴击率
	/// </summary>
	/// <param name="Attackphycrit">攻击方物理暴击值</param>
	/// <param name="DefenseCritagainst">防御方暴击调整值</param>
	private static float CALC_SOLDIER_PHYCRITS(int AttackPhycrit,int DefenseCritagainst)
	{
		return NdUtil.IDivide(AttackPhycrit , DefenseCritagainst);
	}

	/// <summary>
	/// 魔法暴击率
	/// </summary>
	/// <param name="Attackphycrit">攻击方魔法暴击值</param>
	/// <param name="Defensecritagainst">防御方暴击调整值</param>
	private static float CALC_SOLDIER_MAGICCRITS(int AttackMagiccrit,int DefenseCritagainst)
	{
		return NdUtil.IDivide(AttackMagiccrit ,DefenseCritagainst);
	}

	/// <summary>
	/// 命中率
	/// </summary>
	/// <param name="AttackHit">攻击方命中值</param>
	/// <param name="DefenseDodgeRatio">防守方闪避调整值</param>
	/// <param name="DefenseDodge">防御方闪避值</param>
	/// <returns>命中效率</returns>
	private static float CALC_SOLDIER_HIT(int AttackHit,int DefenseDodgeRatio,int DefenseDodge) 
	{
		float f = DefenseDodgeRatio *1.0f; 
		float g = (AttackHit - DefenseDodge) * 1.0f; 
		float value = g / DefenseDodgeRatio + 1.0f;
		return value;
	}

	//物理暴击伤害
	//phydamage表示物理攻击伤害
	//策划文档：白刃战伤害计算
	private static int CALC_SOLDIER_PHYCRITDAMAGE(int phydamage,int PhysicalCritBonusDamage)
	{
		return (int)(phydamage * (2 + PhysicalCritBonusDamage * 0.001f ));
	}

	//魔法暴击伤害
	//magicdamage表示魔法攻击伤害
	//策划文档：白刃战伤害计算
	private static int CALC_SOLDIER_MAGICCRITDAMAGE(int magicdamage,int MagicCritBonusDamage)
	{
		return (int)(magicdamage * (2 + MagicCritBonusDamage * 0.001f));
	}

	//船体摧毁率
	//lb 建筑损失数
	//fb 建筑总数
	//lh 英雄损失数
	//fh 英雄总数
	//ls 炮弹兵损失数
	//fs 炮弹兵总数
	//策划文档：PVP战斗胜负结算说明
	private static int CALC_BOAT_DESTROYRATE(int lb, int fb, int lh, int fh, int ls, int fs)
	{
		/*
		UnityEngine.Debug.Log(String.Format(
			"建筑损失数={0}，建筑总数={1}，英雄损失数={2},  英雄总数={3},炮弹兵损失数={4} , 炮弹兵总数={5}",
		                                    lb,fb,lh,fh,ls,fs));
		*/
		int k1 = ConfigM.GetBoatCombatK(1);
		int k2 = ConfigM.GetBoatCombatK(2);
		int k3 = ConfigM.GetBoatCombatK(3);
		float  a  = k1 * lb + k2 * lh + k3 * ls;
		float b = k1 * fb + k2 * fh + k3 * fs;
		int result=(int)(NdUtil.IDivide(a,b) * 100);
		//UnityEngine.Debug.Log("boatDamate="+result);
		return result;
	}

	/// <summary>
	/// 怒气累计（策划文档：战斗系统——怒气累计说明）
	/// </summary>
	/// <returns>最后的怒气值</returns>
	/// <param name="way">怒气累计的方式</param>
	/// <param name="currentAngerValue">当前怒气值</param>
	private static int CACL_SOLDIER_ANGER(AngerAddWay way ,int currentAngerValue,int damageHp,int fullHp)
	{
		int result = currentAngerValue;

		switch(way)
		{
			case AngerAddWay.Attack:
				result+= ConfigM.GetAngerK(2);
			break;

			case AngerAddWay.BeAttacked:
            result += damageHp * ConfigM.GetAngerK(3) / fullHp;
			break;

			case AngerAddWay.KillSoldier:
				result+= ConfigM.GetAngerK(4);
			break;

			case AngerAddWay.KillHero:
				result+= ConfigM.GetAngerK(5);
			break;
			case AngerAddWay.KillBuild:
			result+= ConfigM.GetAngerK(5);
			break;

			default:
			App.log.To("CalcFunc.cs" ,"CACL_SOLDIER_ANGER Error AngerAddWay");
			break;
		}

		return result;
	}

	//d_hp表示玩家当前血量；
	//l_skillhp1表示技能状态增加的血量值；
	//l_skillhp2表示技能状态增加的血量百分比；
	//l_skillstrength表示技能状态增加的力量值；
	private static int CACL_HP_DELTA(int d_hp ,int l_skillhp1,float l_skillhp2,int l_skillstrength)
	{
		int k = ConfigM.GetAttributeK(0);
		float Lhp=(d_hp + l_skillhp1 + l_skillstrength * k*0.01f) * (1 + l_skillhp2);
		return (int)Lhp;
	}

	//d_phydamge表示玩家当前物理伤害；
	//l_skillphydamage1表示技能状态增加的物理伤害值；
	//l_skillphydamage2表示技能状态增加的物理伤害百分比；
	private static int CACL_PHYDAMAGE_DELTA(int d_phydamage ,int l_skillphydamage1,float l_skillphydamage2)
	{
		float Lphydamage = (d_phydamage + l_skillphydamage1) * (1 + l_skillphydamage2);
		return (int)Lphydamage;
	}
	//d_phyattack表示玩家当前物理攻击；
	//soldier_type表示炮弹兵类型；
	//l_skillstrength表示技能状态增加的力量值；
	//l_skillagility表示技能状态增加的敏捷指；
	//l_skillintelligence表示技能状态增加的智力值；
	//l_skillphyattack1表示技能状态增加的物理攻击值；
	//l_skillphyattack2表示技能状态增加的物理攻击力百分比；
	private static int CACL_PHYATTACK_DELTA(int d_phyattack ,int soldier_type,int l_skillstrength,int l_skillagility,int l_skillintelligence, int l_skillphyattack1,float l_skillphyattack2)
	{
		int k1 = ConfigM.GetAttributeK(1);
		float Lphyattack = 0;
		if(soldier_type == 0)
			Lphyattack=(d_phyattack + l_skillphyattack1 + l_skillstrength + l_skillagility *k1/100f) * (1 + l_skillphyattack2);
		else if( soldier_type == 1)
		   Lphyattack = ( d_phyattack + l_skillphyattack1 +l_skillagility + l_skillagility *k1/100f) * (1 + l_skillphyattack2);
		else
		   Lphyattack = ( d_phyattack + l_skillphyattack1 +l_skillintelligence + l_skillagility *k1/100f) * (1 + l_skillphyattack2);

		return (int)Lphyattack;
	}

	//d_phydefend表示玩家当前物理防御
	//l_skillstrength表示技能状态增加的力量值；
	//l_skillagility表示技能状态增加的敏捷指；
	//l_skilldefend1表示技能状态增加的物理防御值；
	//l_skilldefend2表示技能状态增加的物理防御百分比；
	private static int CACL_PHYDEFEND_DELTA(int d_phydefend ,int l_skillstrength,int l_skillagility,int l_skilldefend1,float l_skilldefend2)
	{

		int k2 = ConfigM.GetAttributeK(2);
		int k3 = ConfigM.GetAttributeK(3);

		float lphydefend = (d_phydefend + l_skilldefend1 + l_skillstrength/ (k2 / 100f) + l_skillagility/ (k3 / 100f))*(1+l_skilldefend2);
		return (int)lphydefend;
	}
	//d_magicdamage表示玩家当前魔法伤害；
	//l_skillmagicdamage1表示技能状态增加的魔法伤害值；
	//l_skillmagicdamage2表示技能状态增加的魔法伤害；
	private static int CACL_MAGICDAMAGE_DELTA(int d_magicdamage ,int l_skillmagicdamage1,float l_skillmagicdamage2)
	{
		
		float Lmagicdamage = (d_magicdamage + l_skillmagicdamage1)*(1 +l_skillmagicdamage2);
		return (int)Lmagicdamage;
	}
	//d_magicattack表示玩家当前魔法攻击；
	//l_skillmagicattack1表示技能状态增加的魔法攻击值；
	//l_skillmagicattack2表示技能状态增加的魔法攻击百分比；
	//l_skillintelligence表示技能状态增加的智力值;
	private static int CACL_MAGICATTACK_DELTA(int d_magicattack ,int l_skillmagicattack1,float l_skillmagicattack2,int l_skillintelligence)
	{
		int k4 = ConfigM.GetAttributeK(4);
		float Lmagicattack = (d_magicattack + l_skillmagicattack1 + l_skillintelligence*k4/100f)*(1 + l_skillmagicattack2);
		return (int)Lmagicattack;
	}
	//d_magicdefend表示玩家当前魔法防御；
	//l_skllmagicdefend1表示技能状态增加的魔法防御值；
	//l_skillmagicdefend2表示技能状态增加的魔法防御百分比；
	//l_skillintelligence表示技能状态增加的智力值;
	private static int CACL_MAGICDEFEND_DELTA(int d_magicdefend ,int l_skillintelligence,int l_skllmagicdefend1,float l_skillmagicdefend2)
	{
		
		int k5 = ConfigM.GetAttributeK(5);
		float Lmagicdefend = (d_magicdefend + l_skllmagicdefend1 + l_skillintelligence *k5/100)*(1 +l_skillmagicdefend2);

		return (int)Lmagicdefend;
	}
	//d_phycrit表示玩家当前物理暴击值；
	//l_skillphycrit表示技能状态增加的物理暴击值；
	//l_skillagility表示技能状态增加的敏捷值；
	private static int CACL_PHYCRIT_DELTA(int d_phycrit ,int l_skillagility,int l_skillphycrit)
	{
		
		int k6 = ConfigM.GetAttributeK(6);
		float Lphycrit = d_phycrit +l_skillagility*k6/100 + l_skillphycrit;
		return (int)Lphycrit;
	}
	//d_magiccrit表示玩家当前魔法暴击值；
	//l_skillmagiccrit表示技能状态增加的魔法暴击值；
	//l_skillintelligence表示技能状态增加的智力值；
	private static int CACL_MAGICCRIT_DELTA(int d_magiccrit ,int l_skillintelligence,int l_skillmagiccrit)
	{
		int k7 = ConfigM.GetAttributeK(7);
		float Lmagiccrit = d_magiccrit +l_skillintelligence*k7/100 +l_skillmagiccrit;

		return (int)Lmagiccrit;
	}
    /// <summary>
    /// 战斗力计算
    /// </summary>
    /// <param name="skill01Level"></param>
    /// <param name="skill02Level"></param>
    /// <param name="skill03Level"></param>
    /// <param name="skill04Level"></param>
    /// <param name="flySkill01Level">炮战技能等级</param>
    /// <param name="quality">炮弹兵品质-</param>
    /// <param name="level">炮弹兵等级</param>
    /// <returns></returns>
    private static int CACL_COMBAT_POWER(int skill01Level,int skill02Level,int skill03Level,int skill04Level,
        int flySkill01Level,
        int quality,
        int level)
    {
        int result = (skill01Level + skill02Level + skill03Level + skill04Level + flySkill01Level) * 4 +
            (6 + (quality - 1) * 3) * level;
        return result;
    }
	/// <summary>
	///	 HP回复量计算
	/// </summary>
	/// magicattack 魔法攻击力
	/// power3
	/// power4
	/// count 治疗技能回复波数
	/// addition 治疗效果提升系数
	private static int CACL_GENGEN_HP(int magicattack,int power3,int power4,int count,int addition)
	{
		int result = (int)((magicattack * power3 * 0.001f + power4 * count) * (1 + addition *0.01f) / count);
		return result;
	}
	/// <summary>
	///	 计算吸血回复生命值
	/// </summary>
	/// attrVampire 吸血属性
	/// damage 伤害值
	private static int CACL_VAMPIRE_HP(int attrVampire, int damage)
	{
		int result = (int)(attrVampire * 0.01f * damage);
		return result;
	}
	/// <summary>
	///	 计算具有属性的技能的攻击伤害值
	/// </summary>
	/// damage 伤害值
	/// resistance 抗性
	/// reduction 减免
	private static int CALC_ATTRIBUTABLE_SKILL_DAMAGE(int damage, int attack, int resistance, int reduction)
	{
		int result = (int)((float)(damage + attack) * (1f - ((float)resistance * 0.001f))) - reduction;
		if (result < 0)
			result = 0;
		return result;
	}
	/// <summary>
	///	 计算等级差伤害
	/// </summary>
	/// damage 伤害值
	/// factorLevel 等级差因子
	/// factorStar 星级差因子
	private static int CALC_DISTAIN_DAMAGE(int damage, float factorLevel, float factorStar)
	{
		int result = (int)((float)damage * (1 - factorLevel - factorStar));
		return result;
	}
}
