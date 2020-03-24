using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PutERR {
    ERR_NORMAL    = 0,
    ERR_NOLink    = 1,
    ERR_HaveChild = 2,
}

/// <summary>
/// 拼船规则
/// </summary>
/// <author>zhulin</author>
public class PutCanvasM
{


    private static bool s_bCanOperate;
    public static List<CanvasCore> s_listNoLinkPutRoomcores = null;
    public static List<CanvasCore> s_listNoLinkPutDeckRoomcores = null;
    public static List<CanvasCore> s_listNoLinkSolders = null;
    public static bool CanOperate {
        get{
            return s_bCanOperate;
        }
        set{
            s_bCanOperate = value;
            if (s_bCanOperate == false)
            {
                PutCanvasM.ClearNoLinkList();
            }
        }
    }
    private static bool s_bSaveSuc = true;
    public static bool SaveSuc {
        get
        {
            return s_bSaveSuc;
        }
        set
        {
            s_bSaveSuc = value;
        }
    }
    /// <summary>
    /// 检测该位置是否适合放置,新建摆放对象和显示可放不可放状态时使用
    /// </summary>
    public static bool CheckCanPut(CanvasCore Core,  Int2 posMapGrid)
    {
        if (Core == null) {
            return false;
        }
        ShipPlan P = ShipPlanDC.GetCurShipPlan();
        if (P == null) {
            return false;
        }
        ShipPutInfo Info = P.GetShipBuildInfo(Core);
        if (Info == null) {
            return false;
        }
        
        RoomGrid grid = RoomMap.FindRoomGrid(posMapGrid, XYSYS.MapGrid);
        if (grid != null) {
            bool result = BattleEnvironmentM.BuildInShape(grid.mPosRoomGrid);
            if (!result) {
                return false;
            }
        }
        
        
        if (Core.m_type == ShipBuildType.BuildRoom) {
            List<Int2> AddList = Core.GetMovetoRoomGridPos(posMapGrid);
            if (Core.IsDeckRoom == false) {
                List<Int2> oldList = Core.GetPutRoomGridPos();
                if (Core.IsNewCreate) {
                    oldList.Clear();//新建连接房间没位置
                }
                //连接房判定是否可放
                bool bLinkOK = RoomMap.CheckLinkToGoldRoom(oldList, AddList, Core.IsNewCreate);
                if (bLinkOK) {
                    List<RoomGrid> listRoomGrid = RoomMap.GetPutRoomGrid(Core);
                    List<RoomGrid> listMovetoRoomGrid = RoomMap.GetMovetoRoomGrid(Core, posMapGrid);
                    if (listRoomGrid.Count != listMovetoRoomGrid.Count) {
                        return false;
                    }
                    int nMoveToCount = listMovetoRoomGrid.Count;
                    int nMoveCnt = 0;
                    for (nMoveCnt = 0; nMoveCnt < nMoveToCount; nMoveCnt++) {
                        RoomGrid roomGrid = listMovetoRoomGrid[nMoveCnt];
                        CanvasCore buildCore = roomGrid.GetBuildRoom();
                        //所放的地方有其它的连接房或甲板房则不能临时放
                        if (buildCore.m_ID != RoomGrid.EMPTYGRIDID && buildCore.m_ID != Core.m_ID && buildCore.m_type == ShipBuildType.BuildRoom) {
                            return false;
                        }
                        
                    }
                    return bLinkOK;
                }
            } else {
                //甲板房放置判定
                return RoomMap.CheckCanPutDeckBuild(Core.m_ID, AddList);
            }
        } else if (Core.m_type == ShipBuildType.BuildStair) {
            //楼梯需保证在活动范围内移动。
            Int2 Area = RoomMap.GetStairActiveArea(Info.cyMapGrid, Info.cxMapGrid);
            if (posMapGrid.Unit < Area.Layer || posMapGrid.Unit > Area.Unit) {
                return false;
            }
            if (posMapGrid.Layer != Info.cyMapGrid) {
                return false;
            }
            RoomMap.RemoveMapPosition(Core, RoomMap.GetPutRoomGridPosition(Core));
            RoomMap.AddMapPosition(Core, RoomMap.GetPutRoomGridPosition(Core));
            return true;
        } else if (Core.m_type == ShipBuildType.Soldier) {
            //原占有先移除
            if (!Core.IsNewCreate) {
                RoomMap.RemoveMapPosition(Core, RoomMap.GetPutRoomGridPosition(Core));
            }
            //new
            bool ret = RoomMap.CheckMapPosition(RoomMap.GetMovetoRoomGridPosition(Core, posMapGrid));
            //原占有复原
            if (!Core.IsNewCreate) {
                RoomMap.AddMapPosition(Core, RoomMap.GetPutRoomGridPosition(Core));
            }
            return ret;
        }
        return false;
        
    }
    /// <summary>
    /// 检测是否可临时放置（只要空白空间够就可以）
    /// </summary>
    public static bool CheckCanTempPut(CanvasCore Core,  Int2 posMapGrid)
    {
        if (Core == null) {
            return false;
        }
        ShipPlan P = ShipPlanDC.GetCurShipPlan();
        if (P == null) {
            return false;
        }
        ShipPutInfo Info = P.GetShipBuildInfo(Core);
        if (Info == null) {
            return false;
        }
        
        //是否在设计图方案区域内.
        RoomGrid grid = RoomMap.FindRoomGrid(posMapGrid, XYSYS.MapGrid);
        if (grid != null) {
            bool result = BattleEnvironmentM.BuildInShape(grid.mPosRoomGrid);
            if (!result) {
                return false;
            }
        }
        
        
        if (Core.m_type == ShipBuildType.BuildRoom) {
            List<RoomGrid> listRoomGrid = RoomMap.GetPutRoomGrid(Core);
            List<RoomGrid> listMovetoRoomGrid = RoomMap.GetMovetoRoomGrid(Core, posMapGrid);
            if (listRoomGrid.Count != listMovetoRoomGrid.Count) {
                return false;
            }
            int nMoveToCount = listMovetoRoomGrid.Count;
            int nMoveCnt = 0;
            for (nMoveCnt = 0; nMoveCnt < nMoveToCount; nMoveCnt++) {
                RoomGrid roomGrid = listMovetoRoomGrid[nMoveCnt];
                CanvasCore buildCore = roomGrid.GetBuildRoom();
                //所放的地方有其它的连接房或甲板房则不能临时放
                if (buildCore.m_ID != RoomGrid.EMPTYGRIDID && buildCore.m_ID != Core.m_ID && buildCore.m_type == ShipBuildType.BuildRoom) {
                    return false;
                }
                //普通房间（非甲板房）不能放在有炮弹兵甲板上
                if (buildCore.m_ID == RoomGrid.EMPTYGRIDID) {
                    List<CanvasCore> childrenCore = roomGrid.GetOtherBuild();
                    foreach (CanvasCore childCore in childrenCore) {
                        if (childCore.m_type == ShipBuildType.Soldier) {
                            return false;
                        }
                    }
                }
            }
            return true;
        } else if (Core.m_type == ShipBuildType.BuildStair) {
            //楼梯需保证在活动范围内移动。
            Int2 Area = RoomMap.GetStairActiveArea(Info.cyMapGrid, Info.cxMapGrid);
            if (posMapGrid.Unit < Area.Layer || posMapGrid.Unit > Area.Unit) {
                return false;
            }
            if (posMapGrid.Layer != Info.cyMapGrid) {
                return false;
            }
            RoomMap.RemoveMapPosition(Core, RoomMap.GetPutRoomGridPosition(Core));
            RoomMap.AddMapPosition(Core, RoomMap.GetPutRoomGridPosition(Core));
            return true;
        } else if (Core.m_type == ShipBuildType.Soldier) {
            //原占有先移除
            if (!Core.IsNewCreate) {
                RoomMap.RemoveMapPosition(Core, RoomMap.GetPutRoomGridPosition(Core));
            }
            //new
            bool ret = RoomMap.CheckMapPosition(RoomMap.GetMovetoRoomGridPosition(Core, posMapGrid));
            //原占有复原
            if (!Core.IsNewCreate) {
                RoomMap.AddMapPosition(Core, RoomMap.GetPutRoomGridPosition(Core));
            }
            if (!ret) {
                List<RoomGrid> lRoomGrid = RoomMap.GetMovetoRoomGrid(Core, posMapGrid);
                if (null != lRoomGrid && null != lRoomGrid[0]) {
                    //所在位置是连接房则上面已经判定不能放就是不能临时放
                    if (lRoomGrid[0].GetBuildRoom().m_ID == RoomGrid.EMPTYGRIDID) {
                        ret = true;
                    }
                }
            }
            return ret;
        }
        return false;
        
    }
    /// <summary>
    /// 确认能否移除
    /// </summary>
    /// <returns>true 能被删除，false 不能被删除</returns>
    public static bool CheckCanRemove(CanvasCore Core, ref PutERR err)
    {
        if (Core == null) {
            return  false;
        }
        ShipPlan P = ShipPlanDC.GetCurShipPlan();
        if (P == null) {
            return false;
        }
        ShipPutInfo Info = P.GetShipBuildInfo(Core);
        if (Info == null) {
            return false;
        }
        
        if (Core.m_type == ShipBuildType.BuildRoom && !Core.IsNewCreate) {
            if (Core.IsDeckRoom == false) {
                /*
                List<Int2> SubList =  Core.GetPutRoomGridXY();
                List<Int2> AddList = new List<Int2>();
                if(RoomMap.CheckMapLinkOk(SubList ,AddList ,Core.m_style, true) == false)
                {
                	err = PutERR.ERR_NOLink;
                	return false;
                }*/
                return true;
            } else {
                return true;
            }
        }
        return true;
    }
    /// <summary>
    /// 确认房间是否可置换
    /// </summary>
    public static bool CheckExchange(CanvasCore coreSrc, CanvasCore coreExchange)
    {
        if (coreSrc ==  null || coreExchange == null) {
            return false;
        }
        if (coreSrc.m_ID == coreExchange.m_ID) {
            return false;
        }
        if (coreSrc.m_type == ShipBuildType.BuildStair) {
            return false;
        }
        if (coreSrc.m_type != coreExchange.m_type) {
            return false;
        }
        if (coreSrc.Data == null || coreExchange.Data == null) {
            return false;
        }
        if (coreSrc.m_type == ShipBuildType.Soldier) {
            return true;
        }
        if (coreSrc.Data.m_DeckRoom != coreExchange.Data.m_DeckRoom) {
            return false;
        }
        TouchMove tmSrc = PutCanvasM.GetTouchMoveByCore(coreSrc);
        TouchMove tmExchange = PutCanvasM.GetTouchMoveByCore(coreExchange);
        if (tmSrc == null || tmExchange == null) {
            return false;
        }
        if (tmSrc.m_orgPosMapGrid.Layer + tmExchange.MyCore().GetSize().Layer - 1 >= RoomMap.DeckRoomTopLayer) {
            return false;
        }
        
        List<RoomGrid> listRoomGridSrc = RoomMap.GetPutRoomGrid(coreSrc);
        List<RoomGrid> listMovetoRoomGridSrc = RoomMap.GetMovetoRoomGrid(coreSrc, tmSrc.m_posMapGrid);
        if (listRoomGridSrc.Count != listMovetoRoomGridSrc.Count) {
            return false;
        }
        List<RoomGrid> listRoomGridExchange = RoomMap.GetPutRoomGrid(coreExchange);
        List<RoomGrid> listMoveRoomGridExchange = RoomMap.GetMovetoRoomGrid(coreExchange, tmSrc.m_orgPosMapGrid);
        if (listRoomGridExchange.Count != listMoveRoomGridExchange.Count) {
            return false;
        }
        
        int nMoveToCount = listMovetoRoomGridSrc.Count;
        int nMoveCnt = 0;
        for (nMoveCnt = 0; nMoveCnt < nMoveToCount; nMoveCnt++) {
            RoomGrid roomGrid = listMovetoRoomGridSrc[nMoveCnt];
            if (listMoveRoomGridExchange.Contains(roomGrid)) {
                return false;
            }
            CanvasCore buildCore = roomGrid.GetBuildRoom();
            if (buildCore.m_ID != RoomGrid.EMPTYGRIDID && buildCore.m_ID != coreSrc.m_ID && buildCore.m_ID != coreExchange.m_ID) {
                return false;
            }
            if (buildCore.m_ID == RoomGrid.EMPTYGRIDID) {
                List<CanvasCore> childrenCore = roomGrid.GetOtherBuild();
                foreach (CanvasCore childCore in childrenCore) {
                    if (childCore.m_type == ShipBuildType.Soldier) {
                        return false;
                    }
                }
            }
            
        }
        nMoveToCount = listMoveRoomGridExchange.Count;
        for (nMoveCnt = 0; nMoveCnt < nMoveToCount; nMoveCnt++) {
            RoomGrid roomGrid = listMoveRoomGridExchange[nMoveCnt];
            if (listMovetoRoomGridSrc.Contains(roomGrid)) {
                return false;
            }
            CanvasCore buildCore = roomGrid.GetBuildRoom();
            if (buildCore.m_ID != RoomGrid.EMPTYGRIDID && buildCore.m_ID != coreExchange.m_ID && buildCore.m_ID != coreSrc.m_ID) {
                return false;
            }
        }
        
        return 	true;//ShipPutInfo.CheckExchangeRoom(core1.Data ,core2.Data);
    }
    
    
    /// <summary>
    /// 加入一个房间。从仓库中加载。或加载方案
    /// </summary>
    /// <param name="Core">核心结构</param>
    /// <param name="TargetPos">格子坐标</param>
    public static void AddNewCore(CanvasCore Core, Int2 TargetPos)
    {
        if (Core == null) {
            return  ;
        }
        ShipPlan P = ShipPlanDC.GetCurShipPlan();
        if (P == null) {
            return ;
        }
        ShipPutInfo Info = P.GetShipBuildInfo(Core);
        if (Info == null) {
            return;
        }
        
        if (Core.IsNewCreate) {
            Info.SetBuildPostion(TargetPos);
            P.AddShipBuildInfo(Info, Core.m_type);
            TouchMoveManager.MoveToShip(Core);
            Core.IsNewCreate = false;
        }
        //
        if (Core.m_type == ShipBuildType.BuildRoom) {
            if (Core.IsDeckRoom == false) {
                ShapeType Shape = Info.GetPutRoomShape();
                if (Shape == null) {
                    return;
                }
                RoomMap.OpenCanvans(Shape.GetShapeData(new Int2(Info.cxMapGrid, Info.cyMapGrid)), Core.m_ID);
                //设置甲板区域
                RoomMap.UpdateDeckRoomGrid();
                //更新金库房间
                if (Info.IsGoldBuild() == true) {
                    RoomMap.UpdateGoldRoomGrid(Core.GetPutRoomGridPos());
                }
            } else {
                RoomMap.AddDeckBuild(Core.m_ID, Core.GetPutRoomGridPos());
            }
        } else {
            RoomMap.AddMapPosition(Core, RoomMap.GetPutRoomGridPosition(Core));
        }
    }
    
    
    /// <summary>
    /// 移除一个房间
    /// </summary>
    public static void RemoveBuildRoom(CanvasCore Core)
    {
        if (Core == null) {
            return ;
        }
        ShipPlan P = ShipPlanDC.GetCurShipPlan();
        if (P == null) {
            return ;
        }
        ShipPutInfo Info = P.GetShipBuildInfo(Core);
        if (Info == null) {
            return;
        }
        
        if (Core.m_type == ShipBuildType.BuildRoom) {
            if (Core.IsDeckRoom == false) {
                if (Info.GetPutRoomShape() != null) {
                    List<CanvasCore> l = PutCanvasM.GetChildByCore(Core);
                    foreach (CanvasCore c in l) {
                        ShipPutInfo II = P.GetShipBuildInfo(c);
                        if (II != null) {
                            P.RemoveShipBuildInfo(II, c.m_type);
                        }
                    }
                    RoomMap.CloseCanvans(Core.GetPutRoomGridPos());
                    //设置甲板区域
                    RoomMap.UpdateDeckRoomGrid();
                }
            } else {
                List<Int2> l = Core.GetPutRoomGridPos();
                RoomMap.RemoveDeckBuild(l);
            }
            
        } else {
            RoomMap.RemoveMapPosition(Core, RoomMap.GetPutRoomGridPosition(Core));
        }
        P.RemoveShipBuildInfo(Info, Core.m_type);
    }
    /// <summary>
    /// 移动一个房间
    /// </summary>
    public static bool MoveBuildRoom(CanvasCore Core, Int2 TargetPos)
    {
        if (Core == null || Core.IsNewCreate) {
            return false;
        }
        ShipPlan P = ShipPlanDC.GetCurShipPlan();
        if (P == null) {
            return false;
        }
        ShipPutInfo Info = P.GetShipBuildInfo(Core);
        if (Info == null) {
            return false;
        }
        if (TargetPos.Unit == Info.cxMapGrid && TargetPos.Layer == Info.cyMapGrid) {
            return false;
        }
        
        if (Core.m_type == ShipBuildType.BuildRoom) {
            if (Core.IsDeckRoom == false) {
                //先移动子对象数据。
                List<CanvasCore> l = PutCanvasM.GetChildByCore(Core);
                foreach (CanvasCore c in l) {
                    Int2 move = new  Int2(TargetPos.Unit - Info.cxMapGrid, TargetPos.Layer - Info.cyMapGrid);
                    ShipPutInfo II = P.GetShipBuildInfo(c);
                    if (II != null) {
                        II.cxMapGrid += move.Unit;
                        II.cyMapGrid += move.Layer;
                    }
                }
                RoomMap.MoveCanvans(RoomMap.GetPutRoomGrid(Core), RoomMap.GetMovetoRoomGrid(Core, TargetPos));
                //设置甲板区域
                RoomMap.UpdateDeckRoomGrid();
                //更新金库房间
                if (Info.IsGoldBuild() == true) {
                    RoomMap.UpdateGoldRoomGrid(Core.GetMovetoRoomGridPos(TargetPos));
                }
                
            } else {
                List<Int2> lDeck = Core.GetPutRoomGridPos();
                RoomMap.RemoveDeckBuild(lDeck);
                lDeck = Core.GetMovetoRoomGridPos(TargetPos);
                RoomMap.AddDeckBuild(Core.m_ID, lDeck);
            }
        } else {
            RoomMap.RemoveMapPosition(Core, RoomMap.GetPutRoomGridPosition(Core));
            RoomMap.AddMapPosition(Core, RoomMap.GetMovetoRoomGridPosition(Core, TargetPos));
        }
        Info.SetBuildPostion(TargetPos);
        return true;
    }
    
    
    /// <summary>
    /// 调换位置
    /// </summary>
    public static bool ExchangeRoom(CanvasCore Core1, Int2 tomapGrid1, CanvasCore Core2, Int2 tomapGrid2)
    {
        if (Core1 == null || Core1.IsNewCreate) {
            return false;
        }
        if (Core2 == null || Core2.IsNewCreate) {
            return false;
        }
        if (Core1.m_type != ShipBuildType.BuildRoom) {
            return false;
        }
        if (Core2.m_type != ShipBuildType.BuildRoom) {
            return false;
        }
        if (Core1.IsDeckRoom != Core2.IsDeckRoom) {
            return false;
        }
        ShipPlan P = ShipPlanDC.GetCurShipPlan();
        if (P == null) {
            return false;
        }
        //
        ShipPutInfo Info1 = P.GetShipBuildInfo(Core1);
        if (Info1 == null) {
            return false;
        }
        ShipPutInfo Info2 = P.GetShipBuildInfo(Core2);
        if (Info1 == null) {
            return false;
        }
        
        Int2 Pos1 = new Int2(Info1.cxMapGrid, Info1.cyMapGrid);
        Int2 Pos2 = new Int2(Info2.cxMapGrid, Info2.cyMapGrid);
        //先子对象互换
        
        //先移动子对象数据。
        //core1 --> core2
        List<CanvasCore> l = PutCanvasM.GetChildByCore(Core1);
        foreach (CanvasCore c in l) {
            Int2 move = new  Int2(tomapGrid1.Unit - Info1.cxMapGrid, tomapGrid1.Layer - Info1.cyMapGrid);
            ShipPutInfo II = P.GetShipBuildInfo(c);
            if (II != null) {
                II.cxMapGrid += move.Unit;
                II.cyMapGrid += move.Layer;
            }
        }
        //core2 --> core1
        l = PutCanvasM.GetChildByCore(Core2);
        foreach (CanvasCore c in l) {
            Int2 move = new  Int2(tomapGrid2.Unit - Info2.cxMapGrid, tomapGrid2.Layer - Info2.cyMapGrid);
            ShipPutInfo II = P.GetShipBuildInfo(c);
            if (II != null) {
                II.cxMapGrid += move.Unit;
                II.cyMapGrid += move.Layer;
            }
        }
        //房间数据互相置换
        RoomMap.ExchangeCanvans(RoomMap.GetPutRoomGrid(Core1), RoomMap.GetMovetoRoomGrid(Core1, tomapGrid1), RoomMap.GetPutRoomGrid(Core2), RoomMap.GetMovetoRoomGrid(Core2, tomapGrid2));
        
        //更新金库房间
        if (Info1.IsGoldBuild() == true) {
            RoomMap.UpdateGoldRoomGrid(Core1.GetMovetoRoomGridPos(tomapGrid1));
        }
        if (Info2.IsGoldBuild() == true) {
            RoomMap.UpdateGoldRoomGrid(Core2.GetMovetoRoomGridPos(tomapGrid2));
        }
        
        //更新数据
        Info1.SetBuildPostion(tomapGrid1);
        Info2.SetBuildPostion(tomapGrid2);
        return true;
    }
    
