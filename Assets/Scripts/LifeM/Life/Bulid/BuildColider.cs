using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildColider : MonoBehaviour {

	public List<BoxCollider> listBattleColider=new List<BoxCollider>();
	public List<BoxCollider> listEditorColider=new List<BoxCollider>();
	// Use this for initialization
	void Start ()
    
    {
		
	
	}

    public void EnableEditorCollider(bool bEnable)
    {
		int nEditorColiderCount = listEditorColider.Count;
		for(int nCnt=0;nCnt<nEditorColiderCount;nCnt++)
        {
			listEditorColider[nCnt].enabled = bEnable;
        }
    }
	public void EnableBattleCollider(bool bEnable)
	{
		int nBattleColiderCount = listBattleColider.Count;
		for(int nCnt=0;nCnt<nBattleColiderCount;nCnt++)
		{
			listBattleColider[nCnt].enabled = bEnable;
		}
	}
}
