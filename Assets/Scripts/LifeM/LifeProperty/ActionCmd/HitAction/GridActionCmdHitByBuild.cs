using UnityEngine;
using System.Collections;

/// <summary>
/// 受建筑物攻击，角色受击Action 基类
/// </summary>
/// <author>zhulin</author>
public class GridActionCmdHitByBuild :GridActionCmd{

	public override void SetTarget (Life Parent)
	{
		base.SetTarget (Parent);
		StartHitByBuild();
	}

	public override   void Update () {
		base.Update();
		UpdateHitByBuild();
	}

	public virtual void UpdateHitByBuild()
	{

	}

	public virtual void StartHitByBuild()
	{
		
	}
}




