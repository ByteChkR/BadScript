echo "Clear Build Directory"
Remove-Item "./build" -Recurse -ErrorAction Ignore
mkdir "./build"

echo "Building Console"
dotnet publish -c Release "./src/BadScript.Console"
Copy-Item -Path "./src/BadScript.Console/bin/Release/net6.0/publish/*" -Destination "./build" -Recurse

echo "Creating Plugin Folders"
mkdir "./build/data/plugins"
mkdir "./build/data/plugins/Http"
mkdir "./build/data/plugins/Utils"
mkdir "./build/data/plugins/Platform"
mkdir "./build/data/plugins/Serialization"
mkdir "./build/data/plugins/Serialization/Drawing"

echo "Building Plugins"
dotnet publish -c Release "./src/BadScript.ConsoleUtils"
cp "src/BadScript.ConsoleUtils/bin/Release/netstandard2.0/publish/BadScript.ConsoleUtils.dll" "./build/data/plugins/Platform/"

dotnet publish -c Release "./src/BadScript.Http"
cp "src/BadScript.Http/bin/Release/netstandard2.0/publish/BadScript.Http.dll" "./build/data/plugins/Http/"

dotnet publish -c Release "./src/BadScript.HttpServer"
cp "src/BadScript.HttpServer/bin/Release/net6.0/publish/BadScript.HttpServer.dll" "./build/data/plugins/Http/"
cp "src/BadScript.HttpServer/bin/Release/net6.0/publish/Ceen.Httpd.dll" "./build/data/plugins/Http/"
cp "src/BadScript.HttpServer/bin/Release/net6.0/publish/Ceen.Common.dll" "./build/data/plugins/Http/"
cp "src/BadScript.HttpServer/bin/Release/net6.0/publish/OpenSSL.PrivateKeyDecoder.dll" "./build/data/plugins/Http/"
cp "src/BadScript.HttpServer/bin/Release/net6.0/publish/OpenSSL.X509Certificate2.Provider.dll" "./build/data/plugins/Http/"

dotnet publish -c Release "./src/BadScript.Imaging"
cp "src/BadScript.Imaging/bin/Release/netstandard2.0/publish/BadScript.Imaging.dll" "./build/data/plugins/Serialization/Drawing/"
cp "src/BadScript.Imaging/bin/Release/netstandard2.0/publish/System.Drawing.Common.dll" "./build/data/plugins/Serialization/Drawing/"

dotnet publish -c Release "./src/BadScript.IO"
cp "src/BadScript.IO/bin/Release/netstandard2.0/publish/BadScript.IO.dll" "./build/data/plugins/Platform/"

dotnet publish -c Release "./src/BadScript.Json"
cp "src/BadScript.Json/bin/Release/netstandard2.0/publish/BadScript.Json.dll" "./build/data/plugins/Serialization/"

dotnet publish -c Release "./src/BadScript.Math"
cp "src/BadScript.Math/bin/Release/netstandard2.0/publish/BadScript.Math.dll" "./build/data/plugins/Utils/"

dotnet publish -c Release "./src/BadScript.Process"
cp "src/BadScript.Process/bin/Release/netstandard2.0/publish/BadScript.Process.dll" "./build/data/plugins/Platform/"

dotnet publish -c Release "./src/BadScript.StringUtils"
cp "src/BadScript.StringUtils/bin/Release/netstandard2.0/publish/BadScript.StringUtils.dll" "./build/data/plugins/Utils/"

dotnet publish -c Release "./src/BadScript.Threading"
cp "src/BadScript.Threading/bin/Release/netstandard2.0/publish/BadScript.Threading.dll" "./build/data/plugins/Platform/"

dotnet publish -c Release "./src/BadScript.Xml"
cp "src/BadScript.Xml/bin/Release/netstandard2.0/publish/BadScript.Xml.dll" "./build/data/plugins/Serialization/"

dotnet publish -c Release "./src/BadScript.Zip"
cp "src/BadScript.Zip/bin/Release/netstandard2.0/publish/BadScript.Zip.dll" "./build/data/plugins/Serialization/"