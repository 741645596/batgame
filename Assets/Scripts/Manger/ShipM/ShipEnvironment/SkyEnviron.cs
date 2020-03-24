using UnityEngine;
using System.Collections;

/// <summary>
/// 天空背景环境数据
/// </summary>
/// <author>zhulin</author>
public class SkyEnviron : MonoBehaviour {

	public Transform FlyNode ;
	public Transform SkyHelp;
	void OnEnable() {
		BattleEnvironmentM.JoinSkyData(this);
	}
	
	void OnDisable() {
		BattleEnvironmentM.ExitSkyData();

	}


}
