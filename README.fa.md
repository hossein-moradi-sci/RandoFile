<div dir="rtl">

<div align="center">

<img src="src/RandoFile.App/Assets/Brand/logo.png" width="128" alt="لوگوی RandoFile" />

<h1>🎲 RandoFile</h1>

<p><b>هر پوشه را در یک ترتیب کاملاً تصادفی به‌هم می‌ریزد — اول پیش‌نمایش، بعد تغییر نام مطمئن، و با یک کلیک بازگرداندن.</b></p>

<p>
  <a href="https://github.com/hossein-moradi-sci/hm-file-randomizer/actions/workflows/ci.yml"><img src="https://github.com/hossein-moradi-sci/hm-file-randomizer/actions/workflows/ci.yml/badge.svg" alt="CI" /></a>
  <a href="https://github.com/hossein-moradi-sci/hm-file-randomizer/releases/latest"><img src="https://img.shields.io/github/v/release/hossein-moradi-sci/hm-file-randomizer?label=release" alt="آخرین نسخه" /></a>
  <a href="https://github.com/hossein-moradi-sci/hm-file-randomizer/releases/latest"><img src="https://img.shields.io/github/downloads/hossein-moradi-sci/hm-file-randomizer/total?label=downloads" alt="دانلودها" /></a>
  <img src="https://img.shields.io/badge/Windows-10%20%2F%2011-0078D4" alt="ویندوز ۱۰ و ۱۱" />
  <img src="https://img.shields.io/badge/.NET-8.0-512BD4" alt=".NET 8" />
  <img src="https://img.shields.io/badge/license-MIT-34D058" alt="مجوز MIT" />
</p>

<p>
  <a href="README.md"><img src="https://img.shields.io/badge/English-64748B?style=for-the-badge" alt="English" /></a>
  <a href="README.fa.md"><img src="https://img.shields.io/badge/%D9%81%D8%A7%D8%B1%D8%B3%DB%8C-2563EB?style=for-the-badge" alt="فارسی" /></a>
  <a href="README.fr.md"><img src="https://img.shields.io/badge/Fran%C3%A7ais-64748B?style=for-the-badge" alt="Français" /></a>
  <a href="README.ar.md"><img src="https://img.shields.io/badge/%D8%A7%D9%84%D8%B9%D8%B1%D8%A8%D9%8A%D8%A9-64748B?style=for-the-badge" alt="العربية" /></a>
</p>

</div>

---

**RandoFile** تک‌تک فایل‌های یک پوشه را با یک قرعه‌کشی واقعی به‌هم می‌ریزد، بعد کل مجموعه را با یک الگوی تمیز و قابل پیش‌بینی دوباره نام‌گذاری می‌کند — و پیش از آن‌که حتی یک نام روی دیسکت عوض شود، کامل‌ترین پیش‌نمایش «نام قدیمی ← نام جدید» را جلوی چشمت می‌گذارد.

یک فایل `.exe` مستقل و تک‌فایلی برای ویندوز ۱۰ و ۱۱ است و هیچ‌وقت در مرورگر اجرا نمی‌شود.

> **RandoFile** یک نام تجاری است و هرگز ترجمه نمی‌شود — در هر زبانی دقیقاً همین‌طور نوشته می‌شود.

> 💛 این نسخه فارسی از اول به‌صورت طبیعی به فارسی نوشته شده، نه ترجمه لفظ‌به‌لفت انگلیسی؛ همان عمقِ نسخه انگلیسی را دارد و روان خوانده می‌شود.

---

## ✨ امکانات

