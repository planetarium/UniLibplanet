$DOTENV_PATH = ".\.env.xml"
$UNITY_DIR = (Select-Xml -Path $DOTENV_PATH -XPath "//UNITY_DIR[1]" | Select-Object -ExpandProperty Node).InnerText
$UNITY_PATH = Join-Path -Path $UNITY_DIR -ChildPath "Unity.exe"

Write-Host "Unity build start..."
& $UNITY_PATH -batchmode -quit -logFile debug.txt -nographics -projectPath ".\UniLibplanet" -executeMethod PackageExporter.Export
Write-Host "Unity build finished"
