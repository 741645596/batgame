using UnityEngine;
using System.Collections;
using System.Reflection;

public class NavTest : MonoBehaviour {

	public Transform target;
	private UnityEngine.AI.NavMeshAgent navMeshAgent;
	void Start()	
	{
		navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();	
		//gameObject.AddComponent("FsmIo");


	}
	
	void Update()
	{
		
		//if(Input.GetKeyDown(KeyCode.F))
		
		//{
		
		//    navMeshAgent.destination = target.position;
		
		//}
		
		navMeshAgent.destination =target.position;
	}
}