| | |
|---|---|
| **انتخاب پوشه با شمارنده زنده** | هر پوشه‌ای را انتخاب کنید و بلافاصله ببینید چند فایل پیدا شده است. |
| **ترتیب کاملاً تصادفی** | به‌هم‌ریختن از الگوریتم Fisher–Yates، یعنی ترتیب جدید واقعاً قابل حدس نیست. |
| **هر نوع فایلی** | تصویر، ویدیو، سند — پسوند اصلی همیشه حفظ می‌شود. |
| **الگوهای آماده نام‌گذاری** | `Image 1, Image 2 …`، `File 1, File 2 …` یا پیشوند دلخواه خودتان. |
| **کنترل شماره‌گذاری** | شماره شروع و صفرگذاری اختیاری، مثلاً `Image 007.jpg`. |
| **پیش‌نمایش قبل از تغییر نام** | هر جفت «قدیمی ← جدید» فهرست می‌شود، همراه با خلاصه زنده تغییرات. |
| **محافظ تکراری و بازنویسی** | اگر طرحِ نام‌گذاری شامل تداخل باشد رد می‌شود؛ هیچ فایلی تغییر نمی‌کند. |
| **موتور امن** | تغییر نام دومرحله‌ای (مرحله‌بندی ← اجرا) تا جابه‌جایی نام‌ها و چرخه‌های نامی هم درست کار کنند. |
| **بازگردانی خودکار** | اگر سر راه فایلی شکست بخورد، همه تغییرات انجام‌شده برمی‌گردند. |
| **واگردانی (Undo)** | یک کلیک، آخرین اجرا را به حالت اول برمی‌گرداند. |
| **پیشرفت و لغو** | نوار پیشرفت و دکمه‌ای برای لغو که همه‌چیز را تمیز به عقب برمی‌گرداند. |
| **اسپلش متحرک** | چهار و نیم ثانیه آغازین: قطعه‌های لوگو کنار هم می‌نشینند، با اندکی اشاره بیش از حد جا می‌افتند، بعد برنامه نفس می‌کشد و نشان اعتبار نئونی زیرش آرام روشن می‌شود. |
| **چهار زبان** | فارسی · English · Français · العربية، با چیدمان کامل راست‌به‌چپ برای فارسی و عربی. |
| **پوسته‌ها** | روشن، تاریک، یا همراه با تنظیمات خود ویندوز. |
| **بررسی به‌روزرسانی** | نسخه‌ها را از GitHub Releases می‌خواند و اگر نسخه تازه‌تری آمده باشد بنر نشان می‌دهد. |

---

## ⬇️ دانلود و نصب

