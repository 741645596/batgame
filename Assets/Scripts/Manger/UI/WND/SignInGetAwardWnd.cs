using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using sdata;
/// <summary>
/// 领取当天奖励
/// </summary>
public class SignInGetAwardWnd : WndBase
{
    public SignInGetAwardWnd_h MyHead
    {
        get
        {
            return (base.BaseHead() as SignInGetAwardWnd_h);
        }
    }

    public override void WndStart()
    {
        base.WndStart();
        MyHead.BtnConfirm.OnClickEventHandler += BtnConfirm_OnClickEventHandler;
    }

    void BtnConfirm_OnClickEventHandler(UIButton sender)
    {
        WndManager.DestoryDialog<SignInGetAwardWnd>();
    }

    public  void SetData(s_signinInfo info)
    {
        string str = string.Format("{0} x{1}", GetName(info), info.item_num);
        NGUIUtil.SetLableText(MyHead.LblItem,str);
        SetItemIcon((SignInRewardType)info.reward_type, info.item_type);
    }

    private string GetName(s_signinInfo info)
    {
        string result = "";
        int id = info.item_type;
        SignInRewardType type = (SignInRewardType) info.reward_type;
        switch (type)
        {
            case SignInRewardType.BlackScience:
                CaptionInfo cInfo = new CaptionInfo();
                GodSkillM.GetCaption(id, ref cInfo);
                GodSkillInfo gInfo = new GodSkillInfo();
                GodSkillM.GetGodSkill(cInfo.m_godskilltype1, 1, ref gInfo);
                result = gInfo.m_name;
                break;

            case SignInRewardType.HeroSoulFragment:
            case SignInRewardType.ItemAndEquip:
            case SignInRewardType.TrapFragment:
            case SignInRewardType.BlackScienceFragment:
                s_itemtypeInfo info1 =  ItemM.GetItemInfo(id);
                if (info1!=null)
                    result = info1.name;
                break;
            case SignInRewardType.Hero:
                SoldierInfo info2 = SoldierM.GetSoldierInfo(id);
                if (info2 != null)
                    result = info2.m_name;
                break;
            case SignInRewardType.Trap:
                BuildInfo info3 = buildingM.GetStartBuildInfo(id);
                if (info3 != null)
                    result = info3.m_name;
                break;
            case SignInRewardType.GoldCoin:
                result = NGUIUtil.GetStringByKey(88800061);
                break;
            case SignInRewardType.Diamond:
                result = NGUIUtil.GetStringByKey(99700001);
                break;
            case SignInRewardType.Crystal:
                result = NGUIUtil.GetStringByKey(10000177);
                break;
            case SignInRewardType.Wood:
                result = NGUIUtil.GetStringByKey(10000178);
                break;
        }
        return result;
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
                NGUIUtil.Set2DSprite(MyHead.Spr2DItem, ConstantData.CurrencyIconPath, (int)type);
                return;
            case SignInRewardType.GoldCoin:
                NGUIUtil.Set2DSprite(MyHead.Spr2DItem, ConstantData.CurrencyIconPath, 2);
                return;
            case SignInRewardType.Trap:
                path = ConstantData.TrapIconPath;
                break;
        }
        NGUIUtil.Set2DSprite(MyHead.Spr2DItem, path, itemType.ToString());
    }
}