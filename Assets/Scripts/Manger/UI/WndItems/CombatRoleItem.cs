
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Flags]
public enum FIRE_BTN_STATE
{
    FireBtnNone = 0,

    FireBtnSelected = 1<<0,

    FireBtnDied = 1<<1,

    FireBtnFired = 1<<2,

    FireBtnAngerFull = 1<<3,

    FireBtnSkill9001 = 1<<4,//等待使命召唤
}

public class CombatRoleItem : MonoBehaviour
{

	public SoldierScrollItem m_soldierItem
	{
		get
		{
			return MyHead.soldierItem.GetComponent<SoldierScrollItem>();
		}
	}
	public CombatRoleItem_h MyHead
	{
		get 
		{
			return ( GetComponent<CombatRoleItem_h>() );
		}
	}

    [HideInInspector]
    /// <summary>
    /// 正在清空怒气条UI的标识
    /// </summary>
    public bool m_bEmptyAngerBar = false;

    // [HideInInspector]
    private FIRE_BTN_STATE eBtnState = FIRE_BTN_STATE.FireBtnNone;

    //[HideInInspector]
    public int SoldierDataID = -1;

    /// <summary>
    /// 当前Walk的炮弹兵
    /// </summary>
    private Role m_currentRAW = null;
    /// <summary>
    ///  清空怒气条的定时器
    /// </summary>
    private float m_fEmptyAngerBarCounter = 0f;

    private const float m_fLongUnFireMax = 30f;
    private float m_fLongUnFireCounter = 0f;

    public string FireBtnState;
	public bool m_isPlayer = true;

	public float OffsetY = 50f;
	Vector3 mTweenFrom = Vector3.zero;
	Vector3 mTweenTo = Vector3.zero;
	TweenPosition mTweenReadyToFire = null;
	AnimatorState mAnimatorState = AnimatorState.UIStandby;


    void Start()
    {
        if((BattleEnvironmentM.GetBattleEnvironmentMode() == BattleEnvironmentMode.CombatPVE &&
		        StageDC.GetPveMode() == PVEMode.Defense) || !m_isPlayer)
            ShowHpAnger(true);
        else
            ShowHpAnger(false);
    }

    void Update()
    {
        //DoLongUnFire(); //美术要求屏蔽此特效
        FireBtnState = ShowFireBtnState();
        CheckReleaseSkill();
    }

    void CheckReleaseSkill()
    {
        GetRole();
        if (m_currentRAW!=null && IsInFireBtnState(FIRE_BTN_STATE.FireBtnAngerFull))
        {
			bool nottarget = false;
			bool canRelease =  m_currentRAW.CheckReleaseSkill(ref nottarget);
			ShowNoTargetEffect(nottarget);
            if (canRelease == false)
            {
                DestroyAngryUI();
            }
            else
            {
                SpawnAngryUI();
            }
        }
    }


    void DoLongUnFire()
    {
		if (CombatScheduler.State == CSState.End)
        {
            return;
        }

        if (eBtnState != FIRE_BTN_STATE.FireBtnNone)
        {
            ShowUnFireEffect(false);
            return;
        }
        m_fLongUnFireCounter += Time.deltaTime;
        if (m_fLongUnFireCounter>=m_fLongUnFireMax)
        {
            ShowUnFireEffect(true);
            m_fLongUnFireCounter = 0;
        }
    }

    public void SetRoleAlpha(float a)
    {
        if (MyHead.soldierItem != null)
        {
			m_soldierItem.SetRoleAlpha(a);
        }
    }

	public void SetAngerSpriteColor(bool red = false)
	{
		if(red)
		{
			MyHead.angerSprite.spriteName = "zd_pic007";
			MyHead.angerSprite.flip = UIBasicSprite.Flip.Horizontally;
		}
		else
		{
			MyHead.angerSprite.spriteName = "zd_pic009";
			MyHead.angerSprite.flip = UIBasicSprite.Flip.Nothing;

		}
	}
    public void ShowHpAnger(bool isShow)
    {
        if (MyHead.hpSprite)
        {
            MyHead.hpSprite.parent.gameObject.SetActive(isShow);
        }
        if (MyHead.angerSprite)
        {
            MyHead.angerSprite.parent.gameObject.SetActive(isShow);
        }

        if (isShow)
        {
//			MyHead.lblDestructivePower.gameObject.SetActive(false);
//			MyHead.sprDestructivePower.gameObject.SetActive(false);
//            transform.localPosition += new Vector3(0, 30f, 0);
        }
        else
		{
//			MyHead.lblDestructivePower.gameObject.SetActive(true);
//			MyHead.sprDestructivePower.gameObject.SetActive(true);
//            transform.localPosition += new Vector3(0, -30f, 0);
        }

    }

