$bs = "$pwd/../src/BadScript.Console/bin/Release/net5.0/"


if (!(Test-Path $bs)) {
  Write-Warning "$bs Folder is was not found.. Rebuilding"
  cd ../src
  dotnet build -c Release
  cd ../docs
  clear
}
$bs = (Get-Item $bs).fullname

$Env:Path += ";$bs"
echo "Bad Script Executable Directory: $bs"
echo "Run Bad Script: 'bs <script-path>'"