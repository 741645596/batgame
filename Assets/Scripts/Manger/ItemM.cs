using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Logic;
using sdata;

/// <summary>
/// s_itemtype 物品表 （背包系统功能设计总案）
/// </summary>
/// <author>QFord</author>
public struct KeyValue
{
	public int key;
	public int value;
}
public struct KeyValueName
{
	public int Key;
	public int value;
	public string name;
}

public class ItemM  {

    private static List<s_itemtypeInfo> m_lItemTypeInfo = new List<s_itemtypeInfo>();
	private static List<s_equipcomposeInfo> m_lEquipCompose = new List<s_equipcomposeInfo>();
    /// <summary>
    /// 签到奖励表s_singin
    /// </summary>
    private static List<s_signinInfo> m_lSignIn = new List<s_signinInfo>();
    /// <summary>
    /// 商店售卖物品 s_shop_sell
    /// </summary>
    private static List<s_shop_sellInfo> m_lShopSell = new List<s_shop_sellInfo>();
    /// <summary>
    /// 商店类型 s_shop_type
    /// </summary>
    private static List<s_shop_typeInfo> m_lShopType = new List<s_shop_typeInfo>();
    /// <summary>
    /// 商店手动刷新 s_shop_manual_refresh 
    /// </summary>
    private static List<s_shop_manual_refreshInfo> m_lShopManualRefresh = new List<s_shop_manual_refreshInfo>();

	public static void Init (object obj)
	{
        sdata.StaticDataResponse sdrsp = obj as sdata.StaticDataResponse;
        if (sdrsp == null)
        {
            NGUIUtil.DebugLog("sdata.StaticDataResponse null Error!");
            return;
        }
        
        m_lItemTypeInfo = sdrsp.s_itemtype_info;
		m_lEquipCompose = sdrsp.s_equipcompose_info;
        m_lShopSell = sdrsp.s_shop_sell_info;
        m_lShopType = sdrsp.s_shop_type_info;
        m_lShopManualRefresh = sdrsp.s_shop_manual_refresh_info;

        m_lSignIn = sdrsp.s_signin_info;
        //SortSingIn(ref m_lSignIn);
        
	}
    /// <summary>
    /// 获取物品
    /// </summary>
	/// <param name="ItemTypeID">物品材料ID</param>
    public static s_itemtypeInfo GetItemInfo(int ItemTypeID)
    {
        foreach (var v in m_lItemTypeInfo)
        {
			if (v.id == ItemTypeID)
            {
                return v;
            }
        }
        return null;
    }
    /// <summary>
    /// 获取物品图标
    /// </summary>
    public static string GetItemIcon(int ItemTypeID)
    {
        s_itemtypeInfo info = GetItemInfo(ItemTypeID);
        if (info != null)
        {
            return info.icon.ToString();
        }
        else
            NGUIUtil.DebugLog("静态表s_itemtypeInfo id=" + ItemTypeID + " 未找到！");
        return null;
    }

