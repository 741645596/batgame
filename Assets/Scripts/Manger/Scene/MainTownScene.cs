using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 功能模块
/// </summary>
public enum FunBlock {
    None           = 0,
    Boat           = 1,        //船只编辑
    Stage          = 2,        //战役
}



public class MainTownScene : IScene
{
    public  static string GetSceneName()
    {
        return "MainTown";
    }
    /// <summary>
    /// 资源载入入口
    /// </summary>
    //private AsyncOperation async;
    public override IEnumerator Load()
    {
        async = Application.LoadLevelAsync(MainTownScene.GetSceneName());
        return null;
    }
    
    
    private bool m_bShowFrutionOnce = false;
    
    /// <summary>
    /// 准备载入场景
    /// </summary>
    public override void PrepareLoad() {}
    /// <summary>
    /// 资源卸载
    /// </summary>
    public override void Clear()
    {
        WndManager.DestroyAllDialogs();
    }
    
    
    /// <summary>
    /// 是否已经载入完成
    /// </summary>
    public override bool IsEnd()
    {
        if (async != null) {
            return async.isDone;
        } else {
            return false;
        }
    }
    /// <summary>
    /// 构建UI
    /// </summary>
    public override void BuildUI()
    {
        ShowMainTownUI();
    }
    /// <summary>
    /// 构建世界空间
    /// </summary>
    public override void BuildWorld()
    {
        Life.Environment = LifeEnvironment.View;
        SoundPlay.PlayBackGroundSound("bgm_city_loop", true, true);
        MainCameraM.s_Instance.ResetCameraDataByHaven();
        MainCameraM.s_Instance.SetCameraLimitParam(MainCameraM.s_reaLimitPyramidHavenView);
        RevertFrCamPosTemp(false);
        MainCameraM.s_Instance.EnableDrag(true);
    }
    /// <summary>
    /// 执行功能模块事件
    /// </summary>
    public void DoFunBlock(FunBlock block)
    {
        BlockObj obj = FindFunBlockObj(block);
        if (obj == null) {
            return ;
        }
        obj.OnClick();
    }
    /// <summary>
    /// 查找功能节点
    /// </summary>
    private BlockObj FindFunBlockObj(FunBlock block)
    {
        List<SceneObj> l  =  GetAllSceneObj();
        foreach (SceneObj s in l) {
            if (CheckFunBlock(s, block) == true) {
                return s as BlockObj;
            }
        }
        return null;
    }
    
