using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoleViewItem : MonoBehaviour {

    public RoleViewItem_h MyHead;
    private StageTipWnd m_wnd;
    private SoldierInfo m_info;
	
    void Awake()
    {
        MyHead = GetComponent<RoleViewItem_h>();
    }

	public void SetRoleRewardItem(SoldierInfo soldier )
	{
		if(soldier == null) return ;

        m_info = soldier;

		if(MyHead.level != null)
			MyHead.level.text =  "[ffffff]" + soldier.Level + "[-]";
		
		if(MyHead.m_star != null && MyHead.m_star.Count == 5 && soldier.StarLevel <= 5)
		{
			for(int i = 0 ; i < soldier.StarLevel ; i ++)
			{
				MyHead.m_star[i].SetActive(true);
			}
		}
        if (MyHead.head != null)
            //head.spriteName = soldier.m_modeltype.ToString() ;
            NGUIUtil.Set2DSprite(MyHead.head, "Textures/role/", soldier.m_modeltype.ToString());

		if (MyHead.BtnShowTip)
        {
			MyHead.BtnShowTip.OnPressDownEventHandler += BtnShowTip_OnPressDownEventHandler;
			MyHead.BtnShowTip.OnPressUpEventHandler += BtnShowTip_OnPressUpEventHandler;
		}
		NGUIUtil.SetSprite(MyHead.SprQuality, ConfigM.GetBigQuality(soldier.Quality));
		int smallquality = ConfigM.GetSmallQuality(soldier.Quality);
		if (smallquality > 0)
			NGUIUtil.SetLableText(MyHead.LblQualityPlus,"+" + smallquality);
		else
			NGUIUtil.SetLableText(MyHead.LblQualityPlus,"");
		NGUIUtil.SetLableText(MyHead.level,soldier.Level);
		//NGUIUtil.SetStarLevelNum (MyHead.m_star,soldier.StarLevel);
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
			pos = MyHead.T_Center.position;
        }
        ClickDown(pos);
    }

    void ClickDown(Vector3 pos)
    {
        if (m_info == null)
        {
            return;
        }

        int TipID = m_info.m_modeltype;
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

}