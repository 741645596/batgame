using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill2Action  {


	private static Dictionary<int,string>m_LSkillAction = new Dictionary<int,string> ();

    public  static void Init()
	{

	}


	public static string GetAction(int SkillType)
	{
		if (m_LSkillAction.ContainsKey (SkillType)) 
		{
			return m_LSkillAction[SkillType];
		}
		return string.Empty;
	}
}
