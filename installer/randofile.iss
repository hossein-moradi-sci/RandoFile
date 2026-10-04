; =============================================================================
;  RandoFile (HM File Randomizer) - Inno Setup script
; =============================================================================
;
;  Built in CI, not on a developer machine. See .github/workflows/ci.yml.
;
;      iscc /DAppVersion=1.0.0 /DReleaseTag=v1.0.0 installer/randofile.iss
;
;  AppVersion is injected so the installer can never claim a version the app
;  does not have. ReleaseTag is the git tag the same version came from; it is
;  only used in the output file name, and matches the portable zip so both
;  assets on a release page are named the same way. The fallbacks below exist
;  only so the script still opens in the Inno Setup IDE; InstallerScriptTests.cs
;  fails the build if they drift from AppInfo.Version.
;
;  AppId is a fixed GUID and must never change. Inno Setup uses it to decide
;  whether an existing installation is an older RandoFile to upgrade, or a
;  different product that has to live beside it. Changing it silently leaves
;  every user with two copies of the app.
; =============================================================================

#ifndef AppVersion
  #define AppVersion "1.0.0"
#endif

#ifndef ReleaseTag
  #define ReleaseTag "v1.0.0"
#endif

#define AppName        "RandoFile"
#define AppFullName    "HM File Randomizer"
#define AppPublisher   "Hossein Moradi"
#define AppExe         "RandoFile.exe"

; The project page. AppPublisherURL points straight at it, the support and update addresses are
; built on it, and [Run] opens it from the finish page - so the address is written once here
; instead of in four places that can drift apart.
#define ProjectUrl     "https://github.com/hossein-moradi-sci/hm-file-randomizer"

; Where CI drops the self-contained publish output, relative to this script.
#define PublishDir     "..\dist\win-x64"

; The licence is translated, one file per wizard language. Inno Setup has no per-language
; LicenseFile, so [Code] picks the right one and fills the page; LicenseFile= below only
; has to exist so that Inno creates the licence page at all. The translations sit flat
; next to license.txt because [Code] reads them from {src}, which does not keep the
; subdirectory structure of the source paths.
#define LicenceDefault "license.txt"
#define LicenceFarsi   "license.farsi.txt"
#define LicenceFrench  "license.french.txt"
#define LicenceArabic  "license.arabic.txt"

; The shell identifier for the registration below. Changing it makes Windows forget the old
; entry and leaves it behind as an orphaned "Open with RandoFile" choice, so treat it as fixed.
#define ProgId         "RandoFile.Shell"

