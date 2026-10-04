<#
.SYNOPSIS
Signs release files with the project owner's own code-signing certificate.

.DESCRIPTION
Used by the release workflow and usable by the owner locally. It signs with signtool when the
Windows SDK is installed (the CI runner has it) and falls back to Set-AuthenticodeSignature when it
is not, which is what a developer machine usually looks like.

Every signature is timestamped, because a signature without a timestamp stops being trusted the
moment the certificate expires. If no certificate is configured the script says so and exits 0 -
a tag on a fork, or a build made before a certificate was bought, still has to produce a release.

.EXAMPLE
# The documented local path, after putting your certificate there:
tools\Sign-Release.ps1 -Files dist\installer\RandoFile-Setup-v1.0.0-win-x64.exe

.EXAMPLE
# A certificate that is already installed, for example from a hardware token:
tools\Sign-Release.ps1 -Files dist\win-x64\RandoFile.exe -Thumbprint 0123456789ABCDEF...
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string[]] $Files,

    # The documented path for the owner's own certificate. The certificates folder is git ignored,
    # so a private key can never be committed by accident. Filled in below rather than here:
    # $PSScriptRoot is still empty while parameter defaults are evaluated.
    [string] $CertificatePath,

    [string] $Password = $env:RANDOFILE_CERT_PASSWORD,

    # The workflow passes the .pfx itself, base64 encoded, because a runner has no certificate store
    # to read and no file checked in.
    [string] $Base64Certificate = $env:RANDOFILE_CERT_PFX_BASE64,

    # A certificate that is already installed, such as one a hardware token or a signing client
    # provided. Prefer this one for a publicly trusted certificate: since June 2023 a certificate
    # authority is not allowed to hand out an exportable private key, so there is no .pfx to point at.
    [string] $Thumbprint,

    [string] $TimestampUrl = 'http://timestamp.digicert.com'
)

$ErrorActionPreference = 'Stop'

if (-not $CertificatePath) {
    $CertificatePath = Join-Path (Split-Path -Parent $PSScriptRoot) 'certificates\code-signing.pfx'
}

function Write-Notice([string]$Text) {
    Write-Host $Text

    if ($env:GITHUB_ACTIONS -eq 'true') {
        Write-Host "::warning title=Unsigned release::$Text"
    }
}

function Find-SignTool {
    # The Windows SDK keeps signtool in its own bin folder and never adds it to PATH, so the newest
    # installed version is looked up the way the documentation says it is laid out.
    $roots = @(
        (Join-Path ${env:ProgramFiles(x86)} 'Windows Kits\10\bin'),
        (Join-Path $env:ProgramFiles 'Windows Kits\10\bin')
    )

    foreach ($root in $roots) {
        if (-not (Test-Path -LiteralPath $root)) { continue }

        $found = Get-ChildItem -LiteralPath $root -Filter 'signtool.exe' -Recurse -ErrorAction SilentlyContinue |
            Where-Object { $_.DirectoryName -like '*x64' } |
            Sort-Object FullName -Descending |
            Select-Object -First 1

        if ($found) { return $found.FullName }
    }

    $command = Get-Command 'signtool.exe' -ErrorAction SilentlyContinue
    if ($command) { return $command.Source }

    return ''
}

function Get-PfxCertificateObject([string]$Path, [string]$Secret) {
    # Get-PfxCertificate has no -Password in Windows PowerShell 5.1, and that is the PowerShell the
    # fallback signing path runs in, so the certificate type is loaded directly instead.
    if ($Secret) {
        return New-Object -TypeName System.Security.Cryptography.X509Certificates.X509Certificate2 -ArgumentList @($Path, $Secret)
    }

    return New-Object -TypeName System.Security.Cryptography.X509Certificates.X509Certificate2 -ArgumentList @($Path)
}

# Resolve the files first: a typo has to fail before anything is signed.
$targets = @()
foreach ($file in $Files) {
    if (-not (Test-Path -LiteralPath $file)) { throw "Nothing to sign: '$file' does not exist." }
    $targets += (Resolve-Path -LiteralPath $file).Path
}

