using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Unity3D常用的工具接口
/// </summary>
/// <author>zhulin</author>
public class U3DUtil
{

    public static void Destroy(GameObject go)
    {
        if (go)
        {
            GameObject.Destroy(go);
        }
    }

    public static void DestroyAllChild(GameObject ob, bool Immediate = false)
    {
        if (ob == null)
            return;
        for (int i = 0; i < ob.transform.childCount; i++)
        {
            Transform t = ob.transform.GetChild(i);
            t.gameObject.SetActive(false);//此设置是因为删除对象要在下一帧才执行，这样会导致一些问题
            if (Immediate == true)
            {
                GameObject.DestroyImmediate(t.gameObject);
            }
            else GameObject.Destroy(t.gameObject);
        }
    }

    public static void DestroyAllChild(GameObject ob,string excludePattern)
    {
        if (ob == null)
            return;
        for (int i = 0; i < ob.transform.childCount; i++)
        {
            Transform t = ob.transform.GetChild(i);
            if (t.name.IndexOf(excludePattern) > 0)
            {
                continue;
            }
            t.gameObject.SetActive(false);
            GameObject.Destroy(t.gameObject);
        }
    }
    
	/// <summary>
	/// 根据名字查找子对象
	/// </summary>
	public static GameObject FindChild(GameObject ob, string name)
	{
		Transform t = ob.transform.Find(name);
		if (t == null)
			return null;
		
		return t.gameObject;
	}
	
	/// <summary>
	/// 在所有下属物件中寻找子对象
	/// </summary>
	public static GameObject FindChildDeep(GameObject ob, string name)
	{
		GameObject o = FindChild(ob, name);
		if (o != null)
			return o;
		
		foreach (Transform t in ob.transform)
		{
			o = FindChildDeep(t.gameObject, name);
			if (o != null)
				return o;
		}
		return null;
	}

	public static GameObject GetParent<T>(GameObject ob)
	{
		if (ob == null)
			return null;

		Transform t = ob.transform;
		while (t != null) 
		{
			T c = (T)(object) t.GetComponent (typeof(T));
			if (c != null)
				return t.gameObject;
			else t =t.parent;
		}
		return null;
	}
	
	/// <summary>
	/// 区别于系统的GetComponentInChildren，本接口会在所有层级的下属物件中查找
	/// bWantActiveself 是否有显示有要求
	/// </summary>
	public static T GetComponentInChildren<T>(GameObject ob ,bool bWantActiveself)
	{
		// 先在自己的直接下属物件中查找
		foreach (Transform t in ob.transform)
		{
			T c = (T) (object) t.GetComponent(typeof(T));
			if (c != null)
				if(bWantActiveself&&t.gameObject.activeSelf||!bWantActiveself)
				// 找到了，直接返回
				return c;
		}
		
		// 没有找到，则继续在下属的下属中查找
		foreach (Transform t in ob.transform)
		{
			T c = GetComponentInChildren<T>(t.gameObject,bWantActiveself);
			if (c != null)
				if(bWantActiveself&&t.gameObject.activeSelf||!bWantActiveself)
				// 找到了
				return c;
		}
		
		// 没有此组件
		return (T) (object)null;
	}
	
	/// <summary>
    ///遍历查找所有下属附件（isRecusive 是否递归）
	/// </summary>
	public static T[] GetComponentsInChildren<T>(GameObject ob,bool isRecusive = true)
	{
		// 存放结果
		List<T> list = new List<T>();
		
		// 先在自己的直接下属物件中查找
		foreach (Transform t in ob.transform)
		{
			T c = (T) (object) t.GetComponent(typeof(T));
			if (c != null)
				list.Add(c);
		}
        if (isRecusive)
        {
            foreach (Transform t in ob.transform)
            {
                T[] arr = GetComponentsInChildren<T>(t.gameObject);
                foreach (T c in arr)
                    list.Add(c);
            }
        }
		// 继续在下属的下属中查找
		
		return list.ToArray();
	}
	
	/// <summary>
	/// 截屏
	/// </summary>
	/// <param name="bDefault">是否只切地图背景图，如果为false</param>
	public static Texture ScreenShot(bool bDefault)
	{
		GameObject ob = new GameObject();
		Camera c = ob.AddComponent<Camera>();
		
		GameObject fromCamera = GameObject.Find("MapCamera");
		if (fromCamera == null)
			fromCamera = GameObject.Find("Map/地图相机");
		if (fromCamera != null)
			c.CopyFrom(fromCamera.GetComponent<Camera>());
		else if (Camera.main != null)
			c.CopyFrom(Camera.main);
		else 
			return null;
		
		if (! bDefault)
			c.cullingMask = 1 << LayerMask.NameToLayer("map");
		
		int W = Screen.width;
		int H = Screen.height;
		if (W > 1024)
		{
			W = 1024;
			H = W*H/1024;
		}
		if (H > 768)
		{
			H = 768;
			W = H*W/768;
		}
		RenderTexture texture = new RenderTexture(Screen.width, Screen.height, 
												  16, RenderTextureFormat.Default);
		c.targetTexture = texture;
		c.Render();
		
		RenderTexture.active = texture;
		Texture2D t = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		t.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
		t.Apply();
		
		RenderTexture.active = null;
		GameObject.Destroy(ob);
		
		return t;
	}


