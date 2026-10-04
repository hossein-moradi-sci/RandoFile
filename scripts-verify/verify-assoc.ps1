# Verifies the installer's file association end to end, without needing elevation.
#
# The production install is machine-wide (PrivilegesRequired=admin), but /CURRENTUSER is a
# supported install mode and it is what makes this checkable from an ordinary shell: Setup then
# installs under %LOCALAPPDATA%\Programs and resolves HKA to HKCU. The machine hive is asserted to
# stay untouched, so the per-user run cannot mask a write that should have gone to HKLM.
#
# Run it from anywhere:  powershell -NoProfile -ExecutionPolicy Bypass -File scripts-verify\verify-assoc.ps1
# It writes the same output to scripts-verify\verify-assoc.log and exits 0 only if every check passes.
#
# Shares its helpers with verify-machine.ps1 through common.ps1 (dot-sourced below).
# ASCII only: PowerShell 5.1 reads a .ps1 as ANSI unless the file has a BOM, so non-ASCII text here
# would be mangled.

$ErrorActionPreference = 'Continue'

. (Join-Path $PSScriptRoot 'common.ps1')

$log = Join-Path $PSScriptRoot 'verify-assoc.log'
$appDir = Join-Path $env:LOCALAPPDATA 'Programs\RandoFile'
$appExe = Join-Path $appDir $appExeName
$startMenuShortcut = Join-Path $env:APPDATA 'Microsoft\Windows\Start Menu\Programs\RandoFile\HM File Randomizer.lnk'
$uninstallRoot = 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall'

$setup = Find-Setup

if (-not $setup) {
    throw 'No dist\installer\RandoFile-Setup-*.exe found. Build the installer first.'
}

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

function KeysPresent([string]$hive, [string]$label) {
    $missing = @()
    foreach ($sub in $subkeys) {
        if (-not (Test-Path -LiteralPath (Join-Path $hive $sub))) { $missing += $sub }
    }

    Check "$label - every association key exists" ($missing.Count -eq 0)
    foreach ($sub in $missing) { Say "        missing: $sub" }
}

function KeysAbsent([string]$hive, [string]$label) {
    $found = @()
    foreach ($sub in $subkeys) {
        if (Test-Path -LiteralPath (Join-Path $hive $sub)) { $found += $sub }
    }

    Check "$label - no association key is left behind" ($found.Count -eq 0)
    foreach ($sub in $found) { Say "        left behind: $sub" }
}

function Remove-Leftovers {
    # Driving the installer and uninstaller leaves the install directory present but empty, which
    # is what makes the next run stop on a "Folder Exists" prompt.
    Start-Sleep -Seconds 2
    Remove-Item -LiteralPath $appDir -Recurse -Force -ErrorAction SilentlyContinue
}

function Install-Silent([bool]$withTask) {
    $arguments = @('/VERYSILENT', '/SUPPRESSMSGBOXES', '/NORESTART', '/SP-', '/CURRENTUSER')
    if ($withTask) { $arguments += '/TASKS=fileassoc' }

    $process = Start-Process -FilePath $setup -ArgumentList $arguments -Wait -PassThru
    Say ("        install exit code: {0}" -f $process.ExitCode)

    return $process.ExitCode -eq 0
}

Say ('setup:    {0}' -f $setup)
Say ('app dir:  {0}' -f $appDir)
Say ''

# -------------------------------------------------------------------------------------------
# Baseline: nothing installed.
Say '=== before any install ==='
if (Test-Path -LiteralPath (Join-Path $appDir 'unins000.exe')) {
    if (Uninstall-Silent $appDir) { Say '        removed a pre-existing installation' }
    Remove-Leftovers
}
# Leftovers from an older build are reported, not failed: the requirement is that this build
# removes its association on uninstall, which the end state below proves.
$userBefore = SnapshotKeys 'HKCU:\'
if ($userBefore.Count -gt 0) {
    Say ('        note: HKCU already holds {0} association key(s) before this run; the install has to take them over and still remove them:' -f $userBefore.Count)
    foreach ($sub in $userBefore) { Say "            $sub" }
}

