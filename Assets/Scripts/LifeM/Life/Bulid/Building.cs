#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    #define UNITY_EDITOR_LOG
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Build_AnimatorState 枚举为7位数 ABBCCCDE
//A表示参类型0 int ,1 Trigger,2 bool ,3 float
//BB表增对应动作控制器的参数索引Building.s_szBuildAnimatorParam
//CCCDE为参数值  CCC表示动作编号
public enum Build_AnimatorState {
    Stand10000 = 00110000, //待机0
    Stand10010 = 00110010, //待机1
    Stand10020 = 00110020, //待机2
    EditStand11000 = 00111000, //编辑初始待机动作
    Hit20000 = 00120000, //受击0
    Hit20010 = 00120010, //受击1
    Hit20020 = 00120020, //受击2
    Hit20030 = 00120030, //受击3
    Hit20040 = 00120040, //受击4
    Die20100 = 00120100, //死亡0
    Die20200 = 00120200, //死亡1
    Trigger30000 = 00130000, //触发1
    Trigger30100 = 00130100, //触发2
    CDOver40100 = 00140100, //CD结束待触发
    ReadyTriggered = 00100000, //
    
    
    
}
/// 建筑
/// <author>zhulin</author>
public class Building : Life
{
    public static string[] s_szBuildAnimatorParam =  new string[] {"iState"};
    protected List<float> m_Attacktimes = new List<float>();
    protected bool m_bTriggerSkill;
    protected float m_fTriggerTime = 0;
    protected float m_eventtime = 0;
    private float m_fAttackTickReady;
    private int m_nAttackCount;
    private int m_nAttackIndex;
    private float m_fReleaseDelay = 0;
    public bool m_bDark;
    
    
    //------buildingroom
    private float m_fShowHitActionTime;
    public  RoomType m_roomType;
    //private List<BuildingTrap> m_lTrap = new List<BuildingTrap>();
    private List<IggFloor> m_lLinkFloor = new List<IggFloor>();
    private List<IggWall> m_lWall = new List<IggWall>();
    private List<BuildingRoomBg> m_lBg = new List<BuildingRoomBg>();
    private List<Accessories> m_lAccessories = new List<Accessories>();
    
    //------buildingtrap
    protected GameObject AttackRangeInCircle01;//攻击范围 光效环（外环 表示可攻击的范围） 出现
    protected GameObject AttackRangeInCircle02;//攻击范围 光效环（外环 表示可攻击的范围） 循环
    
    protected GameObject AttackRangeOutCircle01;//无法攻击范围 光效环（外环 表示不可攻击的范围） 出现
    protected GameObject AttackRangeOutCircle02;//无法攻击范围 光效环（外环 表示不可攻击的范围） 循环
    
    protected GameObject AttackRangeSemiCircle01;//攻击范围 光效半圆环 出现
    protected GameObject AttackRangeSemiCircle02;//攻击范围 光效半圆环 循环
    
    public void SetBuildLife(BuildInfo Info, LifeProperty SkinProperty, LifeEnvironment Environment)
    {
        if (Info == null || SkinProperty == null) {
            return ;
        }
        LifeProperty(SkinProperty);
        if (Info.m_Skill != null) {
            m_Skill = new BuildSkill(Info);
        }
        if (Environment == LifeEnvironment.Combat) {
            //初始化技能
            m_Attr = new BuildAttribute(Info);
            m_Status = new Status();
            BuildColider bc = this.GetComponent<BuildColider>();
            if (bc != null && m_Attr.IsDamage) {
                bc.EnableBattleCollider(true);
            }
        } else {
            //HideHp();
            SetEditModeSetting(Info, Environment);
        }
    }
    
    public override void NDStart()
    {
        if (LifeEnvironment.Combat == Environment) {
            InitBuild();
        }
    }
    
