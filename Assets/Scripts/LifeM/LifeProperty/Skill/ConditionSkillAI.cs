using UnityEngine;
using System.Collections;

/// <summary>
/// 条件技能AI, 判断条件技能是否可释放
/// </summary>
/// <author>zhulin</author>

public enum SkillCondition{
	Walk           = 1, //走路
	NotAttack      = 2, //不被攻击且不进行攻击
	ActiveSkill    = 3, //放大招
	Attack         = 4, //进入战斗
	Die            = 5, //死亡
	HasTrapInRange = 6, //范围内有陷阱 data0 范围值
	ClickSoldier   = 7, //点击炮弹兵
	NoPet          = 8, //无召唤物或召唤物死亡
	HaveDoor       = 9, //有门
    BigSkillNoPet = 10,//大招无召唤物
	LowHp = 11,//血量低于一定值 data0 低于的血量值
	InTrapRange = 12,//进入压扁陷阱攻击范围
	NoFullHpNoSelf = 13,//除了己方以外血量未满的已方炮弹兵
	SameLayer = 14,//同层有目标
	TrapAttack = 15,//陷阱条件 data0 陷阱ID
	HaveStatus = 16,//当处在某种状态的时候 data0 状态类型
	RemoveStatus = 17,//当移除某个状态的时候，data0 状态类型
	LowHpAndHaveDownLayer = 18,//血量低于一定值且有下层，data0 低于的血量百分比 
	near = 19,//攻击记录范围内
	HitByElectric = 20, //受电系伤害
	LowHPAndDie = 21, //低于多少或死亡
	AttackBuild =22,  //攻击陷阱 data0 data1 陷阱ID
	BoomDie = 23, //延时炸弹结束触发
}

public class ConditionSkillAI  {
	/// <summary>
	/// check 能否释放技能
	/// </summary>
	/// <param name="Attacker">技能发起者</param>
	/// <param name="ConditionSkillInfo">条件技能</param>
	/// <param name="attackgo"></param>
	/// <returns>false:不能释放；true： 能释放</returns>
	public static bool CheckCanRelease(Role Attacker,SoldierSkill ConditionSkillInfo)
	{
		return false;
	}


}
