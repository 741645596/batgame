/// <summary>
/// 粒子从World到NGUI的缩放
/// </summary>
/// <Author>QFord</Author>
/// <Data>2014-11-13   16:03</Data>
/// <Path>E:\Projs\SVN_Root\trunk\SeizeTheShip\Assets\Scripts\Client\UI</Path>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 粒子从World到NGUI的缩放
/// </summary>
public class ParticleEffect : MonoBehaviour {
	
	public static List<ParticleEffect> s_listParticleEffect=new List<ParticleEffect>();
	public static void AddParticleEffect(ParticleEffect pe)
	{
		//if (!s_listMaterail.Contains (matM))
		s_listParticleEffect.Add (pe);
	}
	public static void RemoveParticleEffect(ParticleEffect pe)
	{
		s_listParticleEffect.Remove(pe);
	}
	public static void StaticPlay(bool isPlay)
	{
		foreach (ParticleEffect pe in s_listParticleEffect) 
		{
			pe.Play(isPlay);
		}
	}
    /// <summary>
    /// 是否启用粒子缩放
    /// </summary>
    public bool EnabelScale = false;
    /// <summary>
    /// 粒子从world到NGUI的缩放因子
    /// </summary>
    public float ScaleFactor=0.13f;
    /// <summary>
    /// 根节点缩放比例
    /// </summary>
    public float TransformFactor = 1f;

    private ParticleSystem[] m_ps;
    void Start()
    {
        Stop();
        m_ps = transform.GetComponentsInChildren<ParticleSystem>();
		ScaleParticle();
		ParticleEffect.AddParticleEffect (this);
        Play(true);
	}
	public void OnDestroy()
	{
		ParticleEffect.RemoveParticleEffect(this);
	}
    /// <summary>
    /// 删除粒子对象
    /// </summary>
    public void DestroyParticleGO()
	{
		Destroy(gameObject);
    }

    public void Play(bool isPlay)
    {
        m_ps = transform.GetComponentsInChildren<ParticleSystem>();
        if (m_ps!=null)
        {
            for (int i = 0; i < m_ps.Length; i++)
            {
                if (isPlay)
                {
                    m_ps[i].Play();
                }
                else
                {
                    m_ps[i].Pause();
                }
            }
        }
    }

    public void Stop()
    {
        m_ps = transform.GetComponentsInChildren<ParticleSystem>();
        if (m_ps != null)
        {
            for (int i = 0; i < m_ps.Length; i++)
            {        
                 m_ps[i].Stop();
            }
        }
    }

    void ScaleParticle()
    {
        m_ps = transform.GetComponentsInChildren<ParticleSystem>();
        if (EnabelScale&&m_ps!=null)
        {
            for (int i = 0; i < m_ps.Length; i++)
            {

                m_ps[i].startSize = m_ps[i].startSize * ScaleFactor;
                m_ps[i].startSpeed = m_ps[i].startSpeed * ScaleFactor;
            }

            if (TransformFactor!=1f)
            {
                transform.parent.localScale *= TransformFactor;
            }
        }
    }


}
