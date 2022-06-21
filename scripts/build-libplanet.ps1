$DLLS_DIR = ".\Pack\bin\Release\netstandard2.0\*.dll"
$PLUGINS_DIR = "$PWD\Unilibplanet\Assets\Plugins\"

New-Item -Path $PLUGINS_DIR -ItemType Directory

Write-Output ""
Write-Output "Start DLL Build and Copy"
Write-Output ""

& dotnet build -p:Configuration=Release

Copy-Item -Path $DLLS_DIR -Exclude "Pack.dll", "Microsoft.CSharp.dll", "System.ServiceModel.Primitives.dll" -Destination $PLUGINS_DIR
