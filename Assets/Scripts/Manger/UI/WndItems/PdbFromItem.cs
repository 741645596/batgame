using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 炮弹兵 出处 项
/// <From> </From>
/// <Author>QFord</Author>
/// </summary>
public class PdbFromItem : WndBase {

    public PdbFromItem_h MyHead
    {
        get
        {
            return (base.BaseHead() as PdbFromItem_h);
        }
    }

    public SoldierInfo Info;

	public override void WndStart()
	{
		base.WndStart ();
        SetUI();
	}

    void SetUI()
    {
        if (Info == null)
        {
            return;
        }
        SetRolePhoto(Info.SoldierTypeID);
    }
    /// <summary>
    /// 设置角色头像/品质框
    /// </summary>
    /// <param name="id"></param>
    public void SetRolePhoto(int id)
    {
        NGUIUtil.Set2DSprite(MyHead.SprHead, "Textures/role/", id.ToString());
        NGUIUtil.SetSprite(MyHead.SprQuality, "0");
    }
}
