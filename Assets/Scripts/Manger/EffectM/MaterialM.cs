using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaterialPropertyItem
{
    public string strPropertyName;
    public Color clrValue;
    public Material matValue;
}
public class MaterialM : MonoBehaviour
{
    public static List<MaterialM> s_listMaterail = new List<MaterialM>();
    
    public static void AddMaterialM(MaterialM matM)
    {
        s_listMaterail.Add(matM);
    }
    public static void RemoveMaterialM(MaterialM matM)
    {
        s_listMaterail.Remove(matM);
    }
    public static void DarkBattleEnv()
    {
        foreach (MaterialM matM in s_listMaterail) {
            matM.ToDark();
        }
    }
    public static void DarkRevertBattleEnv()
    {
        foreach (MaterialM matM in s_listMaterail) {
            matM.DarkRevert();
        }
    }
    
    
    public List<int> m_DarkList = new List<int>();
    private List<MaterialPropertyItem> m_matDarkPropertyTemp = new List<MaterialPropertyItem>();
    void Start()
    {
        MaterialM.AddMaterialM(this);
    }
    public void OnDestroy()
    {
        MaterialM.RemoveMaterialM(this);
    }
    public void ToDark()
    {
        DarkRevert();
        Renderer[] renderList = GetComponents<Renderer>();
        foreach (Renderer r in renderList) {
            Material[] mList = r.materials;
            int nMatIDValue = 0;
            foreach (Material m in mList) {
                foreach (int nMatID in m_DarkList) {
                    if (nMatIDValue == nMatID) {
                        string strProperty = "_Emission";
                        Color clr = m.GetColor(strProperty);
                        if (null != clr) {
                            MaterialPropertyItem item = new MaterialPropertyItem();
                            item.matValue = m;
                            item.strPropertyName = strProperty;
                            item.clrValue = clr;
                            m_matDarkPropertyTemp.Add(item);
                            m.SetColor(strProperty, Color.black);
                            m.SetColor("_Color", Color.gray);
                        }
                        break;
                    }
                }
                nMatIDValue += 1;
            }
        }
    }
    public void DarkRevert()
    {
        foreach (MaterialPropertyItem item in m_matDarkPropertyTemp) {
            item.matValue.SetColor(item.strPropertyName, item.clrValue);
        }
        m_matDarkPropertyTemp.Clear();
    }
}
