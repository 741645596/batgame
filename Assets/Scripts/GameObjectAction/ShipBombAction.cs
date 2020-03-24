using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// 船只爆炸表现
/// </summary>
/// <author>zhulin</author>
public class ShipBombAction : MonoBehaviour {

	private List <BombArea> m_Bomb = new List <BombArea>();
	private float m_timeCount = 0.0f ;
	//爆炸几个时间节点,从配置表中读取
	private  float m_st1 = 0.0f;   
	private  float m_dt  = 0.0f;
	private  float m_dt1 = 0.0f;
	private  float m_dt2 = 0.0f;
	private  float m_dt3 = 0.0f;
	//
	private  float m_dtime = 0.0f;
	private  bool  m_IsOver = false;
	private  float m_OverTime = 3.0f;

	private bool m_win = false;
	public delegate void ShowResult(bool win);
	private ShowResult m_Finishfun;

	public void SetFinishFun(ShowResult f ,bool Win)
	{
		m_Finishfun = f ;
		m_win = Win;
	}

	void Start()
	{
		m_dtime = m_dt;
		ConfigM.GetShipBombPara(ref m_st1 ,ref m_dt ,ref m_dt1, ref m_dt2 ,ref m_dt3);
		m_IsOver = false;
		GetStartBombArea(ref m_dtime ,ref m_IsOver);
	}
	
	void Update()
	{
		m_timeCount += Time.deltaTime ;
		if(m_IsOver == false)
		{
			if(m_timeCount >= m_dtime)
			{
				m_timeCount = 0.0f;
				GetNextBombArea(ref m_dtime ,ref m_IsOver);
			}
		}
		else 
		{
			if(m_timeCount >= m_OverTime)
			{
				m_timeCount = 0;
				if(m_Finishfun != null)
					m_Finishfun(m_win);
				Destroy(this);
			}
		}
		PlayBombArea() ;
	}


	void GetStartBombArea(ref float dtime ,ref bool IsOver)
	{
		BombArea bomb = ShipBombRule.GetStartBombArea();
		if(bomb != null)
		{
			bomb.SetBombAreaTime(m_dt1 ,m_dt2 ,m_dt3);
			m_Bomb.Add(bomb);
			if(bomb.ProcessType == BombProcessType.Start)
				dtime = m_dt + m_st1;
			else dtime = m_dt;
		}
		else
		{
			IsOver = true;
		}
	}


	void GetNextBombArea(ref float dtime ,ref bool IsOver)
	{
		BombArea bomb = ShipBombRule.GetNextBombArea();
		if(bomb != null)
		{
			bomb.SetBombAreaTime(m_dt1 ,m_dt2 , m_dt3);
			m_Bomb.Add(bomb);
			if(bomb.ProcessType == BombProcessType.Start)
				dtime = m_dt + m_st1;
			else dtime = m_dt;
		}
		else IsOver = true;
	}


	void PlayBombArea()
	{
		for(int i = 0; i < m_Bomb.Count ; i++)
		{
			if(m_Bomb[i] != null)
				m_Bomb[i].Update(Time.deltaTime);
		}
	}


	void OnDestroy()
	{
		m_Bomb.Clear();
	}
	
}
