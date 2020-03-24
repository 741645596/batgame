using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackRadarAI : RadarAI {
	
	// 获取目标列表
	protected override void GetTargetList (int AttackLike)
	{
		mTargetlist.Clear();
		//炮弹兵
		if (AttackLike == 1 ) 
		{
			LikeSoldier();
		}
		//建筑物
		else if (AttackLike == 2)
		{
			LikeBuild();
		}
		//资源建筑物
		else if(AttackLike == 3)
		{
			LikeResBuild();
		}

		if(mTargetlist.Count == 0)
		{
			LikeNear();
		}
	}


	private void LikeSoldier()
	{
		List<Life> Enemy =  new List<Life>();
		mTargetlist.Clear ();
		CM.SearchLifeMListInBoat(ref Enemy ,LifeMType.SOLDIER, LifeMCamp.DEFENSE);
		CheckInVisionAttack(Enemy);
	}

	private void LikeBuild()
	{
		List<Life> Enemy =  new List<Life>();
		List<Life> Build =  new List<Life>();
		CM.SearchLifeMListInBoat(ref Build ,LifeMType.BUILD);
		foreach(Life l in Build)
		{
			if (l is Building)
			{
				if(l.m_Attr.IsDamage && !l.m_Attr.IsResource)
					Enemy.Add(l);
			}
		}
		CheckInVisionAttack(Enemy);
	}

	private void LikeResBuild()
	{
		List<Life> Build =  new List<Life>();
		List<Life> Enemy =  new List<Life>();
		mTargetlist.Clear ();
		CM.SearchLifeMListInBoat(ref Build ,LifeMType.BUILD);
		foreach(Life l in Build)
		{
			if(l.m_Attr.IsDamage && l.m_Attr.IsResource)
				Enemy.Add(l);
		}
		CheckInVisionAttack(Enemy);
	}

	private void LikeNear()
	{
		List<Life> Enemy =  new List<Life>();
		List<Life> Build =  new List<Life>();
		CM.SearchLifeMListInBoat(ref Enemy ,LifeMType.SOLDIER | LifeMType.SUMMONPET, LifeMCamp.DEFENSE);
		CM.SearchLifeMListInBoat(ref Build ,LifeMType.BUILD);
		foreach(Life b in Build)
		{
			if (b is Building && (b as Building).m_roomType == RoomType.ResRoom)
			{
				if(b.m_Attr.IsDamage)
					Enemy.Add(b);
			}
		}

		CheckInVisionAttack(Enemy);
	}
}
