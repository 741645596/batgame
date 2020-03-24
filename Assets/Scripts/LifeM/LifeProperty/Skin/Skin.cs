using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// 皮肤表现属性
/// </summary>
/// <author>zhulin</author>

public enum HitbyBuilding
{
	HitByBuild1501   = 0 ,  //冰窟
	HitByBuild1502   = 1 ,  //ktv受击
	HitByBuild1503   = 2 ,  //电击受击
	HitByBuild1504   = 3 ,  //桑拿受击
	HitByBuild1505   = 4 ,  //压扁受击
}

public enum HitEffectMode
{
	Normal     = 0,     //普通模式
	CoverBody  = 1,    //隐藏主题模式
}

public delegate void HitEffectHook(Transform EffectNode);

public class StatusEffectInfo{
	public GameObject go;
	public int SpePriority;
	public int sort;
}

public class Skin  {
	/// <summary>
	/// lifem 对象
	/// </summary>
	protected   Life  m_SkinOwner = null;
	/// <summary>
	/// 阵营
	/// </summary>
	protected   bool   m_IsPlayer;
	protected   LifeMCamp m_Camp; 
	/// <summary>
	/// igameRole
	/// </summary>
	/*private   IGameRole  m_iGameRole = null;
	public IGameRole iGameRole
	{
		get{return m_iGameRole;}
		set{m_iGameRole = value;}
	}*/
	protected   MoveState  m_State;
	
	protected Transform m_tRoot = null;
	public Transform tRoot
	{
		get{return m_tRoot;}
	}
	protected Transform m_tbody = null;
	public Transform tBody
	{
		get{return m_tbody;}
	}
	protected HPAciton m_hpa = null;
	/// <summary>
	/// 获取HPAciton组件
	/// </summary>
	public virtual HPAciton MyHPAction()
	{
		return m_hpa;
	}

	public void ShowHP(int Hp ,int FullHp, int demage ,AttackResult result)
	{
		if (MyHPAction() != null && FullHp != 0)
			MyHPAction().ShowHP(Hp,FullHp,demage, result);
	}

	public void ShowBuff(string StatusName)
	{
		if (MyHPAction() != null)
			MyHPAction().ShowBuff(StatusName);
	}

	public void ShowDebuff(string StatusName)
	{
		if (MyHPAction() != null)
			MyHPAction().ShowDebuff(StatusName);
	}

	public virtual void ShowHP(int RoomID,int hp,int nFullHP,int bear)
	{
	}
	public void DestroyHp()
	{
		if (MyHPAction() != null)
		{
			if (MyHPAction() != null)
				MyHPAction().DestroyHP();
		}
	}
	
	
	public void ShowAnger(int deltaanger)
	{
		if (MyHPAction() != null )
			MyHPAction().ShowAnger(deltaanger);
	}
	public void ShowKillAnget(int anger)
	{
		if (MyHPAction() != null )
			MyHPAction().ShowKillAnger(anger);
	}
	protected RolePropertyM m_rpm = null;
	public RolePropertyM ProPerty
	{
		get{return m_rpm;}
		set{m_rpm = value;}
	}
	// 深度间距
	public static float s_DeepDistint = 1.3f;
	//状态特效管理
	protected Dictionary<int,StatusEffectInfo> m_StatusEffect = new Dictionary<int, StatusEffectInfo>();

	/// <summary>
	/// 加载模型
	/// </summary>
	public virtual void SetGameRole(Life life,MoveState State,LifeMCamp Camp,bool IsPlayer)
	{
		if(life == null)  return ;
		m_SkinOwner = life ;
		m_State = State;
		m_IsPlayer = IsPlayer;
		m_Camp = Camp;
	}


	public bool CheckAttrChange()
	{
		if(m_SkinOwner == null 
		   || m_SkinOwner.m_Attr == null
		   ||m_SkinOwner.m_Attr.AttrChange == false) return false;
		return true;
	}