# The machine hive is compared before and after rather than asserted empty: a machine-wide
# install may already have left keys there (an older build that uninstalled incompletely), and
# attributing those to this run would be wrong.
$machineBefore = SnapshotKeys 'HKLM:\'
if ($machineBefore.Count -gt 0) {
    Say ('        note: HKLM already holds {0} association key(s) from an earlier machine-wide install:' -f $machineBefore.Count)
    foreach ($sub in $machineBefore) { Say "            $sub" }
}
Say ''

# -------------------------------------------------------------------------------------------
# With the task: the association must exist and must be usable.
Say '=== install with /TASKS=fileassoc ==='
Check 'the installer finished successfully' (Install-Silent $true)
Check 'the executable is installed' (Test-Path -LiteralPath $appExe)
Check 'the Start Menu shortcut is installed' (Test-Path -LiteralPath $startMenuShortcut)
Check 'an entry in Apps and features exists' ((Find-UninstallEntry $uninstallRoot) -ne $null)

KeysPresent 'HKCU:\' 'after install with the task'

$machineAdded = @(SnapshotKeys 'HKLM:\' | Where-Object { $machineBefore -notcontains $_ })
Check 'the per-user install must not write the machine hive' ($machineAdded.Count -eq 0)
foreach ($sub in $machineAdded) { Say "        new in HKLM: $sub" }

$command = GetDefaultValue 'HKCU:\Software\Classes\RandoFile.Shell\shell\open\command'
Say ("        shell command: {0}" -f $command)
Check 'the shell command runs the installed exe' ($command -like ('"{0}"*' -f $appExe))
Check 'the shell command passes the selected path on' ($command -like '*"%1"*')

$friendlyType = GetDefaultValue 'HKCU:\Software\Classes\RandoFile.Shell'
Check 'the ProgId has a friendly name' ($friendlyType -eq 'HM File Randomizer')

$friendlyApp = Get-ItemProperty -LiteralPath ('HKCU:\Software\Classes\Applications\{0}' -f $appExeName) -ErrorAction SilentlyContinue
Check 'Default apps sees a friendly application name' ($friendlyApp.FriendlyAppName -eq 'HM File Randomizer')

$verb = Get-ItemProperty -LiteralPath ('HKCU:\Software\Classes\Directory\shell\{0}' -f $progId) -ErrorAction SilentlyContinue
Check 'the folder right-click entry has a label' ($verb.MUIVerb -eq 'Open with HM File Randomizer')

foreach ($scope in @('Directory', '*')) {
    $commandPath = "HKCU:\Software\Classes\$scope\shell\$progId\command"
    $verbCommand = GetDefaultValue $commandPath
    Check "the $scope right-click entry runs the app with the path" ($verbCommand -like ('"{0}"*' -f $appExe) -and $verbCommand -like '*"%1"*')
}

$supportedTypes = Get-ItemProperty -LiteralPath ('HKCU:\Software\Classes\Applications\{0}\SupportedTypes' -f $appExeName) -ErrorAction SilentlyContinue
Check 'Default apps is told the app handles files' ($supportedTypes.PSObject.Properties['*'] -ne $null)
Check 'Default apps is told the app handles folders' ($supportedTypes.PSObject.Properties['Directory'] -ne $null)
Say ''

# -------------------------------------------------------------------------------------------
# Uninstall must leave nothing behind: a half-removed association is a broken "Open with" entry.
Say '=== uninstall ==='
Check 'the uninstaller ran to completion' (Uninstall-Silent $appDir)
Remove-Leftovers
Check 'the install directory is gone' (-not (Test-Path -LiteralPath $appDir))
Check 'the Start Menu shortcut is gone' (-not (Test-Path -LiteralPath $startMenuShortcut))
Check 'the Apps and features entry is gone' ((Find-UninstallEntry $uninstallRoot) -eq $null)
KeysAbsent 'HKCU:\' 'after uninstall'
Say ''

# -------------------------------------------------------------------------------------------
# Without the task: ticking the box is what creates the association, so unticking it must not.
Say '=== install without the task ==='
Check 'the installer finished successfully' (Install-Silent $false)
Check 'the executable is installed' (Test-Path -LiteralPath $appExe)
KeysAbsent 'HKCU:\' 'install without the task'
Say ''

Say '=== final cleanup ==='
Check 'the uninstaller ran to completion' (Uninstall-Silent $appDir)
Remove-Leftovers
Say ''

Say ('RESULT: {0} checks, {1} failed' -f $script:checks, $script:failures)
exit $script:failures
