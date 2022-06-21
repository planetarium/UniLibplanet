$DOTENV_FILE = Get-Content ".\.env"
$UNITY_DIR = [regex]::Match($DOTENV_FILE, "UNITY_DIR=`"(.*?)\\`"").Groups[1].Value

$UNITY_PATH = $UNITY_DIR + "Unity.exe"
 
Write-Output ""
Write-Output "Unity Build Start"
Write-Output ""

& $UNITY_PATH -batchmode -quit -logFile debug.txt -nographics -projectPath "$PWD\UnitySDK" -executeMethod PackageExporter.Export
