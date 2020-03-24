using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 拼船数据管理
/// </summary>
/// <author>zhulin</author>
public class TouchMoveManager  {

	/// <summary>
	/// 管理船只摆设数据。
	/// </summary>
	public  static List<TouchMove> m_Build = new List<TouchMove>();
	private static Dictionary<Int2 ,CanvasUnitBG> m_CanvanUnitBG = new Dictionary<Int2 ,CanvasUnitBG>();
	private static TouchMove m_SelTouch = null;
    private static TouchMove m_PreTouch = null;

	/// <summary>
	/// 清理船只摆设数据
	/// </summary>
	public static void ClearShipBuild()
	{
		m_Build.Clear();
		m_CanvanUnitBG.Clear();
	}
	/// <summary>
	/// 新增船只建筑，兵，加入。
	/// </summary>
	public static void JointShipBuild(TouchMove touch )
	{
		if(touch == null || touch.MyCore() == null)
		{
			Debug.LogError("船只摆设数据加入有误，请调查");
			return ;
		}

		if(CheckHaveBuild(touch.MyCore()) == false)
		{
			m_Build.Add(touch);
			if(!touch.MyCore().IsNewCreate)
			{
				PutCanvasM.AddNewCore(touch.MyCore(), touch.m_posMapGrid);
			}
		}
	}

    public static CanvasUnitBG GetCanvasUnitBG(Int2 key)
    {
        if (m_CanvanUnitBG.ContainsKey(key))
        {
            return m_CanvanUnitBG[key];
        }
        return null;
    }

	/// <summary>
	/// 检查是否已建
	/// </summary>
	public static  bool CheckHaveBuild(CanvasCore  Core )
	{
		if( Core == null)  return false;
		for(int i = 0; i < m_Build.Count; i++)
		{
			if(m_Build[i].MyCore().Equal(Core) == true)
			{
				return true;
			}
		}
		return false;
	}

    public static void HideDeckCanvasUnit()
    {
        foreach (var item in m_CanvanUnitBG)
        {
            if (item.Key.Layer == 4)
            {
                CanvasUnitBG bg = item.Value;
                bg.SetCanvasUnitState(CanvasUnitState.Normal);
            }
        }
    }
    /// <summary>
    /// 删除房间或保存时修正传送门传送点
    /// </summary>
    public static void DoTransgatePoint()
    {
        //获取所有传送门
        foreach (var canvas in m_Build)
        {
			CanvasCore core = canvas.MyCore();
            if (core.Data.IsTransgateRoom())
            {
                Int2 transgateSendGrid = new Int2(core.Data.shipput_data0, core.Data.shipput_data1);
                transgateSendGrid.Unit -= 4;
                if (CheckTransgatePoint(transgateSendGrid) == false)
                {
                    core.Data.shipput_data0 = core.Data.cxMapGrid + 4;
                    core.Data.shipput_data1 = core.Data.cyMapGrid;
                }
            }
        }
    }
    /// <summary>
    /// 检测传送门的传送点是否合法
    /// </summary>
    /// <returns></returns>
    static bool CheckTransgatePoint(Int2 grid)
    {        
        RoomGrid roomGrid = RoomMap.FindRoomGrid(grid, XYSYS.MapGrid);
        return RoomMap.CheckRoomGridInBoat(roomGrid);
    }



	/// <summary>
	/// 删除本地对象。
	/// </summary>
	public static void DeleteShipBuild(CanvasCore  Core)
	{
		if( Core == null)  return ;
		List<CanvasCore> l = PutCanvasM.GetChildByCore(Core);
		foreach (CanvasCore c in l)
        {
            for (int i = 0; i < m_Build.Count; i++)
            {
                if (m_Build[i].MyCore().Equal(c) == true)
                {
                    m_Build.RemoveAt(i);
                }
            }
        }

        for (int i = 0; i < m_Build.Count; i++)
        {
			if (m_Build[i].MyCore().Equal(Core) == true)
            {
                m_Build.RemoveAt(i);
            }
        }
	}

