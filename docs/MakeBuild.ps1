$compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal

$inRoot = "$pwd/../src/BadScript.Console/bin/Release/net5.0"
$inWin = "$pwd/../src/BadScript.Console/bin/Release/net5.0/win-x64/publish"
$inLinux = "$pwd/../src/BadScript.Console/bin/Release/net5.0/linux-x64/publish"
$outWin = "$pwd/build-win.zip"
$outLinux = "$pwd/build-linux.zip"

del $inRoot -Recurse

$buildProject = {
    param([string]$rid, [string]$docsRoot)
    cd $docsRoot
    cd ../src
    dotnet publish -c Release -r $rid
    cd ../docs
}

$makeZip = {
    param([string]$sourceDir, [string]$destinationFile)
    del $destinationFile
    Add-Type -Assembly System.IO.Compression.FileSystem
    [System.IO.Compression.ZipFile]::CreateFromDirectory($sourceDir, $destinationFile)
}

$debugBuild = {
    param([string]$docsRoot)
    cd $docsRoot
    cd ../src
    dotnet publish -c Release
    cd ../docs
}

echo "Building Windows-x64"
$jW = Start-Job $buildProject -ArgumentList ("win-x64", $pwd) -Name Windows-x64

Wait-Job $jW

echo "Building Linux-x64"
$jL = Start-Job $buildProject -ArgumentList ("linux-x64", $pwd) -Name Linux-x64

Wait-Job $jL

echo "Building Debug Build"
$jD = Start-Job $debugBuild -ArgumentList ($pwd) -Name DebugBuild

Wait-Job $jD

echo "Packing Windows-x64"
Start-Job $makeZip -ArgumentList ($inWin, $outWin) -Name Zip-Windows-x64

echo "Packing Linux-x64"
Start-Job $makeZip -ArgumentList ($inLinux, $outLinux) -Name Zip-Linux-x64

Get-Job | Wait-Job
Get-Job | Receive-Job
Remove-Job *