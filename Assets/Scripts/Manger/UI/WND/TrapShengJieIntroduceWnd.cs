using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using sdata ;

public class TrapShengJieIntroduceWnd : WndBase
{

    private UIButton m_BtnShengJie;
    
    public TrapShengJieIntroduceWnd_h MyHead {
        get
        {
            return (base.BaseHead() as TrapShengJieIntroduceWnd_h);
        }
    }
    
    private TrapShowWnd m_parent = null;
    private BuildInfo m_Info;
    private Dictionary<int, int> m_lNeedMaterial = new Dictionary<int, int>();
    private Dictionary<int, int> m_lhaveFragment = new Dictionary<int, int> ();
    private bool m_CanUp = true;
    
    public void SetBuildInfo(BuildInfo info, TrapShowWnd wnd)
    {
        m_parent = wnd;
        m_Info = info;
        ReSetUI();
        SetUI();
    }
    
    public void SetButtonEnable(bool isEnbale)
    {
        if (m_BtnShengJie != null) {
            m_BtnShengJie.IsTweenTarget = isEnbale;
            BoxCollider bc = m_BtnShengJie.gameObject.GetComponent<BoxCollider>();
            bc.enabled = isEnbale;
        }
    }
    
    public override void WndStart()
    {
        base.WndStart();
        RegisterHooks();
    }
    private void BtnShengJie_OnClickEventHandler(UIButton sender)
    {
        if (m_parent.m_bIsShowQualityUp) {
            return;
        }
        
        CanQualityResult  result = buildingM.GetQualityCanUP(m_Info);
        if (result == CanQualityResult.CanUp) {
            m_parent.BackUpInfo();
            BuildDC.Send_BuildQualityUpRequest(m_Info.ID);
            //			SetButtonEnable(false);
        }
        
    }
    public void Clear()
    {
    
    }
    public void ReSetUI()
    {
        MyHead.table.ClearTable();
    }
    private void DoCanShengJie()
    {
        if (m_BtnShengJie == null) {
            return;
        }
        CanQualityResult  result = buildingM.GetQualityCanUP(m_Info);
        if (result == CanQualityResult.CanUp) {
            m_BtnShengJie.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            TweenScale ts = TweenScale.Begin(m_BtnShengJie.gameObject, 0.2f, new Vector3(1.2f, 1.2f, 1.2f));
            ts.style = UITweener.Style.PingPong;
            
        } else {
            TweenScale ts = m_BtnShengJie.gameObject.GetComponent<TweenScale>();
            if (ts != null) {
                Destroy(ts);
            }
        }
    }
    
    private void SetUI()
    {
    
        //		AddDoubleLabelItem ("[552d0a]"+NGUIUtil.GetStringByKey(10000058)+"[-]"+m_Info.m_Solidity.ToString()
        //		                    ,"[552d0a]"+NGUIUtil.GetStringByKey(10000059)+"[-]"+m_Info.m_Intensity.ToString(),22);
        //
        //		AddDoubleLabelItem ("[552d0a]"+NGUIUtil.GetStringByKey(10000060)+"[-]"+m_Info.m_Tenacity.ToString()
        //		                    ,"[552d0a]"+NGUIUtil.GetStringByKey(10000057)+"[-]"+m_Info.m_hp.ToString(),22);
        //
        //		AddDoubleLabelItem ("[552d0a]"+NGUIUtil.GetStringByKey(10000053)+"[-]"+m_Info.m_phyattack.ToString()
        //		                    ,"[552d0a]"+NGUIUtil.GetStringByKey(10000055)+"[-]"+m_Info.m_phydefend.ToString(),22);
        //
        //		AddDoubleLabelItem ("[552d0a]"+NGUIUtil.GetStringByKey(10000071)+"[-]"+m_Info.m_magicattack.ToString()
        //		                    ,"[552d0a]"+NGUIUtil.GetStringByKey(10000056)+"[-]"+m_Info.m_magicdefend.ToString(),22);
        AddGrayItem();
        
        
        string StrLimit = "";
        
        int NextQuality = ConfigM.GetNextQuality(m_Info.Quality);
        int LimitLev = buildingM.GetUpQualityLevelNeed(m_Info);
        
        CanQualityResult result = buildingM.GetQualityCanUP(m_Info);
        if (result == CanQualityResult.QualityMax) {
            MyHead.table.Reposition();
            return ;
        }
        
        if (result != CanQualityResult.CanUp) {
            StrLimit = string.Format("[552d0a]" + NGUIUtil.GetStringByKey(10000072) + "[-]" + "[FF0000]" + NGUIUtil.GetStringByKey(10000091) + "[-]", LimitLev);
        } else {
            StrLimit = string.Format("[552d0a]" + NGUIUtil.GetStringByKey(10000072) + ":[-]");
        }
        
        m_CanUp = result == CanQualityResult.CanUp;
        AddLabelItem(StrLimit, 20);
        
        AddNeedMaTerial();
        AddShengJieBtn();
        
        MyHead.table.Reposition();
        
        DoCanShengJie();
    }
    /// <summary>
    /// Adds the gray item for 技能描述.
    /// </summary>
    private void AddGrayItem()
    {
        if (m_Info.m_Skill == null) {
            return;
        }
        
        GameObject SingleItemGo = NDLoad.LoadWndItem("SmallSingleLabelItem", MyHead.table.transform);
        SingleLabelItem itemSing = SingleItemGo.GetComponent<SingleLabelItem>();
        
        if (itemSing.MyHead.label01 && m_Info.m_Skill != null) {
            itemSing.MyHead.label01.overflowMethod = UILabel.Overflow.ResizeHeight;
            itemSing.SetData("[552d0a]" + m_Info.m_Skill.m_desc.Replace("\\n", System.Environment.NewLine) + "[-]", 22);
        }
        
    }
    /// <summary>
    /// 获取需要的碎片及拥有数量。
    /// </summary>
    private void GetFragment()
    {
        int LimitLev = 0;
        buildingM.GetUpQualityNeed(m_Info.BuildType, m_Info.Quality, ref m_lNeedMaterial, ref LimitLev);
        
        //获取已有的资源。
        List<int> litemType = new List<int>();
        foreach (int itemtype in m_lNeedMaterial.Keys) {
            litemType.Add(itemtype);
        }
        ItemDC.GetItemCount(litemType, ref m_lhaveFragment);
    }
    
