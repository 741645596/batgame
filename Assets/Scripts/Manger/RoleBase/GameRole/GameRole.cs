using UnityEngine;
using System.Collections;
/*
public class GameRole : IGameRole {
	private GameObject m_objRoleRoot;
	private GameObject m_objBody;
	private RolePropertyM m_rpm;
	//private string m_strActionGroup;
	private int m_nActionGroup;
	private AnimatorState m_emAniState;
	private int m_nRoleType;
	private string m_strRoleName;
	private Transform m_objParent;
	private float m_PreSpeed = -1f;
	private int m_nRoleID;
	public virtual GameObject GetGameRoleRootObj()
	{
		return m_objRoleRoot;
	}
	public virtual GameObject GetBodyObj(){ return m_objBody;}
	public virtual T GetBodyComponent<T>()where T : UnityEngine.Component
	{
		if (null != m_objBody) 
		{
			return m_objBody.GetComponent<T>();
		} 
	    return default(T);
	}
	public virtual void SetGameRoleObj(GameObject obj){ m_objBody = obj;}
	
	public virtual RolePropertyM GetGamePropertyM(){ return m_rpm;}
	public virtual void SetGameRoleObj(RolePropertyM rpm){ m_rpm = rpm;}
	public GameRole(Transform parent,int roleType,string roleName,AnimatorState aniState)
	{
		m_emAniState = aniState;
		IAnimatorStateRule rule = new AnimatorStateRule(aniState,false);
		m_nActionGroup = rule.GetAniGroup();
		m_nRoleType = roleType;
		m_strRoleName = roleName;
		m_objParent = parent;
		m_objRoleRoot = new GameObject ();
		m_objRoleRoot.transform.parent = parent;
		m_objRoleRoot.transform.localPosition =  new Vector3 (0,0,0);
		m_objRoleRoot.name = roleName;
		m_objBody = loadRole(m_objRoleRoot.transform,m_nRoleType,m_nActionGroup);	
		if (m_objBody != null) 
		{
			if(m_rpm == null)
				m_rpm = U3DUtil.GetComponentInChildren<RolePropertyM>(m_objRoleRoot,true);
		}
		if(m_rpm != null)
			m_rpm.ChangeState(aniState);
	}


	private   GameObject loadRole (Transform parent, int RoleType , int ActionGrop)
	{
		if (parent == null)
			return null;
		string strResName = RoleType.ToString()+"@skin";
		GameObject go = GameObjectLoader.LoadPath("Prefabs/Roles/", strResName, parent);
		return go;
	}

	public virtual int GetRoleType()
	{
		return m_nRoleType;
	}
	public virtual AnimatorState ExcuteCmd(AnimatorState aniState,AnimatorState aniLayerState=AnimatorState.Empty)
	{
		AnimatorState preAniState=m_emAniState;
		AnimatorState realAniState = m_emAniState;
		if (m_emAniState != aniState) 
		{
			m_emAniState = aniState;
			IAnimatorStateRule rule = new AnimatorStateRule(aniState,false);
			int nActionGroup = rule.GetAniGroup();
			if (m_nActionGroup != nActionGroup) 
			{  
				m_nActionGroup=nActionGroup;
				//现在使用一个模型了，不用进行重新加载模型的。
			}
		}
		if (m_rpm != null)
		{
			AnimatorState blendAniState = AnimatorState.Empty;
			float fBendTime=0.0f;
			AnimatorStateRule.GetBlendRule(m_nRoleID,preAniState,m_emAniState,ref blendAniState,ref fBendTime);
			realAniState=m_rpm.ChangeState (m_emAniState, aniLayerState,blendAniState,fBendTime);
		}
		return realAniState;
	}
	public virtual float SetAnimatorSpeed(float s)
	{
		if(m_rpm != null)
		{
			return	m_rpm.SetAnimatorSpeed(s);
		}
		return -1;
	}

	public virtual void SetMirror (int nXMirror=1, int nYMirror=1, int nZMirror=1)
	{
		if (m_objBody != null) 
		{
			Vector3 vMirror = m_objBody.transform.localScale;
			m_objBody.transform.localScale = new Vector3(Mathf.Abs(vMirror.x)*nXMirror,Mathf.Abs(vMirror.y)*nYMirror,Mathf.Abs(vMirror.z)*nZMirror);
		}
	}
	public virtual Transform GetGameRoleRootTransfrom ()
	{
		return m_objRoleRoot.transform;
	}
	public virtual void PauseAnimator()
	{
		float old = SetAnimatorSpeed(0);
		if (old > 0)
			m_PreSpeed = old;
	}
	public virtual void ContiuneAnimator()
	{
		if (m_PreSpeed != -1)
		{
			SetAnimatorSpeed(m_PreSpeed);
			m_PreSpeed = -1;
		}
	}


	
	public virtual  RolePropertyM  GetRolePropertyM ()
	{
		return m_rpm;
	}
	public virtual void SetIRole (int id)
	{
		m_nRoleID = id;
		m_rpm.SetIRole(m_nRoleID);
	}
	public virtual void SetVisable(bool visable)
	{
		m_rpm.SetVisable(visable);
	}
}
*/