	/// <summary>
	/// s_equipcompose静态表数据
	/// </summary>
	private static s_equipcomposeInfo Getequipcompose(int itemTypeID)
	{
		foreach (s_equipcomposeInfo item in m_lEquipCompose)
		{
			if (item.itemtypeid == itemTypeID)
			{
				return item;
			}
		}
		return null;
	}
    /// <summary>
    /// 获取s_shop_sell 静态表数据
    /// </summary>
    public static s_shop_sellInfo GetShopSellInfo(int id)
    {
        foreach (s_shop_sellInfo item in m_lShopSell)
        {
            if (item.id == id)
            {
                return item;
            }
        }
        return null;
    }
    /// <summary>
    /// 获取 s_shop_type静态表数据
    /// </summary>
    /// <param name="shopType"></param>
    /// <returns></returns>
    public static s_shop_typeInfo GetShopTypeInfo(int shopType)
    {
        foreach (s_shop_typeInfo item in m_lShopType)
        {
            if (item.shop_type == shopType)
            {
                return item;
            }
        }
        return null;
    }
    /// <summary>
    /// 获取 s_shop_manual_refresh静态表数据
    /// </summary>
    private static s_shop_manual_refreshInfo GetShopManualRefresh(int shopType)
    {
        foreach (s_shop_manual_refreshInfo item in m_lShopManualRefresh)
        {
            if (item.shop_type == shopType)
            {
                return item;
            }
        }
        return null;
    }
    /// <summary>
    /// 获取商店代币类型
    /// </summary>
    public static int GetShopCurrencyType(int shopType)
    {
        s_shop_manual_refreshInfo info = GetShopManualRefresh(shopType);
        if (info!=null)
        {
            return info.currency_type;
        }
        return -1;
    }
    /// <summary>
    /// 获取商店在第n次刷新所消耗的代币量
    /// </summary>
    public static int GetShopRefreshCost(int shopType, int times)
    {
        s_shop_manual_refreshInfo info = GetShopManualRefresh(shopType);
        if (info != null)
        {
            string costStr = info.cost;
            int costCount = NdUtil.GetLength(costStr);
            if (times>=costCount)
            {
                times = costCount-1;
            }
            return NdUtil.GetIntValue(costStr, times);
        }
        return -1;
    }
    /// <summary>
    /// 获取当前年月所有奖励数据
    /// </summary>
    public static List<s_signinInfo> GetCurSignInData()
    {
        List<s_signinInfo> m_lData = new List<s_signinInfo>();
        string nowTime = NdUtil.ConvertServerTimeYYMM();
        foreach (s_signinInfo info in m_lSignIn)
        {
            if (info.id.ToString().Substring(0, 6) == nowTime)
                m_lData.Add(info);
        }
        SortSingIn(ref m_lData);
        return m_lData;
    }

    /// <summary>
    /// 获取当前年月第n次签到的奖励
    /// </summary>
    public static s_signinInfo GetSignInInfo(int times)
    {
        //先选出当前年月的奖励数据
        List<s_signinInfo> m_lData = new List<s_signinInfo>();
        m_lData = GetCurSignInData();
        if (times >= m_lData.Count)
        {
            NGUIUtil.DebugLog(string.Format("获取第 {0}次奖励数据未在s_signin表中找到",times));
            return null;
        }
        return m_lData[times - 1];
    }
    /// <summary>
    /// 对签到奖励数据进行排序后方便获取第n次签到奖励
    /// </summary>
    private static void SortSingIn(ref List<s_signinInfo> l)
    {
        l.Sort(CompSingInInfo);
    }
    static int CompSingInInfo(s_signinInfo a, s_signinInfo b)
    {
        if (a.id < b.id) return -1;
        if (a.id > b.id) return 1;
        return 0;
    }
	/// <summary>
	/// 判断材料是合成该材料的子材料
	/// </summary>
	private static bool CanCombineEquip(s_equipcomposeInfo Info ,int materialType)
	{
		if(Info == null) 
			return false;
		if(Info.material0 != 0 && Info.num0 != 0)
		{
			if(materialType == Info.material0)
				return true;
		}
		if(Info.material1 != 0 && Info.num1 != 0) 
		{
			if(materialType == Info.material1)
				return true;
		}
		if(Info.material2 != 0 && Info.num2 != 0) 
		{
			if(materialType == Info.material2)
				return true;
		}
		if(Info.material3 != 0 && Info.num3 != 0) 
		{
			if(materialType == Info.material3)
				return true;
		}
		if(Info.material4 != 0 && Info.num4 != 0) 
		{
			if(materialType == Info.material4)
				return true;
		}
		if(Info.material5 != 0 && Info.num5 != 0) 
		{
			if(materialType == Info.material5)
				return true;
		}
		return false;
	}

