using UnityEngine;
using System.Collections;

/// <summary>
/// life 对象挂接点模块
/// </summary>
/// <author>zhulin</author>
public class LifeProperty : MonoBehaviour {

	//private HPAciton m_hpa;
	private object m_life = null;
	public DictionaryGameObject m_HelpPoint;
	public DictionaryGameObject HelpPoint
	{
		get{return m_HelpPoint;}
	}

	
	public  void SetActive(bool active)
	{
		enabled = active ;
	}

	public object GetLife()//对应Life
	{
		return m_life;
	}

	public void SetLife(object life)//对应Life
	{
		m_life = life;
	}
}
