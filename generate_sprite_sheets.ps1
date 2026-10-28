Add-Type -AssemblyName System.Drawing

function Save-PngWithMeta($bitmap, $pngPath, $slices, $ppu = 32) {
    $bitmap.Save($pngPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $guid = [System.Guid]::NewGuid().ToString("N")
    
    $sliceYaml = ""
    if ($slices) {
        $sliceYaml = "  spriteSheet:`n    serializedVersion: 2`n    sprites:`n"
        foreach ($s in $slices) {
            $sGuid = [System.Guid]::NewGuid().ToString("N")
            $sliceYaml += @"
    - serializedVersion: 2
      name: $($s.name)
      rect:
        serializedVersion: 2
        x: $($s.x)
        y: $($s.y)
        width: $($s.w)
        height: $($s.h)
      alignment: 0
      pivot: {x: 0.5, y: 0.5}
      border: {x: 0, y: 0, z: 0, w: 0}
      outline: []
      physicsShape: []
      tessellationDetail: 0
      bones: []
      spriteID: $sGuid
      internalID: 0
      vertices: []
      indices: 
      edges: []
      weights: []
"@ + "`n"
        }
    }

    $spriteMode = if ($slices) { 2 } else { 1 } # 2 = Multiple, 1 = Single
    
    $metaContent = @"
fileFormatVersion: 2
guid: $guid
TextureImporter:
  internalIDToNameTable: []
  externalObjects: {}
  serializedVersion: 13
  mipmaps:
    mipMapMode: 0
    enableMipMap: 0
    sRGBTexture: 1
    linearTexture: 0
    fadeOut: 0
    borderMipMap: 0
    mipMapsPreserveCoverage: 0
    alphaTestReferenceValue: 0.5
    mipMapFadeDistanceStart: 1
    mipMapFadeDistanceEnd: 3
  bumpmap:
    convertToNormalMap: 0
    externalNormalMap: 0
    heightScale: 0.25
    normalMapFilter: 0
    flipGreenChannel: 0
  isReadable: 0
  streamingMipmaps: 0
  streamingMipmapsPriority: 0
  vTOnly: 0
  ignoreMasterTextureLimit: 0
  doesMipmapHaveCorrectPhysicalSize: 1
  grayScaleToAlpha: 0
  generateCubemap: 6
  cubemapConvolution: 0
  seamlessCubemap: 0
  textureFormat: 1
  maxTextureSize: 2048
  textureSettings:
    serializedVersion: 2
    filterMode: 0
    aniso: 1
    mipBias: 0
    wrapU: 1
    wrapV: 1
    wrapW: 1
  nPOTScale: 0
  lightmap: 0
  compressionQuality: 50
  spriteMode: $spriteMode
  spriteExtrude: 1
  spriteMeshType: 1
  alignment: 0
  spritePivot: {x: 0.5, y: 0.5}
  spritePixelsPerUnit: $ppu
  spriteBorder: {x: 0, y: 0, z: 0, w: 0}
  spriteGenerateFallbackPhysicsShape: 1
  alphaUsage: 1
  alphaIsTransparency: 1
  spriteTessellationDetail: -1
  textureType: 8
  textureShape: 1
  singleChannelComponentMapping: 0
  maxTextureSizeSet: 0
  compressionQualitySet: 0
  textureFormatSet: 0
  ignorePngGamma: 0
  applyGammaDecoding: 0
  cookieLightType: 0
  platformSettings: []
$sliceYaml
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"@
    Set-Content -Path ($pngPath + ".meta") -Value $metaContent -NoNewline
}

# 1. Goku_Sheet.png (7 frames: 4 Idle, 3 Attack), each 48x48
$gokuBmp = New-Object System.Drawing.Bitmap(336, 48)
$g = [System.Drawing.Graphics]::FromImage($gokuBmp)
$g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::NearestNeighbor
$g.Clear([System.Drawing.Color]::Transparent)

# Draw 4 Idle frames
for ($i = 0; $i -lt 4; $i++) {
    $ox = $i * 48
    $bob = if ($i % 2 -eq 0) { 0 } else { 1 }
    # Hair (Spiky black Goku hair)
    $brushHair = [System.Drawing.Brushes]::Black
    $g.FillEllipse($brushHair, $ox + 16, 6 + $bob, 18, 18)
    $g.FillPolygon($brushHair, @([System.Drawing.Point]::new($ox + 20, 2 + $bob), [System.Drawing.Point]::new($ox + 26, 10 + $bob), [System.Drawing.Point]::new($ox + 14, 10 + $bob)))
    $g.FillPolygon($brushHair, @([System.Drawing.Point]::new($ox + 10, 8 + $bob), [System.Drawing.Point]::new($ox + 18, 14 + $bob), [System.Drawing.Point]::new($ox + 14, 20 + $bob)))
    $g.FillPolygon($brushHair, @([System.Drawing.Point]::new($ox + 34, 6 + $bob), [System.Drawing.Point]::new($ox + 26, 12 + $bob), [System.Drawing.Point]::new($ox + 30, 20 + $bob)))
    # Face
    $brushSkin = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 255, 218, 185))
    $g.FillEllipse($brushSkin, $ox + 18, 14 + $bob, 13, 13)
    # Eyes
    $g.FillRectangle([System.Drawing.Brushes]::Black, $ox + 25, 18 + $bob, 3, 2)
    # Orange Gi Body
    $brushGi = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 255, 120, 0))
    $g.FillRectangle($brushGi, $ox + 17, 26 + $bob, 15, 12)
    # Blue undershirt & belt
    $brushBlue = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 0, 90, 210))
    $g.FillRectangle($brushBlue, $ox + 20, 26 + $bob, 9, 5)
    $g.FillRectangle($brushBlue, $ox + 17, 34 + $bob, 15, 3)
    # Boots
    $g.FillRectangle($brushBlue, $ox + 17, 39 + $bob, 6, 7)
    $g.FillRectangle($brushBlue, $ox + 26, 39 + $bob, 6, 7)
}

# Draw 3 Attack frames (thrust forward & fire)
for ($i = 0; $i -lt 3; $i++) {
    $ox = (4 + $i) * 48
    # Hair
    $g.FillEllipse([System.Drawing.Brushes]::Black, $ox + 13, 6, 19, 18)
    $g.FillPolygon([System.Drawing.Brushes]::Black, @([System.Drawing.Point]::new($ox + 16, 2), [System.Drawing.Point]::new($ox + 24, 10), [System.Drawing.Point]::new($ox + 12, 10)))
    $g.FillPolygon([System.Drawing.Brushes]::Black, @([System.Drawing.Point]::new($ox + 6, 8), [System.Drawing.Point]::new($ox + 15, 14), [System.Drawing.Point]::new($ox + 11, 20)))
    # Face
    $brushSkin = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 255, 218, 185))
    $g.FillEllipse($brushSkin, $ox + 17, 14, 13, 13)
    # Fierce eye
    $g.FillRectangle([System.Drawing.Brushes]::Black, $ox + 24, 17, 4, 3)
    # Body leaning forward
    $brushGi = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 255, 120, 0))
    $g.FillRectangle($brushGi, $ox + 16, 26, 16, 12)
    $brushBlue = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 0, 90, 210))
    $g.FillRectangle($brushBlue, $ox + 16, 34, 16, 3)
    $g.FillRectangle($brushBlue, $ox + 14, 39, 7, 7)
    $g.FillRectangle($brushBlue, $ox + 26, 39, 7, 7)
    # Extended arms & Ki flash
    $armX = $ox + 30 + ($i * 3)
    $g.FillRectangle($brushSkin, $ox + 24, 27, 12, 5)
    $brushKi = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 100, 220, 255))
    $g.FillEllipse($brushKi, $armX, 23, 10 + ($i * 4), 12 + ($i * 2))
}
$g.Dispose()

$gokuSlices = @(
    @{ name = "Goku_Idle_0"; x = 0; y = 0; w = 48; h = 48 },
    @{ name = "Goku_Idle_1"; x = 48; y = 0; w = 48; h = 48 },
    @{ name = "Goku_Idle_2"; x = 96; y = 0; w = 48; h = 48 },
    @{ name = "Goku_Idle_3"; x = 144; y = 0; w = 48; h = 48 },
    @{ name = "Goku_Attack_0"; x = 192; y = 0; w = 48; h = 48 },
    @{ name = "Goku_Attack_1"; x = 240; y = 0; w = 48; h = 48 },
    @{ name = "Goku_Attack_2"; x = 288; y = 0; w = 48; h = 48 }
)
Save-PngWithMeta $gokuBmp "Assets\Sprites\Goku_Sheet.png" $gokuSlices 32

# 2. RedRibbon_Sheet.png (4 frames), each 48x48
$rrBmp = New-Object System.Drawing.Bitmap(192, 48)
$g = [System.Drawing.Graphics]::FromImage($rrBmp)
$g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::NearestNeighbor
$g.Clear([System.Drawing.Color]::Transparent)

for ($i = 0; $i -lt 4; $i++) {
    $ox = $i * 48
    $bob = if ($i % 2 -eq 0) { 0 } else { 2 }
    # Armor body
    $brushMetal = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 120, 130, 140))
    $g.FillEllipse($brushMetal, $ox + 12, 10 + $bob, 24, 26)
    # Red Ribbon insignia
    $brushRed = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 220, 20, 20))
    $g.FillRectangle($brushRed, $ox + 18, 20 + $bob, 12, 8)
    # Visor Eye (Cyan scanner)
    $brushVisor = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 0, 240, 255))
    $g.FillRectangle($brushVisor, $ox + 10, 14 + $bob, 8, 4)
    # Legs/Jets
    $jetLen = 6 + ($i * 2)
    $brushFlame = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 255, 180, 0))
    $g.FillPolygon($brushFlame, @([System.Drawing.Point]::new($ox + 20, 36 + $bob), [System.Drawing.Point]::new($ox + 28, 36 + $bob), [System.Drawing.Point]::new($ox + 24, 36 + $bob + $jetLen)))
}
$g.Dispose()

$rrSlices = @(
    @{ name = "RedRibbon_0"; x = 0; y = 0; w = 48; h = 48 },
    @{ name = "RedRibbon_1"; x = 48; y = 0; w = 48; h = 48 },
    @{ name = "RedRibbon_2"; x = 96; y = 0; w = 48; h = 48 },
    @{ name = "RedRibbon_3"; x = 144; y = 0; w = 48; h = 48 }
)
Save-PngWithMeta $rrBmp "Assets\Sprites\RedRibbon_Sheet.png" $rrSlices 32

# 3. Saibamen_Sheet.png (4 frames), each 48x48
$sbBmp = New-Object System.Drawing.Bitmap(192, 48)
$g = [System.Drawing.Graphics]::FromImage($sbBmp)
$g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::NearestNeighbor
$g.Clear([System.Drawing.Color]::Transparent)

for ($i = 0; $i -lt 4; $i++) {
    $ox = $i * 48
    $bob = if ($i % 2 -eq 0) { 1 } else { -1 }
    # Green alien body & head
    $brushGreen = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 60, 180, 50))
    $g.FillEllipse($brushGreen, $ox + 14, 8 + $bob, 20, 18)
    $g.FillEllipse($brushGreen, $ox + 16, 22 + $bob, 16, 16)
    # Claws
    $brushDarkGreen = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 30, 120, 25))
    $g.FillRectangle($brushDarkGreen, $ox + 8, 20 + $bob, 6, 8)
    $g.FillRectangle($brushDarkGreen, $ox + 34, 20 + $bob, 6, 8)
    # Glowing red menacing eyes
    $brushRed = [System.Drawing.Brushes]::Red
    $g.FillRectangle($brushRed, $ox + 18, 14 + $bob, 3, 3)
    $g.FillRectangle($brushRed, $ox + 26, 14 + $bob, 3, 3)
    # Scuttling legs
    $legShift = ($i % 2) * 4
    $g.FillRectangle($brushDarkGreen, $ox + 14 - $legShift, 36, 6, 8)
    $g.FillRectangle($brushDarkGreen, $ox + 28 + $legShift, 36, 6, 8)
}
$g.Dispose()

$sbSlices = @(
    @{ name = "Saibamen_0"; x = 0; y = 0; w = 48; h = 48 },
    @{ name = "Saibamen_1"; x = 48; y = 0; w = 48; h = 48 },
    @{ name = "Saibamen_2"; x = 96; y = 0; w = 48; h = 48 },
    @{ name = "Saibamen_3"; x = 144; y = 0; w = 48; h = 48 }
)
Save-PngWithMeta $sbBmp "Assets\Sprites\Saibamen_Sheet.png" $sbSlices 32

# 4. Items_Sheet.png (DragonBallAltar + KiBlast), 128x64
$itemBmp = New-Object System.Drawing.Bitmap(128, 64)
$g = [System.Drawing.Graphics]::FromImage($itemBmp)
$g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::NearestNeighbor
$g.Clear([System.Drawing.Color]::Transparent)

# Left: 7 Dragon Balls on Altar (64x64)
$brushPedestal = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 70, 75, 90))
$g.FillRectangle($brushPedestal, 8, 42, 48, 18)
$brushCloth = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 180, 30, 30))
$g.FillRectangle($brushCloth, 12, 38, 40, 6)

$ballBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 255, 170, 0))
$starBrush = [System.Drawing.Brushes]::Red

# 7 golden dragon balls in a cluster
$ballCoords = @(
    @{x=28; y=16},
    @{x=20; y=24}, @{x=36; y=24},
    @{x=14; y=32}, @{x=24; y=32}, @{x=34; y=32}, @{x=44; y=32}
)
foreach ($pt in $ballCoords) {
    $g.FillEllipse($ballBrush, $pt.x, $pt.y, 10, 10)
    $g.FillRectangle($starBrush, $pt.x + 4, $pt.y + 4, 2, 2)
}

# Right: Ki Blast (64x64)
$kiOuter = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 255, 200, 0))
$kiInner = [System.Drawing.Brushes]::White
$g.FillEllipse($kiOuter, 80, 18, 30, 28)
$g.FillEllipse($kiInner, 88, 24, 14, 16)
$g.Dispose()

$itemSlices = @(
    @{ name = "DragonBallAltar_0"; x = 0; y = 0; w = 64; h = 64 },
    @{ name = "KiBlast_0"; x = 64; y = 0; w = 64; h = 64 }
)
Save-PngWithMeta $itemBmp "Assets\Sprites\Items_Sheet.png" $itemSlices 32

# 5. Background.png (1024x512)
$bgBmp = New-Object System.Drawing.Bitmap(1024, 512)
$g = [System.Drawing.Graphics]::FromImage($bgBmp)
$g.Clear([System.Drawing.Color]::FromArgb(255, 30, 75, 60)) # Namek Green Sky

# Distant Namekian turquoise mountains
$brushMtn = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 20, 110, 90))
$g.FillPolygon($brushMtn, @([System.Drawing.Point]::new(0, 360), [System.Drawing.Point]::new(180, 220), [System.Drawing.Point]::new(380, 360)))
$g.FillPolygon($brushMtn, @([System.Drawing.Point]::new(300, 360), [System.Drawing.Point]::new(520, 190), [System.Drawing.Point]::new(740, 360)))
$g.FillPolygon($brushMtn, @([System.Drawing.Point]::new(680, 360), [System.Drawing.Point]::new(880, 230), [System.Drawing.Point]::new(1024, 360)))

# Battle arena ground
$brushGround = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 50, 140, 70))
$g.FillRectangle($brushGround, 0, 350, 1024, 162)
$brushPath = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 140, 120, 80))
$g.FillRectangle($brushPath, 0, 380, 1024, 100)
$g.Dispose()

Save-PngWithMeta $bgBmp "Assets\Sprites\Background.png" $null 100

Write-Output "All 5 Sprite Sheets generated with sliced metadata!"
