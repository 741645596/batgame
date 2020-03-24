using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 已召唤英雄项  
/// </summary>
public class ExistHeroItem : MonoBehaviour
{
	private ExistHeroItem_h MyHead;

	void Awake()
	{
		MyHead = GetComponent<ExistHeroItem_h>();
	}
    public SoldierInfo Info;//数据
    /// <summary>
    /// 可装备标记（新手引导）
    /// </summary>
    public EquipmentPutType EquipPutType;
    public bool BGuideSelect = false;
    
    public void BtnSelect_OnClickEventHandler(UIButton sender)
    {
        //NGUIUtil.DebugLog(string.Format("选取了 炮弹兵："+Info.m_name));
        if ( Info == null)
        {
            NGUIUtil.DebugLog("ExistHeroItem.cs SoldierInfo == null !!!");
            return;
        }
        //SoldierInfoWnd wnd = WndManager.GetDialog<SoldierInfoWnd>();
        PdbycWnd wnd = WndManager.GetDialog<PdbycWnd>();
        wnd.SetData(Info,true);
        
    }

	void Start () 
    {
        if (MyHead.BtnSelect)
        {
            MyHead.BtnSelect.OnClickEventHandler += BtnSelect_OnClickEventHandler;
        }
        //SetUI();
	}

    public void SetData(SoldierInfo info)
    {
        Info = info;
        SetUI();
    }

    /// <summary>
    /// 设置角色等级
    /// </summary>
    public void SetRoleLevel(int level)
    {
        NGUIUtil.SetLableText<int>(MyHead.LblLevel, level);
    }
    /// <summary>
    /// 设置角色星级
    /// </summary>
    /// <param name="starLevel"></param>
    public void SetRoleStarLevel(int starLevel)
    {
        //Debug.Log("PrepareRoleWnd.cs SetStarLevel=" + starLevel);
        if ( starLevel < 0 || starLevel > ConstantData.MaxStarLevel )
        {
            Debug.Log("PrepareRoleWnd.cs SetStarLevel=" + starLevel + " 的值非法");
        }
     
		NGUIUtil.SetStarLevelNum (MyHead.StarSprites,starLevel);
    }
    /// <summary>
    /// 设置角色头像
    /// </summary>
    /// <param name="id"></param>
    public void SetRolePhoto(int id)
    {
        NGUIUtil.Set2DSprite(MyHead.SprRolePhoto, "Textures/role/", id.ToString());
    }

    /// <summary>
    /// 设置 角色名称 + 彩色品质等级 + 设置角色品质框（背景） + 战斗力
    /// </summary>
    /// <param name="name">角色名称 s_soldierType</param>
    /// <param name="quality">角色品质 d_soldier</param>
    public void SetNameLevel(string name,int quality)
    {
		int bigLevel = ConfigM.GetBigQuality(quality);

		MyHead.LblBigQuality.text = NGUIUtil.GetSmallQualityStr(quality);

        NGUIUtil.SetLableText<string>(MyHead.LblNameLevel, NGUIUtil.GetBigQualityName(name,quality));
        NGUIUtil.SetSprite(MyHead.SprQuality, bigLevel.ToString());
		NGUIUtil.SetSprite(MyHead.SprHeadBigQualityBg, bigLevel.ToString());
        NGUIUtil.SetSprite(MyHead.SprQualityBg, bigLevel.ToString());
        NGUIUtil.SetLableText<string>(MyHead.LblZhanDouLi, string.Format("{0}", Info.m_combat_power));
    }
    /// <summary>
    /// 设置装备图片
    /// </summary>
    /// <param name="listItemID">装备的s_itemtype  id 列表</param>
    public void SetZhuangBei(List<int> listItemID)
    {
        for ( int i = 0 ; i < listItemID.Count ; i++ )
        {
            NGUIUtil.SetSprite(MyHead.SprsZhuangBei[i], i.ToString());
        }
    }

	public void SetZhuangBeiList(List<int> listItemID)
	{
		bool bRedPot = false;
	

		for (int i = 0; i < listItemID.Count; i++)
		{
			int itemTypeID = 0;
			EquipmentPutType type = SoldierM.CheckCanPutEquip(Info,listItemID[i],i,ref itemTypeID);
            EquipPutType = type;
			if(type == EquipmentPutType.CanPut || type == EquipmentPutType.CanCombine || type == EquipmentPutType.CanCombinePut)
			{
				NGUIUtil.Set2DSprite(MyHead.SprZhuangBeiList[i],"Textures/item/","pdbxx_002ic");
			}
			else if(type == EquipmentPutType.HaveCannot)
			{
				NGUIUtil.Set2DSprite(MyHead.SprZhuangBeiList[i],"Textures/item/","pdbxx_003ic");
			}
			else if(type == EquipmentPutType.HavePut)
			{
				ItemTypeInfo info1 = ItemDC.GetItem(listItemID[i]);
				if (info1 == null || info1.Positon == 0)
				{
					NGUIUtil.DebugLog("获取已装备的信息失败 itemID =" + listItemID[i]);
					MyHead.SprZhuangBeiList[i].sprite2D = null;
					continue;
				}
				else
					NGUIUtil.Set2DSprite(MyHead.SprZhuangBeiList[i],"Textures/item/",info1.m_Icon.ToString());
			}
			else
			{
				MyHead.SprZhuangBeiList[i].sprite2D = null;
			}
			if(!bRedPot)
				bRedPot = (type == EquipmentPutType.CanPut) || (type == EquipmentPutType.CanCombinePut);
		}

		MyHead.SprLittleRed.gameObject.SetActive (bRedPot);

	}
    private void SetUI()
    {
        if (Info != null)
        {
            SetRoleLevel(Info.Level);
            SetRoleStarLevel(Info.StarLevel);
            SetRolePhoto(Info.SoldierTypeID);
			NGUIUtil.SetRoleType(MyHead.SprRoleType,Info.m_main_proerty);
            SetNameLevel(Info.m_name, Info.Quality);

            List<int> listEquipID = new List<int>();
            listEquipID.Add(Info.Equipment0);
            listEquipID.Add(Info.Equipment1);
            listEquipID.Add(Info.Equipment2);
            listEquipID.Add(Info.Equipment3);
            listEquipID.Add(Info.Equipment4);
            listEquipID.Add(Info.Equipment5);

            SetZhuangBei(listEquipID);

			SetZhuangBeiList(listEquipID);
        }
        else //隐藏此项
        {
            if (MyHead.BG)
            {
                MyHead.BG.SetActive(false);
            }
        }
    }



	
}
