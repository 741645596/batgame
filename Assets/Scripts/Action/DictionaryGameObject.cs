using UnityEngine;
using System.Collections; 
using System.Collections.Generic;

public class DictionaryGameObject : MonoBehaviour {
	[HideInInspector]
	public string type;
	[HideInInspector]
	public List<ItemGameObject> Perfabs = new List<ItemGameObject>();
	/*[HideInInspector]
	public Dictionary<string,HlepPoint> DicPerfabs = new Dictionary<string, HlepPoint>();*/
	// Use this for initialization
	void Start () {
		/*NewUiManager.shareInstance().CreateWnd = CreateWnd;
		
		for (int i = 0; i < Perfabs.Count;i++ )
		{
			CPerfabDelegate del = Perfabs[i];
			CPerfabDelegate tempdel;
			if (!DicPerfabs.TryGetValue(del.ID,out tempdel))
			{
				DicPerfabs.Add(del.ID,del);
			}
		}*/
		/*for (int i = 0; i < Perfabs.Count; )
		{
			CPerfabDelegate del = Perfabs[i];

			if (del.ID >= 0)
			{
				GameObject go = GameObject.Instantiate(del.target) as GameObject;
				if (go != null)
				{
					if (RootCamera != null)
					{
						go.transform.parent = RootCamera.transform;
						go.transform.localPosition = del.target.transform.localPosition;
						go.transform.localRotation = del.target.transform.localRotation;
					}
					NewUiManager.shareInstance().AddPanel(del.ID,go);
				}
				++i;
			}
		}*/
	}
	public GameObject CreateWnd(int ID)
	{
		/*CPerfabDelegate Perfab=null;
		if(DicPerfabs.TryGetValue(ID,out Perfab))
		{
			GameObject go = GameObject.Instantiate(DicPerfabs[ID].target) as GameObject;
			if (go != null)
			{
				GameObject parent = null;
				if (DicPerfabs[ID].Parent != null)
					parent = DicPerfabs[ID].Parent;
				else if (RootCamera != null)
					parent = RootCamera;
				
				if (parent != null)
				{
					go.transform.parent = parent.transform;
					go.transform.localPosition = DicPerfabs[ID].target.transform.localPosition;
					go.transform.localRotation = DicPerfabs[ID].target.transform.localRotation;
					go.transform.localScale = DicPerfabs[ID].target.transform.localScale;
				}
				return go;
			}
		}*/
		return null;
	}
	// Update is called once per frame
	void Update () {
	
	}
	public int  CheckKey(string key)
	{
		int count = 0;
		foreach(ItemGameObject h in Perfabs)
		{
			if (h.ID == key)
				count++;
		}
		return count;
	}
	public GameObject GetVauleByKey(string key)
	{
		foreach(ItemGameObject h in Perfabs)
		{
			if (h.ID == key)
				return h.target;
		}
		return null;
	}
}
