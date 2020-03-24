using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 
/// <From> </From>
/// <Author>QFord</Author>
/// </summary>
public class SoldierUpQualityWndOld : WndTopBase {

    public SoldierUpQualityWndOld_h MyHead
    {
        get
        {
			return (base.BaseHead() as SoldierUpQualityWndOld_h);
        }
    }
	
	public override void WndStart()
	{
		base.WndStart();
        MyHead.BtnClose.OnClickEventHandler += BtnClose_OnClickEventHandler;
		WndEffects.DoWndEffect(gameObject);
	}
	void DestoryDialogCallBack(object o)
	{
		WndManager.DestoryDialog<SoldierUpQualityWndOld>();
	}
	void BtnClose_OnClickEventHandler(UIButton sender)
    {

		WndEffects.DoCloseWndEffect(gameObject,DestoryDialogCallBack);
    }
	
    public void HideBtnClose()
    {
        NGUIUtil.SetActive(MyHead.BtnClose.gameObject, false);
    }

    public void SetData(SoldierInfo preInfo ,SoldierInfo curInfo)
    {        
        NGUIUtil.SetLableText<int>(MyHead.LblPreCombatPower, preInfo.m_combat_power);
        NGUIUtil.SetLableText<int>(MyHead.LblCurCombatPower, curInfo.m_combat_power);
        NGUIUtil.SetLableText<int>(MyHead.LblPreHp, preInfo.m_hp);
        NGUIUtil.SetLableText<int>(MyHead.LblCurHp, curInfo.m_hp);

		if(MyHead.AfterItem != null)
		{
			GameObject go = NDLoad.LoadWndItem("CanvasItem", MyHead.AfterItem.transform);
			if (go)
			{
				CanvasItem item = go.GetComponent<CanvasItem>();
				if (item)
				{
					item.SetCanvasItem(curInfo,0,false);
				}
			}
			
		}
		if(MyHead.PreItem != null)
		{
			GameObject go = NDLoad.LoadWndItem("CanvasItem", MyHead.PreItem.transform);
			if (go)
			{
				CanvasItem item = go.GetComponent<CanvasItem>();
				if (item)
				{
					item.SetCanvasItem(preInfo,0,false);
				}
			}
		}

		SetNewSkill (curInfo,preInfo);
    }

	/// <summary>
	/// 设置 角色名称 + 彩色品质等级 + 设置角色品质框（背景） + 战斗力.
	/// </summary>
	/// <param name="name">角色名称 s_soldierType</param>
	/// <param name="quality">角色品质 d_soldier</param>
	public void SetSmallQuality(UILabel labl,int quality)
	{
		labl.text = NGUIUtil.GetSmallQualityStr(quality);

	}
    public void SetLevelSprite(UISprite spr, int quality)
    {
        int bigLevel = ConfigM.GetBigQuality(quality);
        NGUIUtil.SetSprite(spr, bigLevel.ToString());
    }

	public void SetNewSkill(SoldierInfo curInfo,SoldierInfo preInfo)
	{
		int CurSmallQuality = ConfigM.GetSmallQuality (curInfo.Quality);
		int CurBigQuality = ConfigM.GetBigQuality (curInfo.Quality);

		int PreSmallQuality = ConfigM.GetSmallQuality (preInfo.Quality);
		int PreBigQuality = ConfigM.GetBigQuality (preInfo.Quality);

		if(CurSmallQuality == 0 && CurBigQuality > PreBigQuality)
		{
			MyHead.GoShowNewSkill.SetActive(true);
			int skillIndex = ConfigM.GetEnableSkill(curInfo.Quality);
			SoldierSkill info = curInfo.m_Skill.GetSkill(skillIndex);
			if(info != null)
			{
				MyHead.GoShowNewSkill.SetActive(true);
				NGUIUtil.Set2DSprite(MyHead.Spr2DSkillIcon, "Textures/skill/", info.m_type.ToString());
				MyHead.LblSkillDes.text = info.m_description1.Replace("\\n",System.Environment.NewLine);
			}
			else
			{
				MyHead.GoShowNewSkill.SetActive(false);
			}
		}
		else
		{
			MyHead.GoShowNewSkill.SetActive(false);
		}
	}

}
