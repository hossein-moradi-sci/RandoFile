param(
  [string]$OutputFolder = "D:\projects\image randomizer\src\RandoFile.App\Assets\Brand"
)

# Generates the RandoFile brand mark: three document cards swept into a fan, with a ring segment
# behind them standing for the randomising motion. The mark is deliberately free standing rather
# than a generic squircle tile, so its silhouette is recognisable on its own.
#
# Run it whenever the mark changes, then commit the produced PNG/ICO. The splash screen does not
# use these files: it rebuilds the same shapes as vectors so it can animate the parts separately.
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing

New-Item -ItemType Directory -Force -Path $OutputFolder | Out-Null

$HeroFrom = [System.Drawing.Color]::FromArgb(255, 64, 132, 255)
$HeroTo = [System.Drawing.Color]::FromArgb(255, 124, 77, 255)
$LeftTint = [System.Drawing.Color]::FromArgb(255, 133, 176, 255)
$RightTint = [System.Drawing.Color]::FromArgb(255, 176, 150, 255)
$RingTint = [System.Drawing.Color]::FromArgb(96, 96, 132, 255)

function New-RoundedPath([single]$x, [single]$y, [single]$w, [single]$h, [single]$r) {
  $d = $r * 2
  $p = [System.Drawing.Drawing2D.GraphicsPath]::new()
  $p.AddArc($x, $y, $d, $d, 180, 90)
  $p.AddArc($x + $w - $d, $y, $d, $d, 270, 90)
  $p.AddArc($x + $w - $d, $y + $h - $d, $d, $d, 0, 90)
  $p.AddArc($x, $y + $h - $d, $d, $d, 90, 90)
  $p.CloseFigure()
  return $p
}

# A document card: a rounded rectangle whose top right corner is folded over.
function New-CardPath([single]$x, [single]$y, [single]$w, [single]$h, [single]$r, [single]$fold) {
  $d = $r * 2
  $p = [System.Drawing.Drawing2D.GraphicsPath]::new()
  $p.AddArc($x, $y, $d, $d, 180, 90)
  $p.AddLine([System.Drawing.PointF]::new($x + $w - $fold, $y), [System.Drawing.PointF]::new($x + $w, $y + $fold))
  $p.AddLine([System.Drawing.PointF]::new($x + $w, $y + $fold), [System.Drawing.PointF]::new($x + $w, $y + $h - $r))
  $p.AddArc($x + $w - $d, $y + $h - $d, $d, $d, 0, 90)
  $p.AddLine([System.Drawing.PointF]::new($x + $w - $r, $y + $h), [System.Drawing.PointF]::new($x + $r, $y + $h))
  $p.AddArc($x, $y + $h - $d, $d, $d, 270, 90)
  $p.CloseFigure()
  return $p
}

function Add-Card($g, [single]$size, [single]$cx, [single]$cy, [single]$w, [single]$h,
                  [single]$angle, $from, $to, [bool]$folded) {
  $r = $size * 0.030
  $fold = $w * 0.30

  $state = $g.Save()
  $g.TranslateTransform($cx, $cy)
  $g.RotateTransform($angle)
  $g.TranslateTransform(-$cx, -$cy)

  $path = New-CardPath ($cx - $w / 2) ($cy - $h / 2) $w $h $r $fold
  $fill = [System.Drawing.Drawing2D.LinearGradientBrush]::new(
    [System.Drawing.PointF]::new($cx - $w / 2, $cy - $h / 2),
    [System.Drawing.PointF]::new($cx + $w / 2, $cy + $h / 2),
    $from, $to)
  $g.FillPath($fill, $path)

  # A hairline in a darker shade of the same hue keeps the cards apart where they overlap.
  $edge = [System.Drawing.Color]::FromArgb(
    $from.A,
    [math]::Max(0, $from.R - 40), [math]::Max(0, $from.G - 40), [math]::Max(0, $from.B - 40))
  $g.DrawPath([System.Drawing.Pen]::new($edge, $size * 0.006), $path)

  if ($folded) {
    $foldPath = [System.Drawing.Drawing2D.GraphicsPath]::new()
    $foldPath.AddPolygon([System.Drawing.PointF[]]@(
      [System.Drawing.PointF]::new($cx + $w / 2 - $fold, $cy - $h / 2),
      [System.Drawing.PointF]::new($cx + $w / 2, $cy - $h / 2 + $fold),
      [System.Drawing.PointF]::new($cx + $w / 2 - $fold, $cy - $h / 2 + $fold)))
    $light = [System.Drawing.Color]::FromArgb(
      [math]::Min(255, $from.A + 60),
      [math]::Min(255, $from.R + 70), [math]::Min(255, $from.G + 70), [math]::Min(255, $from.B + 70))
    $g.FillPath([System.Drawing.SolidBrush]::new($light), $foldPath)
  }

  $g.Restore($state)
}

function Add-ArrowHead($g, [single]$tipX, [single]$tipY, [single]$angle, [single]$size, $brush) {
  $state = $g.Save()
  $g.TranslateTransform($tipX, $tipY)
  $g.RotateTransform($angle)
  $head = [System.Drawing.Drawing2D.GraphicsPath]::new()
  $head.AddPolygon([System.Drawing.PointF[]]@(
    [System.Drawing.PointF]::new(0, 0),
    [System.Drawing.PointF]::new(-$size, -$size * 0.60),
    [System.Drawing.PointF]::new(-$size * 0.58, 0),
    [System.Drawing.PointF]::new(-$size, $size * 0.60)))
  $g.FillPath($brush, $head)
  $g.Restore($state)
}

