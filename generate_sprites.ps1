Add-Type -AssemblyName System.Drawing

function New-GuidString {
    return [System.Guid]::NewGuid().ToString("N")
}

function Write-SpriteMeta {
    param(
        [string]$filePath,
        [int]$pixelsPerUnit = 100,
        [int]$filterMode = 1
    )
    $guid = New-GuidString
    $metaPath = "$filePath.meta"
    if (Test-Path $metaPath) { return }

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
  isReadable: 1
  streamingMipmaps: 0
  streamingMipmapsPriority: 0
  vTOnly: 0
  ignoreMipmapLimit: 0
  grayScaleToAlpha: 0
  generateCubemap: 6
  cubemapConvolution: 0
  seamlessCubemap: 0
  textureFormat: 1
  maxTextureSize: 2048
  textureSettings:
    serializedVersion: 2
    filterMode: $filterMode
    aniso: 1
    mipBias: 0
    wrapU: 1
    wrapV: 1
    wrapW: 1
  nPOTScale: 0
  lightmap: 0
  compressionQuality: 50
  spriteMode: 1
  spriteExtrude: 1
  spriteMeshType: 1
  alignment: 0
  spritePivot: {x: 0.5, y: 0.5}
  spritePixelsToUnits: $pixelsPerUnit
  spriteBorder: {x: 0, y: 0, z: 0, w: 0}
  spriteGenerateFallbackPhysicsShape: 1
  alphaUsage: 1
  alphaIsTransparency: 1
  spriteTessellationMethod: 0
  spriteTessellationDetail: -1
  textureType: 8
  textureShape: 1
  singleDimension: 0
"@
    Set-Content -Path $metaPath -Value $metaContent
}

$dir = "Assets\Sprites"
if (!(Test-Path $dir)) { New-Item -ItemType Directory -Path $dir | Out-Null }

