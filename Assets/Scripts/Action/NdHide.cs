using UnityEngine;
using System.Collections;
/// <summary>
/// 定时隐藏游戏对象
/// </summary>
public class NdHide : MonoBehaviour {
    public float m_Duration = 0.01f;

    //void Start()
    //{

    //}

    // Update is called once per frame
    void Update()
    {
        m_Duration -= Time.deltaTime;
        if (m_Duration <= 0)
        {
            gameObject.SetActive(false);
            //Debug.Log(m_Duration + "update hide");
        }
    }

    public void ResetDuration(float Duration)
    {
        //Debug.Log("ResetDuration");
        //if (m_Duration < Duration)
            m_Duration = Duration;
        gameObject.SetActive(true);
    }

}