# <param name="plate">Draws the dark rounded plate that gives the Windows icon a solid silhouette.</param>
function New-Fan([int]$size, [bool]$plate, [bool]$detail) {
  $bmp = [System.Drawing.Bitmap]::new($size, $size, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
  $g = [System.Drawing.Graphics]::FromImage($bmp)
  $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
  $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
  $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
  $g.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
  $g.Clear([System.Drawing.Color]::Transparent)

  if ($plate) {
    $p = $size * 0.97
    $o = ($size - $p) / 2
    $platePath = New-RoundedPath $o $o $p $p ($p * 0.235)
    $plateBrush = [System.Drawing.Drawing2D.LinearGradientBrush]::new(
      [System.Drawing.PointF]::new($o, $o),
      [System.Drawing.PointF]::new($o + $p, $o + $p),
      [System.Drawing.Color]::FromArgb(255, 22, 27, 41),
      [System.Drawing.Color]::FromArgb(255, 12, 15, 24))
    $g.FillPath($plateBrush, $platePath)
    $g.DrawPath([System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(255, 44, 52, 76), $size * 0.006), $platePath)
  }

  # Ring segment, swept across the top of the fan to stand for the randomising motion.
  if ($detail) {
    $r = $size * 0.450
    $ring = [System.Drawing.Pen]::new($RingTint, $size * 0.048)
    $ring.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
    $ring.EndCap = [System.Drawing.Drawing2D.LineCap]::Round
    $g.DrawArc($ring, ($size / 2) - $r, ($size / 2) - $r, $r * 2, $r * 2, 215, 105)
    $end = 320 * [math]::PI / 180
    Add-ArrowHead $g (($size / 2) + ($r * [math]::Cos($end))) (($size / 2) + ($r * [math]::Sin($end))) 50 ($size * 0.082) `
      ([System.Drawing.SolidBrush]::new($RingTint))
  }

  # The two side cards first so the hero card overlaps them.
  Add-Card $g $size ($size * 0.355) ($size * 0.530) ($size * 0.380) ($size * 0.470) -26 `
    $LeftTint ([System.Drawing.Color]::FromArgb(255, 106, 150, 255)) $detail
  Add-Card $g $size ($size * 0.645) ($size * 0.530) ($size * 0.380) ($size * 0.470) 26 `
    $RightTint ([System.Drawing.Color]::FromArgb(255, 150, 122, 255)) $detail
  Add-Card $g $size ($size * 0.500) ($size * 0.500) ($size * 0.420) ($size * 0.540) 0 `
    $HeroFrom $HeroTo $detail

  $g.Dispose()
  return $bmp
}

function Save-Png($bitmap, [string]$path) {
  $bitmap.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
  Write-Host ("  " + (Split-Path $path -Leaf).PadRight(16) + (Get-Item $path).Length + " bytes")
}

Write-Host "Brand assets:"

# The README master and the interface mark carry the full artwork.
$readme = New-Fan 512 $true $true
Save-Png $readme (Join-Path $OutputFolder "logo.png")
$readme.Dispose()

$mark = New-Fan 512 $false $true
Save-Png $mark (Join-Path $OutputFolder "logo-mark.png")
$mark.Dispose()

# The Windows icon sits on a plate so it keeps a solid silhouette on any taskbar, and loses the
# fine detail below 48 px where it would only turn to mush.
$sizes = @(16, 24, 32, 48, 64, 128, 256)
$payloads = New-Object System.Collections.Generic.List[byte[]]
foreach ($size in $sizes) {
  $b = New-Fan $size $true ($size -ge 48)
  $ms = [System.IO.MemoryStream]::new()
  $b.Save($ms, [System.Drawing.Imaging.ImageFormat]::Png)
  $b.Dispose()
  $payloads.Add($ms.ToArray())
  $ms.Dispose()
}

$icoPath = Join-Path $OutputFolder "randofile.ico"
$stream = [System.IO.MemoryStream]::new()
$writer = [System.IO.BinaryWriter]::new($stream)
$writer.Write([UInt16]0)
$writer.Write([UInt16]1)
$writer.Write([UInt16]$sizes.Count)

[int]$offset = 6 + (16 * $sizes.Count)
for ($i = 0; $i -lt $sizes.Count; $i++) {
  [byte]$dim = if ($sizes[$i] -ge 256) { 0 } else { [byte]$sizes[$i] }
  $writer.Write($dim)
  $writer.Write($dim)
  $writer.Write([byte]0)
  $writer.Write([byte]0)
  $writer.Write([UInt16]1)
  $writer.Write([UInt16]32)
  $writer.Write([UInt32]$payloads[$i].Length)
  $writer.Write([UInt32]$offset)
  $offset += $payloads[$i].Length
}
foreach ($p in $payloads) { $writer.Write($p) }
$writer.Flush()
[System.IO.File]::WriteAllBytes($icoPath, $stream.ToArray())
$writer.Dispose()
$stream.Dispose()

Write-Host ("  " + "randofile.ico".PadRight(16) + (Get-Item $icoPath).Length + " bytes (" + $sizes.Count + " sizes)")
Write-Host "Done."
