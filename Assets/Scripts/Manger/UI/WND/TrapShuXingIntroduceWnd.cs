using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrapShuXingIntroduceWnd : WndBase
{
	public TrapShuXingIntroduceWnd_h MyHead
	{
		get 
		{
			return (base.BaseHead () as TrapShuXingIntroduceWnd_h);
		}
	}
	private BuildInfo m_Info;
	public void SetBuildInfo(BuildInfo info)
	{
		m_Info = info;

		if(MyHead.table != null)
			MyHead.table.ClearTable();
        SetUI();
	}

	public override void WndStart ()
	{
		base.WndStart ();
	}
	private void SetUI()
	{

//		AddLabelItem("[2B5891]"+NGUIUtil.GetStringByKey(10000049)+ GetRoomTypeString(m_Info.m_RoomKind)+"[-]",27);
		if(m_Info.m_Skill != null)
		{
			string show = m_Info.m_Skill.m_desc.Replace("\\n",System.Environment.NewLine);
			AddLabelItem("[552d0a]"+show+"[-]",22);
		}

		AddGrayItem ();

		//过滤0的数字不显示.
		List<KeyValueName> l = GetKeyValueList(); 
		for(int i = 0;i < l.Count; i += 2)	
		{
			KeyValueName value1 = l[i];

			KeyValueName value2 = new KeyValueName();
			value2.Key = -1;
			if(i < l.Count -1)
			{
				value2 = l[i+1];
			}
			if(value2.Key < 0)
			{
				AddDoubleLabelItem ("[552d0a]"+NGUIUtil.GetStringByKey(value1.Key)+"[-]",value1.name,"","");
			}
			else
			{
				AddDoubleLabelItem ("[552d0a]"+NGUIUtil.GetStringByKey(value1.Key)+"[-]",value1.name,"[552d0a]"+NGUIUtil.GetStringByKey(value2.Key)+"[-]",value2.name);
			}
		}

		MyHead.table.Reposition ();
	}
	private List<KeyValueName> GetKeyValueList()
	{
		List<KeyValueName> l = new List<KeyValueName>(); 
		KeyValueName value = new KeyValueName();
		if(m_Info.m_Solidity > 0)
		{
			value.Key = 10000058;
			value.name = m_Info.m_Solidity.ToString("#.##");
			l.Add(value);
		}
		if(m_Info.m_Intensity > 0)
		{
			value.Key = 10000059;
			value.name = m_Info.m_Intensity.ToString("#.##");
			l.Add(value);
		}

		if(m_Info.m_Tenacity > 0)
		{
			value.Key = 10000060;
			value.name = m_Info.m_Tenacity.ToString("#.##");
			l.Add(value);
		}
		if(m_Info.m_hp > 0)
		{
			value.Key = 10000057;
			value.name = m_Info.m_hp.ToString("#.##");
			l.Add(value);
		}
		if(m_Info.m_phyattack > 0)
		{
			value.Key = 10000053;
			value.name = m_Info.m_phyattack.ToString("#.##");
			l.Add(value);
		}
		if(m_Info.m_phydefend > 0)
		{
			value.Key = 10000055;
			value.name = m_Info.m_phydefend.ToString("#.##");
			l.Add(value);
		}
		if(m_Info.m_magicattack > 0)
		{
			value.Key = 10000071;
			value.name = m_Info.m_magicattack.ToString("#.##");
			l.Add(value);
		}
		if(m_Info.m_magicdefend > 0)
		{
			value.Key = 10000056;
			value.name = m_Info.m_magicdefend.ToString("#.##");
			l.Add(value);
		}
		if(m_Info.m_bear > 0)
		{
			value.Key = 10000165;
			value.name = m_Info.m_bear.ToString("#.##");
			l.Add(value);
		}
		return l;
	}
	private void AddGrayItem()
	{
		float iSolidGrow = 0; 
		float iIntensityGrow = 0; 
		float iTenacityGrow = 0;
		buildingM.GetStarInfoGrow(m_Info,ref iSolidGrow,ref iIntensityGrow,ref iTenacityGrow);

		GameObject go2 = NDLoad.LoadWndItem("HeroIntroduceItem04",MyHead.table.transform);
		
		
		HeroIntroduceItem04 item = go2.GetComponent<HeroIntroduceItem04>();
		if (item)
		{
			string strength = "[ff7761]" + NGUIUtil.GetStringByKey(10000068) +"[-]";
			string strIntell = "[78defb]" + NGUIUtil.GetStringByKey(10000069) +"[-]";
			string strAgility = "[4eff00]" + NGUIUtil.GetStringByKey(10000070) +"[-]";
			item.SetLblName(strength,strIntell,strAgility);
			item.SetUI(iSolidGrow.ToString("0.00"),iIntensityGrow.ToString("0.00"),iTenacityGrow.ToString("0.00"));
		}
		
	}
	private void AddLabelItem(string text,int FontSize = 0)
	{
		GameObject go = NDLoad.LoadWndItem("SingleLabelItem", MyHead.table.transform);
		SingleLabelItem item = go.GetComponent<SingleLabelItem>();
		item.MyHead.label01.overflowMethod = UILabel.Overflow.ResizeHeight;
		item.SetData (text,FontSize);
	}

	private void AddDoubleLabelItem(string text1Name,string text1,string text2Name,string text2,int iFontSize = 0)
	{
		GameObject go = NDLoad.LoadWndItem("DoubleLabelItem", MyHead.table.transform);
		DoubleLabelItem  item = go.GetComponent<DoubleLabelItem>();

		item.SetData (text1,text1Name,text2,text2Name,iFontSize);
		item.MyHead.SprIcon.enabled = false;
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
