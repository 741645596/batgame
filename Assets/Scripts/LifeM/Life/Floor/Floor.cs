using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
public enum FloorType {
    Normal  = 0,
    top     = 1,
    bottom  = 2,
    left    = 3,
    right   = 4,
}


public class IggFloor : Life
{
    //所在位置及大小
    public int Layer  ;
    public int StartUnit ;
    public int Size ;
    /// <summary>
    /// 是否是垂直放置的
    /// </summary>
    public bool m_bVertical = false;
    public bool IsCanAttack = false;
    protected DefenceColider m_DefenceColider;
    
    /// <summary>
    /// 伤害点
    /// </summary>
    private int _damagePoint ;
    
    public IggFloor m_Left;
    public IggFloor m_Right;
    public List<GameObject> itemfloor = new List<GameObject>();
    public List<Corner> m_corners = new List<Corner>();
    
    
    public FloorType m_FloorType = FloorType.Normal;
    
    private bool m_bStart = false;
    public List<Building> m_rooms = new List<Building>();
    public Building Room {
        get{
            if (m_rooms.Count > 0)
            {
                return m_rooms[0];
            } else
            {
                return null;
            }
        }
    }
    
    
    public void SetFloorLife(FloorInfo Info, LifeEnvironment Environment)
    {
        if (Info == null) {
            return ;
        }
    }
    
    public override void NDStart()
    {
        if (! m_bStart) {
            m_bStart = true;
            if (Environment == LifeEnvironment.Combat) {
                CombatInit();
            }
        }
    }
    public FloorProperty m_Property;
    public override LifeProperty GetLifeProp()
    {
        return m_Property;
    }
    public override void SetLifeProp(LifeProperty lifeObj)
    {
        m_Property = lifeObj as FloorProperty;
    }
    //所在格子的坐标
    public override Int2 GetMapPos()
    {
        return new Int2(StartUnit, Layer);
    }
    
    
    /// <summary>
    /// 战斗模式 Start 接口
    /// </summary>
    public  void CombatInit()
    {
        m_DefenceColider = GetComponent<DefenceColider> ();
        _damagePoint = 0;
        isDead = false;
        SetFloorCore();
        HP = fullHP;
        GetBuildRoom(m_FloorType, Layer, StartUnit);
    }
    
    private void SetFloorCore()
    {
        //SetFloorType();
        bool IsPlayer = CmCarbon.GetCamp2Player(LifeMCamp.DEFENSE);
        m_Core = new LifeMCore(NdUtil.GetSceneID(), IsPlayer, LifeMType.FLOOR, LifeMCamp.DEFENSE, MoveState.Static);
        SetLifeCore(m_Core);
        m_Attr = new FloorAttribute();
        m_Attr.Init(SceneID, m_Core, this);
    }
    
    /// <summary>
    /// Floor不加入cm列表
    /// </summary>
    /// <author>zhulin</author>
    public override int SetLifeCore(LifeMCore Core)
    {
        m_SceneID = NdUtil.GetSceneID();
        m_Core.Copy(Core);
        if (IsCanAttack) {
            InBoat = true;
            CM.JoinCombat(m_SceneID, this, m_Core);
        }
        return m_SceneID;
    }
    
    
    public virtual void SetFloorType()
    {
        m_FloorType = FloorType.Normal;
    }
    
    /// <summary>
    /// 伤害点
    /// </summary>
    public int damagePoint {
        set {
            _damagePoint = value;
            HP += _damagePoint;
            if (HP <= 0)
            {
                isDead = true;
            }
        }
        get { return _damagePoint; }
    }
    public override MapGrid GetTargetMapGrid()
    {
        return GetMapGrid();
    }
    //所在的地图格子
    public override MapGrid GetMapGrid()
    {
        return MapGrid.GetMG(GetMapPos());
    }
    
    
    
    private void SetDamageTexture()
    {
        int damageP = (int)(((fullHP - HP) * 1.0f / (fullHP * 1.0f) * 100));
        int damageL = ConfigM.GetFloorDamageLevel(damageP);
        
        
    }
    
