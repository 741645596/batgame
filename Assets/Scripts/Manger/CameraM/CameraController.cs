using UnityEngine;
using System.Collections;

public class AreaLimitPyramid
{
	public Vector3 m_vTopL;
	public Vector3 m_vTopR;
	public Vector3 m_vBottomLT;
	public Vector3 m_vBottomLB;
	public Vector3 m_vBottomRT;
	public Vector3 m_vBottomRB;
	public float m_fZDeep;
	public float m_fxLeftDeep;
	public float m_fxRightDeep;
	public float m_fYBTopDeep;
	public float m_fYBottomDeep;
}

public class CameraController : MonoBehaviour {

	protected AreaLimitPyramid m_areaLimitPyramid;
	protected bool m_bEnableZoomBack=false;

	public virtual void Start()
	{
		if(null==gameObject.GetComponent<Camera>())
			Debug.LogError("gameobject is no camera!");
		//m_nextcontrler = null;
		if(null==m_areaLimitPyramid)
			m_areaLimitPyramid = new AreaLimitPyramid();
	}
	
	public virtual void EnterControler()
	{
		Debug.LogError("EnterControler not implement");
	}


	public virtual void LeaveControler()
	{
		Debug.LogError("LeaveControler not implement");
	}




	public void SetCameraLimitParam(AreaLimitPyramid areaLimitPyramid)
	{
		if(null==m_areaLimitPyramid)
			m_areaLimitPyramid = new AreaLimitPyramid();
		m_areaLimitPyramid.m_vTopL = areaLimitPyramid.m_vTopL;
		m_areaLimitPyramid.m_vTopR = areaLimitPyramid.m_vTopR;
		m_areaLimitPyramid.m_vBottomLT = areaLimitPyramid.m_vBottomLT;
		m_areaLimitPyramid.m_vBottomLB = areaLimitPyramid.m_vBottomLB;
		m_areaLimitPyramid.m_vBottomRT = areaLimitPyramid.m_vBottomRT;
		m_areaLimitPyramid.m_vBottomRB = areaLimitPyramid.m_vBottomRB;
		
		m_areaLimitPyramid.m_fZDeep = areaLimitPyramid.m_fZDeep;
		m_areaLimitPyramid.m_fxLeftDeep = areaLimitPyramid.m_fxLeftDeep;
		m_areaLimitPyramid.m_fxRightDeep = areaLimitPyramid.m_fxRightDeep;
		m_areaLimitPyramid.m_fYBTopDeep = areaLimitPyramid.m_fYBTopDeep;
		m_areaLimitPyramid.m_fYBottomDeep = areaLimitPyramid.m_fYBottomDeep;
	}

