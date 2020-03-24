using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 音乐播放控件
/// </summary>
public class SoundPlay : MonoBehaviour
{	
	/*private static SoundPlay instance;
	
	// 当前准备播放的音乐列表
	private static List<string> list = new List<string>();
	private static List<WaitMuisc> Waitlist = new List<WaitMuisc>();
		
	// 是否禁止播放音乐
	private static bool denySound = false;
	
	// 最大音量
	private const float SOUND_MAX = 0.7f;
	
	void Awake()
	{
		instance = this;
		DontDestroyOnLoad(gameObject);
	}


	void Update()
	{
		if(Waitlist.Count == 0)
			return ;
		for(int i = 0; i < Waitlist.Count; i ++)
		{
			if(Waitlist[i].CheckPlay(Time.deltaTime) == true)
			{
				Play(Waitlist[i].MuiscName, false, false );
				Waitlist.RemoveAt(i);
				i = i -1;
			}
		}
	}
	
	/// <summary>
	/// 开启或禁止播放音乐
	/// </summary>
	public static bool Enable
	{
		get { return ! denySound; }
		set
		{
			denySound = !value;
		
			if (! value)
			{
				foreach (Transform t in instance.transform)
					GameObject.Destroy(t.gameObject);
			}
			
			PlayerPrefs.SetInt("deny_sound", denySound ? 1 : 0);
		}
	}
	
	/// <summary>
	/// 播放音乐
	/// </summary>
	public static void Play(string soundFile, bool bLoop, bool bFade )
	{		

		AudioClip clip = AudioLoader.LoadAudio(soundFile);
		if (clip == null)
		{
			App.log.Print("音乐文件{0}不存在", soundFile);
			return;
		}
		Play(clip, bLoop, bFade );
	}
	
	public static void Play(AudioClip ac, bool bLoop, bool bFade )
	{		
		if (denySound)
			return;
		
		// 开始播放
		GameObject o = new GameObject(ac.name);
		o.transform.parent = instance.transform;
        AudioSource source = o.AddComponent<AudioSource>();
		source.clip = ac;
        source.volume = SOUND_MAX;
        source.minDistance = 1000f;
        source.maxDistance = 1000f;
        source.time = 0f;
		source.loop = bLoop;
		source.Play();
		
		if (bFade)
			// 淡入
			Coroutine.DispatchService(instance.FadeIn(source), instance.gameObject, null);
		
		// 等待播放结束
		if (! bLoop)
			Coroutine.DispatchService(instance.WaitEnd(source), instance.gameObject, null);
	}
	
	/// <summary>
	/// 停止音乐播放
	/// </summary>
	public static void Stop(string soundFile, bool bFade)
	{
		if (string.IsNullOrEmpty(soundFile))
			return;
		
		// 从待播放列表中摘除
		list.Remove(soundFile);
		
		// 便利所有的子对象，干掉
		if (instance == null)
			return;
		foreach (Transform t in instance.transform)
		{
			if (t.name != soundFile)
				continue;
			
			// 符合条件，直接干掉
			if (! bFade) 
				GameObject.Destroy(t.gameObject);
			else
			{
				// 需要淡出
				Coroutine.DispatchService(instance.FadeOut(t.GetComponent<AudioSource>()), instance.gameObject, null);
			}
		}
	}
	public static void Stop()
	{
		list.Clear();
		
		// 便利所有的子对象，干掉
		if (instance == null)
			return;
		foreach (Transform t in instance.transform)
		{
			instance.StartCoroutine(instance.FadeOut(t.GetComponent<AudioSource>()));
		}
	}
	public static void StopByType(string type, bool bFade)
	{

	}

	/// <summary>
	/// 加入等待播放队列
	/// </summary>
	public static void JoinPlayQueue(string  soundFile,float  delay)
	{
		Waitlist.Add(new WaitMuisc(soundFile, delay));
	}
	


    
    public static void EnableAudioLister(bool bEnable)
    {
        if (instance == null)
            return;
        foreach (Transform t in instance.transform)
        {
            AudioListener audioListener= t.GetComponent<AudioListener>();
            if (audioListener)
            {
                audioListener.enabled = bEnable;
            }
        }
    }
	// 等待音乐播放完毕
	IEnumerator WaitEnd(AudioSource source)
	{
		while (source != null && source.isPlaying)
			yield return null;
		
		if (source != null)
			GameObject.Destroy(source.gameObject);
	}
	
	// 淡出音乐
	IEnumerator FadeOut(AudioSource source)
	{
		// 0.5s播放完毕
		float speed = SOUND_MAX/0.5f;
		
		float vol = 1;
		while (vol > 0)
		{
			vol -= Time.deltaTime*speed;
			if (vol < 0f) vol = 0f;
			
			if (source == null)
				yield break;
			source.volume = vol;
			yield return null;
		}
		
		// 析构掉
		if (source != null)
			GameObject.Destroy(source.gameObject);
	}
	
	// 淡入音乐
	IEnumerator FadeIn(AudioSource source)
	{
		// 0.5s播放完毕
		float speed = SOUND_MAX/0.5f;
		
		float vol = 0;
		while (vol < SOUND_MAX)
		{
			vol += Time.deltaTime*speed;
			if (vol > SOUND_MAX) vol = SOUND_MAX;
			
			if (source == null)
				yield break;
			source.volume = vol;
			yield return null;
		}
	}*/
	private static SoundPlay instance;
	private static List<GameObjectActionExcute> GaeList = new List<GameObjectActionExcute>();
	private static List<GameObjectActionExcute> GaeBackGroundList = new List<GameObjectActionExcute>();
	// 是否禁止播放音乐
	private static bool denySound = false;
	private static bool denyBackGroundSound = false;
	// 最大音量
	private const float SOUND_MAX = 0.7f;
	private static string backGroundSoundName;

