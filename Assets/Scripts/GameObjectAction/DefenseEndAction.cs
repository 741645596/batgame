using UnityEngine;
using System.Collections;

/// <summary>
/// 防御战胜利播放胜利动作
/// </summary>
///<anthor>zhulin</anthor>

public class DefenseEndAction : MonoBehaviour
{
	private float m_timeCount = 0.0f;
	private bool m_IsOver = false;
	private float m_OverTime = 3.0f;
	private bool m_win = false;
	public delegate void ShowResult(bool win);
	private ShowResult m_Finishfun;

	public void SetFinishFun(ShowResult f, bool Win)
	{
		m_Finishfun = f;
		m_win = Win;
	}


	// Update is called once per frame
	void Update ()
	{
		m_timeCount += Time.deltaTime;
		if (m_timeCount >= m_OverTime)
		{
			m_timeCount = 0;
			if (m_Finishfun != null)
				m_Finishfun(m_win);
			Destroy(this);
		}
	}
}
