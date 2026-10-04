# Walks the built installer wizard and checks the finish page for real.
#
# The finish page is the one page a text check cannot vouch for: Inno Setup builds its caption and
# its checkbox list itself, which is exactly how the credits were lost once before while every
# static check stayed green.
#
# Driving it takes the Win32 route on purpose. The wizard's controls are Inno's own custom classes
# (TNewButton, TNewCheckBox, TNewRadioButton) and UI Automation exposes them as generic Panes with
# no pattern to invoke, so they are found by class and caption through EnumChildWindows and clicked
# with real mouse input. Only the finish label and the Explorer window are read back through UI
# Automation, where the Document and CabinetWClass types do exist.
#
# It installs per-user (/CURRENTUSER), so no elevation and no UAC dialog, and it leaves the machine
# as it found it: the app it opened is closed, the folder window is closed, the installation is
# removed again, and the mouse cursor is put back.
#
# Run it from anywhere:  powershell -NoProfile -ExecutionPolicy Bypass -File scripts-verify\verify-finish-page.ps1
# ASCII only, because PowerShell 5.1 reads a .ps1 as ANSI unless the file has a BOM.

$ErrorActionPreference = 'Continue'

Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Windows.Forms
Add-Type -TypeDefinition @'
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

public static class Wizard32
{
    public delegate bool EnumProc(IntPtr hWnd, IntPtr lParam);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT { public int Left; public int Top; public int Right; public int Bottom; }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT { public int X; public int Y; }

    [DllImport("user32.dll")]
    private static extern bool EnumChildWindows(IntPtr hWnd, EnumProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    private static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int X, int Y);

    [DllImport("user32.dll")]
    private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

    [DllImport("user32.dll")]
    public static extern bool SetProcessDPIAware();

    [DllImport("user32.dll")]
    public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, StringBuilder lParam);

    // The finish page keeps its three options in a TNewCheckListBox, which is backed by a real
    // Win32 list box, so its captions come out through the list box messages.
    public static List<string> ListBoxItems(IntPtr hwnd)
    {
        var items = new List<string>();
        var count = (int)SendMessage(hwnd, 0x018B, IntPtr.Zero, IntPtr.Zero);   // LB_GETCOUNT
        for (var i = 0; i < count; i++)
        {
            var buffer = new StringBuilder(1024);
            SendMessage(hwnd, 0x0189, (IntPtr)i, buffer);                       // LB_GETTEXT
            items.Add(buffer.ToString());
        }
        return items;
    }

    public class Control
    {
        public IntPtr Handle;
        public string ClassName;
        public string Text;
        public bool Visible;
    }

    public static List<Control> Children(IntPtr parent)
    {
        var found = new List<Control>();
        EnumChildWindows(parent, delegate(IntPtr hWnd, IntPtr lParam)
        {
            var cls = new StringBuilder(256);
            var text = new StringBuilder(1024);
            GetClassName(hWnd, cls, cls.Capacity);
            GetWindowText(hWnd, text, text.Capacity);
            found.Add(new Control
            {
                Handle = hWnd,
                ClassName = cls.ToString(),
                Text = text.ToString(),
                Visible = IsWindowVisible(hWnd),
            });
            return true;
        }, IntPtr.Zero);
        return found;
    }

    public static void Activate(IntPtr hwnd)
    {
        SetForegroundWindow(hwnd);
        Thread.Sleep(150);
    }

    public static void Click(IntPtr hwnd)
    {
        RECT rect;
        if (!GetClientRect(hwnd, out rect)) { return; }

        var point = new POINT();
        point.X = (rect.Right - rect.Left) / 2;
        point.Y = (rect.Bottom - rect.Top) / 2;
        if (!ClientToScreen(hwnd, ref point)) { return; }

        SetCursorPos(point.X, point.Y);
        Thread.Sleep(150);
        mouse_event(0x0002, 0, 0, 0, UIntPtr.Zero);
        Thread.Sleep(80);
        mouse_event(0x0004, 0, 0, 0, UIntPtr.Zero);
    }
}
'@

$root = Split-Path -Parent $PSScriptRoot
$log = Join-Path $PSScriptRoot 'verify-finish-page.log'
$appDir = Join-Path $env:LOCALAPPDATA 'Programs\RandoFile'

# Physical pixels, before any window is measured: this machine runs at 125%.
[Wizard32]::SetProcessDPIAware() | Out-Null

$setup = Get-ChildItem -LiteralPath (Join-Path $root 'dist\installer') -Filter 'RandoFile-Setup-*.exe' |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1 -ExpandProperty FullName

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

function Get-WizardProcess {
    return Get-Process -ErrorAction SilentlyContinue |
        Where-Object { $_.ProcessName -like 'RandoFile-Setup*' -and $_.MainWindowHandle -ne 0 } |
        Select-Object -First 1
}

function Visible-Controls([IntPtr]$handle) {
    return @([Wizard32]::Children($handle) | Where-Object { $_.Visible })
}

