#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#define UNITY_EDITOR_LOG
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public enum TouchMoveState
{
	None = 0,//无状态
	CanEdit = 0,//选中框（绿）可编辑提示
	CannotEdit = 1,//选中框（红）编辑不正确显示
}
/// <summary>
/// 摆放房间、陷阱、士兵
/// </summary>
/// <author>QFord</author>
/// <Refactor>zhulin</Refactor>
public class TouchMove : MonoBehaviour
{
	private TouchMove m_ExchangeTouch = null;
	
	private Color m_initialEmissionColor = new Color(0.392f, 0.392f, 0.392f, 0f);
    private CanvasCore m_Core = new CanvasCore();//核心数据
    private GameObject GoClickBuildBtns;//操作建筑物UI（升级、信息、删除）
    public GameObject SelectEffectParent;//光效--选中框 父节点
    /// <summary>
    /// 用于判断是否有建筑或炮弹兵正在设置参数
    /// </summary>
    public static bool g_bSetParaing = false;
    /// <summary>
    /// 标识自己正在设置参数
    /// </summary>
    public bool m_bSelfSetParaing = false;

    private Building m_Building = null;

    private const float BUILD_FIT_DEPTH = 0f;//贴合固定位置
    private const float BUILD_TOP_DEPTH = -4f;//选中浮动位置
    private const float STAIR_FIT_DEPTH = 1f;
    private const float STAIR_TOP_DEPTH = 0f;
    private const float SOLDIER_FIT_DEPTH = -2f;
    private const float SOLDIER_TOP_DEPTH = -5f;

    private Transform m_tTarget;
    private Transform m_tStart;
    private Animation m_ani;
    /// <summary>
    /// 当前位置格子
    /// </summary>
    public Int2 m_posMapGrid;
	public Int2 m_OffsetGrid;
	public Int2 m_posPreMapGrid;
	/// <summary>
    /// 移动前的格子
    /// </summary>
    public Int2 m_orgPosMapGrid;
    private Int2 m_BornPos;//出生时的格子

    public string name;//便于观察的对象名称

    private Int2 m_TouchDownGrid;
	private Int2 m_LastMouseMoveGrid;
	private Int2 m_TouchDownRoomGrid;
	private Vector3 m_v3TouchDownOffset;

    private Vector3 m_v3GoClickBuildBtns;
   // private bool m_bAnimating = false;

    public CanvasCore MyCore()
    {
		return m_Core;
    }
   
    /// <summary>
    /// 初始化TouchMoveData 的核心数据
    /// </summary>
    public void InitTouchMoveCore(CanvasCore Core, Int2 BornPos ,Life life)
    {
        m_tTarget = transform.parent;
        m_Core.Copy(Core);
        m_orgPosMapGrid = m_posMapGrid = m_BornPos = BornPos;
		if(life != null && life is Building)
		{
			m_Building = life as Building;
		}
    }
	/// <summary>
	/// 设置编辑建筑的配件
	/// </summary>
	public void SetPlugin(GameObject selectEffectParent)
	{
        SelectEffectParent = selectEffectParent;
	}

    public void StartInit()
    {
        name = transform.name;
        m_tStart = BattleEnvironmentM.GetLifeMBornNode(true);
        m_ani = GetComponent<Animation>();
        if (m_Core.m_type != ShipBuildType.BuildStair)
        {
            InitTrapRoomUI();
        }
    }

     public bool IsContainsRoomGrid(RoomGrid roomGrid)
    {
		List<RoomGrid> roomGrids = RoomMap.GetMovetoRoomGrid(m_Core,m_posMapGrid);
        if (roomGrids == null)
        {
            return false;
        }
        return roomGrids.Contains(roomGrid); 
    }

    public void SetClickBuildBtns(bool haveSoldier, bool haveTransgate)
    {
        if (GoClickBuildBtns)
        {
            ClickBuildBtns btns = GoClickBuildBtns.GetComponent<ClickBuildBtns>();
            if (btns)
            {
                btns.ShowBtnDeleteSoldier(haveSoldier);
                btns.ShowBtnSetTransGate(haveTransgate);
                btns.ShowBtnBtnReturnBag(!m_Core.Data.IsGoldBuild());
                btns.RefreshUI();
            }
        }
    }

