using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 技能项
/// <From>炮弹兵详细信息界面 </From>
/// <Author>QFord</Author>
/// </summary>
public class AddSkillItem : MonoBehaviour
{
    public AddSkillItem_h MyHead {
        get
        {
            return (GetComponent<AddSkillItem_h>());
        }
    }
    
    private SoldierSkill m_soldierSkill;
    private int m_iSkillNo;
    private int m_dSoldierID;
    private int m_iSoldierLevel;
    
    public bool BGuideSelect = false;
    
    void Start()
    {
        if (MyHead.BtnAddSkill) {
            MyHead.BtnAddSkill.OnClickEventHandler += BtnAddSkill_OnClickEventHandler;
        }
        MyHead.BtnClickSkill.IsTweenTarget = false;
        MyHead.BtnClickSkill.OnPressDownEventHandler += BtnClickSkill_OnPressDownEventHandler;
        MyHead.BtnClickSkill.OnPressUpEventHandler += BtnClickSkill_OnPressUpEventHandler;
        
        if (CheckSkillCanUp() == 2) {
            StartCoroutine(RepostionLblCoin());
        }
    }
    /// <summary>
    /// 技能已满级的时候UI设定
    /// </summary>
    IEnumerator RepostionLblCoin()
    {
        yield return U3DUtil.WaitForFrames(1);
        Vector3 pos = MyHead.LblCoin.transform.localPosition;
        MyHead.LblCoin.transform.localPosition = U3DUtil.AddX(pos, -50f);
    }
    
    void BtnClickSkill_OnPressUpEventHandler(UIButton sender)
    {
        WndManager.DestoryDialog<ClickSkillDescWnd>();
    }
    
    void BtnClickSkill_OnPressDownEventHandler(UIButton sender)
    {
        ClickSkillDescWnd wnd = WndManager.GetDialog<ClickSkillDescWnd>();
        if (wnd) {
            wnd.SetData(m_soldierSkill.m_description1, m_soldierSkill.m_description2, gameObject);
        }
    }
    
    public void SetData(SoldierSkill skill, int skillNo, int dsoldierID, int soldierLevel)
    {
        m_soldierSkill = skill;
        m_iSkillNo = skillNo;
        m_dSoldierID = dsoldierID;
        m_iSoldierLevel = soldierLevel;
        SetUI();
    }
    
    public void SetUI()
    {
        //skillType = 1003;//临时 缺少技能图标资源
        //m_soldierSkill.m_type = 1003;
        
        if (!m_soldierSkill.m_enable) {
            NGUIUtil.Set2DSpriteGraySV(MyHead.Spr2dSkill, "Textures/skill/", m_soldierSkill.m_type.ToString());
        } else {
            NGUIUtil.Set2DSprite(MyHead.Spr2dSkill, "Textures/skill/", m_soldierSkill.m_type.ToString());
        }
        MyHead.LblSkillName.text = m_soldierSkill.m_name;
        
        if (m_soldierSkill.m_level >= 0) {
            string str = string.Format("{0}{1}[-]{2}", "[FFF000]", NGUIUtil.GetStringByKey(60000005), m_soldierSkill.m_level);
            MyHead.LblLevel.text = str;
        }
        NGUIUtil.SetActive(MyHead.LblLevel.gameObject, m_soldierSkill.m_enable);
        NGUIUtil.SetActive(MyHead.BtnAddSkill.gameObject, m_soldierSkill.m_enable);
        NGUIUtil.SetActive(MyHead.LblCoin.gameObject.transform.parent.gameObject, m_soldierSkill.m_enable);
        
        if (!m_soldierSkill.m_enable) {
            NGUIUtil.SetActive(MyHead.LblDisableSkillDesc.gameObject, true);
            string text = string.Format(NGUIUtil.GetStringByKey(30000034), NGUIUtil.GetStringByKey(99600000 + m_soldierSkill.m_enableQuality));
            NGUIUtil.SetLableText(MyHead.LblDisableSkillDesc, text);
        }
        if (CheckSkillCanUp() == 2) {
            string text = NGUIUtil.GetStringByKey(30000059);
            NGUIUtil.SetLableText(MyHead.LblCoin, string.Format("{0}{1}", "[c5ff71]", text));
            UISprite sprGold = MyHead.LblCoin.GetComponentInParent<UISprite>();
            NGUIUtil.SetSprite(sprGold, "0");
            int coin =  SoldierM.GetUpSkillLevelNeed(m_soldierSkill.m_level, m_iSkillNo);
            if (coin > 0) {
                string coinText = "";
                if (coin < UserDC.GetCoin()) {
                    coinText = string.Format("{0}{1}", "[ECB408]", coin);
                } else {
                    coinText = string.Format("{0}{1}", "[FF0000]", coin);
                }
                MyHead.LblCoin.text = coinText;
            }
            return;
        }
        int coin2 =  SoldierM.GetUpSkillLevelNeed(m_soldierSkill.m_level + 1, m_iSkillNo);
        if (coin2 > 0) {
            string coinText = "";
            if (coin2 < UserDC.GetCoin()) {
                coinText = string.Format("{0}{1}", "[ECB408]", coin2);
            } else {
                coinText = string.Format("{0}{1}", "[FF0000]", coin2);
            }
            MyHead.LblCoin.text = coinText;
        }
        
    }
    