$pfxPath = ''
$temporaryPfx = $false
$useThumbprint = $false

if ($Base64Certificate) {
    $pfxPath = Join-Path ([IO.Path]::GetTempPath()) ('randofile-signing-' + [Guid]::NewGuid().ToString('n') + '.pfx')
    [IO.File]::WriteAllBytes($pfxPath, [Convert]::FromBase64String($Base64Certificate))
    $temporaryPfx = $true
}
elseif ($Thumbprint) {
    $useThumbprint = $true
}
elseif (Test-Path -LiteralPath $CertificatePath) {
    $pfxPath = (Resolve-Path -LiteralPath $CertificatePath).Path
}
else {
    Write-Notice ('No code-signing certificate is configured, so the build stays unsigned. See "Code signing" in README.md (the two repository secrets for CI, or a certificate at ' + $CertificatePath + ' locally).')
    exit 0
}

$signTool = Find-SignTool
if ($signTool) {
    Write-Host "signtool: $signTool"
}
else {
    Write-Host 'signtool not found; falling back to Set-AuthenticodeSignature (Windows PowerShell can timestamp with it).'
}

try {
    foreach ($target in $targets) {
        if ($signTool) {
            $arguments = @('sign')

            if ($useThumbprint) {
                $arguments += @('/sha1', $Thumbprint)
            }
            else {
                $arguments += @('/f', $pfxPath)
                if ($Password) { $arguments += @('/p', $Password) }
            }

            $arguments += @('/fd', 'sha256')
            if ($TimestampUrl) { $arguments += @('/td', 'sha256', '/tr', $TimestampUrl) }
            $arguments += $target

            & $signTool $arguments
            if ($LASTEXITCODE -ne 0) { throw "signtool failed for $target with exit code $LASTEXITCODE." }
        }
        else {
            $parameters = @{ FilePath = $target; HashAlgorithm = 'SHA256' }

            if ($useThumbprint) {
                $store = 'Cert:\CurrentUser\My'
                $certificate = Get-Item -Path (Join-Path $store $Thumbprint) -ErrorAction SilentlyContinue
                if (-not $certificate) { throw "No certificate $Thumbprint in $store." }
                $parameters.Certificate = $certificate
            }
            else {
                $parameters.Certificate = Get-PfxCertificateObject -Path $pfxPath -Secret $Password
            }

            if ($TimestampUrl) {
                # PowerShell 6 dropped -TimestampServer, so a machine without the SDK has to run this
                # in Windows PowerShell rather than shipping a signature that expires with the
                # certificate. Saying so is better than silently dropping the timestamp.
                $supportsTimestamp = (Get-Command Set-AuthenticodeSignature).Parameters.ContainsKey('TimestampServer')
                if (-not $supportsTimestamp) {
                    throw 'This PowerShell cannot timestamp a signature. Install the Windows SDK for signtool, or run the script with Windows PowerShell (powershell.exe).'
                }

                $parameters.TimestampServer = $TimestampUrl
            }

            Set-AuthenticodeSignature @parameters | Out-Null
        }

        $signature = Get-AuthenticodeSignature -LiteralPath $target
        if (-not $signature.SignerCertificate) { throw "$target has no signature after signing." }

        if ($TimestampUrl -and -not $signature.TimeStamperCertificate) {
            throw "$target was signed without a timestamp, so the signature would expire with the certificate."
        }

        $stamp = if ($signature.TimeStamperCertificate) { 'timestamped' } else { 'no timestamp' }
        Write-Host ('Signed {0}: {1}, {2}, by {3}' -f (Split-Path -Leaf $target), $signature.Status, $stamp, $signature.SignerCertificate.Subject)
    }
}
finally {
    # The decoded private key never outlives the run.
    if ($temporaryPfx) { Remove-Item -LiteralPath $pfxPath -Force -ErrorAction SilentlyContinue }
}
