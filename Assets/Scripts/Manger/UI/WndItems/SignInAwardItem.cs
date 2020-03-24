using UnityEngine;
using System.Collections;
using sdata;
/// <summary>
/// 签到项
/// </summary>
/// QFord
public class SignInAwardItem : MonoBehaviour {

	public UISprite SprBg;
	public UISprite SprItemQuality;
	public UI2DSprite Spr2DItem;
	public UISprite SprVipBg;
	public UILabel LblVipDouble;
    public GameObject SprCheck;
    public UILabel LblNum;
    public GameObject SignEffect2001301;
    public UIButton BtnBg;
    public Transform PivotDown;
    public Transform PivotUp;

    private s_signinInfo m_info;
    /// <summary>
    /// 是否已签到
    /// </summary>
    private bool m_bCheck;
    /// <summary>
    /// 是否是当前签到项
    /// </summary>
    private bool m_bSelect = false;
    /// <summary>
    /// 签到项序号
    /// </summary>
    private int m_iIndex = 0;
    /// <summary>
    /// 数据设定
    /// </summary>
    /// <param name="info"></param>
    /// <param name="bCheck">是否已签到</param>
    /// <param name="bSelect">是否选中</param>
	public void SetData(s_signinInfo info,bool bCheck,bool bSelect = false,int index = 0)
    {
        m_info = info;
        m_bCheck = bCheck;
        m_bSelect = bSelect;
        m_iIndex = index;
        UIEventListener.Get(SprBg.gameObject).onClick = ButtonClick;
        if (!bSelect)
        {
            BtnBg.OnPressDownEventHandler += BtnBg_OnPressDownEventHandler;
            BtnBg.OnPressUpEventHandler += BtnBg_OnPressUpEventHandler;
        }
        NGUIUtil.SetActive(SprCheck.gameObject, bCheck);
        SetItemIcon((SignInRewardType)info.reward_type, info.item_type);
        NGUIUtil.SetLableText(LblNum, string.Format("X{0}",info.item_num));
        NGUIUtil.SetActive(SignEffect2001301, bSelect);
        BtnBg.IsTweenTarget = false;
    }

    void BtnBg_OnPressUpEventHandler(UIButton sender)
    {
        WndManager.DestoryDialog<SignInItemTipsWnd>();
    }

    void BtnBg_OnPressDownEventHandler(UIButton sender)
    {
        Vector3 pos = Vector3.zero;
        if (Mathf.Abs(PivotDown.position.y) > 0.5f)
            pos = PivotUp.position;
        else
            pos = PivotDown.position;
        
        SignInItemTipsWnd wnd = WndManager.GetDialog<SignInItemTipsWnd>();
        wnd.SetData(m_info, m_iIndex);
        wnd.transform.position = pos;
    }

    private void ButtonClick(GameObject go)
    {
        if (m_bSelect)
        {
            //发送签到请求
            UserDC.Send_UserSigninRequest();
            SignInGetAwardWnd wnd = WndManager.GetDialog<SignInGetAwardWnd>();
            if (wnd)
                wnd.SetData(m_info);
            m_bCheck = true;
            m_bSelect = false;
            BtnBg.OnPressDownEventHandler += BtnBg_OnPressDownEventHandler;
            BtnBg.OnPressUpEventHandler += BtnBg_OnPressUpEventHandler;
            NGUIUtil.SetActive(SprCheck.gameObject, m_bCheck);
            NGUIUtil.SetActive(SignEffect2001301, false);
        }
    }
    /// <summary>
    /// 设定签到项的图标
    /// </summary>
    /// <param name="type">类型参照ItemM定义的枚举</param>
    private void SetItemIcon(SignInRewardType type, int itemType)
    {
        string path = "";
        switch (type)
        {
            case SignInRewardType.HeroSoulFragment:
            case SignInRewardType.ItemAndEquip:
            case SignInRewardType.TrapFragment:
            case SignInRewardType.BlackScienceFragment:
            case SignInRewardType.BlackScience:
                path = ConstantData.ItemIconPath;
                break;
            case SignInRewardType.Hero:
                path = ConstantData.HeroIconPath;
                break;
            case SignInRewardType.Wood:
            case SignInRewardType.Crystal:
            case SignInRewardType.Diamond:
                NGUIUtil.Set2DSprite(Spr2DItem, ConstantData.CurrencyIconPath, (int)type);
                return;
            case SignInRewardType.GoldCoin:
                NGUIUtil.Set2DSprite(Spr2DItem, ConstantData.CurrencyIconPath, 2);
                return;
            case SignInRewardType.Trap:
                path = ConstantData.TrapIconPath;
                break;
        }
        NGUIUtil.Set2DSprite(Spr2DItem, path, itemType.ToString());
    }
}
