using UnityEngine;
using System.Collections;
using sdata;

public class RewardItem : MonoBehaviour {

	public RewardItem_h MyHead;
    private int m_iItemID;
    StageTipWnd m_wnd;
 
	void Awake()
	{
		MyHead = GetComponent<RewardItem_h>();
	}
    void Start()
    {
		if (MyHead.BtnClickItem)
        {
			MyHead.BtnClickItem.OnPressDownEventHandler += BtnClickItem_OnPressDownEventHandler;
			MyHead.BtnClickItem.OnPressUpEventHandler +=BtnClickItem_OnPressUpEventHandler;
        }
    }

    void BtnClickItem_OnPressUpEventHandler(UIButton sender)
    {
        //NGUIUtil.DebugLog(sender.name);
        ClickUp();
    }

    void BtnClickItem_OnPressDownEventHandler(UIButton sender)
    {
        //NGUIUtil.DebugLog(sender.name);
        Vector3 pos = Vector3.zero;
		if (MyHead.T_Center)
        {
			pos = MyHead.T_Center.position;
        }
        ClickDown(pos);
    }

    void ClickDown(Vector3 pos)
    {
		sdata.s_itemtypeInfo Info = ItemM.GetItemInfo(m_iItemID);
        if (Info != null)
        {
            string Name = Info.name;
            int Level = Info.level;
			string Description = Info.title.Replace("\\n", System.Environment.NewLine);
            int Money = Info.money;
			int HaveCount = ItemDC.GetItemCount(m_iItemID);

            m_wnd = WndManager.GetDialog<StageTipWnd>();
			if(Info.gtype == 0)
				m_wnd.SetTipData(pos, StageClickType.Item, Info.icon,Info.quality, Name, Level, Description ,HaveCount ,Money ,Info.gtype);
			else if(Info.gtype == 1)
				m_wnd.SetTipData(pos, StageClickType.Item, Info.gid,Info.quality, Name, Level, Description ,HaveCount ,Money ,Info.gtype);
			if(Info.gtype == 2)
				m_wnd.SetTipData(pos, StageClickType.Item, Info.gid,Info.quality, Name, Level, Description ,HaveCount ,Money ,Info.gtype);
			if(Info.gtype == 3)
				m_wnd.SetTipData(pos, StageClickType.Item, Info.gid,Info.quality, Name, Level, Description ,HaveCount ,Money ,Info.gtype);
		}
	}

    void ClickUp()
    {
        if (m_wnd)
        {
            WndManager.DestoryDialog<StageTipWnd>();
        }
    }

	public void SetRewardItem(int buildtype ,int num)
	{
        m_iItemID = buildtype;
		sdata.s_itemtypeInfo Info = ItemM.GetItemInfo(m_iItemID);
		if(Info == null)
			Debug.Log("不存在该物品：" + buildtype);
		else
		{
			int quality =  ConfigM.GetBigQuality(Info.quality) ;
			SetHead(Info.gtype ,Info.icon,Info.gid);
			if (MyHead.SprQuality != null)
				MyHead.SprQuality.spriteName =  quality.ToString();
			if(MyHead.NumLabel != null )
			{
				if( num > 0) MyHead.NumLabel.text =  num.ToString();
				else MyHead.NumLabel.text =  "";
			}				
		}
	}


	public void SetRewardItem(int itemType, int id, int num)
	{
		if (itemType == 1)//英雄整卡
		{
			s_soldier_typeInfo info = SoldierM.GetSoldierType(id);
			if (info == null)
			{
				NGUIUtil.DebugLog("s_soldiertype id = " + id + " 静态表数据未配置！");
				return;
			}
			else
			{
				SetHead(itemType, 0, info.modeltype);
				MyHead.SprQuality.spriteName = "1";
				MyHead.SprQuality.gameObject.SetActive(true);
			}
		}
		else if (itemType == 2)//道具/碎片
		{
			s_itemtypeInfo info = ItemM.GetItemInfo(id);
			if (info == null)
			{
				NGUIUtil.DebugLog("s_itemtype id = " + id + " 静态表数据未配置！");
				return;
			}
			else
			{
				SetHead(info.gtype, info.icon, info.gid);
				int quality = ConfigM.GetBigQuality(info.quality);
				MyHead.SprQuality.spriteName = quality.ToString();
			}
		}
		else if (itemType == 3)//陷阱
		{
			s_building_typeInfo buildinfo = buildingM.GetBuildType(id);
			MyHead.Roomhead.gameObject.SetActive(true);
			NGUIUtil.Set2DSprite(MyHead.Roomhead, "Textures/room/", buildinfo.modeltype.ToString());
			MyHead.SprQuality.spriteName = "1";
		}
		else if (itemType == 4)//黑科技
		{
			string icon = "Textures/role/" + id;
			MyHead.SprQuality.gameObject.SetActive(true);
			MyHead.CaptainHead.gameObject.SetActive(true);
			NGUIUtil.Set2DSprite(MyHead.CaptainHead, icon);
			MyHead.SprQuality.spriteName = "1";
		}
	}

	public void SetHead(int gtype ,int icon ,int gid )
	{
		if(gtype == 0 && MyHead.Itemhead != null)
		{
			MyHead.Itemhead.gameObject.SetActive(true);
			//Itemhead.spriteName = icon.ToString() ;
			NGUIUtil.Set2DSprite(MyHead.Itemhead, "Textures/item/", icon.ToString());
		}
		if(gtype == 1 && MyHead.Soldierhead != null)
		{
			MyHead.Soldierhead.gameObject.SetActive(true);
			NGUIUtil.Set2DSprite(MyHead.Soldierhead, "Textures/role/", gid.ToString());
		}
		if(gtype == 2 && MyHead.Roomhead != null)
		{
			MyHead.Roomhead.gameObject.SetActive(true);
			NGUIUtil.Set2DSprite(MyHead.Roomhead, "Textures/room/", gid.ToString());
		}
		if(gtype == 3 && MyHead.CaptainHead != null)
		{
			MyHead.CaptainHead.gameObject.SetActive(true);
			NGUIUtil.Set2DSprite(MyHead.CaptainHead, "Textures/item/", gid.ToString());
		}
	}
   
}
