using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 英雄介绍界面
/// </summary>
/// <From> </From>
/// <Author>QFord</Author>
public class HeroIntroduceWnd :  WndBase{

	public HeroIntroduceWnd_h MyHead
	{
		get 
		{
			return (base.BaseHead () as HeroIntroduceWnd_h);
		}
	}

    private SoldierInfo m_soldierInfo;
    private struct DescData
    {
        public string desc;
        public string data;

        public DescData(string desc, string data)
        {
            this.desc = desc;
            this.data = data;
        }
    }
    private List<DescData> m_lHeroDescData = new List<DescData>();

    public override void WndStart()
    {
        base.WndStart();
    
    }

    public void SetData(SoldierInfo info)
    {
        m_soldierInfo = info;
        SetUI();
    }

    private void SetUI()
    {
        if (MyHead.AttributesTable == null || m_soldierInfo == null)
        {
            NGUIUtil.DebugLog("HeroIntroduceWnd.cs AttributesTable parent or m_soldierInfo null !!! ");
            return;
        }
        AddItem02();
        AddItem04();
        AddAllItem05();
        NGUIUtil.RepositionTable(MyHead.AttributesTable);
    }

    private void AddItem02()
    {
        GameObject go = NDLoad.LoadWndItem("HeroIntroduceItem02", MyHead.AttributesTable.transform);
        HeroIntroduceItem02 item = go.GetComponent<HeroIntroduceItem02>();
        if (item)
        {
            item.SetUI(m_soldierInfo.m_desc);
        }
    }
    
    private void AddItem04()
    {
		GameObject go = NDLoad.LoadWndItem("HeroIntroduceItem04", MyHead.AttributesTable.transform);
        HeroIntroduceItem04 item = go.GetComponent<HeroIntroduceItem04>();
        if (item)
        {
			string strength = "[FA3B3B]" + NGUIUtil.GetStringByKey(88800044) +"[-]";
			string strIntell = "[32C6FA]" + NGUIUtil.GetStringByKey(88800045) +"[-]";
			string strAgility = "[3BFA48]" + NGUIUtil.GetStringByKey(88800046) +"[-]";
			item.SetLblName(strength,strIntell,strAgility);
            item.SetUI((m_soldierInfo.m_strength_grow*0.01).ToString()
                ,(m_soldierInfo.m_intelligence_grow*0.01).ToString()
                ,(m_soldierInfo.m_agility_grow*0.01).ToString());
        }
    }
    /// <summary>
    /// 获取属性文字
    /// </summary>
    private string GetAttrText(EffectType type)
    {
         return string.Format("[552d0a]" + NGUIUtil.GetStringByKey(99800000 + (int)type) + "[-]");
    }
    /// <summary>
    /// 添加文本和数值 数据(type 1 百分比  / 2 千分比 )
    /// </summary>
    private void AddTextData(string text1, string text2, float data1, float data2,int type1 = 0,int type2 = 0)
    {
        if (data1 != 0)
        {
            if (type1 == 0)
                m_lHeroDescData.Add(new DescData(text1, data1.ToString()));
            else if (type1 == 1)
                m_lHeroDescData.Add(new DescData(text1, NdUtil.ConvertPercent(data1)));
            else if (type1 == 2)
                m_lHeroDescData.Add(new DescData(text1, NdUtil.ConvertPermillage(data1)));
        }
        if (data2 != 0)
        {
            if (type2 == 0)
                m_lHeroDescData.Add(new DescData(text2, data1.ToString()));
            else if (type2 == 1)
                m_lHeroDescData.Add(new DescData(text2, NdUtil.ConvertPercent(data2)));
            else if (type2 == 2)
                m_lHeroDescData.Add(new DescData(text2, NdUtil.ConvertPermillage(data2)));
        }
    }

