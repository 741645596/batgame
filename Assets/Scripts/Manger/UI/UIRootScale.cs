/// <summary>
/// NGUI屏幕自适应方案
/// </summary>
/// <Author>QFord</Author>
/// <Data>2014-11-11   16:03</Data>
/// <Path>E:\Projs\SVN_Root\trunk\SeizeTheShip\Assets\Scripts\Client\UI</Path>
/*
使用注意：

1、和策划制定好开发时分辨率。这很重要，要保证所有UI都在同样的分辨率下制作。

2、这个脚本挂在UIRoot上。UIRoot的Scaling Style修改为FixedSize。

3、aspectRatioHeight、aspectRatioWidth分别为开发时的高和宽。

4、每个UIRoot都需要调整ManualHeight到和策划制定的高度。

5、Unity3D的Game窗口，调整到相应的分辨率。
 
6、背景图大小用代码进行满屏（可参考CombatPrepareWnd L38）
 */
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(UIRoot))]
public class UIRootScale : MonoBehaviour
{
    public int aspectRatioHeight;
    public int aspectRatioWidth;
    public bool runOnlyOnce = false;
    private UIRoot mRoot;
    private bool mStarted = false;

    void Awake()
    {
        UICamera.onScreenResize += ScreenSizeChanged;
        mRoot = NGUITools.FindInParents<UIRoot>(this.gameObject);

        mRoot.scalingStyle = UIRoot.Scaling.FixedSize;

        this.Update();
        mStarted = true;
    }

    void OnDestroy()
    {
        UICamera.onScreenResize -= ScreenSizeChanged;
    }

    void Start()
    {
        //mRoot = NGUITools.FindInParents<UIRoot>(this.gameObject);

        //mRoot.scalingStyle = UIRoot.Scaling.FixedSize;

        //this.Update();
        //mStarted = true;
    }

    void ScreenSizeChanged()
    {
        if (mStarted && runOnlyOnce)
        {
            this.Update();
        }
    }

    void Update()
    {
        float defaultAspectRatio = aspectRatioWidth * 1f / aspectRatioHeight;
        float currentAspectRatio = Screen.width * 1f / Screen.height;

        if (defaultAspectRatio > currentAspectRatio)
        {
            int horizontalManualHeight = Mathf.FloorToInt(aspectRatioWidth / currentAspectRatio);
            mRoot.manualHeight = horizontalManualHeight;
        }
        else
        {
            mRoot.manualHeight = aspectRatioHeight;
        }

        if (runOnlyOnce && Application.isPlaying)
        {
            this.enabled = false;
        }
    }
}  
