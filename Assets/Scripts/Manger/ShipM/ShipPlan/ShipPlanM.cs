using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using sdata;

/// <summary>
/// 炮弹兵数据
/// </summary>
public class ShipPlanM
{


    private static bool _IsLoad = false;
    private static List<StaticShipCanvas> m_lShipCanvasInfo = new List<StaticShipCanvas>();
    public static void Init(object obj)
    {
        if (_IsLoad == true) {
            return;
        }
        System.Diagnostics.Debug.Assert(obj is sdata.StaticDataResponse);
        StaticDataResponse sdrsp = obj as StaticDataResponse;
        InitShipCanvasData(sdrsp.s_shipcanvas_info);
        
        _IsLoad = true;
    }
    static void InitShipCanvasData(List<s_shipcanvasInfo> l)
    {
        m_lShipCanvasInfo.Clear();
        foreach (s_shipcanvasInfo info in l) {
            StaticShipCanvas item = new StaticShipCanvas(info);
            m_lShipCanvasInfo.Add(item);
        }
    }
    public static List<StaticShipCanvas> GetAllShipCanvasInfo()
    {
        return m_lShipCanvasInfo;
    }
    public static StaticShipCanvas GetShipCanvasInfo(int shipCanvasTypeID)
    {
        foreach (StaticShipCanvas info in m_lShipCanvasInfo) {
            if (info.Id == shipCanvasTypeID) {
                return info;
            }
        }
        return null;
    }
    
    public static List<string> ParseShapeData(string str)
    {
        //NGUIUtil.DebugLog("Shape =  "+str);
        List<string> data = new List<string>();
        string[] layerShape = str.Split(',');
        for (int i = layerShape.Length - 1; i >= 0 ; i--) {
            data.Add(FillShape(layerShape[i]));
        }
        return FillData(data);
    }
    private static string FillShape(string s)
    {
        while (s.Length < 8) {
            if (s.Length % 2 == 0) {
                s = s + "0";
            } else {
                s = "0" + s;
            }
            FillShape(s);
        }
        
        return s;
    }
    private static List<string> FillData(List<string> data)
    {
        while (data.Count < 4) {
            data.Insert(0, "00000000");
            FillData(data);
        }
        return data;
    }
    
}
