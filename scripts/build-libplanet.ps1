$LIBPLANET_UNITY_DIR = ".\Libplanet.Unity\"
$DLLS_DIR = ".\Libplanet.Unity\bin\Release\netstandard2.1\*.dll"
$RUNTIME_DLL_DIR = ".\Libplanet.Unity\runtimes\*"
$PLUGINS_DIR = ".\UniLibplanet\Assets\Plugins\"
$EXCLUDES = @("Microsoft.CSharp.dll", "System.ServiceModel.Primitives.dll", "Unity*.dll")
$ARTIFACT_DIRS = @(".\Libplanet.Unity\bin\", ".\Libplanet.Unity\obj\")

Push-Location (Split-Path -Parent $PSScriptRoot)
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
Copy-Item -Path $RUNTIME_DLL_DIR -Destination $PLUGINS_DIR -Recurse

Write-Host "Removing artifacts..."
Remove-Item -Path $ARTIFACT_DIRS -Recurse
Pop-Location
