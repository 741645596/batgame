using System.Collections.Generic;
using canvasedit;
using sdata;
/// <summary>
/// 船只方案DC
/// </summary>
/// <author>zhulin</author>
public class ShipPlanDC
{
    /// <summary>
    /// 船只方案列表
    /// </summary>
    private static Dictionary<int, ShipPlan> m_lShipPlan = new Dictionary<int, ShipPlan>();
    // key:动态id
    // value: 设计图id 关联着表呢。
    // 玩家拥有的设计图纸。
    private static Dictionary<int, StaticShipCanvas> m_lShipDesignInfo = new Dictionary<int, StaticShipCanvas>();
    /// <summary>
    /// 选择的方案
    /// </summary>
    private static ShipPlan m_SelPlan ;
    private static int m_curShipDesignID = 0;
    public static int CurShipDesignID {
        get { return m_curShipDesignID; }
    }
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
            case (int)gate.Command.CMD.CMD_510:
                Recv_CanvasBuildResponse(Info);
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
        foreach (ShipPlan plan in m_lShipPlan.Values) {
            plan.ClearShipPlan();
        }
        m_lShipPlan.Clear();
    }
    /// <summary>
    /// 0509 发送获取方案摆设数据
    /// </summary>
    public static void SendCanvasBuildRequest(List<PlanType> m_lType)
    {
        if (m_lType == null || m_lType.Count < 1) {
            return;
        }
        
        canvasedit.CanvasBuildRequest request = new canvasedit.CanvasBuildRequest();
        
        foreach (PlanType type in m_lType) {
            ShipPlan Plan = GetShipPlan(type);
            if (Plan == null) {
                continue ;
            }
            
            request.canvasids.Add(Plan.ID);
        }
        
        
    }
    /// <summary>
    /// 510 获取方案摆设数据
    /// </summary>
    public static void Recv_CanvasBuildResponse(object Info)
    {
        if (Info == null) {
            return ;
        }
        CanvasBuildResponse res = Info as CanvasBuildResponse;
        foreach (CanvasBuildResponse.CanvasBuildInfo item in res.canvasbuild_info) {
            if (m_lShipPlan.ContainsKey(item.canvasid) == true) {
                ShipPlan P = m_lShipPlan[item.canvasid];
                P.SetPlanShipPut(item);
            }
        }
    }
    /// <summary>
    /// 保存修改画布方案
    /// </summary>
    public static void SaveCanvansInfo()
    {
        m_SelPlan.SaveCanvansInfo();
    }
    
    /// <summary>
    /// 获取当前船只方案。
    /// </summary>
    public static ShipPlan GetCurShipPlan()
    {
        return m_SelPlan;
    }
    /// <summary>
    /// 获取方案，但没有设置为当前方案
    /// </summary>
    public static ShipPlan GetShipPlan(PlanType Type)
    {
        foreach (ShipPlan Plan  in m_lShipPlan.Values) {
            if (Plan.Type == Type) {
                return Plan;
            }
        }
        return null;
    }
    
    /// <summary>
    /// 设置当前方案
    /// </summary>
    public static void SetCurShipPlan(PlanType Type)
    {
        foreach (ShipPlan Plan  in m_lShipPlan.Values) {
            if (Plan.Type == Type) {
                m_SelPlan = Plan;
                m_curShipDesignID = m_SelPlan.ShipDesignId;
                return ;
            }
        }
    }
    /// <summary>
    /// 设置当前编辑船只图纸方案ID.
    /// </summary>
    /// <param name="ShipCanvasTypeId">Ship canvas type identifier.</param>
    public static void SetShipDesignID(int ShipDesignID)
    {
        m_curShipDesignID = ShipDesignID;
        m_SelPlan.ShipDesignId = m_curShipDesignID;
    }
    
    /// <summary>
    /// 获取设计图id
    /// </summary>
    /// <returns></returns>
    public static StaticShipCanvas GetCurShipDesignInfo()
    {
        return GetShipDesignInfo(m_curShipDesignID);
    }
    
    public static List<StaticShipCanvas> GetAllShipDesignList(ShipModemType type)
    {
        Int2 range = ConfigM.GetShipDesignCellRange(type);
        List<StaticShipCanvas> l = new List<StaticShipCanvas>();
        foreach (StaticShipCanvas canvas in m_lShipDesignInfo.Values) {
            if (canvas != null && canvas.Cell > range.Layer && canvas.Cell <= range.Unit) {
                l.Add(canvas);
            }
        }
        return l;
    }
    
    /// <summary>
    /// 获取设计图
    /// </summary>
    /// <param name="DesignID"></param>
    /// <returns></returns>
    public static StaticShipCanvas GetShipDesignInfo(int DesignID)
    {
        if (m_lShipDesignInfo.ContainsKey(DesignID) == true) {
            return m_lShipDesignInfo[DesignID];
        }
        return null;
    }
    
    
    /// <summary>
    /// 获取所有的方案
    /// </summary>
    public static  List<ShipPlan> GetAllShipPlan()
    {
        List<ShipPlan> l = new List<ShipPlan>();
        foreach (ShipPlan p in m_lShipPlan.Values) {
            l.Add(p);
        }
        return l;
    }
    
    /// <summary>
    /// 模拟数据
    /// </summary>
    public static void SimulationData()
    {
        List<StaticShipCanvas>l =  ShipPlanM.GetAllShipCanvasInfo();
        foreach (StaticShipCanvas v in l) {
            m_lShipDesignInfo.Add(v.Id, v);
        }
        //
        ShipPlan p = new ShipPlan();
        p.ID = 1;
        p.bExitCanvas = false;
        p.Name = "我的小船船";
        p.Type = PlanType.Default;
        p.BlackScienceID = 1;
        
        p.Canvans = StageM.GetCounterPartMap(101001);
        p.ShipDesignId = 100005; // 16格大船
        StaticShipCanvas shipInfo = ShipPlanDC.GetShipDesignInfo(p.ShipDesignId);
        if (shipInfo != null) {
            p.Canvans.CorrectionShiftError(new Int2(shipInfo.Width, shipInfo.Height));
        }
        // 船只摆设数据
        List<SoldierInfo> lSoldier = null;
        List<BuildInfo> lBuild = null;
        StageM.GetCounterPartShipPut(101001, ref lSoldier, ref lBuild);
        // 建筑跟楼梯
        foreach (BuildInfo v in lBuild) {
            if (v.BuildType == 1300) {
                BuildInfo gold = BuildDC.GetVaultBuildInfo();
                ShipPutInfo Info = new ShipPutInfo();
                Info.id = ShipPutInfo.GetNewShipPutId();
                Info.objid = gold.ID;
                Info.type = 1;
                Info.cxMapGrid = v.m_cx;
                Info.cyMapGrid = v.m_cy;
                Info.shipput_data0 = 0;
                Info.shipput_data1 = 0;
                p.AddShipBuildInfo(Info, ShipBuildType.BuildRoom);
            }
            if (v.BuildType == 1201) {
                ShipPutInfo Info = new ShipPutInfo();
                Info.id = ShipPutInfo.GetNewShipPutId();
                Info.objid = v.BuildType;
                Info.type = 3;
                Info.cxMapGrid = v.m_cx;
                Info.cyMapGrid = v.m_cy;
                Info.shipput_data0 = 0;
                Info.shipput_data1 = 0;
                p.AddShipBuildInfo(Info, ShipBuildType.BuildStair);
            }
        }
        
        m_lShipPlan.Add(p.ID, p);
        m_SelPlan = p;
    }
    
}
public class StaticShipCanvas
{
    public int Id;
    public int Width;
    public int Height;
    public string Shape;
    public string Name;
    public int StarLevel;
    public int Quality;
    public int Cell;
    //这个值特定只有在设计图背包中使用.
    public int ShipDesignID;
    public StaticShipCanvas(s_shipcanvasInfo info)
    {
        this.Id = info.id;
        this.Width = info.width;
        this.Height = info.height;
        this.Shape = info.shape;
        this.Name = info.name;
        this.StarLevel = info.starlevel;
        this.Quality = info.quality;
        this.Cell = info.cell;
    }
}
