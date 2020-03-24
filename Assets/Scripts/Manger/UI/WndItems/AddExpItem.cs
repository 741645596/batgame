using UnityEngine;

public class AddExpItem : MonoBehaviour 
{
	private SoldierInfo Info;
	private AddExpWnd m_parent;
	private HUDText m_HudText;
	private GameObject m_AddHudText;

	private Vector2 FirstTouchPosition = Vector2.zero;

    private bool m_bMouseDown = false;
	private bool m_bMouseUp = false;
	private bool m_bLongPress = false;

	/// <summary>
	/// 要使用的物品ID
	/// </summary>
	private ItemTypeInfo m_ItemInfo ;
	/// <summary>
	/// 按住0.5s吃m_iPerAddNum个经验.
	/// </summary>
	private float m_fAddSpeed = 0.2f;
	/// <summary>
	/// 鼠标按下时间.
	/// </summary>
	private float m_fPressDownTime = 0;

	/// <summary>
	/// 长按时间.
	/// </summary>
	private float m_iPressTime = 1.0f;
	
	private int m_iHaveAddNum = 0;
	private int m_iLongPressAddNum = 0;

	private float m_delay = 0f;

	private AddExpItem_h MyHead ;

	public void SetData(SoldierInfo I,ItemTypeInfo itemInfo,AddExpWnd  wnd)
	{
		Info = I ;
		m_parent = wnd ;
		m_ItemInfo = itemInfo;
	}
	
	// Use this for initialization.
	void Start () 
	{
		MyHead = GetComponent<AddExpItem_h>();
//		MyHead.HudText.bTopRank = false;
		MyHead.LblNumUsed.gameObject.SetActive (false);
		if (MyHead.BtnSelect)
		{
            MyHead.BtnSelect.OnPressDownEventHandler += BtnSelect_OnPressDownEventHandler;
            MyHead.BtnSelect.OnPressUpEventHandler += BtnSelect_OnPressUpEventHandler;
			MyHead.BtnSelect.OnDragEventHandler += BtnSelect_OnDragEventHandler;           
		}
		SetUI();

        m_AddHudText = NDLoad.LoadWndItem("AddExpHUDText", MyHead.Target.transform);
		GameObject child = NGUITools.AddChild(WndManager.GetWndRoot(), m_AddHudText);
		m_HudText = child.GetComponentInChildren<HUDText>();
		child.AddComponent<UIFollowTarget>().target = MyHead.Target.transform;
		child.GetComponent<UIFollowTarget>().disableIfInvisible = false;
	}

    void Update()
    {
		float time = Time.realtimeSinceStartup - m_fPressDownTime;

		if (m_bMouseDown )
        {
			if(time < m_iPressTime && !m_bLongPress)
			{
				return;
			}

			if(time >= m_fAddSpeed/(1+m_iLongPressAddNum))
			{
				m_bLongPress = true;
				int can = CheckCanAdd();

				if(can == 0)
				{
					AddItemUseNum(1);
					m_iLongPressAddNum += 1;
				}
				else if( can == 1)
				{
                    NGUIUtil.ShowFreeSizeTipWnd(NGUIUtil.GetStringByKey(10000102));
					m_bMouseDown = false;
				}
				else if(can == 2)
				{
                    NGUIUtil.ShowFreeSizeTipWnd(NGUIUtil.GetStringByKey(10000103));
					m_bMouseDown = false;
				}

				if(m_iLongPressAddNum > 0)
				{
					m_bMouseUp = true;
				}

				m_fPressDownTime = Time.realtimeSinceStartup;
			}
		}
		else if(m_bMouseUp)
		{
			if(!m_bLongPress && time < m_iPressTime)
			{
				int can = CheckCanAdd();
				
				if(can == 0)
				{
					AddItemUseNum(1);
					m_parent.SendUseItem (m_ItemInfo.ID,1,Info.ID);
				}
				else if( can == 1)
				{
                    NGUIUtil.ShowFreeSizeTipWnd(NGUIUtil.GetStringByKey(10000102));
				}
				else if(can == 2)
				{
                    NGUIUtil.ShowFreeSizeTipWnd(NGUIUtil.GetStringByKey(10000103));
				}

				m_bMouseUp = false;
			}
			if(m_iLongPressAddNum > 0)
			{
				m_parent.SendUseItem (m_ItemInfo.ID,m_iLongPressAddNum,Info.ID);
			}
			m_bLongPress = false;
			m_iLongPressAddNum = 0;
			m_bMouseUp = false;


		}
		else
		{
			m_bLongPress = false;
			m_iLongPressAddNum = 0;
			m_delay = 0f;
		}
	
    }
	