    /// <summary>
    /// 一键清除
    /// </summary>
    public static void DeleteAll()
    {
        for (int i = m_Build.Count-1; i >=0; i--)
        {
            if (m_Build[i].MyCore().Data.IsGoldBuild())
            {
                continue;
            }
            m_Build[i].DestroyShipBuild();
        }
    }


	/// <summary>
	/// 获取指定对象。
	/// </summary>
	public static TouchMove GetShipBuild(CanvasCore  Core)
	{
		if( Core == null)  return null;
		for(int i = 0; i < m_Build.Count; i++)
		{
			if(m_Build[i].MyCore().Equal(Core) == true)
			{
				return m_Build[i];
			}
		}
		return null;
	}

    public static List<TouchMove> GetAllTouchMoves()
    {
        List<TouchMove> tmList = new List<TouchMove>();
        for (int i = 0; i < m_Build.Count; i++)
        {
			TouchMove tm = m_Build[i];
            if (tm)
            {
                tmList.Add(tm);
            }
        }
        return tmList;
    }

	/// <summary>
	/// 设置移动到船上
	/// </summary>
	public static void MoveToShip(CanvasCore  Core)
	{
		TouchMove tm = GetShipBuild(Core);
		if(tm != null) 
			tm.MyCore().IsNewCreate = false;
	}

	/// <summary>
	/// 状态背景加入管理
	/// </summary>
	public static void JoinCanvasUnitBk(Int2 pos, CanvasUnitBG bk )
	{
		if(/*RGrid == null ||*/ bk == null)
		{
			return ;
		}
		if(m_CanvanUnitBG.ContainsKey(pos) == false)
		{
			m_CanvanUnitBG.Add(pos ,bk);
		}
	}
    /// <summary>
    /// 画布显示控制
    /// </summary>
    public static void ShowCanvas(bool isShow)
	{
		int nLayerTop = RoomMap.DeckRoomTopLayer;
		//if (isShow==true&&curTouchCore!=null&&curTouchCore.IsDeckRoom == false)
		//		nLayerTop = RoomMap.DeckTopLayer - 1;
        foreach (var item in m_CanvanUnitBG)
        {
			if (item.Key.Layer<0||item.Key.Unit<0
			    ||item.Key.Layer>nLayerTop||item.Key.Unit>=RoomMap.RealMapSize.Unit)
            {
                continue;
			}
			
			CanvasUnitBG bk = item.Value;
            if (bk==null)
            {
                return;
            }
            bk.Fade(isShow, 0.1f,true);
        }
    }
    /// <summary>
    /// 设定格子背景
    /// </summary>
	public static void SetGridBgState(Int2 grid ,CanvasUnitState state,bool bClearAllBgNorml)
	{
        if (m_CanvanUnitBG.ContainsKey(grid) == true)
        {
            CanvasUnitBG bk = m_CanvanUnitBG[grid];
            if (bk != null)
            {
                bk.SetCanvasUnitState(state);
            }
        }
    }

	/// <summary>
	/// 设置全部背景状态 
	/// </summary>
	public static void SetGridsBgStates(List<Int2> l, CanvasUnitState state,bool bClearAllBgNorml)
	{
		if(bClearAllBgNorml)
			SetAllGridBgStates(CanvasUnitState.Normal);
		for(int i = 0; i < l.Count; i++)
		{
			if(m_CanvanUnitBG.ContainsKey(l[i]) == true)
			{
				CanvasUnitBG bk = m_CanvanUnitBG[l[i]];
				if(bk != null)
				{
					bk.SetCanvasUnitState(state);
				}
			}
		}
	}

	/// <summary>
	/// 设置全部背景状态(目前未被使用)
	/// </summary>
	public static void SetAllGridBgStates(Int2 l, CanvasUnitState state,bool bClearAllBgNorml)
	{
		if(bClearAllBgNorml)
			SetAllGridBgStates(CanvasUnitState.Normal);
		if(m_CanvanUnitBG.ContainsKey(l) == true)
		{
			CanvasUnitBG bk = m_CanvanUnitBG[l];
			if(bk != null)
			{
				bk.SetCanvasUnitState(state);
			}
		}
	}

