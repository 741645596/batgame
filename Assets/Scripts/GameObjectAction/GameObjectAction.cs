using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void Complete(object o);

public class GameObjectAction
{
    public const float MAXDURTION = 99999f;
    public float m_TimeCount = 0;
    public float m_Duration = 0;
    public float m_Delay = 0;
    public GameObject m_target = null;
    public GameObject m_OnCompleteTarget = null;
    public string m_OnComplete = null;
    public object m_OnCompleteParam = null;
    public bool m_bStart = false;
    
    public Complete m_complete;
    
    public delegate void CompleteSound(string soundName);
    public CompleteSound m_completeSound;
    
    
    public virtual void Start()
    {
        m_bStart = true;
        if (m_Duration <= 0) {
            Finish();
        }
        
    }
    public void ReStart()
    {
        m_bStart = false;
        m_TimeCount = 0;
    }
    // false 表示结束，true 表示还没结束
    public bool Update(float deltatime)
    {
        if (m_Delay > 0) {
            m_Delay -= deltatime;
            return true;
        } else {
            if (!m_bStart) {
                Start();
            }
            if (m_TimeCount < m_Duration) {
                m_TimeCount += deltatime;
                ActionUpdate(deltatime);
                if (m_TimeCount >= m_Duration) {
                    Finish();
                    return false;
                }
                return true;
            }
        }
        return false;
    }
    public virtual void ActionUpdate(float deltatime)
    {
    }
    public virtual void Finish()
    {
        if (m_OnComplete != null) {
            if (m_OnCompleteTarget != null) {
                m_OnCompleteTarget.SendMessage(m_OnComplete, m_OnCompleteParam, SendMessageOptions.DontRequireReceiver);
            } else {
                m_target.SendMessage(m_OnComplete, m_OnCompleteParam, SendMessageOptions.DontRequireReceiver);
            }
        }
        if (m_complete != null) {
            m_complete(m_target);
        }
    }
    public float GetLeftTime()
    {
        return m_Delay + m_Duration - m_TimeCount;
    }
    public virtual void SetTarget(GameObject target)
    {
        m_target = target;
        //Start();
    }
}
/// <summary>
/// 队列执行
/// </summary>
public class GameObjectActionQueue : GameObjectAction
{
    public List<GameObjectAction> m_list = new List<GameObjectAction>();
    public void AddAction(GameObjectAction action)
    {
        if (action != null) {
            m_list.Add(action);
            m_Duration += action.GetLeftTime();
            /*if (m_target != null)
            {
                action.SetTarget(m_target);
            }*/
        }
    }
    
    public override void ActionUpdate(float deltatime)
    {
        if (m_list.Count > 0) {
            if (m_list[0].m_target != null) {
                m_list[0].SetTarget(m_target);
            }
            if (!m_list[0].Update(Time.deltaTime)) {
                m_list.RemoveAt(0);
            }
        }
    }
    public override void SetTarget(GameObject target)
    {
        base.SetTarget(target);
        /*foreach(GameObjectAction ga in m_list)
        {
            ga.SetTarget(target);
        }*/
    }
}
/// <summary>
/// 并发
/// </summary>
public class GameObjectActionSpawn : GameObjectAction
{
    public List<GameObjectAction> m_list = new List<GameObjectAction>();
    public void AddAction(GameObjectAction action)
    {
        if (action != null) {
            m_list.Add(action);
            if (GetLeftTime() < action.GetLeftTime()) {
                m_Duration += action.GetLeftTime() - GetLeftTime();
            }
            if (m_target != null) {
                action.SetTarget(m_target);
            }
        }
    }
    
    public override void ActionUpdate(float deltatime)
    {
        foreach (GameObjectAction ga in m_list) {
            ga.Update(deltatime);
        }
    }
    public override void SetTarget(GameObject target)
    {
        base.SetTarget(target);
        foreach (GameObjectAction ga in m_list) {
            ga.SetTarget(target);
        }
    }
}

