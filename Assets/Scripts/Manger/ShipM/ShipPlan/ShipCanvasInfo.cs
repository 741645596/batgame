using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MapArea {
    LeftUp      = 0,  //左上区块
    RightDown   = 1,  //右下区块
    LeftDown    = 2,  //左下区块
    RightUp     = 3,  //右上区块
    LeftMiddle  = 4,  //左中
    RightMiddle = 5,  //右中
    UpMiddle    = 6,  //左中
    DownMiddle  = 7,  //右中
    Middle      = 8,  //正中
}

public class ShipCanvasInfo
{

    public int width;//几个层中最大的宽度
    public int height;//
    public int shift_cx;
    public int shift_cy;
    public string map;//地图数据 1有 2楼梯口 0 没有
    public string shape;//1 有房间 0没有
    
    public int skin = 100;
    public int environment = 1;
    public ShipCanvasInfo() {}
    
    public ShipCanvasInfo(battle.DefInfoResponse info)
    {
        map = info.map;
        shape = info.shape;
        width = info.width;
        height = info.height;
        shift_cx = 0;
        shift_cy = 0;
    }
    
    public static bool IsEqual(ShipCanvasInfo a, ShipCanvasInfo b)
    {
        if (a == null || b == null) {
            return false;
        }
        bool isEqual = false;
        if (a.shift_cx == b.shift_cx && a.shift_cy == b.shift_cy) {
            isEqual = true;
        }
        return isEqual;
    }
    
    /// <summary>
    /// 修正shiftx,shifty 的错误
    /// </summary>
    public void CorrectionShiftError(Int2 designMapSize)
    {
        if (shift_cx / MapGrid.m_UnitRoomGridNum + width > designMapSize.Unit) {
            shift_cx = (designMapSize.Unit - width) * MapGrid.m_UnitRoomGridNum ;
        }
        
        if (shift_cy  + height > designMapSize.Layer) {
            shift_cy = designMapSize.Layer - height;
        }
    }
    
    public void Copy(ShipCanvasInfo s)
    {
        this.width = s.width;
        this.height = s.height;
        this.shift_cx = s.shift_cx;
        this.shift_cy = s.shift_cy;
        this.map = s.map;
        this.shape = s.shape;
        this.skin = s.skin;
        this.environment = s.environment;
    }
    public ShipCanvasInfo(sdata.s_countershipcanvasInfo Info)
    {
        this.width = Info.width;
        this.height = Info.height;
        this.map = Info.map;
        this.shape = Info.shape;
        this.skin = Info.skin;
        this.environment = Info.environment;
    }
    public void EmptyMapArea()
    {
        width = 0;
        height = 0;
        shift_cx = 0;
        shift_cy = 0;
        map = "";
        shape = "";
    }
    public void SetMapData(int width, int height, string map, string shape, int skin = 100, int environment = 1)
    {
        this.width = width;
        this.height = height;
        this.map = map;
        this.shape = shape;
        this.skin = skin;
        this.environment = environment;
    }
    public void ResetMapArea()
    {
        width = 0;
        height = 0;
        shift_cx = 0;
        shift_cy = 0;
    }
    /// <summary>
    /// 对楼梯点上层裁剪掉
    /// </summary>
    public void SetStairMap(List<Int2> lStairPoint, XYmode mode)
    {
        if (lStairPoint == null || lStairPoint.Count <= 0) {
            return ;
        }
        List<List<int>> l = GetMap();
        
        foreach (Int2 CutPos in lStairPoint) {
            int unit = CutPos.Unit ;
            int layer = CutPos.Layer;
            if (mode == XYmode.Save2Edit) {
                unit = CutPos.Unit - shift_cx / MapGrid.m_UnitRoomGridNum;
                layer = CutPos.Layer - shift_cy;
            }
            if (l.Count <= layer || layer < 0) {
                return ;
            }
            if (l[layer].Count <= unit || unit < 0) {
                return ;
            }
            if (l[layer][unit] == 1) {
                l[layer][unit] = 2;
            }
        }
        
        SetMap(l);
    }
    public void SetMapArea(int shift_cx, int shift_cy,  int width, int height)
    {
        this.width = width;
        this.height = height;
        this.shift_cx = shift_cx;
        this.shift_cy = shift_cy;
    }
    public Int2 GetMapSize()
    {
        return new Int2(width, height);
    }
    public Int2 GetStart()
    {
        return new Int2(shift_cx, shift_cy);
    }
    /// <summary>
    /// 设置形状
    /// </summary>
    public void SetShape(List<List<int>> lshpae)
    {
        shape = GridToString(lshpae);
    }
    /// <summary>
    /// 获取形状
    /// </summary>
    public List<List<int>> GetShape()
    {
        return StringToGrid(shape);
    }
    
