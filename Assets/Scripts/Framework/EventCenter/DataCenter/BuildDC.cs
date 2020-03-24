using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic;
using System;
using sdata;
using build;

/// <summary>
/// 陷阱数据相关DC.
/// </summary>
public class BuildDC
{

    private static Dictionary<int, BuildInfo> m_Buildings = new Dictionary<int, BuildInfo>();
    
    /// <summary>
    /// 处理事件.
    /// </summary>
    public static bool ProcessData(int CmdID, int nErrorCode, object Info)
    {
        if (nErrorCode == 0) {
            SaveData(CmdID, Info);
        }
        
        return true;
    }
    /// <summary>
    /// 存储数据，供外部使用.
    /// </summary>
    private static bool SaveData(int cmdID, object Info)
    {
        switch (cmdID) {
            case (int)gate.Command.CMD.CMD_908:
                Recv_RoomInfoResponse(Info);
                break;
            case (int)gate.Command.CMD.CMD_912:
                Recv_BuildComposeResponse(Info);
                break;
                
        }
        
        return true;
    }
    
    
    /// <summary>
    ///  901 陷阱升级请求.
    /// </summary>
    public static bool Send_BuildlevelUpRequest(int buildID)
    {
        BuildLevelUpRequest request = new BuildLevelUpRequest();
        request.buildid = buildID ;
        return true;
    }
    
    
    
    /// <summary>
    ///  903 陷阱升阶请求.
    /// </summary>
    public static bool Send_BuildQualityUpRequest(int buildID)
    {
        BuildQualityUpRequest request = new BuildQualityUpRequest();
        request.buildid = buildID;
        return true;
    }
    
    
    /// <summary>
    ///  905 陷阱升阶请求.
    /// </summary>
    public static bool Send_BuildStarUpRequest(int buildID)
    {
        BuildStarUpRequest request = new BuildStarUpRequest();
        request.buildid = buildID;
        return true;
    }
    /// <summary>
    /// 907 发送 建筑信息（仓库）.
    /// </summary>
    /// <returns></returns>
    public static bool Send_BuildInfoRequest()
    {
        BuildInfoRequest request = new BuildInfoRequest();
        return true;
    }
    
    
    /// <summary>
    /// 909 发送 陷阱拆解.
    /// </summary>
    /// <returns></returns>
    public static bool Send_BuildAnalyzeRequest(int Buildid)
    {
        BuildAnalyzeRequest request = new BuildAnalyzeRequest();
        request.buildid = Buildid ;
        return true;
    }
    /// <summary>
    /// 陷阱合成.
    /// </summary>
    /// <returns><c>true</c>, if build compose request was send_ed, <c>false</c> otherwise.</returns>
    /// <param name="BuildType">Build type.</param>
    public static bool Send_BuildComposeRequest(int BuildType)
    {
        BuildComposeRequest request = new BuildComposeRequest();
        request.buildtype = BuildType ;
        return true;
    }
    
    public static bool Recv_BuildComposeResponse(object Info)
    {
        if (Info == null) {
            return false;
        }
        return true;
        //		BuildComposeResponse response = Info as BuildComposeResponse;
    }
    
    /// <summary>
    ///  908 获取玩家仓库房间.
    /// </summary>
    /// <param name="Info"></param>
    /// <returns></returns>
    public static bool Recv_RoomInfoResponse(object Info)
    {
        if (Info == null) {
            return false;
        }
        BuildInfoResponse response = Info as BuildInfoResponse;
        foreach (build.BuildInfo item in response.build_info) {
            int action = item.action ;
            //add.
            if (action == 0) {
                if (m_Buildings.ContainsKey(item.id) == true) {
                    BuildInfo I = m_Buildings[item.id];
                    buildingM.UpdateBuildInfo(item, ref I);
                } else {
                    BuildInfo  I = buildingM.GetBuildInfo(item);
                    if (I != null) {
                        m_Buildings.Add(item.id, I);
                    }
                }
                
            }
            //update.
            else if (action == 1) {
                if (m_Buildings.ContainsKey(item.id) == true) {
                    BuildInfo I = m_Buildings[item.id];
                    buildingM.UpdateBuildInfo(item, ref I);
                }
            }
            //del.
            else if (action == 2) {
                m_Buildings.Remove(item.id);
            }
        }
        return true;
    }
    
