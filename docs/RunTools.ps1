$preBuildDir = "$pwd/tools/"
$solutionRoot = "$pwd/../src/"
$consoleProjectPath = "$pwd/../src/BadScript.Console/BadScript.Console.csproj"

echo "Running Pre Build Scripts"
dotnet run --project $consoleProjectPath $preBuildDir $solutionRoot

pause

