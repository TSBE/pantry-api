Push-Location $PSScriptRoot
try {
    dotnet restore ..\Pantry.sln
    dotnet build ..\Pantry.sln --no-restore --configuration release
    dotnet test ..\Pantry.sln --no-build --no-restore --configuration release --logger:trx -v minimal --filter Category!=SystemIntegration /p:CollectCoverage=true '/p:ExcludeByAttribute=\"GeneratedCodeAttribute\"' /p:Exclude=[*]*.Migrations.* /p:CoverletOutputFormat=opencover
    dotnet tool update -g dotnet-reportgenerator-globaltool
    reportgenerator "-reports:..\tests\**\*.opencover.xml" "-targetdir:coveragereport" -reporttypes:HTMLInline

    . .\coveragereport\index.htm
}
finally {
    Pop-Location
}
