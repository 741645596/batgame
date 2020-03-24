using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class   PetGridRun : GridRun{

	public PetGridRun(Life role)
		:base(role)
	{
		
	}

	public Pet PetParent
	{
		get { return m_parent as Pet; }
	}
    public void SetBorn(Int2 BornPos, int deep)
    {
        MapPos = BornPos;
        m_AttackStation = MapPos;
        m_NAttackStation = MapPos;
        m_NGrid = BornPos;
        RankDeep = deep;

    }
    public void PathAI(MapGrid start, MapGrid end, NDAttribute Attr)
    {
        Path.PathAI(m_SceneID, start, end, m_parent.m_Attr , CurrentGS, WalkDIR);

        //CheackIdlePath();
        //判断首个节点是否为楼梯
        PathData Road = Path.GetPathData(PathAccess.Cur);
        if (Road != null && Road.state == RoleState.STAIR)
        {
            m_Reject = true;
        }

        //获取路线上的下一个攻击位，包含当前路径点
        ErrorInfo Error = new ErrorInfo();
        PathData NextAttackStation = Path.GetNextAttackPosCurInPath(ref Error);
        if (NextAttackStation != null)
        {
            m_AttackStation = NextAttackStation.Road.GridPos;
            m_NAttackStation = NextAttackStation.Road.GridPos;
        }

        //FirstCheckAttack();
    }

    public void ClearPath()
    {
        Path.ClearPath();
    }

    public void UpdataPath()
    {
    /*    if (!m_UpdataPathFlag)
            return;*/


       /* if (m_AIEventData != null)
            FileLog.write(m_SceneID, "UpdataPath1" + m_AIEventData.Event);*/
        m_UpdataPathFlag = false;

        if (Path.CheckHavePath() == false)
        {
            PathAI(MapGrid.GetMG(MapPos),PetParent.m_target.GetMapGrid(),PetParent.m_Attr );
			Path.ClearPath();
			Path.Path = Path.SearchPath;
        }
        else
        {
                //FileLog.write(m_SceneID, "DOUpdataPath2" + m_AIEventData.Event);
                m_AIEventData = null;
                ClearPath();
                PathAI(MapGrid.GetMG(MapPos), PetParent.m_target.GetMapGrid(), PetParent.m_Attr);
        }
    }

    //取下一个动作
    public GridActionCmd GetNextAction()
    {
		Vector3 pos = PetParent.m_thisT.localPosition;
        GridActionCmd action = null;
        if (Path.CheckRunFinishRoad())
            return null;

        if (PetParent == null || PetParent.m_Attr == null)
        {
            action = new GridActionCmdStand();
            action.SetData(pos, pos, RoleState.STAND, Time.deltaTime, -1, WalkDIR, RankDeep);
            action.SetTarget(PetParent);
            return action;
        }

        m_PreGrid = MapPos;
        m_PreAttackStation = m_AttackStation;

        MapGrid attackstation = MapGrid.GetMG(m_AttackStation);
        PathData CurRoad = Path.GetPathData(PathAccess.Cur);


        if (PetParent.m_Attr.CanMove)
        {
            PathData NextRoad = Path.GetPathData(PathAccess.Next);
            if (NextRoad != null && CurRoad != null)
            {
                if (MapGrid.GetMG(MapPos).PropStations == StationsProp.ATTACK)
                {
                    m_AttackStation = m_NAttackStation;
                    //获取下一个攻击位，不包含当前节点
                    ErrorInfo Error = new ErrorInfo();
                    PathData AttackStation = Path.GetNextAttackPosNoCurInPath(ref Error);
                    if (AttackStation != null)
                    {
                        m_AttackStation = AttackStation.Road.GridPos;
                    }
                }

                RoleState roleState = CurRoad.state;
                Vector3 vStart = CurRoad.Road.pos;
                Vector3 vEnd = Vector3.zero;
                CurrentGS = CurRoad.gs;
                vEnd = NextRoad.Road.pos;
                if (roleState == RoleState.FALL)
                {
					vStart = pos;
					if (Path.CheckRunFirstRoad () == true)
						action = new GridActionCmdFall(vStart,vStart,vEnd, CurRoad.deltaTime, CurRoad.dir, RankDeep);
					else
					{
						Vector3 mid = Vector3.zero;
						if (CurRoad.dir == WalkDir.WALKLEFT)
						{
							if (CurRoad.Road.Left.Down != null)
							{
								NextRoad.Road = CurRoad.Road.Left.Down;//MapGrid.GetMG()
								mid = CurRoad.Road.Left.pos;
							}
						}
						else
						{
							if (CurRoad.Road.Right.Down != null)
							{
								NextRoad.Road = CurRoad.Road.Right.Down;
								mid = CurRoad.Road.Right.pos;
							}
						}
						
						while (NextRoad.Road.Down != null)
							NextRoad.Road = NextRoad.Road.Down;
						vEnd =  NextRoad.Road.pos;	
						action = new GridActionCmdJumpHoleDown(vStart,mid,vEnd, CurRoad.deltaTime, CurRoad.dir, RankDeep);
					}

                }
                else if (roleState == RoleState.JUMP)
                {
                        action = new GridActionCmdJump(vStart, vEnd, CurRoad.deltaTime, CurRoad.dir, RankDeep, 0.2f, 0.2f,
                                                       0f, null, null, null);

                }
                else if (roleState == RoleState.STAIR)
                {
					MapStair s1 = MapStair.GetStair(CurRoad.Road.GridPos,CurRoad.Road.GridPos.Layer > NextRoad.Road.GridPos.Layer);
					Vector3 inpos = Vector3.zero;
					Vector3 outpos = Vector3.zero;
                    if (s1 != null)
                    {
						if(CurRoad.Road.GridPos.Layer > NextRoad.Road.GridPos.Layer)
						{
							inpos = MapGrid.GetMG(s1.GetStairUp()).pos;
							outpos = MapGrid.GetMG(s1.GetStairDown()).pos;
							outpos.y -=0.5f;
						}
						else
						{
							outpos = MapGrid.GetMG(s1.GetStairUp()).pos;
							inpos = MapGrid.GetMG(s1.GetStairDown()).pos;
							inpos.y-=0.5f;
						}
                    }
                    else Debug.Log("找不到楼梯");
					WalkDIR = (CurRoad.Road.GridPos.Unit < NextRoad.Road.GridPos.Unit)? WalkDir.WALKRIGHT:WalkDir.WALKLEFT;
					action = new GridActionCmdStair(vStart, vEnd, CurRoad.deltaTime,WalkDIR , RankDeep, 0.2f, 0.2f,inpos,outpos);
                }
                else if (roleState == RoleState.WALK)
                {
                    float time = 1f / (PetParent.m_Attr.Speed);
                    if (CurRoad.gs == GridSpace.Space_UP)
                    {
                        vStart = CurRoad.Road.Uppos;
                        vEnd = NextRoad.Road.Uppos;
                        action = new GridActionCmdUnderWalk(vStart, vEnd, time, CurRoad.dir, RankDeep);
                    }
                    else
                        action = new GridActionCmdWalk(vStart, vEnd, time, CurRoad.dir, RankDeep);
                }
                else if (roleState == RoleState.FALLDOWN)
                {

                    action = new GridActionCmdFallDown(Time.deltaTime, RankDeep, vStart);
                    m_Reject = true;
                    AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_SELFSP), PetParent);
                    action.SetTarget(PetParent);
                    return action;
                }
                else if (roleState == RoleState.JUMPDOWN)
                {
                    vStart = CurRoad.Road.Uppos;
                    action = new GridActionCmdJumpDown(vStart, vEnd, CurRoad.deltaTime, CurRoad.dir, RankDeep);
                }
                else if (roleState == RoleState.JUMPUP)
                {
                    vEnd = NextRoad.Road.Uppos;
                    action = new GridActionCmdJumpUp(vStart, vEnd, CurRoad.deltaTime, CurRoad.dir, RankDeep);
                }
                else if (roleState == RoleState.REVERSEJUMP)
                {
                    vStart = CurRoad.Road.Uppos;
                    vEnd = NextRoad.Road.Uppos;
                    action = new GridActionCmdReverseJump(vStart, vEnd, CurRoad.deltaTime, CurRoad.dir, RankDeep, 0.2f, 0.2f,
                                                       0f, null, null, null);

                }
                else if (roleState == RoleState.HITFLY)
                {


                }

                if (action != null)
                {
                    MapPos = NextRoad.Road.GridPos;
                    m_NGrid = MapPos;
                    WalkDIR = CurRoad.dir;
                    action.SetTarget(PetParent);
                    action.SetSpeed(PetParent.m_Attr.SpeedPercent);

                }
                //运行到下一个路径节点
                Path.RunNextPath();
            }
            else if (CurrentGS == GridSpace.Space_UP)
            {
                action = new GridActionCmdJumpDown(MapGrid.GetMG(MapPos).Uppos, MapGrid.GetMG(MapPos).pos, 1f, m_AttackDir, RankDeep);
                action.SetTarget(PetParent);
                CurrentGS = GridSpace.Space_DOWN;
            }
        }
        if (action == null)
        {
            action = new GridActionCmdStand();
            action.SetData(pos, pos, RoleState.STAND, Time.deltaTime, -1, WalkDIR, RankDeep);
            action.SetTarget(PetParent);
        }
        m_Reject = true;

        return action;
    }

   
}
