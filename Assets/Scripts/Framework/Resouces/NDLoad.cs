using UnityEngine;

/// <summary>
/// 资源加载模块
/// </summary>
/// <author>zhulin</author>
/// Server--> Local (down) ---> perfab ====> gameobject
public class NDLoad
{

    /// <summary>
    /// 加载窗口
    /// </summary>
    public static GameObject LoadWnd(string WndName, Transform parent)
    {
        GameObject Prefab = null;
        
        Prefab = LoadPrefabInRes(ResoucesPathConfig.Res_WndPath + WndName);
        
        
        if (Prefab == null) {
            return null;
        }
        
        
        return Instantiate(Prefab, parent);
    }
    
    /// <summary>
    /// 加载窗口Item
    /// </summary>
    public static GameObject LoadWndItem(string ItemName, Transform parent)
    {
        GameObject Prefab = null;
        Prefab = LoadPrefabInRes(ResoucesPathConfig.Res_WndItemPath + ItemName);
        
        if (Prefab == null) {
            return null;
        }
        
        return Instantiate(Prefab, parent);
    }
    
    
    
    
    /// <summary>
    /// 从Resourcer 加载预置
    /// </summary>
    private static GameObject LoadPrefabInRes(string PrefabName)
    {
        GameObject Prefab = Resources.Load(PrefabName) as GameObject;
        if (Prefab == null) {
            App.log.To("NDLoad.cs", PrefabName + " perfab is null");
            return null;
        }
        return Prefab;
    }
    
    
    
    /// <summary>
    /// 实例化预制
    /// </summary>
    public static GameObject Instantiate(GameObject Prefab, Transform parent)
    {
        if (Prefab == null) {
            return null;
        }
        
        GameObject go = GameObject.Instantiate(Prefab, Vector3.zero, Quaternion.identity) as GameObject;
        if (go == null) {
            return null;
        }
        
        go.SetActive(false);
        if (parent != null) {
            go.transform.parent = parent;
        }
        go.name = Prefab.name;
        go.transform.localPosition = Prefab.transform.localPosition;
        go.transform.localRotation = Prefab.transform.localRotation;
        go.transform.localScale = Prefab.transform.localScale;
        go.SetActive(true);
        return go;
    }
    
}
