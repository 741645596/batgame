using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 点击网格处理
/// </summary>
/// <author>QFord</author>
public class PlaneClickGrid : MonoBehaviour {
    /// <summary>
    /// 当前选定的CanvasCore
    /// </summary>
    private CanvasCore m_currentCanvasCore;

    private RoomGrid m_roomGridTouchDown;//按下时的房间格子
    private TouchMove m_roomTouchMoveTouchDown;//按下时的物件
    private bool m_bTouchMoveChange=true;//触控的物件发生变化
   
    private bool m_bMouseDown;//是否按下
    private Vector3 m_v3MouseTouchDown;//按下时的鼠标位置信息
    private float m_fMouseDownTime; //按下时间
	private bool m_bLongTouch;//是否为长按

	private bool m_isDrag = false;         //是否 在平移中
	private Vector2 m_vtotalmouseDelta;	//记录手指按下后的屏幕移动总和，可用于判断移动和捏合操作
	private float m_mag;//移动的增量值
	public float mouseClickThreshold = 20f;
	private Vector2 m_vlastFirstTouch;     //设备上次触摸点1

    /// <summary>
    /// 用来识别当前点击的楼梯还是兵（规则可以参看UE）
    /// </summary>
    public ShipBuildType m_curClickType;
	
	private Vector2 GetTouchPos(int iTouchCnt)
	{
		Vector2 pos  = Vector3.zero;
		if (SystemInfo.deviceType == DeviceType.Desktop)
			pos = Input.mousePosition;
		else 
			pos = Input.GetTouch(iTouchCnt).position;
		return pos;
	}
    void OnMouseDown()
    {
		if (PutCanvasM.CanOperate == false)
        {
            return;
        }

        RaycastHit hit;
        if (WndManager.IsHitNGUI(out hit))//当点击到UI时不做处理
            return;
        if (Input.touchCount>=2)
            return;
        m_bMouseDown = true;
        m_bLongTouch = false;
        m_bTouchMoveChange = true;
		m_fMouseDownTime = Time.time;
		m_vtotalmouseDelta= Vector2.zero;
		m_vlastFirstTouch = GetTouchPos(0);
		m_isDrag = false;
        if (SystemInfo.deviceType == DeviceType.Desktop)
            m_v3MouseTouchDown = Input.mousePosition;
        else
			m_v3MouseTouchDown = Input.GetTouch(0).position;
		
		//获取按下时画布格子
		Vector3 m_v3TouchScreenPos = new Vector3(m_v3MouseTouchDown.x, m_v3MouseTouchDown.y, -Camera.main.transform.position.z);
        Vector3 m_v3TouchWorldPos = Camera.main.ScreenToWorldPoint(m_v3TouchScreenPos);
		Vector3 v3dLocalPos = BattleEnvironmentM.World2LocalPos(m_v3TouchWorldPos);
		m_roomGridTouchDown = RoomMap.FindRoomGrid(v3dLocalPos);
		if (m_roomGridTouchDown == null) 
		{
			return;
		}
		m_curClickType = RoomMap.PickupShipBuildType(m_roomGridTouchDown,v3dLocalPos,m_roomGridTouchDown.mPosRoomGrid.Layer);

        CanvasCore buildRoomCoreTouchDown = m_roomGridTouchDown.GetBuildRoom();
        if (TouchMove.g_bSetParaing == true)
        {
            return;
        }
        
        TouchMove curTouchMove =  TouchMoveManager.GetCurTouchMove();
        if (null != curTouchMove) 
		{
			//如果当前选中的是房间，新选中的不管是什么都不做变更
			//如果当前选中的不是房间，但是新选中的对象和当前选中的一样则也不变量。
			if (curTouchMove.IsContainsRoomGrid (m_roomGridTouchDown)) {
					TouchMove selNewTouchMov = PutCanvasM.GetTouchMoveByRoomGrid (m_roomGridTouchDown, m_curClickType);
					if (curTouchMove.MyCore ().m_type == ShipBuildType.BuildRoom
							|| selNewTouchMov != null && selNewTouchMov == curTouchMove) {
							curTouchMove.PlayEditSelectAnimationEnd ();//恢复上个房间内陷阱/角色 动画
							curTouchMove.OnMouseDown ();
							m_bTouchMoveChange = false;
					}
			}
			PutCanvasM.ClearNoLinkList ();
		} 
		else 
		{
			//判断选中物件是否在上次保存失败的物件上，如果是则直接选择
			bool bInNoLinkList=false;
			if(buildRoomCoreTouchDown!=null)
			{
				TouchMove selNewTouchMov=PutCanvasM.GetTouchMoveByCore(buildRoomCoreTouchDown);
				if(null==selNewTouchMov)
				{
					List<CanvasCore> childCores = m_roomGridTouchDown.GetOtherBuild();
					for (int i = 0; i < childCores.Count; i++) 
					{
						selNewTouchMov=PutCanvasM.GetTouchMoveByCore(childCores[i]);
						break;
					}
				}
				if(null!=selNewTouchMov&&PutCanvasM.NoLinkListContain(selNewTouchMov.MyCore()))
				{
					PutCanvasM.ClearNoLinkList ();
					TouchMoveManager.SetCurTouchMove(selNewTouchMov);
					selNewTouchMov.PlayEditSelectAnimationEnd ();//恢复上个房间内陷阱/角色 动画
					selNewTouchMov.OnMouseDown ();
					m_bTouchMoveChange = false;
					bInNoLinkList=true;
				}
			}
			if(bInNoLinkList==false)
				PutCanvasM.ClearNoLinkList ();
				
		}
    }

