using UnityEngine;
using System.Collections;

public class BlackScienceItem : MonoBehaviour
{
    /// <summary>
    /// 1:已召唤黑科技 0:未召唤黑科技
    /// </summary>
    public int ItemType = 1;
    public CaptionInfo m_caption;
    public GodSkillInfo Godskill {
        get{return m_caption.GetGodSkillInfo();}
    }
    
    public BlackSciencdWnd m_parent;
    public bool m_CanLevUP = false;
    public bool m_CanStarUP = false;
    private BlackScienceItem_h MyHead;
    
    private int m_iHave;
    private int m_iNeed;
    
    public void CheckUp()
    {
        CheckLevUp();
        CheckStarUp();
        bool topLev = CheckLevTop() && CheckStarTop();
        if (m_CanLevUP || m_CanStarUP) {
            MyHead.sprCanUse.gameObject.SetActive(true);
        } else {
            MyHead.sprCanUse.gameObject.SetActive(false);
        }
        
        if (topLev) {
            MyHead.sprCanUse.gameObject.SetActive(false);
        }
    }
    
    public void ResetUI()
    {
        SetUI();
        Invoke("SetPanelClipping", 0.15f);
    }
    
    void Start()
    {
        MyHead = GetComponent<BlackScienceItem_h>();
        EventDelegate.Add(MyHead.toggle.onChange, Select);
        EventDelegate.Add(MyHead.toggle.onClick, Click);
        SetUI();
        Invoke("SetPanelClipping", 0.15f);
    }
    
    private void SetPanelClipping()
    {
        NGUIUtil.SetPanelClipping(m_parent.MyHead.PanelMask, UIDrawCall.Clipping.None);
    }
    public void SetCaption(CaptionInfo c, BlackSciencdWnd parent)
    {
        m_caption = c;
        m_parent = parent;
    }
    private void SetUI()
    {
        if (ItemType == 1) {
            NGUIUtil.SetActive(MyHead.LblLevelNum.gameObject, true);
            NGUIUtil.SetActive(MyHead.GroupNumPercentage, false);
            NGUIUtil.Set2DSprite(MyHead.sprHead, "Textures/role/", m_caption.m_captionid.ToString());
        } else if (ItemType == 0) {
            NGUIUtil.SetActive(MyHead.LblLevelNum.gameObject, false);
            NGUIUtil.SetActive(MyHead.lblLevelneed.gameObject, false);
            NGUIUtil.SetActive(MyHead.GroupNumPercentage, true);
            NGUIUtil.Set2DSpriteGraySV(MyHead.sprHead, "Textures/role/", m_caption.m_captionid.ToString());
            int itemtype = Godskill.m_needbook;
            m_iHave = ItemDC.GetItemCount(itemtype);
            m_iNeed = GodSkillM.GetCaptainNeedBookNum(m_caption.m_captionid);
            float fillAmount = m_iHave / (m_iNeed * 1.0f);
            NGUIUtil.SetSpriteFillAmount(MyHead.SprNumPercentage, fillAmount);
            SetSummonNum(m_iHave, m_iNeed);
        }
        
        gameObject.name = m_caption.m_captionid.ToString();
        
        if (MyHead.lblName) {
            MyHead.lblName.text = m_caption.m_GodSkill[m_caption.m_godskilltype1].m_name;
        }
        MyHead.sprCanUse.gameObject.SetActive(false);
        if (UserDC.GetLevel() < m_caption.m_levelneed) {
            MyHead.lblLevelneed.text = string.Format(NGUIUtil.GetStringByKey("60000004"), m_caption.m_levelneed.ToString());
            MyHead.toggle.activeSprite.enabled = false;
            MyHead.LblLevelNum.gameObject.SetActive(false);
            MyHead.SprStarparent.SetActive(false);
            SetStarLevel(0);
        } else {
            CheckUp();
            SetStarLevel(m_caption.m_star);
        }
        MyHead.LblLevelNum.text = Godskill.m_level.ToString();
    }
    /// <summary>
    /// 设置星级
    /// </summary>
    private void SetStarLevel(int starLevel)
    {
        NGUIUtil.SetStarLevelNum(MyHead.SprStarList, starLevel);
    }
    /// <summary>
    /// 设置未召唤碎片量
    /// </summary>
    private void SetSummonNum(int have, int need)
    {
        string text = string.Format("{0}/{1}", have, need);
        NGUIUtil.SetLableText(MyHead.LblNumPercentage, text);
    }
    private void CheckLevUp()
    {
        m_CanLevUP = BlackScienceDC.CheckLevUp(m_caption);
    }
    private void CheckStarUp()
    {
        m_CanStarUP = BlackScienceDC.CheckStarUp(m_caption);
    }
    private bool CheckLevTop()
    {
        if (Godskill.m_coinneed == 0 && Godskill.m_woodneed == 0 && Godskill.m_crystalneed == 0) {
            return true;
        } else {
            return false;
        }
    }
    private bool CheckStarTop()
    {
        if (m_caption.m_star == ConstantData.MaxStarLevel) {
            return true;
        }
        
        if (Godskill.m_needbooknum == 0) {
            return true;
        } else {
            return false;
        }
    }
    private void Click()
    {
        DoSelect();
    }
    private void Select()
    {
        if (!MyHead.toggle.value) {
            return;
        }
        DoSelect();
    }
    
    private void DoSelect()
    {
        if (ItemType == 1) {
            if (MyHead.toggle.value && m_CanLevUP == false && m_CanStarUP == false && UserDC.GetLevel() < m_caption.m_levelneed) {
                NGUIUtil.ShowTipWndByKey(10000156);
                return;
            }
            if (MyHead.toggle.value) {
                m_parent.Select(m_caption.m_captionid);
            }
        } else if (ItemType == 0) {
            if (m_iHave < m_iNeed) {
                NGUIUtil.ShowTipWndByKey(30000061);
            } else {
                m_parent.CurSummonCaptianID = m_caption.m_captionid;
                BlackScienceDC.SendCaptainActivateRequest(m_caption.m_captionid);
            }
        }
    }
    
    private void SetSelect()
    {
        if (UserDC.GetLevel() >= m_caption.m_levelneed) {
            MyHead.toggle.value = true;
        }
    }
    private void SetCaptionLevel(int level)
    {
        m_caption.SetCaption(level);
    }
}
