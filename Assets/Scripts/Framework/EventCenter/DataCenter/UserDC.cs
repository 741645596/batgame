using System.Collections.Generic;
using user;


/// <summary>
/// 玩家信息数据中心
/// </summary>

public class UserDC
{
    /// <summary>
    /// 冶金暴击倍数返回.
    /// </summary>
    private static List<int> m_lCritValues = new List<int>();
    private static UserInfo m_UserInfo = new UserInfo();
    
    private static List<user.StatusInfoResponse.StatusInfo> m_lStatusInfo = new List<StatusInfoResponse.StatusInfo>();
    
    public static void ClearDC()
    {
        m_lCritValues.Clear();
        m_UserInfo.ClearUserInfo();
    }
    
    public  static UserInfo GetPlayer()
    {
        return m_UserInfo ;
    }
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
    private static bool SaveData(int  cmdID, object Info)
    {
        switch (cmdID) {
            case (int)gate.Command.CMD.CMD_606:
                Recv_USER_INFO_RESP(Info);
                break;
            case (int)gate.Command.CMD.CMD_610:
                Recv_NameResponse(Info);
                break;
            case (int)gate.Command.CMD.CMD_620:
                Recv_BuyPhysicalResponse(Info);
                break;
            case (int)gate.Command.CMD.CMD_624:
                BuySkillPointResponse(Info);
                break;
            case (int)gate.Command.CMD.CMD_626:
                Recv_UserTimes_RESP(Info);
                break;
            case (int)gate.Command.CMD.CMD_628:
                Recv_EMoneyBuyResource_RESP(Info);
                break;
            case (int)gate.Command.CMD.CMD_632:
                StatusInfoResponse(Info);
                break;
                
        }
        return true;
    }
    
    /// <summary>
    /// 购买体力
    /// </summary>
    public static bool Send_BuyPhysicalRequest()
    {
        user.BuyPhysicalRequest request = new user.BuyPhysicalRequest();
        return true;
    }
    
    /// <summary>
    ///  角色起名
    /// </summary>
    public static bool Send_RoleNameRequest(string name)
    {
        user.RoleNameRequest roleName = new user.RoleNameRequest();
        roleName.name = name;
        return true;
    }
    /// <summary>
    /// 0605 获取玩家信息请求
    /// </summary>
    /// <param name="info">二进制32位表示，具体参考协议</param>
    public static bool Send_USER_INFO()
    {
        user.UserInfoRequest userInfo = new user.UserInfoRequest();
        return true;
    }
    /// <summary>
    /// 631 获取玩家状态
    /// </summary>
    public static bool SendStatusInfoRequest()
    {
        user.StatusInfoRequest userInfo = new user.StatusInfoRequest();
        return true;
    }
    /// <summary>
    /// 购买技能点数
    /// </summary>
    public  static bool  Send_BuySkillPointRequest()
    {
        user.BuySkillPointRequest request = new user.BuySkillPointRequest();
        return true;
    }
    /// <summary>
    /// 606 获取玩家信息回应
    /// </summary>
    public static bool Recv_USER_INFO_RESP(object Info)
    {
        if (Info == null) {
            return false;
        }
        user.UserInfoResponse user_info = Info as user.UserInfoResponse;
        if (m_UserInfo == null) {
            m_UserInfo = new UserInfo();
        }
        m_UserInfo.SetUserInfo(user_info);
        GlobalTimer.SetServerTime(user_info.login_time);
        GlobalTimer.SetBlackhopCloseTotal(m_UserInfo.BlackShopTime);
        GlobalTimer.SetGnomeShopCloseTotal(m_UserInfo.GnomeShopTime);
        return true;
    }
    /// <summary>
    /// 获取月卡产生状态
    /// </summary>
    private static bool StatusInfoResponse(object Info)
    {
        if (Info == null) {
            return false;
        }
        StatusInfoResponse response = Info as StatusInfoResponse;
        if (response == null) {
            return false;
        }
        m_lStatusInfo = response.infos;
        return true;
    }
    /// <summary>
    /// 625玩家次数请求
    /// </summary>
    public  static bool  Send_UserTimesRequest()
    {
        user.UserTimesRequest request = new user.UserTimesRequest();
        return true;
    }
    /// <summary>
    /// 629签到
    /// </summary>
    public static bool Send_UserSigninRequest()
    {
        user.UserSigninRequest request = new user.UserSigninRequest();
        return true;
    }
    
    
    /// <summary>
    /// 626 更新玩家次数信息
    /// </summary>
    public static bool Recv_UserTimes_RESP(object Info)
    {
        if (Info == null) {
            return false;
        }
        user.UserTimesResponse times_info = Info as user.UserTimesResponse;
        if (m_UserInfo == null) {
            m_UserInfo = new UserInfo();
        }
        m_UserInfo.SetUserTimesInfo(times_info);
        return true;
    }
    /// <summary>
    /// 628 玩家炼金数据返回
    /// </summary>
    public static bool Recv_EMoneyBuyResource_RESP(object Info)
    {
        if (Info == null) {
            return false;
        }
        user.EMoneyBuyResourceResponse ResponInfo = Info as user.EMoneyBuyResourceResponse;
        m_lCritValues.Clear();
        m_lCritValues.AddRange(ResponInfo.crit_values);
        return true;
    }
    
