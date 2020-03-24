using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using sdata;
using canvasedit;

public enum XYmode {
    Edit2Save  = 0,
    Save2Edit  = 1,
}

public enum PlanType {
    Default = 0, //默认船只方案
}

/// <summary>
/// 船只方案
/// </summary>
/// <author>zhulin</author>
public class ShipPlan
{
    /// <summary>
    /// 方案ID
    /// </summary>
    private int m_ID;
    public int ID {
        get { return m_ID; }
        set { m_ID = value; }
    }
    /// <summary>
    /// 方案类型
    /// </summary>
    private PlanType m_Type;
    public PlanType Type {
        get { return m_Type; }
        set { m_Type = value; }
    }
    /// <summary>
    /// 方案名称
    /// </summary>
    private string m_Name;
    public string Name {
        get { return m_Name; }
        set { m_Name = value; }
    }
    /// <summary>
    /// 方案战斗力
    /// </summary>
    private int m_CombatPower;
    public int CombatPower {
        get { return GetCurCombat(); }
    }
    /// <summary>
    /// s_shipCabnvasId.
    /// </summary>
    private int m_shipDesignId;
    public int ShipDesignId {
        get { return m_shipDesignId; }
        set { m_shipDesignId = value; }
    }
    /// <summary>
    /// 当前船只编辑所在的模式  编辑或非编辑
    /// </summary>
    private XYmode m_xymode = XYmode.Save2Edit;
    public XYmode Mode {
        get { return m_xymode; }
        set { m_xymode = value; }
    }
    //建筑
    private List<ShipPutInfo> m_RoomBuildings = new List<ShipPutInfo>();
    public List<ShipPutInfo> RoomBuilding {
        get { return m_RoomBuildings; }
        set { m_RoomBuildings = value; }
    }
    //楼梯
    private List<ShipPutInfo> m_StairBuildings = new List<ShipPutInfo>();
    public List<ShipPutInfo> StairBuildings {
        get { return m_StairBuildings; }
        set { m_StairBuildings = value; }
    }
    //炮弹兵，
    private List<ShipPutInfo> m_Soldiers = new List<ShipPutInfo>();
    public List<ShipPutInfo> Soldiers {
        get { return m_Soldiers; }
        set { m_Soldiers = value; }
    }
    //画布
    private ShipCanvasInfo m_Canvans = new ShipCanvasInfo();
    public  ShipCanvasInfo Canvans {
        get{return m_Canvans;}
        set { m_Canvans = value; }
    }
    //黑科技
    private int m_BlackScienceID;
    public int BlackScienceID {
        get{return m_BlackScienceID;}
        set{m_BlackScienceID = value;}
    }
    
    //过滤相同设计图使用.
    private bool m_bExitCanvas = false;
    public bool bExitCanvas {
        get{return m_bExitCanvas;}
        set{m_bExitCanvas = value;}
    }
    
    //保存初始画布等数据
    private  ShipCanvasInfo m_BackupCanvas = new ShipCanvasInfo();
    private  List<ShipPutInfo> m_BackupRoomBuildings = new List<ShipPutInfo>();
    private  List<ShipPutInfo> m_BackupSoldiers = new List<ShipPutInfo>();
    private  List<ShipPutInfo> m_BackupStairBuildings = new List<ShipPutInfo>();
    private int m_BackupBlackScienceID;
    /// <summary>
    /// 清理船只方案
    /// </summary>
    public  void ResetPutID()
    {
        ShipPutInfo.ClearNewShipPutId();
    }
    
    
    /// <summary>
    /// 清理船只方案
    /// </summary>
    public  void ClearShipPlan()
    {
        m_RoomBuildings.Clear();
        m_Soldiers.Clear();
        m_StairBuildings.Clear();
        
        m_BackupRoomBuildings.Clear();
        m_BackupStairBuildings.Clear();
        m_BackupSoldiers.Clear();
        ShipPutInfo.ClearNewShipPutId();
    }
    
