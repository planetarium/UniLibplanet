$XMLENV_PATH = ".\.env.xml"
$DOTENV_PATH = ".\.env"

$expected_paths = Get-ChildItem -Path "\Program Files" -Filter "unity.exe" -Recurse | Select-Object Fullname | Format-List | Out-String
$expected_paths = $expected_paths.Split("`n", [StringSplitOptions]::RemoveEmptyEntries)

$unity_paths = @()

foreach ($p in $expected_paths) {
    if ($p) {
        $path_data = [regex]::Match($p, '^FullName\s:\s(.*)Unity.exe').captures.groups
        if ($path_data) {
            $unity_paths = $unity_paths + $path_data[1].value
        }
    }
}

$TARGET_PATH = "null"

do {
    $option = 1
    foreach ($p in $unity_paths) {
        Write-Output $option" - [ "$p" ]"
        $option++
    }

    Write-Output ""
    $Prompt = Read-host "Pls Select"
    if ( ($Prompt -gt 0 -and $Prompt -lt $option) ) {
        $TARGET_PATH = $unity_paths[$Prompt - 1]
        Write-Output ""
        Write-Output "Selected Path"$TARGET_PATH
        Write-Output ""
    }
    else {
        Write-Output ""
        Write-Output "Wrong Selection!"
        Write-Output ""
    }
} while ($TARGET_PATH -eq "null")

$UNITY_DIR = "$TARGET_PATH"
$UNITY_ENGINE_DIR = $UNITY_DIR + "Data\Managed\UnityEngine\"

$xml_value =
"<Project>
`t<PropertyGroup>
`t`t<UNITY_DIR>$UNITY_DIR</UNITY_DIR>
`t`t<UNITY_ENGINE_DIR>$UNITY_ENGINE_DIR</UNITY_ENGINE_DIR>
`t</PropertyGroup>
</Project>
"

$dotenv_value =
"UNITY_ENGINE_DIR=`"$UNITY_ENGINE_DIR\`"
UNITY_DIR=`"$UNITY_DIR\`"
"

Remove-Item $XMLENV_PATH
New-Item $XMLENV_PATH -ItemType File
Set-Content $XMLENV_PATH $xml_value

Remove-Item $DOTENV_PATH
New-Item $DOTENV_PATH -ItemType File
Set-Content $DOTENV_PATH $dotenv_value
