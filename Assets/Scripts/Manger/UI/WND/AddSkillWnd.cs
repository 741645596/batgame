using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
///  加技能
/// <From>炮弹兵详细信息界面 </From>
/// <Author>QFord</Author>
/// </summary>
public class AddSkillWnd : WndBase
{

    public AddSkillWnd_h MyHead {
        get
        {
            return (base.BaseHead() as AddSkillWnd_h);
        }
    }
    
    private SoldierInfo m_Info;
    /// <summary>
    /// 0 技能点数满 / 1 技能点数未满 / 2 技能点数为0
    /// </summary>
    private int m_wndType;
    
    private List<AddSkillItem> m_skillItems = new List<AddSkillItem>();
    
    private float m_fAutoRefreshTimer = ConfigM.GetResumeSkillTime();
    
    private int m_iTotalSecond;
    
    private AddSkillItem m_tGuide = null;
    
    private bool m_bRepositionFinish = false;
    
    public override void WndStart()
    {
        base.WndStart();
        RegisterHooks();
        MyHead.BtnBuySkillPoint.OnClickEventHandler += BtnBuySkillPoint_OnClickEventHandler;
        
        InvokeRepeating("RefreshUIBySecond", 0f, 1f);
    }
    
    void BtnBuySkillPoint_OnClickEventHandler(UIButton sender)
    {
        DialogWnd dialogWnd = WndManager.GetDialog<DialogWnd>();
        if (dialogWnd) {
            int a = 10;//购买技能点
            int c = UserDC.GetBuySkillPointTime();//刷新次数
            int b = ConfigM.GetBuyResumeSkill(c);//钻石费用
            dialogWnd.Align = NGUIText.Alignment.Left;
            string str = string.Format(NGUIUtil.GetStringByKey("30000021"), a, b, c);
            dialogWnd.SetDialogLable(str, NGUIUtil.GetStringByKey("88800063"), NGUIUtil.GetStringByKey("88800064"));
            dialogWnd.YESButtonOnClick = YesReAuth;
            dialogWnd.ShowDialog();
        }
    }
    /// <summary>
    /// 不购买技能点
    /// </summary>
    private void NoReAuth(UIButton sender)
    {
    }
    /// <summary>
    /// 确定购买技能点
    /// </summary>
    private void YesReAuth(UIButton sender)
    {
        UserDC.Send_BuySkillPointRequest();
        
    }
    
