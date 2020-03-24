using UnityEngine;
using System.Collections;
//睡眠
public class GridActionCmdSleep : GridActionCmdHitByBuild
{
	Vector3 m_Start;
	Vector3 m_End;
	MapGrid desgrid;
    public GridActionCmdSleep(float duration)
	{
        //m_AppearTime = 0.5f;
        m_Duration = duration;
        //m_DisapperaTime = m_Duration - 1f;
        //m_ContinueTime = m_DisapperaTime - m_AppearTime;        
	}

    public override void StartHitByBuild()
    {
        PlayAction(AnimatorState.Squash, m_Start);
    }

    //public override void Update ()
    //{
    //    base.Update ();
    //    Vector3 pos = m_Start;
		
    //    float height = -0.8f;
    //    pos.y =m_Start.y + height * (  Mathf.Pow((m_TimeCount  /m_Duration - 0.5f) / 0.5f,2) - 1);
    //    PlayAction(AnimatorState.Hit,pos,true);
    //}
	public override void Finish ()
	{
		//(m_LifePrent as Role).RoleWalk.Teleport(desgrid);
	}
}
