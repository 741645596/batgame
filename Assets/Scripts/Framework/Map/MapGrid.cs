
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    #define UNITY_EDITOR_LOG
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// 地图的最小单元
/// </summary>
/// <author>zhulin</author>
public enum GridType {
    GRID_NORMAL =  0x00,  //普通类型
    GRID_WALL   =  0x01,  //墙体
    GRID_HOLE   =  0x02,  //向左可通
    GRID_STAIR  =  0x03,  //楼梯
    GRID_HOLESTAIR = 0X04, //即是楼梯又是洞
};





public enum StationsProp {
    NORMAL    =  0x00,  //正常站位格子
    ATTACK    =  0x01,  //攻击位格子，可以摆兵
};

public enum NearestStationDir {
    NSD_LEFTDOWN = 0,
    NSD_CENTERDOWN,
    NSD_RIGHTDOWN,
    NSD_RIGHTTOP,
    NSD_CENTERTOP,
    NSD_LEFTTOP,
    NSD_END,
};


public class MapGrid  : MapStations
{
    //一个单位房间包含的格子长度数
    public const int m_UnitRoomGridNum = 4;
    //格子列表
    private  static Dictionary<int, Dictionary<int, MapGrid> >s_GridList = new Dictionary<int, Dictionary<int, MapGrid>>();
    //建筑列表,SceneID,Is Build ROOM
    protected List<int>m_lBulid = new List<int> ();
    //墙体列表
    protected List<int>m_lWall = new List<int> ();
    
    public enum _ListState {Unassigned, Open, Close};
    public const float m_width = 0.75f;
    public const float m_heigth = 3.3f;
    public const int   m_Pixel = 75;
    //格子高度方向上的偏离
    private float m_OffPosY = 0;
    public float OffPosY{
        get {return m_OffPosY;}
        set{m_OffPosY = value;}
    }
    
    private bool m_UpHaveHole = false;
    public bool UpHaveHole{
        get {return m_UpHaveHole;}
        set{m_UpHaveHole = value;}
    }
    
    
    private Int2 _GridPos;
    public Int2 GridPos{
        get {return _GridPos;}
        set{_GridPos = value;}
    }
    
    //定义格子属性
    private StationsProp m_PropStations = StationsProp.NORMAL;
    public StationsProp PropStations{
        get {return m_PropStations;}
        set{
            m_PropStations = value;
        }
    }
    
    public Vector3   pos {
        get {
            return new Vector3(m_width * _GridPos.Unit /*+ m_width /2*/,
                m_heigth * _GridPos.Layer + OffPosY,
                0);
        }
        
    }
    public Vector3   Uppos {
        get {return new Vector3(pos.x, pos.y + 2.0f, pos.z);}
    }
    public Vector3   WorldPos {
        get {
            Transform t = BattleEnvironmentM.GetLifeMBornNode(true);
            if (t != null)
            {
                return t.TransformPoint(pos);
            } else
            {
                return Vector3.zero;
            }
        }
    }
    
    
    private GridType gridType = GridType.GRID_NORMAL;
    public GridType Type {
        get { return gridType;}
        set{gridType = value;}
    }
    
    public MapGrid preNode ;
    public bool walkable = true;
    public float scoreG;
    public float scoreH;
    public float scoreF;
    public _ListState listState = _ListState.Unassigned;
    
    //每个地图单元的联通属性
    public MapGrid Left = null;
    public MapGrid Right = null;
    public MapGrid Up = null;
    public MapGrid Down = null;
    //最近的联通点
    public const float fcounstScoreG = 0.5f;
    public MapGrid[] m_NpNearestStation = {null, null, null, null, null, null};
    public float[] m_NfNearestStationscoreG = {0, 0, 0, 0, 0, 0};
    public MapGrid pNearestSpecialStation = null;
    public float fNearestSpecialscoreG;
    
    
    public override void Init()
    {
        m_HoldRole = new RoleGridStations();
        m_TempRole = new RoleGridStations();
        m_Channel = new Channel();
        m_Channel.Init();
    }
    
    //获取洞的最底下一个点
    public static MapGrid GetHoleBottom(MapGrid m)
    {
        if (m == null) {
            return null;
        }
        if (m.Type != GridType.GRID_HOLE  && m.Type != GridType.GRID_HOLESTAIR) {
            return m;
        } else {
            if (m.Down == null) {
                Debug.Log("船型数据异常，请调查原因");
                return null;
            } else {
                return GetHoleBottom(m.Down);
            }
        }
    }
    
    //相邻格子路径合法性判断
    public static bool PathGridLegal(MapGrid start, MapGrid end, float JumpDistance)
    {
        if (start == null || end == null) {
            return false;
        }
        if (start.GridPos.Layer == end.GridPos.Layer) {
            if (start.Type == GridType.GRID_HOLE) {
                return false;
            } else {
                int distance = Mathf.Abs(start.GridPos.Unit - end.GridPos.Unit);
                if (distance >  JumpDistance) {
                    return false;
                } else {
                    return true;
                }
            }
        } else {
            if (start.Type == GridType.GRID_HOLE) {
                MapGrid m = GetHoleBottom(start);
                if (m == end) {
                    return true;
                }
                return false;
            } else if (start.Type == GridType.GRID_STAIR) {
                if (start.Up == end || start.Down == end) {
                    return true;
                } else {
                    return false;
                }
            }
            return true;
        }
        
    }
    public bool HasUpGrid()
    {
        MapGrid g = MapGrid.GetMG(GridPos.Layer - 1, GridPos.Unit);
        return g != null;
    }
    
