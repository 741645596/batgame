using UnityEngine;
using System.Collections;

/// <summary>
/// 画布单元状态
/// </summary>
public enum CanvasUnitState
{
    Normal = 0,
    CanPut  = 1,
    CanntPut = 2,
    Top = 3,
}
/// <summary>
/// 设置 画布单元格 表现状态
/// </summary>
/// <authro>QFord</authro>
public class CanvasUnitBG : MonoBehaviour {

    public Texture TextureNormal;
    public Texture TextureCanPut;
    public Texture TextureCanntPut;
    public Texture TextureTop;

    private Material m_material;
    private Transform m_tTarget;
    private float m_fDuration = 0;
    private float m_fCounter = 0;
    public bool m_isShow = false;

	const float BUILD_TOP_DEPTH = 8.1f;
    const float BUILD_FIT_DEPTH =8.1f;
    /// <summary>
    /// 设置画布单元格 颜色表现
    /// </summary>
    public void SetCanvasUnitState(CanvasUnitState state)
    {
        if (state == CanvasUnitState.Normal)
        {
            if (m_material!=null && TextureNormal!=null)
            {
                m_material.mainTexture = TextureNormal;
                Vector3 pos = m_tTarget.transform.position;
                m_tTarget.transform.position  = U3DUtil.SetZ(pos, BUILD_FIT_DEPTH);
            }
        }
        else if (state == CanvasUnitState.CanPut)
        {
            if (m_material != null && TextureCanPut != null)
            {
                m_material.mainTexture = TextureCanPut;
                Vector3 pos = m_tTarget.transform.position;
                m_tTarget.transform.position = U3DUtil.SetZ(pos, BUILD_TOP_DEPTH);
            }
        }
        else if (state == CanvasUnitState.CanntPut)
        {
            if (m_material != null && TextureCanntPut != null)
            {
                m_material.mainTexture = TextureCanntPut;
                Vector3 pos = m_tTarget.transform.position;
                m_tTarget.transform.position = U3DUtil.SetZ(pos, BUILD_TOP_DEPTH);
            }
        }
        /*
        if (m_material)
        {
            Color color = m_material.color;
            color = new Color(color.r, color.g, color.b, 1);
            SetColor(color);
        }
        */
    }
	
    private void SetColor(Color c)
    {
        if (m_material == null)
        {
			m_material = gameObject.GetComponent<Renderer>().material;
        }
		if(m_material != null)
		{
			m_material.color = c;
		}
    }

    void Awake()
    {
        m_material = gameObject.GetComponent<Renderer>().material;
        m_tTarget = transform;
        Color color = m_material.color;
        color = new Color(color.r, color.g, color.b, 1.0f);//起始隐藏画布单元格
        SetColor(color);
    }

    void Update()
    {
        if (m_fCounter<m_fDuration)
        {
            Color color = m_material.color;
            float alpha = color.a;
            m_fCounter += Time.deltaTime;
            if (m_isShow)
            {
				alpha = Mathf.Lerp(0.10f, 1, m_fCounter / m_fDuration);     
            }
            else
            {
				alpha = Mathf.Lerp(1, 0.10f, m_fCounter / m_fDuration);
            }
            color = new Color(color.r, color.g, color.b, alpha);
            SetColor(color);
        }
    }
    /// <summary>
    ///  控制画布单元格的淡入淡出
    /// </summary>
    public void Fade(bool isShow,float duration,bool bImediate)
    {
        if (m_isShow == isShow)
        {
            return;
		}
		m_isShow = isShow;
		if (bImediate)
		{
			Color color = m_material.color;
			if (m_isShow)
			{
				color = new Color(color.r, color.g, color.b, 1.0f);  
			}
			else
			{
				color = new Color(color.r, color.g, color.b, 0.1f);  
			}
			SetColor(color);
			return ;
		}

        m_fDuration = duration;
        m_fCounter = 0;
    }
}
