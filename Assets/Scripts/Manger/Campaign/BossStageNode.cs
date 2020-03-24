using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossStageNode : StageNode
{

    public  List<GameObject> m_lStar = new List<GameObject>();
    public  GameObject m_AttackFlag ;
    public  UILabel   m_lbStageName ;
    
    
    public override void InitUI()
    {
        //副本名称
        if (m_lbStageName != null) {
            m_lbStageName.text = GetCounterpartname() ;
        }
        //星星数量
        int StarNum = GetStarNum();
        bool IsOpen = true;
        SetStarNum(StarNum, IsOpen);
        SetFlag(StarNum, IsOpen);
    }
    
    public void SetStarNum(int num, bool IsOpen)
    {
        if (m_lStar == null || m_lStar.Count == 0 || IsOpen == false) {
            return ;
        }
        for (int i = 0; i < m_lStar.Count  ; i ++) {
            m_lStar[i].SetActive(true);
            UISprite spr = m_lStar[i].GetComponent<UISprite>();
            if (spr == null) {
                continue ;
            }
            if (i < num) {
                spr.spriteName = "icon032" ;
            } else {
                spr.spriteName = "icon033" ;
            }
        }
    }
    
    public void SetFlag(int StarNum, bool Open)
    {
        if (StarNum == 0 && Open == true) {
            if (m_AttackFlag != null) {
                m_AttackFlag.SetActive(true);
                float y = m_AttackFlag.transform.localPosition.y;
                NGUIUtil.TweenGameObjectPosYPingPong(m_AttackFlag, y, y + 10, 0.3f);
            }
        }
    }
    
}