    /// <summary>
    /// 确认是否为该功能模块
    /// </summary>
    private bool CheckFunBlock(SceneObj obj, FunBlock block)
    {
        if (obj == null || block == FunBlock.None) {
            return false;
        }
        
        if (block == FunBlock.Boat && obj is BoatObj) {
            return true;
        }
        if (block == FunBlock.Stage && obj is StageObj) {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 场景start 接口
    /// </summary>
    public override void Start()
    {
        switch (MainTownInit.s_currentState) {
            case MainTownState.StageMap:
                DoFunBlock(FunBlock.Stage);
                MainTownInit.s_currentState = MainTownState.None;
                break;
            case MainTownState.MainMenuPdbbb:
                WndManager.GetDialog<PdbbbWnd>();
                MainTownInit.s_currentState = MainTownState.None;
                break;
            case MainTownState.CanvasEdit:
                DoFunBlock(FunBlock.Boat);
                MainTownInit.s_currentState = MainTownState.None;
                break;
        }
        
        RestoreScene();
        OpenSceneFunc();
    }
    
    /// <summary>
    /// 预打开某项功能
    /// </summary>
    private void OpenSceneFunc()
    {
    
    }
    
    /// <summary>
    /// 还原场景现场
    /// </summary>
    private void RestoreScene()
    {
    
    }
    /// <summary>
    /// 接管场景中关注对象的Update
    /// </summary>
    public override void Update(float deltaTime)
    {
        if (null != m_objTouchDown) {
            if (Input.touchCount > 1f) {
                m_objTouchDown = null;
            } else {
                Vector2 vMovePos;
                if (SystemInfo.deviceType == DeviceType.Desktop) {
                    vMovePos = Input.mousePosition;
                } else {
                    vMovePos = Input.GetTouch(0).position;
                }
                m_vtotalmouseDelta += m_vposLastTouchMove - vMovePos;
                bool bMouseMove = false;
                if (SystemInfo.deviceType == DeviceType.Desktop) {
                    bMouseMove = true;
                }
                float click = bMouseMove ? 10f : 40f;
                click *= click;
                if (m_vtotalmouseDelta.sqrMagnitude > click) {
                    m_objTouchDown = null;
                }
            }
        }
        DoNewFrutionShow();
        DoShowShopEffect();
    }
    /// <summary>
    /// 接管场景中关注对象的LateUpdate
    /// </summary>
    public override void LateUpdate(float deltaTime)
    {
    
    }
    /// <summary>
    /// 接管场景中关注对象的FixedUpdate
    /// </summary>
    public override void FixedUpdate(float deltaTime)
    {
    
    }
    
    private SceneObj m_objTouchDown;
    private Vector2 m_vposLastTouchMove;
    private Vector2 m_vtotalmouseDelta;	//记录手指按下后的屏幕移动总和，可用于判断移动和捏合操作
    public override void OnMouseDown(SceneObj objScene)
    {
        RaycastHit hit;
        if (!WndManager.IsHitNGUI(out hit)) {
            m_vtotalmouseDelta = Vector3.zero;
            m_objTouchDown = objScene;
            if (SystemInfo.deviceType == DeviceType.Desktop) {
                m_vposLastTouchMove = Input.mousePosition;
            } else {
                m_vposLastTouchMove = Input.GetTouch(0).position;
            }
        }
    }
    
    public override void OnMouseUp(SceneObj objScene)
    {
        if (m_objTouchDown == objScene) {
            OnClick(m_objTouchDown);
        }
        m_objTouchDown = null;
    }
    
    public void OnClick(SceneObj objScene)
    {
        if (objScene is BlockObj) {
            BlockObj bobj = objScene as BlockObj;
            bobj.OnClick();
        }
    }
    
    void ShowMainTownUI()
    {
    }
    
    void HideMainTownUI()
    {
    }
    /// <summary>
    /// 执行新成就检测和表现
    /// </summary>
    void DoNewFrutionShow()
    {
    }
    /// <summary>
    /// 执行 地精、黑市 商店打开 表现
    /// </summary>
    void DoShowShopEffect()
    {
    
    }
    
    /// <summary>
    /// 检测主场景UI是否处于可以做表现的清空状态
    /// </summary>
    bool CheckUIClear()
    {
        if (WndManager.GetCurWndsCount() == ConstantData.MainTownClearWndCount) {
            return true;
        } else {
            return false;
        }
    }
    
    public static bool s_bTempCameraPos = false;//避风港场景跳到编辑场景后缓存的
    public static Vector3 s_vCamPosTemp { get; set; }//避风港场景跳到编辑场景后缓存的
    public static void SaveCameraPosToCamPosTemp()
    {
        s_bTempCameraPos = true;
        s_vCamPosTemp = Camera.main.transform.position;
    }
    
    public static void RevertFrCamPosTemp(bool bAni = true)
    {
        if (s_bTempCameraPos) {
            s_bTempCameraPos = false;
            if (bAni) {
                MainCameraM.s_Instance.AutoMoveTo(MainTownScene.s_vCamPosTemp, 0.3f);
            } else {
                Camera.main.transform.position = MainTownScene.s_vCamPosTemp;
            }
        } else {
            Camera.main.transform.position = MainCameraM.s_vHavenViewFarthestCamPos;
        }
    }
    
}

public class LoadingMainTown : ILoading
{
    AsyncOperation mAsyncLoading;
    LoadingWnd mLoadingWnd = null;
    
    public void SetAsyncLoading(AsyncOperation async)
    {
        mAsyncLoading = async;
    }
    
    
    public void FadeIn()
    {
        if (mLoadingWnd == null) {
            mLoadingWnd = WndManager.GetDialog<LoadingWnd>();
        }
        mLoadingWnd.FadeIn();
    }
    
    /// 准备载入的动画
    /// </summary>
    public bool IsFadingIn()
    {
        return mLoadingWnd.IsFadindIn();
    }
    
    public void Load()
    {
        if (mLoadingWnd == null) {
            mLoadingWnd = WndManager.GetDialog<LoadingWnd>();
        }
        mLoadingWnd.Loading();
    }
    
    /// <summary>
    /// 动画是否已经播放完毕了
    /// </summary>
    public bool IsLoading()
    {
        if (mAsyncLoading == null) {
            return false;
        }
        float progress = mAsyncLoading.progress;
        
        if (progress < 1) {
            return true;
        }
        return false;
    }
    
    public void FadeOut()
    {
        if (mLoadingWnd == null) {
            mLoadingWnd = WndManager.GetDialog<LoadingWnd>();
        }
        mLoadingWnd.FadeOut();
    }
    
    /// <summary>
    /// 载入完毕的动画
    /// </summary>
    public bool IsFadingOut()
    {
        return mLoadingWnd.IsFadindOut();
    }
    
    
    /// <summary>
    /// 播放结束后尝试回收loading资源
    /// </summary>
    public void TryDestroy()
    {
        mLoadingWnd.DestroyDialogNow();
    }
}
