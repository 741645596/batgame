using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 
/// <From> </From>
/// <Author>QFord</Author>
/// </summary>
public class FreeNewSkillWnd : WndTopBase {

    public UIButton BtnClose;
    public UI2DSprite SprSkillIcon;
    public UILabel LblSkillName;
    public UILabel LblSkillDesc;

    public void SetData(int skillIconID, string skillName, string skillDesc)
    {
        NGUIUtil.Set2DSprite(SprSkillIcon, "Textures/skill/", skillIconID.ToString());
        NGUIUtil.SetLableText(LblSkillName, skillName);
        NGUIUtil.SetLableText(LblSkillDesc, skillDesc);
    }

    public override void WndStart()
	{
        base.WndStart();
        BtnClose.OnClickEventHandler += BtnClose_OnClickEventHandler;
	}

    void BtnClose_OnClickEventHandler(UIButton sender)
    {
        WndManager.DestoryDialog<SoldierUpQualityWnd>();
        WndManager.DestoryDialog<FreeNewSkillWnd>();
    }
	
	
}
