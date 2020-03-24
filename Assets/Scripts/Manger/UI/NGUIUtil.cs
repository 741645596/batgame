#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    #define UNITY_EDITOR_LOG
#endif

using UnityEngine;
using DG.Tweening;


//RGB 颜色对照表  http://www.114la.com/other/rgb.htm
// itween easetype http://www.robertpenner.com/easing/easing_demo.html

public enum NamedValueColor {
    /// <summary>
    /// The main color of a material. Used by default and not required for Color methods to work in iTween.
    /// </summary>
    _Color,
    /// <summary>
    /// The specular color of a material (used in specular/glossy/vertexlit shaders).
    /// </summary>
    _SpecColor,
    /// <summary>
    /// The emissive color of a material (used in vertexlit shaders).
    /// </summary>
    _Emission,
    /// <summary>
    /// The reflection color of the material (used in reflective shaders).
    /// </summary>
    _ReflectColor,
    _TintColor
    
}


public delegate void CallBack();

/// <summary>
/// NGUI 便捷方法类
/// </summary>
/// <author>QFord</author>
public class NGUIUtil
{
    /// <summary>
    /// 设置Sprite图片
    /// </summary>
    /// <param name="ui"></param>
    /// <param name="Name"></param>
    
    const int TopMostDepthZ = - 1000;
    
    public static void SetSprite<T>(UISprite ui, T name)
    {
        if (ui != null) {
            ui.spriteName = name.ToString();
            ui.MarkAsChanged();
        }
    }
    /// <summary>
    /// 设置精灵图片（大小变成原始尺寸）
    /// </summary>
    /// <param name="ui"></param>
    /// <param name="name"></param>
    public static void SetSpritePixelPerfect(UISprite ui, string name)
    {
        if (ui != null) {
            ui.spriteName = name;
            ui.MakePixelPerfect();
            ui.MarkAsChanged();
        }
    }
    /// <summary>
    ///  根据图集设置Sprite的图片(这个必须把图集放置到Resource文件夹下)
    /// </summary>
    /// <param name="ui"></param>
    public static void SetSprite(UISprite ui, string atlasPath, string atlasName, string spriteName)
    {
        GameObject go = Resources.Load(atlasPath + atlasName) as GameObject;
        if (go) {
            UIAtlas a = go.GetComponent<UIAtlas>();
            if (a) {
                ui.atlas = a;
                SetSprite(ui, spriteName);
            }
        }
    }
    
    /// <summary>
    /// 设置 UI2DSprite 的精灵
    /// </summary>
    public static void Set2DSprite(UI2DSprite ui, string path, string name, string defaultTexture = null)
    {
        if (ui != null) {
            UnityEngine.Sprite sprite = Resources.Load<UnityEngine.Sprite>(path + name);
            if (null != sprite) {
                ui.sprite2D = sprite;
                return;
            }
            if (defaultTexture != null) {
#if UNITY_EDITOR_LOG
                Debug.LogWarning("Cant Find Sprite: " + path + name);
#endif
                sprite = Resources.Load<UnityEngine.Sprite>(path + defaultTexture);
                ui.sprite2D = sprite;
            }
        }
    }
    /// <summary>
    /// 设置 UI2DSprite 的精灵
    /// </summary>
    public static void Set2DSprite(UI2DSprite ui, string path, int idSprite)
    {
        Set2DSprite(ui, path, idSprite.ToString());
    }
    /// <summary>
    /// 设置 UI2DSprite 的精灵
    /// </summary>
    public static void Set2DSprite(UI2DSprite ui, string pathName)
    {
        if (ui != null) {
            UnityEngine.Sprite sprite = Resources.Load<UnityEngine.Sprite>(pathName);
            if (null != sprite) {
                ui.sprite2D = sprite;
            }
        }
    }
    ///<summary>
    /// 设置UISprite图片为黑白
    /// http://www.xiaobao1993.com/373.html
    /// </summary>
    public static void SetSpriteGray(UISprite sprite, string spriteName)
    {
        if (sprite == null) {
            return;
        }
        UIAtlas atlas = sprite.atlas;
        UIAtlas cloneAtlas = Object.Instantiate(atlas) as UIAtlas;
        Material mGrayMaterial = new Material(Shader.Find("Unlit/Transparent Gray"));
        mGrayMaterial.mainTexture = atlas.spriteMaterial.mainTexture;
        cloneAtlas.spriteMaterial = mGrayMaterial;
        sprite.atlas = cloneAtlas;
        sprite.spriteName = spriteName;
        GameObject.DestroyImmediate(cloneAtlas.gameObject);
    }
    /// <summary>
    /// 设置UI2DSprite黑白
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="path"></param>
    /// <param name="name"></param>
    public static void Set2DSpriteGray(UI2DSprite sprite, string path, string name)
    {
        if (sprite == null) {
            return;
        }
        Set2DSprite(sprite, path, name);
        Material mGrayMaterial = new Material(Shader.Find("Custom/GrayShader"));
        mGrayMaterial.mainTexture = sprite.mainTexture;
        sprite.material = mGrayMaterial;
        
    }
    public static void Change2DSpriteGray(UI2DSprite sprite)
    {
        if (sprite == null) {
            return;
        }
        Material mGrayMaterial = new Material(Shader.Find("Custom/GrayShader"));
        mGrayMaterial.mainTexture = sprite.mainTexture;
        sprite.material = mGrayMaterial;
    }
    /// <summary>
    /// 设置UI2DSprite黑白(需要裁切的,注意目前黑白shader不支持多层裁切)
    /// </summary>
    public static void Set2DSpriteGraySV(UI2DSprite sprite, string path, string name)
    {
        if (sprite == null) {
            return;
        }
        Set2DSprite(sprite, path, name);
        Material mGrayMaterial = new Material(Shader.Find("Unlit/Transparent Gray"));
        mGrayMaterial.mainTexture = sprite.mainTexture;
        sprite.material = mGrayMaterial;
    }
    
