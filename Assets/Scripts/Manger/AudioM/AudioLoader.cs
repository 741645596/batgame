using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 音乐加载。
/// </summary>
public class AudioLoader  {
	

	public static AudioClip LoadAudio(string Path,string AudioName)
	{
		return LoadAudio(Path + AudioName);
	}

	public static AudioClip LoadAudio(string AudioName)
	{
		if (string.IsNullOrEmpty(AudioName))
			return null;
		
		AudioClip ob =Resources.Load ("Audio/" + AudioName) as AudioClip;
		if(ob == null)
		{
			Debug.Log("请调查音乐路径是否错误:" + AudioName);
		}
		return ob;
	}
}
