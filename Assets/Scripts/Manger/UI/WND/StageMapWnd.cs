using UnityEngine;
using System.Collections.Generic;
using sdata;

/// <summary>
/// 战役副本地图
/// </summary>
public class StageMapWnd : WndBase
{

    public StageMapWnd_h MyHead {
        get { return (base.BaseHead() as StageMapWnd_h); }
    }
    private int m_Chapter;
    private StageType m_type;
    private int m_totalStar = 0;
    private int m_haveStar = 0;
    private bool m_bSetMenuTop = true;
    private bool m_bUpdate = true;
    
    /// <summary>
    /// 副本内容
    /// </summary>
    private StageContext m_Contxt;
    private List<s_stage_rewardInfo> m_ChapterReward = new List<s_stage_rewardInfo>();
    
    // Use this for initialization
    public override void WndStart()
    {
        base.WndStart();
        RegisterHooks();
        AddEvents();
    }
    /// <summary>
    /// 是否设置菜单置顶.
    /// </summary>
    /// <param name="Top">If set to <c>true</c> top.</param>
    public void SetMainMenuTop(bool Top = true)
    {
        m_bSetMenuTop = Top;
        m_bUpdate = true;
    }
    
    void Update()
    {
        if (m_bUpdate) {
            m_bUpdate = false;
            if (m_bSetMenuTop) {
                NGUIUtil.SetMainMenuTop(this);
            }
        }
        //DisableStageNode(1);
    }
    
    /// <summary>
    /// 前往指定章节
    /// </summary>
    /// <param name="type">副本类型</param>
    /// <param name="Chapter">章</param>
    /// <param name="Node">节</param>
    /// <returns></returns>
    public void GotoChapter(StageType type, int Chapter, int Stage)
    {
        m_type = type;
        m_Chapter = Chapter;
        UpdateData(m_type, m_Chapter);
    }
    
    public override bool IsFullWnd()
    {
        return true;
    }
    
    
    void UpdateData(StageType Type, int Chapter)
    {
        m_type = Type;
        m_Chapter = Chapter;
        m_ChapterReward = StageM.GetChapterReward(m_type, m_Chapter);
        GetStarData();
        LoadStageContext(m_Chapter, m_type);
        SetUI();
        
        GetBoxDataStage();
    }
    
    public void GetBoxDataStage()
    {
        List<int> lReward = new List<int>();
        foreach (s_stage_rewardInfo I in m_ChapterReward) {
            lReward.Add(I.id);
        }
        StageDC.SendStageRewardFlagRequest(lReward);
    }
    
    void SetUI()
    {
        if (MyHead == null) {
            return;
        }
        bool PrevChapter = StageM.CheckHaveChapters(m_Chapter - 1, m_type);
        bool NextChapter = StageM.CheckHaveChapters(m_Chapter + 1, m_type);
        
        if (MyHead.Btn_Left) {
            MyHead.Btn_Left.gameObject.SetActive(PrevChapter);
        }
        
        if (MyHead.Btn_Right) {
            MyHead.Btn_Right.gameObject.SetActive(NextChapter);
        }
        
        bool HardLock = false ;
        HardLock = StageM.CheckHaveChapters(m_Chapter, StageType.Hard);
        bool TeamLock = StageM.CheckHaveChapters(m_Chapter, StageType.Team);
        
        if (HardLock == false) {
            MyHead.SprEliteLock.gameObject.SetActive(true);
            MyHead.Btn_Elite.isEnabled = false;
            MyHead.LblElite.alignment = NGUIText.Alignment.Right;
        } else {
            MyHead.SprEliteLock.gameObject.SetActive(false);
            MyHead.Btn_Elite.isEnabled = true;
            MyHead.LblElite.alignment = NGUIText.Alignment.Center;
        }
        //
        if (TeamLock == false) {
            MyHead.SprEliteRaid.gameObject.SetActive(true);
            MyHead.Btn_Raid.isEnabled = false;
            MyHead.LblRaid.alignment = NGUIText.Alignment.Right;
        } else {
            MyHead.SprEliteRaid.gameObject.SetActive(false);
            MyHead.Btn_Raid.isEnabled = true;
            MyHead.LblRaid.alignment = NGUIText.Alignment.Center;
        }
        
        if (MyHead.Lbl_Star != null) {
            MyHead.Lbl_Star.text = /*"[c7945b]"+ */m_haveStar.ToString() + /* "[-][3f3c2f]" + */ "/" + m_totalStar.ToString() /*+ "[-]"*/;
        }
        
        if (MyHead.Sprite_Progress != null) {
            MyHead.Sprite_Progress.fillAmount = CalcfillAmount(m_haveStar, m_totalStar);
        }
        
        
        if (MyHead.Lbl_NeedStartext1 != null) {
            MyHead.Lbl_NeedStartext1.text = GetNeedStar(0).ToString();
        }
        
        if (MyHead.Lbl_NeedStartext2 != null) {
            MyHead.Lbl_NeedStartext2.text = GetNeedStar(1).ToString();
        }
        
        if (MyHead.Lbl_NeedStartext3 != null) {
            MyHead.Lbl_NeedStartext3.text = GetNeedStar(2).ToString();
        }
        
        UpdataBoxState();
        
    }
    
