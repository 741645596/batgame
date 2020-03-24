using UnityEngine;
using System.Collections;
using sdata;

/// <summary>
/// 模块解锁标识
/// </summary>
public enum DEBLOCKINGFLAG {
    UPSKILLLEV     = 0x01,        //技能升级
    CAPTAIN        = 0x02,        //黑科技
    SHOP           = 0x04,        //商店
    EDITSHIP       = 0x08,        //船只编辑
    TREASURE       = 0x10,        //金银岛
    ATHLETICS      = 0x20,        //海神杯
    TRAP           = 0x40,         //陷阱
    MAIL           = 0x80,         //邮箱
    HardStage      = 0x100,        //精英副本
    DiamondsUser   = 0x200,        //点石成金
}
/// <summary>
/// 货币类型
/// </summary>
public enum Currency {
    /// <summary>
    /// 钻石/代币
    /// </summary>
    Diamond = 1,
    /// <summary>
    /// 钻石/代币
    /// </summary>
    EMoney = Diamond,
    /// <summary>
    /// 金币
    /// </summary>
    GoldCoin = 2,
    /// <summary>
    /// 海神币
    /// </summary>
    AthleticCoin = 3,
    /// <summary>
    /// 水晶
    /// </summary>
    Crystal = 4,
    /// <summary>
    /// 木材
    /// </summary>
    Wood = 5,
    
    
}

/// <summary>
/// 玩家信息
/// </summary>
public class UserInfo
{
    private string m_name;
    public string Name {
        get{return m_name;}
        set{
            if (string.IsNullOrEmpty(value) == false && value != "null")
            {
                m_name = value;
            }
        }
    }
    
    
    private int m_level;
    public int Level {
        get{return m_level;}
        set{
            if (value != -1)
            {
                m_level = value;
            }
        }
    }
    
    private int m_vipLevel;
    public int VIPLevel {
        get { return m_vipLevel; }
        set
        {
            if (value != -1)
            {
                m_vipLevel = value;
            }
        }
    }
    
    private int m_exp;
    public int Exp {
        get{return m_exp;}
        set{
            if (value != -1)
            {
                m_exp = value;
            }
        }
    }
    
    private int m_diamond;//currency=1
    public int Diamond {
        get { return m_diamond; }
        set
        {
            if (value != -1)
            {
                m_diamond = value;
            }
        }
    }
    
    private int m_preDiamond;
    public int PreDiamond {
        get { return m_preDiamond; }
        set
        {
            if (value != -1)
            {
                m_preDiamond = value;
            }
        }
    }
    private int m_Athletics_coin;//currency=2
    public int AthleticsCoin {
        get{return m_Athletics_coin;}
        set{
            if (value != -1)
            {
                m_Athletics_coin = value;
            }
        }
    }
    
    private int m_coin;//currency=2
    public int Coin {
        get{return m_coin;}
        set{
            if (value != -1)
            {
                m_coin = value;
            }
        }
    }
    
    private int m_preCoin;
    public int PreCoin {
        get { return m_preCoin; }
        set
        {
            if (value != -1)
            {
                m_preCoin = value;
            }
        }
    }
    
    private int m_wood;
    public int Wood {
        get{return m_wood;}
        set{
            if (value != -1)
            {
                m_wood = value;
            }
        }
    }
    
    
    
    private int m_stone;
    public int Stone {
        get{return m_stone;}
        set{
            if (value != -1)
            {
                m_stone = value;
            }
        }
    }
    
    private int m_steel;
    public int Steel {
        get{return m_steel;}
        set{
            if (value != -1)
            {
                m_steel = value;
            }
        }
    }
    
    
    private int m_hdiamond;
    public int Hdiamond {
        get{return m_hdiamond;}
        set{
            if (value != -1)
            {
                m_hdiamond = value;
            }
        }
    }
    