function Find-Control($controls, [string]$className, [string]$caption) {
    return $controls |
        Where-Object { $_.ClassName -eq $className -and (($_.Text -replace '&', '') -match $caption) } |
        Select-Object -First 1
}

function Get-FinishText([IntPtr]$handle) {
    $element = [System.Windows.Automation.AutomationElement]::FromHandle($handle)
    if (-not $element) { return '' }

    # The finish label is a rich edit exposed as a plain Pane whose Name is the whole caption, so
    # the names are what has to be read; rich edit documents are read as body text on top of that.
    $parts = @()
    foreach ($child in $element.FindAll([System.Windows.Automation.TreeScope]::Descendants, ([System.Windows.Automation.Condition]::TrueCondition))) {
        if ($child.Current.Name) { $parts += $child.Current.Name }

        if ($child.Current.ControlType -eq [System.Windows.Automation.ControlType]::Document) {
            try {
                $parts += $child.GetCurrentPattern([System.Windows.Automation.TextPattern]::Pattern).DocumentRange.GetText(-1)
            }
            catch { }
        }
    }
    return ($parts -join "`n")
}

function Uninstall-App {
    $uninstaller = Join-Path $appDir 'unins000.exe'
    if (Test-Path -LiteralPath $uninstaller) {
        Start-Process -FilePath $uninstaller -ArgumentList '/VERYSILENT', '/SUPPRESSMSGBOXES', '/NORESTART' -Wait | Out-Null
        for ($i = 0; $i -lt 90; $i++) {
            if (-not (Test-Path -LiteralPath (Join-Path $appDir 'unins000.exe'))) { break }
            Start-Sleep -Seconds 1
        }
    }
    Start-Sleep -Seconds 2
    Remove-Item -LiteralPath $appDir -Recurse -Force -ErrorAction SilentlyContinue
}

function Close-OpenedFolder([string]$titlePart) {
    $desktop = [System.Windows.Automation.AutomationElement]::RootElement
    foreach ($window in $desktop.FindAll([System.Windows.Automation.TreeScope]::Children, [System.Windows.Automation.Condition]::TrueCondition)) {
        if ($window.Current.ClassName -eq 'CabinetWClass' -and $window.Current.Name -like "*$titlePart*") {
            # WM_CLOSE, so Explorer closes that one window instead of the whole process.
            [Wizard32]::PostMessage([IntPtr]$window.Current.NativeWindowHandle, 0x0010, [IntPtr]::Zero, [IntPtr]::Zero) | Out-Null
        }
    }
}

Say ('setup:   {0}' -f $setup)
Say ('app dir: {0}' -f $appDir)
Say ''

# Nothing may be installed before the walk, or Setup stops on "Folder Exists".
Uninstall-App

$cursor = [System.Windows.Forms.Cursor]::Position
$wizardHandle = [IntPtr]::Zero
$wizardProcessId = 0