    /// <summary>
    /// 对某对象及其下属的所有对象设置激活
    /// 等同于
    /// </summary>
    public static void SetActive(GameObject ob, bool active)
    {
        foreach (Transform t in ob.transform)
            SetActive(t.gameObject, active);
        ob.SetActive(active);
    }
    /// <summary>
    /// 子对象激活控制
    /// </summary>
    public static void SetChildrenActive(GameObject ob, bool active)
    {
        foreach (Transform t in ob.transform)
            SetActive(t.gameObject, active);
    }
    /// <summary>
    /// 设置材质alpha
    /// </summary>
    public static void SetGameObjectAlpha(GameObject go, float alpha)
    {
        if (go)
        {
            if (go.GetComponent<Renderer>())
            {
                Material m = go.GetComponent<Renderer>().material;
                Color color = m.color;
                color = new Color(color.r, color.g, color.b, alpha);
                go.GetComponent<Renderer>().material.color = color;
            }
        }
    }
	
	/// <summary>
	/// 获得置灰后的图片
	/// </summary>
	public static Texture GetGrayTexture(Texture texture)
	{
		Texture2D tex2D = texture as Texture2D;
		if (tex2D == null)
			return texture;
		
		Texture2D grayTex = new Texture2D(tex2D.width, tex2D.height);
		for (int i = 0; i < tex2D.width; i++)
		{
			for (int j = 0; j < tex2D.height; j++)
			{
				Color c = tex2D.GetPixel(i, j);
				float gray = c.r * 0.299f + c.g * 0.587f + c.b * 0.114f;
				grayTex.SetPixel(i, j, new Color(gray, gray, gray, c.a));
			}
		}
		grayTex.Apply();
		return grayTex;
	}
	
	/// <summary>
	/// 获得脚本
	/// </summary>
	public static T Get<T>(GameObject go)  where T : Component
	{
		T main = go.GetComponent<T>();
		if (main == null)
			main =	go.AddComponent<T>();
		return main;
	}
	

	public static string[] GetString(string str,string cutStr)
	{
		char[] cutChar = cutStr.ToCharArray();
		string[] sArray = str.Split(cutChar);
		return sArray;
	}
    /// <summary>
    /// 设置游戏对象镜像
    /// QFord
    /// </summary>
    /// <param name="go"></param>
    /// <param name="nXMirror"></param>
    /// <param name="nYMirror"></param>
    /// <param name="nZMirror"></param>
    public static void  SetMirror (GameObject go,int nXMirror=1, int nYMirror=1, int nZMirror=1)
    {
        if (go!=null)
        {
            Vector3 vMirror = go.transform.localScale;
            go.transform.localScale = new Vector3(Mathf.Abs(vMirror.x) * nXMirror, Mathf.Abs(vMirror.y) * nYMirror, Mathf.Abs(vMirror.z) * nZMirror);
        }
    }
    /// <summary>
    /// 设置vector3分量值的便捷方法
    /// </summary>
    public static Vector3 SetZ(Vector3 pos ,float z)
    {
        Vector3 r = new Vector3(pos.x, pos.y, z);
        return r;
    }
    public static Vector3 SetY(Vector3 pos ,float y)
    {
        Vector3 r = new Vector3(pos.x, y, pos.z);
        return r;
    }
    public static Vector3 SetX(Vector3 pos, float x)
    {
        Vector3 r = new Vector3(x, pos.y, pos.z);
        return r;
    }
    public static Vector3 AddZ(Vector3 pos, float z)
    {
        Vector3 r = new Vector3(pos.x, pos.y, pos.z+z);
        return r;
    }
    public static Vector3 AddY(Vector3 pos, float y)
    {
        Vector3 r = new Vector3(pos.x, pos.y+y, pos.z);
        return r;
    }
    public static Vector3 AddX(Vector3 pos, float x)
    {
        Vector3 r = new Vector3(pos.x+x, pos.y, pos.z);
        return r;
    }
    //end 设置vector3分量值的便捷方法

    /// <summary>
    /// 修改材质贴图
    /// </summary>
    public static void ChangeTexture(GameObject go,Texture t)
    {
        Renderer ren = go.GetComponent<Renderer>();
        if (go==null || t==null)
        {
            NGUIUtil.DebugLog("GameObject == null || Texture == null ");
            return;
        }
        foreach (Material m in ren.materials)
        {
            m.mainTexture = t;
        }
    }
    /// <summary>
    /// 跳过指定帧
    /// </summary>
    public static IEnumerator WaitForFrames(int frameCount)
    {
        while (frameCount > 0)
        {
            frameCount--;
            yield return null;
        }
    }

}
/// <summary>
/// 类对象数据扩展
/// </summary>
/// <Author>QFord</Author>
public static class ObjectExt
{
    /// <summary>
    /// 拷贝父类数据到子类
    /// </summary>
    /// <typeparam name="T1">子类类型</typeparam>
    /// <typeparam name="T2">父类类型</typeparam>
    /// <param name="obj">子类对象</param>
    /// <param name="otherObject">父类对象</param>
    /// <returns></returns>
    public static T1 CopyFrom<T1, T2>(this T1 obj, T2 otherObject)
        where T1 : class
        where T2 : class
    {
        PropertyInfo[] srcFields = otherObject.GetType().GetProperties(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

        PropertyInfo[] destFields = obj.GetType().GetProperties(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);

        foreach (var property in srcFields)
        {
            var dest = GetDestFieldByName(destFields, property.Name);
            if (dest != null && dest.CanWrite)
                dest.SetValue(obj, property.GetValue(otherObject, null), null);
        }

        return obj;
    }

    static PropertyInfo GetDestFieldByName(PropertyInfo [] props,string name)
    {
        foreach (var item in props)
        {
            if (item.Name == name)
            {
                return item;
            }
        }
        return null;
    }
}// end ObjectExt
