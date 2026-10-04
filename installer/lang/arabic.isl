; Inno Setup - Arabic wizard strings for RandoFile.
; RightToLeft=yes mirrors the wizard controls and reverses text alignment.
; Save this file as UTF-8 with a byte order mark, otherwise the compiler reads
; it as ANSI and the Arabic text is mangled.

[LangOptions]
LanguageName=العربية
LanguageID=$0401
LanguageCodePage=1256
RightToLeft=yes
DialogFontName=Segoe UI
DialogFontSize=9
DialogFontBaseScaleWidth=7
DialogFontBaseScaleHeight=15
WelcomeFontName=Segoe UI
WelcomeFontSize=14

[Messages]

SetupAppTitle=التثبيت
SetupWindowTitle=التثبيت ‎- %1
UninstallAppTitle=إلغاء التثبيت
UninstallAppFullTitle=إلغاء تثبيت ‎%1

InformationTitle=معلومات
ConfirmTitle=تأكيد
ErrorTitle=خطأ

SetupLdrStartupMessage=سيؤدي هذا إلى تثبيت %1‎. هل تريد المتابعة؟
LdrCannotCreateTemp=تعذّر إنشاء ملف مؤقت. تم إلغاء التثبيت
LdrCannotExecTemp=تعذّر تنفيذ الملف في المجلد المؤقت. تم إلغاء التثبيت
HelpTextNote=

LastErrorMessage=%1.%n%nالخطأ %2‎: %3
SetupFileMissing=الملف %1‎ مفقود من مجلد التثبيت. يرجى تصحيح المشكلة أو الحصول على نسخة جديدة من البرنامج.
SetupFileCorrupt=ملفات التثبيت تالفة. يرجى الحصول على نسخة جديدة من البرنامج.
SetupFileCorruptOrWrongVer=ملفات التثبيت تالفة أو غير متوافقة مع إصدار التثبيت هذا. يرجى تصحيح المشكلة أو الحصول على نسخة جديدة من البرنامج.
InvalidParameter=تم تمرير معامل غير صالح في سطر الأوامر:%n%n%1
SetupAlreadyRunning=التثبيت قيد التشغيل بالفعل.
WindowsVersionNotSupported=لا يدعم هذا البرنامج إصدار ويندوز الذي يعمل عليه حاسوبك.
WindowsServicePackRequired=يتطلب هذا البرنامج حزمة الخدمة %1‎ رقم %2‎ أو أحدث.
NotOnThisPlatform=لن يعمل هذا البرنامج على %1‎.
OnlyOnThisPlatform=يجب تشغيل هذا البرنامج على %1‎.
OnlyOnTheseArchitectures=يمكن تثبيت هذا البرنامج فقط على إصدارات ويندوز المصممة لبنيات المعالجات التالية:%n%n%1
WinVersionTooLowError=يتطلب هذا البرنامج %1‎ الإصدار %2‎ أو أحدث.
WinVersionTooHighError=لا يمكن تثبيت هذا البرنامج على %1‎ الإصدار %2‎ أو أحدث.
AdminPrivilegesRequired=يجب أن تكون مسجّل الدخول كمسؤول لتثبيت هذا البرنامج.
PowerUserPrivilegesRequired=يجب أن تكون مسجّل الدخول كمسؤول أو كعضو في مجموعة Power Users لتثبيت هذا البرنامج.
SetupAppRunningError=اكتشف التثبيت أن %1‎ قيد التشغيل حاليًا.%n%nيرجى إغلاق كل نسخه الآن، ثم انقر على موافق للمتابعة أو إلغاء للخروج.
UninstallAppRunningError=اكتشف إلغاء التثبيت أن %1‎ قيد التشغيل حاليًا.%n%nيرجى إغلاق كل نسخه الآن، ثم انقر على موافق للمتابعة أو إلغاء للخروج.

