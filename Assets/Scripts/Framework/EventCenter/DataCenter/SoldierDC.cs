using System.Collections.Generic;

public class SoldierDC
{

    /// <summary>
    /// 玩家所有炮弹兵
    /// </summary>
    private static Dictionary<int, SoldierInfo> m_PlayerSoldiers = new Dictionary<int, SoldierInfo>();
    
    static List<int> m_SoldiersLastCombat = new List<int>();
    static int m_CaptainidLastCombat;
    
    /// <summary>
    /// 处理事件
    /// </summary>
    public static bool ProcessData(int CmdID, int nErrorCode, object Info)
    {
        if (nErrorCode == 0) {
            SaveData(CmdID, Info);
        }
        
        return true;
    }
    /// <summary>
    /// 存储数据，供外部使用
    /// </summary>
    private static bool SaveData(int cmdID, object Info)
    {
        switch (cmdID) {
            case (int)gate.Command.CMD.CMD_214:
                Recv_SOLDIER_LASTCOMBAT(Info);
                break;
            default:
                break;
        }
        return true;
    }
    /// <summary>
    /// 清空数据
    /// </summary>
    public static void ClearDC()
    {
        m_PlayerSoldiers.Clear();
    }
    
    
    /// <summary>
    /// 0214 获取上次参战的炮弹兵列表
    /// </summary>
    /// <param name="Info"></param>
    /// <returns></returns>
    static bool Recv_SOLDIER_LASTCOMBAT(object Info)
    {
        if (Info == null) {
            return false;
        }
        soldier.SoldierBattleListResponse response = Info as soldier.SoldierBattleListResponse;
        m_SoldiersLastCombat.AddRange(response.dsoldier_id);
        m_CaptainidLastCombat = response.captainid;
        return true;
    }
    
    public static List<int> GetSoldiersLastCombat()
    {
        return m_SoldiersLastCombat;
    }
    
    public static int GetCaptainLastCombat()
    {
        return m_CaptainidLastCombat;
    }
    
