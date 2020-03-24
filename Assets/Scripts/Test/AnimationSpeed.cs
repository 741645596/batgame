using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 控制播放速度
/// <From> </From>
/// <Author>QFord</Author>
/// </summary>
public class AnimationSpeed : MonoBehaviour {

    public float Speed = 1.0f;

	void Start () 
	{
        Animator animator = GetComponent<Animator>();
        if (animator)
        {
            animator.speed = Speed;
        }
	}
	
	
	
}
