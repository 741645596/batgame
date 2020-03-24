using UnityEngine;
using System.Collections;

/// <summary>
/// 背包道具 详细信息
/// <From>背包系统功能设计总案</From>
/// <Author>QFord</Author>
/// </summary>
public class BeiBaoItemInfoWnd : WndBase {

    public ItemTypeInfo Info;

    /// <summary>
    /// 道具图标
    /// </summary>
    public UISprite SprType;
    /// <summary>
    /// 道具品质
    /// </summary>
    public UISprite SprQuality;
    /// <summary>
    /// 道具名称
    /// </summary>
    public UILabel LblName;
    /// <summary>
    /// 道具数量
    /// </summary>
    public UILabel LblCount;

    /// <summary>
    /// 道具 装备作用说明
    /// </summary>
    public UILabel LblTitle;
    /// <summary>
    /// 道具 装备额外解释
    /// </summary>
    public UILabel LblMessage;

    /// <summary>
    /// 道具出售价格
    /// </summary>
    public UILabel LblPrice;

    /// <summary>
    /// 道具 出售
    /// </summary>
    public UIButton BtnSell;
    /// <summary>
    /// 道具 详情
    /// </summary>
    public UIButton BtnDetail;
    /// <summary>
    /// 道具 使用
    /// </summary>
    public UIButton BtnUse;


    public override void WndStart()
    {
        base.WndStart();
    }
	
}
