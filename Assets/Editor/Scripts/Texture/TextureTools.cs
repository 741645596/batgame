using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;
using System.IO;

public class TextureTools
{
    public static List<Texture2D> GetSelectedTextures()
    {
        Object[] assetObjs = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets);
        List<Texture2D> texList = new List<Texture2D>();
        foreach (Object obj in assetObjs) {
            texList.Add((Texture2D)obj);
        }
        if (texList.Count == 0) {
            EditorUtility.DisplayDialog("贴图工具", "没有选中任何贴图", "确定");
        }
        return texList;
    }
    
    public static List<Material> GetSelectedMaterials()
    {
        Object[] assetObjs = Selection.GetFiltered(typeof(Material), SelectionMode.Assets);
        List<Material> matList = new List<Material>();
        foreach (Object obj in assetObjs) {
            matList.Add((Material)obj);
        }
        if (matList.Count == 0) {
            EditorUtility.DisplayDialog("贴图工具", "没有选中任何贴图", "确定");
        }
        return matList;
    }
    
    public static void SeparateAlpha(Texture2D tex, out Texture2D texRGB, out Texture2D texA)
    {
        texRGB = new Texture2D(tex.width, tex.height, TextureFormat.RGB24, false);
        texA = new Texture2D(tex.width, tex.height, TextureFormat.RGB24, false);
        
        for (int x = 0; x < tex.width; ++x) {
            for (int y = 0; y < tex.height; ++y) {
                Color color = tex.GetPixel(x, y);
                
                Color color_rgb = new Color(color.r, color.g, color.b, 1);
                texRGB.SetPixel(x, y, color_rgb);
                
                Color color_a = new Color(color.a, 0, 0, 1);
                texA.SetPixel(x, y, color_a);
            }
        }
        
        texRGB.Apply();
        texA.Apply();
    }
    
    public static Texture2D CombineAlpha(Texture2D texRGB, Texture2D texA)
    {
        Texture2D tex = new Texture2D(texRGB.width, texRGB.height, TextureFormat.RGBA32, false);
        for (int x = 0; x < tex.width; ++x) {
            for (int y = 0; y < tex.height; ++y) {
                Color colorRGB = texRGB.GetPixel(x, y);
                Color colorA = texA.GetPixel(x, y);
                Color color = new Color(colorRGB.r, colorRGB.g, colorRGB.b, colorA.r);
                tex.SetPixel(x, y, color);
            }
        }
        tex.Apply();
        return tex;
    }
    
    [MenuItem("Assets/贴图工具/分离透明通道")]
    public static void SeparateAlphaForSelected()
    {
        List<string> outTexPaths = new List<string>();
        
        List<Texture2D> texList = GetSelectedTextures();
        foreach (Texture2D tex in texList) {
            Texture2D texRGB, texA;
            
            SeparateAlpha(tex, out texRGB, out texA);
            
            string path = AssetDatabase.GetAssetOrScenePath(tex);
            
            byte[] bytesRGB = texRGB.EncodeToPNG();
            string pathRGB = Path.ChangeExtension(path, ".rgb.png");
            File.WriteAllBytes(pathRGB, bytesRGB);
            outTexPaths.Add(pathRGB);
            
            byte[] bytesA = texA.EncodeToPNG();
            string pathA = Path.ChangeExtension(path, ".a.png");
            File.WriteAllBytes(pathA, bytesA);
            outTexPaths.Add(pathA);
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        ReimportETC(outTexPaths);
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    
    [MenuItem("Assets/贴图工具/重导UI贴图")]
    public static void ReimportSelectedTextures()
    {
        List<Texture2D> texList = GetSelectedTextures();
        foreach (Texture2D tex in texList) {
            string path = AssetDatabase.GetAssetOrScenePath(tex);
            Reimport(path);
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    
    public static void Reimport(string path)
    {
        TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
        ti.textureType = TextureImporterType.Default;
        ti.npotScale = TextureImporterNPOTScale.None;
        ti.isReadable = true;
        ti.alphaIsTransparency = true;
        ti.linearTexture = true;
        ti.mipmapEnabled = false;
        ti.wrapMode = TextureWrapMode.Clamp;
        ti.maxTextureSize = 1024;
        ti.textureFormat = TextureImporterFormat.AutomaticTruecolor;
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
    }
    
    public static void ReimportETC(List<string> outTexPaths)
    {
        foreach (string path in outTexPaths) {
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            ti.textureType = TextureImporterType.Default;
            ti.npotScale = TextureImporterNPOTScale.None;
            ti.mipmapEnabled = false;
            ti.maxTextureSize = 1024;
            ti.textureFormat = TextureImporterFormat.ETC_RGB4;
            
            // According to Unity document, when texture is not readable, it consumes much less memory,
            // because a system-memory copy does not have to be kept around after texture is uploaded to the graphics API.
            ti.isReadable = false;
            
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
        }
    }
    
    [MenuItem("Assets/贴图工具/更改材质为分离透明通道")]
    public static void SeparateAlphaMaterialForSelected()
    {
        List<string> outTexPaths = new List<string>();
        
        List<Material> matList = GetSelectedMaterials();
        
        foreach (Material mat in matList) {
            if (!mat.shader.name.Contains("VertexLit")) {
                continue;
            }
            
            outTexPaths.Clear();
            Texture2D tex = mat.mainTexture as Texture2D;
            Texture2D texRGB, texA;
            
            string path = AssetDatabase.GetAssetOrScenePath(tex);
            Reimport(path);
            
            SeparateAlpha(tex, out texRGB, out texA);
            
            byte[] bytesA = texA.EncodeToPNG();
            if (!ExistAlpha(bytesA)) {
                continue;
            }
            string pathA = Path.ChangeExtension(path, ".a.png");
            File.WriteAllBytes(pathA, bytesA);
            outTexPaths.Add(pathA);
            
            byte[] bytesRGB = texRGB.EncodeToPNG();
            string pathRGB = Path.ChangeExtension(path, ".rgb.png");
            File.WriteAllBytes(pathRGB, bytesRGB);
            outTexPaths.Add(pathRGB);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            ReimportETC(outTexPaths);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Shader sd = mat.shader;
            Color mainColor = mat.GetColor("_Color");
            Color specColor = mat.GetColor("_SpecColor");
            Color emission = mat.GetColor("_Emission");
            float shininess = mat.GetFloat("_Shininess");
            
            texRGB = AssetDatabase.LoadAssetAtPath(pathRGB, typeof(Texture2D)) as Texture2D;
            texA = AssetDatabase.LoadAssetAtPath(pathA, typeof(Texture2D)) as Texture2D;
            mat.shader = Shader.Find("Custom/VertexLit(Transparent, AlphaSep)");
            mat.SetTexture("_MainTexRGB", texRGB);
            mat.SetTexture("_MainTexA", texA);
            mat.SetColor("_Color", mainColor);
            mat.SetColor("_SpecColor", specColor);
            mat.SetColor("_Emission", emission);
            mat.SetFloat("_Shininess", shininess);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
    
    public static bool ExistAlpha(byte[] bytesA)
    {
        for (int i = 0; i < bytesA.Length; i++) {
            if (bytesA[i] > 0) {
                return true;
            }
        }
        return false;
    }
}

