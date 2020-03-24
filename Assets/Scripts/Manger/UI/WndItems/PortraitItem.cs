using UnityEngine;
using System.Collections;

public class SimpleAnimation
{
	public enum Direction
	{
		Reverse = -1,
		Forward = 1,
	}

	public enum PlayingState
	{
		None = 0,
		Playing = 1,
		Stop = 2,
	}

	protected Direction mPlayDirection = Direction.Forward;
	protected string mClipName = null;
	protected Animation mAnimation;
	protected AnimationState mAnimationState;
	protected PlayingState mPlayingState = PlayingState.None;
	protected float mFowardSpeed = 1f;
	protected float mReverseSpeed = 1f;

	public delegate void EventDelegateOnStart();
	public delegate void EventDelegateOnStop();
	public EventDelegateOnStart EventOnStart;
	public EventDelegateOnStop EventOnStop;

	public SimpleAnimation(Animation anim)
	{
		mAnimation = anim;
	}

	public Direction PlayDirection
	{
		set	{	mPlayDirection = value;	}
	}

	public float FowardSpeed
	{
		set	{	mFowardSpeed = value;	}
	}

	public float ReverseSpeed
	{
		set	{	mReverseSpeed = value;	}
	}

	public PlayingState State
	{
		get	{	return mPlayingState;	}
	}

	public void Play(AnimationClip clip)
	{
		mClipName = clip.name;

		foreach (AnimationState state in mAnimation)
		{
			if (state.name == mClipName)
			{
				mAnimationState = state;
			}
		}

		Play();
	}

	public void Play()
	{
		float speed = 0f;

		if (mAnimationState != null)
		{
			speed = Mathf.Abs(mAnimationState.speed);
			mAnimationState.time = (mPlayDirection == Direction.Forward) ? 0 : mAnimationState.length;
			mAnimationState.speed = (mPlayDirection == Direction.Forward) ? mFowardSpeed : (-1f * mReverseSpeed);
			mAnimation.Play(mClipName);
		}

		if (EventOnStart != null)
		{
			EventOnStart();
		}
		mPlayingState = PlayingState.Playing;
	}

	public void OnUpdate()
	{
		if (mPlayingState == PlayingState.Playing)
		{
			if (!mAnimation.IsPlaying(mClipName))
			{
				mPlayingState = PlayingState.Stop;
				if ((EventOnStop != null) && (mPlayDirection == Direction.Forward))
				{
					EventOnStop();
					EventOnStop = null;
				}
			}
		}
	}

	public void AddAnimationClip(AnimationClip clip)
	{
		if (clip == null)
		{
			return;
		}
		mAnimation.AddClip(clip, clip.name);
	}
}

public class PortraitItem : MonoBehaviour
{
	public AnimationClip ClipSelect;
	public AnimationClip ClipCancelSelect;
	public AnimationClip ClipFire;

	Animation mAnimation;
	SimpleAnimation mSimpleAnimation;

	int mOldSoldierID;
	int mNewSoldierID;

	void Awake()
	{
 		mAnimation = GetComponent<Animation>();
		if(mAnimation == null)
		{
			mAnimation = gameObject.AddComponent<Animation>();
		}
		mSimpleAnimation = new SimpleAnimation(mAnimation);
		mSimpleAnimation.AddAnimationClip(ClipSelect);
		mSimpleAnimation.AddAnimationClip(ClipCancelSelect);
		mSimpleAnimation.AddAnimationClip(ClipFire);
	}

	UI2DSprite mPortraitSprite = null;
	public UI2DSprite PortraitSprite
	{
		get
		{
			if (mPortraitSprite == null)	{	mPortraitSprite = GetComponent<PortraitItem_h>().SpritePortrait;	}
			return mPortraitSprite;
		}
	}

	UILabel mDialogue;
	public UILabel Dialogue
	{
		get
		{
 			if(mDialogue == null)	{	mDialogue = GetComponent<PortraitItem_h>().Dialogue;	}
			return mDialogue;
		}
	}


	public void Hide()
	{
		mOldSoldierID = 0;
		mSimpleAnimation.Play(ClipCancelSelect);
	}

	public void ShowSoldierPortrait(int soldierID)
	{
		mNewSoldierID = soldierID;

		// 原先是隐藏的
		if (mOldSoldierID == 0)
		{
			Show();
		}
		// 原先已经有选中的
		else
		{
			Hide();
			mSimpleAnimation.EventOnStop = Show;
		}
		
	}

	void Show()
	{
		mOldSoldierID = mNewSoldierID;
		NGUIUtil.Set2DSprite(PortraitSprite, "Textures/rolePortrait/", mNewSoldierID.ToString(), "null");
		mSimpleAnimation.Play(ClipSelect);
	}

	public void Fire()
	{
		mOldSoldierID = 0;
		mSimpleAnimation.Play(ClipFire);
	}

	void Update()
	{
		mSimpleAnimation.OnUpdate();
	}


	void ShowDialogue(int soldierID)
	{
		Dialogue.text = SoldierM.GetSoldierDialogue(soldierID);
	}
}
