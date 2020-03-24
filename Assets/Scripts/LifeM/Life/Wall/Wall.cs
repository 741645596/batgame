using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 墙体类型
/// </summary>
public enum WallType {
    Normal  = 0,
    LeftTop = 2,
}


/// <summary>
/// 发起开门方式
/// </summary>
public enum OpenDoorStyle {
    passDoor        = 0, // defense  pass door in 2 Grid(100 pixels)
    // 一下2中规则已经没有了。  by  zhulin 策划文档：炮弹兵遇墙规则。
    //Attack          = 1, // defense attack
    //OpenDoorSkill   = 2, // attack use open door skill
}

public class IggWall : Life
{


    //所在位置及大小
    public int Layer  ;
    public int StartUnit ;
    public int Size ;
    /// <summary>
    /// 是否是垂直放置的
    /// </summary>
    public bool m_bVertical = false;
    
    protected DefenceColider m_DefenceColider;
    
    public WallType m_WallType = WallType.Normal;
    public Texture Wallf_l;
    public Texture Wallf_m;
    public Texture Wallf_r;
    public Texture Wall1_l;
    public Texture Wall1_m;
    public Texture Wall1_r;
    public Texture Wall2_l;
    public Texture Wall2_m;
    public Texture Wall2_r;
    public Texture Wall3_l;
    public Texture Wall3_m;
    public Texture Wall3_r;
    
    private Animator ani;
    protected int m_WallID = 0;
    private bool m_open = false; //是否正在开门途中
    public bool Open{
        get{return m_open;}
    }
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
    public List<Corner> m_corners = new List<Corner>();
    
    
    public void SetWallLife(WallInfo Info, LifeEnvironment Environment)
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
    
    public LifeProperty m_Property;
    public override LifeProperty GetLifeProp()
    {
        return m_Property;
    }
    public override void SetLifeProp(LifeProperty lifeObj)
    {
        m_Property = lifeObj as LifeProperty;
    }
    public override void NDUpdate(float deltaTime)
    {
        //base.NDUpdate (deltaTime);
        
        List<Life> l = new List<Life>();
        CM.SearchLifeMListInBoat(ref l, LifeMType.SOLDIER, LifeMCamp.ATTACK);
        //Debug.Log("wall: " + l.Count + "," +LifeMType.SOLDIER + "," + LifeMCamp.ATTACK);
        
        foreach (Role r in l) {
            if (NdUtil.IsSameMapLayer(r.GetMapPos(), GetMapPos())) {
                if (Mathf.Abs(r.m_thisT.position.x - m_thisT.position.x) < 1) {
                    OpenDoor();
                    return;
                }
            }
        }
        CloseDoor(null);
    }
    
    public  void CombatInit()
    {
        m_DefenceColider = GetComponent<DefenceColider> ();
        GetBuildRoom(Layer, StartUnit);
        InitHP();
        isDead = false;
        m_bVertical = true;
        ani = m_Property.GetComponentInChildren<Animator>();
        SetCore();
        SetWall();
        InBoat = true;
        Reset();
    }
    
    
    public virtual void SetCore()
    {
        bool IsPlayer = CmCarbon.GetCamp2Player(LifeMCamp.DEFENSE);
        m_Core = new LifeMCore(m_WallID, IsPlayer, LifeMType.WALL, LifeMCamp.DEFENSE, MoveState.Static);
        SetLifeCore(m_Core);
        
        m_Attr = new WallAttribute();
        m_Attr.Init(SceneID, m_Core, this);
        if (m_WallType == WallType.Normal) {
            StartUnit = m_Attr.StartPos.Unit;
            Layer = m_Attr.StartPos.Layer;
            Size = m_Attr.Size;
        }
        HP = fullHP;
    }
    
    /// <summary>
    /// 开门
    /// </summary>
    private void OpenDoor()
    {
        if (ani != null) {
            ani.SetInteger("iState", 1);
            //			SoundPlay.Play("open_door",false,false);
            m_open = true;
            GameObjectActionExcute gae = GameObjectActionExcute.CreateExcute(m_thisT.gameObject);
            GameObjectActionWait wait = new GameObjectActionWait(0.5f, SetOpenDoor);
            gae.AddAction(wait);
        }
    }
    