    /// <summary>
    /// 获取未连接的放置房间
    /// </summary>
    public static List<CanvasCore> GetNoLinkPutRoom(/*CanvasCore Core,  Int2 TargetPos*/)
    {
    
        List<CanvasCore> lNolinkCore = new List<CanvasCore>();
        /*
        if (Core.m_type != ShipBuildType.BuildRoom)
        	return lNolinkCore;
        if (Core.IsDeckRoom == true)
        {
            lNolinkCore.Add(Core);
            return lNolinkCore;
        }*/
        //
        List<Int2> lOld = new List<Int2>();//Core.GetPutRoomGridXY();
        List<Int2> lNew = new List<Int2>();//Core.GetMovetoRoomGridXY(TargetPos);
        //获取未连接的节点
        List<RoomGrid> lNolink = RoomMap.GetNoLinkRoom(lOld, lNew);
        
        List<int> lBuild = new List<int>();
        foreach (RoomGrid R in lNolink) {
            if (R != null) {
                if (lBuild.Contains(R.buildingid) == false) {
                    lBuild.Add(R.buildingid);
                }
            }
        }
        //
        foreach (int buildingid in lBuild) {
            if (buildingid != RoomGrid.EMPTYGRIDID) {
                lNolinkCore.Add(new CanvasCore(ShipBuildType.BuildRoom, false, buildingid, buildingid, Vector2.zero));
            }
            //else lNolinkCore.Add(Core);
        }
        return lNolinkCore;
    }
    
