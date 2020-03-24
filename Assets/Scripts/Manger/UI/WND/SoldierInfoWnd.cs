using UnityEngine;
using System;
using System.Collections;
using sdata;

/// <summary>
/// 炮弹兵信息界面
/// <From> 炮弹兵详细信息界面</From>
/// <Author>QFord</Author> 150/150
/// </summary>
public class SoldierInfoWnd : WndBase
{

	public SoldierInfoWnd_h MyHead
	{
		get 
		{
			return (base.BaseHead () as SoldierInfoWnd_h);
		}
	}


    private SoldierInfo m_soldierInfo;
    /// <summary>
    /// true 显示 详细信息 false 显示 技能
    /// </summary>
    private bool m_bShowInfo;
	
    void ClearUI()
    {
        WndManager.DestoryDialog<AddSkillWnd>();
		if (MyHead.EquipmentsParent)
        {
			U3DUtil.DestroyAllChild(MyHead.EquipmentsParent);
        }
    }

    public void SetData(SoldierInfo info)
    {
		ClearUI();
		m_soldierInfo = info;

		if (m_soldierInfo == null)
		{
			NGUIUtil.DebugLog("SoldierInfoWnd.cs   m_soldierInfo==null !");
			return;
		}
		SetUI();
	}

    private void SetUI()
    {
        m_bShowInfo = true;

        //Top
		if (MyHead.SprType)//设置角色的类型（力量0、敏捷1、智力2）
        {
			MyHead.SprType.spriteName = string.Format("bb_00{0}ic",m_soldierInfo.m_main_proerty+1);
        }
		if (MyHead.LblTitle)
        {
			MyHead.LblTitle.text = m_soldierInfo.m_name;
        }
        SetStarNum(m_soldierInfo.StarLevel);
        //Mid
        //设置角色3D
        
        //装备 数据暂未配置
        GetEquimment();

        //Bottom
		if (MyHead.LblLevel)
        {
			MyHead.LblLevel.text = m_soldierInfo.Level.ToString();
        }
        SetExp();
		if (MyHead.LblCombatPower)
        {
			MyHead.LblCombatPower.text = m_soldierInfo.m_combat_power.ToString();
        }
        SetSoulPercentage();

        if (m_bShowInfo)
        {
            HeroIntroduceWnd heroWnd = WndManager.GetDialog<HeroIntroduceWnd>();
            heroWnd.SetData(m_soldierInfo);
        }
        SoldierScrollWnd wnd = WndManager.GetDialog<SoldierScrollWnd>();
        if (wnd)
        {
			wnd.m_currentSoldierInfo = m_soldierInfo;
        }

    }
    /// <summary>
    /// 获取装备情况
    /// </summary>
    private void GetEquimment()
    {
		if (MyHead.EquipmentsParent == null)
        {
            NGUIUtil.DebugLog("SoldierInfoWnd.cs   EquipmentsParent == null !");
            return;
        }
        for (int i = 0; i < 6; i++)
        {
			NDLoad.LoadWndItem("EquipmentItem", MyHead.EquipmentsParent.transform);
        }
		NGUIUtil.RepositionTable(MyHead.EquipmentsParent);
    }

	public override void WndStart()
	{
		base.WndStart();
		if (MyHead.BtnClose)
		{
			MyHead.BtnClose.OnClickEventHandler += BtnClose_OnClickEventHandler;
        }

		if (MyHead.BtnUpgradSoulStone)
        {
			MyHead.BtnUpgradSoulStone.OnClickEventHandler += BtnUpgradSoulStone_OnClickEventHandler;
        }

		if (MyHead.BtnJiNeng)
        {
			MyHead.BtnJiNeng.OnClickEventHandler += BtnJiNeng_OnClickEventHandler;
        }
		if (MyHead.BtnJinHua)
        {
			MyHead.BtnJinHua.OnClickEventHandler += BtnJinHua_OnClickEventHandler;
        }
		if (MyHead.BtnJinJie)
        {
			MyHead.BtnJinJie.OnClickEventHandler += BtnJinJie_OnClickEventHandler;
        }
	}