    private void AddAllItem05()
    {
		string text1 = GetAttrText(EffectType.Strength);
        string text2 = GetAttrText(EffectType.Intelligence); ;
        float data1 = m_soldierInfo.GetSoldierAttr(EffectType.Strength);
        float data2 = m_soldierInfo.GetSoldierAttr(EffectType.Intelligence); ;
        if (data1 != 0)
            m_lHeroDescData.Add(new DescData(text1, data1.ToString()));
        if (data2 != 0)
            m_lHeroDescData.Add(new DescData(text2, data2.ToString()));
        
        text1 = GetAttrText(EffectType.Agility);
        text2 = GetAttrText(EffectType.Hp);
        data1 = m_soldierInfo.GetSoldierAttr(EffectType.Agility);
        data2 = m_soldierInfo.GetSoldierAttr(EffectType.Hp);
        if (data1 != 0)
            m_lHeroDescData.Add(new DescData(text1, data1.ToString()));
        if (data2 != 0)
            m_lHeroDescData.Add(new DescData(text2, data2.ToString()));

        text1 = GetAttrText(EffectType.Anger);
        text2 = GetAttrText(EffectType.PhyAttack);
        data1 = m_soldierInfo.GetSoldierAttr(EffectType.Anger);
        data2 = m_soldierInfo.GetSoldierAttr(EffectType.PhyAttack);
        if (data1 != 0)
            m_lHeroDescData.Add(new DescData(text1, data1.ToString()));
        if (data2 != 0)
            m_lHeroDescData.Add(new DescData(text2, data2.ToString()));

        text1 = GetAttrText(EffectType.PhyDefense);
        text2 = GetAttrText(EffectType.MagicAttack);
        data1 = m_soldierInfo.GetSoldierAttr(EffectType.PhyDefense);
        data2 = m_soldierInfo.GetSoldierAttr(EffectType.MagicAttack);
        if (data1 != 0)
            m_lHeroDescData.Add(new DescData(text1, data1.ToString()));
        if (data2 != 0)
            m_lHeroDescData.Add(new DescData(text2, data2.ToString()));

        text1 = GetAttrText(EffectType.MagicDefense);
        text2 = GetAttrText(EffectType.Hit);
        data1 = m_soldierInfo.GetSoldierAttr(EffectType.MagicDefense);
        data2 = m_soldierInfo.GetSoldierAttr(EffectType.Hit);
        if (data1 != 0)
            m_lHeroDescData.Add(new DescData(text1, data1.ToString()));
        if (data2 != 0)
            m_lHeroDescData.Add(new DescData(text2, data2.ToString()));

        text1 = GetAttrText(EffectType.Dodge);
        text2 = GetAttrText(EffectType.PhyCrit);
        data1 = m_soldierInfo.GetSoldierAttr(EffectType.Dodge);
        data2 = m_soldierInfo.GetSoldierAttr(EffectType.PhyCrit);
        if (data1 != 0)
            m_lHeroDescData.Add(new DescData(text1, data1.ToString()));
        if (data2 != 0)
            m_lHeroDescData.Add(new DescData(text2, data2.ToString()));

        text1 = GetAttrText(EffectType.MagicCrit);
        text2 = string.Format("[552d0a]" + NGUIUtil.GetStringByKey("30000058") + "[-]");
        data1 = m_soldierInfo.GetSoldierAttr(EffectType.MagicCrit);
        if (data1 != 0)
            m_lHeroDescData.Add(new DescData(text1, data1.ToString()));
        if (m_soldierInfo.m_concussion != 0)
            m_lHeroDescData.Add(new DescData(text2, m_soldierInfo.m_concussion.ToString()));
        /* 攻速和移动速度不要
        text1 = GetAttrText(EffectType.MoveSpeed);
        text2 = GetAttrText(EffectType.AttackTime);
        data1 = m_soldierInfo.GetSoldierAttr(EffectType.MoveSpeed);
        data2 = m_soldierInfo.GetSoldierAttr(EffectType.AttackTime);
        AddTextData(text1, text2, data1, data2);
        */
        text1 = GetAttrText(EffectType.RecoHp);
        text2 = GetAttrText(EffectType.RecoAnger);
        data1 = m_soldierInfo.GetSoldierAttr(EffectType.RecoHp);
        data2 = m_soldierInfo.GetSoldierAttr(EffectType.RecoAnger);
        AddTextData(text1, text2, data1, data2);

        text1 = GetAttrText(EffectType.Vampire);
        text2 = GetAttrText(EffectType.AntiPress);
        data1 = m_soldierInfo.GetSoldierAttr(EffectType.Vampire);
        data2 = m_soldierInfo.GetSoldierAttr(EffectType.AntiPress);
        AddTextData(text1, text2, data1, data2);

        text1 = GetAttrText(EffectType.CutphyDamage);
        text2 = GetAttrText(EffectType.IcePoint);
        data1 = m_soldierInfo.GetSoldierAttr(EffectType.CutphyDamage);
        data2 = m_soldierInfo.GetSoldierAttr(EffectType.IcePoint);
        AddTextData(text1, text2, data1, data2);

        text1 = GetAttrText(EffectType.CutMagDamage);
        text2 = GetAttrText(EffectType.CutPhyDefend);
        data1 = m_soldierInfo.GetSoldierAttr(EffectType.CutMagDamage);
        data2 = m_soldierInfo.GetSoldierAttr(EffectType.CutPhyDefend);
        AddTextData(text1, text2, data1, data2);

        text1 = GetAttrText(EffectType.CutMagDefend);
        text2 = GetAttrText(EffectType.AddDoctor);
        data1 = m_soldierInfo.GetSoldierAttr(EffectType.CutMagDefend);
        data2 = m_soldierInfo.GetSoldierAttr(EffectType.AddDoctor);
        AddTextData(text1, text2, data1, data2);

        text1 = GetAttrText(EffectType.PhysicalCritBonusDamage);
        text2 = GetAttrText(EffectType.MagicCritBonusDamage);
        data1 = m_soldierInfo.GetSoldierAttr(EffectType.PhysicalCritBonusDamage);
        data2 = m_soldierInfo.GetSoldierAttr(EffectType.MagicCritBonusDamage);
        AddTextData(text1, text2, data1, data2,1,1);

        text1 = GetAttrText(EffectType.SetHpByPercent);
        text2 = GetAttrText(EffectType.FireAttack);
        data1 = m_soldierInfo.GetSoldierAttr(EffectType.SetHpByPercent);
        data2 = m_soldierInfo.GetSoldierAttr(EffectType.FireAttack);
        AddTextData(text1, text2, data1, data2,1,1);
        //火水电毒气 攻击和抗性
        text1 = GetAttrText(EffectType.AntiFire);
        text2 = GetAttrText(EffectType.WaterAttack);
        data1 = m_soldierInfo.GetSoldierAttr(EffectType.AntiFire);
        data2 = m_soldierInfo.GetSoldierAttr(EffectType.WaterAttack);
        AddTextData(text1, text2, data1, data2,0,2);

        text1 = GetAttrText(EffectType.AntiWater);
        text2 = GetAttrText(EffectType.ElectricAttack);
        data1 = m_soldierInfo.GetSoldierAttr(EffectType.AntiWater);
        data2 = m_soldierInfo.GetSoldierAttr(EffectType.ElectricAttack);
        AddTextData(text1, text2, data1, data2,0,2);

        text1 = GetAttrText(EffectType.AntiElectric);
        text2 = GetAttrText(EffectType.PotionAttack);
        data1 = m_soldierInfo.GetSoldierAttr(EffectType.AntiElectric);
        data2 = m_soldierInfo.GetSoldierAttr(EffectType.PotionAttack);
        AddTextData(text1, text2, data1, data2,0,2);

        text1 = GetAttrText(EffectType.AntiPotion);
        text2 = GetAttrText(EffectType.GasAttack);
        data1 = m_soldierInfo.GetSoldierAttr(EffectType.AntiPotion);
        data2 = m_soldierInfo.GetSoldierAttr(EffectType.GasAttack);
        AddTextData(text1, text2, data1, data2,0,2);

        text1 = GetAttrText(EffectType.AntiGas);
        text2 = GetAttrText(EffectType.FireDamageReduction);
        data1 = m_soldierInfo.GetSoldierAttr(EffectType.AntiGas);
        data2 = m_soldierInfo.GetSoldierAttr(EffectType.FireDamageReduction);
        AddTextData(text1, text2, data1, data2,0,2);
        //伤害减免
        text1 = GetAttrText(EffectType.WaterDamageReduction);
        text2 = GetAttrText(EffectType.ElectricDamageReduction);
        data1 = m_soldierInfo.GetSoldierAttr(EffectType.WaterDamageReduction);
        data2 = m_soldierInfo.GetSoldierAttr(EffectType.ElectricDamageReduction);
        AddTextData(text1, text2, data1, data2);

        text1 = GetAttrText(EffectType.PotionDamageReduction);
        text2 = GetAttrText(EffectType.GasDamageReduction);
        data1 = m_soldierInfo.GetSoldierAttr(EffectType.PotionDamageReduction);
        data2 = m_soldierInfo.GetSoldierAttr(EffectType.GasDamageReduction);
        AddTextData(text1, text2, data1, data2);

        for (int i = 0; i < m_lHeroDescData.Count; i = i + 2)
        {
            if (i >= m_lHeroDescData.Count-1)
                AddItem05(m_lHeroDescData[i].data, "",
                    m_lHeroDescData[i].desc, "");
            else
                AddItem05(m_lHeroDescData[i].data, m_lHeroDescData[i + 1].data,
                    m_lHeroDescData[i].desc, m_lHeroDescData[i + 1].desc);
        }
       
    }
    /// <summary>
    /// 添加 描述和数据
    /// </summary>
    /// <param name="text1">数据1</param>
    /// <param name="text2">数据2</param>
    /// <param name="text1Name">数据1的描述</param>
    /// <param name="text2Name">数据2的描述</param>
	private void AddItem05(string text1,string text2,string text1Name,string text2Name)
    {
        GameObject go = NDLoad.LoadWndItem("DoubleLabelItem", MyHead.AttributesTable.transform);
        DoubleLabelItem item = go.GetComponent<DoubleLabelItem>();
        if (item)
        {
			item.SetData(text1,text1Name, text2,text2Name);
        }
    }



}