	/// <summary>
	/// 获取合成需要材料需要
	/// </summary>
	/// <param name="EquipType">装备类型type ID</param>
    /// <param name="lNeedSubEquip">需要的装备ID和数量</param>
	/// <param name="NeedCoin">需要的金币</param>
	public static bool GetCombineEquipNeed(int EquipType  ,ref List<KeyValue> lNeedSubEquip ,ref int NeedCoin)
	{
		NeedCoin = 0 ;
		if(lNeedSubEquip == null)
			lNeedSubEquip = new List<KeyValue>();
		lNeedSubEquip.Clear();
		
		s_equipcomposeInfo Info = Getequipcompose(EquipType);
		if(Info != null)
		{
			NeedCoin = Info.coin ;
			if(Info.material0 != 0 && Info.num0 != 0)
			{
				KeyValue ky = new KeyValue();
				ky.key = Info.material0;
				ky.value = Info.num0;
				lNeedSubEquip.Add(ky);
			}
			if(Info.material1 != 0 && Info.num1 != 0) 
			{
				KeyValue ky = new KeyValue();
				ky.key = Info.material1;
				ky.value = Info.num1;
				lNeedSubEquip.Add(ky);
			}
			if(Info.material2 != 0 && Info.num2 != 0) 
			{
				KeyValue ky = new KeyValue();
				ky.key = Info.material2;
				ky.value = Info.num2;
				lNeedSubEquip.Add(ky);
			}
			if(Info.material3 != 0 && Info.num3 != 0) 
			{
				KeyValue ky = new KeyValue();
				ky.key = Info.material3;
				ky.value = Info.num3;
				lNeedSubEquip.Add(ky);
			}
			if(Info.material4 != 0 && Info.num4 != 0) 
			{
				KeyValue ky = new KeyValue();
				ky.key = Info.material4;
				ky.value = Info.num4;
				lNeedSubEquip.Add(ky);
			}
			if(Info.material5 != 0 && Info.num5 != 0) 
			{
				KeyValue ky = new KeyValue();
				ky.key = Info.material5;
				ky.value = Info.num5;
				lNeedSubEquip.Add(ky);
			}
			return true;
		}
		return false;
	}
	/// <summary>
	/// 获取装备材料的用途
	/// </summary>
	/// <param name="EquipMaterialType">装备原料类型type ID</param>
	/// <returns>返回能合成装备的列表list</returns>
	public static List<s_itemtypeInfo> GetEquipMaterialUse(int EquipMaterialType )
	{
		List<s_itemtypeInfo> l = new List<s_itemtypeInfo> ();
		foreach (s_equipcomposeInfo item in m_lEquipCompose)
		{
			if(CanCombineEquip (item ,EquipMaterialType) == true)
			{
				s_itemtypeInfo Info =  GetItemInfo(item.itemtypeid);
				if(Info != null)
					l.Add(Info);
			}
		}
		return l;
	}

    /// <summary>
    /// 检测指定装备是否可合成(合成材料足够)
    /// </summary>
    /// <param name="itemTypeID"></param>
    /// <returns></returns>
    public static bool CheckEquipCompose(int ItemTypeID)
    {
        bool result = true;
        List<KeyValue> lNeedSubEquip = new List<KeyValue>();
        int coin = 0;
        GetCombineEquipNeed(ItemTypeID, ref lNeedSubEquip, ref coin);
        if (lNeedSubEquip.Count == 0)
        {
            return false;
        }
		List<int>  listUsedItem=new List<int>();
        foreach (KeyValue item in lNeedSubEquip)
        {
			if (!CheckEquipEnough(item.key,item.value,ref listUsedItem))
            {
                if (CheckEquipCompose(item.key) == false)
                {
                    return false;
                }
            }
        }
		listUsedItem.Clear();
        return result;
    }
    /// <summary>
    /// 检测装备是否是能够通过合成生成的装备
    /// </summary>
    /// <param name="ItemTypeID"></param>
    /// <returns></returns>
    public static bool CheckEquipCanCompose(int ItemTypeID)
    {
        bool result = false;
        foreach (s_equipcomposeInfo item in m_lEquipCompose)
        {
            if (item.itemtypeid == ItemTypeID)
            {
                return true;
            }
        }
        return result;
    }

    /// <summary>
    /// 检测装备是否大等于某个数量
    /// </summary>
    /// <returns></returns>
	public static bool CheckEquipEnough(int itemTypeID, int count,ref List<int> listUsedItem)
	{
		int totalCount = ItemDC.GetItemCount(itemTypeID);
		int countUsed = listUsedItem.Count;
		if (totalCount > 0) 
		{
			int nCnt =0;
			for(nCnt =0;nCnt<countUsed;nCnt++)
			{
				if(listUsedItem[nCnt]==itemTypeID)
				{
					totalCount--;
					if (totalCount == 0) 
						break;
				}
			}
			for(nCnt =0;nCnt<count;nCnt++)
			{
				listUsedItem.Add(itemTypeID);
			}
			
		}
		return totalCount >= count;
    }

