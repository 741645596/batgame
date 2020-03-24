using System.Collections.Generic;
using sdata;

public class ShipDesignDC
{
    private static List<s_shipcanvasInfo> m_sShipCabvasInfo = new List<s_shipcanvasInfo>();
    
    public static void ClearDC()
    {
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
    private static bool SaveData(int  CmdID, object Info)
    {
        switch (CmdID) {
            case (int)gate.Command.CMD.CMD_1418:
                Rec_RankChangeRespone(Info);
                
                break;
        }
        return true;
    }
    
    
    public static void Rec_RankChangeRespone(object Info)
    {
        if (Info == null) {
            return;
        }
        athletics.AthleticsRankingResponse  response = Info as athletics.AthleticsRankingResponse ;
    }
    
}