    void Start()
    {
		PutCanvasM.CanOperate = true;
       
    }

    void Update()
    {
        if (MainCameraM.s_Instance.CheckIsZooming() || MainCameraM.s_Instance.CheckCameraMove())
        {
			PutCanvasM.ShowRoomGridUI(false);
        }
		if (PutCanvasM.CanOperate == false)
        {
            return;
        }
        if (Input.touchCount >= 2)
        {
			PutCanvasM.ShowRoomGridUI(false);
            m_bTouchMoveChange = true;
            m_roomGridTouchDown = null;
        }
       // MainCameraM.s_Instance.EnableDrag(true);
        if (m_bMouseDown)
		{
			if(!m_isDrag)
			{
				float click  = mouseClickThreshold;
				click *= click;
				m_vtotalmouseDelta += m_vlastFirstTouch-GetTouchPos(0);
				m_vlastFirstTouch = GetTouchPos(0);
				m_mag = m_vtotalmouseDelta.sqrMagnitude;
				if ( m_mag > click) 
				{
					m_isDrag = true;
				}
				else
				{
					//没有发生移动地图 或 没有发生长按  要判定
					if (!m_isDrag && !m_bLongTouch)
					{
						//长按判定
						if (!m_isDrag && m_roomGridTouchDown != null)
						{
							if (Time.time - m_fMouseDownTime > 0.5f)
								m_bLongTouch = true;
							//没有发生移动地图，但有发生长按，则更换选中房间，并将m_bTouchMoveChange=false
							if (m_bLongTouch)
							{
								CanvasCore buildRoomCoreTouchDown = m_roomGridTouchDown.GetBuildRoom();
								TouchMove selNewTouchMov = PutCanvasM.GetTouchMoveByCore(buildRoomCoreTouchDown);
								TouchMove selOldeTouchMove = TouchMoveManager.GetCurTouchMove();
								if (selNewTouchMov != null && selNewTouchMov != selOldeTouchMove)
								{
									m_bTouchMoveChange = false;
									TouchMoveManager.SetCurTouchMove(selNewTouchMov);
									selNewTouchMov.OnMouseDown();
								}
								
							}
						}
					}
				}
			}
			if(m_isDrag)
			{
				//按下是选中的是原选中房间，则直接移动该房间
				//长按时会自动将最新选中房间，并将m_bTouchMoveChange设置为false
				if (!m_bTouchMoveChange)
				{
					MainCameraM.s_Instance.EnableDrag(false);
					TouchMove curTouchMove = TouchMoveManager.GetCurTouchMove();
					if (null != curTouchMove)
					{
						PutCanvasM.ShowRoomGridUI(false);
						//TouchMoveManager.ShowCanvas(true);
						PutCanvasM.ShowRoomGridUI(false);
						curTouchMove.MoveBuild();
					}
				}
			}
        }
    }

