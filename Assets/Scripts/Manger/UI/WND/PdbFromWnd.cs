using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 炮弹兵 来源（灵魂石）
/// <From> </From>
/// <Author>QFord</Author>
/// </summary>
public class PdbFromWnd : WndBase {

    public PdbFromWnd_h MyHead
    {
        get
        {
            return (base.BaseHead() as PdbFromWnd_h);
        }
    }

    public SoldierInfo Info = null;

	public override void WndStart()
	{
		base.WndStart();
        MyHead.BtnReturn.OnClickEventHandler += BtnReturn_OnClickEventHandler;
        MyHead.BtnBgReturn.OnClickEventHandler += BtnReturn_OnClickEventHandler;
        SetUI();
        //LoadItems();
	}

    void BtnReturn_OnClickEventHandler(UIButton sender)
    {
        WndManager.DestoryDialog<PdbFromWnd>();
    }

    void SetUI()
    {
        if (Info == null)
        {
            return;
        }
        SetRolePhoto(Info.SoldierTypeID);
        SetNameLevel(Info.m_name, 0);
		int NeedNum = 0;
		int NeedCoin = 0;
		SoldierM.GetUpStarNeed(Info.SoldierTypeID ,Info.StarLevel  , ref NeedNum ,ref  NeedCoin);
		int Have = Info.GetHaveFragmentNum();//当前灵魂石
		SetPercentageNum(Have, NeedNum);
    }

    void LoadItems()
    {
        GameObject go = NDLoad.LoadWndItem("PdbFromItem", MyHead.Parent.transform);
        if (go != null)
        {
            PdbFromItem item = go.GetComponent<PdbFromItem>();
            item.Info = Info;
        }

        NGUIUtil.RepositionTable(MyHead.Parent);
    }

    /// <summary>
    /// 设置角色头像
    /// </summary>
    /// <param name="id"></param>
    public void SetRolePhoto(int id)
    {
        NGUIUtil.Set2DSprite(MyHead.SprHead, "Textures/role/", id.ToString());
    }

    /// <summary>
    /// 设置 角色名称 + 彩色品质等级
    /// </summary>
    /// <param name="name">角色名称 s_soldierType</param>
    /// <param name="quality">角色品质 d_soldier</param>
    public void SetNameLevel(string name, int quality)
    {
        int bigLevel = ConfigM.GetBigQuality(quality);
    
        NGUIUtil.SetLableText<string>(MyHead.LblName, NGUIUtil.GetBigQualityName(name,quality));
        NGUIUtil.SetSprite(MyHead.SprQuality, bigLevel.ToString());
    }
    /// <summary>
    /// 设置 灵魂石数量/升级数量  和 进度条
    /// </summary>
    /// <param name="current">拥有的量</param>
    /// <param name="upValue">升级数量</param>
    public void SetPercentageNum(int current, int upValue)
    {
        string result = string.Format("{0}/{1}", current, upValue);
        NGUIUtil.SetLableText<string>(MyHead.LblCount, result);
    }
	
}