    public void SetSoldierUI(SoldierInfo info)
    {
		if (MyHead.soldierItem)
        {
			m_soldierItem.SetUI(info);
			
//			int nGridHP = ConfigM.GetBuildHPType();
//			SoldierSkill fireskill = info.GetFireSkill();
//			int power =  Mathf.CeilToInt( fireskill.m_power2 *1.0f  / nGridHP);
			MyHead.lblDestructivePower.text = info.m_concussion.ToString();
//			MyHead.sprDestructivePower.cachedTransform.localScale = new Vector3(power == 0?0f:1.0f/power,1,1);
//			MyHead.sprDestructivePower.width =  (int)(fireskill.m_power2 *1.0f*100  / nGridHP);

        }
    }

    /// <summary>
    /// 添加枚举状态
    /// </summary>
    public FIRE_BTN_STATE AddFireBtnState(FIRE_BTN_STATE state)
    {
        eBtnState = eBtnState | state;
        return eBtnState;
    }

    /// <summary>
    ///  删除枚举状态
    /// </summary>
    public FIRE_BTN_STATE DelFireBtnState(FIRE_BTN_STATE state)
    {
        eBtnState = eBtnState & (~state);
        return eBtnState;
    }
    public void ResetFireBtnState()
    {
        eBtnState = FIRE_BTN_STATE.FireBtnNone;
    }

    /// <summary>
    /// 是否处于某个FIRE_BTN_STATE状态
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public bool IsInFireBtnState(FIRE_BTN_STATE state)
    {
        if ((eBtnState & state) != 0)
        {
            return true;
        }
        return false;
    }

    public string ShowFireBtnState()
    {
        string str = "";
        if (IsInFireBtnState(FIRE_BTN_STATE.FireBtnNone))
        {
            str += "None ||";
        }
        if (IsInFireBtnState(FIRE_BTN_STATE.FireBtnDied))
        {
            str += "Died ||";
        }
        if (IsInFireBtnState(FIRE_BTN_STATE.FireBtnFired))
        {
            str += "Fired ||";
        }
        if (IsInFireBtnState(FIRE_BTN_STATE.FireBtnSelected))
        {
            str += "Selected ||";
        }
        if (IsInFireBtnState(FIRE_BTN_STATE.FireBtnAngerFull))
        {
            str += "AngerFull ||";
        }
        if (IsInFireBtnState(FIRE_BTN_STATE.FireBtnSkill9001))
        {
            str += "Skill9001 ||";
        }

        return str;
    }

    /// <summary>
    ///  根据字符串数组设置2DUI
    /// </summary>
    /// <param name="emAniState"></param>
    /// <param name="names"></param>
    void SetStateByArrayName(AnimatorState emAniState, string[] names)
    {
		if (mAnimatorState == emAniState) return;
		mAnimatorState = emAniState;

        if (MyHead.LongUnFireEffect)
        {
            MyHead.LongUnFireEffect.SetActive(false);
        }
        switch (emAniState)
        {
            case AnimatorState.UIStandby:
				CancleFire();
                break;

            case AnimatorState.UIPick:
				ReadyToFire();
                break;

            case AnimatorState.UIAttack:
				Fire();
                break;

            case AnimatorState.UIDie:

                break;
        }
    }

	void ReadyToFire()
	{
		MyHead.SelectEffect.SetActive(true);
		if (mTweenReadyToFire == null)
		{
			mTweenReadyToFire = GetComponent<TweenPosition>();
			mTweenFrom = transform.localPosition;
			mTweenTo = mTweenFrom;
			mTweenTo.y = mTweenFrom.y + OffsetY;
			mTweenReadyToFire.duration = 0.2f;
			mTweenReadyToFire.delay = 0f;
		}
		mTweenReadyToFire.from = transform.localPosition;
		mTweenReadyToFire.to = mTweenTo;
		mTweenReadyToFire.ResetToBeginning();
		mTweenReadyToFire.PlayForward();
	}

