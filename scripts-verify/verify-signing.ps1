# Smoke test for tools/Sign-Release.ps1: proves it can sign a real executable with a real
# certificate, and that the documented local certificate path works.
#
# It creates a throwaway self-signed code-signing certificate in the current user's store, exports it
# to certificates\code-signing.pfx (the path the README documents, and one git ignores), signs a
# copy of the published executable with it, checks the signature and its timestamp, then removes the
# certificate, the .pfx and the copy again. The published file itself is never touched.
#
# It needs internet access for the timestamp server, and it runs in Windows PowerShell: PowerShell 6
# dropped the timestamp parameter the fallback signer uses.
#
# Run it from anywhere:  powershell -NoProfile -ExecutionPolicy Bypass -File scripts-verify\verify-signing.ps1
# ASCII only, because PowerShell 5.1 reads a .ps1 as ANSI unless the file has a BOM.

$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $PSScriptRoot
$signScript = Join-Path $root 'tools\Sign-Release.ps1'
$source = Join-Path $root 'dist\win-x64\RandoFile.exe'
$certificateFolder = Join-Path $root 'certificates'
$pfx = Join-Path $certificateFolder 'code-signing.pfx'
$password = 'randofile-signing-smoke-test'
$log = Join-Path $PSScriptRoot 'verify-signing.log'

$work = Join-Path $env:TEMP ('randofile-signing-test-' + [Guid]::NewGuid().ToString('n'))
$copy = Join-Path $work 'RandoFile.exe'

Remove-Item -LiteralPath $log -Force -ErrorAction SilentlyContinue

$script:checks = 0
$script:failures = 0

function Say([string]$text) {
    Write-Host $text
    Add-Content -LiteralPath $log -Value $text -Encoding UTF8
}

function Check([string]$label, [bool]$ok) {
    $script:checks++
    if (-not $ok) { $script:failures++ }
    $mark = if ($ok) { '[PASS]' } else { '[FAIL]' }
    Say ('{0} {1}' -f $mark, $label)
}

# Runs the signing script in a child process, the way the release workflow calls it, with every path
# quoted by hand: PowerShell 5.1 does not quote arguments that contain spaces.
function Invoke-Signing {
    param([string[]] $Extra)

    $arguments = '-NoProfile -ExecutionPolicy Bypass -File "{0}" -Files "{1}"' -f $signScript, $copy
    foreach ($extra in $Extra) { $arguments += ' ' + $extra }

    $start = New-Object System.Diagnostics.ProcessStartInfo
    $start.FileName = 'powershell.exe'
    $start.Arguments = $arguments
    $start.UseShellExecute = $false
    $start.RedirectStandardOutput = $true
    $start.RedirectStandardError = $true

    $process = [System.Diagnostics.Process]::Start($start)
    $output = $process.StandardOutput.ReadToEnd() + $process.StandardError.ReadToEnd()
    $process.WaitForExit()

    Say ('        exit code: {0}' -f $process.ExitCode)
    foreach ($line in ($output -split "`r?`n")) {
        if ($line.Trim()) { Say ('        | ' + $line.Trim()) }
    }

    return $process.ExitCode
}

$certificate = $null
$createdPfx = $false
$createdFolder = $false

try {
    Check 'the signing script is there' (Test-Path -LiteralPath $signScript)
    Check 'the published executable is there' (Test-Path -LiteralPath $source)

    if ($script:failures -gt 0) {
        Say 'Nothing to test.'
        exit 1
    }

    New-Item -ItemType Directory -Force -Path $work | Out-Null
    Copy-Item -LiteralPath $source -Destination $copy

    Say ''
    Say '=== with no certificate configured ==='
    if (Test-Path -LiteralPath $pfx) {
        Say '        (a certificate already exists at the documented path; skipping this half)'
    }
    else {
        # Has to be a warning with a successful exit: a tag on a fork still builds a release.
        Check 'an unsigned build does not fail' ((Invoke-Signing @()) -eq 0)
    }

    Say ''
    Say '=== with the owners own certificate at the documented path ==='
    $certificate = New-SelfSignedCertificate `
        -Type CodeSigningCert `
        -Subject 'CN=RandoFile Signing Smoke Test' `
        -CertStoreLocation 'Cert:\CurrentUser\My' `
        -KeyUsage DigitalSignature `
        -KeyExportPolicy Exportable `
        -NotAfter (Get-Date).AddDays(2)
    Say ('        test certificate: {0}' -f $certificate.Thumbprint)

    # A real certificate at the documented path is never touched: only what this run creates is
    # removed again.
    if (-not (Test-Path -LiteralPath $certificateFolder)) {
        New-Item -ItemType Directory -Force -Path $certificateFolder | Out-Null
        $createdFolder = $true
    }

    Export-PfxCertificate `
        -Cert ('Cert:\CurrentUser\My\' + $certificate.Thumbprint) `
        -FilePath $pfx `
        -Password (ConvertTo-SecureString -String $password -AsPlainText -Force) | Out-Null
    $createdPfx = $true

    # No -CertificatePath here on purpose: the default has to be the documented path.
    Check 'signing succeeds' ((Invoke-Signing @('-Password', ('"' + $password + '"'))) -eq 0)

    $signature = Get-AuthenticodeSignature -LiteralPath $copy
    Check 'the file carries a signature' ($signature.SignerCertificate -ne $null)
    Check 'the signature is the test certificate' ($signature.SignerCertificate.Thumbprint -eq $certificate.Thumbprint)
    Check 'the signature is timestamped' ($signature.TimeStamperCertificate -ne $null)
    Say ('        authenticode status: {0}' -f $signature.Status)
}
catch {
    Check 'the signing smoke test ran' $false
    Say ('        error: {0}' -f $_)
}
finally {
    Say ''
    Say '=== cleanup ==='
    if ($certificate) {
        Remove-Item -Path ('Cert:\CurrentUser\My\' + $certificate.Thumbprint) -Force -ErrorAction SilentlyContinue
        Say ('test certificate removed: {0}' -f (-not (Test-Path -LiteralPath ('Cert:\CurrentUser\My\' + $certificate.Thumbprint))))
    }

    if ($createdPfx) {
        Remove-Item -LiteralPath $pfx -Force -ErrorAction SilentlyContinue
        Say ('test certificate file removed: {0}' -f (-not (Test-Path -LiteralPath $pfx)))
    }

    if ($createdFolder) {
        Remove-Item -LiteralPath $certificateFolder -Force -Recurse -ErrorAction SilentlyContinue
    }

    Remove-Item -LiteralPath $work -Force -Recurse -ErrorAction SilentlyContinue
}

Say ''
Say ('RESULT: {0} checks, {1} failed' -f $script:checks, $script:failures)
exit $script:failures
