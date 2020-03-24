using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 
/// <From> </From>
/// <Author>QFord</Author>
/// </summary>
public class SoldierScrollItem : WndBase {

	public SoldierInfo m_info;
	
	
	private int m_iItemIndex = -1;
	
	public int ItemIndex
	{
		set { m_iItemIndex = value; }
		get { return m_iItemIndex; }
	}


	public SoldierScrollItem_h MyHead
	{
		get 
		{
			return (base.BaseHead () as SoldierScrollItem_h);
		}
	}

	void Start () 
	{
		if (MyHead.BtnSelect)
        {
			MyHead.BtnSelect.OnClickEventHandler += BtnSelect_OnClickEventHandler;
        }
	}

    public void BtnSelect_OnClickEventHandler(UIButton sender)
    {
        //NGUIUtil.DebugLog(string.Format("选取了 炮弹兵："+Info.m_name));
        //SoldierInfoWnd wnd  =  WndManager.FindDialog<SoldierInfoWnd>();
        //if (wnd)
        //{
        //    if (m_info!=null)
        //    {
        //        wnd.SetData(m_info);
        //    }
        //}
        SoldierScrollWnd scrollWnd = WndManager.FindDialog<SoldierScrollWnd>();
        if (scrollWnd)
        {
            scrollWnd.DoSelectItem(transform);
        }
		else
		{
			SelectSoldierwnd selectWnd = WndManager.FindDialog<SelectSoldierwnd>();
			if (selectWnd)
			{
				selectWnd.SelectSoldier(m_info.ID);
			}
		}
    }

    public void SetRoleAlpha(float a)
    {
		MyHead.SprRolePhoto.alpha = a;
		MyHead.SprQuality.alpha = a;
    }

    public void PlayBtnAnimation()
    {
		if (MyHead.SoldierBtnAnimation!=null)
	    {
			ActiveAnimation.Play(MyHead.SoldierBtnAnimation, AnimationOrTween.Direction.Forward);
	    }
    }

    public void SetUI(SoldierInfo Info)
    {
        if (Info != null)
        {
			m_info = Info;
			NGUIUtil.SetLableText<int>(MyHead.LblLevel, Info.Level);
            SetRoleStarLevel(Info.StarLevel);
			NGUIUtil.Set2DSprite(MyHead.SprRolePhoto, "Textures/role/", Info.SoldierTypeID.ToString());
			int quality = ConfigM.GetBigQuality( Info.Quality );
			NGUIUtil.SetSprite(MyHead.SprQuality, quality.ToString());

			if(MyHead.LblQualityPlus)
			{
				MyHead.LblQualityPlus.text = NGUIUtil.GetSmallQualityStr(Info.Quality);
			}
        }
    }

    void SetRoleStarLevel(int starLevel)
    {
        //Debug.Log("PrepareRoleWnd.cs SetStarLevel=" + starLevel);
        if (starLevel < 0 || starLevel > ConstantData.MaxStarLevel)
        {
            Debug.Log("PrepareRoleWnd.cs SetStarLevel=" + starLevel + " 的值非法");
        }
		if (MyHead.StarListParent)
        {
			UISprite[] starSprites = U3DUtil.GetComponentsInChildren<UISprite>(MyHead.StarListParent);
			NGUIUtil.SetStarLevelNum(starSprites,starLevel);

			NGUIUtil.SetStarHidden(starSprites,starLevel);
        }
    }
	public void SetMaskActive(bool isactive)
	{
		MyHead.maskgo.SetActive(isactive);
	}

	public void SetDead()
	{
		MyHead.BtnSelect.enabled = false;
		NGUIUtil.Change2DSpriteGray(MyHead.SprRolePhoto);
	}
}
