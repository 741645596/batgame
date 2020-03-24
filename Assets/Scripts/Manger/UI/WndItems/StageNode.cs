using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using sdata;


/// <summary>
/// 关卡节点
/// <summary>
/// <Author>zhulin</Author>
public enum StageNodeType {
    Small    = 0,    //小关卡
    Boss     = 1,    //大关卡
}


public class StageNode : MonoBehaviour
{

    private StageType m_Type;   //关卡类型
    private int m_Chapter;      //章
    public  int m_Stage;        //节
    private int m_StageID;      //副本ID
    protected CounterPartInfo m_CounterInfo;
    public  UIButton Btn_IsLand;
    /// <summary>
    /// 设置关卡副本数据
    /// <summary>
    public void SetStageData(StageType Type, int Chapter, CounterPartInfo Info)
    {
        m_Type = Type ;
        m_Chapter = Chapter ;
        m_StageID = StageM.GetStageID(m_Type, m_Chapter, m_Stage);
        m_CounterInfo = Info ;
        
        if (Info.isboss == 1) {
            if ((this is BossStageNode) == false) {
                //Debug.LogError("data skin is error");
            }
        } else {
            if ((this is SmallStageNode) == false) {
                //Debug.LogError("data skin is error");
            }
        }
        
        AddEvents() ;
        InitUI();
    }
    
    
    
    public virtual void InitUI()
    {
    
    }
    
    
    void AddEvents()
    {
        if (Btn_IsLand != null) {
            Btn_IsLand.OnClickEventHandler += DoEvent;
            Btn_IsLand.isEnabled = true;
        }
        
    }
    
    
    public void DoEvent(UIButton sender)
    {
        BattleEnvironmentM.SetBattleEnvironmentMode(BattleEnvironmentMode.CombatPVE);
        //获取关卡信息
        StageDC.SetCompaignStage(m_Type, m_StageID);
        SceneM.Load(ViewStageScene.GetSceneName(), false, false);
    }
    
    /// <summary>
    /// 获取战役名称
    /// <summary>
    public string GetCounterpartname()
    {
        if (m_CounterInfo != null) {
            return m_CounterInfo.counterpartname ;
        } else {
            return string.Empty;
        }
    }
    
    
    /// <summary>
    /// 获取星级
    /// <summary>
    public int GetStarNum()
    {
        return StageDC.GetPassStageStar(m_Type, m_StageID);
    }
    
    
    
}
