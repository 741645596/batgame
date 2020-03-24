using UnityEngine;
/// <summary>
/// 保证粒子系统优先于NGUI绘制
/// </summary>
/// <remarks>http://www.tasharen.com/forum/index.php?topic=776.msg34546#msg34546</remarks>
public class SetRenderQueue : MonoBehaviour
{
    public int RenderQueue = 5000;

    void Awake()
    {
        SetParticleRender();
        SetMeshRender();
    }

    public void SetRenderBottom()
    {
        RenderQueue = 1;
        SetParticleRender();
        SetMeshRender();
    }

	public void SetParticleRender()
	{
		Renderer ren ;
		ParticleSystem[] sys = GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem p in sys)
		{
			if (p != null)
			{
				ren = p.GetComponent<Renderer>();
				if (ren != null)
				{
					Material mMat = new Material(ren.sharedMaterial);
                    mMat.renderQueue = RenderQueue;
					ren.material = mMat;
				}
			}
		}
	}


	public void SetMeshRender()
	{
		Renderer ren ;
		MeshRenderer[] sys = GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer p in sys)
		{
			if (p != null)
			{
				ren = p.GetComponent<Renderer>();
				if (ren != null)
				{
					Material mMat = new Material(ren.sharedMaterial);
                    mMat.renderQueue = RenderQueue;
					ren.material = mMat;
				}
			}
		}
	}

    public void SetParticleRender(int renderQ)
    {
        Renderer ren;
        ParticleSystem[] sys = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem p in sys)
        {
            if (p != null)
            {
                ren = p.GetComponent<Renderer>();
                if (ren != null)
                {
                    Material mMat = new Material(ren.sharedMaterial);
                    mMat.renderQueue = renderQ;
                    ren.material = mMat;
                }
            }
        }
    }

    public void SetMeshRender(int renderQ)
    {
        Renderer ren;
        MeshRenderer[] sys = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer p in sys)
        {
            if (p != null)
            {
                ren = p.GetComponent<Renderer>();
                if (ren != null)
                {
                    Material mMat = new Material(ren.sharedMaterial);
                    mMat.renderQueue = renderQ;
                    ren.material = mMat;
                }
            }
        }
    }

}