	void CancleFire()
	{
		MyHead.SelectEffect.SetActive(false);
		mTweenReadyToFire.from = transform.localPosition;
		mTweenReadyToFire.to = mTweenFrom;
		mTweenReadyToFire.ResetToBeginning();
		mTweenReadyToFire.PlayForward();
	}

	void Fire()
	{
		SetPowerActive(false);
		MyHead.SelectEffect.SetActive(false);
	}

    private void ShowUnFireEffect(bool isShow)
    {
        if (MyHead.LongUnFireEffect)
        {
            MyHead.LongUnFireEffect.SetActive(isShow);
        }
    }

    void Set2DHeadState(int roleType, AnimatorState emAniState)
    {
        const int nameLength = 4;
        string[] names = null;
        names = new string[nameLength] { roleType.ToString() + "UnSelect", roleType.ToString() + "Select", roleType.ToString() + "Combat", roleType.ToString() + "Die" };
        SetStateByArrayName(emAniState, names);
    }
    /// <summary>
    /// 设置3D头像 动画状态
    /// </summary>
    public void Set3DHeadState(AnimatorState emAniState)
    {

		SoldierInfo soldierInfo = CmCarbon.GetSoldierInfo(CmCarbon.GetPlayer2Camp(m_isPlayer),SoldierDataID);//GetPlayerSoldierInfo(SoldierDataID);
        int roleType = soldierInfo.m_modeltype;
        Set2DHeadState(roleType, emAniState);
    }
    /// <summary>
    /// 设置3D头像 待命状态
    /// </summary>
    public void Set3DHeadStandby()
    {
        Set3DHeadState(AnimatorState.UIStandby);
    }
	/// <summary>
	/// 设置3D头像 待命状态
	/// </summary>
	public void Set3DHeadFired()
	{
		Set3DHeadState(AnimatorState.UIAttack);
	}

    /// <summary>
    /// 设置3D头像 选中状态
    /// </summary>
    public void SetReadyToFire()
    {
        Set3DHeadState(AnimatorState.UIPick);
    }
	public void SetPowerActive(bool active)
	{
		MyHead.lblDestructivePower.gameObject.SetActive(active);
	}
    /// <summary>
    /// 设置3D头像 取消选中
    /// </summary>
    public void SetCancelToFire()
    {
        if (IsInFireBtnState(FIRE_BTN_STATE.FireBtnFired))
        {
            return;
        }
        Set3DHeadStandby();
    }
    /// <summary>
    /// 设置3D头像 死亡
    /// </summary>
    public void Set3DHeadDie()
    {
        Set3DHeadState(AnimatorState.UIDie);
    }

    public void SetHp(float HP)
    {
        if (MyHead.hpSprite.fillAmount > HP)//扣血
        {
            MyHead.hpBG.fillAmount = MyHead.hpSprite.fillAmount;
            NGUIUtil.SetActive(MyHead.hpBG.gameObject, true);
            //NGUIUtil.UpdateSpriteValue(head.hpSprite, HP, 0.4f);
            MyHead.hpSprite.fillAmount = HP;
            UpdateHpBG();
        }
        else
        {
            MyHead.hpSprite.fillAmount = HP;
            MyHead.hpBG.fillAmount = HP;
        }       
    }

    private void UpdateHpBG()
    {
        NGUIUtil.UpdateSpriteValue(MyHead.hpBG, MyHead.hpSprite.fillAmount, 0.3f);
        GameObjectActionExcute gae = GameObjectActionExcute.CreateExcute(gameObject);
        GameObjectActionWait wait = new GameObjectActionWait(0.3f, HideHpBG);
        gae.AddAction(wait);
    }
    private void HideHpBG(object o)
    {
        NGUIUtil.SetActive(MyHead.hpBG.gameObject, false);
    }

    public void SetAnger(float Anger)
    {
        NGUIUtil.UpdateSpriteValue(MyHead.angerSprite, Anger, 0.4f);
    }

