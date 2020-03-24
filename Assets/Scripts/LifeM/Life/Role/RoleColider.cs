using UnityEngine;
using System.Collections;

[System.Flags]
public enum ColiderType
{
	Fire  = 0x01,    //炮战碰撞器
	Click = 0x02,    //点击碰撞器
	ALL     = Fire | Click , //所有碰撞器
}

public class RoleColider : MonoBehaviour {
	
	public Collider FireColider; //炮战碰撞器
	public Collider ClickColider;//点击碰撞器


	private void EnableFireColider(bool bEnable)
	{
		if (FireColider != null)
			FireColider.enabled = bEnable;
	}

	public bool IsFireColider(Collider collider)
	{
		if (FireColider != null && FireColider == collider)
			return true;
		else
			return false;
	}

	private void EnableClickColider(bool bEnable)
	{
		if (ClickColider != null)
			ClickColider.enabled = bEnable;
	}

	public bool IsClickColider(Collider collider)
	{
		if (ClickColider != null && ClickColider == collider)
			return true;
		else
			return false;
	}

	public void EnableColider(ColiderType Type,bool bEnable)
	{
		if((Type & ColiderType.Fire) == ColiderType.Fire)
		{
			EnableFireColider(bEnable);
			EnableClickColider(!bEnable);
		}
		//
		if((Type & ColiderType.Click) == ColiderType.Click)
		{
			EnableFireColider(!bEnable);
			EnableClickColider(bEnable);
		}
	}

    public void EnableCollider(bool bEnable)
    {
        EnableFireColider(bEnable);
        EnableClickColider(bEnable);
    }
}
