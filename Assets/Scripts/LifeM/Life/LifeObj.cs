using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LifeObj : SceneObj {

	private Life m_life = null;


	public override void Start () 
	{
		base.Start();
	}
	
	/// <summary>
	/// NDStart
	/// </summary>
	protected  override void NDStart ()
	{
	}
	/// <summary>
	/// Update
	/// </summary>
	public  override void NDUpdate (float deltaTime) 
	{
		if(m_life != null)
		{
			m_life.NDUpdate(deltaTime);
		}
			
	}
	/// <summary>
	/// NDFixedUpdate
	/// </summary>
	public override  void NDFixedUpdate (float deltaTime)
	{
		if(m_life != null)
			m_life.NDFixedUpdate(deltaTime);
	}
	/// <summary>
	/// NDLateUpdate
	/// </summary>
	public  override void NDLateUpdate(float deltaTime) 
	{
		if(m_life != null )
			m_life.NDLateUpdate(deltaTime);
	}

	/// <summary>
	/// 托管life对象
	/// </summary>
	public void SetLife(Life life ,LifeProperty Property )
	{
		m_life = life;
		if(m_life != null)
		{
			m_life.LifeProperty(Property);
			m_life.NDStart();
		}
	}
	
	public void ChangeLife(Life life ,LifeProperty Property)
	{
		m_life = life;
		if(m_life != null)
		{
			m_life.LifeProperty(Property);
		}
	}
	public Life GetLife()
	{
		return m_life ;
	}
}
