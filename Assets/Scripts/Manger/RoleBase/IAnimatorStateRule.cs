using UnityEngine;
using System.Collections;

//AnimatorState 枚举为8位数 AABBCCCD
//AA表增动作组  (00-09)组表示基本动作 (90-99)组表示UI动作组
//BB表增对应动作控制器的参数索引AnimatorStateRule.s_szAnimatorParam 
//CCCD为参数值  CCC表示动作编号(900-999) UI动作段     D表示CCC动作分解序号
public enum AnimatorState{
	Empty=-1,

	//弹射相关状态
	Fly00000=010100000,  /// 飞行1
	FlyToAttack00010 = 010100010,/// 飞行转攻击1
	FlyAttack00100 = 010100100, /// 飞行攻击1
	FlyFallStand00200=010100200,    /// 飞行落地起立1
	FlyFallStand00210=010100210,    /// 飞行落地起立1

	Fly01000 = 010101000,	/// 飞行2
	FlyToAttack01010 = 010101010,    /// 飞行转攻击2
	FlyAttack01100 = 010101100,/// 飞行攻击2
	FlyFallStand01200 = 010101200,/// 飞行落地起立2
    FlyClick00300 = 010100300,//飞行点击操作（蹦大点击操作）
    FlyFall00400 = 010100400,//飞行下落动作


	//常态
	Stand=010110000,
	Idle=010110100,
	Walk=010110400,
	Jump1=010110500,
	Jump2=010110501,
	Jump3=010110502,
	Climb=010110600,
	UnderWalk=010118000,
	UnderJump1=010118100,
	UnderJump2=010118101,
	UnderJump3=010118102,
    WinStart = 010110200,//胜利动作开始
    WinLoop = 010110210,//胜利动作循环
	LoseStart = 010110300,//失败动作开始
	LoseLoop = 010110310,//失败动作循环

	GetDown = 010110700,//击退倒下
	GetDown1 = 010110710,//击退倒下
	GetDown2 = 010110720,//击退倒下
	GetUp = 010110800,//击退站起

	Skill01=010180000,
	JumpUp=010180010,
	JumpDown=010180020,
	PreSkill01=010180030,
	Skill04=010180040,
	Attack85000=010185000,
	Attack86000=010186000,
	Attack87000=010187000,
	Attack85100=010185100,
	Attack85200=010185200,
	Attack81000=010181000,
	Attack81010=010181010,
	Attack80100=010180100,
	Attack81100=010181100,
	Attack81200=010181200,
	Attack81300=010181300,
	Attack82000=010182000,
	Attack82001=010182001,
	Attack83000=010183000,
	Attack84000=010184000,
	Hit=010170000,
	Die = 010111000,

	//异常状态
	IceWalk=010150170,//冰冻状态
	KTV=010150240,//KTV状态
	NoSquash=010150260, //不可压扁状态
	dizziness=010150010,//晕眩状态
	NoSquashStart = 010170400,
	NoSquashing = 010170401,
	NoSquashEnd = 010170410,
	//异常状态消除
	Squash=010160220, //压扁恢复状态

	//受击状态
	SquashPeak=010170400,//受压顶起受击状态
	SanNa=010170500,//桑拿受击状态

	
    //UI
    UIAttack = 990190400,
    UIAttackStandby = 990190100,
    UIDie = 990190500,
    UIPick = 990190100,
    UIPickStandby = 990190400,
    UIStandby = 990190000,

	PetAppear = 010190000,
	PetDown = 010190100,

}
public interface IAnimatorStateRule{
	int GetAniGroup();
	string GetAnimatorParam();
	int GetValue();
}


public class AnimatorStateRule: IAnimatorStateRule{
	public static string[] s_szAnimatorParam =  new string[] {"iState", "tHit"};
	public static string[] s_szLayerAnimatorParam =  new string[] {"iLayerState", "tLayeHit"};
	private int m_nAniGroup;
	private int m_nAniParamIndex;
	private int m_nValue;
	private bool m_bLayerState=false;
	public AnimatorStateRule(AnimatorState aniState,bool bLayerState)
	{ 
		int nRule = (int)aniState;
		m_nAniGroup = nRule / 10000000;
		m_nAniParamIndex = nRule / 100000 % 100-1;
		m_nValue = nRule % 100000;
		m_bLayerState = bLayerState;
	}
	public virtual int GetAniGroup()
	{
		return m_nAniGroup;
	}
	public virtual string GetAnimatorParam()
	{
		if (m_nAniParamIndex >= s_szAnimatorParam.Length||m_nAniParamIndex<0) 
		{
			Debug.LogError("AnimatorStateRule error:paramindex overflow!");
			if(m_bLayerState)
				return s_szLayerAnimatorParam [0];
			else
				return s_szAnimatorParam [0];
		}
		if(m_bLayerState)
			return s_szLayerAnimatorParam [m_nAniParamIndex];
		else
			return s_szAnimatorParam [m_nAniParamIndex];
	}
	public virtual int GetValue()
	{
		return m_nValue;
	}

	static public void GetBlendRule(int nRoleID,AnimatorState preState,AnimatorState nextState,ref AnimatorState blendSTate,ref float fBlendTime)
	{
		/*if (nRoleID == 102001)
		{
			if (preState == AnimatorState.Skill01 && nextState != AnimatorState.Skill01)
			{
				blendSTate = AnimatorState.JumpUp;
				fBlendTime = 1.3f;
			}
			else if (preState == AnimatorState.JumpDown)
			{
				blendSTate = AnimatorState.Skill04;
				fBlendTime = 0.6f;
			}
			else if (nextState == AnimatorState.NoSquash)
			{
				blendSTate = AnimatorState.SquashPeak;
				fBlendTime = 0.9f;
			}

		}
		else
		{*/
			blendSTate = AnimatorState.Empty;
			fBlendTime = 0.0f;
		//}
	}


}