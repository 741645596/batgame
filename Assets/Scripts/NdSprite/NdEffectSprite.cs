using UnityEngine;
using System.Collections.Generic;

public class NdEffectSprite : NdSprite {
	public List<ParticleSystem> m_pslist = new List<ParticleSystem>();
	public List<Animator> m_anilist = new List<Animator>();
	public override void SpriteStart ()
	{
		base.SpriteStart ();
		/*if (particleSystem != null)
			m_pslist.Add(particleSystem);*/
		ParticleSystem[] childps= GetComponentsInChildren<ParticleSystem>();
		foreach(ParticleSystem ps in childps)
		{
			m_pslist.Add(ps);
		}
		Animator[] childani= GetComponentsInChildren<Animator>();
		foreach(Animator a in childani)
		{
			m_anilist.Add(a);
		}
	}
	public override void Play ()
	{
		base.Play ();
		/*if (m_pslist.Count > 0)
		{
			m_pslist[0].Play(true);
		}*/
		foreach(ParticleSystem p in m_pslist)
		{
			p.playbackSpeed = 1;
		}
		foreach(Animator a in m_anilist)
		{
			a.speed = 1;
		}
	}
	public override void Puase ()
	{
		base.Puase ();
		/*if (m_pslist.Count > 0)
		{
			m_pslist[0].Pause(true);
		}*/
		foreach(ParticleSystem p in m_pslist)
		{
			p.playbackSpeed = 0;
					
		}
		foreach(Animator a in m_anilist)
		{
			a.speed = 0;
		}
	}
	public override void ChangeSpeed ()
	{
		base.ChangeSpeed ();
		float s = (float)Attr.GetValue(AttrKeyName.Speed_Float);
		foreach(ParticleSystem ps in m_pslist)
		{
			ps.playbackSpeed = s;
		}
		foreach(Animator a in m_anilist)
		{
			a.speed = s;
		}
	}

}
