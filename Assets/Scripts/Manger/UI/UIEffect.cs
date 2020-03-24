using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/**
    * This class is just a place holder for UI special effect to set a correct renderQueue.
    */
public class UIEffect : UIWidget
{
    Material mPlaceHolderMaterial = null;
    Rect mRect = new Rect(0f, 0f, 1f, 1f);
	Transform mCachedTransform;
	Material[] mTrueMaterials;

    public override Material material
    {
        get
        {
            return mPlaceHolderMaterial;
        }
    }

    protected override void Awake()
    {
        base.Awake();

		mCachedTransform = transform;
        // Any shader is fine, we just need a shader to create a material on the fly.
        Shader shader = Shader.Find("Unlit/Transparent Colored");
        mPlaceHolderMaterial = new Material(shader);

		// Real materials

		NGUITools.SetLayer(gameObject, LayerMask.NameToLayer("NGUICamera"));
		Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>(true);
		List<Material> mats = new List<Material>();
		for (int i = 0; i < renderers.Length; ++i)
		{
			Material mat = Application.isPlaying ? renderers[i].material : renderers[i].sharedMaterial;
			if (mat != null)
			{
				mats.Add(mat);
			}
		}
		mTrueMaterials = mats.ToArray();


        // Avoid the scale being changed in UIWidget.OnInit() and UIWidget.OnValidate.
        // Actually width and height is useless since special effects have their own meshes.
        width = minWidth + 2;
        height = minHeight + 2;

        //DisableParticleSystemEx();
    }

	private void EnableParticleSystemEx()
	{
		gameObject.SetActive(true);
	}

    // UI special effects should not be optimized with camera culling.
    private void DisableParticleSystemEx()
	{
		gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        RemoveFromPanel();  // UIWidget.OnDestroy()

        if (Application.isPlaying)
        {
            Destroy(mPlaceHolderMaterial);
			if (mTrueMaterials != null)
			{
				for (int i = 0; i < mTrueMaterials.Length; ++i)
				{
					Destroy(mTrueMaterials[i]);
				}
			}     
        }
    }

    protected override void OnUpdate ()
    {
        base.OnUpdate();

		if (drawCall != null && mTrueMaterials != null)
		{
			int expectedRenderQueue = drawCall.finalRenderQueue;
			for (int i = 0; i < mTrueMaterials.Length; ++i)
			{
				Material mat = mTrueMaterials[i];
				if (mat.renderQueue != expectedRenderQueue)
				{
					mat.renderQueue = expectedRenderQueue;
				}
			}
		}
    }

	public void Play()
	{
		gameObject.SetActive(false);
		gameObject.SetActive(true);
	}

	// Widgets need to have vertices to participate in draw call or render queue computing.
	public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Vector4 v = drawingDimensions;

		verts.Add(new Vector3(v.x, v.y));
		verts.Add(new Vector3(v.x, v.w));
		verts.Add(new Vector3(v.z, v.w));
		verts.Add(new Vector3(v.z, v.y));

		uvs.Add(new Vector2(mRect.xMin, mRect.yMin));
		uvs.Add(new Vector2(mRect.xMin, mRect.yMax));
		uvs.Add(new Vector2(mRect.xMax, mRect.yMax));
		uvs.Add(new Vector2(mRect.xMax, mRect.yMin));

		cols.Add(mColor);
		cols.Add(mColor);
		cols.Add(mColor);
		cols.Add(mColor);
	}

#if UNITY_EDITOR
    // Suppress the gizmos of UIWidget because UISpecialEffect is usually attached to an sfx GameObject whose
    // localScale is very large. There will be a huge rectangle in the scene view if the UIWidget gizmos is shown.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.01f);
    }
#endif
}

