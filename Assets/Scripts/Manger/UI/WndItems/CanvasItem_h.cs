using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CanvasItem_h : MonoBehaviour {

    public ButtonClick BtnSelect;
    public UILabel LblLevel;
    public UISprite[] SprsStar;
    public UI2DSprite SprItem;//底图类型
    public UISprite SprQuality;//品阶，最外层外框
    public UILabel LblSmallQuality;
    public UI2DSprite Shape;//形状
    public UILabel LblNumber;//数量
    public UISprite SprHP;
    public GameObject Mask;//选兵 选定遮罩
    public Transform Pivot;//控件中心点

	public UISprite SprQualityBg;

	public UISprite SprDestroy;
	public UILabel LblDestroyPts;

	public UISprite SprBearBg;
	public UISprite SprBearPic;
	public UILabel LblBearPts;

}