	void Awake()
	{
		instance = this;
		DontDestroyOnLoad(gameObject);
	}

	
	/// <summary>
	/// 开启或禁止播放音乐
	/// </summary>
	/*public static bool Enable
	{
		get { return ! denySound; }
		set
		{
			denySound = !value;
			
			if (! value)
			{
				foreach (Transform t in instance.transform)
					GameObject.Destroy(t.gameObject);
			}
			
			PlayerPrefs.SetInt("deny_sound", denySound ? 1 : 0);
		}
	}*/

	/// <summary>
	/// 播放背景音乐
	/// </summary>
	public static void PlayBackGroundSound(string soundFile, bool bLoop, bool bFade )
	{	
		if(denyBackGroundSound)
		{
			return;
		}
		if (string.IsNullOrEmpty(backGroundSoundName)) 
		{
			backGroundSoundName = soundFile;
		}
		else if (backGroundSoundName.Equals(soundFile)) 
		{
			return;
		}
		else
		{
			StopImmediately(backGroundSoundName);
			backGroundSoundName = soundFile;
		}

		AudioClip clip = AudioLoader.LoadAudio(soundFile);
		if (clip == null)
		{
			App.log.Print("音乐文件{0}不存在", soundFile);
			return;
		}
		Play(clip, bLoop, bFade,true);
	}

	/// <summary>
	/// 停止背景音乐
	/// </summary>
	public static void StopBackGroundSound()
	{
		if (string.IsNullOrEmpty (backGroundSoundName))
		{
			Stop();
		}
		else
		{
			StopImmediately(backGroundSoundName);
			backGroundSoundName = null;
		}
	}
	/// <summary>
	/// 播放音乐
	/// </summary>
	public static bool Play(string soundFile, bool bLoop, bool bFade )
	{		
		AudioClip clip = AudioLoader.LoadAudio(soundFile);
		if (clip == null)
		{
			App.log.Print("音乐文件{0}不存在", soundFile);
			return false;
		}
		Play(clip, bLoop, bFade );
		return true;
	}
	/// <summary>
	/// 延时播放音乐
	/// </summary>
	public static void Play(string soundFile, bool bLoop, bool bFade,float delay )
	{
		AudioClip clip = AudioLoader.LoadAudio(soundFile);
		if (clip == null)
		{
			App.log.Print("音乐文件{0}不存在", soundFile);
			return;
		}
		PlayByDelay(clip, bLoop, bFade,delay );
	}

