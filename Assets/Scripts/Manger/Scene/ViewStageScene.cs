using UnityEngine;
using System.Collections;


public class ViewStageScene : IScene
{
    /// <summary>
    /// 获取金库对象
    /// </summary>
    private Transform m_goldTransform = null;
    public Transform GoldTransform {
        get {return m_goldTransform;}
    }
    
    public new static string GetSceneName()
    {
        return "ViewStage";
    }
    /// <summary>
    /// 资源载入入口
    /// </summary>
    //private AsyncOperation async;
    public override IEnumerator Load()
    {
        async = Application.LoadLevelAsync(ViewStageScene.GetSceneName());
        return null;
    }
    
    /// <summary>
    /// 准备载入场景
    /// </summary>
    public override void PrepareLoad()
    {
    }
    
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
    
    
    public override void BuildUI()
    {
        Life.Environment = LifeEnvironment.View;
        StageDC.JoinCompaignBattle();
        
        if (StageDC.GetPveMode() == PVEMode.Attack) {
            BattleEnvironmentM.BuildViewStageScene();
        }
        
        ViewStageWnd wnd = WndManager.GetDialog<ViewStageWnd>();
        if (wnd != null) {
            wnd.SetStageid(StageDC.GetCompaignStageType(), StageDC.GetCompaignStageID());
        }
        
        FindGoldBuilding();
    }
    
    public override void BuildWorld()
    {
    
    }
    
    /// <summary>
    /// 查找金库
    /// </summary>
    private void FindGoldBuilding()
    {
        m_goldTransform = null;
        Transform t = BattleEnvironmentM.GetLifeMBornNode(true);
        if (t == null) {
            return ;
        }
        
        foreach (LifeObj obj in t.GetComponentsInChildren<LifeObj>()) {
            if (obj == null) {
                continue ;
            }
            Life l = obj.GetLife();
            if (l != null && l is Building1300) {
                m_goldTransform = obj.transform;
                return ;
            }
        }
    }
    /// <summary>
    /// 场景start 接口
    /// </summary>
    public override void Start()
    {
    
    }
    
    /// <summary>
    /// 接管场景中关注对象的Update
    /// </summary>
    public override void Update(float deltaTime)
    {
    
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
    public override void OnMouseDown(SceneObj objScene)
    {
    }
    
    public override void OnMouseUp(SceneObj objScene)
    {
    }
    
    
}