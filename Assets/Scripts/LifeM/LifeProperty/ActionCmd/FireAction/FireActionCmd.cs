#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#define UNITY_EDITOR_LOG
#endif
using UnityEngine;
using System.Collections;


/// <summary>
/// 炮战Action 基类
/// </summary>
/// <author>zhulin</author>
/// 

public delegate void EventCallBack(FlyCollisionInfo Info);
public class FireActionCmd : GridActionCmd {
	
	protected FlyCollisionInfo m_CollisionInfo = null;
	protected Vector3 m_FlyDir;
	protected float m_FlySpeed = 10;
	protected float m_PauseFlyTime = 0.2f;
	protected float m_PauseDuration = 0.2f;
	protected bool  m_Pause = false;
	protected EventCallBack m_BornEvent = null;
	protected DoneCallBack m_FallEndEvent = null;

	/// <summary>
	/// 炮战构造函数
	/// </summary>
	/// <param name="Parent">炮战角色</param>
	/// <param name="FlyDir">发生炮战前的飞行方向</param>
	/// <param name="FlySpeed">发生炮战前的飞行速度</param>
	public FireActionCmd(Life Parent ,Vector3 FlyDir,float FlySpeed, bool enableTrail)
	{
		SetTarget(Parent);
		m_FlyDir = FlyDir;
		m_FlySpeed = FlySpeed;
		EnableTrail(enableTrail);
	}

	/// <summary>
	/// 设置炮战信息
	/// </summary>
	/// <param name="FlyDir">发生炮战前的飞行方向</param>
	/// <param name="FlySpeed">发生炮战前的飞行速度</param>
	public virtual void SetCollisionInfo(FlyCollisionInfo Info,EventCallBack BornEvent ,DoneCallBack FallEndEvent)
	{
		#if UNITY_EDITOR_LOG
		FileLog.write(m_LifePrent.SceneID,  "SetCollisionInfo" );
		#endif
		if(Info == null) return ;
		m_CollisionInfo = Info;
		m_BornEvent = BornEvent;
		m_FallEndEvent = FallEndEvent;
		#if UNITY_EDITOR_LOG
		FileLog.write(m_LifePrent.SceneID, m_CollisionInfo.flyCollisionAction + "," + m_CollisionInfo.FlyfallSTandGridPos);
		#endif
		if(m_CollisionInfo.flyCollisionAction == FlyCollisionAction.FlyDirectional)
		{
			RolePlayAnimation(AnimatorState.Fly00000);
		}
		else if(m_CollisionInfo.flyCollisionAction == FlyCollisionAction.PauseContinueFlyDirectional)
		{
			PauseContinueFlight(m_CollisionInfo.bVertical);
		}
		else if(m_CollisionInfo.flyCollisionAction == FlyCollisionAction.DropInBoat)
		{
            Info.bVertical = false;
			ForceRoleFallDir( Info.droppos );
		}
		else if(m_CollisionInfo.flyCollisionAction == FlyCollisionAction.DropOutBoat)
		{

			HitFall(1.0f,m_CollisionInfo.bLeft, Info.droppos);
		}
		else if(m_CollisionInfo.flyCollisionAction == FlyCollisionAction.FlyFallStand)
		{
			FlyReboundAction(Info.bVertical);
			if(m_BornEvent != null)
			{
				m_BornEvent(m_CollisionInfo);
			}
		}
	}
	
	
	public override void Update()
	{
		base.Update();
		if(m_Pause == true)
		{
			m_PauseFlyTime = m_PauseFlyTime + Time.deltaTime;
			if(m_PauseFlyTime >= m_PauseDuration)
			{
				m_Pause = false;
			}
		}
		else
		{
			MoveAction(AnimatorState.Fly00000, m_FlyDir * m_FlySpeed * Time.deltaTime, ActionMode.Delta);
		}

	}


	
	public override bool IsDone()
	{
		if(base.IsDone())
		{
			return true;
		}
		return false;
	}


	/// <summary>
	/// 强制角色掉落
	/// </summary>
	public void ForceRoleFallDir( Vector3 pos )
	{
        Transform t = m_Skin.tRoot;
		t.position += pos;
		//t.localRotation = new Quaternion (0, 0, 0,1);
		m_FlyDir = Vector3.down;
		m_Duration = 5f;
	}

	/// <summary>
	/// 撞击出生前置动作
	/// </summary>
	public virtual void FlyReboundAction(bool bVertical)
	{
      
	}

