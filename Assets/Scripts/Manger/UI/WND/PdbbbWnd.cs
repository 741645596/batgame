using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 炮弹兵背包
/// <Author>QFord</Author>
/// </summary>
///
public enum FindSoldierStlye {
    CanEquip           = 0,     //能穿装备的炮弹兵
    CanSummon          = 1,     //能召唤的炮弹兵
    CanSkillUp         = 2,     //能升级的
    ExistEquipPosNoEquip    = 3, //存在空闲装备位，但装备不足的炮弹兵
}

public class PdbbbWnd : WndBase
{

    public PdbbbWnd_h MyHead {
        get
        {
            return (base.BaseHead() as PdbbbWnd_h);
        }
    }
    
    /// <summary>
    /// 当前已召唤的炮弹兵 数据
    /// </summary>
    private List<SoldierInfo> m_allExistSoldier = new List<SoldierInfo>();
    public List<SoldierInfo> AllExistSoldier {
        get {return m_allExistSoldier;}
    }
    /// <summary>
    /// 可以召唤但未召唤的炮弹兵
    /// </summary>
    private List<SoldierInfo> m_allCanExistSoldierInfo = new List<SoldierInfo>();
    /// <summary>
    /// 尚未召唤（灵魂石不够）的炮弹兵
    /// </summary>
    private List<SoldierInfo> m_allNoExistSoldierInfo = new List<SoldierInfo>();
    /// <summary>
    /// 当前选定 -1 全部，1 按肉盾，2：按输出，3 按辅助
    /// </summary>
    private int m_iSelectLocation = -1;
    /// <summary>
    /// 创建炮弹兵列表的计数器
    /// </summary>
    private int m_iCreatCount = 0;
    private bool m_bRunOnce = true;
    //用来保存 创建的炮弹兵 对象
    private List<NOExistHeroItem> m_CanExistSoldier = new List<NOExistHeroItem>();
    private List<NOExistHeroItem> m_NoExistSoldier = new List<NOExistHeroItem>();
    private List<ExistHeroItem> m_ExistSoldier = new List<ExistHeroItem>();
    /// <summary>
    /// 能穿装备
    /// </summary>
    private ExistHeroItem m_CanJinJieitem = null;
    /// <summary>
    /// 能召唤
    /// </summary>
    private NOExistHeroItem m_tCanSummon = null;
    /// <summary>
    /// 能升级的
    /// </summary>
    private ExistHeroItem m_tCanSkillUp = null;
    
    
    
    public override void WndStart()
    {
        base.WndStart();
        NGUIUtil.SetMainMenuTop(this);
        if (MyHead.BtnReturn) {
            MyHead.BtnReturn.OnClickEventHandler += BtnClose_OnClickEventHandler;
        }
        m_iSelectLocation = -1;
        GetAllSoldiersData();
        CreateHeroItems();
        MainCameraM.s_Instance.EnableDrag(false);
        float len = 0.25f;
        MyHead.PanelPdbbbWnd.ResetAndUpdateAnchors();
        NGUIUtil.UpdatePanelValue(MyHead.PanelMask, MyHead.PanelPdbbbWnd.width, len);
        ShowTogglesAni(0.05f, 0.05f);
        NGUIUtil.TweenGameObjectPosX(MyHead.ScrollLeft.gameObject, -MyHead.PanelPdbbbWnd.width / 2, 0.25f, gameObject, "HideAniScroll");
        NGUIUtil.TweenGameObjectPosX(MyHead.ScrollRight.gameObject, MyHead.PanelPdbbbWnd.width / 2, 0.25f);
        
        EventDelegate.Add(MyHead.TogAll.onChange, TogAll_OnChangeEventHandler);
        EventDelegate.Add(MyHead.TogTank.onChange, TogTank_OnChangeEventHandler);
        EventDelegate.Add(MyHead.TogDPS.onChange, TogDPS_OnChangeEventHandler);
        EventDelegate.Add(MyHead.TogAssisst.onChange, TogAssisst_OnChangeEventHandler);
    }
    /// <summary>
    /// 隐藏左右两个动画轴
    /// </summary>
    public void HideAniScroll()
    {
        NGUIUtil.SetActive(MyHead.ScrollLeft.gameObject, false);
        NGUIUtil.SetActive(MyHead.ScrollRight.gameObject, false);
        NGUIUtil.SetPanelClipping(MyHead.PanelMask, UIDrawCall.Clipping.None);
    }
    
    
    void Update()
    {
        float wi = MyHead.PanelPdbbbWnd.width;
        if (m_bRunOnce) {
            m_bRunOnce = false;
            NGUIUtil.RepositionTable(MyHead.Parent);
        }
    }
    /// <summary>
    /// 刷新UI
    /// </summary>
    public void RefreshUI()
    {
        ClearUI();
        GetAllSoldiersData();
        CreateHeroItems();
        NGUIUtil.RepositionTable(MyHead.Parent);
    }
    