try {
    # /LANG=english skips the language dialog, /CURRENTUSER keeps the UAC dialog away.
    $launcher = Start-Process -FilePath $setup -ArgumentList '/CURRENTUSER', '/LANG=english' -PassThru
    Say ('launcher started, pid {0}' -f $launcher.Id)

    $wizard = $null
    for ($i = 0; $i -lt 60; $i++) {
        $wizard = Get-WizardProcess
        if ($wizard) { break }
        Start-Sleep -Seconds 1
    }

    if (-not $wizard) {
        Check 'the wizard window appears' $false
        throw 'The setup window never appeared.'
    }

    $wizardProcessId = $wizard.Id
    $wizardHandle = $wizard.MainWindowHandle
    Say ('wizard window: pid {0}, title "{1}", handle {2}' -f $wizard.Id, $wizard.MainWindowTitle, $wizardHandle)
    [Wizard32]::Activate($wizardHandle) | Out-Null
    Start-Sleep -Seconds 2

    # Walk the pages: pick the acceptance radio where the page has one, then click Next or Install.
    # The finish page is the one whose visible controls include a Finish button.
    $finish = $null
    for ($step = 0; $step -lt 120; $step++) {
        $controls = Visible-Controls($wizardHandle)

        $finish = Find-Control $controls 'TNewButton' '^Finish'
        if ($finish) { break }

        $radio = @($controls | Where-Object {
            $clean = $_.Text -replace '&', ''
            $_.ClassName -eq 'TNewRadioButton' -and $clean -match 'accept' -and $clean -notmatch 'not'
        }) | Select-Object -First 1
        if ($radio) {
            [Wizard32]::Click($radio.Handle)
            Start-Sleep -Milliseconds 500
        }

        $next = Find-Control $controls 'TNewButton' '^(Next|Install)'
        $summary = (@($controls | Where-Object { $_.Text } | ForEach-Object { $_.Text }) -join ' | ')
        if ($summary.Length -gt 110) { $summary = $summary.Substring(0, 110) }
        Say ('        step {0}: {1}' -f $step, $summary)

        if ($next) {
            [Wizard32]::Click($next.Handle)
            Start-Sleep -Seconds 1
        }
        else {
            # Installing: there is no Next, and clicking around could reach Cancel.
            Start-Sleep -Seconds 2
        }
    }

    Say ''
    Say '=== finish page ==='
    Check 'the wizard reached the finish page' ($finish -ne $null)

    # The three [Run] postinstall entries are items of the finish page's check list box, and their
    # captions come out through the list box messages: TNewCheckListBox is backed by a real Win32
    # list box, and LB_GETTEXT from another process works against it - measured. UI Automation
    # exposes no ListItems for that control at all, so there is no fallback to reach for.
    $lists = @(Visible-Controls($wizardHandle) | Where-Object { $_.ClassName -eq 'TNewCheckListBox' })
    $captions = @()
    foreach ($list in $lists) {
        foreach ($item in [Wizard32]::ListBoxItems($list.Handle)) {
            Say ('        option: "{0}"' -f $item)
            $captions += $item
        }
    }

    Check 'the finish page offers all three options' ($captions.Count -eq 3)
    Check 'the launch option is there' ($captions -contains 'Launch RandoFile')
    Check 'the install folder option is there' ($captions -contains 'Open the RandoFile folder')
    Check 'the project page option is there' ($captions -contains 'Open the project page on GitHub')

    # The finish label is a rich edit, so it is read through UI Automation - and only once the page
    # has actually painted, which is why this retries instead of reading straight away.
    $finishText = ''
    for ($i = 0; $i -lt 10; $i++) {
        $finishText = Get-FinishText $wizardHandle
        if ($finishText -like '*github.com*') { break }
        Start-Sleep -Seconds 1
    }
    Check 'the finish page still carries the credits' ($finishText -like '*github.com/hossein-moradi-sci/hm-file-randomizer*')
    Check 'the credits name the creator' ($finishText -like '*Hossein Moradi*')

    Say ''
    Say '=== clicking Finish, with every option left ticked ==='
    $clickedAt = Get-Date
    if ($finish) {
        [Wizard32]::Activate($wizardHandle) | Out-Null
        [Wizard32]::Click($finish.Handle)
    }

    Start-Sleep -Seconds 25

    # All three options are ticked by default. If any of them were not, its window would not appear,
    # so these three checks are also what proves the boxes start ticked.
    $app = Get-Process -Name 'RandoFile' -ErrorAction SilentlyContinue
    Check 'Finish launched the application' ($app -ne $null)

    $desktop = [System.Windows.Automation.AutomationElement]::RootElement
    $windows = $desktop.FindAll([System.Windows.Automation.TreeScope]::Children, [System.Windows.Automation.Condition]::TrueCondition)
    $folderWindows = @()
    foreach ($window in $windows) {
        if ($window.Current.ClassName -eq 'CabinetWClass' -and $window.Current.Name -like '*RandoFile*') {
            $folderWindows += $window.Current.Name
        }
    }
    Check 'Finish opened the install folder' ($folderWindows.Count -gt 0)
    foreach ($name in $folderWindows) { Say ('        window: {0}' -f $name) }

    # The project page opens in whatever browser the machine uses. A browser that is already running
    # opens a tab instead of a process, so accept either a new process or a window naming the repo.
    $browsers = @('chrome', 'msedge', 'firefox', 'brave', 'opera', 'vivaldi')
    $newBrowser = $null
    foreach ($process in (Get-Process -ErrorAction SilentlyContinue)) {
        if ($browsers -notcontains $process.ProcessName.ToLowerInvariant()) { continue }
        # StartTime throws for processes this account cannot query; those are not our browser.
        try { if ($process.StartTime -gt $clickedAt) { $newBrowser = $process } } catch { }
        if ($newBrowser) { break }
    }

    $projectWindow = @($windows | ForEach-Object { $_.Current.Name } | Where-Object { $_ -like '*GitHub*' -or $_ -like '*hm-file-randomizer*' })
    Check 'Finish opened the project page' (($newBrowser -ne $null) -or ($projectWindow.Count -gt 0))
    foreach ($name in $projectWindow) { Say ('        window: {0}' -f $name) }
    if ($newBrowser) { Say ('        process: {0}' -f $newBrowser.ProcessName) }
}
catch {
    # A walk that breaks must fail the run rather than quietly report nothing.
    Check 'the wizard walk completed' $false
    Say ('        error: {0}' -f $_)
}
finally {
    Say ''
    Say '=== cleanup ==='
    Stop-Process -Name 'RandoFile' -Force -ErrorAction SilentlyContinue
    Get-Process -Name 'RandoFile-Setup*' -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
    Close-OpenedFolder 'RandoFile'
    Uninstall-App
    [System.Windows.Forms.Cursor]::Position = $cursor
    Say ('installed app removed: {0}' -f (-not (Test-Path -LiteralPath $appDir)))
}

Say ''
Say ('RESULT: {0} checks, {1} failed' -f $script:checks, $script:failures)
exit $script:failures
