using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
public enum LocationType {
    Top,
    Middle,
    Bottom,
    Left,
    Right,
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight
}
public class HitDeckInfo
{
    public Life m_target;
    public int m_unit;
    public int m_layer;
    public LocationType m_type;
    public HitDeckInfo(Life l, int unit, int layer, LocationType t)
    {
        m_target = l;
        m_unit = unit;
        m_layer = layer;
        m_type = t;
    }
    
}
public class GenerateShip
{
    /// <summary>
    /// 生成船用的Start 根节点
    /// </summary>
    public static Transform m_Start;
    public static Transform m_Boat;
    public static float m_totalanitime;
    public const float beamwith = 0.3f;
    public const float corner = 0.65f;
    public const float roomwidth = 3f;
    public const float inval = 0.05f;
    public static Vector3 m_lbpos;
    public static Vector3 m_rtpos;
    /// <summary>
    /// 偏移后起始位置
    /// </summary>
    public static Vector3 m_vStart;
    public static bool isAni = false;
    public static bool isBattle = false;
    public static float delay = 0;
    public static bool isWave = false;
    //public static string shipinfo.skin = "100";
    public static ShipCanvasInfo shipinfo;
    public static Dictionary<int, Transform> Drooms = new Dictionary<int, Transform>();
    //横
    public static Dictionary<int, Transform> DicHorizontal = new Dictionary<int, Transform>();
    //竖
    public static Dictionary<int, Transform> DicVertical = new Dictionary<int, Transform>();
    //角落
    public static Dictionary<int, Transform> DicCorner = new Dictionary<int, Transform>();
    
    public static Dictionary<int, GameObject> DicBattle = new Dictionary<int, GameObject>();
    public static Dictionary<int, GameObject> DicWall = new Dictionary<int, GameObject>();
    
    public static Dictionary<int, GameObject> DicBattleVertical = new Dictionary<int, GameObject>();
    
    //满足撞的甲板，怪物AI
    public static List<HitDeckInfo> LHitDeck = new List<HitDeckInfo>();
    //配撞破的甲板
    public static List<HitDeckInfo> LHitBrokeDeck = new List<HitDeckInfo>();
    /// <summary>
    /// 形状
    /// </summary>
    public static List<List<int>> grid = new List<List<int>>();
    public static List<List<int>> map = new List<List<int>>();
    
    public static List<Vector3> m_v3RejectPolygon = new List<Vector3>();
    public static List<Vector3> m_v3OutRejectPolygon = new List<Vector3>();
    public static List<GameObject> MidBeam = new List<GameObject>();
    public static List<GameObject> RoomWall = new List<GameObject>();
    
    //保存解体对象
    private static List<GameObject> m_goTop = new List<GameObject>();
    private static List<GameObject> m_goBottom = new List<GameObject>();
    private static List<GameObject> m_goLeft = new List<GameObject>();
    private static List<GameObject> m_goRight = new List<GameObject>();
    
    
    public static Transform BoatHead = null;
    public static Transform BoatTail = null;
    public static Transform BoatMast = null;
    public static Vector3 GetbuildPos(Int2 grid)
    {
        return new Vector3(grid.Unit * MapGrid.m_width, grid.Layer * (roomwidth + beamwith), 0);
    }
    public static Vector3 GetRealPos(float x, float y, float z)
    {
        return new Vector3(x, y, z) + m_vStart;
    }
    public static void FlyToPos(GameObject go, LocationType lt, float x, float y, float z, bool effect = true, int wave = 0)
    {
        if (!isAni) {
            go.transform.localPosition = GetRealPos(x, y, z);
            return;
        }
        float duration = 0.3f;
        float bdelay = 0f;
        if (wave == 0) {
            go.transform.localPosition = new Vector3(x, y, z);
        } else {
            m_totalanitime = delay + duration + 3 * 0.2f + duration;
            bdelay = delay + duration + wave * 0.2f;
            string effectname = "2000381_01";
            if (lt == LocationType.Top) {
                go.transform.localPosition = new Vector3(x, m_rtpos.y, z);
                //delay = 3 * duration /4;
            } else if (lt == LocationType.Bottom) {
                go.transform.localPosition = new Vector3(x, m_lbpos.y - 6f, z);
                //delay = 1 * duration/4;
            } else if (lt == LocationType.Left) {
                go.transform.localPosition = new Vector3(m_lbpos.x, y, z);
                effectname = "2000381_02";
            } else if (lt == LocationType.Right) {
                go.transform.localPosition = new Vector3(m_rtpos.x, y, z);
                effectname = "2000381_02";
                //delay = 2 * duration/4;
            }
            if (effect) {
                GameObjectActionExcute gae = go.AddComponent<GameObjectActionExcute>();
                Vector3 pos = m_Start.TransformPoint(new Vector3(x, y, z));
                pos.z = -0.5f;
                GameObjectActionCreateEffect gace = new GameObjectActionCreateEffect(bdelay + duration, 1f, effectname, pos, null);
                gace.m_complete = Shake;
                gae.AddAction(gace);
                
                GameObjectActionPlayEffectSound gaceSound = new GameObjectActionPlayEffectSound(0f, 1f, "ship_compound_04");
                
                gae.AddAction(gaceSound);
                
            }
            go.transform.DOMove(new Vector3(x, y, z), duration);
            m_Start.transform.DOShakePosition(0.2f);
        }
    }
    //解体飞行
    static void FlyToPos(GameObject go, LocationType lt, GameObject completeTarget, string completefun)
    {
        float duration = 0.7f;
        Vector3 fromPos = go.transform.localPosition;
        Vector3 toPos = Vector3.zero;
        float offset = 30f;
        switch (lt) {
            case LocationType.Top:
                toPos = U3DUtil.AddY(fromPos, offset);
                break;
                
            case LocationType.Bottom:
                toPos = U3DUtil.AddY(fromPos, -offset);
                break;
                
            case LocationType.Left:
                toPos = U3DUtil.AddX(fromPos, -offset);
                break;
                
            case LocationType.Right:
                toPos = U3DUtil.AddX(fromPos, offset);
                break;
        }
        Tweener tweener = go.transform.DOMove(toPos, duration);
        //tweener.OnComplete(completefun);
    }
    