	private void SetUI()
	{
		SetRolePhoto(Info.SoldierTypeID);
		NGUIUtil.SetStarLevelNum(MyHead.StarSprites,Info.StarLevel);
		NGUIUtil.SetRoleType(MyHead.SprRoleType,Info.m_main_proerty);
		SetNameLevel(Info.m_name, Info.Quality);//默认未召唤的是100.
		
		SetPercentageNum();
	}
	/// <summary>
	/// 炮弹兵是否能增加经验 0:可以增加，1：已经满级不能增加,2:吃完了.
	/// </summary>
	/// <returns><c>true</c>, if can add exp was checked, <c>false</c> otherwise.</returns>
	/// <param name="SoldireTypeID">炮弹兵ID.</param>
	/// <param name="AddExpNum">炮弹兵增加的经验值</param>
	/// 
	private int CheckCanAdd()
	{
		if(m_parent.GetItemNum() <= 0)
		{
			return 2;
		}

		sdata.s_soldier_experienceInfo expInfo = SoldierM.GetSoldierExpData (Info.Level);
		if(expInfo.exp == 0) return 1;
		
		int MaxLvl = UserM.GetUserMaxHeroLevel(UserDC.GetLevel());
		int MaxExp = SoldierM.GetSoldierExp (MaxLvl);
	

		if(Info.EXP >= MaxExp && Info.Level >= MaxLvl)
		{
			return 1;
		}

		return 0;

	}
	/// <summary>
	/// 吃经验.
	/// </summary>
	/// <param name="Num">Number.</param>
	private void AddItemUseNum(int Num)
	{
		m_parent.SetItemNum ();
		m_iHaveAddNum = m_iHaveAddNum + Num;

		if(!MyHead.LblNumUsed.gameObject.activeInHierarchy)
		{
			MyHead.LblNumUsed.gameObject.SetActive(true);
		}
		MyHead.LblNumUsed.text = "X" + m_iHaveAddNum.ToString ();

		int MaxLvl = UserM.GetUserMaxHeroLevel(UserDC.GetLevel());

		Info.EXP += int.Parse(m_ItemInfo.m_args);

		int MaxExp = SoldierM.GetSoldierTotalExpAtLevel (MaxLvl);


		int LevExp = SoldierM.GetSoldierExp (Info.Level);

		if(Info.EXP >= LevExp && Info.Level < MaxLvl)
		{
			int exp = SoldierM.GetSoldierExp(Info.Level);
			CalUplevelNum(exp,MaxLvl);
		}

		int Exp = SoldierM.GetSoldierExp (Info.Level);
		float pre = (Info.EXP*1.0f)/(Exp*1.0f);

		AddLeveAndExp(Info.Level,pre,true);
	}

	private void CalUplevelNum(int LevExp,int MaxLvl)
	{
		if(Info.EXP >= LevExp && LevExp > 0)
		{
			if(Info.Level < MaxLvl)
				Info.EXP -= LevExp;

			AddLeveAndExp(Info.Level,1f,true);
			ShowLabel();

			Info.Level += 1;

			int NextLevExp = SoldierM.GetSoldierExp (Info.Level);
			SoldierM.GetSoldierInfo(ref Info);
			SetPower(Info.m_combat_power);
			if(NextLevExp > 0 && Info.Level < MaxLvl)
			{
				CalUplevelNum(NextLevExp,MaxLvl);
			}

		}
	}
	private void ShowLabel()
	{
		m_HudText.Add (NGUIUtil.GetStringByKey(10000105),new Color(223.0f/255.0f,69.0f/255.0f,229.0f/255.0f), 0);
	}
    void BtnSelect_OnPressUpEventHandler(UIButton sender)
    {
		if(UICamera.currentTouch !=null && UICamera.currentTouch.dragStarted)
		{
			Vector2 delta = FirstTouchPosition - UICamera.currentTouch.pos;
			bool posChanged = delta.sqrMagnitude > 0.001f;
			if(posChanged)
			{
				if(m_iLongPressAddNum > 0)
				{
					m_parent.SendUseItem (m_ItemInfo.ID,m_iLongPressAddNum,Info.ID);
				}
				m_bMouseDown = false;
				m_bMouseUp = false;

				return;
			}
		}
		m_bMouseDown = false;
		m_bMouseUp = true;
    }

