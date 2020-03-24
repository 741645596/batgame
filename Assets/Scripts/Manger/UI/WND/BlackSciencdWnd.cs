using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlackSciencdWnd : WndBase
{

    private  Dictionary<int, BlackScienceItem> m_CaptionList = new Dictionary<int, BlackScienceItem>();
    public BlackScienceItem m_CurItem;
    public int CurSummonCaptianID = 0;
    
    public BlackSciencdWnd_h MyHead {
        get
        {
            return (base.BaseHead() as BlackSciencdWnd_h);
        }
    }
    
    public override void WndStart()
    {
        base.WndStart();
        
        DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_1108, CMD_1108);
        
        MyHead.btnUp.OnClickEventHandler += Up;
        MyHead.BtnUpStar.OnClickEventHandler += Up;
        MyHead.btnBack.OnClickEventHandler += Back;
        
        EventDelegate.Add(MyHead.togUpLevel.onChange, UpLevel);
        EventDelegate.Add(MyHead.togUpStar.onChange, UpStar);
        RefreshUI();
        
        MyHead.tBtnUpLevel.isEnabled = false;
        MyHead.tBtnUpStar.isEnabled = false;
        
        ShowTipOrContent(true);
        
        NGUIUtil.UpdatePanelValue(MyHead.PanelMask, 1024, 0.15f);
        NGUIUtil.TweenGameObjectPosX(MyHead.ScrollLeft, -461, 0.15f, gameObject, "HideAniScroll");
        NGUIUtil.TweenGameObjectPosX(MyHead.ScrollRight, 461, 0.15f);
        NGUIUtil.SetMainMenuTop(this);
    }
    
    
    public void RefreshUI()
    {
        U3DUtil.DestroyAllChild(MyHead.SelectCaptionTable.gameObject);
        m_CaptionList.Clear();
        SetUserData();
        MyHead.UpStar.SetActive(false);
        //加载已拥有黑科技
        List<CaptionInfo> caplist = new List<CaptionInfo>();
        BlackScienceDC.GetExistBS(ref caplist);
        LoadCapHeadList(caplist, MyHead.SelectCaptionTable);
        //加载未召唤黑科技
        List<CaptionInfo> caplist1 = new List<CaptionInfo>();
        BlackScienceDC.GetHaveFragmentBS(ref caplist1);
        LoadCapHeadList(caplist1, MyHead.SelectCaptionTable, 0);
    }
    
    /// <summary>
    /// 隐藏左右两个动画轴
    /// </summary>
    public void HideAniScroll()
    {
        NGUIUtil.SetActive(MyHead.ScrollLeft, false);
        NGUIUtil.SetActive(MyHead.ScrollRight, false);
        
        List<ItemTypeInfo> litem = ItemDC.SearchItemListBystype(45);
        if (litem.Count > 0) {
            ItemUseConfirmWnd wnd = WndManager.GetDialog<ItemUseConfirmWnd>();
            if (wnd != null) {
                wnd.SetData(45, litem);
            }
        }
    }
    private void ShowTipOrContent(bool tip)
    {
        MyHead.lblSelectTip.gameObject.SetActive(tip);
        MyHead.Content.SetActive(!tip);
    }
    private void SetUserData()
    {
        if (m_CurItem != null) {
            int  bookcount = ItemDC.GetItemCount(m_CurItem.Godskill.m_needbook);
            MyHead.lblScroll.text = bookcount.ToString();
        } else {
            MyHead.lblScroll.text = "0";
        }
    }
    /// <summary>
    /// 设置角色星级.
    /// </summary>
    public void SetTrapStarLevel(int starLevel)
    {
        //Debug.Log("PrepareRoleWnd.cs SetStarLevel=" + starLevel);
        if (starLevel < 0 || starLevel > ConstantData.MaxStarLevel) {
            Debug.Log("PrepareRoleWnd.cs SetStarLevel=" + starLevel + " 的值非法");
        }
        
        NGUIUtil.SetStarLevelNum(MyHead.SprStarList, starLevel);
    }
    
    /// <summary>
    /// 加载头像列表.
    /// </summary>
    private void LoadCapHeadList(List<CaptionInfo> l, UITable Parent, int itemType = 1)
    {
        if (Parent == null) {
            return;
        }
        
        if (l == null || l.Count == 0) {
            return;
        }
        
        foreach (CaptionInfo  capInfo in l) {
            GameObject go = NDLoad.LoadWndItem("BlackScienceItem", Parent.transform);
            if (go != null) {
                BlackScienceItem item = go.GetComponent<BlackScienceItem>();
                if (item) {
                    m_CaptionList.Add(capInfo.m_captionid, item);
                    item.SetCaption(capInfo, this);
                    item.ItemType = itemType;
                }
            }
        }
        Parent.Reposition();
    }
    
    public void Select(int captionid)
    {
        m_CurItem = m_CaptionList[captionid];
        ShowTipOrContent(false);
        MyHead.tBtnUpLevel.isEnabled = true;
        MyHead.tBtnUpStar.isEnabled = true;
        MyHead.togUpLevel.value = true;
        UpLevel();
        UpStar();
    }
    
    public void UpLevel()
    {
        if (MyHead.togUpLevel.value) {
            MyHead.UpStar.SetActive(false);
            MyHead.UpLevel.SetActive(true);
            MyHead.BtnUpStar.gameObject.SetActive(false);
            MyHead.btnUp.gameObject.SetActive(true);
            MyHead.LblNextName.text = NGUIUtil.GetStringByKey(10000154);
            //MyHead.btnUp.isEnabled = false;
            
            if (m_CurItem != null) {
                m_CurItem.ResetUI();
                MyHead.LblStarName.text = "";
                MyHead.lblName.text = "[71fff0]" + m_CurItem.Godskill.m_name + "[-]  [ffff62]" + NGUIUtil.GetStringByKey("60000005") + m_CurItem.m_caption.m_level1 + "[-]";
                if (m_CurItem.Godskill.m_coinneed == 0 &&  m_CurItem.Godskill.m_crystalneed == 0) {
                    MyHead.lblExplain.text = NGUIUtil.GetStringByKey(10000122);
                } else {
                    MyHead.lblExplain.text = string.Format("[552d0a]" + m_CurItem.Godskill.m_explain + "[-]");
                }
                
                MyHead.lblGoldCost.text = m_CurItem.Godskill.m_coinneed.ToString();
                
                if (UserDC.GetCoin() >= m_CurItem.Godskill.m_coinneed) {
                    MyHead.lblGoldCost.color = Color.white;
                } else {
                    MyHead.lblGoldCost.color = Color.red;
                }
                
                MyHead.lblDiamondCost.text = m_CurItem.Godskill.m_crystalneed.ToString();
                if (UserDC.GetCrystal() >= m_CurItem.Godskill.m_crystalneed) {
                    MyHead.lblDiamondCost.color = Color.white;
                } else {
                    MyHead.lblDiamondCost.color = Color.red;
                }
                m_CurItem.CheckUp();
                //				MyHead.btnUp.isEnabled = m_CurItem.CheckLevTop() && m_CurItem.m_CanLevUP;
                GodSkillInfo nextInfo = new GodSkillInfo();
                
                GodSkillM.GetGodSkill(m_CurItem.Godskill.m_type, m_CurItem.Godskill.m_level + 1, ref nextInfo);
                if (nextInfo != null && nextInfo.m_explain != null) {
                    MyHead.NextStarDes.text = string.Format("[552d0a]" + nextInfo.m_explain + "[-]");
                }
            }
        }
    }
    public void UpStar()
    {
    
        if (MyHead.togUpStar.value) {
            MyHead.UpStar.SetActive(true);
            MyHead.UpLevel.SetActive(false);
            
            MyHead.BtnUpStar.gameObject.SetActive(true);
            MyHead.btnUp.gameObject.SetActive(false);
            MyHead.LblNextName.text = NGUIUtil.GetStringByKey(70000115);
            //MyHead.BtnUpStar.isEnabled = false;
            if (m_CurItem != null) {
                m_CurItem.ResetUI();
                
                MyHead.LblStarName.text = "[71fff0]" + m_CurItem.Godskill.m_name + "[-]";
                MyHead.lblName.text = "";
                MyHead.lblExplain.text = string.Format("[552d0a]" + m_CurItem.Godskill.m_stardescription + "[-]");
                
                int  bookcount = ItemDC.GetItemCount(m_CurItem.Godskill.m_needbook);
                if (m_CurItem.Godskill.m_needbooknum == 0 && m_CurItem.m_caption.m_star == 5) {
                    MyHead.lblScrollNum.text = NGUIUtil.GetStringByKey(10000085);
                    MyHead.SprProgress.fillAmount = 1.0f;
                } else {
                    MyHead.lblScrollNum.text = bookcount + "/" + m_CurItem.Godskill.m_needbooknum;
                    MyHead.SprProgress.fillAmount = bookcount * 1.0f / m_CurItem.Godskill.m_needbooknum;
                }
                
                GodSkillInfo nextInfo = new GodSkillInfo();
                
                GodSkillM.GetGodSkill(m_CurItem.Godskill.m_type, m_CurItem.Godskill.m_level, m_CurItem.m_caption.m_star + 1, ref nextInfo);
                if (m_CurItem.m_caption.m_star == 5) {
                    MyHead.NextStarDes.text = string.Format("[552d0a]" + NGUIUtil.GetStringByKey(10000085) + "[-]");
                } else if (nextInfo != null && nextInfo.m_stardescription != null) {
                    MyHead.NextStarDes.text = string.Format("[552d0a]" + nextInfo.m_stardescription + "[-]");
                }
                
                SetTrapStarLevel(m_CurItem.m_caption.m_star);
                m_CurItem.CheckUp();
                //				MyHead.BtnUpStar.isEnabled = m_CurItem.CheckStarTop();
                UIGrid grid = MyHead.StarParent.GetComponent<UIGrid>();
                grid.repositionNow = true;
            }
        }
    }
    
    
    void Up(UIButton sender)
    {
        if (MyHead.togUpLevel.value) {
            if (m_CurItem.m_CanLevUP) {
                DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_1102, CMD_1102);
                BlackScienceDC.Send_CAPTAIN_LEVEL_UP(m_CurItem.m_caption.m_id);
            } else {
                if (UserDC.GetCoin() < m_CurItem.Godskill.m_coinneed) {
                    NGUIUtil.ShowTipWndByKey("60000021", 2.0f);
                } else if (UserDC.GetCrystal() < m_CurItem.Godskill.m_crystalneed) {
                    NGUIUtil.ShowTipWndByKey("60000023", 2.0f);
                } else if (UserDC.GetLevel() < m_CurItem.m_caption.m_levelneed) {
                    NGUIUtil.ShowTipWndByKey("60000027", 2.0f);
                }
            }
        } else {
            if (m_CurItem.m_CanStarUP) {
                DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_1106, CMD_1106);
                BlackScienceDC.Send_CAPTAIN_STAR_UP(m_CurItem.m_caption.m_id);
            } else {
                NGUIUtil.ShowTipWndByKey("60000024", 2.0f);
            }
        }
    }
    
    void Back(UIButton sender)
    {
        WndManager.DestoryDialog<BlackSciencdWnd>();
    }
    private void CMD_1106(int nErrorCode)
    {
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_1106, CMD_1106);
        
        if (nErrorCode == 0) {
            SetUserData();
            BlackScienceUpstarWnd bsuw = WndManager.GetDialog<BlackScienceUpstarWnd>();
            bsuw.SetData(m_CurItem.m_caption.m_star - 1, m_CurItem.m_caption.m_star, m_CurItem.Godskill.m_explain);
            m_CurItem.SetCaption(m_CurItem.m_caption, this);
            UpStar();
        }
        
    }
    private void CMD_1102(int nErrorCode)
    {
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_1102, CMD_1102);
        
        if (nErrorCode == 0) {
            SetUserData();
            UpLevel();
        }
    }
    /// <summary>
    /// 召唤黑科技回应
    /// </summary>
    private void CMD_1108(int nErrorCode)
    {
        if (nErrorCode == 0) {
            ShowTrophiesAction();
            RefreshUI();
        }
    }
    /// <summary>
    /// 显示首次获得黑科技表现
    /// </summary>
    private void ShowTrophiesAction()
    {
        if (CurSummonCaptianID == 0) {
            return;
        }
        
        CaptionInfo cInfo = new CaptionInfo();
        GodSkillM.GetCaption(CurSummonCaptianID, ref cInfo);
        GodSkillInfo gInfo = new GodSkillInfo();
        //黑科技抽取是1级，王振鑫确认
        GodSkillM.GetGodSkill(cInfo.m_godskilltype1, 1, ref gInfo);
        string name = "";
        name = gInfo.m_name;
        sdata.s_itemtypeInfo  itypeInfo = new sdata.s_itemtypeInfo();
        itypeInfo.gid = CurSummonCaptianID;
        itypeInfo.name = name;
        itypeInfo.gtype = 3;
        
        List<sdata.s_itemtypeInfo> lInfo = new List<sdata.s_itemtypeInfo>();
        lInfo.Add(itypeInfo);
        
        TrophiesActionWnd TropWnd = WndManager.GetDialog<TrophiesActionWnd>();
        if (TropWnd) {
            TropWnd.ClearTropiesData();
            TropWnd.AddTropiesData(lInfo);
            TropWnd.SetWndType(3);
            TropWnd.MyHead.LblDes.gameObject.SetActive(false);
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