    /// <summary>
    /// 获取未连接的甲板房间
    /// </summary>
    public static List<CanvasCore> GetNoLinkPutDeckRoom()
    {
    
        List<CanvasCore> lNolinkCore = new List<CanvasCore>();
        //
        //获取未连接的节点
        List<RoomGrid> lNolink = RoomMap.GetUnLinkDeckRoom();
        
        List<int> lBuild = new List<int>();
        foreach (RoomGrid R in lNolink) {
            if (R != null) {
                if (lBuild.Contains(R.buildingid) == false) {
                    lBuild.Add(R.buildingid);
                }
            }
        }
        
        //
        foreach (int buildingid in lBuild) {
            if (buildingid != RoomGrid.EMPTYGRIDID) {
                lNolinkCore.Add(new CanvasCore(ShipBuildType.BuildRoom, false, buildingid, buildingid, Vector2.zero));
            }
        }
        return lNolinkCore;
    }
    
    /// <summary>
    /// 获取未连接的甲板房间
    /// </summary>
    public static List<CanvasCore> GetNoLinkSolder()
    {
        return RoomMap.GetUnLinkSoldiers();
    }
    
    /// <summary>
    /// 新增房间/炮弹兵 获取可放置的格子位置
    /// </summary>
    /// <returns></returns>
    public static bool GetBornPos(CanvasCore core, ref Int2 gridPos)
    {
        if (core == null) {
            return false;
        }
        /*
        List<RoomGrid> ListCanPut = new List<RoomGrid>();
        if (core.m_type == ShipBuildType.BuildRoom&&core.IsDeckRoom)
        
        	ListCanPut = RoomMap.GetCanPutBuildIdleRoom ();
        else {
        	ListCanPut = RoomMap.GetCanPutSoldierIdleRoom();
        };
        */
        List<Int2> posListCanPutRoomGrid = RoomMap.GetCanPutArea(core.m_type, core.IsDeckRoom);
        
        foreach (Int2 pos in posListCanPutRoomGrid) {
            Int2 posMap = RoomMap.GetMapGrid(pos);
            //if(core.m_type == ShipBuildType.BuildRoom)
            //{
            //	pos = grid.BuildPos;
            //}
            //else pos = grid.SoldierPos;
            if (CheckCanPut(core, posMap)) {
                gridPos = posMap;
                bool isDeck = BattleEnvironmentM.CheckIsDeckGrid(gridPos);
                if (!core.IsDeckRoom && isDeck) {
                    return false;
                }
                if (core.m_type == ShipBuildType.Soldier) {
                    gridPos.Unit += 1;
                }
                
                return true;
            }
        }
        List<RoomGrid> ListCanPut = RoomMap.GetAllRoomGrid();
        foreach (RoomGrid grid in ListCanPut) {
            Int2 pos = Int2.zero;
            if (core.m_type == ShipBuildType.BuildRoom) {
                pos = grid.BuildPos;
            } else {
                pos = grid.SoldierPos;
            }
            if (CheckCanTempPut(core, pos)) {
                gridPos = pos;
                return true;
            }
        }
        return false;
    }
    
    
    