    private void InitTrapRoomUI()
    {
        GoClickBuildBtns = NDLoad.LoadWndItem("ClickBuildBtns", WndManager.GetWndRoot().transform);
        if (GoClickBuildBtns)
        {
            m_v3GoClickBuildBtns = GoClickBuildBtns.transform.position;
            ClickBuildBtns btns = GoClickBuildBtns.GetComponent<ClickBuildBtns>();
            if (btns)
            {
                //btns.BtnInfo.OnClickEventHandler += BtnInfo_OnClickEventHandler;
                btns.BtnReturnBag.OnClickEventHandler += BtnReutrnBag_OnClickEventHandler;
                //btns.BtnUpgrade.OnClickEventHandler += BtnUpgrade_OnClickEventHandler;
                //btns.BtnDeleteSoldier.OnClickEventHandler += BtnDeleteSoldier_OnClickEventHandler;
                btns.BtnSetTransGate.OnClickEventHandler += BtnSetTransGate_OnClickEventHandler;
            }
            GoClickBuildBtns.SetActive(false);
		}
		SetClickBuildBtns(false, false);
		if (m_Core.m_type == ShipBuildType.BuildRoom)
		{
			BuildProperty lifeobj = GetComponent<BuildProperty>();
			if(lifeobj!=null&&lifeobj.GetModeType()==1605)
				SetClickBuildBtns(false, true);
		}
    }

    public void LoadEditEffect(TouchMoveState state)
    {
        Int2 size = m_Core.GetSize();
        if (state == TouchMoveState.CanEdit)
        {
            LoadEffect(size, "2000701_01");
        }
		else if (state == TouchMoveState.CannotEdit)
        {
            LoadEffect(size, "2000701_03");
		}
		else if (state == TouchMoveState.None)
		{
			UnLoadEditEffect ();
		}
    }
    public void UnLoadEditEffect()
    {
        U3DUtil.DestroyAllChild(SelectEffectParent);
    }
    
    private void LoadEffect(Int2 size,string effectName1)
    {
		UnLoadEditEffect ();

        float width = size.Layer;
        float height = size.Unit;
		float fOffheight = 0f;
        if (m_Core.m_type == ShipBuildType.BuildStair)
        {
            width = 0.5f;
		}
		if (m_Core.m_type == ShipBuildType.Soldier)
		{
			width = 0.7f;
			height = 0.8f;
			fOffheight = 0.15f;
		}
		//左下
		Vector3 pos = new Vector3(fOffheight*RoomGrid.m_heigth, Vector3.zero.y, Vector3.zero.z);
		EffectM.LoadEffect(effectName1, pos, Vector3.zero, SelectEffectParent.transform);
        //左上
		pos = new Vector3(fOffheight*RoomGrid.m_heigth, width * RoomGrid.m_heigth, 0);
        Vector3 rotation = new Vector3(0, 0, 270);
        EffectM.LoadEffect(effectName1, pos, rotation, SelectEffectParent.transform);
        //右上
        pos = new Vector3(height * RoomGrid.m_width, width * RoomGrid.m_heigth, 0);
        rotation = new Vector3(0, 0, 180);
        EffectM.LoadEffect(effectName1, pos, rotation, SelectEffectParent.transform);
        //右下
        pos = new Vector3(height * RoomGrid.m_width, 0, 0);
        rotation = new Vector3(0, 0, 90);
        EffectM.LoadEffect(effectName1, pos, rotation, SelectEffectParent.transform);
    }

    void BtnSetTransGate_OnClickEventHandler(UIButton sender)
    {
        SetBuildParaStart();
		List<CanvasCore> childrenCore = PutCanvasM.GetChildByCore(m_Core);
        if (childrenCore.Count == 0)
        {
#if UNITY_EDITOR_LOG
            NGUIUtil.DebugLog("房间内未检测到传送门，请调查！！！");
#endif
            return;
        }
		ShipPlan P = ShipPlanDC.GetCurShipPlan();
		if(P  == null) return ;
        foreach (var childCore in childrenCore)
        {
			TouchMove tm = PutCanvasM.GetTouchMoveByCore(childCore);
			BuildProperty lifeobj = tm.GetComponent<BuildProperty>();
			if(lifeobj!=null&&lifeobj.GetModeType()==1605)
			{
				Building1605 b = lifeobj.GetLife() as Building1605;
				if (b!=null)
				{
					ShipPutInfo Info = P.GetShipBuildInfo(m_Core);
					b.ShowTranGate(new Int2(Info.shipput_data0, Info.shipput_data1));
				}
			}
        }
    }

    void BtnDeleteSoldier_OnClickEventHandler(UIButton sender)
    {
		List<CanvasCore> childrenCore = PutCanvasM.GetChildByCore(m_Core);
        if (childrenCore.Count == 0)
        {
            return;
        }

        foreach (var childCore in childrenCore)
        {
			TouchMove tm = PutCanvasM.GetTouchMoveByCore(childCore);
            if (childCore.m_type == ShipBuildType.Soldier)
            {
                tm.DestroyShipBuild();
            }
        }
		PutCanvasM.ShowRoomGridUI(false);
    }

    private void BtnReutrnBag_OnClickEventHandler(UIButton sender)
    {
        if (m_Core.m_type == ShipBuildType.BuildRoom)
        {
            SetBuildParaOver();
			PutCanvasM.ShowRoomGridUI(false);
			DestroyShipBuild();
        }
        else if (m_Core.m_type == ShipBuildType.Soldier)
        {
            DestroyShipBuild();
			PutCanvasM.ShowRoomGridUI(false);
		}
	}

