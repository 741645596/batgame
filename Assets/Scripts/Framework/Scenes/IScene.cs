﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// 场景载入、资源清理接口
/// </summary>
/// <author>zhulin</author>
public abstract class IScene
{
    protected AsyncOperation async;
    
    public float LoadingProgress {
        get
        {
            return async.progress;
        }
    }
    
    public AsyncOperation AsyncLoading {
        get
        {
            return async;
        }
    }
    
    
    private List<SceneObj> m_listSceneObj = new List<SceneObj>();
    
    public virtual void AddSceneObj(SceneObj go)
    {
        if (null != go && !m_listSceneObj.Contains(go)) {
            m_listSceneObj.Add(go);
        }
    }
    public virtual void ReMoveSceneObj(SceneObj go)
    {
        if (null != go && m_listSceneObj.Contains(go)) {
            m_listSceneObj.Remove(go);
        }
    }
    
    public SceneObj GetSceneObj(string name)
    {
        SceneObj sceneObj = null;
        foreach (SceneObj obj in m_listSceneObj) {
            if (obj == null) {
                continue;
            }
            if (obj.name == name) {
                sceneObj = obj;
            }
        }
        return sceneObj;
    }
    public int GetSceneObjCount()
    {
        return m_listSceneObj.Count;
    }
    
    public List<SceneObj> GetAllSceneObj()
    {
        return m_listSceneObj ;
    }
    
    
    /// <summary>
    /// 获取场景类型
    /// </summary>
    public static string GetSceneName()
    {
        return "IScene";
    }
    
    /// <summary>
    /// 资源载入入口
    /// </summary>
    public abstract IEnumerator Load();
    
    /// <summary>
    /// 准备载入场景
    /// </summary>
    public abstract void PrepareLoad();
    
    /// <summary>
    /// 资源卸载
    /// </summary>
    public abstract void Clear();
    
    /// <summary>
    /// 是否已经载入完成
    /// </summary>
    public abstract bool IsEnd();
    
    /// <summary>
    /// 场景start 函数
    /// </summary>
    public virtual void Start()
    { }
    /// <summary>
    /// 接管场景中关注对象的Update
    /// </summary>
    public virtual void Update(float deltaTime)
    { }
    
    
    /// <summary>
    /// 接管场景中关注对象的LateUpdate
    /// </summary>
    public virtual void LateUpdate(float deltaTime)
    { }
    
    /// <summary>
    /// 接管场景中关注对象的FixedUpdate
    /// </summary>
    public virtual void FixedUpdate(float deltaTime)
    { }
    
    /// <summary>
    /// 构建场景数据
    /// </summary>
    public  void BuildScene()
    {
        BuildWorld();
        BuildUI();
    }
    
    /// <summary>
    /// 构建UI
    /// </summary>
    public virtual void BuildUI()
    {
    
    }
    
    /// <summary>
    /// 构建世界空间
    /// </summary>
    public virtual void BuildWorld()
    {
    
    }
    
    
    public virtual void OnMouseDown(SceneObj objScene)
    { }
    
    public virtual void OnMouseUp(SceneObj objScene)
    { }
    
    
    
}
