using UnityEngine;
using System.Collections;

public class LoadingWnd : WndTopBase
{
	AnimatorStateInfo mStateinfoFadeIn;
	AnimatorStateInfo mStateinfoLoading;
	AnimatorStateInfo mStateinfoFadeOut;

	Animator mFadeIn;
	Animator mLoading;
	Animator mFadeOut;

	public LoadingWnd_h MyHead
	{
		get
		{
			return (base.BaseHead() as LoadingWnd_h);
		}
	}

	// Use this for initialization
	public override void WndStart()
	{
		base.WndStart();

		mFadeIn = MyHead.FadeIn;
		mLoading = MyHead.Loading;
		mFadeOut = MyHead.FadeOut;
	}

	public void FadeIn()
	{
		mFadeIn.gameObject.SetActive(true);
		mLoading.gameObject.SetActive(false);
		mFadeOut.gameObject.SetActive(false);
	}

	public bool IsFadindIn()
	{
		mStateinfoFadeIn = mFadeIn.GetCurrentAnimatorStateInfo(0);
		bool result = (mStateinfoFadeIn.normalizedTime > mStateinfoFadeIn.length) && (mStateinfoFadeIn.IsName(mFadeIn.name));
		return !result;
	}

	public void Loading()
	{
		mFadeIn.gameObject.SetActive(false);
		mLoading.gameObject.SetActive(true);
		mFadeOut.gameObject.SetActive(false);
	}

	public bool IsLoading()
	{
		mStateinfoLoading = mLoading.GetCurrentAnimatorStateInfo(0);
		bool result = (mStateinfoLoading.normalizedTime > mStateinfoLoading.length) && (mStateinfoLoading.IsName(mLoading.name));
		return !result;
	}

	public void FadeOut()
	{
		mFadeIn.gameObject.SetActive(false);
		mLoading.gameObject.SetActive(false);
		mFadeOut.gameObject.SetActive(true);
	}

	public bool IsFadindOut()
	{
		mStateinfoFadeOut = mFadeOut.GetCurrentAnimatorStateInfo(0);
		bool result = (mStateinfoFadeOut.normalizedTime > mStateinfoFadeOut.length) && (mStateinfoFadeOut.IsName(mFadeOut.name));
		return !result;
	}
}
