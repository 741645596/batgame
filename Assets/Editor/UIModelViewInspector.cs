//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Inspector class used to edit UITextures.
/// </summary>

[CanEditMultipleObjects]
#if UNITY_3_5
[CustomEditor(typeof(UIModelView))]
#else
[CustomEditor(typeof(UIModelView), true)]
#endif

public class UIModelViewInspector : UITextureInspector
{

    protected override bool ShouldDrawProperties()
    {
		NGUIEditorTools.DrawProperty("UseRenderTexture", serializedObject, "mUseRenderTexture");
		NGUIEditorTools.DrawProperty("SceneLayer", serializedObject, "mSceneLayer");
        NGUIEditorTools.DrawProperty("ModelCamera", serializedObject, "mModelCamera");

        return base.ShouldDrawProperties();
        
    }
}