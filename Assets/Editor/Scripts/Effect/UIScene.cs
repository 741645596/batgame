using UnityEngine;
using UnityEditor;


public class UIScene : EditorWindow
{	
	[@MenuItem("BAT Scene/UI 制作")]
	private static void UI()
	{
		if(EditorApplication.currentScene != "Assets/Editor/Scene/UIEffect.unity")
		{
			EditorApplication.SaveScene(EditorApplication.currentScene);
			EditorApplication.OpenScene("Assets/Editor/Scene/UIEffect.unity");
		}
	}
	[@MenuItem("BAT Scene/BAT")]
	private static void BAT()
	{
		if(EditorApplication.currentScene != "Assets/Scenes/Work/BAT.unity")
		{
			EditorApplication.SaveScene(EditorApplication.currentScene);
			EditorApplication.OpenScene("Assets/Scenes/Work/BAT.unity");
		}
	}

	[@MenuItem("BAT Scene/Login")]
	private static void Login()
	{
		if(EditorApplication.currentScene != "Assets/Scenes/Work/Login.unity")
		{
			EditorApplication.SaveScene(EditorApplication.currentScene);
			EditorApplication.OpenScene("Assets/Scenes/Work/Login.unity");
		}
	}

	[@MenuItem("BAT Scene/MainTown")]
	private static void MainTown()
	{
		if(EditorApplication.currentScene != "Assets/Scenes/Work/MainTown.unity")
		{
			EditorApplication.SaveScene(EditorApplication.currentScene);
			EditorApplication.OpenScene("Assets/Scenes/Work/MainTown.unity");
		}
	}


	[@MenuItem("BAT Scene/Combat")]
	private static void Combat()
	{
		if(EditorApplication.currentScene != "Assets/Scenes/Work/Combat.unity")
		{
			EditorApplication.SaveScene(EditorApplication.currentScene);
			EditorApplication.OpenScene("Assets/Scenes/Work/Combat.unity");
		}
	}

	[@MenuItem("BAT Scene/ViewStage")]
	private static void ViewStage()
	{
		if(EditorApplication.currentScene != "Assets/Scenes/Work/ViewStage.unity")
		{
			EditorApplication.SaveScene(EditorApplication.currentScene);
			EditorApplication.OpenScene("Assets/Scenes/Work/ViewStage.unity");
		}
	}
	
	[@MenuItem("BAT Scene/Treasure")]
	private static void Treasure()
	{
		if(EditorApplication.currentScene != "Assets/Scenes/Work/Treasure.unity")
		{
			EditorApplication.SaveScene(EditorApplication.currentScene);
			EditorApplication.OpenScene("Assets/Scenes/Work/Treasure.unity");
		}
	}

}