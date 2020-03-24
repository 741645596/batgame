using UnityEngine;
using System.Collections;
/*
public interface IGameRole{


	/// <summary>
	/// 此函数调用时切换动作组时会造成Body重新加载
	//所以使用后GetBodyObj GetBodyComponent等函数的使用要重新获取。
	/// </summary>
	AnimatorState ExcuteCmd(AnimatorState aniState,AnimatorState aniLayerState=AnimatorState.Empty);

	GameObject GetGameRoleRootObj();
	[System.Obsolete("此函数最终将被弃用")]
	GameObject GetBodyObj();
	T GetBodyComponent<T>()where T : UnityEngine.Component;
	int GetRoleType();
	float SetAnimatorSpeed(float s);
	void SetMirror (int nXMirror=1, int nYMirror=1, int nZMirror=1);
	Transform GetGameRoleRootTransfrom ();
	void PauseAnimator();
	void ContiuneAnimator();
	//void FadeOut(float duration);
	RolePropertyM GetRolePropertyM ();
	void SetIRole (int id);
	void SetVisable(bool visable);
}
public class GameRoleFactory
{
	public static IGameRole Create(Transform parent,int roleType,string roleName,AnimatorState aniState)
    {
		IGameRole objRole = new GameRole(parent,roleType,roleName,aniState);
		return objRole;
    }

	public static void Destroy(IGameRole gameRole)
	{
		if (!object.ReferenceEquals (gameRole, null))
		{
			gameRole=null;
		}
	}
}*/
