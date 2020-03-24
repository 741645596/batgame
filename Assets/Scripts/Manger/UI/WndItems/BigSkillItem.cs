using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System;

public class BigSkillItem : MonoBehaviour {

    public UI2DSprite Spr2dSkillIcon;
    public UILabel LblSkillName;
    public UILabel LblSkillDesc;
    public UILabel LblSkillEffect;
    public GameObject LblBiSha;

    public void SetData(SoldierSkill soldierSkill,bool isBigSkill = true)
    {
        int iconID = soldierSkill.m_type;
        string name = soldierSkill.m_name;
        string[] s = soldierSkill.m_description1.Split(new string[] { "\\n" }, StringSplitOptions.RemoveEmptyEntries);
        if (s.Length<2)
        {
            NGUIUtil.DebugLog("Skill m_description1 data error: " + soldierSkill.m_description1);
            return;
        }
        string desc = s[0];
        string skillEffect = s[1];
        SetUI(iconID, name, desc, skillEffect);
        if (!isBigSkill)
        {
            NGUIUtil.SetActive(LblBiSha, false);
            transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            Vector3 pos = Spr2dSkillIcon.gameObject.transform.localPosition;
            Spr2dSkillIcon.gameObject.transform.localPosition = U3DUtil.AddY(pos, 35f);
        }
    }

	private void SetUI(int iconID,string name,string desc,string skillEffect)
    {
        NGUIUtil.Set2DSprite(Spr2dSkillIcon, "Textures/skill/", iconID.ToString());
        NGUIUtil.SetLableText(LblSkillName, name);
        NGUIUtil.SetLableText(LblSkillDesc, desc);
        NGUIUtil.SetLableText(LblSkillEffect, skillEffect);
    }

    
	
}