	public static ItemTypeInfo GetItemInfo(s_itemtypeInfo info)
	{
		if(info == null)
			return null;
		ItemTypeInfo I = new ItemTypeInfo ();
		I.SetType(info.type);
		I.m_stype = info.stype;
		I.m_Icon= info.icon;
		I.m_Quality= info.quality;
		I.m_Name= info.name;
		I.m_Supperpose_limit= info.superpose_limit;
		I.m_isuser= info.isuse;
		I.m_level= info.level;
		I.m_money= info.money;
		I.m_emoney= info.emoney;
		I.m_sellemoney= info.sellmoney;
		I.m_title= info.title;
		I.m_message= info.message;
		I.m_gtype= info.gtype;
		I.m_gid= info.gid;

		return I;

	}


	/// <summary>
	/// 填充物品信息
	/// </summary>
	private  static void FillBaseItemInfo(s_itemtypeInfo info ,ref ItemTypeInfo I)
	{
		if(I == null || info == null) return;
		I.SetType(info.type);
		I.itemType = info.id;
		I.m_stype = info.stype;
		I.m_Icon= info.icon;
		I.m_Quality= info.quality;
		I.m_Name= info.name;
		I.m_Supperpose_limit= info.superpose_limit;
		I.m_isuser= info.isuse;
		I.m_level= info.level;
		I.m_money= info.money;
		I.m_emoney= info.emoney;
		I.m_sellemoney= info.sellmoney;
		I.m_title= info.title;
		I.m_message= info.message;
		I.m_gtype= info.gtype;
		I.m_gid= info.gid;
		I.m_args = info.args;
		I.m_func = info.func;
	}

	/// <summary>
	/// 获取物品
	/// </summary>
	/// <returns></returns>
	public static ItemTypeInfo GetItemInfo(item.ItemInfo item)
	{
		if(item == null) return null;
		ItemTypeInfo I = new ItemTypeInfo ();
		s_itemtypeInfo info = GetItemInfo(item.itemtypeid);
		if(info == null) return null;

		I.ID = item.id;
		I.Positon = item.position;
		I.Num = item.superpose;
		FillBaseItemInfo(info ,ref I);

		return I;
	}
	/// <summary>
	/// 获取物品
	/// </summary>
	/// <returns></returns>
	public static ItemTypeInfo GetItemInfo(stage.StageResource.ItemInfo  item)
	{
		if(item == null) return null;
		ItemTypeInfo I = new ItemTypeInfo ();
		s_itemtypeInfo info = GetItemInfo(item.itemtypeid);
		if(info == null) return null;
		//
		I.ID = item.did;
		I.Positon = 0;
		I.Num = item.superpose;
		FillBaseItemInfo(info ,ref I);
		
		return I;
	}

	/// <summary>
	/// 获取物品
	/// </summary>
	/// <returns></returns>
	public static ItemTypeInfo GetItemInfo(stage.StageRewardGetResponse.ItemInfo  item)
	{
		if(item == null) return null;
		ItemTypeInfo I = new ItemTypeInfo ();
		s_itemtypeInfo info = GetItemInfo(item.sitemtypeid);
		if(info == null) return null;
		//
		I.ID = item.did;
		I.Positon = 0;
		I.Num = item.num;
		FillBaseItemInfo(info ,ref I);
		
		return I;
	}
	/// <summary>
	/// 更新物品
	/// </summary>
	/// <returns></returns>
	public static void UpdateItemInfo(item.ItemInfo item ,ref ItemTypeInfo I)
	{
		if(item == null || I == null) return ;
		//
		I.ID = item.id;
		I.Positon = item.position;
		I.Num = item.superpose;
	}

