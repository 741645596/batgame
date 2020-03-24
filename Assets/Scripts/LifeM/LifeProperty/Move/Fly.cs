#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    #define UNITY_EDITOR_LOG
#endif
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;


/// <summary>
/// 炮弹兵撞击的表现命令
/// </summary>
public enum FlyCollisionAction {
    /// <summary>
    /// 继续飞行
    /// </summary>
    FlyDirectional        = 0,
    /// <summary>
    /// 停顿继续飞行
    /// </summary>
    PauseContinueFlyDirectional   = 1,
    /// <summary>
    /// 掉落船内
    /// </summary>
    DropInBoat            = 2,
    /// <summary>
    /// 掉落船外
    /// </summary>
    DropOutBoat           = 3,
    /// <summary>ContinueFlight
    /// 安全登船
    /// </summary>
    FlyFallStand               = 4,
    
}

public enum FlyDir {
    none,
    LeftTop,
    RightTop,
    LeftBottom,
    RightBottom,
    Top,
    Bottom,
    Left,
    Right
}

/// <summary>
/// 碰撞信息
/// </summary>
public class FlyCollisionInfo
{
    //是为正常炮战，否为海豚顶上来
    public bool bReleaseSkill;
    
    /// 撞击到的对象
    public Life lifemCollision;// 撞击到的对象
    public bool bVertical;// 撞击到对象的是否垂直放置
    
    /// <summary>
    /// 撞击后的动作表现
    /// </summary>
    public FlyCollisionAction  flyCollisionAction;
    public WalkDir  HitDir; /// 撞击方向
    
    //FlyCollisionAction==FlyFallStand时需传回下面参数
    public Int2  FlyfallSTandGridPos;// 撞击到的格子，即登船成功时的格子位置
    public Vector3 vhitPos;// 即登船成功时,最后撞击的坐标
    public AnimatorState m_flyFallStandState = AnimatorState.FlyFallStand00200;// 即登船成功时，所要设置的动作表现（有些炮弹兵可能会用多套动作表现）
    public bool m_bFlyFallStandSmooth = true;/// 即登船成功时，掉落后出生是否平滑移动到出生点（大部份是瞬移的，有些炮弹兵会有不同要求）
    
    /// <summary>
    /// 撞击到船板是否左侧
    /// </summary>
    public bool bLeft;
    
    //炮战伤害相关
    public bool bApplyDamage = true;// 后续是否继续造成伤害
    public int damage;// 炮战单次伤害
    public float DamageTimeInterval ;// 炮战伤害释放间隔
    
    public Vector3 droppos = Vector3.zero;
    /// <summary>
    /// 撞击到的召唤物
    /// </summary>
    public GameObject SummonPet = null;
    
    public void Reset()
    {
        FlyfallSTandGridPos = new Int2(0, 0);
        HitDir = 0;
        lifemCollision = null;
    }
    
}


public class Fly : Move
{
    public Vector3 m_FlyDir;//飞行方向
    /// <summary>
    /// 飞出屏幕（上，下，左 ，右）
    /// </summary>
    private FlyDir m_flyOutDir = FlyDir.none;
    public bool m_bDolphineFly = false;//是否海豚顶飞表现
    private Vector3 m_vhitJumpPos = Vector3.zero; //最后撞击落船位置
    public FlyCollisionInfo m_FlyInfo = new FlyCollisionInfo();//炮战过程和数据传送
    private bool m_bSpawnWater = false;
    private Vector3 m_v3SpawnWater = Vector3.zero;
    /// <summary>
    /// 记录炮战作用对象的ID
    /// </summary>
    private List<int> m_listCollisionGoID = new List<int>();
    public Fly(Life lifeOwner)
        : base(lifeOwner)
    {
        m_Owner.m_Attr.CurConcussion = m_Owner.m_Attr.Concussion;
    }
    
