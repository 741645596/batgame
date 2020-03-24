using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreasureBoatInfoWnd : WndBase {
	List<treasure.TreasureObjInfo> m_status = new List<treasure.TreasureObjInfo>();
	List<battle.SoldierInfo> m_soldiers  = new List<battle.SoldierInfo>();
	bool Updatelist = false;
	bool battack = false;
	public TreasureBoatInfoWnd_h MyHead
	{
		get 
		{
			return (base.BaseHead () as TreasureBoatInfoWnd_h);
		}
	}


	// Use this for initialization
	public override void WndStart () {
		base.WndStart();
	}

	public void SetData(string name,int userlevel, int fightnum,List<battle.SoldierInfo> soldiers ,List<treasure.TreasureObjInfo> statusinfo, bool attck =false)
	{
		MyHead.lblName.text = name;
		MyHead.captainlevel.text = userlevel.ToString();
		MyHead.lblfightnum.text = fightnum.ToString();
		m_status = statusinfo;
		m_soldiers = soldiers;
		Updatelist = true;
		battack = attck;
	}
	public void CreatList()
	{
		U3DUtil.DestroyAllChild(MyHead.table.gameObject,false);
		if ( battack)
		{
			foreach(treasure.TreasureObjInfo si in m_status)
			{
				SoldierInfo soldierInfo = SoldierM.GetSoldierInfo(si);
				SoldierM.GetSoldierInfo(ref soldierInfo);
				CreatItem(soldierInfo);
			}
		}
		else
		{
			foreach(battle.SoldierInfo si in m_soldiers)
			{
				SoldierInfo soldierInfo = SoldierM.GetSoldierInfo(si);
				CreatItem(soldierInfo);
			}
		}
		MyHead.table.Reposition();
	}

	public void CreatItem( SoldierInfo soldierInfo )
	{
		GameObject go = NDLoad.LoadWndItem("CombatRoleItem", MyHead.table.transform);
		//根据类型切换头像
		CombatRoleItem c = go.GetComponent<CombatRoleItem>();
		if(c !=null)
		{
			c.SetSoldierUI(soldierInfo);
			c.m_isPlayer = false;
			c.SetHp(1.0f);
			c.SetAnger(0.0f);
		}
		foreach(treasure.TreasureObjInfo tdi in m_status)
		{
			if (tdi.type == 2 && soldierInfo.ID == tdi.objid)
			{ 
				float hp = tdi.hp == -1 ? 1 : tdi.hp * 1.0f / soldierInfo.m_hp;
				c.SetHp(hp);
				float anger = tdi.mp * 1.0f / ConfigM.GetAngerK(1);
				
				c.SetAnger(anger);
			}
		}
//		NGUIUtil.SetItemPanelDepth(c.gameObject, MyHead.table.GetComponentInParent<UIPanel>());
	}
	public void Update()
	{
		if (Updatelist)
		{
			CreatList();
			Updatelist = false;
		}
	}
}
