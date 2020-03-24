using UnityEngine;
using System.Collections;

public class FlyLimitZone
{
    public static float minX ;
    public static float maxX ;
    public static float minY ;
    public static float maxY ;
}

public class MainCameraM : MonoBehaviour
{

    public static MainCameraM s_Instance;
    /// <summary>
    /// 相机是否处于运动状态
    /// </summary>
    public bool IsMove {
        get
        {
            if (m_cameraGesture != null)
            {
                return m_cameraGesture.IsMove;
            } else
            {
                return false;
            }
        }
    }
    
    private CameraGestureController m_cameraGesture;
    
    
    static public  float s_fov = 8f;
    static public float s_aspect = 0.0f;
    
    //战斗场景专用
    public static Vector3 s_vBattleFarthestCamPos{ get; set;} //战斗最远透视相机位置
    public static Vector3 s_vBattleBoatviewCamPos{ get; set;} //战斗中大船满屏时透视相机位置
    
    //主场景专用场
    public static Vector3 s_vHavenViewFarthestCamPos{ get; set;} //避风港场景船体浏览最远透视相机位置
    public static Vector3 s_vHavenViewBoatviewCamPos{ get; set;} //避风港场景船体浏览大船满屏时透视相机位置
    public static AreaLimitPyramid s_reaLimitPyramidHavenView;//避风港相机限制移动区
    public static Vector3 s_vTreasureViewCamPos = new Vector3(0, 6, -121);
    
    //金银岛场景专用
    public static Vector3 m_vTreasureMainCameraPos = new Vector3(0f, 9.27f, -220.5f);
    
    //编辑场景专用
    public static Vector3 s_vEditViewBoatviewCamPos{ get; set;} //编辑船体浏览大船满屏时透视相机位置
    public static AreaLimitPyramid s_reaLimitPyramidBoatView;//编辑相机限制移动区
    public static AreaLimitPyramid s_reaLimitPyramidEditView;//编辑相机限制移动区
    
    void Awake()
    {
        s_Instance = this;
    }
    
    void Start()
    {
        s_aspect = gameObject.GetComponent<Camera>().aspect;
        CameraGestureController cgc = gameObject.GetComponent<CameraGestureController>();
        if (cgc != null) {
            m_cameraGesture = cgc;
        } else {
            m_cameraGesture = gameObject.AddComponent<CameraGestureController> ();
        }
        
        m_cameraGesture.enabled = true;
        m_cameraGesture.IsEnableDrag = true;
    }
    public void SetCameraGestureEnable(bool enable)
    {
        m_cameraGesture.enabled = enable;
    }
    //CameraGestureController拖动等外部相关调用接口
    public void UpdateprojectionMatrix()
    {
        m_cameraGesture.UpdateprojectionMatrix();
    }
    public void SetCameraLimitParam(AreaLimitPyramid areaLimitPyramid)
    {
        m_cameraGesture.SetCameraLimitParam(areaLimitPyramid);
    }
    
    public void EnableDrag(bool isEnable)
    {
        m_cameraGesture.IsEnableDrag = isEnable;
    }
    
    public void EnableOthOn(bool isOthOn)
    {
        m_cameraGesture.IsOthOn = isOthOn;
    }
    public void AutoMoveTo(Vector3 toPos, float animDuration = 0.5f)
    {
        CameraController nextContrl = m_cameraGesture;
        float distance = Vector3.Distance(toPos, Camera.main.transform.position);
        animDuration *= distance / (15.0f * 2f);
        m_cameraGesture.AutoMoveTo(toPos, animDuration);
    }
    
    public void AutoMoveTo(Transform toT, float duration = 0.5f)
    {
        Vector3 camPos = Camera.main.transform.position;
        Vector3 toPos = new Vector3(toT.position.x, camPos.y, camPos.z);
        AutoMoveTo(toPos, duration);
    }
    
    
    //避风港
    public void ResetCameraDataByHaven()
    {
        Camera cam = Camera.main;
        Vector3 posTemp = new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z);
        