    public BuildProperty m_Property;
    public override LifeProperty GetLifeProp()
    {
        return m_Property;
    }
    public override void SetLifeProp(LifeProperty lifeObj)
    {
        m_Property = lifeObj as BuildProperty;
    }
    public BuildingSkin m_Skin = null;
    public override Skin GetSkin()
    {
        return m_Skin;
    }
    public override bool SetSkin()
    {
        m_Skin = new BuildingSkin();
        if (m_Skin != null) {
            m_Skin.SetGameRole(this, MoveState.Walk, m_Core.m_Camp, m_Core.m_IsPlayer);
            m_Skin.ResetSkin();
        }
        
        return true;
    }
    public virtual void InitBuildModel()
    {
    }
    
    public virtual void InitBuild()
    {
        InitHP();
        HP = fullHP;
        
        SetSkin();
        //读取连击次数
        m_fAttackTickReady = 0;
        if (m_Skill != null) {
            BuildSkill skillBuild = m_Skill as BuildSkill;
            if (skillBuild.PropSkillInfo.m_attckmodeid > 0) {
                skillBuild.PropSkillInfo.GetAttackTimes(ref m_Attacktimes);
            } else {
                m_Attacktimes.Add(0.0f);
            }
        }
        m_nAttackCount = m_Attacktimes.Count;
        
        EventCenter.RegisterHooks(NDEventType.StatusCG, SetStatus);
        // -----buildingroom
        BuildColider bc = GetComponent<BuildColider>();
        if (bc != null) {
            bc.EnableBattleCollider(true);
        }
        BuildInfo info = CmCarbon.GetBuildInfo(this.m_Core.m_DataID);
        if (info != null) {
            ShowHP(info.BuildType, info.m_bear);
            m_roomType = info.m_RoomType ;
        }
    }
    /// <summary>
    /// 用于在NGUI处 显示特效
    /// </summary>
    public virtual void InitBuildUI()
    {
    
    }
    
    public override void NDUpdate(float deltaTime)
    {
        if (Environment == LifeEnvironment.Combat) {
        
            if (HP == 0 && m_Attr.IsDamage) {
                Dead();
            } else {
                UpdateInCombat() ;
            }
        } else if (Environment == LifeEnvironment.Edit) {
            UpdateInEdit() ;
        } else if (Environment == LifeEnvironment.View) {
            UpdateInView() ;
        }
    }
    
    /// <summary>
    /// 战斗环境
    /// </summary>
    public virtual void UpdateInCombat()
    {
    
        if (!m_bDark && m_Skin != null) {
            m_Skin.UpdataSkinEffect();
        }
        
        if (CheckCombatIng() == true) {
            if (m_Skill != null) {
                m_Skill.Update(Time.deltaTime);
            }
            if (m_Status != null) {
                m_Status.Update(Time.deltaTime);
            }
            UpdateActiveHidecdTimer(Time.deltaTime);
            if (m_Attr.Broken) {
                return ;
            }
            BuildUpdate();
        }
    }
    
    /// <summary>
    /// 战斗环境
    /// </summary>
    public virtual void UpdateInEdit()
    {
    
    }
    
    
    /// <summary>
    /// 战斗环境
    /// </summary>
    public virtual void UpdateInView()
    {
    
    }
    
    
    
    
    public virtual void BuildUpdate()
    {
        if (m_bTriggerSkill && Time.time - m_fTriggerTime > m_fReleaseDelay) {
            BuildSkill skillBuild = m_Skill as BuildSkill;
            List<Life> RoleList = skillBuild.GetBuildSkillTarget(this);
            if (RoleList.Count > 0) {
                bool bRet = false;
                m_fAttackTickReady += Time.deltaTime;
                if ((m_fAttackTickReady - Time.deltaTime) <= 0) {
                    QianYaoSkill(ref RoleList, ref m_nAttackIndex);
                }
                if (m_fAttackTickReady > m_eventtime) {
                    if (m_nAttackIndex < m_nAttackCount && m_fAttackTickReady > m_Attacktimes[m_nAttackIndex]) {
                        if ((m_Skill.PropSkillInfo.m_AttributeType & AttributeType.Electric) == AttributeType.Electric) {
                            Skill.GetWetTarget(ref RoleList, m_Skill.PropSkillInfo, this);
                        }
                        bRet = ReleaseSkill(ref RoleList, ref m_nAttackIndex);
                    }
                    if (bRet) {
                        m_nAttackIndex++;
                        if (m_nAttackIndex >= m_nAttackCount) {
                            m_bTriggerSkill = false;
                        }
                        if (m_nAttackIndex == 1 && m_Attr.Durability > 0) {
                            m_Attr.Durability = m_Attr.Durability - 1;
                            if (m_Attr.Durability <= 0) {
                                Dead();
                            }
                        }
                    }
                }
            }
        }
    }
    
