#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#define UNITY_EDITOR_LOG
#endif

using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class MapBoat : MonoBehaviour {
    static public MapBoat _Mapboat =null;
	
    //船只破损硝烟点
    public  GameObject SmokeLarge;
    public  GameObject SmokeMiddle;
    public  GameObject SmokeSmall;

    /// <summary>
    /// 船上左出生点
    /// </summary>
    public Transform EndLeft;
	public Transform EndRight;
	

	void Start () {

		_Mapboat = this;
		if(BattleEnvironmentM.GetBattleEnvironmentMode() == BattleEnvironmentMode.Edit)
		{
			EnableBoatAnimator(false);
		}
		else
		{
			//CreateRejectPolygon ();
		}
	}

	void OnEnable() {
		_Mapboat = this;
	}

	void OnDisable() {
		_Mapboat = null;
	}



	void OnDrawGizmos(){


		//DrawRejectPolygon ();
	}


    /// <summary>
    /// 船震效果
    /// </summary>
    /// <param name="t"></param>
	public void Shake(float t = 0.5f)
	{
		List<Life> l = new List<Life>();
		CM.SearchLifeMListInBoat(ref l,LifeMType.BUILD);
		foreach (Life b in l)
		{
			(b as Building).Shake();
		}
		transform.parent.transform.DOShakePosition(t);
	}
    /// <summary>
    /// 冒烟效果
    /// </summary>
    /// <param name="type"></param>
    public  void ShowSmoke(int type)
    {
        switch (type)
        {
            case 0:
                SmokeSmall.SetActive(true);
                break;
            case 1:
                SmokeMiddle.SetActive(true);
                break;
            case 2:
                SmokeLarge.SetActive(true);
                break;
        }
    }



	/// <summary>
	/// 开关船体动画器
	/// </summary>
	/// <param name="isEnable"></param>
	void EnableBoatAnimator(bool isEnable)
	{
		Animator ani = GetComponent<Animator>();
		if (ani)
		{
			ani.enabled = isEnable;
		}
	}
	
}