    void Start()
    {
        StartInit();
    }

	

    private bool CheckCanSetPara()//检测能否被设置参数
    {
        if (Input.GetMouseButtonDown(0) == true && m_bSelfSetParaing == true)
        {
            return true;
        }
        else return false;
    }


    /// <summary>
    /// 移动房间完毕后，解除房间内对象的父子关系
    /// </summary>
    public void LeaveRoom()
    {
        if (m_Core.m_type != ShipBuildType.BuildRoom)
        {
            return;
		}
		List<CanvasCore> childrenCore = PutCanvasM.GetChildByCore(m_Core);
        if (childrenCore.Count == 0)
        {
            return;
        }
        if (m_tStart == null)
        {
            return;
        }
        foreach (var childCore in childrenCore)
        {
			TouchMove tm = PutCanvasM.GetTouchMoveByCore(childCore);
            //tm.m_bCanOperate = true;
			Transform childShellT = PutCanvasM.GetTransformByCore(childCore).parent;
			childShellT.parent = m_tStart;
        }
    }
    public void UpdateChildmGridWhenUp()
	{
		List<CanvasCore> childrenCore = PutCanvasM.GetChildByCore(m_Core);
		if (childrenCore==null||childrenCore.Count == 0)
        {
            return;
        }
        if (m_tStart == null)
        {
            return;
		}
		foreach (var childCore in childrenCore)
        {
			TouchMove tm = PutCanvasM.GetTouchMoveByCore(childCore);
			if(tm)
			{
				tm.m_posMapGrid = m_posMapGrid - tm.m_OffsetGrid;
				tm.m_orgPosMapGrid = tm.m_posMapGrid;
			}
            
        }
    }
    public void MoveWithRoom(CanvasCore roomCore)
    {
        if (roomCore.m_type != ShipBuildType.BuildRoom)
        {
            return;
        }
		TouchMove tmParent = PutCanvasM.GetTouchMoveByCore(roomCore);
		List<CanvasCore> childrenCore = PutCanvasM.GetChildByCore(roomCore);
		if (childrenCore.Count == 0||roomCore.IsDeckRoom)
        {
            return;
        }
		Transform roomShellT = PutCanvasM.GetTransformByCore(roomCore).parent;
        foreach (var childCore in childrenCore)
        {
			TouchMove tm = PutCanvasM.GetTouchMoveByCore(childCore);
			if(tm != null)
			{
				tm.m_OffsetGrid = tmParent.m_orgPosMapGrid - tm.m_orgPosMapGrid;
			}
			if(PutCanvasM.GetTransformByCore(childCore) != null)
			{
				Transform childShellT = PutCanvasM.GetTransformByCore(childCore).parent;
				childShellT.parent = roomShellT;
			}
		}
	}

    public void ResetPos()
    {
        GenerateDeck();
        List<Int2> moveToGrids = new List<Int2>();
        moveToGrids.Add(m_posMapGrid);
		TouchMoveManager.SetGridsBgStates(moveToGrids, CanvasUnitState.Normal,true);
		m_posMapGrid = m_orgPosMapGrid;
        m_tTarget.localPosition = GetPosByGrid(m_orgPosMapGrid);
    }

	/// <summary>
	/// 生成甲板 排除拖动的房间
	/// </summary>
	private void GenerateDeckWithOutThis()
	{
		ShipPlan P = ShipPlanDC.GetCurShipPlan();
		if(P  == null) return ;
		P.DoDeckDataStartWithOut(m_Core.Data);
		GenerateShip.CreateMiddleBeam();
		P.DoDeckDataEnd();
	}
    /// <summary>
    /// 生成甲板
    /// </summary>
    private void GenerateDeck()
    {
		ShipPlan P = ShipPlanDC.GetCurShipPlan();
		if(P  == null) return ;
        P.DoDeckDataStart();
        GenerateShip.CreateMiddleBeam();
		P.DoDeckDataEnd();
    }
   
    /// <summary>
    /// 根据当前所在格子设置局部坐标位置
    /// </summary>
    private void SetPosByGrid(bool isAni = false)
	{
		#if UNITY_EDITOR_LOG
		if (m_Core.m_type == ShipBuildType.Soldier && m_posMapGrid.Unit % MapGrid.m_UnitRoomGridNum != 4)
        {
                NGUIUtil.DebugLog("炮弹兵位置出错：" + m_posMapGrid);
		}
		#endif
		Vector3 localPos = RoomMap.GetShipBuildLocalPos(m_posMapGrid, m_Core.m_type);
        if (isAni == false)
        {
            m_tTarget.localPosition = localPos;
        }
        else
		{
			//m_bAnimating = true;
			PutCanvasM.CanOperate = false;
            m_tTarget.DOMove(localPos, 0.2f);
		}
	}
	void SetPosByGridAnimationEnd()
	{
		PutCanvasM.CanOperate = true;
		LeaveRoom ();
		//m_bAnimating = false;
		PlayDuskEffect();
	}
    private void SetCenterByGridPos(Vector3 pos)
    {
        m_tTarget.localPosition = pos;
    }