	/// <summary>
	/// 播放动画
	/// </summary>
	public virtual  void PlayAnimation(AnimatorState state,AnimatorState layerstate = AnimatorState.Empty)
	{

	}

	

	/// <summary>
	/// 更新skin 效果
	/// </summary>
	public virtual  float SetAnimatorSpeed(float speed)
	{

		return -1;
	}
	/// <summary>
	/// 更新skin 效果
	/// </summary>
	public virtual void UpdataSkinEffect()
	{
	}

	/// <summary>
	/// 更新设置皮肤
	/// </summary>
	public virtual void ResetSkin()
	{  

	}
    /// <summary>
    ///重置Body 皮肤颜色
    /// </summary>
     public virtual void ResetBodySkin()
    {

    }
    

	/// <summary>
	/// 设置镜像
	/// </summary>
	public virtual  void SetMirror(WalkDir Dir)
	{  

	}
	
	/// <summary>
	/// 旋转
	/// </summary>
	public   void Rotate (Vector3 Angel) 
	{
		if(m_tRoot != null)
			m_tRoot.Rotate(Angel);
	}


	/// <summary>
	/// 缩放
	/// </summary>
	public  void Scale(Vector3 scale)
	{
		if(m_tRoot != null)
			m_tRoot.localScale = scale;
	}
	

	/// <summary>
	/// 位移改变
	/// </summary>
	public  void Move(AnimatorState state,Vector3 Pos,ActionMode Mode)
	{
		PlayAnimation(state);
		
		if(m_tRoot != null)
		{
			if (Mode == ActionMode.Set) m_tRoot.localPosition = Pos;
			else m_tRoot.localPosition += Pos;
		}
	}
    /// <summary>
    /// 位移（世界坐标）改变
    /// </summary>
    public void Move(AnimatorState state, Vector3 Pos, ActionMode Mode,bool isLocal)
    {
        PlayAnimation(state);

        if (m_tRoot != null)
        {
            if (Mode == ActionMode.Set)
                if (isLocal)
                {
                    m_tRoot.localPosition = Pos;
                }
                else
                {
                    m_tRoot.position = Pos;
                }

            else
            {
                if (isLocal)
                {
                    m_tRoot.localPosition += Pos;
                }
                else
                {
                    m_tRoot.position += Pos;
                }
            }
        }
    }

	public  void PlayAction(AnimatorState state,Vector3 pos,int deep, bool isMove = false,AnimatorState layerstate = AnimatorState.Empty)
	{
		PlayAnimation(state,layerstate);
		
		if(m_tRoot != null)
		{	
			if(isMove)
			{
				m_tRoot.localPosition = pos;
			}
			else
			{
				/*pos = m_tRoot.localPosition;
				pos.z = s_DeepDistint * (deep );
				m_tRoot.localPosition = pos;*/
			}
			
		}
	}

	
	public virtual void SetHpAction()
	{
		
	}	




	protected static string GetHitNodeName(HitbyBuilding HitBy)
	{
		if(HitBy == HitbyBuilding.HitByBuild1501) 
			return "HitByBuild1501";
		else if(HitBy == HitbyBuilding.HitByBuild1502) 
			return "HitByBuild1502";
		else if(HitBy == HitbyBuilding.HitByBuild1503) 
			return "HitByBuild1503";
		else if(HitBy == HitbyBuilding.HitByBuild1504) 
			return "HitByBuild1504";
		else if(HitBy == HitbyBuilding.HitByBuild1504) 
			return "HitByBuild1505";
		else return "";
	}
	
	/// <summary>
	/// 启用重力
	/// </summary>
	/// <param name="isEnable"></param>
	public virtual void EnableGravity(bool isEnable)
	{

	}
	
	public virtual void EnableRigidbody(bool isEnable)
	{

	}
    /// <summary>
    /// 播放效果颜色
    /// </summary>
    public virtual void PlayEffectColor(SkinEffectColor effectColor, float duration)
    {

    }


}





