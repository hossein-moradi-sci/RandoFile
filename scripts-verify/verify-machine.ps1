# Verifies the installer the way a real user gets it: machine-wide, into Program Files, with the
# association keys in HKLM.
#
# This one needs administrator rights, which is why it is separate from verify-assoc.ps1: it is the
# production path, and it also clears the empty keys an earlier build left behind (keys without
# uninsdeletekey survived their uninstall - this run makes the fixed installer take them over and
# then remove them).
#
# It is launched detached through run-elevated.ps1 so that an approving prompt does not have to keep
# a console alive. Output goes to scripts-verify\verify-machine.log. Shares its helpers with
# verify-assoc.ps1 through common.ps1 (dot-sourced below). ASCII only, because PowerShell 5.1 reads
# a .ps1 as ANSI unless the file has a BOM.

$ErrorActionPreference = 'Continue'

. (Join-Path $PSScriptRoot 'common.ps1')

$log = Join-Path $PSScriptRoot 'verify-machine.log'
$appDir = Join-Path $env:ProgramFiles 'RandoFile'
$appExe = Join-Path $appDir $appExeName
$startMenuShortcut = Join-Path $env:ProgramData 'Microsoft\Windows\Start Menu\Programs\RandoFile\HM File Randomizer.lnk'
$uninstallRoots = @(
    'HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall',
    'HKLM:\Software\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall')

$setup = Find-Setup

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

$isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

Say ('setup:   {0}' -f $setup)
Say ('app dir: {0}' -f $appDir)
Say ('elevated: {0}' -f $isAdmin)
Say ''

if (-not $isAdmin) {
    Say 'This script must run elevated; nothing was changed.'
    exit 1
}

if (-not $setup) {
    throw 'No dist\installer\RandoFile-Setup-*.exe found. Build the installer first.'
}

Say '=== before install ==='
$before = SnapshotKeys 'HKLM:\'
if ($before.Count -gt 0) {
    Say ('        note: {0} association key(s) already exist (from an earlier build):' -f $before.Count)
    foreach ($sub in $before) { Say "            $sub" }
}
else {
    Say '        no association keys present'
}
Say ''

Say '=== machine-wide install with /TASKS=fileassoc ==='
$process = Start-Process -FilePath $setup -ArgumentList '/VERYSILENT', '/SUPPRESSMSGBOXES', '/NORESTART', '/SP-', '/TASKS=fileassoc' -Wait -PassThru
Say ('        install exit code: {0}' -f $process.ExitCode)
Check 'the installer finished successfully' ($process.ExitCode -eq 0)
Check 'the executable is installed' (Test-Path -LiteralPath $appExe)
Check 'the Start Menu shortcut is installed' (Test-Path -LiteralPath $startMenuShortcut)
Check 'an entry in Apps and features exists' ((Find-UninstallEntry $uninstallRoots) -ne $null)

Check 'every association key exists in the machine hive' ((SnapshotKeys 'HKLM:\').Count -eq $subkeys.Count)

$command = GetDefaultValue 'HKLM:\Software\Classes\RandoFile.Shell\shell\open\command'
Say ('        shell command: {0}' -f $command)
Check 'the shell command runs the installed exe' ($command -like ('"{0}"*' -f $appExe))
Check 'the shell command passes the selected path on' ($command -like '*"%1"*')

$verb = Get-ItemProperty -LiteralPath ('HKLM:\Software\Classes\Directory\shell\{0}' -f $progId) -ErrorAction SilentlyContinue
Check 'the folder right-click entry has a label' ($verb.MUIVerb -eq 'Open with HM File Randomizer')
Say ''

# This is the half that failed before: the program has to leave nothing of itself in the machine
# registry, and that includes the keys an older build abandoned.
Say '=== uninstall ==='
Check 'the uninstaller ran to completion' (Uninstall-Silent $appDir)
Start-Sleep -Seconds 2
Remove-Item -LiteralPath $appDir -Recurse -Force -ErrorAction SilentlyContinue

$after = SnapshotKeys 'HKLM:\'
Check 'the machine hive holds no association key at all' ($after.Count -eq 0)
foreach ($sub in $after) { Say "        left behind: $sub" }
Check 'the install directory is gone' (-not (Test-Path -LiteralPath $appDir))
Check 'the Start Menu shortcut is gone' (-not (Test-Path -LiteralPath $startMenuShortcut))
Check 'the Apps and features entry is gone' ((Find-UninstallEntry $uninstallRoots) -eq $null)
Say ''

Say ('RESULT: {0} checks, {1} failed' -f $script:checks, $script:failures)
exit $script:failures
