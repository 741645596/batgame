using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TrapViewItem : MonoBehaviour {

	public TrapViewItem_h MyHead;
	private TrapViewListWnd myParent = null;
	private BuildInfo m_Info;
	private TrapState m_TrapState = TrapState.Exit;
	/// <summary>
	/// 1陷阱列表，2其他地方调用.
	/// </summary>
	private int m_ItemType = 1;

	void Awake()
	{
		MyHead = GetComponent<TrapViewItem_h>();
	}
	public void SetBuildInfo(BuildInfo info,TrapViewListWnd wnd,TrapState trapState,int ItemType = 1)
	{
		myParent = wnd;
		m_Info = info;
		m_ItemType = ItemType;
		m_TrapState = trapState;
	}

	public void BtnSelect_OnClickEventHandler(UIButton sender)
	{
		if(myParent != null && m_ItemType == 1 && m_TrapState == TrapState.Exit)
		{
			TrapShowWnd wnd = WndManager.GetDialog<TrapShowWnd>();
			wnd.SetBuildInfo (m_Info,myParent.m_Build);
		}
		else if(m_TrapState == TrapState.CanSum)
		{
			SummonHeroWnd wnd = WndManager.GetDialog<SummonHeroWnd>();
			if(wnd != null)
			{
				wnd.SetData(null,m_Info,2);
			}
		}
		else if(m_TrapState == TrapState.CanNotSum)
		{
			sdata.s_itemtypeInfo itemInfo = ItemM.GetItemInfo(m_Info.fragmentTypeID);//当前灵魂石
			ItemComeFromWnd wnd = WndManager.GetDialog<ItemComeFromWnd>();
			wnd.SetData(itemInfo,null,m_Info,5);
		}
	}

	void Start () 
	{
		if (MyHead.BtnSelect)
		{
			MyHead.BtnSelect.OnClickEventHandler += BtnSelect_OnClickEventHandler;
		}
		SetUI();
	}

	private void SetUI()
	{
		if (m_Info != null)
		{
			SetNameLevel(m_Info.m_name, m_Info.Quality);
			SetTrapLevel(m_Info.Level);
			NGUIUtil.SetStarLevelNum (MyHead.StarSprites,m_Info.StarLevel);
			
			SetTrapPhoto(m_Info.BuildType);

		
			NGUIUtil.SetTrapTypeIcon(MyHead.SprRoleType,m_Info.m_RoomKind);

			NGUIUtil.Set2DSprite(MyHead.Shape, "Textures/shape/", "shape1");
			NGUIUtil.SetLableText(MyHead.LblDefanAt,string.Format("[FFFFFF]{0}[-]", m_Info.m_DefensePower));

			ShowRedPot(m_ItemType == 1);

			int NeedNum = 0;
			int NeedCoin = 0;
			buildingM.GetUpStarNeed(m_Info.BuildType ,m_Info.StarLevel , ref NeedNum ,ref  NeedCoin);
			int Have = ItemDC.GetItemCount(m_Info.fragmentTypeID);//当前灵魂石
			SetPercentageNum(Have, NeedNum);

		}
		if(m_TrapState == TrapState.Exit)
		{
			MyHead.GoUnDeder.SetActive(m_ItemType == 1);
			MyHead.GoNotExit.SetActive(false);
			MyHead.StarListParent.SetActive(true);
		}
		else
		{
			MyHead.GoUnDeder.SetActive(false);
			MyHead.GoNotExit.SetActive(true);
			MyHead.StarListParent.SetActive(false);
			ShowRedPot(false);
		}

	}
	/// <summary>
	/// 设置 灵魂石数量/升级数量  和 进度条
	/// </summary>
	/// <param name="current">拥有的量</param>
	/// <param name="upValue">升级数量</param>
	public void SetPercentageNum(int current,int upValue)
	{
		if(current >= upValue)
		{
			NGUIUtil.SetLableText<string>(MyHead.LblNumPercentage, NGUIUtil.GetStringByKey(88800109));
		}
		else
		{
			string result = string.Format("{0}/{1}", current, upValue);
			NGUIUtil.SetLableText<string>(MyHead.LblNumPercentage, result);
		}
		
		if (MyHead.SprNumPercentage)
		{
			MyHead.SprNumPercentage.fillAmount = (current*1.0f)/(upValue*1.0f);
		}
	}

	/// <summary>
	/// 设置角色等级
	/// </summary>
	public void SetTrapLevel(int level)
	{
		NGUIUtil.SetLableText<string>(MyHead.LblLevel, level.ToString());
	}

	/// <summary>
	/// 设置陷阱头像.
	/// </summary>
	/// <param name="id"></param>
	public void SetTrapPhoto(int id)
	{
		if(m_TrapState == TrapState.Exit)
			NGUIUtil.Set2DSprite(MyHead.SprTrapPhoto, "Textures/room/", id.ToString());
		else 
			NGUIUtil.Set2DSpriteGraySV(MyHead.SprTrapPhoto, "Textures/room/", id.ToString());
	}

	/// <summary>
	/// 设置 角色名称 + 彩色品质等级 + 设置角色品质框（背景） + 战斗力
	/// </summary>
	/// <param name="name">角色名称 s_soldierType</param>
	/// <param name="quality">角色品质 d_soldier</param>
	public void SetNameLevel(string name,int quality)
	{
		int bigLevel = ConfigM.GetBigQuality(quality);

		NGUIUtil.SetLableText<string>(MyHead.LblSamllQuality, NGUIUtil.GetSmallQualityStr(quality));
		NGUIUtil.SetLableText<string>(MyHead.LblNameLevel, NGUIUtil.GetBigQualityName(name,quality));
		NGUIUtil.SetSprite(MyHead.SprQuality, bigLevel.ToString());
		NGUIUtil.SetSprite(MyHead.SprQualityBg, bigLevel.ToString());
		NGUIUtil.SetSprite(MyHead.SprTrapPhotoBg, bigLevel.ToString());
	}

	private void ShowRedPot(bool Check = true)
	{
		if(Check)
		{
			bool can = BuildDC.CheckCanUp (m_Info);
			
			MyHead.SprRedSpot.SetActive(can);
		}
		else
		{
			MyHead.SprRedSpot.SetActive(false);
		}

	}

}