    /// <summary>
    /// 设置方案画布
    /// </summary>
    public void SetPlanCanvans(canvasedit.ShipCanvasInfo Info)
    {
        if (Info == null) {
            return ;
        }
        ID = Info.id;
        Name = Info.name ;
        Type = (PlanType) Info.type;
        BlackScienceID = Info.bs_id;
        //
        m_Canvans.width = Info.width;
        m_Canvans.height = Info.height;
        m_Canvans.map = Info.map;
        m_Canvans.shape = Info.shape;
        m_Canvans.shift_cx = Info.shift_cx;
        m_Canvans.shift_cy = Info.shift_cy;
        m_shipDesignId = Info.designid;
        StaticShipCanvas shipInfo = ShipPlanDC.GetShipDesignInfo(m_shipDesignId);
        if (shipInfo != null) {
            m_Canvans.CorrectionShiftError(new Int2(shipInfo.Width, shipInfo.Height));
        }
    }
    
    public void CopyPlan(ShipPlan Info)
    {
        ID = Info.ID;
        Name = Info.Name ;
        Type = Info.Type;
        BlackScienceID = Info.BlackScienceID;
        //
        m_Canvans.width = Info.m_Canvans.width;
        m_Canvans.height = Info.m_Canvans.height;
        m_Canvans.map = Info.m_Canvans.map;
        m_Canvans.shape = Info.m_Canvans.shape;
        m_Canvans.shift_cx = Info.m_Canvans.shift_cx;
        m_Canvans.shift_cy = Info.m_Canvans.shift_cy;
        m_shipDesignId = Info.ShipDesignId;
    }
    public void CopyPlanShipPut(ShipPlan Info)
    {
        m_Canvans.Copy(Info.m_Canvans);
        
        m_Soldiers.Clear();
        foreach (ShipPutInfo item in Info.m_Soldiers) {
            ShipPutInfo newItem = new ShipPutInfo();
            newItem.Copy(item);
            m_Soldiers.Add(newItem);
        }
        m_RoomBuildings.Clear();
        foreach (ShipPutInfo item in Info.m_RoomBuildings) {
            ShipPutInfo newItem = new ShipPutInfo();
            newItem.Copy(item);
            m_RoomBuildings.Add(newItem);
        }
        
        m_StairBuildings.Clear();
        foreach (ShipPutInfo item in Info.m_StairBuildings) {
            ShipPutInfo newItem = new ShipPutInfo();
            newItem.Copy(item);
            m_StairBuildings.Add(newItem);
        }
        
        BlackScienceID = Info.BlackScienceID;
    }
    /// <summary>
    /// 设置方案摆设
    /// </summary>
    public void SetPlanShipPut(CanvasBuildResponse.CanvasBuildInfo item)
    {
        ClearShipPlan();
        if (item == null) {
            return ;
        }
        foreach (canvasedit.ShipPutInfo shipputitem in item.shipput_info) {
            ShipPutInfo s = SaveShipBuildInfo(shipputitem);
            if (s == null) {
                continue;
            }
            AddShipCanvansFromSvr(s);
        }
    }
    /// <summary>
    ///  备份方案
    /// </summary>
    public void BackupPlan()
    {
        m_BackupCanvas.Copy(m_Canvans);
        
        m_BackupSoldiers.Clear();
        foreach (ShipPutInfo item in m_Soldiers) {
            ShipPutInfo newItem = new ShipPutInfo();
            newItem.Copy(item);
            m_BackupSoldiers.Add(newItem);
        }
        m_BackupRoomBuildings.Clear();
        foreach (ShipPutInfo item in m_RoomBuildings) {
            ShipPutInfo newItem = new ShipPutInfo();
            newItem.Copy(item);
            m_BackupRoomBuildings.Add(newItem);
        }
        
        m_BackupStairBuildings.Clear();
        foreach (ShipPutInfo item in m_StairBuildings) {
            ShipPutInfo newItem = new ShipPutInfo();
            newItem.Copy(item);
            m_BackupStairBuildings.Add(newItem);
        }
        
        m_BackupBlackScienceID = BlackScienceID;
    }
    
