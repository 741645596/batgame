using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单选框，以横排来显示
/// </summary>
public class RadioButton
{
	private string[] names;
	private bool[] values;
	private int index = -1;
	
	public RadioButton(string[] names, bool[] values)
	{
		this.names = names;
		this.values = values;
		
		for (int i = 0; i < values.Length; i++)
			if (values[i])
				Index = i;
	}
	
	public int Draw(Rect rc)
	{
		for (int i = 0; i < names.Length; i++)
		{
			Rect rect = new Rect(rc.x + i * rc.width/names.Length,
				rc.y, rc.width/names.Length, rc.height);
			values[i] = GUI.Toggle(rect, values[i], names[i]);
			
			if (values[i])
			{
				Index = i;
			}
			else if (Index == i)
				values[i] = true;
		}
		
		return index;
	}
	
	public int Index
	{
		get { return index; }
		set 
		{
			if (value != index && index != -1)
					values[index] = false;
			index = value; 
			values[value] = true;
		}
	}
}
