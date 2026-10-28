$dotnet = "C:\Program Files\Unity\Hub\Editor\6000.6.0f1\Editor\Data\DotNetSdk\dotnet.exe"
$csc = "C:\Program Files\Unity\Hub\Editor\6000.6.0f1\Editor\Data\DotNetSdk\sdk\8.0.318\Roslyn\bincore\csc.dll"

$netStandardRef = "C:\Program Files\Unity\Hub\Editor\6000.6.0f1\Editor\Data\NetStandard\ref\2.1.0"
$unityEngineManaged = "C:\Program Files\Unity\Hub\Editor\6000.6.0f1\Editor\Data\Managed\UnityEngine"
$unityManaged = "C:\Program Files\Unity\Hub\Editor\6000.6.0f1\Editor\Data\Managed"

$references = (Get-ChildItem -Path $netStandardRef -Filter "*.dll" | Select-Object -ExpandProperty FullName) +
              (Get-ChildItem -Path $unityEngineManaged -Filter "*.dll" | Select-Object -ExpandProperty FullName) +
              (Get-ChildItem -Path $unityManaged -Filter "*.dll" | Select-Object -ExpandProperty FullName) +
              @(
                "Library\ScriptAssemblies\UnityEngine.UI.dll",
                "Library\ScriptAssemblies\Unity.TextMeshPro.dll",
                "Library\ScriptAssemblies\Unity.TextMeshPro.Editor.dll",
                "Library\ScriptAssemblies\Unity.InputSystem.dll",
                "Temp\bin\Debug\Assembly-CSharp.dll"
              )

$refArgs = $references | Where-Object { Test-Path $_ } | ForEach-Object { "-r:`"$_`"" }

$sources = @("Assets\Editor\DragonBallGameSetup.cs")

$argList = @(
    "`"$csc`"",
    "-target:library",
    "-out:`"Temp\EditorTest.dll`"",
    "-langversion:9.0",
    "-nowarn:0169,0649,0282,1701,1702",
    "-define:ENABLE_INPUT_SYSTEM"
) + $refArgs + ($sources | ForEach-Object { "`"$_`"" })

& $dotnet $argList
Write-Output "Editor Compilation exit code: $LASTEXITCODE"