    public void RegisterHooks()
    {
        DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_606, BuySkillPointResponse);
        DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_212, SoldierSkillUpResponse);
        DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_624, BuySkillPointResponse);
    }
    
    
    public void AntiRegisterHooks()
    {
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_624, BuySkillPointResponse);
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_212, SoldierSkillUpResponse);
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_606, BuySkillPointResponse);
    }
    
    public void SetData(SoldierInfo info)
    {
        ClearUI();
        m_Info = info;
        SetUI();
    }
    
    private void ClearUI()
    {
        foreach (AddSkillItem item in m_skillItems) {
            if (item.gameObject != null) {
                GameObject.DestroyImmediate(item.gameObject);
            }
        }
        m_skillItems.Clear();
    }
    /// <summary>
    /// 每秒刷新技能点恢复倒计时
    /// </summary>
    private void RefreshUIBySecond()
    {
        int resumeSkillTime = GlobalTimer.instance.GetSkillResumeCounter();
        if (m_wndType == 1) {
            string timeStr = string.Format("({0})", NdUtil.TimeFormat(resumeSkillTime));
            NGUIUtil.SetLableText<string>(MyHead.LblResumeTimeNotFull, timeStr);
        } else if (m_wndType == 2) {
            NGUIUtil.SetLableText<string>(MyHead.LblResumeTimeEmpty, NdUtil.TimeFormat(resumeSkillTime));
        }
    }
    
    private void SetUI()
    {
        ClearUI();
        int leftPoints = UserDC.GetLeftSkillPoints();
        int maxLeftSkillPoints = ConfigM.GetMaxLeftSkillPoints(UserDC.GetVIPLevel());
        NGUIUtil.SetActive(MyHead.SkillPointsFull, false);
        NGUIUtil.SetActive(MyHead.SkillPointsNotFull, false);
        NGUIUtil.SetActive(MyHead.SkillPointsEmpty, false);
        if (leftPoints >= maxLeftSkillPoints) { //技能点数满
            m_wndType = 0;
            NGUIUtil.SetActive(MyHead.SkillPointsFull, true);
        } else if (leftPoints < maxLeftSkillPoints && leftPoints > 0) { //技能点数未满
            m_wndType = 1;
            NGUIUtil.SetActive(MyHead.SkillPointsNotFull, true);
        } else if (leftPoints == 0) { //技能点数为0
            m_wndType = 2;
            m_iTotalSecond = ConfigM.GetResumeSkillTime() + GlobalTimer.GetNowTimeInt();
            NGUIUtil.SetActive(MyHead.SkillPointsEmpty, true);
        }
        
        SetLeftSkillPoint(leftPoints);
        CreateList();
        NGUIUtil.RepositionTable(MyHead.Parent);
    }
    
    public void RefreshSkillItem()
    {
        m_Info = SoldierDC.GetSoldiers(m_Info.ID);
        SetData(m_Info);
    }
    
    private void SoldierSkillUpResponse(int nErrorCode)
    {
        if (nErrorCode == 0) {
            int leftPoints = UserDC.GetLeftSkillPoints();
            int maxLeftSkillPoints = ConfigM.GetMaxLeftSkillPoints(UserDC.GetVIPLevel());
            if (leftPoints == maxLeftSkillPoints - 1) {
                GlobalTimer.ResetSkillResume();
            }
            RefreshSkillItem();
            PdbycWnd wnd = WndManager.FindDialog<PdbycWnd>();
            if (wnd) {
                wnd.RefreshCombatPower();
            }
        }
    }
    
    private void BuySkillPointResponse(int nErrorCode)
    {
        if (nErrorCode == 0) {
            SetUI();
        }
    }
    
    /// <summary>
    /// 设置剩余技能点数(点数已满)
    /// </summary>
    private void SetLeftSkillPoint(int point)
    {
        int maxLeftSkillPoints = ConfigM.GetMaxLeftSkillPoints(UserDC.GetVIPLevel());
        string str = string.Format("[FF0000]{0}[-]/[FFFFFF]{1}[-]", point, maxLeftSkillPoints);
        if (m_wndType == 0) {
            MyHead.LblSkillPointsFull.text = str;
        } else if (m_wndType == 1) {
            MyHead.LblSkillPointsNotFull.text = str;
        }
    }
    /// <summary>
    /// 加载技能项
    /// </summary>
    private void CreateList()
    {
        if (MyHead.Parent == null) {
            NGUIUtil.DebugLog("AddSkillWnd.cs skillItem parent null !!! ");
            return;
        }
        List<SoldierSkill> skillList = m_Info.m_Skill.GetUpdateSkills();
        //List<int> listFilter = new List<int> { 1, 2, 3, 4,5,6 };
        for (int i = 0; i < skillList.Count; i++) {
            CreateSkillItem(skillList[i], i, m_Info.ID);
        }
        NGUIUtil.RepositionTable(MyHead.Parent);
        StartCoroutine(RepositionFinish(2));
    }
    
    IEnumerator RepositionFinish(int frameCount)
    {
        yield return StartCoroutine(U3DUtil.WaitForFrames(frameCount));
        m_bRepositionFinish = true;
    }
    
    private void CreateSkillItem(SoldierSkill info, int skillNo, int dsoldierID)
    {
        if (info.m_level == 0) {
            return;
        }
        
        GameObject go = NDLoad.LoadWndItem("AddSkillItem", MyHead.Parent.transform);
        AddSkillItem addSkillItem = go.GetComponent<AddSkillItem>();
        addSkillItem.SetData(info, skillNo, dsoldierID, m_Info.Level);
        m_skillItems.Add(addSkillItem);
        if (addSkillItem.CheckSkillCanUp() == 0 && m_tGuide == null) {
            m_tGuide = addSkillItem;
            addSkillItem.BGuideSelect = true;
        }
    }
    
    private void RefreshSkillItems()
    {
        foreach (AddSkillItem item in m_skillItems) {
            item.SetUI();
        }
    }
    
    public void OnDestroy()
    {
        AntiRegisterHooks();
    }
    
}