	public static void PlayByDelay(AudioClip ac, bool bLoop, bool bFade ,float delay = 0f)
	{		
		if (denySound)
			return;
		
		// 开始播放
		if(instance == null)
		{
			return;
		}
		GameObject o = new GameObject(ac.name);
		o.transform.parent = instance.transform;
		AudioSource source = o.AddComponent<AudioSource>();
		source.clip = ac;
		source.volume = SOUND_MAX;
		source.minDistance = 1000f;
		source.maxDistance = 1000f;
		source.time = 0f;
		source.loop = bLoop;
//		source.Play();
		o.AddComponent<NdAudioSprite>();
		GameObjectActionExcute gae = o.AddComponent<GameObjectActionExcute>();
		GaeList.Add(gae);
		
		if (bFade)
		{
			// 淡入
			GameObjectActionAudioFade gaf = new GameObjectActionAudioFade(0,SOUND_MAX,0.5f);
			gae.AddAction(gaf);
		}
		GameObjectActionPlaySound gap = new GameObjectActionPlaySound(delay);
		gae.AddAction(gap);
		// 等待播放结束
		if (! bLoop)
		{
			GameObjectActionDelayDestory gad = new GameObjectActionDelayDestory(delay);
			gad.m_complete = AudioDestroyCallBack;
			//Coroutine.DispatchService(instance.WaitEnd(source), instance.gameObject, null);*/
			gae.AddAction(gad);
		}
	}