    //相邻格子路径合法性判断
    public static RoleState GetRoleState(MapGrid start, MapGrid end, ref WalkDir dir, ref int TimeCycle, bool IsHide, ref GridSpace gs)
    {
        dir = WalkDir.WALKSTOP;
        TimeCycle = 1;
        if (start == null || end == null) {
            return RoleState.WALK;
        }
        if (start.GridPos.Layer == end.GridPos.Layer) {
            if (end.GridPos.Unit > start.GridPos.Unit) {
                dir = WalkDir.WALKRIGHT;
            } else {
                dir = WalkDir.WALKLEFT;
            }
            int distance = Mathf.Abs(start.GridPos.Unit - end.GridPos.Unit);
            if (distance  > 1) {
                int min = end.GridPos.Unit > start.GridPos.Unit ? start.GridPos.Unit : end.GridPos.Unit;
                int max = end.GridPos.Unit > start.GridPos.Unit ? end.GridPos.Unit : start.GridPos.Unit;
                
                if (IsHide == true && gs == GridSpace.Space_UP) {
                    if (start.HaveTrap() && !end.HaveTrap()) {
                        gs = GridSpace.Space_DOWN;
                        return RoleState.JUMPDOWN;
                    } else if (HasReverseLongHole(start.GridPos.Layer, min, max) == true) {
                        return RoleState.REVERSEJUMP;
                    } else {
                        return RoleState.WALK;
                    }
                } else if (HasLongHole(start.GridPos.Layer, min, max) == true) {
                    return RoleState.JUMP;
                } else {
                    return RoleState.FALLDOWN;
                }
            } else {
                if (IsHide == true) {
                    if (!start.HaveTrap() && end.HaveTrap() && !end.UpHaveHole && end.HasUpGrid()) {
                        gs = GridSpace.Space_UP;
                        return	RoleState.JUMPUP;
                    } else if (start.HaveTrap() && !end.HaveTrap() && gs == GridSpace.Space_UP) {
                        gs = GridSpace.Space_DOWN;
                        return RoleState.JUMPDOWN;
                    } else {
                        return RoleState.WALK;
                    }
                } else {
                    return RoleState.WALK;
                }
            }
        } else {
            if (start.GridPos.Layer < end.GridPos.Layer && start.Type == GridType.GRID_HOLESTAIR) {
                return RoleState.STAIR;
            } else if (start.Type == GridType.GRID_HOLE || start.Type == GridType.GRID_HOLESTAIR) {
                TimeCycle = start.GridPos.Layer - end.GridPos.Layer;
                return RoleState.FALL;
            } else if (start.Type == GridType.GRID_STAIR) {
                return RoleState.STAIR;
            } else {
                return RoleState.FALLDOWN;
            }
        }
        return RoleState.WALK;
    }
    
    
    public void SetGridType(GridType type)
    {
        this.Type = type;
    }
    
    public void SetWallDeadType(int WallSceneID)
    {
        if (Type == GridType.GRID_WALL) {
            Type = GridType.GRID_NORMAL;
        }
        m_lWall.Remove(WallSceneID);
    }
    
    public void ResetGraph()
    {
        this.listState = MapGrid._ListState.Unassigned;
        this.preNode = null;
        this.scoreF = 0;
        this.scoreG = 0;
        this.scoreH = 0;
        this.pNearestSpecialStation = null;
        this.fNearestSpecialscoreG = 0;
    }
    
    
    //获取最大拥挤数
    public  override int GetMaxJam()
    {
        return 1;
    }
    
    
    
    public  override int AskForStaionsDeep(int StaionsDeep, LifeMCamp Camp, ref int ret)
    {
        ret = 0;
        if (PropStations != StationsProp.ATTACK) {
            return -1;
        }
        
        if (m_HoldRole.AntiCamp(Camp) == true
            || m_TempRole.AntiCamp(Camp) == true) {
            ret = 2;
            return -1;
        }
        
        
        
        if (m_Channel.GetIdleChanel(ref StaionsDeep)) {
            return StaionsDeep;
        } else {
            ret = 1;
            return -1;
        }
    }
    
    //获取当前格子阵营
    public bool GetGridCamp(LifeMCamp Camp)
    {
        LifeMCamp c = LifeMCamp.NONE;
        DIR dir = DIR.NONE;
        if (m_HoldRole.AntiCamp(Camp) == true) {
            return false;
        }
        //
        if (m_TempRole.AntiCamp(Camp) == true) {
            return false;
        }
        return true;
    }
    
    public override string GetStationInfo()
    {
        return GridPos.ToString();
    }
    
    //获取深度
    public override int GetRankDeep(int SceneID)
    {
        StationsInfo Info = m_HoldRole.GetRoleStationsInfo(SceneID);
        if (Info != null) {
            return Info.m_StaionsDeep;
        }
        
        Info = m_TempRole.GetRoleStationsInfo(SceneID);
        if (Info != null) {
            return Info.m_StaionsDeep;
        }
        return  -1;
    }
    
    
    