    /// <summary>
    /// 设置UI2DSprite图片为黑白（）
    /// </summary>
    public static void SetSpriteGray(UI2DSprite sprite)
    {
        if (sprite == null) {
            return;
        }
        Material mGrayMaterial = new Material(Shader.Find("Unlit/Transparent Gray"));
        mGrayMaterial.mainTexture = sprite.mainTexture;
        sprite.material = mGrayMaterial;
    }
    
    /// <summary>
    /// 设置GameObject上的Sprite颜色
    /// </summary>
    public static void SetSpriteColor(GameObject go, Color c)
    {
        if (go) {
            UISprite s = go.GetComponent<UISprite>();
            SetSpriteColor(s, c);
        }
    }
    /// <summary>
    /// 设置Sprite颜色
    /// </summary>
    public static void SetSpriteColor(UISprite sprite, Color c)
    {
        if (sprite) {
            sprite.color = c;
        }
    }
    public static void Set2dSpriteColor(UI2DSprite sprite, Color c)
    {
        if (sprite) {
            sprite.color = c;
        }
    }
    
    
    /// <summary>
    /// 设置炮弹兵或者陷阱星级.
    /// </summary>
    public static void SetStarLevelNum(UISprite[] SprStars, int starLevel, bool isHD = false)
    {
        if (SprStars == null) {
            return;
        }
        for (int i = 0; i < SprStars.Length; i++) {
            if (i < starLevel) {
                NGUIUtil.SetSprite(SprStars[i], isHD ? "icon048" : "icon032");
            } else {
                NGUIUtil.SetSprite(SprStars[i], isHD ? "icon049" : "icon033");
            }
        }
    }
    /// <summary>
    /// 设置炮弹兵不显示灰色星.
    /// </summary>
    /// <param name="SprStars">Spr stars.</param>
    /// <param name="starLevel">Star level.</param>
    public static void SetStarHidden(UISprite[] SprStars, int starLevel)
    {
        for (int i = 0; i < SprStars.Length; i++) {
            if (i < starLevel) {
                SprStars[i].gameObject.SetActive(true);
            } else {
                SprStars[i].gameObject.SetActive(false);
            }
            
        }
    }
    /// <summary>
    /// 设置角色的类型（力量、敏捷、智力）
    /// </summary>
    /// <param name="type">0 按力量，1：按敏捷，2 按智力</param>
    public static void SetRoleType(UISprite sprRoleType, int roleType)
    {
        if (sprRoleType == null) {
            return;
        }
        
        switch (roleType) {
            case 0 :
                NGUIUtil.SetSprite(sprRoleType, "icon001");
                break;
                
            case 1:
                NGUIUtil.SetSprite(sprRoleType, "icon002");
                break;
                
            case 2:
                NGUIUtil.SetSprite(sprRoleType, "icon003");
                break;
        }
    }
    /// <summary>
    /// 设定UIPanel的裁剪方式
    /// </summary>
    public static void SetPanelClipping(UIPanel panel, UIDrawCall.Clipping clip)
    {
        panel.clipping = clip;
    }
    
    
    
