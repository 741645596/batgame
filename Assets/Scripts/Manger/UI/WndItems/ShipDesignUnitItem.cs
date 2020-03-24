using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using sdata;

public class ShipDesignUnitItem : MonoBehaviour {

    public Transform Parent;
    private int m_id;
    public List<GameObject> m_lGo = new List<GameObject>();
	// Use this for initialization
	void Start ()
    {
        ShowUnit();
	}

    private void ShowUnit()
    {
		StaticShipCanvas info = ShipPlanM.GetShipCanvasInfo(m_id);
        if (info != null)
        {
            List<string> data = ShipPlanM.ParseShapeData(info.Shape);
            int index = 0;
            foreach (string item in data)
            {
                for (int i = 0; i < item.Length; i++)
                {
                    if (item[i] == 49)
                    {
                        NGUIUtil.SetActive(m_lGo[index], true);
                    }
                    index++;
                }
            }
        }
    }

    private void LoadAllUnit()
    {
        for (int i = 0; i < 32; i++)
        {
            GameObject go = NDLoad.LoadWndItem("DesignUnitItem", Parent);
            go.name = "Item" + i;
            m_lGo.Add(go);
        }
        NGUIUtil.RepositionTable(Parent.gameObject);
    }

    public void SetData(int id)
    {
        m_id = id;
    }
	
}