    /// <summary>
    /// 还原方案
    /// </summary>
    public  void ResetPlan()
    {
    
        m_Canvans.Copy(m_BackupCanvas);
        
        m_Soldiers.Clear();
        foreach (ShipPutInfo item in m_BackupSoldiers) {
            ShipPutInfo newItem = new ShipPutInfo();
            newItem.Copy(item);
            m_Soldiers.Add(newItem);
        }
        
        m_RoomBuildings.Clear();
        foreach (ShipPutInfo item in m_BackupRoomBuildings) {
            ShipPutInfo newItem = new ShipPutInfo();
            newItem.Copy(item);
            m_RoomBuildings.Add(newItem);
        }
        
        m_StairBuildings.Clear();
        foreach (ShipPutInfo item in m_BackupStairBuildings) {
            ShipPutInfo newItem = new ShipPutInfo();
            newItem.Copy(item);
            m_StairBuildings.Add(newItem);
        }
        
        BlackScienceID = m_BackupBlackScienceID;
    }
    
    
    
    
    /// <summary>
    /// 获取<船上或仓库>建筑、士兵和楼梯数据 （最核心的一个接口）
    /// </summary>
    public ShipPutInfo GetShipBuildInfo(CanvasCore Core)
    {
        if (Core == null) {
            return null;
        }
        if (!Core.IsNewCreate) {
            List<ShipPutInfo> l = new List<ShipPutInfo>();
            if (Core.m_type == ShipBuildType.Soldier) {
                l = m_Soldiers;
                for (int i = 0; i < l.Count ; i ++) {
                    if (l[i].id == Core.m_ID) {
                        return l[i];
                    }
                }
            } else if (Core.m_type == ShipBuildType.BuildRoom) {
                l = m_RoomBuildings;
                for (int i = 0; i < l.Count ; i ++) {
                    if (l[i].id == Core.m_ID) {
                        return l[i];
                    }
                }
            } else if (Core.m_type == ShipBuildType.BuildStair) {
                l = m_StairBuildings;
                for (int i = 0; i < l.Count; i++) {
                    if (l[i].id == Core.m_ID) {
                        return l[i];
                    }
                }
            }
        } else {
            if (Core.m_type == ShipBuildType.Soldier) {
                return new ShipPutInfo(Core.m_ID, Core.m_DataID);
            } else if (Core.m_type == ShipBuildType.BuildRoom) {
                List<BuildInfo> l = BuildDC.GetBuildingList(AttributeType.ALL);
                for (int i  = 0; i < l.Count; i++) {
                    if (l[i].ID == Core.m_DataID) {
                        return new ShipPutInfo(Core.m_ID, l[i]);
                    }
                }
            }
        }
        return null;
    }
    
    /// <summary>
    /// 添加 新建 建筑或炮弹兵 到船上。
    /// </summary>
    public void AddShipBuildInfo(ShipPutInfo Info, ShipBuildType Type)
    {
        if (Info == null) {
            return ;
        }
        if (Type == ShipBuildType.Soldier) {
            bool b = false;
            for (int i = 0; i < m_Soldiers.Count ; i ++) {
                if (m_Soldiers[i].id == Info.id) {
                    b = true;
                    break;
                }
            }
            if (b == false) {
                m_Soldiers.Add(Info);
            }
            
        } else if (Type == ShipBuildType.BuildRoom) {
            bool b = false;
            for (int i = 0; i < m_RoomBuildings.Count ; i ++) {
                if (m_RoomBuildings[i].id == Info.id) {
                    b = true;
                    break;
                }
            }
            if (b == false) {
                m_RoomBuildings.Add(Info);
            }
        } else if (Type == ShipBuildType.BuildStair) {
            bool b = false;
            for (int i = 0; i < m_StairBuildings.Count; i++) {
                if (m_StairBuildings[i].id == Info.id) {
                    b = true;
                    break;
                }
            }
            if (b == false) {
                m_StairBuildings.Add(Info);
            }
        }
    }
    /// <summary>
    /// 移除建筑物，如果是新增的可以直接移除，否则需等服务端通过才移除
    /// </summary>
    public void RemoveShipBuildInfo(ShipPutInfo Info, ShipBuildType Type)
    {
        if (Info == null) {
            return ;
        }
        if (Type == ShipBuildType.Soldier) {
            if (m_Soldiers.Contains(Info) == true) {
                m_Soldiers.Remove(Info);
            }
        } else if (Type == ShipBuildType.BuildRoom) {
            if (m_RoomBuildings.Contains(Info) == true) {
                m_RoomBuildings.Remove(Info);
            }
        } else if (Type == ShipBuildType.BuildStair) {
            if (m_StairBuildings.Contains(Info) == true) {
                m_StairBuildings.Remove(Info);
            }
        }
    }
    
