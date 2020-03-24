using UnityEngine;
using System.Collections.Generic;

/// <summary>
///  黑科技窗口
/// <From> </From>
/// <Author>QFord</Author>
/// </summary>
public class BlackScienceChoWnd : WndBase
{

    public BlackScienceChoWnd_h MyHead {
        get
        {
            return (base.BaseHead() as BlackScienceChoWnd_h);
        }
    }
    
    private Dictionary<int, CaptionScrollItem> m_CaptionList = new Dictionary<int, CaptionScrollItem>();
    private CaptionScrollItem CurCaption;
    private int m_selectCaptainID = 0;
    
    public override void WndStart()
    {
        base.WndStart();
        MyHead.BtnBackground.OnClickEventHandler += BtnBackground_OnClickEventHandler;
        MyHead.BtnNO.OnClickEventHandler += BtnBackground_OnClickEventHandler;
        MyHead.BtnYes.OnClickEventHandler += BtnYes_OnClickEventHandler;
        
        List<CaptionInfo> caplist = new List<CaptionInfo>();
        BlackScienceDC.GetCaptions(ref caplist, UserDC.GetLevel());
        LoadCapHeadList(caplist, MyHead.Parent);
        
        WndEffects.DoWndEffect(gameObject);
    }
    
    void BtnYes_OnClickEventHandler(UIButton sender)
    {
        if (m_selectCaptainID == 0) {
            return;
        }
        //        NGUIUtil.ShowFreeSizeTipWnd(30000037, SaveCallBack, 2);
        CangKuWnd cangkuWnd = WndManager.FindDialog<CangKuWnd>();
        if (cangkuWnd) {
            cangkuWnd.SetCaption(m_selectCaptainID);
        }
        SelectSoldierwnd soldierWnd = WndManager.FindDialog<SelectSoldierwnd>();
        if (soldierWnd != null) {
            soldierWnd.SetBlackScienceID(m_selectCaptainID);
        }
        SaveCallBack();
    }
    void Close(object o)
    {
        WndManager.DestoryDialog<BlackScienceChoWnd>();
    }
    void SaveCallBack()
    {
        WndEffects.DoCloseWndEffect(gameObject, Close);
    }
    
    public void SetSelectCaptain(CaptionInfo Info)
    {
        if (Info == null) {
            foreach (int captionid in m_CaptionList.Keys) {
                m_selectCaptainID = captionid;
                break;
            }
        } else {
            m_selectCaptainID = Info.m_captionid;
        }
        
        foreach (CaptionScrollItem item in m_CaptionList.Values) {
            if (item.m_caption.m_captionid == m_selectCaptainID) {
                item.MyHead.Maskgo2.SetActive(true);
                SetUI(item.m_caption);
            } else {
                item.MyHead.Maskgo2.SetActive(false);
            }
        }
    }
    
    public void SetUI(CaptionInfo c)
    {
        if (c != null) {
            NGUIUtil.SetLableText<string>(MyHead.LblSkillName, c.GetGodSkillInfo().m_name);
            NGUIUtil.SetLableText<int>(MyHead.LblSkillLevel, c.GetGodSkillInfo().m_level);
            NGUIUtil.SetLableText<string>(MyHead.LblSkillDesc, string.Format("[552d0a]" + c.GetGodSkillInfo().m_explain + "[-]"));
        }
    }
    
    void BtnBackground_OnClickEventHandler(UIButton sender)
    {
        SaveCallBack();
    }
    
    /// <summary>
    /// 清空头像列表
    /// </summary>
    private void EmptyCapHeadList(GameObject Parent)
    {
        if (Parent == null) {
            return;
        }
        m_CaptionList.Clear();
        U3DUtil.DestroyAllChild(Parent);
    }
    
    /// <summary>
    /// 加载头像列表
    /// </summary>
    private void LoadCapHeadList(List<CaptionInfo> l, Transform parent)
    {
        if (parent == null) {
            return;
        }
        EmptyCapHeadList(parent.gameObject);
        if (l == null || l.Count == 0) {
            return;
        }
        foreach (CaptionInfo capInfo in l) {
            GameObject go = NDLoad.LoadWndItem("CaptionScrollItem", parent);
            if (go != null) {
                CaptionScrollItem item = go.GetComponent<CaptionScrollItem>();
                if (item) {
                    m_CaptionList.Add(capInfo.m_captionid, item);
                    item.SetCaption(capInfo);
                    NGUIUtil.SetItemPanelDepth(go, parent.GetComponentInParent<UIPanel>());
                }
            }
            
        }
        NGUIUtil.RepositionTable(parent.gameObject);
    }
    
    
}
