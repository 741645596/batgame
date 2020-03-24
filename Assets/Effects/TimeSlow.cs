using UnityEngine;
using System.Collections;

public class TimeSlow : MonoBehaviour 
{

	public float  timeSpeed;
	// Use this for initialization
	void Start () 
	{
		Time.timeScale=timeSpeed;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
