using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using RandoFile.App.Localization;

namespace RandoFile.Core.Tests;

/// <summary>
/// Guards the Inno Setup script. It is never compiled here — CI builds it on a Windows runner — so
/// the failures it prevents are only visible at release time, when it is far too late: a wrong
/// version in the file name, a missing language file, or a changed <c>AppId</c> that quietly gives
/// every existing user a second copy of the app instead of an upgrade.
/// </summary>
public class InstallerScriptTests
{
    private const string ScriptName = "randofile.iss";

    private static readonly string InstallerRoot = FindInstallerRoot();

    private static string Read(params string[] parts) => File.ReadAllText(Path.Combine(new[] { InstallerRoot }.Concat(parts).ToArray()));

    [Fact]
    public void The_script_sits_where_ci_expects_it()
    {
        Assert.True(File.Exists(ReadPath(ScriptName)), $"Missing {ScriptName}.");
        Assert.True(File.Exists(ReadPath("license.txt")), "Missing license.txt, which the script shows on its licence page.");
    }

    [Fact]
    public void The_fallback_version_matches_the_application()
    {
        // CI passes the version in, so these fallbacks are never used to build a real installer.
        // They are what the Inno Setup IDE shows, so a stale one misleads whoever edits the script.
        var script = Read(ScriptName);

        var version = Define(script, "AppVersion");
        var tag = Define(script, "ReleaseTag");

        Assert.Equal(AppInfo.Version, version);
        Assert.Equal(AppInfo.DisplayVersion, tag);
    }

    [Fact]
    public void The_app_id_is_a_fixed_guid()
    {
        // A changed AppId is not a rename, it is a second installation: the old copy stays in
        // Programs, the new one appears beside it, and both write the same settings file.
        // Inno writes the value as {{GUID}, where the doubled brace escapes a literal one.
        var appId = Setting(Read(ScriptName), "AppId");
        var guid = Regex.Match(appId, @"\{([0-9A-Fa-f-]{36})\}");

        Assert.True(
            guid.Success && Guid.TryParse(guid.Groups[1].Value, out _),
            $"AppId='{appId}' is not a GUID, so Inno Setup cannot recognise an existing installation.");
    }

    [Fact]
    public void The_installer_lands_in_program_files()
    {
        // The user asked for the directory a real installed program lives in. {autopf} is
        // "Program Files" on 64-bit Windows and "Program Files (x86)" on 32-bit Windows, so
        // the path is always the one the operating system reserves for applications.
        var script = Read(ScriptName);

        Assert.Equal(@"{autopf}\{#AppName}", Setting(script, "DefaultDirName"));
        Assert.Equal("RandoFile", Define(script, "AppName"));

        // Program Files is protected, so the installer must actually ask for elevation.
        // A "lowest" setting here would make the default directory silently unwritable.
        Assert.Equal("admin", Setting(script, "PrivilegesRequired"));
    }