    /// <summary>
    /// 角色起名(改名)
    /// </summary>
    public static bool Recv_NameResponse(object Info)
    {
        if (Info == null) {
            return false;
        }
        user.RoleNameResponse response = Info as user.RoleNameResponse;
        return true;
    }
    /// <summary>
    /// 购买体力回应
    /// </summary>
    public static bool Recv_BuyPhysicalResponse(object Info)
    {
        if (Info == null) {
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// 611更换战队头像, 返回UserInfo CMD_606
    /// </summary>
    public static bool Send_HeadModifyRequest(int headID, int headFrameID, int headChartID)
    {
        user.HeadModifyRequest request = new user.HeadModifyRequest();
        if (headID < 1000000) {
            headID = headID + 1000000;
        }
        if (headFrameID < 2000000) {
            headFrameID = headFrameID + 2000000;
        }
        if (headChartID < 3000000) {
            headChartID = headChartID + 3000000;
        }
        request.headid = headID;
        request.headframeid = headFrameID;
        request.headchartid = headChartID;
        return true;
    }
    
    /// <summary>
    /// 购买技能点
    /// </summary>
    private static bool BuySkillPointResponse(object Info)
    {
        if (Info == null) {
            return false;
        }
        user.BuySkillPointResponse response = Info as user.BuySkillPointResponse;
        return true;
    }
    
    /// <summary>
    /// 检测玩家是否持有 某种 足够的代币
    /// </summary>
    /// <param name="currency">代币类型（1钻石 2金币）</param>
    /// <param name="needNum">消耗量</param>
    public static bool CheckCurrencyEnough(int currency, int needNum)
    {
        switch (currency) {
            case 1:
                return GetDiamond() >= needNum;
            case 2:
                return GetCoin() >= needNum;
            case 3:
                return GetAthleticCoin() >= needNum;
        }
        return false;
    }
    /// <summary>
    /// 获取 购买技能点 次数
    /// </summary>
    public static int GetBuySkillPointTime()
    {
        return m_UserInfo.Skilltimes;
    }
    
    /// <summary>
    /// 获取玩家当前体力值
    /// </summary>
    public static int GetPhysical()
    {
        return m_UserInfo.Physical;
    }
    /// <summary>
    /// 获取玩家上一次体力值
    /// </summary>
    public static int GetPrePhyscial()
    {
        return m_UserInfo.PrePhysical;
    }
    
    public static void SetPrePhyscial(int i)
    {
        m_UserInfo.PrePhysical = i;
    }
    
    /// <summary>
    /// 玩家等级（战队等级）
    /// </summary>
    public static int GetLevel()
    {
        return	m_UserInfo.Level ;
    }
    /// <summary>
    /// 获取 剩余技能点数
    /// </summary>
    public  static int GetLeftSkillPoints()
    {
        return m_UserInfo.SkillPoints;
    }
    
    /// <summary>
    /// 海神币.
    /// </summary>
    public static int GetAthleticCoin()
    {
        return m_UserInfo.AthleticsCoin;
    }
    /// <summary>
    /// 获取玩家金币
    /// </summary>
    /// <returns></returns>
    public static int GetCoin()
    {
        return m_UserInfo.Coin;
    }
    
    
    
    public static void SetDiamond(int diamond)
    {
        m_UserInfo.Diamond = diamond;
    }
    /// <summary>
    /// 设置玩家ID
    /// </summary>
    /// <param name="num"></param>
    public static void  SetUserID(int Uid)
    {
        m_UserInfo.Uid = Uid;
    }
    /// <summary>
    /// 设置玩家ID
    /// </summary>
    public static int  GetUserID()
    {
        return m_UserInfo.Uid;
    }
    /// <summary>
    /// 获取玩家名字
    /// </summary>
    public static string GetName()
    {
        return m_UserInfo.Name;
    }
    
    
    /// <summary>
    /// 获取玩家木材
    /// </summary>
    public static int  GetWood()
    {
        return m_UserInfo.Wood ;
    }
    /// <summary>
    /// 设置玩家水晶
    /// </summary>
    /// <param name="num"></param>
    public static int  GetCrystal()
    {
        return m_UserInfo.Crystal ;
    }
    /// <summary>
    /// 获取玩家钻石
    /// </summary>
    public static int GetDiamond()
    {
        return m_UserInfo.Diamond;
    }
    /// <summary>
    /// 甲板等级
    /// </summary>
    public static int GetDeckLevel()
    {
        return m_UserInfo.Decklevel ;
    }
    /// <summary>
    /// 玩家VIP等级
    /// </summary>
    public static int GetVIPLevel()
    {
        return m_UserInfo.VIPLevel;
    }
    
    public static List<int> GetEMoneyBuyGetList()
    {
        return m_lCritValues;
    }
    
    
    public static void SimulationData()
    {
        m_UserInfo.Level = 10;
    }
}