    /// <summary>
    /// 设置建筑房间类型图标. 临时放置，资源管理代码
    /// </summary>
    /// <param name="spr">Spr.</param>
    /// <param name="RoomType">Room type.</param>
    public static void SetTrapTypeIcon(UISprite spr, int RoomKind)
    {
        if (spr == null) {
            return;
        }
        
        AttributeType type = SkillM.GetBuildAttributeType(RoomKind);
        string sprName = "";
        switch (type) {
            case AttributeType.Fire:
                sprName = "icon004";
                break;
            case AttributeType.Water:
                sprName = "icon007";
                break;
            case AttributeType.Electric:
                sprName = "icon006";
                break;
            case AttributeType.Poison:
                sprName = "icon005";
                break;
            case AttributeType.NONE:
                sprName = "icon047";
                break;
            case AttributeType.Gas:
                sprName = "icon008";
                break;
            case AttributeType.Physical:
                sprName = "icon009";
                break;
        }
        NGUIUtil.SetSprite(spr, sprName);
    }
    /// <summary>
    /// 临时放置，资源管理代码
    /// </summary>
    public static string GetTrapTypeName(int RoomKind)
    {
        AttributeType type = SkillM.GetBuildAttributeType(RoomKind);
        int name = 0;
        switch (type) {
            case AttributeType.Fire:
                name =  10000063;
                break;
            case AttributeType.Water:
                name = 10000064;
                break;
            case AttributeType.Electric:
                name = 10000065;
                break;
            case AttributeType.Poison:
                name = 10000066;
                break;
            case AttributeType.NONE:
                name = 10000062;
                break;
            case AttributeType.Gas:
                name = 10000067;
                break;
            case AttributeType.Physical:
                name = 10000164;
                break;
        }
        return GetStringByKey(name) + GetStringByKey(10000129);
    }
    public static void SetActive(GameObject go, bool IsActive)
    {
        if (go) {
            go.SetActive(IsActive);
        }
    }
    
    public static void SetSpriteFillAmount(UISprite sprite, float p)
    {
        if (sprite) {
            sprite.fillAmount = p;
        }
    }
    
    public static void SetSpriteFillAmount(UI2DSprite sprite, float p)
    {
        if (sprite) {
            sprite.fillAmount = p;
        }
    }
    
    public static void SetSpriteAlpha(GameObject go, float alpha)
    {
        if (go) {
            UISprite sprite = go.GetComponent<UISprite>();
            if (sprite) {
                sprite.alpha = alpha;
            }
        }
    }
    
    /// <summary>
    /// 设置UILable文字
    /// </summary>
    public static void SetLableText<T>(UILabel lbl, T value)
    {
        if (value == null) {
            return;
        }
        if (lbl) {
            lbl.text = GetNewLineStr(value.ToString());
        }
    }
    /// <summary>
    /// 设置UILable文字
    /// </summary>
    public static void SetLableTextByKey<T>(UILabel lbl, T value)
    {
        if (lbl) {
            lbl.text = GetStringByKey<T>(value);
        }
    }
    /// <summary>
    /// 设置UILable颜色
    /// </summary>
    public static void SetLableColor(UILabel lbl, int r, int g, int b)
    {
        Color c = ColorUtils.FromArgb(255, r, g, b);
        lbl.color = c;
    }
    /// <summary>
    /// 设置UILabel 渐变颜色
    /// </summary>
    public static void SetLableGradientColor(UILabel lbl, Color top, Color bottom)
    {
        lbl.applyGradient = true;
        lbl.gradientTop = top;
        lbl.gradientBottom = bottom;
    }
    
    /// <summary>
    /// 添加UILable文字
    /// </summary>
    public static void AddLableText<T>(UILabel lbl, T value)
    {
        SetLableText(lbl, lbl.text + value.ToString());
    }
    
    public static void AddLabelItem(string text, GameObject parent)
    {
        GameObject go = NDLoad.LoadWndItem("HeroIntroduceItem", parent.transform);
        if (go == null) {
            DebugLog("HeroIntroduceItem not found!!!");
            return;
        }
        UILabel label = go.GetComponent<UILabel>();
        if (label) {
            label.text = text;
        }
    }
    
