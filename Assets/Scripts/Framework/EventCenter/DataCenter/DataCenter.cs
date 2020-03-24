using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 消息数据分发中心
/// </summary>
///<anthor>zhulin</anthor>
///<editor>QFord</editor>
public delegate void DataHook(int nErrorCode);

public class DataCenter
{

    /// <summary>
    /// 消息分发数据中心/常用数据
    /// </summary>
    ///
    ///
    private static int s_nHookIndex = 0;
    private static Dictionary<int, DataHook> s_IndexDataHook = new Dictionary<int, DataHook>();
    
    private static Dictionary<int, List<int>> s_DataHook = new Dictionary<int, List<int>>();
    private static Dictionary<int, List<int>> s_DataHookAddTemp = new Dictionary<int, List<int>>();
    private static Dictionary<int, List<int>> s_DataHookDelTemp = new Dictionary<int, List<int>>();
    
    
    public static bool ProcessData(int  CmdID, object Para)
    {
        bool bResult = true;
        if (Para is gate.GateMessage.MessageContent) {
            gate.GateMessage.MessageContent msg = Para as gate.GateMessage.MessageContent;
            int nErrorCode = 0;
            gate.ErrorMessage emsg = msg.error_message;
            if (emsg != null) {
                if (emsg.code > 0) {
                
                    FileLog.ProtocolDump(emsg);
                    nErrorCode = emsg.code;
                    NGUIUtil.DebugLog(string.Format("CmdID={0},errorCode={1},description={2}",
                            CmdID, emsg.code, emsg.description));
                    bResult = false;
                    //return false; //如取名重名也会返回error_message.code>0，所以这里不宜return，否则客户端hook代码不会执行
                }
            }
            
            object data = null;
            if (bResult && msg.network_message != null) { //如果失败了，有可能msg.network_message为null
                data = protobufM.Deserialize(msg.proto_name, msg.network_message);
                
            }
            //dump
            FileLog.ProtocolDumpwrite(CmdID.ToString());
            FileLog.ProtocolDumpwriteXml(CmdID.ToString());
            if (CmdID != 9999) {
                FileLog.ProtocolDump(data);
            }
            
            //统一在外围直接解析，不到具体的DC去
            if (bResult == true) {
                int DcBlock = CmdID / 100;
                switch (DcBlock) {
                    case 2:
                        bResult = SoldierDC.ProcessData(CmdID, nErrorCode, data);
                        break;
                    case 5:
                        bResult = ShipPlanDC.ProcessData(CmdID, nErrorCode, data);
                        break;
                    case 6:
                        bResult = UserDC.ProcessData(CmdID, nErrorCode, data);
                        break;
                    case 7:
                        bResult = StageDC.ProcessData(CmdID, nErrorCode, data);
                        break;
                    case 8:
                        bResult = ItemDC.ProcessData(CmdID, nErrorCode, data);
                        break;
                    case 9:
                        bResult  = BuildDC.ProcessData(CmdID, nErrorCode, data);
                        break ;
                    case 11:
                        bResult  = BlackScienceDC.ProcessData(CmdID, nErrorCode, data);
                        break ;
                        
                }
            }
            ProcessRealHooks(CmdID, nErrorCode);
        }
        
        
        return bResult;
        
    }
    
