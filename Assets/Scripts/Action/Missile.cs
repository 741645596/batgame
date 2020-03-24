using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour {
	public float m_fCurrentAngle =0f;
	bool m_bFirstPathing;
	public float angle;
    public bool m_bParticleRotate= true;
    ParticleSystem[] ps;
	// Use this for initialization
	void Start () {
		m_bFirstPathing = true;
		m_fCurrentAngle =0f;
        ps = GetComponentsInChildren<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
		//SetAngle(-angle);
	}
	public void SetAngle(float angle)
	{
		angle += 180f;
		if (m_fCurrentAngle != angle)
		{
			//m_fRotatetime += Time.deltaTime;
			float temp = 0;
			if (m_bFirstPathing)
			{
				m_bFirstPathing=false;
				temp =m_fCurrentAngle = angle;
			}
			else
			{
				float fa = angle-m_fCurrentAngle;
				if (fa > 180)
					fa = fa - 360;
				else if(fa < -180)
					fa = 360+ fa;
				temp = fa;//Mathf.Lerp(0,fa,0.1f );
				m_fCurrentAngle += temp;
				m_fCurrentAngle = NdUtil.ClampAngle(m_fCurrentAngle, 0, 360f);
			}
			{
				transform.Rotate(new Vector3(0,0,-temp),Space.Self);
			}
            if (m_bParticleRotate)
            {
                if (ps!=null)
                {
                    foreach (ParticleSystem p in ps)
                    {
                        p.startRotation = angle * Mathf.PI / 180f;
                        //Debug.Log ("p.startRotation:" + p.startRotation + "," + p.gameObject);
                        //p.startRotation = 0.25f;
                    }
                }
            }
		}
	}
}