    public static  void AddLabelItem(string text, string value, GameObject parent)
    {
        AddLabelItem(string.Format(text, value), parent);
    }
    
    /// <summary>
    /// 设置按钮颜色
    /// </summary>
    /*public static void SetButtonColor(UIButton btn,Color32 color)
    {
        if (btn)
        {
            btn.GetComponent<UISprite>().color = color;
        }
    }*/
    /// <summary>
    /// 设置按钮贴图
    /// </summary>
    /// <param name="btn"></param>
    /// <param name="name">贴图名称（置于同一图集）</param>
    public static void SetButtonSprite(UIButton btn, string name)
    {
        if (btn) {
            UISprite sprite = btn.GetComponent<UISprite>();
            Vector2 size = Vector2.zero;
            if (sprite) {
                size.x = sprite.width;
                size.y = sprite.height;
            }
            btn.normalSprite = name;
            sprite.width = (int)size.x;
            sprite.height = (int)size.y;
        }
    }
    /// <summary>
    /// 设置按钮子节点sprite的颜色
    /// </summary>
    public static void SetButtonChildColor(UIButton btn, Color color)
    {
        if (btn) {
            Transform child = btn.transform.GetChild(0);
            if (child) {
                UISprite spr = child.GetComponent<UISprite>();
                if (spr) {
                    spr.color = color;
                }
            }
        }
    }
    /// <summary>
    /// 设置按钮灰色表现
    /// </summary>
    public static void SetButtonGray(UIButton btn)
    {
        if (btn) {
            btn.isEnabled = false;
        }
    }
    
    /// <summary>
    /// 设置按钮贴图
    /// </summary>
    /// <param name="btn"></param>
    /// <param name="name">贴图名称（置于同一图集）</param>
    public static void SetButtonSpriteDepth(UIButton btn, int depth)
    {
        if (btn) {
            UISprite s = btn.GetComponent<UISprite>();
            if (s) {
                s.depth = depth;
            }
        }
    }
    
    /// <summary>
    /// 将item项的depth设置为scrollview的
    /// </summary>
    public static void SetItemPanelDepth(GameObject go, UIPanel parent)
    {
        if (go && parent) {
            UIPanel p = go.GetComponent<UIPanel>();
            p.depth = parent.depth;
        }
    }
    
    public static void SetItemPanelDepth(GameObject go, GameObject target)
    {
        if (target) {
            UIPanel panel = target.GetComponent<UIPanel>();
            SetItemPanelDepth(go, panel);
        }
    }
    
    /// <summary>
    /// 重新排列 Table
    /// </summary>
    public static void RepositionTable(UITable table)
    {
        if (table != null) {
            table.Reposition();
            table.repositionNow = true;
        }
    }
    /// <summary>
    /// 下一帧重新排列 Table
    /// </summary>
    public static void RepositionTable(GameObject go)
    {
        if (go == null) {
            return;
        }
        UITable table = go.GetComponent<UITable>();
        if (table != null) {
            //table.enabled = true;
            table.Reposition();
            table.repositionNow = true;
        }
    }
    
    /// <summary>
    /// 重新排列 Table
    /// </summary>
    public static void RepositionTablePivot(GameObject go)
    {
        if (go == null) {
            return;
        }
        UITablePivot table = go.GetComponent<UITablePivot>();
        if (table != null) {
            //table.enabled = true;
            //table.Reposition();
            table.repositionNow = true;
        }
    }
    /// <summary>
    /// 设定Table列数
    /// </summary>
    public static void SetTableColumnsNum(GameObject go, int num)
    {
        if (go == null) {
            return;
        }
        UITable table = go.GetComponent<UITable>();
        if (table != null) {
            table.columns = num;
        }
    }
    
