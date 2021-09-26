$preBuildDir = "$pwd/tools/version-helper.bs"
$solutionRoot = "$pwd/../src/"
$consoleProjectPath = "$pwd/../src/BadScript.Console/BadScript.Console.csproj"

echo "Running Pre Build Scripts"
dotnet run --project $consoleProjectPath run --files $preBuildDir -a $solutionRoot

pause