public class GameObjectActionRepeat : GameObjectAction
{
    public List<GameObjectAction> m_list = new List<GameObjectAction>();
    public List<GameObjectAction> m_Donelist = new List<GameObjectAction>();
    int count = 0;
    int maxcount = 1;
    public GameObjectActionRepeat()
    {
        m_Duration = int.MaxValue;
    }
    public void AddAction(GameObjectAction action)
    {
        if (action != null) {
            m_list.Add(action);
            if (m_target != null) {
                action.SetTarget(m_target);
            }
        }
    }
    //设置循环次数
    public void SetTotalCount(int count)
    {
        maxcount = count;
    }
    
    public override void ActionUpdate(float deltatime)
    {
        if (count < maxcount) {
            if (m_list.Count > 0) {
                if (!m_list[0].Update(deltatime)) {
                    m_Donelist.Add(m_list[0]);
                    m_list.RemoveAt(0);
                }
            } else {
                count ++;
                for (int i = 0; i < m_Donelist.Count; i++) {
                    m_Donelist[i].ReStart();
                    m_list.Add(m_Donelist[i]);
                }
                m_Donelist.Clear();
                
            }
        } else {
            m_Duration = m_TimeCount;
        }
    }
    public override void SetTarget(GameObject target)
    {
        base.SetTarget(target);
        foreach (GameObjectAction ga in m_list) {
            ga.SetTarget(target);
        }
    }
}

public class GameObjectActionDelayDestory : GameObjectAction
{
    public GameObjectActionDelayDestory(float duration)
    {
        m_Duration = duration;
    }
    public override void Finish()
    {
        GameObject.Destroy(m_target);
        base.Finish();
    }
    /// <summary>
    /// 重置时间
    /// </summary>
    public void ResetDuration(float Duration)
    {
        if ((m_Duration - m_TimeCount) < Duration) {
            m_Duration = Duration;
            m_TimeCount = 0;
        }
    }
}
public class GameObjectActionCameraEffect : GameObjectAction
{
    float m_distant;
    public GameObjectActionCameraEffect(float duration, float distant)
    {
        m_Duration = duration;
        m_distant = distant;
    }
    public override void ActionUpdate(float deltatime)
    {
        base.ActionUpdate(deltatime);
        if (Camera.main != null) {
            Vector3 pos = Camera.main.transform.position;
            pos.z += m_distant;
            m_target.transform.position = pos;
        }
    }
    public override void Finish()
    {
        GameObject.Destroy(m_target);
        base.Finish();
    }
    /// <summary>
    /// 重置时间
    /// </summary>
    public void ResetDuration(float Duration)
    {
        if (m_Duration < Duration) {
            m_Duration = Duration;
        }
    }
}
public class GameObjectActionCreateEffect : GameObjectAction
{
    float m_effectduartion;
    string m_effectname;
    Transform m_parent;
    Vector3 m_pos;
    Complete m_completeDo;
    
    CompleteSound m_completeDoSound;
    
    public GameObjectActionCreateEffect(float delay, float effectduartion, string effectname, Vector3 pos, Transform parent, Complete completeDo = null)
    {
        m_Delay = delay;
        m_effectname = effectname;
        m_effectduartion = effectduartion;
        m_parent = parent;
        m_pos = pos;
        m_completeDo = completeDo;
    }
    
    public override void Start()
    {
        base.Start();
        GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", m_effectname, m_pos, m_parent);
        GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(m_effectduartion);
        gae.AddAction(ndEffect);
        
        if (m_completeDo != null) {
            ndEffect.m_complete = m_completeDo;
        }
        
        
    }
}

public class GameObjectActionPlayEffectSound : GameObjectAction
{
    float m_effectduartion;
    string m_effectSoundName;
    Transform m_parent;
    Vector3 m_pos;
    Complete m_completeDo;
    public GameObjectActionPlayEffectSound(float delay, float effectduartion, string effectSoundName,  Complete completeDo = null)
    {
        m_Delay = delay;
        m_effectSoundName = effectSoundName;
        m_effectduartion = effectduartion;
        //		m_parent = parent;
        //		m_pos = pos;
        m_completeDo = completeDo;
    }
    
    public override void Start()
    {
        base.Start();
        SoundPlay.Play(m_effectSoundName, false, false);
        
    }
}

