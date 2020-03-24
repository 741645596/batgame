using UnityEngine;
using System.Collections;
///<summary>
/// UV动画控制脚本
/// Developed by：QFord
/// Time：2014.4.4
/// ver 1.0
/// </summary>
public class UV_Animation : MonoBehaviour {
	//材质索引，单个材质默认为0，无需更改
	 int materialIndex = 0;
	//控制材质球移动的方向和速度
	public Vector2 uvAnimationRate = new Vector2( 1.0f, 0.0f );
	//需要控制的shader材质
	 string textureName = "_MainTex";
	Vector2 uvOffset = Vector2.zero;
	
	// Update is called once per frame
	void Update () {
		//设置材质偏移值
		uvOffset += ( uvAnimationRate * Time.deltaTime );
		if( GetComponent<Renderer>().enabled )
		{
			//让材质偏移
			GetComponent<Renderer>().materials[ materialIndex ].SetTextureOffset( textureName, uvOffset );
		}
	}
}
