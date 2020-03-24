using UnityEngine;
using DG.Tweening;

public class CameraGestureController : CameraController
{
    /// <summary>
    /// 相机是否处于运动状态
    /// </summary>
    public bool IsMove {
        get { return m_bAutoMove; }
    }
    
    //控制相关操作
    private bool m_bAutoMove = false;
    private bool m_bOthOn = false;
    private bool m_bEnableDrag = true;
    
    
    private bool m_bOneTouchDown = false;
    private Vector2 m_vTouchDown;	//记录手指按下记录的位置
    private Vector2 m_vtotalmouseDelta;	//记录手指按下后的屏幕移动总和，可用于判断移动和捏合操作
    private float m_mag;//移动的增量值
    
    //平移相关操作相关参数
    private Vector3 m_vToPosition = new Vector3(0, 0, 0);
    private bool m_isDrag = false;         //是否 在平移中
    private Vector3 m_vPanningDir;
    private float m_fPanningSpeed;
    
    //缩放相关参数
    private bool m_isZooming = false;         //是否在缩放中
    private float m_fzoomSpeed = 20.0f;     // 相机前后移动速度
    private float m_fzoomDis = 0.0f;
    private Vector2 m_vlastFirstTouch;     //设备上次触摸点1
    private Vector2 m_vlastSecondTouch;    //设备上次触摸点2
    
    private float m_DoubleTouchCurrDis = 0.0f;//当前双指触控间距
    private float m_DoubleTouchLastDis = 0.0f;//过去双指触控间距
    
    
    /// <summary>
    /// How much the mouse has to be moved after pressing a button before it starts to send out drag events.
    /// </summary>
    public float mouseDragThreshold = 4f;
    
    /// <summary>
    /// How far the mouse is allowed to move in pixels before it's no longer considered for click events, if the click notification is based on delta.
    /// </summary>
    public float mouseClickThreshold = 20f;
    
    /// <summary>
    /// How much the mouse has to be moved after pressing a button before it starts to send out drag events.
    /// </summary>
    public float touchDragThreshold = 40f;
    
    /// <summary>
    /// How far the touch is allowed to move in pixels before it's no longer considered for click events, if the click notification is based on delta.
    /// </summary>
    public float touchClickThreshold = 40f;
    
    public override void Start()
    {
        base.Start();
    }
    public bool IsEnableDrag {
        set{
            m_bEnableDrag = value;
        }
        get{
            return m_bEnableDrag;
        }
    }
    public bool IsOthOn {
        set{
            m_bOthOn = value;
            UpdateprojectionMatrix();
        }
        get{
            return m_bOthOn;
        }
    }
    private Vector2 GetTouchPos(int iTouchCnt)
    {
        Vector2 pos  = Vector3.zero;
        if (SystemInfo.deviceType == DeviceType.Desktop) {
            pos = Input.mousePosition;
        } else {
            pos = Input.GetTouch(iTouchCnt).position;
        }
        return pos;
    }
    
