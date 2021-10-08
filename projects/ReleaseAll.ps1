$dir = Get-ChildItem ./ | ? {$_.PSIsContainer}

$dir | ForEach-Object {
    cd $_.FullName
    bs project make -t release
}