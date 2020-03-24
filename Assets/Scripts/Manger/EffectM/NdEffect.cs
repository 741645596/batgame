using UnityEngine;
using System.Collections;

/// <summary>
/// 特效表现控制基类。
/// </summary>
/// <author>zhulin</author>

public enum EffectDepth
{
	Front   = 0,
	Depth1  = 1,
}


public class NdEffect : MonoBehaviour {

	public float m_Duration = 100000.0f;
	private int m_count = 0;

	void Start () {
		if(m_count == 0)
		{
			m_count = 1;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		m_Duration -= Time.deltaTime;
		if (m_Duration <= 0 || m_count == 0)
			Destroy(gameObject);
		
	}
	/// <summary>
	/// 重置时间
	/// </summary>
	public void ResetDuration(float Duration)
	{
		if (m_Duration < Duration)
			m_Duration = Duration;
	}
	/// <summary>
	/// 设置深度
	/// </summary>
	public void SetEffectDepth(EffectDepth depth)
	{
		if(depth == EffectDepth.Front)
		{
			Vector3 pos = transform.position;
			pos.z = -1.2f; 
			transform.position = pos;
		}
	}

	/// <summary>
	/// 设置镜像
	/// </summary>
	public void SetEffectMirror(WalkDir dir)
	{
		if (dir == WalkDir.WALKRIGHT)
		{
			U3DUtil.SetMirror(gameObject, -1, 1, 1);
		}
	}

	/// <summary>
	/// 设置特效持续时间
	/// </summary>	
	public void SetDuration(float Duration)
	{
		m_Duration = Duration;
	}
    /// <summary>
    /// 设置特效旋转角度
    /// </summary>
    public void SetRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
    }
    /// <summary>
    /// 设置特效旋转角度
    /// </summary>
    public void SetRotation(Vector3  v)
    {
        transform.Rotate(v, Space.Self);
    }

	/// <summary>
	/// 销毁特效。
	/// </summary>	
	public void DestroyEffect()
	{
		Destroy(gameObject);
	}


	public void AddTime()
	{
		m_count++;
	}
	
	public void RemoveTime()
	{
		m_count --;
	}

}
