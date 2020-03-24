using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 已召唤英雄项
/// </summary>
public class NOExistHeroItem : MonoBehaviour
{	
	private SoldierInfo Info;
	private bool m_canExist ;
	public NOExistHeroItem_h MyHead;
	public bool bGuideSelect = false;

	void Awake()
	{
		MyHead = GetComponent<NOExistHeroItem_h>();
	}

	public void SetData(SoldierInfo I,bool  canExist)
	{
		Info = I ;
		m_canExist = canExist ;

	}

    public void BtnSelect_OnClickEventHandler(UIButton sender)
    {
		if (m_canExist == true)//可召唤但未召唤的炮弹兵
        {
            SummonHeroWnd wnd = WndManager.GetDialog<SummonHeroWnd>();
			if(wnd != null)
			{
				wnd.SetData(Info,null,1);
			}
		}
		else//不可召唤的炮弹兵 目前不做处理
        {
			sdata.s_itemtypeInfo itemInfo = ItemM.GetItemInfo(Info.fragmentTypeID);//当前灵魂石
			ItemComeFromWnd wnd = WndManager.GetDialog<ItemComeFromWnd>();
			wnd.SetData(itemInfo,Info,null,3);
        }
    }

	// Use this for initialization
	void Start () 
    {
		if (MyHead.BtnSelect)
        {
			MyHead.BtnSelect.OnClickEventHandler += BtnSelect_OnClickEventHandler;
        }
        SetUI();
	}

    /// <summary>
    /// 设置角色头像
    /// </summary>
    /// <param name="id"></param>
    public void SetRolePhoto(int id)
    {
        NGUIUtil.Set2DSpriteGraySV(MyHead.SprRolePhoto, "Textures/role/", id.ToString());
    }

    /// <summary>
    /// 设置 角色名称 + 彩色品质等级
    /// </summary>
    /// <param name="name">角色名称 s_soldierType</param>
    /// <param name="quality">角色品质 d_soldier</param>
    public void SetNameLevel(string name,int quality)
    {
		int bigLevel = ConfigM.GetBigQuality (quality);
   
		NGUIUtil.SetLableText<string>(MyHead.LblNameLevel, NGUIUtil.GetBigQualityName(name,quality));
        NGUIUtil.SetSpriteGray(MyHead.SprQualityBg, bigLevel.ToString());

    }
    /// <summary>
    /// 设置 灵魂石数量/升级数量  和 进度条
    /// </summary>
    /// <param name="current">拥有的量</param>
    /// <param name="upValue">升级数量</param>
    public void SetPercentageNum(int current,int upValue)
    {
        string result = string.Format("{0}/{1}", current, upValue);
		NGUIUtil.SetLableText<string>(MyHead.LblNumPercentage, result);
        
		if (MyHead.SprNumPercentage)
        {
			MyHead.SprNumPercentage.fillAmount = (current*1.0f)/(upValue*1.0f);
        }
    }

    private void SetUI()
    {
		SetRolePhoto(Info.SoldierTypeID);
		NGUIUtil.SetStarLevelNum(MyHead.StarSprites,Info.StarLevel);
		NGUIUtil.SetRoleType(MyHead.SprRoleType,Info.m_main_proerty);
		SetNameLevel(Info.m_name, Info.Quality);//默认未召唤的是100
		int NeedNum = 0;
		int NeedCoin = 0;
		SoldierM.GetUpStarNeed(Info.SoldierTypeID ,Info.StarLevel , ref NeedNum ,ref  NeedCoin);
		int Have = Info.GetHaveFragmentNum();//当前灵魂石
		SetPercentageNum(Have, NeedNum);

		MyHead.SprRedPoint.SetActive(false);
    }
	
}