    void OnMouseUp()
    {
		if (!m_bMouseDown || PutCanvasM.CanOperate == false)
            return;
        m_bMouseDown = false;
        bool bCancelSel = false ;
//        MainCameraM.s_Instance.EnableDrag(true);
        TouchMove selNewTouchMov = null;
        if (TouchMove.g_bSetParaing == true)
        {
            SetPara();
            return;
        }
        if (m_bTouchMoveChange)
        {
            if (m_roomGridTouchDown != null)
            {
				bCancelSel = true;
				selNewTouchMov = PutCanvasM.GetTouchMoveByRoomGrid(m_roomGridTouchDown,m_curClickType);
				if(null==selNewTouchMov)
				{
					CanvasCore buildRoomCoreTouchDown = m_roomGridTouchDown.GetBuildRoom();	
					selNewTouchMov=PutCanvasM.GetTouchMoveByCore(buildRoomCoreTouchDown);
				}	
				if (selNewTouchMov)
                {
					CanvasCore selCanvasCore = selNewTouchMov.MyCore();
                    bCancelSel = false;
                    TouchMove selOldeTouchMove = TouchMoveManager.GetCurTouchMove();
                    selNewTouchMov.PlayEditSelectAnimation();//点选时播放房间内陷阱动画
					TouchMoveManager.SetCurTouchMove(selNewTouchMov);
					selNewTouchMov.MoveWithRoom(selCanvasCore);
				}
				if (bCancelSel)
                {
                    TouchMoveManager.SetCurTouchMove(null);
                }
            }
        }
        else
        {
            selNewTouchMov = TouchMoveManager.GetCurTouchMove();
        }
        if (selNewTouchMov)
        {
            if (selNewTouchMov.MoveBuildUp())
            {
				//TouchMoveManager.ShowCanvas(false);
				PutCanvasM.ShowRoomGridUI(true);
            }
			else 
			{
				PutCanvasM.ShowRoomGridUI(true);
			}
		}
        else
        {
            TouchMoveManager.SetCurTouchMove(null);
			//TouchMoveManager.ShowCanvas(false);
			PutCanvasM.ShowRoomGridUI(true);
        }

    }
    /// <summary>
    /// 设置参数（如传送门的传送点）
    /// </summary>
    void SetPara()
    {
        if (RoomMap.CheckRoomGridInBoat(m_roomGridTouchDown))
        {
            TouchMove tm = TouchMoveManager.GetCurTouchMove();
            if (tm)
            {
                Int2 paramPos = m_roomGridTouchDown.SoldierPos;
                CanvasCore buildRoomCoreTouchDown = m_roomGridTouchDown.GetBuildRoom();
                tm.SetBuildParam(buildRoomCoreTouchDown, paramPos.Unit, paramPos.Layer);
                tm.SetBuildParaOver();
                //传送门 这里没获取到
				
				BuildProperty lifeobj = tm.GetComponent<BuildProperty>();
				if(lifeobj!=null&&lifeobj.GetModeType()==1605)
				{
					Building1605 b1605 = lifeobj.GetLife() as Building1605;
					if (b1605!=null)
					{
						b1605.SetTransGate(paramPos);
					}
				}
            }
        }
        else
        {
			NGUIUtil.ShowTipWndByKey("88800013");
        }
        m_roomGridTouchDown = null;
    }

	
}
