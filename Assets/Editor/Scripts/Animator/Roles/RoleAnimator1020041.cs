using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
/// <summary>
/// 熊孩子
/// </summary>
public class RoleAnimator1020041 : Editor
{
    [MenuItem("Tool/AnimatorOver/Roles/1020041-维京熊")]
	
	static void DoCreateAnimationAssets() 
	{
		//创建animationController文件，保存在Assets路径下
        string strRoleID = "1020041";
        string strRolePartName = "1020041";
		string strRoleTemplateContrller="Assets/Resources/AnimaCtrl/Roles/Role@Controller.controller";

		{
				AnimatorOverrideController overrideController = new AnimatorOverrideController ();
				overrideController.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath (strRoleTemplateContrller, typeof(UnityEditor.Animations.AnimatorController)) as UnityEditor.Animations.AnimatorController;
				NewRoleOverFromRoleControler (strRoleID, strRolePartName, ref overrideController);
			AssetDatabase.CreateAsset(overrideController, "Assets/Resources/AnimaCtrl/Roles/1020041@Over.overrideController");
				AssetDatabase.SaveAssets ();
		}
	}

	private static void NewRoleOverFromRoleControler (string strRoleID,string strRolePartName, ref AnimatorOverrideController overrideController)
	{
		//Fly
		AddStateTransition (strRoleID,strRolePartName,"00000","00000","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"00010","00010","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"00100","00100","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"00200","00200","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"00300","00300","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"00400","10500","10501", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"01000","01000","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"01010","01010","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"01100","01100","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"01200","01200","", ref overrideController);
		
		
		AddStateTransition (strRoleID,strRolePartName,"10000","10000","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"10100","10100","", ref overrideController);
		AddStateTransition(strRoleID, strRolePartName, "10200", "10200", "", ref overrideController);
		AddStateTransition(strRoleID, strRolePartName, "10210", "10200", "", ref overrideController);
		AddStateTransition(strRoleID, strRolePartName, "10300", "10300", "", ref overrideController);
		AddStateTransition(strRoleID, strRolePartName, "10310", "10300", "", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"10400","10400","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"10500","10500","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"10501","10500","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"10502","10500","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"10600","10600","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"11000","11000","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"11100","11100","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"18000","18000","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"18100","18100","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"18100","18101","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"18100","18102","", ref overrideController);
		
		AddStateTransition (strRoleID,strRolePartName,"50010","50010","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"50260","50260","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"50170","50170","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"50240","50240","", ref overrideController);
		
		AddStateTransition (strRoleID,strRolePartName,"60220","60220","", ref overrideController);
		
		AddStateTransition (strRoleID,strRolePartName,"70000","70000","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"70300","70300","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"70400","70400","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"70401","70401","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"70410","70410","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"70500","70500","", ref overrideController);
		
		AddStateTransition (strRoleID,strRolePartName,"80000","80000","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"80010","80010","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"80020","80020","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"80030","80030","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"80040","80040","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"81000","81000","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"81010","81010","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"81100","81100","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"82000","82000","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"83000","83000","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"84000","84000","", ref overrideController);
		AddStateTransition (strRoleID,strRolePartName,"85000","85000","", ref overrideController);
	}
	/// <summary>
	/// 添加状态
	/// </summary>
	/// <param name="strRoleID">角色ID</param>
	/// <param name="strRolePartName">角色部件名称</param>
	/// <param name="strClipSourceName">源名称</param>
	/// <param name="strKey">控制器主键</param>
	/// <param name="strClipName">剪辑名称</param>
	/// <param name="overrideController"></param>
	private static void AddStateTransition(string strRoleID,string strRolePartName,string strKey,string strClipSourceName,string strClipName, ref AnimatorOverrideController overrideController)
	{
		bool bPair = false;
		string strCliPath = "Assets/Models/Roles/"+strRoleID+"/"+strRolePartName+"@Ani/"+strRolePartName+"@"+strClipSourceName+".FBX";
		if(strClipName=="")
		 strClipName =strRolePartName+"@"+strKey;
		else
			strClipName =strRolePartName+"@"+strClipName;
		//根据动画文件读取它的AnimationClip对象
		Object []obj = AssetDatabase.LoadAllAssetsAtPath(strCliPath);
		int nCount = obj.Length;
		for (int nCnt=0; nCnt<nCount; nCnt++) 
		{
			if(obj[nCnt]is AnimationClip&& obj[nCnt].name==strClipName)
			{
				overrideController [strKey] = obj[nCnt] as AnimationClip;
				bPair=true;
				break;
			}
		}
		if (!bPair) 
		{
			string strErr = "角色"+strRoleID+"的部件"+strRolePartName+"的动作"+strKey+"找不到匹配"+strClipName;
			//Debug.LogError(strErr);
            NGUIUtil.DebugLog(strErr,"red");
		}
	}
}

