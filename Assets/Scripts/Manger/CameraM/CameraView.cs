using UnityEngine;
using System.Collections;

/// <summary>
///  根据房间宽度 设定相机在不同宽高比的设备中 的zoomLimitMax
/// 基本算法：三角函数
/// http://docs.unity3d.com/ScriptReference/Camera-fieldOfView.html
/// 14.8.20 QFord
/// </summary>
public class CameraView : MonoBehaviour
{

		private Camera theCamera;
	
		//距离摄像机8.5米 用黄色宽、绿色高 表示
		public float upperDistance = 8.5f;
		//距离摄像机12米 用红色表示
		public float lowerDistance = 12.0f;

		//设定的房间宽度
		
		private float roomWidth = 9.0f;
		//最近镜头房间占屏幕的百分比
		private float nearestRoomByScreen = 0.66667f;
		//最远镜头房间占屏幕的百分比
		private float farestRoomByScreen = 0.152f;
		private Transform transform;

		void Awake ()
		{
				if (!theCamera) {
						theCamera = Camera.main;
				}
				
				transform = theCamera.transform;
				//计算得出的相机限制z的宽度值
				float nearCornerWidth = roomWidth / nearestRoomByScreen;
				float zoomLimitMax = GetCameraLimitZ (nearCornerWidth / 2);
				upperDistance = zoomLimitMax;

				float farCornerWidth = roomWidth / farestRoomByScreen;
				float zoomLimitMin = GetCameraLimitZ (farCornerWidth / 2);

		        //MoveCameraEx cp = theCamera.GetComponent<MoveCameraEx> ();
				//if (cp) {
					//	cp.zoomLimitMax = -zoomLimitMax;
					//	cp.zoomLimitMin = -zoomLimitMin;
						// 设定相机初始位置
						//transform.position = new Vector3 (transform.position.x, transform.position.y, -zoomLimitMin);
				//} else {
						//App.log.To ("CameraView.cs", "CombatPre.cs not found!!!");
				//}
		}
	
		void  Update ()
		{

		}

		float GetCameraLimitZ (float width)
		{
				float halfFOV = (theCamera.fieldOfView * 0.5f) * Mathf.Deg2Rad;
				float height = width / theCamera.aspect;
				return height / Mathf.Tan (halfFOV);
		}
}
