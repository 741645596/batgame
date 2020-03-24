using UnityEngine;
using System.Collections;

public class ClickBuildBtns : MonoBehaviour {

    public GameObject Parent;

    public UIButton BtnInfo;
    public UIButton BtnUpgrade;
    public UIButton BtnReturnBag;
    public UIButton BtnDeleteSoldier;
    public UIButton BtnSetTransGate;

    void Start()
    {
        NGUIUtil.RepositionTable(Parent);
    }

    public void ShowBtnReturnBag(bool b)
    {
        if (BtnReturnBag)
        {
            BtnReturnBag.gameObject.SetActive(b);
        }
    }
    public void ShowBtnDeleteSoldier(bool b)
    {
        if (BtnDeleteSoldier)
        {
            BtnDeleteSoldier.gameObject.SetActive(b);
        }
    }
    public void ShowBtnSetTransGate(bool b)
    {
        if (BtnSetTransGate)
        {
            BtnSetTransGate.gameObject.SetActive(b);
        }
    }
    public void ShowBtnBtnReturnBag(bool b)
    {
        if (BtnReturnBag)
        {
            BtnReturnBag.gameObject.SetActive(b);
        }
    }
    public void RefreshUI()
    {
        NGUIUtil.RepositionTablePivot(Parent);
    }
	
}