public class GameObjectActionEffectInit : GameObjectAction
{

    public EffectDepth m_depth = EffectDepth.Depth1;
    public WalkDir m_dir = WalkDir.WALKLEFT;
    public Vector3 m_rotate = Vector3.zero;
    public GameObjectActionEffectInit()
    {
    }
    public override void Start()
    {
        base.Start();
        if (m_depth == EffectDepth.Front) {
            Vector3 pos = m_target.transform.position;
            pos.z = -1.2f;
            m_target.transform.position = pos;
        }
        
        if (m_dir == WalkDir.WALKRIGHT) {
            U3DUtil.SetMirror(m_target, -1, 1, 1);
        }
        if (m_rotate != Vector3.zero) {
            m_target.transform.Rotate(m_rotate, Space.Self);
        }
    }
    /// <summary>
    /// 设置深度
    /// </summary>
    public void SetEffectDepth(EffectDepth depth)
    {
        m_depth = depth;
    }
    
    /// <summary>
    /// 设置镜像
    /// </summary>
    public void SetEffectMirror(WalkDir dir)
    {
        m_dir = dir;
    }
    
    
    /// <summary>
    /// 设置特效旋转角度
    /// </summary>
    public void SetRotation(Vector3 v)
    {
        m_rotate = v;
    }
}

public class GameObjectActionWait : GameObjectAction
{
    public object Data1 {
        get;
        set;
    }
    public object Data2 {
        get;
        set;
    }
    
    public object Data3 {
        get;
        set;
    }
    public GameObjectActionWait(float duration)
    {
        m_Duration = duration;
    }
    
    public GameObjectActionWait(float duration, Complete completeDo)
    {
        m_Duration = duration;
        m_complete = completeDo;
    }
    
    public override void Finish()
    {
        base.Finish();
    }
    
}


public class GameObjectActionBoxWait : GameObjectAction
{

    public GameObjectActionBoxWait(float duration)
    {
        m_Duration = duration;
    }
    
    
    public override void Start()
    {
        base.Start();
        
        GameObject box = U3DUtil.FindChild(m_target, "box");
        if (box != null) {
            box.SetActive(true);
        }
        
        GameObject Item = U3DUtil.FindChild(m_target, "Item");
        if (Item != null) {
            Item.SetActive(false);
        }
    }
    
    
    public override void Finish()
    {
        base.Finish();
    }
    
}

public class GameObjectActionOpenBox : GameObjectAction
{
    private Vector3 m_StartScale = new Vector3(0.1f, 0.1f, 1.0f);
    private Vector3 m_EndScale = new Vector3(2.4f, 2.4f, 1.0f);
    private Vector3 m_EndPos = new Vector3(-0.6f, 0.4f, 0.0f);
    private StageClickType m_itemtype;
    private Transform m_itemTransform = null;
    private float m_sclacTime = 0.5f ;
    private	int m_itemID;
    
    public GameObjectActionOpenBox(float duration, sdata.s_itemtypeInfo item)
    {
        m_Duration = duration;
        if (item != null) {
            if (item.gtype == 0) {
                m_itemtype = StageClickType.Item;
            } else if (item.gtype == 1) {
                m_itemtype = StageClickType.Role;
            } else if (item.gtype == 2) {
                m_itemtype = StageClickType.Room;
            } else if (item.gtype == 3) {
                m_itemtype = StageClickType.Captain;
            }
            m_itemID = item.icon;
        }
    }
    
    public override void Start()
    {
        base.Start();
        
        GameObject box = U3DUtil.FindChild(m_target, "box");
        if (box != null) {
            box.SetActive(false);
        }
        
        
        GameObject go = GameObjectLoader.LoadPath("effect/prefab/", "2000601", m_target.transform) ;
        if (go != null) {
            GameObjectLoader.SetGameObjectLayer(go, LayerMask.NameToLayer("Resource")) ;
            ParticleSystem s = go.GetComponent<ParticleSystem>();
            if (s != null) {
                s.Play();
            }
        }
        
        m_itemTransform = SetEffect(m_itemtype, m_itemID);
    }
    
