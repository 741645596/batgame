using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

public class BuildAnimator1603 : Editor{
	/*[MenuItem("Tool/AnimatorOver/builds/1603--牢笼")]
	static void DoCreateAnimationAssets() 
	{
		//创建animationController文件，保存在Assets路径下
        string strBuildID = "1603";
		string strBuildPartName = "1603";
		CreateAnimatorController(strBuildID,strBuildPartName);
	}
	
	public static UnityEditor.Animations.AnimatorController CreateAnimatorController(string strBuildID,string strBuildPartName)
	{
		string strBuildControlerPath="Assets/Models/Buildings/Animator/"+strBuildID+"@Controller.controller";
		UnityEditor.Animations.AnimatorController animatorController = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(strBuildControlerPath);
		UnityEditor.Animations.AnimatorControllerLayer layer = animatorController.GetLayer(0);
		animatorController.AddParameter("iState",AnimatorControllerParameterType.Int);
		animatorController.AddParameter("tTriggerParam",AnimatorControllerParameterType.Trigger);
		AddStates(strBuildID,strBuildPartName,ref layer);
		return animatorController;
		
	}
	private static void AddStates (string strBuildID,string strBuildPartName, ref UnityEditor.Animations.AnimatorControllerLayer layer)
	{
		Dictionary<string, UnityEditor.Animations.AnimatorState> dicState=new Dictionary<string,UnityEditor.Animations.AnimatorState>();
		AddState (strBuildID,strBuildPartName,"10000","10000","","Stand", ref layer ,ref dicState);
		AddState (strBuildID,strBuildPartName,"20000","20000","","Hit", ref layer ,ref dicState);
		AddState (strBuildID,strBuildPartName,"20100","20100","","Die", ref layer ,ref dicState);
		AddState (strBuildID,strBuildPartName,"30000","30000","","Trigger", ref layer ,ref dicState);
		AddState (strBuildID,strBuildPartName,"30100","30100","","TriggerLoop", ref layer ,ref dicState);
		UpdateStatePosition (dicState);

		
		AddStateTransition(null, dicState ["10000"], "iState", TransitionConditionMode.Equals, 10000f, ref layer);
		//srcState 10000
		string strCurStateKey = "10000";
		AddStateTransition(dicState [strCurStateKey], dicState ["20000"], "iState", TransitionConditionMode.Equals, 20000f, ref layer);
		AddStateTransition (dicState [strCurStateKey], dicState ["20100"], "iState", TransitionConditionMode.Equals, 20100f, ref layer);
		AddStateTransition(dicState [strCurStateKey], dicState ["30000"], "iState", TransitionConditionMode.Equals, 30000f, ref layer);
		
		//srcState 20000
		strCurStateKey = "20000";
		AddStateTransition (dicState [strCurStateKey], dicState ["10000"], "", TransitionConditionMode.ExitTime, 0f, ref layer);
		
		//srcState 30000
		strCurStateKey = "30000";
		AddStateTransition (dicState [strCurStateKey], dicState ["30100"], "", TransitionConditionMode.ExitTime, 0f, ref layer);

	}

	private static void UpdateStatePosition(Dictionary<string, UnityEditor.Animations.AnimatorState> dicState)
	{
		int nCount = dicState.Count;
		int nCnt = 0;
		float fDelta = 360.0f/(float)nCount;
		foreach (string strkey in dicState.Keys)
		{
			Vector3 vDir = new Vector3(Mathf.Cos(fDelta*nCnt),Mathf.Sin(fDelta*nCnt),0f);
			dicState[strkey].position = 300f*vDir;
			nCnt++;
		}

	}
	/// <summary>
	/// 添加状态
	/// </summary>
	/// <param name="strRoleID">房间ID</param>
	/// <param name="strRolePartName">房间部件名</param>
	/// <param name="strClipSourceName">源名称</param>
	/// <param name="strKey">控制器主键</param>
	/// <param name="strClipName">剪辑名称</param>
	/// <param name="AnimatorControllerLayer"></param>
	private static void AddState(string strBuildID,string strBuildPartName,string strKey,string strClipSourceName,string strClipName,string strStateName, ref UnityEditor.Animations.AnimatorControllerLayer layer,ref Dictionary<string, UnityEditor.Animations.AnimatorState> dicState)
	{
		bool bPair = false;
		string strCliPath = "Assets/Models/Buildings/"+strBuildID+"/"+strBuildPartName+"@"+strClipSourceName+".FBX";
		if(strClipName=="")
		 strClipName =strBuildPartName+"@"+strKey;
		else
			strClipName =strBuildPartName+"@"+strClipName;
		//根据动画文件读取它的AnimationClip对象
		Object []obj = AssetDatabase.LoadAllAssetsAtPath(strCliPath);
		int nCount = obj.Length;
		UnityEditor.Animations.AnimatorStateMachine sm = layer.stateMachine;
		for (int nCnt=0; nCnt<nCount; nCnt++) 
		{
			if(obj[nCnt]is AnimationClip&& obj[nCnt].name==strClipName)
			{
				UnityEditor.Animations.AnimatorState  state = sm.AddState(strStateName+"-"+strKey);
				dicState[strKey]=state;
				state.SetAnimationClip(obj[nCnt] as AnimationClip,layer);
				//state.position = new Vector3(0f,dicState.Count*40f,state.position.z);
				bPair=true;
				break;
			}
		}
		if (!bPair) 
		{
			string strErr = "建筑"+strBuildID+"的部件"+strBuildPartName+"的动作"+strKey+"找不到匹配"+strClipName;
			//Debug.LogError(strErr);
            NGUIUtil.DebugLog(strErr,"red");
		}
	}
	
	private static void AddStateTransition(UnityEditor.Animations.AnimatorState srcState,UnityEditor.Animations.AnimatorState toState,string strparameter,TransitionConditionMode mode,float fthredshold, ref UnityEditor.Animations.AnimatorControllerLayer layer)
	{
		if ( toState == null)
			return;
		UnityEditor.Animations.AnimatorStateMachine sm = layer.stateMachine;
		UnityEditor.Animations.AnimatorTransition trans = null;
		if(srcState == null)
			trans = sm.AddAnyStateTransition(toState);
		else 
				trans = sm.AddTransition(srcState,toState);
		//把默认的时间条件删除
		trans.RemoveCondition(0);
		trans.duration=0.0f;

		AnimatorCondition animCondition =  trans.AddCondition();
		animCondition.parameter = strparameter;//"iState";
		animCondition.mode = mode;//TransitionConditionMode.NotEqual;
		animCondition.threshold = fthredshold;//float.Parse(strKey);
		animCondition.exitTime = fthredshold;

		if (TransitionConditionMode.ExitTime != mode) {
			animCondition =  trans.AddCondition();
			animCondition.parameter = "tTriggerParam";//"iState";
			animCondition.mode = TransitionConditionMode.If;//TransitionConditionMode.NotEqual;
			//animCondition.threshold = fthredshold;//float.Parse(strKey);
			//animCondition.exitTime = fthredshold;

				}

		//trans.dst = 0.0f;
	}*/


}