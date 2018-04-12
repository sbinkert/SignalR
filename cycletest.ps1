& "$PSScriptRoot\build.ps1" /t:Restore
cd "$PSScriptRoot\test\Microsoft.AspNetCore.SignalR.Client.Tests"

dotnet build

1..10 | foreach { 
    dotnet test --no-build --filter CorrectlyHandlesQueryStringWhenAppendingNegotiateToUrl --framework netcoreapp2.1
}