Add-Type -AssemblyName System.Drawing

$outputPath = "c:\Users\thota.jayadev\source\repos\BUSecurityHierarchy\src\BUSecurityHierarchy\Resources\icon.png"

$size = 128
$bmp = New-Object System.Drawing.Bitmap($size, $size)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$g.Clear([System.Drawing.Color]::Transparent)

# ── Rounded-rectangle background with gradient ──
$bgPath = New-Object System.Drawing.Drawing2D.GraphicsPath
$cr = 18
$bgPath.AddArc(0, 0, $cr, $cr, 180, 90)
$bgPath.AddArc($size - $cr - 1, 0, $cr, $cr, 270, 90)
$bgPath.AddArc($size - $cr - 1, $size - $cr - 1, $cr, $cr, 0, 90)
$bgPath.AddArc(0, $size - $cr - 1, $cr, $cr, 90, 90)
$bgPath.CloseFigure()

$bgBrush = New-Object System.Drawing.Drawing2D.LinearGradientBrush(
    [System.Drawing.PointF]::new(0, 0),
    [System.Drawing.PointF]::new(0, $size),
    [System.Drawing.Color]::FromArgb(33, 100, 210),
    [System.Drawing.Color]::FromArgb(14, 55, 135)
)
$g.FillPath($bgBrush, $bgPath)

# ── Brushes & Pens ──
$white    = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::White)
$midBlue  = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(130, 195, 255))
$paleBlue = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(195, 225, 255))
$linePen  = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(180, 200, 225, 255), 2)

# ── Helper: draw a person silhouette (head + shoulders) ──
function Draw-Person([int]$cx, [int]$topY, [int]$r, $brush) {
    # Head circle
    $script:g.FillEllipse($brush, ($cx - $r), $topY, ($r * 2), ($r * 2))
    # Shoulders / body — top half of a wider ellipse
    [int]$bw = $r * 2.4          # half-width of body ellipse
    [int]$bh = $r * 1.6          # half-height of body ellipse
    [int]$by = $topY + $r * 2 + 2
    $script:g.FillPie($brush, ($cx - $bw), $by, ($bw * 2), ($bh * 2), 180, 180)
}

# ══════════  LEVEL 1 — Business Unit (root)  ══════════
Draw-Person 64 10 8 $white
#   head 10-26 · body 28-40

# Connector L1 ─► L2
$g.DrawLine($linePen, 64, 42, 64, 47)     # vertical trunk
$g.DrawLine($linePen, 36, 47, 92, 47)     # horizontal bar
$g.DrawLine($linePen, 36, 47, 36, 52)     # drop-left
$g.DrawLine($linePen, 92, 47, 92, 52)     # drop-right

# ══════════  LEVEL 2 — Teams  ══════════
Draw-Person 36 52 6 $midBlue
Draw-Person 92 52 6 $midBlue
#   head 52-64 · body 66-75

# Connector L2-left ─► L3
$g.DrawLine($linePen, 36, 77, 36, 81)
$g.DrawLine($linePen, 20, 81, 52, 81)
$g.DrawLine($linePen, 20, 81, 20, 85)
$g.DrawLine($linePen, 52, 81, 52, 85)

# Connector L2-right ─► L3
$g.DrawLine($linePen, 92, 77, 92, 81)
$g.DrawLine($linePen, 76, 81, 108, 81)
$g.DrawLine($linePen, 76, 81, 76, 85)
$g.DrawLine($linePen, 108, 81, 108, 85)

# ══════════  LEVEL 3 — Users  ══════════
Draw-Person 20  85 5 $paleBlue
Draw-Person 52  85 5 $paleBlue
Draw-Person 76  85 5 $paleBlue
Draw-Person 108 85 5 $paleBlue
#   head 85-95 · body 97-105

# ── Cleanup & save ──
$linePen.Dispose(); $white.Dispose(); $midBlue.Dispose(); $paleBlue.Dispose()
$bgBrush.Dispose(); $bgPath.Dispose(); $g.Dispose()
$bmp.Save($outputPath, [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()

Write-Host "Icon saved to $outputPath" -ForegroundColor Green
