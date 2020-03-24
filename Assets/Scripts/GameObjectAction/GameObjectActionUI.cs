using UnityEngine;
using System.Collections;

public class GameObjectActionMoveBy : GameObjectAction
{
    public Vector3 m_Start = Vector3.zero;
    public Vector3 m_To = Vector3.zero;
    public Vector3 m_vdelta = Vector3.zero;
    public float m_fatetime = 0;
    public bool m_islocal = false;
    public GameObjectActionMoveBy(Vector3 vdelta, float duration, float fadetime, bool islocal)
    {
        m_vdelta = vdelta;
        m_Duration = duration;
        m_fatetime = fadetime;
        m_islocal = islocal;
    }
    public override void Start()
    {
        base.Start();
        if (m_islocal) {
            m_Start = m_target.transform.localPosition;
        } else {
            m_Start = m_target.transform.position;
        }
    }
    
    public override void ActionUpdate(float deltatime)
    {
        if (m_TimeCount < m_fatetime) {
        
            if (m_islocal) {
                m_target.transform.localPosition = Vector3.Lerp(m_Start, m_Start + m_vdelta, m_TimeCount / m_fatetime);
            } else {
                m_target.transform.position = Vector3.Lerp(m_Start, m_Start + m_vdelta, m_TimeCount / m_fatetime);
            }
        } else {
            UIWidget w = m_target.GetComponent<UIWidget>();
            w.alpha = Mathf.Lerp(1, 0, (m_TimeCount - m_fatetime) / (m_Duration - m_fatetime));
        }
    }
    
    public override void Finish()
    {
        base.Finish();
    }
}


public class GameObjectActionTreasureMoveToUI : GameObjectAction
{
    public enum harvesttype {
        gold,
        diamond,
    }
    harvesttype m_type;
    float m_eventtime;
    Vector3 m_start;
    Vector3 m_mid;
    Vector3 m_to;
    public Camera m_gameCamera;
    public Camera m_uicamera;
    public GameObjectActionTreasureMoveToUI(Camera gameCamera, Camera uicamera, harvesttype t)
    {
        m_gameCamera = gameCamera;
        m_uicamera = uicamera;
        m_type = t;
        m_Duration = 1f;
        m_eventtime = 0.5f;
    }
    public override void Start()
    {
        base.Start();
        m_start = m_target.transform.position;
        m_mid = m_start + new Vector3(0, 10, 0);
    }
    
    public override void ActionUpdate(float deltatime)
    {
        if (m_TimeCount < m_eventtime) {
            m_target.transform.position = Vector3.Lerp(m_start, m_mid, m_TimeCount / m_eventtime);
        } else {
            m_target.transform.position = Vector3.Lerp(m_mid, m_to, (m_TimeCount - m_eventtime) / (m_Duration - m_eventtime));
        }
    }
    
    public override void Finish()
    {
        base.Finish();
        m_target.SetActive(false);
        m_target.transform.position = m_start;
    }
}