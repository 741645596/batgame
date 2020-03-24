#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    #define UNITY_EDITOR_LOG
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class   RoleGridRun : GridRun
{
    private int  m_NextAttackSceneID =  -1;
    //强制去的格子
    
    int  m_bForce = -1;
    Int2  m_ForceGrid = Int2.zero;
    WalkDir  m_ForceDir = WalkDir.WALKSTOP;
    //预测攻击
    private bool  m_Attack = false;
    public bool Attack {
        get{return m_Attack;}
        set{m_Attack = value;}
    }
    //判断路径是否往前移一个；
    private bool m_IndexAdd;
    public bool m_DoUpatePath = false;
    
    public bool Reject {
        get{return m_Reject;}
        set{m_Reject = value;}
    }
    
    //跟目标距离中有空位
    private bool m_Pass = true;
    public bool Pass {
        get{return m_Pass;}
        set{m_Pass = value;}
    }
    
    public bool m_FindAttackpos = false;
    
    public RoleGridRun(Life role)
        : base(role)
    {
    
    }
    
    
    public Role RoleParent {
        get { return m_parent as Role; }
    }
    
    
    
    
    public void SetBorn(Int2 BornPos, int deep)
    {
        MapPos = BornPos;
        m_AttackStation = MapPos;
        m_NAttackStation = MapPos;
        m_NGrid = BornPos;
        RankDeep = deep;
        //
        MapGrid m = MapGrid.GetMG(MapPos);
        if (m != null) {
            if (MapM.AskForMapGridDeep(m_SceneID, m, deep, RoleParent.m_Core.m_Camp, DIR.LEFT, SEARCHAGT.SAGTY_POLL, ref BornPos, ref deep)) {
                m = MapGrid.GetMG(BornPos);
                RankDeep = deep;
                MapCheckStations.RoleStation(m_SceneID, m, new StationsInfo(RoleParent.m_Attr,
                        RankDeep,
                        RoleParent.m_Core.m_Camp,
                        DIR.LEFT,
                        GridSpace.Space_DOWN));
            }
        }
        
    }
    
    public  void ClearPath()
    {
        Path.ClearSeachPath();
    }
    
    public void UpdataPath()
    {
        if (! m_UpdataPathFlag) {
            return;
        }
        if (!m_DoUpatePath) {
            return;
        }
        
#if UNITY_EDITOR_LOG
        if (m_AIEventData != null) {
            FileLog.write(m_SceneID, "UpdataPath1" +  m_AIEventData.Event + "," + m_UpdataPathFlag);
        }
#endif
        m_DoUpatePath = false;
        m_UpdataPathFlag = false;
        if (Path.CheckRunFinishRoad()) {
            m_FindAttackpos = false;
            EventCenter.DoEvent(NDEventType.StatusInterrupt, m_SceneID, LifeAction.WalkToEnd);
        }
        if (Path.CheckHavePath() == false) {
            RoleParent.RoleWalk.SeachTarget();
            if (Path.CheckHavePath() == false) {
                m_FindAttackpos = false;
                CheackIdlePath();
            }
        } else {
            if (RoleParent.RoleWalk.DoPathCheck(m_AIEventData)) {
#if UNITY_EDITOR_LOG
                FileLog.write(m_SceneID, "DOUpdataPath2" +  m_AIEventData.Event + "," + m_UpdataPathFlag);
#endif
                m_AIEventData = null;
                ClearPath();
                RoleParent.RoleWalk.SeachTarget();
            } else {
                //m_UpdataPathFlag = true;
                //ClearPath();
                CheackIdlePath();
            }
        }
    }
    public MapGrid GetFreeAttackStation()
    {
        MapGrid lfreeg = RoleParent.GetMapGrid().Left;
        MapGrid rfreeg = RoleParent.GetMapGrid().Right;//MapGrid.GetMG(MapPos.Layer,MapPos.Unit).Right;
        int i = 1;
        
        /*do
        {
        	lfreeg = MapGrid.GetMG(MapPos.Layer,MapPos.Unit -i);
        	if (lfreeg != null && lfreeg.CheckIdle(m_SceneID))
        		return lfreeg;
        	rfreeg = MapGrid.GetMG(MapPos.Layer,MapPos.Unit +i);
        	if (rfreeg != null && rfreeg.CheckIdle(m_SceneID))
        		return rfreeg;
        	i++;
        }*/
        while (lfreeg != null || rfreeg != null) {
            if (lfreeg != null) {
                if (lfreeg.IsAttackStations()) {
                    return lfreeg;
                } else {
                    lfreeg = lfreeg.Left;
                }
            }
            if (rfreeg != null) {
                if (rfreeg.IsAttackStations()) {
                    return rfreeg;
                } else {
                    rfreeg = rfreeg.Right;
                }
            }
        }
        return null;
    }
    //获取一个可以攻击的攻击位，这个攻击位只能是没人或己方阵营
    public MapGrid GetCanAttackStation()
    {
        MapGrid lfreeg = RoleParent.GetMapGrid().Left;
        MapGrid rfreeg = RoleParent.GetMapGrid().Right;//MapGrid.GetMG(MapPos.Layer,MapPos.Unit).Right;
        int i = 1;
        
        
        while (lfreeg != null || rfreeg != null) {
            if (WalkDIR == WalkDir.WALKRIGHT) {
                if (lfreeg != null) {
                    if (lfreeg.IsAttackStations()) {
                        List<int> samelist = new List<int>();
                        List<int> unsamelist = new List<int>();
                        lfreeg.GetCampRoleList(RoleParent.m_Core.m_Camp, ref samelist, ref unsamelist);
                        if (unsamelist.Count > 0) {
                            lfreeg = lfreeg.Left;
                        } else {
                            return lfreeg;
                        }
                    } else {
                        lfreeg = lfreeg.Left;
                    }
                }
                if (rfreeg != null) {
                    if (rfreeg.IsAttackStations()) {
                        List<int> samelist = new List<int>();
                        List<int> unsamelist = new List<int>();
                        rfreeg.GetCampRoleList(RoleParent.m_Core.m_Camp, ref samelist, ref unsamelist);
                        if (unsamelist.Count > 0) {
                            rfreeg = rfreeg.Right;
                        } else {
                            return rfreeg;
                        }
                    } else {
                        rfreeg = rfreeg.Right;
                    }
                }
            } else {
                if (rfreeg != null) {
                    if (rfreeg.IsAttackStations()) {
                        List<int> samelist = new List<int>();
                        List<int> unsamelist = new List<int>();
                        rfreeg.GetCampRoleList(RoleParent.m_Core.m_Camp, ref samelist, ref unsamelist);
                        if (unsamelist.Count > 0) {
                            rfreeg = rfreeg.Right;
                        } else {
                            return rfreeg;
                        }
                    } else {
                        rfreeg = rfreeg.Right;
                    }
                }
                if (lfreeg != null) {
                    if (lfreeg.IsAttackStations()) {
                        List<int> samelist = new List<int>();
                        List<int> unsamelist = new List<int>();
                        lfreeg.GetCampRoleList(RoleParent.m_Core.m_Camp, ref samelist, ref unsamelist);
                        if (unsamelist.Count > 0) {
                            lfreeg = lfreeg.Left;
                        } else {
                            return lfreeg;
                        }
                    } else {
                        lfreeg = lfreeg.Left;
                    }
                    
                }
            }
        }
        return null;
    }
    public bool CheckAttackPos()
    {
        //拆解机器人不需要找攻击位
        if (RoleParent.m_Attr.AttrType == 3000) {
            return true;
        }
        MapGrid attackstation = MapGrid.GetMG(m_AttackStation);
        if (NdUtil.IsSameMapPos(MapPos, m_AttackStation) && attackstation.IsAttackStations()) {
            List<int> samelist = new List<int>();
            List<int> unsamelist = new List<int>();
            attackstation.GetCampRoleList(RoleParent.m_Core.m_Camp, ref samelist, ref unsamelist);
            if (unsamelist.Count > 0) {
            
                if (!m_FindAttackpos) {
#if UNITY_EDITOR_LOG
                    FileLog.write(m_SceneID, "CheckAttackPos" +  Path.Indext + "," +  m_ForceGrid + "," +  m_NGrid + "," + MapPos + "," + m_AttackStation + "," + m_NAttackStation
                        + "," +   m_Attack + "," + RoleParent.m_Skill.CanAttack()  + "," + Time.realtimeSinceStartup);
#endif
                        
                    MapGrid g = GetCanAttackStation();
                    if (g != null && (!Life.ServerMode || !RoleParent.WaitServer)) {
                        m_FindAttackpos = true;
                        RoleParent.ChangeTarget(ChangeTargetReason.ClearTarget, null);
                        ClearPath();
                        Path.PathAI(m_SceneID, MapGrid.GetMG(MapPos), g, RoleParent.m_Attr, CurrentGS, WalkDIR);
                        PathInit();
                    } else {
#if UNITY_EDITOR_LOG
                        //Debug.Log("没有空闲攻击位 " + MapPos );
#endif
                        return  true;
                    }
                    /*//判断首个节点是否为楼梯
                    PathData Road = Path.GetPathData(PathAccess.Cur);
                    if(Road != null && Road.state == RoleState.STAIR)
                    {
                    	m_Reject = true;
                    	m_DoUpatePath = false;
                    }
                    
                    //获取路线上的下一个攻击位，包含当前路径点
                    ErrorInfo Error = new ErrorInfo();
                    PathData NextAttackStation = Path.GetNextAttackPosCurInPath(ref Error);
                    if(NextAttackStation != null)
                    {
                    	m_AttackStation = NextAttackStation.Road.GridPos;
                    	m_NAttackStation = NextAttackStation.Road.GridPos;
                    }*/
                    
                    FirstCheckAttack();
                }
                return false;
            }
        }
        return  true;
    }
    
    public bool CheackIdlePath()
    {
        /*if (!CheckHaveIdleAttackPosInPath(m_SceneID))
        {
        	MapGrid attackstation =MapGrid.GetMG(m_AttackStation);
        	if (attackstation != null)
        	{
        		#if UNITY_EDITOR_LOG
        		FileLog.write(m_SceneID,"CheackIdlePath   m_AttackStation: " + m_AttackStation + "MapPos:" + MapPos + "attackstation.GetRoleStateInStation(m_SceneID):" + attackstation.GetRoleStateInStation(m_SceneID));
        		#endif
        		if ( (NdUtil.IsSameMapPos(MapPos,m_AttackStation) && attackstation.IsAttackStations()))
        		{
        			FirstCheckAttack();
        			if (!m_Attack)
        				m_Pass = false;
        			return;
        		}
        		else
        		{
        			MapGrid g = GetFreeAttackStation();
        			if (g != null)
        			{
        				ClearPath();
        				Path.PathAI(m_SceneID,MapGrid.GetMG(MapPos),g,RoleParent.m_Attr as RoleAttribute,CurrentGS,WalkDIR);
        			}
        			else
        			{
        				#if UNITY_EDITOR_LOG
        				//Debug.Log("没有空闲攻击位 " + MapPos );
        				#endif
        			}
        		}
        	}
        
        }
        else
        	m_Pass = true;
        //判断首个节点是否为楼梯
        PathData Road = Path.GetPathData(PathAccess.Cur);
        if(Road != null && Road.state == RoleState.STAIR)
        {
        	m_Reject = true;
        	m_DoUpatePath = false;
        }
        
        //获取路线上的下一个攻击位，包含当前路径点
        ErrorInfo Error = new ErrorInfo();
        PathData NextAttackStation = Path.GetNextAttackPosCurInPath(ref Error);
        if(NextAttackStation != null)
        {
        	m_AttackStation = NextAttackStation.Road.GridPos;
        	m_NAttackStation = NextAttackStation.Road.GridPos;
        }*/
        if (Path.SearchPath.Count == 0 || Path.SearchPath.Count == 1 || (Path.SearchPath.Count  == 2 && !Path.SearchPath[0].Road.IsAttackStations() && !Path.SearchPath[1].Road.IsAttackStations())) {
            MapGrid attackstation = MapGrid.GetMG(m_AttackStation);
            if (!(NdUtil.IsSameMapPos(MapPos, m_AttackStation) && attackstation.IsAttackStations())) {
                MapGrid g = GetFreeAttackStation();
                if (g != null) {
                    ClearPath();
                    Path.PathAI(m_SceneID, MapGrid.GetMG(MapPos), g, RoleParent.m_Attr, CurrentGS, WalkDIR);
                    PathInit();
                    return true;
                } else {
#if UNITY_EDITOR_LOG
                    //Debug.Log("没有空闲攻击位 " + MapPos );
#endif
                }
            } else if (RoleParent.m_Attr.AttrType != 3000) {
            
            
                List<int> samelist = new List<int>();
                List<int> unsamelist = new List<int>();
                attackstation.GetCampRoleList(RoleParent.m_Core.m_Camp, ref samelist, ref unsamelist);
                if (unsamelist.Count > 0) {
                
#if UNITY_EDITOR_LOG
                    FileLog.write(m_SceneID, "CheackIdlePath- CheckAttackPos" +  Path.Indext + "," +  m_ForceGrid + "," +  m_NGrid + "," + MapPos + "," + m_AttackStation + "," + m_NAttackStation
                        + "," +   m_Attack + "," + RoleParent.m_Skill.CanAttack()  + "," + Time.realtimeSinceStartup);
#endif
                        
                    MapGrid g = GetCanAttackStation();
                    if (g != null) {
                        m_FindAttackpos = true;
                        RoleParent.ChangeTarget(ChangeTargetReason.ClearTarget, null);
                        ClearPath();
                        Path.PathAI(m_SceneID, MapGrid.GetMG(MapPos), g, RoleParent.m_Attr, CurrentGS, WalkDIR);
                        PathInit();
                        return true;
                    } else {
#if UNITY_EDITOR_LOG
                        //Debug.Log("没有空闲攻击位 " + MapPos );
#endif
                    }
                }
            }
            
        }
        return false;
    }
    
    public void PathInit()
    {
        if (!Life.ServerMode ||  RoleParent.islocal) {
            Path.SetIndex(0);
            Path.Path = Path.SearchPath;
            //判断首个节点是否为楼梯
            PathData Road = Path.GetPathData(PathAccess.Cur);
            if (Road != null && Road.state == RoleState.STAIR) {
                Reject = true;
                m_DoUpatePath = false;
            }
            
            //获取路线上的下一个攻击位，包含当前路径点
            ErrorInfo Error = new ErrorInfo();
            PathData NextAttackStation = Path.GetNextAttackPosCurInPath(ref Error);
            if (NextAttackStation != null) {
                m_AttackStation = NextAttackStation.Road.GridPos;
                m_NAttackStation = NextAttackStation.Road.GridPos;
            }
            if (!Path.CheckHavePath()) {
                return;
            }
        }
        RoleParent.WaitServer = true;
        tga.SoldierRunRoadRequest srrr = new tga.SoldierRunRoadRequest();
        srrr.time = new tga.tgaTime();
        srrr.time.Requesttime = Time.time;
        srrr.start = new tga.GridPos();
        srrr.start.layer = MapPos.Layer;
        srrr.start.unit = MapPos.Unit;
        srrr.end = new tga.GridPos();
        srrr.end.layer = Path.SearchPath[Path.SearchPath.Count - 1].Road.GridPos.Layer;
        srrr.end.unit = Path.SearchPath[Path.SearchPath.Count - 1].Road.GridPos.Unit;
        for (int i = 0; i < Path.SearchPath.Count; i ++) {
            srrr.roadlist.Add(Path.SearchPath[i].toPathRoad());
        }
        BSC.RunRoadRequest(RoleParent.m_Core.m_DataID, srrr);
#if UNITY_EDITOR_LOG
        FileLog.write(RoleParent.SceneID, "RunRoadRequest   " + Time.time, true);
#endif
    }
    public void ServerPathInit(tga.SoldierRunRoadResponse info)
    {
        if (!Life.ServerMode) {
            return;
        }
        float delatime = Time.time - info.time.Requesttime;
        float timecount = 0f;
        int i = 0;
        Path.ClearPath();
        bool flag = false;
#if UNITY_EDITOR_LOG
        FileLog.write(RoleParent.SceneID, "ServerPathInit  " + Time.time, true);
#endif
        for (; i < info.roadlist.Count; i++) {
            if (!flag) {
                timecount += info.roadlist[i].deltaTime * 0.001f;
                if (delatime < timecount) {
                    flag = true;
                    Path.Path.Add(new PathData(info.roadlist[i]));
                    //Path.SetIndex(i+1);
                    //Role r = RoleParent as Role;
                    //if (r.CurrentAction is GridActionCmdStand)
                    {
                        //Path.Path.Add(new PathData(info.roadlist[i]));
                        //r.CurrentAction.SetDone();
                        //Path.Path[0].deltaTime = timecount - delatime;
                        
                    }
                    
                    
                }
            } else {
                Path.Path.Add(new PathData(info.roadlist[i]));
            }
        }
        if (!RoleParent.islocal) {
            //判断首个节点是否为楼梯
            PathData Road = Path.GetPathData(PathAccess.Cur);
            if (Road != null && Road.state == RoleState.STAIR) {
                Reject = true;
                m_DoUpatePath = false;
            }
            
            //获取路线上的下一个攻击位，包含当前路径点
            ErrorInfo Error = new ErrorInfo();
            PathData NextAttackStation = Path.GetNextAttackPosCurInPath(ref Error);
            if (NextAttackStation != null) {
                m_AttackStation = NextAttackStation.Road.GridPos;
                m_NAttackStation = NextAttackStation.Road.GridPos;
            }
            
            if (RoleParent.CurrentAction is GridActionCmdStand) {
                Reject = false;
                
            }
            if (!Path.CheckHavePath()) {
                return;
            }
        }
        //map
        
    }
    public void PathAI(MapGrid start, MapGrid end, NDAttribute Attr, bool CutPath = false)
    {
        Path.PathAI(m_SceneID, start, end, RoleParent.m_Attr, CurrentGS, WalkDIR);
        if (CutPath) {
        
#if UNITY_EDITOR_LOG
            FileLog.write(RoleParent.SceneID, "PathAI");
#endif
            Life target = RoleParent.RoleWalk.m_RadarAI.GetTarget(Path, Attr.AttackLike);
            if (RoleParent.CheckTarget(target)) {
                RoleParent.ChangeTarget(ChangeTargetReason.InGoldPath, target);
            } else {
                /*target = RoleParent.RoleWalk.GetAttacklikeTarget();
                Path.PathAI(m_SceneID,start,target.GetTargetMapGrid(),Attr,CurrentGS,WalkDIR);*/
            }
            //r.Target =  r.RoleWalk.m_RadarAI.GetTarget(Path,Attr.AttackLike);
        }
        
        if (!CheackIdlePath()) {
            PathInit();
        }
        
        
        FirstCheckAttack();
    }
    
    //计算路径上的时间
    private void FirstCheckAttack()
    {
        if (RoleParent.m_Status.HaveState(StatusType.RunAway)) {
            m_Attack = false;
            return;
        }
        //没有路径
        if (Path.CheckHavePath() == false) {
            return ;
        }
        //获取首个节点
        PathData First = Path.GetPathData(PathAccess.Cur, 0);
        if (First.state == RoleState.FALL) {
            m_Attack = false;
            m_NextAttackSceneID = -1;
        } else {
            m_NGrid =  MapPos;
            WalkDIR =  First.dir;
            /*if (!RoleParent.m_Status.CheckStateBySkill(1022))
            {*/
            if (m_NextAttackSceneID == -1 || !RoleParent.m_Skill.CheckAttackTarget()) {
                // 攻击优先
                if (RoleParent.m_Skill.GetAttackTarget(ref m_NextAttackSceneID)) {
                    m_AttackDir =  First.dir;
                    m_Attack = true;
                } else {
                    m_AttackDir = WalkDir.WALKSTOP;
                    m_Attack = false;
                }
            } else {
                m_Attack = true;
            }
            /*}
            else
            	m_Attack = false;*/
        }
    }
    
    public void ChangeDeep(int deep)
    {
        //当深度调整时调整
        RoleParent.m_ajustcount = 0;
        RoleParent.m_ajustfrompostion = RoleParent.m_thisT.localPosition;
        Vector3 offset = Vector3.zero;
        if (WalkDIR == WalkDir.WALKLEFT) {
            offset.x += Role.s_deepoffset * deep;
        } else {
            offset.x -= Role.s_deepoffset * deep;
        }
        offset.z = Skin.s_DeepDistint * deep;
        RoleParent.m_ajusttopostion = RoleParent.GetMapGrid().pos + offset;
    }
    
    public void GetNextDeep()
    {
        int deep = MapM.GetRankDeep(m_SceneID);
        if (deep != -1) {
            if (RankDeep != deep) {
                ChangeDeep(deep);
            }
            RankDeep = deep;
        }
    }
    
    public  void CheckAttack()
    {
    
        if (RoleParent.m_Status.HaveState(StatusType.RunAway)) {
            m_Attack = false;
            return;
        }
        /*if (!RoleParent.m_Status.CheckStateBySkill(1022))
        {*/
        if (!m_FindAttackpos) {
            if (m_NextAttackSceneID == -1 || !RoleParent.m_Skill.CheckAttackTarget()) {
                // 攻击优先
                if (RoleParent.m_Skill.GetAttackTarget(ref m_NextAttackSceneID)) {
                    if (Path.CheckHavePath() == true) {
                        m_AttackDir = WalkDIR;
                    }
                    
                    m_Attack = true;
                }
                
                else {
                    m_AttackDir = WalkDir.WALKSTOP;
                    m_Attack = false;
                }
            } else {
                m_Attack = true;
            }
        } else {
            m_Attack = false;
        }
        /*	}
        else
        	m_Attack = false;*/
        GetNextDeep();
    }
    
    
    public void  Remove()
    {
        ClearPath();
    }
    
    
    
    public  MapGrid GetTargetMapGrid()
    {
        return  MapGrid.GetMG(MapPos);
    }
    public  MapGrid GetAttackStationMapGrid()
    {
        return  MapGrid.GetMG(m_AttackStation);
    }
    public void ResetIndex(bool ismove)
    {
        if (!ismove && m_IndexAdd) {
            Path.ResetPath();
            PathData CurRoad = Path.GetPathData(PathAccess.Cur);
            if (CurRoad != null) {
                MapPos =  CurRoad.Road.GridPos;
            }
            m_NGrid =  MapPos;
        }
    }
    
    //取下一个动作
    public GridActionCmd GetNextAction()
    {
        Vector3 pos = RoleParent.m_thisT.localPosition;
        float cdtime = RoleParent.CDTime;
        //int bodyradius = RoleParent.m_Attr.Radius;
        RoleSkill roleskill = RoleParent.m_Skill as RoleSkill;
        GridActionCmd action = null;
        m_IndexAdd = false;
        if (RoleParent.isDead) {
        
            action = new GridActionCmdDie(pos, 3 * 60 * 60f, WalkDIR, RankDeep);
            action.SetTarget(RoleParent);
            Reject = false;
            return action;
        }
        
        
        if (RoleParent == null || RoleParent.m_Attr == null) {
            action = new GridActionCmdStand();
            action.SetData(pos, pos, RoleState.STAND, Time.deltaTime, -1, WalkDIR, RankDeep);
            action.SetTarget(RoleParent);
            return action;
        }
        
        if (m_bForce == 0) {
            action = new GridActionCmdSpecialJump(0.5f, MapGrid.GetMG(MapPos).pos, MapGrid.GetMG(m_ForceGrid).pos, m_ForceDir, RankDeep);
            action.SetTarget(RoleParent);
            m_bForce = -1;
            SetUpdataPath(new AIEventData(AIEvent.EVENT_MAP));
            m_NGrid =  m_ForceGrid;
            if (MapGrid.GetMG(MapPos).PropStations == StationsProp.ATTACK) {
                m_AttackStation = MapPos;
                m_NAttackStation = MapPos;
            }
            MapPos =  m_NGrid;
            WalkDIR = m_ForceDir;
            Reject = true;
            m_DoUpatePath = false;
            return action;
        }
        if (m_bForce > 0) {
            m_bForce--;
        }
        
        m_PreGrid = MapPos;
        m_PreAttackStation = m_AttackStation;
        
        MapGrid attackstation = MapGrid.GetMG(m_AttackStation);
        PathData CurRoad = Path.GetPathData(PathAccess.Cur);
        if (m_Attack && RoleParent.m_Skill.CanAttack()  && CheckAttackPos()
            // && (!(RoleParent.m_Skill.m_AttackTarget is Role) || RoleParent.m_Skill.CheckCanAttack((RoleParent.m_Skill.m_AttackTarget as Role).CurrentAction))
            && (RoleParent.m_Skill.m_AttackTarget is IggWall  || RoleParent.m_Skill.m_AttackTarget is IggFloor || (NdUtil.IsSameMapPos(MapPos, m_AttackStation)
                    && attackstation.IsAttackStations())//&& attackstation.GetRoleStateInStation(m_SceneID) ==  RoleStationState.HoldStation)
                && !(CurRoad != null && (CurRoad.state == RoleState.FALL || CurRoad.state == RoleState.JUMP)))) {
            if (cdtime > 0) {
                if (cdtime > 0.5 && RoleParent.m_Hit) {
                    action = new GridActionCmdHit(0.5f, pos, RankDeep, WalkDIR);
                    action.SetTarget(RoleParent);
                    RoleParent.m_Hit = false;
                } else {
                    action = new GridActionCmdCDStand(cdtime, pos, RankDeep, WalkDIR);
                    action.SetTarget(RoleParent);
                    action.SetSpeed(RoleParent.m_Attr.SpeedPercent);
                }
            }
            Reject = true;
            m_DoUpatePath = false;
            if (action != null) {
                return action;
            }
        } else if (m_Attack) {
        }
        
        if (! m_Reject && RoleParent.m_Attr.CanMove && m_Pass) {
            PathData NextRoad = Path.GetPathData(PathAccess.Next);
            if (NextRoad != null && CurRoad != null) {
                if (MapGrid.GetMG(MapPos).PropStations == StationsProp.ATTACK) {
                    m_AttackStation = m_NAttackStation;
                    //获取下一个攻击位，不包含当前节点
                    ErrorInfo Error = new ErrorInfo();
                    PathData AttackStation = Path.GetNextAttackPosNoCurInPath(ref Error);
                    if (AttackStation != null) {
                        m_AttackStation = AttackStation.Road.GridPos;
                        m_NAttackStation = m_AttackStation;
                    }
                }
                
                RoleState roleState =  CurRoad.state;
                Vector3 vStart = pos;//CurRoad.Road.pos;
                Vector3 vEnd = Vector3.zero;
                CurrentGS = CurRoad.gs;
                vEnd =  NextRoad.Road.pos;
                if (roleState == RoleState.FALL) {
                    vStart = pos;
                    
                    if (Path.CheckRunFirstRoad() == true) {
                        action = new GridActionCmdFall(vStart, vStart, vEnd, CurRoad.deltaTime, CurRoad.dir, RankDeep);
                    } else {
                        Vector3 mid = Vector3.zero;
                        if (CurRoad.dir == WalkDir.WALKLEFT) {
                            if (CurRoad.Road.Left.Down != null) {
                                NextRoad.Road = CurRoad.Road.Left.Down;//MapGrid.GetMG()
                                mid = CurRoad.Road.Left.pos;
                            }
                        } else {
                            if (CurRoad.Road.Right.Down != null) {
                                NextRoad.Road = CurRoad.Road.Right.Down;
                                mid = CurRoad.Road.Right.pos;
                            }
                        }
                        while (NextRoad.Road.Down != null) {
                            NextRoad.Road = NextRoad.Road.Down;
                        }
                        vEnd =  NextRoad.Road.pos;
                        action = new GridActionCmdJumpHoleDown(vStart, mid, vEnd, CurRoad.deltaTime, CurRoad.dir, RankDeep);
                    }
                    
                    
                } else if (roleState == RoleState.JUMP) {
                    /*if ( NextRoad.Road.Type == GridType.GRID_WALL && RoleParent.m_Core.m_Camp == LifeMCamp.ATTACK)
                    {
                    	Life w = NextRoad.Road.GetWall();
                    	int width = Mathf.Abs(NextRoad.Road.GridPos.Unit - CurRoad.Road.GridPos.Unit);
                    	float t = CurRoad.deltaTime * (width - bodyradius) / (float)width;
                    	int flag = NextRoad.Road.GridPos.Unit > CurRoad.Road.GridPos.Unit ? -1 : 1;
                    	MapGrid m = MapGrid.GetMG(new Int2(NextRoad.Road.GridPos.Unit+ flag * bodyradius,NextRoad.Road.GridPos.Layer));
                    	action = new GridActionCmdJump(vStart,vEnd, CurRoad.deltaTime, CurRoad.dir, RankDeep,0.2f,0.2f,t,RoleParent.JumpAttack,w,m);
                    }
                    else*/
                    action = new GridActionCmdJump(vStart, vEnd, CurRoad.deltaTime, CurRoad.dir, RankDeep, 0.2f, 0.2f,
                        0f, null, null, null);
                        
                } else if (roleState == RoleState.STAIR) {
                    MapStair s1 = MapStair.GetStair(CurRoad.Road.GridPos, CurRoad.Road.GridPos.Layer > NextRoad.Road.GridPos.Layer);
                    Vector3 inpos = Vector3.zero;
                    Vector3 outpos = Vector3.zero;
                    bool bOccupy = false;
                    if (NextRoad.Road.Left != null && NextRoad.Road.Left.Type != GridType.GRID_HOLE) {
                        List<int> rolelist = new List<int>();
                        MapGrid g = NextRoad.Road.Left;
                        g.GetRoleList(ref rolelist);
                        if (rolelist.Count > 0) {
                            for (int i = 0; i < rolelist.Count; i++) {
                                Role l = CM.GetAllLifeM(rolelist[i], LifeMType.SOLDIER | LifeMType.SUMMONPET) as Role;
                                if (l.m_Core.m_Camp != m_parent.m_Core.m_Camp) {
                                    bOccupy = true;
                                    break;
                                }
                            }
                        }
                        if (!bOccupy) {
                            NextRoad.Road = NextRoad.Road.Left;
                        }
                    }
                    if (bOccupy && NextRoad.Road.Right != null && NextRoad.Road.Right.Type != GridType.GRID_HOLE) {
                        bOccupy = false;
                        List<int> rolelist = new List<int>();
                        MapGrid g = NextRoad.Road.Right;
                        g.GetRoleList(ref rolelist);
                        if (rolelist.Count > 0) {
                            for (int i = 0; i < rolelist.Count; i++) {
                                Role l = CM.GetAllLifeM(rolelist[i], LifeMType.SOLDIER | LifeMType.SUMMONPET) as Role;
                                if (l.m_Core.m_Camp != m_parent.m_Core.m_Camp) {
                                    bOccupy = true;
                                    break;
                                }
                            }
                        }
                        if (!bOccupy) {
                            NextRoad.Road = NextRoad.Road.Right;
                        }
                    }
                    if (bOccupy && NextRoad.Road.Left.Type != GridType.GRID_HOLE) {
                        NextRoad.Road = NextRoad.Road.Left;
                    }
                    
                    vEnd =  NextRoad.Road.pos;
                    
                    if (s1 != null) {
                        if (CurRoad.Road.GridPos.Layer > NextRoad.Road.GridPos.Layer) {
                            inpos = CurRoad.Road.pos;//MapGrid.GetMG(s1.GetStairUp()).pos;
                            outpos = NextRoad.Road.pos;//MapGrid.GetMG(s1.GetStairDown()).pos;
                            outpos.y -= 0.5f;
                        } else {
                            outpos = NextRoad.Road.pos;//MapGrid.GetMG(s1.GetStairUp()).pos;
                            inpos = CurRoad.Road.pos;//MapGrid.GetMG(s1.GetStairDown()).pos;
                            inpos.y -= 0.5f;
                        }
                        Building1201 b = s1.GetStairLife();
                        b.ShowUpEffect(CurRoad.deltaTime);
                        SetPet1002StairState(true);
                    } else {
                        Debug.Log("找不到楼梯");
                    }
                    WalkDIR = (CurRoad.Road.GridPos.Unit < NextRoad.Road.GridPos.Unit) ? WalkDir.WALKRIGHT : WalkDir.WALKLEFT;
                    action = new GridActionCmdStair(vStart, vEnd, 0.5f/*CurRoad.deltaTime*/, WalkDIR, RankDeep, 0.2f, 0.2f, inpos, outpos);
                } else if (roleState == RoleState.WALK) {
                
                    float time = 1f / RoleParent.m_Attr.Speed;
                    if (CurRoad.gs == GridSpace.Space_UP) {
                        vStart = CurRoad.Road.Uppos;
                        vEnd = NextRoad.Road.Uppos;
                        action = new GridActionCmdUnderWalk(vStart, vEnd, time, CurRoad.dir, RankDeep);
                    } else {
                        action = new GridActionCmdWalk(vStart, vEnd, time, CurRoad.dir, RankDeep);
                    }
                    SetPet1002WalkState();
                } else if (roleState == RoleState.FALLDOWN) {
                
                    action = new GridActionCmdFallDown(Time.deltaTime, RankDeep, vStart);
                    Reject = true;
                    m_DoUpatePath = false;
                    AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_SELFSP), RoleParent);
                    action.SetTarget(RoleParent);
                    return action;
                } else if (roleState == RoleState.JUMPDOWN) {
                    vStart = CurRoad.Road.Uppos;
                    action = new GridActionCmdJumpDown(vStart, vEnd, CurRoad.deltaTime, CurRoad.dir, RankDeep);
                } else if (roleState == RoleState.JUMPUP) {
                    vEnd = NextRoad.Road.Uppos;
                    action = new GridActionCmdJumpUp(vStart, vEnd, CurRoad.deltaTime, CurRoad.dir, RankDeep);
                } else if (roleState == RoleState.REVERSEJUMP) {
                    vStart = CurRoad.Road.Uppos;
                    vEnd = NextRoad.Road.Uppos;
                    action = new GridActionCmdReverseJump(vStart, vEnd, CurRoad.deltaTime, CurRoad.dir, RankDeep, 0.2f, 0.2f,
                        0f, null, null, null);
                        
                } else if (roleState == RoleState.HITFLY) {
                
                
                }
                
                if (action != null) {
                    MapPos =  NextRoad.Road.GridPos;
                    m_NGrid =  MapPos;
                    WalkDIR =  CurRoad.dir;
                    action.SetTarget(RoleParent);
                    action.SetSpeed(RoleParent.m_Attr.SpeedPercent);
                    
                }
                //运行到下一个路径节点
                Path.RunNextPath();
                m_IndexAdd = true;
            } else if (CurrentGS == GridSpace.Space_UP) {
                action = new GridActionCmdJumpDown(MapGrid.GetMG(MapPos).Uppos, MapGrid.GetMG(MapPos).pos, 1f, m_AttackDir, RankDeep);
                action.SetTarget(RoleParent);
                CurrentGS = GridSpace.Space_DOWN;
            } else {
                //if (!( NdUtil.IsSameMapPos(MapPos,m_AttackStation) && attackstation.GetRoleStateInStation(m_SceneID) ==  RoleStationState.HoldStation))
                AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_SELFSP), RoleParent);
            }
        }
        if (action == null) {
            action = new GridActionCmdStand();
            action.SetData(pos, pos, RoleState.STAND, Time.deltaTime, -1, WalkDIR, RankDeep);
            action.SetTarget(RoleParent);
        }
        Reject = true;
        m_DoUpatePath = false;
        return action;
    }
    
    void FilterPet1002State()//过滤无需处理的状态
    {
        if (RoleParent.m_Attr.AttrType != 100003) {
            return;
        }
        if (RoleParent == null || RoleParent.CurPet == null) {
            return;
        }
    }
    
    /// <summary>
    ///  处理蹦蹦进出楼梯状态
    /// </summary>
    public void SetPet1002StairState(bool isInStair)
    {
        FilterPet1002State();
        
        Pet pet = RoleParent.CurPet;
        if (pet != null && pet.PetMoveAI is PetWalk1002) {
        
            if (isInStair) {
                (pet.PetMoveAI as PetWalk1002).m_petState = Pet1002State.goStair;
            } else {
                (pet.PetMoveAI as PetWalk1002).m_petState = Pet1002State.Follow;
            }
        }
    }
    /// <summary>
    /// 设置蹦蹦移动状态
    /// </summary>
    public void SetPet1002WalkState()
    {
        FilterPet1002State();
        
        Pet pet = RoleParent.CurPet;
        if (pet != null && pet.PetMoveAI is PetWalk1002) {
            (pet.PetMoveAI as PetWalk1002).m_petState = Pet1002State.Follow;
        }
    }
    
    //格子注册，用来预测用
    public void RegisterNextAction(bool IsMove = true)
    {
        //如果需要强制跳跃，就不做预测
        if (m_bForce > -1 || !m_Reject) {
            return;
        }
        
        RoleState CurState = RoleState.WALK;
        PathData PrevRoad = Path.GetPathData(PathAccess.Prev);
        PathData CurRoad = Path.GetPathData(PathAccess.Cur);
        PathData NextRoad = Path.GetPathData(PathAccess.Next);
        if (PrevRoad != null) {
            CurState = PrevRoad.state;
        }
        if (CurRoad != null) {
            int sceneid = 0;
            if (CurRoad.Road.GetBuildRoom(ref sceneid)) {
                if (sceneid == CM.GoldBuild.SceneID) {
                    RoleParent.m_bReachGold = true;
                }
            }
        }
        m_DoUpatePath = true;
        if (CurState == RoleState.STAIR || CurState == RoleState.JUMP || CurState == RoleState.FALL) {
            if (CurState == RoleState.JUMP && CurRoad.state == RoleState.FALL) {
                Reject = false;
                return ;
            }
            
            AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_SELFSP), RoleParent);
        }
        
        //获取下一个Action
        if (CurRoad == null) {
            return ;
        }
        if (CurRoad.state != RoleState.FALLDOWN && IsMove && NextRoad != null) {
            m_NGrid =  NextRoad.Road.GridPos;
            WalkDIR = CurRoad.dir;
        }
