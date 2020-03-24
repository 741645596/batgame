using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoleGridStations : RoleStations {


	public override int GetBestRole()
	{
		int SceneID = -1;
		float speed = 0;
		foreach( StationsInfo Info in RoleList)
		{
			if(Info == null) continue;
			if(Info.m_Speed > speed)
			{
				speed = Info.m_Speed;
				SceneID = Info.m_SceneID;
			}
		}
		
		return SceneID;
	}
	
	
	public override bool CheckFullRole()
	{
		if(RoleList == null ) return false;
		if(RoleList.Count >= 1) return true;
		else return false;
	}
}
