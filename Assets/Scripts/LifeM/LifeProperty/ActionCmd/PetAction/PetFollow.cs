﻿using UnityEngine;
using System.Collections;

public class PetFollow : MonoBehaviour
{
    public float xMargin = 0f;		// Distance in the x axis the player can move before the camera follows.
    public float yMargin = 0f;		// Distance in the y axis the player can move before the camera follows.
    public float xSmooth = 3f;		// How smoothly the camera catches up with it's target movement in the x axis.
    public float ySmooth = 3f;		// How smoothly the camera catches up with it's target movement in the y axis.
    public Vector2 maxXAndY = new Vector2(2,0);		// The maximum x and y coordinates the camera can have.
    public Vector2 minXAndY = new Vector2(-2,0);		// The minimum x and y coordinates the camera can have.

    public bool m_IsFollow = false;

    private Transform player = null;		// Reference to the player's transform.

    public void SetFollowTarget(Transform t)
    {
        if (t!=null)
        {
            player = t;
        }
    }

    bool CheckXMargin()
    {
        // Returns true if the distance between the camera and the player in the x axis is greater than the x margin.
        return Mathf.Abs(transform.position.x - player.position.x) > xMargin;
    }


    bool CheckYMargin()
    {
        // Returns true if the distance between the camera and the player in the y axis is greater than the y margin.
        return Mathf.Abs(transform.position.y - player.position.y) > yMargin;
    }


    void FixedUpdate()
    {
        if (player != null && m_IsFollow)
        {
            TrackPlayer();
        }
    }


    void TrackPlayer()
    {
        // By default the target x and y coordinates of the camera are it's current x and y coordinates.
        float targetX = transform.position.x;
        float targetY = transform.position.y;
        float targetZ = transform.position.z;

        // If the player has moved beyond the x margin...
        if (CheckXMargin())
            // ... the target x coordinate should be a Lerp between the camera's current x position and the player's current x position.
            targetX = Mathf.Lerp(transform.position.x, player.position.x, xSmooth * Time.deltaTime);

        // If the player has moved beyond the y margin...
        if (CheckYMargin())
            // ... the target y coordinate should be a Lerp between the camera's current y position and the player's current y position.
            targetY = Mathf.Lerp(transform.position.y, player.position.y, ySmooth * Time.deltaTime);

        // The target x and y coordinates should not be larger than the maximum or smaller than the minimum.
       /* targetX = Mathf.Clamp(targetX, minXAndY.x, maxXAndY.x);
        targetY = Mathf.Clamp(targetY, minXAndY.y, maxXAndY.y);
        */
        // Set the camera's position to the target position with the same z component.
        transform.position = new Vector3(targetX, targetY, player.position.z);
    }
}