    /// <summary>
    /// 注册通知事件,先注册到缓存
    /// </summary>
    public static void RegisterHooks(int  CmdID, DataHook pf)
    {
        if (pf == null) {
            Debug.Log("注册的Hook 非法");
            return ;
        }
        
        bool bHaveDataHook = false;
        int nAddHookIndex = 0;
        foreach (int nDataHookIndex in s_IndexDataHook.Keys) {
            if (s_IndexDataHook[nDataHookIndex] == pf) {
                bHaveDataHook = true;
                nAddHookIndex = nDataHookIndex;
                break;
            }
            
        }
        if (!bHaveDataHook) {
            s_IndexDataHook[s_nHookIndex] = pf;
            nAddHookIndex = s_nHookIndex;
            s_nHookIndex++;
        }
        
        if (s_DataHookAddTemp == null) {
            s_DataHookAddTemp =  new Dictionary<int, List<int>>();
        }
        
        
        if (!s_DataHookAddTemp.ContainsKey(CmdID)) {
            s_DataHookAddTemp[CmdID] = new List<int>();
        }
        List<int> listHookTemp = s_DataHookAddTemp[CmdID];
        int nCount = listHookTemp.Count;
        bool bHooked = false;
        for (int nCnt = 0; nCnt < nCount; nCnt++) {
            if (listHookTemp[nCnt] == nAddHookIndex) {
                bHooked = true;
                break;
            }
        }
        if (!bHooked) {
            listHookTemp.Add(nAddHookIndex);
        }
    }
    
    
    // 缓存回调列表移到真实回调列表
    static void RegisterHooksTempToRealHooks()
    {
        if (s_DataHook == null) {
            s_DataHook =  new Dictionary<int, List<int>>();
        }
        foreach (int nAddCmdID in s_DataHookAddTemp.Keys) {
            List<int> listHookTemp = s_DataHookAddTemp[nAddCmdID];
            int nTempCount = listHookTemp.Count;
            for (int nCntTemp = 0; nCntTemp < nTempCount; nCntTemp++) {
                int nHookIndex = listHookTemp[nCntTemp];
                if (!s_DataHook.ContainsKey(nAddCmdID)) {
                    s_DataHook[nAddCmdID] = new List<int>();
                }
                List<int> listHook = s_DataHook[nAddCmdID];
                int nCount = listHook.Count;
                bool bHooked = false;
                for (int nCnt = 0; nCnt < nCount; nCnt++) {
                    if (listHook[nCnt] == nHookIndex) {
                        bHooked = true;
                        break;
                    }
                }
                if (!bHooked) {
                    listHook.Add(nHookIndex);
                }
                
            }
        }
        s_DataHookAddTemp.Clear();
    }
    
    // 回调事件
    static void ProcessRealHooks(int  CmdID, int nErrorCode)
    {
        RegisterHooksTempToRealHooks();
        if (s_DataHook.ContainsKey(CmdID)) {
        
            List<int> listHook = s_DataHook[CmdID];
            int nCount = listHook.Count;
            for (int nCnt = 0; nCnt < nCount; nCnt++) {
                int nHookIndex = listHook[nCnt];
                try {
                    if (s_IndexDataHook.ContainsKey(nHookIndex)) {
                        s_IndexDataHook[nHookIndex](nErrorCode);
                    }
                } catch (Exception e) {
                    //当上面s_IndexDataHook回调出现问题时，此回调就会被移除，小心成为坑
                    NGUIUtil.DebugLog(e.Message.ToString()  + "," + CmdID);
                    listHook.Remove(nHookIndex);
                    if (listHook.Count == 0) {
                        s_DataHook.Remove(CmdID);
                        break;
                    }
                }
            }
        }
        AntiRegisterHooksFrDelTemp();
        
    }
    