# 1. Goku Super Saiyan (64x64)
$bmp = New-Object System.Drawing.Bitmap 64, 64, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
# Golden Aura
$auraBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(100, 255, 230, 50))
$g.FillEllipse($auraBrush, 4, 4, 56, 56)
# Body & Orange Gi
$giBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 255, 120, 10))
$g.FillEllipse($giBrush, 16, 24, 32, 34)
# Blue Belt & Chest
$blueBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 30, 80, 180))
$g.FillRectangle($blueBrush, 22, 38, 20, 6)
# Face
$skinBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 255, 220, 180))
$g.FillEllipse($skinBrush, 24, 16, 18, 16)
# Golden Spiky Hair
$hairBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 255, 225, 20))
$pts = @(
    New-Object System.Drawing.Point 20, 20
    New-Object System.Drawing.Point 12, 6
    New-Object System.Drawing.Point 24, 12
    New-Object System.Drawing.Point 30, 2
    New-Object System.Drawing.Point 36, 10
    New-Object System.Drawing.Point 46, 4
    New-Object System.Drawing.Point 42, 18
    New-Object System.Drawing.Point 48, 16
    New-Object System.Drawing.Point 42, 24
)
$g.FillPolygon($hairBrush, $pts)
# Hand with Ki Glow
$kiHandBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(220, 255, 255, 120))
$g.FillEllipse($kiHandBrush, 42, 30, 14, 14)
$g.Dispose()
$bmp.Save("$dir\GokuSSJ.png", [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
Write-SpriteMeta "$dir\GokuSSJ.png" 100

# 2. Saibamen Bomber (48x48)
$bmp = New-Object System.Drawing.Bitmap 48, 48, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
# Body green
$skinGreen = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 90, 185, 60))
$g.FillEllipse($skinGreen, 12, 14, 24, 26)
# Big Head
$g.FillEllipse($skinGreen, 14, 6, 20, 18)
# Red Eyes
$redEye = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 240, 20, 20))
$g.FillEllipse($redEye, 17, 12, 5, 5)
$g.FillEllipse($redEye, 26, 12, 5, 5)
# Bomb Core in chest
$coreBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 255, 200, 30))
$g.FillEllipse($coreBrush, 21, 26, 7, 7)
# Claws
$clawPen = New-Object System.Drawing.Pen ([System.Drawing.Color]::FromArgb(255, 50, 120, 30)), 3
$g.DrawLine($clawPen, 12, 22, 4, 30)
$g.DrawLine($clawPen, 36, 22, 44, 30)
$g.Dispose()
$bmp.Save("$dir\Saibamen.png", [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
Write-SpriteMeta "$dir\Saibamen.png" 100

# 3. Red Ribbon Robot (52x52)
$bmp = New-Object System.Drawing.Bitmap 52, 52, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
# Armored Legs
$metalBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 140, 145, 155))
$g.FillRectangle($metalBrush, 14, 38, 8, 10)
$g.FillRectangle($metalBrush, 30, 38, 8, 10)
# Torso
$g.FillRectangle($metalBrush, 12, 14, 28, 24)
# Red Ribbon Chest Plate
$rrRed = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 220, 30, 30))
$g.FillRectangle($rrRed, 18, 20, 16, 12)
# Cyan Visor Eye
$visorBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 40, 230, 255))
$g.FillRectangle($visorBrush, 18, 8, 16, 6)
# Cannon arm
$g.FillRectangle($metalBrush, 2, 20, 10, 6)
$g.Dispose()
$bmp.Save("$dir\RRRobot.png", [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
Write-SpriteMeta "$dir\RRRobot.png" 100

# 4. Red Ribbon Heavy Tank (64x48)
$bmp = New-Object System.Drawing.Bitmap 64, 48, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
# Heavy Treads
$treadBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 50, 50, 60))
$g.FillRectangle($treadBrush, 8, 30, 48, 14)
# Red Armored Hull
$tankRed = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 190, 25, 25))
$g.FillRectangle($tankRed, 12, 16, 40, 16)
# Turret
$g.FillEllipse($metalBrush, 18, 8, 24, 14)
# Long Cannon pointing left
$g.FillRectangle($metalBrush, 2, 12, 18, 6)
$g.Dispose()
$bmp.Save("$dir\RRTank.png", [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
Write-SpriteMeta "$dir\RRTank.png" 100

# 5. Boss Vegeta (60x60)
$bmp = New-Object System.Drawing.Bitmap 60, 60, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
# Saiyan Armor
$whiteArmor = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 240, 240, 240))
$g.FillRectangle($whiteArmor, 18, 24, 24, 20)
# Armor gold straps
$goldStraps = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 230, 180, 20))
$g.FillRectangle($goldStraps, 22, 28, 16, 12)
# Blue bodysuit
$suitBlue = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 20, 50, 140))
$g.FillRectangle($suitBlue, 20, 42, 20, 14)
# Face
$g.FillEllipse($skinBrush, 22, 14, 16, 14)
# Red Scouter
$scouter = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(200, 255, 20, 40))
$g.FillRectangle($scouter, 20, 16, 8, 6)
# Spiky Flame Hair
$blackHair = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 20, 20, 25))
$vHairPts = @(
    New-Object System.Drawing.Point 20, 16
    New-Object System.Drawing.Point 16, 2
    New-Object System.Drawing.Point 26, 8
    New-Object System.Drawing.Point 30, 0
    New-Object System.Drawing.Point 36, 6
    New-Object System.Drawing.Point 44, 2
    New-Object System.Drawing.Point 38, 16
)
$g.FillPolygon($blackHair, $vHairPts)
$g.Dispose()
$bmp.Save("$dir\BossVegeta.png", [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
Write-SpriteMeta "$dir\BossVegeta.png" 100

# 6. Boss Frieza (64x64)
$bmp = New-Object System.Drawing.Bitmap 64, 64, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
# White body
$whiteFrieza = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 245, 245, 255))
$g.FillEllipse($whiteFrieza, 20, 20, 24, 34)
# Head
$g.FillEllipse($whiteFrieza, 22, 8, 20, 18)
# Purple gems on head and chest
$purpleGem = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 150, 20, 190))
$g.FillEllipse($purpleGem, 26, 8, 12, 9)
$g.FillEllipse($purpleGem, 28, 26, 8, 8)
# Red Eyes
$g.FillEllipse($redEye, 25, 16, 4, 3)
$g.FillEllipse($redEye, 33, 16, 4, 3)
# Tail
$tailPen = New-Object System.Drawing.Pen ([System.Drawing.Color]::FromArgb(255, 245, 245, 255)), 6
$g.DrawArc($tailPen, 34, 30, 24, 24, 0, 180)
$g.Dispose()
$bmp.Save("$dir\BossFrieza.png", [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
Write-SpriteMeta "$dir\BossFrieza.png" 100

# 7. Boss Cell (64x64)
$bmp = New-Object System.Drawing.Bitmap 64, 64, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
# Black wings
$wingBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 25, 25, 30))
$g.FillEllipse($wingBrush, 8, 16, 16, 36)
$g.FillEllipse($wingBrush, 40, 16, 16, 36)
# Green Bio body
$cellGreen = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 50, 180, 60))
$g.FillEllipse($cellGreen, 20, 20, 24, 32)
# Head & Crown
$cellHead = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 40, 160, 50))
$g.FillEllipse($cellHead, 22, 10, 20, 18)
# Crown horns
$crownPts = @(
    New-Object System.Drawing.Point 18, 12
    New-Object System.Drawing.Point 14, 0
    New-Object System.Drawing.Point 24, 8
    New-Object System.Drawing.Point 32, 14
    New-Object System.Drawing.Point 40, 8
    New-Object System.Drawing.Point 50, 0
    New-Object System.Drawing.Point 46, 12
)
$g.FillPolygon($wingBrush, $crownPts)
$g.Dispose()
$bmp.Save("$dir\BossCell.png", [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
Write-SpriteMeta "$dir\BossCell.png" 100

# 8. Boss Buu (64x64)
$bmp = New-Object System.Drawing.Bitmap 64, 64, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
# Pink round body
$buuPink = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 255, 140, 175))
$g.FillEllipse($buuPink, 16, 16, 32, 34)
# Head
$g.FillEllipse($buuPink, 20, 8, 24, 20)
# Head Antenna
$antPen = New-Object System.Drawing.Pen ([System.Drawing.Color]::FromArgb(255, 255, 140, 175)), 5
$g.DrawArc($antPen, 26, 0, 16, 16, 180, 180)
# Black Vest
$buuVest = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 20, 20, 20))
$g.FillRectangle($buuVest, 18, 22, 8, 14)
$g.FillRectangle($buuVest, 38, 22, 8, 14)
# White Pants
$whitePants = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 245, 245, 245))
$g.FillEllipse($whitePants, 16, 36, 32, 18)
$g.Dispose()
$bmp.Save("$dir\BossBuu.png", [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
Write-SpriteMeta "$dir\BossBuu.png" 100

# 9. Bulma Capsule Turret (48x48)
$bmp = New-Object System.Drawing.Bitmap 48, 48, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
# Base Stand
$standBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 80, 90, 105))
$g.FillRectangle($standBrush, 12, 34, 24, 8)
# Capsule Dome
$capsuleCyan = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 40, 180, 220))
$g.FillEllipse($capsuleCyan, 12, 12, 24, 24)
# Dual Laser Barrels pointing right
$barrelBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 255, 220, 30))
$g.FillRectangle($barrelBrush, 28, 18, 16, 4)
$g.FillRectangle($barrelBrush, 28, 26, 16, 4)
$g.Dispose()
$bmp.Save("$dir\BulmaTurret.png", [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
Write-SpriteMeta "$dir\BulmaTurret.png" 100

# 10. Vegeta Ally (52x52)
$bmp = New-Object System.Drawing.Bitmap 52, 52, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
# Suit & Armor
$g.FillRectangle($whiteArmor, 16, 20, 20, 16)
$g.FillRectangle($suitBlue, 18, 34, 16, 12)
$g.FillEllipse($skinBrush, 18, 10, 14, 12)
$g.FillPolygon($blackHair, $vHairPts)
# Big Bang Energy Spark in Hand
$bbBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(220, 80, 160, 255))
$g.FillEllipse($bbBrush, 32, 22, 14, 14)
$g.Dispose()
$bmp.Save("$dir\VegetaAlly.png", [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
Write-SpriteMeta "$dir\VegetaAlly.png" 100

# 11. Piccolo Ally (52x52)
$bmp = New-Object System.Drawing.Bitmap 52, 52, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
# Purple Gi
$picGi = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 110, 35, 145))
$g.FillRectangle($picGi, 16, 22, 20, 24)
# White Cape
$whiteCape = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 245, 245, 250))
$g.FillRectangle($whiteCape, 12, 18, 28, 8)
# Green Face
$picGreen = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 75, 190, 70))
$g.FillEllipse($picGreen, 18, 10, 16, 12)
# White Turban
$g.FillEllipse($whiteCape, 16, 4, 20, 10)
# Makankosappo charging fingers
$beamSpark = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 255, 240, 50))
$g.FillEllipse($beamSpark, 30, 8, 8, 8)
$g.Dispose()
$bmp.Save("$dir\PiccoloAlly.png", [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
Write-SpriteMeta "$dir\PiccoloAlly.png" 100

# 12. Ki Blast (32x32)
$bmp = New-Object System.Drawing.Bitmap 32, 32, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$outerKi = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(120, 255, 150, 20))
$g.FillEllipse($outerKi, 2, 6, 28, 20)
$midKi = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(220, 255, 220, 30))
$g.FillEllipse($midKi, 6, 8, 20, 16)
$coreKi = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 255, 255, 240))
$g.FillEllipse($coreKi, 12, 11, 10, 10)
$g.Dispose()
$bmp.Save("$dir\KiBlast.png", [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
Write-SpriteMeta "$dir\KiBlast.png" 100

# 13. Kamehameha Beam (128x32)
$bmp = New-Object System.Drawing.Bitmap 128, 32, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$outerBeam = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(140, 20, 120, 255))
$g.FillRectangle($outerBeam, 0, 2, 128, 28)
$midBeam = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(220, 50, 220, 255))
$g.FillRectangle($midBeam, 0, 6, 128, 20)
$coreBeam = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 255, 255, 255))
$g.FillRectangle($coreBeam, 0, 11, 128, 10)
$g.Dispose()
$bmp.Save("$dir\KamehamehaBeam.png", [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
Write-SpriteMeta "$dir\KamehamehaBeam.png" 100

# 14. Spirit Bomb (64x64)
$bmp = New-Object System.Drawing.Bitmap 64, 64, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$outerSphere = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(90, 40, 150, 255))
$g.FillEllipse($outerSphere, 2, 2, 60, 60)
$midSphere = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(200, 60, 220, 255))
$g.FillEllipse($midSphere, 8, 8, 48, 48)
$coreSphere = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 240, 255, 255))
$g.FillEllipse($coreSphere, 16, 16, 32, 32)
$g.Dispose()
$bmp.Save("$dir\SpiritBomb.png", [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
Write-SpriteMeta "$dir\SpiritBomb.png" 100

# 15. Zeni Coin (32x32)
$bmp = New-Object System.Drawing.Bitmap 32, 32, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$goldOuter = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 215, 160, 10))
$g.FillEllipse($goldOuter, 2, 2, 28, 28)
$goldInner = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 255, 225, 40))
$g.FillEllipse($goldInner, 5, 5, 22, 22)
# 'Z' letter with lines
$zPen = New-Object System.Drawing.Pen ([System.Drawing.Color]::FromArgb(255, 180, 120, 10)), 3
$zPts = @(
    New-Object System.Drawing.Point 10, 10
    New-Object System.Drawing.Point 22, 10
    New-Object System.Drawing.Point 10, 22
    New-Object System.Drawing.Point 22, 22
)
$g.DrawLines($zPen, $zPts)
$g.Dispose()
$bmp.Save("$dir\ZeniCoin.png", [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
Write-SpriteMeta "$dir\ZeniCoin.png" 100

# 16. Senzu Bean (32x32)
$bmp = New-Object System.Drawing.Bitmap 32, 32, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$pouchBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 180, 140, 90))
$g.FillEllipse($pouchBrush, 6, 10, 20, 18)
$senzuGreen = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 50, 210, 80))
$g.FillEllipse($senzuGreen, 10, 6, 12, 10)
$g.Dispose()
$bmp.Save("$dir\SenzuBean.png", [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
Write-SpriteMeta "$dir\SenzuBean.png" 100

# 17. Ki Orb (32x32)
$bmp = New-Object System.Drawing.Bitmap 32, 32, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$orbOuter = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(120, 30, 160, 255))
$g.FillEllipse($orbOuter, 3, 3, 26, 26)
$orbInner = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(220, 100, 230, 255))
$g.FillEllipse($orbInner, 7, 7, 18, 18)
$orbCore = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 255, 255, 255))
$g.FillEllipse($orbCore, 12, 12, 8, 8)
$g.Dispose()
$bmp.Save("$dir\KiOrb.png", [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
Write-SpriteMeta "$dir\KiOrb.png" 100

# Also write metas for Background.jpg, DragonBallAltar.png, Goku.png
Write-SpriteMeta "$dir\Background.jpg" 100
Write-SpriteMeta "$dir\DragonBallAltar.png" 100
Write-SpriteMeta "$dir\Goku.png" 100

Write-Output "All 17 sprites and metadata successfully generated!"
