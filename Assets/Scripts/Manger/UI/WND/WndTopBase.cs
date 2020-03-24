using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// 置顶窗体
/// </summary>
/// <author>zhulin</author>
public class WndTopBase : WndBase {

	/// <summary>
	/// 设置窗口的深度（控制窗口表现的前后）
	/// </summary>
	/// <param name="depth"></param>
	public override void SetWndDepth()
	{
		if(BaseHead().m_listUIpanel.Count == 0)
		{
			NGUIUtil.DebugLog("请给窗口添加UIPanel" + gameObject.name);
		}
		CheckListUIPanel(BaseHead().m_listUIpanel);
		int MaxDepth =  SetDepth (WndManager.TopWndDepth);
		WndManager.TopWndDepth = MaxDepth;
		transform.localPosition = new Vector3 (transform.localPosition.x,transform.localPosition.y,ConstantData.iDepBefore3DModel);
	}
}