    // Update is called once per frame
    public override void ActionUpdate(float deltatime)
    {
        base.ActionUpdate(deltatime);
        if (m_itemTransform != null && m_TimeCount < m_sclacTime) {
            m_itemTransform.localScale = Vector3.Lerp(m_StartScale, m_EndScale, m_TimeCount / m_sclacTime);
        } else {
            GameObject.Destroy(m_target);
        }
    }
    
    
    
    public override void Finish()
    {
        base.Finish();
    }
    
    
    
    Transform SetEffect(StageClickType type, int ID)
    {
        string path = "";
        if (type == StageClickType.Room) {
            path = "Textures/room/";
        } else if (type == StageClickType.Role || type == StageClickType.Captain) {
            path = "Textures/role/";
        } else if (type == StageClickType.Item) {
            path = "Textures/item/";
        }
        GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "DropReward", m_target.transform.position + m_EndPos, BattleEnvironmentM.GetLifeMBornNode(true));
        if (gae != null) {
            gae.transform.localScale = m_StartScale;
            Sprite s = GameObjectLoader.LoadSprite(path, ID.ToString());
            if (s != null) {
                foreach (SpriteRenderer r in gae.gameObject.GetComponentsInChildren<SpriteRenderer>()) {
                    r.sprite = s;
                }
            }
            GameObjectActionWait gaw = new GameObjectActionWait(1.0f);
            gae.AddAction(gaw);
            GameObjectActionResourceFlyToUI gar = new GameObjectActionResourceFlyToUI();
            gar.SetData(EffectCamera.camera, WndManager.GetNGUICamera(), 1, ResourceType.Box);
            gae.AddAction(gar);
            return gae.transform;
        }
        return null;
    }
}



public class GameObjectActionResourceDrop : GameObjectAction
{
    Vector3 m_start;
    Vector3 m_end;
    public GameObjectActionResourceDrop(float duration, Vector3 start, Vector3 end)
    {
        m_Duration = duration;
        m_start = start;
        m_end = end;
    }
    public override void Start()
    {
        base.Start();
        //m_start = m_target.transform.localPosition;
        
        //Debug.Log("GameObjectActionResourceDrop "  + m_start + "," + m_end);
        /*Random.seed = (int)(Time.realtimeSinceStartup  * 10000);
        Debug.Log("random "  + Random.Range(-1.5f,1.5f));
        m_end = m_start + new Vector3(Random.Range(-1.5f,1.5f),0,0);*/
    }
    public override void ActionUpdate(float deltatime)
    {
        base.ActionUpdate(deltatime);
        m_target.transform.position = EffectCamera.GetEffectPos(Vector3.Lerp(m_start, m_end, m_TimeCount / 1));
    }
}

public class GameObjectActionResourceFlyToUI : GameObjectAction
{
    public GameObjectActionResourceFlyToUI()
    {
        m_Duration = 0.4f;
    }
    public Camera m_gameCamera;
    public Camera m_uicamera;
    public ResourceType m_type;
    public int m_count;
    Vector3 m_start;
    Vector3 m_end;
    private float m_fShowEffectCounter = 0.4f;
    // Use this for initialization
    public void SetData(Camera gameCamera, Camera uicamera, int count, ResourceType type)
    {
        m_gameCamera = gameCamera;
        m_uicamera = uicamera;
        m_count = count;
        m_type = type;
    }
    public override void Start()
    {
        base.Start();
        if (m_type == ResourceType.Wood) {
            SoundPlay.Play("battle_wood_pick", false, false);
        } else if (m_type == ResourceType.Gold) {
            SoundPlay.Play("gold", false, false);
        }
        
        //m_fShowEffectCounter = 0.4f;
        Vector3 pos = Vector3.zero;
        CombatInfoWnd Wnd = WndManager.FindDialog<CombatInfoWnd>();
        if (Wnd != null) {
            pos = m_uicamera.WorldToViewportPoint(Wnd.GetResourcePos(m_type).position);
        }
        pos.z = 20;//Mathf.Abs(m_target.transform.position.z - m_gameCamera.transform.position.z) * 0.5f;
        pos = m_gameCamera.ViewportToWorldPoint(pos);
        m_end = m_gameCamera.transform.InverseTransformPoint(pos);
        m_target.transform.parent = m_gameCamera.transform;
        m_start = m_target.transform.localPosition;
        m_target.transform.localScale = Vector3.one;
        EffectCamera.RemoveFollowList(m_target.transform);
    }
    
