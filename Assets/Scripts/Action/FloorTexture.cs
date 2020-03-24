using UnityEngine;
using System.Collections;

public enum FloorTextureType{Left,Middle,Right,Full,Destory,DamageSmall,DamageMiddle };

public class FloorTexture : MonoBehaviour {
	
	public Texture m_ttDestroyFullTexture;
    public Texture m_ttDamageSmallTexture;
    public Texture m_ttDamageMiddleTexture;
	public Texture m_ttDestroyLeftTexture;
	public Texture m_ttDestroyMiddleTexture;
	public Texture m_ttDestroyRightTexture;
	public Texture m_ttDestroyDestoryTexture;
	public FloorTextureType m_CurrentType = FloorTextureType.Full;
	public void Start()
	{
		m_CurrentType = FloorTextureType.Full;
	}
	public void DestroyTexture ( FloorTextureType type )
	{
		if (m_CurrentType == FloorTextureType.Destory)
			return;
		
		//Debug.Log(gameObject + "," + type + "," + m_CurrentType );
		switch(type)
		{
			case FloorTextureType.Left:
			if (m_CurrentType == FloorTextureType.Right)
			{
				gameObject.GetComponent<Renderer>().material.mainTexture = m_ttDestroyDestoryTexture;
				m_CurrentType = FloorTextureType.Destory;
			}
			else
			{
				gameObject.GetComponent<Renderer>().material.mainTexture = m_ttDestroyLeftTexture;
				m_CurrentType = type;
			}
				break;

			case FloorTextureType.Middle:
			/*if (m_CurrentType == FloorTextureType.Full)
				gameObject.renderer.material.mainTexture = m_ttDestroyMiddleTexture;
			else*/
			gameObject.GetComponent<Renderer>().material.mainTexture = m_ttDestroyMiddleTexture;
			m_CurrentType = type;
				break;

			case FloorTextureType.Right:
			if (m_CurrentType == FloorTextureType.Left)
			{
				gameObject.GetComponent<Renderer>().material.mainTexture = m_ttDestroyDestoryTexture;
				m_CurrentType = FloorTextureType.Destory;
			}
			else
			{
				gameObject.GetComponent<Renderer>().material.mainTexture = m_ttDestroyRightTexture;
				m_CurrentType = type;
			}
				break;

            case FloorTextureType.DamageSmall:
			if (m_CurrentType == FloorTextureType.Full || m_CurrentType == FloorTextureType.DamageSmall ||m_CurrentType == FloorTextureType.DamageMiddle)
			{
                gameObject.GetComponent<Renderer>().material.mainTexture = m_ttDamageSmallTexture;
                m_CurrentType = type;
			}
                break;

		case FloorTextureType.DamageMiddle:
			if (m_CurrentType == FloorTextureType.Full || m_CurrentType == FloorTextureType.DamageSmall ||m_CurrentType == FloorTextureType.DamageMiddle)
			{
                gameObject.GetComponent<Renderer>().material.mainTexture = m_ttDamageMiddleTexture;
                m_CurrentType = type;
			}
                break;

			default:
				App.log.To("FloorTexture.cs","unknow FloorTextureType");
				break;
		}
	}


}
