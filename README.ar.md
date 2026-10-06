<div dir="rtl">

<div align="center">

<img src="src/RandoFile.App/Assets/Brand/logo.png" width="128" alt="شعار RandoFile" />

<h1>🎲 RandoFile</h1>

<p><b>يخلط كل ملفات المجلد في ترتيب عشوائي حقيقي — عاين أولاً، ثم أعد التسمية بأمان، وارجع بضغطة واحدة.</b></p>

<p>
  <a href="https://github.com/hossein-moradi-sci/randofile/actions/workflows/ci.yml"><img src="https://github.com/hossein-moradi-sci/randofile/actions/workflows/ci.yml/badge.svg" alt="CI" /></a>
  <a href="https://github.com/hossein-moradi-sci/randofile/releases/latest"><img src="https://img.shields.io/github/v/release/hossein-moradi-sci/randofile?label=release" alt="أحدث إصدار" /></a>
  <a href="https://github.com/hossein-moradi-sci/randofile/releases/latest"><img src="https://img.shields.io/github/downloads/hossein-moradi-sci/randofile/total?label=downloads" alt="التنزيلات" /></a>
  <img src="https://img.shields.io/badge/Windows-10%20%2F%2011-0078D4" alt="Windows 10 / 11" />
  <img src="https://img.shields.io/badge/.NET-8.0-512BD4" alt=".NET 8" />
  <img src="https://img.shields.io/badge/license-MIT-34D058" alt="رخصة MIT" />
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

**RandoFile** يخلط كل ملف في المجلد بقرعة حقيقية، ثم يعيد تسمية المجموعة كاملة بنمط نظيف
قابل للتوقع — ويعرض لك معاينة كاملة لكل اسم قديم ← اسم جديد قبل أن يتغيّر أي اسم على قرصك.

يأتي كملف `.exe` واحد مستقل لويندوز ١٠ و ١١، ولا يعمل في المتصفح أبداً.

> **RandoFile** علامة تجارية، ولا تُترجم أبداً — تُكتب كما هي في كل اللغات.

> 💛 كُتب هذا الملف بالعربية مباشرة، لا ترجمة حرفيّة للإنجليزية؛ فيوجد العمق نفسه، ولكن
> بلغة تُقرأ بتيسّر.

---

## ✨ المزايا

| | |
|---|---|
| **اختيار مجلد بعداد حيّ** | اختر أي مجلد واعرف فوراً كم ملفاً وُجد. |
| **ترتيب عشوائي حقيقي** | خلط بخوارزمية Fisher–Yates، فالترتيب الجديد غير قابل للتخمين فعلاً. |
| **كل أنواع الملفات** | صور وفيديو ومستندات — يُحفظ الامتداد الأصلي دائماً. |
| **قوالب تسمية جاهزة** | `Image 1, Image 2 …` أو `File 1, File 2 …` أو بادئة من اختيارك. |
| **تحكم كامل بالترقيم** | رقم ابتداء وصفر اختياري، مثل `Image 007.jpg`. |
| **معاينة قبل التسمية** | تُعرض كل زوجة «قديم ← جديد» مع ملخّص حيّ للتغييرات. |
| **حماية من التكرار والطباعة** | إذا تضمّن الخطة تعارضاً رُفضت تماماً؛ لا يُعاد تسمية أي ملف. |
| **محرّك آمن** | إعادة تسمية على مرحلتين (تجهيز ← تنفيذ) تعمل مع التباديل ودورات الأسماء. |
| **تراجع تلقائي** | إن تعثّر ملف، يُرجَع كل ما سبق تسميته. |
| **تراجع (Undo)** | ضغطة واحدة تُعيد آخر تنفيذ ناجح إلى ما كان عليه. |
| **تقدّم وإلغاء** | شريط تقدّم وزر إلغاء يُرجع كل شيء بنظافة. |
| **شاشة افتتاح متحركة** | أربع ثوانٍ ونصف: يتشكّل الشعار قطعة قطعة، يستقر بارتداد خفيف، ثم يتنفّس بينما يضيء شارة الائتمان النيونية تحته. |
| **أربع لغات** | فارسی · English · Français · العربية، مع اتجاه كامل من اليمين لليسار للعربية والفارسية. |
| **السمات** | فاتحة، داكنة، أو اتباع إعدادات ويندوز. |
| **فحص التحديثات** | يقرأ إصدارات GitHub ويعرض شارة عند توفر إصدار أحدث. |