    public static void ShowRoomGridUI(bool bShow)
    {
        TouchMove tm = TouchMoveManager.GetCurTouchMove();
        if (tm != null) {
            tm.ShowTrapRoomUI(bShow);
        }
    }
    public static void HideCangKuUI()
    {
        CangKuWnd wnd = WndManager.FindDialog<CangKuWnd>();
        if (wnd) {
            wnd.CloseMenu();
        }
    }
    
    public static TouchMove GetTouchMoveByCore(CanvasCore core)
    {
        TouchMove tm = TouchMoveManager.GetShipBuild(core);
        if (tm != null) {
            return tm;
        }
        return null;
    }
    public static TouchMove GetTouchMoveByRoomGrid(RoomGrid rGrid, ShipBuildType type)
    {
        if (null == rGrid) {
            return null;
        }
        List<CanvasCore> childCores = rGrid.GetOtherBuild();
        for (int i = 0; i < childCores.Count; i++) {
            CanvasCore curCore = childCores[i];
            if (curCore.m_type == type) {
                return PutCanvasM.GetTouchMoveByCore(curCore);
            }
        }
        return null;
    }
    
    public static Transform GetTransformByCore(CanvasCore core)
    {
        Transform t = null;
        TouchMove tm = TouchMoveManager.GetShipBuild(core);
        if (tm != null) {
            return tm.gameObject.transform;
        }
        return t;
    }
    public static void StopEdit()
    {
        TouchMoveManager.SetCurTouchMove(null);
    }
    public static void PutDownNewBuild()
    {
        TouchMove curTouchMove = TouchMoveManager.GetCurTouchMove();
        if (null != curTouchMove) {
            if (curTouchMove.MyCore().IsNewCreate) {
                bool canput = curTouchMove.MoveBuildUp();
                if (canput == false) {
                    curTouchMove.DestroyShipBuild();
                }
            }
        }
    }
    