    public override int SetLifeCore(LifeMCore Core)
    {
        int ret = base.SetLifeCore(Core);
        SetLifeData();
        return ret;
    }
    
    public void SetAnimator(Build_AnimatorState emState)
    {
        //临时
        if (m_Property == null) {
            return ;
        }
        
        if ((m_Property as BuildProperty).m_listanimator.Count == null) {
            return;
        }
        foreach (Animator animator in (m_Property as BuildProperty).m_listanimator) {
            if (!animator.enabled) {
                continue;
            }
            int nRule = (int)emState;
            int nType = nRule / 10000000;
            int nParam = nRule % 10000000 / 100000 - 1;
            string strParam = Building.s_szBuildAnimatorParam [nParam];
            int nValue = nRule % 100000;
            if (strParam.Length < 2) {
                return;
            }
            
            if (nType == 2/*strParam.StartsWith ("b")*/) {
                bool bValue = false;
                if (nValue != 0) {
                    bValue = true;
                }
                animator.SetBool(strParam, bValue);
            } else if (nType == 0/*strParam.StartsWith ("i")*/) {
                animator.SetInteger(strParam, nValue);
                animator.SetTrigger("tTriggerParam");
            } else if (nType == 1/*strParam.StartsWith ("t")*/) {
                animator.SetInteger(strParam, nValue);
                animator.SetTrigger("tTriggerParam");
            } else {
#if UNITY_EDITOR
                Debug.Log("无效接口变量");
#endif
            }
        }
    }
    
    public override void SetLifeData()
    {
        if (m_Skill != null) {
            if (m_Skill.Init(m_SceneID, m_Core) == true) {
                (m_Skill as BuildSkill).SkillTrrigerHandler = TriggerConditionHandler;
            }
        }
        if (m_Attr != null) {
            m_Attr.Init(SceneID, m_Core, this);
        }
        
        if (m_Status != null) {
            m_Status.Init(SceneID);
        }
        
        InBoat = true;
    }
    
    public virtual bool TriggerConditionHandler()
    {
        if (m_Attr.Broken) {
            return false;
        }
        if (m_Skill.PropSkillInfo == null) {
            return false;
        }
        BuildSkill skillBuild = m_Skill as BuildSkill;
        if (!skillBuild.CheckSkillTrigger(this)) {
            return false;
        }
        m_bTriggerSkill = true;
        m_fTriggerTime = Time.time;
        m_nAttackIndex = 0;
        m_fAttackTickReady = 0;
        ActiveHidecdTimer();
        List<Life> RoleList = skillBuild.GetBuildSkillTarget(this);
        if (RoleList.Count > 0) {
            TriggerSkillTime(ref RoleList, ref m_fReleaseDelay);
        }
        return true;
        
    }
    protected virtual bool QianYaoSkill(ref List<Life> RoleList, ref int nAttackIndex)
    {
        return false;
    }
    protected virtual bool ReleaseSkill(ref List<Life> RoleList, ref int nAttackIndex)
    {
        return false;
    }
    
    protected virtual void TriggerSkillTime(ref List<Life> RoleList, ref float fReleaseDelay)
    {
    
    }
    
    public virtual void HitFly(WalkDir Dir, float duration, float delay, bool bearhit = false)
    {
    
    }
    
