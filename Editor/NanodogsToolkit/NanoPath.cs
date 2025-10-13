// NanoPath.cs (Editor)
using UnityEditor;
using UnityEngine;
using System.IO;

public static class NanoPath
{
    private static string _rootAssetPath; // e.g. "Assets/Plugins/Nanodogs-Toolkit"
    private static string _rootFullPath;  // e.g. "C:\Project\Assets\Plugins\Nanodogs-Toolkit"

    private const string ToolkitFolderName = "Nanodogs-Toolkit";

    public static string RootAssetPath
    {
        get
        {
            if (string.IsNullOrEmpty(_rootAssetPath))
                _rootAssetPath = FindToolkitRootAssetPath();
            return _rootAssetPath;
        }
    }

    public static string RootFullPath
    {
        get
        {
            if (string.IsNullOrEmpty(_rootFullPath))
            {
                if (string.IsNullOrEmpty(RootAssetPath))
                    return null;

                // Convert Unity asset path to full path
                // If it starts with "Assets", use Application.dataPath
                if (RootAssetPath.StartsWith("Assets"))
                {
                    // Application.dataPath points to full path to "Assets" folder.
                    string relative = RootAssetPath.Substring("Assets".Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    _rootFullPath = Path.GetFullPath(Path.Combine(Application.dataPath, relative));
                }
                else
                {
                    // For Packages or other paths, use AssetDatabase to get GUID then convert
                    // Fallback to Path.GetFullPath on AssetDatabase path
                    _rootFullPath = Path.GetFullPath(RootAssetPath);
                }
            }

            return _rootFullPath;
        }
    }

    public static string PrefabsAssetPath => Path.Combine(RootAssetPath, "Prefabs").Replace('\\', '/');
    public static string ImportFolderFullPath => Path.Combine(RootFullPath, "Editor", "NanodogsToolkit", "Import");

    private static string FindToolkitRootAssetPath()
    {
        // Search text assets, filter for package.json or README.md filenames
        string[] guids = AssetDatabase.FindAssets("t:TextAsset");
        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileName(assetPath);
            if (fileName == "package.json" || fileName == "README.md")
            {
                int idx = assetPath.IndexOf(ToolkitFolderName, System.StringComparison.OrdinalIgnoreCase);
                if (idx != -1)
                {
                    string root = assetPath.Substring(0, idx + ToolkitFolderName.Length).Replace('\\', '/');
                    return root;
                }
            }
        }

        Debug.LogError($"NanoPath: Could not locate {ToolkitFolderName} root. Make sure package.json or README.md exists in the toolkit root.");
        return null;
    }
}