        //主场景最大画面，目前定死
        //主场景场景最大画面时，0面占屏幕的纵横世界长度
        float zoomPyramid_S = -267.6354f;//锥体顶点
        float zPosBackGround = 0f; //最后面背景所在的位置
        cam.transform.position = new Vector3(0, 13.09499f, zoomPyramid_S);
        Vector3 posScreenCenterWorldB = cam.ViewportToWorldPoint(new Vector3(1.0f, 0f, zPosBackGround - zoomPyramid_S));
        Vector3 posScreenLeftWorldB = cam.ViewportToWorldPoint(new Vector3(0f, 0f, zPosBackGround - zoomPyramid_S));
        float fWidthBackGroundScreen = posScreenCenterWorldB.x - posScreenLeftWorldB.x;
        float zoomLimitMinX = - 86f + fWidthBackGroundScreen / 2f;
        float zoomLimitMaxX = 0;//28f-fWidthBackGroundScreen/2f;
        
        s_vHavenViewFarthestCamPos = new Vector3(-28.0f, 13.09499f, zoomPyramid_S);
        cam.transform.position = new Vector3(s_vHavenViewFarthestCamPos.x, s_vHavenViewFarthestCamPos.y, zoomPyramid_S);
        
        float zoomPyramid_O = -CalcCameraLimitZBySWH(9f, s_fov);
        s_reaLimitPyramidHavenView = s_reaLimitPyramidHavenView != null ? s_reaLimitPyramidHavenView : new AreaLimitPyramid();
        s_reaLimitPyramidHavenView.m_vTopL = new Vector3(zoomLimitMinX, s_vHavenViewFarthestCamPos.y, zoomPyramid_S);
        s_reaLimitPyramidHavenView.m_vTopR = new Vector3(zoomLimitMaxX, s_vHavenViewFarthestCamPos.y, zoomPyramid_S);
        cam.transform.position = new Vector3(zoomLimitMinX, s_vHavenViewFarthestCamPos.y, zoomPyramid_S);
        s_reaLimitPyramidHavenView.m_vBottomLT = cam.ViewportToWorldPoint(new Vector3(0, 1, zoomPyramid_O - zoomPyramid_S));
        s_reaLimitPyramidHavenView.m_vBottomLB = cam.ViewportToWorldPoint(new Vector3(0, 0, zoomPyramid_O - zoomPyramid_S));
        cam.transform.position = new Vector3(zoomLimitMaxX, s_vHavenViewFarthestCamPos.y, zoomPyramid_S);
        s_reaLimitPyramidHavenView.m_vBottomRT = cam.ViewportToWorldPoint(new Vector3(1, 1, zoomPyramid_O - zoomPyramid_S));
        s_reaLimitPyramidHavenView.m_vBottomRB = cam.ViewportToWorldPoint(new Vector3(1, 0, zoomPyramid_O - zoomPyramid_S));
        
        s_reaLimitPyramidHavenView.m_fZDeep = zoomPyramid_S;
        s_reaLimitPyramidHavenView.m_fxRightDeep = s_reaLimitPyramidHavenView.m_vBottomRT.x;
        s_reaLimitPyramidHavenView.m_fxLeftDeep = s_reaLimitPyramidHavenView.m_vBottomLT.x;
        s_reaLimitPyramidHavenView.m_fYBTopDeep = s_reaLimitPyramidHavenView.m_vBottomLT.y;
        s_reaLimitPyramidHavenView.m_fYBottomDeep = s_reaLimitPyramidHavenView.m_vBottomRB.y;
        
