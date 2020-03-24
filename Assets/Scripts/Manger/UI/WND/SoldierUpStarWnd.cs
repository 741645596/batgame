using UnityEngine;
using System.Collections;

public class SoldierUpStarWnd : WndTopBase {

    private SoldierInfo m_preInfo;
    private SoldierInfo m_curInfo;

	public SoldierUpStarWnd_h MyHead
	{
		get 
		{
			return (base.BaseHead () as SoldierUpStarWnd_h);
		}
	}

    public void SetData(SoldierInfo preInfo, SoldierInfo curInfo)
    {
        m_preInfo = preInfo;
        m_curInfo = curInfo;
        SetUI();
    }

	public override void WndStart()
	{
		base.WndStart();
		//设置窗体在3D模型之前
		transform.localPosition = new Vector3 (transform.localPosition.x,transform.localPosition.y,ConstantData.iDepBefore3DModel);
		MyHead.BtnClose.OnClickEventHandler += BtnClose_OnClickEventHandler;

		WndEffects.DoWndEffect(gameObject);
	}

	void DestoryDialogCallBack(object o)
	{
        WndManager.DestoryDialog<SoldierUpStarWnd>();	
	}
	private void BtnClose_OnClickEventHandler(UIButton sender)
	{
		WndEffects.DoCloseWndEffect(gameObject,DestoryDialogCallBack);
	}

    void SetUI()
    {
        NGUIUtil.SetLableText<string>(MyHead.Lbl01, (m_preInfo.m_strength_grow*0.01).ToString());
        NGUIUtil.SetLableText<string>(MyHead.Lbl02, (m_curInfo.m_strength_grow*0.01).ToString());

        NGUIUtil.SetLableText<string>(MyHead.Lbl03, (m_preInfo.m_intelligence_grow * 0.01).ToString());
        NGUIUtil.SetLableText<string>(MyHead.Lbl04, (m_curInfo.m_intelligence_grow*0.01).ToString());

        NGUIUtil.SetLableText<string>(MyHead.Lbl05, (m_preInfo.m_agility_grow*0.01).ToString());
        NGUIUtil.SetLableText<string>(MyHead.Lbl06, (m_curInfo.m_agility_grow*0.01).ToString());

		if(MyHead.AfterItem != null)
		{
			GameObject go = NDLoad.LoadWndItem("CanvasItem", MyHead.AfterItem.transform);
			if (go)
			{
				CanvasItem item = go.GetComponent<CanvasItem>();
				if (item)
				{
					item.SetCanvasItem(m_curInfo,0,false);
				}
			}

		}
		if(MyHead.PreItem != null)
		{
			GameObject go = NDLoad.LoadWndItem("CanvasItem", MyHead.PreItem.transform);
			if (go)
			{
				CanvasItem item = go.GetComponent<CanvasItem>();
				if (item)
				{
					item.SetCanvasItem(m_preInfo,0,false);
				}
			}
		}

        float strAddNum = Mathf.Abs( (m_curInfo.m_strength_grow - m_preInfo.m_strength_grow) *0.01f);
        float intAddNum =Mathf.Abs( (m_curInfo.m_intelligence_grow - m_preInfo.m_intelligence_grow) *0.01f);
        float agiAddNum =Mathf.Abs( (m_curInfo.m_agility_grow - m_preInfo.m_agility_grow) *0.01f);

        MyHead.LblAdd1.text = "(" + NGUIUtil.GetStringByKey(88800047) + "+" +( strAddNum).ToString() + ")";
        MyHead.LblAdd2.text = "(" + NGUIUtil.GetStringByKey(88800049) + "+" +( intAddNum).ToString() + ")";
        MyHead.LblAdd3.text = "(" + NGUIUtil.GetStringByKey(88800048) + "+" + (agiAddNum).ToString() + ")";
    }

	public void SetLevelSprite(UISprite spr,int quality)
	{
		int bigLevel = ConfigM.GetBigQuality (quality);
		NGUIUtil.SetSprite(spr, bigLevel.ToString());
	}
	/// <summary>
	/// 设置 角色名称 + 彩色品质等级 + 设置角色品质框（背景） + 战斗力
	/// </summary>
	/// <param name="name">角色名称 s_soldierType</param>
	/// <param name="quality">角色品质 d_soldier</param>
	public void SetSmallQuality(UILabel labl,int quality)
	{
		labl.text = NGUIUtil.GetSmallQualityStr(quality);
		
	}

}
