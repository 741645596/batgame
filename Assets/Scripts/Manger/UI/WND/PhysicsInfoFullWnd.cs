using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 体力信息显示窗口
/// <From> </From>
/// <Author>QFord</Author>
/// </summary>
public class PhysicsInfoFullWnd : WndBase {

    private int m_iType = -1;

    public PhysicsInfoFullWnd_h MyHead
    {
        get
        {
            return (base.BaseHead() as PhysicsInfoFullWnd_h);
        }
    }

	public override void WndStart()
	{
		base.WndStart();
        RefreshUI();
        InvokeRepeating("RefreshUI", 0f, 1f);
	}

    void RefreshUI()
    {
        int currentPhysics = UserDC.GetPhysical();
        int total = UserM.GetMaxPhysical(UserDC.GetLevel());
        if (currentPhysics >= total)
            SetType(0);
        else
            SetType(1);

        int serverTime = GlobalTimer.GetNowTimeInt();
        NGUIUtil.SetLableText<string>(MyHead.LblTime, NdUtil.ConvertServerTime(serverTime));
        if (m_iType == 1)
        {
            int resumePhysicsTime = GlobalTimer.instance.GetPhysicsResumeCounter();
            NGUIUtil.SetLableText<string>(MyHead.LblNextResumeTime, NdUtil.TimeFormat(resumePhysicsTime));
            
            int resumePhysicsAllTime =GlobalTimer.instance.GetPhysicsResumeAllCounter();
            NGUIUtil.SetLableText<string>(MyHead.LblResumeAllTime, NdUtil.TimeFormat(resumePhysicsAllTime));
             
        }
    }
    /// <summary>
    /// 设置体力信息显示窗口的不同类型
    /// </summary>
    /// <param name="type">0 体力恢复满  1 体力未满</param>
    public void SetType(int type)
    {
        m_iType = type;
        switch (type)
        {
            case 0:
                NGUIUtil.SetActive(MyHead.FullGroup,true);
                NGUIUtil.SetActive(MyHead.NoFullGroup, false);
                break;

            case 1:
                NGUIUtil.SetActive(MyHead.FullGroup, false);
                NGUIUtil.SetActive(MyHead.NoFullGroup, true);
                
                int resumeInterval =  ConfigM.GetResumePhysicsTime()/60;
                string temp = resumeInterval+ Localization.Get("30000007");
                NGUIUtil.SetLableText<string>(MyHead.LblResumeInterval, temp);
                break;
        }
    }
    ///// <summary>
    ///// 设定体力恢复满
    ///// </summary>
    ///// <param name="current">当前体力</param>
    ///// <param name="total">全部体力</param>
    //public void SetPhysicsResumeAll(int current,int total)
    //{
    //    int resumePhysicsTime = GlobalTimer.instance.GetPhysicsResumeCounter();
    //    int temp = (total - current - 1) * ConfigM.GetResumePhysicsTime();
    //    temp += resumePhysicsTime;
    //    GlobalTimer.instance.SetPhysicsResumeAllCounter(temp);
    //}
}
