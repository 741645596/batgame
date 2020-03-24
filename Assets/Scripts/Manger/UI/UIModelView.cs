
using UnityEngine;
using System;
using System.Collections.Generic;


public class UIModelView : UITexture
{
    [HideInInspector][SerializeField] public Camera mModelCamera;
	[HideInInspector][SerializeField] public GameObject mSceneLayer = null;
	[HideInInspector][SerializeField] bool mUseRenderTexture = false;
	public bool UseRenderTexture { set { mUseRenderTexture = value; } }
    GameObject mModelObj;
    bool mLocked = false;
    RenderTexture mRenderTexture = null;

    public Camera ModelCamera
    {
        get
        {
            return mModelCamera;
        }

        set
        {
            mModelCamera = value;
        }
    }


    public bool LockRotation
    {
        get { return mLocked; }
        set { mLocked = value; }
    }

    void Awake()
    {
        base.Awake();
        mModelCamera.depth = 0.0f;
    }

	void OnEnable()
	{
		base.OnEnable();
		if (mUseRenderTexture)
		{
			CreateRenderTexture();
		}
		else
		{
			ModelCamera.gameObject.SetActive(true);
		}		
	}

	void OnDisable()
	{
		base.OnDisable();
		if (mUseRenderTexture)
		{
			ForceReleaseRenderTarget();
		}
		else
		{
			ModelCamera.gameObject.SetActive(false);
		}
		
	}

	void CreateRenderTexture()
	{
        if (Application.isPlaying && mRenderTexture == null)
		{
            mRenderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            mRenderTexture.Create();

            mModelCamera.targetTexture = mRenderTexture;
            mainTexture = mRenderTexture;
			ModelCamera.gameObject.SetActive(false);
			ModelCamera.gameObject.SetActive(true);
		}
	}

    //强制RenderTarget Release
    void ForceReleaseRenderTarget()
    {
        if (mRenderTexture != null)
        {
            mainTexture = null;
            ModelCamera.targetTexture = null;
            mRenderTexture.Release();
            RenderTexture.active = null;
            GameObject.DestroyImmediate(mRenderTexture);
            mRenderTexture = null;
        }
        ModelCamera.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            base.Update();
        }
    }

    void OnDrag(Vector2 delta)
    {
        if (mModelObj != null && !LockRotation)
        {
            Quaternion deltaRotation = Quaternion.Euler(0, -delta.x, 0);
            mModelObj.transform.localRotation *= deltaRotation;
        } 
    }

    void OnDestroy()
    {
		ForceReleaseRenderTarget();
    }





}