    private void ClearUI()
    {
        U3DUtil.DestroyAllChild(MyHead.Parent.gameObject);
        
        m_NoExistSoldier.Clear();
        m_CanExistSoldier.Clear();
        m_ExistSoldier.Clear();
    }
    /// <summary>
    ///  获取所有炮弹兵数据
    /// </summary>
    /// <param name="mainProperty">-1 全部</param>
    private void GetAllSoldiersData()
    {
        m_allExistSoldier.Clear();
        m_allCanExistSoldierInfo.Clear();
        m_allNoExistSoldierInfo.Clear();
        
        List<SoldierInfo> lAll = SoldierM.GetAllSoldier();
        foreach (SoldierInfo I in lAll) {
            SoldierInfo Info = SoldierDC.GetSoldiersBySoldierType(I.SoldierTypeID);
            if (Info != null) {
                m_allExistSoldier.Add(Info);
                continue;
            }
            
            if (I.CheckCanSummon() == true) {
                m_allCanExistSoldierInfo.Add(I);
            } else {
                m_allNoExistSoldierInfo.Add(I);
            }
        }
        //sort
        //已召唤
        SoldierM.SortSoldierList(ref m_allExistSoldier);
        // 能召唤
        m_allCanExistSoldierInfo.Sort((a, b) => {
        
            if (a.SoldierTypeID > b.SoldierTypeID) {
                return 1;
            } else if (a.SoldierTypeID < b.SoldierTypeID) {
                return -1;
            } else {
                return 0;
            }
        });
        //不能召唤
        m_allNoExistSoldierInfo.Sort((a, b) => {
        
            if (a.GetHaveFragmentNum() > b.GetHaveFragmentNum()) {
                return -1;
            } else if (a.GetHaveFragmentNum() < b.GetHaveFragmentNum()) {
                return 1;
            }
            
            if (a.SoldierTypeID > b.SoldierTypeID) {
                return 1;
            } else if (a.SoldierTypeID < b.SoldierTypeID) {
                return -1;
            } else {
                return 0;
            }
        });
        
    }
    
    
    /// <summary>
    /// 根据选择显示的炮弹兵主属性创建炮弹兵
    /// </summary>
    /// <param name="mainProperty">-1 全部，1 按肉盾，2：按输出，3 按辅助</param>
    private void CreateHeroItems()
    {
        m_iCreatCount = 0;
        ClearUI();
        
        CreateCanExistHero();
        CreateExistHero();
        CheckNoExistHero();
        CreateNoExistHero();
        MyHead.Parent.Reposition();
        MyHead.Parent.repositionNow = true;
        
        SetSoldierCount();
    }
    /// <summary>
    ///  创建 可以召唤但未召唤的炮弹兵
    /// </summary>
    private void CreateCanExistHero()
    {
        PdbbbItem pbbItem = null;
        int count = 0;
        
        for (int i = 0; i < m_allCanExistSoldierInfo.Count; i++) {
            SoldierInfo info = m_allCanExistSoldierInfo[i];
            if (m_iSelectLocation != -1) {
                if (m_iSelectLocation != info.m_loaction) {
                    continue;
                }
            }
            m_iCreatCount++;
            
            if (count % 4 == 0) {
                pbbItem = null;
                GameObject go = NDLoad.LoadWndItem("PdbbbItem", MyHead.Parent.transform);
                pbbItem = go.GetComponent<PdbbbItem>();
            }
            count ++;
            if (pbbItem != null && pbbItem.MyHead.Table != null) {
                GameObject go = NDLoad.LoadWndItem("NOExistHeroItem", pbbItem.MyHead.Table.transform);
                if (go != null) {
                    NOExistHeroItem item = go.GetComponent<NOExistHeroItem>();
                    if (item != null) {
                        item.SetData(info, true);
                        
                        m_CanExistSoldier.Add(item);
                        
                        if (m_tCanSummon == null) {
                            m_tCanSummon = item;
                            item.bGuideSelect = true;
                        }
                    }
                }
                pbbItem.MyHead.Table.Reposition();
            }
            
        }
    }
    /// <summary>
    /// 创建已召唤炮弹兵
    /// </summary>
    private void CreateExistHero()
    {
        int count = 0;
        PdbbbItem pdbbItem = null;
        for (int i = 0; i < m_allExistSoldier.Count; i++) {
            SoldierInfo info = m_allExistSoldier[i];
            if (m_iSelectLocation != -1) {
                if (m_iSelectLocation != info.m_loaction) {
                    continue;
                }
            }
            if (info == null) {
                continue;
            }
            m_iCreatCount++;
            if (count % 4 == 0) {
                pdbbItem = null;
                GameObject go = NDLoad.LoadWndItem("PdbbbItem", MyHead.Parent.transform);
                pdbbItem = go.GetComponent<PdbbbItem>();
                if (pdbbItem != null) {
                    pdbbItem.MyHead.Table.pivot = UITablePivot.Pivot.Left;
                }
            }
            count ++;
            if (pdbbItem != null && pdbbItem.MyHead.Table != null) {
                GameObject go = NDLoad.LoadWndItem("ExistHeroItem", pdbbItem.MyHead.Table.transform);
                if (go != null) {
                    ExistHeroItem item = go.GetComponent<ExistHeroItem>();
                    if (item != null) {
                        item.SetData(info);
                        m_ExistSoldier.Add(item);
                        if (info.CheckSoldierJinJie() && m_CanJinJieitem == null) {
                            m_CanJinJieitem = item;
                            item.BGuideSelect = true;
                        }
                        if (info.CheckHaveSkillUp() && m_tCanSkillUp == null) {
                            m_tCanSkillUp = item;
                            item.BGuideSelect = true;
                        }
                    }
                }
                pdbbItem.MyHead.Table.Reposition();
            }
            
        }
    }
    private void CreateBanner()
    {
        GameObject go = NDLoad.LoadWndItem("PdbbbItem", MyHead.Parent.transform);
        PdbbbItem Pitem = go.GetComponent<PdbbbItem>();
        if (Pitem != null && Pitem.MyHead.Table != null) {
            GameObject Banner = NDLoad.LoadWndItem("PdbbbBanner", Pitem.MyHead.Table.transform);
            PdbbbBanner banner = Banner.GetComponent<PdbbbBanner>();
            if (banner != null) {
                banner.SetLabelText(NGUIUtil.GetStringByKey(10000027));
            }
            Pitem.MyHead.Table.Reposition();
            Pitem.MyHead.Table.repositionNow = true;
            UIPanel panel = MyHead.Parent.transform.parent.gameObject.GetComponent<UIPanel>();
            float x = 150f;
            if (panel != null) {
                UISprite spr = Pitem.GetComponentInChildren<UISprite>();
                if (spr != null) {
                    x = (panel.width - spr.width) / 2;
                }
            }
            Pitem.MyHead.Table.transform.localPosition = new Vector3(x, Pitem.MyHead.Table.transform.localPosition.y, Pitem.MyHead.Table.transform.localPosition.z);
        }
    }
    
