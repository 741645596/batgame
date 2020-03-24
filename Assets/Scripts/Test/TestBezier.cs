using UnityEngine;
using System.Collections;

public class TestBezier : MonoBehaviour
{
    /// <summary>
    /// start
    /// </summary>
	public Transform v1;
	public Transform v2;
	public Transform v3;
    /// <summary>
    /// end
    /// </summary>
	public Transform v4;
	Bezier myBezier;
	// Use this for initialization
	void Start () {
	
	}
	// Update is called once per frame
	void OnDrawGizmos(){
		myBezier = new Bezier( v1.position,  v2.position,  v3.position, v4.position );
		
		Gizmos.color = Color.red;
		if (myBezier != null)
		{
			string str = "start:";
			for(float i = 0; i <= 1; i+=0.01f)
			{
				str += myBezier.GetPointAtTime(i) + ",  ";
				Vector3 s  = myBezier.GetPointAtTime(i);
				Vector2 e =myBezier.GetPointAtTime(i+0.01f);
				Gizmos.DrawLine(s,e);
			}
			//Debug.Log(str);
		}
	}
}
