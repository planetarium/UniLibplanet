$DOTENV_PATH = ".\.env.xml"

function Get-Unity-Paths {
    return @(Get-ChildItem -Path "\Program Files\Unity" -Filter "Unity.exe" -Recurse)
}

function Select-Unity-Path ($paths) {
    $caption = "Unity Editor versions found"
    $i = 0
    $list = ""
    foreach ($path in $paths) {
        $i++
        $list = $list + "$i - $path" + [Environment]::NewLine
    }

    $message = @"
Choose a version
$list
"@

    $i = 0
    $options = @()
    foreach ($path in $paths) {
        $i++
        $options = $options + [System.Management.Automation.Host.ChoiceDescription]::new("&$i", "$path")
    }
    $options = $options + [System.Management.Automation.Host.ChoiceDescription]::new("&Quit", "Terminate")
    $choice = $Host.UI.PromptForChoice($caption, $message, $options, $options.Count - 1)
    if ($choice -eq $options.Count - 1)
    {
        Write-Host "Terminating..."
        exit
    }
    Write-Host "$($paths[$choice]) selected"
    return Split-Path -Path $paths[$choice]
}

function Write-Env ($content) {
    if (Test-Path -Path $DOTENV_PATH) {
        $caption = "Existing $DOTENV_PATH found"
        $message = "Overwrite it?"
        $options = @(
            [System.Management.Automation.Host.ChoiceDescription]::new("&Yes", "Remove existing $DOTENV_PATH and create a new $DOTENV_PATH")
            [System.Management.Automation.Host.ChoiceDescription]::new("&No", "Do nothing")
        )
        $choice = $Host.UI.PromptForChoice($caption, $message, $options, 1)
        if ($choice -eq 0) {
            Write-Host "Removing existing $DOTENV_PATH..."
            Remove-Item $DOTENV_PATH
        }
        else {
            return
        }
    }

    Write-Host "Writing new $DOTENV_PATH..."
    Set-Content $DOTENV_PATH $content
}

$unity_paths = Get-Unity-Paths
if ($unity_paths.Count -eq 0) {
    Write-Host "No Unity Editor was found"
    exit
}
$unity_dir = Select-Unity-Path($unity_paths)
$unity_engine_dir = Join-Path -Path $unity_dir -ChildPath "Data\Managed\UnityEngine"
$dotenv_content = @"
<Project>
	<PropertyGroup>
		<UNITY_DIR>$unity_dir</UNITY_DIR>
		<UNITY_ENGINE_DIR>$unity_engine_dir</UNITY_ENGINE_DIR>
	</PropertyGroup>
</Project>
"@
Write-Env($dotenv_content)
