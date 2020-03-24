using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// 楼梯
/// </summary>
/// <author>zhulin</author>
public class Building1201 : Building {


	public void SetMapGrid(int cx ,int cy)
	{
		m_MapPos = new Int2(cx, cy);
		MapStair stair = MapStair.GetStair(m_MapPos ,false);
		if(stair != null)
		{
			stair.JoinStairLife(this);
		}
	}

	public override void InitBuild()
	{

	}

	/// <summary>
	/// 楼梯不加入cm列表
	/// </summary>
	/// <author>zhulin</author>
	public override int SetLifeCore(LifeMCore Core)
	{
		return 0;
	}

	public override Int2 GetMapPos()
	{
		return m_MapPos;
	}

	public void ShowUpEffect(float duration)
	{
		GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "2000591_02", m_thisT.GetChild(0).position, m_thisT.GetChild(0));
		if (gae != null)
		{
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(duration);
			gae.AddAction(ndEffect);
		}
	}
}