	/// <summary>
	/// Play the specified ac, bLoop, bFade, bBackGround and delay.
	/// </summary>
	/// <param name="ac">Ac.</param>
	/// <param name="bLoop">If set to <c>true</c> b loop.</param>
	/// <param name="bFade">If set to <c>true</c> b fade.</param>
	/// <param name="bBackGround">是否是背景音乐.</param>
	/// <param name="delay">Delay.</param>
	public static void Play(AudioClip ac, bool bLoop, bool bFade ,bool bBackGround =false,float delay = 0f)
	{		
		if (denySound)
			return;
		
		// 开始播放
		if(instance == null)
		{
			return;
		}
		GameObject o = new GameObject(ac.name);
		o.transform.parent = instance.transform;
		AudioSource source = o.AddComponent<AudioSource>();
		source.clip = ac;
		source.volume = SOUND_MAX;
		source.minDistance = 1000f;
		source.maxDistance = 1000f;
		source.time = 0f;
		source.loop = bLoop;
		source.Play();
		o.AddComponent<NdAudioSprite>();
		GameObjectActionExcute gae = o.AddComponent<GameObjectActionExcute>();
		GaeList.Add(gae);
		
		if (bFade)
		{
			// 淡入
			GameObjectActionAudioFade gaf = new GameObjectActionAudioFade(0,SOUND_MAX,0.5f);
			gae.AddAction(gaf);
		}
		if(!bBackGround)
		{
			GameObjectActionPlaySound gap = new GameObjectActionPlaySound(0f);
			gae.AddAction(gap);
		}
		// 等待播放结束
		if (! bLoop)
		{
			GameObjectActionDelayDestory gad = new GameObjectActionDelayDestory(delay);
			gad.m_complete = AudioDestroyCallBack;
			//Coroutine.DispatchService(instance.WaitEnd(source), instance.gameObject, null);*/
			gae.AddAction(gad);
		}
	}
	public static void AudioDestroyCallBack(object g)
	{
		for(int i = 0; i < GaeList.Count; i++)
		{

 			if (GaeList[i].gameObject == g)
			{
				//Debug.Log(i + "," + GaeList.Count + "," + GaeList[i].gameObject);
				GaeList.RemoveAt(i);
				break;
			}
		}
	}
	/// <summary>
	/// 停止音乐播放
	/// </summary>
	public static void Stop(string soundFile, bool bFade)
	{
		if (string.IsNullOrEmpty(soundFile))
			return;
		
		for(int i = 0; i < GaeList.Count; i ++)
		{
			if (GaeList[i]!= null && GaeList[i].gameObject.name == soundFile)
			{
				GaeList[i].Stop();
				if (bFade)
				{
					GameObjectActionAudioFade gaf = new GameObjectActionAudioFade(SOUND_MAX,0,0.5f);
					GaeList[i].AddAction(gaf);
				}
				
				GameObjectActionDelayDestory gad = new GameObjectActionDelayDestory(0f);
				gad.m_complete = AudioDestroyCallBack;
				GaeList[i].AddAction(gad);
			}
		}
		// 从待播放列表中摘除
		//list.Remove(soundFile);
		
		// 便利所有的子对象，干掉
		
		/*if (instance == null)
			return;
		foreach (Transform t in instance.transform)
		{
			if (t.name != soundFile)
				continue;
			
			// 符合条件，直接干掉
			if (! bFade) 
				GameObject.Destroy(t.gameObject);
			else
			{
				// 需要淡出
				Coroutine.DispatchService(instance.FadeOut(t.GetComponent<AudioSource>()), instance.gameObject, null);
			}
		}*/
	}
	public static void Stop()
	{
		//list.Clear();
		
		// 便利所有的子对象，干掉
		/*if (instance == null)
			return;
		foreach (Transform t in instance.transform)
		{
			instance.StartCoroutine(instance.FadeOut(t.GetComponent<AudioSource>()));
		}*/
		for(int i = 0; i < GaeList.Count; i ++)
		{

			if (GaeList[i]!= null && GaeList[i].gameObject.name != backGroundSoundName)
			{
				Debug.Log(i + "," + GaeList.Count + "," +GaeList[i].gameObject);
				GaeList[i].Stop();
				
				GameObjectActionAudioFade gaf = new GameObjectActionAudioFade(SOUND_MAX,0,0.5f);
				//if(GaeList.Count <= i)
				//	Debug.Log("Stop:元素被毁掉删除掉了");
				GaeList[i].AddAction(gaf);
				
				GameObjectActionDelayDestory gad = new GameObjectActionDelayDestory(0f);
				gad.m_complete = AudioDestroyCallBack;
				GaeList[i].AddAction(gad);
			}
		}
	}
	/// <summary>
	/// Stop this instance Immediately .
	/// </summary>
	public static void StopImmediately(string soundFile)
	{
		//list.Clear();
		
		// 便利所有的子对象，干掉
		/*if (instance == null)
			return;
		foreach (Transform t in instance.transform)
		{
			instance.StartCoroutine(instance.FadeOut(t.GetComponent<AudioSource>()));
		}*/
		for(int i = 0; i < GaeList.Count; i ++)
		{
			
			if (GaeList[i]!= null && GaeList[i].gameObject.name ==  soundFile)
			{
				//Debug.Log(i + "," + GaeList.Count + "," +GaeList[i].gameObject);
				GaeList[i].Stop();
				GaeList[i].gameObject.GetComponent<AudioSource>().Stop();

//				GameObjectActionAudioFade gaf = new GameObjectActionAudioFade(SOUND_MAX,0,0.5f);
//				//if(GaeList.Count <= i)
//				//	Debug.Log("Stop:元素被毁掉删除掉了");
//				GaeList[i].AddAction(gaf);
				
				GameObjectActionDelayDestory gad = new GameObjectActionDelayDestory(0f);
				gad.m_complete = AudioDestroyCallBack;
				GaeList[i].AddAction(gad);
			}
		}
	}

	/// <summary>
	/// 加入等待播放队列
	/// </summary>
	public static void JoinPlayQueue(string  soundFile,float  delay)
	{
		//Waitlist.Add(new WaitMuisc(soundFile, delay));
		AudioClip clip = AudioLoader.LoadAudio(soundFile);
		if (clip == null)
		{
			App.log.Print("音乐文件{0}不存在", soundFile);
			return;
		}
		Play(clip, false, true,false,delay );
	}
	

}

/*public class WaitMuisc{ 
	
	public string  MuiscName ;       
	public float   WaitTime;
	
	public WaitMuisc(string MuiscName,float WaitTime )
	{
		this.MuiscName = MuiscName;
		this.WaitTime = WaitTime;
	}

	public bool CheckPlay(float Deletime)
	{
		WaitTime -= Deletime;
		if(WaitTime <= 0.0f)
		{
			return true;
		}
		return false;
	}
};*/

