# Starts verify-machine.ps1 detached, so the elevated caller can return immediately and the install
# is not cut short by whatever timeout the caller has. The child inherits the elevation.
# ASCII only (PowerShell 5.1 reads .ps1 as ANSI without a BOM).

$script = Join-Path $PSScriptRoot 'verify-machine.ps1'
$log = Join-Path $PSScriptRoot 'verify-machine.log'

Remove-Item -LiteralPath $log -Force -ErrorAction SilentlyContinue

# PowerShell 5.1 joins -ArgumentList with spaces and does NOT quote what it joins, so a script path
# containing a space has to carry its own quotes or the child is handed a truncated path.
$quotedScript = '"' + $script + '"'

Start-Process -FilePath 'powershell.exe' `
    -ArgumentList @('-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', $quotedScript) `
    -WindowStyle Hidden

# The log appears as soon as the child writes its first line.
for ($i = 0; $i -lt 30; $i++) {
    if (Test-Path -LiteralPath $log) { break }
    Start-Sleep -Seconds 1
}

if (Test-Path -LiteralPath $log) {
    Write-Host 'verify-machine.ps1 is running detached; watching its log.'
}
else {
    Write-Host ('verify-machine.ps1 did not start. Script path: {0}' -f $script)
    exit 1
}