    /// <summary>
    /// 设置地图
    /// </summary>
    public void SetMap(List<List<int>> lmap)
    {
        map = GridToString(lmap);
    }
    /// <summary>
    /// 获取地图
    /// </summary>
    public List<List<int>> GetMap()
    {
        return StringToGrid(map);
    }
    public List<List<int>> StringToGrid(string str)
    {
        List<List<int>> grid = new List<List<int>>();
        if (string.IsNullOrEmpty(str)) {
            return grid;
        }
        string[] rows = str.Split(',');
        for (int j = 0; j < rows.Length; j++) {
            List<int> row = new List<int>();
            for (int k = 0; k < rows[j].Length; k++) {
                row.Add(int.Parse(rows[j].Substring(k, 1)));
            }
            grid.Add(row);
        }
        return grid;
    }
    public string GridToString(List<List<int>> grid)
    {
        string str = "";
        for (int j = 0; j < grid.Count; j++) {
            for (int k = 0; k < grid[j].Count; k++) {
                str += grid[j][k].ToString();
            }
            if (j < (grid.Count - 1)) {
                str += ",";
            }
        }
        return str;
    }
    /// <summary>
    /// 获取指定的房间地图数据
    /// </summary>
    private string GetMapValue(int layer, int Unit)
    {
        string strlayer = NdUtil.GetStrValue(map, layer);
        return strlayer.Substring(Unit, 1);
    }
    List<Int3> m_MapSizeList = new List<Int3>();
    public List<Int3> MapSizeData {
        get{return m_MapSizeList;}
    }
    List<Int2> m_HoleList = new List<Int2>();
    public List<Int2> HoleData {
        get{return m_HoleList;}
    }
    List<Int2> m_AttackList = new List<Int2>();
    public List<Int2> AttackData {
        get{return m_AttackList;}
    }
    public void GetMapData()
    {
        GetMapDataSize();
        GetHoleList();
        GetAttackList();
    }
    /// <summary>
    /// 获取地图大小
    /// </summary>
    private void  GetMapDataSize()
    {
        m_MapSizeList.Clear();
        for (int i = 0; i < height + 1; i++) {
            Int3 g = new Int3(0, 0, 0);
            g.GridStart = 0;
            g.RoomStart = 0;
            g.GridLength = width * MapGrid.m_UnitRoomGridNum  + 1;
            m_MapSizeList.Add(g);
        }
    }
    /// <summary>
    /// 获取洞列表
    /// </summary>
    private void GetHoleList()
    {
        m_HoleList.Clear();
        for (int Layer = 0; Layer < height + 1; Layer++) {
            for (int Unit = 0; Unit < width; Unit ++) {
                string Value = GetMapValue(Layer, Unit);
                if (Value == "0") {
                    List<Int2>l = new List<Int2>();
                    for (int i = 0; i < MapGrid.m_UnitRoomGridNum; i++) {
                        if (i == 0) {
                            if (Unit == 0 || GetMapValue(Layer, Unit - 1) == "0") {
                                l.Add(new Int2(Unit * MapGrid.m_UnitRoomGridNum, Layer));
                            }
                        } else {
                            l.Add(new Int2(Unit * MapGrid.m_UnitRoomGridNum + i, Layer));
                        }
                        /*l.Add(new Int2(Unit * MapGrid.m_UnitRoomGridNum + 2,Layer));
                        l.Add(new Int2(Unit * MapGrid.m_UnitRoomGridNum + 3,Layer));
                        l.Add(new Int2(Unit * MapGrid.m_UnitRoomGridNum + 4,Layer));
                        l.Add(new Int2(Unit * MapGrid.m_UnitRoomGridNum + 5,Layer));*/
                    }
                    if (Unit == width - 1) {
                        l.Add(new Int2(Unit * MapGrid.m_UnitRoomGridNum + MapGrid.m_UnitRoomGridNum, Layer));
                    }
                    m_HoleList.AddRange(l);
                }
            }
        }
    }
    /// <summary>
    /// 获取每层的首个攻击位
    /// </summary>
    private  void GetAttackList()
    {
        m_AttackList.Clear();
        for (int Layer = 0; Layer < height + 1; Layer++) {
            for (int Unit = 0; Unit < width; Unit ++) {
                string Value = GetMapValue(Layer, Unit);
                if (Value != "0") {
                    List<Int2>l = new List<Int2>();
                    l.Add(new Int2(Unit * MapGrid.m_UnitRoomGridNum + Mathf.CeilToInt(MapGrid.m_UnitRoomGridNum / 4.0f), Layer));
                    l.Add(new Int2(Unit * MapGrid.m_UnitRoomGridNum + Mathf.FloorToInt(MapGrid.m_UnitRoomGridNum * 3.0f / 4.0f), Layer));
                    m_AttackList.AddRange(l);
                }
            }
        }
    }
    /// <summary>
    /// 获取地图区块
    /// </summary>
    public void GetMapAreaRoom(MapArea Area, ref Int2 StartPos, ref  List<Int2> lLinkPos)
    {
        if (lLinkPos == null) {
            lLinkPos = new List<Int2>();
        }
        lLinkPos.Clear();
        List<int> lStartShpae = new List<int>();
        int StartLayer = 0;
        List<int> lEndShpae = new List<int>();
        int Endlayer = 0;
        
        switch (Area) {
        
            case MapArea.RightDown: {
                GetDoubleLinePos(0, ref lStartShpae, ref StartLayer, ref lEndShpae, ref Endlayer);
                GetLinkPos(2, ref StartPos, ref lLinkPos, lStartShpae, StartLayer, lEndShpae, Endlayer);
                break;
            }
            case MapArea.LeftDown: {
                GetDoubleLinePos(0, ref lStartShpae, ref StartLayer, ref lEndShpae, ref Endlayer);
                GetLinkPos(0, ref StartPos, ref lLinkPos, lStartShpae, StartLayer, lEndShpae, Endlayer);
                break;
            }
            case MapArea.DownMiddle: {
                GetDoubleLinePos(0, ref lStartShpae, ref StartLayer, ref lEndShpae, ref Endlayer);
                GetLinkPos(1, ref StartPos, ref lLinkPos, lStartShpae, StartLayer, lEndShpae, Endlayer);
                break;
            }
            case MapArea.LeftUp: {
                GetDoubleLinePos(2, ref lStartShpae, ref StartLayer, ref lEndShpae, ref Endlayer);
                GetLinkPos(0, ref StartPos, ref lLinkPos, lStartShpae, StartLayer, lEndShpae, Endlayer);
                break;
            }
            case MapArea.UpMiddle: {
                GetDoubleLinePos(2, ref lStartShpae, ref StartLayer, ref lEndShpae, ref Endlayer);
                GetLinkPos(1, ref StartPos, ref lLinkPos, lStartShpae, StartLayer, lEndShpae, Endlayer);
                break;
            }
            case MapArea.RightUp: {
                GetDoubleLinePos(2, ref lStartShpae, ref StartLayer, ref lEndShpae, ref Endlayer);
                GetLinkPos(2, ref StartPos, ref lLinkPos, lStartShpae, StartLayer, lEndShpae, Endlayer);
                break;
            }
            
            case MapArea.LeftMiddle: {
                GetDoubleLinePos(1, ref lStartShpae, ref StartLayer, ref lEndShpae, ref Endlayer);
                GetLinkPos(0, ref StartPos, ref lLinkPos, lStartShpae, StartLayer, lEndShpae, Endlayer);
                break;
            }
            case MapArea.Middle: {
                GetDoubleLinePos(1, ref lStartShpae, ref StartLayer, ref lEndShpae, ref Endlayer);
                GetLinkPos(1, ref StartPos, ref lLinkPos, lStartShpae, StartLayer, lEndShpae, Endlayer);
                break;
            }
            case MapArea.RightMiddle: {
                GetDoubleLinePos(1, ref lStartShpae, ref StartLayer, ref lEndShpae, ref Endlayer);
                GetLinkPos(2, ref StartPos, ref lLinkPos, lStartShpae, StartLayer, lEndShpae, Endlayer);
                break;
            }
            
            default:
                break;
        }
    }
    /// <summary>
    /// 获取地图中心连接块，上，下，左 ，右4块
    /// </summary>
    public void GetMapCenterAreaRoom(ref  List<List<Int2>> lLinkCentrPos)
    {
        if (lLinkCentrPos == null) {
            lLinkCentrPos = new List<List<Int2>>();
        } else {
            lLinkCentrPos.Clear();
        }
        
        List<Int2> l = new List<Int2>();
        //上
        l = GetCenterUp() ;
        if (l.Count > 0) {
            lLinkCentrPos.Add(l);
        }
        //下
        l = GetCenterDown() ;
        if (l.Count > 0) {
            lLinkCentrPos.Add(l);
        }
        //左
        l = GetCenterLeft() ;
        if (l.Count > 0) {
            lLinkCentrPos.Add(l);
        }
        //右
        l = GetCenterRight() ;
        if (l.Count > 0) {
            lLinkCentrPos.Add(l);
        }
    }
    private List<Int2> GetCenterUp()
    {
        List<Int2> l = new List<Int2>();
        if (height > 2) {
            if (width > 4) {
                if (width == 5) {
                    SetPoint(ref l, height - 1, 2);
                } else {
                    SetPoint(ref l, height - 1, width / 2 - 1);
                    SetPoint(ref l, height - 1, width / 2);
                }
            }
        }
        return l;
    }
    private List<Int2> GetCenterDown()
    {
        List<Int2> l = new List<Int2>();
        if (height > 3) {
            if (width > 4) {
                if (width == 5) {
                    SetPoint(ref l, 0, 2);
                } else {
                    SetPoint(ref l, 0, width / 2 - 1);
                    SetPoint(ref l, 0, width / 2);
                }
            }
        }
        return l;
    }
    private List<Int2> GetCenterLeft()
    {
        List<Int2> l = new List<Int2>();
        if (width == 8) {
            for (int i = 0 ; i < height ; i++) {
                SetPoint(ref l, i, 2);
            }
        }
        return l;
    }
    private List<Int2> GetCenterRight()
    {
        List<Int2> l = new List<Int2>();
        if (width >= 7) {
            for (int i = 0 ; i < height ; i++) {
                SetPoint(ref l, i, width - 3);
            }
        }
        return l;
    }
    private void SetPoint(ref List<Int2> l, int Layer, int unit)
    {
        List<List<int>> lshpae = GetShape();
        List<int> line = lshpae[Layer] ;
        if (line[unit] != 0) {
            l.Add(new Int2(unit, Layer));
        }
    }
    /// <summary>
    /// Gets the link position.
    /// </summary>
    /// <param name="NeedPos">0:左边点，1中间点，2右边点;</param>
    /// <param name="StartPos">>爆炸起点.</param>
    /// <param name="lLinkPos">L >爆炸附属起点.</param>
    /// <param name="lStartShpae">爆炸起点层数据.</param>
    /// <param name="StartLayer">爆炸起点层.</param>
    /// <param name="lEndShpae">爆炸附属层数据.</param>
    /// <param name="Endlayer">爆炸层附属层.</param>
    private void GetLinkPos(int NeedPos, ref Int2 StartPos, ref List<Int2> lLinkPos, List<int>lStartShpae, int StartLayer, List<int>lEndShpae, int Endlaye)
    {
        StartPos.Layer = StartLayer;
        
        GetStartPos(NeedPos, ref StartPos, lStartShpae, StartLayer);
        
        int unit = 0;
        if (NeedPos == 0 || NeedPos == 1) {
            unit =  StartPos.Unit + 1;
            if (unit >= width) {
                unit = width - 1;
            }
            
        } else if (NeedPos == 2) {
            unit =  StartPos.Unit - 1;
            if (unit < 0) {
                unit = 0;
            }
        }
        if (StartLayer != Endlaye) {
            lLinkPos.Add(new Int2(unit, StartLayer));
            lLinkPos.Add(new Int2(unit, Endlaye));
            lLinkPos.Add(new Int2(StartPos.Unit, Endlaye));
        } else {
            lLinkPos.Add(new Int2(unit, StartLayer));
        }
    }
    /// <summary>
    /// Gets the double line position.
    /// </summary>
    /// <param name="Area"> 0：最下两层.1:中间两层 2：最上两层</param>
    /// <param name="lStartShpae">爆炸起点层数据.</param>
    /// <param name="StartLayer">爆炸起点层.</param>
    /// <param name="lEndShpae">爆炸附属层数据.</param>
    /// <param name="Endlayer">爆炸层附属层.</param>
    private void GetDoubleLinePos(int Area, ref List<int>lStartShpae, ref int StartLayer, ref List<int>lEndShpae, ref int Endlayer)
    {
        List<List<int>> lshpae = GetShape();
        if (lStartShpae == null) {
            lStartShpae = new List<int>();
        }
        lStartShpae.Clear();
        if (lEndShpae == null) {
            lEndShpae = new List<int>();
        }
        lEndShpae.Clear();
        
        if (height == 1) {
            Endlayer = StartLayer =  0;
            lStartShpae  =  lshpae[0];
            return;
        } else if (Area == 2) {
            StartLayer =  height - 1;
            Endlayer =  height - 2;
        } else if (Area == 1) {
            StartLayer =  height / 2;
            Endlayer =  height / 2 - 1;
        } else if (Area == 0) {
            StartLayer =  0;
            Endlayer =  1;
        }
        
        lStartShpae = lshpae [StartLayer];
        lEndShpae = lshpae[Endlayer];
    }
    /// <summary>
    /// Gets the start position.
    /// </summary>
    /// <param name="NeedPos">0:左边点，1中间点，2右边点;</param>
    /// <param name="StartPos">>爆炸起点.</param>
    /// <param name="StartLayer">L >爆炸层.</param>
    /// <param name="lStartShpae">爆炸层数据.</param>
    private void GetStartPos(int NeedPos, ref Int2 StartPos, List<int>lStartShpae, int StartLayer)
    {
        List<int>lHaveValue = new List<int>();
        //取出不空的索引值列表。
        
        for (int i = 0; i < lStartShpae.Count; i++) {
            if (lStartShpae[i] > 0) {
                lHaveValue.Add(i);
            }
        }
        
        //根据needpos 到索引列表取值。
        if (NeedPos == 0) {
            StartPos.Unit = lHaveValue[0];
        } else if (NeedPos == 1) {
            //列表中心
            //	int centerIndex = lHaveValue.Count/2 -1  + lHaveValue.Count %2;
            //	StartPos.Unit = lHaveValue[centerIndex];
            //几何中心
            StartPos.Unit = (lHaveValue[0] + lHaveValue[lHaveValue.Count - 1]) / 2 ;
        } else if (NeedPos == 2) {
            StartPos.Unit = lHaveValue[lHaveValue.Count - 1];
        }
    }
}
