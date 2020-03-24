using UnityEngine;
using System.Collections;

/// <summary>
/// 特效管理器
/// </summary>
/// <author>zhulin</author>
public class EffectM  {

    public static  string sPath = "effect/prefab/";

	/// <summary>
	/// 加载特效
	/// </summary>
	/// <param name="path">特效路径</param>
	/// <param name="name">特效名称</param>
	/// <param name="pos">特效加载相对位置</param>
	/// <param name="Parent">特效挂载点</param>
	public static GameObjectActionExcute LoadEffect(string path, string name, Vector3 EffectPos,Transform Parent  )
	{
		GameObject go = GameObjectLoader.LoadPath(path, name,Parent) as GameObject;
		if(go != null)
		{
			//EffectPos.z = -1f;
			go.transform.position = EffectPos;
			go.AddComponent<NdEffectSprite>();
			GameObjectActionExcute effect = go.AddComponent<GameObjectActionExcute>();
			return effect;
		}
		return null;
	}
/*	public static NdEffect LoadEffect(string path, string name, Vector3 EffectPos,Transform Parent  )
	{
		GameObject go = GameObjectLoader.LoadPath(path, name) as GameObject;
		if(go != null)
		{
			go.transform.parent = Parent;
			go.transform.position = EffectPos;
			NdEffect effect = go.AddComponent<NdEffect>();
			return effect;
		}
		return null;
	}*/


	/// <summary>
	/// 加载特效
	/// </summary>
	/// <param name="path">特效路径</param>
	/// <param name="name">特效名称</param>
	/// <param name="pos">特效加载相对位置</param>
	/// <param name="Parent">特效挂载点</param>
	public static ResourceAction LoadResouceEffect(string path, string name, Vector3 EffectPos,Transform Parent  )
	{
		GameObject go = GameObjectLoader.LoadPath(path, name, Parent) as GameObject;
		if(go != null)
		{
			go.transform.position = EffectPos;
			ResourceAction effect = go.AddComponent<ResourceAction>();
			return effect;
		}
		return null;
	}

    public static GameObjectActionExcute LoadEffect(string name, Vector3 localPos,Vector3 localRotation,Transform Parent)
    {
        GameObject go = GameObjectLoader.LoadPath("effect/prefab/", name, Parent) as GameObject;
        if (go != null)
        {
            //EffectPos.z = -1f;
            go.transform.localPosition = localPos;
            go.transform.Rotate(localRotation,Space.Self);
            GameObjectActionExcute effect = go.AddComponent<GameObjectActionExcute>();
            return effect;
        }
        return null;
    }
    /// <summary>
    ///  加载可自动销毁的特效（一般就是播放一次就没了）
    /// </summary>
    public static void LoadAutoDestroyEffect(string name,Transform parent,float duration)
    {
        //这里更好的方法是动态获取动作时长<如果没有，获取粒子的>，动画时长和粒子销毁时间一致
        //不过如果这样的话，需要在特效中增加一个引用类<也可规范特效命名>，来获取的某个粒子时长
        GameObjectActionExcute gae = LoadEffect(name, Vector3.zero, Vector3.zero, parent);
        if (gae!=null)
        {
            GameObjectActionDelayDestory autoDestroy = new GameObjectActionDelayDestory(duration);
            gae.AddAction(autoDestroy);
        }
    }
}
