using UnityEngine;
using System.Collections;

public class DefenceColider : MonoBehaviour {

	public Collider leftCollider;
	public Collider rightCollider;
	public Collider topCollider;
	public Collider bottomCollider;
	// Use this for initialization
	void Start () {

		Collider []coliders = gameObject.GetComponents<Collider>();
		if (coliders.Length == 2) 
		{
			if (null != leftCollider&&null != rightCollider)
			{
				if(coliders[0].bounds.center.x>coliders[1].bounds.center.x)
				{
					leftCollider = coliders[1];
					rightCollider = coliders[0];
				}
				else{
					leftCollider = coliders[0];
					rightCollider = coliders[1];
				}
			}
			if (null != topCollider&&null != bottomCollider)
			{
				if(coliders[0].bounds.center.y>coliders[1].bounds.center.y)
				{
					topCollider = coliders[0];
					bottomCollider = coliders[1];
				}
				else{
					topCollider = coliders[1];
					bottomCollider = coliders[0];
				}
			}
		}
		if(BattleEnvironmentM.GetBattleEnvironmentMode() == BattleEnvironmentMode.Edit)
		{
			if(leftCollider!=null)
				leftCollider.enabled=false;
			if(rightCollider!=null)
				rightCollider.enabled=false;
			if(topCollider!=null)
				topCollider.enabled=false;
			if(bottomCollider!=null)
				bottomCollider.enabled=false;
			return;
			
		}
	
	}

	public void EnableAllColider(bool bEnable)
	{
		if (null != leftCollider)
			leftCollider.enabled = bEnable;
		if (null != rightCollider)
			rightCollider.enabled = bEnable;
		if (null != topCollider)
			topCollider.enabled = bEnable;
		if (null != bottomCollider)
			bottomCollider.enabled = bEnable;
	}
	public bool IsInLeftCollider(Collider colider)
	{
		if (leftCollider != null && leftCollider == colider)
						return true;
		return false;
	}
	public bool IsInRightCollider(Collider colider)
	{
		if (rightCollider != null && rightCollider == colider)
			return true;
		return false;
	}
	public bool IsInTopCollider(Collider colider)
	{
		if (topCollider != null && topCollider == colider)
			return true;
		return false;
	}
	public bool IsInBottomCollider(Collider colider)
	{
		if (bottomCollider != null && bottomCollider == colider)
			return true;
		return false;
	}

    public bool IsUnderCollider(Collider colider)
    {
        if (gameObject.transform.position.y<= colider.gameObject.transform.position.y)
        {
            return true;
        }
        return false;
    }
}

