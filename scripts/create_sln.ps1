#
# create sln
#

Param(
    [Parameter(Mandatory=$true,HelpMessage="Azure Active Directory's Application(client) ID")]
    [string]$AppId
    )

Set-StrictMode -Version 3.0
$OutputEncoding = [System.Text.Encoding]::GetEncoding('shift_jis')
$ErrorActionPreference = "Stop"

$OwnDir    = Split-Path -Parent $MyInvocation.MyCommand.Path
$BaseDir   = Split-Path -Parent $OwnDir
$SrcDir    = Join-Path $BaseDir "src"

function main {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory=$true,HelpMessage="Azure Active Directory's Application(client) ID")]
        [string]$appId
    )

    mkdir $SrcDir

    Push-Location $SrcDir

        dotnet new sln --name permafnotes

        dotnet new blazorwasm -au SingleOrg --client-id $appId --tenant-id "common" -o permafnotes

        Push-Location permafnotes

            dotnet add package Microsoft.Graph
            dotnet add package PublishSPAforGitHubPages.Build

        Pop-Location

        dotnet sln  add   permafnotes/permafnotes.csproj

    Pop-Location
}


if (( Resolve-Path -Path $MyInvocation.InvocationName ).ProviderPath -eq $MyInvocation.MyCommand.Path ) {
    main $AppId
}