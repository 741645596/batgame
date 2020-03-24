using UnityEngine;
using System.Collections;

public class LongPressAddExp : MonoBehaviour
{
	private bool m_bMouseDown;//是否按下
	private bool m_bMoudeUp;
	private Vector3 m_v3MouseTouchDown;//按下时的鼠标位置信息
	private float m_fMouseDownTime; //按下时间
	private bool m_bLongTouch;//是否为长按

	private Vector2 GetTouchPos(int iTouchCnt)
	{
		Vector2 pos  = Vector3.zero;
		if (SystemInfo.deviceType == DeviceType.Desktop)
			pos = Input.mousePosition;
		else 
			pos = Input.GetTouch(iTouchCnt).position;
		return pos;
	}

	// Use this for initialization
	void Start ()
	{

	}

	// Update is called once per frame
	void Update ()
	{
		float fPresTime = Time.time - m_fMouseDownTime;
		if(m_bMouseDown && fPresTime >= 0.5f)
		{
			if((fPresTime*10)%5 == 0)
			{
				NGUIUtil.ShowTipWndByKey("88800013");
			}
		}
		if(m_bMoudeUp)
		{
			m_bLongTouch = false;
			m_bMouseDown = false;
		}
	}
	void OnMouseDown()
	{

		RaycastHit hit;
		if (WndManager.IsHitNGUI(out hit))//当点击到UI时不做处理
			return;
		if (Input.touchCount>=2)
			return;
		m_bMouseDown = true;
		m_bLongTouch = false;
		m_bMoudeUp = false;
		m_fMouseDownTime = Time.time;

	}
	void OnMouseUp()
	{
		m_bMoudeUp = true;
	}
}