آخرین نسخه را از
[صفحه Releases](https://github.com/hossein-moradi-sci/hm-file-randomizer/releases/latest)
بردارید — دو کلیک و تمام.

**نصب‌کننده (پیشنهادی)**

1. فایل `RandoFile-Setup-<version>-win-x64.exe` را دانلود کنید.
2. روی آن دوبار کلیک کنید، زبان را انتخاب کنید و قرارداد را بپذیرید.
3. یک بار پنجره *User Account Control* ویندوز را تأیید کنید.
4. دکمه Install را بزنید.

RandoFile در `C:\Program Files\RandoFile` نصب می‌شود — جایی که ویندوز برنامه‌های نصب‌شده را نگه می‌دارد — با یک ورودی در منوی Start، میان‌بر اختیاری روی دسکتاپ و رکورد درست در *Apps & features* برای حذف. **نیازی به نصب .NET نیست**، و نصب نسخه جدید روی نسخه قبلی صرفاً آن را جایگزین می‌کند. متن قرارداد در صفحه مجوز **به همان زبانی نشان داده می‌شود که برای نصب‌کننده انتخاب کردید** — یعنی فارسی — و همه ترجمه‌ها کنار فایل اجرایی نصب می‌شوند.

جادوگر نصب دو وظیفه اختیاری هم دارد: میان‌بر دسکتاپ، و ثبت RandoFile در منوی **Open with** برای همه پوشه‌ها و فایل‌ها، که همان چیزی است که انتخابش را در *Default apps* ویندوز ممکن می‌کند — هیچ‌کدام اجباری نیستند و حذف نصب، هرچه افزوده پاک می‌کند. وقتی Setup تمام شد، صفحه آخرش پیشنهاد می‌دهد RandoFile را اجرا کند، پوشه نصب را باز کند و همین صفحه پروژه را باز کند.

اگر اصلاً دلتان نمی‌خواهد ارتقای دسترسی بدهید، نصب‌کننده `/CURRENTUSER` را می‌پذیرد و برنامه به‌جای Program Files داخل پروفایل خودتان نصب می‌شود.

**نسخه قابل حمل، اگر ترجیح می‌دهید**

`RandoFile-<version>-win-x64.zip` همان برنامه بدون نصب‌کننده است: هر جایی که دوست دارید از حالت فشرده خارج کنید و `RandoFile.exe` را اجرا کنید.

هر دو نسخه self-contained هستند، پس روی هر ویندوز ۱۰ یا ۱۱ کار می‌کنند.

### 🔐 بررسی فایل دانلودشده

نصب‌کننده و `RandoFile.exe` (همان فایل داخل zip) هر دو امضای دیجیتال دارند، پس می‌توانید بفهمید فایل واقعاً از همین پروژه آمده و دستکاری نشده. خودِ فایل `.zip` امضا ندارد — اول از حالت فشرده خارج کنید، بعد فایل اجرایی را بسنجید.

**از راه گرافیکی:** روی فایل راست‌کلیک کنید، *Properties* را بزنید، سراغ تب **Digital Signatures** بروید، مورد را انتخاب کنید و **Details** را بزنید.

**در PowerShell** (به‌جای `<version>` نسخه‌ای را که دانلود کرده‌اید بگذارید، مثلاً `v1.0.0`):

```powershell
Get-AuthenticodeSignature .\RandoFile-Setup-<version>-win-x64.exe |
    Format-List Status, @{n='Signer';e={$_.SignerCertificate.Subject}},
                 @{n='Timestamped';e={$_.TimeStamperCertificate -ne $null}}
```

چه چیزی باید ببینید:

- **Signer** — `CN=Hossein Moradi`، سازنده پروژه.
- **Timestamped** — `True`، یعنی امضا بعد از انقضای گواهی هم معتبر می‌ماند.
- **Status** — اگر `Valid` باشد یعنی ویندوز صادرکننده را معتبر می‌شناسد. بیلدهایی که با گواهی خود پروژه امضا شده‌اند `UnknownError` نشان می‌دهند: امضا و مهر زمان هر دو سالم‌اند، اما صادرکننده ریشه عموماً معتبر نیست — دقیقاً همان چیزی که SmartScreen به آن واکنش نشان می‌دهد (بخش [امضای کد](#امضای-کد) را ببینید).

---

## 🚀 نحوه استفاده

1. **Browse** — پوشه‌ای که فایل‌هایتان در آن است را انتخاب کنید.
2. **الگو را بچینید** — `Image`، `File` یا `Custom` با پیشوند دلخواه.
3. **پیش‌نمایش** — دکمه **Preview** را بزنید تا دقیقاً ببینید هر فایل به چه نامی می‌رود.
4. **Randomize** — دکمه **Randomize** را بزنید و تأیید کنید. نوار پیشرفت، اجرا را نشانتان می‌دهد.
5. **Undo** — اگر نتیجه را نپسندیدید، **Undo** بزنید تا نام‌های اصلی برگردند.

تنظیمات (زبان، پوسته، پویش زیرپوشه‌ها، بررسی به‌روزرسانی) در پنجره **Settings** است؛ اطلاعات سازنده و نسخه هم در پنجره **About**.

یک راهنمای تصویری کوتاه داخل خود برنامه است: کلید <kbd>F1</kbd> را بزنید یا دکمه `?` در سربرگ. همان چهار مرحله بالا، گزینه‌های نام‌گذاری، میان‌برهای صفحه‌کلید، تضمین‌های ایمنی و پرتکرارترین پرسش‌ها را پوشش می‌دهد — به همان چهار زبانِ بقیه رابط.

---

## 🏷️ الگوهای نام‌گذاری

| الگو | نتیجه |
|---|---|
| Image | `Image 1.jpg`، `Image 2.jpg`، `Image 3.png`، … |
| File | `File 1.docx`، `File 2.mp4`، `File 3.txt`، … |
| Custom | `<your prefix> 1.ext`، `<your prefix> 2.ext`، … |

اگر **Start number** برابر `101` و **Number of digits** برابر `3` باشد و الگوی `Image` را انتخاب کنید، اولین فایل `Image 101.jpg` می‌شود، دومی `Image 102.jpg` و الی آخر.

---

## 🛡️ چطور نام‌گذاری امن می‌ماند

تغییر نام فایل‌های داخل یک پوشه یک کار یک‌به‌یک ساده نیست — نام‌ها ممکن است با هم جابه‌جا شوند یا یک چرخه بسازند (`a → b → c → a`). به همین دلیل موتور در دو مرحله کار می‌کند:

1. **مرحله‌بندی** — هر فایل به یک نام موقت یکتا (`.rf_tmp_<guid><ext>`) می‌رود.
2. **اجرا** — وقتی دیگر هیچ تداخلی ممکن نبود، نام‌های موقت به نام نهایی می‌رسند.

اگر وسط کار همه‌چیز خراب شود، مرحله‌ای که تمام شده بود خودبه‌خود برمی‌گردد. همین ساز و کار، فرمان **Undo** را هم پشتیبانی می‌کند. فایل‌های مخفی، سیستمی و موقتِ باقی‌مانده هنگام پویش نادیده گرفته می‌شوند.

---

## 🎨 دارایی‌های برند

![RandoFile logo](src/RandoFile.App/Assets/Brand/logo.png)

نشان، سه کارت سند است که به شکل بادبزن کنار هم نشسته‌اند و یک قطعه حلقه بالایشان برای حس حرکت تصادفی قرار دارد. مستقل از هر کاشی عمومی‌ای ساخته شده تا سایه خودش به‌تنهایی قابل تشخیص باشد، و عمداً از بخش‌های جداشدنی درست شده — صفحه آغازین همان شکل‌ها را به‌صورت برداری بازسازی می‌کند تا بتواند تک‌تکشان را متحرک کند، بعد در نشان جا بیفتد و تا ابد نفس بکشد.

| فایل | کاربرد |
|---|---|
| `src/RandoFile.App/Assets/Brand/logo.png` | لوگوی اصلی ۵۱۲ پیکسلی، روی زمینه تاریکش |
| `src/RandoFile.App/Assets/Brand/logo-mark.png` | نشان شفاف که در رابط کاربری استفاده می‌شود |
| `src/RandoFile.App/Assets/Brand/randofile.ico` | آیکون ویندوز، از ۱۶ تا ۲۵۶ پیکسل، روی همان زمینه |

هر سه تولیدشده‌اند، نه دستی. برای تغییر نشان، فایل
[`tools/Generate-Logo.ps1`](tools/Generate-Logo.ps1)
را ویرایش کنید و اجرا کنید:

```powershell
powershell -ExecutionPolicy Bypass -File tools/Generate-Logo.ps1
```

---

## 🛠️ ساخت از روی کد

پیش‌نیازها: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) و ویندوز.

```powershell
git clone https://github.com/hossein-moradi-sci/hm-file-randomizer.git
cd hm-file-randomizer

dotnet restore RandoFile.sln
dotnet build   RandoFile.sln -c Release --no-restore
dotnet test    RandoFile.sln -c Release --no-build
```

### ساخت فایل تک‌فایلی `.exe`

```powershell
dotnet publish src/RandoFile.App/RandoFile.App.csproj `
  -c Release -r win-x64 --self-contained true `
  -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true `
  -o dist/win-x64
```

خروجی، یک فایل مستقل `dist/win-x64/RandoFile.exe` (حدود ۶۵ مگابایت) است که بدون هیچ runtime‌ای از .NET روی هر ویندوز ۱۰ یا ۱۱ اجرا می‌شود.

---

## 📁 ساختار پروژه

```
RandoFile.sln
Directory.Build.props            نسخه و اطلاعات assembly مشترک
src/
  RandoFile.Core/                موتور، بدون وابستگی به UI
    Models/                      NamingPattern, RenamePlan, RenameProgress, RenameResult
    Services/                    FileShuffler, FileScanner, RenamePlanBuilder, RenameExecutor
    Abstractions/                IFileScanner, IFileShuffler
    CommandLineTarget.cs         مسیری که ویندوز می‌دهد را به پوشه قابل بازکردن تبدیل می‌کند
    AppInfo.cs                   برند، نام محصول، نسخه و هویت مخزن
  RandoFile.App/                 برنامه WPF
    ViewModels/                  MainViewModel, SettingsOption, PreviewRow
    Views/                       SplashWindow, SettingsWindow, AboutWindow, HelpWindow
    Services/                    Settings, Localization, Theme, Dialog, Update, CrashLog
    Resources/Themes/            Light.xaml, Dark.xaml, Controls.xaml
    Resources/Strings/           Strings.en.xaml, Strings.fa.xaml, Strings.fr.xaml, Strings.ar.xaml
    Resources/Help/              Mockups.xaml (تصاویر برداری راهنمای درون‌برنامه‌ای)
    Assets/Brand/                لوگو و آیکون ویندوز
tests/
  RandoFile.Tests/               مجموعه xUnit برای موتور، برند، منابع و bindingهای XAML
installer/
  randofile.iss                  اسکریپت Inno Setup: نصب‌کننده‌ای که کاربران دانلود می‌کنند
  license.txt                    قرارداد، به انگلیسی
  license.farsi.txt              همان قرارداد، به فارسی
  license.french.txt             ... به فرانسوی
  license.arabic.txt             ... به عربی
  lang/                          رشته‌های جادوگر نصب: english, farsi, french, arabic
tools/
  Generate-Logo.ps1              نشان برند را می‌کشد و فایل‌های PNG/ICO را می‌سازد
  Sign-Release.ps1               انتشار را با گواهی خودتان امضا می‌کند
scripts-verify/
  verify-assoc.ps1               نصب per-user و بررسی همه کلیدهای رجیستری association
  verify-machine.ps1             همین کار برای نصب سراسری در Program Files
  verify-finish-page.ps1         جادوگر واقعی را تا صفحه آخر می‌برد و Finish را می‌زند
  verify-signing.ps1             اسکریپت امضا را با یک گواهی موقت اثبات می‌کند
  common.ps1                     کمک‌تابع‌های مشترک اسکریپت‌های association
```

پروژه `Core` هیچ وابستگی‌ای به WPF ندارد؛ همین باعث می‌شود منطق تغییر نام کاملاً قابل تست باشد.

---

## 🧪 تست‌ها

```powershell
dotnet test RandoFile.sln
```

مجموعه تست، الگوی نام‌گذاری، برنامه‌ریز (تشخیص تکراری، بازنویسی، بی‌اثر) و اجراکننده (جابه‌جایی‌ها، چرخه‌ها، بازگردانی، لغو، Undo، نبودن فایل موقت باقی‌مانده) را پوشش می‌دهد، به‌علاوه پویش پوشه، مقایسه نسخه برای بررسی به‌روزرسانی، هم‌ارزی منابع در همه چهار زبان، قواعد هویت برند و bindingهای XAML.

بعضی از این نگهبان‌ها به خاطر باگ‌هایی نوشته شده‌اند که به نسخه منتشرشده رسیده بودند:

- **بایند دوطرفه به خصوصیات فقط‌خواندنی.** WPF خصوصیاتی مثل `ProgressBar.Value`، `TextBox.Text` و `ToggleButton.IsChecked` را به‌صورت پیش‌فرض *دوطرفه* بایند می‌کند؛ پس اگر به خصوصیت محاسبشده‌ای در ViewModel وصلشان کنید، همان لحظه اولین چیدمان پنجره استثنا می‌دهد — برنامه به‌جای باز شدن دیالوگ «Problem» نشان می‌دهد. هر بایند جدید روی این خصوصیات باید `Mode=OneWay` صریح یا setter عمومی داشته باشد.
- **برند نباید به فایل‌های زبان راه پیدا کند.** `BrandIdentityTests` اگر «RandoFile» در مقداری از `Strings.*.xaml` بیاید شکست می‌خورد، چون آن‌وقت یک مترجم می‌تواند نامش را عوض کند یا حذفش کند.

اسپلش هم نگهبان زمان خودش را دارد: تا وقتی هیچ‌کدام از استوری‌بوردهایش تمام نشده‌اند صفحه بسته نمی‌شود، و خط اعتبار باید بعد از جاافتادن حداقل یک و نیم ثانیه خوانا بماند — تا هیچ لحظه‌ای نصفه قطع نشود و آغازین تبدیل به چشمک‌زدن نشود.

`InstallerScriptTests` هم اسکریپت Inno Setup را همین‌طور نگه می‌دارد. این اسکریپت فقط در CI کامپایل می‌شود، وگرنه این خطاها موقع انتشار سر باز می‌کردند: نسخه کهنه در نصب‌کننده، `AppId` که دیگر GUID نباشد (که بی‌صدا به هر کاربر یک نسخه دوم به‌جای ارتقا می‌دهد)، زبانی که از اسکریپت جا افتاده باشد، یا ترجمه نیمه‌کاره‌ای که Inno Setup بی‌صدا به انگلیسی برمی‌گرداندش.

خودِ جادوگر نصب هم با اجرا واقعی آزموده می‌شود. `scripts-verify/verify-assoc.ps1` با `/CURRENTUSER` (پس بدون پنجره UAC) و با وظیفه association نصب می‌کند، همه رکوردهای رجیستری را بازمی‌خواند، بعد بدون وظیفه نصب و حذف می‌کند — و در هر حال بررسی می‌شود دقیقاً همان کلیدهای درست موجودند. `scripts-verify/verify-machine.ps1` همین کار را برای نصب سراسری انجام می‌دهد که پوشه elevated لازم دارد. `scripts-verify/verify-finish-page.ps1` جادوگر واقعی را تا صفحه آخر می‌برد و تأیید می‌کند سه گزینه سر جایشان هستند و زدن Finish برنامه، پوشه نصب و صفحه پروژه را باز می‌کند. `scripts-verify/verify-signing.ps1` نسخه **کپی** فایل اجرایی منتشرشده را با گواهی‌ای امضا می‌کند که خودش می‌سازد و بعد پاک می‌کند، تا مسیر امضا روی سیستمی بدون هیچ گواهی‌ای هم اثبات شود.

---

## 📦 نسخه‌گذاری و انتشار

نسخه در یک جا نگه داشته می‌شود، [`Directory.Build.props`](Directory.Build.props)
(ویژگی `VersionPrefix`)، و در رابط کاربری به‌صورت `v1.0.0` نمایش داده می‌شود.

انتشارها از [Semantic Versioning](https://semver.org/) پیروی می‌کنند. انتشار خودکار است: با فرستادن تگ `v*`، فایل اجرایی ساخته می‌شود و GitHub Release ساخته می‌شود. چون خود برنامه `releases/latest` را می‌خواند و نسخه‌ها را با یک تحلیل‌گر SemVer نرمال مقایسه می‌کند، افزودن دانلود خودکارِ به‌روزرسانی یک قدم کوچک بعدی است.

### امضای کد

انتشارها **به‌صورت پیش‌فرض بدون امضا** منتشر می‌شوند، و به همین دلیل SmartScreen ویندوز تا وقتی گواهی تنظیم نشده درباره نصب‌کننده هشدار می‌دهد. وقتی گواهی باشد، workflow دو بار امضا می‌کند: `RandoFile.exe` منتشرشده را پیش از بسته‌بندی توسط Inno Setup، و نصب‌کننده را بعد از آن — یک برنامه امضاشده داخل یک نصب‌کننده بدون امضا به‌تنهایی دانلود را معتبر نمی‌کند. هر امضا مهر زمان دارد، پس بعد از انقضای گواهی هم معتبر می‌ماند.

دو secret در مخزن آن را روشن می‌کنند (*Settings → Secrets and variables → Actions*):

| Secret | مقدار |
|---|---|
| `WINDOWS_CERT_PFX_BASE64` | گواهی شما به‌صورت فایل `.pfx`، base64 شده |
| `WINDOWS_CERT_PASSWORD` | رمز عبور همان `.pfx` |

ساختن مورد اول از یک گواهی محلی:

```powershell
[Convert]::ToBase64String([IO.File]::ReadAllBytes('certificates\code-signing.pfx')) | Set-Clipboard
```

برای انتشاری که روی سیستم خودتان ساخته می‌شود، گواهی را در `certificates\code-signing.pfx` بگذارید — دقیقاً همان مسیری که
[`tools/Sign-Release.ps1`](tools/Sign-Release.ps1)
دنبالش می‌گردد، و `certificates/` را git نادیده می‌گیرد تا کلید خصوصی هرگز commit نشود — و اجرا کنید:

```powershell
tools\Sign-Release.ps1 -Files dist\installer\RandoFile-Setup-v1.0.0-win-x64.exe
```

اگر فایل رمز عبور دارد، `-Password <password>` را اضافه کنید (یا `RANDOFILE_CERT_PASSWORD` را تنظیم کنید).
وقتی گواهی به‌جای فایل، در انبار گواهی ویندوز نصب شده — جایی که توکن سخت‌افزاری یا کلاینت یک سرویس امضا آن را می‌گذارد — با thumbprint امضا کنید:

```powershell
tools\Sign-Release.ps1 -Files dist\win-x64\RandoFile.exe -Thumbprint <thumbprint>
```

از ژوئن ۲۰۲۳ یک مرجع صدور گواهی عموماً معتبر اجازه ندارد کلید خصوصی قابل استخراج صادر کند: باید روی توکن سخت‌افزاری، HSM یا یک سرویس امضای ابری زنده باشد. بنابراین گواهی عمومی معمولاً از همان مسیر `-Thumbprint` می‌رود و مسیر `.pfx` برای گواهی‌های self-signed و داخلی مناسب است. سرویس‌های ابری (Azure Trusted Signing، DigiCert KeyLocker، SSL.com eSigner) با ابزار خودشان امضا می‌کنند، که جای دو مرحله امضای workflow را می‌گیرد.

`scripts-verify/verify-signing.ps1` کل این مسیر را روی سیستمی بدون هیچ گواهی‌ای اثبات می‌کند: یک گواهی self-signed موقت می‌سازد، یک **کپی** از فایل اجرایی منتشرشده را امضا می‌کند، مهر زمانش را بررسی می‌کند، و بعد گواهی، فایل `.pfx` و همان کپی را دوباره پاک می‌کند.

تاریخچه انتشارها را در [CHANGELOG.md](CHANGELOG.md) ببینید.

---

## 👤 درباره سازنده

**Hossein Moradi** — HM
کاردان فنی مهندسی قدرت الکتریکی
مهندسی برق قدرت · توسعه‌دهنده نرم‌افزار و اپلیکیشن وب · طراح وب‌سایت · مدیریت و برنامه‌ریزی پروژه

---

## 📄 مجوز

MIT © 2026 Hossein Moradi — [LICENSE](LICENSE) را ببینید.

<br/>

<div align="center">

<img src="src/RandoFile.App/Assets/Brand/logo-mark.png" width="64" alt="RandoFile" />

<p>اگر RandoFile به کارتان آمد، به این مخزن یک ستاره ⭐ بدهید — واقعاً کمک می‌کند.</p>

<p>ساخته شده با 💛 توسط <b>Hossein Moradi</b></p>

</div>

</div>
