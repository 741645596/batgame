using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic;
using System;
using sdata;

/// <summary>
/// 物品(装备、卷轴、灵魂石、消耗品)数据中心 8xx
/// </summary>
public class ItemDC
{

    /// <summary>
    /// 物品列表,key :物品id，服务器分配的。不是类型
    /// </summary>
    private static Dictionary<int, ItemTypeInfo > m_lItem = new Dictionary<int, ItemTypeInfo>();
    
    /// <summary>
    /// 处理事件
    /// </summary>
    public static bool ProcessData(int  CmdID, int nErrorCode, object Info)
    {
        if (nErrorCode == 0) {
            SaveData(CmdID, Info);
        }
        return true;
    }
    /// <summary>
    /// 存储数据，供查询
    /// </summary>
    private static bool SaveData(int  CmdID, object Info)
    {
        switch (CmdID) {
            case (int)gate.Command.CMD.CMD_802:
                Recv_ItemGetResponse(Info);
                break;
                
            case (int)gate.Command.CMD.CMD_804:
                Recv_ItemUseRespone(Info);
                break;
                
            case (int)gate.Command.CMD.CMD_808:
                RecvEquipComposeResponse(Info);
                break;
        }
        return true;
    }
    /// <summary>
    /// 道具出售
    /// </summary>
    /// <param name="key">Item identifier.</param>
    /// <param name="value">Item number.</param>
    public static bool Send_ItemSellRequest(List<KeyValue> lvalue)
    {
        item.ItemSellRequest request = new item.ItemSellRequest();
        List<item.ItemSellRequest.ItemSellInfo> sellList = request.sell_list;
        item.ItemSellRequest.ItemSellInfo info = new item.ItemSellRequest.ItemSellInfo();
        foreach (KeyValue item in lvalue) {
            info.itemid = item.key;
            info.sellnum = item.value;
            sellList.Add(info);
        }
        return true;
    }
    /// <summary>
    ///  801 获取物品列表
    /// </summary>
    public static bool Send_ItemGetRequest()
    {
        ClearDC();
        item.ItemInfoRequest request = new item.ItemInfoRequest();
        return true;
    }
    /// <summary>
    /// 802
    /// </summary>
    private static void Recv_ItemGetResponse(object Info)
    {
        if (Info == null) {
            return ;
        }
        item.ItemInfoResponse response = Info as item.ItemInfoResponse;
        
        foreach (item.ItemInfo itemInfo in response.item_infos) {
            int action = itemInfo.action;
            int id = itemInfo.id;
            //添加
            if (action == 0) { //新增
                if (m_lItem.ContainsKey(id) == true) {
                    ItemTypeInfo I = m_lItem[id];
                    ItemM.UpdateItemInfo(itemInfo, ref I);
                } else {
                    ItemTypeInfo I = ItemM.GetItemInfo(itemInfo);
                    if (I != null) {
                        m_lItem.Add(id, I);
                    }
                }
            }
            //update
            else if (action == 1) {
                if (m_lItem.ContainsKey(id) == true) {
                    ItemTypeInfo I = m_lItem[id];
                    ItemM.UpdateItemInfo(itemInfo, ref I);
                }
            } else { //删除
                if (m_lItem.ContainsKey(id) == true) {
                    m_lItem.Remove(id);
                }
            }
        }
    }
    /// <summary>
    /// 803使用物品
    /// </summary>
    /// <returns><c>true</c>, if item use request was send_ed, <c>false</c> otherwise.</returns>
    public static bool Send_ItemUseRequest(int ItemId, int ItemNum, int iSoldierID)
    {
        item.ItemUseRequest request = new item.ItemUseRequest();
        request.item_id = ItemId;
        request.to_id = iSoldierID;
        request.num = ItemNum;
        return true;
    }
    /// <summary>
    /// 804 使用物品回应
    /// </summary>
    /// <returns><c>true</c>, if item user respone was recv_ed, <c>false</c> otherwise.</returns>
    public static void Recv_ItemUseRespone(object Info)
    {
        if (Info == null) {
            return ;
        }
    }
    /// <summary>
    /// 807 装备合成
    /// </summary>
    public static bool SendEquipComposeRequest(int ItemTypeID)
    {
        item.EquipComposeRequest request = new item.EquipComposeRequest();
        request.itemtypeid = ItemTypeID;
        return true;
    }
    /// <summary>
    /// 808
    /// </summary>
    private static void RecvEquipComposeResponse(object Info)
    {
        if (Info == null) {
            return;
        }
        item.EquipComposeResponse response = Info as item.EquipComposeResponse;
    }
    
    /// <summary>
    /// 809 批量使用
    /// </summary>
    public static bool SendItemBatchUseRequest(List<KeyValue> lvalue, int iSoldierID)
    {
        item.ItemBatchUseRequest request = new item.ItemBatchUseRequest();
        foreach (KeyValue v in lvalue) {
            item.ItemBatchUseRequest.ItemUseInfo info = new item.ItemBatchUseRequest.ItemUseInfo();
            info.item_id = v.key;
            info.num = v.value;
            info.to_id = iSoldierID;
            request.use_infos.Add(info);
        }
        return true;
    }
    
