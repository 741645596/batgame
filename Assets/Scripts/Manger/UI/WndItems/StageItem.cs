using UnityEngine;
using System.Collections;

public class StageItem : WndBase {

	private StageType m_type;
    private int m_StageID;//关卡ID
	public bool IsOpen = true;//是否开启
	

	public StageItem_h MyHead
	{
		get 
		{
			return (base.BaseHead () as StageItem_h);
		}
	}

	void Start () 
    {
		if (MyHead.BtnStage)
        {
			MyHead.BtnStage.OnClickEventHandler += BtnStage_OnClickEventHandler;
			MyHead.BtnStage.isEnabled = IsOpen;
        }
	}
	public StageItem_h GetHead()
	{
		return MyHead;
	}

    void BtnStage_OnClickEventHandler(UIButton sender)
    {
		BattleEnvironmentM.SetBattleEnvironmentMode(BattleEnvironmentMode.CombatPVE);
		//获取关卡信息
		StageDC.SetCompaignStage(m_type,m_StageID);
		SceneM.Load(ViewStageScene.GetSceneName(), false, null, false);
    }

	public void SetStage(StageType type ,int stageid)
	{
		m_StageID = stageid ;
		m_type= type;
	}

	public void SetStageText(string name)
	{
		if(MyHead.StageText != null)
			MyHead.StageText.text = name;
	}

	public void SetStarNum(int num ,bool Pass)
	{
		if(MyHead.StarNum != null)
		{
			if(Pass == true && num > 0)
			{
				string text =  num + "star" ;
				MyHead.StarNum.text = text  ;
			}
			else MyHead.StarNum.text = ""  ;
		}
	}
}