#if UNITY_EDITOR_LOG
        FileLog.write(m_SceneID, "DORegisterNextAction" +  Path.Indext + "," +  m_ForceGrid + "," +  m_NGrid + "," + MapPos + "," + m_AttackStation + "," + m_NAttackStation
            + "," +   m_Attack + "," + RoleParent.m_Skill.CanAttack()  + "," + Time.realtimeSinceStartup);
#endif
        MapStations cur =  CurRoad.Road;
        MapStations next = CurRoad.Road;
        if (NextRoad != null) {
            next =  NextRoad.Road;
        }
        RoleState NextState = CurRoad.state;
        //
        if (MapPos != PreGrid) {
            RoleParent.m_ajustcount += RoleParent.m_ajustduration;
        }
        if (MapPos == m_AttackStation) {
        
            cur =  MapGrid.GetMG(m_AttackStation) as MapStations;
            DIR dir = WalkDIR == WalkDir.WALKLEFT ? DIR.LEFT : DIR.RIGHT;
            
            MapCheckStations.RoleStation(m_SceneID, cur, new StationsInfo(RoleParent.m_Attr, RankDeep, RoleParent.m_Core.m_Camp, dir, GridSpace.Space_DOWN));
            bool isEnd = true;
            ErrorInfo Error = new ErrorInfo();
            PathData AttackStation = Path.GetNextAttackPosNoCurInPath(ref Error);
            if (AttackStation != null) {
                isEnd = false;
            } else if (Error.Err == ErrReason.NoAttackPos) {
                isEnd = true;
            } else if (Error.Err == ErrReason.RePathPoint) {
                isEnd = false;
                if (Error.state == RoleState.STAIR) {
                    if (m_Pass) {
                        bool IsUP = (Error.dir == DIR.DOWN) ? true : false;
                        next = MapStair.GetStair(Error.GridPos, IsUP);
#if UNITY_EDITOR_LOG
                        if (next == null) {
                            Debug.LogError("楼梯为空" + Error.GridPos + "," + Error.dir);
                        }
                        FileLog.write(m_SceneID, "DORegisterNextActionSTAIRin" +  Path.Indext + "," +  MapPos + "," +  m_NGrid + "," + RankDeep + "," + next.GetStationsPos() + "," + cur.GetStationsPos() + "," + Time.realtimeSinceStartup);
#endif
                        MapCheckStations.RoleStation(m_SceneID, next,  new StationsInfo(RoleParent.m_Attr, RankDeep, RoleParent.m_Core.m_Camp, Error.dir, GridSpace.Space_DOWN));
                    }
                }
            }
            if (isEnd) {
                ClearPath();
            }
        } else {
            DIR dir = WalkDIR == WalkDir.WALKLEFT ? DIR.LEFT : DIR.RIGHT;
            MapCheckStations.RoleStation(m_SceneID, cur, new StationsInfo(RoleParent.m_Attr, RankDeep, RoleParent.m_Core.m_Camp, dir, GridSpace.Space_DOWN));
        }
        Reject = false;
    }
    
    public void Teleport(MapGrid g)
    {
        MapCheckStations.RoleStation(m_SceneID,
            g,
            new StationsInfo(RoleParent.m_Attr as RoleAttribute, RankDeep, RoleParent.m_Core.m_Camp, DIR.LEFT, GridSpace.Space_DOWN));
        MapPos = g.GridPos;
        m_NGrid = g.GridPos;
        m_AttackStation = g.GridPos;
        m_NAttackStation = g.GridPos;
    }
    
    public bool CheckAttackCondition()
    {
        PathData PrevRoad = Path.GetPathData(PathAccess.Prev, true);
        MapGrid attackstation = MapGrid.GetMG(m_AttackStation);
        if (m_Attack && RoleParent.m_Skill.CanAttack()
            && (!(RoleParent.m_Skill.m_AttackTarget is Role) || RoleSkill.CheckCanAttack((RoleParent.m_Skill.m_AttackTarget as Role).CurrentAction))
            && (RoleParent.m_Skill.m_AttackTarget is IggWall  || RoleParent.m_Skill.m_AttackTarget is IggFloor || (NdUtil.IsSameMapPos(m_PreGrid, m_AttackStation)
                    && attackstation.IsAttackStations()/* && attackstation.GetRoleStateInStation(m_SceneID) ==  RoleStationState.HoldStation*/)
                && !(PrevRoad != null && (PrevRoad.state == RoleState.FALL || PrevRoad.state == RoleState.JUMP)))) {
            return true;
        }
        return false;
    }
    
    public GridActionCmd GetAttackAction()
    {
        RoleSkill roleskill = RoleParent.m_Skill as RoleSkill;
        GridActionCmd action = null;
        
#if UNITY_EDITOR_LOG
        FileLog.write(m_SceneID, "GetAttackAction:  pre  "  + m_PreAttackStation + "," + m_AttackStation  + RoleParent.m_thisT.localPosition);
#endif
        if (!NdUtil.IsSameMapPos(m_PreAttackStation, m_NAttackStation)) {
            m_AttackStation = m_PreAttackStation;
            m_NAttackStation = m_AttackStation;
            
            MapCheckStations.RoleStation(m_SceneID,
                MapGrid.GetMG(m_AttackStation),
                new StationsInfo(RoleParent.m_Attr, RankDeep, RoleParent.m_Core.m_Camp, DIR.LEFT, GridSpace.Space_DOWN)
            );
#if UNITY_EDITOR_LOG
            FileLog.write(m_SceneID, "GetAttackAction:" + m_AttackStation);
#endif
        }
        
        if (CurrentGS == GridSpace.Space_UP) {
            action = new GridActionCmdJumpDown(MapGrid.GetMG(MapPos).Uppos, MapGrid.GetMG(MapPos).pos, 1f, m_AttackDir, RankDeep);
            action.SetTarget(RoleParent);
            CurrentGS = GridSpace.Space_DOWN;
        } else {
            if (RoleParent.m_Skill.m_AttackTarget == null) {
                Debug.Log(RoleParent.PropAttackSkillInfo.m_type);
            }
            //if (RoleParent.m_Skill.CheckDoAttackTarget(RoleParent.m_Skill.m_AttackTarget.SceneID))
            {
            
                action = RoleSkill.GetAttackSkillAction(RoleParent.PropAttackSkillInfo.m_type,
                        m_NextAttackSceneID, m_AttackDir, RankDeep, RoleParent.m_Attr.ModelType, m_SceneID);
                RoleParent.m_Skill.ReSetCDTime();
                if (action == null) {
                    Debug.Log(RoleParent.PropAttackSkillInfo.m_type + "," + m_SceneID);
                } else {
                    (action as GridActionCmdAttack).StartWithTarget(RoleParent, RoleParent.StartAttack);
                }
                WalkDIR =  m_AttackDir;
            }
            if (action != null) {
                if (RoleParent.m_Skill.m_AttackTarget == RoleParent.Target) {
                    RoleParent.m_TargetDuration = RoleParent.m_changetargettime;
                }
                action.SetSpeed(RoleParent.m_Attr.AttackSpeed);
            } else {
                //bool check = RoleParent.m_Skill.CheckDoAttackTarget(RoleParent.m_Skill.m_AttackTarget.SceneID);
            }
        }
        
        m_Attack = false;
        Reject = true;
        m_DoUpatePath = false;
        return action ;
    }
    
    //获取当前层的路径，小黄鸭用
    public List<PathData>  GetCurrentLayerPathList()
    {
        return Path.GetCurrentLayerPathList(MapPos);
    }
    
    /// <summary>
    /// 检测同层路线行走方向上，确定范围内有物可触发陷阱
    /// </summary>
    /// <param name="Area">路线范围</param>
    /// <returns>false，没有，true 有</returns>
    public  bool CheckTrapInRoadSameLayer(int Area)
    {
        List<PathData> l = GetCurrentLayerPathList();
        for (int i = 0 ; i < l.Count ; i ++) {
            if (i > Area) {
                break;
            }
            if (l[i].HaveObstacles() == true) {
                return false;
            }
            if (l[i].CheckTrap() == true) {
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// 场景中绘制路线节点
    /// </summary>
    public void OnDrawGizmos()
    {
        Path.OnDrawGizmos();
    }
    
    /// <summary>
    /// 确认剩余的路线上，有空闲的攻击位
    /// </summary>
    /// <returns>Error 错误哦原因</returns>
    public bool CheckHaveIdleAttackPosInPath(int SceneID)
    {
        return Path.CheckHaveIdleAttackPosInPath(SceneID);
    }
    
    
    /// <summary>
    /// 确认剩余的路线上，有空闲的攻击位
    /// </summary>
    /// <returns>Error 错误哦原因</returns>
    public bool CheckInRoad(Int2 pos)
    {
        return Path.InRoad(pos);
    }
}