    private void CreateEmpty(Transform t)
    {
        if (t != null) {
            GameObject go = NDLoad.LoadWndItem("ExistHeroItem", t);
            if (go != null) {
                ExistHeroItem item = go.GetComponent<ExistHeroItem>();
                if (item != null) {
                    item.SetData(null);
                    m_ExistSoldier.Add(item);
                }
            }
        }
        
    }
    /// <summary>
    /// 创建未召唤炮弹兵
    /// </summary>
    private void CreateNoExistHero()
    {
        int count = 0;
        PdbbbItem pbbItem = null;
        bool bHaveNot = false;
        for (int i = 0; i < m_allNoExistSoldierInfo.Count; i++) {
            SoldierInfo info = m_allNoExistSoldierInfo[i];
            if (m_iSelectLocation != -1) {
                if (m_iSelectLocation != info.m_loaction) {
                    continue;
                }
            }
            if (bHaveNot == false) {
                bHaveNot = true;
                CreateBanner();
            }
            if (count % 4 == 0) {
                pbbItem = null;
                GameObject go = NDLoad.LoadWndItem("PdbbbItem", MyHead.Parent.transform);
                pbbItem = go.GetComponent<PdbbbItem>();
                if (pbbItem != null) {
                    pbbItem.MyHead.Table.pivot = UITablePivot.Pivot.Left;
                }
            }
            count ++;
            if (pbbItem != null && pbbItem.MyHead.Table != null) {
            
                GameObject go = NDLoad.LoadWndItem("NOExistHeroItem", pbbItem.MyHead.Table.transform);
                if (go != null) {
                    NOExistHeroItem item = go.GetComponent<NOExistHeroItem>();
                    if (item != null) {
                        item.SetData(info, false);
                        m_NoExistSoldier.Add(item);
                    }
                }
                pbbItem.MyHead.Table.Reposition();
            }
            
        }
    }
    /// <summary>
    /// 是否需要创建分割线
    /// </summary>
    /// <returns>true 当前不存在未召唤炮弹兵</returns>
    private bool CheckNoExistHero()
    {
        if (m_CanExistSoldier.Count == 0 && m_ExistSoldier.Count == 0) {
            return false;
        }
        
        int count = 0;
        foreach (SoldierInfo info in m_allNoExistSoldierInfo) {
            if (m_iSelectLocation != -1) {
                if (m_iSelectLocation != info.m_loaction) {
                    continue;
                }
            }
            count++;
        }
        return count != 0;
    }
    
