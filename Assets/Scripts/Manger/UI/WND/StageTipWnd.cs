using UnityEngine;
using System.Collections;
/// <summary>
/// 战役点击信息弹窗
/// <From> </From>
/// <Author>QFord</Author> 
/// </summary>
public class StageTipWnd : WndBase {

	public StageClickType m_StageClickType;

	public StageTipWnd_h MyHead
	{
		get 
		{
			return (base.BaseHead () as StageTipWnd_h);
		}
	}
	public override void WndStart()
	{
		base.WndStart();
	}
	
	/// <summary>
	/// WorldPos 用户tips的展示位置
	/// Type tips类型
	/// level 等级
	/// Description描述
	/// HaveCount 拥有的数量 物品时需要
	/// price 单价          物品时需要
	/// </summary>
    public void SetTipData(Vector3 WorldPos,
	                       StageClickType Type,
                           int Icon,
	                       int Quality,
	                       string name,
	                       int level,
	                       string Description,
	                       int HaveCount = 0,
	                       int price = 0,
	                       int subType = 0 
	                       )
	{
        switch (Type)
        {
            case StageClickType.Room:
			NGUIUtil.SetActive(MyHead.SprRoom.gameObject,true);
			NGUIUtil.SetActive(MyHead.LblGold.parent.gameObject, false);
			NGUIUtil.SetActive(MyHead.LblCount.gameObject, false);
			NGUIUtil.Set2DSprite(MyHead.SprRoom, "Textures/room/",Icon.ToString());
            break;

            case StageClickType.Item:
                if(subType == 1)
			    {
				NGUIUtil.SetActive(MyHead.SprMonster.gameObject, true);
				NGUIUtil.Set2DSprite(MyHead.SprMonster, "Textures/role/", Icon.ToString());
			    }
				else if(subType == 2)
				{
				NGUIUtil.SetActive(MyHead.SprRoom.gameObject, true);
				NGUIUtil.Set2DSprite(MyHead.SprRoom, "Textures/room/", Icon.ToString());
				}
				else
				{
				NGUIUtil.SetActive(MyHead.SprGoods.gameObject, true);
				NGUIUtil.Set2DSprite(MyHead.SprGoods, "Textures/item/", Icon.ToString());
				}
			NGUIUtil.SetLableText<string>(MyHead.LblCount, string.Format("[FFD894]("+NGUIUtil.GetStringByKey("88800040")+"{0}"+NGUIUtil.GetStringByKey("88800037")+")[-]", HaveCount));
			NGUIUtil.SetLableText<int>(MyHead.LblGold, price);
                break;

            case StageClickType.Role:
			NGUIUtil.SetActive(MyHead.SprMonster.gameObject, true);
			NGUIUtil.SetActive(MyHead.LblGold.parent.gameObject, false);
			NGUIUtil.SetActive(MyHead.LblCount.gameObject, false);
			NGUIUtil.Set2DSprite(MyHead.SprMonster, "Textures/role/", Icon.ToString());
            break;
        }

		NGUIUtil.SetSprite(MyHead.SprIconQuality,ConfigM.GetBigQuality(Quality));
		NGUIUtil.SetLableText<string>(MyHead.LblName, name);
		NGUIUtil.SetLableText<string>(MyHead.LblDesc, string.Format("[FFFFFF]"+NGUIUtil.GetStringByKey("88800039")+"{0}", Description));
        
        if (Type!=StageClickType.Item)
        {
			NGUIUtil.SetLableText<string>(MyHead.LblLevel, string.Format("[FFEC8B]LV:[-]{0}", level));
            NGUIUtil.Set3DUIPos(gameObject, WorldPos);
        }
        else
        {
			NGUIUtil.SetLableText<string>(MyHead.LblLevel, string.Format("[FFEC8B]"+NGUIUtil.GetStringByKey("88800036")+"[-]{0}", level));
			NGUIUtil.SetLableText<string>(MyHead.LblGold, string.Format("[FFEC8B]{0}", price));
            transform.position = WorldPos;
        }
	}
    /// <summary>
    /// 设置2D UI 的弹出窗口
    /// </summary>
    public void SetRoleTipData(Vector3 WorldPos,
                           int Icon,
	                       string name,
	                       int level,
	                       int Quality,
	                       string Description)
    {
		NGUIUtil.SetActive(MyHead.SprMonster.gameObject, true);
		NGUIUtil.SetActive(MyHead.LblGold.parent.gameObject, false);
		NGUIUtil.SetActive(MyHead.LblCount.gameObject, false);
		NGUIUtil.Set2DSprite(MyHead.SprMonster, "Textures/role/", Icon.ToString());
        transform.position = WorldPos;
		NGUIUtil.SetSprite(MyHead.SprIconQuality,ConfigM.GetBigQuality(Quality));
		NGUIUtil.SetLableText<string>(MyHead.LblName, name);
		NGUIUtil.SetLableText<string>(MyHead.LblDesc, string.Format("[FFFFFF]"+NGUIUtil.GetStringByKey("88800039")+"{0}", Description));
		NGUIUtil.SetLableText<string>(MyHead.LblLevel, string.Format("[FFEC8B]LV:[-]{0}", level));
    }

}
