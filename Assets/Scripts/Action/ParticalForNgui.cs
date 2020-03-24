using UnityEngine;
using System.Collections;

public class ParticalForNgui : MonoBehaviour {
	public float Size = 100;
	public float Scale;
	// Use this for initialization
	void Start () {
		
	}
	
    public void DoScale()
    {
        UIRoot root = transform.GetComponentInParent<UIRoot>();
        //Debug.Log("root" + root);
        if (root != null)
            Scale = 1 / root.transform.localScale.x;

        float Rscale = Size / Scale;
        ParticleSystem[] ps = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem p in ps)
        {
            p.startSize = p.startSize * Rscale;
        }
        foreach (ParticleSystem p in ps)
        {
            p.Play();
        }
        foreach (Transform tran in transform)
        {
            tran.localScale = new Vector3(Size, Size, Size);
        }
    }

	// Update is called once per frame
    /*
	void LateUpdate () {
		float Rscale = Size/Scale;
		foreach (Transform tran in transform)
		{
			tran.localScale = new Vector3(Rscale,Rscale,Rscale);
		}
		ParticleSystem[] ps = GetComponentsInChildren<ParticleSystem>();
		foreach(ParticleSystem p in ps )
		{
			p.startSize = p.startSize * Rscale;
			ParticleSystem.Particle[] particles = new ParticleSystem.Particle[p.particleCount];
  			int count = p.GetParticles(particles);
			
			for(int i = 0; i < count; i++)
			{
				particles[i].size = particles[i].size ;
				Debug.Log(i + "," + particles[i].size);
			}
			
			p.SetParticles(particles, count);
		}
	}
    */
}