    /// <summary>
    /// 清空数据.
    /// </summary>
    public static void ClearDC()
    {
        m_Buildings.Clear();
    }
    
    public static void SortBuildList(ref List<BuildInfo> list)
    {
        list.Sort((obj1, obj2) => {
            int ret = obj1.Level.CompareTo(obj2.Level);
            if (ret != 0) {
                return ret;
            }
            
            ret = obj1.Quality.CompareTo(obj2.Quality);
            if (ret != 0) {
                return ret;
            }
            
            ret = obj1.StarLevel.CompareTo(obj2.StarLevel);
            if (ret != 0) {
                return ret;
            }
            
            ret = obj1.m_DefensePower.CompareTo(obj2.m_DefensePower);
            if (ret != 0) {
                return ret;
            }
            
            return obj1.ID.CompareTo(obj2.ID);
        }
        );
        
        list.Reverse();
    }
    
    public static void SortCanNotExitBuildList(ref List<BuildInfo> list)
    {
        list.Sort((obj1, obj2) => {
            int have1 = ItemDC.GetItemCount(obj1.fragmentTypeID);
            int have2 = ItemDC.GetItemCount(obj2.fragmentTypeID);
            int ret = have1.CompareTo(have2);
            if (ret != 0) {
                return ret;
            }
            
            return obj2.fragmentTypeID.CompareTo(obj1.fragmentTypeID);
        }
        );
        
        list.Reverse();
    }
    
    /// <summary>
    /// 获取仓库建筑物列表 ，根据属系来搜索.
    /// </summary>
    public static List<BuildInfo> GetBuildingList(AttributeType KindType)
    {
        List<BuildInfo> l = new List<BuildInfo>();
        foreach (BuildInfo Info in m_Buildings.Values) {
            if (Info.CheckAttributeType(KindType) == true) {
                l.Add(Info);
            }
        }
        return l ;
    }
    /// <summary>
    /// 根据type搜索数据.
    /// </summary>
    public static BuildInfo SearchBuilding(int BuildType)
    {
        foreach (BuildInfo Info in m_Buildings.Values) {
            if (Info.BuildType == BuildType) {
                return Info;
            }
        }
        return null;
    }
    /// <summary>
    /// 根据type要数据.
    /// </summary>
    public static BuildInfo GetBuilding(int BuildID)
    {
        if (m_Buildings.ContainsKey(BuildID) == true) {
            return m_Buildings[BuildID] ;
        }
        return null;
    }
    /// <summary>
    /// 获取金库信息.
    /// </summary>
    /// <returns>The vault build info.</returns>
    public static BuildInfo GetVaultBuildInfo()
    {
        BuildInfo info = SearchBuilding(1300);
        return info;
    }
    
    /// <summary>
    /// 是否有陷阱可升星升阶升级.
    /// </summary>
    /// <returns><c>true</c>, if can up was checked, <c>false</c> otherwise.</returns>
    public static bool CheckCanUp()
    {
        foreach (BuildInfo Info in m_Buildings.Values) {
            bool can = CheckCanUp(Info);
            if (can) {
                return true;
            }
        }
        return false;
    }
    
    public static bool CheckCanUpLevel()
    {
        foreach (BuildInfo Info in m_Buildings.Values) {
            CanLevelResult LevResult = buildingM.GetLevelCanUP(Info);
            if (LevResult == CanLevelResult.CanUp) {
                return true;
            }
        }
        return false;
    }
    
    public static bool CheckCanUp(BuildInfo Info)
    {
        CanQualityResult QuaReslut = buildingM.GetQualityCanUP(Info);
        CanLevelResult LevResult = buildingM.GetLevelCanUP(Info);
        CanStarResult StarResult = buildingM.GetCanUpStar(Info);
        
        if (LevResult == CanLevelResult.CanUp || QuaReslut == CanQualityResult.CanUp || StarResult == CanStarResult.CanUp) {
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 模拟数据
    /// </summary>
    public static void SimulationData()
    {
        int id = 1;
        List<BuildInfo> s_allBuild = buildingM.GetAllBuildInfo();
        foreach (BuildInfo item in s_allBuild) {
            if (item.BuildType == 1201 || item.BuildType >= 9990) {
                continue;
            }
            item.ID = id++;
            m_Buildings.Add(item.ID, item);
        }
    }
}