    /// <summary>
    /// 生成怒气特效
    /// </summary>
    public void SpawnAngryUI()
    {
		if (CombatScheduler.State == CSState.End)
        {
            return;
        }

		Transform t = transform.Find("2000631");
        if (t == null)
        {
            GameObject go = GameObjectLoader.LoadPath("effect/prefab/", "2000631", transform);
			go.AddComponent<SetRenderQueue>();
			go.AddComponent<ParticaleAnimator>();
        }
        else
            NGUIUtil.SetActive(t.gameObject, true);
    }
    /// <summary>
    /// 销毁怒气特效
    /// </summary>
    public void DestroyAngryUI()
	{
		Transform t = transform.Find("2000631");
        if (t)
        {
            //Destroy(t.gameObject);
            NGUIUtil.SetActive(t.gameObject, false);
        }
    }

    public void SetDied(GameObject go)
    {
        if (go)
        {
            if (IsInFireBtnState(FIRE_BTN_STATE.FireBtnAngerFull))
            {
                Coroutine.DispatchService(EmptyAngerBar(), gameObject, null);
            }
            else
            {
                SetAnger(0);
            }
            ShowSKill9001Effect0(false);
            Set3DHeadDie();
        }
    }

    void InitAnger()
    {
        if (MyHead.angerSprite != null)
        {
            MyHead.angerSprite.fillAmount = 0;
        }
    }

    public void ReleaseAnger()
    {
        GetRole();
        if (m_currentRAW != null)
        {
            if (m_currentRAW.ReleaseSkillEffect())
            {
				ShowNoTargetEffect(false);
                DelFireBtnState(FIRE_BTN_STATE.FireBtnAngerFull);
                DelFireBtnState(FIRE_BTN_STATE.FireBtnSelected);
                DestroyAngryUI();
                //CombatScheduler.ResumeCombat();
                Time.timeScale = 1.0f;
			}
		}
        
    }

    /// <summary>
    ///  成功释放技能后的UI操作
    /// </summary>
    /// <param name="soldierID">Soldier I.</param>
    public void ReleaseAngerUI()
    {
        GetRole();
        if (m_currentRAW != null)
        {
            DelFireBtnState(FIRE_BTN_STATE.FireBtnAngerFull);
            DelFireBtnState(FIRE_BTN_STATE.FireBtnSelected);
            DestroyAngryUI();
        }
    }

    /// <summary>
    /// 清空怒气UI
    /// </summary>
    /// <returns></returns>
    IEnumerator EmptyAngerBar()
    {
        m_bEmptyAngerBar = true;
        float emptyAngerBartTime = 0.5f;
        while (m_fEmptyAngerBarCounter < emptyAngerBartTime)
        {
            m_fEmptyAngerBarCounter += Time.deltaTime;
            float angerP = m_fEmptyAngerBarCounter / emptyAngerBartTime;
            float setAnger = 1 - angerP;
            Mathf.Clamp01(setAnger);
            SetAnger(setAnger);
            yield return null;
        }
        m_fEmptyAngerBarCounter = 0f;
        m_bEmptyAngerBar = false;
    }

    /// <summary>
    /// 重置怒气
    /// </summary>
    /// <param name="para"></param>
    /// <param name="list"></param>
    void ResetAnger(object para, object[] list)
    {
        if (!IsInFireBtnState(FIRE_BTN_STATE.FireBtnDied))
        {
            GetRole();
            if (null != m_currentRAW)
            {
				TweenScale ts = TweenScale.Begin(m_currentRAW.m_Property.gameObject, 0.5f, new Vector3(1, 1, 1));
            }
        }
    }

    public void ResetUI()
    {
        eBtnState = FIRE_BTN_STATE.FireBtnNone;
        Set3DHeadStandby();
        DestroyAngryUI();
        SetHp(1);

        SoldierInfo soldierInfo = new SoldierInfo();
		soldierInfo = CmCarbon.GetSoldierInfo(CmCarbon.GetPlayer2Camp(m_isPlayer),SoldierDataID);//.GetPlayerSoldierInfo(SoldierDataID);
        float anger = soldierInfo.m_mp * 1.0f / ConfigM.GetAngerK(1);
        SetAnger(anger);
    }
    /// <summary>
    /// 清除怒气特效、清除角色状态特效、清除长时间未发特效
    /// </summary>
    public void ClearUIEffect()
    {
        DestroyAngryUI();
        NGUIUtil.SetActive(MyHead.StatusEffectParent, false);
        ShowUnFireEffect(false);
    }

