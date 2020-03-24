using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipDesignMapItem : MonoBehaviour 
{
	public UITablePivot Table;
	public UITable[] TableList;

	private string m_shape;
	private int m_width;
	private KeyValue m_Size;
	private int m_height;

	public void SetData(string shape,int width,KeyValue size)
	{
		m_shape = shape;
		m_width = width;
		m_Size = size;

		SetUI();
	}

	public void SetUI()
	{
		string [] l = m_shape.Split(',');
		m_height = l.Length;
		for(int i = 0;i < l.Length; i++)
		{
			for(int j = 0;j < l[i].Length; j++)
			{
				GameObject go = NDLoad.LoadWndItem("ShipModelItem",TableList[i].transform);
				if(go != null)
				{
					ShipModelItem item = go.GetComponent<ShipModelItem>();
					if(item != null)
					{
						if(l[i][j] == '1')
						{
							item.SetSize(GetSingleWidth(),GetSingleWidth());
						}
						//有可能是甲板层.
						else if(l[i][j] == '0')
						{
							//如果他的下一层不为0则是甲板层.
							bool deck = BattleEnvironmentM.CheckIsDeckGrid(l,i,j);
							item.SetSize(GetSingleWidth(),GetSingleWidth(),deck);
							item.gameObject.SetActive(deck);
						}
					}
				}
			}
			TableList[i].Reposition();
			TableList[i].repositionNow = true;
		}
		StartCoroutine(RepositionTable(1));

	}
	IEnumerator  RepositionTable(int frameCount)
	{
		yield return StartCoroutine(U3DUtil.WaitForFrames(frameCount));
		Table.Reposition();
		Table.repositionNow = true;
		ResetTableX();

	}
	void ResetTableX()
	{
		int leftX = m_Size.value - GetSingleWidth() * m_width;
		int leftY = m_Size.key - GetSingleWidth() * m_height;
		Vector3 pos = Table.transform.localPosition;
		Table.transform.localPosition = new Vector3(pos.x +leftX/2,pos.y -leftY/2,pos.z);
	}
	int GetSingleWidth()
	{
		int offsetX = m_width > 4?18:0;
		int offsetY = m_height > 3?18:0;
		int width =  (m_Size.value - offsetX)/m_width;
		int height = (m_Size.key- offsetY)/m_height;

		int final = width > height ?height:width;
		if(final > 30) final = 30;
		return final;
	}

}