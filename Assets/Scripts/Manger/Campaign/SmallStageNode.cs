using UnityEngine;
using System.Collections;

public class SmallStageNode : StageNode
{

    public  GameObject m_AttackFlag ;
    public  GameObject m_FinishFlag ;
    
    
    
    public override void InitUI()
    {
        //星星数量
        int StarNum = GetStarNum();
        bool IsOpen = true;
        SetFlag(StarNum, IsOpen);
    }
    
    
    public void SetFlag(int StarNum, bool Open)
    {
        if (Open == false) {
            return ;
        }
        if (StarNum == 0) {
            if (m_AttackFlag != null) {
                m_AttackFlag.SetActive(true);
                float y = m_AttackFlag.transform.localPosition.y;
                NGUIUtil.TweenGameObjectPosYPingPong(m_AttackFlag, y, y + 10, 0.3f);
            }
        } else if (StarNum > 0) {
            if (m_FinishFlag != null) {
                m_FinishFlag.SetActive(true);
            }
        }
    }
}
