using sdata;
using System.Collections.Generic;
/// <summary>
/// 召唤英雄
/// </summary>
/// <author>QFord</author>
public class SummonHeroWnd : WndTopBase
{
    /// <summary>
    /// 1英雄召唤，2陷阱召唤.
    /// </summary>
    public int m_iWndType = 1;
    public SoldierInfo m_soldierInfo;
    public BuildInfo m_buildInfo;
    
    //public static DialogWnd _instance;
    public SummonHeroWnd_h MyHead {
        get
        {
            return (base.BaseHead() as SummonHeroWnd_h);
        }
    }
    public void SetData(SoldierInfo soldierInfo, BuildInfo buildInfo, int wndType)
    {
        m_iWndType = wndType;
        m_soldierInfo = soldierInfo;
        m_buildInfo = buildInfo;
        SetUI();
    }
    // Use this for initialization
    public override void WndStart()
    {
        base.WndStart();
        DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_206, SoldierSummonResponse);
        DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_912, BuildSummonResponse);
        
        if (MyHead.BtnYES) {
            MyHead.BtnYES.OnClickEventHandler += BtnYES_OnClickEventHandler;
        }
        if (MyHead.BtnNO) {
            MyHead.BtnNO.OnClickEventHandler += BtnNO_OnClickEventHandler;
        }
        
        WndEffects.DoWndEffect(gameObject);
    }
    
    private void SoldierSummonResponse(int nErrorCode)
    {
        if (nErrorCode == 0 && m_iWndType == 1) {
        
            GameObjectActionExcute gae = GameObjectActionExcute.CreateExcute(gameObject);
            s_itemtypeInfo info = new s_itemtypeInfo();
            s_soldier_typeInfo TypeInfo = SoldierM.GetSoldierType(m_soldierInfo.SoldierTypeID);
            if (TypeInfo != null) {
                info.name = TypeInfo.name;
            }
            info.gtype = 1;
            info.gid = m_soldierInfo.SoldierTypeID;
            
            if (info == null) {
#if UNITY_EDITOR_LOG
                NGUIUtil.DebugLog("SoldierSummonResponse soldTypeId get s_itemtypeInfo is null" + soldTypeId.ToString());
#endif
                
                return;
            }
            List<s_itemtypeInfo> l = new List<s_itemtypeInfo>();
            l.Add(info);
            
            TrophiesActionWnd TropWnd = WndManager.GetDialog<TrophiesActionWnd>();
            if (TropWnd) {
                TropWnd.ClearTropiesData();
                TropWnd.AddTropiesData(l);
                TropWnd.SetWndType(3);
                TropWnd.MyHead.LblDes.gameObject.SetActive(false);
            }
            
            WndManager.DestoryDialog<SummonHeroWnd>();
        }
    }
    private void BuildSummonResponse(int nErrorCode)
    {
        if (nErrorCode == 0 && m_iWndType == 2) {
            GameObjectActionExcute gae = GameObjectActionExcute.CreateExcute(gameObject);
            s_itemtypeInfo info = new s_itemtypeInfo();
            s_building_typeInfo buildInfo = buildingM.GetBuildType(m_buildInfo.BuildType);
            if (buildInfo != null && buildInfo.name != null) {
                info.name = buildInfo.name;
            }
            info.gtype = 3;//item.ErnieRewardType;//如果是碎片该类型也是1.
            info.gid = m_buildInfo.BuildType;
            
            if (info == null) {
#if UNITY_EDITOR_LOG
                NGUIUtil.DebugLog("SoldierSummonResponse soldTypeId get s_itemtypeInfo is null" + soldTypeId.ToString());
#endif
                
                return;
            }
            List<s_itemtypeInfo> l = new List<s_itemtypeInfo>();
            l.Add(info);
            
            TrophiesActionWnd TropWnd = WndManager.GetDialog<TrophiesActionWnd>();
            if (TropWnd) {
                TropWnd.ClearTropiesData();
                TropWnd.AddTropiesData(l);
                TropWnd.SetWndType(4);
                TropWnd.MyHead.LblDes.gameObject.SetActive(false);
            }
            
            WndManager.DestoryDialog<SummonHeroWnd>();
        }
    }
    private void SetUI()
    {
        if (m_soldierInfo == null && m_buildInfo == null) {
            NGUIUtil.DebugLog("SummonHeroWnd 's Info is null!!!");
            return;
        }
        int NeedNum = 0;
        int NeedCoin = 0;
        if (m_iWndType == 1 && m_soldierInfo != null) {
            SoldierM.GetUpStarNeed(m_soldierInfo.SoldierTypeID, m_soldierInfo.StarLevel, ref NeedNum, ref  NeedCoin);
        } else if (m_iWndType == 2 && m_buildInfo != null) {
            buildingM.GetUpStarNeed(m_buildInfo.BuildType, m_buildInfo.StarLevel, ref NeedNum, ref  NeedCoin);
        }
        
        
        SetDialogLable(NeedCoin);
        if (NeedCoin > UserDC.GetCoin()) {
            MyHead.BtnYES.SetState(UIButtonColor.State.Disabled, true);
            MyHead.BtnYES.isEnabled = false;
        }
        
    }
    /// <summary>
    /// 取消召唤
    /// </summary>
    void BtnNO_OnClickEventHandler(UIButton sender)
    {
        WndEffects.DoCloseWndEffect(gameObject, DestoryDialogCallBack);
    }
    void DestoryDialogCallBack(object o)
    {
        WndManager.DestoryDialog<SummonHeroWnd>();
    }
    /// <summary>
    /// 召唤英雄
    /// </summary>
    void BtnYES_OnClickEventHandler(UIButton sender)
    {
        if (m_iWndType == 1) {
            SoldierDC.Send_SoldierSummonRequest(m_soldierInfo.SoldierTypeID);
        } else if (m_iWndType == 2) {
            BuildDC.Send_BuildComposeRequest(m_buildInfo.BuildType);
        }
    }
    /// <summary>
    /// 召唤英雄需要花费 XXXXX 金币
    /// </summary>
    public void SetDialogLable(int coin)
    {
        if (MyHead.LblTitle) {
            if (m_iWndType == 1) {
                MyHead.LblTitle.text = string.Format("[552d0a]" + NGUIUtil.GetStringByKey("88800035") + " {0} " + NGUIUtil.GetStringByKey("88800061") + "[-]", coin);
            } else if (m_iWndType == 2) {
                MyHead.LblTitle.text = string.Format("[552d0a]" + NGUIUtil.GetStringByKey(10000194) + " {0} " + NGUIUtil.GetStringByKey("88800061") + "[-]", coin);
            }
        }
    }
    
    void OnDestroy()
    {
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_206, SoldierSummonResponse);
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_912, BuildSummonResponse);
    }
}