	/// <summary>
	/// 设置所有背景状态
	/// </summary>
	public static void SetAllGridBgStates(CanvasUnitState state)
	{
		foreach(CanvasUnitBG bk in m_CanvanUnitBG.Values)
		{
			if(bk != null)
			{
				bk.SetCanvasUnitState(state);
			}
		}
	}
    /// <summary>
    /// 设置可放置区域颜色
    /// </summary>
	public static void SetCanPutArea(ShipBuildType type,bool bDeckRoom)
	{
		List<Int2> l = RoomMap.GetCanPutArea(type,bDeckRoom);
		SetGridsBgStates(l, CanvasUnitState.CanPut,true);
	}

	/// <summary>
	/// 判断是否已经存在了。用于检测有没被创建。
	/// </summary>
	/// <param name="soldierID"></param>
	/// <returns></returns>
	public static bool CheckHaveExist(ShipBuildType type ,int ID)
	{
		for(int i = 0; i < m_Build.Count; i++)
		{
			if(m_Build[i].MyCore().m_DataID == ID && m_Build[i].MyCore().m_type == type)
			{
				return true;
				//Debug.Log("xxxxxxxxxxxx");
			}
		}
		return false;
	}
	/// <summary>
	/// 设置当前选中对象
	/// </summary>
	public static void SetCurTouchMove(TouchMove touch)
    {
        if (null != m_SelTouch)
        {
            if (touch!=m_SelTouch)
            {
                //m_SelTouch.ResetPos();
                //m_SelTouch.ShowMoveArrow(false);
                m_SelTouch.ShowTrapRoomUI(false);
                m_SelTouch.DestroyAttackRange();
                m_SelTouch.UnLoadEditEffect();
                //m_SelTouch.LeaveRoom();
            }
        }
        if (touch != null)
        {
			touch.TweenColor();
			touch.LoadEditEffect(TouchMoveState.CanEdit);
			// touch.ShowMoveArrow(true);
        }
        if (touch != m_SelTouch)
        {
            if (m_SelTouch)
            {
                m_PreTouch = m_SelTouch;
                m_SelTouch.StopTweenColor();
            }
        }
       
		m_SelTouch = touch;
	}
	/// <summary>
	/// 获取当前选中对象。
	/// </summary>
	public static TouchMove GetCurTouchMove( )
	{
		return m_SelTouch ;
	}
	/// <summary>
	/// 获取建筑对应的gird位置.
	/// </summary>
	/// <returns>The all build grid position.</returns>
	public static List<TouchMove> GetAllBuildOutShape()
	{
		List<TouchMove> tmList = new List<TouchMove>();
		for (int i = 0; i < m_Build.Count; i++)
		{
			TouchMove tm = m_Build[i];
			if (tm && tm.MyCore() != null && tm.MyCore().m_type != ShipBuildType.BuildStair)
			{
				List<Int2> l = tm.MyCore().GetPutRoomGridPos();
				foreach(Int2 item in l)
				{
					if(item.Layer >= RoomMap.RealMapSize.Layer || item.Unit >= RoomMap.RealMapSize.Unit)
					{
						tmList.Add(tm);
					}
				}

			}
		}
		return tmList;
	}

	public static List<TouchMove> GetDeckBuildListInMaxLayer()
	{
		List<TouchMove> tmList = new List<TouchMove>();
		for (int i = 0; i < m_Build.Count; i++)
		{
			TouchMove tm = m_Build[i];
			if (tm && tm.MyCore() != null && tm.MyCore().m_type == ShipBuildType.BuildRoom
			    && !tm.MyCore().IsDeckRoom)
			{
				List<Int2> l = tm.MyCore().GetPutRoomGridPos();
				foreach(Int2 item in l)
				{
					if(item.Layer == RoomMap.RealMapSize.Layer -1)
					{
						tmList.Add(tm);
					}
				}
				
			}
		}
		return tmList;
	}
}