---

## ⬇️ التنزيل والتثبيت

احصل على أحدث إصدار من
[صفحة الإصدارات](https://github.com/hossein-moradi-sci/randofile/releases/latest) —
نقرتان وتنتهي.

**المثبّت (موصى به)**

1. نزّل `RandoFile-Setup-<version>-win-x64.exe`.
2. انقر عليه مرتين، اختر لغتك، واقبل الاتفاقية.
3. وافق على نافذة *User Account Control* الوحيدة في ويندوز.
4. اضغط Install.

يُثبَّت RandoFile في `C:\Program Files\RandoFile`، حيث يحتفظ ويندوز بالبرامج المثبّتة، مع
مدخل في قائمة Start واختصار مكتبي اختياري وقيد صحيح في *Apps & features* لإلغاء
التثبيت. **لا حاجة لتثبيت .NET**، وتركيب إصدار أحدث فوق أقدم يستبدله بهدوء. تُعرض
اتفاقية الترخيص **بلغة اخترتها للمثبّت** — أي بالعربية — وتُثبَّت كل الترجمات بجانب
الملف التنفيذي.

يقدّم المعالج أيضاً مهمتين اختياريتين: اختصار مكتبي، وتسجيل RandoFile في قائمة
**Open with** لكل المجلدات والملفات، وهو ما يتيح اختياره في *Default apps* في ويندوز —
لا شيء إجباري، وإلغاء التثبيت يزيل كل ما أضافه. وعند انتهاء المعالج تعرض صفحته الأخيرة
تشغيل RandoFile وفتح مجلد التثبيت وفتح صفحة هذا المشروع.

تفضّل بلا رفع للصلاحيات؟ يقبل المثبّت `/CURRENTUSER` فيُثبَّت داخل ملفك الشخصي بدل Program Files.

**النسخة المحمولة، إن فضّلت**

`RandoFile-<version>-win-x64.zip` هو البرنامج نفسه بلا مثبّت: فكّ الضغط أين شئت وشغّل `RandoFile.exe`.

كلتا النسختين مستقلة (*self-contained*) فتعمل على أي ويندوز ١٠ أو ١١.

### 🔐 التحقق من التنزيل

يحمل المثبّت و`RandoFile.exe` (الملف داخل الأرشيف) توقيعاً رقمياً، فتستطيع التأكد أن الملف
جاء فعلاً من هذا المشروع ولم يُعدّل. أما حاوية `.zip` فغير موقّعة — فكّها أولاً، ثم تحقّق
من الملف التنفيذي.

**بالشكل:** انقر بالزر الأيمن على الملف، اختر **Properties**، افتح تبويب **Digital Signatures**،
ثم حدد القيود واضغط **Details**.

**في PowerShell** (استبدل `<version>` بالإصدار الذي نزّلته، مثل `v1.0.0`):

```powershell
Get-AuthenticodeSignature .\RandoFile-Setup-<version>-win-x64.exe |
    Format-List Status, @{n='Signer';e={$_.SignerCertificate.Subject}},
                 @{n='Timestamped';e={$_.TimeStamperCertificate -ne $null}}
```

ما الذي تتوقع رؤيته:

- **Signer** — `CN=Hossein Moradi`، مطوّر المشروع.
- **Timestamped** — `True`، أي أن التوقيع يبقى صالحاً بعد انتهاء الشهادة.
- **Status** — تعني `Valid` أن ويندوز يثق بالجهة المُصدِرة. أما النسخ الموقّعة بشهادة
  المشروع فتُظهر `UnknownError`: التوقيع والطابع الزمني كلاهما سليم، لكن الجهة ليست من
  جذور الثقة العامة — وهو ردّ فعل SmartScreen بالضبط (راجع [توقيع الشيفرة](#توقيع-الشيفرة)).

---

## 🚀 الاستخدام

1. **Browse** — اختر المجلد الذي يحوي ملفاتك.
2. **اختر نمطاً** — ‏`Image` أو‏ `File` أو‏ `Custom` ببادئتك الخاصة.
3. **معاينة** — اضغط **Preview** لتعرف بالضبط أي ملف يصير أي اسم جديد.
4. **Randomize** — اضغط **Randomize** وأكّد. شريط التقدّم يتبع التنفيذ.
5. **Undo** — إن لم يعجبك النتيجة، اضغط **Undo** لرجوع الأسماء الأصلية.

الإعدادات (اللغة والسمة ومسح المجلدات الفرعية وفحص التحديث) في نافذة **Settings**؛ ومعلومات
المطوّر والإصدار في نافذة **About**.

يوجد دليل مصوّر قصير داخل البرنامج نفسه: اضغط <kbd>F1</kbd> أو زر `?` في الشريط العلوي.
يغطي الخطوات الأربع السابقة وخيارات التسمية واختصارات لوحة المفاتيح وضمانات الأمان
وأكثر الأسئلة تكراراً — بالأربع اللغات نفسها لبقية الواجهة.

---

## 🏷️ أنماط التسمية

| النمط | النتيجة |
|---|---|
| Image | `Image 1.jpg`، ‏`Image 2.jpg`، ‏`Image 3.png`، … |
| File | `File 1.docx`، ‏`File 2.mp4`، ‏`File 3.txt`، … |
| Custom | `<your prefix> 1.ext`، ‏`<your prefix> 2.ext`، … |

بعدد ابتداء `101` وعدد خانات `3` ونمط `Image`، يصير الملف الأول `Image 101.jpg` والثاني
`Image 102.jpg` وهكذا.

---

## 🛡️ كيف تبقى التسمية آمنة

إعادة تسمية ملفات داخل مجلد واحد ليست نسخاً بسيطاً واحداً لواحد — قد تتبادل الأسماء أو
تصنع دورة (`a → b → c → a`). لذلك يعمل المحرّك بمرحلتين:

1. **التجهيز** — ينتقل كل ملف إلى اسم مؤقت فريد (`.rf_tmp_<guid><ext>`).
2. **التنفيذ** — بعد التأكد من عدم وجود أي تعارض، تأخذ الأسماء المؤقتة أسماءها النهائية.

وإن تعثّر شيء في الطريق، تُلغى المرحلة المنجزة تلقائياً. الآلية نفسها تدعم أمر **Undo**.
وتُتجاهل الملفات المخفية والنظام والمؤقتة المتبقية عند المسح.

---

## 🎨 أصول العلامة التجارية

![RandoFile logo](src/RandoFile.App/Assets/Brand/logo.png)

يتكوّن الشعار من ثلاث بطاقات مستندات مرتبة على شكل مروحة، فوقها قطعة حلقة توحي بحركة العشوائية.
مستقلة لا تتبع أي بلاطة عامة، فشكلها المميّز يُعرف وحده، وبُنيت عمداً من أجزاء منفصلة —
تشتق شاشة الافتتاح الأشكال نفسها كمتجهات لتحريكها واحدة تلو الأخرى، ثم تستقر على العلامة
وتنفّس إلى الأبد.

| الملف | الاستخدام |
|---|---|
| `src/RandoFile.App/Assets/Brand/logo.png` | الشعار الأساسي ٥١٢ بكسل على خلفيته الداكنة |
| `src/RandoFile.App/Assets/Brand/logo-mark.png` | العلامة الشفافة المستخدمة في الواجهة |
| `src/RandoFile.App/Assets/Brand/randofile.ico` | أيقونة ويندوز من ١٦ إلى ٢٥٦ بكسل على الخلفية نفسها |

الثلاثة مولّدة لا مرسومة يدوياً. لتغيير العلامة عدّل
[`tools/Generate-Logo.ps1`](tools/Generate-Logo.ps1) وشغّله:

```powershell
powershell -ExecutionPolicy Bypass -File tools/Generate-Logo.ps1
```

---

## 🛠️ البناء من المصدر

المتطلبات: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) وويندوز.

```powershell
git clone https://github.com/hossein-moradi-sci/randofile.git
cd randofile

dotnet restore RandoFile.sln
dotnet build   RandoFile.sln -c Release --no-restore
dotnet test    RandoFile.sln -c Release --no-build
```

### إنتاج ملف `.exe` الواحد

```powershell
dotnet publish src/RandoFile.App/RandoFile.App.csproj `
  -c Release -r win-x64 --self-contained true `
  -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true `
  -o dist/win-x64
```

الناتج ملف واحد مستقل `dist/win-x64/RandoFile.exe` (نحو ٦٥ ميغابايت) يعمل على أي ويندوز ١٠ أو ١١ دون runtime من .NET.

---

## 📁 بنية المشروع

```
RandoFile.sln
Directory.Build.props            الإصدار المشترك وبيانات assembly
src/
  RandoFile.Core/                المحرّك، بلا اعتماد على الواجهة
    Models/                      NamingPattern, RenamePlan, RenameProgress, RenameResult
    Services/                    FileShuffler, FileScanner, RenamePlanBuilder, RenameExecutor
    Abstractions/                IFileScanner, IFileShuffler
    CommandLineTarget.cs         يحوّل المسار الذي يسلّمه ويندوز إلى مجلد يُفتح
    AppInfo.cs                   العلامة واسم المنتج والإصدار وهوية المستودع
  RandoFile.App/                 تطبيق WPF
    ViewModels/                  MainViewModel, SettingsOption, PreviewRow
    Views/                       SplashWindow, SettingsWindow, AboutWindow, HelpWindow
    Services/                    Settings, Localization, Theme, Dialog, Update, CrashLog
    Resources/Themes/            Light.xaml, Dark.xaml, Controls.xaml
    Resources/Strings/           Strings.en.xaml, Strings.fa.xaml, Strings.fr.xaml, Strings.ar.xaml
    Resources/Help/              Mockups.xaml (رسوم متجهية للمساعدة داخل البرنامج)
    Assets/Brand/                الشعار وأيقونة ويندوز
tests/
  RandoFile.Tests/               حزمة xUnit للمحرّك والعلامة والموارد وربطات XAML
installer/
  randofile.iss                  سكربت Inno Setup: المثبّت الذي ينزّله المستخدمون من الإصدار
  license.txt                    الاتفاقية بالإنجليزية
  license.farsi.txt              الاتفاقية نفسها بالفارسية
  license.french.txt             ... بالفرنسية
  license.arabic.txt             ... بالعربية
  lang/                          نصوص المعالج: english, farsi, french, arabic
tools/
  Generate-Logo.ps1              يرسم علامة المشروع ويكتب ملفات PNG/ICO
  Sign-Release.ps1               يوقّع الإصدار بشهادتك الخاصة
scripts-verify/
  verify-assoc.ps1               يثبّت للمستخدم الحالي ويقرأ كل مفاتيح التسجيل
  verify-machine.ps1             الشيء نفسه على تثبيت على مستوى الجهاز
  verify-finish-page.ps1         يقود المعالج الحقيقي حتى صفحته الأخيرة ويضغط Finish
  verify-signing.ps1             يثبت سكربت التوقيع بشهادة مؤقتة
  common.ps1                     دوال مشتركة بين سكربتات التسجيل
```

مشروع `Core` بلا أي اعتماد على WPF، ولهذا فإن منطق إعادة التسمية قابل للاختبار بالكامل.

---

## 🧪 الاختبارات

```powershell
dotnet test RandoFile.sln
```

تغطي الحزمة نمط التسمية والمخطّط (كشف التكرار والتكرار والاستبدال والحالات التي لا تتطلب تغييراً) والمنفّذ (التباديل
والدورات والتراجع والإلغاء وUndo وعدم بقاء ملفات مؤقتة) ومسح المجلدات ومقارنة الإصدارات
لفحص التحديث وتكافؤ الموارد بين اللغات الأربع وقواعد هوية العلامة وربطات XAML.

بعض هذه الحواجز وُجدت بسبب أخطاء وصلت إلى نسخ منشورة:

- **ربط ثنائي الاتجاه بخصائص للقراءة فقط.** يربط WPF خصائص مثل `ProgressBar.Value` و`TextBox.Text`
  و`ToggleButton.IsChecked` افتراضياً *ثنائي الاتجاه*، فأي ربط لها بخاصية
  محسوبة في النموذج يرمي استثناءً عند أول تخطيط للنافذة — يعرض التطبيق حوار «Problem» بدل
  أن يُفتح. أي ربط جديد عليها يجب أن يصرّح بـ `Mode=OneWay` أو يوفّر setter عمومياً.
- **العلامة يجب ألا تدخل ملفات اللغة.** يفشل `BrandIdentityTests` إن ظهر «RandoFile» في
  قيمة داخل `Strings.*.xaml`، لأن مترجماً يمكنه عندها تغيير اسمه أو حذفه.

لشاشة الافتتاح حارس توقيت خاص: لا تُغلق قبل انتهاء كل storyboard فيها، ويجب أن يبقى سطر
الائتمان مستقراً ومقروءاً ثانية ونصف على الأقل بعد ذلك، فلا يُبتر أي إيقاع في منتصفه
ليتحوّل الافتتاح إلى ومضة.

يحرس `InstallerScriptTests` سكربت Inno Setup بنفس الطريقة. لا يُبنى إلا في CI، ولو لا
ذلك لظهرت هذه الأخطاء وقت الإصدار: إصدار قديم في المثبّت، أو `AppId` لم يعُد GUID (مما
يمنح كل مستخدم نسخة ثانية بهدوء بدل الترقية)، أو لغة ناقصة من السكربت، أو ترجمة نصف
منجزة يعيدها Inno Setup بهدوء إلى الإنجليزية.

ويُختبر المعالج نفسه بالتشغيل الفعلي. `scripts-verify/verify-assoc.ps1` يثبّت للمستخدم
الحالي (`/CURRENTUSER` فبلا UAC) مع مهمة التسجيل، ويعيد قراءة كل قيد في التسجيل، ثم يثبّت
بلا مهمة ويُلغي التثبيت — مع التحقق في كل مرة أن المفاتيح الصحيحة بالضبط موجودة.
و`scripts-verify/verify-machine.ps1` يفعل الشيء نفسه لتثبيت على مستوى الجهاز الذي يحتاج
صلاحية مرتفعة. و`scripts-verify/verify-finish-page.ps1` يقود المعالج الحقيقي حتى صفحته
الأخيرة ويؤكد أن الخيارات الثلاثة موجودة وأن الضغط على Finish يفتح البرنامج ومجلد
التثبيت وصفحة المشروع. أما `scripts-verify/verify-signing.ps1` فيوقّع **نسخة** من الملف
التنفيذي بشهادة يصنعها ثم يحذفها، فتثبت مسار التوقيع على جهاز لا يملك شهادة أصلاً.

---

## 📦 الإصدارات والإنتشار

الإصدار يُحفظ في مكان واحد، [`Directory.Build.props`](Directory.Build.props)
(خاصية `VersionPrefix`)، ويظهر في الواجهة كـ `v1.0.0`.

تتبع الإصدارات [Semantic Versioning](https://semver.org/). والنشر آلي: إرسال وسم `v*`
يبني الملف التنفيذي ويُنشئ إصداراً على GitHub. ولأن البرنامج يقرأ `releases/latest` أصلاً
ويقارن الإصدارات بمحلّل SemVer معياري، فإن إضافة تنزيل تحديث آلي خطوة صغيرة تالية.

### توقيع الشيفرة

تُنشر الإصدارات **غير موقّعة افتراضياً**، ولهذا يحذّر SmartScreen في ويندوز من المثبّت ما
دام لم يُضبط شهادة. وعند ضبطها يوقّع workflow مرتين: الملف `RandoFile.exe` المنشور قبل أن
يحزمه Inno Setup، ثم المثبّت بعده — برنامج موقّع داخل مثبّت غير موقّع لا يجعل التنزيل
موثوقاً بذاته. وكل توقيع مختوم بالوقت فيبقى صالحاً بعد انتهاء الشهادة.

سِرّان (Secret) يفعّلان ذلك (*Settings → Secrets and variables → Actions*):

| Secret | القيمة |
|---|---|
| `WINDOWS_CERT_PFX_BASE64` | شهادتك بصيغة `.pfx` مُرمّزة base64 |
| `WINDOWS_CERT_PASSWORD` | كلمة مرور ذلك `.pfx` |

لإنتاج الأول من شهادة محلية:

```powershell
[Convert]::ToBase64String([IO.File]::ReadAllBytes('certificates\code-signing.pfx')) | Set-Clipboard
```

لإصدار يُبنى على جهازك، ضع الشهادة في `certificates\code-signing.pfx` — وهو المسار بالضبط
الذي يبحث عنه [`tools/Sign-Release.ps1`](tools/Sign-Release.ps1)، ومجلد `certificates/`
متجاهل في git فلا تُرفع المفتاح الخاص أبداً — ثم نفّذ:

```powershell
tools\Sign-Release.ps1 -Files dist\installer\RandoFile-Setup-v1.0.0-win-x64.exe
```

إن كان الملف محمياً بكلمة مرور أضف `-Password <password>` (أو اضبط `RANDOFILE_CERT_PASSWORD`).
وإذا كانت الشهادة مثبّتة في مخزن شهادات ويندوز — وهو مكان رمز مادي أو عميل خدمة توقيع —
فوقّع بالبصمة:

```powershell
tools\Sign-Release.ps1 -Files dist\win-x64\RandoFile.exe -Thumbprint <thumbprint>
```

منذ يونيو ٢٠٢٣ لا يجوز لجهة إصدار عامة موثوقة إصدار مفتاح خاص قابل للتصدير: يجب أن يعيش
على رمز مادي أو HSM أو خدمة توقيع سحابية. لذا فالشهادة العامة تسلك عادة مسار `-Thumbprint`،
ومسار `.pfx` مناسب للشهادات الذاتية والداخلية. أما خدمات السحابة (Azure Trusted Signing،
DigiCert KeyLocker، SSL.com eSigner) فتوقّع بأداة خاصة بها، которая تحل محل خطوتَي التوقيع
في الـ workflow.

يثبت `scripts-verify/verify-signing.ps1` المسار كله على جهاز بلا شهادة إطلاقاً: يصنع شهادة
ذاتية مؤقتة، ووقّع **نسخة** من الملف التنفيذي المنشور، ويتأكد من وجود طابع زمني، ثم يحذف
الشهادة و`.pfx` والنسخة مجدداً.

راجع [CHANGELOG.md](CHANGELOG.md) لتاريخ الإصدارات.

---

## 👤 عن المطوّر

**Hossein Moradi** — HM
تقني هندسة كهربائية – قوى
الهندسة الكهربائية – القوى · مطوّر تطبيقات وبرمجيات ويب · مصمم مواقع · إدارة وتخطيط مشاريع

---

## 📄 الرخصة

MIT © 2026 Hossein Moradi — راجع [LICENSE](LICENSE).

<br/>

<div align="center">

<img src="src/RandoFile.App/Assets/Brand/logo-mark.png" width="64" alt="RandoFile" />

<p>إن نفعك RandoFile، فامنح المستودع نجمة ⭐ — تفيد فعلاً.</p>

<p>صُنع بـ 💛 بواسطة <b>Hossein Moradi</b></p>

</div>

</div>