    public override Int2 GetStationsPos()
    {
        return GridPos;
    }
    
    
    public int GetWallID()
    {
        if (m_lWall.Count > 0) {
            return m_lWall[0];
        } else {
            return -1;
        }
    }
    
    
    /// <summary>
    /// 格子中是否包含指定建筑分类类型
    /// </summary>
    public bool HaveTrap()
    {
        if (m_lBulid.Count > 0) {
            return true;
        }
        return false;
    }
    
    
    
    public void HasRole(ref List<string> AttackList, ref List<string> DefenseList)
    {
        List<int> Hold = new List<int>();
        List<int> Temp = new List<int>();
        GetRoleList(ref Hold, ref Temp);
        if (Hold.Count + Temp.Count <= 0) {
            return ;
        }
        foreach (int key  in Hold) {
            Life life = CM.GetLifeM(key, LifeMType.SOLDIER);
            if (life == null || life.m_Core == null) {
                continue ;
            }
            if (life.m_Core.m_Camp == LifeMCamp.ATTACK) {
                AttackList.Add(/*"正式：" + */life.m_Attr.AttrName + ":" + (life as Role).RankDeep);
            } else {
                DefenseList.Add(/*"正式：" + */life.m_Attr.AttrName + ":" + (life as Role).RankDeep);
            }
        }
        
        foreach (int key  in Temp) {
            Life life = CM.GetLifeM(key, LifeMType.SOLDIER);
            if (life == null || life.m_Core == null) {
                continue ;
            }
            if (life.m_Core.m_Camp == LifeMCamp.ATTACK) {
                AttackList.Add(/*"临时" + */life.m_Attr.AttrName + ":" + (life as Role).RankDeep);
            } else {
                DefenseList.Add(/*"临时" + */life.m_Attr.AttrName + ":" + (life as Role).RankDeep);
            }
        }
    }
    
    /// <summary>
    /// 下一个攻击位
    /// dir 方向
    /// IsInRoom 限制是否在房间内，true 限制在房间内，不限制
    /// </summary>
    public  MapGrid GetNextAttackStation(DIR dir, bool IsInRoom = true)
    {
        MapGrid m = null;
        if (dir == DIR.LEFT) {
            m = Left;
            while (m != null && m.PropStations != StationsProp.ATTACK) {
                if (m.Type == GridType.GRID_WALL && IsInRoom == true) {
                    return null;
                }
                m = m.Left;
            }
            return m;
        } else if (dir == DIR.RIGHT) {
            m = Right;
            while (m != null && m.PropStations != StationsProp.ATTACK) {
                if (m.Type == GridType.GRID_WALL && IsInRoom == true) {
                    return null;
                }
                m = m.Right;
            }
            return m;
        } else {
            return null;
        }
    }
    
    
    
    public bool HaveBuildRoom()
    {
        if (m_lBulid.Count > 0) {
            return true;
        } else {
            return false;
        }
    }
    
