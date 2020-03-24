/// <summary>
/// 窗体 效果类
/// </summary>
/// <Author>QFord</Author>
/// <Data>2014-10-31   13:59</Data>
/// <Path>E:\Projs\SVN_Root\trunk\SeizeTheShip\Assets\Scripts\Client\UI\WND</Path>

using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

/// <summary>
/// 窗体 效果类
/// </summary>
public class WndEffects : MonoBehaviour
{

    public GameObject AnimationGo;
    
    public delegate void AnimationComplete(object o);
    public AnimationComplete m_animationComplete;
    
    public virtual void Start()
    {
        if (AnimationGo == null) {
            AnimationGo = gameObject;
        }
    }
    
    public void MoveDownThenShake()
    {
        TweenPosition tp = TweenPosition.Begin(AnimationGo, 0.7f, new Vector3(AnimationGo.transform.position.x, 34f, 0));
        tp.AddOnFinished(DoShake);
    }
    
    void DoShake()
    {
        AnimationGo.transform.DOShakePosition(0.3f);
    }
    /// <summary>
    /// http://itween.pixelplacement.com/documentation.php  easytype 参数说明
    /// http://robertpenner.com/easing/easing_demo.html  easytype 动画演示
    /// </summary>
    public void DoScaleUp()
    {
        AnimationGo.transform.localScale = Vector3.zero;
        AnimationGo.transform.DOScale(Vector3.one, 0.8f);
    }
    
    public static void AddBtnAnimation(UIButton btn)
    {
        if (btn) {
            Animation ani = btn.gameObject.AddComponent<Animation>();
            if (ani) {
                AnimationClip clip = GameObjectLoader.LoadResouce("UIAnimations/", "CurveButton") as AnimationClip;
                if (clip != null) {
                    ani.AddClip(clip, clip.name);
                    ani.clip = clip;
                }
                
            }
            UIPlayAnimation uip =  btn.gameObject.AddComponent<UIPlayAnimation>();
            
        }
    }
    
    public static void PlayWndAnimation(GameObject gameObject, string animationName)
    {
        Animation ani = gameObject.GetComponent<Animation>();
        if (ani) {
            //ani.enabled = true;
            ActiveAnimation.Play(ani, animationName, AnimationOrTween.Direction.Forward);
        }
    }
    /// <summary>
    /// 带有回调的动画播放
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="animationName"></param>
    /// <param name="lEvents"></param>
    public static void PlayWndAnimation(GameObject gameObject, string animationName, List<EventDelegate> lEvents)
    {
        Animation ani = gameObject.GetComponent<Animation>();
        if (ani) {
            ActiveAnimation aa = ActiveAnimation.Play(ani, animationName, AnimationOrTween.Direction.Forward);
            aa.onFinished = lEvents;
        }
    }
    
    /// <summary>
    /// Shows the window animation when the window  will distory.
    /// </summary>
    /// <param name="gameObject">Game object.</param>
    /// <param name="gad">Gad.</param>
    /// <param name="animationName">Animation name.</param>
    public static void PlayWndAnimation(GameObject gameObject, GameObjectActionWait gad, string animationName)
    {
        Animation ani = gameObject.GetComponent<Animation>();
        if (ani) {
            //ani.enabled = true;
            ActiveAnimation.Play(ani, animationName, AnimationOrTween.Direction.Forward);
        }
        
        GameObjectActionExcute gae = gameObject.AddComponent<GameObjectActionExcute>();
        gae.AddAction(gad);
        
    }
    public static GameObject FineChildGameObject(GameObject parent, string childName)
    {
        foreach (Transform child in parent.transform) {
            if (child.gameObject.name == childName) {
                return child.gameObject;
            }
        }
        return null;
        
    }
    public static void DoCloseWndEffect(GameObject obj, Complete complete = null, string ControlStr = "Control", string wndStr = "WndBackground")
    {
        GameObject Bott = WndEffects.FineChildGameObject(obj, ControlStr);
        WndEffects.PlayWndAnimation(Bott, "popupEnd");
        
        GameObjectActionWait gadBack = new  GameObjectActionWait(ConstantData.fPopDownAniTime);;
        gadBack.m_complete = complete;
        
        GameObject WndBack = WndEffects.FineChildGameObject(obj, wndStr);
        if (WndBack != null) {
            WndEffects.PlayWndAnimation(WndBack, gadBack, "wndBackOver");
        } else {
            WndEffects.PlayWndAnimation(Bott, gadBack, "popupEnd");
        }
        
    }
    public static void DoWndEffect(GameObject obj, string ControlStr = "Control", string wndStr = "WndBackground")
    {
        GameObject Bott = WndEffects.FineChildGameObject(obj, ControlStr);
        if (Bott != null) {
            WndEffects.PlayWndAnimation(Bott, "popupStart");
        }
        GameObject WndBack = WndEffects.FineChildGameObject(obj, wndStr);
        if (WndBack != null) {
            UIButton btn = WndBack.GetComponent<UIButton>();
            if (btn != null) {
                btn.IsTweenTarget = false;
                
            }
            //WndEffects.PlayWndAnimation (WndBack,"wndBackStart");
        }
    }
    
    public static void SetWndBBGTweenTarget(GameObject obj, string wndStr = "WndBackground")
    {
        GameObject WndBack = WndEffects.FineChildGameObject(obj, wndStr);
        UIButton btn = WndBack.GetComponent<UIButton>();
        if (btn != null) {
            btn.IsTweenTarget = false;
            btn.IsPlaySound = false;
        }
    }
    
}