    private Vector3 GetPosByGrid(Int2 grid)
    {
		return RoomMap.GetShipBuildLocalPos(grid, m_Core.m_type);
    }
    /// <summary>
    /// 房间置换
    /// </summary>
    void DoRoomExchange(Int2 mapGrid)
	{
        if (m_Core.m_type == ShipBuildType.BuildStair)
		{
			return ;
        }
		if (!m_Core.IsNewCreate)
		{
			TouchMove tmCurExchange = m_ExchangeTouch;
			if(null!=tmCurExchange)
			{
				if (m_ExchangeTouch.MyCore ().IsDeckRoom == MyCore ().IsDeckRoom && m_ExchangeTouch.MyCore ().m_type == ShipBuildType.BuildRoom && MyCore ().m_type == ShipBuildType.BuildRoom) {
					PutCanvasM.ExchangeRoom (m_ExchangeTouch.MyCore (), m_ExchangeTouch.m_orgPosMapGrid, MyCore (), m_orgPosMapGrid);
				} else {
					m_ExchangeTouch.MoveShipBuilding (m_ExchangeTouch.m_orgPosMapGrid);
					MoveShipBuilding (m_orgPosMapGrid);
				}
			}
			CanvasCore core = RoomMap.FindCanvasCore(mapGrid,m_Core.m_type);
			if (core != null&&core.m_ID!=m_Core.m_ID)
	        {
	            bool b = PutCanvasM.CheckExchange(m_Core, core);
	            if (b)
				{
					TouchMove tmNew = PutCanvasM.GetTouchMoveByCore(core);
					if(tmNew!=null)
					{
						m_ExchangeTouch = tmNew;			
						if (m_ExchangeTouch.MyCore ().IsDeckRoom == MyCore ().IsDeckRoom && m_ExchangeTouch.MyCore ().m_type == ShipBuildType.BuildRoom && MyCore ().m_type == ShipBuildType.BuildRoom) {
							PutCanvasM.ExchangeRoom (m_ExchangeTouch.MyCore (), m_orgPosMapGrid, MyCore (), m_posMapGrid);
						} else {
							m_ExchangeTouch.MoveShipBuilding (m_orgPosMapGrid);
							MoveShipBuilding (m_posMapGrid);
						}
						if (m_ExchangeTouch != tmCurExchange)
						{
							m_ExchangeTouch.ExchangeAnimation(m_orgPosMapGrid);
						}
					}
					
				}
	            else
	            {
					m_ExchangeTouch=null;
				}
			}
	        else
			{
				m_ExchangeTouch=null;
			}
			if(null!=tmCurExchange&&m_ExchangeTouch!=tmCurExchange)
			{
				//MoveShipBuilding (m_posMapGrid);
				tmCurExchange.MoveShipBuilding (tmCurExchange.m_orgPosMapGrid);
				tmCurExchange.ResetExchangePos ();
			}
		}
    }
    /// <summary>
    /// 设置位置动画表现
    /// </summary>
    public void ExchangeAnimation(Int2 grid)
	{
		//LeaveRoom ();
		MoveWithRoom(MyCore());
		Vector3 posWold = RoomMap.GetShipBuildLocalPos(grid, m_Core.m_type);
		Transform tStart =  BattleEnvironmentM.GetLifeMBornNode(true);
		if (null != tStart)
			posWold = tStart.TransformPoint(posWold);
        m_tTarget.DOMove(posWold, 0.3f);
	}

