using UnityEngine;
using System.Collections.Generic;
//特别说明，宏值不可随意改动。有跟服务端数据进行关联
public enum BattleEnvironmentMode {
    CombatPVE = 0,  //PVE战斗模式
    Edit      = 1,  //编辑模式
}
/// <summary>
/// 战场环境数据,船只摆放环境数据，包含船体本身数据
/// </summary>
/// <author>zhulin</author>
public class BattleEnvironmentM
{
    /// <summary>
    /// 战场环境模式
    /// </summary>
    private static BattleEnvironmentMode m_Mode = BattleEnvironmentMode.CombatPVE;
    /// <summary>
    /// 海洋环境数据
    /// </summary>
    private static SeaEnviron m_SeaEnviron = null;
    
    /// <summary>
    /// 船体数据
    /// </summary>
    private static MapStart m_MapStart = null;
    
    private static Vector3 m_CenterPos = new Vector3(0, 0, 0);
    /// <summary>
    /// 天空背景数据
    /// </summary>
    private static SkyEnviron m_SkyEnviron = null;
    /// <summary>
    /// 设置战场环境模式
    /// </summary>
    public static void SetBattleEnvironmentMode(BattleEnvironmentMode Mode)
    {
        m_Mode = Mode;
    }
    
    /// <summary>
    /// 获取战场模式
    /// </summary>
    public static BattleEnvironmentMode GetBattleEnvironmentMode()
    {
        return m_Mode ;
    }
    /// <summary>
    /// 加入海洋环境数据
    /// </summary>
    public static void JoinSeaData(SeaEnviron Sea)
    {
        m_SeaEnviron = Sea;
    }
    
    /// <summary>
    /// 移除海洋环境数据
    /// </summary>
    public static void ExitSeaData()
    {
        m_SeaEnviron = null;
    }
    
    /// <summary>
    /// 获取海洋环境数据
    /// </summary>
    public static SeaEnviron GetSeaData()
    {
        return m_SeaEnviron;
    }
    
    /// <summary>
    /// 加入天空环境数据
    /// </summary>
    public static void JoinSkyData(SkyEnviron Sky)
    {
        m_SkyEnviron = Sky;
    }
    
    /// <summary>
    /// 移除海洋环境数据
    /// </summary>
    public static void ExitSkyData()
    {
        m_SkyEnviron = null;
    }
    
    /// <summary>
    /// 获取天空环境数据
    /// </summary>
    public static SkyEnviron GetSkyData()
    {
        return m_SkyEnviron;
    }
    /// <summary>
    /// 加入船体数据
    /// </summary>
    public static void JoinBoatData(MapStart start)
    {
        m_MapStart = start;
    }
    
