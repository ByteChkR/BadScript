Add-Type -Assembly System.IO.Compression.FileSystem
$compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal

$in = "$pwd/../src/BadScript.Console/bin/Release/net5.0/publish"
$out = "$pwd/build.zip"

del $in -Recurse
del $out

cd ../src
dotnet publish -c Release
cd ../docs

[System.IO.Compression.ZipFile]::CreateFromDirectory($in, $out)