    /// <summary>
    /// 清空数据
    /// </summary>
    public static void ClearDC()
    {
        m_lItem.Clear();
    }
    /// <summary>
    /// 获取玩家物品列表(排除已经装备的物品)
    /// </summary>
    public static List<ItemTypeInfo> GetItemList(ItemType type)
    {
        List<ItemTypeInfo> l = new List<ItemTypeInfo>();
        foreach (ItemTypeInfo item in m_lItem.Values) {
            if (item.Positon != 0) {
                continue;
            }
            int value = (int) type;
            int iv = (int)item.m_type;
            if ((value & iv) == iv) {
                l.Add(item);
            }
        }
        return l;
    }
    /// <summary>
    /// 获取每种类型物品的数量(ItemTypeID)
    /// </summary>
    /// <returns>返回搜索列表list</returns>
    public static int GetItemCount(int ItemType)
    {
        int count = 0;
        List<ItemTypeInfo> l = SearchItemList(ItemType) ;
        foreach (ItemTypeInfo I in l) {
            count += I.Num;
        }
        return count;
    }
    /// <summary>
    /// 根据itemid 获取itemtypeid
    /// </summary>
    /// <param name="itemID"></param>
    /// <returns></returns>
    public static int GetItemID2ItemType(int itemID)
    {
        ItemTypeInfo info = GetItem(itemID);
        if (info != null) {
            return info.itemType;
        }
        return 0;
    }
    /// <summary>
    /// 获取物品
    /// </summary>
    public static ItemTypeInfo GetItem(int id)
    {
        if (m_lItem.ContainsKey(id) == true) {
            return m_lItem[id] ;
        }
        return null;
    }
    /// <summary>
    /// 检测物品是否存在
    /// </summary>
    public static int CheckItem(int itemType)
    {
        foreach (ItemTypeInfo Info in m_lItem.Values) {
            if (Info.itemType == itemType) {
                return Info.Num;
            }
        }
        return 0;
    }
    /// <summary>
    /// 查找物品（不包含已装备的）
    /// </summary>
    public static ItemTypeInfo SearchItem(int itemType)
    {
        foreach (ItemTypeInfo Info in m_lItem.Values) {
            if (Info.itemType == itemType && Info.Positon == 0) {
                return Info;
            }
        }
        return null;
    }
    
    
    /// <summary>
    /// 查找物品(已经装备的排除在外)
    /// </summary>
    private static List<ItemTypeInfo> SearchItemList(int itemType)
    {
        List<ItemTypeInfo> l = new List<ItemTypeInfo>();
        foreach (ItemTypeInfo Info in m_lItem.Values) {
            if (Info.itemType == itemType && Info.Positon != 1) {
                l.Add(Info);
            }
        }
        return l;
    }
    
    /// <summary>
    /// 获取已经物品数量
    /// </summary>
    /// <param name="lItemType">物品类型列表</param>
    /// <param name="Star">已有物品数量，key 物品id，value 数量</param>
    public static void GetItemCount(List<int> lItemType, ref Dictionary<int, int> lNeed)
    {
        if (lItemType == null || lItemType.Count == 0) {
            return ;
        }
        if (lNeed == null) {
            lNeed = new Dictionary<int, int>();
        }
        lNeed.Clear() ;
        
        foreach (int itemType in lItemType) {
            lNeed.Add(itemType, GetItemCount(itemType));
        }
    }
    /// <summary>
    /// 检测物品数量是否超过上限
    /// </summary>
    /// <param name="itemTypeID"></param>
    /// <param name="addNum">增加的量</param>
    /// <returns></returns>
    public static bool CheckItemOverLimit(int itemTypeID, int addNum)
    {
        ItemTypeInfo iTypeInfo = SearchItem(itemTypeID);
        if (iTypeInfo != null) {
            int limit = iTypeInfo.m_Supperpose_limit;
            int addAfterCount = GetItemCount(itemTypeID) + addNum;
            if (addAfterCount <= limit) {
                return false;
            } else {
                return true;
            }
        } else {
            NGUIUtil.DebugLog("静态表 s_itemtype_info id=" + itemTypeID + "数据未配置");
        }
        return false;
    }
    /// <summary>
    /// 检测背包中是否已经大等于列表中的全部物品
    /// </summary>
    public static bool CheckItemsEnough(List<KeyValue> lNeedItemsIdNum)
    {
        List<int> listUsedCheck = new List<int>();
        listUsedCheck.Clear();
        foreach (KeyValue item in lNeedItemsIdNum) {
            int totalCount = ItemDC.GetItemCount(item.key);
            int countUsed = listUsedCheck.Count;
            if (totalCount > 0) {
                int nCnt = 0;
                for (nCnt = 0; nCnt < countUsed; nCnt++) {
                    if (listUsedCheck[nCnt] == item.key) {
                        totalCount--;
                        if (totalCount == 0) {
                            break;
                        }
                    }
                }
                for (nCnt = 0; nCnt < item.value; nCnt++) {
                    listUsedCheck.Add(item.key);
                }
                
            }
            if (totalCount < item.value) {
                listUsedCheck.Clear();
                return false;
            }
        }
        listUsedCheck.Clear();
        return true;
    }
    
    /// <summary>
    /// 按stype 物品列表
    /// </summary>
    public static List<ItemTypeInfo> SearchItemListBystype(int sType)
    {
        List<ItemTypeInfo> l = new List<ItemTypeInfo>();
        foreach (ItemTypeInfo Info in m_lItem.Values) {
            if (Info.m_stype == sType && Info.Positon != 1) {
                l.Add(Info);
            }
        }
        return l;
    }
    
    /// <summary>
    /// 获取扫荡券个数
    /// </summary>
    public static ItemTypeInfo GetSweepTickets()
    {
        int sweepTicketID = ConfigM.GetSweepTicketID();
        List<ItemTypeInfo> sweepTickets = new List<ItemTypeInfo>();
        foreach (ItemTypeInfo info in m_lItem.Values) {
            if (info.itemType == sweepTicketID) {
                return info;
            }
        }
        return null;
    }
    
}