    void BtnUpgradSoulStone_OnClickEventHandler(UIButton sender)
    {
        
    }

    void BtnJinJie_OnClickEventHandler(UIButton sender)
    {
        
    }

    void BtnJinHua_OnClickEventHandler(UIButton sender)
    {
        
    }

    void BtnJiNeng_OnClickEventHandler(UIButton sender)
    {
        if (m_soldierInfo == null)
        {
            NGUIUtil.DebugLog("SoldierInfoWnd m_soldierInfo==null !!!");
            return;
        }

        if (m_bShowInfo)//技能按钮用来 切换显示炮弹兵信息面板 和 技能面板
        {
            m_bShowInfo = false;
            WndManager.DestoryDialog<HeroIntroduceWnd>();
			UISprite sprite = MyHead.BtnJiNeng.transform.GetChild(0).GetComponentInChildren<UISprite>();
            if (sprite)
            {
                sprite.spriteName = "pdbxx_011wd";
                sprite.MarkAsChanged();
            }
            AddSkillWnd wnd = WndManager.GetDialog<AddSkillWnd>();
            if (wnd)
            {
                wnd.SetData(m_soldierInfo);
            }
        }
        else
        {
            m_bShowInfo = true;
            WndManager.DestoryDialog<AddSkillWnd>();
			UISprite sprite = MyHead.BtnJiNeng.transform.GetChild(0).GetComponentInChildren<UISprite>();
            if (sprite)
            {
                sprite.spriteName = "pdbxx_001wd";
                sprite.MarkAsChanged();
            }
            HeroIntroduceWnd heroWnd = WndManager.GetDialog<HeroIntroduceWnd>();
            if (heroWnd)
            {
   
                heroWnd.SetData(m_soldierInfo);
            }
        }
    }
    /// <summary>
    /// 设置星级
    /// </summary>
    private void SetStarNum(int num)
    {
		for (int i = 0; i < MyHead.SprStars.Length; i++)
        {
            if (i<num)
            {
				MyHead.SprStars[i].spriteName = "cz_001ic";
            }
            else
            {
				MyHead.SprStars[i].spriteName = "gong_003ic";
            }
        }
    }
    /// <summary>
    /// 设置灵魂石
    /// </summary>
    private void SetSoulPercentage( )
    {
		if (MyHead.SprSoulPercentage == null || MyHead.LblSoulNum == null)
        {
            NGUIUtil.DebugLog("SoldierInfoWnd.cs SprSoulPercentage or LblSoulNum null !!!");
            return;
        }
        if (m_soldierInfo.StarLevel == 5)
        {
			MyHead.SprSoulPercentage.fillAmount = 1.0f;
			MyHead.LblSoulNum.text = NGUIUtil.GetStringByKey("88800091");
        }
        else
        {
			//判断碎片满足
			int NeedCoin = 0 ;
			int NeedNum = 0;
			SoldierM.GetUpStarNeed(m_soldierInfo.SoldierTypeID ,m_soldierInfo.StarLevel  , ref NeedNum ,ref  NeedCoin);
			int Have = m_soldierInfo.GetHaveFragmentNum();//当前灵魂石

			MyHead.SprSoulPercentage.fillAmount = (Have * 1.0f) / NeedNum;
			MyHead.LblSoulNum.text = string.Format("{0}/{1}", Have, NeedNum);
        }
    }
    /// <summary>
    /// 设置经验值
    /// </summary>
    private void SetExp( )
    {
		int Needexp = SoldierM.GetUpLevelNeedExp(m_soldierInfo.Level);
		if (MyHead.LblExp)
        {
			MyHead.LblExp.text = string.Format("{0}/{1}", m_soldierInfo.EXP, Needexp);
        }
    }

    void BtnClose_OnClickEventHandler(UIButton sender)
    {
        WndManager.DestoryDialog<SoldierInfoWnd>();
        WndManager.DestoryDialog<HeroIntroduceWnd>();
        WndManager.DestoryDialog<AddSkillWnd>();
        WndManager.DestoryDialog<SoldierScrollWnd>();
    }
	
}
