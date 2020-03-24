using UnityEngine;
using System.Collections;

using sdata;

public class ItemUIInterface
{
	public enum IconType
	{
		None = 0,
		//IconType_Item = 1,
		IconType_Soldier = 1,	// 英雄整卡
		IconType_Fragment = 2,	// 道具碎片
		IconType_Bulding = 3,	// 建筑类
		IconType_Captain = 4,	// 黑科技
	}
	public class ItemIconInfo
	{
		public IconType mType;
		public int mID;
		public int mCount;
		public string mName;
		public string mTexturePath;
		public bool mIsSoul = false;
		public bool mIsBook = false;
	}
	// isSoul是为了兼容闪亮登场界面的信息。
	public static ItemIconInfo GetIconInfo(IconType iconType, int id, bool isSoul, bool isBook)
	{
		ItemIconInfo iconInfo = new ItemIconInfo();
		if (iconType == IconType.IconType_Fragment)
		{
			if (isSoul)
			{
				int soldierTypeId = SoldierM.GetSoldierStarID(id);
				s_soldier_typeInfo info = SoldierM.GetSoldierType(soldierTypeId);
				if (info == null)
				{
					NGUIUtil.DebugLog("s_soldiertype id = " + id + " 静态表数据未配置！");
					return null;
				}
				else
				{
					iconType = IconType.IconType_Soldier;
					id = info.modeltype;
				}
			}
			else if (isBook)
			{
				iconType = IconType.IconType_Captain;
				int godSkillType = GodSkillM.GetGodSkillType(id);
				id = GodSkillM.GetCaptainID(godSkillType);
			}
		}

		if (iconType == IconType.IconType_Soldier)//英雄整卡
		{
			s_soldier_typeInfo info = SoldierM.GetSoldierType(id);
			if (info == null)
			{
				NGUIUtil.DebugLog("s_soldiertype id = " + id + " 静态表数据未配置！");
				return null;
			}
			else
			{
				iconInfo.mType = iconType;
				iconInfo.mName = info.name;
				iconInfo.mID = info.modeltype;
			}
		}
		else if (iconType == IconType.IconType_Fragment)//道具/碎片
		{
			s_itemtypeInfo info = ItemM.GetItemInfo(id);
			if (info == null)
			{
				NGUIUtil.DebugLog("s_itemtype id = " + id + " 静态表数据未配置！");
				return null;
			}
			else
			{
				iconInfo.mType = iconType;
				iconInfo.mName = info.name;
				int soldierTypeId = SoldierM.GetSoldierStarID(id);
				iconInfo.mID = info.gid;
			}
		}
		else if (iconType == IconType.IconType_Bulding)//陷阱
		{
			s_building_typeInfo buildinfo = buildingM.GetBuildType(id);

			iconInfo.mType = iconType;
			iconInfo.mName = buildinfo.name;
			iconInfo.mID = buildinfo.modeltype;
		}
		else if (iconType == IconType.IconType_Captain)//黑科技
		{
			iconInfo.mType = iconType;
			CaptionInfo captionInfo = new CaptionInfo();
			GodSkillM.GetCaption(id, ref captionInfo);
			GodSkillInfo godSkillInfo = new GodSkillInfo();
			GodSkillM.GetGodSkill(captionInfo.m_godskilltype1, 1, ref godSkillInfo);
			iconInfo.mName = godSkillInfo.m_name;
			iconInfo.mID = id;
		}
		iconInfo.mIsSoul = isSoul;
		iconInfo.mIsBook = isBook;
		return iconInfo;
	}
}
