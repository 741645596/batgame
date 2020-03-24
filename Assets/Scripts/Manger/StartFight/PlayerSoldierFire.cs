
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#define UNITY_EDITOR_LOG
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic;


/// <summary>
/// 玩家发射炮弹兵
/// </summary>
/// <author>zhulin</author>
public class PlayerSoldierFire : MonoBehaviour
{
	/// <summary>
	/// 触摸点列表
	/// </summary>
	protected List<Vector3> m_v3TouchPostions = new List<Vector3>();
    /// <summary>
    /// 用来控制是否显示手势轨迹
    /// </summary>
    public static bool isTrace = false;

	public static int soldierDataID = -1;
    /// <summary>
    /// 手势拖尾
    /// </summary>
    private GameObject m_goFingerTrail;
    /// <summary>
    /// 是否允许发射
    /// </summary>
    public static bool s_bEnableFire = true;
    public static bool s_bCollectTouchPoint;

	private bool isPanning = false;         //是否 在平移中
	private Vector3 mouseLast;        //鼠标按下前的位置
	private Vector3 mousePosition ;

	private Vector3 m_vtotalmouseDelta;	//记录手指按下后的屏幕移动总和，可用于判断移动和捏合操作
	
	void Update()
	{
		CheckState();
		CheckPanning();
	}

	void CheckState()
	{
		//前后Pinch 缩放镜头
		if (SystemInfo.deviceType == DeviceType.Desktop)
		{ 
			if (!Input.GetMouseButton(0))
			{
				if(isPanning)
				{
					m_vtotalmouseDelta += mouseLast-Input.mousePosition;
					FingerUp(MainCameraM.ScreenToWorldPos(Input.mousePosition));
				}
				isPanning = false;
			}
			else
			{
				if (!isPanning) 
				{
					mouseLast = Input.mousePosition;
					FingerStart(MainCameraM.ScreenToWorldPos(Input.mousePosition));
					isPanning = true;
					m_vtotalmouseDelta= Vector3.zero;
				}
			}
		}
		else
		{
			if(Input.touchCount == 1)
			{
				if (!isPanning) 
				{
					isPanning = true;
					FingerStart(MainCameraM.ScreenToWorldPos(Input.GetTouch(0).position));
					mouseLast = Input.mousePosition;
					m_vtotalmouseDelta= Vector3.zero;
				}
			}
			else
			{
				if(isPanning)
				{
					m_vtotalmouseDelta += mouseLast-Input.mousePosition;
					FingerUp(MainCameraM.ScreenToWorldPos(Input.mousePosition));
				}
				isPanning = false;
			}
		}
	}

	void CheckPanning()
	{
		if (isPanning)
		{
			if (SystemInfo.deviceType == DeviceType.Desktop)
				mousePosition = Input.mousePosition;
			else mousePosition = Input.GetTouch(0).position;

			if(Mathf.Abs(mouseLast.x-mousePosition.x)>15f || Mathf.Abs(mouseLast.y-mousePosition.y)>15f)
			{
				FingerMove(MainCameraM.ScreenToWorldPos(mousePosition));
				m_vtotalmouseDelta += mouseLast-mousePosition;
				mouseLast = mousePosition;
			}
		}
	}
    /// <summary>
    /// 手势开始
    /// </summary>
    /// <param name="gesture"></param>
    public  void FingerStart(Vector3 posWorld)
    {
        if (s_bEnableFire == false)
        {
            return;
        }
		m_v3TouchPostions.Clear ();
		if (CmCarbon.IsBorn (soldierDataID))
			return;
		SpawnFingerTrail(posWorld);
		PlayerSoldierFire.s_bCollectTouchPoint = true;
		MainCameraM.s_Instance.EnableDrag (false);

		if(GenerateShip.pointInRejectPolygon(posWorld,GenerateShip.GetRejectPolygon()))
        {
			PlayerSoldierFire.s_bCollectTouchPoint = false;
			MainCameraM.s_Instance.EnableDrag (true);
            return;
        }
		m_v3TouchPostions.Add(posWorld);
	}

	/// <summary>
    /// 手势移动中
    /// </summary>
    /// <param name="gesture"></param>
	public  void FingerMove(Vector3 posWorld)
	{
        if (s_bEnableFire == false)
        {
            return;
        }
		if (!PlayerSoldierFire.s_bCollectTouchPoint)
					return;
		if (CmCarbon.IsBorn (soldierDataID))
			return;
		int nTouchCount = m_v3TouchPostions.Count;
		if (nTouchCount > 0)
        {
            Vector3 oldPos = m_v3TouchPostions[m_v3TouchPostions.Count - 1];
			if (NdUtil.V3Equal(posWorld, oldPos))
                return;
		}
		if (isTrace && m_goFingerTrail) {
			m_goFingerTrail.transform.position = posWorld;
		}

		if (NdUtil.V3Equal(posWorld, Vector3.zero))
		{
            return;
		}
		m_v3TouchPostions.Add(posWorld);
    }
    /// <summary>
    /// 手指抬起
    /// </summary>
    /// <param name="gesture"></param>
	public  void FingerUp(Vector3 posWorld)
	{
        if (s_bEnableFire == false)
        {
            return;
        }
		if (!PlayerSoldierFire.s_bCollectTouchPoint)
						return;
		PlayerSoldierFire.s_bCollectTouchPoint = false;
		MainCameraM.s_Instance.EnableDrag (true);
		m_v3TouchPostions.Add(posWorld);
		if (!SoldierFire.CheckFlyLine(ref m_v3TouchPostions))
        {
            return;
		}
		float click  = 40f;
		click *= click;
		Vector2 v2totalmouseDelta=m_vtotalmouseDelta;
		if ( v2totalmouseDelta.sqrMagnitude <= click) 
		{
			return;
		}
        //兵已全部发射出去
        if (CmCarbon.IsAllFireOut())
		{
            return;
		}
		SoldierInfo soldierInfo = CmCarbon.GetSoldierInfo(LifeMCamp.ATTACK,soldierDataID);
		if(SoldierFire.Fire(m_v3TouchPostions ,soldierDataID ,soldierInfo ,true) == true)
		{
			CmCarbon.SetBorn(soldierDataID);
			CombatWnd Wnd = WndManager.FindDialog<CombatWnd>();
			if(Wnd != null)
			{
				Wnd.FireLater(soldierDataID);
				Wnd.ResetUIScale();
				//Wnd.AutoSelectNextSoldier();
			}
		}
		if ( CmCarbon.IsAllFireOut()) 
		{
			MainCameraM.s_Instance.AutoMoveTo(MainCameraM.s_vBattleBoatviewCamPos);
		}
	}
     /// <summary>
     /// 添加手势轨迹效果
     /// </summary>
	void SpawnFingerTrail(Vector3 posWorld)
	{
        if (s_bEnableFire == false)
        {
            return;
        }
         if (m_goFingerTrail)
         {
             Destroy(m_goFingerTrail);
         }
         m_goFingerTrail = GameObjectLoader.LoadPath("effect/prefab/", "fingerTrail");
		 m_goFingerTrail.transform.position = posWorld;
     }

}