	void ExchangeAnimationEnd()
	{
		LeaveRoom ();
       // m_bAnimating = false;
	}
	public void ResetExchangePos()
	{
		m_posMapGrid = m_orgPosMapGrid;
		ExchangeAnimation(m_posMapGrid);
	}
	public void OnMouseDown()
	{
		PlayEditSelectAnimation();
		m_TouchDownGrid = GetMousePosGrid();
		m_LastMouseMoveGrid = m_TouchDownGrid;
		m_TouchDownRoomGrid = new Int2(m_posMapGrid.Unit,m_posMapGrid.Layer);
		MoveWithRoom(m_Core);
		m_v3TouchDownOffset = GetMouseGridPos()-m_tTarget.localPosition;
		m_posPreMapGrid = m_posMapGrid;
		UpdateMyGridColor(true);
	}
	/// <summary>
	/// 移动建筑处理
	/// </summary>
    public void MoveBuild()
    {


		Int2 curMouseMoveGrid = GetMousePosGrid();
		Vector3 gridPos = GetMouseGridPos()-m_v3TouchDownOffset;
		SetCenterByGridPos(gridPos);
        Int2 offsetGrid = curMouseMoveGrid - m_TouchDownGrid;
		Int2 iNew = new Int2(m_TouchDownRoomGrid.Unit + offsetGrid.Unit * MapGrid.m_UnitRoomGridNum, m_TouchDownRoomGrid.Layer + offsetGrid.Layer);
		//判断是否移出界外
		if (curMouseMoveGrid == new Int2(-1,-1)
		    ||m_Core.IsDeckRoom==false&&iNew.Layer+m_Core.GetSize().Layer-1>=RoomMap.DeckRoomTopLayer&&m_Core.m_type == ShipBuildType.BuildRoom)
		{
			m_LastMouseMoveGrid = curMouseMoveGrid;
			return;
		}
		if (RoomMap.CheckAllInMap(m_Core,iNew))
			m_posMapGrid = iNew;
		if (m_LastMouseMoveGrid != curMouseMoveGrid)
		{
			Int2 mapGrid = new Int2(curMouseMoveGrid.Unit * MapGrid.m_UnitRoomGridNum, curMouseMoveGrid.Layer);
			DoRoomExchange(mapGrid); //同shape房间置换
			bool bCanPutTemp = PutCanvasM.CheckCanTempPut (m_Core, m_posMapGrid);
			if (bCanPutTemp) {
				MoveShipBuilding (m_posMapGrid);
			} 
			if(m_Core.m_type == ShipBuildType.BuildRoom&&!m_Core.IsDeckRoom)
				GenerateDeckWithOutThis();
			UpdateMyGridColor(bCanPutTemp);
		}
		m_LastMouseMoveGrid = curMouseMoveGrid;
		m_posPreMapGrid = m_posMapGrid;
	}
	/// <summary>
	/// 放开建筑处理
	/// </summary>
	public bool MoveBuildUp()
	{
		if (m_ExchangeTouch && m_ExchangeTouch!=this)
		{
			m_ExchangeTouch.m_posMapGrid = m_orgPosMapGrid;
			m_ExchangeTouch.m_orgPosMapGrid = m_ExchangeTouch.m_posMapGrid;
			m_ExchangeTouch.UpdateChildmGridWhenUp();

			m_orgPosMapGrid = m_posMapGrid;
			UpdateChildmGridWhenUp();
			SetPosByGrid(true);
			if(!m_ExchangeTouch.MyCore ().IsDeckRoom)
				CreateStair(m_Core);
			m_ExchangeTouch=null;
			GenerateDeck ();
			return true;
		}
		
		
		bool bCanPut = PutCanvasM.CheckCanTempPut(m_Core, m_posMapGrid);
		if (bCanPut) 
		{
			m_orgPosMapGrid = m_posMapGrid;
			UpdateChildmGridWhenUp ();
			SetPosByGrid(true);
		} 
		else 
		{
			m_posMapGrid = m_orgPosMapGrid;
			UpdateChildmGridWhenUp();
			MoveShipBuilding (m_posMapGrid);
			SetPosByGrid(true);
			
		}
		if(!MyCore ().IsDeckRoom)
			CreateStair(m_Core);
		GenerateDeck ();
		return true;
	}
	public void UpdateMyGridColor(bool bCanPutTemp=true)
	{
			List<Int2> moveToGrids = m_Core.GetMovetoRoomGridPos(m_posMapGrid);
			if (!bCanPutTemp) 
			{
				TouchMoveManager.SetGridsBgStates (moveToGrids, CanvasUnitState.CanntPut, true);
				return ;
			}
            if (PutCanvasM.CheckCanPut(m_Core, m_posMapGrid))
            {
				TouchMoveManager.SetGridsBgStates(moveToGrids, CanvasUnitState.CanPut,true);
			}
			else
				TouchMoveManager.SetGridsBgStates(moveToGrids, CanvasUnitState.CanntPut,true);

			if (null != m_ExchangeTouch) 
			{
				moveToGrids = m_ExchangeTouch.MyCore().GetMovetoRoomGridPos(m_orgPosMapGrid);
				if (PutCanvasM.CheckCanPut(m_ExchangeTouch.MyCore(), m_orgPosMapGrid))
				{
						TouchMoveManager.SetGridsBgStates(moveToGrids, CanvasUnitState.CanPut,false);
				}
				else
					TouchMoveManager.SetGridsBgStates(moveToGrids, CanvasUnitState.CanntPut,false);
			}
	}
	
	public  Int2 GetMousePosGrid()
	{
		Vector3 v3TouchScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z);
		Vector3 v3TouchWorldPos = Camera.main.ScreenToWorldPoint(v3TouchScreenPos);
		Vector3 v3Local = BattleEnvironmentM.World2LocalPos(v3TouchWorldPos);
		RoomGrid roomGrid = RoomMap.FindRoomGrid(v3Local);

