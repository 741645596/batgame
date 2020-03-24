using UnityEngine;
using System.Collections;

/// <summary>
/// 拳击手套击飞
/// </summary>
public class GridActionCmdHitFly : GridActionCmd
{
    Bezier bezier;
    Bezier bezierspeed;
    float m_fDelay;
    bool m_bearhit;
    public GridActionCmdHitFly(Life Parent, Vector3 Target, float duration, WalkDir dir, float delay, bool bearhit)
    {
        SetTarget(Parent);
        Vector3 start = m_Skin.tRoot.position;
        m_fDelay = delay;
        base.SetData(start, Target, RoleState.HITFLY, duration, -1, dir, 1);
        if (bearhit) {
            bezier = new Bezier(start, new Vector3(0, 5, 0), new Vector3(0, 20, 0), Target);
            //用来控制变速的曲线
            bezierspeed = new Bezier(new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(1, 1, 0));
        } else {
            bezier = new Bezier(start, new Vector3(0, 20, 0), new Vector3(0, 20, 0), Target);
        }
        m_bearhit = bearhit;
        //Debug.Log( start + "," + Target);
    }
    
    public override void Update()
    {
        base.Update();
        if (m_TimeCount < m_fDelay) {
            return;
        }
        Vector3 pos = bezier.GetPointAtTime((m_TimeCount - m_fDelay) / (m_Duration - m_fDelay));
        if (m_bearhit) {
            //Debug.Log(m_TimeCount + "," + m_Duration + "," + ((m_TimeCount-m_fDelay) / (m_Duration - m_fDelay)) + "," + bezierspeed.GetPointAtTime((m_TimeCount-m_fDelay) / (m_Duration - m_fDelay)).y);
            pos = bezier.GetPointAtTime(bezierspeed.GetPointAtTime((m_TimeCount - m_fDelay) / (m_Duration - m_fDelay)).y);
        }
        pos.z = -3f;//前置被击飞的
        PlayAction(AnimatorState.Fly00000, pos, true);
    }
}
