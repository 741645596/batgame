using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoleRewardItem : MonoBehaviour {

	public RoleRewardItem_h MyHead
	{
		get 
		{
			return (GetComponent<RoleRewardItem_h>());
		}
	}
    private StageTipWnd m_wnd;
    private SoldierInfo m_info;

	public void SetRoleRewardItem(stage.StageSoldierSettle Info ,bool UpLine = true)
	{
		if(Info == null) return ;
		SoldierInfo soldier = SoldierDC.GetSoldiers(Info.soldierid);
		m_info = soldier;
		if(soldier  == null) return ;

		SetSoldierQuality(m_info.Quality);

		if(MyHead.level != null)
			MyHead.level.text =  "[ffffff]" + soldier.Level + "[-]";
		//
		if(MyHead.exp != null)
			MyHead.exp.text = "[ffffff]+" + Info.exp + "[-]";


		if(MyHead.m_star != null && MyHead.m_star.Length == 5 && soldier.StarLevel <= 5)
		{
			NGUIUtil.SetStarLevelNum(MyHead.m_star,soldier.StarLevel);
			NGUIUtil.SetStarHidden(MyHead.m_star,soldier.StarLevel);
		}

		NGUIUtil.Set2DSprite(MyHead.head, "Textures/role/", soldier.m_modeltype.ToString());


		if(MyHead.expline != null)
		{
			int Needexp = SoldierM.GetUpLevelNeedExp(soldier.Level);
			float value = soldier.EXP * 1.0f / Needexp ;
			if(UpLine)
				NGUIUtil.UpdateFromValue(MyHead.expline, value);
			else 
				MyHead.expline.fillAmount = System.Convert.ToSingle(value);
		}
		if (MyHead.BtnShowTip)
        {
			MyHead.BtnShowTip.OnPressDownEventHandler += BtnShowTip_OnPressDownEventHandler;
			MyHead.BtnShowTip.OnPressUpEventHandler += BtnShowTip_OnPressUpEventHandler;
        }
	}

    public void SetRoleRewardItem(SoldierSettlement Info, bool UpLine = true)
    {
        if (Info == null) return;
        SoldierInfo soldier = SoldierDC.GetSoldiers(Info.mSoldierid);
        m_info = soldier;
        if (soldier == null) return;

        SetSoldierQuality(m_info.Quality);

        if (MyHead.level != null)
            MyHead.level.text = "[ffffff]" + soldier.Level + "[-]";
        //
        if (MyHead.exp != null)
            MyHead.exp.text = "[ffffff]+" + Info.mExp + "[-]";


        if (MyHead.m_star != null && MyHead.m_star.Length == 5 && soldier.StarLevel <= 5)
        {
            NGUIUtil.SetStarLevelNum(MyHead.m_star, soldier.StarLevel);
            NGUIUtil.SetStarHidden(MyHead.m_star, soldier.StarLevel);
        }

        NGUIUtil.Set2DSprite(MyHead.head, "Textures/role/", soldier.m_modeltype.ToString());


        if (MyHead.expline != null)
        {
            int Needexp = SoldierM.GetUpLevelNeedExp(soldier.Level);
            float value = soldier.EXP * 1.0f / Needexp;
            if (UpLine)
                NGUIUtil.UpdateFromValue(MyHead.expline, value);
            else
                MyHead.expline.fillAmount = System.Convert.ToSingle(value);
        }
        if (MyHead.BtnShowTip)
        {
            MyHead.BtnShowTip.OnPressDownEventHandler += BtnShowTip_OnPressDownEventHandler;
            MyHead.BtnShowTip.OnPressUpEventHandler += BtnShowTip_OnPressUpEventHandler;
        }
    }


	void SetSoldierQuality(int quality)
	{
		int bigLevel = ConfigM.GetBigQuality(quality);

		MyHead.LblSmallQuality.text = NGUIUtil.GetSmallQualityStr(quality);

		NGUIUtil.SetSprite(MyHead.SpriBigQuality, bigLevel.ToString());
		NGUIUtil.SetSprite(MyHead.SprRolePhotoBg, bigLevel.ToString());

	}
    void BtnShowTip_OnPressUpEventHandler(UIButton sender)
    {
        ClickUp();
    }

    void BtnShowTip_OnPressDownEventHandler(UIButton sender)
    {
        Vector3 pos = Vector3.zero;
		if (MyHead.T_Center)
        {
			pos = MyHead.T_Center.position ;
        }
        ClickDown(pos);
    }

    void ClickDown(Vector3 pos)
    {
        if (m_info == null)
        {
            return;
        }
        
        int TipID = m_info.SoldierTypeID;
        string Name = m_info.m_name;
        int Level = m_info.Level;
        string Description = m_info.m_desc;

        m_wnd = WndManager.GetDialog<StageTipWnd>();
        m_wnd.SetRoleTipData(pos, TipID, Name, Level,m_info.Quality, Description);
        
    }

    void ClickUp()
    {
        if (m_wnd)
        {
            WndManager.DestoryDialog<StageTipWnd>();
        }
    }

	public void SetRoleRewardItem(SoldierInfo soldier )
	{
		if(soldier == null) return ;
		
		if(MyHead.level != null)
			MyHead.level.text =  "[ffffff]" + soldier.Level + "[-]";
		SetSoldierQuality(soldier.Quality);
		//
		if(MyHead.exp != null)
			MyHead.exp.text = "[ffffff]+0"  + "[-]";
		
		if(MyHead.m_star != null && MyHead.m_star.Length == 5 && soldier.StarLevel <= 5)
		{
			NGUIUtil.SetStarLevelNum( MyHead.m_star,soldier.StarLevel);
		}
		NGUIUtil.Set2DSprite(MyHead.head, "Textures/role/", soldier.m_modeltype.ToString());

		if(MyHead.expline != null)
		{
			int Needexp = SoldierM.GetUpLevelNeedExp(soldier.Level);
			float value = soldier.EXP * 1.0f / Needexp ;
			NGUIUtil.UpdateFromValue(MyHead.expline, value);
		}
	}
}
