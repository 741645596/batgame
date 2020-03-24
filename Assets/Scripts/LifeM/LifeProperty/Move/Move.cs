using UnityEngine;
using System.Collections;

public class Move  {

	protected Role m_Owner = null;
	
	public Int2 RolePos
	{
		get{return m_Owner.MapPos;}
		set{m_Owner.MapPos = value;}
	}

	//当前Action
	public GridActionCmd CurrentAction
	{
		get{return m_Owner.CurrentAction;}
		set{
			if(null!=m_Owner&&null!=m_Owner.CurrentAction)
				m_Owner.CurrentAction.SetDone();
			m_Owner.CurrentAction = value;
		}
	}
	
	public Move(Life lifeOwner )
	{
		if(lifeOwner is Role)
		{
			SetParent(lifeOwner as Role);
		}
	}

	void SetParent(Role lifeOwner)
	{
		m_Owner = lifeOwner;
	}

	/// <summary>
	/// 运动中碰撞处理
	/// </summary>	
	public   virtual void ColliderProc (Collision collision)
	{

	}
	

	public virtual void FixedUpdate ()
	{

	}

	/// <summary>
	/// LateUpdate 调用
	/// </summary>
	public virtual void LateUpdate()
	{

	}

	/// <summary>
	/// Update 调用
	/// </summary>
	public virtual void Update ()
	{

	}

	public virtual void SetBornPos(Int2 BornPos ,int deep)
	{
	}

}

public class MoveAIFactory
{
	/// <summary>
	/// 创建FireActionCmd
	/// </summary>
	public static Move Create(Life lifeOwner , LifeMCore Core )
	{
		if(Core.m_MoveState == MoveState.Fly)
		{
			return new Fly(lifeOwner);
		}
		else if(Core.m_MoveState == MoveState.Walk)
		{
			return new Walk(lifeOwner);
		}
		else return null;
	}
}
