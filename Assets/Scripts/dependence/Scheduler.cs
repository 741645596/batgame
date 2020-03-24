using UnityEngine;
using Logic;
using System.IO;

/// <summary>
/// 帧调度，本组件绑定到gameobject以发挥作用
/// </summary>
public class Scheduler : MonoBehaviour
{
    public bool ac = false;
    
    void Awake()
    {
    
        DontDestroyOnLoad(gameObject);
        SceneM.LinkScheduler(gameObject);
        App.Init();
    }
    
    void Start()
    {
        TextAsset binAsset = Resources.Load("Config/sdata", typeof(TextAsset)) as TextAsset;
        byte[] data = binAsset.bytes;
        sdata.StaticDataResponse sdResp = new sdata.StaticDataResponse();
        sdResp = protobufM.Deserialize(sdResp.ToString(), data) as sdata.StaticDataResponse;
        DataCenter.LoadStaticDataToLocal(sdResp);
        DataCenter.SimulationData();
        SceneM.Load(MainTownScene.GetSceneName(), false, null, false);
    }
    
    
    
    
    void Update()
    {
        try {
            // 定时器调度
            Timer.Update();
            // 协程调度
            Coroutine.Update();
            // 场景调度
            SceneM.Update(Time.deltaTime);
            //数据服务中心
            BSsyncD.Update(Time.deltaTime);
        } catch (System.Exception e) {
            Debug.LogError(e);
        }
        
        if (ac) {
            Time.timeScale = 20f;
        }
    }
    
    
    void LateUpdate()
    {
        // 场景调度
        SceneM.LateUpdate(Time.deltaTime);
    }
    
    
    
    void FixedUpdate()
    {
        SceneM.FixedUpdate(Time.deltaTime);
    }
    
    // 退出游戏的处理 临时
    bool allowQuit = false;
    void OnApplicationQuit()
    {
        if (allowQuit) {
            return;
        }
        App.Empty();
        Application.Quit();
        
    }
    
    
}