PrivilegesRequiredOverrideTitle=اختر طريقة التثبيت
PrivilegesRequiredOverrideInstruction=اختر طريقة التثبيت
PrivilegesRequiredOverrideText1=يمكن تثبيت %1‎ لجميع المستخدمين (يتطلب صلاحيات إدارية) أو لك وحدك.
PrivilegesRequiredOverrideText2=يمكن تثبيت %1‎ لك وحدك أو لجميع المستخدمين (يتطلب صلاحيات إدارية).
PrivilegesRequiredOverrideAllUsers=التثبيت &لكل المستخدمين
PrivilegesRequiredOverrideAllUsersRecommended=التثبيت &لكل المستخدمين (موصى به)
PrivilegesRequiredOverrideCurrentUser=التثبيت &لي وحدي
PrivilegesRequiredOverrideCurrentUserRecommended=التثبيت &لي وحدي (موصى به)

ErrorCreatingDir=تعذّر على التثبيت إنشاء المجلد ‎«%1»
ErrorTooManyFilesInDir=تعذّر إنشاء ملف في المجلد ‎«%1»‎ لأنه يحتوي على عدد كبير من الملفات

ExitSetupTitle=إنهاء التثبيت
ExitSetupMessage=لم يكتمل التثبيت. إذا خرجت الآن فلن يتم تثبيت البرنامج.%n%nيمكنك تشغيل التثبيت لاحقًا لإكمال العملية.%n%nهل تريد الخروج من التثبيت؟
AboutSetupMenuItem=&حول التثبيت...
AboutSetupTitle=حول التثبيت
AboutSetupMessage=%1 %2%n%3%n%nالصفحة الرئيسية لـ %1‎:%n%4
AboutSetupNote=
TranslatorNote=

ButtonBack=<السابق &‏
ButtonNext=التالي&‏ >
ButtonInstall=&تثبيت
ButtonOK=موافق
ButtonCancel=إلغاء
ButtonYes=نعم&
ButtonYesToAll=نعم إلى &الكل
ButtonNo=لا&
ButtonNoToAll=لا إلى &الكل
ButtonFinish=&إنهاء
ButtonBrowse=استعراض&...
ButtonWizardBrowse=است&عراض...
ButtonNewFolder=إنشاء مجلد &جديد

SelectLanguageTitle=اختر لغة التثبيت
SelectLanguageLabel=اختر اللغة التي تريد استخدامها أثناء التثبيت.

ClickNext=انقر على التالي للمتابعة، أو على إلغاء للخروج من التثبيت.
BeveledLabel=
BrowseDialogTitle=استعراض مجلد
BrowseDialogLabel=اختر مجلدًا من القائمة أدناه، ثم انقر على موافق.
NewFolderName=مجلد جديد

WelcomeLabel1=مرحبًا بك في معالج تثبيت ‎[name]
WelcomeLabel2=سيؤدي هذا إلى تثبيت ‎[name/ver]‎ على حاسوبك.%n%nيُنصح بإغلاق بقية التطبيقات قبل المتابعة.

WizardPassword=كلمة المرور
PasswordLabel1=هذا التثبيت محمي بكلمة مرور.
PasswordLabel3=يرجى إدخال كلمة المرور، ثم انقر على التالي للمتابعة. كلمات المرور حساسة لحالة الأحرف.
PasswordEditLabel=&كلمة المرور:
IncorrectPassword=كلمة المرور التي أدخلتها غير صحيحة. يرجى المحاولة مرة أخرى.

WizardLicense=اتفاقية الترخيص
LicenseLabel=يرجى قراءة المعلومات المهمة التالية قبل المتابعة.
LicenseLabel3=يرجى قراءة اتفاقية الترخيص التالية. يجب أن تقبل شروطها قبل متابعة التثبيت.
LicenseAccepted=أ&قبل الاتفاقية
LicenseNotAccepted=لا أ&قبل الاتفاقية

WizardInfoBefore=معلومات
InfoBeforeLabel=يرجى قراءة المعلومات المهمة التالية قبل المتابعة.
InfoBeforeClickLabel=عندما تكون جاهزًا لمتابعة التثبيت، انقر على التالي.
WizardInfoAfter=معلومات
InfoAfterLabel=يرجى قراءة المعلومات المهمة التالية قبل المتابعة.
InfoAfterClickLabel=عندما تكون جاهزًا لمتابعة التثبيت، انقر على التالي.