    public override bool ApplyDamage(SkillReleaseInfo Info, Transform attackgo)
    {
        for (int i = 0; i < m_rooms.Count;) {
            if (m_rooms[i].ApplyDamage(Info, attackgo)) {
                i++;
            }
        }
        return true;
    }
    /// <summary>
    /// 撞击处理
    /// </summary>
    /// <param name="Damage">撞击伤害</param>
    /// <param name="dir">撞击方向</param>
    /// <param name="Info">返回撞击信息</param>
    /// <returns></returns>
    public override void CollisionProcByFire(Collision collision, int Damage/*,Vector2 dir*/, ref FlyCollisionInfo Info, FlyDir flydir)
    {
        //惊醒防御方
        Role.WakeEnemy(this);
        Info.bVertical = m_bVertical;
        Info.FlyfallSTandGridPos = new Int2(StartUnit, Layer);
        Info.lifemCollision = this;
        
        SkillReleaseInfo sInfo = new SkillReleaseInfo();
        sInfo.m_InterruptSkill = false;
        sInfo.m_MakeStatus = new List<StatusType> ();
        sInfo.m_bImmunity = false;
        sInfo.m_Damage = Damage;
        sInfo.Result = AttackResult.Fire;
        ApplyDamage(sInfo, null);
        
        //........................................................................
        // 需要继续查..........................
        if (m_DefenceColider == null) {
            isDead = true;
        }
        //........................................................................
        
        if (isDead) { //穿了
        
            GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "2000291", collision.contacts[0].point, BattleEnvironmentM.GetLifeMBornNode(true));
            GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
            gae.AddAction(ndEffect);
            Info.flyCollisionAction = FlyCollisionAction.PauseContinueFlyDirectional;
            Info.bApplyDamage = true;
            MakeHole();
        } else {
            if (Info.bReleaseSkill) {
                shake(collision.contacts[0].point);
                SetDamageTexture();
                GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "2000281", collision.contacts[0].point, BattleEnvironmentM.GetLifeMBornNode(true));
                GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
                gae.AddAction(ndEffect);
            }
            if (flydir == FlyDir.LeftTop || flydir == FlyDir.RightTop || flydir == FlyDir.Top) {
                if (m_FloorType == FloorType.bottom) {
                    Info.flyCollisionAction = FlyCollisionAction.DropOutBoat;
                } else {
                    Info.flyCollisionAction = FlyCollisionAction.DropInBoat;
                }
                
                Info.bApplyDamage = false;
            } else if (flydir == FlyDir.LeftBottom || flydir == FlyDir.RightBottom || flydir == FlyDir.Bottom) {
                Info.flyCollisionAction = FlyCollisionAction.FlyFallStand;
                Info.bApplyDamage = false;
            } else {
                if (m_FloorType == FloorType.top) {
                    Info.flyCollisionAction = FlyCollisionAction.FlyFallStand;
                    Info.bApplyDamage = false;
                } else if (m_DefenceColider.IsInBottomCollider(collision.collider)) {
                    if (m_FloorType == FloorType.bottom) {
                        Info.flyCollisionAction = FlyCollisionAction.DropOutBoat;
                    } else {
                        Info.flyCollisionAction = FlyCollisionAction.DropInBoat;
                    }
                    Info.bApplyDamage = false;
                } else if (m_DefenceColider.IsInTopCollider(collision.collider)) { //从上往下撞
                    Info.flyCollisionAction = FlyCollisionAction.FlyFallStand;
                    Info.bApplyDamage = false;
                }
            }
        }
    }
    
    public void shake(Vector3 pos)
    {
        Vector3 localpos = BattleEnvironmentM.World2LocalPos(pos);
        int item = Mathf.FloorToInt(localpos.x / MapGrid.m_width) - StartUnit;
        if (item >= 0 && item < MapGrid.m_UnitRoomGridNum  ) {
            
        }
        
        for (int i = 1; i <= 3; i ++) {
            GameObject go = null;
            int left = item - i;
            if (left >= 0) {
                go = itemfloor[left];
            } else if (m_Left != null) {
                go = m_Left.itemfloor[MapGrid.m_UnitRoomGridNum + left];
            }
            if (go != null ) {
                
            }
            int right = item + i;
            if (right < MapGrid.m_UnitRoomGridNum) {
                go = itemfloor[right];
            } else if (m_Right != null) {
                go = m_Right.itemfloor[right - MapGrid.m_UnitRoomGridNum];
            }
            if (go != null ) {
                
            }
        }
    }
    void CreateHole()
    {
        if (Size <= 0) {
            return;
        }
        for (int i = 0; i <= Size; i++) {
            int WallID = -1;
            if (i == 0) {
                MapGrid.CreateLeftShareHole(new Int2(StartUnit + i, Layer), ref WallID);
            } else if (i == Size) {
                MapGrid.CreateRightShareHole(new Int2(StartUnit + i, Layer), ref WallID);
            } else {
                MapGrid.CreateHole(new Int2(StartUnit + i, Layer), ref WallID);
            }
            if (WallID != -1) {
                Life m = CM.GetLifeM(WallID, LifeMType.WALL);
                if (m is IggWall) {
                    (m as IggWall).SetWallDead();
                }
            }
        }
    }
    
    public void ToMakeHole()
    {
        if (m_isDead) {
            return ;
        }
        m_isDead = true;
        CreateHole();
    }
    public void MakeHole()
    {
    
        CreateHole();
        
        List<Life> RoleList = new List<Life>();
        CM.SearchLifeMListInBoat(ref RoleList, LifeMType.SOLDIER | LifeMType.SUMMONPET);
        AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_MAP), RoleList);
    }
    
    
    public void DestoryTextrue(FloorTexture t, FloorTextureType  tt)
    {
        if (t != null) {
            t.DestroyTexture(tt);
            
            FloorTexture[] ftChilds =  t.gameObject.GetComponentsInChildren<FloorTexture> ();
            foreach (FloorTexture ft in ftChilds)
                if (ft != null) {
                    ft.DestroyTexture(tt);
                }
        }
    }
    public override void BeforeDead()
    {
        foreach (Corner c in m_corners) {
            if (c != null) {
                c.RemoveLife(this.m_Property);
            }
        }
        MakeHole();
        foreach (GameObject go in itemfloor) {
            GameObject.Destroy(go);
        }
    }
    public void AddBuildRoom(Int2 Pos)
    {
    
        MapGrid m = MapGrid.GetMG(Pos);
        if (m != null) {
            int BuildRoomSceneID =  -1;
            if (m.GetBuildRoom(ref BuildRoomSceneID) == true) {
                Life buildRoom = CM.GetLifeM(BuildRoomSceneID, LifeMType.BUILD);
                if (buildRoom != null) {
                    m_rooms.Add(buildRoom as Building);
                    (buildRoom as Building).AddFloor(this);
                }
            }
        }
    }
    protected void GetBuildRoom(FloorType type, int layer, int unit)
    {
        Int2 Pos = new Int2(unit, layer);
        if (type == FloorType.Normal || type == FloorType.top) {
        
            Pos.Unit += MapGrid.m_UnitRoomGridNum / 2;
            AddBuildRoom(Pos);
            Pos.Layer = Pos.Layer - 1;
            if (Pos.Layer < 0) {
                Pos.Layer = 0;
            }
            AddBuildRoom(Pos);
        } else if (type == FloorType.left) {
            Pos.Unit += MapGrid.m_UnitRoomGridNum / 2;
            AddBuildRoom(Pos);
        } else if (type == FloorType.right) {
            Pos.Unit -= MapGrid.m_UnitRoomGridNum / 2;
            AddBuildRoom(Pos);
        }
        
    }
    public void AddCorner(Corner c)
    {
        m_corners.Add(c);
    }
    public void DestroyRoom(Building room)
    {
        if (m_rooms.Contains(room)) {
            m_rooms.Remove(room);
        }
        if (m_rooms.Count == 0) {
            Dead();
        }
    }
}
