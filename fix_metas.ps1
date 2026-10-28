$template = Get-Content "Assets\Sprites\GokuSSJ.png.meta" -Raw
$files = @('Background.jpg', 'DragonBallAltar.png', 'Goku.png')
foreach ($f in $files) {
    $metaPath = "Assets\Sprites\$f.meta"
    $lines = Get-Content $metaPath
    $oldGuid = $lines[1].Split(' ')[1].Trim()
    $newContent = $template -replace 'guid: [0-9a-fA-F]+', "guid: $oldGuid"
    Set-Content -Path $metaPath -Value $newContent
}
Write-Output "Primary sprite metas configured as 2D Sprites!"
