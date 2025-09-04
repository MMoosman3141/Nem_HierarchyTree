$coverageFound = (dotnet tool list -g dotnet-coverage | Where-Object {$_.Split(' ')[0] -eq "dotnet-coverage"})
$generatorFound = (dotnet tool list -g dotnet-reportgenerator-globaltool | Where-Object {$_.Split(' ')[0] -eq "dotnet-reportgenerator-globaltool"})

if($null -eq $coverageFound) {
    dotnet tool install -g dotnet-coverage
}
if($null -eq $generatorFound) {
    dotnet tool install -g dotnet-reportgenerator-globaltool
}

$coverageFile = ".\bin\coverage.xml"
$projectName = "Nem_HierarchyTree"
$targetDir = ".\bin\coverageReport"

dotnet-coverage collect -f xml -o "${coverageFile}" dotnet test ${projectName}.sln
reportgenerator -reports:"${coverageFile}" -targetDir:"${targetDir}" -assemblyfilters:+${projectName}.dll

Invoke-Item "${targetDir}/index.html"