	public float DistanceToRayPlaneIntersection(Vector3 vRayOrigin, 
		 								Vector3 vRayHeading,
		 								Vector3 vPlanePoint,//any point on the plane
		 								Vector3 vPlaneNormal
	                                    )
	{
		float d = - Vector3.Dot(vPlaneNormal,vPlanePoint);
		float numer = Vector3.Dot(vPlaneNormal,vRayOrigin) + d;
		float denom = Vector3.Dot(vPlaneNormal,vRayHeading);
		// normal is parallel to vector
		if ((denom < 0.000001f) && (denom > -0.000001f))
		{
			return 0.0f;
		}
		//else if(Mathf.Abs(-(numer / denom))<0.000001f)
		//	return 0.0f;
		return -(numer / denom); 
	}
	public  virtual bool limitPosition()
	{
		if(m_areaLimitPyramid==null)
			return false;
		bool bResutl=false;
		float zLocal = GetComponent<Camera>().transform.localPosition.z;                      
		float xLocal = GetComponent<Camera>().transform.localPosition.x;
		float yLocal = GetComponent<Camera>().transform.localPosition.y;
		
		if (zLocal > m_areaLimitPyramid.m_vBottomLT.z+5f) 
		{
			zLocal = m_areaLimitPyramid.m_vBottomLT.z+5f;
			GetComponent<Camera>().transform.localPosition = new Vector3(xLocal, yLocal, zLocal);
			bResutl = true;
			m_bEnableZoomBack = true;
		} 
		else if (zLocal <= m_areaLimitPyramid.m_vBottomLT.z+5f
		         &&zLocal > m_areaLimitPyramid.m_vBottomLT.z) 
		{
			//zLocal = m_areaLimitPyramid.m_vBottomLT.z+5f;
			//camera.transform.localPosition = new Vector3(xLocal, yLocal, zLocal);
			bResutl = true;
			m_bEnableZoomBack = true;
		} 
		else if(zLocal<m_areaLimitPyramid.m_fZDeep)
		{
			zLocal = m_areaLimitPyramid.m_fZDeep+0.1f;
			GetComponent<Camera>().transform.localPosition = new Vector3(xLocal, yLocal, zLocal);
			bResutl = true;
		}

		//leftface
		Vector3 vRayOrigin = GetComponent<Camera>().transform.localPosition;
		Vector3 vRayHeading=new Vector3(GetComponent<Camera>().transform.position.x-1,GetComponent<Camera>().transform.position.y,GetComponent<Camera>().transform.position.z)-GetComponent<Camera>().transform.localPosition;
		Vector3 vPlanePoint=m_areaLimitPyramid.m_vTopL;
		Vector3 vPlaneNormal = Vector3.Cross(m_areaLimitPyramid.m_vTopL - m_areaLimitPyramid.m_vBottomLT,m_areaLimitPyramid.m_vBottomLB - m_areaLimitPyramid.m_vTopL);
		float disance = DistanceToRayPlaneIntersection(vRayOrigin,vRayHeading,vPlanePoint,vPlaneNormal);
		if (disance < 0&&Mathf.Abs (disance)>0.01f&&zLocal>=m_areaLimitPyramid.m_vTopL.z) 
		{
			Vector3 vNewPos = vRayOrigin+disance*vRayHeading.normalized;
			xLocal = vNewPos.x;
			bResutl=true;
		}

		//rightface
		vRayOrigin = GetComponent<Camera>().transform.localPosition;
		vRayHeading=new Vector3(GetComponent<Camera>().transform.position.x+1,GetComponent<Camera>().transform.position.y,GetComponent<Camera>().transform.position.z)-GetComponent<Camera>().transform.localPosition;
		vPlanePoint=m_areaLimitPyramid.m_vTopR;
		vPlaneNormal = Vector3.Cross(m_areaLimitPyramid.m_vTopR - m_areaLimitPyramid.m_vBottomRB,m_areaLimitPyramid.m_vBottomRT - m_areaLimitPyramid.m_vTopR);
		disance = DistanceToRayPlaneIntersection(vRayOrigin,vRayHeading,vPlanePoint,vPlaneNormal);
		if (disance < 0&&Mathf.Abs (disance)>0.01f&&zLocal>=m_areaLimitPyramid.m_vTopR.z) 
		{
			Vector3 vNewPos = vRayOrigin+disance*vRayHeading.normalized;
			xLocal = vNewPos.x;
			bResutl=true;
		}

		//topface
		vRayOrigin = GetComponent<Camera>().transform.localPosition;
		vRayHeading=new Vector3(GetComponent<Camera>().transform.position.x,GetComponent<Camera>().transform.position.y+1,GetComponent<Camera>().transform.position.z)-GetComponent<Camera>().transform.localPosition;
		vPlanePoint=m_areaLimitPyramid.m_vTopL;
		vPlaneNormal = Vector3.Cross(m_areaLimitPyramid.m_vTopL - m_areaLimitPyramid.m_vBottomRT,m_areaLimitPyramid.m_vBottomLT - m_areaLimitPyramid.m_vTopL);
		disance = DistanceToRayPlaneIntersection(vRayOrigin,vRayHeading,vPlanePoint,vPlaneNormal);
		if (disance < 0&&Mathf.Abs (disance)>0.01f&&zLocal>=m_areaLimitPyramid.m_vTopL.z) 
		{
			Vector3 vNewPos = vRayOrigin+disance*vRayHeading.normalized;
			yLocal = vNewPos.y;
			bResutl=true;
		}

		
		//bottomface
		vRayOrigin = GetComponent<Camera>().transform.localPosition;
		vRayHeading=new Vector3(GetComponent<Camera>().transform.position.x,GetComponent<Camera>().transform.position.y-1,GetComponent<Camera>().transform.position.z)-GetComponent<Camera>().transform.localPosition;
		vPlanePoint=m_areaLimitPyramid.m_vTopR;
		vPlaneNormal = Vector3.Cross(m_areaLimitPyramid.m_vTopR - m_areaLimitPyramid.m_vBottomRB,m_areaLimitPyramid.m_vBottomLB - m_areaLimitPyramid.m_vTopR);
		disance = DistanceToRayPlaneIntersection(vRayOrigin,vRayHeading,vPlanePoint,vPlaneNormal);
		if (disance < 0&&Mathf.Abs (disance)>0.01f&&zLocal>=m_areaLimitPyramid.m_vTopR.z) 
		{
			Vector3 vNewPos = vRayOrigin+disance*vRayHeading.normalized;
			yLocal = vNewPos.y;
			bResutl=true;
		}
		
		if(xLocal<m_areaLimitPyramid.m_fxLeftDeep)
		{
			xLocal = m_areaLimitPyramid.m_fxLeftDeep;
			bResutl = true;
		}
		else if(xLocal>m_areaLimitPyramid.m_fxRightDeep)
		{
			xLocal = m_areaLimitPyramid.m_fxRightDeep;
			bResutl = true;
		}
		if(yLocal>m_areaLimitPyramid.m_fYBTopDeep)
		{
			yLocal = m_areaLimitPyramid.m_fYBTopDeep;
			bResutl = true;
		}
		else if(yLocal<m_areaLimitPyramid.m_fYBottomDeep)
		{
			yLocal = m_areaLimitPyramid.m_fYBottomDeep;
			bResutl = true;
		}

		GetComponent<Camera>().transform.localPosition = new Vector3(xLocal, yLocal, zLocal);
	
		return bResutl;
	}
	float CalcCameraXValue (float fValueZ)
	{
		float halfFOV = (GetComponent<Camera>().fieldOfView * 0.5f) * Mathf.Deg2Rad;
		return fValueZ*Mathf.Tan (halfFOV)*GetComponent<Camera>().aspect;
	}
}
