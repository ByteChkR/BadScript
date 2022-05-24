$bs = "$pwd/../build/"
$bsFile = $bs + 'bs.exe'

if (!(Test-Path $bsFile)) {
  Write-Warning "$bsFile File is was not found.. Rebuilding"
  cd ../src
  dotnet build -c Release
  cd ../docs
  clear
}
$bs = (Get-Item $bs).fullname

$Env:Path += ";$bs"
echo "Bad Script Executable Directory: $bs"
echo "Run Bad Script: 'bs <script-path>'"