    /// <summary>
    /// 获取船上数据
    /// </summary>
    public List<ShipPutInfo> GetShipBuildInfo(ShipBuildType type)
    {
        List<ShipPutInfo> l = new List<ShipPutInfo>();
        if (type == ShipBuildType.BuildRoom || type == ShipBuildType.All) {
            if (m_RoomBuildings.Count == 0) { //当没有金库时，添加一个
                BuildInfo info = BuildDC.SearchBuilding(1300);
                if (info != null) {
                    ShipPutInfo Build = new ShipPutInfo(ShipPutInfo.GetNewShipPutId(), info);
                    Build.cyMapGrid = 0;
                    Build.cxMapGrid = 24;
                    AddShipBuildInfo(Build, ShipBuildType.BuildRoom);
                }
            }
            l.AddRange(m_RoomBuildings);
        }
        
        if (type == ShipBuildType.Soldier || type == ShipBuildType.All) {
            l.AddRange(m_Soldiers);
        }
        
        if (type == ShipBuildType.BuildStair || type == ShipBuildType.All) {
            l.AddRange(m_StairBuildings);
        }
        
        return l;
    }
    
    /// <summary>
    /// 确认炮弹兵数量超量
    /// </summary>
    public bool CheckSoldierUpMaxCount()
    {
        if (m_Soldiers.Count >= 5) {
            return true;
        } else {
            return false;
        }
    }
    
    /// <summary>
    /// 服务端摆放信息归类到到具体类型类型（房间，陷阱，炮弹兵）
    /// </summary>
    private  void AddShipCanvansFromSvr(ShipPutInfo Info)
    {
        if (Info == null) {
            return ;
        }
        if (Info.type ==  1) {
            m_RoomBuildings.Add(Info);
        } else if (Info.type ==  2) {
            m_Soldiers.Add(Info);
        } else if (Info.type == 3) {
            m_StairBuildings.Add(Info);
        }
    }
    
    /// <summary>
    /// 协议转换成本地结构数据
    /// </summary>
    private  ShipPutInfo SaveShipBuildInfo(canvasedit.ShipPutInfo Info)
    {
        if (Info != null) {
            ShipPutInfo sbi = new ShipPutInfo();
            sbi.id = Info.id;
            //sbi.battlemode = Info.battlemode;
            sbi.objid = Info.objid;
            sbi.type = Info.type;
            sbi.cxMapGrid = Info.cx;
            sbi.cyMapGrid = Info.cy;
            sbi.shipput_data0 = Info.shipput_data0;
            sbi.shipput_data1 = Info.shipput_data1;
            return sbi;
        }
        return null;
    }
    
    public List<canvasedit.ShipPutInfo> GetShipPutInfos()
    {
        List<canvasedit.ShipPutInfo> infos = new List<canvasedit.ShipPutInfo>();
        
        foreach (var item in m_RoomBuildings) {
            infos.Add(FillShipPutInfo(item, ShipBuildType.BuildRoom));
        }
        foreach (var item in m_Soldiers) {
            infos.Add(FillShipPutInfo(item, ShipBuildType.Soldier));
        }
        foreach (var item in m_StairBuildings) {
            infos.Add(FillShipPutInfo(item, ShipBuildType.BuildStair));
        }
        return infos;
    }
    
    private canvasedit.ShipPutInfo FillShipPutInfo(ShipPutInfo item, ShipBuildType Type)
    {
        canvasedit.ShipPutInfo info = new canvasedit.ShipPutInfo();
        info.id = item.id;
        if (info.id < 0) {
            info.id = 0;
        }
        info.type = item.type;
        info.objid = item.objid;
        if (Type == ShipBuildType.BuildStair) {
            info.objid = 1201 ;
            info.type = 3;
        }
        info.cx = item.cxMapGrid;
        info.cy = item.cyMapGrid;
        if (Type == ShipBuildType.BuildRoom && item.IsTransgateRoom()) {
            item.IsTransgateRoom();
            info.shipput_data0 = item.shipput_data0;
            info.shipput_data1 = item.shipput_data1;
        } else {
            info.shipput_data0 = item.shipput_data0;
            info.shipput_data1 = item.shipput_data1;
        }
        return info;
    }
    
    /// <summary>
    /// 保存画布方案
    /// </summary>
    public  void SaveCanvansInfo()
    {
        List<ShipPutInfo> l = GetShipBuildInfo(ShipBuildType.BuildStair);
        List<Int2> lCutMapPoint = new List<Int2> ();
        
        foreach (ShipPutInfo I in l) {
            //裁剪掉上层
            lCutMapPoint.Add(new Int2(I.cxMapGrid / MapGrid.m_UnitRoomGridNum, I.cyMapGrid + 1));
        }
        m_Canvans.SetStairMap(lCutMapPoint, m_xymode);
    }
    