WizardUserInfo=معلومات المستخدم
UserInfoDesc=يرجى إدخال معلوماتك.
UserInfoName=&اسم المستخدم:
UserInfoOrg=&المنشأة:
UserInfoSerial=&الرقم التسلسلي:
UserInfoNameRequired=يجب إدخال اسم.

WizardSelectDir=اختر وجهة التثبيت
SelectDirDesc=أين ينبغي تثبيت ‎[name]‎؟
SelectDirLabel3=سيثبّت التثبيت ‎[name]‎ في المجلد التالي.
SelectDirBrowseLabel=للمتابعة، انقر على التالي. وإذا أردت اختيار مجلد آخر، انقر على استعراض.
DiskSpaceGBLabel=يلزم ‎[gb]‎ غيغابايت على الأقل من المساحة الحرة.
DiskSpaceMBLabel=يلزم ‎[mb]‎ ميغابايت على الأقل من المساحة الحرة.
CannotInstallToNetworkDrive=لا يمكن التثبيت على محرك شبكي.
CannotInstallToUNCPath=لا يمكن التثبيت في مسار UNC.
InvalidPath=يجب إدخال مسار كامل مع حرف المحرك، على سبيل المثال:%n%nC:\App%n%nou مسار UNC بالشكل التالي:%n%n\\server\share
InvalidDrive=المحرك أو المورد UNC الذي اخترته غير موجود أو غير متاح. يرجى اختيار آخر.
DiskSpaceWarningTitle=لا توجد مساحة كافية على القرص
DiskSpaceWarning=يتطلب التثبيت ‎%1‎ كيلوبايت على الأقل من المساحة الحرة، لكن المحرك المختار فيه ‎%2‎ كيلوبايت فقط.%n%nهل تريد المتابعة على أي حال؟
DirNameTooLong=اسم المجلد أو المسار طويل جدًا.
InvalidDirName=اسم المجلد غير صالح.
BadDirName32=لا يمكن أن يحتوي اسم المجلد على أي من الأحرف التالية:%n%n%1
DirExistsTitle=المجلد موجود
DirExists=المجلد:%n%n%1%n%nmوجود بالفعل. هل تريد التثبيت في هذا المجلد على أي حال؟
DirDoesntExistTitle=المجلد غير موجود
DirDoesntExist=المجلد:%n%n%1%n%nغير موجود. هل تريد إنشاءه؟

WizardSelectComponents=اختر المكونات
SelectComponentsDesc=ما المكونات التي ينبغي تثبيتها؟
SelectComponentsLabel2=اختر المكونات التي تريد تثبيتها، وأزل تحديد ما لا تريد تثبيته. انقر على التالي عندما تكون جاهزًا.
FullInstallation=تثبيت كامل
CompactInstallation=تثبيت مضغوط
CustomInstallation=تثبيت مخصص
NoUninstallWarningTitle=هناك مكونات موجودة
NoUninstallWarning=اكتشف التثبيت أن المكونات التالية مثبتة بالفعل على حاسوبك:%n%n%1%n%nإزالة تحديد هذه المكونات لن تزيلها.%n%nهل تريد المتابعة على أي حال؟
ComponentSize1=%1 كيلوبايت
ComponentSize2=%1 ميغابايت
ComponentsDiskSpaceGBLabel=يتطلب الاختيار الحالي ‎[gb]‎ غيغابايت على الأقل من مساحة القرص.
ComponentsDiskSpaceMBLabel=يتطلب الاختيار الحالي ‎[mb]‎ ميغابايت على الأقل من مساحة القرص.

WizardSelectTasks=اختر المهام الإضافية
SelectTasksDesc=ما المهام الإضافية التي ينبغي تنفيذها؟
SelectTasksLabel2=اختر المهام الإضافية التي تريد أن ينفذها التثبيت أثناء تثبيت ‎[name]‎، ثم انقر على التالي.

