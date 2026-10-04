# RandoFile

<img src="src/RandoFile.App/Assets/Brand/logo.png" alt="RandoFile" width="128" align="right" />

**Version:** `v1.0.0` · **License:** MIT · **Creator:** Hossein Moradi
**Platform:** Windows 10/11 · **Stack:** C# / .NET 8 / WPF

**RandoFile** — short for *HM File Randomizer* — shuffles every file in a folder into a random order
and renames it with a clean, predictable naming pattern, with a full preview before anything touches disk.

It ships as a standalone `.exe` and never runs in a browser.

> The name **RandoFile** is a brand and is never translated. Every language of the interface keeps
> it exactly as written.

[English](README.md) · [فارسی](#فارسی--persian) · [Français](#français--french) · [العربية](#العربية--arabic)

---

## Features

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

## Download and install

Grab the latest build from the
[Releases page](https://github.com/hossein-moradi-sci/hm-file-randomizer/releases/latest).

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

---

## Usage

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

## Naming patterns

| Preset | Result |
|---|---|
| Image | `Image 1.jpg`, `Image 2.jpg`, `Image 3.png`, … |
| File | `File 1.docx`, `File 2.mp4`, `File 3.txt`, … |
| Custom | `<your prefix> 1.ext`, `<your prefix> 2.ext`, … |

With **Start number** set to `101` and **Number of digits** set to `3` and the `Image` preset, the
first file becomes `Image 101.jpg`, the second `Image 102.jpg`, and so on.

---

## How the rename stays safe

Renaming files inside one folder is not a simple one-to-one copy — names can swap with each other,
or form a cycle (`a → b → c → a`). The engine therefore works in two phases:

1. **Stage** — every file is moved to a unique temporary name (`.rf_tmp_<guid><ext>`).
2. **Commit** — once nothing can collide, the temporary names are moved to their final names.

If anything fails mid-way, the phase already completed is undone automatically. The same machinery
backs the **Undo** command. Hidden, system and leftover temporary files are skipped when scanning.

---

## The brand assets

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

## Building from source

Requirements: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) and Windows.

```powershell
git clone https://github.com/hossein-moradi-sci/hm-file-randomizer.git
cd hm-file-randomizer

dotnet restore
dotnet build   -c Release
dotnet test    -c Release
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

## Project layout

```
RandoFile.sln
Directory.Build.props            Shared version + assembly metadata
src/
  RandoFile.Core/                Engine, no UI dependencies
    Models/                      NamingPattern, RenamePlan, RenameProgress, RenameResult
    Services/                    FileShuffler, FileScanner, RenamePlanBuilder, RenameExecutor
    Abstractions/                IFileScanner, IFileShuffler
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
```

The `Core` project has no WPF dependency, which is what makes the rename logic fully testable.

---

## Testing

```powershell
dotnet test
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
exist. `scripts-verify/verify-finish-page.ps1` walks the real wizard to its last page and confirms
that the three options are there and that clicking Finish opens the app, the install folder and the
project page.

---

## Versioning and releases

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

## About the creator

**Hossein Moradi** — HM
Electrical Power Engineering Technologist
Power Electrical Engineering · Software & Web App Developer · Website Designer · Project Management & Planning

---

## License

MIT © 2026 Hossein Moradi — see [LICENSE](LICENSE).

---

# فارسی — Persian

**نسخه:** `v1.0.0` · **مجوز:** MIT · **سازنده:** حسین مرادی

**RandoFile** (کوتاه‌شدهٔ *HM File Randomizer*) همهٔ فایل‌های یک پوشه را به‌طور کاملاً تصادفی جابه‌جا می‌کند
و با یک الگوی نام‌گذاری تمیز و یکسان تغییر نام می‌دهد — و پیش از هر تغییری، پیش‌نمایش کامل را نشان می‌دهد.

> نام **RandoFile** یک نام برند است و **هرگز ترجمه نمی‌شود**؛ در هر چهار زبان دقیقاً به همین شکل می‌ماند.

این برنامه به‌صورت یک فایل `.exe` مستقل اجرا می‌شود و داخل مرورگر اجرا نمی‌شود.

## قابلیت‌ها

- صفحهٔ شروع انیمیشنی با لوگو هنگام باز شدن برنامه
- انتخاب پوشه به‌همراه نمایش تعداد فایل‌ها
- جابه‌جایی کاملاً تصادفی (Shuffle) با الگوریتم Fisher–Yates
- پشتیبانی از هر نوع فایل؛ پسوند اصلی همیشه حفظ می‌شود
- الگوهای آمادهٔ `Image 1, Image 2 …` و `File 1, File 2 …` و پیشوند دلخواه
- تنظیم شمارهٔ شروع و تعداد ارقام، مثلاً `Image 007.jpg`
- پیش‌نمایش کامل نام‌های جدید پیش از تغییر روی دیسک
- جلوگیری از نام تکراری و بازنویسی فایل موجود
- موتور تغییر نام دو مرحله‌ای با بازگشت خودکار (Rollback) در صورت خطا
- امکان Undo برای آخرین اجرای موفق
- نوار پیشرفت و امکان لغو عملیات
- چهار زبان: فارسی 🇮🇷، English 🇬🇧، Français 🇫🇷، العربية 🇸🇦 — با چیدمان راست‌به‌چپ
- پوستهٔ روشن، تاریک یا هم‌رنگ با ویندوز
- بررسی خودکار نسخهٔ جدید از GitHub Releases

## طراح و سازنده

**حسین مرادی**
کارشناس ارشد برق
مهندسی برق قدرت · توسعه‌دهندهٔ نرم‌افزار و وب · طراح وب‌سایت · مدیریت و برنامه‌ریزی پروژه

---

# Français — French

**Version :** `v1.0.0` · **Licence :** MIT · **Créateur :** Hossein Moradi

**RandoFile** (raccourci de *HM File Randomizer*) mélange tous les fichiers d'un dossier dans un ordre
totalement aléatoire, puis les renomme selon un schéma clair et régulier — avec un aperçu complet
avant toute modification sur le disque.

> Le nom **RandoFile** est une marque : il n'est **jamais traduit** et reste identique dans les quatre langues.

Elle se distribue sous forme d'un fichier `.exe` autonome et ne s'exécute jamais dans un navigateur.

## Fonctionnalités

- Écran de démarrage animé avec le logo
- Sélection de dossier avec compteur de fichiers en direct
- Mélange réellement aléatoire (Fisher–Yates)
- Tous types de fichiers acceptés ; l'extension d'origine est toujours conservée
- Préréglages `Image 1, Image 2 …` et `File 1, File 2 …`, plus un préfixe personnalisé
- Numéro de départ et remplissage par des zéros, par exemple `Image 007.jpg`
- Aperçu de chaque renommage avant exécution
- Détection des doublons et des écrasements : un plan en conflit est refusé
- Moteur de renommage en deux phases avec annulation automatique (rollback)
- Commande **Annuler** pour revenir sur le dernier traitement réussi
- Barre de progression et possibilité d'interrompre l'opération
- Quatre langues : فارسی, English, Français, العربية — avec mise en page RTL pour le persan et l'arabe
- Thèmes clair, sombre ou système
- Vérification des nouvelles versions via GitHub Releases

## Créateur

**Hossein Moradi** — Technologue en génie électrique de puissance
Génie électrique de puissance · Développeur logiciel et web · Concepteur de sites · Gestion et planification de projets

---

# العربية — Arabic

**الإصدار:** `v1.0.0` · **الترخيص:** MIT · **المطوّر:** Hossein Moradi

**RandoFile** (اختصارًا لـ *HM File Randomizer*) يخلط ملفات المجلد ترتيبًا عشوائيًا بالكامل ثم يعيد
تسميتها بنمط واضح ومنسّق — مع معاينة كاملة قبل أي تعديل على القرص.

> اسم **RandoFile** علامة تجارية، وهو **لا يُترجم أبدًا** ويبقى كما هو في اللغات الأربع.

يُوزَّع التطبيق كملف `.exe` مستقل، ولا يعمل داخل المتصفح إطلاقًا.

## المزايا

- شاشة بدء متحركة مع الشعار
- اختيار مجلد مع إظهار عدد الملفات مباشرةً
- خلط عشوائي حقيقي (Fisher–Yates)
- دعم جميع أنواع الملفات؛ يُحتفظ بالامتداد الأصلي دائمًا
- أنماط جاهزة `Image 1, Image 2 …` و`File 1, File 2 …` مع بادئة مخصّصة
- رقم بداية اختياري مع أصفار بادئة، مثل `Image 007.jpg`
- معاينة لكل عملية إعادة تسمية قبل تنفيذها
- كشف التكرار والكتابة فوق الملفات الموجودة ورفض الخطة غير الآمنة
- محرّك إعادة تسمية على مرحلتين مع تراجع تلقائي عند حدوث خطأ
- زر **تراجع** لإرجاع آخر عملية ناجحة
- شريط تقدّم مع إمكانية إلغاء العملية
- أربع لغات: فارسی، English، Français، العربية — مع تخطيط من اليمين إلى اليسار
- سمات فاتحة وداكنة أو تتبع النظام
- فحص الإصدارات الجديدة عبر GitHub Releases

## المطوّر

**Hossein Moradi** — تقني هندسة الكهرباء الصناعية
هندسة الكهرباء · مطوّر برمجيات ومواقع · مصمم مواقع · إدارة وتخطيط المشاريع
