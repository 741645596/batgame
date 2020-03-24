using UnityEngine;
using System.Collections;

public class StagePoint : MonoBehaviour {

	public UISprite PointSprit;
	private LineState m_State;
	private bool Init = false;
	private float m_delay = 1.0f; 
	private float m_interval = 1.0f; 
	private float m_time = 0.0f;

	public void SetState(LineState stage ,int index ,int Count)
	{
		m_State = stage ;
		float dt = ConfigM.GetStagePointFlashTime();
		m_delay = dt * index;
		m_interval = dt * Count ;
		if(PointSprit == null)
			return;
		if(m_State == LineState.Finish)
			PointSprit.spriteName = "zy_btn015";
		else PointSprit.spriteName = "zy_btn016";
	}
	


	// Update is called once per frame
	void Update () {
		if(m_State != LineState.Roading || PointSprit == null)
			return ;
		m_time += Time.deltaTime;
		if(Init == false)
		{
			if(m_time >= m_delay)
			{
				Init = true;
				m_time = 0.0f;
				PointSprit.spriteName = "zy_btn015";
			}
		}
		else
		{
			if(m_time >= m_interval)
			{
				m_time = 0.0f;
				if(PointSprit.spriteName == "zy_btn015")
					PointSprit.spriteName = "zy_btn016";
				else PointSprit.spriteName = "zy_btn015";
			}
		}
	}
}
