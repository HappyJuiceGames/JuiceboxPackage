using System.IO;
using UnityEditor;
using UnityEngine;

public class EditorConfig : ScriptableObject
{
    [SerializeField]
    private string _bundleName;

    public static string BundleName => Instance._bundleName;

    private static EditorConfig s_instance;

    public static EditorConfig Instance
    {
        get
        {
            if (!s_instance)
            {
                s_instance = Resources.Load<EditorConfig>("EditorConfig");
                if (!s_instance)
                    CreateEditorConfig();
            }

            return s_instance;
        }
    }

    private static void CreateEditorConfig()
    {
        if (!Directory.Exists(Path.Combine(Application.dataPath, "Editor")))
            AssetDatabase.CreateFolder("Assets", "Editor");

        if (!Directory.Exists(Path.Combine(Application.dataPath, "Editor/Resources")))
            AssetDatabase.CreateFolder("Assets/Editor", "Resources");

        EditorConfig config = CreateInstance<EditorConfig>();
        AssetDatabase.CreateAsset(config, "Assets/Editor/Resources/EditorConfig.asset");
        s_instance = config;
    }
}