WizardSelectProgramGroup=اختر مجلد قائمة ابدأ
SelectStartMenuFolderDesc=أين ينبغي للتثبيت وضع اختصارات البرنامج؟
SelectStartMenuFolderLabel3=سينشئ التثبيت اختصارات البرنامج في مجلد قائمة ابدأ التالي.
SelectStartMenuFolderBrowseLabel=للمتابعة، انقر على التالي. وإذا أردت اختيار مجلد آخر، انقر على استعراض.
MustEnterGroupName=يجب إدخال اسم مجلد.
GroupNameTooLong=اسم المجلد أو المسار طويل جدًا.
InvalidGroupName=اسم المجلد غير صالح.
BadGroupName=لا يمكن أن يحتوي اسم المجلد على أي من الأحرف التالية:%n%n%1
NoProgramGroupCheck2=لا تنشئ مجلدًا في &قائمة ابدأ

WizardReady=جاهز للتثبيت
ReadyLabel1=التثبيت مستعد الآن لبدء تثبيت ‎[name]‎ على حاسوبك.
ReadyLabel2a=انقر على «تثبيت» لمتابعة التثبيت، أو على «السابق» إذا أردت مراجعة الإعدادات أو تغييرها.
ReadyLabel2b=انقر على «تثبيت» لمتابعة التثبيت.
ReadyMemoUserInfo=معلومات المستخدم:
ReadyMemoDir=جهة التثبيت:
ReadyMemoType=نوع التثبيت:
ReadyMemoComponents=المكونات المحددة:
ReadyMemoGroup=مجلد قائمة ابدأ:
ReadyMemoTasks=المهام الإضافية:

DownloadingLabel2=جارٍ تنزيل الملفات...
ButtonStopDownload=إيقاف &التنزيل
StopDownload=هل أنت متأكد من إيقاف التنزيل؟
ErrorDownloadAborted=تم إلغاء التنزيل
ErrorDownloadFailed=فشل التنزيل: %1 %2
ErrorDownloadSizeFailed=تعذّر الحصول على الحجم: %1 %2
ErrorProgress=تقدم غير صالح: %1 من %2
ErrorFileSize=حجم الملف غير صالح: المتوقع %1‎ وال الموجود %2

ExtractingLabel=جارٍ استخراج الملفات...
ButtonStopExtraction=إيقاف &الاستخراج
StopExtraction=هل أنت متأكد من إيقاف الاستخراج؟
ErrorExtractionAborted=تم إلغاء الاستخراج
ErrorExtractionFailed=فشل الاستخراج: %1

ArchiveIncorrectPassword=كلمة المرور غير صحيحة
ArchiveIsCorrupted=الأرشيف تالف
ArchiveUnsupportedFormat=صيغة الأرشيف غير مدعومة

WizardPreparing=التحضير للتثبيت
PreparingDesc=التثبيت يحضّر تثبيت ‎[name]‎ على حاسوبك.
PreviousInstallNotCompleted=لم يكتمل تثبيت أو إزالة برنامج سابق. ستحتاج إلى إعادة تشغيل حاسوبك لإتمام تلك العملية.%n%nبعد إعادة التشغيل، شغّل التثبيت مرة أخرى لإكمال تثبيت ‎[name].
CannotContinue=لا يمكن للتثبيت المتابعة. يرجى النقر على إلغاء للخروج.
ApplicationsFound=تستخدم التطبيقات التالية ملفات تحتاج إلى تحديث. يُنصح بالسماح للتثبيت بإغلاقها تلقائيًا.
ApplicationsFound2=تستخدم التطبيقات التالية ملفات تحتاج إلى تحديث. يُنصح بالسماح للتثبيت بإغلاقها تلقائيًا. وبعد انتهاء التثبيت سيحاول إعادة تشغيل التطبيقات.
CloseApplications=&أغلق التطبيقات تلقائيًا
DontCloseApplications=لا &تغلق التطبيقات
ErrorCloseApplications=تعذّر على التثبيت إغلاق كل التطبيقات تلقائيًا. يُنصح بإغلاق كل التطبيقات التي تستخدم الملفات المطلوب تحديثها قبل المتابعة.
PrepareToInstallNeedsRestart=يجب أن يعيد التثبيت تشغيل حاسوبك. بعد إعادة التشغيل، شغّل التثبيت مرة أخرى لإكمال تثبيت ‎[name].%n%nهل تريد إعادة التشغيل الآن؟