    public override bool ApplyDamage(SkillReleaseInfo Info, Transform attackgo)
    {
    
        Shake();
        bool result = base.ApplyDamage(Info, attackgo);
        /*if (Info.m_struckeffect!=null&&Info.m_struckeffect != ""&& Info.m_struckeffect != "null")
        {
        
        		//SkillEffects._instance.LoadEffect("effect/prefab/", Info.m_struckeffect, t.position, 0.5f, true);
        	MapGrid g = GetMapGrid();
        	Vector3 pos = g.pos;
        	pos.y += 1.5f;
        	GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, Info.m_struckeffect, pos, m_thisT);
        	GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1);
        	gae.AddAction(ndEffect);
        }*/
        if (!m_bDark && attackgo == null) {
            m_Skin.PlayEffectColor(SkinEffectColor.BeHit, 0.12f);
        }
        if (result) {
            ActiveHidecdTimer();
            Hit(Info.m_Damage);
        }
        
        return result;
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
        //Info.bVertical = m_bVertical;
        Info.FlyfallSTandGridPos = GetMapPos();
        Info.lifemCollision = this;
        SkillReleaseInfo sInfo = new SkillReleaseInfo();
        sInfo.m_InterruptSkill = false;
        sInfo.m_MakeStatus = new List<StatusType> ();
        sInfo.m_bImmunity = false;
        sInfo.m_Damage = Damage;
        sInfo.Result = AttackResult.Fire;
        bool result =  ApplyDamage(sInfo, null);
        if (!m_isDead) {
            Info.FlyfallSTandGridPos = m_Attr.Pos;
            Info.bVertical = false;
            Info.flyCollisionAction = FlyCollisionAction.DropInBoat;
            //Debug.Log("not dead " + Info.flyCollisionAction + "," + Info.FlyfallSTandGridPos);
            Info.bApplyDamage = false;
        } else {
            Info.FlyfallSTandGridPos = m_Attr.Pos;
            Info.bVertical = false;
            Info.flyCollisionAction = FlyCollisionAction.FlyDirectional;
            //Debug.Log(" dead " + Info.flyCollisionAction + "," +Info.FlyfallSTandGridPos);
            Info.bApplyDamage = false;
        }
    }
    
    /// <summary>
    /// 判断life 是否在视野范围内
    /// </summary>
    /// <param name="life">被观察对象</param>
    /// <returns>在视野范围内 true，否则false</returns>
    public override bool CheckInVision(Life life)
    {
        if (life == null || life.m_Attr == null) {
            return false;
        }
        if (life.m_Attr.IsHide == true) {
            return false;
        }
        return true;
    }
    /// <summary>
    /// 是否是背景
    /// </summary>
    public bool IsBackGround()
    {
        if (m_Attr == null) {
            return false;
        }
        if (m_Attr.Type >= 2000 && m_Attr.Type < 3000) {
            return true;
        } else {
            return false;
        }
    }
    public bool IsInCDStatus()
    {
        if (m_Skill == null) {
            return  true;
        } else {
            BuildSkill skill = m_Skill as BuildSkill;
            return skill.IsInCDStatus();
        }
    }
    public float GetCDTime()
    {
        if (m_Skill == null) {
            return  0;
        } else {
            BuildSkill skill = m_Skill as BuildSkill;
            return skill.GetCDTime();
        }
    }
    
