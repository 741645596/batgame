using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayTweenByUITablePivot : MonoBehaviour
{
	public Vector3 Offset;
	public AnimationCurve AnimationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
	public bool IgnoreTimeScale = true;
	public float Duration = 1f;
	public float Step = 1f;

	UITablePivot mTablePivot;
	Transform mCachedTransform;
	Dictionary<Vector3, Vector3> mTweenPositionMap = null;
	List<Dictionary<Vector3, Vector3>> mTweenPositionMaps = null;
	List<Transform> mChildren = null;

	void Awake()
	{
		mCachedTransform = transform;
		mTweenPositionMap = new Dictionary<Vector3, Vector3>();
		mTweenPositionMaps = new List<Dictionary<Vector3, Vector3>>();
		mTablePivot = GetComponent<UITablePivot>();
		mTablePivot.onReposition += PlayTween;
	}

	void OnDestroy()
	{
		mTweenPositionMap = null;
		mTweenPositionMaps = null;

		mTablePivot.onReposition -= PlayTween;
	}

	void PlayTween()
	{
		//return;
		mChildren = mTablePivot.children;
		for (int i = 0; i < mChildren.Count; i++ )
		{
			Transform child = mChildren[i];
			Vector3 localPostionTo = child.localPosition;
			Vector3 localPositionFrom = localPostionTo - Offset;
			child.localPosition = localPositionFrom;
			TweenPosition tweenPosition = child.GetComponent<TweenPosition>();
			if (tweenPosition == null)
			{
				tweenPosition = child.gameObject.AddComponent<TweenPosition>();
			}
			tweenPosition.from = localPositionFrom;
			tweenPosition.to = localPostionTo;
			tweenPosition.duration = Duration;
			tweenPosition.animationCurve = AnimationCurve;
			tweenPosition.ignoreTimeScale = IgnoreTimeScale;
			tweenPosition.delay = Step * i;


			tweenPosition.ResetToBeginning();
			tweenPosition.PlayForward();
			
		}


	}

}