    private int m_sociatyid;
    public int Sociatyid {
        get{return m_sociatyid;}
        set{
            if (value != -1)
            {
                m_sociatyid = value;
            }
        }
    }
    
    
    
    private int m_decklevel ;
    public int Decklevel {
        get{return m_decklevel;}
        set{
            if (value != -1)
            {
                m_decklevel = value;
            }
        }
    }
    /// <summary>
    /// 体力值
    /// </summary>
    private int m_physical;
    public int Physical {
        get{return m_physical;}
        set{
            if (value != -1)
            {
                int total = UserM.GetMaxPhysical(UserDC.GetLevel());
                if (m_physical >= total && value < total) { //第一次消耗体力小于最大值时重置体力恢复倒计时
                    GlobalTimer.ResetPhysicsResume();
                }
                m_physical = value;
                
            }
        }
    }
    
    private int m_prePhysical;
    public int PrePhysical {
        get { return m_prePhysical; }
        set { m_prePhysical = value; }
    }
    
    /// <summary>
    /// 用户技能点数
    /// </summary>
    private int m_SkillPoints;
    public int SkillPoints {
        get{return m_SkillPoints;}
        set{
            if (value != -1)
            {
                m_SkillPoints = value;
            }
        }
    }
    
    private int m_skillpoint_time;
    /// <summary>
    /// 技能点恢复时间基点
    /// </summary>
    public int SkillPointTime {
        get { return m_skillpoint_time; }
        set
        {
            if (value != -1)
            {
                m_skillpoint_time = value;
            }
        }
    }
    
    private int m_physical_time;
    /// <summary>
    /// 体力恢复时间基点
    /// </summary>
    public int Physical_time {
        get { return m_physical_time; }
        set
        {
            if (value != -1)
            {
                m_physical_time = value;
            }
        }
    }
    
    /// <summary>
    /// 账户ID / 玩家ID
    /// </summary>
    private int m_uid;
    public  int Uid {
        get{return m_uid;}
        set{
            if (value != -1)
            {
                m_uid = value;
            }
        }
    }
    /// <summary>
    /// 水晶数量
    /// </summary>
    private int m_crystal;
    public  int Crystal {
        get{return m_crystal;}
        set{
            if (value != -1)
            {
                m_crystal = value;
            }
        }
    }
    
    /// <summary>
    /// 金银岛雷达搜索次数
    /// </summary>
    private int m_pirate_ref_time;
    public  int Pirate_ref_time {
        get{return m_pirate_ref_time;}
        set{
            if (value != -1)
            {
                m_pirate_ref_time = value;
            }
        }
    }
    /// <summary>
    /// 技能购买次数
    /// </summary>
    private int m_skilltimes;
    public  int Skilltimes {
        get{return m_skilltimes;}
        set{
            if (value != -1)
            {
                m_skilltimes = value;
            }
        }
    }
    /// <summary>
    /// 金银岛雷达搜索次数
    /// </summary>
    private int m_treasuretimes;
    public  int Treasuretimes {
        get{return m_treasuretimes;}
        set{
            if (value != -1)
            {
                m_treasuretimes = value;
            }
        }
    }
    /// <summary>
    /// 清空玩家数据（避免重新登录后异常）
    /// </summary>
    public void ClearUserInfo()
    {
        this.Name = "";
    }
    
