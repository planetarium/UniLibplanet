$LIBPLANET_UNITY_DIR = ".\UniLibplanet\Assets\Libplanet.Unity\"
$DLLS_DIR = ".\UniLibplanet\Assets\Libplanet.Unity\bin\Release\netstandard2.1\*.dll"
$PLUGINS_DIR = ".\UniLibplanet\Assets\Plugins\"
$EXCLUDES = @("Libplanet.Unity.dll", "Microsoft.CSharp.dll", "System.ServiceModel.Primitives.dll", "Unity*.dll")
$ARTIFACT_DIRS = @(".\UniLibplanet\Assets\Libplanet.Unity\bin\", ".\UniLibplanet\Assets\Libplanet.Unity\obj\")

Write-Host "Starting DLL build..."
& dotnet build $LIBPLANET_UNITY_DIR --configuration Release

if (Test-Path $PLUGINS_DIR)
{
    Write-Host "Existing $PLUGINS_DIR found"
    Write-Host "Removing existing $PLUGINS_DIR..."
    Remove-Item -Path $PLUGINS_DIR -Recurse
}
New-Item -Path $PLUGINS_DIR -ItemType Directory

Write-Host "Copying DLLs to target directory..."
Copy-Item -Path $DLLS_DIR -Exclude $EXCLUDES -Destination $PLUGINS_DIR

Write-Host "Removing artifacts..."
Remove-Item -Path $ARTIFACT_DIRS -Recurse