using UnityEngine;
using System.Collections;

public class PlayAnimationQueuedOnEnable : MonoBehaviour 
{
	public AnimationClip[] AnimationClips;
	public bool IgnoreTimeScale = true;
	public float Val = 0;

	Animation mAniamtion;
	AnimationClip mFistClip;
	string[] mClipNames;
	string  mCurClipName;
	int mCurClipIndex;

	float mLastFrameTime;
	float mProgressTime;
	float mcurTime;
	bool bUpdate = false;
	
	void Awake ()
	{
		mAniamtion = GetComponent<Animation>();
		if (mAniamtion == null)
		{
			mAniamtion = gameObject.AddComponent<Animation>();
		}
		mClipNames = new string[AnimationClips.Length];
		for (int i=0; i<AnimationClips.Length; i++)
		{
			AnimationClip clip = AnimationClips[i];
			mAniamtion.AddClip(clip, clip.name);
			mClipNames[i] = clip.name;
		}
	}

	void OnEnable ()
	{
		if (mAniamtion != null)
		{
			mAniamtion.Rewind();
			mCurClipIndex = 0;
			mAniamtion.clip = AnimationClips[mCurClipIndex];
			mCurClipName = mClipNames[mCurClipIndex];
			mAniamtion.Play(mClipNames[mCurClipIndex]);
			mLastFrameTime = Time.realtimeSinceStartup; // 记录此帧时间，下一帧用
			mcurTime = Time.realtimeSinceStartup;
			mProgressTime = 0; // 动画已播放时间
		}
	}
	void PlayNextClip()
	{
		if (mAniamtion != null)
		{
			mCurClipIndex++;
			if (mCurClipIndex < mClipNames.Length)
			{
				mAniamtion.clip = AnimationClips[mCurClipIndex];
				mCurClipName = mClipNames[mCurClipIndex];
				mAniamtion.Play(mCurClipName);
				mLastFrameTime = Time.realtimeSinceStartup; // 记录此帧时间，下一帧用
				mcurTime = Time.realtimeSinceStartup;
				mProgressTime = 0; // 动画已播放时间
			}	
		}	
	}

	void OnDisable ()
	{
		mAniamtion.Stop();
	}

	void OnDestroy()
	{
		mClipNames = null;
	}

	void Update()
	{
		if (!bUpdate)
		{
			bUpdate = true;
			OnEnable();
		}
		if (mAniamtion.IsPlaying(mCurClipName))
		{
			AnimationState animState = mAniamtion[mCurClipName]; // 当前动画状态
			mcurTime = Time.realtimeSinceStartup;
			float deltaTime = mcurTime - mLastFrameTime; // 此帧与上一帧的时间间隔
			mLastFrameTime = mcurTime; // 记录此帧时间，下一帧用
			mProgressTime += deltaTime; // 动画已播放时间
			animState.normalizedTime = mProgressTime / animState.length; // 动画规范化时间[0-1]
			return;
		}
		PlayNextClip();

	}
}
