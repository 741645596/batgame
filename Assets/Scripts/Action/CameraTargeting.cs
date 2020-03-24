using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraTargeting : MonoBehaviour
{
    // Which layers targeting ray must hit (-1 = everything)
    public LayerMask targetingLayerMask = -1;

    // Targeting ray length
    private float targetingRayLength = Mathf.Infinity;

    // Camera component reference
    private Camera cam;

    // 
    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    // 
    void Update()
    {
        TargetingRaycast();
    }

    // 
    public void TargetingRaycast()
    {
        // Current target object transform component
        Transform targetTransform = null;

        // If camera component is available
        if (cam != null)
        {
            RaycastHit hitInfo;

            // Create a ray from mouse coords
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            // Targeting raycast
            if (Physics.Raycast(ray, out hitInfo, targetingRayLength, targetingLayerMask.value))
            {
                // Cache what we've hit
                targetTransform = hitInfo.collider.transform;
            }
        }

       
    }
}