    /// <summary>
    /// 普通商店刷新次数
    /// </summary>
    private int m_shop_Normal_times;
    public  int Shop_Normal_times {
        get{return m_shop_Normal_times;}
        set{
            if (value != -1)
            {
                m_shop_Normal_times = value;
            }
        }
    }
    /// <summary>
    /// 海神杯商店刷新次数
    /// </summary>
    private int m_shop_Athletics_times;
    public  int Shop_Athletics_times {
        get{return m_shop_Athletics_times;}
        set{
            if (value != -1)
            {
                m_shop_Athletics_times = value;
            }
        }
    }
    /// <summary>
    /// 地精商店刷新次数
    /// </summary>
    private int m_shop_Gnome_times;
    public  int Shop_Gnome_times {
        get{return m_shop_Gnome_times;}
        set{
            if (value != -1)
            {
                m_shop_Gnome_times = value;
            }
        }
    }
    /// <summary>
    /// 黑市商店刷新次数
    /// </summary>
    private int m_shop_Black_times;
    public  int Shop_Black_times {
        get{return m_shop_Black_times;}
        set{
            if (value != -1)
            {
                m_shop_Black_times = value;
            }
        }
    }
    /// <summary>
    /// 海神杯挑战次数
    /// </summary>
    private int m_athletics_times;
    public  int Athletics_times {
        get{return m_athletics_times;}
        set{
            if (value != -1)
            {
                m_athletics_times = value;
            }
        }
    }
    /// <summary>
    /// 海神杯购买门票次数
    /// </summary>
    private int m_athletics_buy_times;
    public  int Athletics_buy_times {
        get{return m_athletics_buy_times;}
        set{
            if (value != -1)
            {
                m_athletics_buy_times = value;
            }
        }
    }
    /// <summary>
    /// 金币抽奖次数
    /// </summary>
    private int m_ernie_coin_times;
    public  int Ernie_coin_times {
        get{return m_ernie_coin_times;}
        set{
            if (value != -1)
            {
                m_ernie_coin_times = value;
            }
        }
    }
    /// <summary>
    /// 钻石抽奖次数
    /// </summary>
    private int m_ernie_diamond_times;
    public  int Ernie_diamond_times {
        get{return m_ernie_diamond_times;}
        set{
            if (value != -1)
            {
                m_ernie_diamond_times = value;
            }
        }
    }
    /// <summary>
    /// 冶金次数
    /// </summary>
    private int m_EBuyCoin_times;
    public  int EBuyCoin_times {
        get{return m_EBuyCoin_times;}
        set{
            if (value != -1)
            {
                m_EBuyCoin_times = value;
            }
        }
    }
    /// <summary>
    /// 冶晶次数
    /// </summary>
    private int m_EBuyCryStal_times;
    public  int EBuyCryStal_times {
        get{return m_EBuyCryStal_times;}
        set{
            if (value != -1)
            {
                m_EBuyCryStal_times = value;
            }
        }
    }
    /// <summary>
    /// 伐木次数
    /// </summary>
    private int m_EBuyWood_times;
    public  int EBuyWood_times {
        get{return m_EBuyWood_times;}
        set{
            if (value != -1)
            {
                m_EBuyWood_times = value;
            }
        }
    }
    /// <summary>
    /// 模块解锁
    /// </summary>
    private long m_deblocking_flag;
    public  long Deblocking_flag {
        get{return m_deblocking_flag;}
        set{
            if (value != -1)
            {
                m_deblocking_flag = value;
                m_IsUpdataBlockFlag = true;
                //NGUIUtil.DebugLog("Deblocking_flag:" + m_deblocking_flag);
            } else
            {
                m_IsUpdataBlockFlag = false;
            }
        }
    }
    private int m_iGnomeShopTime;
    /// <summary>
    /// 地精商店到期时间点
    /// </summary>
    public int GnomeShopTime {
        get { return m_iGnomeShopTime; }
        
        set
        {
            if (value != -1)
            {
                this.m_iGnomeShopTime = value;
            }
        }
    }
    private int m_iBlackShopTime;
    /// <summary>
    /// 黑市商店到期时间点
    /// </summary>
    public int BlackShopTime {
        get { return this.m_iBlackShopTime; }
        set
        {
            if (value != -1)
            {
                this.m_iBlackShopTime = value;
            }
        }
    }
    
    /// <summary>
    /// 模块解锁标识是否变更
    /// </summary>
    private bool m_IsUpdataBlockFlag = false;
    public bool IsUpdataBlockFlag {
        get{return m_IsUpdataBlockFlag;}
    }
    
