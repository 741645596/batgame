using UnityEngine;
using System.Collections;

public class TrapIntroduceWnd : WndBase
{
	public TrapIntroduceWnd_h MyHead
	{
		get 
		{
			return (base.BaseHead () as TrapIntroduceWnd_h);
		}
	}
	private BuildInfo m_Info;
	public void SetBuildInfo(BuildInfo info)
	{
		m_Info = info;
	}

	public override void WndStart ()
	{
		base.WndStart ();
		AddLabelItem("[2B5891]"+NGUIUtil.GetStringByKey(10000049)+ GetRoomTypeString(m_Info.m_RoomKind)+"[-]");
		if(m_Info.m_Skill != null)
		   AddLabelItem("[000000]"+m_Info.m_Skill.m_desc+"[-]");
		AddGrayItem ();

		AddDoubleLabelItem (m_Info.m_phyattack.ToString(),"[552d0a]"+NGUIUtil.GetStringByKey(10000053)+"[-]",m_Info.m_phydefend.ToString(),"[552d0a]"+NGUIUtil.GetStringByKey(10000055)+"[-]");
		AddDoubleLabelItem (m_Info.m_Solidity.ToString("0.0"),"[552d0a]"+NGUIUtil.GetStringByKey(10000058)+"[-]",m_Info.m_Intensity.ToString("0.0"),"[552d0a]"+NGUIUtil.GetStringByKey(10000059)+"[-]");
		AddDoubleLabelItem (m_Info.m_Tenacity.ToString("0.0"),"[552d0a]"+NGUIUtil.GetStringByKey(10000060)+"[-]",m_Info.m_hp.ToString(),"[552d0a]"+NGUIUtil.GetStringByKey(10000057)+"[-]");
		AddDoubleLabelItem (m_Info.m_Tenacity.ToString("0.0"),"[552d0a]"+NGUIUtil.GetStringByKey(10000056)+"[-]","","");
		MyHead.table.Reposition ();
	}
	private void AddGrayItem()
	{
		float iSolid = 0; 
		float iIntensity = 0; 
		float iTenacity = 0;
		buildingM.GetStarInfoGrow(m_Info,ref iSolid,ref iIntensity,ref iTenacity);


		GameObject go = GameObjectLoader.LoadPath("Prefabs/UI/", "SkillIntroduceWnd", MyHead.table.transform);
		go.transform.localPosition = new Vector3 (MyHead.table.transform.localPosition.x,go.transform.localPosition.y,go.transform.localPosition.z);
		SkillIntroduceWnd skilWnd = go.GetComponent<SkillIntroduceWnd>();

		GameObject labGo = NDLoad.LoadWndItem("DoubleLabelItem", skilWnd.MyHead.table.transform);
		DoubleLabelItem  item = labGo.GetComponent<DoubleLabelItem>();
		item.SetData (iSolid.ToString(),"[EC0808]"+NGUIUtil.GetStringByKey(10000068)+"[-]",iIntensity.ToString(),"[40B1E1]"+NGUIUtil.GetStringByKey(10000069)+"[-]");
		//MyHead在SetData中初始化，所以这句要在后面
		item.MyHead.SprIcon.enabled = false;

		GameObject SinglabGo = NDLoad.LoadWndItem("DoubleLabelItem", skilWnd.MyHead.table.transform);
		DoubleLabelItem  SingleItem = SinglabGo.GetComponent<DoubleLabelItem>();
		SingleItem.SetData (iTenacity.ToString(),"[088454]"+NGUIUtil.GetStringByKey(10000070)+"[-]","","");
		SingleItem.MyHead.SprIcon.enabled = false;


		skilWnd.MyHead.table.Reposition ();
	}
	private void AddLabelItem(string text)
	{
		GameObject go = NDLoad.LoadWndItem("SingleLabelItem", MyHead.table.transform);
		SingleLabelItem item = go.GetComponent<SingleLabelItem>();
		item.MyHead.label01.overflowMethod = UILabel.Overflow.ResizeHeight;
		item.SetData (text);
	}

	private void AddDoubleLabelItem(string text1,string text1Name,string text2,string text2Name)
	{
		GameObject go = NDLoad.LoadWndItem("DoubleLabelItem", MyHead.table.transform);
		DoubleLabelItem  item = go.GetComponent<DoubleLabelItem>();
		item.SetData (text1,text1Name,text2,text1Name);
	}

	private string GetRoomTypeString(int RoomType)
	{
		// 0 无属性房间, 1.火属性房间, 2.水属性 3.雷属性,4.毒属性，5.气属性
		switch(RoomType)
		{
		case 0:
			return NGUIUtil.GetStringByKey(10000062);
		case 1:
			return NGUIUtil.GetStringByKey(10000063);
		case 2:
			return NGUIUtil.GetStringByKey(10000064);

		case 3:
			return NGUIUtil.GetStringByKey(10000065);

		case 4:
			return NGUIUtil.GetStringByKey(10000066);

		case 5:
			return NGUIUtil.GetStringByKey(10000067);
		}
		return NGUIUtil.GetStringByKey(10000062);
	}

}
