//--------------------------------------------
//            NGUI: HUD Text
// Copyright 漏 2012 Tasharen Entertainment
//--------------------------------------------

using UnityEngine;

/// <summary>
/// Attaching this script to an object will make it visibly follow another object, even if the two are using different cameras to draw them.
/// </summary>

[AddComponentMenu("NGUI/Examples/Follow Target")]
public class UIFollowTarget : MonoBehaviour
{
    public bool IsBtnsUI = false;
    public bool IsCrossUI = false;
    /// <summary>
    /// 鐩?爣鐨勯暱瀹继
    /// </summary>
    public Vector2 Size = Vector2.zero;
	/// <summary>
	/// 3D target that this object will be positioned above.
	/// </summary>

	public Transform target;

	/// <summary>
	/// Game camera to use.
	/// </summary>

	public Camera gameCamera;

	/// <summary>
	/// UI camera to use.
	/// </summary>

	public Camera uiCamera;

	/// <summary>
	/// Whether the children will be disabled when this object is no longer visible.
	/// </summary>

	public bool disableIfInvisible = true;

	Transform mTrans;
	bool mIsVisible = false;

	/// <summary>
	/// Cache the transform;
	/// </summary>

    BoxCollider boxCollider;

	void Awake () { mTrans = transform; }

	/// <summary>
	/// Find both the UI camera and the game camera so they can be used for the position calculations
	/// </summary>

	void Start()
	{
		if (target != null)
		{
			if (gameCamera == null) gameCamera = NGUITools.FindCameraForLayer(target.gameObject.layer);
			if (uiCamera == null) uiCamera = NGUITools.FindCameraForLayer(gameObject.layer);
			SetVisible(false);
			boxCollider = target.gameObject.GetComponent<BoxCollider>();
		}
		else
		{
			Debug.LogError("Expected to have 'target' set to a valid transform", this);
			enabled = false;
		}
	}

	/// <summary>
	/// Enable or disable child objects.
	/// </summary>

	void SetVisible (bool val)
	{
		mIsVisible = val;

		for (int i = 0, imax = mTrans.childCount; i < imax; ++i)
		{
			NGUITools.SetActive(mTrans.GetChild(i).gameObject, val);
		}
	}

	/// <summary>
	/// Update the position of the HUD object every frame such that is position correctly over top of its real world object.
	/// </summary>

	void LateUpdate ()
	{
		if (gameCamera == null || target ==null)
						return;
        Vector3 tPos = target.position;

        if (IsBtnsUI)
        {
            float xOffect = 0;
            float yOffect = 1.5f;
            
            if (boxCollider)
            {
                xOffect = 3;
            }
            tPos = new Vector3(tPos.x + xOffect, tPos.y - yOffect, tPos.z);
        }
        if (IsCrossUI)
        {
            float xOffect = 1.5f * Size.x;
            float yOffect = 1.5f * Size.y;
            tPos = new Vector3(tPos.x + xOffect, tPos.y + yOffect, tPos.z);
        }

        Vector3 pos = gameCamera.WorldToViewportPoint(tPos);


		// Determine the visibility and the target alpha
		bool isVisible = (gameCamera.orthographic || pos.z > 0f) && (!disableIfInvisible || (pos.x > 0f && pos.x < 1f && pos.y > 0f && pos.y < 1f));

		// Update the visibility flag
		if (mIsVisible != isVisible) SetVisible(isVisible);

		// If visible, update the position
		if (isVisible) 
		{
				transform.position = uiCamera.ViewportToWorldPoint (pos);
				pos = mTrans.localPosition;
				pos.x = Mathf.FloorToInt (pos.x);
				pos.y = Mathf.FloorToInt (pos.y);
				pos.z = 0f;
				mTrans.localPosition = pos;
		} 
		else 
		{
		}
		OnUpdate(isVisible);
	}

	/// <summary>
	/// Custom update function.
	/// </summary>

	protected virtual void OnUpdate (bool isVisible) { }
}