    public static string GetStringByKey<T>(T sKey)
    {
        return Localization.Get(sKey.ToString());
    }
    /// <summary>
    /// 显示提示信息窗口
    /// </summary>
    /// <param name="text">要显示的文本</param>
    /// <param name="BgType">背景类型，0是默认，2是暗底</param>
    /// <param name="duration">自动消失时间间隔</param>
    public static void ShowFreeSizeTipWnd(string text, CallBack callBack = null, int SprType = 0, float duration = 1.0f, int iDep = 0)
    {
        UnityEngine.Debug.Log("xxxxxxxx");
        FreeSizeTipWnd t = WndManager.GetDialog<FreeSizeTipWnd>();
        t.SprType = SprType;
        t.FinishCallBack = callBack;
        if (iDep != 0) {
            t.iDep = iDep;
        } else {
            t.iDep = ConstantData.iDepBefore3DModel;
        }
        //t.gameObject.transform.localPosition = U3DUtil.SetZ(t.gameObject.transform.localPosition, TopMostDepthZ);
        t.ShowDuration = duration;
        NGUIUtil.SetLableText(t.MyHead.LblTitle, text);
    }
    /// <summary>
    /// 显示提示信息窗口(使用多语言key)
    /// </summary>
    public static void ShowFreeSizeTipWnd(int textKey, CallBack callBack = null, int SprType = 0, float duration = 1.0f, int iDep = 0)
    {
        UnityEngine.Debug.Log("xxxxxxxx");
        string text = GetStringByKey(textKey);
        ShowFreeSizeTipWnd(text, callBack, SprType, duration, iDep);
    }
    
    /// <summary>
    /// 显示提示信息窗口
    /// </summary>
    /// <param name="text">要显示的文本</param>
    /// <param name="duration">自动消失时间间隔</param>
    public static void ShowTipWnd(string text, float duration = 1.0f, int iDep = 0)
    {
        UnityEngine.Debug.Log("xxxxxxxx");
        FreeSizeTipWnd t = WndManager.GetDialog<FreeSizeTipWnd>();
        if (iDep != 0) {
            t.iDep = iDep;
        } else {
            t.iDep = ConstantData.iDepBefore3DModel;
        }
        //t.gameObject.transform.localPosition = U3DUtil.SetZ(t.gameObject.transform.localPosition, TopMostDepthZ);
        t.ShowDuration = duration;
        NGUIUtil.SetLableText(t.MyHead.LblTitle, text);
    }
    /// <summary>
    /// 显示提示信息窗口
    /// </summary>
    /// <param name="text">要显示的文本对应的KeyCode</param>
    /// <param name="duration">自动消失时间间隔</param>
    public static void ShowTipWndByKey<T>(T skey, float duration = 1.0f, int iDep = 0)
    {
        UnityEngine.Debug.Log("xxxxxxxx");
        if (!Localization.Exists(skey.ToString())) {
            NGUIUtil.DebugLog(skey.ToString() + "使用不存在的keyCode!");
            return;
        }
        
        FreeSizeTipWnd t = WndManager.GetDialog<FreeSizeTipWnd>();
        if (iDep != 0) {
            t.iDep = iDep;
        }
        t.ShowDuration = duration;
        // t.gameObject.transform.localPosition =U3DUtil.SetZ(t.gameObject.transform.localPosition, TopMostDepthZ);//保证窗口置顶
        string text = Localization.Get(skey.ToString());
        NGUIUtil.SetLableText(t.MyHead.LblTitle, text);
    }
    /// <summary>
    /// 获取多语言配置文本
    /// </summary>
    public static string GetLocalizationStr<T>(T skey)
    {
        if (!Localization.Exists(skey.ToString())) {
            NGUIUtil.DebugLog(skey.ToString() + "使用不存在的keyCode!");
            return "";
        }
        return Localization.Get(skey.ToString());
    }
    
    /// <summary>
    /// 输出带有颜色的文本日志
    /// http://docs.unity3d.com/Manual/StyledText.html
    /// </summary>
    /// <param name="text"></param>
    /// <param name="color">red|green|yellow|blue等</param>
    public static void DebugLog(string text, string color)
    {
#if UNITY_EDITOR_LOG
        Debug.Log(string.Format("<color={0}>{1}</color>", color, text));
#endif
    }
    /// <summary>
    /// 输出红色日志便捷方法
    /// </summary>
    /// <param name="text"></param>
    public static void DebugLog<T>(T text)
    {
#if UNITY_EDITOR_LOG
        DebugLog(text.ToString(), "red");
#endif
    }
    
    public static void UpdateSpriteValue(UISprite sprite, float toValue, float time)
    {

    }
    /// <summary>
    /// panel 卷轴效果
    /// </summary>
    /// <param name="panel">遮罩Panel（SoftClip）</param>
    /// <param name="toValue">Clipping Size x value</param>
    /// <param name="time">持续时长</param>
    public static void UpdatePanelValue(UIPanel panel, float toValue, float time = 1f)
    {

    }
    