        //编辑船时最大场景画面
        Int2 Start = Int2.zero;
        ShipPlan p = ShipPlanDC.GetCurShipPlan();
        if (p != null) {
            ShipCanvasInfo Canvans = p.Canvans;
            Start = Canvans.GetMapSize();
        }
        float fZeroHeight = (Start.Layer + 2) * 3.3f * 12f / 7f; //船占高度的7/12
        float fZeroWidth = fZeroHeight * s_aspect;
        float fBoatHeight = (Start.Layer + 1f) * 3.3f; //船高
        float fBoatWidth = Start.Unit * 3.0f;//船宽
        if (fZeroWidth < (Start.Unit + 2) * 3.0f) {
            fZeroWidth = (Start.Unit + 2) * 3.0f;
            fZeroHeight = fZeroWidth / s_aspect;
        }
        float fMaxZeroHeight = fZeroHeight + fBoatHeight;//最大场景0面对应的高度
        float fMaxZeroWidth = fZeroWidth + fBoatWidth; //最大场景0面对应的高度
        zoomPyramid_S = -CalcCameraLimitZBySWH(fMaxZeroHeight, s_fov); //锥体顶点
        cam.transform.position = new Vector3(0, (fMaxZeroHeight - fZeroHeight) / 2.0f + 3f, zoomPyramid_S); //最大场景显示时的相机位置
        float zoomPyramid_H = -CalcCameraLimitZBySWH(fZeroHeight, s_fov);//锥体高点坐标
        zoomPyramid_O = -CalcCameraLimitZBySWH(9f, s_fov); //锥体底点坐标
        s_reaLimitPyramidBoatView = s_reaLimitPyramidBoatView != null ? s_reaLimitPyramidBoatView : new AreaLimitPyramid();
        s_reaLimitPyramidBoatView.m_vTopL = new Vector3(cam.transform.position.x, cam.transform.position.y, zoomPyramid_S);
        s_reaLimitPyramidBoatView.m_vTopR = new Vector3(cam.transform.position.x, cam.transform.position.y, zoomPyramid_S);
        s_reaLimitPyramidBoatView.m_vBottomLT = cam.ViewportToWorldPoint(new Vector3(0, 1, zoomPyramid_O - zoomPyramid_S));
        s_reaLimitPyramidBoatView.m_vBottomLB = cam.ViewportToWorldPoint(new Vector3(0, 0, zoomPyramid_O - zoomPyramid_S));
        s_reaLimitPyramidBoatView.m_vBottomRT = cam.ViewportToWorldPoint(new Vector3(1, 1, zoomPyramid_O - zoomPyramid_S));
        s_reaLimitPyramidBoatView.m_vBottomRB = cam.ViewportToWorldPoint(new Vector3(1, 0, zoomPyramid_O - zoomPyramid_S));
        s_reaLimitPyramidBoatView.m_fZDeep = zoomPyramid_H;
        s_reaLimitPyramidBoatView.m_fxRightDeep = fBoatWidth / 2f;
        s_reaLimitPyramidBoatView.m_fxLeftDeep = -fBoatWidth / 2f;
        s_reaLimitPyramidBoatView.m_fYBTopDeep = fBoatHeight + 3f;
        s_reaLimitPyramidBoatView.m_fYBottomDeep = 3f;
        
        float fBottom = fZeroHeight / 4f + 3.0f; //屏幕底往上1/4处为船底坐标位置
        s_vHavenViewBoatviewCamPos = new Vector3(0f, fBottom, zoomPyramid_H);
        
        Start = new Int2(8, 4);
        fZeroHeight = (Start.Layer + 2) * 3.3f * 12f / 7f; //船占高度的7/12
        fZeroWidth = fZeroHeight * s_aspect;
        fBoatHeight = (Start.Layer + 1f) * 3.3f; //船高
        fBoatWidth = Start.Unit * 3.0f;//船宽
        if (fZeroWidth < (Start.Unit + 2) * 3.0f) {
            fZeroWidth = (Start.Unit + 2) * 3.0f;
            fZeroHeight = fZeroWidth / s_aspect;
        }
        fMaxZeroHeight = fZeroHeight + fBoatHeight;//最大场景0面对应的高度
        fMaxZeroWidth = fZeroWidth + fBoatWidth; //最大场景0面对应的高度
        zoomPyramid_S = -CalcCameraLimitZBySWH(fMaxZeroHeight, s_fov); //锥体顶点
        cam.transform.position = new Vector3(0, (fMaxZeroHeight - fZeroHeight) / 2.0f + 3f, zoomPyramid_S); //最大场景显示时的相机位置
        zoomPyramid_H = -CalcCameraLimitZBySWH(fZeroHeight, s_fov);//锥体高点坐标
        zoomPyramid_O = -CalcCameraLimitZBySWH(9f, s_fov); //锥体底点坐标
        s_reaLimitPyramidEditView = s_reaLimitPyramidEditView != null ? s_reaLimitPyramidEditView : new AreaLimitPyramid();
        s_reaLimitPyramidEditView.m_vTopL = new Vector3(cam.transform.position.x, cam.transform.position.y, zoomPyramid_S);
        s_reaLimitPyramidEditView.m_vTopR = new Vector3(cam.transform.position.x, cam.transform.position.y, zoomPyramid_S);
        s_reaLimitPyramidEditView.m_vBottomLT = cam.ViewportToWorldPoint(new Vector3(0, 1, zoomPyramid_O - zoomPyramid_S));
        s_reaLimitPyramidEditView.m_vBottomLB = cam.ViewportToWorldPoint(new Vector3(0, 0, zoomPyramid_O - zoomPyramid_S));
        s_reaLimitPyramidEditView.m_vBottomRT = cam.ViewportToWorldPoint(new Vector3(1, 1, zoomPyramid_O - zoomPyramid_S));
        s_reaLimitPyramidEditView.m_vBottomRB = cam.ViewportToWorldPoint(new Vector3(1, 0, zoomPyramid_O - zoomPyramid_S));
        s_reaLimitPyramidEditView.m_fZDeep = zoomPyramid_H;
        s_reaLimitPyramidEditView.m_fxRightDeep = fBoatWidth / 2f;
        s_reaLimitPyramidEditView.m_fxLeftDeep = -fBoatWidth / 2f;
        s_reaLimitPyramidEditView.m_fYBTopDeep = fBoatHeight + 3f;
        s_reaLimitPyramidEditView.m_fYBottomDeep = 3f;
        