WizardInstalling=جارٍ التثبيت
InstallingLabel=يرجى الانتظار بينما يثبت التثبيت ‎[name]‎ على حاسوبك.

FinishedHeadingLabel=إتمام معالج تثبيت ‎[name]
FinishedLabelNoIcons=اكتمل تثبيت ‎[name]‎ على حاسوبك.
FinishedLabel=اكتمل تثبيت ‎[name]‎ على حاسوبك. ويمكن تشغيل التطبيق باختيار الاختصارات المثبتة.
ClickFinish=انقر على «إنهاء» للخروج من التثبيت.
FinishedRestartLabel=لإتمام تثبيت ‎[name]‎، يجب أن يعيد التثبيت تشغيل حاسوبك. هل تريد إعادة التشغيل الآن؟
FinishedRestartMessage=لإتمام تثبيت ‎[name]‎، يجب أن يعيد التثبيت تشغيل حاسوبك.%n%nهل تريد إعادة التشغيل الآن؟
ShowReadmeCheck=نعم، أود عرض ملف README
YesRadio=&نعم، أعد تشغيل الحاسوب الآن
NoRadio=&لا، سأعيد تشغيل الحاسوب لاحقًا

RunEntryExec=تشغيل %1
RunEntryShellExec=عرض %1

ChangeDiskTitle=التثبيت يحتاج إلى القرص التالي
SelectDiskLabel2=يرجى إدخال القرص %1‎ والنقر على موافق.%n%nإذا وُجدت ملفات هذا القرص في مجلد غير المجلد المعروض أدناه، فأدخل المسار الصحيح أو انقر على استعراض.
PathLabel=&المسار:
FileNotInDir2=لم يُعثر على الملف ‎«%1»‎ في ‎«%2». يرجى إدخال القرص الصحيح أو اختيار مجلد آخر.
SelectDirectoryLabel=يرجى تحديد موقع القرص التالي.

SetupAborted=لم يكتمل التثبيت.%n%nيرجى تصحيح المشكلة وتشغيل التثبيت مرة أخرى.
AbortRetryIgnoreSelectAction=اختر إجراء
AbortRetryIgnoreRetry=&حاول مرة أخرى
AbortRetryIgnoreIgnore=&تجاهل الخطأ وتابع
AbortRetryIgnoreCancel=إلغاء التثبيت
RetryCancelSelectAction=اختر إجراء
RetryCancelRetry=&حاول مرة أخرى
RetryCancelCancel=إلغاء

StatusClosingApplications=جارٍ إغلاق التطبيقات...
StatusCreateDirs=جارٍ إنشاء المجلدات...
StatusExtractFiles=جارٍ استخراج الملفات...
StatusDownloadFiles=جارٍ تنزيل الملفات...
StatusCreateIcons=جارٍ إنشاء الاختصارات...
StatusCreateIniEntries=جارٍ إنشاء مدخلات INI...
StatusCreateRegistryEntries=جارٍ إنشاء مدخلات السجل...
StatusRegisterFiles=جارٍ تسجيل الملفات...
StatusSavingUninstall=جارٍ حفظ معلومات إلغاء التثبيت...
StatusRunProgram=جارٍ إنهاء التثبيت...
StatusRestartingApplications=جارٍ إعادة تشغيل التطبيقات...
StatusRollback=جارٍ التراجع عن التغييرات...

ErrorInternal2=خطأ داخلي: %1
ErrorFunctionFailedNoCode=فشل %1
ErrorFunctionFailed=فشل %1‎؛ الرمز %2
ErrorFunctionFailedWithMessage=فشل %1‎؛ الرمز %2.%n%3
ErrorExecutingProgram=تعذّر تنفيذ الملف:%n%1

ErrorRegOpenKey=خطأ في فتح مفتاح السجل:%n%1\%2
ErrorRegCreateKey=خطأ في إنشاء مفتاح السجل:%n%1\%2
ErrorRegWriteKey=خطأ في الكتابة إلى مفتاح السجل:%n%1\%2

ErrorIniEntry=خطأ في إنشاء مدخل INI في الملف ‎«%1».

