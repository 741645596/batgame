using UnityEngine;
using System.Collections;

/// <summary>
/// 海洋环境数据
/// </summary>
/// <author>zhulin</author>
public class SeaEnviron : MonoBehaviour {

	public Transform LeftHitFlyPoint;
	public Transform RightHitFlyPoint;
	public Transform LeftDolphine;
	public Transform RightDolphine;

	void OnEnable() {
		BattleEnvironmentM.JoinSeaData(this);
		
	}
	
	void OnDisable() {
		BattleEnvironmentM.ExitSeaData();
	}
}