    private float GetZoomDis()
    {
        if (SystemInfo.deviceType == DeviceType.Desktop) {
            return Input.GetAxis("Mouse ScrollWheel");
        } else {
            if (Input.touchCount == 2) {
                if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved) {
                    m_DoubleTouchLastDis = m_DoubleTouchCurrDis;
                    m_DoubleTouchCurrDis = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
                    return (m_DoubleTouchCurrDis - m_DoubleTouchLastDis) * 0.02f;
                } else if (Input.GetTouch(0).phase == TouchPhase.Began && Input.GetTouch(1).phase == TouchPhase.Began) {
                    m_DoubleTouchCurrDis = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
                    m_DoubleTouchLastDis = m_DoubleTouchCurrDis;
                }
            } else {
                m_DoubleTouchCurrDis = 0.0f;
                m_DoubleTouchLastDis = 0.0f;
            }
        }
        return 0.0f;
    }
    
    
    public virtual void LateUpdate()
    {
        if (!IsEnableDrag) {
            CancelOp();
            return;
        }
        
        CheckState();
        CheckPanning();
        CheckZooming();
        
        limitPosition();
        
        UpdateprojectionMatrix();
        m_fzoomDis = GetZoomDis();
    }
    
    
    void CheckState()
    {
        m_fzoomDis = GetZoomDis();
        bool bZoomFlagOld = m_isZooming;
        bool bZoomFlagNew = m_fzoomDis != 0.0f;
        if (bZoomFlagOld && !bZoomFlagNew) {
            OnZoomBackAtion();
        }
        m_isZooming = bZoomFlagNew;
        if (!Input.GetMouseButton(0)) {
            if (m_bOneTouchDown) {
                OnDragOut();
                m_fPanningSpeed = 0;
                m_bOneTouchDown = false;
                
            }
            m_isDrag = false;
        } else {
            if (!m_bOneTouchDown) {
                RaycastHit hit;
                if (!WndManager.IsHitNGUI(out hit)) {
                    m_vTouchDown = GetTouchPos(0);
                    m_vlastFirstTouch = GetTouchPos(0);
                    m_vToPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
                    m_fPanningSpeed = 0f;
                    m_bOneTouchDown = true;
                    m_vtotalmouseDelta = Vector2.zero;
                }
            }
        }
        if (m_bOneTouchDown && !m_isDrag) {
            float click  = mouseClickThreshold;
            click *= click;
            m_vtotalmouseDelta += m_vlastFirstTouch - GetTouchPos(0);
            m_mag = m_vtotalmouseDelta.sqrMagnitude;
            if (m_mag > click) {
                m_vlastFirstTouch = m_vTouchDown;
                //m_vTouchDownDragOff = GetTouchPos(0)-m_vTouchDown;
                m_isDrag = true;
            }
        }
    }
    
    
    void CheckPanning()
    {
        if (m_isDrag) {
            if (m_vlastFirstTouch.x != GetTouchPos(0).x || m_vlastFirstTouch.y != GetTouchPos(0).y) {
                StopOpAction();
                
                Vector3 posMy = new Vector3(0.0f, 0.0f, 0.0f);
                Vector3 screenSpace = Camera.main.WorldToScreenPoint(posMy);
                Vector3 posWorldLastTemp =  Camera.main.ScreenToWorldPoint(new Vector3(m_vlastFirstTouch.x, m_vlastFirstTouch.y, screenSpace.z));
                Vector3 posWorldTempNow =  Camera.main.ScreenToWorldPoint(new Vector3(GetTouchPos(0).x, GetTouchPos(0).y, screenSpace.z));
                Vector3 p = posWorldTempNow - posWorldLastTemp;
                m_vToPosition = new Vector3(transform.localPosition.x - p.x, transform.localPosition.y - p.y, transform.localPosition.z);
                m_vPanningDir = (m_vToPosition - transform.localPosition).normalized;
                m_fPanningSpeed = Vector3.Distance(posWorldLastTemp, posWorldTempNow) / Time.deltaTime;
                if (m_fPanningSpeed > 0) {
                    transform.localPosition = transform.localPosition + m_vPanningDir * m_fPanningSpeed * 0.2f * Time.deltaTime;
                    m_vlastFirstTouch += (GetTouchPos(0) - m_vlastFirstTouch) * 0.2f;
                }
                
            } else {
                OnDragPause();
                m_vToPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
                m_vTouchDown = GetTouchPos(0);
                m_fPanningSpeed = 0;
                m_isDrag = false;
            }
        }
    }
    
    
    void CheckZooming()
    {
        if (m_isZooming) {
            StopOpAction();
            m_bEnableZoomBack = false;
            Vector3 move = new Vector3(0, 0, m_fzoomDis * m_fzoomSpeed);
            transform.Translate(move, Space.World);
            m_vToPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        }
    }
    
    
    public override void EnterControler()
    {
        CancelOp();
        this.enabled = true;
        m_vToPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
    }
    
    
    public override void LeaveControler()
    {
        CancelOp();
        this.enabled = false;
    }
    public void OnDragPause()
    {
        if (m_fPanningSpeed > 0) {
            StopOpAction();
            gameObject.transform.DOMove(m_vToPosition, 0.5f);
            m_vtotalmouseDelta = Vector2.zero;
        }
    }
    
    
    public void OnDragOut()
    {
        if (m_isDrag) {
            m_vToPosition = transform.localPosition + m_vPanningDir * m_fPanningSpeed * 0.1f;
            StopOpAction();
            gameObject.transform.DOMove(m_vToPosition, 1.0f);
        }
    }
    public void AutoMoveTo(Vector3 toPos, float animDuration = 0.5f)
    {
        CancelOp();
        m_bAutoMove = true;
        gameObject.transform.DOMove(toPos, animDuration);
    }
    public void OnAutoMoveEnd()
    {
        CancelOp();
        m_bAutoMove = false;
        m_vToPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
    }
    public void OnZoomBackAtion()
    {
        if (m_bEnableZoomBack && m_isZooming) {
            m_isZooming = false;
            float zLocal = m_areaLimitPyramid.m_vBottomLT.z;
            float xLocal = GetComponent<Camera>().transform.localPosition.x;
            float yLocal = GetComponent<Camera>().transform.localPosition.y;
            gameObject.transform.DOMove(new Vector3(xLocal, yLocal, zLocal), 0.5f);
        }
    }
    public void OnZoomBackEnd()
    {
        m_bEnableZoomBack = false;
    }
    public  override bool limitPosition()
    {
        if (!m_bAutoMove) {
            bool bResutl = base.limitPosition();
            return bResutl;
        }
        return true;
    }
    
    
    public void UpdateprojectionMatrix()
    {
        if (IsOthOn) {
            float fov = Camera.main.fieldOfView;
            float near = Camera.main.nearClipPlane;
            float far = Camera.main.farClipPlane;
            float halfFOV = (fov * 0.5f) * Mathf.Deg2Rad;
            float aspect = Camera.main.aspect;
            float orthographicSize = Mathf.Abs(Camera.main.gameObject.transform.position.z) * Mathf.Tan(halfFOV);
            GetComponent<Camera>().projectionMatrix = Matrix4x4.Ortho(-orthographicSize * aspect, orthographicSize * aspect, -orthographicSize, orthographicSize, near, far);
            GetComponent<Camera>().orthographic = true;
        } else {
            if (GetComponent<Camera>().orthographic) {
                float fov = Camera.main.fieldOfView;
                float near = Camera.main.nearClipPlane;
                float far = Camera.main.farClipPlane;
                float halfFOV = (fov * 0.5f) * Mathf.Deg2Rad;
                float aspect = Camera.main.aspect;
                GetComponent<Camera>().projectionMatrix = Matrix4x4.Perspective(fov, aspect, near, far);
                GetComponent<Camera>().orthographic = false;
            }
        }
    }
    public void CancelOp()
    {
        StopOpAction();
        m_isDrag = false;
        m_isZooming = false;
        m_bOneTouchDown = false;
        m_fPanningSpeed = 0;
    }
    public void StopOpAction()
    {
        m_bAutoMove = false;
    }
    public bool CheckIsZooming()
    {
        return m_isZooming;
    }
    public bool CheckIsDragging()
    {
        return m_isDrag;
    }
}