    /// <summary>
    /// 反注册通知事件
    /// </summary>
    public static void AntiRegisterHooks(int  CmdID, DataHook pfRemove)
    {
        if (pfRemove == null) {
            return ;
        }
        
        bool bHaveDataHook = false;
        int nRemovHookIndex = 0;
        foreach (int nDataHookIndex in s_IndexDataHook.Keys) {
            if (s_IndexDataHook [nDataHookIndex] == pfRemove) {
                bHaveDataHook = true;
                nRemovHookIndex = nDataHookIndex;
                break;
            }
        }
        if (!bHaveDataHook) {
            return;
        }
        s_IndexDataHook.Remove(nRemovHookIndex);
        
        if (s_DataHookDelTemp == null) {
            s_DataHookDelTemp =  new Dictionary<int, List<int>>();
        }
        
        if (!s_DataHookDelTemp.ContainsKey(CmdID)) {
            s_DataHookDelTemp[CmdID] = new List<int>();
        }
        List<int> listHookTemp = s_DataHookDelTemp[CmdID];
        int nCount = listHookTemp.Count;
        bool bHooked = false;
        for (int nCnt = 0; nCnt < nCount; nCnt++) {
            if (listHookTemp[nCnt] == nRemovHookIndex) {
                bHooked = true;
                break;
            }
        }
        if (!bHooked) {
            listHookTemp.Add(nRemovHookIndex);
        }
    }
    public static void AntiRegisterHooksFrDelTemp()
    {
        foreach (int nRemoveCmdID in s_DataHookDelTemp.Keys) {
            List<int> listDelHook =  s_DataHookDelTemp[nRemoveCmdID];
            int nDelCount = listDelHook.Count;
            for (int nDelCnt = 0; nDelCnt < nDelCount; nDelCnt++) {
                int nDelHookIndex = listDelHook[nDelCnt];
                if (s_DataHook.ContainsKey(nRemoveCmdID)) {
                    List<int> listHook =  s_DataHook[nRemoveCmdID];
                    int nCount = listHook.Count;
                    for (int nCnt = 0; nCnt < nCount; nCnt++) {
                        int nHookIndex = listHook[nCnt];
                        if (nHookIndex == nDelHookIndex) {
                            listHook.Remove(nDelHookIndex);
                            break;
                        }
                    }
                    nCount = listHook.Count;
                    if (nCount == 0) {
                        s_DataHook.Remove(nRemoveCmdID);
                    }
                } else {
                    s_DataHook.Remove(nRemoveCmdID);
                }
                if (s_DataHookAddTemp.ContainsKey(nRemoveCmdID)) {
                    List<int> listHookTemp =  s_DataHookAddTemp[nRemoveCmdID];
                    int nCount = listHookTemp.Count;
                    for (int nCnt = 0; nCnt < nCount; nCnt++) {
                        int nHookIndex = listHookTemp[nCnt];
                        if (nHookIndex == nDelHookIndex) {
                            listHookTemp.Remove(nDelHookIndex);
                            break;
                        }
                    }
                    nCount = listHookTemp.Count;
                    if (nCount == 0) {
                        s_DataHookAddTemp.Remove(nRemoveCmdID);
                    }
                } else {
                    s_DataHookAddTemp.Remove(nRemoveCmdID);
                }
            }
        }
        s_DataHookDelTemp.Clear();
        
    }
    
    /// <summary>
    /// 清理数据中心数据
    /// </summary>
    public static void ClearDC()
    {
        UserDC.ClearDC();
        SoldierDC.ClearDC();
        BuildDC.ClearDC();
        ItemDC.ClearDC();
        BlackScienceDC.ClearDC();
        ShipPlanDC.ClearDC();
    }
    
    /// <summary>
    /// 模拟数据接口。
    /// </summary>
    public static void SimulationData()
    {
        UserDC.SimulationData();
        SoldierDC.SimulationData();
        StageDC.SimulationData();
        BuildDC.SimulationData();
        BlackScienceDC.SimulationData();
        ShipPlanDC.SimulationData();
    }
    /// <summary>
    /// 从protobuf 解析静态数据到本地
    /// </summary>
    /// <param name="msg"></param>
    public static void LoadStaticDataToLocal(sdata.StaticDataResponse msg)
    {
        ConfigM.Init(msg);
        buildingM.Init(msg);
        DeckM.Init(msg);
        UserM.Init(msg);
        SoldierM.Init(msg);
        SummonM.Init(msg);
        SkillM.Init(msg);
        StageM.Init(msg);
        RoleNameM.Init(msg);
        ItemM.Init(msg);
        GodSkillM.Init(msg);
        AthleticsM.Init(msg);
        ErnieM.Init(msg);
        FruitionM.Init(msg);
        ShipPlanM.Init(msg);
    }
}
