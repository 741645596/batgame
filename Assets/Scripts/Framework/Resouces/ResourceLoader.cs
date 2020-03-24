using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*----------------------------------------------------------------------------
 * 本文件将所有与资源载入相关的实现都放在这个地方
 *---------------------------------------------------------------------------*/
/// <summary>
/// 资源解析接口
/// </summary>
public interface IResourceParser
{
    string Parse(string name);
}

/// <summary>
/// 资源载入
/// </summary>
public class ResourceLoader<RT, T> where RT : UnityEngine.Object
    where T : IResourceParser, new ()
{
    /// <summary>
    /// 载入原始资源
    /// </summary>
    public static RT RawObject(string name) {
        T parser = new T();
        string loadName = parser.Parse(name);
        RT prefab = Resources.Load(loadName, typeof(RT)) as RT;
        
        if (prefab == null) {
            Debug.Log("ResourceLoader.cs ,资源:" + loadName + "不存在，无法载入。");
        }
        return prefab;
    }
    
    /// <summary>
    /// 载入资源后并构建n个实例
    /// </summary>
    public static RT[] Instantiate(string name, int n) {
        RT prefab = RawObject(name);
        List<RT> list = new List<RT>();
        for (int i = 0; i < n; i++) {
            list.Add(UnityEngine.Object.Instantiate(prefab) as RT);
        }
        
        return list.ToArray();
    }
}

#region 开始各种资源解析器的定义

/// <summary>
/// 默认解析器，原样返回
/// </summary>
public class DefaultParser : IResourceParser
{
    public string Parse(string name)
    {
        return name;
    }
}

#endregion

#region 各种资源的载入实现

/// <summary>
/// GameObject的载入
/// </summary>
public class GameObjectLoader : MonoBehaviour
{

    public static UnityEngine.Object LoadResouce(string path, string name)
    {
        if (name == "") {
            return null;
        }
        UnityEngine.Object ob = Resources.Load(path + name) ;
        
        return ob;
    }
    
    public static GameObject LoadPath(string path, string name, Transform parent = null)
    {
        if (name == "") {
            return null;
        }
        GameObject ob = Resources.Load(path + name) as GameObject;
        if (ob == null) {
            App.log.To("ResourceLoader.cs", path + name + " perfab is null");
            return null;
        }
        
        
        GameObject iob = null;
        iob = Instantiate(ob, Vector3.zero, Quaternion.identity) as GameObject;
        if (iob == null) {
            return null;
        }
        
        iob.SetActive(false);
        if (parent != null) {
            iob.transform.parent = parent;
        }
        iob.name = ob.name;
        iob.transform.localPosition = ob.transform.localPosition;
        iob.transform.localRotation = ob.transform.localRotation;
        iob.transform.localScale = ob.transform.localScale;
        iob.SetActive(true);
        return iob;
    }
    //带初始位置的加载资源，主要用在有拖尾的特效上，不然特效会从0点拖出来
    public static GameObject LoadPath(string path, string name, Vector3 pos,  Transform parent = null)
    {
        if (name == "") {
            return null;
        }
        GameObject ob = Resources.Load(path + name) as GameObject;
        if (ob == null) {
            App.log.To("ResourceLoader.cs", path + name + " perfab is null");
            return null;
        }
        
        
        GameObject iob = null;
        iob = Instantiate(ob, pos, Quaternion.identity) as GameObject;
        //Resources.UnloadUnusedAssets();
        if (iob == null) {
            return null;
        }
        
        
        if (parent != null) {
            iob.transform.parent = parent;
        }
        iob.name = ob.name;
        //iob.transform.localPosition = ob.transform.localPosition;
        iob.transform.localRotation = ob.transform.localRotation;
        iob.transform.localScale = ob.transform.localScale;
        
        return iob;
    }
    
    /// <summary>
    /// 遍历设置GameObject的Layer
    /// </summary>
    /// <param name="ob"></param>
    /// <param name="layer"></param>
    public static void SetGameObjectLayer(GameObject ob, int layer)
    {
        ob.layer = layer;
        
        for (int i = 0; i < ob.transform.childCount; i++) {
            GameObject childOb = ob.transform.GetChild(i).gameObject;
            SetGameObjectLayer(childOb, layer);
        }
    }
    
    
    /// <summary>
    /// 加载精灵
    /// </summary>
    public static Sprite LoadSprite(string path, string name)
    {
        Sprite sprite = Resources.Load<Sprite>(path + name);
        return sprite;
    }
}


#endregion
