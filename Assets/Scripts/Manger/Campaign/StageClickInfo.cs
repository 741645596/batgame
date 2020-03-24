using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 战役点击信息
/// <From> </From>
/// <Author>QFord</Author>
/// </summary>
public class StageClickInfo : MonoBehaviour 
{
    private StageClickType m_stageClickType;

	private int TipID;
    /// <summary>
    /// 名称
    /// </summary>
    private string Name;
    /// <summary>
    /// 等级
    /// </summary>
    private int Level;
    /// <summary>
    /// 玩家拥有个数
    /// </summary>
    private int HaveCount;
    /// <summary>
    /// 购买价格
    /// </summary>
	private int Money;
	/// <summary>
    /// 描述
    /// </summary>
    private string Description;

	private int Quality;

	public void SetInfo(BuildInfo Info)
	{
		if(Info != null)
		{
			m_stageClickType = StageClickType.Room;
            TipID = Info.BuildType;
            Name = Info.m_name;
			Level = Info.Level;
            Description = Info.m_Desc;
			HaveCount = 0;
			Money = 0;
			Quality = Info.Quality;
		}
	}

	public void SetInfo(SoldierInfo Info)
	{
		m_stageClickType = StageClickType.Role;
		TipID = Info.m_modeltype;
		Name = Info.m_name;
		Level = Info.Level;
		HaveCount = 0;
		Money = 0;
		Description = Info.m_desc;
		Quality = Info.Quality;
		foreach(LifeObj m in GetComponentsInChildren<LifeObj>())
		{
			if(m!= null)
			{
				(m.GetLife() as Role).RoleSkinCom.SetCampModel(false );
			}
		}
	}

	public void SetInfo(int itemID)
	{
		m_stageClickType = StageClickType.Item;
		sdata.s_itemtypeInfo Info = ItemM.GetItemInfo(itemID);
		if(Info != null)
		{
			TipID = itemID;
			Name = Info.name;
			Level = Info.level;
			Description = Info.title;
			Money = Info.money;
			HaveCount = ItemDC.GetItemCount(itemID);
			Quality = Info.quality;
		}
	}

	public void ClickDown(Vector3 MousePos)
	{
        Vector3 pos = U3DUtil.SetZ(MousePos, -Camera.main.transform.position.z);
        pos = Camera.main.ScreenToWorldPoint(pos);
        if (Mathf.Abs(Camera.main.transform.position.z) > 15)
        {
            pos = U3DUtil.AddY(pos, 2.5f);
        }
        else
        {
            pos = U3DUtil.AddY(pos, 1.5f);
        }
        StageTipWnd wnd = WndManager.GetDialog<StageTipWnd>();
        wnd.SetTipData(pos, m_stageClickType, TipID,Quality, Name, Level, Description, HaveCount, Money);
	}

	public void ClickUp(Vector3 MousePos)
	{
        WndManager.DestoryDialog<StageTipWnd>();
	}


    void OnMouseDown()
    {
        ClickDown(Input.mousePosition);
    }

    void OnMouseUp()
    {
       ClickUp(Input.mousePosition);
    }


}
/// <summary>
/// 战役点击信息分类
/// </summary>
public enum StageClickType
{
    /// <summary>
    /// 角色
    /// </summary>
    Role,
    /// <summary>
    /// 房间
    /// </summary>
    Room,
    /// <summary>
    /// 物品
    /// </summary>
    Item,
    /// <summary>
    /// 船长
    /// </summary>
    Captain,
}
