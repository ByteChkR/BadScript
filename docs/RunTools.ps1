$preBuildDir = "$pwd/tools/version-helper.bs"
$solutionRoot = "$pwd/../src/"
$consoleProjectPath = "$pwd/../../BadScript.Console/src/BadScript.Console.Desktop/BadScript.Console.Desktop.csproj"

echo "Running Pre Build Scripts"
dotnet run --project $consoleProjectPath $preBuildDir $solutionRoot

pause

