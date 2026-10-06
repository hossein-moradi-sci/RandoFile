<div align="center">

<img src="src/RandoFile.App/Assets/Brand/logo.png" width="128" alt="RandoFile logo" />

<h1>🎲 RandoFile</h1>

<p><b>Shuffle any folder into a truly random order — preview first, rename safely, undo with one click.</b></p>

<p>
  <a href="https://github.com/hossein-moradi-sci/RandoFile/actions/workflows/ci.yml"><img src="https://github.com/hossein-moradi-sci/RandoFile/actions/workflows/ci.yml/badge.svg" alt="CI" /></a>
  <a href="https://github.com/hossein-moradi-sci/RandoFile/releases/latest"><img src="https://img.shields.io/github/v/release/hossein-moradi-sci/RandoFile?label=release" alt="Latest release" /></a>
  <a href="https://github.com/hossein-moradi-sci/RandoFile/releases/latest"><img src="https://img.shields.io/github/downloads/hossein-moradi-sci/RandoFile/total?label=downloads" alt="Downloads" /></a>
  <img src="https://img.shields.io/badge/Windows-10%20%2F%2011-0078D4" alt="Windows 10 / 11" />
  <img src="https://img.shields.io/badge/.NET-8.0-512BD4" alt=".NET 8" />
  <img src="https://img.shields.io/badge/license-MIT-34D058" alt="MIT license" />
</p>

<p align="center">
  <a href="README.md"><strong>ENGLISH</strong></a>
  &nbsp;•&nbsp;
  <a href="README.fa.md"><strong>فارسی</strong></a>
  &nbsp;•&nbsp;
  <a href="README.fr.md"><strong>FRANÇAIS</strong></a>
  &nbsp;•&nbsp;
  <a href="README.ar.md"><strong>العربية</strong></a>
</p>

</div>

---

**RandoFile** shuffles every file in a folder into a genuinely random order, then renames the
whole set with a clean, predictable pattern — and shows you a full preview of every old → new name
before anything touches your disk.

It ships as a single standalone `.exe` for Windows 10/11 and never runs in a browser.

> **RandoFile** is a brand name. It is never translated — every language keeps it exactly as
> written.

---

## ✨ Features

| | |
|---|---|
| **Folder picker with live count** | Choose any folder and see how many files were found. |
| **True random order** | Fisher–Yates shuffle, so the new order is genuinely unpredictable. |
| **Any file type** | Images, videos, documents — the original extension is always preserved. |
| **Naming presets** | `Image 1, Image 2 …`, `File 1, File 2 …`, or your own custom prefix. |
| **Numbering control** | Start number and optional zero padding, e.g. `Image 007.jpg`. |
| **Preview before rename** | Every old → new pair is listed, with a live summary of changes. |
| **Duplicate & overwrite guard** | A plan containing conflicts is refused; nothing is renamed. |
| **Safe engine** | Two-phase rename (stage → commit) makes swaps and circular renames work. |
| **Automatic rollback** | If a file fails, everything already renamed is restored. |
| **Undo** | One click reverts the last successful run. |
| **Progress & cancellation** | Progress bar plus a cancel button that rolls back cleanly. |
| **Animated splash** | A four-and-a-half second opening: the logo assembles itself, settles with a soft overshoot, then keeps breathing while a glowing neon credit badge lights up underneath. |
| **Four languages** | فارسی · English · Français · العربية, with full RTL for Persian and Arabic. |
| **Themes** | Light, dark, or follow the Windows setting. |
| **Update check** | Reads GitHub Releases and shows a banner when a newer version exists. |

---

## ⬇️ Download and install