	void BtnSelect_OnDragEventHandler(UIButton sender)
	{
		if(UICamera.currentTouch !=null && UICamera.currentTouch.dragStarted)
		{
			Vector2 delta = FirstTouchPosition - UICamera.currentTouch.pos;
			bool posChanged = delta.sqrMagnitude > 0.001f;
			if(posChanged)
			{
				if(m_iLongPressAddNum > 0)
				{
					m_parent.SendUseItem (m_ItemInfo.ID,m_iLongPressAddNum,Info.ID);
				}
				m_bMouseDown = false;
				m_bMouseUp = false;
			}
		}

	}
    void BtnSelect_OnPressDownEventHandler(UIButton sender)
    {
        m_bMouseDown = true;
		m_fPressDownTime = Time.realtimeSinceStartup;
		FirstTouchPosition = Input.mousePosition;
    }

	void AddEffect(object o)
	{
		GameObjectActionExcute gae = gameObject.GetComponent<GameObjectActionExcute>();
		if (gae)
		{
			GameObjectActionWait wait = gae.GetCurrentAction() as GameObjectActionWait;
			if (wait != null)
			{
				int level = (int)wait.Data1;
				float fill = (float)wait.Data2;
				bool bShow = (bool)wait.Data3;

				NGUIUtil.UpdateExpFromValue(MyHead.SprNumPercentage,fill,0.1f);

				NGUIUtil.SetLableText<string>(MyHead.LblLv, level.ToString());

				if(gae.GetNextAction() == null)
					MyHead.LblNumUsed.gameObject.SetActive (false);
				else
					MyHead.LblNumUsed.gameObject.SetActive (bShow);

			}
		}

	}
	private void AddLeveAndExp(int level,float fill,bool bShowNum)
	{
//		if(m_bLongPress)
//		{
//			m_delay = 0.13f/(m_iLongPressAddNum + 1);
//		}
//		else
//		{
//			m_delay = 0.13f;
//		}
		m_delay = 0.13f/(m_iLongPressAddNum + 1);
		GameObjectActionExcute gae = GameObjectActionExcute.CreateExcute(gameObject);
		GameObjectActionWait wait = new GameObjectActionWait(m_delay);
		wait.Data1 = level;
		wait.Data2 = fill;
		wait.Data3 = bShowNum;
		wait.m_complete = AddEffect;
		gae.AddAction(wait);

	}
	/// <summary>
	/// 设置角色头像.
	/// </summary>
	/// <param name="id"></param>
	public void SetRolePhoto(int id)
	{
		NGUIUtil.Set2DSprite(MyHead.SprRolePhoto, "Textures/role/", id.ToString());
	}

	/// <summary>
	/// 设置 角色名称 + 彩色品质等级.
	/// </summary>
	/// <param name="name">角色名称 s_soldierType</param>
	/// <param name="quality">角色品质 d_soldier</param>
	public void SetNameLevel(string name,int quality)
	{
		int bigLevel = ConfigM.GetBigQuality(quality);
	
		NGUIUtil.SetLableText<string>(MyHead.LblNameLevel, NGUIUtil.GetBigQualityName(name,quality));
		MyHead.LblSmallQuality.text = NGUIUtil.GetSmallQualityStr(quality);

		NGUIUtil.SetLableText<string>(MyHead.LblLv, Info.Level.ToString());

		NGUIUtil.SetSprite(MyHead.SprQuality, bigLevel.ToString());
		NGUIUtil.SetSprite(MyHead.SprQualityBg, bigLevel.ToString());
		NGUIUtil.SetSprite(MyHead.SprRolePhotoBg, bigLevel.ToString());

		SetPower(Info.m_combat_power);
	}

	private void SetPower(int power)
	{
		MyHead.LblPower.text = power.ToString ();
	}
	/// <summary>
	/// 设置 灵魂石数量/升级数量  和 进度条.
	/// </summary>
	/// <param name="current">拥有的量</param>
	/// <param name="upValue">升级数量</param>
	public void SetPercentageNum()
	{
		int Exp = SoldierM.GetSoldierExp (Info.Level);


		if (MyHead.SprNumPercentage)
		{
			MyHead.SprNumPercentage.fillAmount = (Info.EXP*1.0f)/(Exp*1.0f);
		}
	}
	public void OnDestroy()
	{
		Destroy (m_HudText.gameObject);
		m_HudText = null;
	}


}