Add-Type -AssemblyName System.Drawing

$sourcePath = "C:\Users\7lmiq\Desktop\app#\RetailApp\Assets\icon.png"
$targetPath = "C:\Users\7lmiq\Desktop\app#\RetailApp\Assets\icon.ico"

# Load the image
$originalImage = [System.Drawing.Image]::FromFile($sourcePath)

# Resize to 256x256
$width = 256
$height = 256
$resizedImage = New-Object System.Drawing.Bitmap($width, $height)
$graphics = [System.Drawing.Graphics]::FromImage($resizedImage)
$graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
$graphics.DrawImage($originalImage, 0, 0, $width, $height)
$graphics.Dispose()

# Save resized image to memory as PNG
$ms = New-Object System.IO.MemoryStream
$resizedImage.Save($ms, [System.Drawing.Imaging.ImageFormat]::Png)
$ms.Position = 0
$bytes = $ms.ToArray()

# Write ICO file
$iconFile = New-Object System.IO.FileStream($targetPath, [System.IO.FileMode]::Create)
$iconWriter = New-Object System.IO.BinaryWriter($iconFile)

# ICO header
$iconWriter.Write([Int16]0) # Reserved
$iconWriter.Write([Int16]1) # Type 1 = ICO
$iconWriter.Write([Int16]1) # 1 image

# Directory Entry
$iconWriter.Write([byte]0) # Width 0 = 256
$iconWriter.Write([byte]0) # Height 0 = 256
$iconWriter.Write([byte]0) # Color count
$iconWriter.Write([byte]0) # Reserved
$iconWriter.Write([Int16]1) # Planes
$iconWriter.Write([Int16]32) # BitCount
$iconWriter.Write([int]$bytes.Length) # SizeInBytes
$iconWriter.Write([int]22) # Offset

# Image Data
$iconWriter.Write($bytes)

$iconWriter.Flush()
$iconWriter.Close()
$originalImage.Dispose()
$resizedImage.Dispose()
$ms.Dispose()

Write-Output "ICO file created successfully."
