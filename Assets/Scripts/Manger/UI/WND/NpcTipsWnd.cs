using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// NPC所在的位置
/// </summary>
public enum NpcDirection {
    Left = 0,
    Right = 1,
}

/// <summary>
/// NPC 提示窗口(支持九宫格布局，对齐2D和3D对象)
/// <Author>QFord</Author>
/// </summary>
public class NpcTipsWnd : WndTopBase
{

    public NpcTipsWnd_h MyHead {
        get { return base.BaseHead() as NpcTipsWnd_h; }
    }
    
    private int m_iTipIndex = 0;
    private List<string> m_lStrTips = new List<string>();
    private List<string> m_lStrIcon = new List<string>();
    private int m_npcid;
    
    public override void WndStart()
    {
        base.WndStart();
    }
    /// <summary>
    /// 设置2D偏移
    /// </summary>
    /// <param name="offset"></param>
    public void SetOffset(Vector3 offset)
    {
        StartCoroutine(SetOffset1(offset, Time.deltaTime));
    }
    
    IEnumerator SetOffset1(Vector3 offset, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        int height = Screen.height;
        int width = Screen.width;
        float hScale = height / 600f;
        float wScale = width / 1024f;
        offset.x = wScale * offset.x;
        offset.y = hScale * offset.y;
        MyHead.WndAnchorLeft.enabled = false;
        MyHead.WndAnchorRight.enabled = false;
        Vector3 pos = transform.localPosition + offset;
        transform.localPosition = pos;
    }
    
    void BtnMainBg_OnClickEventHandler(UIButton sender)
    {
        m_iTipIndex++;
        if (m_iTipIndex > m_lStrTips.Count - 1) {
            return;
            //WndManager.DestoryDialog<NpcTipsWnd>();
        } else {
            if (m_lStrTips.Count > 0) {
                NGUIUtil.SetLableText(MyHead.LblTipContentLeft, m_lStrTips[m_iTipIndex]);
            } else if (m_lStrIcon.Count > 0) {
            
            }
        }
    }
    /// <summary>
    /// 设置窗口的九种位置
    /// </summary>
    private void SetWndAlign(UIWidget.Pivot align, NpcDirection direction)
    {
        if (direction == NpcDirection.Left) {
            MyHead.SprMainBgLeft.pivot = align;
        }
        
        switch (align) {
            case UIWidget.Pivot.Left:
                if (direction == NpcDirection.Left) {
                    MyHead.WndAnchorLeft.side = UIAnchor.Side.Left;
                } else {
                    MyHead.WndAnchorRight.side = UIAnchor.Side.Left;
                    MyHead.SprMainBgRight.pivot = UIWidget.Pivot.Right;
                }
                break;
                
            case UIWidget.Pivot.Right:
                if (direction == NpcDirection.Left) {
                    MyHead.WndAnchorLeft.side = UIAnchor.Side.Right;
                } else {
                    MyHead.WndAnchorRight.side = UIAnchor.Side.Right;
                    MyHead.SprMainBgRight.pivot = UIWidget.Pivot.Left;
                }
                break;
                
            case UIWidget.Pivot.Top:
                if (direction == NpcDirection.Left) {
                    MyHead.WndAnchorLeft.side = UIAnchor.Side.Top;
                } else {
                    MyHead.WndAnchorRight.side = UIAnchor.Side.Top;
                    MyHead.SprMainBgRight.pivot = UIWidget.Pivot.Top;
                }
                break;
                
            case UIWidget.Pivot.Bottom:
                if (direction == NpcDirection.Left) {
                    MyHead.WndAnchorLeft.side = UIAnchor.Side.Bottom;
                } else {
                    MyHead.WndAnchorRight.side = UIAnchor.Side.Bottom;
                }
                break;
                
            case UIWidget.Pivot.TopLeft:
                if (direction == NpcDirection.Left) {
                    MyHead.WndAnchorLeft.side = UIAnchor.Side.TopLeft;
                } else {
                    MyHead.WndAnchorRight.side = UIAnchor.Side.TopLeft;
                    MyHead.SprMainBgRight.pivot = UIWidget.Pivot.TopRight;
                }
                break;
                
            case UIWidget.Pivot.TopRight:
                if (direction == NpcDirection.Left) {
                    MyHead.WndAnchorLeft.side = UIAnchor.Side.TopRight;
                } else {
                    MyHead.WndAnchorRight.side = UIAnchor.Side.TopRight;
                    MyHead.SprMainBgRight.pivot = UIWidget.Pivot.TopLeft;
                }
                break;
                
            case UIWidget.Pivot.BottomLeft:
                if (direction == NpcDirection.Left) {
                    MyHead.WndAnchorLeft.side = UIAnchor.Side.BottomLeft;
                } else {
                    MyHead.WndAnchorRight.side = UIAnchor.Side.BottomLeft;
                    MyHead.SprMainBgRight.pivot = UIWidget.Pivot.BottomRight;
                }
                break;
                
            case UIWidget.Pivot.BottomRight:
                if (direction == NpcDirection.Left) {
                    MyHead.WndAnchorLeft.side = UIAnchor.Side.BottomRight;
                } else {
                    MyHead.WndAnchorRight.side = UIAnchor.Side.BottomRight;
                    MyHead.SprMainBgRight.pivot = UIWidget.Pivot.BottomLeft;
                }
                break;
        }
    }
    
    /// <summary>
    /// 设置窗口对准3D对象
    /// </summary>
    /// <param name="pos">3D对象的世界坐标</param>
    private void Set3DPos(Vector3 pos)
    {
        NGUIUtil.Set3DUIPos(gameObject, pos);
    }
    /// <summary>
    /// 设置窗口对准2D对象
    /// </summary>
    /// <param name="pos">2D对象的世界坐标</param>
    private void Set2DPos(Vector3 pos)
    {
        transform.position = pos;
    }
    
}