    // Update is called once per frame
    public override void ActionUpdate(float deltatime)
    {
        base.ActionUpdate(deltatime);
        
        if (m_fShowEffectCounter > 0) {
            m_fShowEffectCounter -= deltatime;
            if (m_fShowEffectCounter <= 0) {
                SetEffect();
                //gameObject.SetActive(false);
                
            }
        }
        m_target.transform.localPosition = Vector3.Lerp(m_start, m_end, m_TimeCount / m_Duration);
    }
    
    
    
    void SetEffect()
    {
        CombatInfoWnd Wnd = WndManager.FindDialog<CombatInfoWnd>();
        if (Wnd != null) {
            Wnd.PlayResourceAni(m_type);
        }
        /* if (m_type == ResourceType.Gold)
         {
             CombatInfoWnd Wnd = WndManager.FindDialog<CombatInfoWnd>();
             if (Wnd != null)
             {
                 Transform parent = Wnd.GetResourcePos(m_type);
                 GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "2000301", parent.position, parent);
                 if (gae != null)
                 {
                     GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(0.5f);
                     gae.AddAction(ndEffect);
                 }
             }
         }
         else if (m_type == ResourceType.Wood)
         {
             CombatInfoWnd Wnd = WndManager.FindDialog<CombatInfoWnd>();
             if (Wnd != null)
             {
                 Transform parent = Wnd.GetResourcePos(m_type);
                 GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "2000311", parent.position, parent);
                 if (gae != null)
                 {
                     GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(0.5f);
                     gae.AddAction(ndEffect);
                 }
             }
        
         }
         else if (m_type == ResourceType.Box)
         {
             CombatInfoWnd Wnd = WndManager.FindDialog<CombatInfoWnd>();
             if (Wnd != null)
             {
                 Transform parent = Wnd.GetResourcePos(m_type);
                 GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "2000481", parent.position, parent);
                 if (gae != null)
                 {
                     GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(0.5f);
                     gae.AddAction(ndEffect);
                 }
             }
        
         }*/
        //分支
        //Destroy();
    }
    
    public override void Finish()
    {
        base.Finish();
        if (m_type == ResourceType.Gold) {
            //SoundPlay.Play("gold", false, false);
            CombatInfoWnd Wnd = WndManager.FindDialog<CombatInfoWnd>();
            if (Wnd != null) {
                Wnd.SetCombatGold(m_count, true);
                Wnd.SetCombatGold(-m_count, false);
            }
        } else if (m_type == ResourceType.Wood) {
            //SoundPlay.Play("battle_wood_pick", false, false);
            CombatInfoWnd Wnd = WndManager.FindDialog<CombatInfoWnd>();
            if (Wnd != null) {
                Wnd.SetCombatWood(m_count, true);
                Wnd.SetCombatWood(-m_count, false);
            }
            
        } else if (m_type == ResourceType.Box) {
            //SoundPlay.Play("battle_wood_pick", false, false);
            CombatInfoWnd Wnd = WndManager.FindDialog<CombatInfoWnd>();
            if (Wnd != null) {
                Wnd.SetCombatItem();
            }
            
        }
        GameObject.Destroy(m_target);
    }
    
}

public class GameObjectActionGraveResourceFlyToUI : GameObjectAction
{
    public GameObjectActionGraveResourceFlyToUI()
    {
        m_Duration = 1f;
    }
    public ResourceType m_type;
    public int m_count;
    Vector3 m_start;
    Vector3 m_end;
    private float m_fShowEffectCounter = 0;
    // Use this for initialization
    public void SetData(Vector3 pos)
    {
        m_end = pos;
    }
    public override void Start()
    {
        base.Start();
        m_fShowEffectCounter = 0.6f;
        m_start = m_target.transform.position;
        
    }
    
