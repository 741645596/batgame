using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DefenseRadarAI : RadarAI {


	protected override void GetTargetList(int AttackLike)
	{
		List<Life> RoleList = new List<Life>();
		CM.SearchLifeMListInBoat(ref RoleList,LifeMType.SOLDIER | LifeMType.SUMMONPET, LifeMCamp.ATTACK);
		CheckInVisionAttack(RoleList);
	}
}