    /// <summary>
    /// 在连续一片格子范围内是否包含障碍物
    /// </summary>
    /// <param name="Start">起始格子位置</param>
    /// <param name="length">长度</param>
    /// <param name="dir">方向</param>
    /// <returns>返回格子区域内是否有障碍物体</returns>
    public static bool HaveObstaclesInArea(Int2 Start, int length, DIR dir)
    {
        int def = 0;
        if (dir == DIR.LEFT) {
            def = -1;
        } else if (dir == DIR.RIGHT) {
            def = 1;
        } else {
            return false;
        }
        
        for (int i = 0; i < length; i++) {
            Int2 pos = Start;
            pos.Unit += i * def;
            MapGrid mg = MapGrid.GetMG(pos);
            if (mg != null && mg.HaveBuildRoom() == true) {
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// 在连续一片格子范围内获取攻击未
    /// </summary>
    /// <param name="Attacklist">返回攻击位列表</param>
    /// <param name="Start">起始格子位置</param>
    /// <param name="length">长度</param>
    public static void GetAttackStation(ref List<MapGrid> Attacklist, Int2 Start, int length)
    {
        if (Attacklist == null) {
            Attacklist  = new List<MapGrid>();
        }
        Attacklist.Clear();
        
        for (int i = 0; i < length; i++) {
            Int2 pos = Start;
            pos.Unit += i;
            MapGrid mg = MapGrid.GetMG(pos);
            if (mg != null && mg.PropStations == StationsProp.ATTACK) {
                Attacklist.Add(mg);
            }
        }
    }
    
    /// <summary>
    /// 在连续一片格子范围内,中间的攻击位
    /// </summary>
    /// <param name="Start">起始格子位置</param>
    /// <param name="length">长度</param>
    /// <returns>返回中间攻击位的位置</returns>
    public static Int2 GetMidAttackStation(Int2 Start, int length)
    {
        List<MapGrid> Attacklist = new List<MapGrid>();
        
        GetAttackStation(ref Attacklist, Start, length);
        
        if (Attacklist.Count == 0) {
            return Int2.zero;
        }
        int mid = Attacklist.Count / 2 ;
        return Attacklist[mid].GridPos;
    }
    
    
    /// <summary>
    /// 在连续一片格子范围内,获取门
    /// </summary>
    /// <param name="Start">起始格子位置</param>
    /// <param name="End">结束点格子位置</param>
    /// <returns>返回墙列表</returns>
    public static void GetWallList(ref List<IggWall>WallList, Int2 Start, Int2 End)
    {
        if (WallList == null) {
            WallList  = new List<IggWall>();
        }
        WallList.Clear();
        
        if (Start.Layer != End.Layer) {
            return ;
        }
        
        if (Start.Unit >= End.Unit) {
            for (int i = Start.Unit; i >= End.Unit ; i--) {
                AddWall(ref WallList, Start.Layer, i);
            }
        } else {
            for (int i = Start.Unit; i <= End.Unit ; i ++) {
                AddWall(ref WallList, Start.Layer, i);
            }
        }
    }
    
    /// <summary>
    /// 获取格子的墙体，并添加到列表中
    /// </summary>
    /// <param name="Start">起始格子位置</param>
    /// <param name="End">结束点格子位置</param>
    /// <returns>返回墙列表</returns>
    private  static void AddWall(ref List<IggWall>WallList, int Layer, int Unit)
    {
        if (WallList == null) {
            WallList  = new List<IggWall>();
        }
        MapGrid m = GetMG(Layer, Unit);
        if (m != null) {
            IggWall w = m.GetWall();
            if (w != null && WallList.Contains(w) == false) {
                WallList.Add(w);
            }
        }
        
    }
    
    
    public static void CreateMapGrid(List<Int3> SizeList)
    {
        ClearMapGrid();
        if (SizeList == null
            || SizeList.Count == 0) {
            return;
        }
        
        for (int layer = 0; layer < SizeList.Count; layer++) {
            Dictionary<int, MapGrid>l = new Dictionary<int, MapGrid>();
            int start = SizeList[layer].GridStart;
            int end =   start + SizeList[layer].GridLength;
            //crate mapgrid
            int unit = start;
            for (; unit < end; unit++) {
                MapGrid m = new MapGrid();
                m.Init();
                m.GridPos = new Int2(unit, layer);
                l.Add(unit, m);
            }
            //建立left ，right 连接
            unit = start;
            for (; unit < end; unit++) {
                if (unit == start && unit  != end - 1) {
                    l[unit].Right = l[unit + 1];
                } else if (unit == end - 1 && unit != start) {
                    l[unit].Left = l[unit - 1];
                } else if (unit == end - 1 && unit == start) {
                } else {
                    l[unit].Left = l[unit - 1];
                    l[unit].Right = l[unit + 1];
                }
            }
            s_GridList.Add(layer, l);
        }
    }
    
    public static MapGrid GetMG(Int2 p)
    {
        return GetMG(p.Layer, p.Unit);
    }
    public static bool CheckLink(MapGrid start, MapGrid end)
    {
        if (start.GridPos.Layer == end.GridPos.Layer) {
            MapGrid g = start;
            if (start.GridPos.Unit > end.GridPos.Unit) {
                while (g.Left != null) {
                    if (g.Left == end) {
                        return true;
                    }
                    g = g.Left;
                }
            } else if (start.GridPos.Unit < end.GridPos.Unit) {
                while (g.Right != null) {
                    if (g.Right == end) {
                        return true;
                    }
                    g = g.Right;
                }
            } else {
                return true;
            }
        }
        return false;
    }
    public static void CutLine(int layer, int unit, bool bLeft)
    {
    
        ShipCanvasInfo shipinfo = CmCarbon.GetDefenseMap();
        MapGrid g = GetMG(layer, unit);
        if (g != null) {
            if (bLeft) {
                if (g.Left != null) {
                    g.Left.Right = null;
                }
                g.Left = null;
            } else {
                if (g.Right != null) {
                    g.Right.Left = null;
                }
                g.Right = null;
            }
            for (int i = 0; i < unit; i++) {
                MapGrid left = GetMG(layer, i);
                MapGrid righttop = left.m_NpNearestStation[(int)NearestStationDir.NSD_RIGHTTOP];
                if (righttop != null && righttop.GridPos.Unit >= unit) {
                    left.m_NpNearestStation[(int)NearestStationDir.NSD_RIGHTTOP] = null;
                }
                MapGrid rightdown = left.m_NpNearestStation[(int)NearestStationDir.NSD_RIGHTDOWN];
                if (rightdown != null && rightdown.GridPos.Unit >= unit) {
                    left.m_NpNearestStation[(int)NearestStationDir.NSD_RIGHTDOWN] = null;
                }
                
            }
            for (int i = unit + 1; i < (shipinfo.width  * MapGrid.m_UnitRoomGridNum - 1); i++) {
                MapGrid right = GetMG(layer, i);
                MapGrid lefttop = right.m_NpNearestStation[(int)NearestStationDir.NSD_LEFTTOP];
                if (lefttop != null && lefttop.GridPos.Unit <= unit) {
                    right.m_NpNearestStation[(int)NearestStationDir.NSD_LEFTTOP] = null;
                }
                MapGrid leftdown = right.m_NpNearestStation[(int)NearestStationDir.NSD_LEFTDOWN];
                if (leftdown != null && leftdown.GridPos.Unit <= unit) {
                    right.m_NpNearestStation[(int)NearestStationDir.NSD_LEFTDOWN] = null;
                }
                
            }
        }
    }
    public static MapGrid GetMG(int Layer, int Unit)
    {
        if (s_GridList == null) {
            return null;
        }
        
        if (s_GridList.ContainsKey(Layer) == false) {
            return null;
        }
        
        if (s_GridList [Layer].ContainsKey(Unit) == false) {
            return null;
        }
        return s_GridList [Layer] [Unit];
    }
    
    
    
    public static  Vector3 GetMGPos(int layer, int unit)
    {
        MapGrid m = MapGrid.GetMG(layer, unit);
        
        if (m == null) {
            return Vector3.zero;
        } else {
            return m.pos;
        }
    }
    
    
    public static MapGrid Getbrothers(Int2 p, DIR dir)
    {
        Int2 pos = Int2.zero;
        if (MapSize.Getbrothers(p, dir, ref pos) == false) {
            return null;
        }
        return GetMG(pos);
    }
    
    public static void CreateHole(List<Int2>l)
    {
        if (l == null || l.Count == 0) {
            return;
        }
        foreach (Int2 hole in l) {
            CreateHole(hole);
        }
    }
    
    
    public void CreateWall(Int2 p)
    {
        MapGrid Grid = GetMG(p);
        if (Grid != null) {
            Grid.Type = GridType.GRID_WALL;
        }
    }
    //设置交界处格子洞
    public static void CreateLeftShareHole(Int2 p, ref int WallID)
    {
        MapGrid g = GetMG(p);
        if (g.Left == null || g.Left.Type == GridType.GRID_HOLE) {
            CreateHole(p, ref  WallID);
        }
    }
    public static void CreateRightShareHole(Int2 p, ref int WallID)
    {
        MapGrid g = GetMG(p);
        if (g.Right == null || g.Right.Type == GridType.GRID_HOLE) {
            CreateHole(p, ref  WallID);
        }
    }
    public static void CreateHole(Int2 p)
    {
        MapGrid g = GetMG(p);
        if (g == null) {
            return;
        }
        if (g.Type == GridType.GRID_HOLE
            || g.Type == GridType.GRID_HOLESTAIR) {
            return;
        }
        MapGrid Down = Getbrothers(g.GridPos, DIR.DOWN);
        if (g.Type == GridType.GRID_STAIR) {
            g.Type = GridType.GRID_HOLESTAIR;
            if (g.PropStations == StationsProp.ATTACK) {
                g.PropStations = StationsProp.NORMAL;
            }
        } else {
            g.Type = GridType.GRID_HOLE;
            if (Down != null) {
                if (g.PropStations == StationsProp.ATTACK) {
                    g.PropStations = StationsProp.NORMAL;
                }
            }
        }
        if (Down != null) {
            Down.UpHaveHole = true;
            g.Down = Down;
            SetNNearestDownPort(g, Down, true);
        }
    }
    
    
    public static void CreateHole(Int2 p, ref int WallID)
    {
        WallID = -1;
        MapGrid g = GetMG(p);
        //Debug.Log("CreateHole :" + p + "," + g.m_lBulid.Count );
        if (g == null) {
            return;
        }
        if (g.Type == GridType.GRID_HOLE
            || g.Type == GridType.GRID_HOLESTAIR) {
            return;
        }
        
        MapGrid Down = Getbrothers(g.GridPos, DIR.DOWN);
        if (g.Type == GridType.GRID_STAIR) {
            g.Type = GridType.GRID_HOLESTAIR;
            if (g.PropStations == StationsProp.ATTACK) {
                g.PropStations = StationsProp.NORMAL;
            }
        } else {
            g.Type = GridType.GRID_HOLE;
            if (Down != null) {
                if (g.PropStations == StationsProp.ATTACK) {
                    g.PropStations = StationsProp.NORMAL;
                }
            }
        }
        if (Down != null) {
            g.Down = Down;
            WallID =  g.GetWallID();
            Down.UpHaveHole = true;
            
            //外墙破了之后，会修复Cutline中被断开的连接
            if (g.Left != null && g.Left.Type == GridType.GRID_HOLE && Down.Left == null && Down.Type != GridType.GRID_HOLE) {
                Down.Left = g.Left.Down;
                if (Down.Left != null) {
                    Down.Left.Right = Down;
                    SetNNearestDownPort(Down.Left, Down.Left.Down, true);
                }
            }
            if (g.Right != null && g.Right.Type == GridType.GRID_HOLE && Down.Right == null && Down.Type != GridType.GRID_HOLE) {
                Down.Right = g.Right.Down;
                if (Down.Right != null) {
                    Down.Right.Left = Down;
                    SetNNearestDownPort(Down.Right, Down.Right.Down, true);
                }
            }
            SetNNearestDownPort(g, Down, true);
        }
    }
    
    
    
    public static void CreateStair(List<StairInfo>l)
    {
        if (l == null || l.Count == 0) {
            return;
        }
        foreach (StairInfo Stair in l) {
            CreateStair(Stair);
        }
    }
    public static void CreateStair(StairInfo Stair)
    {
        MapGrid UpStair = GetMG(Stair.Up);
        MapGrid DownStair = GetMG(Stair.Down);
        if (UpStair != null && DownStair != null) {
            MapStair stair = new MapStair();
            stair.Init();
            stair.SetStair(Stair.Up, Stair.Down);
            
            if (UpStair.Type == GridType.GRID_HOLE) {
                UpStair.Type = GridType.GRID_HOLESTAIR;
            } else {
                UpStair.Type = GridType.GRID_STAIR;
            }
            if (DownStair.Type == GridType.GRID_HOLE) {
                DownStair.Type = GridType.GRID_HOLESTAIR;
            } else {
                DownStair.Type = GridType.GRID_STAIR;
            }
            UpStair.Down = DownStair;
            DownStair.Up = UpStair;
            MapStair.AddStair(stair);
            SetNNearestUpPort(DownStair, UpStair);
            SetNNearestDownPort(UpStair, DownStair, false);
            //这个需要调整。
            //MapGrid m = GetMG(Stair.Down.Layer,Stair.Up.Unit);
            //if(m != null)
            //{
            //	DownStair.UpHaveHole = true;
            //}
            
            //foreach(Int2 HolePos in Stair.m_NearHole )
            //{
            //	CreateHole(HolePos);
            //}
            
            
        }
    }
    
    protected static void SetNNearestDownPort(MapGrid gridPort, MapGrid gridDown, bool bHole)
    {
        gridPort.m_NpNearestStation[(int)NearestStationDir.NSD_CENTERDOWN] = gridDown;
        gridPort.m_NfNearestStationscoreG[(int)NearestStationDir.NSD_CENTERDOWN] = fcounstScoreG;
        
        MapGrid gridTemp = gridPort;
        MapGrid rightestHole = gridTemp;
        while (null != gridTemp && gridTemp.Type == GridType.GRID_HOLE
            || null != gridTemp && gridTemp.Type == GridType.GRID_HOLESTAIR) {
            rightestHole = gridTemp;
            gridTemp = gridTemp.Right;
        }
        gridTemp = gridPort;
        MapGrid leftestHole = gridTemp;
        while (null != gridTemp && gridTemp.Type == GridType.GRID_HOLE
            || null != gridTemp && gridTemp.Type == GridType.GRID_HOLESTAIR) {
            gridTemp = gridTemp.Left;
        }
        
        MapGrid nextNode = null;
        float fscoreG = fcounstScoreG;
        if (leftestHole != rightestHole) {
            nextNode = leftestHole.Right;
            fscoreG = fcounstScoreG;
            while (null != nextNode) {
                nextNode.m_NpNearestStation[(int)NearestStationDir.NSD_LEFTDOWN] = leftestHole;
                nextNode.m_NfNearestStationscoreG[(int)NearestStationDir.NSD_LEFTDOWN] = fscoreG;
                if (nextNode == rightestHole) {
                    break;
                }
                fscoreG += fcounstScoreG;
                nextNode = nextNode.Right;
            }
            
            
            nextNode = rightestHole.Left;
            fscoreG = fcounstScoreG;
            while (null != nextNode) {
                nextNode.m_NpNearestStation[(int)NearestStationDir.NSD_RIGHTDOWN] = rightestHole;
                nextNode.m_NfNearestStationscoreG[(int)NearestStationDir.NSD_RIGHTDOWN] = fscoreG;
                if (nextNode == leftestHole) {
                    break;
                }
                fscoreG += fcounstScoreG;
                nextNode = nextNode.Left;
            }
        }
        
        if (rightestHole == gridPort) {
            //上端梯子口右边格子的左边最近站点（前往下层)
            nextNode = gridPort.Right;
            fscoreG = fcounstScoreG;
            while (null != nextNode) {
                if (nextNode.m_NpNearestStation[(int)NearestStationDir.NSD_LEFTDOWN] == null
                    || nextNode.m_NpNearestStation[(int)NearestStationDir.NSD_LEFTDOWN] == gridPort.m_NpNearestStation[(int)NearestStationDir.NSD_LEFTDOWN]) {
                    nextNode.m_NpNearestStation[(int)NearestStationDir.NSD_LEFTDOWN] = gridPort;
                    nextNode.m_NfNearestStationscoreG[(int)NearestStationDir.NSD_LEFTDOWN] = fscoreG;
                    fscoreG += fcounstScoreG;
                    nextNode = nextNode.Right;
                } else {
                    break;
                }
            }
        }
        
        if (leftestHole == gridPort) {
            //上端梯子口左边格子的右边最近站点（前往下层)
            nextNode = gridPort.Left;
            fscoreG = fcounstScoreG;
            while (null != nextNode) {
                if (nextNode.m_NpNearestStation [(int)NearestStationDir.NSD_RIGHTDOWN] == null
                    || nextNode.m_NpNearestStation [(int)NearestStationDir.NSD_RIGHTDOWN] == gridPort.m_NpNearestStation [(int)NearestStationDir.NSD_RIGHTDOWN]) {
                    nextNode.m_NpNearestStation [(int)NearestStationDir.NSD_RIGHTDOWN] = gridPort;
                    nextNode.m_NfNearestStationscoreG [(int)NearestStationDir.NSD_RIGHTDOWN] = fscoreG;
                    fscoreG += fcounstScoreG;
                    nextNode = nextNode.Left;
                } else {
                    break;
                }
            }
        }
        
    }
    
    protected static void SetNNearestUpPort(MapGrid gridPort, MapGrid gridUp)
    {
        gridPort.m_NpNearestStation[(int)NearestStationDir.NSD_CENTERTOP] = gridUp;
        gridPort.m_NfNearestStationscoreG[(int)NearestStationDir.NSD_CENTERTOP] = fcounstScoreG;
        
        //下端梯子口右边格子的左边最近站点（前往上层)
        MapGrid nextNode = gridPort.Right;
        float fscoreG = fcounstScoreG;
        while (null != nextNode) {
            if (nextNode.m_NpNearestStation[(int)NearestStationDir.NSD_LEFTTOP] == null
                || nextNode.m_NpNearestStation[(int)NearestStationDir.NSD_LEFTTOP] == gridPort.m_NpNearestStation[(int)NearestStationDir.NSD_LEFTTOP]) {
                nextNode.m_NpNearestStation[(int)NearestStationDir.NSD_LEFTTOP] = gridPort;
                nextNode.m_NfNearestStationscoreG[(int)NearestStationDir.NSD_LEFTTOP] = fscoreG;
                fscoreG += fcounstScoreG;
                nextNode = nextNode.Right;
            } else {
                break;
            }
        }
        //gridPort.m_NpNearestStation[(int)NearestStationDir.NSD_CENTERTOP]=gridUp;
        //gridPort.m_NfNearestStationscoreG[(int)NearestStationDir.NSD_CENTERTOP] = fcounstScoreG;
        
        //下端梯子口左边格子的右边最近站点（前往上层)
        nextNode = gridPort.Left;
        fscoreG = fcounstScoreG;
        while (null != nextNode) {
            if (nextNode.m_NpNearestStation[(int)NearestStationDir.NSD_RIGHTTOP] == null
                || nextNode.m_NpNearestStation[(int)NearestStationDir.NSD_RIGHTTOP] == gridPort.m_NpNearestStation[(int)NearestStationDir.NSD_RIGHTTOP]) {
                nextNode.m_NpNearestStation[(int)NearestStationDir.NSD_RIGHTTOP] = gridPort;
                nextNode.m_NfNearestStationscoreG[(int)NearestStationDir.NSD_RIGHTTOP] = fscoreG;
                fscoreG += fcounstScoreG;
                nextNode = nextNode.Left;
            } else {
                break;
            }
        }
        
        
    }
    
    
    public static void ClearMapGrid()
    {
        if (s_GridList == null) {
            s_GridList = new Dictionary<int, Dictionary<int, MapGrid>>();
        }
        
        foreach (Dictionary<int, MapGrid> l in s_GridList.Values) {
            if (l != null) {
                l.Clear();
            }
        }
        s_GridList.Clear();
        
    }
    
    
    public static int GetMapGridCount()
    {
        int count = 0;
        foreach (Dictionary<int, MapGrid> l in s_GridList.Values) {
            count += l.Count;
        }
        return count;
        
    }
    
    public static bool HasReverseLongHole(int Layer, int Min, int Max)
    {
        for (int Unit = Min + 1; Unit < Max; Unit++) {
            MapGrid m = GetMG(Layer, Unit);
            if (m == null ||
                (!m.UpHaveHole)) {
                return false;
            }
        }
        return true;
    }
    public static bool HasLongHole(int Layer, int Min, int Max)
    {
        for (int Unit = Min + 1; Unit < Max; Unit++) {
            MapGrid m = GetMG(Layer, Unit);
            if (m == null ||
                (m.Type != GridType.GRID_HOLE && m.Type != GridType.GRID_HOLESTAIR)) {
                return false;
            }
        }
        return true;
    }
    
    
    /// <summary>
    /// 设置攻击位
    /// </summary>
    public static  void SetAttackGrid(List<Int2> AttackPos)
    {
        foreach (Int2 I in AttackPos) {
            MapGrid m = GetMG(I);
            if (m !=  null && m.Type == GridType.GRID_NORMAL) {
                m.PropStations = StationsProp.ATTACK;
            }
        }
    }
    /// <summary>
    /// 获取格子
    /// </summary>
    public static void GetMapGridList(ref List<MapGrid> list)
    {
        if (list == null) {
            list = new List<MapGrid> ();
        }
        list.Clear();
        if (s_GridList == null || s_GridList.Count == 0) {
            return;
        }
        
        foreach (Dictionary<int, MapGrid> l in s_GridList.Values) {
            if (l == null || l.Count == 0) {
                continue;
            }
            foreach (MapGrid m in l.Values) {
                list.Add(m);
            }
        }
    }
    
    
    /// <summary>
    /// 获取指定层的格子列表
    /// </summary>
    /// <param name="layer">指定层</param>
    public static void GetLayerGridList(ref List<MapGrid> list, int layer)
    {
        if (list == null) {
            list = new List<MapGrid> ();
        }
        list.Clear();
        if (s_GridList == null || s_GridList.Count == 0) {
            return;
        }
        if (s_GridList.ContainsKey(layer) == false) {
            return;
        }
        
        foreach (MapGrid m in s_GridList[layer].Values) {
            list.Add(m);
        }
    }
    /// <summary>
    /// 获取附近的攻击位,原则左优先扫描
    /// </summary>
    /// <param name="layer">指定层</param>
    public  MapGrid GetNearAttackPos()
    {
        if (PropStations == StationsProp.ATTACK) {
            return this;
        }
        MapGrid left = this.Left;
        MapGrid right = this.Right;
        int ldistance = 10000;
        int rdistance = 10000;
        
        
        while (left != null) {
            if (left != null) {
                if (left.PropStations == StationsProp.ATTACK) {
                    ldistance = GridPos.Unit - left.GridPos.Unit;
                    break;
                } else {
                    left = left.Left;
                }
            }
        }
        //
        while (right != null) {
        
            if (right != null) {
                if (right.PropStations == StationsProp.ATTACK) {
                    rdistance =  right.GridPos.Unit - GridPos.Unit;
                    break;
                } else {
                    right = right.Right;
                }
            }
        }
        
        if (left != null && right != null) {
            if (ldistance <= rdistance) {
                return left;
            } else {
                return right;
            }
        } else if (left != null) {
            return left;
        } else if (right != null) {
            return right;
        }
        return null;
    }
    
    
    
    
    public  override void SortRank()
    {
        m_Channel.SetRolePower(m_HoldRole, m_TempRole);
        m_Channel.SortChannel();
        m_HoldRole.SetChannel(m_Channel);
        m_TempRole.SetChannel(m_Channel);
        m_HoldRole.ResetTop();
        m_TempRole.ResetTop();
    }
    
    
    /// <summary>
    /// 是否为空闲攻击位
    /// </summary>
    /// <returns>true 空闲攻击位</returns>
    public bool CheckIdleAttackStation()
    {
        if (PropStations != StationsProp.ATTACK) {
            return false;
        }
        bool Hold = m_HoldRole.HaveRole();
        bool Temp = m_TempRole.HaveRole();
        if (Hold == true || Temp == true) {
            return false;
        } else {
            return true;
        }
    }
    
    
    public void GetBuildList(ref List<int> list)
    {
        if (list == null) {
            list = new List<int> ();
        }
        list.Clear();
        
        foreach (int  key in m_lBulid) {
            list.Add(key);
        }
    }
    
    //建筑物登记
    public void JoinBuild(int SceneID)
    {
        if (m_lBulid.Contains(SceneID) == false) {
            m_lBulid.Add(SceneID);
        }
        //Debug.Log("JoinBuild " + GridPos + "," + SceneID + "," + m_lBulid.Count);
    }
    //获取地形房间
    public bool GetBuildRoom(ref int BuildRoomSceneID)
    {
        BuildRoomSceneID =  -1;
        foreach (int key in m_lBulid) {
            BuildRoomSceneID = key;
            return true;
        }
        return false;
    }
    
    //移除建筑物
    public bool RemoveBuild(int SceneID)
    {
        if (m_lBulid.Contains(SceneID)) {
            m_lBulid.Remove(SceneID);
        }
        return true;
    }
    
    
    //移除墙体
    public void JoinWall(int SceneID)
    {
        if (m_lWall.Contains(SceneID) == false) {
            if (Type == GridType.GRID_HOLE || Type == GridType.GRID_HOLESTAIR) {
                Down = null;
                int width = MapSize.GetLayerSize(GridPos.Layer);
                for (int i = 0; i < width; i++) {
                    MapGrid g = GetMG(GridPos.Layer, i);
                    if (g != null) {
                        if (g.m_NpNearestStation[(int)NearestStationDir.NSD_LEFTDOWN] == this) {
                            if (Left != null && (Left.Type == GridType.GRID_HOLE || Left.Type == GridType.GRID_HOLESTAIR)) {
                                g.m_NpNearestStation[(int)NearestStationDir.NSD_LEFTDOWN] = Left;
                            } else {
                                g.m_NpNearestStation[(int)NearestStationDir.NSD_LEFTDOWN] = null;
                            }
                        }
                        if (g.m_NpNearestStation[(int)NearestStationDir.NSD_CENTERDOWN] == this) {
                            g.m_NpNearestStation[(int)NearestStationDir.NSD_CENTERDOWN] = null;
                        }
                        if (g.m_NpNearestStation[(int)NearestStationDir.NSD_RIGHTDOWN] == this) {
                            if (Right != null && (Right.Type == GridType.GRID_HOLE || Right.Type == GridType.GRID_HOLESTAIR)) {
                                g.m_NpNearestStation[(int)NearestStationDir.NSD_RIGHTDOWN] = Right;
                            } else {
                                g.m_NpNearestStation[(int)NearestStationDir.NSD_RIGHTDOWN] = null;
                            }
                        }
                        
                    }
                }
            }
            m_lWall.Add(SceneID);
            SetGridType(GridType.GRID_WALL);
        }
        
    }
    
    public bool RemoveWall(int SceneID)
    {
        if (m_lWall.Contains(SceneID) == false) {
            m_lWall.Remove(SceneID);
        }
        return true;
    }
    
    
    public IggWall GetWall()
    {
        if (m_lWall == null || m_lWall.Count == 0) {
            return null;
        }
        foreach (int key in m_lWall) {
            Life m = CM.GetLifeM(key, LifeMType.WALL);
            if (m != null && m is IggWall) {
                return m as IggWall;
            }
        }
        
        return null;
    }
    
    public bool HaveBuild()
    {
        if ((m_lBulid == null || m_lBulid.Count == 0)
            && (m_lWall == null || m_lWall.Count == 0)) {
            return false;
        } else {
            return true;
        }
    }
    
    /// <summary>
    /// 清除格子数据中的life相关数据
    /// </summary>
    /// <returns></returns>
    public void ClearLife()
    {
        ClearRoleStations();
        if (m_lBulid != null) {
            m_lBulid.Clear();
        }
        if (m_lWall != null) {
            m_lWall.Clear();
        }
    }
}