    // Update is called once per frame
    public override void ActionUpdate(float deltatime)
    {
        base.ActionUpdate(deltatime);
        
        if (m_fShowEffectCounter > 0) {
            m_fShowEffectCounter -= deltatime;
            if (m_fShowEffectCounter <= 0.2f && m_fShowEffectCounter > (0.2f - deltatime)) {
                //gameObject.SetActive(false);
                
            }
        }
        m_target.transform.position = Vector3.Lerp(m_start, m_end, m_TimeCount / m_Duration);
    }
    
    
    
    public override void Finish()
    {
        base.Finish();
        GameObject.Destroy(m_target);
    }
    
}

public class GameObjectAction3DFlyToUI : GameObjectAction
{
    public Camera m_gameCamera;
    public Camera m_uicamera;
    public Vector3 m_UIPos;
    /// <summary>
    /// 删除前消失的时间
    /// </summary>
    public float PauseDuration = 0f;
    
    Vector3 m_start;
    Vector3 m_end;
    
    public GameObjectAction3DFlyToUI()
    {
        m_Duration = 1f;
    }
    
    public void SetData(Camera gameCamera, Camera uicamera, Vector3 UIPos, bool IsMoveChild = false)
    {
        m_gameCamera = gameCamera;
        m_uicamera = uicamera;
        m_UIPos = UIPos;
    }
    
    public override void Start()
    {
        base.Start();
        Vector3 pos = Vector3.zero;
        pos = m_uicamera.WorldToViewportPoint(m_UIPos);
        float z = Mathf.Abs(m_target.transform.position.z - m_gameCamera.transform.position.z) * 0.5f;
        pos.z = z;
        pos = m_gameCamera.ViewportToWorldPoint(pos);
        m_end = m_gameCamera.transform.InverseTransformPoint(pos);
        
        m_target.transform.parent = m_gameCamera.transform;
        m_start = m_target.transform.localPosition;
        
    }
    
    public override void ActionUpdate(float deltatime)
    {
        base.ActionUpdate(deltatime);
        //if (m_TimeCount> m_Duration-PauseDuration)
        //{
        //    //NGUIUtil.DebugLog("pause position " + Time.time);
        //    //NGUIUtil.PauseGame();
        //    m_target.transform.localPosition = Vector3.Lerp(m_start, m_end,0.5f);
        //    return;
        //}
        m_target.transform.localPosition = Vector3.Lerp(m_start, m_end, m_TimeCount / (m_Duration - PauseDuration));
    }
    
    public override void Finish()
    {
        base.Finish();
        //NGUIUtil.DebugLog("action stop:"+Time.time);
        GameObject.Destroy(m_target);
    }
    
}



public class GameObjectActionPlaySound : GameObjectAction
{
    AudioSource m_audio;
    public GameObjectActionPlaySound(float delay)
    {
        m_Duration = MAXDURTION;
        m_Delay = delay;
    }
    public override void Start()
    {
        base.Start();
        m_audio = m_target.GetComponent<AudioSource>();
        m_audio.Play();
    }
    public override void ActionUpdate(float deltatime)
    {
        base.ActionUpdate(deltatime);
        if (!m_audio.isPlaying) {
            m_Duration = m_TimeCount;
            Finish();
        }
    }
    public override void Finish()
    {
        base.Finish();
    }
    
}

public class GameObjectActionStopSound : GameObjectAction
{
    AudioSource m_audio;
    public GameObjectActionStopSound(float delay)
    {
        m_Duration = MAXDURTION;
        m_Delay = delay;
    }
    public override void Start()
    {
        base.Start();
        m_audio = m_target.GetComponent<AudioSource>();
        m_audio.Stop();
    }
    public override void ActionUpdate(float deltatime)
    {
        base.ActionUpdate(deltatime);
        if (!m_audio.isPlaying) {
            m_Duration = m_TimeCount;
            Finish();
        }
    }
    public override void Finish()
    {
        base.Finish();
    }
    
}


