$compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal

$inRoot = "$pwd/../src/BadScript.Console/bin/Release/net5.0"
$inWin = "$pwd/../src/BadScript.Console/bin/Release/net5.0/win-x64/publish"
$inLinux = "$pwd/../src/BadScript.Console/bin/Release/net5.0/linux-x64/publish"
$outWin = "$pwd/build-win.zip"
$outLinux = "$pwd/build-linux.zip"

del $inRoot -Recurse

$makeZip = {
    param([string]$sourceDir, [string]$destinationFile, [string]$rid, [string]$docsRoot)
    del $destinationFile
    cd $docsRoot
    cd ../src
    dotnet publish -c Release -r $rid
    cd ../docs

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

Start-Job $makeZip -ArgumentList ($inWin, $outWin, "win-x64", $pwd) -Name Windows-x64
Start-Job $makeZip -ArgumentList ($inLinux, $outLinux, "linux-x64", $pwd) -Name Linux-x64
Start-Job $debugBuild -ArgumentList ($pwd) -Name DebugBuild


While (Get-Job -State "Running") { Start-Sleep 2 }
Get-Job | Receive-Job
Remove-Job *