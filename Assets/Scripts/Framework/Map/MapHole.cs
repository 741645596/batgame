using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// 地图洞结构，连续几个格子类型为洞类型 称为一个洞
/// </summary>
/// <author>zhulin</author>
/// 

public class MapHole : MapStations  {

	public Int2 Start = Int2.zero;
	public Int2 End = Int2.zero;
	
    //初始化用
	public void  SetHole(Int2 Start ,Int2 End)
	{
		if (Start.Layer != End.Layer) 
		{
			Debug.Log("SetHole 数据出错");
			return;
		}
		this.Start = Start;
		this.End = End;
	}
}
