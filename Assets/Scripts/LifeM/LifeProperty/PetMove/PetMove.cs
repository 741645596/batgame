using UnityEngine;
using System.Collections;

public class PetMove   {
	
	public GridActionCmd m_CurrentAction = null;
	public Pet m_Owner;
	// Use this for initialization
	public virtual void Init (Pet pet) {
		m_Owner = pet;
	}
	
	// Update is called once per frame
	public virtual void FixUpdate () {
		
	}
	// Update is called once per frame
	public virtual void Update () {
	
	}
	
	/// <summary>
	/// 运动中碰撞处理
	/// </summary>	
	public   virtual void ColliderProc (Collision collision)
	{
		
	}
}
