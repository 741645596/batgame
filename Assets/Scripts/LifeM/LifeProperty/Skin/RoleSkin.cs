using UnityEngine;
using DG.Tweening;
/// <summary>
/// 宠物皮肤表现属性
/// </summary>
/// <author>zhulin</author>
/// <Editor>QFord</Editor>
public class RoleSkin : Skin {
	private GameObject m_AngerEffect = null;

    private bool m_bPlayEffectColor = false;
    private float m_fPlayEffectDuration ;
    private float m_bPlayEffectCounter = 0;
	private bool m_bDarkEnv = false;

	//roleProperty 移过来的变量
	private float m_TimeCount;
	private float m_Duration;
	private AnimatorState m_currentstate = AnimatorState.Stand;
	private AnimatorState m_curLayerState = AnimatorState.Empty;
	private AnimatorState m_curBlendState = AnimatorState.Empty;
	private float m_fBlendTime=0.0f;

	//gamerole移过来的变量
	
	/*private GameObject m_objRoleRoot;
	private GameObject m_objBody;
	private RolePropertyM m_rpm;*/
	//private string m_strActionGroup;
	private int m_nActionGroup;
	private AnimatorState m_emAniState;
	private int m_nRoleType;
	private string m_strRoleName;
	private Transform m_objParent;
	private float m_PreSpeed = -1f;
	private int m_nRoleID;
	public void CreateSkin(Transform parent,int roleType,string roleName,AnimatorState aniState,bool isPlayer)
	{
		m_emAniState = aniState;
		IAnimatorStateRule rule = new AnimatorStateRule(aniState,false);
		m_nActionGroup = rule.GetAniGroup();
		m_nRoleType = roleType;
		m_strRoleName = roleName;
		m_objParent = parent;
		GameObject go = new GameObject ();
		m_tRoot = go.transform;
		m_tRoot.parent = parent;
		m_tRoot.localPosition =  new Vector3 (0,0,0);
		m_tRoot.name = roleName;
		m_tbody = loadRole(m_tRoot,m_nRoleType,m_nActionGroup).transform;	
		if (m_tbody != null) 
		{
			if(ProPerty == null)
				ProPerty = U3DUtil.GetComponentInChildren<RolePropertyM>(m_tRoot.gameObject,true);
		}
		ChangeState(aniState);
		SetCampModel(isPlayer);
	}
	private   GameObject loadRole (Transform parent, int RoleType , int ActionGrop)
	{
		if (parent == null)
			return null;
		string strResName = RoleType.ToString()+"@skin";
		GameObject go = GameObjectLoader.LoadPath("Prefabs/Roles/", strResName, parent);
		return go;
	}
	/// <summary>
	/// 加载模型
	/// </summary>
	public override void SetGameRole(Life life,MoveState State,LifeMCamp Camp,bool IsPlayer)
	{
		base.SetGameRole(life,State,Camp,IsPlayer);
		/*iGameRole = igameRole;
		m_tRoot = iGameRole.GetGameRoleRootTransfrom();
		m_tbody = iGameRole.GetBodyObj().transform;
		ProPerty = iGameRole.GetRolePropertyM();*/
		
		m_Duration = 0;
		m_TimeCount = 0;
	}
	public  int GetRoleType()
	{
		return m_nRoleType;
	}
	public  AnimatorState ExcuteCmd(AnimatorState aniState,AnimatorState aniLayerState=AnimatorState.Empty)
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
			realAniState= ChangeState (m_emAniState, aniLayerState,blendAniState,fBendTime);
		}
		return realAniState;
	}

	
	public  void SetMirror (int nXMirror=1, int nYMirror=1, int nZMirror=1)
	{
		if (m_tbody != null) 
		{
			Vector3 vMirror = m_tbody.transform.localScale;
			m_tbody.transform.localScale = new Vector3(Mathf.Abs(vMirror.x)*nXMirror,Mathf.Abs(vMirror.y)*nYMirror,Mathf.Abs(vMirror.z)*nZMirror);
		}
	}
	public  Transform GetGameRoleRootTransfrom ()
	{
		return m_tRoot.transform;
	}
	public  void PauseAnimator()
	{
		float old = SetAnimatorSpeed(0);
		if (old > 0)
			m_PreSpeed = old;
	}
	public  void ContiuneAnimator()
	{
		if (m_PreSpeed != -1)
		{
			SetAnimatorSpeed(m_PreSpeed);
			m_PreSpeed = -1;
		}
	}
	/*public virtual void FadeOut(float duration)
	{
		m_rpm.FadeOut(duration);
	}*/
	
	
	public virtual  RolePropertyM  GetRolePropertyM ()
	{
		return m_rpm;
	}
	public float BlendTime{
		get{ 
			return m_fBlendTime;
		}
	}
	/// <summary>
	/// 更新skin 效果
	/// </summary>
	public override void UpdataSkinEffect()
	{

		if(CheckAttrChange() == false ) return ;
		if(ProPerty == null) return;
		
		if (m_bDarkEnv) 
		{
			SetBodyModelColor("_Color", RoleSkinColor.DarkEntMain);
			SetBodyModelColor("_Emission", RoleSkinColor.DarkEntEmission);
			return ;
		}


        if (UpdateEffectColor())
        {
            return;
        }
		
		if(m_SkinOwner.m_Attr.IcePoint != 0)
		{
			SetBodyModelColor("_Color",RoleSkinColor.IcePointMain);
			SetBodyModelColor("_Emission",RoleSkinColor.IcePointEmission);
		}
		else
		{
			SetBodyModelColor("_Color",RoleSkinColor.Main);
			SetBodyModelColor("_Emission",RoleSkinColor.Emission);
		}

	}

	public override void ResetSkin()
	{  
		if(ProPerty == null)  return ;
		SetIRole(m_SkinOwner.m_Attr.AttrType);
		//if(m_State == MoveState.Walk)
		{
			SetCampModel(m_IsPlayer );
		}
		SetHpAction();
	}

    public override void ResetBodySkin()
    {
		if (ProPerty == null) return;
        
       SetIRole(m_SkinOwner.m_Attr.AttrType);
       if (m_State == MoveState.Walk)
       {
		   if (m_IsPlayer == true)
           {
               EnableColider(ColiderType.Fire, false);
           }
       }
       SetHpAction();
		
		SetBodyModelColor("_Color",RoleSkinColor.Main);
		SetBodyModelColor("_Emission",RoleSkinColor.Emission);
    }

    /// <summary>
    /// 执行特效颜色
    /// </summary>
    /// <returns></returns>
    private bool UpdateEffectColor()
	{
        if (m_bPlayEffectColor)
        {
            m_bPlayEffectCounter += Time.deltaTime;
            if (m_bPlayEffectCounter < m_fPlayEffectDuration)
            {
                return true;
            }
            else
            {
				ResetBodySkin();
                m_bPlayEffectColor = false;
                return false;
            }
        }
        return false;
    }

    private void SetBeHitBlinkColor()
    {
		if (ProPerty != null)
        {
			SetBodyModelColor("_Color", RoleSkinColor.BeHitMain);
			SetBodyModelColor("_Emission", RoleSkinColor.BeHitEmission);
        }
    }
    private void SetAngerColor()
    {
        if (ProPerty != null)
        {
            SetBodyModelColor("_Color", RoleSkinColor.AngerMain);
            SetBodyModelColor("_Emission", RoleSkinColor.AngerEmission); 
        }
    }

    /// <summary>
    /// 播放受击颜色表现
    /// </summary>
    public override void PlayEffectColor(SkinEffectColor effectColor,float duration)
    {
        if (m_bPlayEffectColor)
        {
            return;
        }

        m_bPlayEffectColor = true;
        m_bPlayEffectCounter = 0f;
        m_fPlayEffectDuration = duration;

        switch (effectColor)
        {
            case SkinEffectColor.BeHit:
                SetBeHitBlinkColor();
            break;

            case SkinEffectColor.Anger:
                //SetAngerColor();
            break;
        }

    }

	/// <summary>
	/// 大招变暗
	/// </summary>
	public void SetDark(bool bDark)
	{
		m_bDarkEnv = bDark;
		if (bDark)
		{
			SetBodyModelColor("_Color", RoleSkinColor.DarkEntMain);
			SetBodyModelColor("_Emission", RoleSkinColor.DarkEntEmission);
		}
		else
		{
			SetBodyModelColor("_Color", RoleSkinColor.Main);
			SetBodyModelColor("_Emission", RoleSkinColor.Emission);
		}
	}
	/// <summary>
	/// 添加怒气特效
	/// </summary>
	/// <param name="soldierID"></param>
	private void AddAngerEffect()
	{
        PlayEffectColor(SkinEffectColor.Anger, 0.04f);
		if(m_AngerEffect != null) return;
        //ProPerty.h.FlashingOn(Color.white, Color.yellow, 5);
		GameObject go = GameObjectLoader.LoadPath("effect/prefab/", "2000211", m_tRoot);

		//go.transform.position += new Vector3(0, 0.2f, 0);
		GameObject posgo = ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
		Vector3 pos = posgo.transform.position;
		pos.z += 1;
		go.transform.position = pos;
		go.transform.parent = m_tbody;
		if (go != null)
		{
			m_AngerEffect = go;
		}
		else
		{
			Debug.Log("怒气特效未加载失败");
		}
	}

    public void ShowAngerEffect(bool isShow)
    {
        if (m_AngerEffect!=null)
        {
            m_AngerEffect.SetActive(isShow);
        }
    }
	
	/// <summary>
	/// 销毁怒气特效
	/// </summary>
	private void RemoveAngerEffect()
	{
		if(m_AngerEffect == null) return;
        ResetBodySkin();
        //ProPerty.h.FlashingOff();
		GameObject.Destroy(m_AngerEffect);
	}
	
	public override HPAciton MyHPAction()
	{
		if (m_SkinOwner!=null&&m_SkinOwner.isDead) 
		{
			if(m_hpa!=null)
				m_hpa.DestroyHP();
			m_hpa = null;
		} 
		else 
		{
			if(m_hpa==null)
			{
				GameObject posgo = ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectTopPos);
				GameObject objHP =  GameObjectLoader.LoadPath ("Prefabs/Roles/","RoleHPSlider",m_tRoot);
				objHP.transform.position = posgo.transform.position;
				m_hpa = objHP.GetComponent<HPAciton>();
			}
		}
		return m_hpa;
	}
	public override void SetHpAction()
	{
		if (MyHPAction() != null)  
		{
			m_hpa.SetPlayer(m_IsPlayer);
		}
	}

	/// <summary>
	/// 启用重力
	/// </summary>
	/// <param name="isEnable"></param>
	public override void EnableGravity(bool isEnable)
	{
		Rigidbody r = m_tbody.GetComponent<Rigidbody> ();
		if (r) {
			if(isEnable)
			{
				r.useGravity = isEnable;
				r.constraints = RigidbodyConstraints.FreezeRotation;
			}
			else
			{
				r.useGravity = isEnable;
				r.constraints = RigidbodyConstraints.FreezeAll;
			}
		}
	}
	
	public override void EnableRigidbody(bool isEnable)
	{
		if(isEnable);
			//EnableColider (ColiderType.Click,true);
		else
			EnableColider (ColiderType.ALL,false);
		if (isEnable) 
		{
			Rigidbody rigid = ProPerty.gameObject.GetComponent<Rigidbody> ();
			if (null==rigid) 
			{
				rigid = ProPerty.gameObject.AddComponent<Rigidbody>();
				rigid.useGravity = isEnable;
				rigid.constraints = RigidbodyConstraints.FreezeAll;
			}
		} 
		else 
		{
			Rigidbody rigid = ProPerty.gameObject.GetComponent<Rigidbody> ();
			if (rigid) 
			{
				GameObject.Destroy (rigid);
			}
		}
	}
	
	public void SetIRole(int id)
	{
		foreach(GameObject go in ProPerty.roleProperties)
		{
			if (go==null)
			{
				continue;
			}
			Animator ani = go.GetComponent<Animator>();
			if (ani!=null)
			{
				ani.SetInteger("iRole", id);
			}
		}
	}
	/// <summary>
	/// 显示或隐藏左手下的物件（目前用与隐藏 小蹦蹦）
	/// </summary>
	public void ShowLeftHand(bool isShow)
	{
		GameObject go = ProPerty.HelpPoint.GetVauleByKey(HelpPointName.LeftHandPos);
		if (go)
		{
			go.SetActive(isShow);
		}
	}
	public AnimatorState ChangeState(AnimatorState state,AnimatorState aniLayerState=AnimatorState.Empty,AnimatorState aniBlendState=AnimatorState.Empty,float fBlendTime=0.0f)
	{
		bool bChange = false;
		if (m_currentstate == state&&m_curLayerState==state&&m_fBlendTime<=0.0f)
			return m_currentstate;
		if (m_currentstate != state) 
		{
			m_currentstate = state;
			m_fBlendTime = fBlendTime;
			m_curBlendState = aniBlendState;
			bChange=true;
		}
		if (m_curLayerState != aniLayerState) 
		{
			m_curLayerState = aniLayerState;
			bChange=true;
		}
		if (m_fBlendTime>0.0f) 
		{
			m_fBlendTime -=Time.deltaTime; 
			IAnimatorStateRule rule;
			if (m_curBlendState != AnimatorState.Empty) 
			{
				rule = new AnimatorStateRule(m_curBlendState,false);
				SetAnimationValue (rule.GetAnimatorParam(),rule.GetValue());
			}
			if (m_curLayerState != AnimatorState.Empty) 
			{
				rule = new AnimatorStateRule(m_curLayerState,true);
				SetAnimationValue (rule.GetAnimatorParam(),rule.GetValue());
			}
			return m_curBlendState;
		}
		else{
			if(bChange||m_curBlendState!=AnimatorState.Empty)
			{
				IAnimatorStateRule rule;
				if (m_currentstate != AnimatorState.Empty) {
					rule = new AnimatorStateRule(m_currentstate,false);
					SetAnimationValue (rule.GetAnimatorParam(),rule.GetValue());
				}
				if (m_curLayerState != AnimatorState.Empty) 
				{
					rule = new AnimatorStateRule(m_curLayerState,true);
					SetAnimationValue (rule.GetAnimatorParam(),rule.GetValue());
				}
			}
			return m_currentstate;
		}
	}
	
	/// <summary>
	/// 获取当前动画时长
	/// </summary>
	/// <returns></returns>
	public float GetAniLength()
	{
		foreach (GameObject go in ProPerty.roleProperties)
		{
			Animator ani = go.GetComponent<Animator>();
			if (ani)
			{
				AnimatorStateInfo aniInfo = ani.GetCurrentAnimatorStateInfo(0);
				return aniInfo.length;
			}
		}
		return 0f;
	}
	/// <summary>
	/// 禁用动画.
	/// </summary>
	/// <param name="enable">If set to <c>true</c> enable.</param>
	public void SetAnimatorEnable(bool enable)
	{
		foreach (GameObject go in ProPerty.roleProperties)
		{
			Animator ani = go.GetComponent<Animator>();
			if (ani)
			{
				ani.enabled = enable;
			}
		}
	}
	void SetAnimationValue(string key, int nValue)
	{
		if (key.Length < 2)
			return;
		
		if (key.StartsWith ("b")) 
		{
			bool bValue=false;
			if(nValue!=0)
				bValue = true;
			SetBool(key,bValue);
		} 
		else if (key.StartsWith ("i")) 
		{
			SetInt(key,nValue);
			SetTrigger("tTriggerParam");
		} 
		else if (key.StartsWith ("t")) 
		{
			SetTrigger(key);
		} 
		else 
		{
			#if UNITY_EDITOR
			Debug.Log("无效接口变量");
			#endif
		}
	}
	
	
	void SetFloat(string name,float value)
	{
		foreach(GameObject go in ProPerty.roleProperties)
		{
			Animator ani = go.GetComponent<Animator>();
			ani.SetFloat(name,value);
			
		}
	}
	void SetBool(string name,bool bvalue)
	{
		foreach(GameObject go in ProPerty.roleProperties)
		{
			Animator ani = go.GetComponent<Animator>();
			ani.SetBool(name,bvalue);
		}
	}
	void SetInt(string name,int bvalue)
	{
		foreach(GameObject go in ProPerty.roleProperties)
		{
			Animator ani = go.GetComponent<Animator>();
			ani.SetInteger(name,bvalue);
			
		}
	}
	void SetTrigger(string name)
	{
		foreach(GameObject go in ProPerty.roleProperties)
		{
			Animator ani = go.GetComponent<Animator>();
			ani.SetTrigger(name);
			
		}
	}
	
	public   void OnCollisionEnter (Collision collision)
	{
		//由炮战阶段下发炮战碰撞信息
		if(ProPerty.m_roleColider.IsFireColider(collision.contacts[0].thisCollider))
		{
			if (ProPerty.transform.parent == null)
			{
				#if UNITY_EDITOR_LOG
				NGUIUtil.DebugLog("不存在父节点","red");
				#endif
				return;
			}
			Role R = ProPerty.transform.GetComponent<LifeProperty>().GetLife() as Role;
			if (R != null) 
			{
				R.ColliderProc(collision);
			}
			Pet p = ProPerty.transform.GetComponent<LifeProperty>().GetLife() as Pet;
			if (p != null)
			{
				p.ColliderProc(collision);
			}
		}
	}
	
	
	public   void OnMouseDown()
	{
		StageClickInfo Click = ProPerty.transform.parent.GetComponent<StageClickInfo>();
		if (Click != null) 
		{
			Click.ClickDown(Input.mousePosition);
		}
	}
	
	public   void OnMouseUp()
	{
		StageClickInfo Click = ProPerty.transform.parent.GetComponent<StageClickInfo>();
		if (Click != null) 
		{
			Click.ClickUp(Input.mousePosition);
		}
	}
	
	
	public override float SetAnimatorSpeed(float s)
	{
		float old  = 0;
		foreach(GameObject go in ProPerty.roleProperties)
		{
			Animator ani = go.GetComponent<Animator>();
			old = ani.speed;
			ani.speed = s;
		}
		
		return old;
	}
	
	/*public void FadeOut(float duration)
	{
		m_Duration = duration;
		m_TimeCount = m_Duration;
	}*/
	
	public void StealthMode(bool state)
	{
		
		int nCount = ProPerty.m_StealthRenderer.Count;
		for (int nCnt=0; nCnt<nCount; nCnt++) 
		{
			SkinnedMeshRenderer ren = ProPerty.m_StealthRenderer[nCnt];
			if (ren != null)
			{
				if (state)
				{
					
					Material  m =  Resources.Load("Materials/Stealth") as Material;
					ren.material = m;
					//Debug.Log(ren.material + "," + m);
					Resources.UnloadAsset(m);
				}
				else
					ren.material = null;
			}
		}
	}
	public void SetCampModel(bool   IsPlayer /*,LifeMCamp Camp*/)
	{
		if(IsPlayer == false)
		{
			int nCount = ProPerty.m_modelCampSkinMesh.Count;
			for (int nCnt=0; nCnt<nCount; nCnt++) 
			{
				SkinnedMeshRenderer render = ProPerty.m_modelCampSkinMesh[nCnt];
				CampTectureM texM = render.gameObject.GetComponent<CampTectureM>();
				if(null!=texM&&null!=render&&null!=render.materials)
				{
					int nMatCount = render.materials.Length;
					for (int nMatCnt=0; nMatCnt<nMatCount; nMatCnt++) 
					{
						Material mat = render.materials[nMatCnt];
						if(null!=mat)
							mat.mainTexture = texM.GetDefense();
					}
				}
			}
		}
		else if(IsPlayer == true)
		{
			int nCount = ProPerty.m_modelCampSkinMesh.Count;
			for (int nCnt=0; nCnt<nCount; nCnt++) 
			{
				SkinnedMeshRenderer render = ProPerty.m_modelCampSkinMesh[nCnt];
				CampTectureM texM = render.gameObject.GetComponent<CampTectureM>();
				if(null!=texM&&null!=render&&null!=render.materials)
				{
					int nMatCount = render.materials.Length;
					for (int nMatCnt=0; nMatCnt<nMatCount; nMatCnt++) 
					{
						Material mat = render.materials[nMatCnt];
						if(null!=mat)
							mat.mainTexture = texM.GetAttack();
					}
				}
			}
		}
		/*if(Camp == LifeMCamp.DEFENSE)
		{
			if(null!= ProPerty.m_roleColider)
			{
				EnableRigidbody(false);
				ProPerty.m_roleColider.EnableColider(ColiderType.Fire,false);
			}
		}*/
	}
	public void SetBodyModelColor(string strPropertyName,Color clr)
	{
		int nCount = ProPerty.m_modelSkinMesh.Count;
		for (int nCnt=0; nCnt<nCount; nCnt++) 
		{
			SkinnedMeshRenderer render = ProPerty.m_modelSkinMesh[nCnt];
			if(null!=render&&null!=render.materials)
			{
				int nMatCount = render.materials.Length;
				for (int nMatCnt=0; nMatCnt<nMatCount; nMatCnt++) 
				{
					Material mat = render.materials[nMatCnt];
					if(null!=mat)
						mat.SetColor(strPropertyName,clr);
				}
			}
		}
	}
	
	/*public void EnableRigidbody(bool isEnable)
	{
		if (isEnable) 
		{
			Rigidbody rigid = ProPerty.gameObject.GetComponent<Rigidbody> ();
			if (null==rigid) 
			{
				rigid = ProPerty.gameObject.AddComponent<Rigidbody>();
				rigid.useGravity = isEnable;
				rigid.constraints = RigidbodyConstraints.FreezeAll;
			}
		} 
		else 
		{
			Rigidbody rigid = ProPerty.gameObject.GetComponent<Rigidbody> ();
			if (rigid) 
			{
				GameObject.Destroy (rigid);
			}
		}
	}*/
	public bool SetVisable(bool visable)
	{
		bool old = true;
		int nCount = ProPerty.m_modelSkinMesh.Count;
		for (int nCnt=0; nCnt<nCount; nCnt++) 
		{
			SkinnedMeshRenderer render = ProPerty.m_modelSkinMesh[nCnt];
			if(null!=render&&null!=render.materials)
			{
				old = render.enabled;
				render.enabled = visable;
			}
		}
		nCount = ProPerty.m_modelCampSkinMesh.Count;
		for (int nCnt=0; nCnt<nCount; nCnt++) 
		{
			SkinnedMeshRenderer render = ProPerty.m_modelCampSkinMesh[nCnt];
			if(null!=render&&null!=render.materials)
			{
				render.enabled = visable;
			}
		}
		return old;
	}
	
	/// <summary>
	/// 获取受击对象节点
	/// </summary>
	public Transform FindHitGameObject(string HitByName)
	{
		if(ProPerty.m_HitByBuilding == null || ProPerty.m_HitByBuilding.Count == 0)
			return null;
		for(int i = 0; i < ProPerty.m_HitByBuilding.Count; i++)
		{
			if(ProPerty.m_HitByBuilding[i].name == HitByName)
			{
				return ProPerty.m_HitByBuilding[i];
			}
		}
		return null;
	}
	/// <summary>
	/// 碰撞器管理
	/// </summary>
	public void EnableColider(ColiderType Type,bool bEnable)
	{
		if (BattleEnvironmentM.GetBattleEnvironmentMode()== BattleEnvironmentMode.Edit)
		{
			return;
		}
		
		if(ProPerty.m_roleColider != null)
		{
			ProPerty.m_roleColider.EnableColider(Type ,bEnable);
		}
	}
	
	public void EnableColider(bool bEnable)
	{
		if(ProPerty.m_roleColider != null)
		{
			ProPerty.m_roleColider.EnableCollider(bEnable);
		}
	}
	public void AttachFx(string fx, string attachPoint, Vector3 localOffset, Vector3 localRotation,float duration)
	{
		GameObject goAttached = ProPerty.m_HelpPoint.GetVauleByKey(attachPoint);
		GameObjectActionExcute gae = EffectM.LoadEffect(fx, localOffset, localRotation, goAttached.transform);
		GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(duration);
		gae.AddAction(ndEffect);
	}
	
	public void AttachFxWorldCoord(string fx, string attachPoint, Vector3 offset, Vector3 rotation,float duration)
	{
		GameObject goAttached = ProPerty.m_HelpPoint.GetVauleByKey(attachPoint);
		GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, fx, goAttached.transform.position, BattleEnvironmentM.GetLifeMBornNode(true));
		GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(duration);
		gae.AddAction(ndEffect);
	}
	
	/// <summary>
	/// 开启尾迹
	/// </summary>
	public void EnableTrail(bool isEnable = true)
	{
		if (ProPerty.m_HelpPoint.GetVauleByKey(HelpPointName.FireTrailPos))
		{
			GameObject objHelp = ProPerty.m_HelpPoint.GetVauleByKey(HelpPointName.FireTrailPos);
			objHelp.SetActive(isEnable);
			if (isEnable)
			{
				GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", objHelp.name ,objHelp.transform.position ,objHelp.transform.transform);
			}
			else
			{
				GameObjectActionExcute gae = objHelp.GetComponent<GameObjectActionExcute>();
				if (gae != null)
					GameObject.Destroy(gae.gameObject);
			}
		}
	}
	
	/// <summary>
	/// 挂载角色状态特效
	/// </summary>
	/// <param name="EffectPos">特效挂载位置</param>
	/// <param name="time">特效持续时间</param>
	/// <param name="EffectID">特效ID ，转换成对应的特效名称</param>
	/// <returns>返回搜索列表list</returns>
	public GameObject AddStatusEffect(int EffectPos,float time,int EffectID,WalkDir dir)
	{ 
		if (EffectID == 1101031)//临时
		{
			GameObject posgo = ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectPos);
			Vector3 pos = posgo.transform.position;
			pos.z = -1f;
			string temp ;
			if (dir==WalkDir.WALKLEFT)
			{
				temp = "1101031_0";
			}
			else
			{
				temp = "1101031_1";
			}
			GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", temp ,pos ,posgo.transform);
			if(gae != null)
			{
				GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(8.5f);
				gae.AddAction(ndEffect);
			}
			return null;
			
		}
		if(EffectID == 0) return null;
		if (EffectPos == 1)
		{
			GameObject posgo = ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectTopPos);
			Vector3 pos = posgo.transform.position;
			pos.z = -1f;
			GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", EffectID.ToString() ,pos ,posgo.transform);
			if(gae != null)
			{
				GameObjectActionEffectInit effectinit = new GameObjectActionEffectInit();
				effectinit.SetEffectMirror(dir);
				gae.AddAction(effectinit);
				GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(time);
				gae.AddAction(ndEffect);
				return gae.gameObject;
			}
			return null;
		}
		else if (EffectPos == 2)
		{
			GameObject posgo = ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectPos);
			Vector3 pos = posgo.transform.position;
			pos.z = -1f;
			GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", EffectID.ToString() ,pos ,posgo.transform);
			if(gae != null)
			{
				GameObjectActionEffectInit effectinit = new GameObjectActionEffectInit();
				effectinit.SetEffectMirror(dir);
				gae.AddAction(effectinit);
				GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(time);
				gae.AddAction(ndEffect);
				return gae.gameObject;
			}
			return null;
		}
		else if (EffectPos == 3)
		{
			GameObject posgo = ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
			Vector3 pos = posgo.transform.position;
			pos.z += -2f;
			GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", EffectID.ToString() ,pos ,posgo.transform);
			if(gae != null)
			{
				GameObjectActionEffectInit effectinit = new GameObjectActionEffectInit();
				effectinit.SetEffectMirror(dir);
				gae.AddAction(effectinit);
				GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(time);
				gae.AddAction(ndEffect);
				return gae.gameObject;
			}
			return null;
		}
		else if (EffectPos == 4)
		{
			GameObject posgo = ProPerty.HelpPoint.GetVauleByKey(HelpPointName.LeyePos);
			Vector3 pos = posgo.transform.position;
			GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", EffectID.ToString() ,pos ,posgo.transform);
			if(gae != null)
			{
				GameObjectActionEffectInit effectinit = new GameObjectActionEffectInit();
				effectinit.SetEffectMirror(dir);
				gae.AddAction(effectinit);
				GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(time);
				gae.AddAction(ndEffect);
				return gae.gameObject;
			}
			return null;
		}
		
		return null;
		
	}
	public void TweenPingponeColor(Color color,float duration)
	{
		int nBodyCount = ProPerty.m_modelSkinMesh.Count;
		for (int nBodyCnt=0; nBodyCnt<nBodyCount; nBodyCnt++) 
		{
			GameObject obj = ProPerty.m_modelSkinMesh[nBodyCnt].gameObject;
			if(obj)
			{

			}
			
		}
	}

	/// <summary>
	/// 播放动画
	/// </summary>
	public override  void PlayAnimation(AnimatorState state,AnimatorState layerstate = AnimatorState.Empty)
	{
		ExcuteCmd(state,layerstate);
	}
	
	/// <summary>
	/// 设置镜像
	/// </summary>
	public override  void SetMirror(WalkDir Dir)
	{  
		if (Dir == WalkDir.WALKLEFT) 
		{
			SetMirror(1,1,1);
		} 
		else if (Dir == WalkDir.WALKRIGHT) 
		{
			SetMirror(-1,1,1);
		}
	}
	/// <summary>
	/// 受建筑物攻击表现效果
	/// </summary>
	/// <param name="HitBy">受击类型</param>
	/// <param name="state">受击状态，true 开始， false 结束</param>
	/// <param name="step">受击步骤</param>
	public void HitByBuildingEffect(HitbyBuilding HitBy ,bool state,int step ,HitEffectMode Mode ,HitEffectHook pfun)
	{
		//if(ProPerty == null) ;
		string HitNodeName = GetHitNodeName(HitBy);
		Transform  HitNode = FindHitGameObject(HitNodeName);
		if(HitNode == null)
		{
			Debug.LogError("获取角色受击对象节点失败，请调查角色预制受击节点配置  " + HitBy +  ",  " + Mode + "," + m_SkinOwner);
			return  ;
		}
		
		string name = step.ToString();
		Transform effect = HitNode.Find(name);
		if(effect == null) return  ;
		
		if(Mode == HitEffectMode.CoverBody)
		{
			if(state == true)
			{
				HitNode.parent = tRoot;
				SetVisable(false);
				//tBody.gameObject.SetActive(false);
			}
			else
			{
				SetVisable(true);
				//tBody.gameObject.SetActive(true);
				HitNode.parent = tBody;
			}
		}
		//
		{
			if(state == true)
			{
				effect.gameObject.SetActive(true);
				if(pfun != null)
				{
					pfun(effect);
				}
			}
			else
			{
				effect.gameObject.SetActive(false);
			}
		}
	}

	/// <summary>
	/// 添加状态特效
	/// </summary>
	/// <param name="isEnable"></param>
	public void AddStatusEffect(StatusInfo Info ,WalkDir dir)
	{
		if(Info == null) return ;
		if (m_StatusEffect.ContainsKey(Info.effectid) && m_StatusEffect[Info.effectid] != null)
		{
			GameObjectActionExcute gae = m_StatusEffect[Info.effectid].go.GetComponent<GameObjectActionExcute>();
			GameObjectAction ga = gae.GetCurrentAction();
			if (ga != null && ga is GameObjectActionDelayDestory)
			{
				(ga as GameObjectActionDelayDestory).ResetDuration(Info.time);
			}
			/*td.AddTime();
			if (td != null)
				td.ResetDuration(Info.time);*/
			/*if (Info.effectid == 1101041)
			{
				td = m_StatusEffect[1101031].GetComponent<NdDestroy>();
				td.AddTime();
				if (td != null)
					td.ResetDuration(Info.time);
			}*/
		}
		else
		{
			GameObject go = AddStatusEffect(Info.position,Info.time,Info.effectid,dir);
			if(go != null) 
			{
				StatusEffectInfo info = new StatusEffectInfo();
				info.go = go;
				info.SpePriority = Info.SpePriority;
				m_StatusEffect[Info.effectid] = info;
				SortEffect(Info.effectid,true);
			}
			
			/*if (Info.effectid == 1101041)
			{
				go = ProPerty.AddStatusEffect(3,Info.time,1101031,dir);
				if(go != null) m_StatusEffect[1101031] = go;
			}*/
			
		}
	}
	
	/// <summary>
	/// 移除状态特效
	/// </summary>
	/// <param name="isEnable"></param>
	public void RemoveStatusEffect(StatusInfo Info)
	{
		if(Info == null) return ;
		if (m_StatusEffect.ContainsKey(Info.effectid) && m_StatusEffect[Info.effectid] != null)
		{
			GameObject.Destroy(m_StatusEffect[Info.effectid].go);
			SortEffect(Info.effectid,false);
			m_StatusEffect.Remove(Info.effectid);
			/*if (Info.effectid == 1101041)
			{
				GameObject.Destroy(m_StatusEffect[1101031].go);
				m_StatusEffect.Remove(1101031);
			}*/
		}
	}
	//对特效进行排序
	public void SortEffect(int effectid,bool isinsert)
	{
		if (isinsert)
		{
			int spepriority = m_StatusEffect[effectid].SpePriority;
			int sort = int.MaxValue;
			Vector3 pos = Vector3.zero;
			foreach(int id in m_StatusEffect.Keys)
			{
				if(effectid != id)
				{
					if (spepriority < m_StatusEffect[id].SpePriority)
					{
						if (sort > m_StatusEffect[id].sort)
							sort = m_StatusEffect[id].sort;
						m_StatusEffect[id].sort ++;
						pos = m_StatusEffect[id].go.transform.localPosition;
						pos.z = m_StatusEffect[id].sort * 0.5f;
						m_StatusEffect[id].go.transform.localPosition = pos;
					}
				}
			}
			
			pos = m_StatusEffect[effectid].go.transform.localPosition;
			pos.z = m_StatusEffect[effectid].sort * 0.5f;
			m_StatusEffect[effectid].go.transform.localPosition = pos;
		}
		else
		{
			Vector3 pos = Vector3.zero;
			foreach(int id in m_StatusEffect.Keys)
			{
				int sort = m_StatusEffect[effectid].sort;
				if(effectid != id)
				{
					if (sort < m_StatusEffect[id].sort)
					{
						m_StatusEffect[id].sort --;
						pos = m_StatusEffect[id].go.transform.localPosition;
						pos.z = m_StatusEffect[id].sort * 0.5f;
						m_StatusEffect[id].go.transform.localPosition = pos;
					}
				}
				
			}
		}
	}
	/// <summary>
	/// 移除所有状态特效
	/// </summary>
	/// <param name="isEnable"></param>
	public void RemoveAllStatusEffect()
	{
		foreach(int i in m_StatusEffect.Keys)
		{
			GameObject.Destroy(m_StatusEffect[i].go);
			//m_StatusEffect.Remove(i);
		}
		m_StatusEffect.Clear();
	}
}

/// <summary>
/// 角色皮肤颜色定义
/// </summary>
public class RoleSkinColor 
{
	//body Color
	public static Color Main = new Color(1.0f,1.0f,1.0f);                          //默认Main颜色
	public static Color Emission =  new Color(0.392f,0.392f,0.392f,0);                //默认Emission颜色

	public static Color IcePointMain = new Color(1.0f,1.0f,1);                      //默认冰冻Main颜色
	public static Color IcePointEmission = new Color(0.2f,0.41f,0.82f);        //默认冰冻Emission颜色

    public static Color BeHitMain = new Color(1f, 1f, 1f);
    public static Color BeHitEmission = new Color(1f, 0f, 0f);//受击Emission颜色

    public static Color AngerMain = new Color(1f, 1f, 1f);
    public static Color AngerEmission = new Color(1f, 0.78125f, 0f);//怒气满Emission颜色
	
	public static Color DarkEntMain = Color.gray;                      //黑屏颜色Main颜色
	public static Color DarkEntEmission = new Color(0f,0f,0f);        //黑屏Emission颜色
};

public enum SkinEffectColor
{
    BeHit = 1,//受击
    Anger = 2,//怒气满
}