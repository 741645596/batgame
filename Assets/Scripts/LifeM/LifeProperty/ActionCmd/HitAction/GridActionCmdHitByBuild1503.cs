using UnityEngine;
using System.Collections;

/// <summary>
/// 1503(电击) 受击
/// </summary>
/// <author>zhulin</author>
public class GridActionCmdHitByBuild1503 :GridActionCmdHitByBuild{



	public GridActionCmdHitByBuild1503()
	{
		m_Duration = 1.5f;
	}
	public override void StartHitByBuild ()
	{
		//m_Skin.HitByBuildingEffect(HitbyBuilding.HitByBuild1503,true ,0 ,HitEffectMode.CoverBody ,null);
		(m_Skin as RoleSkin).SetVisable(false);
	}
	public override void UpdateHitByBuild()
	{
		if(IsDone())
		{
			(m_Skin as RoleSkin).SetVisable(true);
			//m_Skin.HitByBuildingEffect(HitbyBuilding.HitByBuild1503,false ,0 ,HitEffectMode.CoverBody ,null);
		}
	}
	
	public override void SetDone ()
	{
		base.SetDone ();
		(m_Skin as RoleSkin).SetVisable(true);
		//m_Skin.HitByBuildingEffect(HitbyBuilding.HitByBuild1503,false ,0 ,HitEffectMode.CoverBody,null);
	}
	
	public override void Finish ()
	{
		base.Finish ();
		(m_Skin as RoleSkin).SetVisable(true);
	}
}