    /// <summary>
    /// 获取玩家炮弹兵
    /// </summary>
    public static void GetSoldiers(ref List<SoldierInfo>l, CombatLoactionType type)
    {
        if (l == null) {
            l = new List<SoldierInfo>();
        }
        l.Clear();
        
        
        
        foreach (SoldierInfo s in m_PlayerSoldiers.Values) {
            int value = (int)type;
            int v = (1 << s.m_loaction) ;
            if ((value & v) == v) {
                l.Add(s);
            }
        }
    }
    /// <summary>
    /// 获取炮弹兵，根据did
    /// </summary>
    public static SoldierInfo GetSoldiers(int ID)
    {
        if (m_PlayerSoldiers == null) {
            return null;
        }
        if (m_PlayerSoldiers.ContainsKey(ID)) {
            return m_PlayerSoldiers[ID];
        }
        return null;
    }
    /// <summary>
    /// 获取炮弹兵，根据SoldierTypeID
    /// </summary>
    public static SoldierInfo GetSoldiersBySoldierType(int SoldierTypeID)
    {
        if (m_PlayerSoldiers == null) {
            return null;
        }
        foreach (SoldierInfo item in m_PlayerSoldiers.Values) {
            if (item.SoldierTypeID == SoldierTypeID) {
                return item;
            }
        }
        return null;
    }
    /// <summary>
    /// 检测是否存在这样的炮弹兵：存在空闲装备位，且装备可以通过合成得到（金币足够）或直接关卡掉落得到
    /// </summary>
    public static bool CheckExistEquipPosNoEquip()
    {
        bool can = false;
        foreach (SoldierInfo item in m_PlayerSoldiers.Values) {
            can = item.CheckExistEquipPosNoEquip();
            if (can) {
                return true;
            }
        }
        return can;
    }
    /// <summary>
    /// 检测是否存在能够进阶的英雄
    /// </summary>
    public static bool CheckExistSoldierJinJie()
    {
        foreach (SoldierInfo info in m_PlayerSoldiers.Values) {
            if (info.CheckSoldierJinJie() == true) {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 是否可以装备.
    /// </summary>
    /// <returns><c>true</c>, if have can equip was checked, <c>false</c> otherwise.</returns>
    public static bool CheckHaveCanEquip()
    {
        bool can = false;
        foreach (SoldierInfo item in m_PlayerSoldiers.Values) {
            can = item.CheckEquipCanPut();
            if (can) {
                return true;
            }
        }
        return can;
    }
    /// <summary>
    /// 是否有可升级技能的炮弹兵
    /// </summary>
    /// <returns></returns>
    public static bool CheckHaveSkillUp()
    {
        foreach (SoldierInfo item in m_PlayerSoldiers.Values) {
            if (item.CheckHaveSkillUp() == true) {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 0201 获取炮弹兵数据
    /// </summary>
    /// <returns></returns>
    public static bool Send_QUERY_SOLDIER()
    {
        soldier.SoldierInfoRequest soldierInfo = new soldier.SoldierInfoRequest();
        return true;
    }
    
    /// <summary>
    ///  炮弹兵召唤
    /// </summary>
    public static bool Send_SoldierSummonRequest(int soldierID)
    {
        soldier.SoldierSummonRequest soldierInfo = new soldier.SoldierSummonRequest();
        soldierInfo.soldiertypeid = soldierID;
        return true;
    }
    /// <summary>
    /// 炮弹兵 穿 装备
    /// </summary>
    /// <param name="soldier_id"></param>
    /// <param name="item_id">物品ID</param>
    /// <param name="equip_pos"></param>
    /// <returns></returns>
    public static bool Send_SoldierEquipRequest(int soldier_id, int equip_pos)
    {
        soldier.SoldierEquipRequest request = new soldier.SoldierEquipRequest();
        request.soldier_id = soldier_id;
        request.equip_pos = equip_pos;
        return true;
    }
    /// <summary>
    /// 炮弹兵升星
    /// </summary>
    public static bool Send_SoldierStarUpRequest(int dsoldier_id)
    {
        soldier.SoldierStarUpRequest request = new soldier.SoldierStarUpRequest();
        request.dsoldier_id = dsoldier_id;
        return true;
    }
    /// <summary>
    /// 209 炮弹兵升阶（穿满装备 进阶）
    /// </summary>
    /// <param name="d_soldierID"></param>
    /// <returns></returns>
    public static bool Send_SoldierQualityUpRequest(int d_soldierID)
    {
        soldier.SoldierQualityUpRequest request = new soldier.SoldierQualityUpRequest();
        request.soldier_id = d_soldierID;
        return true;
    }
    /// <summary>
    /// 211  炮弹兵技能升级
    /// </summary>
    /// <param name="d_soldierID"></param>
    /// <param name="skillNum">技能编号。炮战技能为0。1~8对应skill1~skill8</param>
    /// <returns></returns>
    public static bool Send_SoldierSkillUpRequest(int d_soldierID, int skillNum)
    {
        soldier.SoldierSkillUpRequest request = new soldier.SoldierSkillUpRequest();
        request.soldier_id = d_soldierID;
        request.skill_no = skillNum;
        return true;
    }
    
    /// <summary>
    /// 213  获取上次战斗选兵列表
    /// </summary>
    /// <param name="battletype">关卡类型</param>
    /// <returns></returns>
    public static bool Send_SoldierBattleListRequest(int battletype)
    {
        m_SoldiersLastCombat.Clear();
        soldier.SoldierBattleListRequest request = new soldier.SoldierBattleListRequest();
        request.battletype = battletype;
        return true;
    }
    
    /// <summary>
    /// 获取炮弹兵数量
    /// </summary>
    /// <returns></returns>
    public static int GetSoldierNum()
    {
        return m_PlayerSoldiers.Count ;
    }
    
    
    /// <summary>
    /// 模拟数据
    /// </summary>
    public static void SimulationData()
    {
        m_PlayerSoldiers.Clear();
        List<SoldierInfo> ls = SoldierM.GetAllSoldier();
        for (int i = 0; i < ls.Count; i++) {
            m_PlayerSoldiers.Add(ls[i].ID, ls[i]);
        }
    }
    
}