    static void ScaleDown(GameObject go)
    {
        Vector3 pos = go.transform.localPosition;
        go.transform.localPosition = U3DUtil.AddZ(pos, 2f);
        go.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.5f);
    }
    
    /// <summary>
    /// 解体前缩小
    /// </summary>
    public static void BreakUpFallBack(GameObject completetarget, string completefun)
    {
        Vector3 scaleFactor = new Vector3(0.8f, 0.8f, 0.8f);
        foreach (GameObject go in m_goBottom) {
            ScaleDown(go);
        }
        foreach (GameObject go in m_goLeft) {
            ScaleDown(go);
        }
        foreach (GameObject go in m_goRight) {
            ScaleDown(go);
        }
        foreach (GameObject go in m_goTop) {
           // iTween.ScaleTo(go, iTween.Hash("x", scaleFactor.x, "y", scaleFactor.y, "z", scaleFactor.z, "time", 0.3f, "islocal", true,
           //         "oncompletetarget", completetarget, "oncomplete", completefun));
            go.transform.DOScale(scaleFactor, 0.3f);
        }
    }
    /// <summary>
    /// 解体表现
    /// </summary>
    public static void BreakUp(GameObject completetarget, string completefun)
    {
        foreach (GameObject go in m_goTop) {
            FlyToPos(go, LocationType.Top, completetarget, "");
        }
        foreach (GameObject go in m_goBottom) {
            FlyToPos(go, LocationType.Bottom, completetarget, "");
        }
        foreach (GameObject go in m_goLeft) {
            FlyToPos(go, LocationType.Left, completetarget, "");
        }
        foreach (GameObject go in m_goRight) {
            FlyToPos(go, LocationType.Right, completetarget, completefun);
        }
    }
    
    public static void Shake(object o)
    {
        //iTween.ShakePosition(m_Start.gameObject, iTween.Hash("amount", new Vector3(0.5f, 0.5f, 0), "time", 0.2f, "islocal", true));
        m_Start.DOShakePosition(0.2f);
    }
    public static int GetIndex(int unit, int layer, int off = 0)
    {
        return layer * (shipinfo.width + 1) * MapGrid.m_UnitRoomGridNum + unit * MapGrid.m_UnitRoomGridNum + off;
    }
    public static Int2 IndexToPos(int index)
    {
        int layer = index / ((shipinfo.width + 1) * MapGrid.m_UnitRoomGridNum);
        int unit = index % ((shipinfo.width + 1) * MapGrid.m_UnitRoomGridNum);
        return new Int2(unit, layer);
        //return layer * (shipinfo.width+1)*6 + unit*6 + off;
    }
    public static void DoFlyAction(Transform t, float delay)
    {
        if (isAni) {
            Vector3 pos = t.localPosition;
            t.position = new Vector3(t.position.x, t.position.y, MainCameraM.s_vHavenViewBoatviewCamPos.z - 10f);
            //iTween.MoveTo(t.gameObject, iTween.Hash("position", pos, "time", 0.3f, "islocal", true, "easetype", iTween.EaseType.easeInCubic, "delay", delay));
            t.transform.DOMove(pos, 0.3f);
        }
    }
    public static void WallFly(Int2 start)
    {
        LocationType CurDirection = LocationType.Top;
        Int2 curpos = new Int2(start.Unit, start.Layer);
        Int2 oldpos = new Int2(-1, -1); ;
        if (isBattle) {
            foreach (Transform t in DicCorner.Values) {
                Corner c = t.GetComponent<Corner>();
                if (c != null) {
                    if (DicBattle.ContainsKey(GetIndex(c.pos.Unit, c.pos.Layer))) {
                        c.AddLife(DicBattle[GetIndex(c.pos.Unit, c.pos.Layer)].GetComponent<LifeProperty>());
                    }
                    if (c.pos.Unit > 0 && DicBattle.ContainsKey(GetIndex(c.pos.Unit - 1, c.pos.Layer))) {
                        c.AddLife(DicBattle[GetIndex(c.pos.Unit - 1, c.pos.Layer)].GetComponent<LifeProperty>());
                    }
                    if (DicBattleVertical.ContainsKey(GetIndex(c.pos.Unit, c.pos.Layer))) {
                        c.AddLife(DicBattleVertical[GetIndex(c.pos.Unit, c.pos.Layer)].GetComponent<LifeProperty>());
                    }
                    if (c.pos.Layer > 0 && DicBattleVertical.ContainsKey(GetIndex(c.pos.Unit, c.pos.Layer - 1))) {
                        c.AddLife(DicBattleVertical[GetIndex(c.pos.Unit, c.pos.Layer - 1)].GetComponent<LifeProperty>());
                    }
                }
            }
        }
        while (true) {
            if (CurDirection == LocationType.Top || CurDirection == LocationType.Bottom) {
                if (CurDirection == LocationType.Top && DicCorner.ContainsKey(GetIndex(curpos.Unit, curpos.Layer))) {
                    DoFlyAction(DicCorner[GetIndex(curpos.Unit, curpos.Layer)], delay);
                    DicCorner.Remove(GetIndex(curpos.Unit, curpos.Layer));
                    delay += inval;
                } else if (CurDirection == LocationType.Bottom && DicCorner.ContainsKey(GetIndex(curpos.Unit, curpos.Layer + 1))) {
                    DoFlyAction(DicCorner[GetIndex(curpos.Unit, curpos.Layer + 1)], delay);
                    if (oldpos.Unit != curpos.Unit || oldpos.Layer != curpos.Layer + 1) {
                        m_v3RejectPolygon.Add(m_Start.TransformPoint(curpos.Unit * roomwidth, (curpos.Layer + 1) * (roomwidth + beamwith), 0));
                        oldpos = new Int2(curpos.Unit, curpos.Layer + 1);
                    }
                    DicCorner.Remove(GetIndex(curpos.Unit, curpos.Layer + 1));
                    delay += inval;
                }
                
                if (oldpos.Unit != curpos.Unit || oldpos.Layer != curpos.Layer) {
                    m_v3RejectPolygon.Add(m_Start.TransformPoint(curpos.Unit * roomwidth, (curpos.Layer) * (roomwidth + beamwith), 0));
                    oldpos = curpos;
                }
                
                if (isAni) {
                    GameObjectActionExcute gae = DicVertical[GetIndex(curpos.Unit, curpos.Layer)].gameObject.AddComponent<GameObjectActionExcute>();
                    Vector3 pos = m_Start.TransformPoint(DicVertical[GetIndex(curpos.Unit, curpos.Layer)].localPosition);
                    pos.z = -0.5f;
                    GameObjectActionCreateEffect gace = new GameObjectActionCreateEffect(delay + 0.3f, 1f, "2000381_02", pos, null);
                    gae.AddAction(gace);
                    int randName = Random.Range(2, 3);
                    string soundName = "ship_compound_0" + randName;
                    GameObjectActionPlayEffectSound gaceSound = new GameObjectActionPlayEffectSound(0f, 1f, soundName);
                    
                    gae.AddAction(gaceSound);
                }
                DoFlyAction(DicVertical[GetIndex(curpos.Unit, curpos.Layer)], delay);
                DicVertical.Remove(GetIndex(curpos.Unit, curpos.Layer));
                delay += inval;
                if (CurDirection == LocationType.Top && DicCorner.ContainsKey(GetIndex(curpos.Unit, curpos.Layer + 1))) {
                    DoFlyAction(DicCorner[GetIndex(curpos.Unit, curpos.Layer + 1)], delay);
                    if (oldpos.Unit != curpos.Unit || oldpos.Layer != curpos.Layer + 1) {
                        m_v3RejectPolygon.Add(m_Start.TransformPoint(curpos.Unit * roomwidth, (curpos.Layer + 1) * (roomwidth + beamwith), 0));
                        oldpos = new Int2(curpos.Unit, curpos.Layer + 1);
                    }
                    DicCorner.Remove(GetIndex(curpos.Unit, curpos.Layer + 1));
                    delay += inval;
                } else if (CurDirection == LocationType.Bottom && DicCorner.ContainsKey(GetIndex(curpos.Unit, curpos.Layer))) {
                    DoFlyAction(DicCorner[GetIndex(curpos.Unit, curpos.Layer)], delay);
                    DicCorner.Remove(GetIndex(curpos.Unit, curpos.Layer));
                    delay += inval;
                    
                }
            } else if (CurDirection == LocationType.Left || CurDirection == LocationType.Right) {
                if (map[curpos.Layer][curpos.Unit] != 1) {
                    for (int i = 0; i < MapGrid.m_UnitRoomGridNum; i++) {
                    
                        DicHorizontal[GetIndex(curpos.Unit, curpos.Layer, i)].GetComponent<Renderer>().enabled = false;
                        //GameObject.Destroy(DicHorizontal[GetIndex(curpos.Unit, curpos.Layer,i)].gameObject);
                        //DicHorizontal.Remove(GetIndex(curpos.Unit, curpos.Layer, i));
                        //DicBattle.Remove(GetIndex(curpos.Unit, curpos.Layer, i));
                    }
                } else {
                    if (isAni) {
                        GameObjectActionExcute gae = DicHorizontal[GetIndex(curpos.Unit, curpos.Layer)].gameObject.AddComponent<GameObjectActionExcute>();
                        Vector3 pos = m_Start.TransformPoint(DicHorizontal[GetIndex(curpos.Unit, curpos.Layer)].localPosition);
                        pos.z = -0.5f;
                        GameObjectActionCreateEffect gace = new GameObjectActionCreateEffect(delay + 0.3f, 1f, "2000381_01", pos, null);
                        gae.AddAction(gace);
                        
                        int randName = Random.Range(1, 3);
                        string soundName = "ship_compound_0" + randName;
                        GameObjectActionPlayEffectSound gaceSound = new GameObjectActionPlayEffectSound(0f, 1f, soundName);
                        
                        
                        gae.AddAction(gaceSound);
                    }
                    
                    
                    for (int i = 0; i < MapGrid.m_UnitRoomGridNum; i++) {
                        DoFlyAction(DicHorizontal[GetIndex(curpos.Unit, curpos.Layer, i)], delay);
                        
                        Transform t = DicHorizontal[GetIndex(curpos.Unit, curpos.Layer, i)];
                        if (map[curpos.Layer][curpos.Unit] != 1) {
                            t.GetComponent<Renderer>().enabled = false;
                        }
                        DicHorizontal.Remove(GetIndex(curpos.Unit, curpos.Layer, i));
                    }
                    delay += inval;
                }
                if (!(start.Unit == curpos.Unit && start.Layer == curpos.Layer) && (oldpos.Unit != curpos.Unit || oldpos.Layer != curpos.Layer)) {
                    m_v3RejectPolygon.Add(m_Start.TransformPoint(curpos.Unit * roomwidth, (curpos.Layer) * (roomwidth + beamwith), 0));
                    oldpos = curpos;
                }
                if (start.Unit == curpos.Unit && start.Layer == curpos.Layer) {
                    break;
                }
            } else {
                break;
            }
            
            if (CurDirection == LocationType.Top) {
                if (curpos.Unit > 0 && DicHorizontal.ContainsKey(GetIndex(curpos.Unit - 1, curpos.Layer + 1))) {
                    CurDirection = LocationType.Left;
                    curpos.Layer += 1;
                    curpos.Unit -= 1;
                } else if (DicHorizontal.ContainsKey(GetIndex(curpos.Unit, curpos.Layer + 1))) {
                    CurDirection = LocationType.Right;
                    curpos.Layer += 1;
                } else if (DicVertical.ContainsKey(GetIndex(curpos.Unit, curpos.Layer + 1))) {
                    CurDirection = LocationType.Top;
                    curpos.Layer += 1;
                } else {
                    Debug.Log("error " + curpos + "  ,  " + CurDirection);
                }
            } else if (CurDirection == LocationType.Bottom) {
                if (DicHorizontal.ContainsKey(GetIndex(curpos.Unit, curpos.Layer))) {
                    CurDirection = LocationType.Right;
                } else if (curpos.Unit > 0 && DicHorizontal.ContainsKey(GetIndex(curpos.Unit - 1, curpos.Layer))) {
                    CurDirection = LocationType.Left;
                    curpos.Unit -= 1;
                } else if (curpos.Layer > 0 && DicVertical.ContainsKey(GetIndex(curpos.Unit, curpos.Layer - 1))) {
                    CurDirection = LocationType.Bottom;
                    curpos.Layer -= 1;
                } else {
                    Debug.Log("error " + curpos + "  ,  " + CurDirection);
                }
            } else if (CurDirection == LocationType.Left) {
                if (curpos.Layer > 0 && DicVertical.ContainsKey(GetIndex(curpos.Unit, curpos.Layer - 1))) {
                    CurDirection = LocationType.Bottom;
                    curpos.Layer -= 1;
                } else if (DicVertical.ContainsKey(GetIndex(curpos.Unit, curpos.Layer))) {
                    CurDirection = LocationType.Top;
                } else if (curpos.Unit > 0 && DicHorizontal.ContainsKey(GetIndex(curpos.Unit - 1, curpos.Layer))) {
                    CurDirection = LocationType.Left;
                    curpos.Unit -= 1;
                } else {
                    Debug.Log("error " + curpos + "  ,  " + CurDirection);
                }
            } else if (CurDirection == LocationType.Right) {
                if (DicVertical.ContainsKey(GetIndex(curpos.Unit + 1, curpos.Layer))) {
                    CurDirection = LocationType.Top;
                    curpos.Unit += 1;
                } else if (curpos.Layer > 0 && DicVertical.ContainsKey(GetIndex(curpos.Unit + 1, curpos.Layer - 1))) {
                    CurDirection = LocationType.Bottom;
                    curpos.Unit += 1;
                    curpos.Layer -= 1;
                } else if (DicHorizontal.ContainsKey(GetIndex(curpos.Unit + 1, curpos.Layer))) {
                    CurDirection = LocationType.Right;
                    curpos.Unit += 1;
                } else {
                    Debug.Log("error " + curpos + "  ,  " + CurDirection);
                }
            }
        }
        if (isAni) {
            foreach (Transform t in DicCorner.Values) {
                DoFlyAction(t, delay);
            }
            DicCorner.Clear();
            foreach (Transform t in DicVertical.Values) {
                GameObjectActionExcute gae = t.gameObject.AddComponent<GameObjectActionExcute>();
                Vector3 pos = m_Start.TransformPoint(t.localPosition);
                pos.z = -0.5f;
                GameObjectActionCreateEffect gace = new GameObjectActionCreateEffect(delay + 0.3f, 1f, "2000381_02", pos, null);
                gae.AddAction(gace);
                int randName = Random.Range(1, 3);
                string soundName = "ship_compound_0" + randName;
                GameObjectActionPlayEffectSound gaceSound = new GameObjectActionPlayEffectSound(0f, 1f, soundName);
                
                gae.AddAction(gaceSound);
                DoFlyAction(t, delay);
            }
            DicVertical.Clear();
            foreach (int i in DicHorizontal.Keys) {
                Transform t = DicHorizontal[i];
                Int2 Ipos = IndexToPos(i);
                if (map[Ipos.Layer][Ipos.Unit / MapGrid.m_UnitRoomGridNum] != 1) {
                    t.GetComponent<Renderer>().enabled = false;
                    //GameObject.Destroy(t.gameObject);
                    //DicBattle.Remove(i);
                } else {
                    if ((i % MapGrid.m_UnitRoomGridNum) == 0) {
                        GameObjectActionExcute gae = t.gameObject.AddComponent<GameObjectActionExcute>();
                        Vector3 pos = m_Start.TransformPoint(t.localPosition);
                        pos.z = -0.5f;
                        GameObjectActionCreateEffect gace = new GameObjectActionCreateEffect(delay + 0.3f, 1f, "2000381_01", pos, null);
                        int randName = Random.Range(1, 3);
                        string soundName = "ship_compound_0" + randName;
                        GameObjectActionPlayEffectSound gaceSound = new GameObjectActionPlayEffectSound(0f, 1f, soundName);
                        
                        gae.AddAction(gaceSound);
                        gae.AddAction(gace);
                    }
                    DoFlyAction(t, delay);
                }
            }
            DicHorizontal.Clear();
            delay += inval;
        }
        //把楼梯的板去掉
        if (isBattle) {
            foreach (int i in DicHorizontal.Keys) {
                Transform t = DicHorizontal[i];
                Int2 Ipos = IndexToPos(i);
                if (map[Ipos.Layer][Ipos.Unit / MapGrid.m_UnitRoomGridNum] != 1) {
                    t.GetComponent<Renderer>().enabled = false;
                    //GameObject.Destroy(t.gameObject);
                    //DicBattle.Remove(i);
                }
            }
            DicHorizontal.Clear();
        }
    }
    public static void Clear()
    {
        DicCorner.Clear();
        DicHorizontal.Clear();
        DicVertical.Clear();
        DicBattle.Clear();
        map.Clear();
        grid.Clear();
        Drooms.Clear();
        DicWall.Clear();
        m_v3RejectPolygon.Clear();
        m_v3OutRejectPolygon.Clear();
        LHitBrokeDeck.Clear();
        LHitDeck.Clear();
        DicBattleVertical.Clear();
        m_goTop.Clear();
        m_goBottom.Clear();
        m_goLeft.Clear();
        m_goRight.Clear();
    }
    public static void GenerateShipsWithAni()
    {
        delay = 0;
        isBattle = false;
        Clear();
        ClearRoomWall();
        ShipPlan p = ShipPlanDC.GetCurShipPlan();
        m_Start = BattleEnvironmentM.GetLifeMBornNode(true);
        m_Boat = m_Start.parent;
        m_totalanitime = 0;
        m_vStart = RoomMap.GetRoomGridLocalPos(p.Canvans.GetStart());
        CenterBuilding();
        
        DoCenterBuilds();
        isAni = true;
        Vector3 viewport = Camera.main.WorldToViewportPoint(m_Start.parent.position);
        float f = 0.5f;
        m_lbpos = Camera.main.ViewportToWorldPoint(new Vector3(0 - f, 0 - f, viewport.z));
        m_rtpos = Camera.main.ViewportToWorldPoint(new Vector3(1 + f, 1 + f, viewport.z));
        
        m_lbpos = m_Start.InverseTransformPoint(m_lbpos);
        m_rtpos = m_Start.InverseTransformPoint(m_rtpos);
        
        BattleEnvironmentM.ResetStartPos(new Int2(shipinfo.width, shipinfo.height), true);
        CreateShip();
        
        GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "2000401_01", m_Boat.transform.position, m_Boat);
        if (gae != null) {
            GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(m_totalanitime + 3f);
            gae.AddAction(ndEffect);
        }
        gae = EffectM.LoadEffect("effect/prefab/", "2000401_02", m_Boat.transform.position, m_Boat);
        if (gae != null) {
            GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(m_totalanitime);
            gae.AddAction(ndEffect);
        }
        gae = m_Boat.gameObject.AddComponent<GameObjectActionExcute>();
        GameObjectActionCreateEffect gace = new GameObjectActionCreateEffect(m_totalanitime, 3f, "2000401_03", m_Boat.position, m_Boat, GotoShipSet);
        gae.AddAction(gace);
        
    }
    
    private static void GotoShipSet(object o)
    {
        ShipCanvasWnd wnd = WndManager.FindDialog<ShipCanvasWnd>();
        if (wnd) {
            wnd.DoReturn();
        }
    }
    public static void GenerateShips()
    {
        isBattle = false;
        ShipPlan plan = ShipPlanDC.GetCurShipPlan();
        if (plan == null) {
            return ;
        }
        shipinfo = plan.Canvans;
        m_Start = BattleEnvironmentM.GetLifeMBornNode(true);
        m_vStart = RoomMap.GetRoomGridLocalPos(shipinfo.GetStart());
        
        CenterBuilding();
        isAni = false;
        Vector3 viewport = Camera.main.WorldToViewportPoint(m_Start.position);
        float f = 0;
        m_lbpos = Camera.main.ViewportToWorldPoint(new Vector3(0 - f, 0 - f, viewport.z));
        m_rtpos = Camera.main.ViewportToWorldPoint(new Vector3(1 + f, 1 + f, viewport.z));
        
        m_lbpos = m_Start.InverseTransformPoint(m_lbpos);
        m_rtpos = m_Start.InverseTransformPoint(m_rtpos);
        Clear();
        ClearRoomWall();
        BattleEnvironmentM.ResetStartPos(new Int2(shipinfo.width, shipinfo.height), true);
        CreateShip();
        CreateRoomEdit();
        
    }
    /// <summary>
    /// 金银岛新场景增加创建船只.
    /// </summary>
    /// <param name="tstart">Tstart.</param>
    public static void GenerateShips(MapStart tstart)
    {
        isBattle = false;
        ShipPlan plan = ShipPlanDC.GetCurShipPlan();
        if (plan == null) {
            return ;
        }
        shipinfo = plan.Canvans;
        m_Start = tstart.transform;
        BattleEnvironmentM.JoinBoatData(tstart);
        m_vStart = RoomMap.GetRoomGridLocalPos(shipinfo.GetStart());
        
        CenterBuilding();
        isAni = false;
        Vector3 viewport = Camera.main.WorldToViewportPoint(m_Start.position);
        float f = 0;
        m_lbpos = Camera.main.ViewportToWorldPoint(new Vector3(0 - f, 0 - f, viewport.z));
        m_rtpos = Camera.main.ViewportToWorldPoint(new Vector3(1 + f, 1 + f, viewport.z));
        
        m_lbpos = m_Start.InverseTransformPoint(m_lbpos);
        m_rtpos = m_Start.InverseTransformPoint(m_rtpos);
        Clear();
        ClearRoomWall();
        //		BattleEnvironmentM.ResetStartPos(new Int2(shipinfo.width, shipinfo.height),true);
        CreateShip();
        CreateRoomEdit();
        
    }
    public static void GenerateShips(ShipCanvasInfo info, Transform tstart, List<BuildInfo> blist, List<SoldierInfo> slist)
    {
        isBattle = false;
        shipinfo = info;
        m_Start = tstart;
        m_vStart = RoomMap.GetRoomGridLocalPos(shipinfo.GetStart());
        BattleEnvironmentM.ResetStartPos(new Int2(shipinfo.width, shipinfo.height), true);
        isAni = false;
        Clear();
        ClearRoomWall();
        isWave = true;
        CreateShip();
        isWave = false;
        foreach (BuildInfo b in blist) {
            if (b != null) {
                CreateViewBuilding(b);
            }
        }
        
        foreach (SoldierInfo s in slist) {
            CreateViewSoilder(s);
        }
        
    }
    
    public static void GenerateShips(ShipCanvasInfo info, Transform tstart, List<battle.ShipBuildInfo> blist, List<battle.SoldierInfo> slist, List<treasure.TreasureObjInfo> statuslist)
    {
        U3DUtil.DestroyAllChild(tstart.gameObject, false);
        isBattle = false;
        shipinfo = info;
        m_Start = tstart;
        m_vStart = RoomMap.GetRoomGridLocalPos(shipinfo.GetStart());
        BattleEnvironmentM.ResetStartPos(new Int2(shipinfo.width, shipinfo.height), true);
        isAni = false;
        Clear();
        ClearRoomWall();
        CreateShip();
        foreach (battle.ShipBuildInfo b in blist) {
            BuildInfo Info = buildingM.GetBuildInfo(b);
            if (Info != null) {
                CreateRoomBuilding(Info);
            }
        }
        
        foreach (battle.SoldierInfo s in slist) {
            /*foreach (treasure.TreasureObjInfo to in statuslist)
            {
            	if (s.id == to.objid)
            	{
            		SoldierInfo i = SoldierM.GetSoldierInfo(s);
            		if (to.hp != 0)
            		{
            			//CreateSoilder(i);
            			Transform t = CreateSoilder(i);
            
            
            			RolePropertyM rpm = t.gameObject.GetComponentInChildren<RolePropertyM>();
            			if (rpm != null)
            			{
            				rpm.SetCampModel(false,LifeMCamp.ATTACK);
            			}
            		}
            		else
            		{
            			CreateGrayInsteadSoilder(i,false);
            		}
            	}
            }*/
            SoldierInfo i = SoldierM.GetSoldierInfo(s);
            //CreateSoilder(i);
            Role r = CreateSoilder(i);
            
            r.RoleSkinCom.SetCampModel(false);
        }
        
        /*foreach (treasure.TreasureObjInfo s in statuslist)
        {
        	SoldierInfo i = SoldierM.GetSoldierInfo(s);
        	if(i.m_hp != 0)
        	{
        		Transform t = CreateSoilder(i);
        
        
        		RolePropertyM rpm = t.gameObject.GetComponentInChildren<RolePropertyM>();
        		if (rpm != null)
        		{
        			rpm.SetCampModel(false,LifeMCamp.ATTACK);
        		}
        	}
        	else
        	{
        		CreateGrayInsteadSoilder(i,false);
        	}
        }*/
    }
    
    
    public static void GenerateShipsBattle(ShipCanvasInfo map, Transform tstart)
    {
        isBattle = true;
        shipinfo = map;
        //Debug.Log("GenerateShipsBattle  " + map.width + "," + map.height + "," + map.shape + "," + map.map);
        m_Start = tstart;
        isAni = false;
        Clear();
        ClearRoomWall();
        isWave = true;
        CreateShip();
        isWave = false;
        CalcHitDeck();
    }
    
    
    public static void CreateViewBuilding(BuildInfo b)
    {
        if (b == null) {
            return;
        }
        
        Vector3 local = GetRealPos(b.m_cx * MapGrid.m_width, b.m_cy * 3 + b.m_cy * 0.3f, 0);
        Vector3 world = m_Start.TransformPoint(local);
        Building buildlife = LifeFactory.CreateBuilding(b, 0, m_Start, world, LifeEnvironment.View);
        
        if (buildlife == null) {
            return ;
        }
        
        
        if (b.m_RoomType == RoomType.NormalTrap || b.m_RoomType == RoomType.DeckTrap || b.m_RoomType == RoomType.ResRoom) {
            BuildColider Colider = buildlife.GetComponent<BuildColider>();
            Colider.EnableEditorCollider(true);
            
            StageClickInfo Click = buildlife.AddComponent<StageClickInfo>();
            Click.SetInfo(b);
        }
    }
    
    
    
    public static void CreateViewSoilder(SoldierInfo s)
    {
        if (s == null) {
            return;
        }
        /*IGameRole igr = GameRoleFactory.Create(m_Start, s.m_modeltype, s.m_name, AnimatorState.Stand);
        GameObject go = igr.GetGameRoleRootObj();
        go.transform.rotation = Quaternion.identity;
        go.transform.localPosition = GetRealPos((s.CX ) * MapGrid.m_width, s.CY * 3 + s.CY * 0.3f, 0);
        go.transform.localScale = Vector3.one;*/
        Vector3 local = GetRealPos((s.CX) * MapGrid.m_width, s.CY * 3 + s.CY * 0.3f, 0);
        Vector3 world = m_Start.TransformPoint(local);
        Role rolelife = LifeFactory.CreateRole(m_Start, s, true, world, AnimatorState.Stand, true);
        //RolePropertyM RoleProperty = rolelife.m_Property;
        rolelife.RoleSkinCom.EnableColider(ColiderType.Click, true);
        
        StageClickInfo Click = rolelife.m_thisT.gameObject.AddComponent<StageClickInfo>();
        Click.SetInfo(s);
    }
    
    public static void CreateGrayInsteadSoilder(SoldierInfo s, bool isMe)
    {
        string effect = "2000231_01";
        if (isMe) {
            effect = "2000231_02";
        }
        
        GameObject build = GameObjectLoader.LoadPath("effect/prefab/", effect, m_Start);
        if (build == null) {
            Debug.Log("坟墓资源不存在");
            return;
        }
        build.transform.rotation = Quaternion.identity;
        build.transform.Rotate(0, 180, 0, Space.Self);
        Int2 pos = new Int2(s.CX, s.CY);
        build.transform.localPosition = GetRealPos((s.CX - 1) * MapGrid.m_width, s.CY * 3 + s.CY * 0.3f, 0);
    }
    
    
    
    
    public static void CalcHitDeck()
    {
        if (LHitDeck.Count == 0) {
            foreach (int k in DicBattle.Keys) {
                if (DicBattle[k] != null) {
                    IggFloor nf = DicBattle[k].GetComponent<LifeProperty>().GetLife() as IggFloor;
                    if (nf != null) {
                        if (nf.m_FloorType == FloorType.top || nf.m_FloorType == FloorType.bottom || nf.m_FloorType == FloorType.Normal) {
                            bool canhit = true;
                            for (int i = nf.Layer + 1; i <= shipinfo.height; i++) {
                                if (DicBattle.ContainsKey(GetIndex(nf.StartUnit / MapGrid.m_UnitRoomGridNum, i))) {
                                    canhit = false;
                                    break;
                                }
                            }
                            if (canhit) {
                                LHitDeck.Add(new HitDeckInfo(nf, nf.StartUnit, nf.Layer, LocationType.Top));
                            }
                        }
                    }
                    
                }
            }
            foreach (int k in DicBattleVertical.Keys) {
                if (DicBattleVertical[k] != null) {
                    IggFloor nf = DicBattleVertical[k].GetComponent<LifeProperty>().GetLife() as IggFloor;
                    if (nf != null) {
                        if (nf.m_FloorType == FloorType.left) {
                            bool canhit = true;
                            for (int i = nf.StartUnit / MapGrid.m_UnitRoomGridNum - 1; i >= 0; i--) {
                                if (DicBattleVertical.ContainsKey(GetIndex(i, nf.Layer))) {
                                    canhit = false;
                                    break;
                                }
                            }
                            if (canhit) {
                                LHitDeck.Add(new HitDeckInfo(nf, nf.StartUnit, nf.Layer, LocationType.Left));
                            }
                        } else if (nf.m_FloorType == FloorType.right) {
                            bool canhit = true;
                            for (int i = (nf.StartUnit + 1) / MapGrid.m_UnitRoomGridNum + 1; i <= shipinfo.width; i++) {
                                if (DicBattleVertical.ContainsKey(GetIndex(i, nf.Layer))) {
                                    canhit = false;
                                    break;
                                }
                            }
                            if (canhit) {
                                LHitDeck.Add(new HitDeckInfo(nf, nf.StartUnit, nf.Layer, LocationType.Right));
                            }
                        }
                    } else {
                        LeftFloorWall lfw = DicBattleVertical[k].GetComponent<LifeProperty>().GetLife() as LeftFloorWall;
                        if (lfw != null) {
                            bool canhit = true;
                            for (int i = lfw.StartUnit / MapGrid.m_UnitRoomGridNum - 1; i >= 0; i--) {
                                if (DicBattleVertical.ContainsKey(GetIndex(i, lfw.Layer))) {
                                    canhit = false;
                                    break;
                                }
                            }
                            if (canhit) {
                                LHitDeck.Add(new HitDeckInfo(lfw, lfw.StartUnit, lfw.Layer, LocationType.Left));
                            }
                        } else {
                            rightFloorWall rfw = DicBattleVertical[k].GetComponent<LifeProperty>().GetLife() as rightFloorWall;
                            if (rfw != null) {
                                bool canhit = true;
                                for (int i = (rfw.StartUnit + 1) / MapGrid.m_UnitRoomGridNum + 1; i <= shipinfo.width; i++) {
                                    if (DicBattleVertical.ContainsKey(GetIndex(i, rfw.Layer))) {
                                        canhit = false;
                                        break;
                                    }
                                }
                                if (canhit) {
                                    LHitDeck.Add(new HitDeckInfo(rfw, rfw.StartUnit, rfw.Layer, LocationType.Right));
                                }
                            }
                        }
                    }
                }
            }
        } else {
            for (int i = 0; i < LHitDeck.Count;) {
                if (LHitDeck[i].m_target == null) {
                    if (LHitDeck[i].m_type == LocationType.Top) {
                        //DicBattle.Remove(GetIndex(LHitDeck[i].m_unit,LHitDeck[i].m_layer));
                        for (int j = LHitDeck[i].m_layer - 1; j >= 0; j--) {
                            if (DicBattle.ContainsKey(GetIndex(LHitDeck[i].m_unit / MapGrid.m_UnitRoomGridNum, j)) && DicBattle[GetIndex(LHitDeck[i].m_unit / MapGrid.m_UnitRoomGridNum, j)] != null) {
                                IggFloor nf = DicBattle[GetIndex(LHitDeck[i].m_unit / MapGrid.m_UnitRoomGridNum, j)].GetComponent<LifeProperty>().GetLife() as IggFloor;
                                LHitDeck.Add(new HitDeckInfo(nf, nf.StartUnit, nf.Layer, LocationType.Top));
                                //i++;
                                break;
                            }
                        }
                    } else if (LHitDeck[i].m_type == LocationType.Left) {
                        //DicVertical.Remove(GetIndex(LHitDeck[i].m_unit,LHitDeck[i].m_layer));
                        
                        for (int j = LHitDeck[i].m_unit / MapGrid.m_UnitRoomGridNum + 1; j <= shipinfo.width; j++) {
                            if (DicBattleVertical.ContainsKey(GetIndex(j, LHitDeck[i].m_layer)) && DicBattleVertical[GetIndex(j, LHitDeck[i].m_layer)] != null) {
                                Life nf = DicBattleVertical[GetIndex(j, LHitDeck[i].m_layer)].GetComponent<LifeProperty>().GetLife() as Life;
                                LHitDeck.Add(new HitDeckInfo(nf, j, LHitDeck[i].m_layer, LocationType.Left));
                                //i++;
                                break;
                            }
                        }
                    } else if (LHitDeck[i].m_type == LocationType.Right) {
                        //DicVertical.Remove(GetIndex(LHitDeck[i].m_unit,LHitDeck[i].m_layer));
                        
                        for (int j = LHitDeck[i].m_unit / MapGrid.m_UnitRoomGridNum - 1; j >= 0; j--) {
                            if (DicBattleVertical.ContainsKey(GetIndex(j, LHitDeck[i].m_layer)) && DicBattleVertical[GetIndex(j, LHitDeck[i].m_layer)] != null) {
                                Life nf = DicBattleVertical[GetIndex(j, LHitDeck[i].m_layer)].GetComponent<LifeProperty>().GetLife()as Life;
                                LHitDeck.Add(new HitDeckInfo(nf, j, LHitDeck[i].m_layer, LocationType.Left));
                                //i++;
                                break;
                            }
                        }
                    }
                    LHitBrokeDeck.Add(LHitDeck[i]);
                    LHitDeck.RemoveAt(i);
                } else {
                    i++;
                }
                
            }
        }
    }
    public static void ClearRoomWall()
    {
        foreach (GameObject go in RoomWall) {
            if (go) {
                GameObject.DestroyImmediate(go);
            }
        }
        RoomWall.Clear();
    }
    public static void CreateMiddleBeam()
    {
        isBattle = false;
        isAni = false;
        ClearRoomWall();
        
        shipinfo = ShipPlanDC.GetCurShipPlan().Canvans;
        //NGUIUtil.DebugLog(shipinfo.map);
        m_Start = BattleEnvironmentM.GetLifeMBornNode(true);
        m_vStart = RoomMap.GetRoomGridLocalPos(shipinfo.GetStart());
        grid = shipinfo.GetShape();
        map = shipinfo.GetMap();
        CreateWall();
        /*
        Int2 start = new Int2(-1, 0);
        for (int j = 0; j < grid.Count; j++)
        {
            for (int k = 0; k < grid[j].Count; k++)
            {
                if (grid[j][k] == 1)
                {
                    float y = (j + 1) * roomwidth + j * beamwith;
                    float x = k * roomwidth;
        
        
                    if (j < (grid.Count - 1))
                    {
                        if (grid[j + 1][k] == 1)
                        {
                            if (map[j + 1][k] == 1)
                            {
                                Createbeam(x, y, LocationType.Middle, k, j + 1);
                            }
                        }
                    }
                }
            }
        }
         */
    }
    public static void ResetBuildingToCanvas(float duration, GameObject completetarget, string completefun)
    {
    
    
        SetBuildingParent(ShipBuildType.BuildRoom, null);
        SetBuildingParent(ShipBuildType.BuildStair, null);
        SetBuildingParent(ShipBuildType.Soldier, null);
        BattleEnvironmentM.ResetStartPos(new Int2(8, 4), false);
        MoveNewGameobject();
        DoResetBuildingToCanvas(ShipBuildType.BuildRoom, duration, completetarget, completefun);
        DoResetBuildingToCanvas(ShipBuildType.BuildStair, duration, null, completefun);
        DoResetBuildingToCanvas(ShipBuildType.Soldier, duration, null, completefun);
        
        //GameObject.Destroy(m_Start.gameObject);
    }
    public static void SetBuildingParent(ShipBuildType t, Transform tfParent)
    {
        Int2 start = shipinfo.GetStart();
        ShipPlan p = ShipPlanDC.GetCurShipPlan();
        List<ShipPutInfo> rooms = p.GetShipBuildInfo(t);
        if (rooms != null) {
            for (int i = 0; i < rooms.Count; i++) {
                if (Drooms.ContainsKey(rooms[i].id)) {
                    Transform tbuild = Drooms[rooms[i].id];
                    tbuild.parent = tfParent;
                } else {
                }
            }
        }
    }
    public static void DoResetBuildingToCanvas(ShipBuildType t, float duration, GameObject completetarget, string completefun)
    {
        Int2 start = shipinfo.GetStart();
        ShipPlan p = ShipPlanDC.GetCurShipPlan();
        List<ShipPutInfo> rooms = p.GetShipBuildInfo(t);
        if (rooms != null) {
            for (int i = 0; i < rooms.Count; i++) {
                Transform tbuild;
                Drooms.TryGetValue(rooms[i].id, out tbuild);
                if (tbuild == null) {
                
                    continue;
                }
                if (m_Start == null) {
                    m_Start = BattleEnvironmentM.GetLifeMBornNode(true);
                }
                tbuild.parent = m_Start.parent;
                Vector3 pos = new Vector3((rooms[i].cxMapGrid + start.Unit) * MapGrid.m_width, (rooms[i].cyMapGrid + start.Layer) * (roomwidth + 0.3f));
                if (i == 0 && completetarget != null) {
                    //iTween.MoveTo(tbuild.gameObject, iTween.Hash("position", pos, "time", duration, "islocal", true, "easetype", iTween.EaseType.easeInCubic, "oncompletetarget", completetarget, "oncomplete", completefun));
                    tbuild.transform.DOMove(pos, duration);
                } else {
                    //iTween.MoveTo(tbuild.gameObject, iTween.Hash("position", pos, "time", duration, "islocal", true, "easetype", iTween.EaseType.easeInCubic));
                    tbuild.transform.DOMove(pos, duration);
                }
                //tbuild.localPosition = PutCanvansM.GetLocalPos(new Int2(rooms[i].m_cx + start.Unit,rooms[i].m_cy + start.Layer));
            }
        }
    }
    
    public static void MoveNewGameobject()
    {
        Int2 start = shipinfo.GetStart();
        Vector3 pos = new Vector3(start.Unit * MapGrid.m_width, start.Layer * (roomwidth + 0.3f));
        pos += m_Start.transform.localPosition;
        //iTween.MoveTo(m_Start.gameObject, iTween.Hash("position", pos, "time", .35f, "islocal", true, "easetype", iTween.EaseType.easeInCubic));
        m_Start.transform.DOMove(pos, 0.35f);
        //NGUIUtil.PauseGame();
    }
    
    public static void CenterBuilding()
    {
        Int2 canvanssize = RoomMap.RealMapSize;
        Int2 shipsize = shipinfo.GetMapSize();
        m_vStart = new Vector3(0, 0, 0);//new Vector3((canvanssize.Unit - shipsize.Unit) * roomwidth / 2f,(canvanssize.Layer - shipsize.Layer) * roomwidth / 2f,0);
        GameObject go = new GameObject();
        go.transform.parent = m_Start;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localPosition = m_vStart;
        m_Start = go.transform;
    }
    public static void DoCenterBuilds()
    {
        ShipPlan p = ShipPlanDC.GetCurShipPlan();
        List<ShipPutInfo> rooms = p.GetShipBuildInfo(ShipBuildType.All);
        if (rooms != null) {
            foreach (ShipPutInfo r in rooms) {
                ShipBuildType type = ShipBuildType.BuildRoom;
                
                if (r.type == (int)ShipBuildType.BuildRoom) {
                    type = ShipBuildType.BuildRoom;
                } else if (r.type == (int)ShipBuildType.Soldier) {
                    type = ShipBuildType.Soldier;
                } else if (r.type == (int)ShipBuildType.BuildStair) {
                    type = ShipBuildType.BuildStair;
                }
                
                Transform tbuild = GetBuildTransform(type, r.id);
                if (tbuild != null) {
                    tbuild.parent = m_Start;
                    tbuild.localPosition = RoomMap.GetShipBuildLocalPos(new Int2(r.cxMapGrid, r.cyMapGrid), type);
                }
            }
        }
    }
    public static Transform GetBuildTransform(ShipBuildType stype, int id)
    {
        CanvasCore core = new CanvasCore();
        core.m_ID = id;
        core.m_type = stype;
        TouchMove tm = TouchMoveManager.GetShipBuild(core);
        if (tm == null) {
            return null;
        }
        return tm.transform;
    }
    public static void CreateShip()
    {
        grid = shipinfo.GetShape();
        map = shipinfo.GetMap();
        CreateWall();
        int width = shipinfo.GetMapSize().Unit;
        int height = shipinfo.GetMapSize().Layer;
        int layer = height > 2 ? 2 : 1;
        int head = layer;
        if (height == 4) {
            head = 4;
        }
        GameObject shipbow = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + head + "01@Skin", m_Start);
        BoatHead = shipbow.transform;
        m_goLeft.Add(shipbow);
        FlyToPos(shipbow, LocationType.Left, 0, 0, 0, true, 2);
        //if (isWave)
        //{
        //GameObject go2001021_01  = GameObjectLoader.LoadPath("effect/prefab/", "2001021_01", shipbow.transform);
        //}
        GameObject hullfront = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + layer + "02@Skin", m_Start);
        m_goBottom.Add(hullfront);
        FlyToPos(hullfront, LocationType.Bottom, 0, 0, 0, true, 1);
        if (width > 2) {
            for (int i = 0; i < (width - 2); i++) {
                GameObject hullmid = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + layer + "03@Skin", m_Start);
                m_goBottom.Add(hullmid);
                FlyToPos(hullmid, LocationType.Bottom, roomwidth * (i + 1), 0, 0, true, 1);
            }
        } else {
            width = 2;
        }
        GameObject hullback = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + layer + "04@Skin", m_Start);
        m_goBottom.Add(hullback);
        FlyToPos(hullback, LocationType.Bottom, roomwidth * (width - 1), 0, 0, true, 1);
        GameObject hulltail = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + layer + "05@Skin", m_Start);
        BoatTail = hulltail.transform;
        m_goRight.Add(hulltail);
        //if (isWave)
        //{
        //	GameObject go2001021_02  = GameObjectLoader.LoadPath("effect/prefab/", "2001021_02", hulltail.transform);
        //}
        FlyToPos(hulltail, LocationType.Right, roomwidth * width, 0, 0, true, 2);
        
        CreateMast(grid[grid.Count - 1]);
        //创建配件
        CreateAccessories();
        //创建环境
        sdata.s_environmentInfo environment = StageM.GetEnvironmentInfo(shipinfo.environment);
        SeaEnviron sea = BattleEnvironmentM.GetSeaData();
        if (sea != null && environment != null) {
            GameObjectLoader.LoadPath("effect/prefab/", environment.ground, sea.transform);
        }
        SkyEnviron sky = BattleEnvironmentM.GetSkyData();
        if (sky != null && environment != null) {
            GameObject go =  GameObjectLoader.LoadPath("Prefabs/ShipEnvionment/", environment.sky, sky.SkyHelp);
            GameObjectLoader.LoadPath("effect/prefab/", environment.distant_view, go.transform);
            string[] ss = environment.climate.Split(',');
            foreach (string s in ss) {
                GameObjectLoader.LoadPath("effect/prefab/", s, go.transform);
            }
        }
    }
    public static void CreateAccessoriesGameObject(string name, float x, float y, Int2 start, Int2 end, bool ismirr)
    {
    
        GameObject go = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + name, m_Start);
        go.transform.localPosition = new Vector3(x, y, 0);
        if (ismirr) {
            go.transform.localScale = new Vector3(-1, 1, 1);
        }
        Accessories a = go.AddComponent<Accessories>();
        a.Init(start, end);
    }
    public static void CreateAccessories()
    {
        int toplayer = 0;
        GameObject go = null;
        List<int> top = new List<int>();
        for (int i = 0; i < shipinfo.width; i++) {
            top.Add(-1);
        }
        for (int i = shipinfo.height - 1; i >= 0; i--) {
            int cout = 0;
            for (int j = 0; j < shipinfo.width; j++) {
                if (grid[i][j] == 0) {
                    //创建夹角配件
                    if (i > 0) {
                        if (j < (shipinfo.width - 1) && grid[i][j + 1] == 1 && i > 0 && grid[i - 1][j] == 1) {
                            //go = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + "303@Skin", m_Start);
                            float x = (j + 1) * roomwidth;
                            float y = i * (beamwith + roomwidth);
                            //go.transform.localPosition = new Vector3(x,y,0);
                            //go.transform.localScale = new Vector3(-1,1,1);
                            CreateAccessoriesGameObject("303@Skin", x, y, new Int2(j, i - 1), new Int2(j, i - 1), true);
                        }
                        if (j > 0 && grid[i][j - 1] == 1 &&  i > 0 && grid[i - 1][j] == 1) {
                            //go = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + "303@Skin", m_Start);
                            float x = j * roomwidth;
                            float y = i * (beamwith + roomwidth);
                            //go.transform.localPosition = new Vector3(x,y,0);
                            CreateAccessoriesGameObject("303@Skin", x, y, new Int2(j, i - 1), new Int2(j, i - 1), false);
                        }
                    }
                } else {
                    //创建顶角配件
                    if ((j == 0 || grid[i][j - 1] == 0) &&
                        (i >= (shipinfo.height - 1) || grid[i + 1][j] == 0) &&
                        (j == 0 || i == (shipinfo.height - 1) || grid[i + 1][j - 1] == 0)) {
                        //go = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + "119@Skin", m_Start);
                        float x = j * roomwidth;
                        float y = (i + 1) * (beamwith + roomwidth) - beamwith;
                        //go.transform.localPosition = new Vector3(x,y,0);
                        //go.transform.localScale = new Vector3(-1,1,1);
                        CreateAccessoriesGameObject("119@Skin", x, y, new Int2(j, i), new Int2(j, i), true);
                    }
                    if ((j == (shipinfo.width - 1) || grid[i][j + 1] == 0) &&
                        (i >= (shipinfo.height - 1) || grid[i + 1][j] == 0) &&
                        (j == (shipinfo.width - 1) || i == (shipinfo.height - 1) || grid[i + 1][j + 1] == 0)) {
                        //go = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + "119@Skin", m_Start);
                        float x = (j + 1) * roomwidth;
                        float y = (i + 1) * (beamwith + roomwidth) - beamwith;
                        //go.transform.localPosition = new Vector3(x,y,0);
                        CreateAccessoriesGameObject("119@Skin", x, y, new Int2(j, i), new Int2(j, i), false);
                    }
                    /*if (shipinfo.height > 1 )
                    {
                    	if (j == 0)
                    	{
                    		if (i > 0 && i == shipinfo.height - 1)
                    		{
                    			go = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + "119@Skin", m_Start);
                    			float x = j * roomwidth;
                    			float y = (i + 1) * (beamwith + roomwidth) - beamwith;
                    			go.transform.localPosition = new Vector3(x,y,0);
                    			go.transform.localScale = new Vector3(-1,1,1);
                    		}
                    		if ((j == (shipinfo.width -1) || grid[i][j + 1] == 0) && (i >= (shipinfo.height - 1) || grid[i + 1][j] == 0))
                    		{
                    			go = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + "119@Skin", m_Start);
                    			float x = (j + 1) * roomwidth;
                    			float y = (i + 1) * (beamwith + roomwidth) - beamwith;
                    			go.transform.localPosition = new Vector3(x,y,0);
                    		}
                    	}
                    	else if (j == (shipinfo.width -1))
                    	{
                    		if ((j == 0 || grid[i][j - 1] == 0) && (i >= (shipinfo.height - 1) || grid[i + 1][j] == 0))
                    		{
                    			go = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + "119@Skin", m_Start);
                    			float x = j * roomwidth;
                    			float y = (i + 1) * (beamwith + roomwidth) - beamwith;
                    			go.transform.localPosition = new Vector3(x,y,0);
                    			go.transform.localScale = new Vector3(-1,1,1);
                    		}
                    		if (i > 0 && i == shipinfo.height - 1 )
                    		{
                    			go = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + "119@Skin", m_Start);
                    			float x = (j + 1) * roomwidth;
                    			float y = (i + 1) * (beamwith + roomwidth) - beamwith;
                    			go.transform.localPosition = new Vector3(x,y,0);
                    		}
                    	}
                    	else
                    	{
                    		if (grid[i][j - 1] == 0 && (i >= (shipinfo.height - 1) || grid[i + 1][j] == 0))
                    		{
                    			go = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + "119@Skin", m_Start);
                    			float x = j * roomwidth;
                    			float y = (i + 1) * (beamwith + roomwidth) - beamwith;
                    			go.transform.localPosition = new Vector3(x,y,0);
                    			go.transform.localScale = new Vector3(-1,1,1);
                    		}
                    		if (grid[i][j + 1] == 0 && (i >= (shipinfo.height - 1) || grid[i + 1][j] == 0))
                    		{
                    			go = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + "119@Skin", m_Start);
                    			float x = (j + 1) * roomwidth;
                    			float y = (i + 1) * (beamwith + roomwidth) - beamwith;
                    			go.transform.localPosition = new Vector3(x,y,0);
                    		}
                    	}
                    }*/
                    //创建顶部配件，栏杆
                    if (top[j] == -1 || grid[i + 1][j] == 0) {
                        top[j] = i;
                    }
                    if (i == (shipinfo.height - 1) || grid[i + 1][j] == 0) {
                        //go = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + "302@Skin", m_Start);
                        float x = j * roomwidth;
                        float y = (i + 1) * (beamwith + roomwidth);
                        //go.transform.localPosition = new Vector3(x,y,0);
                        CreateAccessoriesGameObject("302@Skin", x, y, new Int2(j, i), new Int2(j, i), false);
                    }
                    if (shipinfo.width > 3 && j > 0) {
                    
                        if (top[j] == top[j - 1]) {
                            if (j == (shipinfo.width - 1)) {
                                if (cout >= 2) {
                                    CreateTopRoom(cout, j - cout, j - 1, i + 1);
                                }
                            } else if (i == (shipinfo.height - 1)) {
                                if (grid[i][j + 1] == 0) {
                                    if (cout >= 2) {
                                        CreateTopRoom(cout, j - cout, j - 1, i + 1);
                                    }
                                } else {
                                    cout ++;
                                }
                            } else if (grid[i + 1][j] == 0 && grid[i][j + 1] == 0 && grid[i + 1][j + 1] == 0) {
                                if (cout >= 2) {
                                    CreateTopRoom(cout, j - cout, j - 1, i + 1);
                                }
                            } else if (grid[i + 1][j] == 0 && grid[i + 1][j + 1] == 1) {
                                cout++;
                                if (cout >= 2) {
                                    CreateTopRoom(cout, j - cout + 1, j, i + 1);
                                }
                            } else if (grid[i + 1][j] == 0 && grid[i + 1][j + 1] == 0 && grid[i][j + 1] == 1) {
                                cout++;
                            }
                        } else if (top[j] < top[j - 1]) {
                            cout ++;
                        } else {
                            cout = 0;
                        }
                    }
                }
            }
        }
        /*if (top.Count > 3)
        {
        	int cout = 0;
        	for (int i = 1; i < top.Count; i++)
        	{
        		if (top[i] == top[i-1])
        		{
        			if (i == top.Count -1 || top[i] > top[i+1])
        			{
        				if (cout >= 2)
        					CreateTopRoom(cout,i-cout,i - 1,top[i] + 1);
        			}
        			else if (top[i] < top[i+1])
        			{
        				cout++;
        				if (cout >= 2)
        					CreateTopRoom(cout,i-cout + 1,i,top[i] + 1);
        			}
        			else
        				cout++;
        		}
        		else if (top[i] < top[i-1])
        			cout ++;
        		else
        			cout = 0;
        	}
        }*/
    }
    public static void CreateTopRoom(int cout, int start, int end, int layer)
    {
    
        int cout3 = cout / 3;
        int cout2 = (cout - 3 * cout3) / 2;
        for (int i = 0; i < cout3; i++) {
            //GameObject go = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + "120@Skin", m_Start);
            float x = start * roomwidth + i * 3 * roomwidth + 1.5f * roomwidth;
            float y = layer * (beamwith + roomwidth);
            //go.transform.localPosition = new Vector3(x,y,0);
            CreateAccessoriesGameObject("120@Skin", x, y, new Int2(start + i * 3, layer - 1), new Int2(start + i * 3 + 2, layer - 1), false);
        }
        for (int i = 0; i < cout2; i++) {
            //GameObject go = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + "117@Skin", m_Start);
            float x = start * roomwidth +  cout3 * 3 * roomwidth + i * 2 * roomwidth +  roomwidth;
            float y = layer * (beamwith + roomwidth);
            //go.transform.localPosition = new Vector3(x,y,0);
            CreateAccessoriesGameObject("117@Skin", x, y, new Int2(start + cout3 * 3 + i * 2, layer - 1), new Int2(start + cout3 * 3 + i * 2 + 1, layer - 1), false);
        }
    }
    //获取船帆位置的优先级
    public static int GetMastPos(List<int> poss)
    {
        int pos = 9;
        for (int i = 0; i < poss.Count; i++) {
            pos = CheckPriority(pos, poss[i]);
        }
        return pos;
    }
    public static int CheckPriority(int oldpos, int newpos)
    {
        int[] priorityrule = new int[] {8, 7, 5, 3, 1, 2, 4, 6, 9};
        int oldpriority = oldpos < 8 ? priorityrule[oldpos] : priorityrule[8];
        int newpriority = newpos < 8 ? priorityrule[newpos] : priorityrule[8];
        if (oldpriority > newpriority) {
            return newpos;
        }
        return oldpos;
    }
    
    public static void CreateMast(List<int> toprow)
    {
        List<List<int>> area = new List<List<int>>();
        bool f = false;
        int maxcount = 0;
        //找到每个连续段
        for (int k = 0; k < toprow.Count; k++) {
            if (toprow[k] != 0) {
                //判断是否重新开始计算段
                if (!f) {
                    //计算所有段中最大的宽度
                    if (area.Count > 0 && area[area.Count - 1].Count > maxcount) {
                        maxcount = area[area.Count - 1].Count;
                    }
                    f = true;
                    List<int> a = new List<int>();
                    a.Add(k);
                    area.Add(a);
                } else {
                    area[area.Count - 1].Add(k);
                }
            } else {
                f = false;
            }
            
        }
        if (area.Count > 0 && area[area.Count - 1].Count > maxcount) {
            maxcount = area[area.Count - 1].Count;
        }
        int mid = -1;
        if (area.Count == 1) {
            if (area[0].Count >= 3) {
                area[0].RemoveAt(0);
                area[0].RemoveAt(area[0].Count - 1);
            }
            mid = GetMastPos(area[0]);
        } else if (area.Count > 1) {
            //如果可以放大船帆，就把放小船帆的地方排除
            if (maxcount > 2) {
                for (int i = 0; i < area.Count;) {
                    if (area[i].Count < 3) {
                        area.RemoveAt(i);
                    } else {
                        area[i].RemoveAt(0);
                        area[i].RemoveAt(area[i].Count - 1);
                        i++;
                    }
                }
            }
            List<int> put = new List<int>();
            if (area.Count > 1) {
                for (int i = 0; i < area.Count; i++) {
                    put.AddRange(area[i]);
                }
            } else {
                put.AddRange(area[0]);
            }
            mid = GetMastPos(put);
        }
        if (mid != -1) {
            float x = mid * roomwidth + roomwidth / 2;
            float y = shipinfo.GetMapSize().Layer * (beamwith + roomwidth) - beamwith;
            string masttype = maxcount > 2 ? "301@Skin" : "300@Skin";
            
            GameObject mast = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + masttype, m_Start);
            BoatMast = mast.transform;
            m_goTop.Add(mast);
            FlyToPos(mast, LocationType.Top, x, y, 8, true, 3);
        } else {
        
#if UNITY_EDITOR_LOG
            Debug.Log("船帆没位置");
#endif
        }
    }
    public static void CreateWall()
    {
        Int2 start = new Int2(-1, 0);
        for (int j = 0; j < grid.Count; j++) {
            for (int k = 0; k < grid[j].Count; k++) {
                if (grid[j][k] == 0) {
                    if (j < (grid.Count - 1)) {
                        if (grid[j + 1][k] == 1) {
                            float y = (j + 1) * (roomwidth + beamwith);
                            float x = k * roomwidth;
                            RoomWall.Add(Createbeam(x, y, LocationType.Bottom, k, j + 1));
                        }
                    }
                    if (k < (grid[j].Count - 1)) {
                        if (grid[j][k + 1] == 1) {
                            Createbeam((k + 1) * roomwidth, j * (roomwidth + beamwith), LocationType.Left, k + 1, j);
                            CreateCorner(k + 1, j + 1, LocationType.TopLeft);
                            if (j == 0 || (grid[j - 1][k] == 1 && grid[j - 1][k + 1] == 1) || (grid[j - 1][k] == 0 && grid[j - 1][k + 1] == 0)) {
                                CreateCorner(k + 1, j, LocationType.BottomLeft);
                            }
                        }
                    }
                    
                } else if (grid[j][k] == 1) {
                    GameObject rbg = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + "107@Skin", m_Start);
                    if (isBattle) {
                        BuildingRoomBg bg = rbg.GetComponent<BuildingRoomBg>();
                        if (bg != null) {
                            bg.m_pos = new Int2(k * MapGrid.m_UnitRoomGridNum  +  MapGrid.m_UnitRoomGridNum / 2, j);
                            bool l = false;
                            bool r = false;
                            bool t = false;
                            if (j >= shipinfo.height - 1 || grid[j + 1][k] == 0) {
                                t = true;
                            }
                            if (k <= 0 || grid[j][k - 1] == 0) {
                                l = true;
                            }
                            if (k >= shipinfo.width - 1 || grid[j][k + 1] == 0) {
                                r = true;
                            }
                            if (t && !l && !r) {
                                bg.m_type = roombgtype.t;
                            }
                            if (l && !r) {
                                bg.m_type = roombgtype.l;
                            }
                            if (!l && r) {
                                bg.m_type = roombgtype.r;
                            }
                            if (l && r) {
                                bg.m_type = roombgtype.lr;
                            }
                            if (t && l && r) {
                                bg.m_type = roombgtype.lrt;
                            }
                        }
                    }
                    RoomWall.Add(rbg);
                    rbg.transform.localPosition = GetRealPos(k * roomwidth, j * (roomwidth + beamwith), 0f);
                    float y = (j + 1) * roomwidth + j * beamwith;
                    float x = k * roomwidth;
                    
                    if (j == 0) {
                        if (start.Unit < 0) {
                            start.Unit = k;
                        }
                        Createbeam(x, 0, LocationType.Bottom, k, 0);
                    }
                    if (j == (grid.Count - 1)) {
                        if (((isBattle || isAni) && map[j + 1][k] == 2) || map[j + 1][k] == 1) {
                            Createbeam(x, y, LocationType.Top, k, j + 1);
                        }
                    } else if (j < (grid.Count - 1)) {
                        if (grid[j + 1][k] == 1) {
                            if (map[j + 1][k] == 1 || ((isBattle || isAni) && map[j + 1][k] == 2)) {
                                Createbeam(x, y, LocationType.Middle, k, j + 1);
                            }
                            
                            GameObject bg = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + "114@Skin", m_Start);
                            RoomWall.Add(bg);
                            bg.transform.localPosition = GetRealPos(x, y, 0f);
                        } else {
                            if (((isBattle || isAni) && map[j + 1][k] == 2) || map[j + 1][k] == 1) {
                                Createbeam(x, y, LocationType.Top, k, j + 1);
                            }
                        }
                    }
                    
                    y = j * (roomwidth + beamwith);
                    x = (k + 1) * roomwidth;
                    if (k == 0) {
                    
                        Createbeam(0, y, LocationType.Left, 0, j);
                        CreateCorner(k, j + 1, LocationType.TopLeft);
                        if (j == 0 || grid[j - 1][k] == 0) {
                            CreateCorner(k, j, LocationType.BottomLeft);
                        }
                    }
                    if (k == (grid[j].Count - 1)) {
                    
                        Createbeam(x, y, LocationType.Right, k + 1, j);
                        CreateCorner(k + 1, j + 1, LocationType.TopRight);
                        if (j == 0 || grid[j - 1][k] == 0) {
                            CreateCorner(k + 1, j, LocationType.BottomRight);
                        }
                    } else if (k < (grid[j].Count - 1)) {
                        if (grid[j][k + 1] == 0) {
                            Createbeam(x, y, LocationType.Right, k + 1, j);
                            CreateCorner(k + 1, j + 1, LocationType.TopRight);
                            if (j == 0 || (grid[j - 1][k] == 1 && grid[j - 1][k + 1] == 1) || (grid[j - 1][k] == 0 && grid[j - 1][k + 1] == 0)) {
                                CreateCorner(k + 1, j, LocationType.BottomRight);
                            }
                        }
                    }
                } else {
#if UNITY_EDITOR_LOG
                    Debug.Log("数据有问题");
#endif
                }
            }
        }
        
        if (isAni || isBattle) {
            WallFly(start);
        }
    }
    
    /// <summary>
    /// 创建甲板
    /// </summary>
    /// <param name="parent">默认是在t_Start下创建</param>
    public static void CreateWallDeck(Transform parent)
    {
        Int2 start = new Int2(-1, 0);
        for (int j = 0; j < grid.Count; j++) {
            for (int k = 0; k < grid[j].Count; k++) {
                if (grid[j][k] == 0) {
                    if (j < (grid.Count - 1)) {
                        if (grid[j + 1][k] == 1) {
                            float y = (j + 1) * (roomwidth + beamwith);
                            float x = k * roomwidth;
                            Createbeam(x, y, LocationType.Bottom, k, j + 1, parent);
                        }
                    }
                    if (k < (grid[j].Count - 1)) {
                        if (grid[j][k + 1] == 1) {
                            Createbeam((k + 1) * roomwidth, j * (roomwidth + beamwith), LocationType.Left, k + 1, j, parent);
                            CreateCorner(k + 1, j + 1, LocationType.TopLeft, parent);
                            if (j == 0 || (grid[j - 1][k] == 1 && grid[j - 1][k + 1] == 1) || (grid[j - 1][k] == 0 && grid[j - 1][k + 1] == 0)) {
                                CreateCorner(k + 1, j, LocationType.BottomLeft, parent);
                            }
                        }
                    }
                    
                } else if (grid[j][k] == 1) {
                    GameObject rbg = null;
                    if (parent == null) {
                        rbg = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + "107@Skin", m_Start);
                        rbg.transform.localPosition = GetRealPos(k * roomwidth, j * (roomwidth + beamwith), 0f);
                    }
                    
                    float y = (j + 1) * roomwidth + j * beamwith;
                    float x = k * roomwidth;
                    
                    if (j == 0) {
                        if (start.Unit < 0) {
                            start.Unit = k;
                        }
                        Createbeam(x, 0, LocationType.Bottom, k, 0, parent);
                    }
                    if (j == (grid.Count - 1)) {
                        Createbeam(x, y, LocationType.Top, k, j + 1, parent);
                    } else if (j < (grid.Count - 1)) {
                        if (grid[j + 1][k] == 1) {
                            if (map[j + 1][k] == 1) {
                                Createbeam(x, y, LocationType.Middle, k, j + 1, parent);
                            }
                            GameObject bg = null;
                            if (parent == null) {
                                bg = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + "114@Skin", m_Start);
                                bg.transform.localPosition = GetRealPos(x, y, 0f);
                            }
                        } else {
                            Createbeam(x, y, LocationType.Top, k, j + 1, parent);
                        }
                    }
                    
                    y = j * (roomwidth + beamwith);
                    x = (k + 1) * roomwidth;
                    if (k == 0) {
                        Createbeam(0, y, LocationType.Left, 0, j, parent);
                        CreateCorner(k, j + 1, LocationType.TopLeft, parent);
                        if (j == 0 || grid[j - 1][k] == 0) {
                            CreateCorner(k, j, LocationType.BottomLeft, parent);
                        }
                    }
                    if (k == (grid[j].Count - 1)) {
                        Createbeam(x, y, LocationType.Right, k + 1, j, parent);
                        CreateCorner(k + 1, j + 1, LocationType.TopRight, parent);
                        if (j == 0 || grid[j - 1][k] == 0) {
                            CreateCorner(k + 1, j, LocationType.BottomRight, parent);
                        }
                    } else if (k < (grid[j].Count - 1)) {
                        if (grid[j][k + 1] == 0) {
                            Createbeam(x, y, LocationType.Right, k + 1, j, parent);
                            CreateCorner(k + 1, j + 1, LocationType.TopRight, parent);
                            if (j == 0 || (grid[j - 1][k] == 1 && grid[j - 1][k + 1] == 1) || (grid[j - 1][k] == 0 && grid[j - 1][k + 1] == 0)) {
                                CreateCorner(k + 1, j, LocationType.BottomRight, parent);
                            }
                        }
                    }
                } else {
#if UNITY_EDITOR_LOG
                    Debug.Log("数据有问题");
#endif
                }
            }
        }
        
        if (isAni || isBattle) {
            WallFly(start);
        }
    }
    
    /// <summary>
    /// 编辑时创建甲板
    /// </summary>
    public static GameObject CreateDeck(GameObject go, ShapeType shape)
    {
        List<List<int>> grids = StringToGrid(shape.shape);//格子数据
        List<List<int>> maps = StringToGrid(shape.map);//地图数据
        isAni = false;
        grid = grids;
        map = maps;
        GameObject deckParent = new GameObject();
        deckParent.name = "NewBuild";
        deckParent.transform.parent = go.transform;
        deckParent.transform.localPosition = Vector3.zero;
        CreateWallDeck(deckParent.transform);
        return deckParent;
    }
    static List<List<int>> StringToGrid(string str)
    {
        List<List<int>> grid = new List<List<int>>();
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
    
    public static IggFloor CreatetFloor(GameObject go, LocationType t, int unit, int layer, bool isattack = false)
    {
        IggFloor nf = null;
        LifeObj lo = go.AddComponent<LifeObj>();
        if (t == LocationType.Top || t == LocationType.Bottom || t == LocationType.Middle) {
            BoxCollider bc = go.AddComponent<BoxCollider>();
            bc.center = new Vector3(bc.center.x * MapGrid.m_UnitRoomGridNum, bc.center.y - 2, bc.center.z / 2);
            bc.size = new Vector3(bc.size.x * MapGrid.m_UnitRoomGridNum, bc.size.y, bc.size.z / 2);
            DefenceColider dc = go.AddComponent<DefenceColider>();
            dc.bottomCollider = bc;
            bc = go.AddComponent<BoxCollider>();
            bc.size = new Vector3(bc.size.x * MapGrid.m_UnitRoomGridNum, bc.size.y, bc.size.z / 2);
            bc.center = new Vector3(bc.center.x * MapGrid.m_UnitRoomGridNum, bc.center.y - 2, bc.size.z + bc.center.z / 2);
            dc.topCollider = bc;
            nf = new IggFloor();
            if (t == LocationType.Top) {
                nf.m_FloorType = FloorType.top;
            } else if (t == LocationType.Middle) {
                nf.m_FloorType = FloorType.Normal;
            } else {
                bool isbottom = true;
                for (int i = layer - 1; i >= 0; i--) {
                    if (grid[i][unit] == 1) {
                        isbottom = false;
                    }
                }
                if (isbottom) {
                    nf.m_FloorType = FloorType.bottom;
                } else {
                    nf.m_FloorType = FloorType.Normal;
                }
            }
            nf.Layer = layer;
            nf.StartUnit = unit * MapGrid.m_UnitRoomGridNum;
            nf.Size = MapGrid.m_UnitRoomGridNum;
            if (nf.m_FloorType == FloorType.top) {
                for (int i = 0; i < MapGrid.m_UnitRoomGridNum; i ++) {
                    MapGrid g = MapGrid.GetMG(layer, i + nf.StartUnit);
                    g.OffPosY = 0f;
                }
            }
            if (nf != null) {
                LifeProperty lp = go.AddComponent<FloorProperty>();
                lo.SetLife(nf, lp);
            }
        }
        if (t == LocationType.Left || t == LocationType.Right) {
            BoxCollider bc = go.AddComponent<BoxCollider>();
            bc.center = new Vector3(0.125f, 1.5f, -2f);
            bc.size = new Vector3(0.25f, 3, 8);
            DefenceColider dc = go.AddComponent<DefenceColider>();
            dc.leftCollider = bc;
            bc = go.AddComponent<BoxCollider>();
            bc.center = new Vector3(-0.125f, 1.5f, -2f);
            bc.size = new Vector3(0.25f, 3, 8);
            dc.rightCollider = bc;
            
            if (isattack) {
                IggWall w = null;
                if (t == LocationType.Left) {
                    w = new LeftFloorWall();
                } else {
                    w = new rightFloorWall();
                }
                w.m_bVertical = true;
                w.Layer = layer;
                w.StartUnit = unit * MapGrid.m_UnitRoomGridNum;
                if (t == LocationType.Right) {
                    w.StartUnit++;
                }
                //w.IsCanAttack = isattack;
                if (t == LocationType.Right) {
                    w.StartUnit -= 1;
                }
                w.Size = 0;
                
                LifeProperty lp = go.AddComponent<WallProperty>();
                lo.SetLife(w, lp);
                w.NDStart();
            } else {
            
                if (t == LocationType.Left) {
                    nf = new LeftFloor();
                    MapGrid.CutLine(layer, unit * MapGrid.m_UnitRoomGridNum - 1, false);
                } else {
                    nf = new RightFloor();
                    MapGrid.CutLine(layer, unit * MapGrid.m_UnitRoomGridNum + 1, true);
                }
                nf.m_bVertical = true;
                nf.Layer = layer;
                nf.StartUnit = unit * MapGrid.m_UnitRoomGridNum;
                nf.IsCanAttack = isattack;
                if (t == LocationType.Left) {
                    nf.m_FloorType = FloorType.left;
                } else {
                    nf.m_FloorType = FloorType.right;
                    nf.StartUnit -= 1;
                }
                
                nf.Size = 0;
                if (nf != null) {
                    LifeProperty lp = go.AddComponent<FloorProperty>();
                    lo.SetLife(nf, lp);
                }
                nf.NDStart();
            }
        }
        return nf;
    }
    
    public static GameObject Createbeam(float x, float y, LocationType type, int unit, int layer, Transform parent = null)
    {
        GameObject beam = null;
        if (type == LocationType.Bottom) {
            IggFloor nf = null;
            for (int n = 0; n < MapGrid.m_UnitRoomGridNum; n++) {
                if (parent == null) {
                    beam = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + "110@Skin", m_Start);
                } else {
                    if (parent.name == "NewBuild") {
                        beam = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + "110@Skin", parent);
                    } else {
                        beam = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + "110@Skin", m_Start);
                        beam.transform.parent = parent;
                    }
                }
                RemoveHP(beam);
                RoomWall.Add(beam);
                
                if (isAni || isBattle) {
                    DicHorizontal[GetIndex(unit, layer, n)] = beam.transform;
                }
                if (isBattle) {
                    if (n > 0) {
                        nf.itemfloor.Add(beam);
                    } else {
                        DicBattle[GetIndex(unit, layer, n)] = beam;
                        nf = CreatetFloor(beam, type, unit, layer);
                        nf.itemfloor.Add(beam);
                        if (DicBattle.ContainsKey(GetIndex(unit - 1, layer))) {
                            IggFloor left = DicBattle[GetIndex(unit - 1, layer)].GetComponent<LifeProperty>().GetLife() as IggFloor;
                            left.m_Right = nf;
                            nf.m_Left = left;
                        }
                        
                    }
                }
                //if (parent == null)
                {
                    if (n > 0) {
                        FlyToPos(beam, LocationType.Bottom, x + n * MapGrid.m_width, y, 0, false);
                    } else {
                        FlyToPos(beam, LocationType.Bottom, x + n * MapGrid.m_width, y, 0);
                    }
                }
            }
        } else if (type == LocationType.Top) {
            IggFloor nf = null;
            for (int n = 0; n < MapGrid.m_UnitRoomGridNum; n++) {
                if (parent == null) {
                    beam = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + "108@Skin", m_Start);
                } else {
                    beam = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + "108@Skin", parent);
                }
                
                RemoveHP(beam);
                RoomWall.Add(beam);
                
                if (isAni || isBattle) {
                    DicHorizontal[GetIndex(unit, layer, n)] = beam.transform;
                }
                
                if (isBattle) {
                    if (n > 0) {
                        nf.itemfloor.Add(beam);
                    } else {
                        DicBattle[GetIndex(unit, layer, n)] = beam;
                        nf = CreatetFloor(beam, type, unit, layer);
                        nf.itemfloor.Add(beam);
                        if (DicBattle.ContainsKey(GetIndex(unit - 1, layer))) {
                            IggFloor left = DicBattle[GetIndex(unit - 1, layer)].GetComponent<LifeProperty>().GetLife() as IggFloor;
                            left.m_Right = nf;
                            nf.m_Left = left;
                        }
                        
                    }
                }
                //if (parent == null)
                {
                    if (n > 0) {
                        FlyToPos(beam, LocationType.Top, x + n * MapGrid.m_width, y, 0, false);
                    } else {
                        FlyToPos(beam, LocationType.Top, x + n * MapGrid.m_width, y, 0);
                    }
                }
            }
        } else if (type == LocationType.Middle) {
            IggFloor nf = null;
            for (int n = 0; n < MapGrid.m_UnitRoomGridNum; n++) {
                if (parent == null) {
                    beam = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + "109@Skin", m_Start);
                } else {
                    beam = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + "109@Skin", parent);
                }
                RemoveHP(beam);
                RoomWall.Add(beam);
                beam.transform.localPosition = GetRealPos(x + n * MapGrid.m_width, y, 0f);
                if (map[layer][unit] == 2) {
                    beam.GetComponent<Renderer>().enabled = false;
                }
                //MidBeam.Add(beam);
                if (isBattle) {
                    if (n > 0) {
                        nf.itemfloor.Add(beam);
                    } else {
                        DicBattle[GetIndex(unit, layer, n)] = beam;
                        nf = CreatetFloor(beam, type, unit, layer);
                        nf.itemfloor.Add(beam);
                        if (DicBattle.ContainsKey(GetIndex(unit - 1, layer))) {
                            IggFloor left = DicBattle[GetIndex(unit - 1, layer)].GetComponent<LifeProperty>().GetLife() as IggFloor;
                            left.m_Right = nf;
                            nf.m_Left = left;
                        }
                        
                    }
                }
            }
            
        } else if (type == LocationType.Left) {
            if (parent == null) {
                beam = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + "112@Skin", m_Start);
            } else {
                beam = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + "112@Skin", parent);
            }
            RemoveHP(beam);
            RoomWall.Add(beam);
            
            if (isAni || isBattle) {
                DicVertical[GetIndex(unit, layer)] = beam.transform;
                DicBattleVertical[GetIndex(unit, layer)] = beam;
            }
            
            if (isBattle) {
                DicWall[GetIndex(unit, layer)] = beam;
                if (unit > 0 && map[layer][unit - 1] != 0) {
                    CreatetFloor(beam, type, unit, layer, true);
                } else {
                    CreatetFloor(beam, type, unit, layer);
                }
            }
            //if (parent == null)
            {
                FlyToPos(beam, LocationType.Left, x, y, 0);
            }
        } else if (type == LocationType.Right) {
            if (parent == null) {
                beam = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + "113@Skin", m_Start);
            } else {
                beam = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + "113@Skin", parent);
            }
            RemoveHP(beam);
            RoomWall.Add(beam);
            if (isAni || isBattle) {
                DicVertical[GetIndex(unit, layer)] = beam.transform;
                DicBattleVertical[GetIndex(unit, layer)] = beam;
            }
            if (isBattle) {
                DicWall[GetIndex(unit, layer)] = beam;
                if (unit < (shipinfo.width) && map[layer][unit] != 0) {
                    CreatetFloor(beam, type, unit, layer, true);
                } else {
                    CreatetFloor(beam, type, unit, layer);
                }
            }
            
            FlyToPos(beam, LocationType.Right, x, y, 0);
            
        }
        
        return beam;
    }
    public static void CreateCorner(int i, int j, LocationType type, Transform parent = null)
    {
        float x = i * roomwidth;
        float y = j * roomwidth + (j - 1) * beamwith + beamwith / 2;
        if (j == shipinfo.height) {
            y = j * roomwidth + (j - 1) * beamwith + 0.25f;
        } else if (j == 0) {
            y = -0.25f;
        }
        string name = "115@skin";
        if (type == LocationType.TopLeft || type == LocationType.BottomLeft) {
            if (j > 0 && i > 0 && grid[j - 1][i - 1] == 1) {
                name = "111@Skin";
            }
        } else if (parent == null) {
            if (j > 0 && i < shipinfo.width && grid[j - 1][i] == 1) {
                name = "111@Skin";
            }
        }
        GameObject corner;
        if (parent == null) {
            corner = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + name, m_Start);
        } else {
            corner = GameObjectLoader.LoadPath("Prefabs/boats/" + shipinfo.skin + "/", shipinfo.skin.ToString() + name, parent);
        }
        RemoveHP(corner);
        RoomWall.Add(corner);
        if (isAni || isBattle) {
            Corner c = corner.AddComponent<Corner>();
            c.pos = new Int2(i, j);
            DicCorner[GetIndex(i, j)] = corner.transform;
        }
        if (type == LocationType.TopLeft || type == LocationType.BottomLeft) {
            // if (parent == null)
            {
                FlyToPos(corner, LocationType.Left, x, y, 0, false);
            }
        } else if (type == LocationType.BottomRight || type == LocationType.TopRight) {
            // if (parent == null)
            {
                FlyToPos(corner, LocationType.Right, x, y, 0, false);
            }
        }
        
        if (parent != null) {
            Transform t_touchMove = parent.parent;
            if (t_touchMove) {
                TouchMove tm = t_touchMove.GetComponent<TouchMove>();
                if (tm) {
                    LocationType tmType = LocationType.Top;
                    if (type == LocationType.TopLeft || type == LocationType.TopRight) {
                        tmType = LocationType.Top;
                    }
                    if (type == LocationType.BottomLeft || type == LocationType.BottomRight) {
                        tmType = LocationType.Bottom;
                    }
                    
                }
            }
        }
    }
    
    private static void RemoveHP(GameObject go)
    {
        if (go) {
            Transform t = go.transform.Find("RoleHPSlider");
            if (t) {
                GameObject.DestroyImmediate(t.gameObject);
            }
        }
    }
    
    private static Transform CreateBuild(BuildInfo Info, Transform parent)
    {
        Vector3 local = GetRealPos(Info.m_cx * MapGrid.m_width, Info.m_cy * 3.3f, 0);
        Vector3 world = m_Start.TransformPoint(local);
        
        Building buildlife = LifeFactory.CreateBuilding(Info, 0, parent, world, LifeEnvironment.Edit);
        if (buildlife == null) {
            return null;
        }
        
        return buildlife.m_thisT ;
    }
    
    public static Transform CreateStairBuilding(BuildInfo stair)
    {
        if (stair == null) {
            return null;
        }
        return CreateBuild(stair, m_Start);
    }
    
    
    public static Transform CreateRoomBuilding(BuildInfo RoomBuild)
    {
        if (RoomBuild == null) {
            return null;
        }
        
        return  CreateBuild(RoomBuild, m_Start);
        
    }
    
    public static Role CreateSoilder(SoldierInfo s)
    {
        if (s == null) {
            return null;
        }
        /* IGameRole igr = GameRoleFactory.Create(m_Start, s.m_modeltype, s.m_name, AnimatorState.Stand);
         GameObject go = igr.GetGameRoleRootObj();
         Transform t = go.transform;
         t.transform.rotation = Quaternion.identity;
         t.transform.localPosition = GetRealPos((s.CX+1) * MapGrid.m_width, s.CY * 3 + s.CY * 0.3f, 0);
         t.transform.localScale = Vector3.one;*/
        Vector3 local = GetRealPos((s.CX + 1) * MapGrid.m_width, s.CY * 3 + s.CY * 0.3f, 0);
        Vector3 world = m_Start.TransformPoint(local);
        Role rolelife = LifeFactory.CreateRole(m_Start, s, true, world, AnimatorState.Stand, true);
        rolelife.RoleSkinCom.EnableColider(ColiderType.Click, true);
        GameObjectLoader.SetGameObjectLayer(rolelife.RoleSkinCom.tRoot.gameObject, m_Start.gameObject.layer);
        if (s.SoldierTypeID == 100003) {
            rolelife.RoleSkinCom.ShowLeftHand(false);
        }
        return rolelife;
    }
    
    
    public static void CreateRoomEdit()
    {
        ShipPlan p = ShipPlanDC.GetCurShipPlan();
        List<ShipPutInfo> rooms = p.GetShipBuildInfo(ShipBuildType.All);
        if (rooms != null) {
            foreach (ShipPutInfo r in rooms) {
                if (r == null) {
                    continue;
                }
                if (r.type == (int)ShipBuildType.BuildRoom) {
                    BuildInfo RoomBuild = r.GetBuildInfo();
                    if (RoomBuild != null) {
                        Drooms[r.id] = CreateBuild(RoomBuild, m_Start); ;
                    }
                }
                // 楼梯特殊处理
                else if (r.type == (int)ShipBuildType.BuildStair) {
                    BuildInfo RoomBuild = buildingM.GetStartBuildInfo(r.objid);
                    
                    if (RoomBuild != null) {
                        RoomBuild.ID = r.id;
                        RoomBuild.m_cx = r.cxMapGrid;
                        RoomBuild.m_cy = r.cyMapGrid;
                        RoomBuild.m_ShipPutdata0 = r.shipput_data0;
                        RoomBuild.m_ShipPutdata1 = r.shipput_data1;
                        if (RoomBuild.m_RoomType == RoomType.DeckTrap) {
                            r.m_DeckRoom = true;
                        } else {
                            r.m_DeckRoom = false;
                        }
                        Drooms[r.id] = CreateBuild(RoomBuild, m_Start);
                    }
                } else if (r.type == (int)ShipBuildType.Soldier) {
                    SoldierInfo s = new SoldierInfo();
                    if (r.GetSoldier(ref s) == true) {
                        Drooms[r.id] = CreateSoilder(s).m_thisT;
                    }
                    
                }
            }
        }
    }
    
    
    /// <summary>
    /// 创建拒绝区相关方法
    /// </summary>
    static public bool on_Ray(float x0, float y0, float x1, float y1, float x, float y)//点（x,y）已经在点(x0,y0)(x1,y1)所在的直接上了。
    {
        if (x0 <= x1 && x0 <= x || x0 >= x1 && x0 >= x) {
            return true;
        }
        return false;
    }
    static public bool on_Segment(float x0, float y0, float x1, float y1, float x, float y)//点（x,y）已经在点(x0,y0)(x1,y1)所在的直接上了。
    {
        float xmax = x0;
        float xmin = x1;
        if (x0 < x1) {
            xmax = x1;
            xmin = x0;
        }
        
        
        float ymax = y0;
        float ymin = y1;
        if (y0 < y1) {
            ymax = y1;
            ymin = y0;
        }
        if (xmax == xmin && y >= ymin && y <= ymax
            || x >= xmin && x <= xmax && y >= ymin && y <= ymax
            || x >= xmin && x <= xmax && ymin == ymax) {
            return true;
        } else {
            return false;
        }
    }
    
    static public bool RayToSegment(Vector3 posRaySrc, Vector3 posRayTo, Vector3 posEdgeSrc, Vector3 posEdgeTo, ref float xReturn, ref float yReturn)
    {
    
        float ZERO = 0.001f;
        float x0 = posRaySrc.x;
        float y0 = posRaySrc.y;
        float x1 = posRayTo.x;
        float y1 = posRayTo.y;
        
        float x2 = posEdgeSrc.x;
        float y2 = posEdgeSrc.y;
        float x3 = posEdgeTo.x;
        float y3 = posEdgeTo.y;
        
        float k1 = 0.0f;
        float b1 = 0.0f;
        float k2 = 0.0f;
        float b2 = 0.0f;
        /*斜率和Y轴截距  y = kx + b */
        bool lim1 = false;
        bool lim2 = false;
        //是否垂直X轴
        /* 判断2条直线是否垂直x轴 */
        if (x1 >= x0 && x1 - x0 < ZERO
            || x1 < x0 && x0 - x1 < ZERO) {
            lim1 = true;
        } else {
            k1 = (y1 - y0) / (x1 - x0); /* 斜率 */
            b1 = y0 - k1 * x0;  /* 截距 */
        }
        if (x3 >= x2 && x3 - x2 < ZERO
            || x3 < x2 && x2 - x3 < ZERO) {
            lim2 = true;    /* true表示垂直X轴 */
        } else {
            k2 = (y3 - y2) / (x3 - x2);
            b2 = y2 - k2 * x2;
        }
        
        //都不垂直于X轴
        if (!lim1 && !lim2) {
            if (k1 >= k2 && k1 - k2 < ZERO
                || k1 < k2 && k2 - k1 < ZERO) { /* 斜率相同 */
                if (k1 - 0 < ZERO && k2 - 0 < ZERO) {
                    return false;
                }
                /* 斜率,截矩都相同可能相交，可能交点在线段的某一个端点 */
                /* 射线方向背离线段时，不相交 */
                /* 射线方向朝线段时，离射线源点距离近的线段端点为实际交点 */
                if (b1 >= b2 && b1 - b2 < ZERO
                    || b1 < b2 && b2 - b1 < ZERO) { /* 斜率相同 */
                    /* 射线源点坐标为所有坐标中最大或最小*/
                    /* 由于斜率相同，x，y同时为最值，判断一个即可 */
                    if (x0 > x1 && x0 > x2 && x0 > x3) {
                        xReturn = (x2 > x3) ? x2 : x3;
                        yReturn = k1 * xReturn + b1;
                        if (!on_Ray(x0, y0, x1, y1, xReturn, yReturn) || !on_Segment(x2, y2, x3, y3, xReturn, yReturn)) {
                            return false;
                        }
                        //cout < <x < <"  " < <y < <endl;
                        return true;
                    } else if (x0 < x1 && x0 < x2 && x0 < x3) {
                        xReturn = (x2 < x3) ? x2 : x3;
                        yReturn = k1 * xReturn + b1;
                        if (!on_Ray(x0, y0, x1, y1, xReturn, yReturn) || !on_Segment(x2, y2, x3, y3, xReturn, yReturn)) {
                            return false;
                        }
                        return true;
                    }
                    return false;
                }
                /* b1==b2 平行无交点 */
                return false;
            }/* end of if(k1 == k2) */
            else { /* 斜率不同 */
                xReturn = (b2 - b1) / (k1 - k2);
                yReturn = k1 * xReturn + b1;
                if (!on_Ray(x0, y0, x1, y1, xReturn, yReturn) || !on_Segment(x2, y2, x3, y3, xReturn, yReturn)) {
                    return false;
                }
                return true;
            }
        }/* end of if(!lim1 && !lim2) */
        //2直线全垂直X轴，斜率无穷大
        if (lim1 && lim2) {
            /* x0==x1, x2==x3 */
            /* 可能不相交，可能交点在线段的某一个端点 */
            if (x0 >= x2 && x0 - x2 < 0.001
                || x0 < x2 && x2 - x0 < 0.001) {
                if (y0 < y1 && y0 < y2 && y0 < y3) {
                    xReturn = x0;
                    if ((y2 < y3)) {
                        yReturn = y2;
                    } else {
                        yReturn = y3;
                    }
                    if (!on_Ray(x0, y0, x1, y1, xReturn, yReturn) || !on_Segment(x2, y2, x3, y3, xReturn, yReturn)) {
                        return false;
                    }
                    return true;
                }
                if (y0 > y1 && y0 > y2 && y0 > y3) {
                    xReturn = x0;
                    yReturn = (y2 > y3) ? y2 : y3;
                    if (!on_Ray(x0, y0, x1, y1, xReturn, yReturn) || !on_Segment(x2, y2, x3, y3, xReturn, yReturn)) {
                        return false;
                    }
                    return true;
                }
                return false;
            }
            /* x0 != x2 平行无交点 */
            return false;
        }/* end of if(lim1 && lim2) */
        /* 射线垂直X轴 */
        if (lim1 && !lim2) {
            xReturn = x1;
            yReturn = k2 * xReturn + b2;
            if (!on_Ray(x0, y0, x1, y1, xReturn, yReturn) || !on_Segment(x2, y2, x3, y3, xReturn, yReturn)) {
                return false;
            }
            return true;
        }
        /* 线段所在直线垂直x轴 */
        if (!lim1 && lim2) {
            xReturn = x3;
            yReturn = k1 * xReturn + b1;
            if (!on_Ray(x0, y0, x1, y1, xReturn, yReturn) || !on_Segment(x2, y2, x3, y3, xReturn, yReturn)) {
                return false;
            }
            return true;
        }
        /* 缺省返回假 */
        return false;
        
    }
    static public List<Vector3> GetRejectPolygon()
    {
        return m_v3RejectPolygon;
    }
    static public List<Vector3> GetOutRejectPolygon()
    {
        if (m_v3OutRejectPolygon.Count == 0 && m_v3RejectPolygon.Count > 0) {
            Vector3 posMin = new Vector3(m_v3RejectPolygon[0].x - 3, m_v3RejectPolygon[0].y - 3, m_v3RejectPolygon[0].z);
            Vector3 posMax = new Vector3(m_v3RejectPolygon[0].x + 3, m_v3RejectPolygon[0].y + 3, m_v3RejectPolygon[0].z);
            for (int i = 1; i < m_v3RejectPolygon.Count; i++) {
                Vector3 posTemp = m_v3RejectPolygon[i];
                if (posMin.x > posTemp.x - 3) {
                    posMin.x = posTemp.x - 3;
                }
                if (posMin.y > posTemp.y - 3) {
                    posMin.y = posTemp.y - 3;
                }
                if (posMax.x < posTemp.x + 3) {
                    posMax.x = posTemp.x + 3;
                }
                if (posMax.y < posTemp.y + 3) {
                    posMax.y = posTemp.y + 3;
                }
            }
            m_v3OutRejectPolygon.Add(new Vector3(posMin.x, posMin.y, posMin.z));
            m_v3OutRejectPolygon.Add(new Vector3(posMin.x, posMax.y, posMin.z));
            m_v3OutRejectPolygon.Add(new Vector3(posMax.x, posMax.y, posMin.z));
            m_v3OutRejectPolygon.Add(new Vector3(posMax.x, posMin.y, posMin.z));
        }
        return m_v3OutRejectPolygon;
    }
    static public void DrawRejectPolygon()
    {
        Gizmos.color = Color.red;
        for (int i = 1; i <= GenerateShip.m_v3RejectPolygon.Count; i++) {
            Gizmos.DrawCube(GenerateShip.m_v3RejectPolygon[i - 1], Vector3.one * 0.25f);
            if (i == GenerateShip.m_v3RejectPolygon.Count) {
                Gizmos.DrawLine(GenerateShip.m_v3RejectPolygon[i - 1], GenerateShip.m_v3RejectPolygon[0]);
            } else {
                Gizmos.DrawLine(GenerateShip.m_v3RejectPolygon[i - 1], GenerateShip.m_v3RejectPolygon[i]);
            }
        }
        
        Gizmos.color = Color.green;
        for (int i = 1; i <= GenerateShip.m_v3OutRejectPolygon.Count; i++) {
            Gizmos.DrawCube(GenerateShip.m_v3OutRejectPolygon[i - 1], Vector3.one * 0.25f);
            if (i == GenerateShip.m_v3OutRejectPolygon.Count) {
                Gizmos.DrawLine(GenerateShip.m_v3OutRejectPolygon[i - 1], GenerateShip.m_v3OutRejectPolygon[0]);
            } else {
                Gizmos.DrawLine(GenerateShip.m_v3OutRejectPolygon[i - 1], GenerateShip.m_v3OutRejectPolygon[i]);
            }
        }
    }
    public static bool pointInRejectPolygon(Vector3 pos, List<Vector3> v3RejectPolygon)
    {
        if (v3RejectPolygon == null) {
            return false;
        }
        int nPolySides = v3RejectPolygon.Count;
        int i, j = nPolySides - 1;
        bool oddNodes = false;
        
        for (i = 0; i < nPolySides; i++) {
            if ((v3RejectPolygon[i].y < pos.y && v3RejectPolygon[j].y >= pos.y
                    || v3RejectPolygon[j].y < pos.y && v3RejectPolygon[i].y >= pos.y)
                && (v3RejectPolygon[i].x <= pos.x || v3RejectPolygon[j].x <= pos.x)) {
                oddNodes ^= (v3RejectPolygon[i].x + (pos.y - v3RejectPolygon[i].y) / (v3RejectPolygon[j].y - v3RejectPolygon[i].y) * (v3RejectPolygon[j].x - v3RejectPolygon[i].x) < pos.x);
            }
            j = i;
        }
        return oddNodes;
    }
    static public int RayToRejectPolygon(Vector3 posRaySrc, Vector3 posRayTo, ref Vector3 refReturnPos, List<Vector3> v3RejectPolygon)
    {
        int nPolySides = v3RejectPolygon.Count;
        refReturnPos = new Vector3(0.0f, 0.0f, 0.0f);
        int i, j = nPolySides - 1;
        float fMinDiatance = -1f;
        int nEdage = -1;
        for (i = 0; i < nPolySides; i++) {
            float xPoint = 0.0f;
            float yPoint = 0.0f;
            if (RayToSegment(posRaySrc, posRayTo, v3RejectPolygon[i], v3RejectPolygon[j], ref xPoint, ref yPoint)) {
                Vector3 vPoint = new Vector3(xPoint, yPoint, posRaySrc.z);
                float fTempDiance = Mathf.Abs(Vector3.Distance(posRaySrc, vPoint));
                if (fTempDiance < fMinDiatance || fMinDiatance < 0) {
                    fMinDiatance = fTempDiance;
                    refReturnPos.x = xPoint;
                    refReturnPos.y = yPoint;
                    nEdage = j;
                }
            }
            j = i;
        }
        return nEdage;
    }
    
    /// <summary>
    /// 给房间生成柱子，墙体，板等外壳 ，生成的对象需挂在buildingBody 节点下
    /// </summary>
    /// <param name="buildingBody">房间对象</param>
    /// <param name="BuildingMap">房间地图</param>
    public static void GenerateShipBuildingShell(Transform buildingBody, ShipCanvasInfo BuildingMap)
    {
        isBattle = false;
        Clear();
        ClearRoomWall();
        shipinfo = BuildingMap;
        m_Start = buildingBody;
        isAni = false;
        grid = shipinfo.GetShape();
        map = shipinfo.GetMap();
        CreateWall();
    }
    
    
    public static void GenerateShipBuildingShellWithouClear(Transform buildingBody, ShipCanvasInfo BuildingMap)
    {
        isBattle = false;
        Clear();
        shipinfo = BuildingMap;
        m_Start = buildingBody;
        isAni = false;
        grid = shipinfo.GetShape();
        map = shipinfo.GetMap();
        CreateWall();
    }
}