		if (roomGrid == null)
		{
			return new Int2(-1, -1);
		}
		return roomGrid.mPosRoomGrid;
	}
	public  Vector3 GetMouseGridPos()
	{
		Vector3 v3TouchScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z);
		Vector3 v3TouchWorldPos = Camera.main.ScreenToWorldPoint(v3TouchScreenPos);
		Vector3 gridPos = BattleEnvironmentM.World2LocalPos(v3TouchWorldPos);
		return gridPos;
	}

	/// <summary>
	/// 判断有没移动到边界
	/// </summary>
	public bool CheckMoveBorder(Int2 offsetGrid)
	{
		Int2 shipSize = RoomMap.RealMapSize;
		if (m_posMapGrid.Layer + offsetGrid.Layer < 0 || m_posMapGrid.Unit + offsetGrid.Unit* MapGrid.m_UnitRoomGridNum < 0)
		{
			return false;
		}

		if (m_posMapGrid.Layer + offsetGrid.Layer > shipSize.Layer + (m_Core.GetSize().Layer  - 1) 
		    || m_posMapGrid.Unit + (offsetGrid.Unit + m_Core.GetSize().Unit  - 1) * MapGrid.m_UnitRoomGridNum > shipSize.Unit* MapGrid.m_UnitRoomGridNum )
		{
			return false;
		}
		return true;
	}

    /// <summary>
    /// 从仓库放到船上。
    /// </summary>
    public void WareHouse2Ship(Int2 TargetMg)
    {
        PutCanvasM.AddNewCore(m_Core, TargetMg);
        SetTouchMoveData(m_Core.m_type, false, m_Core.m_ID);
		CreateStair(m_Core);
		GenerateDeck();
        //SetDepth(true);
    }
    /// <summary>
    /// 创建楼梯
    /// </summary>
    /// <param name="Core"></param>
    public static void CreateStair(CanvasCore Core)
	{
		ShipPlan P = ShipPlanDC.GetCurShipPlan();
		if(P  == null) return ;
		if (Core.m_type != ShipBuildType.BuildRoom&&Core.IsDeckRoom)
            return;
		//只有房间变化楼梯才重新创建
        List<Int2> points = RoomMap.GetStairCreatePoint();
        if (points.Count > 0)
        {
            foreach (var grid in points)
            {
                ShipPutInfo Info = new ShipPutInfo();
                Info.id = ShipPutInfo.GetNewShipPutId();
                Info.objid = 1201;
                Info.type = 3;
                Info.cxMapGrid = grid.Unit;
                Info.cyMapGrid = grid.Layer;
                Info.shipput_data0 = 0;
                Info.shipput_data1 = 0;
                P.AddShipBuildInfo(Info, ShipBuildType.BuildStair);
                BuildInfo stair = buildingM.GetStartBuildInfo(1201);
				///// 临时修改，楼梯位置不正确的问题
				//stair.m_type = 1201;
				stair.m_cx = grid.Unit;
				stair.m_cy = grid.Layer;
				stair.m_ShipPutdata0 = 0;
				stair.m_ShipPutdata1 = 0;
				///end
				if (stair != null)
                {
                    BattleEnvironmentM.LoadShipPutStairBuild(Info.id, Info.objid, stair);
                }
            }
        }
        RoomMap.RemoveExcessStair();
    }
    static void BindStair(Int2 grid)
    {
        CanvasCore core = RoomMap.FindCanvasCore(grid,ShipBuildType.BuildRoom);
        if (core!=null)
        {
			TouchMove tm = PutCanvasM.GetTouchMoveByCore(core);
            tm.MoveWithRoom(core);
        }
    }

    /// <summary>
    /// 已在船上的building 进行移动。
    /// </summary>
    void MoveShipBuilding(Int2 TargetMg)
    {
		PutCanvasM.MoveBuildRoom (m_Core, TargetMg);
       // {
            //TouchMoveManager.SetCanPutArea();
        //}
    }
    /// <summary>
    /// 销毁整个对象。
    /// </summary>
    public void DestroyShipBuild()
    {

        PutERR Err = PutERR.ERR_NORMAL;
        if (PutCanvasM.CheckCanRemove(m_Core, ref Err) == false)
        {
            if (Err == PutERR.ERR_NOLink)
            {
				NGUIUtil.ShowTipWndByKey("88800009", 1.0f);
            }
            else if (Err == PutERR.ERR_HaveChild)
            {
				NGUIUtil.ShowTipWndByKey("88800010", 1.0f);
            }
            else
            {
				NGUIUtil.ShowTipWndByKey("88800011", 1.0f);
            }
            return;
        }
        if (m_Core.m_type == ShipBuildType.BuildStair)
        {
			//导致楼梯不删除的原因
			BuildProperty building = GetComponent<BuildProperty>();
            if (building != null)
            {
				GameObject skin = building.m_goAllBodySkin[0];
                if (skin != null)
                {
                    GameObjectActionExcute gae = skin.AddComponent<GameObjectActionExcute>();
                    GameObjectActionColorFade fade = new GameObjectActionColorFade(false, 1f);
                    fade.m_complete = DeleteStair;//删除楼梯表现回调
                    gae.AddAction(fade);
                }
            }
        }
        if (m_Building != null)
        {
            if (m_Core.Data.IsGoldBuild())
            {
				NGUIUtil.ShowTipWndByKey("88800012", 1.0f);
                return;
            }
		}
		MoveWithRoom(MyCore());
		TouchMoveManager.DeleteShipBuild(m_Core);
        CangKuWnd wnd = WndManager.GetDialog<CangKuWnd>();
        PutCanvasM.RemoveBuildRoom(m_Core);
        CreateStair(m_Core);
        GenerateDeck();
        TouchMoveManager.DoTransgatePoint();
        if (m_tTarget != null)
        {
			if (m_Core.m_type != ShipBuildType.BuildStair)
			{
				FlyToCangKuDelete(m_tTarget.gameObject, false);
				if (wnd)
				{
					wnd.RefreshTrapUI();
				}
            }
        }
        //Destroy(GoClickBuildBtns);
    }
    /// <summary>
    /// 删除楼梯回调
    /// </summary>
    /// <param name="obj"></param>
    void DeleteStair(object obj)
    {
        Destroy(m_tTarget.gameObject);
    }
    /// <summary>
    /// 删除 飞行仓库表现
    /// </summary>
    /// <param name="go"></param>
    /// <param name="IsMoveChild">是否移动的是子对象（处理兵的特殊处理）</param>
    void FlyToCangKuDelete(GameObject go, bool IsMoveChild = false)
    {
        if (go)
        {
            CangKuWnd wnd = WndManager.FindDialog<CangKuWnd>();
            if (wnd)
            {
                //Screen.width/2,0)
               Vector3 UIPos = WndManager.GetNGUICamera().ScreenToWorldPoint(new Vector3(Screen.width / 2, 0, 0));
                GameObjectActionExcute gae = go.AddComponent<GameObjectActionExcute>();
                GameObjectAction3DFlyToUI flyToUI = new GameObjectAction3DFlyToUI();
                flyToUI.SetData(Camera.main, WndManager.GetNGUICamera(), UIPos, IsMoveChild);
                gae.AddAction(flyToUI);
            }
            else
            {
                Destroy(go);
            }
        }
    }

    /// <summary>
    /// 播放烟尘表现
    /// </summary>
    void PlayDuskEffect()
    {
        if (m_Core.m_type != ShipBuildType.BuildRoom)//此表现只用于地形和房间
        {
            return;
        }
		List<RoomGrid> roomGridList = RoomMap.GetPutRoomGrid(m_Core);
        foreach (RoomGrid roomGrid in roomGridList)
        {
            PlayDuskAtGrid(roomGrid.BuildPos);
        }
    }
    void PlayDuskAtGrid(Int2 grid)
    {
        Vector3 localPos = RoomMap.GetRoomGridLocalPos(grid);
        Vector3 pos = BattleEnvironmentM.Local2WorldPos(localPos);
        pos = U3DUtil.AddZ(pos, 5f);//在建筑后面播放
        pos = U3DUtil.AddX(pos, RoomGrid.m_width / 2.0f);
        pos = U3DUtil.AddY(pos, RoomGrid.m_heigth / 2.0f);

        GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "2000391", pos, m_tStart);
        GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1.0f);
        gae.AddAction(ndEffect);
    }

    /// <summary>
    /// 修改设置TouchMoveData 的核心数据
    /// </summary>
    /// <param name="Type">类型，炮弹兵或建筑</param>
    /// <param name="Style">所处位置，船上或环境</param>
    /// <param name="ID">ID唯一标识</param>
    /// <returns>变更时设置</returns>
	public void SetTouchMoveData(ShipBuildType Type, bool IsNewCreate, int ID)
    {
        m_Core.m_type = Type;
		m_Core.IsNewCreate = IsNewCreate;
        m_Core.m_ID = ID;
    }

    /// <summary>
    /// 设置建筑参数
    /// </summary>
    public void SetBuildParam(CanvasCore core, int data0, int data1)
    {
		ShipPlan P = ShipPlanDC.GetCurShipPlan();
		if(P  == null) return ;
		if (!m_Core.IsNewCreate)
        {
            if (m_Core.m_type != ShipBuildType.Soldier)
            {
                ShipPutInfo Info = P.GetShipBuildInfo(m_Core);
                if (Info != null)
                {
                    Info.SetBuildPara(core, data0, data1);
                }
            }
        }
        SetBuildParaOver();
    }

    /// <summary>
    /// 设置参数开始状态。
    /// </summary>
    public void SetBuildParaStart()
    {
        ShowTrapRoomUI(false);
        g_bSetParaing = true;
        m_bSelfSetParaing = true;
    }
    /// <summary>
    /// 设置参数结束状态。
    /// </summary>
    public void SetBuildParaOver()
    {
        g_bSetParaing = false;
        m_bSelfSetParaing = false;
    }
    /// <summary>
    /// 操作建筑物UI（升级、信息、删除）
    /// </summary>
    public void ShowTrapRoomUI(bool isShow)
    {
        if (GoClickBuildBtns)
        {
            if (isShow)//显示删除按钮的时候打开主菜单
            {
                CangKuWnd wnd = WndManager.FindDialog<CangKuWnd>();
                if (wnd!=null)
                {
                    wnd.OpenMenu();
                }
            }
            else
            {
                //float posY = GoClickBuildBtns.transform.position.y;
                //NGUIUtil.TweenGameObjectPosY(GoClickBuildBtns, posY, posY - 100, 0.3f, 0f, gameObject, "HideGoClickBuildBtns");
            }
            GoClickBuildBtns.SetActive(isShow);
        }
    }

    void HideGoClickBuildBtns()
    {
        //需重置按钮位置
        Destroy(GoClickBuildBtns);
    }

    void OnDestroy()
    {
    }
    /// <summary>
    /// 启用明亮交替表现
    /// </summary>
    public void TweenColor()
    {
        if (m_Core.m_type == ShipBuildType.BuildRoom)
        {
			(m_Building.m_Property as BuildProperty).TweenPingponeColor(new Color(0.15f, 0.15f, 0.15f, 0), 0.5f);
        }
        else if (m_Core.m_type == ShipBuildType.Soldier)
        {
            RolePropertyM rpm = GetComponent<RolePropertyM>();
            if (rpm)
            {
				(rpm.GetLife() as Role).RoleSkinCom.TweenPingponeColor(new Color(0.15f, 0.15f, 0.15f, 0), 0.5f);
            }
        }
    }
    /// <summary>
    /// 停止明亮交替表现
    /// </summary>
    public void StopTweenColor()
    {
        
        if (m_Core.m_type == ShipBuildType.BuildRoom )
		{
			(m_Building.m_Property as BuildProperty).SetColor("_Emission", m_initialEmissionColor);
        }
        else if (m_Core.m_type == ShipBuildType.Soldier)
        {
			RolePropertyM rpm = GetComponent<RolePropertyM>();
			if (rpm)
			{
				(rpm.GetLife() as Role).RoleSkinCom.TweenPingponeColor(new Color(0.15f, 0.15f, 0.15f, 0), 0.5f);
			}
        }
    }
    /// <summary>
    /// 设置深度 (用于置顶建筑)
    /// </summary>
    /// <param name="isSelect"></param>
    public void SetDepth(bool isSelect)
    {
        float depth = 0;

        if (isSelect)
        {
            if (m_Core.m_type == ShipBuildType.Soldier)
            {
                depth = SOLDIER_TOP_DEPTH;
            }
            else if (m_Core.m_type == ShipBuildType.BuildStair)
            {
                depth = STAIR_TOP_DEPTH;
            }
            else
            {
                depth = BUILD_TOP_DEPTH;
            }
        }
        else
        {
            if (m_Core.m_type == ShipBuildType.Soldier)
            {
                depth = SOLDIER_FIT_DEPTH;
            }
            else if (m_Core.m_type == ShipBuildType.BuildStair)
            {
                depth = STAIR_FIT_DEPTH;
            }
            else
            {
                depth = BUILD_FIT_DEPTH;
            }
        }
        m_tTarget.localPosition = U3DUtil.SetZ(m_tTarget.localPosition, depth);
    }

    /// <summary>
    /// 播放编辑点选时动画
    /// </summary>
    public void PlayEditSelectAnimation()
    {
        if (m_Core.m_type == ShipBuildType.BuildRoom )
        {
			LifeProperty lifeProperty = GetComponent<LifeProperty>();
			if(lifeProperty!=null)
			{
				Building room = lifeProperty.GetLife() as Building;
				if (room!=null)
				{
					room.PlayClickAni();
				}
			}
        }
    }

    /// <summary>
    /// 设置建筑透明（暂未启用）
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowEditAlpha(bool isShow)
    {
        if (m_Core.m_type == ShipBuildType.BuildRoom)
		{
			LifeProperty lifeProperty = GetComponent<LifeProperty>();
			if(lifeProperty!=null)
			{
				Building room = lifeProperty.GetLife() as Building;
				if (room!=null)
				{
					//room.ShowEditAlpha(isShow);
				}
			}
        }
    }
    public void PlayEditSelectAnimationEnd()
    {
        if (m_Core.m_type == ShipBuildType.BuildRoom)
		{
			LifeProperty lifeProperty = GetComponent<LifeProperty>();
			if(lifeProperty!=null)
			{
				Building room = lifeProperty.GetLife() as Building;
				if (room!=null)
				{
					room.PlayViewAni();
				}
			}
        }
    }

    public void DestroyAttackRange()
	{
		LifeProperty lifeProperty = GetComponent<LifeProperty>();
		if(lifeProperty!=null)
		{
			Building room = lifeProperty.GetLife() as Building;
			if (room!=null)
			{
				room.DestroyAttackRange();
			}
		}
    }

}

