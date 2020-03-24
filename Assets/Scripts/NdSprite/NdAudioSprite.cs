using UnityEngine;
using System.Collections.Generic;

public class NdAudioSprite : NdSprite {
	List<AudioSource> m_AudioList = new List<AudioSource>();
	public override void SpriteStart ()
	{
		base.SpriteStart ();
		AudioSource[] audios = GetComponentsInChildren<AudioSource>();
		for(int i = 0; i < audios.Length; i++)
		{
			m_AudioList.Add(audios[i]);
		}

	}
	public override void Play ()
	{
		base.Play ();
		for(int i = 0; i < m_AudioList.Count; i++)
		{
			if (m_AudioList[i])
				m_AudioList[i].Play();
		}

	}
	public override void Puase ()
	{
		base.Puase ();
		
		for(int i = 0; i < m_AudioList.Count; i++)
		{
			if (m_AudioList[i])
				m_AudioList[i].Pause();
		}
	}

}
