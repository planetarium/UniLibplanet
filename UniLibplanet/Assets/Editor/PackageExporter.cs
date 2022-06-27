using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class PackageExporter
{
    private const string OutPath = "out";
    private const string PluginsPath = "Assets/Plugins";
    private const string ScriptsPath = "Assets/Scripts";
    private const string SDKName = "UniLibplanet.unitypackage";

    [MenuItem("Tools/Build SDK")]
    public static void Export()
    {
        DirectoryInfo projectRoot = Directory.GetParent(Directory.GetCurrentDirectory());
        var outputDirectory = new DirectoryInfo(Path.Combine(projectRoot.FullName, OutPath));
        string exportPath = Path.Combine(outputDirectory.FullName, SDKName);
        
        outputDirectory.Create();

        if (!File.Exists(Path.Combine(PluginsPath, "Libplanet.Unity.dll")))
        {
            Debug.LogError("First, please build Libplanet.Unity.");
            return;
        }

        var exportedPackageAssetList = new List<string>();
        exportedPackageAssetList.Add(PluginsPath);
        exportedPackageAssetList.Add(ScriptsPath);

        AssetDatabase.ExportPackage(
            exportedPackageAssetList.ToArray(),
            exportPath,
            ExportPackageOptions.IncludeDependencies | ExportPackageOptions.Recurse);

        Debug.Log("Build Finish");
    }
}
