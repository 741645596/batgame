using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoleStairStations : RoleStations {

	protected const float m_dTime = 0.25f;

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
	
	/// <summary>
	/// 确认占领攻击位是否已经沾满
	/// </summary>
	/// <returns>true，已满，false未满</returns>
	public override bool CheckFullRole()
	{
		if(RoleList == null 
		   || RoleList.Count == 0) return false;

		int total = 0;
		for(int i = 0; i < RoleList.Count; i ++)
		{
			StationsInfo Info = RoleList[i];
			if(Info == null || Info.IsValidStations() == true) 
				continue ;
			float d = Mathf.Abs(Time.realtimeSinceStartup - Info.m_InPutTime);
			if(d < m_dTime) 
				total ++;
		}

		if(total > 0) 
			return true;
		else return false;
	}

}