    public void BtnAddSkill_OnClickEventHandler(UIButton sender)
    {
        ///1 金币不足，2炮弹兵等级不足，0是能够升级，3技能为空不能升级，4技能点不足.
        int result = CheckSkillCanUp();
        if (UserDC.GetLeftSkillPoints() >= 1 && result == 0) {
            if (m_iSkillNo < 6) {
                PlayAddSkillEffect();
                SoldierDC.Send_SoldierSkillUpRequest(m_dSoldierID, m_iSkillNo);
                
            }
        } else if (result == 1) {
            NGUIUtil.ShowTipWndByKey(99901003);
        } else if (result == 2) {
            NGUIUtil.ShowTipWndByKey(10000104);
        } else if (result == 4) {
            NGUIUtil.ShowTipWndByKey(99914004);
        } else if (result == 5) {
            NGUIUtil.ShowTipWndByKey(99914003);
        }
    }
    /// <summary>
    /// 播放技能升级特效
    /// </summary>
    private void PlayAddSkillEffect()
    {
        GameObject go = GameObjectLoader.LoadPath("effect/prefab/", "2001291", transform.parent.parent);
        if (go) {
            UIEffect UIDepth = go.AddComponent<UIEffect>();
            UIDepth.depth = 99;
            Vector3 pos = transform.parent.parent.InverseTransformPoint(MyHead.Spr2dSkill.transform.position);
            go.transform.localPosition = pos;
            GameObjectActionExcute gae = GameObjectActionExcute.CreateExcute(go);
            GameObjectActionDelayDestory destroy = new GameObjectActionDelayDestory(1.0f);
            gae.AddAction(destroy);
        } else {
            NGUIUtil.DebugLog("AddSkillEffect not found!!!");
        }
    }
    
    /// <summary>
    /// 检测技能是否可升级（策划案提供）
    /// 1 金币不足，2等级达到上限，0是能够升级，3技能为空不能升级，4技能点不足 5技能升级空间不足
    /// </summary>
    public int CheckSkillCanUp()
    {
        if (m_soldierSkill == null) {
            NGUIUtil.DebugLog("m_soldierSkill == null ");
            return 3;
        }
        int teamMaxLevel = SoldierM.GetSoldierMaxLevel();
        int skillMaxLevel = ConfigM.GetSkillMaxLevel(m_iSkillNo);
        
        if (m_soldierSkill.m_level == skillMaxLevel) { //技能等级达到上限
            return 2;
        }
        
        int skillUpCoin = SoldierM.GetUpSkillLevelNeed(m_soldierSkill.m_level + 1, m_iSkillNo);
        int userCoin = UserDC.GetCoin();
        if (skillUpCoin > userCoin) {
            return 1;
        }
        
        if (UserDC.GetLeftSkillPoints() < 1) {
            return 4;
        }
        
        int m = 0;
        int n = m_iSoldierLevel;//当前炮弹兵等级
        int p = m_soldierSkill.m_level;//技能等级
        
        int q = teamMaxLevel - skillMaxLevel; //战队等级上限 - 技能等级上限
        m = n - p - q;
        
        if (m >= 1) {
            return 0;
        } else {
            return 5;
        }
    }
}