public class GameObjectActionAudioFade : GameObjectAction
{
    AudioSource m_audio;
    float m_from;
    float m_to;
    public GameObjectActionAudioFade(float from, float to, float duration)
    {
        m_Duration = duration;
        m_from = from;
        m_to = to;
    }
    public override void Start()
    {
        base.Start();
        m_audio = m_target.GetComponent<AudioSource>();
        m_audio.Play();
    }
    public override void ActionUpdate(float deltatime)
    {
        base.ActionUpdate(deltatime);
        m_audio.volume = Mathf.Lerp(m_from, m_to, m_TimeCount / m_Duration);
        //	float vol = 1;
        
        
    }
    public override void Finish()
    {
        base.Finish();
        
    }
    
}

public class GameObjectActionSliderValue : GameObjectAction
{
    private Material m_material;
    private float  m_fToValue = 0f; // false 淡出 true 淡入
    private UISlider m_slider;
    private float m_FromValue = 0f;
    
    public GameObjectActionSliderValue(float toValue, float duration)
    {
        m_fToValue = toValue;
        m_Duration = duration;
        
    }
    
    public override void Start()
    {
        base.Start();
        m_slider = m_target.GetComponent<UISlider>();
        if (m_slider != null) {
            m_FromValue = m_slider.value;
        }
    }
    
    public override void ActionUpdate(float deltatime)
    {
        if (m_slider == null) {
            NGUIUtil.DebugLog(m_target.name + " Slider is null ");
            Finish();
            return;
        }
        // base.ActionUpdate(deltatime);
        if (m_TimeCount < m_Duration) {
            m_slider.value = Mathf.Lerp(m_FromValue, m_fToValue, m_TimeCount / m_Duration);
            
        }
    }
    private void SetColor(Color c)
    {
        if (m_material != null) {
            m_material.color = c;
        }
    }
}

public class GameObjectActionColorFade : GameObjectAction
{
    private Material m_material;
    private bool m_isShow = false; // false 淡出 true 淡入
    
    public GameObjectActionColorFade(bool isShow, float duration)
    {
        m_isShow = isShow;
        m_Duration = duration;
    }
    
    public override void Start()
    {
        base.Start();
        if (m_target.GetComponent<Renderer>() != null) {
            m_material = m_target.GetComponent<Renderer>().material;
        }
        
    }
    
    public override void ActionUpdate(float deltatime)
    {
        if (m_material == null) {
            NGUIUtil.DebugLog(m_target.name + " m_material is null ");
            return;
        }
        // base.ActionUpdate(deltatime);
        if (m_TimeCount < m_Duration) {
            Color color = m_material.color;
            float alpha = color.a;
            if (m_isShow) {
                alpha = Mathf.Lerp(0, 1, m_TimeCount / m_Duration);
            } else {
                alpha = Mathf.Lerp(1, 0, m_TimeCount / m_Duration);
            }
            color = new Color(color.r, color.g, color.b, alpha);
            SetColor(color);
        }
    }
    private void SetColor(Color c)
    {
        if (m_material != null) {
            m_material.color = c;
        }
    }
}

public class GameObjectActionShakePostion : GameObjectAction
{
    private Vector3 m_orignpos = Vector3.zero;
    private Vector3 m_amount = Vector3.zero;
    private bool m_isloacl = false;
    private Role role = null;
    public float m_deltatcount ;
    public int m_count = 0;
    public GameObjectActionShakePostion()
    {
        m_amount = new Vector3(0.2f, 0, 0);
        m_Duration = 0.34f;
        m_isloacl = true;
        m_deltatcount = m_Duration / 4;
    }
    public override void Start()
    {
        base.Start();
        if (m_target != null) {
            if (m_isloacl) {
                m_orignpos = m_target.transform.localPosition;
            } else {
                m_orignpos = m_target.transform.position;
            }
            role = m_target.GetComponentInParent<LifeProperty>().GetLife() as Role;
            //role.m_Skin.PlayEffectColor(SkinEffectColor.BeHit,0.16f);
            m_target.GetComponent<Animation>().Play();
        }
    }
    