Grab the latest build from the
[Releases page](https://github.com/hossein-moradi-sci/RandoFile/releases/latest) —
two clicks and you are done.

**The installer (recommended)**

1. Download `RandoFile-Setup-<version>-win-x64.exe`
2. Double-click it, pick your language, accept the licence
3. Accept the one Windows *User Account Control* prompt
4. Press Install

RandoFile installs into `C:\Program Files\RandoFile`, where Windows keeps installed programs,
with a Start Menu entry, an optional desktop shortcut and a proper entry in *Apps & features*
for uninstalling. **No .NET runtime is required**, and installing a later version over an
earlier one just replaces it. The agreement on the licence page is shown **in the language you
picked for the installer**, and every translation is installed next to the executable.

The wizard also offers two optional tasks: a desktop shortcut, and registering RandoFile in the
**Open with** menu for every folder and file, which is what lets you pick it in Windows' *Default
apps* — nothing is forced, and uninstalling removes everything it added. When Setup finishes, its
last page offers to launch RandoFile, open the folder it was installed into, and open this project
page.

Prefer no elevation at all? The setup accepts `/CURRENTUSER`, which installs it under your
profile instead of Program Files.

**Portable, if you prefer**

`RandoFile-<version>-win-x64.zip` is the same program without an installer: unzip it anywhere and
run `RandoFile.exe`.

Both are self-contained, so they run on any Windows 10 or 11 machine.

### 🔐 Verify the download

The installer and `RandoFile.exe` (the one inside the portable zip) both carry a digital
signature, so you can check that a file really came from this project and has not been
modified. The `.zip` container itself is not signed — extract it first, then check the
executable.

**Graphically:** right-click the file, choose **Properties**, open the **Digital Signatures**
tab, select the entry and press **Details**.

**In PowerShell** (swap `<version>` for the release you downloaded, such as `v1.0.0`):

```powershell
Get-AuthenticodeSignature .\RandoFile-Setup-<version>-win-x64.exe |
    Format-List Status, @{n='Signer';e={$_.SignerCertificate.Subject}},
                 @{n='Timestamped';e={$_.TimeStamperCertificate -ne $null}}
```

What to expect:

- **Signer** — `CN=Hossein Moradi`, the creator of the project.
- **Timestamped** — `True`, so the signature stays valid after the certificate expires.
- **Status** — `Valid` means Windows trusts the issuer. Builds signed with the project's own
  certificate report `UnknownError` instead: signature and timestamp are both intact, but the
  issuer is not a publicly trusted root, which is exactly what SmartScreen reacts to (see
  [Code signing](#code-signing)).

---

## 🚀 Usage

1. **Browse** — pick the folder that holds your files.
2. **Choose a pattern** — `Image`, `File`, or `Custom` with your own prefix.
3. **Preview** — press **Preview** to see exactly which file becomes which new name.
4. **Randomize** — press **Randomize** and confirm. The progress bar tracks the run.
5. **Undo** — if you did not like the result, press **Undo** to restore the original names.

Settings (language, theme, sub-folder scanning, update check) live in the **Settings** window;
creator and version details live in the **About** window.

A short illustrated guide is built into the app: press <kbd>F1</kbd>, or use the `?` button in the
header. It covers the four steps above, the naming options, the keyboard shortcuts, the safety
guarantees and the most common questions — in the same four languages as the rest of the interface.

---

## 🏷️ Naming patterns

| Preset | Result |
|---|---|
| Image | `Image 1.jpg`, `Image 2.jpg`, `Image 3.png`, … |
| File | `File 1.docx`, `File 2.mp4`, `File 3.txt`, … |
| Custom | `<your prefix> 1.ext`, `<your prefix> 2.ext`, … |

With **Start number** set to `101` and **Number of digits** set to `3` and the `Image` preset, the
first file becomes `Image 101.jpg`, the second `Image 102.jpg`, and so on.

---

## 🛡️ How the rename stays safe

Renaming files inside one folder is not a simple one-to-one copy — names can swap with each other,
or form a cycle (`a → b → c → a`). The engine therefore works in two phases:

1. **Stage** — every file is moved to a unique temporary name (`.rf_tmp_<guid><ext>`).
2. **Commit** — once nothing can collide, the temporary names are moved to their final names.

If anything fails mid-way, the phase already completed is undone automatically. The same machinery
backs the **Undo** command. Hidden, system and leftover temporary files are skipped when scanning.

---

## 🎨 The brand assets

![RandoFile logo](src/RandoFile.App/Assets/Brand/logo.png)

The mark is three document cards swept into a fan, with a ring segment above them for the randomising
motion. Free standing rather than a generic tile, so the silhouette is recognisable on its own, and
deliberately built out of separable parts — the splash screen rebuilds the same shapes as vectors so
it can animate them one by one before settling into the mark and then breathing forever.

| File | Used for |
|---|---|
| `src/RandoFile.App/Assets/Brand/logo.png` | 512 px master logo, on its dark plate |
| `src/RandoFile.App/Assets/Brand/logo-mark.png` | transparent mark used in the interface |
| `src/RandoFile.App/Assets/Brand/randofile.ico` | Windows icon, 16 → 256 px, on the same plate |

All three are generated, not drawn by hand. To change the mark, edit
[`tools/Generate-Logo.ps1`](tools/Generate-Logo.ps1) and run it:

```powershell
powershell -ExecutionPolicy Bypass -File tools/Generate-Logo.ps1
```

---

## 🛠️ Building from source

Requirements: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) and Windows.

```powershell
git clone https://github.com/hossein-moradi-sci/RandoFile.git
cd randofile

dotnet restore RandoFile.sln
dotnet build   RandoFile.sln -c Release --no-restore
dotnet test    RandoFile.sln -c Release --no-build
```

### Producing the single-file `.exe`

```powershell
dotnet publish src/RandoFile.App/RandoFile.App.csproj `
  -c Release -r win-x64 --self-contained true `
  -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true `
  -o dist/win-x64
```

The result is a single self-contained `dist/win-x64/RandoFile.exe` (~65 MB) that runs on
any Windows 10/11 machine without a .NET runtime.

---

## 📁 Project layout

```
RandoFile.sln
Directory.Build.props            Shared version + assembly metadata
src/
  RandoFile.Core/                Engine, no UI dependencies
    Models/                      NamingPattern, RenamePlan, RenameProgress, RenameResult
    Services/                    FileShuffler, FileScanner, RenamePlanBuilder, RenameExecutor
    Abstractions/                IFileScanner, IFileShuffler
    CommandLineTarget.cs         Turns the path Windows hands us into a folder to open
    AppInfo.cs                   Brand, product name, version and repository identity
  RandoFile.App/                 WPF application
    ViewModels/                  MainViewModel, SettingsOption, PreviewRow
    Views/                       SplashWindow, SettingsWindow, AboutWindow, HelpWindow
    Services/                    Settings, Localization, Theme, Dialog, Update, CrashLog
    Resources/Themes/            Light.xaml, Dark.xaml, Controls.xaml
    Resources/Strings/           Strings.en.xaml, Strings.fa.xaml, Strings.fr.xaml, Strings.ar.xaml
    Resources/Help/              Mockups.xaml (vector illustrations for the help window)
    Assets/Brand/                Logo and Windows icon
tests/
  RandoFile.Tests/               xUnit suite for the engine, brand, resources and XAML bindings
installer/
  randofile.iss                  Inno Setup script: the installer people download from a release
  license.txt                    The agreement, in English
  license.farsi.txt              The same agreement, in Persian
  license.french.txt             ... in French
  license.arabic.txt             ... in Arabic
  lang/                          Wizard strings: english, farsi, french, arabic
tools/
  Generate-Logo.ps1              Draws the brand mark and writes the PNG/ICO assets
  Sign-Release.ps1               Signs the release with your own certificate
scripts-verify/
  verify-assoc.ps1               Installs per-user and checks every association registry key
  verify-machine.ps1             The same against a machine-wide install in Program Files
  verify-finish-page.ps1         Drives the real wizard and clicks its Finish button
  verify-signing.ps1             Proves the signing script with a throwaway certificate
  common.ps1                     Helpers the association scripts share
```

The `Core` project has no WPF dependency, which is what makes the rename logic fully testable.

---

## 🧪 Testing

```powershell
dotnet test RandoFile.sln
```

The suite covers the naming pattern, the planner (duplicate, overwrite, no-op detection), the
executor (swaps, cycles, rollback, cancellation, undo, no leftover temporary files), the folder
scanner, version comparison for the update check, resource parity across all four languages, the
brand identity rules, and the XAML bindings.

Some of those guards exist because of bugs that reached a published build:

- **Two-way bindings to read-only properties.** WPF binds `ProgressBar.Value`, `TextBox.Text` and
  `ToggleButton.IsChecked` two-way *by default*, so pointing them at a computed view-model property
  throws the instant the window is first laid out — the app shows a "Problem" dialog instead of
  opening. Any new binding on those properties needs an explicit `Mode=OneWay` or a public setter.
- **The brand must stay out of the language files.** `BrandIdentityTests` fails if "RandoFile"
  ever appears in a `Strings.*.xaml` value, because a translator could then rename or drop it.

The splash has its own timing guard: the screen may not close until every storyboard in it has
finished, and the credit line has to stay settled and readable for at least a second and a half
afterwards, so a beat can never be cut off half way and turn the opening into a flicker.

`InstallerScriptTests` guards the Inno Setup script the same way. It is only compiled in CI, so
these failures would otherwise surface at release time: a stale version in the installer, an `AppId`
that stopped being a GUID (which silently gives every user a second copy instead of an upgrade), a
wizard language missing from the script, or a half-finished translation that Inno Setup would
quietly fall back to English for.

The wizard itself is verified by running it. `scripts-verify/verify-assoc.ps1` installs per-user
(`/CURRENTUSER`, so no UAC prompt) with the association task, reads every registry entry back,
then installs without the task and uninstalls — checking in each case that exactly the right keys
exist. `scripts-verify/verify-machine.ps1` does the same for a machine-wide install, which needs an
elevated shell. `scripts-verify/verify-finish-page.ps1` walks the real wizard to its last page and
confirms that the three options are there and that clicking Finish opens the app, the install
folder and the project page. `scripts-verify/verify-signing.ps1` signs a throwaway copy of the
published executable with a certificate it creates and then deletes, so the signing path is proven
on a machine that has no certificate at all.

---

## 📦 Versioning and releases

The version lives in a single place, [`Directory.Build.props`](Directory.Build.props)
(`VersionPrefix`) and is surfaced in the UI as `v1.0.0`.

Releases follow [Semantic Versioning](https://semver.org/). Publishing is automated: pushing a
`v*` tag builds the executable and creates a GitHub Release. Because the app already reads
`releases/latest` and compares versions with a normalised SemVer parser, adding a fully automatic
update download is a small follow-up step.

### Code signing

Releases are published **unsigned by default**, which is why Windows SmartScreen warns about the
installer until a certificate is configured. When one is, the workflow signs twice: the published
`RandoFile.exe` before Inno Setup packages it, and the installer afterwards — a signed program
inside an unsigned installer does not make the download trusted on its own. Every signature is
timestamped, so it stays valid after the certificate expires.

Two repository secrets turn it on (*Settings → Secrets and variables → Actions*):

| Secret | Value |
|---|---|
| `WINDOWS_CERT_PFX_BASE64` | your certificate as a `.pfx`, base64 encoded |
| `WINDOWS_CERT_PASSWORD` | the password for that `.pfx` |

To produce the first one from a local certificate:

```powershell
[Convert]::ToBase64String([IO.File]::ReadAllBytes('certificates\code-signing.pfx')) | Set-Clipboard
```

For a release built on your own machine, put the certificate at `certificates\code-signing.pfx` —
that exact path is where [`tools/Sign-Release.ps1`](tools/Sign-Release.ps1) looks for it, and
`certificates/` is ignored by git so a private key can never be committed — and run:

```powershell
tools\Sign-Release.ps1 -Files dist\installer\RandoFile-Setup-v1.0.0-win-x64.exe
```

If the file is password protected, add `-Password <password>` (or set `RANDOFILE_CERT_PASSWORD`).
When the certificate is installed in the Windows certificate store instead — which is where a
hardware token or a signing service's client puts it — sign by thumbprint:

```powershell
tools\Sign-Release.ps1 -Files dist\win-x64\RandoFile.exe -Thumbprint <thumbprint>
```

Since June 2023 a publicly trusted certificate authority is not allowed to issue an exportable
private key: it has to live on a hardware token, an HSM, or a cloud signing service. A public
certificate therefore usually takes the `-Thumbprint` route, while the `.pfx` route suits
self-signed and internally trusted certificates. Cloud services (Azure Trusted Signing, DigiCert
KeyLocker, SSL.com eSigner) sign through their own tool, which takes the place of the two signing
steps in the workflow.

`scripts-verify/verify-signing.ps1` proves the whole route on a machine with no certificate at
all: it creates a throwaway self-signed certificate, signs a **copy** of the published executable,
checks that the signature carries a timestamp, and removes the certificate, the `.pfx` and the copy
again.

See [CHANGELOG.md](CHANGELOG.md) for the release history.

---

## 👤 About the creator

**Hossein Moradi** — HM
Electrical Power Engineering Technologist
Power Electrical Engineering · Software & Web App Developer · Website Designer · Project Management & Planning

---

## 📄 License

MIT © 2026 Hossein Moradi — see [LICENSE](LICENSE).

<br/>

<div align="center">

<img src="src/RandoFile.App/Assets/Brand/logo-mark.png" width="64" alt="RandoFile" />

<p>If RandoFile saved you a renaming session, give the repo a star ⭐ — it genuinely helps.</p>

<p>Made with 💛 by <b>Hossein Moradi</b></p>

</div>