    /// <summary>
    /// Adds the need material 添加需要材料列表
    /// </summary>
    private void AddNeedMaTerial()
    {
        GameObject dropHeroChild = new GameObject("dropHeroChild");
        dropHeroChild.transform.parent = MyHead.table.transform;
        Vector2 size = new Vector2(426f, 150f);
        Vector3 pos = new Vector3(0f, 0f, 0f);
        Vector3 scaleSize = new Vector3(1f, 1f, 1f);
        
        dropHeroChild.transform.localPosition = pos;
        dropHeroChild.transform.localScale = scaleSize;
        
        Vector3 local = Vector3.zero;
        //Vector3 world = dropHeroChild.transform.TransformPoint(local);
        
        //获取拥有
        GetFragment();
        
        int i = 0;
        int columns = 4;
        int YOffset = 110;
        int XOffset = 90;
        foreach (int itemtype in m_lNeedMaterial.Keys) {
            GameObject goFirst = NDLoad.LoadWndItem("NeedItemNumItem", dropHeroChild.transform);
            goFirst.gameObject.transform.localPosition = new Vector3(-size.x / 2 + 60 + i % columns * XOffset, (i / columns + 1) * 20 - i / columns * YOffset, 0f);
            NeedItemNumItem  item = goFirst.GetComponent<NeedItemNumItem>();
            item.SetHorItem();
            item.SetData(m_lhaveFragment[itemtype], m_lNeedMaterial[itemtype], itemtype);
            i++;
            if (m_CanUp == true) {
                if (m_lhaveFragment[itemtype] < m_lNeedMaterial[itemtype]) {
                    m_CanUp = false;
                }
            }
        }
        
    }
    private void AddLabelItem(string text, int iFontSize = 0)
    {
        GameObject go = NDLoad.LoadWndItem("SingleLabelItem", MyHead.table.transform);
        SingleLabelItem item = go.GetComponent<SingleLabelItem>();
        
        item.MyHead.label01.overflowMethod = UILabel.Overflow.ResizeHeight;
        item.SetData(text, iFontSize);
    }
    
    private void AddDoubleLabelItem(string text1, string text2, int iFontSize = 0)
    {
    
    
    }
    private void AddShengJieBtn()
    {
        GameObject go = NDLoad.LoadWndItem("BtnShengjieView", MyHead.table.transform);
        m_BtnShengJie = go.GetComponentInChildren<UIButton>();
        
        if (m_BtnShengJie != null) {
            m_BtnShengJie.OnClickEventHandler += BtnShengJie_OnClickEventHandler;
        }
    }
    /// <summary>
    /// 反注册.
    /// </summary>
    public void OnDestroy()
    {
        AntiRegisterHooks();
        WndManager.DestoryDialog<SkillIntroduceWnd>();
    }
    
    /// <summary>
    /// 注册事件
    /// </summary>
    public   void RegisterHooks()
    {
        DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_904, TrapQualityUp_Resp);
    }
    
    /// <summary>
    /// 反注册事件
    /// </summary>
    public   void AntiRegisterHooks()
    {
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_904, TrapQualityUp_Resp);
    }
    
    private void UpDateBuildInfo()
    {
        m_Info = BuildDC.GetBuilding(m_Info.ID);
    }
    
    /// <summary>
    /// 902 升阶
    /// </summary>
    /// <param name="nErrorCode"></param>
    void TrapQualityUp_Resp(int nErrorCode)
    {
        if (nErrorCode == 0) {
            //			UpDateBuildInfo();
            //			ReSetGrayItem();
            m_parent.UpQualityEffect();
            
        }
        
    }
}
