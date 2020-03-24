using UnityEngine;
using System.Collections;

/// <summary>
/// 游戏入口场景
/// </summary>
/// <author>weism</author>
public class BATScene : IScene
{

    public new static string GetSceneName()
    {
        return "BATScene";
    }
    /// <summary>
    /// 资源载入入口
    /// </summary>
    public override IEnumerator Load()
    {
        //Application.LoadLevelAsync("Login");
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
        // TODO
    }
    
    /// <summary>
    /// 是否已经载入完成
    /// </summary>
    public override bool IsEnd()
    {
        return true;
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