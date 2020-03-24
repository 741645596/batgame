#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#define UNITY_EDITOR_LOG
#endif

using UnityEngine;
using System.Collections;
using Logic;
/// <summary>
/// 船只摆放-建筑物信息（升级）窗口
/// <Author>QFord</Author>
/// </summary>
public class ShipSetBuildInfoWnd : WndBase {

	[HideInInspector]
	public bool IsUpgrade = false;
	public ShipPutInfo BuildInfo;

	public ShipSetBuildInfoWnd_h MyHead
	{
		get 
		{
			return (base.BaseHead () as ShipSetBuildInfoWnd_h);
		}
	}

	public override void WndStart()
	{
		base.WndStart();
        MainCameraM.s_Instance.EnableDrag(false);
		if (MyHead.BtnClose)
        {
			MyHead.BtnClose.OnClickEventHandler += BtnClose_OnClickEventHandler;
        }
		if (MyHead.BtnUpgrade && IsUpgrade)
        {
			MyHead.BtnUpgrade.OnClickEventHandler += BtnUpgrade_OnClickEventHandler;
			MyHead.BtnUpgrade.gameObject.SetActive(true);
        }
	}


    void UPGRADE_BUILDING_RESP(bool IsSucc, string Message)
    {
        if (IsSucc)
        {
            #if UNITY_EDITOR_LOG
            NGUIUtil.DebugLog("升级建筑成功！" + Message, "green");
            #endif
        }
        else
        {
            #if UNITY_EDITOR_LOG
            NGUIUtil.DebugLog("升级建筑失败：" + Message, "red");
            #endif
        }
    }

    /// <summary>
    /// 检测是否满足升级条件
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    bool CanUpgrade(ShipPutInfo info)
    {
		/*UserInfo userInfo = UserDC.UserInfo;
        if (userInfo!=null)
        {
            if (BuildInfo.m_levellimit>userInfo.level)
            {
                NGUIUtil.ShowTipWnd("升级失败：玩家等级小于建造该建筑物所需的等级！");
                return false;
            }
			if (BuildInfo.m_level == buildingM.GetBuildingMaxLevel(info.m_type))
            {
                NGUIUtil.ShowTipWnd("升级失败：该建筑物等级已经是最高等级！");
                return false;
            }
            if (BuildInfo.m_coin > userInfo.coin || BuildInfo.m_wood>userInfo.wood ||
                BuildInfo.m_stone > userInfo.stone || BuildInfo.m_steel>userInfo.steel )
            {
                NGUIUtil.ShowTipWnd("升级失败：所需的资源不足！");
                return false;
            }
        }*/

        return true;
    }

    void BtnUpgrade_OnClickEventHandler(UIButton sender)
    {
        if (BuildInfo!=null)
	    {
			if (CanUpgrade(BuildInfo))
            {
            }
	    }
        else
        {
            #if UNITY_EDITOR_LOG
            NGUIUtil.DebugLog("ShipSetBuildInfoWnd.cs 升级的建筑物info未设置");
            #endif
        }
    }

    void BtnClose_OnClickEventHandler(UIButton sender)
    {
        WndManager.DestoryDialog<ShipSetBuildInfoWnd>();
        //CameraHit.IsNGUIHit = false;
        MainCameraM.s_Instance.EnableDrag(true);
    }

    void OnDestroy()
    {
		if (MyHead.BtnUpgrade && IsUpgrade)
        {
        }
    }

	public void Init(bool isUpgrade ,ShipPutInfo Info)
    {
		IsUpgrade = isUpgrade;
		BuildInfo = Info;

		/*NGUIUtil.SetLableText(LblTitle, BuildInfo.m_name);

        NGUIUtil.AddLableText(LblMaxCapacity, 1000);
		NGUIUtil.AddLableText(LblBuildLife, BuildInfo.m_hp1);
		NGUIUtil.AddLableText(LblWallMD, BuildInfo.m_magicdefend2);
		NGUIUtil.AddLableText(LblBuildMD, BuildInfo.m_magicdefend1);
		NGUIUtil.AddLableText(LblWallLife, BuildInfo.m_hp2);
		NGUIUtil.AddLableText(LblWallPD, BuildInfo.m_physicaldefend2);
		NGUIUtil.AddLableText(LblBuildPD, BuildInfo.m_physicaldefend1);*/

		NGUIUtil.SetLableText(MyHead.LblBuildInfo, NGUIUtil.GetStringByKey(88800099));
    }
}