    private void SetClickBoxSprite(int box)
    {
        if (box == 0) {
            MyHead.spr_Box1.spriteName = "zy_btn007";
        } else if (box == 1) {
            MyHead.spr_Box2.spriteName = "zy_btn004";
        } else if (box == 2) {
            MyHead.spr_Box3.spriteName = "zy_btn001";
        }
    }
    void UpdataBoxState()
    {
        int RewardID = GetRewardID(0);
        int state = StageDC.GetStarRewardState(RewardID);
        if (MyHead.spr_Box1 != null) {
            if (state == 2) {
                MyHead.spr_Box1.spriteName = "zy_btn009";
            } else if (state == 0) {
                MyHead.spr_Box1.spriteName = "zy_btn008";
            } else if (state == 1) {
                MyHead.spr_Box1.spriteName = "zy_btn019";
            }
        }
        
        RewardID = GetRewardID(1);
        state = StageDC.GetStarRewardState(RewardID);
        if (MyHead.spr_Box2 != null) {
            if (state == 2) {
                MyHead.spr_Box2.spriteName = "zy_btn006";
            } else if (state == 0) {
                MyHead.spr_Box2.spriteName = "zy_btn005";
            } else if (state == 1) {
                MyHead.spr_Box2.spriteName = "zy_btn018";
            }
        }
        
        
        RewardID = GetRewardID(2);
        state = StageDC.GetStarRewardState(RewardID);
        if (MyHead.spr_Box3 != null) {
            if (state == 2) {
                MyHead.spr_Box3.spriteName = "zy_btn003";
            } else if (state == 0) {
                MyHead.spr_Box3.spriteName = "zy_btn002";
            } else if (state == 1) {
                MyHead.spr_Box3.spriteName = "zy_btn017";
            }
        }
    }
    
    
    void AddEvents()
    {
        if (MyHead.Btn_Back) {
            MyHead.Btn_Back.OnClickEventHandler += OnBack;
        }
        if (MyHead.Btn_Normal) {
            MyHead.Btn_Normal.OnClickEventHandler += OnNormalstage;
        }
        if (MyHead.Btn_Elite) {
            MyHead.Btn_Elite.OnClickEventHandler += OnHardStage;
        }
        if (MyHead.Btn_Raid) {
            MyHead.Btn_Raid.OnClickEventHandler += OnTeamStage;
        }
        if (MyHead.Btn_Left) {
            MyHead.Btn_Left.OnClickEventHandler += OnLeft;
        }
        if (MyHead.Btn_Right) {
            MyHead.Btn_Right.OnClickEventHandler += OnRight;
        }
        if (MyHead.Btn_Box1) {
            MyHead.Btn_Box1.OnClickEventHandler += OnBox1;
        }
        if (MyHead.Btn_Box2) {
            MyHead.Btn_Box2.OnClickEventHandler += OnBox2;
        }
        if (MyHead.Btn_Box3) {
            MyHead.Btn_Box3.OnClickEventHandler += OnBox3;
        }
    }
    
    void OnBack(UIButton sender)
    {
        WndManager.DestoryDialog<StageMapWnd>();
    }
    
    
    
    void OnNormalstage(UIButton sender)
    {
        if (m_type == StageType.Normal) {
            return;
        }
        m_type = StageType.Normal;
        m_Chapter = StageDC.GetStageChapter(m_type);
        if (m_Chapter == 0) {
            NGUIUtil.ShowTipWndByKey("88800027", 0.5f);
        }
        UpdateData(m_type, m_Chapter);
        
    }
    
    void OnHardStage(UIButton sender)
    {
        if (m_type == StageType.Hard) {
            return;
        }
        m_type = StageType.Hard;
        m_Chapter = StageDC.GetStageChapter(m_type);
        if (m_Chapter == 0) {
            NGUIUtil.ShowTipWndByKey("88800028", 0.5f);
        }
        UpdateData(m_type, m_Chapter);
    }
    
    
    void OnTeamStage(UIButton sender)
    {
        if (m_type == StageType.Team) {
            return;
        }
        m_type = StageType.Team;
        m_Chapter = StageDC.GetStageChapter(m_type);
        if (m_Chapter == 0) {
            NGUIUtil.ShowTipWndByKey("88800029", 0.5f);
        }
        UpdateData(m_type, m_Chapter);
    }
    
    
    void OnLeft(UIButton sender)
    {
        m_Chapter = m_Chapter - 1;
        UpdateData(m_type, m_Chapter);
    }
    
    void OnRight(UIButton sender)
    {
        m_Chapter = m_Chapter + 1;
        UpdateData(m_type, m_Chapter);
    }
    
    
    void OnBox1(UIButton sender)
    {
        int RewardID = GetRewardID(0);
        int state = StageDC.GetStarRewardState(RewardID);
        if (state == 0) {
            SetClickBoxSprite(0);
        }
        
        StarAwardType Type = GetBoxType(0);
        StarAwardWnd wnd = WndManager.GetDialog<StarAwardWnd>();
        wnd.SetData(this, Type, RewardID, m_haveStar, m_totalStar);
    }
    
