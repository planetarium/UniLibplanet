$DOTENV_FILE = Get-Content ".\.env"
$UNITY_DIR = [regex]::Match($DOTENV_FILE, "UNITY_DIR=`"(.*?)\\`"").Groups[1].Value

$DLL_PATH = "\bin\Release\netstandard2.0"

$REQUIRED_LIBPLANETS = New-Object string[] 6
$REQUIRED_LIBPLANETS[0] = ".\libplanet\Libplanet$DLL_PATH\Libplanet.dll"
$REQUIRED_LIBPLANETS[1] = ".\libplanet\Libplanet.Net$DLL_PATH\Libplanet.Net.dll"
$REQUIRED_LIBPLANETS[2] = ".\libplanet\Libplanet.Node$DLL_PATH\Libplanet.Node.dll"
$REQUIRED_LIBPLANETS[3] = ".\libplanet\Libplanet.RocksDBStore$DLL_PATH\Libplanet.RocksDBStore.dll"
$REQUIRED_LIBPLANETS[4] = ".\libplanet\Libplanet.Stun$DLL_PATH\Libplanet.Stun.dll"
$REQUIRED_LIBPLANETS[5] = ".\Libplanet.Unity$DLL_PATH\Libplanet.Unity.dll"

$PLUGINS_DIR = ".\UnitySDK\Assets\Plugins\Libplanet\"

Write-Output ""
Write-Output "Start DLL Build and Copy"
Write-Output ""

& dotnet build -p:Configuration=Release

foreach ($p in $REQUIRED_LIBPLANETS) {
    Copy-Item $p "$PLUGINS_DIR\" -Force
}

$UNITY_PATH = $UNITY_DIR + "Unity.exe"

Write-Output ""
Write-Output "Unity Build Start"
Write-Output ""

& $UNITY_PATH -batchmode -quit -logFile debug.txt -nographics -projectPath "$PWD\UnitySDK" -executeMethod PackageExporter.Export
