# This script is the entry point for the docker image (see Dockerfile).
# It executes the system integration test and uploads the result files to Artifactory.

function RunTests($testOutputDirectoryPath) {
    # We expect exactly one dll which name is like "Pantry.ApplicationName.Tests.SystemIntegration.dll"
    $files = Get-ChildItem -Filter "*.Tests.SystemIntegration.dll"
    $systemIntegrationTestsDll = $files[0].FullName

    dotnet test --logger:"trx;LogFileName=testresults.trx" --results-directory $testOutputDirectoryPath -v minimal $systemIntegrationTestsDll
}

function Main {
    RunTests -testOutputDirectoryPath $Env:TestOutputDirectoryPath
}

Main