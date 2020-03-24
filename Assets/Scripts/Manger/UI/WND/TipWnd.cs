using UnityEngine;
using System.Collections;
/// <summary>
/// 提示窗口
/// </summary>
/// <Author>QFord</Author>
public class TipWnd : WndTopBase {


	/// <summary>
	/// 标题显示的持续时间
	/// </summary>
	public float ShowDuration = 1.0f;
	/// <summary>
	/// 窗体的深度
	/// </summary>
	public int iDep = 0;

	public TipWnd_h MyHead
	{
		get 
		{
			return (base.BaseHead () as TipWnd_h);
		}
	}

	public override void WndStart ()
	{
		base.WndStart ();
	}
	// Update is called once per frame
	void Update () {
		if(iDep != 0 && iDep != gameObject.transform.localPosition.z)
		{
			gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x,gameObject.transform.localPosition.y,iDep);
		}
		if (ShowDuration <= 0)
        {
            WndManager.DestoryDialog<TipWnd>();
        }
        else
        {
			ShowDuration -= Time.deltaTime;
        }
	}
}