    public static void UpdateNPCPanelValue(UIPanel panel, float toValue, NpcDirection direction = NpcDirection.Left)
    {
        float from = panel.clipOffset.x;
        if (direction == NpcDirection.Right) {
            from = -from;
        }

    }
    
    public static void UpdateExpFromValue(UISprite sprite, float toValue, float time = 1.0f, float delay = 0)
    {
        if (sprite == null) {
            return;
        }
        if (sprite.fillAmount == 1.0f && toValue == 1.0f) {
        } else if (sprite.fillAmount > toValue) {
        }
        
        else {

        }
    }
    
    public static void UpdateFromValue(UISprite sprite, float toValue, float time = 1.0f, float delay = 0)
    {
        if (sprite == null) {
            return;
        }
        if (sprite.fillAmount > toValue) {
        }
        
        else {
        }
    }
    
    
    /// <summary>
    /// 渐进修改sprite的高度
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="time"></param>
    /// <param name="delay"></param>
    public static void TweenSpriteHeight(UISprite sprite, int from, int to, float time = 1.0f, float delay = 0)
    {
        if (sprite == null) {
            return;
        }
    }
    /// <summary>
    /// 渐进修改游戏对象的Y值
    /// </summary>
    /// <param name="go">执行动画的对象</param>
    /// <param name="from">初始值</param>
    /// <param name="to">终止值</param>
    /// <param name="duration">持续时长</param>
    /// <param name="delay">延时</param>
    public static void TweenGameObjectPosY(GameObject go, float from, float to, float duration = 1.0f, float delay = 0)
    {
        if (go == null) {
            return;
        }
    }
    /// <summary>
    /// 渐进修改游戏对象的Y值（支持动画回调）
    /// </summary>
    /// <param name="go">执行动画的对象</param>
    /// <param name="from">初始值</param>
    /// <param name="to">终止值</param>
    /// <param name="duration">持续时长</param>
    /// <param name="delay">延时</param>
    /// <param name="callbackGo">执行回调的游戏对象</param>
    /// <param name="callbackMethod">回调方法名称</param>
    public static void TweenGameObjectPosY(GameObject go, float from, float to, float duration, float delay, GameObject callbackGo, string callbackMethod)
    {
        if (go == null) {
            return;
        }
    }
    
    public static void TweenGameObjectPosX(GameObject go, float to, float duration, GameObject callbackGo, string callbackMethod)
    {
        if (go == null) {
            return;
        }
    }
    
    public static void TweenGameObjectPosX(GameObject go, float to, float duration)
    {
        if (go == null) {
            return;
        }
    }
    /// <summary>
    /// 反复上下移动
    /// </summary>
    public static void TweenGameObjectPosYPingPong(GameObject go, float from, float to, float duration = 1.0f, float delay = 0)
    {
        if (go == null) {
            return;
        }
    }
    /// <summary>
    /// 返回从静态表需要换行的字符串，便于UILabel中换行识别
    /// </summary>
    public static string GetNewLineStr(string str)
    {
        return str.Replace("\\n", System.Environment.NewLine);
    }
    
    public static string GetHexColor(string colorName)
    {
        string result = "[FFFFFF]";
        if (colorName == "white") {
            result = "[FFFFFF]";
        } else if (colorName == "green") {
            result = "[c5ff71]";
        } else if (colorName == "blue") {
            result = "[71fff0]";
        } else if (colorName == "purple") {
            result = "[e982ff]";
        } else if (colorName == "lime") {
            result = "[fff882]";
        }
        
        return result;
    }
    
    
    /// <summary>
    /// 获取 大阶颜色设定
    /// </summary>
    /// <param name="bigQuality"></param>
    public static string GetBigQualityColor(int bigQuality)
    {
        string color = "";
        switch (bigQuality) {
            case 1:
                color = NGUIUtil.GetHexColor("white");
                break;
                
            case 2:
                color = NGUIUtil.GetHexColor("green");
                break;
                
            case 3:
                color = NGUIUtil.GetHexColor("blue");
                break;
                
            case 4:
                color = NGUIUtil.GetHexColor("purple");
                break;
                
            case 5:
                color = NGUIUtil.GetHexColor("lime");
                break;
                
            default:
                color = NGUIUtil.GetHexColor("white");
                break;
        }
        return color;
    }
    