        fZeroHeight = (Start.Layer + 1) * 3.3f * 20f / 13f; //船占高度的7/12
        fZeroWidth = fZeroHeight * s_aspect;
        if (fZeroWidth < Start.Unit * 3.0f) {
            fZeroWidth = Start.Unit * 3.0f;
            fZeroHeight = fZeroWidth / s_aspect;
        }
        float zoomBoatviewZ = -CalcCameraLimitZBySWH(fZeroHeight, s_fov);
        fBottom = fZeroHeight / 4f + 3.0f; //屏幕底往上1/4处为船底坐标位置
        s_vEditViewBoatviewCamPos = new Vector3(0f, fBottom, zoomBoatviewZ);
        
        cam.transform.position = posTemp;
        
        ScreenManager.onScreenSizeChanged = OnScreenSizeChange;
    }
    public static Vector3 GetMainCameraPos(Int2 roomGrid)
    {
        Int2 Start = roomGrid;
        float zoomPyramid_S = -267.6354f;//锥体顶点
        float zPosBackGround = 0f; //最后面背景所在的位置
        
        float fZeroHeight = (Start.Layer + 2) * 3.3f * 12f / 7f; //船占高度的7/12
        float fZeroWidth = fZeroHeight * s_aspect;
        float fBoatHeight = (Start.Layer + 1f) * 3.3f; //船高
        float fBoatWidth = Start.Unit * 3.0f;//船宽
        if (fZeroWidth < (Start.Unit + 2) * 3.0f) {
            fZeroWidth = (Start.Unit + 2) * 3.0f;
            fZeroHeight = fZeroWidth / s_aspect;
        }
        float fMaxZeroHeight = fZeroHeight + fBoatHeight;//最大场景0面对应的高度
        float fMaxZeroWidth = fZeroWidth + fBoatWidth; //最大场景0面对应的高度
        zoomPyramid_S = -CalcCameraLimitZBySWH(fMaxZeroHeight, s_fov); //锥体顶点
        float zoomPyramid_H = -CalcCameraLimitZBySWH(fZeroHeight, s_fov);//锥体高点坐标
        
        
        float fBottom = fZeroHeight / 4f + 3.0f; //屏幕底往上1/4处为船底坐标位置
        
        
        fZeroHeight = (Start.Layer + 2) * 3.3f * 12f / 7f; //船占高度的7/12
        fZeroWidth = fZeroHeight * s_aspect;
        fBoatHeight = (Start.Layer + 1f) * 3.3f; //船高
        fBoatWidth = Start.Unit * 3.0f;//船宽
        if (fZeroWidth < (Start.Unit + 2) * 3.0f) {
            fZeroWidth = (Start.Unit + 2) * 3.0f;
            fZeroHeight = fZeroWidth / s_aspect;
        }
        fMaxZeroHeight = fZeroHeight + fBoatHeight;//最大场景0面对应的高度
        fMaxZeroWidth = fZeroWidth + fBoatWidth; //最大场景0面对应的高度
        zoomPyramid_S = -CalcCameraLimitZBySWH(fMaxZeroHeight, s_fov); //锥体顶点
        zoomPyramid_H = -CalcCameraLimitZBySWH(fZeroHeight, s_fov);//锥体高点坐标
        
        
        
        fZeroHeight = (Start.Layer + 1) * 3.3f * 20f / 13f; //船占高度的7/12
        fZeroWidth = fZeroHeight * s_aspect;
        if (fZeroWidth < Start.Unit * 3.0f) {
            fZeroWidth = Start.Unit * 3.0f;
            fZeroHeight = fZeroWidth / s_aspect;
        }
        float zoomBoatviewZ = -CalcCameraLimitZBySWH(fZeroHeight, s_fov);
        fBottom = fZeroHeight / 4f + 3.0f; //屏幕底往上1/4处为船底坐标位置
        
        return new Vector3(0f, fBottom, zoomBoatviewZ);
        
        
    }
    
    public static Vector3 GetDiffenceDisignCameraPos()
    {
        float OffsetX = 0f;
        Int2 grid = new Int2();
        StaticShipCanvas canva = ShipPlanDC.GetCurShipDesignInfo();
        if (canva != null) {
            grid.Layer = canva.Height;
            grid.Unit = canva.Width;
            OffsetX = (RoomMap.MaxUnit - canva.Width) / 2 * 3f;
        }
        Vector3 pos = GetMainCameraPos(grid);
        pos.x = -OffsetX;
        return	pos;
        
    }
    public void ResetCameraDataByBattle()
    {
        //场景最大画面为船高（船层+2）*3.3f占屏7/12时
        //场景最大画面时，0面占屏幕的纵横世界长度
        Camera cam = Camera.main;
        ShipCanvasInfo Info = CmCarbon.GetDefenseMap();
        Int2 Start = Info.GetMapSize();
        float fZeroHeight = (Start.Layer + 2) * 3.3f * 12f / 7f; //0面占屏幕高度
        float fZeroWidth = fZeroHeight * s_aspect;//0面占屏幕宽度
        float fBoatHeight = (Start.Layer + 1f) * 3.3f; //船高
        float fBoatWidth = Start.Unit * 3.0f;//船宽
        if (fZeroWidth < (Start.Unit + 2) * 3.0f) {
            fZeroWidth = (Start.Unit + 2) * 3.0f;
            fZeroHeight = fZeroWidth / s_aspect;
        }
        float fMaxZeroHeight = fZeroHeight + fBoatHeight;//最大场景0面对应的高度
        float fMaxZeroWidth = fZeroWidth + fBoatWidth; //最大场景0面对应的高度
        float zoomPyramid_S = -CalcCameraLimitZBySWH(fMaxZeroHeight, s_fov); //锥体顶点
        cam.transform.position = new Vector3(0, (fMaxZeroHeight - fZeroHeight) / 2.0f + 3f, zoomPyramid_S); //最大场景显示时的相机位置
        float zoomPyramid_H = -CalcCameraLimitZBySWH(fZeroHeight, s_fov);//锥体高点坐标
        float zoomPyramid_O = -CalcCameraLimitZBySWH(9f, s_fov); //锥体底点坐标
        AreaLimitPyramid reaLimitPyramid = new AreaLimitPyramid();
        reaLimitPyramid.m_vTopL = new Vector3(cam.transform.position.x, cam.transform.position.y, zoomPyramid_S);
        reaLimitPyramid.m_vTopR = new Vector3(cam.transform.position.x, cam.transform.position.y, zoomPyramid_S);
        reaLimitPyramid.m_vBottomLT = cam.ViewportToWorldPoint(new Vector3(0, 1, zoomPyramid_O - zoomPyramid_S));
        reaLimitPyramid.m_vBottomLB = cam.ViewportToWorldPoint(new Vector3(0, 0, zoomPyramid_O - zoomPyramid_S));
        reaLimitPyramid.m_vBottomRT = cam.ViewportToWorldPoint(new Vector3(1, 1, zoomPyramid_O - zoomPyramid_S));
        reaLimitPyramid.m_vBottomRB = cam.ViewportToWorldPoint(new Vector3(1, 0, zoomPyramid_O - zoomPyramid_S));
        
        reaLimitPyramid.m_fZDeep = zoomPyramid_H;
        reaLimitPyramid.m_fxRightDeep = fBoatWidth / 2f;
        reaLimitPyramid.m_fxLeftDeep = -fBoatWidth / 2f;
        reaLimitPyramid.m_fYBTopDeep = fBoatHeight + 3f;
        reaLimitPyramid.m_fYBottomDeep = 3;
        m_cameraGesture.SetCameraLimitParam(reaLimitPyramid);
        
        float fBottom = fZeroHeight / 4f + 3.0f; //屏幕底往上1/4处为船底坐标位置
        s_vBattleFarthestCamPos = new Vector3(0f, fBottom, zoomPyramid_H);
        
        
        fZeroHeight = (Start.Layer + 1) * 3.3f * 20f / 13f; //船占高度的7/12
        fZeroWidth = fZeroHeight * s_aspect;
        if (fZeroWidth < Start.Unit * 3.0f) {
            fZeroWidth = Start.Unit * 3.0f;
            fZeroHeight = fZeroWidth / s_aspect;
        }
        float zoomBoatviewZ = -CalcCameraLimitZBySWH(fZeroHeight, s_fov);
        fBottom = fZeroHeight / 4f + 3.0f; //屏幕底往上1/4处为船底坐标位置
        s_vBattleBoatviewCamPos = new Vector3(0f, fBottom, zoomBoatviewZ);
        cam.transform.position = new Vector3(s_vBattleFarthestCamPos.x, s_vBattleFarthestCamPos.y, s_vBattleFarthestCamPos.z);
        //ScreenManager.onScreenSizeChanged = OnScreenSizeChange;
        FlyZone();
        if (CmCarbon.GetCamp2Player(LifeMCamp.ATTACK) == false) {
            NGUIUtil.DebugLog("被怪物打");
        } else {
            MainCameraM.s_Instance.AutoMoveTo(MainCameraM.s_vBattleBoatviewCamPos, 0.7f);
        }
    }
    
    
    
    void OnScreenSizeChange(Vector2 newScreenSize)
    {
        Debug.Log("OnScreenSizeChange");
        FlyZone();
    }
    
    /// <summary>
    /// 获取飞行限定区域值
    /// </summary>
    public void FlyZone()
    {
        Camera cam = Camera.main;
        
        Vector3 vView = cam.WorldToViewportPoint(new Vector3(0, 0, -cam.transform.position.z));
        //Debug.Log(vView.z);
        Vector3 topLeft = cam.ViewportToWorldPoint(new Vector3(0, 1, vView.z));
        Vector3 bottomRight = cam.ViewportToWorldPoint(new Vector3(1, 0, vView.z));
        
        FlyLimitZone.minX = topLeft.x;
        FlyLimitZone.maxX = bottomRight.x;
        FlyLimitZone.minY = 0f;//临时
        FlyLimitZone.maxY = topLeft.y;
        
        //Debug.Log ("获取飞行区域");
    }
    
    
    
    void OnDrawGizmosSelected()
    {
        Camera cam = Camera.main;
        
        Vector3 topLeft = cam.ViewportToWorldPoint(new Vector3(0, 1, cam.fieldOfView));
        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.fieldOfView));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.fieldOfView));
        Vector3 bottomRight = cam.ViewportToWorldPoint(new Vector3(1, 0, cam.fieldOfView));