    public override void ActionUpdate(float deltatime)
    {
        base.ActionUpdate(deltatime);
        
        
        if (m_TimeCount > 0f && (m_TimeCount - deltatime) <= 0f) {
            role.m_Skin.PlayEffectColor(SkinEffectColor.BeHit, 0.11f);
            
        }
        if (m_TimeCount > 0.22f && (m_TimeCount - deltatime) <= 0.22f) {
            role.m_Skin.PlayEffectColor(SkinEffectColor.BeHit, 0.11f);
            
        }
        
    }
    
    public override void Finish()
    {
        base.Finish();
        
        if (m_isloacl) {
            m_target.transform.localPosition = m_orignpos;
        } else {
            m_target.transform.position = m_orignpos;
        }
    }
}

public class GameobjectActionTreasureTipText : GameObjectAction
{
    private bool m_fadeOut = false;
    private bool m_fadeIn = false;
    private float m_fadeTime = 2;
    public GameobjectActionTreasureTipText(int endtime, float fadeTime = 2f)
    {
        m_Duration = endtime;
        m_fadeTime = fadeTime;
    }
    public override void Start()
    {
        base.Start();
    }
    
    public override void ActionUpdate(float deltatime)
    {
        base.ActionUpdate(deltatime);
        float tt = m_Duration - m_TimeCount;
        if (m_TimeCount < 2 && m_fadeIn == false) {
            m_fadeIn = true;
            //前三秒淡入.
            //iTween.FadeFrom(m_target, 0f, 2f);
        } else if (tt < 2 && m_fadeOut == false) {
            m_fadeOut = true;
            //最后三秒淡出.
            /*iTween.FadeTo(m_target, iTween.Hash(
                    "alpha", 0f,
                    "time", 2f,
                    "NamedColorValue", iTween.NamedValueColor._Color,
                    "looptype", iTween.LoopType.none
                ));*/
                
        }
    }
    private void FadePaoPao(bool fadeOut, float time)
    {
        /*iTween.FadeTo(m_target, iTween.Hash(
                "alpha", !fadeOut ? 0 : 1f,
                "time", 2f,
                "NamedColorValue", iTween.NamedValueColor._Color,
                "looptype", iTween.LoopType.none
            ));*/
    }
    public override void Finish()
    {
        base.Finish();
    }
}




public class GameObjectActionBezierMove : GameObjectAction
{
    Bezier bezier;
    Bezier bezierspeed;
    float m_fDelay = 0;
    
    public Vector3 m_start = Vector3.zero;
    public Vector3 m_end = Vector3.zero;
    public GameObjectActionBezierMove(Vector3 start, float endtime, WalkDir dir)
    {
        m_start = start;
        m_Duration = endtime ;
        if (dir == WalkDir.WALKLEFT) {
            m_end = new Vector3(start.x - 100, 0, start.z);
        } else if (dir == WalkDir.WALKRIGHT) {
            m_end = new Vector3(start.x + 100, 0, start.z);
        }
        bezier = new Bezier(start, new Vector3(0, 20, 0), new Vector3(0, 20, 0), m_end);
    }
    public override void Start()
    {
        base.Start();
    }
    
    public override void ActionUpdate(float deltatime)
    {
        base.ActionUpdate(deltatime);
        if (m_TimeCount < m_fDelay) {
            return;
        }
        Vector3 pos = bezier.GetPointAtTime((m_TimeCount - m_fDelay) / (m_Duration - m_fDelay));
        pos.z = -3f;//前置被击飞的
        m_target.transform.position = pos;
    }
    public override void Finish()
    {
        base.Finish();
    }
}

public class GameObjectActionMove: GameObjectAction
{
    public Vector3 m_start = Vector3.zero;
    public Vector3 m_end = Vector3.zero;
    public float m_endtime = 0;
    public int m_starttime = 0;
    
    public GameObjectActionMove(Vector3 start, Vector3 end, float endtime, float delay = 0)
    {
        m_start = start;
        m_end = end;
        m_Duration = endtime ;
        
        m_Delay = delay;
    }
    
    public override void Start()
    {
        base.Start();
    }
    
    public override void ActionUpdate(float deltatime)
    {
        base.ActionUpdate(deltatime);
        m_target.transform.position = Vector3.Lerp(m_start, m_end, m_TimeCount / m_Duration);
    }
    
    public override void Finish()
    {
        base.Finish();
    }
}