[Setup]
AppId={{6B3C9E14-2F71-4C86-9D2A-8F5B41C7E903}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL={#ProjectUrl}
AppSupportURL={#ProjectUrl}/issues
AppUpdatesURL={#ProjectUrl}/releases/latest
AppContact=hossein.moradi.sci@gmail.com

; {autopf} is Program Files on 64-bit Windows and Program Files (x86) on 32-bit Windows,
; so the app always lands where the operating system keeps installed programs. Writing
; there needs administrator rights, which is why PrivilegesRequired=admin below.
DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes
UninstallDisplayName={#AppName} ({#AppFullName})
UninstallDisplayIcon={app}\{#AppExe}

; A Program Files install needs elevation, so Setup asks once, at the start, exactly like
; every other installed program. 'commandline' (but never 'dialog') still allows /CURRENTUSER
; for scripted installs and keeps the extra Select Setup Install Mode page off the screen.
PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=commandline
ArchitecturesInstallIn64BitMode=x64compatible

; Shown before anything is copied, with an acceptance tick box. [Code] replaces the text with
; the translation that matches the wizard language the user just picked.
LicenseFile={#LicenceDefault}

OutputDir=..\dist\installer
OutputBaseFilename=RandoFile-Setup-{#ReleaseTag}-win-x64
SetupIconFile=..\src\RandoFile.App\Assets\Brand\randofile.ico
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "lang\english.isl"
Name: "farsi";   MessagesFile: "lang\farsi.isl"
Name: "french";  MessagesFile: "lang\french.isl"
Name: "arabic";  MessagesFile: "lang\arabic.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "fileassoc"; Description: "{cm:AssociateFiles}"; GroupDescription: "{cm:Integration}"; Flags: unchecked

[Files]
; The self-contained publish: one RandoFile.exe plus whatever the runtime needs.
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

; The licence travels with the installed files, as the MIT notice requires. Every translation is
; installed too, so the agreement can be read again later in whichever language applies.
Source: "{#LicenceDefault}"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#LicenceFarsi}"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#LicenceFrench}"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#LicenceArabic}"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#AppFullName}"; Filename: "{app}\{#AppExe}"
Name: "{group}\{cm:UninstallProgram,{#AppFullName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#AppFullName}"; Filename: "{app}\{#AppExe}"; Tasks: desktopicon

; Entries with the postinstall flag become checkboxes on the finish page, and they are ticked
; unless they carry the unchecked flag. These two both point at what the install just produced -
; the folder it landed in and the project it came from - so they start ticked, and clearing the
; box is enough to decline.
;
; Neither is an executable, so both need shellexec: that is what opens a folder in Explorer and an
; https address in the default browser, the way a double-click would. They also need skipifsilent,
; so a scripted /VERYSILENT install does not open windows on a machine nobody is watching.
;
; Inno Setup runs postinstall entries as the user who started Setup, not as the elevated
; administrator, so the browser and Explorer do not inherit an administrator token.
[Run]
Filename: "{app}\{#AppExe}"; Description: "{cm:LaunchProgram,{#AppName}}"; Flags: nowait postinstall skipifsilent
Filename: "{app}"; Description: "{cm:OpenInstallFolder,{#AppName}}"; Flags: postinstall shellexec skipifsilent
Filename: "{#ProjectUrl}"; Description: "{cm:OpenProjectPage}"; Flags: postinstall shellexec skipifsilent

; RandoFile randomizes the contents of a folder, so it is registered for folders and for any
; file: a folder argument is opened as it stands, and a file argument means the folder that
; file is in. That is a choice the user makes on this page, never a default.
;
; Windows since 8 decides which program opens a file type, and protects that decision with a
; hash only Windows can compute, so an installer cannot take it over silently. What it can do
; is register itself properly and appear in "Open with" and in Default apps, which is what
; these entries do. Nothing here writes a UserChoice key, and a test fails the build if one
; ever does: a forced default is a broken registry state the user cannot undo from Explorer.
;
; Every entry carries uninsdeletekey, children included. Measured on a real uninstall: without it
; Inno Setup removes only the values it wrote and leaves the keys behind, so Open with keeps an
; empty Applications entry and OpenWithProgids references long after the program is gone. A test
; fails the build if an entry ever loses the flag.
[Registry]
Root: HKA; Subkey: "Software\Classes\{#ProgId}"; ValueType: string; ValueName: ""; ValueData: "{#AppFullName}"; Flags: uninsdeletekey; Tasks: fileassoc
Root: HKA; Subkey: "Software\Classes\{#ProgId}"; ValueType: string; ValueName: "FriendlyTypeName"; ValueData: "{#AppFullName}"; Flags: uninsdeletekey; Tasks: fileassoc
Root: HKA; Subkey: "Software\Classes\{#ProgId}\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\{#AppExe},0"; Flags: uninsdeletekey; Tasks: fileassoc
Root: HKA; Subkey: "Software\Classes\{#ProgId}\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#AppExe}"" ""%1"""; Flags: uninsdeletekey; Tasks: fileassoc

; Makes RandoFile a candidate in "Open with" for every file and for every folder.
Root: HKA; Subkey: "Software\Classes\*\OpenWithProgids\{#ProgId}"; ValueType: string; ValueName: ""; ValueData: ""; Flags: uninsdeletekey; Tasks: fileassoc
Root: HKA; Subkey: "Software\Classes\Directory\OpenWithProgids\{#ProgId}"; ValueType: string; ValueName: ""; ValueData: ""; Flags: uninsdeletekey; Tasks: fileassoc

; What Default apps itself reads to list the program as a possible handler.
Root: HKA; Subkey: "Software\Classes\Applications\{#AppExe}\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#AppExe}"" ""%1"""; Flags: uninsdeletekey; Tasks: fileassoc
Root: HKA; Subkey: "Software\Classes\Applications\{#AppExe}\SupportedTypes"; ValueType: string; ValueName: "*"; ValueData: ""; Flags: uninsdeletekey; Tasks: fileassoc
Root: HKA; Subkey: "Software\Classes\Applications\{#AppExe}\SupportedTypes"; ValueType: string; ValueName: "Directory"; ValueData: ""; Flags: uninsdeletekey; Tasks: fileassoc
Root: HKA; Subkey: "Software\Classes\Applications\{#AppExe}"; ValueType: string; ValueName: "FriendlyAppName"; ValueData: "{#AppFullName}"; Flags: uninsdeletekey; Tasks: fileassoc

; The right-click entry. The brand is never translated, so the label is written out.
Root: HKA; Subkey: "Software\Classes\Directory\shell\{#ProgId}"; ValueType: string; ValueName: "MUIVerb"; ValueData: "Open with {#AppFullName}"; Flags: uninsdeletekey; Tasks: fileassoc
Root: HKA; Subkey: "Software\Classes\Directory\shell\{#ProgId}\Icon"; ValueType: string; ValueName: ""; ValueData: "{app}\{#AppExe}"; Flags: uninsdeletekey; Tasks: fileassoc
Root: HKA; Subkey: "Software\Classes\Directory\shell\{#ProgId}\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#AppExe}"" ""%1"""; Flags: uninsdeletekey; Tasks: fileassoc

Root: HKA; Subkey: "Software\Classes\*\shell\{#ProgId}"; ValueType: string; ValueName: "MUIVerb"; ValueData: "Open with {#AppFullName}"; Flags: uninsdeletekey; Tasks: fileassoc
Root: HKA; Subkey: "Software\Classes\*\shell\{#ProgId}\Icon"; ValueType: string; ValueName: ""; ValueData: "{app}\{#AppExe}"; Flags: uninsdeletekey; Tasks: fileassoc
Root: HKA; Subkey: "Software\Classes\*\shell\{#ProgId}\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#AppExe}"" ""%1"""; Flags: uninsdeletekey; Tasks: fileassoc

[Code]
{ The licence has to be in the same language as the wizard around it. A Persian setup must
  not ask a Persian speaker to agree to an English contract. }

{ Windows keeps the choice of default handler in a key whose value is protected by a hash only
  Windows itself can compute, which is why this opens Default apps for the user instead of writing
  that key. Forcing it leaves registry state the user cannot undo from Explorer. }
procedure CurStepChanged(CurStep: TSetupStep);
var
  ResultCode: Integer;
begin
  if (CurStep = ssPostInstall) and WizardIsTaskSelected('fileassoc') then
    ShellExec('open', 'ms-settings:defaultapps', '', '', SW_SHOWNORMAL, ewNoWait, ResultCode);
end;

function LicenceFileFor(ALanguage: String): String;
begin
  Result := '{#LicenceDefault}';
  if ALanguage = 'farsi' then
    Result := '{#LicenceFarsi}'
  else if ALanguage = 'french' then
    Result := '{#LicenceFrench}'
  else if ALanguage = 'arabic' then
    Result := '{#LicenceArabic}';
end;

procedure InitializeWizard();
var
  LicenceLines: TArrayOfString;
  Name: String;
  P: String;
  Ok: Boolean;
begin
  { ActiveLanguage is the name of the [Languages] entry the user is running the wizard in,
    so the contract is written in exactly the language they just chose.

    ExtractTemporaryFile is what puts the text where it can be read. The src constant does
    NOT point at the extracted data files: it resolves to the folder the Setup executable
    sits in, so loading the licence straight from it silently fails and every language is
    left showing the English licence. LoadStringsFromFile is the same loader Inno Setup uses
    for its .isl files, so it decodes the UTF-8 BOM the Persian, French and Arabic licences
    are saved with and strips it from the text. }
  Name := LicenceFileFor(ActiveLanguage);
  ExtractTemporaryFile(Name);
  P := ExpandConstant('{tmp}\') + Name;
  Ok := LoadStringsFromFile(P, LicenceLines);
  if Ok then
    WizardForm.LicenseMemo.Lines.Text := StringJoin(#13#10, LicenceLines);
end;

procedure CurPageChanged(CurPageID: Integer);
begin
  { The finish page is the one screen every user sees, so it carries the credits.
    CurStepChanged(ssPostInstall) is NOT enough: it runs before Inno builds the finish
    page's caption, and that build overwrites whatever we set. The caption only exists
    once the finish page is shown, which is CurPageChanged(wpFinished). }
  if CurPageID = wpFinished then
    WizardForm.FinishedLabel.Caption :=
      ExpandConstant('{cm:InstalledReady,{#AppFullName}}') + #13#10 +
      ExpandConstant('{cm:InstalledBy,{#AppVersion},{#AppPublisher}}') + #13#10 +
      ExpandConstant('{cm:InstalledProject}') + #13#10 +
      ExpandConstant('{cm:InstalledLinkedIn}') + #13#10 +
      ExpandConstant('{cm:InstalledEmail}');
end;
