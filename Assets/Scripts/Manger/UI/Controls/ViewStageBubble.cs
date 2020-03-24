using UnityEngine;
using System;
using System.Collections;

/// <summary>
///  预览界面泡泡提示
/// <Author>QFord</Author>
/// </summary>
public class ViewStageBubble : MonoBehaviour {

    public UILabel LblText;
    public UIButton BtnBg;
    public TweenAlpha BgTweenAlpha;

    private float m_fHideDuration = 0.1f;
    private bool m_bShow = true;

	public void SetData(CounterBubblePromtInfo info)
    {
        Transform tStart = BattleEnvironmentM.GetLifeMBornNode(true);
        if (tStart!=null)
        {
            NGUIUtil.Set3DUIPos(gameObject, new Vector3(info.x + tStart.position.x, info.y + tStart.position.y, 0f));
        }
        NGUIUtil.SetLableText(LblText, info.text);
    }

    void Start()
    {
        BtnBg.OnClickEventHandler += BtnBg_OnClickEventHandler;
        BtnBg.IsTweenTarget = false;
        m_fHideDuration = ConfigM.GetBubblePromtTime();
    }

    void BtnBg_OnClickEventHandler(UIButton sender)
    {
        if (m_bShow)   //点击隐藏泡泡
        {
            BgTweenAlpha.enabled = true;
            BgTweenAlpha.PlayReverse();
            m_bShow = false;
            StartCoroutine(ShowBubble(m_fHideDuration));
        }
    }
    /// <summary>
    ///  显示泡泡
    /// </summary>
    IEnumerator ShowBubble(float delay)
    {
        yield return new WaitForSeconds(delay);
        BgTweenAlpha.enabled = true;
        BgTweenAlpha.PlayForward();
        m_bShow = true;
    }

}