    /// <summary>
    /// 生成地图及地形数据
    /// </summary>
    public  void CreateCanvans()
    {
        CalcMapArea() ;
        ReCalcShipBuildInfoXY(XYmode.Edit2Save) ;
        CalcMapShape();
        CalcMap();
    }
    
    public void DoDeckData()
    {
        CalcMapArea();
        ReCalcShipBuildInfoXY(XYmode.Edit2Save) ;
        CalcMapShape();
        CalcMap();
        ReCalcShipBuildInfoXY(XYmode.Save2Edit) ;
    }
    
    
    public void DoDeckDataStart()
    {
        CalcMapArea();
        ReCalcShipBuildInfoXY(XYmode.Edit2Save) ;
        CalcMapShape();
        CalcMap();
    }
    public void DoDeckDataStartWithOut(ShipPutInfo info)
    {
        CalcMapArea();
        ReCalcShipBuildInfoXY(XYmode.Edit2Save) ;
        CalcMapShapeWithout(info);
        CalcMapWithout(info);
    }
    
    public void DoDeckDataEnd()
    {
        ReCalcShipBuildInfoXY(XYmode.Save2Edit) ;
    }
    
    /// <summary>
    /// 获取地图区域范围。
    /// </summary>
    private void CalcMapArea()
    {
        bool bint = false;
        if (Canvans == null) {
            return ;
        }
        
        Canvans.ResetMapArea();
        List<ShipPutInfo> list = GetShipBuildInfo(ShipBuildType.BuildRoom);
        if (list == null || list.Count == 0) {
            return ;
        }
        
        Int2 Min = Int2.zero;
        Int2 Max = Int2.zero;
        for (int i = 0; i < list.Count; i++) {
            if (list[i].m_DeckRoom == true) {
                continue;
            }
            int MaxX =  list[i].cxMapGrid + list[i].GetPutRoomShape().width * MapGrid.m_UnitRoomGridNum;
            int MaxY =  list[i].cyMapGrid + list[i].GetPutRoomShape().height;
            int MinX =  list[i].cxMapGrid ;
            int MinY =  list[i].cyMapGrid ;
            //求max
            if (bint == false) {
                bint = true;
                Max.Unit =  MaxX ;
                Max.Layer = MaxY ;
                //求min
                Min.Unit =  MinX ;
                Min.Layer = MinY ;
            } else {
                Max.Unit = (MaxX > Max.Unit) ? MaxX : Max.Unit;
                Max.Layer = (MaxY > Max.Layer) ? MaxY : Max.Layer;
                //求min
                Min.Unit = (MinX < Min.Unit) ? MinX : Min.Unit;
                Min.Layer = (MinY < Min.Layer) ? MinY : Min.Layer;
            }
        }
        int startX = Min.Unit;
        int startY = Min.Layer;
        int width = Max.Unit - startX;
        int height = Max.Layer - startY;
        Canvans.SetMapArea(startX, startY, width / MapGrid.m_UnitRoomGridNum, height);
    }
    /// <summary>
    /// 修正船只建筑物坐标。生成船只用。
    /// </summary>
    public void ReCalcShipBuildInfoXY(XYmode mode)
    {
        Mode = mode;
        
        
        int value = 1;
        if (mode == XYmode.Edit2Save) {
            value = -1;
        } else {
            value =  1;
        }
        
        Int2 Start = Canvans.GetStart();
        List<ShipPutInfo> l = GetShipBuildInfo(ShipBuildType.BuildRoom);
        foreach (ShipPutInfo Info in l) {
            Info.LinkTransgatePointRoom(l);
        }
        foreach (ShipPutInfo Info in l) {
            Info.cxMapGrid += Start.Unit * value;
            Info.cyMapGrid += Start.Layer * value;
        }
        
        l = GetShipBuildInfo(ShipBuildType.Soldier);
        foreach (ShipPutInfo Info in l) {
            Info.cxMapGrid += Start.Unit * value;
            Info.cyMapGrid += Start.Layer * value;
        }
        l = GetShipBuildInfo(ShipBuildType.BuildStair);
        foreach (ShipPutInfo Info in l) {
            Info.cxMapGrid += Start.Unit * value;
            Info.cyMapGrid += Start.Layer * value;
        }
        
        l = GetShipBuildInfo(ShipBuildType.BuildRoom);
        foreach (ShipPutInfo Info in l) {
            Info.UpdateTransgateRoomParam(Start.Unit * value, Start.Layer * value);
        }
    }
    /// <summary>
    /// 计算地图形状
    /// </summary>
    private void CalcMapShape()
    {
        Int2 Start = Canvans.GetMapSize();
        List<List<int>> grid = new List<List<int>>();
        int RoomWidth = Start.Unit ;
        for (int i = 0; i < Start.Layer; i++) {
            List<int>  row = new List<int>();
            for (int j = 0; j < RoomWidth ; j ++) {
                row.Add(0);
            }
            grid.Add(row);
        }
        //
        List<ShipPutInfo> l = GetShipBuildInfo(ShipBuildType.BuildRoom);
        for (int i = 0; i < l.Count; i++) {
            if (l[i].m_DeckRoom == true) {
                continue;
            }
            List<Int2> shape = l[i].GetPutRoomGridPos();
            
            foreach (Int2 Pos in shape) {
                grid[Pos.Layer][Pos.Unit ] = 1;
            }
        }
        Canvans.SetShape(grid);
    }
    /// <summary>
    /// 计算地图形状
    /// </summary>
    private void CalcMapShapeWithout(ShipPutInfo info)
    {
        Int2 Start = Canvans.GetMapSize();
        List<List<int>> grid = new List<List<int>>();
        int RoomWidth = Start.Unit ;
        for (int i = 0; i < Start.Layer; i++) {
            List<int>  row = new List<int>();
            for (int j = 0; j < RoomWidth ; j ++) {
                row.Add(0);
            }
            grid.Add(row);
        }
        //
        List<ShipPutInfo> l = GetShipBuildInfo(ShipBuildType.BuildRoom);
        for (int i = 0; i < l.Count; i++) {
            if (l[i] == info) {
                continue;
            }
            if (l[i].m_DeckRoom == true) {
                continue;
            }
            List<Int2> shape = l[i].GetPutRoomGridPos();
            
            foreach (Int2 Pos in shape) {
                grid[Pos.Layer][Pos.Unit ] = 1;
            }
        }
        Canvans.SetShape(grid);
    }
    /// <summary>
    /// 计算地图
    /// </summary>
    private void CalcMapWithout(ShipPutInfo info)
    {
        Int2 Start = Canvans.GetMapSize();
        List<List<int>> grid = new List<List<int>>();
        int RoomWidth = Start.Unit  ;
        for (int i = 0; i < Start.Layer + 1; i++) {
            List<int>  row = new List<int>();
            for (int j = 0; j < RoomWidth ; j ++) {
                row.Add(0);
            }
            grid.Add(row);
        }
        //
        List<ShipPutInfo> l = GetShipBuildInfo(ShipBuildType.BuildRoom);
        for (int i = 0; i < l.Count; i++) {
            if (l[i] == info) {
                continue;
            }
            if (l[i].m_DeckRoom == true) {
                continue;
            }
            List<Int2> map = l[i].GetPuRoomMapData();
            foreach (Int2 Pos in map) {
                grid[Pos.Layer][Pos.Unit ] = 1;
            }
        }
        Canvans.SetMap(grid);
    }
    /// <summary>
    /// 计算地图
    /// </summary>
    private void CalcMap()
    {
        Int2 Start = Canvans.GetMapSize();
        List<List<int>> grid = new List<List<int>>();
        int RoomWidth = Start.Unit  ;
        for (int i = 0; i < Start.Layer + 1; i++) {
            List<int>  row = new List<int>();
            for (int j = 0; j < RoomWidth ; j ++) {
                row.Add(0);
            }
            grid.Add(row);
        }
        //
        List<ShipPutInfo> l = GetShipBuildInfo(ShipBuildType.BuildRoom);
        for (int i = 0; i < l.Count; i++) {
            if (l[i].m_DeckRoom == true) {
                continue;
            }
            List<Int2> map = l[i].GetPuRoomMapData();
            foreach (Int2 Pos in map) {
                grid[Pos.Layer][Pos.Unit ] = 1;
            }
        }
        Canvans.SetMap(grid);
    }
    