    private void SetOpenDoor(object o)
    {
        m_Attr.DoorState = true;
        GameObjectActionExcute gae = GameObjectActionExcute.CreateExcute(m_thisT.gameObject);
        GameObjectActionWait wait = new GameObjectActionWait(2.5f, CloseDoor);
        gae.AddAction(wait);
    }
    
    /// <summary>
    /// 关门
    /// </summary>
    private void CloseDoor(object o)
    {
        if (ani != null) {
            ani.SetInteger("iState", 0);
            m_Attr.DoorState = false;
            m_open = false;
        }
    }
    
    /// <summary>
    /// 发起开门指令
    /// </summary>
    /// <param name="style">发起开门方式</param>
    /// <param name="Camp">按指定的阵营</param>
    /// <param name="Camp">发起开门时的格子距离</param>
    public void OpenDoor(OpenDoorStyle style, LifeMCamp Camp, int Dis)
    {
        bool bOPEN = false;
        if (m_open == true) {
            if (m_Attr.DoorState == true) {
                //by zhulin  临时屏蔽
                //CancelInvoke("CloseDoor");
                //Invoke("CloseDoor", 2.5f);
            }
        } else {
            if (style == OpenDoorStyle.passDoor) {
                if (/*Camp == m_Core.m_Camp && */Dis <= 2) {
                
                    bOPEN = true;
                }
            }
            /*else if(style == OpenDoorStyle.Attack )
            {
            	if(Camp == m_Core.m_Camp)
            	{
            		bOPEN = true;
            	}
            }
            else if(style == OpenDoorStyle.OpenDoorSkill)
            {
            	if(Camp != m_Core.m_Camp)
            	{
            		bOPEN = true;
            	}
            }*/
        }
        
        if (bOPEN == true) {
            OpenDoor();
        }
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
        Role.WakeEnemy(this);
        Info.bVertical = true;
        Info.FlyfallSTandGridPos = new Int2(StartUnit, Layer);
        SkillReleaseInfo sInfo = new SkillReleaseInfo();
        sInfo.m_InterruptSkill = false;
        sInfo.m_MakeStatus = new List<StatusType> ();
        sInfo.m_bImmunity = false;
        sInfo.m_Damage = Damage;
        sInfo.Result = AttackResult.Fire;
        ApplyDamage(sInfo, null);
        if (isDead) { //破了
            Info.flyCollisionAction = FlyCollisionAction.PauseContinueFlyDirectional;
            Info.bApplyDamage = true;
            EffectM.LoadEffect(EffectM.sPath, "zamen_02", m_thisT.position, null);
        } else {      //没破
            Info.flyCollisionAction = FlyCollisionAction.DropInBoat;
            Info.bApplyDamage = false;
            GameObjectActionExcute gae1 = EffectM.LoadEffect(EffectM.sPath, "zamen_01", m_thisT.position, null);
            GameObjectActionDelayDestory ndEffect1 = new GameObjectActionDelayDestory(1f);
            gae1.AddAction(ndEffect1);
        }
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
    
    public override Int2 GetMapPos()
    {
        return new Int2(StartUnit, Layer);
    }
    
    public override bool ApplyDamage(SkillReleaseInfo Info, Transform attackgo)
    {
        for (int i = 0; i < m_rooms.Count;) {
            if (m_rooms[i].ApplyDamage(Info, attackgo)) {
                i++;
            }
        }
        
        Hit(Info.m_Damage);
        return true;
    }
    
    public override void BeforeDead()
    {
        base.BeforeDead();
        foreach (Corner c in m_corners) {
            c.RemoveLife(this.m_Property);
        }
        int wood = m_Attr.Wood;
        if (wood > 0) {
            Vector3 pos = MapGrid.GetMG(Layer, StartUnit).pos;
            pos += new Vector3(0, 0.5f, 1.2f);
            DropResource.Drop(ResourceType.Wood, wood, pos);
            CmCarbon.SetAddWinWood(wood);
        }
        int stone = m_Attr.Stone;
        if (stone > 0) {
            Debug.Log("get stone:" + stone + "," + this);
        }
        int steel = m_Attr.Steel;
        if (steel > 0) {
            Debug.Log("get steel:" + steel + "," + this);
        }
        List<Life> listRAW = new List<Life>();
        CM.SearchLifeMListInBoat(ref listRAW, LifeMType.SOLDIER);
        foreach (Life item in listRAW) {
            Role raw = item as Role;
            raw.ChangeTarget(ChangeTargetReason.ClearOtherTarget, this);
        }
        
        Destroy();
        SetWallGridDead();
        //by zhlin  临时屏蔽
        //CancelInvoke("CloseDoor");
    }
    
    public void SetWallGridDead()
    {
        MapGrid m = null;
        m = GetMapGrid();
        if (m != null) {
            m.SetWallDeadType(SceneID);
        }
        
        if (m_Attr.StartPos.Unit > 0) {
            m = MapGrid.GetMG(m_Attr.StartPos.Layer, m_Attr.StartPos.Unit - 1);
            if (m != null) {
                m.SetWallDeadType(SceneID);
            }
        }
    }
    
    
    public virtual void SetWall()
    {
        MapGrid m = null;
        m = GetMapGrid();
        if (m != null) {
            //Debug.Log("SetWall:" + SceneID + "," + m.GridPos);
            m.JoinWall(m_SceneID);
        }
        
        /*if(m_Attr.StartPos.Unit > 0)
        {
        	m = MapGrid.GetMG (m_Attr.StartPos.Layer, m_Attr.StartPos.Unit - 1);
        	if (m != null)
        	{
        		//Debug.Log("SetWall:" + SceneID + "," + m.GridPos);
        		m.JoinWall(m_SceneID);
        	}
        }*/
    }
    
    void SetRendererTexture(Texture t, int index)
    {
        if (t) {
            Renderer ren = GetComponentInChildren<Renderer>();
            if (ren != null) {
                int nCnt = 0;
                foreach (Material m in ren.materials) {
                    if (index == nCnt) {
                        m.mainTexture = t;
                        break;
                    }
                    nCnt++;
                }
            }
        }
    }
    
    public virtual void Hit(int damage)
    {
        int damageP = (int)(((fullHP - HP) * 1.0f / (fullHP * 1.0f) * 100));
        int damageL =  ConfigM.GetFloorDamageLevel(damageP);
        switch (damageL) {
            case 1:
                SetRendererTexture(Wall1_r, 0);
                SetRendererTexture(Wall1_l, 1);
                SetRendererTexture(Wall1_m, 2);
                break;
            case 2:
                SetRendererTexture(Wall2_r, 0);
                SetRendererTexture(Wall2_l, 1);
                SetRendererTexture(Wall2_m, 2);
                break;
            case 3:
                SetRendererTexture(Wall3_r, 0);
                SetRendererTexture(Wall3_l, 1);
                SetRendererTexture(Wall3_m, 2);
                break;
                
            default:
                //Debug.Log("damageL="+damageL);
                break;
        }
        
        if (ani != null) {
            ani.SetTrigger("tLHit");
        }
    }
    public virtual void Destroy()
    {
        SetRendererTexture(Wall3_r, 0);
        SetRendererTexture(Wall3_l, 1);
        SetRendererTexture(Wall3_m, 2);
    }
    public void Reset()
    {
        SetRendererTexture(Wallf_r, 0);
        SetRendererTexture(Wallf_l, 1);
        SetRendererTexture(Wallf_m, 2);
        HP = fullHP;
        if (ani != null) {
            ani.SetBool("Destroy", false);
        }
    }
    
    public  void SetWall(int WallID)
    {
        m_WallID = WallID;
        m_WallType = WallType.Normal;
    }
    
    
    public void SetWallDead()
    {
        Dead();
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
                    (buildRoom as Building).AddWall(this);
                    //buildRoom.ad
                }
            }
        }
    }
    protected virtual void GetBuildRoom(int layer, int unit)
    {
    
    
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
    public void AddCorner(Corner c)
    {
        m_corners.Add(c);
    }
}