    /// <summary>
    /// 执行FixedUpdate调用
    /// </summary>
    public override void FixedUpdate()
    {
        if (CurrentAction != null) {
            if (!CurrentAction.IsDone()) {
                CurrentAction.Update();
            } else {
                CurrentAction = null;
            }
        }
        //判定是否飞出视窗区域
        if (!m_Owner.m_bReBorn) {
            DetectOutOfVisualZone();
        }
    }
    /// <summary>
    /// 超出 初始视窗 区域
    /// </summary>
    void DetectOutOfVisualZone()
    {
        DoSpawnDolphine();
        
        if (CurrentAction == null) {
            return;
        }
        if ((CurrentAction is GridActionCmdFly)) {
            GridActionCmdFly LineFly = CurrentAction as GridActionCmdFly;
            if (LineFly.CheckLineFly() == false) {
                return;
            }
        }
        
    }
    
    /// <summary>
    /// 根据具体情况执行 海豚顶飞 Action
    /// </summary>
    /// <param name="Angel"></param>
    /// <param name="isFallEnd"></param>
    void DoSpawnDolphine(bool isFallEnd = false)
    {
        Vector3 Pos = m_Owner.m_Skin.tRoot.position;
        float mid = (FlyLimitZone.minX + FlyLimitZone.maxX) * 0.5f;
        WalkDir dir = Pos.x < mid ? WalkDir.WALKLEFT : WalkDir.WALKRIGHT;
        //SpawnWaterEffect(Pos, dir);
        if (isFallEnd || CheckDolphineFly(Pos, CheckTraceFly())) {
            //WalkDir dir = Pos.x < mid ? WalkDir.WALKLEFT : WalkDir.WALKRIGHT;
            SpawnDolphine(dir, Pos);
        }
    }
    /// <summary>
    /// 生成水花特效
    /// </summary>
    void SpawnWaterEffect(Vector3 pos, WalkDir Dir, bool IsRotate = true)
    {
        Vector3 start = Vector3.zero;
        Vector3 end = Vector3.zero;
        if (BattleEnvironmentM.GetDolphineLine(Dir, ref start, ref end) == false) {
            return;
        }
        Transform FlyNode = BattleEnvironmentM.GetLifeMBornNode(false);
        Vector3 LeftPos = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, -Camera.main.transform.position.z - 10));
        Vector3 LeftRotation = new Vector3(0, 0, -15);
        Vector3 RightRotation = new Vector3(0, 0, 15);
        Vector3 RightPos = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, -Camera.main.transform.position.z - 10));
        Vector3 effectPos = Vector3.zero;
        if (Dir == WalkDir.WALKLEFT) {
            effectPos = LeftPos;
        } else {
            effectPos = RightPos;
        }
        if (m_flyOutDir == FlyDir.Bottom) {
            effectPos = pos;
            IsRotate = false;
        }
        GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "2000791", effectPos, null);
        if (gae != null) {
            if (IsRotate) {
                if (Dir == WalkDir.WALKLEFT) {
                    gae.transform.Rotate(LeftRotation);
                } else {
                    gae.transform.Rotate(RightRotation);
                }
            }
            GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(0.5f);
            gae.AddAction(ndEffect);
        }
    }
    
    /// <summary>
    /// 设置轨迹飞行Action
    /// </summary>
    /// <param name="pos">手势滑动点列表</param>
    public void SetTrailFly(List<Vector3> pos)
    {
        CurrentAction = FlyActionCmdFactory.Create(m_Owner, pos, m_Owner.m_Attr.Flyspeed, m_Owner.m_Attr.ModelType);
#if UNITY_EDITOR_LOG
        string str = "";
        for (int i = 0; i < pos.Count; i++) {
            str += pos[i] + ", ";
        }
        FileLog.write(m_Owner.SceneID, "SetTrailFly  "  + str);
#endif
        GridActionCmdFly fly = CurrentAction as GridActionCmdFly;
        if (fly != null) {
            fly.SetFlyCollisionInfo(m_FlyInfo);
        }
    }
    /// <summary>
    /// 设置拳击击飞Action(击飞状态下不拥有炮战技能)
    /// </summary>
    /// <returns></returns>
    public void SetHitFly(WalkDir Dir, float duration, float delay, bool bearhit)
    {
        m_FlyInfo.flyCollisionAction = FlyCollisionAction.DropOutBoat;
        Vector3 Target = BattleEnvironmentM.GetHitFlyEndPoint(m_Owner.m_thisT.position, Dir, bearhit);
        CurrentAction = new GridActionCmdHitFly(m_Owner, Target, duration, Dir, delay, bearhit);
    }
    
    /// <summary>
    /// 生成海豚顶飞Action
    /// </summary>
    /// <param name="Dir">海豚出现在左侧还是右侧</param>
    public void SpawnDolphine(WalkDir Dir, Vector3 pos)
    {
        m_FlyInfo.lifemCollision = null;
        if (!m_bDolphineFly) {
            m_FlyInfo.flyCollisionAction = FlyCollisionAction.FlyDirectional;//海豚顶飞后改变飞行状态，接受碰撞
            m_bDolphineFly = true;
            m_Owner.InitSkill();
#if UNITY_EDITOR_LOG
            FileLog.write(m_Owner.SceneID, "SpawnDolphine  ");
#endif
            float delay = 0.0f;
            if (m_Owner.m_Status.HaveState(StatusType.ClickFly)) {
                delay = m_Owner.m_Status.GetStateDuration(StatusType.ClickFly);
            }
            CurrentAction = new GridActionDophineFly(m_Owner, Dir, 600f, delay, m_v3SpawnWater);
        }
    }
    
    /// <summary>
    /// 海豚飞触发条件：1.fly阶段，2.非轨迹分型阶段，3.飞出视野阶段
    /// </summary>
    /// <param name="Angel"></param>
    /// <param name="isFallEnd"></param>
    private bool CheckDolphineFly(Vector3 Pos, bool IsTraceFly)
    {
        bool result = false;
        if (Pos.x > FlyLimitZone.maxX) {
            m_flyOutDir = FlyDir.Right;
            result = true;
        }
        if (Pos.x < FlyLimitZone.minX) {
            m_flyOutDir = FlyDir.Left;
            result = true;
        }
        if (Pos.y > FlyLimitZone.maxY && Pos.x > FlyLimitZone.minX && Pos.x < FlyLimitZone.maxX) {
            m_flyOutDir = FlyDir.Top;
            result = true;
        }
        if (Pos.y < FlyLimitZone.minY && Pos.x > FlyLimitZone.minX && Pos.x < FlyLimitZone.maxX) {
            m_flyOutDir = FlyDir.Bottom;
            result = true;
        }
        if (result && m_bSpawnWater == false) {
            m_bSpawnWater = true;
            Vector3 flyPos = m_Owner.m_Skin.tRoot.position;
            float mid = (FlyLimitZone.minX + FlyLimitZone.maxX) * 0.5f;
            WalkDir dir = flyPos.x < mid ? WalkDir.WALKLEFT : WalkDir.WALKRIGHT;
            SpawnWaterEffect(flyPos, dir, m_flyOutDir != FlyDir.Bottom);
            if (m_flyOutDir == FlyDir.Bottom) {
                m_v3SpawnWater = flyPos;
            }
            
        }
        if (IsTraceFly == true) {
            return false;
        }
        return result;
    }
    
    private bool CheckTraceFly()
    {
        if (CurrentAction == null) {
            return false;
        }
        if ((CurrentAction is GridActionCmdFly)) {
            return (CurrentAction as GridActionCmdFly).IsTraceFly;
        } else {
            return false;
        }
    }
    
    /// <summary>
    /// 撞击甲板掉落结束/击飞
    /// </summary>
    public  void FallEnd()
    {
        DoSpawnDolphine(true);
    }
    
    /// <summary>
    /// 飞行角色的碰撞处理
    /// </summary>
    public   override void ColliderProc(Collision collision)
    {
        //NGUIUtil.DebugLog("flyCollisionAction=" + m_FlyInfo.flyCollisionAction, "red");
        
        if (m_FlyInfo != null) { //当掉落船外后不再处理碰撞
            if (m_FlyInfo.flyCollisionAction == FlyCollisionAction.DropOutBoat) {
                return;
            }
        }
        FlyDir dir = FlyDir.none;
        if (CurrentAction is GridActionCmdFly) {
            dir = (CurrentAction as GridActionCmdFly).GetDir();
        } else if (CurrentAction is GridActionDophineFly) {
            dir = (CurrentAction as GridActionDophineFly).GetDir();
        } else if (CurrentAction is FireActionCmd) {
            dir = (CurrentAction as FireActionCmd).GetDir();
        }
        if (m_bDolphineFly && (dir == FlyDir.Top || dir == FlyDir.LeftTop || dir == FlyDir.RightTop)) {
            return;
        }
        Life lifeM = GetCollisionGo(collision);
        if (lifeM == null || lifeM.m_Core.m_Camp == LifeMCamp.ATTACK) {
            return;
        }
        
#if UNITY_EDITOR_LOG
        FileLog.write(m_Owner.SceneID, "ColliderProc  " +  collision.gameObject + lifeM.SceneID + ",  " + collision.contacts[0].point  + "," + dir);
#endif
        //计算撞击方向
        Vector3 target = collision.contacts[0].point;
        Vector3 start = m_Owner.m_thisT.position;
        Vector2 HitDir = new Vector2(target.x - start.x, target.y - start.y);
        
        bool bReleaseSkill = true;
        bReleaseSkill = !m_bDolphineFly;  //海豚顶飞，炮战技能不释放
        bool bApplyDamage = bReleaseSkill;
        if (m_Owner.m_Skill != null) {
            if (m_bDolphineFly == false) {
                bApplyDamage = m_FlyInfo.bApplyDamage;
            }
            (m_Owner.m_Skill as FireSkill).FlyInfo = m_FlyInfo;
        }
        Vector3 vHitWorldPos = new Vector3(target.x, target.y, target.z);
        
        HitSound(lifeM, m_Owner);
        HitShakeBoat();
#if UNITY_EDITOR_LOG
        FileLog.write(m_Owner.SceneID, "ColliderProc  " +  dir + "," + CurrentAction);
#endif
        FlyCollisionInfo Info = m_Owner.FireTrigger(collision, lifeM, m_Owner, bReleaseSkill, bApplyDamage, m_listCollisionGoID.Count, dir);
        if (Info != null && Info.lifemCollision != null) {
            Info.HitDir = HitDir.x > 0 ? WalkDir.WALKRIGHT : WalkDir.WALKLEFT;
            Info.vhitPos = BattleEnvironmentM.World2LocalPos(target);
            FireCombatOver(Info);
        }
    }
    
    private void HitShakeBoat()
    {
        Camera.main.transform.DOShakePosition(0.75f);
    }
    
    public void HitSound(Life lifeM, Life Attacker)
    {
        if (lifeM is IggFloor) {
            if (Attacker.m_Attr.AttrType == 100002 || Attacker.m_Attr.AttrType == 200001) {
                SoundPlay.Play("hit_deck_rui", false, false);
            } else if (Attacker.m_Attr.AttrType == 100003) {
                SoundPlay.Play("hit_deck_rui", false, false);
            } else if (Attacker.m_Attr.AttrType == 101001) {
                SoundPlay.Play("hit_deck_yidao", false, false);
            } else if (Attacker.m_Attr.AttrType == 101002) {
                if (lifeM is IggWall) {
                    SoundPlay.Play("hit_wall", false, false);
                } else {
                    SoundPlay.Play("hit_deck_money", false, false);
                }
            } else if (Attacker.m_Attr.AttrType == 102001) {
                SoundPlay.Play("hit_deck_bear", false, false);
            } else {
                SoundPlay.Play("hit_deck_rui", false, false);
            }
        }
    }
    /// <summary>
    /// 获取炮战对象
    /// </summary>
    private Life GetCollisionGo(Collision collision)
    {
        Transform T = collision.transform;
        int collisionGoID = T.gameObject.GetInstanceID();
        if (m_listCollisionGoID.Contains(collisionGoID)) {
            return null;
        }
        //m_listCollisionGoID.Add(collisionGoID);
        //墙板，建筑等对象
        LifeObj l = T.GetComponent<LifeObj>();
        if (l == null) {
            l = T.GetComponentInParent<LifeObj>();
            if (l == null) {
                return null;
            }
        }
        
        Life life = l.GetLife();
        //角色对象
        if (life == null) {
            Transform t = T.parent;
            if (t == null) {
                return null;
            }
            l =  t.GetComponent<LifeObj>();
            if (t == null) {
                return null;
            }
            
            Role w = l.GetLife() as Role;
            if (w == null) {
                return null;
            }
            if (w.m_Core.m_Camp == LifeMCamp.DEFENSE) {
                m_listCollisionGoID.Add(collisionGoID);
            }
            return w as Life;
        } else {
            if (life.m_Core.m_Camp == LifeMCamp.DEFENSE) {
                m_listCollisionGoID.Add(collisionGoID);
            }
            return life;
        }
    }
    
    /// <summary>
    /// 炮战撞击后的处理 临时屏蔽掉
    /// </summary>
    /// <param name="Info"></param>
    public void FireCombatOver(FlyCollisionInfo Info)
    {
        if (Info == null) {
            return ;
        }
        GetFlyInfo();
#if UNITY_EDITOR_LOG
        FileLog.write(m_Owner.SceneID, "FireCombatOver  " +  CurrentAction);
#endif
        if (CurrentAction is GridActionDophineFly) {
            CurrentAction.SetDone();
        }
        if (CurrentAction == null || (CurrentAction is FireActionCmd) == false) {
            CurrentAction = FireActionCmdFactory.Create(m_Owner, m_FlyDir, m_Owner.m_Attr.Flyspeed, m_bDolphineFly);
            if (Info.flyCollisionAction == FlyCollisionAction.FlyDirectional || Info.flyCollisionAction == FlyCollisionAction.PauseContinueFlyDirectional) {
                CurrentAction.m_Duration = 600;
            }
            FireActionCmd FireAction = CurrentAction as FireActionCmd;
            FireAction.SetCollisionInfo(Info, HitJump, FallEnd);
            return;
        } else if (CurrentAction is FireActionCmd) {
            FireActionCmd FireAction = CurrentAction as FireActionCmd;
            FireAction.SetCollisionInfo(Info, HitJump, FallEnd);
            if (Info.flyCollisionAction == FlyCollisionAction.FlyDirectional || Info.flyCollisionAction == FlyCollisionAction.PauseContinueFlyDirectional) {
                CurrentAction.m_Duration = 600;
            }
        }
    }
    
    /// <summary>
    /// 获取飞行信息
    /// </summary>
    public void GetFlyInfo()
    {
        if (CurrentAction == null) {
            return ;
        }
        if (CurrentAction is GridActionCmdFly) {
            GridActionCmdFly Fly = CurrentAction as GridActionCmdFly;
            m_FlyDir = Fly.GetLastFlyDir();
        }
    }
    
    /// <summary>
    /// 跳跃出生Action,该状态下不拥有炮战技能 临时屏蔽掉
    /// </summary>
    public void HitJump(FlyCollisionInfo Info)
    {
        m_Owner.m_Skill = null;
        m_Owner.m_Skin.EnableColider(ColiderType.Fire, false);
        MapGrid mg = MapGrid.GetMG(Info.FlyfallSTandGridPos);
        if (mg == null) {
            Debug.Log("获取格子失败" + Info.FlyfallSTandGridPos);
            return ;
        }
        
        Int2 pos = Int2.zero;
        Vector3 vHitLifeMGridPos = MapGrid.GetMG(Info.FlyfallSTandGridPos).pos;
        m_vhitJumpPos = new Vector3(Info.vhitPos.x, vHitLifeMGridPos.y, vHitLifeMGridPos.z);
        Int2 BornPos = new Int2(Info.FlyfallSTandGridPos.Unit, Info.FlyfallSTandGridPos.Layer);
        float fGridDisatance = Vector3.Distance(m_vhitJumpPos, vHitLifeMGridPos);
        // Info.HitDir = Info.bLeft ? WalkDir.WALKRIGHT : WalkDir.WALKLEFT;
        
        if (fGridDisatance > 0) {
            BornPos.Unit += (int)(fGridDisatance / MapGrid.m_width);
            mg = MapGrid.GetMG(BornPos);
            if (mg == null) {
                BornPos = new Int2(Info.FlyfallSTandGridPos.Unit, Info.FlyfallSTandGridPos.Layer);
            }
        }
        
        if (Info.m_bFlyFallStandSmooth) {
            m_vhitJumpPos =  MapGrid.GetMG(BornPos).pos;
        }
        
        KickSummonPet(Info);
        
        if (m_bDolphineFly) {
            Info.m_flyFallStandState = AnimatorState.FlyFallStand00200;
        } else {
            RepulseOthers(MapGrid.GetMG(BornPos));
        }
        RolePos = BornPos;
        
#if UNITY_EDITOR_LOG
        FileLog.write(m_Owner.SceneID, "Born  " +  RolePos);
#endif
        CurrentAction = BornActionCmdFactory.Create(m_Owner, m_vhitJumpPos, Info.HitDir, m_Owner.ActionFlyToWalk, Info.m_flyFallStandState, Info.m_bFlyFallStandSmooth);
    }
    
    //
    void RepulseOthers(MapGrid g)
    {
    
        List<Life> l = new List<Life>();
        List<Life> lr = new List<Life>();
        CM.SearchLifeMListInBoat(ref lr, LifeMType.SOLDIER, LifeMCamp.ALL);
        MapGrid left = g;
        for (int i = 0; i < 3; i++) {
            if (left.Left != null && left.Left.Type == GridType.GRID_NORMAL) {
                left = left.Left;
            } else {
                break;
            }
        }
        MapGrid right = g;
        for (int i = 0; i < 3; i++) {
            if (right.Right != null && right.Right.Type == GridType.GRID_NORMAL) {
                right = right.Right;
            } else {
                break;
            }
        }
        foreach (Role r in lr) {
            if (r.GetMapPos().Layer == g.GridPos.Layer) {
                if (r.m_thisT.localPosition.x >= left.pos.x && r.m_thisT.localPosition.x <= g.pos.x) {
                    /*MapGrid toleft = r.GetMapGrid();
                    for(int i =0; i < 3; i++)
                    {
                    	if (toleft.Left != null && toleft.Left.Type == GridType.GRID_NORMAL)
                    	{
                    		toleft = toleft.Left;
                    	}
                    	else break;
                    }*/
                    r.HitFirePower(0.5f, WalkDir.WALKLEFT, r.GetMapGrid());
                } else if (r.m_thisT.localPosition.x >= g.pos.x && r.m_thisT.localPosition.x <= right.pos.x) {
                
                    /*MapGrid toright = r.GetMapGrid();
                    for(int i =0; i < 3; i++)
                    {
                    	if (toright.Right != null && toright.Right.Type == GridType.GRID_NORMAL)
                    	{
                    		toright = toright.Right;
                    	}
                    	else break;
                    }*/
                    r.HitFirePower(0.5f, WalkDir.WALKRIGHT, r.GetMapGrid());
                }
            }
        }
    }
    /// <summary>
    /// 对于有召唤物的 处理
    /// </summary>
    /// <param name="Info"></param>
    void KickSummonPet(FlyCollisionInfo Info)
    {
        if (Info.SummonPet != null) {
            //MapBoat._Mapboat.GetWorldPos(m_vhitJumpPos).x
            if (Info.SummonPet.transform.position.x <= m_Owner.m_thisT.position.x) {
                Info.HitDir = WalkDir.WALKLEFT;
            } else {
                Info.HitDir = WalkDir.WALKRIGHT;
            }
            Pet pet = Info.SummonPet.GetComponentInChildren<LifeProperty>().GetLife() as Pet;
            if (pet != null && pet.PetMoveAI is PetFly1002) {
                if ((pet.PetMoveAI as PetFly1002).m_bHitFloor) {
                    //m_vhitJumpPos = pet.m_Skin.tRoot.localPosition;
                    pet.m_Skin.tRoot.position = U3DUtil.SetY(pet.m_Skin.tRoot.position, m_Owner.m_Skin.tRoot.position.y);
                    (pet.PetMoveAI as PetFly1002).KickBomb(Info.HitDir);
                } else {
                    Info.m_flyFallStandState = AnimatorState.FlyFallStand00200;
                    Object.Destroy(Info.SummonPet);
                }
            }
        }
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
}
