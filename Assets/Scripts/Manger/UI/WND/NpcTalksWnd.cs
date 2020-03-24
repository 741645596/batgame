using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;


/// <summary>
/// NPG 对话窗口
/// <Author>QFord</Author>
/// </summary>
public class NpcTalksWnd : WndTopBase
{

    public NpcTalksWnd_h MyHead {
        get { return base.BaseHead() as NpcTalksWnd_h; }
    }
    /// <summary>
    /// 窗口类型  0新手引导  1战 前/后 对白
    /// </summary>
    private int m_iWndType = 0;
    
    private int m_iTipIndex = 0;
    private List<int> m_lNpcID = new List<int>();
    private List<string> m_lStrTalks = new List<string>();
    private List<NpcDirection> m_lWndDirs = new List<NpcDirection>();
    private int m_taskid;
    private int m_step;
    private CallBack m_finishCallBack;
    private Vector3 m_v3FollowPos;
    private int m_iStyle;
    private NpcDirection m_direction = NpcDirection.Left;
    
    /// <summary>
    ///  战前/后 对白
    /// </summary>
    /// <param name="npcid">NPC ID</param>
    /// <param name="talkStrs">聊天数据串</param>
    /// <param name="direction">npc左右位置</param>
    public void SetData(List<int> npcIDs, List<string> talkStrs, List<NpcDirection> wndDirections, CallBack callBack = null)
    {
        m_iWndType = 1;
        m_lWndDirs = wndDirections;
        m_lStrTalks = talkStrs;
        m_lNpcID = npcIDs;
        m_finishCallBack = callBack;
        SetWndDirection(wndDirections[0]);
        
        if (MainCameraM.s_Instance) {
            MainCameraM.s_Instance.EnableDrag(false);
        }
        //设置首位NPC头像
        NGUIUtil.Set2DSprite(MyHead.Spr2dNpcHead, "Textures/npc/", m_lNpcID[0]);
        //设置首句对白
        string str = NGUIUtil.GetNewLineStr(m_lStrTalks[0]);
        NGUIUtil.SetLableText(MyHead.LblTalkDesc, str);
    }
    
    /// <summary>
    /// 新手引导
    /// </summary>
    /// <param name="taskid">任务ID</param>
    /// <param name="step">步骤</param>
    /// <param name="jd">json数据</param>
    /// <param name="direction">npc左右位置</param>
    public void SetData(int taskid, int step, JsonData jd, NpcDirection direction = NpcDirection.Left)
    {
        m_taskid = taskid;
        m_step = step;
        SetWndDirection(direction);
        if (jd == null) {
            return;
        }
        
        m_lStrTalks.Clear();
        int count = jd["Talk"].Count;
        for (int i = 0; i < count; i++) {
            string str = (string)jd["Talk"][i]["Text"];
            m_lStrTalks.Add(str);
        }
        
        int npcid = (int)jd["NpcID"];
        //设置NPC头像
        NGUIUtil.Set2DSprite(MyHead.Spr2dNpcHead, "Textures/npc/", npcid);
        //设置NPC名字（现在直接配置到对白中，不需要了）
        //NGUIUtil.SetLableText(MyHead.LblNpcName, m_lStrNames[npcid-1]);
        //设置首句对白
        NGUIUtil.SetLableText(MyHead.LblTalkDesc, m_lStrTalks[0]);
    }
    /// <summary>
    /// 0 透明 1 压黑 2洞洞压黑
    /// </summary>
    /// <param name="style"></param>
    public void SetWndStyle(int style, Vector3 pos)
    {
        m_iStyle = style;
        switch (style) {
            case 0:
            
                break;
                
            case 1:
                NGUIUtil.SetActive(MyHead.BtnStyle1.gameObject, true);
                break;
                
            case 2:
                NGUIUtil.SetActive(MyHead.Style2, true);
                Set3DPos(pos);
                break;
        }
    }
    
