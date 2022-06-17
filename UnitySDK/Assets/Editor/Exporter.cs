using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class Exporter
{
    public static void Export()
    {
        string pluginsPath = "Assets/Plugins/";
        var exportPath = "./SDK.unitypackage";

        string[] pluginsFiles = (string[])Directory.GetFiles(pluginsPath, "*.*", SearchOption.AllDirectories);
        string[] files = new string[pluginsFiles.Length];

        pluginsFiles.CopyTo(files, 0);

        AssetDatabase.ExportPackage(
            files,
            exportPath,
            ExportPackageOptions.Default);
    }
}
