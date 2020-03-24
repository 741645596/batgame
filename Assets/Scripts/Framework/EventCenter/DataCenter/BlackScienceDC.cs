using System.Collections.Generic;

public class BlackScienceDC
{

    private static Dictionary<int, CaptionInfo> m_Captions = new Dictionary<int, CaptionInfo>();
    
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
            case (int)gate.Command.CMD.CMD_1102:
                Recv_CAPTAIN_LEVEL_UP_RESP(Info);
                break;
            case (int)gate.Command.CMD.CMD_1104:
                Recv_USER_CAPTION_INFO_RESP(Info);
                break;
            case (int)gate.Command.CMD.CMD_1106:
                Recv_CAPTAIN_STAR_UP_RESP(Info);
                break;
                
        }
        
        return true;
    }
    /// <summary>
    /// CMD_1107 召唤黑科技
    /// </summary>
    public static bool SendCaptainActivateRequest(int sCaptainID)
    {
        captain.CaptainActivateRequest request = new captain.CaptainActivateRequest();
        request.scaptainid = sCaptainID;
        return true;
    }
    
    /// <summary>
    /// 1101 黑科技升级
    /// </summary>
    public static bool Send_CAPTAIN_LEVEL_UP(int id)
    {
        captain.CaptainLevelupRequest clur = new captain.CaptainLevelupRequest();
        clur.dcaptainid = id;
        return true;
    }
    /// <summary>
    /// 1102 0622 回应黑科技升级数据
    /// </summary>
    public static bool Recv_CAPTAIN_LEVEL_UP_RESP(object Info)
    {
        if (Info == null) {
            return false;
        }
        captain.CaptainLevelupResponse clur = Info as captain.CaptainLevelupResponse;
        return true;
    }
    
    /// <summary>
    /// 1103 获取玩家船长信息请求
    /// </summary>
    public static bool Send_USER_CAPTION_INFO(List<int> ids)
    {
        ClearDC();
        captain.CaptainInfoRequest userInfo = new captain.CaptainInfoRequest();
        if (ids != null) {
            for (int i = 0; i < ids.Count; i++) {
                userInfo.id.Add(ids[i]);
            }
        }
        return true;
    }
    
    /// <summary>
    /// 1104 0616 回应船长数据
    /// </summary>
    public static bool Recv_USER_CAPTION_INFO_RESP(object Info)
    {
        if (Info == null) {
            return false;
        }
        captain.CaptainInfoResponse sir = Info as captain.CaptainInfoResponse;
        foreach (captain.CaptainInfo I in  sir.captain_infos) {
            //添加
            int action = I.action;
            int id = I.captainid;
            if (action == 0) {
                bool ishave = false;
                foreach (CaptionInfo c in m_Captions.Values) {
                    if (c.m_id == I.id) {
                        ishave = true;
                        c.SetCaption(I);
                        break;
                    }
                }
                
                if (ishave == false) {
                    CaptionInfo capinfo = new CaptionInfo();
                    GodSkillM.GetCaption(I.captainid, ref capinfo);
                    capinfo.SetCaption(I);
                    m_Captions.Add(id, capinfo);
                }
            } else if (action == 1) { //update
                foreach (CaptionInfo c in m_Captions.Values) {
                    if (c.m_id == I.id) {
                        c.SetCaption(I);
                        break;
                    }
                }
            } else if (action == 2) {
            
                foreach (CaptionInfo c in m_Captions.Values) {
                    if (c.m_id == I.id) {
                        m_Captions.Remove(c.m_captionid);
                        break;
                    }
                }
            }
        }
        return true;
    }
    /// <summary>
    /// 1105 黑科技升星
    /// </summary>
    public static bool Send_CAPTAIN_STAR_UP(int id)
    {
        captain.CaptainStarupRequest clur = new captain.CaptainStarupRequest();
        clur.dcaptainid = id;
        return true;
    }
    /// <summary>
    /// 1102 0622 回应黑科技升级数据
    /// </summary>
    public static bool Recv_CAPTAIN_STAR_UP_RESP(object Info)
    {
        if (Info == null) {
            return false;
        }
        captain.CaptainStarupResponse clur = Info as captain.CaptainStarupResponse;
        return true;
    }
    
    /// <summary>
    /// 清空数据
    /// </summary>
    public static void ClearDC()
    {
        m_Captions.Clear();
    }
    
    /// <summary>
    /// 获取所有船长列表
    /// </summary>
    public static void GetCaptions(ref List<CaptionInfo>l, int  level = -1)
    {
        if (l == null) {
            l = new List<CaptionInfo>();
        }
        l.Clear();
        
        foreach (CaptionInfo s in m_Captions.Values) {
            if (level != -1) {
                if (level >= s.m_levelneed) {
                    l.Add(s);
                }
                continue;
            }
            l.Add(s);
        }
    }
    
    public static void GetExistBS(ref List<CaptionInfo>l)
    {
        if (l == null) {
            l = new List<CaptionInfo>();
        }
        l.Clear();
        foreach (CaptionInfo s in m_Captions.Values) {
            l.Add(s);
        }
    }
    
    /// <summary>
    /// 获取有碎片（尚未召唤）黑科技
    /// </summary>
    public static void GetHaveFragmentBS(ref List<CaptionInfo> l)
    {
        if (l == null) {
            l = new List<CaptionInfo>();
        }
        l.Clear();
        
        l = GodSkillM.GetCaptions();
        for (int i = l.Count - 1; i >= 0; i--) {
            CaptionInfo info = l[i];
            int itemtype = info.GetGodSkillInfo().m_needbook;
            int have = ItemDC.GetItemCount(itemtype);
            if (CheckHaveBS(info.m_captionid) || have == 0) {
                l.RemoveAt(i);
            }
        }
    }
    
    /// <summary>
    /// 获取船长(静态id) 1 / 2...
    /// </summary>
    public static CaptionInfo GetCaption(int id)
    {
        if (m_Captions.ContainsKey(id) == true) {
            return m_Captions[id];
        } else {
            return null;
        }
    }
    /// <summary>
    /// 检测是否已拥有黑科技
    /// </summary>
    public static bool CheckHaveBS(int sCaptainID)
    {
        return m_Captions.ContainsKey(sCaptainID);
    }
    
    /// <summary>
    /// 获取船长(动态id) 1024 / ...
    /// </summary>
    public static CaptionInfo GetCaptionD(int id)
    {
        foreach (var item in m_Captions) {
            if (item.Value.m_id == id) {
                return item.Value;
            }
        }
        return null;
    }
    
    /// <summary>
    /// 判断是否拥有黑科技技能
    /// </summary>
    public static bool CheckHaveCaption()
    {
        if (m_Captions.Count > 0) {
            return true;
        } else {
            return false;
        }
    }
    /// <summary>
    /// 是否有技能可以升星升级.
    /// </summary>
    public static bool CheckCanUp()
    {
        foreach (CaptionInfo info in m_Captions.Values) {
            if (BlackScienceDC.CheckLevUp(info) || BlackScienceDC.CheckStarUp(info)) {
                return true;
            }
        }
        return false;
    }
    
    public static bool CheckLevUp(CaptionInfo info)
    {
        if (info.m_star == ConstantData.MaxStarLevel) {
            return false;
        }
        GodSkillInfo Godskill = info.GetGodSkillInfo();
        if (UserDC.GetLevel() >= info.m_levelneed &&
            Godskill.m_coinneed > 0 && Godskill.m_crystalneed > 0 &&
            UserDC.GetCoin() >= Godskill.m_coinneed &&  UserDC.GetCrystal() >= Godskill.m_crystalneed) {
            return true;
        } else {
            return false;
        }
    }
    public static bool CheckStarUp(CaptionInfo info)
    {
        GodSkillInfo Godskill = info.GetGodSkillInfo();
        int  bookcount = ItemDC.GetItemCount(Godskill.m_needbook);
        if (UserDC.GetLevel() >= info.m_levelneed && Godskill.m_needbooknum > 0 && bookcount >= Godskill.m_needbooknum) {
            return true;
        } else {
            return false;
        }
    }
    
    
    /// <summary>
    /// 模拟数据
    /// </summary>
    public static void SimulationData()
    {
        int id = 1;
        m_Captions.Clear();
        List<CaptionInfo> l = GodSkillM.GetCaptions();
        for (int i = 0; i < l.Count; i++) {
            l[i].m_id = id++;
            m_Captions.Add(l[i].m_id, l[i]);
        }
    }
}
