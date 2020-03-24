using UnityEngine;
using System.Collections;

public class SkillIntroduceWnd : WndBase {

	public SkillIntroduceWnd_h MyHead
	{
		get 
		{
			return (base.BaseHead () as SkillIntroduceWnd_h);
		}
	}
	// Use this for initialization
	public override void WndStart()
	{
		base.WndStart();
	}
	

}
