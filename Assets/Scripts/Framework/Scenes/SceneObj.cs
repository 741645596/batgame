using UnityEngine;
using System.Collections;


public class SceneObj : MonoBehaviour {
	public bool bAutoActive = true;
	private bool bDown = false;
	private Vector3 vDownPos;
	private IScene m_ISceneOwer;
	//public GameObject m_body;
	// Use this for initialization
	public virtual void Start () {
		
		gameObject.SetActive( bAutoActive);
		m_ISceneOwer = SceneM.GetCurIScene ();
		if (SceneM.IsLoading)
			m_ISceneOwer = SceneM.GetLoadingIScene ();
		if(null!=m_ISceneOwer)
		{
			m_ISceneOwer.AddSceneObj(this);
		}
	
	}
	public void ChangerOwner(IScene owner)
	{
		if(null!=m_ISceneOwer)
			m_ISceneOwer.ReMoveSceneObj(this);
		m_ISceneOwer = owner;
		if(null!=m_ISceneOwer)
			m_ISceneOwer.AddSceneObj(this);
	}
	void OnMouseDown()
	{
		IScene scene = SceneM.GetCurIScene ();
		if (null != scene) 
		{
			scene.OnMouseDown(this);
		}

	}
	void OnMouseUp()
	{
		IScene scene = SceneM.GetCurIScene ();
		if (null != scene) 
		{
			scene.OnMouseUp(this);
		}
	}

    public virtual void OnDestroy()
	{
		if(null!=m_ISceneOwer)
			m_ISceneOwer.ReMoveSceneObj(this);
    }
    /// <summary>
    /// 显示/隐藏 collider（不包含子节点）
    /// </summary>
    /// <param name="bShow"></param>
	public void Show(bool bShow)
	{
		//if (null != m_body)
		{
			//m_body.renderer.enabled = bShow;
		}
		if(GetComponent<Collider>())
			GetComponent<Collider>().enabled = bShow;
	}
    /// <summary>
    /// 显示/隐藏GameObject
    /// </summary>
    /// <param name="bShow"></param>
   public void ShowIncludeChild(bool bShow)
    {
        NGUIUtil.SetActive(gameObject, bShow);
    }
    /// <summary>
    /// 显示/隐藏 SceneObj下的子对象
    /// </summary>
    public GameObject ShowChild(string name ,bool bShow)
   {
       Transform child = transform.Find(name);
       if (child!=null)
       {
           NGUIUtil.SetActive(child.gameObject, bShow);
           return child.gameObject;
       }
       return null;
   }
    /// <summary>
    /// 子对象是否可见
    /// </summary>
    public bool IsChildActive(string name)
    {
        Transform child = transform.Find(name);
        if (child != null)
        {
            return child.gameObject.activeInHierarchy;
        }
        return false;
    }

	/// <summary>
	/// FixedUpdate
	/// </summary>
	protected virtual void NDStart ()
	{

	}
	/// <summary>
	/// Update
	/// </summary>
	public virtual void NDUpdate (float deltaTime) 
	{

	}
	/// <summary>
	/// FixedUpdate
	/// </summary>
	public virtual void NDFixedUpdate (float deltaTime)
	{

	}
	/// <summary>
	/// LateUpdate
	/// </summary>
	public virtual void NDLateUpdate(float deltaTime) 
	{

	}
}