    /// <summary>
    /// 返回物品的描述
    /// </summary>
    public static string GetItemTitle(s_signinInfo info)
    {
        string result = "";
        int id = info.item_type;
        SignInRewardType type = (SignInRewardType)info.reward_type;
        switch (type)
        {
            case SignInRewardType.BlackScience:
                CaptionInfo cInfo = new CaptionInfo();
                GodSkillM.GetCaption(id, ref cInfo);
                GodSkillInfo gInfo = new GodSkillInfo();
                GodSkillM.GetGodSkill(cInfo.m_godskilltype1, 1, ref gInfo);
                result = gInfo.m_explain;
                break;

            case SignInRewardType.HeroSoulFragment:
            case SignInRewardType.ItemAndEquip:
            case SignInRewardType.TrapFragment:
            case SignInRewardType.BlackScienceFragment:
                s_itemtypeInfo info1 = ItemM.GetItemInfo(id);
                if (info1 != null)
                    result = info1.title;
                break;
            case SignInRewardType.Hero:
                SoldierInfo info2 = SoldierM.GetSoldierInfo(id);
                if (info2 != null)
                    result = info2.m_desc;
                break;
            case SignInRewardType.Trap:
                BuildInfo info3 = buildingM.GetStartBuildInfo(id);
                if (info3 != null)
                    result = info3.m_Desc;
                break;
            case SignInRewardType.GoldCoin:
                result = NGUIUtil.GetStringByKey(30000084);
                break;
            case SignInRewardType.Diamond:
                result = NGUIUtil.GetStringByKey(30000085);
                break;
            case SignInRewardType.Crystal:
                result = NGUIUtil.GetStringByKey(30000087);
                break;
            case SignInRewardType.Wood:
                result = NGUIUtil.GetStringByKey(30000086);
                break;
        }
        return result;
    }

    /// <summary>
    /// 返回物品的数量
    /// </summary>
    public static int GetItemCount(s_signinInfo info)
    {
        int result = 0;
        int id = info.item_type;
        SignInRewardType type = (SignInRewardType)info.reward_type;
        switch (type)
        {
            case SignInRewardType.BlackScience:
                CaptionInfo cInfo = new CaptionInfo();
                GodSkillM.GetCaption(id, ref cInfo);
                if (cInfo.m_captionid > 0)
                    result = 1;
                break;

            case SignInRewardType.HeroSoulFragment:
            case SignInRewardType.ItemAndEquip:
            case SignInRewardType.TrapFragment:
            case SignInRewardType.BlackScienceFragment:
                result = ItemDC.GetItemCount(id);
                break;
            case SignInRewardType.Hero:
                SoldierInfo info2 = SoldierM.GetSoldierInfo(id);
                if (info2 != null)
                    result = 1;
                break;
            case SignInRewardType.Trap:
                BuildInfo info3 = buildingM.GetStartBuildInfo(id);
                if (info3 != null)
                    result = 1;
                break;
            case SignInRewardType.GoldCoin://下面内容策划待定
                result = UserDC.GetCoin();
                break;
            case SignInRewardType.Diamond:
                result =UserDC.GetDiamond();
                break;
            case SignInRewardType.Crystal:
                result = UserDC.GetCrystal();
                break;
            case SignInRewardType.Wood:
                result = UserDC.GetWood();
                break;
        }
        return result;
    }
}

public enum SignInRewardType
{
    /// <summary>
    /// 英雄灵魂碎片
    /// </summary>
    HeroSoulFragment = 1,
    /// <summary>
    /// 道具和装备
    /// </summary>
    ItemAndEquip = 2,
    /// <summary>
    /// 英雄
    /// </summary>
    Hero = 3,
    /// <summary>
    /// 木材
    /// </summary>
    Wood = 4,
    /// <summary>
    /// 水晶
    /// </summary>
    Crystal = 5,
    /// <summary>
    /// 金币
    /// </summary>
    GoldCoin = 6,
    /// <summary>
    /// 钻石
    /// </summary>
    Diamond = 7,
    /// <summary>
    /// 陷阱碎片
    /// </summary>
    TrapFragment = 8,
    /// <summary>
    /// 整个陷阱
    /// </summary>
    Trap = 9,
    /// <summary>
    /// 黑科技
    /// </summary>
    BlackScience = 10,
    /// <summary>
    /// 黑科技碎片
    /// </summary>
    BlackScienceFragment = 11,
}
