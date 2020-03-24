using UnityEngine;
using System.Collections;

public class CaptionScrollItem : MonoBehaviour {

	public CaptionInfo m_caption;

	public CaptionScrollItem_h MyHead;
	void Awake()
	{
		MyHead = GetComponent<CaptionScrollItem_h>();
	}

	// Use this for initialization
	 void Start () {

        if (MyHead.BtnSelect)
		{
            MyHead.BtnSelect.OnClickEventHandler += BtnSelect_OnClickEventHandler;
		}
        MyHead.LblSkillName.text = "";
	}
	
	void BtnSelect_OnClickEventHandler(UIButton sender)
	{
        BlackScienceChoWnd bsWnd = WndManager.FindDialog<BlackScienceChoWnd>();
        if (bsWnd)
        {
            bsWnd.SetSelectCaptain(m_caption);
            MyHead.Maskgo2.SetActive(true);
        }
	}
	public void SetCaption(CaptionInfo c)
	{
        m_caption = c;
		gameObject.name = c.m_captionid.ToString();
		NGUIUtil.Set2DSprite(MyHead.SprCaptainHead, "Textures/role/", c.m_captionid.ToString());
        if (MyHead.LblSkillName)
        {
            MyHead.LblSkillName.text = " ";
        }
	}
	public void SetMaskActive(bool isactive)
	{
        MyHead.Maskgo.SetActive(isactive);
	}
}