    private int m_iSingInTimes;
    /// <summary>
    /// 本月累计签到次数
    /// </summary>
    public int SignInTimes {
        get { return m_iSingInTimes; }
        set
        {
            if (value != -1)
            {
                this.m_iSingInTimes = value;
            }
        }
    }
    
    private int m_iLastSignInTime;
    /// <summary>
    /// 最近一次签到时间
    /// </summary>
    public int LastSignInTime {
        get { return m_iLastSignInTime; }
        set
        {
            if (value != -1)
            {
                this.m_iLastSignInTime = value;
            }
        }
    }
    
    /// <summary>
    /// 头像ID
    /// </summary>
    private int m_headID;
    public int HeadID {
        get { return m_headID; }
        set
        {
            if (value != -1)
            {
                m_headID = value;
            }
        }
    }
    /// <summary>
    /// 头像框
    /// </summary>
    private int m_headFrameID;
    public int HeadFrameID {
        get { return m_headFrameID; }
        set
        {
            if (value != -1)
            {
                m_headFrameID = value;
            }
        }
    }
    /// <summary>
    /// 头像底图
    /// </summary>
    private int m_headChartID;
    public int HeadChartID {
        get { return m_headChartID; }
        set
        {
            if (value != -1)
            {
                m_headChartID = value;
            }
        }
    }
    /// <summary>
    /// 设置玩家用户信息
    /// </summary>
    public void SetUserInfo(user.UserInfoResponse Info)
    {
        if (Info == null) {
            return ;
        }
        this.Name = Info.name;
        this.Level = Info.level;
        this.Exp = Info.exp;
        this.Coin = Info.coin;
        this.Wood = Info.wood;
        this.Stone = Info.stone;
        this.Steel = Info.steel;
        this.Diamond = Info.diamond;
        this.Sociatyid = Info.sociatyid;
        this.Physical = Info.physical;
        this.Uid = Info.uid;
        this.Decklevel = Info.decklevel;
        this.Crystal = Info.crystal;
        this.Pirate_ref_time = Info.pirate_ref_time;
        this.SkillPoints = Info.skillpoint;
        this.SkillPointTime = Info.skillpoint_time;
        this.Physical_time = Info.physical_time;
        this.Deblocking_flag = Info.deblocking_flag;
        this.AthleticsCoin = Info.athletics_money;
        this.GnomeShopTime = Info.gnomeshop_time;
        this.BlackShopTime = Info.blackshop_time;
        this.HeadID = Info.headid;
        this.HeadFrameID = Info.headframeid;
        this.HeadChartID = Info.headchartid;
        this.LastSignInTime = Info.lastsignin_time;
        this.SignInTimes = Info.signin_times;
    }
    /// <summary>
    /// 设置玩家用户信息
    /// </summary>
    
    public void SetUserInfo(user.BuyPhysicalResponse Info)
    {
        if (Info == null) {
            return ;
        }
        
    }
    
    /// <summary>
    /// 设置玩家次数
    /// </summary>
    public void SetUserTimesInfo(user.UserTimesResponse Info)
    {
        if (Info == null) {
            return ;
        }
        this.Skilltimes = Info.skilltimes;
        this.Treasuretimes = Info.treasuretimes;
        
        this.Shop_Normal_times = Info.shop_normal_times;
        this.Shop_Athletics_times = Info.shop_athletics_times;
        this.Shop_Black_times = Info.shop_black_times;
        this.m_shop_Gnome_times = Info.shop_gnome_times;
        
        this.Athletics_times = Info.athletics_times;
        this.Athletics_buy_times = Info.athletics_buy_times;
        this.Ernie_coin_times = Info.ernie_coin_times;
        this.Ernie_diamond_times = Info.ernie_diamond_times;
        this.EBuyCoin_times = Info.buy_coin_times;
        this.EBuyCryStal_times = Info.buy_crystal_times;
        this.EBuyWood_times = Info.buy_wood_times;
    }
}