    void OnBox2(UIButton sender)
    {
        int RewardID = GetRewardID(1);
        int state = StageDC.GetStarRewardState(RewardID);
        if (state == 0) {
            SetClickBoxSprite(1);
        }
        StarAwardType Type = GetBoxType(1);
        StarAwardWnd wnd = WndManager.GetDialog<StarAwardWnd>();
        wnd.SetData(this, Type, RewardID, m_haveStar, m_totalStar);
    }
    
    
    void OnBox3(UIButton sender)
    {
        int RewardID = GetRewardID(2);
        int state = StageDC.GetStarRewardState(RewardID);
        if (state == 0) {
            SetClickBoxSprite(2);
        }
        StarAwardType Type = GetBoxType(2);
        StarAwardWnd wnd = WndManager.GetDialog<StarAwardWnd>();
        wnd.SetData(this, Type, RewardID, m_haveStar, m_totalStar);
    }
    
    
    StarAwardType GetBoxType(int Box)
    {
        int RewardID = GetRewardID(Box);
        int state = StageDC.GetStarRewardState(RewardID);
        if (state == 0) {
            return StarAwardType.GetItems;
        } else {
            return StarAwardType.GetNothing;
        }
    }
    
    
    void LoadStageContext(int Stage, StageType type)
    {
        if (m_Contxt != null) {
            GameObject.DestroyImmediate(m_Contxt.gameObject);
            m_Contxt = null;
        }
        
        string text = "StageContext_";
        if (type == StageType.Hard) {
            text = "HardStageContext_";
        }
        
        GameObject go = GameObjectLoader.LoadPath("Prefabs/Stage/", text + m_Chapter, MyHead.transform);
        if (go == null) {
            NGUIUtil.ShowFreeSizeTipWnd(20000000, null, 10);
            return;
        }
        GameObjectLoader.SetGameObjectLayer(go, MyHead.gameObject.layer);
        m_Contxt = go.GetComponent<StageContext>();
        if (m_Contxt != null) {
            m_Contxt.SetStageContext(m_type, m_Chapter);
        }
    }
    
    private void GetStarData()
    {
        m_totalStar = 0;
        m_haveStar = 0;
        List<CounterPartInfo> l = StageDC.GetChaptersGate(m_Chapter, m_type);
        foreach (CounterPartInfo Item in l) {
            if (Item.isboss == 1) {
                m_totalStar += 3;
                if (StageDC.CheckOpenStage(m_type, Item.id) == true) {
                    m_haveStar += StageDC.GetPassStageStar(m_type, Item.id);
                }
            }
        }
    }
    
    
    private float CalcfillAmount(int HaveStar, int TotalStar)
    {
        return (HaveStar * 1.0f) / (TotalStar * 1.0f);
    }
    
    private int GetRewardID(int box)
    {
        s_stage_rewardInfo Info = GetStageReward(box);
        if (Info == null) {
            return 0;
        }
        return Info.id;
    }
    
    private s_stage_rewardInfo GetStageReward(int box)
    {
        if (m_ChapterReward == null || m_ChapterReward.Count == 0) {
            return null;
        }
        if (m_ChapterReward.Count <= box || box < 0) {
            return null;
        } else {
            return m_ChapterReward[box];
        }
    }
    
    private int GetNeedStar(int box)
    {
        s_stage_rewardInfo Info = GetStageReward(box);
        if (Info == null) {
            return 0;
        }
        return Info.star;
    }
    
    /// <summary>
    /// 注册事件
    /// </summary>
    public void RegisterHooks()
    {
        DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_710, RewardStatus);
        DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_706, RefreshStageChapter);
    }
    
    /// <summary>
    /// 反注册事件
    /// </summary>
    public void AntiRegisterHooks()
    {
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_710, RewardStatus);
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_706, RefreshStageChapter);
    }
    
    
    void RewardStatus(int nErrorCode)
    {
        if (nErrorCode == 0) {
            UpdataBoxState();
        } else {
        
        }
    }
    
    /// <summary>
    /// 刷新战役进度
    /// </summary>
    void RefreshStageChapter(int nErrorCode)
    {
        if (nErrorCode == 0) {
            GotoChapter(StageDC.GetCompaignStageType(), StageDC.GetStageChapter(StageDC.GetCompaignStageType()), -1);
        } else {
            Debug.Log("获取数据失败：" + nErrorCode.ToString());
        }
    }
    
    public void OnDestroy()
    {
        //NGUIUtil.DebugLog("StageMapWnd destroy");
        AntiRegisterHooks();
    }
    
    /// <summary>
    /// 查找章节对象
    /// </summary>
    public StageNode FindStageNode(int Stage)
    {
        if (m_Contxt == null) {
            return null;
        }
        return m_Contxt.FindStageNode(Stage);
    }
}
