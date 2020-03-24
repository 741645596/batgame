using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class RoleNameM  {
	
	private static List<sdata.s_nameInfo> m_lname = new List<sdata.s_nameInfo>();
	private static List<sdata.s_unlawfulInfo> m_lunlawful = new List<sdata.s_unlawfulInfo>();
	private static List<string> m_ltype1 = new List<string>();
	private static List<string> m_ltype2 = new List<string>();

	private static bool _IsLoad = false;
	// Use this for initialization
	public static void Init (object obj)
	{
		if (_IsLoad == true)
			return;
		System.Diagnostics.Debug.Assert(obj is sdata.StaticDataResponse);
		
		sdata.StaticDataResponse sdrsp = obj as sdata.StaticDataResponse;
		
		m_lname = sdrsp.s_name_info;
		m_lunlawful = sdrsp.s_unlawful_info;
		foreach (sdata.s_nameInfo ni in m_lname)
		{
			if (ni.type == 1)
				m_ltype1.Add(ni.text);
			else if (ni.type == 2)
				m_ltype2.Add(ni.text);

		}
		_IsLoad = true;
		
	}

	public static string GetRandomName()
	{
		int seed = GlobalTimer.GetNowTimeInt();
		Random.seed = seed;
		int l = Random.Range(0,m_ltype1.Count);
		seed = GlobalTimer.GetNowTimeInt();
		Random.seed = seed;
		int r = Random.Range(0,m_ltype2.Count);
		return m_ltype1[l] +  m_ltype2[r];
	}

	public static bool CheckUnlawful(string name)
	{
		foreach(sdata.s_unlawfulInfo u in m_lunlawful)
		{
			string low = name.ToLower();
			int index = low.IndexOf(u.word.ToLower());
			if (index >= 0)
				return true;
		}
		return false;
	}

    public static int GetByteLength(string name)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(name);
        //NGUIUtil.DebugLog("bytes lenght ="+ bytes.Length);
        return bytes.Length;
    }
}
