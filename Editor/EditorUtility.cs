using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class EditorUtility
{
    private static readonly string s_tempBuildPath = Path.Combine(".", "TempBuild");

    private static readonly string s_bundlesPath = Path.Combine(Application.dataPath, "../../../Bundles");

    private static readonly string s_assemblyPath = Path.Combine(Application.dataPath, "../../../JuiceboxFramework/bin/Release");

    private static readonly string s_pluginsPath = Path.Combine(Application.dataPath, "Plugins");

    private static readonly string[] s_assemblyList = new[]
    {
        "JuiceboxFramework.dll",
        "RSG.Promise.dll",
        "websocket-sharp.dll"
    };

    private static string OutputBundleName => EditorConfig.BundleName + ".game.bundle";

    [MenuItem("Juicebox/Configure Editor")]
    private static void SelectEditorConfig() => Selection.activeObject = EditorConfig.Instance;

    [MenuItem("Juicebox/Build AssetBundle _F6")]
    private static void BuildAssetBundle()
    {
        Debug.Log("Building AssetBundle...");
        Directory.CreateDirectory(s_tempBuildPath);
        try
        {
            BuildPipeline.BuildAssetBundles(s_tempBuildPath, BuildAssetBundleOptions.StrictMode | BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows);
            File.Copy(Path.Combine(s_tempBuildPath, "game.bundle"), Path.Combine(s_bundlesPath, OutputBundleName), true);
            Debug.Log($"Build process complete! If successful, built as {OutputBundleName}");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to build AssetBundle: {e}");
        }
        finally
        {
            Directory.Delete(s_tempBuildPath, true);
        }
    }

    [MenuItem("Juicebox/Reload Assemblies _F5")]
    private static void ReloadAssemblies()
    {
        if (!Directory.Exists(s_pluginsPath))
            Directory.CreateDirectory(s_pluginsPath);

        List<string> assemblies = new(s_assemblyList);

        foreach (string name in s_assemblyList)
        {
            string dataName = Path.GetFileNameWithoutExtension(name) + ".xml";
            if (File.Exists(Path.Combine(s_assemblyPath, dataName)))
                assemblies.Add(dataName);
        }

        bool hasUpdated = false;
        foreach (string name in assemblies)
        {
            string pathSource = Path.Combine(s_assemblyPath, name);
            string pathDest = Path.Combine(s_pluginsPath, name);
            if (!File.Exists(pathDest) || File.GetLastWriteTime(pathSource) > File.GetLastWriteTime(pathDest))
            {
                if (!File.Exists(pathSource))
                {
                    Debug.LogWarning($"Could not find {name} in source directory!");
                    continue;
                }

                Debug.Log($"Copying {name}...");
                File.Copy(pathSource, pathDest, true);
                hasUpdated = true;
            }
        }

        if (hasUpdated)
            AssetDatabase.Refresh();

        else
            Debug.Log("Assemblies are up to date.");
    }
}
