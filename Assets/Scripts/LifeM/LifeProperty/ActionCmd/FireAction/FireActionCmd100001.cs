using UnityEngine;
using System.Collections;

/// <summary>
/// 炮战
/// </summary>
/// <author>QFord</author>
public class FireActionCmd100001 : FireActionCmd {

    public FireActionCmd100001(Life Parent, Vector3 FlyDir, float FlySpeed, bool enableTrail)
		: base(Parent, FlyDir, FlySpeed, enableTrail)
	{

	}


	/// <summary>
	/// 撞击出生前置动作
	/// </summary>
	public override void FlyReboundAction(bool bVertical)
	{
		Transform t = m_Skin.tRoot;
		float pauseDuration =0.1f;
		SetCollEffect(bVertical);



		//撞击墙体跌落时调整
		if (bVertical && m_FlyDir == Vector3.down)
		{
			t.rotation = new Quaternion(0, 0, 0.7f, -0.7f);
			t.position += new Vector3(0, 0.5f, 0);
		}
		else
		{
			RolePlayAnimation(AnimatorState.Fly00000);
		}
		
		PauseFly(pauseDuration);
    }

	/// <summary>
	/// 撞传板暂停飞行
	/// </summary>
	public override void PauseContinueFlight(bool bVertical)
	{
        Transform t = m_Skin.tRoot;
		float pauseDuration =0.1f;

		SetCollEffect(bVertical);
		
		//撞击墙体跌落时调整
		if (bVertical && m_FlyDir == Vector3.down)
		{
			t.rotation = new Quaternion(0, 0, 0.7f, -0.7f);
			t.position += new Vector3(0, 0.5f, 0);
		}
		else
		{
            RolePlayAnimation(AnimatorState.Fly00000);
		}
		PauseFly(pauseDuration);
	}
}