    [Fact]
    public void The_installer_offers_no_install_mode_page()
    {
        // Found by running the built installer: PrivilegesRequiredOverridesAllowed=dialog adds a
        // "Select Setup Install Mode" page, asking the user a question the default already
        // answers. 'commandline' is still allowed, so a scripted install can pass /CURRENTUSER
        // without the page ever appearing.
        var allowed = Settings(Read(ScriptName)).GetValueOrDefault("PrivilegesRequiredOverridesAllowed");

        Assert.Equal("commandline", allowed);
        Assert.DoesNotContain("dialog", allowed, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void The_finish_page_credits_are_wired_to_the_page_actually_being_shown()
    {
        // Found by running the built installer: assigning FinishedLabel.Caption from
        // CurStepChanged(ssPostInstall) has no effect, because Inno builds the finish page's own
        // caption afterwards and overwrites it. The credits silently never appeared. wpFinished in
        // CurPageChanged is the only moment the label exists.
        var script = Read(ScriptName);

        Assert.Contains("procedure CurPageChanged(CurPageID: Integer)", script, StringComparison.Ordinal);
        Assert.Contains("CurPageID = wpFinished", script, StringComparison.Ordinal);
        Assert.Contains("WizardForm.FinishedLabel.Caption", script, StringComparison.Ordinal);

        // The message ids the caption is built from have to be translated in every wizard language.
        foreach (var (_, file) in DeclaredLanguages())
        {
            var messages = Read("lang", Path.GetFileName(file));
            foreach (var id in new[] { "InstalledReady", "InstalledBy", "InstalledProject", "InstalledLinkedIn", "InstalledEmail" })
            {
                Assert.Contains(id + "=", messages, StringComparison.Ordinal);
            }
        }
    }

    [Fact]
    public void The_finish_page_opens_the_install_folder_and_the_project_page()
    {
        // The user asked for both: a finished install should offer the folder the app landed in
        // and the project page. Inno Setup renders [Run] entries carrying the postinstall flag as
        // checkboxes on the finish page, and shellexec is what lets it open a folder or an https
        // address at all - neither of those is an executable.
        var script = Read(ScriptName);
        var entries = RunEntries(script);

        // The address is a #define shared with AppPublisherURL, so the page the installer opens
        // cannot drift from the repository the installer names as its own.
        Assert.Equal(AppInfo.RepositoryUrl, Define(script, "ProjectUrl"));
        Assert.Equal("{#ProjectUrl}", Setting(script, "AppPublisherURL"));

        var folder = Assert.Single(entries.Where(entry => entry.Contains("Filename: \"{app}\"", StringComparison.Ordinal)));
        var project = Assert.Single(entries.Where(entry => entry.Contains("Filename: \"{#ProjectUrl}\"", StringComparison.Ordinal)));

        foreach (var entry in new[] { folder, project })
        {
            Assert.Contains("postinstall", entry, StringComparison.Ordinal);
            Assert.Contains("shellexec", entry, StringComparison.Ordinal);

            // A silent install must not open windows on a machine nobody is watching.
            Assert.Contains("skipifsilent", entry, StringComparison.Ordinal);

            // Ticked by default, because they are what the user asked the finish page to open. An
            // 'unchecked' added later would silently turn both back into extras nobody notices.
            Assert.DoesNotContain("unchecked", entry, StringComparison.Ordinal);
        }

        Assert.Contains("{cm:OpenInstallFolder,{#AppName}}", folder, StringComparison.Ordinal);
        Assert.Contains("{cm:OpenProjectPage}", project, StringComparison.Ordinal);

        // The labels come from [CustomMessages]. Inno Setup falls back to English for a missing
        // key instead of failing, so without this a Persian or Arabic wizard would quietly show
        // the English wording.
        foreach (var (_, file) in DeclaredLanguages())
        {
            var messages = Read("lang", Path.GetFileName(file));

            Assert.Contains("OpenInstallFolder=", messages, StringComparison.Ordinal);
            Assert.Contains("OpenProjectPage=", messages, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void The_file_association_is_offered_and_opt_in()
    {
        var script = Read(ScriptName);
        var progId = Define(script, "ProgId");
        Assert.False(string.IsNullOrWhiteSpace(progId), "The shell ProgId must be a fixed name.");

        // It has to be behind a task, off by default: taking over the right-click menu of every
        // file on the machine is not something an installer may do without being asked.
        Assert.Contains("Name: \"fileassoc\"", script, StringComparison.Ordinal);
        Assert.Contains(
            "Name: \"fileassoc\"; Description: \"{cm:AssociateFiles}\"; GroupDescription: \"{cm:Integration}\"; Flags: unchecked",
            script,
            StringComparison.Ordinal);

        // Every registration is gated on the task, so it cannot apply to someone who unticked the
        // box, and it carries uninsdeletekey so the uninstall takes the key down with it -
        // children included, since deleting a child of a key that is already going costs nothing.
        // Measured on a real uninstall: without the flag the values go but the keys stay behind,
        // and Open with keeps empty entries for a program that no longer exists.
        var registry = RegistryEntries(script);

        Assert.NotEmpty(registry);
        foreach (var line in registry)
        {
            Assert.True(
                line.Contains("Tasks: fileassoc", StringComparison.Ordinal),
                $"Registered without the task, so it applies even when the user opts out: {line}");
            Assert.True(
                line.Contains("Flags: uninsdeletekey", StringComparison.Ordinal),
                $"Survives an uninstall because uninsdeletekey is missing: {line}");
        }

        // The three things Windows needs to list the program: its own class key, an Open with
        // candidate for every file and for folders, and the SupportedTypes Default apps reads.
        Assert.Contains(registry, l => l.Contains(@"Software\Classes\{#ProgId}\shell\open\command", StringComparison.Ordinal));
        Assert.Contains(registry, l => l.Contains(@"Software\Classes\*\OpenWithProgids\", StringComparison.Ordinal));
        Assert.Contains(registry, l => l.Contains(@"Software\Classes\Directory\OpenWithProgids\", StringComparison.Ordinal));
        Assert.Contains(registry, l => l.Contains(@"Applications\{#AppExe}\SupportedTypes", StringComparison.Ordinal)
                                         && l.Contains(@"ValueName: ""*""", StringComparison.Ordinal));
        Assert.Contains(registry, l => l.Contains(@"Applications\{#AppExe}\SupportedTypes", StringComparison.Ordinal)
                                         && l.Contains(@"ValueName: ""Directory""", StringComparison.Ordinal));

        // The command has to be quoted and pass the path on: the program opens the folder the
        // user picked, which is the entire point of registering it.
        Assert.All(
            registry.Where(l => l.EndsWith("\\command", StringComparison.Ordinal)),
            line =>
            {
                Assert.Contains(@"""{app}\{#AppExe}"" ""%1""", line, StringComparison.Ordinal);
            });

        // The strings the page shows have to exist in all four languages, and the context-menu
        // label is written out in English because a brand is never translated.
        foreach (var (_, file) in DeclaredLanguages())
        {
            var messages = Read("lang", Path.GetFileName(file));

            Assert.Contains("AssociateFiles=", messages, StringComparison.Ordinal);
            Assert.Contains("Integration=", messages, StringComparison.Ordinal);
        }

        Assert.Contains($@"ValueData: ""Open with {{#AppFullName}}""", script, StringComparison.Ordinal);
    }

    [Fact]
    public void The_installer_never_forces_itself_to_be_the_default_handler()
    {
        // Windows since 8 protects the default-handler choice with a hash only Windows can
        // compute. Writing UserChoice anyway is what "uninstalling the default association" cannot
        // undo from Explorer, so the installer opens Default apps and lets the user decide.
        var script = Read(ScriptName);

        Assert.DoesNotContain("UserChoice", Section(script, "[Registry]"), StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("UserChoice", Section(script, "[Code]"), StringComparison.OrdinalIgnoreCase);

        Assert.Contains("WizardIsTaskSelected('fileassoc')", script, StringComparison.Ordinal);
        Assert.Contains("ms-settings:defaultapps", script, StringComparison.Ordinal);
    }

    [Fact]
    public void The_application_actually_opens_the_path_it_is_handed()
    {
        // The installer registers `RandoFile.exe "%1"`. If the program ignores its arguments the
        // association is a promise it does not keep: double-clicking a folder would open the app
        // to whatever it remembered last time and say nothing about why.
        var core = Read("..", "src", "RandoFile.Core", "CommandLineTarget.cs");
        var app = Read("..", "src", "RandoFile.App", "App.xaml.cs");

        Assert.Contains("ResolveFolderFromArguments(e.Args)", app, StringComparison.Ordinal);
        Assert.Contains("Settings.LastFolder = requestedFolder", app, StringComparison.Ordinal);

        // A file association covers every file, so the argument may be a file, and the program
        // has to make sense of that by opening the folder the file is in.
        Assert.Contains("Directory.Exists", core, StringComparison.Ordinal);
        Assert.Contains("File.Exists", core, StringComparison.Ordinal);
        Assert.Contains("Path.GetDirectoryName", core, StringComparison.Ordinal);
    }

    /// <summary>Every <c>Root:</c> line of the <c>[Registry]</c> section.</summary>
    private static List<string> RegistryEntries(string script) => SectionLines(script, "[Registry]", "Root:");

    /// <summary>Every <c>Filename:</c> line of the <c>[Run]</c> section.</summary>
    private static List<string> RunEntries(string script) => SectionLines(script, "[Run]", "Filename:");

    /// <summary>Every line of one section of the script that starts with the given key.</summary>
    private static List<string> SectionLines(string script, string header, string key)
    {
        var lines = new List<string>();
        var inSection = false;

        foreach (var raw in script.Split('\n'))
        {
            var line = raw.Trim();

            if (line.StartsWith("[", StringComparison.Ordinal))
            {
                inSection = line.Equals(header, StringComparison.OrdinalIgnoreCase);
                continue;
            }

            if (inSection && line.StartsWith(key, StringComparison.OrdinalIgnoreCase))
            {
                lines.Add(line);
            }
        }

        return lines;
    }

    /// <summary>The body of one section of the script, up to the next section header.</summary>
    private static string Section(string script, string header)
    {
        var lines = new List<string>();
        var inSection = false;

        foreach (var raw in script.Split('\n'))
        {
            var line = raw.Trim();

            if (line.StartsWith("[", StringComparison.Ordinal))
            {
                if (inSection)
                {
                    break;
                }

                inSection = line.Equals(header, StringComparison.OrdinalIgnoreCase);
                continue;
            }

            if (inSection)
            {
                lines.Add(line);
            }
        }

        return string.Join('\n', lines);
    }

    [Fact]
    public void Every_language_the_script_claims_exists()
    {
        var declared = DeclaredLanguages().ToList();

        Assert.Equal(4, declared.Count);

        foreach (var (name, file) in declared)
        {
            Assert.True(File.Exists(file), $"The wizard offers '{name}' but {Path.GetFileName(file)} is not there.");
        }
    }

    [Fact]
    public void The_four_wizard_languages_match_the_interface_languages()
    {
        // The app speaks fa, en, fr and ar. An installer that speaks something else, or is missing
        // one of them, is a support problem waiting to happen.
        var wizardNameFor = LanguageInfo.All.ToDictionary(
            language => language.Code,
            language => language switch
            {
                { Code: "en" } => "english",
                { Code: "fa" } => "farsi",
                { Code: "fr" } => "french",
                { Code: "ar" } => "arabic",
                _ => throw new InvalidOperationException(
                    $"RandoFile now speaks '{language.EnglishName}', so the installer needs a matching wizard language too."),
            });

        var offered = DeclaredLanguages()
            .Select(language => language.Name)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToList();

        Assert.Equal(
            wizardNameFor.Values.OrderBy(name => name, StringComparer.Ordinal).ToList(),
            offered);
    }

    [Fact]
    public void Persian_and_arabic_are_marked_right_to_left()
    {
        // Without this the wizard lays the controls out left to right and the text reads oddly.
        foreach (var language in new[] { "farsi", "arabic" })
        {
            var options = LangOptions(Read("lang", language + ".isl"));

            Assert.Equal("yes", Value(options, "RightToLeft"));
        }

        Assert.Equal("no", Value(LangOptions(Read("lang", "english.isl")), "RightToLeft"));
        Assert.Equal("no", Value(LangOptions(Read("lang", "french.isl")), "RightToLeft"));
    }

    [Fact]
    public void Every_wizard_language_defines_the_same_messages()
    {
        // Inno Setup silently falls back to English when a key is missing, so a half-finished
        // translation shows up as one English label in an otherwise Persian wizard. [CustomMessages]
        // matters just as much here: the script references the credit strings from its [Code]
        // block, and a missing one prints a literal {cm:...} on the finish page.
        var problems = new List<string>();

        foreach (var section in new[] { "Messages", "CustomMessages" })
        {
            var reference = Messages(Read("lang", "english.isl"), section);
            Assert.NotEmpty(reference);

            foreach (var language in new[] { "farsi", "french", "arabic" })
            {
                var keys = Messages(Read("lang", language + ".isl"), section);

                var missing = reference.Keys.Except(keys.Keys).OrderBy(key => key).ToList();
                var extra = keys.Keys.Except(reference.Keys).OrderBy(key => key).ToList();

                if (missing.Count > 0)
                {
                    problems.Add($"{section}: {language} is missing: {string.Join(", ", missing)}");
                }

                if (extra.Count > 0)
                {
                    problems.Add($"{section}: {language} has unknown keys: {string.Join(", ", extra)}");
                }

                foreach (var (key, value) in keys)
                {
                    if (string.IsNullOrWhiteSpace(value) && !BlankIsAllowed.Contains(key))
                    {
                        problems.Add($"{section}: {language}: '{key}' is empty");
                    }
                }
            }
        }

        Assert.True(problems.Count == 0, string.Join(Environment.NewLine, problems));
    }

    [Fact]
    public void Placeholders_match_between_wizard_languages()
    {
        var reference = Messages(Read("lang", "english.isl"));
        var problems = new List<string>();

        foreach (var language in new[] { "farsi", "french", "arabic" })
        {
            var keys = Messages(Read("lang", language + ".isl"));

            foreach (var (key, value) in keys)
            {
                if (!reference.TryGetValue(key, out var english))
                {
                    continue;
                }

                // [name] and %n are substituted by Inno itself and have to survive translation:
                // losing one turns a sentence into a broken one at run time.
                foreach (var placeholder in new[] { "%1", "%2", "%n", "[name]", "[name/ver]", "[gb]", "[mb]" })
                {
                    if (Count(english, placeholder) != Count(value, placeholder))
                    {
                        problems.Add($"{language}: '{key}' has a different number of '{placeholder}'");
                    }
                }
            }
        }

        Assert.True(problems.Count == 0, string.Join(Environment.NewLine, problems));
    }

    [Fact]
    public void The_licence_page_carries_the_credits()
    {
        // The user asked for the creator's details to be visible during install, so they are
        // asserted here rather than trusted to survive an edit of the licence text.
        var licence = Read("license.txt");

        Assert.Contains(AppInfo.Creator, licence, StringComparison.Ordinal);
        Assert.Contains(AppInfo.CreatorEmail, licence, StringComparison.Ordinal);
        Assert.Contains(AppInfo.RepositoryUrl, licence, StringComparison.Ordinal);
        Assert.Contains("linkedin.com/in/hossein-moradi-sci", licence, StringComparison.OrdinalIgnoreCase);

        // MIT still has to be there in full, because that is the licence the project is under.
        Assert.Contains("MIT License", licence, StringComparison.Ordinal);
        Assert.Contains("THE SOFTWARE IS PROVIDED", licence, StringComparison.Ordinal);

        // And the wizard has to actually show it. LicenseFile only makes Inno Setup create the
        // licence page; [Code] replaces the text with the translation once the language is known.
        Assert.Equal("{#LicenceDefault}", Setting(Read(ScriptName), "LicenseFile"));
    }

    [Fact]
    public void The_licence_is_written_in_the_language_the_installer_is_running_in()
    {
        // The user asked for this explicitly: pick Persian in the wizard and the contract must be
        // in Persian, not English. Inno Setup has no per-language LicenseFile, so this happens in
        // [Code], which means nothing about it is checked by the compiler - only these tests and
        // a real run of the built installer can catch it going wrong.
        var script = Read(ScriptName);

        Assert.Contains("procedure InitializeWizard()", script, StringComparison.Ordinal);
        Assert.Contains("ActiveLanguage", script, StringComparison.Ordinal);

        // Loading the licence straight out of {src} silently fails and every language falls back
        // to the English text; the file has to be extracted to {tmp} first.
        Assert.Contains("ExtractTemporaryFile", script, StringComparison.Ordinal);
        Assert.DoesNotContain("LoadStringsFromFile('{src}", script, StringComparison.Ordinal);

        // One licence file per wizard language, named so that [Code] can find it, wired up for
        // every language the script declares - a missing branch would leave that one English.
        var licences = DeclaredLanguages()
            .Select(pair => (
                Name: pair.Name,
                // English is the default the page is created from, so it has its own define.
                Define: pair.Name == "english"
                    ? "LicenceDefault"
                    : "Licence" + char.ToUpperInvariant(pair.Name[0]) + pair.Name[1..]))
            .ToDictionary(x => x.Name, x => x.Define, StringComparer.Ordinal);

        foreach (var (language, defineName) in licences)
        {
            var file = Define(script, defineName);
            var path = ReadPath(file);
            Assert.True(File.Exists(path), $"The wizard offers '{language}' but {file} is not there.");

            // LoadStringsFromFile decodes the UTF-8 BOM; without it the Persian and Arabic
            // agreements render as boxes. The BOM is invisible in a diff, so it is asserted.
            var bytes = File.ReadAllBytes(path);
            Assert.True(
                bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF,
                $"{file} must be saved as UTF-8 with a BOM.");

            var text = File.ReadAllText(path);

            // Same deal as the English licence: the credits, and the MIT terms in full.
            Assert.Contains(AppInfo.Creator, text, StringComparison.Ordinal);
            Assert.Contains(AppInfo.CreatorEmail, text, StringComparison.Ordinal);
            Assert.Contains("hm-file-randomizer", text, StringComparison.Ordinal);
            Assert.Contains("linkedin.com/in/hossein-moradi-sci", text, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("MIT License", text, StringComparison.Ordinal);
            Assert.Contains("THE SOFTWARE IS PROVIDED", text, StringComparison.Ordinal);

            // And the branch in [Code] that selects it. English is the fallback rather than a
            // branch, so it is checked the other way round.
            if (language == "english")
            {
                Assert.Contains("Result := '{#LicenceDefault}'", script, StringComparison.Ordinal);
            }
            else
            {
                Assert.Contains($"'{language}'", script, StringComparison.Ordinal);
            }

            // It also travels with the install, next to the executable.
            Assert.Contains($"Source: \"{{#{defineName}}}\"", script, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void The_licence_reads_like_a_contract_rather_than_a_list_of_rules()
    {
        // The old text was a flat list of prohibitions, which is not how an agreement is built and
        // not what MIT permits: MIT grants unconditionally, so an EULA cannot take permissions
        // away. The rewrite keeps the ask separate from the grant and says so explicitly.
        var licence = Read("license.txt");

        Assert.Contains("Contents", licence, StringComparison.Ordinal);
        Assert.Contains("ACCEPTANCE OF THIS AGREEMENT", licence, StringComparison.Ordinal);
        Assert.Contains("DEFINITIONS", licence, StringComparison.Ordinal);
        Assert.Contains("DISCLAIMER OF WARRANTIES", licence, StringComparison.Ordinal);
        Assert.Contains("LIMITATION OF LIABILITY", licence, StringComparison.Ordinal);
        Assert.Contains("TERMINATION", licence, StringComparison.Ordinal);

        Assert.Contains(
            "is the whole of the rights",
            licence,
            StringComparison.Ordinal);
    }

    [Fact]
    public void The_release_actually_ships_the_installer()
    {
        // Building the installer is pointless if the release page does not offer it, and the
        // hand written install notes silently replace the generated ones unless append_body is set.
        var workflow = Read("..", ".github", "workflows", "ci.yml");

        Assert.Contains("choco install innosetup", workflow, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ISCC.exe", workflow, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("installer/randofile.iss", workflow, StringComparison.OrdinalIgnoreCase);

        Assert.Contains(
            "dist/installer/RandoFile-Setup-${{ github.ref_name }}-win-x64.exe",
            workflow,
            StringComparison.Ordinal);

        // /DAppVersion and /DReleaseTag are two halves of the version; losing either makes the
        // file name or the reported version wrong without anything else complaining.
        Assert.Contains("/DAppVersion=$version", workflow, StringComparison.Ordinal);
        Assert.Contains("/DReleaseTag=$env:GITHUB_REF_NAME", workflow, StringComparison.Ordinal);

        Assert.True(
            workflow.Contains("append_body: true", StringComparison.Ordinal),
            "Without append_body the custom release body replaces the generated release notes.");
    }

    [Fact]
    public void The_release_can_be_signed_with_the_owners_own_certificate()
    {
        // The certificate belongs to the owner, so what is guarded here is the wiring: the two
        // signing points, the secrets that switch them on, and the documented local path.
        var workflow = Read("..", ".github", "workflows", "ci.yml");
        var script = Read("..", "tools", "Sign-Release.ps1");

        // Both are optional, so a tag on a fork - or one pushed before a certificate exists -
        // still produces a release instead of failing the build.
        Assert.Contains("secrets.WINDOWS_CERT_PFX_BASE64", workflow, StringComparison.Ordinal);
        Assert.Contains("secrets.WINDOWS_CERT_PASSWORD", workflow, StringComparison.Ordinal);
        Assert.Contains("RANDOFILE_CERT_PFX_BASE64", script, StringComparison.Ordinal);

        // The executable has to be signed before Inno Setup packages it, and the installer after:
        // signing only one of them leaves the other download untrusted.
        var published = workflow.IndexOf("Sign the published executable", StringComparison.Ordinal);
        var compiler = workflow.IndexOf("ISCC.exe", StringComparison.Ordinal);
        var installer = workflow.IndexOf("Sign the installer", StringComparison.Ordinal);
        var zip = workflow.IndexOf("Package portable zip", StringComparison.Ordinal);

        Assert.True(published > 0, "The published executable is never signed.");
        Assert.True(
            published < compiler,
            "The executable is signed after Inno Setup packaged it, so the copy inside the installer stays unsigned.");
        Assert.True(installer > compiler, "The installer is signed before it has been built.");
        Assert.True(installer < zip, "The installer is signed after the release assets were collected.");

        // The owner's own certificate lives at one documented path, and it must not be committable.
        Assert.Contains("certificates\\code-signing.pfx", script, StringComparison.Ordinal);

        var ignore = Read("..", ".gitignore");
        Assert.Contains("/certificates/", ignore, StringComparison.Ordinal);
        Assert.Contains("*.pfx", ignore, StringComparison.Ordinal);

        // Every signature has to be timestamped: one without a timestamp stops being trusted when
        // the certificate expires, which is the whole point of signing a release.
        Assert.Contains("timestamp.digicert.com", script, StringComparison.Ordinal);
        Assert.Contains("TimeStamperCertificate", script, StringComparison.Ordinal);

        // And the README documents both routes for whoever has to set this up.
        var readme = Read("..", "README.md");
        Assert.Contains("WINDOWS_CERT_PFX_BASE64", readme, StringComparison.Ordinal);
        Assert.Contains("certificates\\code-signing.pfx", readme, StringComparison.Ordinal);
        Assert.Contains("Thumbprint", readme, StringComparison.Ordinal);

        // And users are shown how to check the result themselves: both the graphical path and
        // the PowerShell command have to stay documented.
        Assert.Contains("Digital Signatures", readme, StringComparison.Ordinal);
        Assert.Contains("Get-AuthenticodeSignature", readme, StringComparison.Ordinal);
    }

    [Fact]
    public void The_release_workflow_never_quotes_a_line_by_itself()
    {
        // This shipped broken: inside a YAML block scalar (path: |) quotes are literal, so
        // "'*.zip'" is a glob for a file whose name starts with an apostrophe. Nothing matched,
        // the artifact upload failed with "No files were found" and the release never appeared
        // even though the installer, the zip and every other step of the tag run went green.
        var workflow = Read("..", ".github", "workflows", "ci.yml");
        var lines = workflow.Replace("\r\n", "\n").Split('\n');

        var quoted = lines
            .Select(line => line.Trim())
            .Where(text => text.Length >= 2 &&
                           ((text[0] == '\'' && text[text.Length - 1] == '\'') ||
                            (text[0] == '"' && text[text.Length - 1] == '"')))
            .ToList();

        Assert.True(
            quoted.Count == 0,
            "A line that is nothing but a quoted scalar keeps its quotes inside a block scalar: " +
            string.Join(", ", quoted));

        // And the upload step still asks for exactly what the release ships.
        var trimmed = lines.Select(line => line.Trim()).ToList();
        Assert.Contains("*.zip", trimmed);
        Assert.Contains("dist/installer/*.exe", trimmed);
    }

    [Fact]
    public void The_signing_script_runs_on_windows_powershell()
    {
        // Windows PowerShell 5.1 reads a .ps1 as ANSI unless it has a BOM, and that is exactly the
        // PowerShell the script's fallback signing path runs in on a developer machine. One smart
        // quote pasted from a browser would break it there, with a misleading error.
        var bytes = File.ReadAllBytes(ReadPath("..", "tools", "Sign-Release.ps1"));

        Assert.All(
            bytes,
            b => Assert.True(b < 0x80, "Sign-Release.ps1 must stay ASCII: PowerShell 5.1 reads it as ANSI when it has no BOM."));
    }

    /// <summary>Messages Inno Setup itself expects to be blank.</summary>
    private static readonly HashSet<string> BlankIsAllowed = new(StringComparer.Ordinal)
    {
        "HelpTextNote",
        "AboutSetupNote",
        "TranslatorNote",
        "BeveledLabel",
    };

    /// <summary>Walks up from the test output to the folder holding <c>installer\</c>.</summary>
    private static string FindInstallerRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, "installer");

            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        // Reported by the test rather than thrown here, so the failure reads as a missing file.
        return Path.Combine(AppContext.BaseDirectory, "installer");
    }

    private static string ReadPath(params string[] parts) => Path.Combine(new[] { InstallerRoot }.Concat(parts).ToArray());

    private static int Count(string value, string needle)
    {
        var count = 0;
        var index = 0;

        while ((index = value.IndexOf(needle, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += needle.Length;
        }

        return count;
    }

    /// <summary>Reads <c>Key=Value</c> lines from a script section, ignoring comments.</summary>
    private static Dictionary<string, string> Settings(string script)
    {
        var settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var line in script.Split('\n'))
        {
            var trimmed = line.Trim().TrimEnd('\r');

            if (trimmed.Length == 0 || trimmed.StartsWith(';') || trimmed.StartsWith('['))
            {
                continue;
            }

            var separator = trimmed.IndexOf('=');

            if (separator > 0)
            {
                settings[trimmed[..separator].Trim()] = trimmed[(separator + 1)..].Trim();
            }
        }

        return settings;
    }

    private static string Setting(string script, string key) =>
        Settings(script).TryGetValue(key, out var value) ? value : throw new InvalidOperationException($"The script has no {key}.");

    /// <summary>Reads the value of a <c>#define</c>, which is a plain literal here.</summary>
    private static string Define(string script, string name)
    {
        var match = Regex.Match(script, $@"#define\s+{name}\s+""([^""]+)""");

        Assert.True(match.Success, $"The script has no #define {name}.");

        return match.Groups[1].Value;
    }

    private static Dictionary<string, string> LangOptions(string isl) =>
        Messages(isl, "LangOptions");

    /// <summary>The wizard languages the script offers, as (display name, .isl path).</summary>
    private static IEnumerable<(string Name, string File)> DeclaredLanguages()
    {
        var entries = Sections(Read(ScriptName)).GetValueOrDefault("Languages") ?? string.Empty;

        foreach (Match match in Regex.Matches(entries, @"(?m)^Name:\s*""([^""]+)"";\s*MessagesFile:\s*""([^""]+)"""))
        {
            var file = match.Groups[2].Value;

            // The script writes lang\english.isl with a Windows separator.
            yield return (match.Groups[1].Value, ReadPath("lang", Path.GetFileName(file)));
        }
    }

    /// <summary>Splits a script or language file into its <c>[Section]</c> bodies.</summary>
    private static Dictionary<string, string> Sections(string script)
    {
        var sections = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var name = string.Empty;
        var lines = new List<string>();

        foreach (var raw in script.Split('\n'))
        {
            var line = raw.Trim().TrimEnd('\r');

            if (line.StartsWith('[') && line.EndsWith(']'))
            {
                sections[name] = string.Join('\n', lines);
                name = line[1..^1];
                lines = [];
                continue;
            }

            lines.Add(line);
        }

        sections[name] = string.Join('\n', lines);

        return sections;
    }

    /// <summary>Reads the <c>Key=Value</c> pairs of one section of a language file.</summary>
    private static Dictionary<string, string> Messages(string isl, string section = "Messages")
    {
        var messages = new Dictionary<string, string>(StringComparer.Ordinal);
        var inside = false;

        foreach (var raw in isl.Split('\n'))
        {
            var line = raw.Trim().TrimEnd('\r');

            if (line.StartsWith('['))
            {
                inside = line.Equals($"[{section}]", StringComparison.OrdinalIgnoreCase);
                continue;
            }

            if (!inside || line.Length == 0 || line.StartsWith(';'))
            {
                continue;
            }

            var separator = line.IndexOf('=');

            if (separator > 0)
            {
                messages[line[..separator].Trim()] = line[(separator + 1)..];
            }
        }

        return messages;
    }

    private static string Value(Dictionary<string, string> values, string key) =>
        values.TryGetValue(key, out var value) ? value : throw new InvalidOperationException($"No {key}.");
}