FileAbortRetryIgnoreSkipNotRecommended=&تخطَّ هذا الملف (غير مستحسن)
FileAbortRetryIgnoreIgnoreNotRecommended=&تجاهل الخطأ وتابع (غير مستحسن)
SourceIsCorrupted=الملف المصدر تالف
SourceDoesntExist=الملف المصدر ‎«%1»‎ غير موجود
SourceVerificationFailed=فشل التحقق من الملف المصدر: %1
VerificationSignatureDoesntExist=ملف التوقيع ‎«%1»‎ غير موجود
VerificationSignatureInvalid=ملف التوقيع ‎«%1»‎ غير صالح
VerificationKeyNotFound=ملف التوقيع ‎«%1»‎ يستخدم مفتاحًا غير معروف
VerificationFileNameIncorrect=اسم الملف غير صحيح
VerificationFileTagIncorrect=وسم الملف غير صحيح
VerificationFileSizeIncorrect=حجم الملف غير صحيح
VerificationFileHashIncorrect=بصمة الملف غير صحيحة
ExistingFileReadOnly2=لا يمكن استبدال الملف الموجود لأنه للقراءة فقط.
ExistingFileReadOnlyRetry=&أزل سمة للقراءة فقط وحاول مرة أخرى
ExistingFileReadOnlyKeepExisting=&أبقِ الملف الموجود
ErrorReadingExistingDest=حدث خطأ أثناء قراءة الملف الموجود:
FileExistsSelectAction=اختر إجراء
FileExists2=الملف موجود بالفعل.
FileExistsOverwriteExisting=&استبدل الملف الموجود
FileExistsKeepExisting=&أبقِ الملف الموجود
FileExistsOverwriteOrKeepAll=افعل هذا مع التعارضات التالية
ExistingFileNewerSelectAction=اختر إجراء
ExistingFileNewer2=الملف الموجود أحدث من الملف الذي يحاول التثبيت تثبيته.
ExistingFileNewerOverwriteExisting=&استبدل الملف الموجود
ExistingFileNewerKeepExisting=&أبقِ الملف الموجود (موصى به)
ExistingFileNewerOverwriteOrKeepAll=افعل هذا مع التعارضات التالية
ErrorChangingAttr=حدث خطأ أثناء تغيير سمات الملف الموجود:
ErrorCreatingTemp=حدث خطأ أثناء إنشاء ملف في مجلد الوجهة:
ErrorReadingSource=حدث خطأ أثناء قراءة الملف المصدر:
ErrorCopying=حدث خطأ أثناء نسخ ملف:
ErrorDownloading=حدث خطأ أثناء تنزيل ملف:
ErrorExtracting=حدث خطأ أثناء محاولة استخراج أرشيف:
ErrorReplacingExistingFile=حدث خطأ أثناء استبدال الملف الموجود:
ErrorRestartReplace=فشل RestartReplace‎:
ErrorRenamingTemp=حدث خطأ أثناء إعادة تسمية ملف في مجلد الوجهة:
ErrorRegisterServer=تعذّر تسجيل DLL/OCX‎: %1
ErrorRegSvr32Failed=فشل RegSvr32‎ برمز الخروج %1
ErrorRegisterTypeLib=تعذّر تسجيل مكتبة الأنواع: %1

UninstallDisplayNameMark=%1 (%2)
UninstallDisplayNameMarks=%1 (%2, %3)
UninstallDisplayNameMark32Bit=32 بت
UninstallDisplayNameMark64Bit=64 بت
UninstallDisplayNameMarkAllUsers=كل المستخدمين
UninstallDisplayNameMarkCurrentUser=المستخدم الحالي

ErrorOpeningReadme=حدث خطأ أثناء فتح ملف README.
ErrorRestartingComputer=تعذّر على التثبيت إعادة تشغيل الحاسوب. يرجى فعل ذلك يدويًا.