    private static  bool  CheckShipPutInfoEqual(List<ShipPutInfo> a, List<ShipPutInfo> b)
    {
        bool result = true;
        if (a.Count != b.Count) {
            return false;
        }
        if (a.Count == 0 && b.Count == 0) {
            return true;
        }
        foreach (ShipPutInfo item in a) {
            if (!IsInShipPutInfoList(b, item)) {
                result = false;
                break;
            }
        }
        return result;
    }
    private static bool IsInShipPutInfoList(List<ShipPutInfo> a, ShipPutInfo b)
    {
        bool result = false;
        foreach (ShipPutInfo item in a) {
            if (ShipPutInfo.IsEqual(item, b)) {
                result = true;
                break;
            }
        }
        return result;
    }
    
    public  bool IsShipEditChange()
    {
        bool result = false;
        if (!(CheckShipPutInfoEqual(m_BackupRoomBuildings, m_RoomBuildings))) {
            return true;
        }
        if (!(CheckShipPutInfoEqual(m_BackupSoldiers, m_Soldiers))) {
            return true;
        }
        if (!(CheckShipPutInfoEqual(m_BackupStairBuildings, m_StairBuildings))) {
            return true;
        }
        return result;
    }
    /// <summary>
    /// 获取选择的战斗方案
    /// </summary>
    public  void GetShipCansInfoPlan(ref ShipCanvasInfo CanvasInfo, ref List<SoldierInfo> lSoldier, ref List<ShipPutInfo> lBuild)
    {
        if (lSoldier == null) {
            lSoldier = new List<SoldierInfo>();
        }
        if (lBuild == null) {
            lBuild = new List<ShipPutInfo>();
        }
        
        CanvasInfo = Canvans;
        
        foreach (ShipPutInfo Info in m_Soldiers) {
            SoldierInfo s = new SoldierInfo();
            if (Info.GetSoldier(ref s) == true && s != null) {
                lSoldier.Add(s);
            }
        }
        
        lBuild.AddRange(m_RoomBuildings);
        lBuild.AddRange(m_StairBuildings);
    }
    /// <summary>
    /// 获取当前方案战斗力
    /// </summary>
    private  int GetCurCombat()
    {
    
        List<SoldierInfo> lSoldier = new List<SoldierInfo>();
        List<BuildInfo> lBuild = new List<BuildInfo>();
        List<BuildInfo> lStair = new List<BuildInfo>();
        
        GetShipLifeObj(ref lSoldier, ref lBuild, ref lStair);
        
        int CombatPower = 0;
        foreach (SoldierInfo s in lSoldier) {
            CombatPower += s.m_combat_power;
        }
        
        foreach (BuildInfo b in lBuild) {
            CombatPower += b.m_DefensePower;
        }
        
        return CombatPower ;
    }
    /// <summary>
    /// 获取船上数据
    /// </summary>
    public  void GetShipLifeObj(ref List<SoldierInfo> lSoldier, ref List<BuildInfo> lBuild, ref List<BuildInfo> lStair)
    {
        if (lSoldier == null) {
            lSoldier = new List<SoldierInfo>();
        }
        lSoldier.Clear();
        
        if (lBuild == null) {
            lBuild = new List<BuildInfo>();
        }
        lBuild.Clear();
        
        if (lStair == null) {
            lStair = new List<BuildInfo>();
        }
        lStair.Clear();
        
        if (m_RoomBuildings.Count == 0) { //当没有金库时，添加一个
            BuildInfo info = BuildDC.SearchBuilding(1300);
            if (info != null) {
                ShipPutInfo Build = new ShipPutInfo(ShipPutInfo.GetNewShipPutId(), info);
                Build.cyMapGrid = 0;
                Build.cxMapGrid = 24;
                AddShipBuildInfo(Build, ShipBuildType.BuildRoom);
                lBuild.Add(info);
            }
        } else {
            //建筑
            foreach (ShipPutInfo r in m_RoomBuildings) {
                BuildInfo info = r.GetBuildInfo();
                if (info != null) {
                    lBuild.Add(r.GetBuildInfo());
                }
            }
            //炮弹兵
            foreach (ShipPutInfo r in m_Soldiers) {
                SoldierInfo s = new SoldierInfo();
                if (r.GetSoldier(ref s) == true) {
                    lSoldier.Add(s);
                }
            }
            //楼梯
            foreach (ShipPutInfo r in m_StairBuildings) {
                lStair.Add(r.GetBuildInfo());
            }
        }
    }
    /// <summary>
    /// 判断是否包含指定的陷阱
    /// </summary>
    public bool CheckHaveTrap(int buildID)
    {
        foreach (ShipPutInfo Info in m_RoomBuildings) {
            if (Info.objid == buildID) {
                return true;
            }
        }
        return false;
    }
    
}