#if UNITY_EDITOR
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(topLeft, 0.3f);
        Gizmos.DrawSphere(bottomLeft, 0.3f);
        Gizmos.DrawSphere(topRight, 0.3f);
        Gizmos.DrawSphere(bottomRight, 0.3f);
#endif
    }
    
    
    
    #region 根据宽度计算所需要的限定 Z 值
    public static float CalcCameraLimitZBySWW(float fZeroWidth, float fieldOfView, float aspect)
    {
        float halfFOV = (fieldOfView * 0.5f) * Mathf.Deg2Rad;
        float height = fZeroWidth / 2 / aspect;
        return height / Mathf.Tan(halfFOV);
    }
    public static float CalcCameraLimitZBySWH(float fZeroHeight, float fieldOfView)
    {
        fZeroHeight = fZeroHeight / 2;
        float halfFOV = (fieldOfView * 0.5f) * Mathf.Deg2Rad;
        return fZeroHeight / Mathf.Tan(halfFOV);
    }
    #endregion
    
    /// <summary>
    /// 屏幕坐标转世界坐标
    /// </summary>
    public static Vector3 ScreenToWorldPos(Vector3 ScreenPos)
    {
        if (s_Instance == null || s_Instance.GetComponent<Camera>() == null) {
            return Vector3.zero;
        }
        Camera camera = s_Instance.GetComponent<Camera>() ;
        
        Vector3 v  = camera.WorldToScreenPoint(Vector3.zero);
        Vector3 posWorld =  camera.ScreenToWorldPoint(new Vector3(ScreenPos.x, ScreenPos.y, v.z));
        if (posWorld.y < 0f) {
            posWorld = new Vector3(posWorld.x, 0.0f, posWorld.z);
        }
        return posWorld;
    }
    
    public  bool CheckIsZooming()
    {
        return m_cameraGesture.CheckIsZooming();
    }
    
    public bool CheckCameraMove()
    {
        return m_cameraGesture.CheckIsDragging();
    }
    
    
}