    public static bool CheckSave()
    {
        PutCanvasM.StopEdit();
        
        List<TouchMove> lOutShape = CheckBuildOutOfShape();
        if (lOutShape.Count > 0) {
            NGUIUtil.ShowTipWndByKey<string>("10000204");
            foreach (TouchMove item in lOutShape) {
                item.LoadEditEffect(TouchMoveState.CannotEdit);
            }
            PutCanvasM.SaveSuc = false;
            return PutCanvasM.SaveSuc;
        }
        
        List<TouchMove> lDeckBuild = CheckDeckRoomInMaxLayer();
        if (lDeckBuild.Count > 0) {
            NGUIUtil.ShowTipWndByKey<string>("10000206");
            foreach (TouchMove item in lDeckBuild) {
                item.LoadEditEffect(TouchMoveState.CannotEdit);
            }
            PutCanvasM.SaveSuc = false;
            return PutCanvasM.SaveSuc;
        }
        
        s_listNoLinkPutRoomcores = PutCanvasM.GetNoLinkPutRoom();
        if (s_listNoLinkPutRoomcores.Count > 0) {
            //TouchMoveManager.ShowCanvas(true);
            TouchMoveManager.SetCanPutArea(ShipBuildType.BuildRoom, false);
            NGUIUtil.ShowTipWndByKey<string>("30000013");
            int nCoreCount = s_listNoLinkPutRoomcores.Count;
            int nCoreCnt = 10;
            for (nCoreCnt = 0; nCoreCnt < nCoreCount; nCoreCnt++) {
                CanvasCore core = s_listNoLinkPutRoomcores[nCoreCnt];
                PutCanvasM.GetTouchMoveByCore(core).LoadEditEffect(TouchMoveState.CannotEdit);
            }
            PutCanvasM.SaveSuc = false;
            return PutCanvasM.SaveSuc;
        }
        s_listNoLinkPutDeckRoomcores = PutCanvasM.GetNoLinkPutDeckRoom();
        if (s_listNoLinkPutDeckRoomcores.Count > 0) {
            //TouchMoveManager.ShowCanvas(true);
            TouchMoveManager.SetCanPutArea(ShipBuildType.BuildRoom, true);
            NGUIUtil.ShowTipWndByKey<string>("30000016");
            int nCoreCount = s_listNoLinkPutDeckRoomcores.Count;
            int nCoreCnt = 10;
            for (nCoreCnt = 0; nCoreCnt < nCoreCount; nCoreCnt++) {
                CanvasCore core = s_listNoLinkPutDeckRoomcores[nCoreCnt];
                PutCanvasM.GetTouchMoveByCore(core).LoadEditEffect(TouchMoveState.CannotEdit);
            }
            PutCanvasM.SaveSuc = false;
            return PutCanvasM.SaveSuc;
        }
        s_listNoLinkSolders = PutCanvasM.GetNoLinkSolder();
        if (s_listNoLinkSolders.Count > 0) {
            //TouchMoveManager.ShowCanvas(true);
            TouchMoveManager.SetCanPutArea(ShipBuildType.Soldier, false);
            NGUIUtil.ShowTipWndByKey<string>("30000015");
            int nCoreCount = s_listNoLinkSolders.Count;
            int nCoreCnt = 10;
            for (nCoreCnt = 0; nCoreCnt < nCoreCount; nCoreCnt++) {
                CanvasCore core = s_listNoLinkSolders[nCoreCnt];
                PutCanvasM.GetTouchMoveByCore(core).LoadEditEffect(TouchMoveState.CannotEdit);
            }
            PutCanvasM.SaveSuc = false;
            return PutCanvasM.SaveSuc;
        }
        
        
        return PutCanvasM.SaveSuc;
    }
    
    
    public static bool NoLinkListContain(CanvasCore coreTouchDown)
    {
        if (PutCanvasM.SaveSuc) {
            return false;
        }
        bool bresult = false;
        if (null != s_listNoLinkPutRoomcores) {
            int nCoreCount = s_listNoLinkPutRoomcores.Count;
            int nCoreCnt = 10;
            for (nCoreCnt = 0; nCoreCnt < nCoreCount; nCoreCnt++) {
                CanvasCore core = s_listNoLinkPutRoomcores[nCoreCnt];
                if (core.m_ID == coreTouchDown.m_ID) {
                    bresult = true;
                }
                PutCanvasM.SaveSuc = false;
            }
        }
        if (null != s_listNoLinkPutDeckRoomcores) {
            int nCoreCount = s_listNoLinkPutDeckRoomcores.Count;
            int nCoreCnt = 10;
            for (nCoreCnt = 0; nCoreCnt < nCoreCount; nCoreCnt++) {
                CanvasCore core = s_listNoLinkPutDeckRoomcores[nCoreCnt];
                if (core.m_ID == coreTouchDown.m_ID) {
                    bresult = true;
                }
                PutCanvasM.SaveSuc = false;
            }
        }
        if (null != s_listNoLinkSolders) {
            int nCoreCount = s_listNoLinkSolders.Count;
            int nCoreCnt = 10;
            for (nCoreCnt = 0; nCoreCnt < nCoreCount; nCoreCnt++) {
                CanvasCore core = s_listNoLinkSolders[nCoreCnt];
                if (core.m_ID == coreTouchDown.m_ID) {
                    bresult = true;
                }
                PutCanvasM.SaveSuc = false;
            }
        }
        return bresult;
    }
    public static void ClearNoLinkList()
    {
        if (PutCanvasM.SaveSuc) {
            return ;
        }
        PutCanvasM.SaveSuc = true;
        if (null != s_listNoLinkPutRoomcores) {
            int nCoreCount = s_listNoLinkPutRoomcores.Count;
            int nCoreCnt = 10;
            for (nCoreCnt = 0; nCoreCnt < nCoreCount; nCoreCnt++) {
                CanvasCore core = s_listNoLinkPutRoomcores[nCoreCnt];
                PutCanvasM.GetTouchMoveByCore(core).UnLoadEditEffect();
            }
            s_listNoLinkPutRoomcores.Clear();
        }
        if (null != s_listNoLinkPutDeckRoomcores) {
            int nCoreCount = s_listNoLinkPutDeckRoomcores.Count;
            int nCoreCnt = 10;
            for (nCoreCnt = 0; nCoreCnt < nCoreCount; nCoreCnt++) {
                CanvasCore core = s_listNoLinkPutDeckRoomcores[nCoreCnt];
                PutCanvasM.GetTouchMoveByCore(core).UnLoadEditEffect();
            }
            s_listNoLinkPutRoomcores.Clear();
        }
        if (null != s_listNoLinkSolders) {
            int nCoreCount = s_listNoLinkSolders.Count;
            int nCoreCnt = 10;
            for (nCoreCnt = 0; nCoreCnt < nCoreCount; nCoreCnt++) {
                CanvasCore core = s_listNoLinkSolders[nCoreCnt];
                PutCanvasM.GetTouchMoveByCore(core).UnLoadEditEffect();
            }
            s_listNoLinkSolders.Clear();
        }
        return ;
    }
    /// <summary>
    /// 获取房间内的子对象
    /// </summary>
    public static List<CanvasCore> GetChildByCore(CanvasCore core)
    {
        List<CanvasCore>  l = new List<CanvasCore>();
        if (core.m_type == ShipBuildType.BuildRoom) {
            List<CanvasCore>  ll = new List<CanvasCore>();
            List<RoomGrid> Rlist = RoomMap.GetPutRoomGrid(core);
            foreach (RoomGrid R in Rlist) {
                ll.AddRange(R.GetOtherBuild());
            }
            
            foreach (CanvasCore Core in ll) {
                if (l.Contains(Core) == false) {
                    l.Add(Core);
                }
            }
        }
        return l;
    }
    /// <summary>
    /// 检测建筑是否超出设计图范围.
    /// </summary>
    /// <returns><c>true</c>, if out of shape was built, <c>false</c> otherwise.</returns>
    public static List<TouchMove> CheckBuildOutOfShape()
    {
        return TouchMoveManager.GetAllBuildOutShape();
    }
    
    public static List<TouchMove> CheckDeckRoomInMaxLayer()
    {
        return TouchMoveManager.GetDeckBuildListInMaxLayer();
        
    }
    
}