    void OnDestroy()
    {
        GetRole();
        
    }

    private void GetRole()
    {
        if (m_currentRAW == null)
        {
            int SceneID = CM.DataID2SceneIDInSoldier(SoldierDataID);
            Life life = CM.GetLifeM(SceneID, LifeMType.SOLDIER);
            if (life == null) return;
            if (life is Role)
                m_currentRAW = life as Role;
        }
    }
    /// <summary>
    /// 使命召唤触发后的特效
    /// </summary>
    public void ShowSkill9001Effect()
    {
		GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "1000051_02", MyHead.soldierItem.transform.position, MyHead.soldierItem.transform);

        if (gae != null)
        {
            GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(2f);
            gae.AddAction(ndEffect);
        }
    }
    /// <summary>
    /// 使命召唤点击后显示的特效
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowSKill9001Effect0(bool isShow)
    {
        if (isShow)
        {
			//GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "2000621", MyHead.soldierItem.transform.position, soldierItem.transform);
           // Skill9001Effect0 = gae.gameObject;
            AddFireBtnState(FIRE_BTN_STATE.FireBtnSkill9001);
        }
        else
        {
           // U3DUtil.Destroy(Skill9001Effect0);
            DelFireBtnState(FIRE_BTN_STATE.FireBtnSkill9001);
        }
    }
    /// <summary>
    /// 显示/删除 状态特效
    /// </summary>
    /// <param name="type">眩晕、冰冻、压扁、击飞、麻痹、死亡</param>
    public void ShowStatusEffect(StatusType type,float duration)
    {
		if (CombatScheduler.State == CSState.End)
        {
            return;
        }
        string name = "";
        switch (type)
        {
            case StatusType.Vertigo://眩晕
                U3DUtil.DestroyAllChild(MyHead.StatusEffectParent, true);
                name = "2000691_02";
            break;
            case StatusType.IceBuild://冰冻
            U3DUtil.DestroyAllChild(MyHead.StatusEffectParent, true);
                name = "2000691_01";
            break;
            case StatusType.Squash://压扁
                U3DUtil.DestroyAllChild(MyHead.StatusEffectParent, true);
                name = "2000691_05";
            break;
            case StatusType.ClickFly://击飞
                U3DUtil.DestroyAllChild(MyHead.StatusEffectParent, true);
                name = "2000691_03";
            break;
            case StatusType.paralysis://麻痹
                U3DUtil.DestroyAllChild(MyHead.StatusEffectParent, true);
                name = "2000691_04";
            break;
            case StatusType.Die://死亡
                U3DUtil.DestroyAllChild(MyHead.StatusEffectParent, true);
			    NGUIUtil.Change2DSpriteGray(m_soldierItem.MyHead.SprRolePhoto);
            break;
            
            case StatusType.None:
                U3DUtil.DestroyAllChild(MyHead.StatusEffectParent, true);
            break;

            default:
                    
            break;
        }
        if (name!="")
        {
            GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", name, MyHead.StatusEffectParent.transform.position, MyHead.StatusEffectParent.transform);
            gae.gameObject.AddComponent<SetRenderQueue>();
            GameObjectActionDelayDestory delay = new GameObjectActionDelayDestory(duration);
            gae.AddAction(delay);
        }
    }
	void ShowNoTargetEffect( bool isshow)
	{
		if (MyHead.StatusEffectParent.transform.childCount > 0)
		{
			if(!isshow)
			{
				if (MyHead.StatusEffectParent.transform.GetChild(0).gameObject.name == "2000691_06")
				{
					Destroy(MyHead.StatusEffectParent.transform.GetChild(0).gameObject);
				}
			}
			
		}
		else
		{
			if(isshow)
			{
				GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "2000691_06", MyHead.StatusEffectParent.transform.position, MyHead.StatusEffectParent.transform);
				gae.gameObject.AddComponent<SetRenderQueue>();
				GameObjectActionDelayDestory delay = new GameObjectActionDelayDestory(99999);
				gae.AddAction(delay);
			}
			
		}
	}
}
