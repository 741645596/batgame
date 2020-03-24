using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CanvasItem : MonoBehaviour {
 
	public CanvasItem_h MyHead
	{
		get 
		{
            return (GetComponent < CanvasItem_h>() );
		}
	}

    /// <summary>
    /// 0 船只编辑  1 船只编辑查看 2 金银岛 3 战斗选兵
    /// </summary>
    private int m_iItemType = 0;

	private List<BuildInfo> m_WareHouse = new List<BuildInfo>();
	private int buildType ;
	private int level;
	private int star;
	private int quality;
	public SoldierInfo Soldier;
    private ShipBuildType m_ShipBuildType;
	public bool m_bSelect = false;
	/// <summary>
	/// 养成界面中显示灰色星星，战斗中隐藏.
	/// </summary>
	public bool m_bIsBattle = false;

    public void BtnSelect_OnClickEventHandler()
    {
        if (m_iItemType == 3)
        {
			if(m_bSelect) return;
			m_bSelect = true;

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
                    selectWnd.SelectSoldier(Soldier.ID);
                }
            }
            return;
        }

        if (m_iItemType != 0)
        {
            return;
        }

        int id = 0;
        if (m_ShipBuildType == ShipBuildType.BuildRoom)
        {
			id = m_WareHouse [0].ID;
        }
        else if (m_ShipBuildType == ShipBuildType.Soldier)
        {
            id = Soldier.ID;
        }
        if (TouchMoveManager.CheckHaveExist(m_ShipBuildType, id))
        {
            return;
        }
        CangKuWnd wnd = WndManager.FindDialog<CangKuWnd>();
        //PlaneClickGrid.HideCangKuUI();
		if (m_ShipBuildType == ShipBuildType.BuildRoom )
        {
			BattleEnvironmentM.CreateBuildingFromWarehouse(m_WareHouse [0]);
			m_WareHouse.RemoveAt(0);
            if (wnd)
            {
                wnd.RefreshTrapUI();

            }
        }
		else if (m_ShipBuildType == ShipBuildType.Soldier)
        {
			ShipPlan P = ShipPlanDC.GetCurShipPlan();
            if (P.CheckSoldierUpMaxCount () == true)
                NGUIUtil.ShowTipWndByKey("88800014", 2.0f);
            else
            {
                BattleEnvironmentM.CreateSoldierFromWarehouse(Soldier.ID);
                wnd.RefreshSoldierUI(CombatLoactionType.ALL);
            }
        }
    }

	// Use this for initialization
	public  void Start () 
    {
        if (MyHead.BtnSelect)
        {
            MyHead.BtnSelect.OnClickEventHandler += BtnSelect_OnClickEventHandler;
        } 
	}


	public bool Equalof(BuildInfo Info)
	{
		return Info.EqualOf(buildType , level , star , quality);
	}

	public void SetCanvasItem(BuildInfo Info,bool IsBattle = true)
	{
		m_bIsBattle = IsBattle;
		if(m_WareHouse .Count == 0)
		{
			m_WareHouse.Add(Info);
			buildType = Info.BuildType;
			level = Info.Level;
			star = Info.StarLevel;
		    quality = Info.Quality;
		}
		else
		{
			if(Info.EqualOf(buildType , level , star , quality) == true)
			{
				m_WareHouse.Add(Info);
			}
		}
		m_ShipBuildType = ShipBuildType.BuildRoom ;
		SetUI();
	}
    /// <summary>
    /// 0 船只编辑  1 船只编辑查看 2 金银岛 3战斗选兵
    /// </summary>
	public void SetCanvasItem(SoldierInfo Info,int type = 0,bool IsBattle = true)
	{
		m_bIsBattle = IsBattle;
        m_iItemType = type;
		Soldier = Info;
		m_ShipBuildType = ShipBuildType.Soldier ;
		SetUI();
	}

    private void SetUI()
    {
		MyHead.SprBearBg.gameObject.SetActive(false);
		MyHead.SprDestroy.gameObject.SetActive(false);

		if( m_ShipBuildType == ShipBuildType.BuildRoom )
        {
			if(m_WareHouse.Count == 0) return ;
			BuildInfo roomBuild =  m_WareHouse[0];
			if(roomBuild != null)
			{
				NGUIUtil.Set2DSprite(MyHead.SprItem, "Textures/room/", roomBuild.BuildType.ToString());
                NGUIUtil.SetLableText<int>(MyHead.LblNumber, m_WareHouse.Count);
				NGUIUtil.SetLableText<int>(MyHead.LblLevel, level);
				//白绿蓝紫橙
				NGUIUtil.SetSprite(MyHead.SprQuality, ConfigM.GetBigQuality(quality));
                SetSmallQuality(MyHead.LblSmallQuality, quality);
				NGUIUtil.Set2DSprite(MyHead.Shape, "Textures/shape/", "shape" + roomBuild.m_Shape.shape.ToString());
				NGUIUtil.SetStarLevelNum (MyHead.SprsStar,roomBuild.StarLevel);

				MyHead.LblBearPts.text = roomBuild.m_bear.ToString();
				if(m_bIsBattle)
					NGUIUtil.SetStarHidden(MyHead.SprsStar,roomBuild.StarLevel);
			}
        }
		else
		{
            if (m_iItemType == 2)
            {
                //设定血条
                //MyHead.SprHP.fillAmount = 
                NGUIUtil.SetActive(MyHead.SprHP.gameObject, true);
            }

			HiddenTrapNumLabel();
			if(Soldier == null) return ;
            MyHead.Shape.gameObject.SetActive(false);
            NGUIUtil.Set2DSprite(MyHead.SprItem, "Textures/role/", Soldier.SoldierTypeID.ToString());
            NGUIUtil.SetLableText<int>(MyHead.LblLevel, Soldier.Level);
            NGUIUtil.SetSprite(MyHead.SprQuality, ConfigM.GetBigQuality(Soldier.Quality));
			NGUIUtil.SetSprite(MyHead.SprQualityBg, ConfigM.GetBigQuality(Soldier.Quality));
            SetSmallQuality(MyHead.LblSmallQuality, Soldier.Quality);
			NGUIUtil.SetStarLevelNum (MyHead.SprsStar,Soldier.StarLevel);
			MyHead.LblDestroyPts.text = Soldier.m_concussion.ToString();


			if(m_bIsBattle)
				NGUIUtil.SetStarHidden(MyHead.SprsStar,Soldier.StarLevel);
		} 
    }
	public void ShowBearDestroy(bool show)
	{
		if(m_ShipBuildType == ShipBuildType.BuildRoom)
		{
			MyHead.SprBearBg.gameObject.SetActive(show);
			MyHead.SprDestroy.gameObject.SetActive(false);
		}
		else
		{
			MyHead.SprBearBg.gameObject.SetActive(false);
			MyHead.SprDestroy.gameObject.SetActive(show);
		}
	}
	public void HiddenTrapNumLabel()
	{
		MyHead.LblNumber.gameObject.SetActive(false);
	}
    /// <summary>
    /// 设置小阶
    /// </summary>
    private void SetSmallQuality(UILabel lbl, int Quality)
    {

		lbl.text = NGUIUtil.GetSmallQualityStr(Quality);
       
    }
    /// <summary>
    /// 选兵 设定遮罩显示
    /// </summary>
    /// <param name="isactive"></param>
    public void SetMaskActive(bool isactive)
    {
        // 选定设定颜色为RGB=150
        Color gray = ColorUtils.FromArgb(255,150,150,150);
        Color white = ColorUtils.FromArgb(255,255,255,255);
        Color green = ColorUtils.FromArgb(255, 148, 255, 43);

        NGUIUtil.Set2dSpriteColor(MyHead.SprItem, isactive == true ? gray : white);
        NGUIUtil.SetSpriteColor(MyHead.SprQuality, isactive == true ? gray : white);
        NGUIUtil.Set2dSpriteColor(MyHead.Shape, isactive == true ? gray : white);
        MyHead.LblSmallQuality.color = isactive == true ? gray : green;
        foreach (UISprite s in MyHead.SprsStar)
        {
            NGUIUtil.SetSpriteColor(s, isactive == true ? gray : white);
        }
        
        NGUIUtil.SetActive(MyHead.Mask, isactive);
    }

    public void SetDead(int SoldierTypeID,SelectSoldierwnd wnd)
    {
		if( wnd!= null)
		{
			NGUIUtil.SetPanelClipping(wnd.MyHead.PanelMask, UIDrawCall.Clipping.None);
		}
		GetComponent<Collider>().enabled = false;
        //MyHead.BtnSelect.enabled = false;
		NGUIUtil.Set2DSpriteGraySV(MyHead.SprItem,"Textures/role/", SoldierTypeID.ToString());
    }

    void OnDestroy()
    {
        m_WareHouse.Clear();
    }
	
}
