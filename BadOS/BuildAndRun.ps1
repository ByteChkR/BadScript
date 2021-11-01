cd BadShell
. bs project make -t release
cd ../BadOS.App
. bs project make -t release
cd ..
. bs app .\BadOS.App\bin\release\BadOS.App0.0.0.1.bsapp -s