    private void BtnClose_OnClickEventHandler(UIButton sender)
    {
        WndManager.DestoryDialog<PdbbbWnd>();
        WndManager.ShowAllWnds(true);
        MainCameraM.s_Instance.EnableDrag(true);
    }
    
    private void SetSoldierCount()
    {
        int existCount = m_allExistSoldier.Count;
        int canExistCount = m_allCanExistSoldierInfo.Count;
        int noExistCount = m_allNoExistSoldierInfo.Count;
        int totalCount = existCount + canExistCount + noExistCount;
        NGUIUtil.SetLableText<string>(MyHead.LblCount, string.Format("{0}/{1}", existCount, totalCount));
    }
    
    /// <summary>
    /// 全部
    /// </summary>
    public void TogAll_OnChangeEventHandler()
    {
        if (!MyHead.TogAll.value) {
            return;
        }
        if (m_iSelectLocation == -1) {
            return;
        }
        MyHead.LblCount.parent.gameObject.SetActive(true);
        m_iSelectLocation = -1;
        CreateHeroItems();
        
    }
    /// <summary>
    /// 力量筛选
    /// </summary>
    public void TogTank_OnChangeEventHandler()
    {
        if (!MyHead.TogTank.value) {
            return;
        }
        if (m_iSelectLocation == 1) {
            return;
        }
        MyHead.LblCount.parent.gameObject.SetActive(false);
        m_iSelectLocation = 1;
        CreateHeroItems();
        
    }
    /// <summary>
    /// 敏捷筛选
    /// </summary>
    public void TogDPS_OnChangeEventHandler()
    {
        if (!MyHead.TogDPS.value) {
            return;
        }
        if (m_iSelectLocation == 2) {
            return;
        }
        MyHead.LblCount.parent.gameObject.SetActive(false);
        m_iSelectLocation = 2;
        CreateHeroItems();
        
    }
    /// <summary>
    /// 智力筛选
    /// </summary>
    public void TogAssisst_OnChangeEventHandler()
    {
        if (!MyHead.TogAssisst.value) {
            return;
        }
        if (m_iSelectLocation == 3) {
            return;
        }
        MyHead.LblCount.parent.gameObject.SetActive(false);
        m_iSelectLocation = 3;
        CreateHeroItems();
        
    }
    //卷轴动画.
    /// <summary>
    /// 显示toggle动画
    /// </summary>
    /// <param name="delay">开始显示第一个toggle的延时</param>
    /// <param name="interval">显示下一个toggle的间隔</param>
    void ShowTogglesAni(float delay, float interval)
    {
        GameObjectActionExcute gae = GameObjectActionExcute.CreateExcute(gameObject);
        float waitInterval = delay;
        for (int i = 0; i < MyHead.ToggleList.Length; i++) {
            GameObjectActionWait wait = new GameObjectActionWait(waitInterval, WaitFinish);
            wait.Data1 = i;
            gae.AddAction(wait);
            waitInterval = interval;
        }
    }
    /// <summary>
    /// 顺序显示toggle
    /// </summary>
    private void WaitFinish(object o)
    {
        GameObject go = o as GameObject;
        if (go == null) {
            return;
        }
        GameObjectActionExcute gae = go.GetComponent<GameObjectActionExcute>();
        if (gae) {
            GameObjectActionWait wait = gae.GetCurrentAction() as GameObjectActionWait;
            if (wait != null) {
                int index = (int)wait.Data1;
                NGUIUtil.SetActive(MyHead.ToggleList[index].gameObject, true);
            }
        }
    }
    /// <summary>
    ///  是否为全屏窗口
    /// </summary>
    public override bool IsFullWnd()
    {
        return true ;
    }
}
