using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using sdata;
/// <summary>
/// 成就静态数据管理
/// </summary>
public class FruitionM 
{
    private static bool _IsLoad = false;
    private static List<s_fruitionInfo> m_lFruitionInfo = new List<s_fruitionInfo>();

    public static void Init(object obj)
    {
        if (_IsLoad == true)
            return;
        System.Diagnostics.Debug.Assert(obj is sdata.StaticDataResponse);
        StaticDataResponse sdrsp = obj as StaticDataResponse;
        m_lFruitionInfo = sdrsp.s_fruition_info;

        _IsLoad = true;
    }
    /// <summary>
    /// 获取s_frution数据
    /// </summary>
    public static s_fruitionInfo GetSFruition(int sFruitionID)
    {
        foreach(s_fruitionInfo item in m_lFruitionInfo)
        {
            if (item.id == sFruitionID)
                return item;
        }
        return null;
    }
    /// <summary>
    /// 获取成就总数量
    /// </summary>
    public static int GetFruitionCount()
    {
        return m_lFruitionInfo.Count;
    }

}
/// <summary>
/// 成就条件类型定义
/// </summary>
public enum FruitionCond
{
    // 一般类
    FRUITIONCOND_VIP_LEVEL = 10101, //玩家VIP等级
    FRUITIONCOND_USER_LEVEL = 10102, //玩家等级
    FRUITIONCOND_JOIN_GUILD = 10103, //首次加入公会
    FRUITIONCOND_SOLDIER_FRAGMENT_CONVERT = 10104, //使用碎片召唤一名英雄
    FRUITIONCOND_BUILD_FRAGMENT_CONVERT = 10105, //使用碎片召唤一个陷阱
    FRUITIONCOND_EDIT_SHIP = 10106, //编辑一次战船
    FRUITIONCOND_DEBLOCKING_EDIT_SHIP = 10107, //解锁船只编辑功能
    FRUITIONCOND_DEBLOCKING_ATHLETICS = 10108, //解锁大海神杯
    FRUITIONCOND_DEBLOCKING_BUILD = 10109, //解锁陷阱功能
    FRUITIONCOND_DEBLOCKING_TREASURE = 10110, //解锁金银岛功能
    FRUITIONCOND_DEBLOCKING_SHOP = 10111, //解锁商店功能
    FRUITIONCOND_ACCUM_GET_SOLDIER = 10112, //累积获得英雄数量
    //FRUITIONCOND_USE_CAPTION_SKILL                  = 10113 --使用一次黑科技*********客户端也要有成就系统
    //--以上不需要统计----
    FRUITIONCOND_BUILD_ANALYZE = 101, //拆N次陷阱
    FRUITIONCOND_MADE_ITEM = 102, //制造N件道具
    FRUITIONCOND_BUY_ITEM = 103, //在商店中购买N件道具
    FRUITIONCOND_ERNIE_HIT_SOLDIER = 104, //在酒馆中招募到N次英雄
    FRUITIONCOND_CONTINUOUS_SIGNIN = 105, //连续签到N次
    FRUITIONCOND_ACCUM_CONSUME_SP = 106, //累积消耗总体力
    FRUITIONCOND_TOTAL_BUY_MOON_CARD = 107, //总计购买月卡次数
    FRUITIONCOND_GUILD_SEND_MERCENARY = 108, //公会派遣佣兵次数
    FRUITIONCOND_WORLD_CHANNEL_SPEAK = 109, //世界频道说话次数
    FRUITIONCOND_ACCUM_FINISH_TASK = 110, //累计完成任意任务次数
    FRUITIONCOND_ACCUM_COLLECT_COIN = 111, //累积获得金币
    FRUITIONCOND_ACCUM_COLLECT_WOOD = 112, //累积获得木材
    FRUITIONCOND_ACCUM_COLLECT_CRYSTAL = 113, //累积获得水晶
    FRUITIONCOND_ACCUM_CONSUME_COIN = 114, //X-累积消耗金币
    FRUITIONCOND_ACCUM_CONSUME_WOOD = 115, //X-累积消耗木材
    FRUITIONCOND_ACCUM_CONSUME_CRYSTAL = 116, //X-累积消耗水晶
    FRUITIONCOND_ACCUM_CONSUME_DIAMOND = 117, //累积消耗钻石
    FRUITIONCOND_ACCUM_CONSUME_HSB = 118, //累积消耗海神币
    FRUITIONCOND_ACCUM_BUY_RUM_1 = 119, //累积在酒馆购买普通朗姆酒的次数
    FRUITIONCOND_ACCUM_BUY_RUM_2 = 120, //累积在酒馆购买烈性朗姆酒的次数
    FRUITIONCOND_ACCUM_BUY_RUM_3 = 121, //累积在酒馆购买至尊朗姆酒的次数
    FRUITIONCOND_TOTAL_BATTLE_TIMES = 122, //总战斗次数
    FRUITIONCOND_TOTAL_LOGIN_TIMES = 123, //总登陆次数
    FRUITIONCOND_ACCUM_GET_BUILD = 124, //累积获得陷阱数量
    //------------------------------------------------------------------------------------
    // 养成类(这里分段只为配置上的方便,程序实现上没有用到)
    //SOLDIER
    FRUITIONCOND_SOLDIER_ACCUM_OWN_5STAR = 200, //累积拥有5星炮弹兵的数量
    FRUITIONCOND_SOLDIER_ACCUM_OWN_TANK = 201, //累积拥有肉盾型炮弹兵的数量
    FRUITIONCOND_SOLDIER_ACCUM_OWN_ATTACK = 202, //累积拥有输出型炮弹兵的数量
    FRUITIONCOND_SOLDIER_ACCUM_OWN_ASSISTANT = 203, //累积拥有辅助型炮弹兵的数量
    FRUITIONCOND_SOLDIER_ACCUM_QUALITY_GREEN = 204, //累积拥有绿色品质炮弹兵的数量
    FRUITIONCOND_SOLDIER_ACCUM_QUALITY_BLUE = 205, //累积拥有蓝色品质炮弹兵的数量
    FRUITIONCOND_SOLDIER_ACCUM_QUALITY_PURPLE = 206, //累积拥有紫色品质炮弹兵的数量
    FRUITIONCOND_SOLDIER_ACCUM_QUALITY_ORANGE = 207, //累积拥有橙色品质炮弹兵的数量
    FRUITIONCOND_SOLDIER_ACCUM_QUALITY_RED = 208, //累积拥有红色品质炮弹兵的数量
    FRUITIONCOND_SOLDIER_ACCUM_UP_LEV = 209, //累计提升炮弹兵的等级数
    FRUITIONCOND_SOLDIER_ACCUM_UP_QUALITY = 210, //累积升级炮弹兵品质次数
    FRUITIONCOND_SOLDIER_ACCUM_UP_STAR = 211, //累积升级炮弹兵星级次数
    FRUITIONCOND_SOLDIER_ACCUM_UP_SKILL_LEV = 212, //累积升级炮弹兵技能次数
    //BUILD
    FRUITIONCOND_BUILD_ACCUM_OWN_5STAR = 220, //累积拥有5星陷阱的数量
    FRUITIONCOND_BUILD_ACCUM_QUALITY_GREEN = 221, //累积拥有绿色品质陷阱的数量
    FRUITIONCOND_BUILD_ACCUM_QUALITY_BLUE = 222, //累积拥有蓝色品质陷阱的数量
    FRUITIONCOND_BUILD_ACCUM_QUALITY_PURPLE = 223, //累积拥有紫色品质陷阱的数量
    FRUITIONCOND_BUILD_ACCUM_QUALITY_ORANGE = 224, //累积拥有橙色品质陷阱的数量
    FRUITIONCOND_BUILD_ACCUM_QUALITY_RED = 225, //累积拥有色色品质陷阱的数量
    FRUITIONCOND_BUILD_ACCUM_UP_LEV = 226, //累计提升陷阱的等级数
    FRUITIONCOND_BUILD_ACCUM_UP_QUALITY = 227, //累积升级陷阱品质次数
    FRUITIONCOND_BUILD_ACCUM_UP_STAR = 228, //累积升级陷阱星级次数
    //黑科技
    FRUITIONCOND_CAPTAIN_ACCUM_OWN_ANY = 240, //累积拥有任意黑科技的数量
    FRUITIONCOND_CAPTAIN_ACCUM_UP_LEV = 241, //累计提升英雄的等级数
    FRUITIONCOND_CAPTAIN_ACCUM_UP_STAR = 242, //累积升级炮弹兵星级次数
    //------------------------------------------------------------------------------------
    // 战斗类
    //海神杯相关
    FRUITIONCOND_ATHLETICS_RANK = 10300, //海神杯排名
    //FRUITIONCOND_ATHLETICS_RANK_CONTINUE_TIME   = 1030? --在海神杯中在100/75/50/25/3名次之内保持一周时间。
    //todo: XX名保持一段时间;战斗过程相关,fuck dog!
    FRUITIONCOND_ATHLETICS_BATTLE_TIMES = 300, //海神杯战斗次数
    FRUITIONCOND_ATHLETICS_CHALLENGE_VICTORY = 301, //海神杯进攻胜利次数
    FRUITIONCOND_ATHLETICS_DEFEND_VICTORY = 302, //海神杯防守胜利次数
    FRUITIONCOND_ATHLETICS_ACCUM_DAMAGE = 303, //在海神杯中累积造成的伤害量
    //金银岛相关
    FRUITIONCOND_TREASURE_ROB = 310, //金银岛掠夺次数
    FRUITIONCOND_TREASURE_ROB_VICTORY = 311, //金银岛掠夺胜利次数
    FRUITIONCOND_TREASURE_EXPLOIT = 312, //金银岛开采次数
    FRUITIONCOND_TREASURE_AVENGE_VICTORY = 313, //金银岛成功复仇
    FRUITIONCOND_TREASURE_ACCUM_EXPLOIT_COIN = 314, //金银岛通过开采累积获得的金币量
    FRUITIONCOND_TREASURE_ACCUM_ROB_COIN = 315, //金银岛通过掠夺累积获得的金币量
    //战役相关
    FRUITIONCOND_STAGE_TOTAL_STAR_NORMAL = 320, //普通副本总星星数
    FRUITIONCOND_STAGE_TOTAL_STAR_HARD = 321, //精英副本总星星数
}
/// <summary>
/// 成就分类
/// </summary>
public enum FruitionCategory
{
    /// <summary>
    /// 综合性成就
    /// </summary>
    FRUITIONCATE_GENERAL                            = 1,
    /// <summary>
    /// 出战
    /// </summary>
    FRUITIONCATE_BATTLE                             = 2 ,
    /// <summary>
    /// 挑战
    /// </summary>
    FRUITIONCATE_CHALLENGE                          = 3,
    /// <summary>
    /// 制造
    /// </summary>
    FRUITIONCATE_MAKE                               = 4,
}