    /// <summary>
    /// 移除海洋环境数据
    /// </summary>
    public static void ExitBoatData()
    {
        m_MapStart = null;
    }
    /// <summary>
    /// 获取海豚上船路线节点
    /// </summary>
    /// <param name="Dir">方向</param>
    public static bool GetDolphineLine(WalkDir Dir, ref Vector3 Start, ref Vector3 End)
    {
    
        if (Dir == WalkDir.WALKLEFT) {
            if (m_SeaEnviron != null) {
                Start = m_SeaEnviron.LeftDolphine.transform.position;
            }
            if (m_MapStart != null) {
                End = m_MapStart.GetDolphineLeftPoint();
                /*if (Start.x <= End.x)
                	End = m_MapStart.GetDolphineRightPoint();*/
            }
            return true;
        } else if (Dir == WalkDir.WALKRIGHT) {
            if (m_SeaEnviron != null) {
                Start = m_SeaEnviron.RightDolphine.transform.position;
            }
            if (m_MapStart != null) {
                End = m_MapStart.GetDolphineRightPoint();
                /*if (Start.x >= End.x)
                	End = m_MapStart.GetDolphineLeftPoint();*/
            }
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 获取击飞落脚点
    /// </summary>
    /// <param name="Dir">方向</param>
    public static Vector3 GetHitFlyEndPoint(Vector3 pos, WalkDir Dir, bool bearhit)
    {
        if (Dir == WalkDir.WALKLEFT) {
            if (m_SeaEnviron != null) {
                if (!bearhit) {
                    return m_SeaEnviron.LeftHitFlyPoint.transform.position;
                }
                ShipCanvasInfo shipinfo =  GetShipBodyMap();
                Vector3 flypos = m_SeaEnviron.LeftHitFlyPoint.transform.position;
                flypos.x = pos.x - shipinfo.width * MapGrid.m_UnitRoomGridNum * MapGrid.m_width * 0.5f;
                flypos.y -= 10;
                return flypos;
            }
        } else if (Dir == WalkDir.WALKRIGHT) {
            if (m_SeaEnviron != null) {
                if (!bearhit) {
                    return m_SeaEnviron.RightHitFlyPoint.transform.position;
                }
                ShipCanvasInfo shipinfo =  GetShipBodyMap();
                Vector3 flypos = m_SeaEnviron.RightHitFlyPoint.transform.position;
                flypos.x = pos.x + shipinfo.width * MapGrid.m_UnitRoomGridNum * MapGrid.m_width * 0.5f;
                flypos.y -= 10;
                return flypos;
            }
        }
        return Vector3.zero;
    }
    
    /// <summary>
    /// 获取LifeM 出生节点
    /// </summary>
    /// <param name="InBoat">true 船内，flase 船外</param>
    /// <returns>获取LifeM 出生节点</returns>
    public static Transform GetLifeMBornNode(bool  InBoat)
    {
        if (InBoat == true) {
            if (m_MapStart != null) {
                return m_MapStart.transform;
            }
        } else {
            if (m_SkyEnviron != null) {
                return m_SkyEnviron.FlyNode;
            }
        }
        return null;
    }
    
    public static Transform GetEditShipStart()
    {
        return TreasureScene.GetMapStart();
    }
    
    /// <summary>
    /// 构建战斗环境
    /// </summary>
    public  static void BuildScene()
    {
        U3DUtil.DestroyAllChild(m_MapStart.gameObject);
        if (m_Mode == BattleEnvironmentMode.CombatPVE) {
            MapM.ClearMap();
            CM.ExitCm();
            BuildCombatPVEScene();
        } else if (m_Mode == BattleEnvironmentMode.Edit) {
            BuildCombatEditScene();
        }
    }
    /// <summary>
    /// 添加发兵模块
    /// </summary>
    public static void AddFireSoldierCompent()
    {
        if (m_MapStart == null) {
            return ;
        }
        GameObject g = m_MapStart.transform.parent.gameObject;
        if (g == null) {
            return ;
        }
        if (CmCarbon.GetCamp2Player(LifeMCamp.ATTACK) == false) {
            g.AddComponent<EnemyMonsterFire>();
        } else {
            g.AddComponent<PlayerSoldierFire>();
        }
    }
    
    private static void ClearCombatScene()
    {
        if (m_MapStart != null) {
            U3DUtil.DestroyAllChild(m_MapStart.gameObject);
        }
    }
    
    public static void BuildViewStageScene()
    {
        CounterPartInfo Info = StageDC.GetCounterPartInfo();
        ShipCanvasInfo Map = StageM.GetCounterPartMap(Info.countershipcanvasid);
        List<SoldierInfo> lSoldier = new List<SoldierInfo>();
        List<BuildInfo> lBuild = new  List<BuildInfo>();
        StageM.GetCounterPartShipPut(Info.id, ref lSoldier, ref lBuild);
        CmCarbon.SetDefenseMap(Map);
        GenerateShip.GenerateShips(GetShipBodyMap(), GetLifeMBornNode(true), lBuild, lSoldier);
        MainCameraM.s_Instance.ResetCameraDataByBattle();
    }
    
    
    /// <summary>
    /// 构建PVE战斗环境
    /// </summary>
    private  static void BuildCombatPVEScene()
    {
        MainCameraM.s_Instance.ResetCameraDataByBattle();
        LoadCombatMap();
        LoadDefenseBulid();
        GenerateShip.GenerateShipsBattle(GetShipBodyMap(), GetLifeMBornNode(true));
        LoadDefenseSoldier();
    }
    /// <summary>
    /// 构建编辑战斗环境
    /// </summary>
    private  static void BuildCombatEditScene()
    {
        ShipPlan plan = ShipPlanDC.GetCurShipPlan();
        if (plan != null) {
            LoadShipCanvans(plan.ShipDesignId);
        }
        LoadShipBuild();
    }
    
    /// <summary>
    /// 创建船体骨架
    /// </summary>
    private static ShipCanvasInfo GetShipBodyMap()
    {
        List<StairInfo> lStair = new List<StairInfo>();
        CmCarbon.GetStairInfo(ref lStair);
        
        ShipCanvasInfo Info = new ShipCanvasInfo();
        Info.Copy(CmCarbon.GetDefenseMap());
        
        List<Int2> lCutMapPoint = new List<Int2> ();
        
        foreach (StairInfo I in lStair) {
            //裁剪掉上层
            lCutMapPoint.Add(new Int2(I.Up.Unit / MapGrid.m_UnitRoomGridNum, I.Up.Layer));
        }
        Info.SetStairMap(lCutMapPoint, XYmode.Save2Edit);
        return Info;
    }
    
    /// <summary>
    /// 加载编辑船只数据。
    /// </summary>
    public static void LoadShipBuild()
    {
        ShipPlan P = ShipPlanDC.GetCurShipPlan();
        List<ShipPutInfo> PutBuild = P.GetShipBuildInfo(ShipBuildType.All);
        if (PutBuild != null) {
            foreach (ShipPutInfo r in PutBuild) {
                if (r == null) {
                    continue;
                }
                if (r.type == (int)ShipBuildType.BuildRoom) {
                    BuildInfo RoomBuild = r.GetBuildInfo();
                    if (RoomBuild != null) {
                        LoadShipPutBuild(r.id, r.objid, RoomBuild);
                    }
                } else if (r.type == (int)ShipBuildType.BuildStair) {
                    BuildInfo RoomBuild = r.GetBuildInfo();
                    if (RoomBuild != null) {
                        LoadShipPutStairBuild(r.id, r.objid, RoomBuild);
                    }
                } else if (r.type == (int)ShipBuildType.Soldier) {
                    SoldierInfo s = new SoldierInfo();
                    if (r.GetSoldier(ref s) == true) {
                        LoadShipPutSoldier(r.id, r.objid, s);
                    }
                }
            }
        }
    }
    
    
    public static void ResetStartPos(Int2 size, bool bSetimmediately)
    {
        if (m_MapStart != null) {
            m_MapStart.CalcPositition(size, m_CenterPos, true);
        }
        
        
    }
    public static void ReLoadShipCanvans(int shipDesignID)
    {
        ShipPlanDC.SetShipDesignID(shipDesignID);
        TouchMoveManager.ClearShipBuild();
        ClearCombatScene();
        //		LoadCombatMap();
        LoadShipCanvans(shipDesignID);
        LoadShipBuild();
        GenerateDeck();
    }
    private static void GenerateDeck()
    {
        ShipPlan P = ShipPlanDC.GetCurShipPlan();
        if (P  == null) {
            return ;
        }
        P.DoDeckDataStart();
        GenerateShip.CreateMiddleBeam();
        P.DoDeckDataEnd();
    }
    /// <summary>
    /// 加载船的画布表现
    /// </summary>
    public  static void LoadShipCanvans(int shipDesignID)
    {
        Transform parent = GetLifeMBornNode(true);
        if (parent != null) {
            GameObjectLoader.LoadPath("Prefabs/Others/", "PlaneCollider", parent);
            Int2 n2Pos = new Int2(0, 0);
            Int2 nEndPos = GetEndPosByShape(shipDesignID);
            string []ShapeList = GetDesignShape(shipDesignID);
            
            if (ShapeList == null || ShapeList.Length != nEndPos.Layer) {
                NGUIUtil.DebugLog(" low version");
                return;
            }
            RoomMap.RealMapSize = nEndPos;
            
            for (n2Pos.Layer = RoomMap.Mapv2Start.Layer; n2Pos.Layer < nEndPos.Layer; n2Pos.Layer++) {
                for (n2Pos.Unit = RoomMap.Mapv2Start.Unit; n2Pos.Unit < nEndPos.Unit; n2Pos.Unit++) {
                    if (ShapeList.Length > 0 && ShapeList.Length <= nEndPos.Layer && ShapeList[n2Pos.Layer].Length <= nEndPos.Unit) {
                        bool bDeck = CheckIsDeckGrid(ShapeList, n2Pos.Layer, n2Pos.Unit);
                        if (ShapeList[n2Pos.Layer][n2Pos.Unit] == '0' && !bDeck) {
                            continue;
                        }
                    }
                    GameObject go = GameObjectLoader.LoadPath("Prefabs/Others/", "CanvasUnitBG", parent);
                    go.name = string.Format("[{0},{1}]", n2Pos.Layer, n2Pos.Unit);
                    go.transform.localPosition = new Vector3(3.0f * n2Pos.Unit, (3.0f + 0.3f) * n2Pos.Layer, 8.1f);
                    CanvasUnitBG bk = go.GetComponent<CanvasUnitBG>();
                    if (bk != null) {
                        TouchMoveManager.JoinCanvasUnitBk(n2Pos, bk);
                    }
                }
            }
        }
        TouchMoveManager.SetAllGridBgStates(CanvasUnitState.Normal);
        TouchMoveManager.ShowCanvas(true);
        Vector3 pos = MainCameraM.GetDiffenceDisignCameraPos();
        MainCameraM.s_Instance.SetCameraLimitParam(MainCameraM.s_reaLimitPyramidBoatView);
        MainCameraM.s_Instance.AutoMoveTo(pos, 0f);
    }
    /// <summary>
    /// 获取当前方案最大的格子数.
    /// </summary>
    /// <returns>The end position by shape.</returns>
    /// <param name="ShipCanvasTypeId">Ship canvas type identifier.</param>
    public static Int2 GetEndPosByShape(int shipDesignID)
    {
        StaticShipCanvas canvas = ShipPlanDC.GetShipDesignInfo(shipDesignID);
        if (canvas != null) {
            Int2 endPos = new Int2();
            endPos.Layer = canvas.Height;
            endPos.Unit = canvas.Width;
            return endPos;
        }
        return RoomMap.RealMapSize;
    }
    
    public static string [] GetDesignShape(int shipDesignID)
    {
        StaticShipCanvas canvas = ShipPlanDC.GetShipDesignInfo(shipDesignID);
        if (canvas != null) {
            return canvas.Shape.Split(',');
        }
        return null;
    }
    
    public static bool CheckIsDeckGrid(string [] l, int curHIndex, int curWIndex)
    {
        for (int layer = curHIndex; layer < l.Length; layer ++) {
            if (l[layer].Length > curWIndex) {
                if (l[layer][curWIndex] == '1') {
                    return false;
                }
            }
        }
        //如果他的下一层不为0则是甲板层.
        if (curHIndex > 0 && l[curHIndex - 1][curWIndex] == '0') {
            return false;
        }
        
        return true;
    }
    
    public static bool CheckIsDeckGrid(Int2 GridPos)
    {
        StaticShipCanvas canvas = ShipPlanDC.GetCurShipDesignInfo();
        if (canvas != null) {
            string[] l = canvas.Shape.Split(',');
            return CheckIsDeckGrid(l, GridPos.Layer, GridPos.Unit);
        }
        return false;
    }
    /// <summary>
    /// 目的区域是否是图形区域内.
    /// </summary>
    /// <returns><c>true</c>, if in shape was built, <c>false</c> otherwise.</returns>
    /// <param name="roomPosGrid">Room position grid.</param>
    public static bool BuildInShape(Int2 roomPosGrid)
    {
        StaticShipCanvas canvas = ShipPlanDC.GetCurShipDesignInfo();
        if (canvas != null) {
            string[] ShapeList = canvas.Shape.Split(',');
            //是否在设计图宽高内.
            if (roomPosGrid.Layer >= RoomMap.Mapv2Start.Layer && roomPosGrid.Layer < canvas.Height) {
                if (roomPosGrid.Unit >= RoomMap.Mapv2Start.Unit && roomPosGrid.Unit < canvas.Width) {
                    //设计图对应的位置是否能摆放
                    if (ShapeList.Length > 0 && ShapeList.Length <= canvas.Height && ShapeList[roomPosGrid.Layer].Length <= canvas.Width) {
                        if (ShapeList[roomPosGrid.Layer][roomPosGrid.Unit] == '0') {
                            return false;
                        }
                    }
                    
                    return true;
                }
            }
        }
        return false;
    }
    /// <summary>
    /// 加载战斗地图
    /// </summary>
    private  static void LoadCombatMap()
    {
        ShipCanvasInfo Info = CmCarbon.GetDefenseMap();
        if (Info != null) {
            Info.GetMapData();
            BattleEnvironmentM.ResetStartPos(Info.GetMapSize(), true);
            MapM.CreateMap(Info);
            ShipBombRule.SetBombMap(Info);
        }
    }
    /// <summary>
    /// 加载防御方建筑物
    /// </summary>
    private  static void LoadDefenseBulid()
    {
        Dictionary<int, BuildInfo> l = CmCarbon.GetBuildData();
        if (l == null || l.Count == 0) {
            return ;
        }
        foreach (int DataID in  l.Keys) {
            ProduceBulidRoom(DataID, l[DataID]);
        }
    }
    /// <summary>
    /// 创建建筑物体
    /// </summary>
    private static void ProduceBulidRoom(int DataID, BuildInfo Info)
    {
        if (Info == null) {
            return ;
        }
        if (Info.m_RoomType == RoomType.Stair) {
            ProduceStairBulid(Info);
        } else {
            Building  mBuild = ProduceRoomBulid(DataID, Info);
            if (mBuild != null) {
                List<Int2> lPutRoom = Info.GetPutRoom();
                if (Info.m_RoomType == RoomType.DeckTrap) {
                    ShipBombRule.JoinDeckBuildRoom(lPutRoom, mBuild);
                } else if (Info.m_RoomType == RoomType.ResRoom) {
                    ShipBombRule.JoinGoldBuildRoom(lPutRoom, mBuild);
                } else {
                    ShipBombRule.JoinBuildRoom(lPutRoom, mBuild);
                }
            }
        }
    }
    
    /// <summary>
    /// 创建建筑物体
    /// </summary>
    private static void ProduceStairBulid(BuildInfo Info)
    {
        if (Info == null) {
            return ;
        }
        Int2 Pos = new Int2(Info.m_cx, Info.m_cy);
        Transform t	= GetLifeMBornNode(true);
        if (t == null) {
            return ;
        }
        Vector3 worldPos = Vector3.zero ;
        MapGrid mg = MapGrid.GetMG(Pos);
        if (mg != null) {
            worldPos.x = mg.WorldPos.x ;//-MapGrid.m_width/2 ;
            worldPos.y = mg.WorldPos.y ;
        } else {
            return ;
        }
        
        Building1201 stair = LifeFactory.CreateBuilding(Info, 0, t, worldPos, LifeEnvironment.Combat) as Building1201;
        if (stair != null) {
            stair.SetMapGrid(Info.m_cx +  2, Info.m_cy);
        }
    }
    /// <summary>
    /// 创建建筑物体
    /// </summary>
    private static Building ProduceRoomBulid(int DataID, BuildInfo Info)
    {
        if (Info == null) {
            return null;
        }
        MapGrid mg = MapGrid.GetMG(Info.m_cy, Info.m_cx);
        if (mg != null) {
            return LifeFactory.CreateBuilding(Info, DataID, GetLifeMBornNode(true), mg.WorldPos, LifeEnvironment.Combat) ;
        }
        return null;
    }
    /// <summary>
    /// 加载防御方炮弹兵
    /// </summary>
    private  static void LoadDefenseSoldier()
    {
        List<int> list = new List<int>();
        CmCarbon.GetDefenseList(ref list);
        foreach (int DataID in  list) {
            ProduceEmemy(DataID);
        }
    }
    
    /// <summary>
    /// 加载防御方炮弹兵
    /// </summary>
    public static void ProduceEmemy(int DataID)
    {
        SoldierInfo s = CmCarbon.GetSoldierInfo(LifeMCamp.DEFENSE, DataID);
        if (s == null) {
            return ;
        }
        Transform t	= GetLifeMBornNode(true);
        if (t == null) {
            return ;
        }
        Int2 pos = new Int2(s.CX, s.CY);
        MapGrid mg = MapGrid.GetMG(pos);
        if (mg == null) {
            return ;
        }
        
        //IGameRole iRole= GameRoleFactory.Create(t, s.m_modeltype, s.m_name, AnimatorState.Stand);
        Role r = LifeFactory.CreateRole(t, AnimatorState.Stand, s, LifeMCamp.DEFENSE, DataID, MoveState.Walk, pos, LifeEnvironment.Combat);
        GameObject gRole = r.RoleSkinCom.tRoot.gameObject;
        gRole.transform.parent = t;
        gRole.transform.position = mg.WorldPos;
        
    }
    /// <summary>
    /// 添加编辑建筑配件
    /// </summary>
    /// <returns></returns>
    private static void AddBuildingEditPlugin(TouchMove touch)
    {
        if (touch == null) {
            return ;
        }
        GameObject selectEffectParent = null;
        selectEffectParent = new GameObject("SelectEffectParent");
        selectEffectParent.transform.parent = touch.gameObject.transform.parent;
        selectEffectParent.transform.localPosition = new Vector3(0, 0, -1);
        selectEffectParent.transform.localRotation = touch.gameObject.transform.parent.localRotation;
        if (touch.MyCore().m_type == ShipBuildType.Soldier) {
            selectEffectParent.transform.localPosition = new Vector3(-RoomGrid.m_width / 2.0f, 0, -1);
        } else if (touch.MyCore().m_type == ShipBuildType.BuildStair) {
            selectEffectParent.transform.localPosition = new Vector3(0, RoomGrid.m_heigth / 2.0f + 0.3f, -1);
        }
        touch.SetPlugin(selectEffectParent);
    }
    /// <summary>
    /// 加载船上地形及其内部陷阱房间
    /// </summary>
    /// <param name="Info">房间</param>
    /// <param name="lTrap">房间内部陷阱</param>
    public static void LoadShipPutBuild(int ID, int DataID, BuildInfo Info)
    {
        if (Info == null) {
            return ;
        }
        Transform parentT = GetLifeMBornNode(true);
        if (parentT == null) {
            return ;
        }
        
        Int2 BornPos = new Int2(Info.m_cx, Info.m_cy);
        Vector3 local = RoomMap.GetShipBuildLocalPos(BornPos, ShipBuildType.BuildRoom);
        Vector3 world = Local2WorldPos(local);
        Building buildlife = LifeFactory.CreateBuilding(Info, DataID, parentT, world, LifeEnvironment.Edit);
        if (buildlife == null) {
            return ;
        }
        
        buildlife.ShowEditHp(Info.m_bear);
        TouchMove touch = buildlife.m_thisT.GetChild(0).gameObject.AddComponent<TouchMove>();
        Vector2 size = new Vector2(Info.m_Shape.width, Info.m_Shape.height);
        CanvasCore Core = new CanvasCore(ShipBuildType.BuildRoom, false, ID, DataID, size);
        
        touch.InitTouchMoveCore(Core, BornPos, buildlife);
        
        AddBuildingEditPlugin(touch);
        
        TouchMoveManager.JointShipBuild(touch);
    }
    /// <summary>
    /// 加载船上楼梯
    /// </summary>
    public static void LoadShipPutStairBuild(int ID, int DataID, BuildInfo Info)
    {
        if (Info == null) {
            return;
        }
        Transform parentT = GetLifeMBornNode(true);
        if (parentT == null) {
            return;
        }
        Int2 BornPos = new Int2(Info.m_cx, Info.m_cy);
        Vector3 local = RoomMap.GetShipBuildLocalPos(BornPos, ShipBuildType.BuildStair);
        Vector3 world = Local2WorldPos(local);
        Building buildlife = LifeFactory.CreateBuilding(Info, DataID, parentT, world, LifeEnvironment.Edit);
        TouchMove touch = buildlife.m_thisT.GetChild(0).gameObject.AddComponent<TouchMove>();
        Vector2 size = new Vector2(Info.m_Shape.width, Info.m_Shape.height);
        CanvasCore Core = new CanvasCore(ShipBuildType.BuildStair, false, ID, DataID, size);
        touch.InitTouchMoveCore(Core, BornPos, buildlife);
        
        AddBuildingEditPlugin(touch);
        TouchMoveManager.JointShipBuild(touch);
    }
    
    /// <summary>
    /// 加载船上炮弹兵
    /// </summary>
    public static void LoadShipPutSoldier(int ID, int DataID, SoldierInfo Info)
    {
        if (Info == null) {
            return;
        }
        
        Role r = new Role();
        r.CreateSkin(GetLifeMBornNode(true), Info.m_modeltype, Info.m_name, AnimatorState.Stand, true);
        //IGameRole i = GameRoleFactory.Create(GetLifeMBornNode(true), Info.m_modeltype, Info.m_name, AnimatorState.Stand);
        GameObject go = r.RoleSkinCom.tRoot.gameObject;
        go.name = Info.m_name;
        Int2 BornPos = new Int2(Info.CX, Info.CY);
        go.transform.localPosition = RoomMap.GetShipBuildLocalPos(BornPos, ShipBuildType.Soldier);
        go.transform.localPosition = U3DUtil.SetZ(go.transform.localPosition, -2.0f);
        
        Transform tRole = go.transform.GetChild(0);
        TouchMove touch = tRole.gameObject.AddComponent<TouchMove>();
        CanvasCore Core = new CanvasCore(ShipBuildType.Soldier, false, ID, DataID, Vector2.zero);
        touch.InitTouchMoveCore(Core, BornPos, null);
        
        AddBuildingEditPlugin(touch);
        if (Info.SoldierTypeID == 100003) { //隐藏蹦蹦
            //RolePropertyM rpm = i.GetBodyComponent<RolePropertyM>();
            r.RoleSkinCom.ShowLeftHand(false);
        }
        TouchMoveManager.JointShipBuild(touch);
    }
    
    
    /// <summary>
    ///  从仓库中创建建筑。
    /// </summary>
    public static void CreateBuildingFromWarehouse(BuildInfo infoBuild)
    {
        if (infoBuild == null) {
            return ;
        }
        
        Vector2 size = new Vector2(infoBuild.m_Shape.width, infoBuild.m_Shape.height);
        CanvasCore Core = new CanvasCore(ShipBuildType.BuildRoom, true, ShipPutInfo.GetNewShipPutId(), infoBuild.ID, size);
        Int2 BornPos = new Int2(0, 0);
        bool isCanPut = PutCanvasM.GetBornPos(Core, ref BornPos);
        if (isCanPut == false) {
            NGUIUtil.ShowTipWndByKey("88800007", 1.0f);
            return;
        }
        
        Transform t_start = GetLifeMBornNode(true);
        Vector3 local = RoomMap.GetShipBuildLocalPos(BornPos, ShipBuildType.BuildRoom);
        Vector3 world = Local2WorldPos(local);
        Building buildlife = LifeFactory.CreateBuilding(infoBuild, 0, t_start, world, LifeEnvironment.Edit) ;
        if (buildlife == null) {
            return ;
        }
        
        buildlife.ShowEditHp(infoBuild.m_bear);
        TouchMove touch = buildlife.m_thisT.GetChild(0).gameObject.AddComponent<TouchMove>();
        touch.InitTouchMoveCore(Core, BornPos, buildlife);
        
        AddBuildingEditPlugin(touch);
        
        GenerateShip.m_vStart = Vector3.zero;
        
        TouchMoveManager.JointShipBuild(touch);
        TouchMoveManager.SetCurTouchMove(touch);
        touch.WareHouse2Ship(BornPos);
        
    }
    
    /// <summary>
    /// 新增士兵
    /// </summary>
    public static void CreateSoldierFromWarehouse(int soldierId)
    {
        SoldierInfo infoSoldier = SoldierDC.GetSoldiers(soldierId);
        if (infoSoldier == null) {
            return ;
        }
        
        CanvasCore Core = new CanvasCore(ShipBuildType.Soldier, true, ShipPutInfo.GetNewShipPutId(), infoSoldier.ID, Vector2.zero);
        Int2 BornPos = new Int2(0, 0);
        bool isCanPut = PutCanvasM.GetBornPos(Core, ref BornPos);
        if (isCanPut == false) {
            NGUIUtil.ShowTipWndByKey("88800007", 1.0f);
            return;
        }
        
        Transform t_start = GetLifeMBornNode(true);
        Role r = new Role();
        r.CreateSkin(t_start, infoSoldier.m_modeltype, infoSoldier.m_name, AnimatorState.Stand, true);
        //IGameRole i = GameRoleFactory.Create(t_start, infoSoldier.m_modeltype, infoSoldier.m_name, AnimatorState.Stand);
        GameObject go = r.RoleSkinCom.tRoot.gameObject;;
        go.name = infoSoldier.m_name;
        
        Vector3 posMy = t_start.position;
        Vector3 screenSpace = Camera.main.WorldToScreenPoint(posMy);
        Vector3 pos = new Vector3(Screen.width / 2, Screen.height / 1.5f);
        Vector3 posTempmouse = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, screenSpace.z));
        posTempmouse = U3DUtil.SetZ(posTempmouse, 0f);
        go.transform.localPosition = Vector3.zero;
        Transform tRole = go.transform.GetChild(0);
        
        TouchMove touch = tRole.gameObject.AddComponent<TouchMove>();
        tRole.gameObject.transform.parent.localPosition = RoomMap.GetShipBuildLocalPos(BornPos, ShipBuildType.Soldier);
        
        touch.InitTouchMoveCore(Core, BornPos, null);
        
        
        AddBuildingEditPlugin(touch);
        if (infoSoldier.SoldierTypeID == 100003) { //隐藏蹦蹦
            //RolePropertyM rpm = i.GetBodyComponent<RolePropertyM>();
            r.RoleSkinCom.ShowLeftHand(false);
        }
        TouchMoveManager.JointShipBuild(touch);
        TouchMoveManager.SetCurTouchMove(touch);
        touch.WareHouse2Ship(BornPos);
    }
    
    
    /// <summary>
    /// 编辑模式下添加墙
    /// </summary>
    public static void CreateWall(Vector3 pos, Transform t)
    {
        GameObject g = GameObjectLoader.LoadPath("Prefabs/Boats/100/", "100116@Skin", t);
        g.transform.rotation = Quaternion.identity;
        g.transform.Rotate(0, 180, 0, Space.Self);
        g.transform.position = pos;
    }
    
