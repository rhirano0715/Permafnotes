#
# APISETTINGS__BASEURI
# AZUREAD__AUTHORITY
# AZUREAD__CLIENTID
# AZUREAD__VALIDATEAUTHORITY
#

Param(
    [Parameter(Mandatory=$true)]
    [string]$AzureAdAuthority,
    [Parameter(Mandatory=$true)]
    [string]$AzureAdClientId,
    [Parameter(Mandatory=$true)]
    [string]$AzureAdValidateAuthority,
    [Parameter(Mandatory=$true)]
    [string]$ApiSettingsBaseUri,
    [Parameter(Mandatory=$true)]
    [string]$OutputPath
)

Set-StrictMode -Version 3.0
$OutputEncoding = [System.Text.Encoding]::GetEncoding('shift_jis')
$ErrorActionPreference = "Stop"

[bool]$validate = $false
if ($AzureAdValidateAuthority -eq "true") {
    $validate = $true
}

$Settings = @{
    "AzureAd" = @{
        "Authority" = $AzureAdAuthority
        "ClientId" = $AzureAdClientId
        "ValidateAuthority" = $validate
    }
    "ApiSettings" = @{
        "BaseUri" = $ApiSettingsBaseUri
    }
}

ConvertTo-Json $Settings | Out-File -FilePath $OutputPath -Encoding utf8