    public override void BeforeDead()
    {
        base.BeforeDead();
        int wood = m_Attr.Wood;
        if (wood > 0) {
            Vector3 pos = MapGrid.GetMG(m_Attr.Pos).pos;
            pos += new Vector3(0, 0.5f, 1.2f);//此处y控制深度, z控制高度
            
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
        CM.SearchLifeMListInBoat(ref listRAW, LifeMType.SOLDIER, LifeMCamp.ATTACK);
        
        foreach (var item in listRAW) {
            Role r = item as Role;
            r.ChangeTarget(ChangeTargetReason.ClearOtherTarget, this);
        }
        EventCenter.AntiRegisterHooks(NDEventType.StatusCG, SetStatus);
        Destroy();
    }
    public virtual void Hit(int damage)
    {
        //SetAnimator(Build_AnimatorState.Hit20000);
    }
    public virtual void Shake()
    {
        //-----buildingroom
        if (!isDead) {
            if (Time.time - m_fShowHitActionTime > 0.5f) {
                m_fShowHitActionTime = Time.time;
                //iTween.ShakePosition(m_thisT.gameObject, new Vector3(0.4f, 0.8f, 0), 0.5f);
            }
            
        }
    }
    
    public virtual void  Destroy()
    {
        SetAnimator(Build_AnimatorState.Die20100);
        (m_Property as BuildProperty).SetColor("_Emission", new Color(0, 0, 0, 0));
        
    }
    public virtual void Reset()
    {
    
        HP = fullHP;
        
        SetAnimator(Build_AnimatorState.Stand10000);
    }
    public override List<MapGrid> GetAllMapGrid()
    {
    
        return (m_Attr as BuildAttribute).GetAllMapGrid();
    }
    
    public override MapGrid GetMapGrid()
    {
        return MapGrid.GetMG(GetMapPos());
    }
    
    //作为寻路目标时，对应的地图格子
    public override MapGrid GetTargetMapGrid()
    {
        return GetMapGrid();
    }
    
    public override Int2 GetMapPos()
    {
        return m_Attr.Pos;
    }
    
    public bool CheckInBuildMap(Int2 Pos)
    {
        if (null == m_Attr) {
            return false;
        }
        return (m_Attr as BuildAttribute).CheckInBuildMap(Pos);
    }
    
    public override Vector3 GetPos()
    {
        return GetMapGrid().WorldPos;
    }
    
    /// <summary>
    /// 获取对象中心点，对于建筑物，在最下层的中心点
    /// </summary>
    public override Vector3 GetCenterPos()
    {
        return GetPos();
    }
    
    
    /// <summary>
    /// 生成房间损毁的特效
    /// </summary>
    public void SpawnRoomDamage()
    {
        GameObject go = GameObjectLoader.LoadPath("effect/prefab/", "1000031");
        if (go == null) {
            Debug.Log("BuildM.cs 房间损毁特效未加载");
            return;
        }
        Vector3 pos = GetPos();
        if (pos != Vector3.zero) {
            go.transform.position = pos;
        } else {
            go.transform.position = m_thisT.position;
        }
    }
    
    protected float m_fActiveHidecdTimer = 0.0f;
    public void ActiveHidecdTimer()
    {
        m_fActiveHidecdTimer = m_Attr.HideCd;
    }
    public void UpdateActiveHidecdTimer(float detaTime)
    {
        if (m_Attr == null) {
            return ;
        }
        m_fActiveHidecdTimer -= detaTime;
        if (m_fActiveHidecdTimer <= 0.0f && m_Attr.HideCd > 0) {
            m_Attr.TargetImmune = ImmuneTarget.ALL;
        } else {
            m_Attr.TargetImmune = ImmuneTarget.Normal;
        }
    }
    
    public override void KillSelf(float delay = 0)
    {
        EventCenter.AntiRegisterHooks(NDEventType.StatusCG, SetStatus);
        Dead();
        Destroy();
    }
    
    public override void SetDark(bool bDark)
    {
        if (!bDark) {
            if (this is Building) {
                if (m_Status.HaveState(StatusType.IceBuild)) {
                    return;
                }
            }
        }
        m_bDark = bDark;
        if (bDark) {
            (m_Property as BuildProperty).SetColor("_Color", Color.gray);
            (m_Property as BuildProperty).SetColor("_Emission", new Color(0.11f, 0.11f, 0.11f, 0.11f));
        } else {
            (m_Property as BuildProperty).SetColor("_Color", new Color(1, 1, 1, 1));
            (m_Property as BuildProperty).SetColor("_Emission", RoleSkinColor.Emission);
            
        }
    }
    
    /// <summary>
    /// 编辑模式下,建筑设置。
    /// </summary>
    public virtual void SetEditModeSetting(BuildInfo Info, LifeEnvironment Environment)
    {
        SetAnimator(Build_AnimatorState.EditStand11000);
    }
    
    public void SetStatus(int SceneID, object Param)
    {
        if (SceneID != this.SceneID) {
            return ;
        }
        
        if (Param is StatusInfo) {
            StatusInfo Info = Param as StatusInfo;
            if (Info.State == StatusState.Add) {
                SetSkillStatus(Info);
            } else if (Info.State == StatusState.Remove) {
                RemoveStatus(Info);
            }
            
        }
    }
    protected virtual void SetSkillStatus(StatusInfo Info)
    {
        if (Info == null) {
            return ;
        }
        
        AddStatusEffect(Info);
        if (Info.Type == StatusType.IceBuild) {
#if UNITY_EDITOR_LOG
            //Debug.Log("  add icebuild........................");
#endif
            //m_Skin.iGameRole.PauseAnimator();
            SetDark(true);
            m_Attr.Broken = true;
        } else if (Info.Type == StatusType.Paralyze) {
            Debug.Log("  add Paralyze........................");
            SetDark(true);
            SetAnimator(Build_AnimatorState.Hit20000);
            m_Attr.Broken = true;
        }
        
    }
    
    protected virtual void RemoveStatus(StatusInfo Info)
    {
        if (Info == null) {
            return ;
        }
        
        if (Info.Type == StatusType.IceBuild) {
            //m_Skin.iGameRole.ContiuneAnimator();
            SetDark(false);
            m_Attr.Broken = false;
        } else if (Info.Type == StatusType.Paralyze) {
            Debug.Log("  remove Paralyze........................");
            if (!m_Status.HaveState(StatusType.IceBuild)) {
                SetDark(false);
                m_Attr.Broken = false;
            }
            SetAnimator(Build_AnimatorState.Hit20020);
        }
    }
    
    // ---------------buildingroom
    /*public override void InitBuild ()
    {
    	base.InitBuild ();
    
    }*/
    
    
    /// <summary>
    /// 加入跟房间关联的板
    /// </summary>
    public void AddFloor(IggFloor f)
    {
        if (m_lLinkFloor == null) {
            m_lLinkFloor = new List<IggFloor>();
        }
        m_lLinkFloor.Add(f);
    }
    /// <summary>
    /// 加入墙体
    /// </summary>
    public void AddWall(IggWall w)
    {
        if (m_lWall == null) {
            m_lWall = new List<IggWall>();
        }
        m_lWall.Add(w);
    }
    /// <summary>
    /// 加入背景
    /// </summary>
    public void AddBg(BuildingRoomBg w)
    {
        if (m_lBg == null) {
            m_lBg = new List<BuildingRoomBg>();
        }
        m_lBg.Add(w);
    }
    public void AddAccessories(Accessories w)
    {
        if (m_lAccessories == null) {
            m_lAccessories = new List<Accessories>();
        }
        m_lAccessories.Add(w);
    }
    
    
    public void DestroyAttackRange()
    {
    
        ShowAttackRangeSemi(false);
        ShowAttackRange(false);
    }
    
    
    public void DemageByDeck(int demage)
    {
    
        HP += demage;
        if (HP < 0) {
            Dead();
        }
    }
    public override void Dead()
    {
    
    
        for (int i = 0; i < m_lLinkFloor.Count; i++) {
            if (m_lLinkFloor[i] != null) {
                m_lLinkFloor[i].DestroyRoom(this);
            }
        }
        m_lLinkFloor.Clear();
        for (int i = 0; i < m_lWall.Count; i++) {
            if (m_lWall[i] != null) {
                m_lWall[i].DestroyRoom(this);
            }
        }
        m_lWall.Clear();
        for (int i = 0; i < m_lBg.Count; i++) {
            if (m_lBg[i] != null) {
                m_lBg[i].DestroyRoom();
            }
        }
        m_lBg.Clear();
        for (int i = 0; i < m_lAccessories.Count; i++) {
            if (m_lAccessories[i] != null) {
                m_lAccessories[i].DestroyRoom(this);
            }
        }
        m_lAccessories.Clear();
        base.Dead();
    }
    /*public override bool ApplyDamage (SkillReleaseInfo Info, Transform attackgo)
    {
    	Shake ();
    	return base.ApplyDamage (Info, attackgo);
    }*/
    /*public override void Shake()
    {
    	if (!isDead)
    	{
    		if (Time.time-m_fShowHitActionTime>0.5f)
    		{
    			m_fShowHitActionTime = Time.time;
    			iTween.ShakePosition(m_thisT.gameObject, new Vector3(0.4f,0.8f,0), 0.5f);
    			for(int i = 0; i < m_lTrap.Count; i++)
    			{
    				m_lTrap[i].Shake();
    			}
    		}
    
    	}
    }*/
    
    public void AddStatusEffect(StatusInfo Info)
    {
        BuildInfo info = CmCarbon.GetBuildInfo(m_Core.m_DataID);
        for (int i = 0; i < info.m_Shape.height; i++) {
            for (int j = 0; j < info.m_Shape.width; j++) {
            
                if (info.m_Shape.GetShapeValue(i, j) == 1) {
                    MapGrid g = MapGrid.GetMG(info.m_cy + i, info.m_cx + j * MapGrid.m_UnitRoomGridNum);
                    GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", Info.effectid.ToString(), g.WorldPos, null);
                    if (gae != null) {
                        gae.transform.parent = m_thisT;
                        GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(Info.time);
                        gae.AddAction(ndEffect);
                    }
                }
            }
        }
    }
    
    /// <summary>
    ///  攻击范围 光效完整环
    /// </summary>
    /// <param name="isShow">是否显示</param>
    /// <param name="radius">攻击半径</param>
    public void ShowAttackRange(bool isShow, int radius = 1)
    {
        if (isShow) {
            if (AttackRangeInCircle01 == null && AttackRangeInCircle02 == null) {
                AttackRangeInCircle01 = GameObjectLoader.LoadPath("effect/prefab/", "2000661_01", m_thisT);
                Vector3 pos = AttackRangeInCircle01.transform.position;
                AttackRangeInCircle01.transform.position = U3DUtil.AddX(pos, 1.5f);
                pos = AttackRangeInCircle01.transform.position;
                AttackRangeInCircle01.transform.position = U3DUtil.AddY(pos, 1.5f);
                AttackRangeInCircle01.transform.Rotate(0, -180f, 0);
                GameObjectActionExcute gae = AttackRangeInCircle01.AddComponent<GameObjectActionExcute>();
                GameObjectActionWait wait = new GameObjectActionWait(1f, ShowInLoop01);
                GameObjectActionDelayDestory fade = new GameObjectActionDelayDestory(1.0f);
                gae.AddAction(wait);
                gae.AddAction(fade);
            }
            if (AttackRangeOutCircle01 == null && AttackRangeOutCircle02 == null) {
                AttackRangeOutCircle01 = GameObjectLoader.LoadPath("effect/prefab/", "2000671_01", m_thisT);
                Vector3 pos = AttackRangeOutCircle01.transform.position;
                AttackRangeOutCircle01.transform.position = U3DUtil.AddX(pos, 1.5f);
                pos = AttackRangeOutCircle01.transform.position;
                AttackRangeOutCircle01.transform.position = U3DUtil.AddY(pos, 1.5f);
                AttackRangeOutCircle01.transform.Rotate(0, -180f, 0);
                GameObjectActionExcute gae = AttackRangeOutCircle01.AddComponent<GameObjectActionExcute>();
                GameObjectActionWait wait = new GameObjectActionWait(1.0f, ShowInLoop02);
                GameObjectActionDelayDestory fade = new GameObjectActionDelayDestory(1.0f);
                gae.AddAction(wait);
                gae.AddAction(fade);
            }
        } else {
            U3DUtil.Destroy(AttackRangeInCircle01);
            U3DUtil.Destroy(AttackRangeInCircle02);
            U3DUtil.Destroy(AttackRangeOutCircle01);
            U3DUtil.Destroy(AttackRangeOutCircle02);
        }
    }
    
    private void ShowInLoop01(object o)
    {
        AttackRangeInCircle02 = GameObjectLoader.LoadPath("effect/prefab/", "2000661_02", m_thisT);
        Vector3 pos = AttackRangeInCircle02.transform.position;
        AttackRangeInCircle02.transform.position = U3DUtil.AddX(pos, 1.5f);
        pos = AttackRangeInCircle02.transform.position;
        AttackRangeInCircle02.transform.position = U3DUtil.AddY(pos, 1.5f);
        AttackRangeInCircle02.transform.Rotate(0, -180f, 0);
    }
    private void ShowInLoop02(object o)
    {
        AttackRangeOutCircle02 = GameObjectLoader.LoadPath("effect/prefab/", "2000671_02", m_thisT);
        Vector3 pos = AttackRangeOutCircle02.transform.position;
        AttackRangeOutCircle02.transform.position = U3DUtil.AddX(pos, 1.5f);
        pos = AttackRangeOutCircle02.transform.position;
        AttackRangeOutCircle02.transform.position = U3DUtil.AddY(pos, 1.5f);
        AttackRangeOutCircle02.transform.Rotate(0, -180f, 0);
    }
    
    /// <summary>
    ///  攻击范围 光效半圆环
    /// </summary>
    /// <param name="isShow">是否显示</param>
    /// <param name="radius">攻击半径</param>
    public void ShowAttackRangeSemi(bool isShow, float radius = 1.0f)
    {
        if (isShow) {
            if (AttackRangeSemiCircle02 == null && AttackRangeSemiCircle01 == null) {
                AttackRangeSemiCircle01 = GameObjectLoader.LoadPath("effect/prefab/", "2000681_01", m_thisT);
                Vector3 pos = AttackRangeSemiCircle01.transform.position;
                AttackRangeSemiCircle01.transform.position = U3DUtil.AddX(pos, 1.5f);
                AttackRangeSemiCircle01.transform.Rotate(0, -180f, 0);
                GameObjectActionExcute gae = AttackRangeSemiCircle01.AddComponent<GameObjectActionExcute>();
                GameObjectActionWait wait = new GameObjectActionWait(1 + Time.deltaTime, ShowSemiLoop);
                GameObjectActionDelayDestory fade = new GameObjectActionDelayDestory(1f);
                gae.AddAction(wait);
                gae.AddAction(fade);
            }
        } else {
            if (AttackRangeSemiCircle01) {
                GameObject.Destroy(AttackRangeSemiCircle01);
            }
            if (AttackRangeSemiCircle02) {
                GameObject.Destroy(AttackRangeSemiCircle02);
            }
        }
        
    }
    
    private void ShowSemiLoop(object o)
    {
        AttackRangeSemiCircle02 = GameObjectLoader.LoadPath("effect/prefab/", "2000681_02", m_thisT);
        Vector3 pos = AttackRangeSemiCircle02.transform.position;
        AttackRangeSemiCircle02.transform.position = U3DUtil.AddX(pos, 1.5f);
        AttackRangeSemiCircle02.transform.Rotate(0, -180f, 0);
    }
    
    /// <summary>
    ///  查看陷阱时播放的动作接口
    /// </summary>
    public virtual void PlayViewAni()
    {
        SetAnimator(Build_AnimatorState.Stand10000);
    }
    /// <summary>
    /// 点击陷阱时播放动作接口
    /// </summary>
    public virtual void PlayClickAni()
    {
        if (m_Skill != null) {
            SetAnimator(Build_AnimatorState.Trigger30000);
        } else {
            SetAnimator(Build_AnimatorState.Stand10000);
        }
    }
    public static void ShowAllHp(bool bshow)
    {
        List<Life> l = new List<Life>();
        CM.SearchLifeMListInBoat(ref l, LifeMType.BUILD);
        foreach (Life b in l) {
            (b as Building).ShowAlwayHp(bshow);
        }
    }
    
    public void ShowAlwayHp(bool bshow)
    {
        if (MyHPAction() != null) {
            if (MyHPAction() is BuildHPAciton) {
                MyHPAction().gameObject.SetActive(true);
                BuildHPAciton buildHpAction = MyHPAction() as BuildHPAciton;
                buildHpAction.ShowAlwayHp(bshow);
            }
        }
    }
    
    public void ShowEditHp(int bear)
    {
        if (m_Skin == null) {
            m_Skin = new BuildingSkin();
            m_Skin.SetGameRole(this, MoveState.Fly, LifeMCamp.ATTACK, true);
        }
        
        m_Skin.ShowHP(1, 1, 1, bear);
        ShowAlwayHp(true);
    }
}



