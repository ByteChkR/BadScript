Add-Type -Assembly System.IO.Compression.FileSystem
$compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal

$inWin = "$pwd/../src/BadScript.Console/bin/Release/net5.0/win-x64/publish"
$inLinux = "$pwd/../src/BadScript.Console/bin/Release/net5.0/linux-x64/publish"
$outWin = "$pwd/build-win.zip"
$outLinux = "$pwd/build-linux.zip"

del $inWin -Recurse
del $inLinux -Recurse
del $outWin
del $outLinux

cd ../src
dotnet publish -c Release -r win-x64
dotnet publish -c Release -r linux-x64
dotnet publish -c Release
cd ../docs

[System.IO.Compression.ZipFile]::CreateFromDirectory($inWin, $outWin)
[System.IO.Compression.ZipFile]::CreateFromDirectory($inLinux, $outLinux)