# Helpers and data shared by the installer verification scripts. Dot-source this file; it defines
# no state of its own.
#
# ASCII only: PowerShell 5.1 reads a .ps1 as ANSI unless the file has a BOM, so non-ASCII text
# here would be mangled.

$repoRoot = Split-Path -Parent $PSScriptRoot

$progId = 'RandoFile.Shell'
$appExeName = 'RandoFile.exe'

# Every key the [Registry] section is meant to create. LiteralPath everywhere: the association
# lives under the key named '*' as well, which the provider would otherwise treat as a wildcard.
$subkeys = @(
    "Software\Classes\$progId",
    "Software\Classes\$progId\DefaultIcon",
    "Software\Classes\$progId\shell\open\command",
    "Software\Classes\*\OpenWithProgids\$progId",
    "Software\Classes\Directory\OpenWithProgids\$progId",
    "Software\Classes\Applications\$appExeName",
    "Software\Classes\Applications\$appExeName\shell\open\command",
    "Software\Classes\Applications\$appExeName\SupportedTypes",
    "Software\Classes\Directory\shell\$progId",
    "Software\Classes\Directory\shell\$progId\Icon",
    "Software\Classes\Directory\shell\$progId\command",
    "Software\Classes\*\shell\$progId",
    "Software\Classes\*\shell\$progId\Icon",
    "Software\Classes\*\shell\$progId\command"
)

function Find-Setup {
    return Get-ChildItem -LiteralPath (Join-Path $repoRoot 'dist\installer') -Filter 'RandoFile-Setup-*.exe' |
        Sort-Object LastWriteTime -Descending |
        Select-Object -First 1 -ExpandProperty FullName
}

function SnapshotKeys([string]$hive) {
    $present = @()
    foreach ($sub in $subkeys) {
        if (Test-Path -LiteralPath (Join-Path $hive $sub)) { $present += $sub }
    }
    return $present
}

function GetDefaultValue([string]$path) {
    if (Test-Path -LiteralPath $path) {
        # Get-ItemProperty cannot read the unnamed value; this is the call that can.
        return Get-ItemPropertyValue -LiteralPath $path -Name '(Default)'
    }
    return $null
}

function Uninstall-Silent([string]$Directory) {
    $uninstaller = Join-Path $Directory 'unins000.exe'
    if (-not (Test-Path -LiteralPath $uninstaller)) { return $false }

    Start-Process -FilePath $uninstaller -ArgumentList '/VERYSILENT', '/SUPPRESSMSGBOXES', '/NORESTART' -Wait | Out-Null

    # The uninstaller copies itself to %TEMP% and returns, so wait for the copy to finish rather
    # than trusting the launcher's exit.
    for ($i = 0; $i -lt 120; $i++) {
        if (-not (Test-Path -LiteralPath $uninstaller)) { break }
        Start-Sleep -Seconds 1
    }

    return -not (Test-Path -LiteralPath $uninstaller)
}

function Find-UninstallEntry([string[]]$Roots) {
    foreach ($root in $Roots) {
        if (-not (Test-Path -LiteralPath $root)) { continue }

        $entry = Get-ChildItem -LiteralPath $root -ErrorAction SilentlyContinue |
            Where-Object { (Get-ItemProperty -LiteralPath $_.PSPath -ErrorAction SilentlyContinue).DisplayName -like 'RandoFile*' } |
            Select-Object -First 1

        if ($entry) { return $entry }
    }
    return $null
}
