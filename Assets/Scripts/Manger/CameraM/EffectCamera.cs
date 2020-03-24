using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public struct FollowInfo{
	public Transform tran;
	public Vector3 worldpos;
	public FollowInfo(Transform t,Vector3 p)
	{
		tran = t;
		worldpos = p;
	}
}

public class EffectCamera : MonoBehaviour {
	public static Camera camera;
	public static Camera maincamera;
	public static List<FollowInfo> FollowList = new List<FollowInfo>();
	// Use this for initialization
	void Start () {
		maincamera = Camera.main;
		camera = GetComponent<Camera>();
	}


	public static Vector3 GetEffectPos(Vector3 worldpos,float  extend = 0f)
	{
		if (maincamera != null && camera != null)
		{
			float distant = Mathf.Abs(worldpos.z - maincamera.transform.position.z) + extend;
			Vector3 pos = maincamera.WorldToScreenPoint(worldpos);
			pos.z = 0.9f * distant * maincamera.fieldOfView / camera.fieldOfView;
			return camera.ScreenToWorldPoint(pos);
		}
		else Debug.Log("相机错误");
		return Vector3.zero;
	}
	public void LateUpdate()
	{
		for(int i = 0; i < FollowList.Count; )
		{
			if (FollowList[i].tran == null)
			{
				FollowList.RemoveAt(i);
			}
			else
			{

				float distant = Mathf.Abs(FollowList[i].worldpos.z - maincamera.transform.position.z);
				Vector3 pos = maincamera.WorldToScreenPoint(FollowList[i].worldpos);
				pos.z = 0.9f * distant * maincamera.fieldOfView / camera.fieldOfView;
				FollowList[i].tran.position = camera.ScreenToWorldPoint(pos);
				i++;
			}
		}
	}
	// 如何镜头变化时需要跟着移动要调用这个方法
	public static void AddFollowList(Transform t,Vector3 worldpos)
	{
		for(int i = 0; i < FollowList.Count; )
		{
			if (FollowList[i].tran == null)
			{
				FollowList.RemoveAt(i);
			}
			else
				i++;
		}
		FollowList.Add(new FollowInfo(t,worldpos));
	}
	public static void RemoveFollowList(Transform t)
	{
		for(int i = 0; i < FollowList.Count; )
		{
			if (FollowList[i].tran == null || FollowList[i].tran == t)
			{
				FollowList.RemoveAt(i);
			}
			else
				i++;
		}
	}
}
