#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#define UNITY_EDITOR_LOG
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic;


/// <summary>
/// 角色对象挂接点模块
/// </summary>
/// <author>zhulin</author>

public class HelpPointName{
	public const string EffectPos = "EffectPos";
	public const string FirePos = "FirePos";
	public const string EffectTopPos = "EffectTopPos";
	public const string EffectBottomPos = "EffectBottomPos";
	public const string LeftHandPos = "LeftHandPos";
	public const string NavelPos = "NavelPos";
	public const string PetPos = "PetPos";
	public const string LeyePos = "LeyePos";
	public const string ReyePos = "ReyePos";
	public const string bagPos = "bagPos";
	public const string petFollowPos = "petFollowPos";
	public const string GuoPos = "GuoPos";
	public const string RightHandPos = "RightHandPos";
	public const string Bone1Pos = "Bone1Pos";
	public const string Bone2Pos = "Bone2Pos";
	public const string Bone3Pos = "Bone3Pos";
	public const string Bone4Pos = "Bone4Pos";
	public const string Bone5Pos = "Bone5Pos";
	public const string FireTrailPos = "FireTrailPos";
}
[RequireComponent(typeof(DictionaryGameObject))]
public class RolePropertyM : LifeProperty {

	public List<GameObject> roleProperties = new List<GameObject>();

	public RoleColider m_roleColider;

	public List<SkinnedMeshRenderer> m_modelSkinMesh = new List<SkinnedMeshRenderer>();
	public List<SkinnedMeshRenderer> m_modelCampSkinMesh = new List<SkinnedMeshRenderer>();
	public List<SkinnedMeshRenderer> m_StealthRenderer = new List<SkinnedMeshRenderer>();

	//尾迹
	//public Transform m_Trailing;
	public List<Transform> m_HitByBuilding = new List<Transform>();

    //public Highlighter h;

	void Start () {
		if (m_HelpPoint == null)
		{
			m_HelpPoint = GetComponent<DictionaryGameObject>();
			if (m_HelpPoint != null)
				m_HelpPoint.type = "HelpPoint";
		}
	}
	public RoleColider  MyRoleColider{
		get{ 
			return m_roleColider;
		}
	}
    /// <summary>
    /// 显示或隐藏左手下的物件（目前用与隐藏 小蹦蹦）
                      /// </summary>

	/*void Update(){		
		if (m_TimeCount > 0)
		{
			m_TimeCount -= Time.deltaTime;
			Renderer[] rens = GetComponentsInChildren<Renderer>();
			foreach(Renderer ren in rens)
			{
				foreach(Material m in ren.materials)
				{
					Color c = m.color;
					c.a = m_TimeCount / m_Duration;
					m.color = c;
				}
			}
		}
	}*/

	public   void OnCollisionEnter (Collision collision)
	{
		if (m_roleColider == null || collision.contacts[0].thisCollider == null)
		{
			return;
		}
		if (GetLife()!= null)
		{
			if (GetLife() is Role)
			{
				Role r = GetLife() as Role;
				r.RoleSkinCom.OnCollisionEnter(collision);
			}
			else if (GetLife() is Pet)
			{
				(GetLife() as Pet).PetSkinCom.OnCollisionEnter(collision);
			}
		}
	}
	
	
	void OnMouseDown()
	{
		
		if (GetLife()!= null)
		{			
			if (GetLife() is Role)
			{
				Role r = GetLife() as Role;
				r.RoleSkinCom.OnMouseDown();
			}
			else if (GetLife() is Pet)
			{
				(GetLife() as Pet).PetSkinCom.OnMouseDown();
			}
		}
	}
	
	void OnMouseUp()
	{
		if (GetLife()!= null)
		{	
			if (GetLife() is Role)
			{
				Role r = GetLife() as Role;
				r.RoleSkinCom.OnMouseUp();
			}
			else if (GetLife() is Pet)
			{
				(GetLife() as Pet).PetSkinCom.OnMouseUp();
			}
		}
	}

}
