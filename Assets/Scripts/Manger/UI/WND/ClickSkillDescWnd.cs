using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 
/// <From> </From>
/// <Author>QFord</Author>
/// </summary>
public class ClickSkillDescWnd : WndTopBase {

    public ClickSkillDescWnd_h MyHead
    {
        get
        {
            return (base.BaseHead() as ClickSkillDescWnd_h);
        }
    }

    public override void WndStart()
	{
        base.WndStart();
        
	}

    public void SetData(string str1, string str2,GameObject go)
    {
        MyHead.LblDesc1.text = NGUIUtil.GetNewLineStr(str1);
        MyHead.LblDesc2.text = NGUIUtil.GetNewLineStr(str2);
        FixDepth(go);
    }
	
    /// <summary>
    /// 解决该弹窗和炮弹兵重叠
    /// </summary>
    private void FixDepth(GameObject go)
    {
        //int pdbycWndDepth = WndManager.GetDialogDepth<PdbycWnd>();
        ////NGUIUtil.DebugLog("pdbyc depth =" + pdbycWndDepth);
        //UIPanel panel = GetComponent<UIPanel>();
        //if (panel.depth <= pdbycWndDepth)
        //{
        //    panel.depth = pdbycWndDepth + 1;
        //}
        Vector3 pos = WndManager.GetWndRoot().transform.InverseTransformPoint(go.transform.position);
        transform.localPosition = new Vector3(-400f, pos.y+80f, -1000f);
    }
}
