using UnityEngine;
using System.Collections;

public class CaptionUpgradeWnd : WndTopBase {
	public CaptionUpgradeWnd_h MyHead
	{
		get 
		{
			return (base.BaseHead () as CaptionUpgradeWnd_h);
		}
	}
	// Use this for initialization
    public override void WndStart()
    {
		base.WndStart();
		UIPanel panel = GetComponent<UIPanel>();
		Debug.Log("CaptionUpgradeWnd  " +  panel.startingRenderQueue);
		if (MyHead.BtnBg)
		{
			MyHead.BtnBg.OnClickEventHandler += BtnBg_OnClickEventHandler;
		}
		WndEffects.DoWndEffect(gameObject);
	}
	
	void BtnBg_OnClickEventHandler(UIButton sender)
	{
		WndEffects.DoCloseWndEffect(gameObject,CloseCallBack);
	}

	void CloseCallBack(object o)
	{
		UIPanel panel = GetComponent<UIPanel>();
		Debug.Log("CaptionUpgradeWnd  " +  panel.startingRenderQueue);
		WndManager.DestoryDialog<CaptionUpgradeWnd>();
	}

	public void SetData(int levelfrom,int levelto,int physicalfrom, int physicalto,int upphysicalfrom, int upphysicalto, int upherolevelfrom, int upherolevelto)
	{
		MyHead.TxtPreLevel.text = levelfrom.ToString();
		MyHead.TxtLevel.text = levelto.ToString();
		MyHead.TxtLevelFrom.text = levelfrom.ToString();
		MyHead.TxtLevelTo.text = levelto.ToString();
		MyHead.TxtPhysicalFrom.text = physicalfrom.ToString();
		MyHead.TxtPhysicalTo.text = physicalto.ToString();
		MyHead.TxtUpPhysicalFrom.text = upphysicalfrom.ToString();
		MyHead.TxtUpPhysicalTo.text = upphysicalto.ToString();
		MyHead.TxtUpHeroFrom.text = upherolevelfrom.ToString();
		MyHead.TxtUpHeroTo.text = upherolevelto.ToString();
	}
//	void Update()
//	{
//        UIPanel panel = GetComponent<UIPanel>();
//        MyHead.bg.SetMeshRender(panel.startingRenderQueue);
//        MyHead.bg.SetParticleRender(panel.startingRenderQueue);
//        MyHead.fg.SetMeshRender(panel.startingRenderQueue);
//        MyHead.fg.SetParticleRender(panel.startingRenderQueue);
//	}
}