    public static string GetBigQualityName(string strName, int Quality)
    {
        int bigLevel = ConfigM.GetBigQuality(Quality);
        
        string color = NGUIUtil.GetBigQualityColor(bigLevel);
        
        return color + strName + "[-]";
    }
    
    public static string GetSmallQualityStr(int Quality)
    {
        int bigLevel = ConfigM.GetBigQuality(Quality);
        int smallLevel = ConfigM.GetSmallQuality(Quality);
        
        string color = NGUIUtil.GetBigQualityColor(bigLevel);
        string smallQuality = "";
        
        if (smallLevel > 0) {
            smallQuality = color + "+" + smallLevel.ToString() + "[-]";
        }
        
        return smallQuality;
        
    }
    
    /// <summary>
    /// 暂停游戏，方便调试
    /// </summary>
    public static void PauseGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExecuteMenuItem("Edit/Pause");
#endif
    }
    
    
    
    /// <summary>
    /// 设置3D UI 的位置
    /// </summary>
    /// <param name="go"></param>
    /// <param name="tPos">世界坐标</param>
    /// <param name="localPos">go的worldPosition</param>
    public static Vector3 Set3DUIPos(GameObject go, Vector3 tPos)
    {
        if (go == null) {
            return Vector3.zero;
        }
        
        Camera gameCamera = Camera.main;
        Camera uiCamera = NGUITools.FindCameraForLayer(go.layer);
        if (gameCamera == null || uiCamera == null) {
            return Vector3.zero;
        }
        Vector3 pos = gameCamera.WorldToViewportPoint(tPos);
        go.transform.position = uiCamera.ViewportToWorldPoint(pos);
        pos = go.transform.localPosition;
        pos.x = Mathf.FloorToInt(pos.x);
        pos.y = Mathf.FloorToInt(pos.y);
        pos.z = 0f;
        go.transform.localPosition = pos;
        return pos;
    }
    
    /// <summary>
    /// 实现伤害数字向上漂浮消失的表现
    /// </summary>
    public static void AddHudTextShow(GameObject go, HUDText hud, string showText, Color c, float delay = 0)
    {
        GameObjectActionExcute gae = GameObjectActionExcute.CreateExcute(go);
        GameObjectActionWait wait = new GameObjectActionWait(delay);
        wait.Data1 = hud;
        wait.Data2 = showText;
        wait.Data3 = c;
        wait.m_complete = ShowHudText;
        gae.AddAction(wait);
    }
    static void ShowHudText(object o)
    {
        GameObject go = o as GameObject;
        if (go == null) {
            return;
        }
        GameObjectActionExcute gae = go.GetComponent<GameObjectActionExcute>();
        if (gae) {
            GameObjectActionWait wait = gae.GetCurrentAction() as GameObjectActionWait;
            if (wait != null) {
                HUDText hud = wait.Data1 as HUDText;
                Color c = (Color)wait.Data3;
                if (hud != null) {
                    hud.Add(wait.Data2.ToString(), c, 0f);
                }
            }
        }
    }
    
    public static void ExcuteWaitAction(GameObject go, float waitDuration, Complete callBack)
    {
        if (go == null) {
            return;
        }
        GameObjectActionWait gaw = new GameObjectActionWait(waitDuration);
        gaw.m_complete = callBack;
        GameObjectActionExcute gae = GameObjectActionExcute.CreateExcute(go);
        gae.AddAction(gaw);
    }
    /// <summary>
    /// 让主菜单置于某窗口前
    /// </summary>
    public static void SetMainMenuTop(WndBase wnd1)
    {
        MainMenuWnd wnd = WndManager.FindDialog<MainMenuWnd>();
        if (wnd) {
            wnd.CloseMenu();
            WndManager.SetBeforeWnd(wnd, wnd1);
        }
    }
    
    
    public static void SetMoneyNumType(int money, UILabel label)
    {
        if (money > 1000000) {
            float coin = money / 1000000.0f;
            if (coin - (int)coin > 0) {
                label.text = coin.ToString("#0.00") + "M";
            } else {
                label.text = coin.ToString("0") + "M";
            }
            
        } else if (money > 1000) {
            float coin = money / 1000.0f;
            if (coin - (int)coin > 0) {
                label.text = coin.ToString("#0.00") + "K";
            } else {
                label.text = coin.ToString("0") + "K";
            }
        } else {
            label.text = money.ToString();
        }
    }
    
}