    private void Set3DPos(Vector3 pos)
    {
        m_v3FollowPos = pos;
        NGUIUtil.Set3DUIPos(MyHead.Style2, pos);
    }
    public override void WndStart()
    {
        if (MainCameraM.s_Instance) {
            MainCameraM.s_Instance.EnableDrag(false);
        }
        base.WndStart();
        
        MyHead.BtnStyle0.OnClickEventHandler += BtnClick_OnClickEventHandler;
        MyHead.BtnStyle1.OnClickEventHandler += BtnClick_OnClickEventHandler;
        
        //SetWndDirection(NpcDirection.Right);
        MyHead.Style0.width = Screen.width;
        MyHead.Style0.height = Screen.height;
        
        MyHead.End.GetComponent<UIWidget>().ResetAndUpdateAnchors();//适配设定
        MyHead.From.GetComponent<UIWidget>().ResetAndUpdateAnchors();//适配设定
        MyHead.Parent.GetComponent<TweenPosition>().AddOnFinished(TweenMask);
        TweenPostion();
        //NGUIUtil.PauseGame();
    }
    
    void TweenPostion()
    {
        MyHead.TpMainBg.from = MyHead.From.localPosition;
        MyHead.TpMainBg.to = MyHead.End.localPosition;
        MyHead.Parent.position = MyHead.From.localPosition;
        MyHead.TpMainBg.animationCurve = MyHead.TpMainBg1.animationCurve;
        MyHead.TpMainBg.enabled = true;
        MyHead.TpMainBg.PlayForward();
    }
    
    public void TweenMask()
    {
        NGUIUtil.UpdateNPCPanelValue(MyHead.PanelMask, 0f, m_direction);
        if (m_direction == NpcDirection.Right) {
            MyHead.TpScrollBar.duration = 0.39f;
            MyHead.TpScrollBar.delay = 0f;
        } else {
            MyHead.TpScrollBar.duration = 0.42f;
        }
        
        MyHead.TpScrollBar.enabled = true;
        MyHead.TpScrollBar.PlayForward();
    }
    
    void Update()
    {
        if (m_iStyle == 2) {
            Set3DPos(m_v3FollowPos);
        }
    }
    
    void BtnClick_OnClickEventHandler(UIButton sender)
    {
        m_iTipIndex++;
        if (m_iTipIndex >= m_lStrTalks.Count) {
            WndManager.DestoryDialog<NpcTalksWnd>();
            if (m_iWndType == 1) { //开启进入战斗按钮
                ViewStageWnd wnd = WndManager.FindDialog<ViewStageWnd>();
                if (wnd != null) {
                    wnd.MyHead.btnCombat.enabled = true;
                    CmCarbon.StartTalkOver = true;
                }
                if (m_finishCallBack != null) {
                    m_finishCallBack();
                }
            }
        } else {
            if (m_iWndType == 1) {
                SetWndDirection(m_lWndDirs[m_iTipIndex]);
                NGUIUtil.Set2DSprite(MyHead.Spr2dNpcHead, "Textures/npc/", m_lNpcID[m_iTipIndex]);
            }
            NGUIUtil.SetLableText(MyHead.LblTalkDesc, m_lStrTalks[m_iTipIndex]);
            TypewriterEffect twe = MyHead.LblTalkDesc.gameObject.GetComponent<TypewriterEffect>();
            if (twe == null) {
                MyHead.LblTalkDesc.gameObject.AddComponent<TypewriterEffect>();
            } else { //修复第二次打字效果不触发的问题
                DestroyImmediate(twe);
                MyHead.LblTalkDesc.gameObject.AddComponent<TypewriterEffect>();
            }
        }
    }
    
    private void SetWndDirection(NpcDirection direction)
    {
        m_direction = direction;
        if (direction == NpcDirection.Right) {
            MyHead.Parent.localScale = new Vector3(-1f, 1f, 1f);
            MyHead.MainBg.localScale = new Vector3(-1f, 1f, 1f);
            MyHead.PanelMask.transform.localScale = new Vector3(-1f, 1f, 1f);
            Vector3 lPos = MyHead.MainBg.localPosition;
            if (lPos.x < 0) {
                MyHead.MainBg.localPosition = new Vector3(-lPos.x, lPos.y, lPos.z);
            }
        } else {
            MyHead.Parent.localScale = new Vector3(1f, 1f, 1f);
            MyHead.MainBg.localScale = new Vector3(1f, 1f, 1f);
            MyHead.PanelMask.transform.localScale = new Vector3(1f, 1f, 1f);
            Vector3 lPos = MyHead.MainBg.localPosition;
            if (lPos.x > 0) {
                MyHead.MainBg.localPosition = new Vector3(-lPos.x, lPos.y, lPos.z);
            }
            
        }
    }
    
    void OnDestroy()
    {
        if (MainCameraM.s_Instance) {
            MainCameraM.s_Instance.EnableDrag(true);
        }
    }
    
}