    static void EnableRoleCollider(GameObject go, bool isEnable)
    {
        if (go) {
            RoleColider col = go.GetComponent<RoleColider>();
            if (col) {
                col.EnableCollider(isEnable);
            }
        }
    }
    /// <summary>
    /// 世界坐标转局部坐标，相对start
    /// </summary>
    public static Vector3 World2LocalPos(Vector3 WorldPos)
    {
        Transform t = GetLifeMBornNode(true);
        if (t != null) {
            return t.InverseTransformPoint(WorldPos);
        }
        return Vector3.zero;
    }
    
    /// <summary>
    /// start下对象，转世界坐标
    /// </summary>
    public static Vector3 Local2WorldPos(Vector3 LocalPos)
    {
        Transform t = GetLifeMBornNode(true);
        if (t != null) {
            return t.TransformPoint(LocalPos);
        }
        return Vector3.zero;
    }
    
    static List<GameObject> s_effectgolist = new List<GameObject>();
    public static void ShowSmoke()
    {
        int deadcout = CmCarbon.GetDestroyBuild().Count;
        int allcount = CmCarbon.GetDestoryBuildCount();
        
        float y = deadcout * 1.0f / allcount ;
        Transform start = BattleEnvironmentM.GetLifeMBornNode(true);
        ShipCanvasInfo info = CmCarbon.GetDefenseMap();
        Transform boathead = GenerateShip.BoatHead;
        Transform boattail = GenerateShip.BoatTail;
        Transform boatmast = GenerateShip.BoatMast;
        foreach (GameObject go in s_effectgolist) {
            if (go != null) {
                GameObject.Destroy(go);
            }
        }
        s_effectgolist.Clear();
        if (boathead == null || boatmast == null || boattail == null) {
            return;
        }
        Vector3 pos = Vector3.zero;
        if (0f < y && y <= 0.25f) {
            if (info.height <= 2) {
                pos = boathead.position;
                pos.y += 3;
                GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "2000111", pos, null);
                s_effectgolist.Add(gae.gameObject);
                pos = boattail.position;
                gae = EffectM.LoadEffect("effect/prefab/", "2000111", pos, null);
                s_effectgolist.Add(gae.gameObject);
                
            } else {
                pos = boathead.position;
                pos.y += 6;
                GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "2000111", pos, null);
                s_effectgolist.Add(gae.gameObject);
                pos = boattail.position;
                pos.y += 3;
                gae = EffectM.LoadEffect("effect/prefab/", "2000111", pos, null);
                s_effectgolist.Add(gae.gameObject);
            }
        } else if (0.25f < y && y <= 0.5f) {
            if (info.height <= 2) {
                pos = boathead.position;
                pos.y += 3;
                GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "2000101", pos, null);
                s_effectgolist.Add(gae.gameObject);
                pos = boattail.position;
                gae = EffectM.LoadEffect("effect/prefab/", "2000101", pos, null);
                s_effectgolist.Add(gae.gameObject);
                pos = boatmast.position;
                gae = EffectM.LoadEffect("effect/prefab/", "2000111", pos, null);
                s_effectgolist.Add(gae.gameObject);
            } else {
                pos = boathead.position;
                pos.y += 6;
                GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "2000101", pos, null);
                s_effectgolist.Add(gae.gameObject);
                pos = boattail.position;
                pos.y += 3;
                gae = EffectM.LoadEffect("effect/prefab/", "2000101", pos, null);
                s_effectgolist.Add(gae.gameObject);
                pos = boatmast.position;
                gae = EffectM.LoadEffect("effect/prefab/", "2000111", pos, null);
                s_effectgolist.Add(gae.gameObject);
            }
        } else if (0.5f < y) {
            if (info.height <= 2) {
                pos = boathead.position;
                pos.y += 3;
                GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "2000091", pos, null);
                s_effectgolist.Add(gae.gameObject);
                pos.y -= 3;
                pos.x -= 3;
                gae = EffectM.LoadEffect("effect/prefab/", "2000111", pos, null);
                s_effectgolist.Add(gae.gameObject);
                pos = boattail.position;
                gae = EffectM.LoadEffect("effect/prefab/", "2000091", pos, null);
                s_effectgolist.Add(gae.gameObject);
                pos.y += 3;
                pos.x += 3;
                gae = EffectM.LoadEffect("effect/prefab/", "2000111", pos, null);
                s_effectgolist.Add(gae.gameObject);
                pos = boatmast.position;
                gae = EffectM.LoadEffect("effect/prefab/", "2000091", pos, null);
                s_effectgolist.Add(gae.gameObject);
            } else {
                pos = boathead.position;
                pos.y += 6;
                GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "2000091", pos, null);
                s_effectgolist.Add(gae.gameObject);
                pos.y -= 3;
                pos.x -= 3;
                gae = EffectM.LoadEffect("effect/prefab/", "2000111", pos, null);
                s_effectgolist.Add(gae.gameObject);
                pos.y += 3;
                pos = boattail.position;
                gae = EffectM.LoadEffect("effect/prefab/", "2000091", pos, null);
                s_effectgolist.Add(gae.gameObject);
                pos.y += 3;
                pos.x += 3;
                gae = EffectM.LoadEffect("effect/prefab/", "2000111", pos, null);
                s_effectgolist.Add(gae.gameObject);
                pos = boatmast.position;
                gae = EffectM.LoadEffect("effect/prefab/", "2000091", pos, null);
                s_effectgolist.Add(gae.gameObject);
            }
        }
    }
    
    
}