    /// <summary>
    /// 撞击 左右 船板 掉落
    /// </summary>
    /// <param name="fallTime"></param>
    /// <param name="isLeft">是否是左侧甲板</param>
	public void HitFall(float fallTime, bool isLeft, Vector3 droppos)
	{
		Transform t = m_Skin.tRoot;
		t.position += droppos;
		Vector3 pos = t.localPosition;
		pos.y += -10;
        if (isLeft)
		{
            pos.x -= 5;
            t.Rotate(0, 0, -90);
		}
		else
		{
            pos.x += 8;
            t.Rotate(0, 0, 90);
		}
		TweenPosition tp= TweenPosition.Begin (t.gameObject, fallTime, pos,false);
		tp.AddOnFinished(FallEnd);
		t.localRotation = new Quaternion (0, 0, 0,1);
        SoundPlay.Play("drop_sea", false, false);
	}

	/// <summary>
	/// 掉落结束后调用
	/// </summary>
	void FallEnd()
	{
		if(m_FallEndEvent !=null)
		{
			m_FallEndEvent();
		}
	}
	/// <summary>
	/// 暂停飞行
	/// </summary>
	public void PauseFly(float pauseDuration)
	{
		m_PauseDuration = pauseDuration;
		m_PauseFlyTime = 0.0f;
		m_Pause = true;

	}

	/// <summary>
	/// 撞传板暂停飞行
	/// </summary>
	public virtual void PauseContinueFlight(bool bVertical)
	{
		PauseFly(0.2f);
	}

	public void SetCollEffect(bool bVertical)
	{
		Vector3 pos = m_Skin.tRoot.position;
		if (bVertical)
		{
			pos += new Vector3(0, 0.2f, 0);
		}
		else
		{
			pos += new Vector3(0.8f, -0.5f, 0);
		}
		GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "1051081" ,pos ,BattleEnvironmentM.GetLifeMBornNode(true));
		if(gae != null)
		{
			GameObjectActionEffectInit effectinit = new GameObjectActionEffectInit();
			effectinit.SetRotation(new Vector3(0,0,90));
			gae.AddAction(effectinit);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(0.6f);
			gae.AddAction(ndEffect);
		}
	}
	public FlyDir GetDir()
	{
		Vector3 dir = Vector3.zero;
			dir = m_FlyDir;

		if (dir.x > 0 && dir.y >0)
			return FlyDir.RightTop;
		if (dir.x < 0 && dir.y >0)
			return FlyDir.LeftTop;
		if (dir.x < 0 && dir.y <0)
			return FlyDir.LeftBottom;
		if (dir.x > 0 && dir.y <0)
			return FlyDir.RightBottom;
		if (dir.x == 0 && dir.y > 0)
			return FlyDir.Top;
		if (dir.x == 0 && dir.y < 0)
			return FlyDir.Bottom;
		if (dir.y == 0 && dir.x > 0)
			return FlyDir.Right;
		if (dir.y == 0 && dir.x < 0)
			return FlyDir.Left;
		
		return FlyDir.none;
		
	}

}

public class FireActionCmdFactory
{
	/// <summary>
	/// 创建FireActionCmd
	/// </summary>
    public static FireActionCmd Create(Life Parent, Vector3 FlyDir, float FlySpeed, bool isDolphineFly)
	{
	
		int nAttrType = Parent.m_Attr.AttrType;

		if(nAttrType == 103002)
		{
			return new FireActionCmd103002(Parent,FlyDir,FlySpeed);
		}

		if(nAttrType == 100001|| nAttrType == 100002 ||nAttrType == 100004 
		   || nAttrType == 101003
		   || nAttrType == 102002 || nAttrType == 102005
		   || nAttrType == 200000 || nAttrType == 200001 || nAttrType == 200002 || nAttrType == 200003 || nAttrType == 200004 
		   || nAttrType == 200005 || nAttrType == 200006 || nAttrType == 200007 || nAttrType == 200008 || nAttrType == 200009
		   || nAttrType == 102003 || nAttrType == 102004 || nAttrType == 103001 || nAttrType == 103002
		   //|| nAttrType == 103003
			)
			return new FireActionCmd100001(Parent,FlyDir,FlySpeed, false);
		else if(nAttrType == 100003)
            return new FireActionCmd100003(Parent, FlyDir, FlySpeed, isDolphineFly);
		else if(nAttrType == 101001)
			return new FireActionCmd101001(Parent,FlyDir,FlySpeed);
		else if(nAttrType == 102001)
			return new FireActionCmd102001(Parent,FlyDir,FlySpeed);
		else if (nAttrType == 103003 || nAttrType == 101004)
			return new FireActionCmd100001(Parent, FlyDir, FlySpeed, true);
		else 
		{
			Debug.LogError("请创建对应的炮战Action");
			return null;
		}
	}
}
