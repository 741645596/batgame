using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 战利品（房间/陷阱 船长）
/// <From> </From>
/// <Author>QFord</Author>
/// </summary>
public class TrophyItem : WndBase {
    
	public TrophyItem_h MyHead
	{
		get 
		{
			return (base.BaseHead () as TrophyItem_h);
		}
	}
	
    /// <summary>
    /// 设置战利品 数据
    /// </summary>
    /// <param name="type">2:房间/陷阱   3:船长</param>
    /// <param name="id">要显示的图片ID</param>
    public void SetData(int type, int id)
    {
        switch (type)
        {
            case 2:
			NGUIUtil.Set2DSprite(MyHead.SprItem, "Textures/room/", id.ToString());
            break;
            case 3:
			NGUIUtil.Set2DSprite(MyHead.SprItem, "Textures/role/", id.ToString());
            break;
        }
    }
	
}