UninstallNotFound=الملف ‎«%1»‎ غير موجود. لا يمكن إلغاء التثبيت.
UninstallOpenError=تعذّر فتح الملف ‎«%1». لا يمكن إلغاء التثبيت
UninstallUnsupportedVer=ملف سجل إلغاء التثبيت ‎«%1»‎ بصيغة لا يتعرف عليها هذا الإصدار. لا يمكن إلغاء التثبيت
UninstallUnknownEntry=ظهر مدخل غير معروف (%1) في سجل إلغاء التثبيت
ConfirmUninstall=هل أنت متأكد من رغبتك في إزالة %1 وجميع مكوناته تمامًا؟
UninstallOnlyOnWin64=لا يمكن إلغاء تثبيت هذا التثبيت إلا على ويندوز 64 بت.
OnlyAdminCanUninstall=لا يمكن إلغاء تثبيت هذا التثبيت إلا من مستخدم يتمتع بصلاحيات إدارية.
UninstallStatusLabel=يرجى الانتظار بينما تتم إزالة %1 من حاسوبك.
UninstalledAll=تمت إزالة %1 بنجاح من حاسوبك.
UninstalledMost=اكتمل إلغاء تثبيت %1.%n%nتعذّرت إزالة بعض العناصر. يمكنك إزالتها يدويًا.
UninstalledAndNeedsRestart=لإتمام إلغاء تثبيت %1‎، يجب إعادة تشغيل حاسوبك.%n%nهل تريد إعادة التشغيل الآن؟
UninstallDataCorrupted=الملف ‎«%1»‎ تالف. لا يمكن إلغاء التثبيت

ConfirmDeleteSharedFileTitle=إزالة الملف المشترك؟
ConfirmDeleteSharedFile2=يشير النظام إلى أن الملف المشترك التالي لم يعد مستخدمًا من أي برنامج. هل تريد أن يقوم إلغاء التثبيت بإزالته؟%n%nإذا كانت بعض البرامج لا تزال تستخدم هذا الملف وأُزيل، فقد لا تعمل تلك البرامج بشكل صحيح. وإذا لم تكن متأكدًا، اختر «لا». وترك الملف على نظامك لن يلحق به ضرر.
SharedFileNameLabel=اسم الملف:
SharedFileLocationLabel=الموقع:
WizardUninstalling=حالة إلغاء التثبيت
StatusUninstalling=جارٍ إلغاء تثبيت %1...

ShutdownBlockReasonInstallingApp=جارٍ تثبيت %1.
ShutdownBlockReasonUninstallingApp=جارٍ إلغاء تثبيت %1.

[CustomMessages]
NameAndVersion=%1‎ الإصدار %2
AdditionalIcons=اختصارات إضافية:
Integration=تكامل مع Windows:
AssociateFiles=&الفتح بواسطة HM File Randomizer (انقر بزر الفأرة الأيمن على أي مجلد أو ملف)
CreateDesktopIcon=إنشاء اختصار على &سطح المكتب
CreateQuickLaunchIcon=إنشاء اختصار في &شريط التشغيل السريع
ProgramOnTheWeb=%1‎ على الويب
UninstallProgram=إلغاء تثبيت %1
LaunchProgram=تشغيل %1
AssocFileExtension=&ربط %1‎ بامتداد الملف %2
AssocingFileExtension=جارٍ ربط %1‎ بامتداد الملف %2...
AutoStartProgramGroupDescription=بدء التشغيل:
AutoStartProgram=تشغيل %1‎ تلقائيًا
AddonHostProgramNotFound=تعذّر العثور على %1‎ في المجلد الذي اخترته.%n%nهل تريد المتابعة على أي حال؟

; RandoFile credits, shown on the last wizard page.
InstalledReady=%1‎ جاهز للاستخدام.
InstalledBy=الإصدار %1‎ ‎-‎ من إعداد %2
InstalledProject=المشروع والشيفرة المصدرية:  github.com/hossein-moradi-sci/hm-file-randomizer
InstalledLinkedIn=لينكدإن:  linkedin.com/in/hossein-moradi-sci
InstalledEmail=البريد الإلكتروني:  hossein.moradi.sci@gmail.com

; Finish-page options beside the launch checkbox.
OpenInstallFolder=فتح مجلد %1
OpenProjectPage=فتح صفحة المشروع على GitHub
