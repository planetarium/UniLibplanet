using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class PackageExporter
{
    public static void Export()
    {
        DirectoryInfo projectRoot = Directory.GetParent(Directory.GetCurrentDirectory());
        var outputDirectory = new DirectoryInfo(Path.Combine(projectRoot.FullName, "out"));
        string exportPath = Path.Combine(outputDirectory.FullName, "Unilibplanet.unitypackage");
        
        outputDirectory.Create();

        var exportedPackageAssetList = new List<string>();
        //Add Prefabs folder into the asset list
        exportedPackageAssetList.Add("Assets/Plugins");
        exportedPackageAssetList.Add("Assets/Libplanet.Unity");

        AssetDatabase.ExportPackage(
            exportedPackageAssetList.ToArray(),
            exportPath,
            ExportPackageOptions.IncludeDependencies | ExportPackageOptions.Recurse);
    }
}
