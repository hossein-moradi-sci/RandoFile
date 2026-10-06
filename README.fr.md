<div align="center">

<img src="src/RandoFile.App/Assets/Brand/logo.png" width="128" alt="Logo RandoFile" />

<h1>🎲 RandoFile</h1>

<p><b>Bouleversez n'importe quel dossier dans un ordre vraiment aléatoire — prévisualisez d'abord, renommez en toute sécurité, annulez en un clic.</b></p>

<p>
  <a href="https://github.com/hossein-moradi-sci/RandoFile/actions/workflows/ci.yml"><img src="https://github.com/hossein-moradi-sci/RandoFile/actions/workflows/ci.yml/badge.svg" alt="CI" /></a>
  <a href="https://github.com/hossein-moradi-sci/RandoFile/releases/latest"><img src="https://img.shields.io/github/v/release/hossein-moradi-sci/RandoFile?label=release" alt="Dernière version" /></a>
  <a href="https://github.com/hossein-moradi-sci/RandoFile/releases/latest"><img src="https://img.shields.io/github/downloads/hossein-moradi-sci/RandoFile/total?label=downloads" alt="Téléchargements" /></a>
  <img src="https://img.shields.io/badge/Windows-10%20%2F%2011-0078D4" alt="Windows 10 / 11" />
  <img src="https://img.shields.io/badge/.NET-8.0-512BD4" alt=".NET 8" />
  <img src="https://img.shields.io/badge/license-MIT-34D058" alt="Licence MIT" />
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

**RandoFile** mélange chaque fichier d'un dossier dans un ordre réellement aléatoire, puis
renomme l'ensemble avec un motif propre et prévisible — et vous montre un aperçu complet de
chaque ancien → nouveau nom avant que quoi que ce soit touche votre disque.

Il est fourni sous la forme d’un fichier `.exe` autonome pour Windows 10/11 et ne s'exécute
jamais dans un navigateur.

> **RandoFile** est une marque. Elle n'est jamais traduite — toutes les langues l'écrivent
> exactement pareil.

---

## ✨ Fonctionnalités

| | |
|---|---|
| **Sélecteur de dossier avec compteur en direct** | Choisissez un dossier et voyez immédiatement combien de fichiers ont été trouvés. |
| **Ordre vraiment aléatoire** | Mélange Fisher–Yates : le nouvel ordre est réellement imprévisible. |
| **Tous types de fichiers** | Images, vidéos, documents — l'extension d'origine est toujours conservée. |
| **Modèles de nommage** | `Image 1, Image 2 …`, `File 1, File 2 …` ou le préfixe de votre choix. |
| **Maîtrise de la numérotation** | Numéro de départ et zéros facultatifs, par ex. `Image 007.jpg`. |
| **Aperçu avant renommage** | Chaque paire ancien → nouveau est listée, avec un résumé live des changements. |
| **Protection contre les doublons et l’écrasement** | Un plan contenant un conflit est refusé : rien n'est renommé. |
| **Moteur sûr** | Renommage en deux phases (mise en staging → commit) : les échanges et cycles de noms fonctionnent. |
| **Annulation automatique** | Si un fichier échoue, tout ce qui a déjà été renommé est restauré. |
| **Undo** | Un clic révertit la dernière exécution réussie. |
| **Progression & annulation** | Barre de progression et bouton d'annulation qui remettent tout proprement à l'état initial. |
| **Écran d'accueil animé** | Quatre secondes et demie d'ouverture : le logo s'assemble morceau par morceau, se pose avec un léger dépassement, puis respire pendant que le badge de crédit néon s'allume en dessous. |
| **Quatre langues** | فارسی · English · Français · العربية, avec un sens complet droite à gauche pour le persan et l'arabe. |
| **Thèmes** | Clair, sombre, ou suivre le réglage de Windows. |
| **Vérification des mises à jour** | Lit `releases/latest` de GitHub et affiche un bandeau quand une version plus récente existe. |

---

## ⬇️ Téléchargement et installation

Récupérez la dernière version sur la
[page des Releases](https://github.com/hossein-moradi-sci/RandoFile/releases/latest) —
deux clics et c'est terminé.

**L'installateur (recommandé)**

1. Téléchargez `RandoFile-Setup-<version>-win-x64.exe`
2. Double-cliquez dessus, choisissez votre langue, acceptez la licence
3. Acceptez l'unique demande *User Account Control* de Windows
4. Appuyez sur Install

RandoFile s'installe dans `C:\Program Files\RandoFile`, l'endroit où Windows garde les
programmes installés, avec une entrée dans le menu Démarrer, un raccourci bureau facultatif
et une vraie entrée dans *Apps & features* pour désinstaller. **Aucun runtime .NET n'est
requis**, et installer une version ultérieure par-dessus une précédente la remplace
simplement. Le contrat de la page de licence s'affiche **dans la langue choisie pour
l'installateur** — le français, donc — et toutes les traductions sont installées à côté de
l'exécutable.

L'assistant propose aussi deux tâches facultatives : un raccourci bureau, et l'enregistrement
de RandoFile dans le menu **Ouvrir avec** pour tous les dossiers et fichiers — c'est ce qui
permet de le choisir dans les *Applications par défaut* de Windows. Rien n'est imposé, et la
désinstallation retire tout ce qui a été ajouté. Quand Setup se termine, sa dernière page
propose de lancer RandoFile, d'ouvrir le dossier d'installation et d'ouvrir cette page du
projet.

Vous préférez éviter toute élévation de privilèges ? L'installateur accepte `/CURRENTUSER`,
qui installe sous votre profil plutôt que dans Program Files.

**La version portable, si vous préférez**

`RandoFile-<version>-win-x64.zip` est le même programme sans installateur : décompressez-le
où vous voulez et lancez `RandoFile.exe`.

Les deux sont autonomes (*self-contained*) : ils tournent sur n'importe quel Windows 10 ou 11.

### 🔐 Vérifier le téléchargement

L'installateur et `RandoFile.exe` (celui à l'intérieur du zip portable) portent tous deux une
signature numérique : vous pouvez donc vérifier qu'un fichier vient bien de ce projet et
qu'il n'a pas été modifié. Le conteneur `.zip` lui-même n'est pas signé — extrayez-le
d'abord, puis vérifiez l'exécutable.

**Graphiquement :** faites un clic droit sur le fichier, choisissez **Propriétés**, ouvrez
l'onglet **Digital Signatures**, sélectionnez l'entrée et cliquez sur **Détails**.

**Dans PowerShell** (remplacez `<version>` par la version téléchargée, par ex. `v1.0.0`) :

```powershell
Get-AuthenticodeSignature .\RandoFile-Setup-<version>-win-x64.exe |
    Format-List Status, @{n='Signer';e={$_.SignerCertificate.Subject}},
                 @{n='Timestamped';e={$_.TimeStamperCertificate -ne $null}}
```

À quoi s'attendre :

- **Signer** — `CN=Hossein Moradi`, le créateur du projet.
- **Timestamped** — `True` : la signature reste valide même après l'expiration du certificat.
- **Status** — `Valid` signifie que Windows fait confiance à l'émetteur. Les builds signés
  avec le certificat du projet affichent `UnknownError` : signature et horodatage sont
  intacts, mais l'émetteur n'est pas une racine publiquement de confiance — exactement ce à
  quoi SmartScreen réagit (voir [Signature de code](#signature-de-code)).

---

## 🚀 Utilisation

1. **Browse** — choisissez le dossier qui contient vos fichiers.
2. **Choisissez un motif** — `Image`, `File`, ou `Custom` avec votre propre préfixe.
3. **Aperçu** — cliquez sur **Preview** pour voir exactement quel fichier devient quel nouveau nom.
4. **Randomize** — cliquez sur **Randomize** et confirmez. La barre de progression suit l'opération.
5. **Undo** — si le résultat ne vous convient pas, **Undo** restaure les noms d'origine.

Les réglages (langue, thème, scan des sous-dossiers, vérification des mises à jour) se
trouvent dans la fenêtre **Settings** ; les informations sur le créateur et la version, dans
la fenêtre **About**.

Un guide illustré est intégré à l'application : appuyez sur <kbd>F1</kbd> ou utilisez le
bouton `?` de l'en-tête. Il couvre les quatre étapes ci-dessus, les options de nommage, les
raccourcis clavier, les garanties de sécurité et les questions les plus fréquentes — dans les
quatre langues de l'interface.

---

## 🏷️ Motifs de nommage

| Modèle | Résultat |
|---|---|
| Image | `Image 1.jpg`, `Image 2.jpg`, `Image 3.png`, … |
| File | `File 1.docx`, `File 2.mp4`, `File 3.txt`, … |
| Custom | `<your prefix> 1.ext`, `<your prefix> 2.ext`, … |

Avec **Start number** sur `101`, **Number of digits** sur `3` et le modèle `Image`, le
premier fichier devient `Image 101.jpg`, le deuxième `Image 102.jpg`, et ainsi de suite.

---

## 🛡️ Comment le renommage reste sûr

Renommer des fichiers dans un même dossier n'est pas un simple recopiage un pour un — les
noms peuvent s'échanger entre eux ou former un cycle (`a → b → c → a`). Le moteur travaille
donc en deux phases :

1. **Mise en staging** — chaque fichier part vers un nom temporaire unique
   (`.rf_tmp_<guid><ext>`).
2. **Commit** — une fois qu'aucune collision n'est plus possible, les noms temporaires
   prennent leur nom final.

Si quelque chose échoue en chemin, la phase déjà terminée est annulée automatiquement.
La même mécanique soutient la commande **Undo**. Les fichiers cachés, système et temporaires
résiduels sont ignorés lors du scan.

---

## 🎨 Les ressources de la marque

![RandoFile logo](src/RandoFile.App/Assets/Brand/logo.png)

La marque est trois cartes de documents disposées en éventail, surmontées d'un segment d'anneau
qui évoque le mouvement d'aléatoire. Détachée de toute forme générique, sa silhouette reste
reconnaissable par elle-même, et elle est volontairement composée de pièces séparables —
l'écran d'accueil reconstruit les mêmes formes en vecteurs pour les animer une par une, puis
les fait se poser sur la marque et respirer à jamais.

| Fichier | Usage |
|---|---|
| `src/RandoFile.App/Assets/Brand/logo.png` | Logo maître 512 px, sur son fond sombre |
| `src/RandoFile.App/Assets/Brand/logo-mark.png` | Marque transparente utilisée dans l'interface |
| `src/RandoFile.App/Assets/Brand/randofile.ico` | Icône Windows, de 16 à 256 px, sur le même fond |

Les trois sont générés, pas dessinés à la main. Pour changer la marque, éditez
[`tools/Generate-Logo.ps1`](tools/Generate-Logo.ps1) et lancez-le :

```powershell
powershell -ExecutionPolicy Bypass -File tools/Generate-Logo.ps1
```

---

## 🛠️ Compiler depuis les sources

Prérequis : [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) et Windows.

```powershell
git clone https://github.com/hossein-moradi-sci/RandoFile.git
cd randofile

dotnet restore RandoFile.sln
dotnet build   RandoFile.sln -c Release --no-restore
dotnet test    RandoFile.sln -c Release --no-build
```

### Produire l'`.exe` autonome

```powershell
dotnet publish src/RandoFile.App/RandoFile.App.csproj `
  -c Release -r win-x64 --self-contained true `
  -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true `
  -o dist/win-x64
```

Le résultat est un `dist/win-x64/RandoFile.exe` autonome unique (~65 Mo) qui tourne sur tout
Windows 10/11 sans runtime .NET.

---

## 📁 Structure du projet

```
RandoFile.sln
Directory.Build.props            Version partagée + métadonnées d'assembly
src/
  RandoFile.Core/                Moteur, sans dépendance UI
    Models/                      NamingPattern, RenamePlan, RenameProgress, RenameResult
    Services/                    FileShuffler, FileScanner, RenamePlanBuilder, RenameExecutor
    Abstractions/                IFileScanner, IFileShuffler
    CommandLineTarget.cs         Transforme le chemin donné par Windows en dossier à ouvrir
    AppInfo.cs                   Marque, nom du produit, version et identité du dépôt
  RandoFile.App/                 Application WPF
    ViewModels/                  MainViewModel, SettingsOption, PreviewRow
    Views/                       SplashWindow, SettingsWindow, AboutWindow, HelpWindow
    Services/                    Settings, Localization, Theme, Dialog, Update, CrashLog
    Resources/Themes/            Light.xaml, Dark.xaml, Controls.xaml
    Resources/Strings/           Strings.en.xaml, Strings.fa.xaml, Strings.fr.xaml, Strings.ar.xaml
    Resources/Help/              Mockups.xaml (illustrations vectorielles de l'aide)
    Assets/Brand/                Logo et icône Windows
tests/
  RandoFile.Tests/               Suite xUnit : moteur, marque, ressources et bindings XAML
installer/
  randofile.iss                  Script Inno Setup : l'installateur téléchargé depuis une release
  license.txt                    Le contrat, en anglais
  license.farsi.txt              Le même contrat, en persan
  license.french.txt             ... en français
  license.arabic.txt             ... en arabe
  lang/                          Chaînes de l'assistant : english, farsi, french, arabic
tools/
  Generate-Logo.ps1              Dessine la marque et écrit les assets PNG/ICO
  Sign-Release.ps1               Signe la release avec votre propre certificat
scripts-verify/
  verify-assoc.ps1               Installe en mode utilisateur et vérifie chaque clé de registre
  verify-machine.ps1             La même chose pour une installation machine dans Program Files
  verify-finish-page.ps1         Pilote le vrai assistant jusqu'au bout et clique sur Finish
  verify-signing.ps1             Prouve le script de signature avec un certificat jetable
  common.ps1                     Helpers partagés par les scripts d'association
```

Le projet `Core` n'a aucune dépendance WPF, ce qui rend la logique de renommage entièrement
testable.

---

## 🧪 Tests

```powershell
dotnet test RandoFile.sln
```

La suite couvre le motif de nommage, le planificateur (détection de doublons, d'écrasement, de
no-op), l'exécuteur (échanges, cycles, rollback, annulation, undo, absence de fichiers
temporaires résiduels), le scanner de dossiers, la comparaison de versions pour la vérification
des mises à jour, la parité des ressources entre les quatre langues, les règles d'identité de
marque et les bindings XAML.

Certaines de ces protections existent parce que des bugs sont arrivés dans une build publiée :

- **Des bindings deux-ways sur des propriétés en lecture seule.** WPF lie `ProgressBar.Value`,
  `TextBox.Text` et `ToggleButton.IsChecked` *par défaut* en deux-ways : les pointer vers une
  propriété calculée du ViewModel lève une exception dès le premier layout de la fenêtre —
  l'app affiche une boîte « Problem » au lieu de s'ouvrir. Tout nouveau binding sur ces
  propriétés doit préciser `Mode=OneWay` ou exposer un setter public.
- **La marque doit rester hors des fichiers de langue.** `BrandIdentityTests` échoue si
  « RandoFile » apparaît dans une valeur `Strings.*.xaml` : un traducteur pourrait alors le
  renommer ou le supprimer.

L'écran d'accueil a sa propre garde de timing : il ne peut pas se fermer tant que tous ses
storyboards ne sont pas terminés, et la ligne de crédit doit rester posée et lisible au
minimum une seconde et demie après — un temps ne peut jamais être coupé en deux et transformer
l'ouverture en scintillement.

`InstallerScriptTests` protège le script Inno Setup de la même façon. Il n'est compilé que
dans la CI : sans cela, ces échecs n'apparaîtraient qu'au moment de la release — une version
périmée dans l'installateur, un `AppId` qui a cessé d'être un GUID (ce qui donne
silencieusement à chaque utilisateur une deuxième copie au lieu d'une mise à jour), un langage
manquant dans le script, ou une traduction à moitié finie qu'Inno Setup remplace
silencieusement par l'anglais.

L'assistant lui-même est vérifié en l'exécutant. `scripts-verify/verify-assoc.ps1` installe en
mode utilisateur (`/CURRENTUSER`, donc sans UAC) avec la tâche d'association, relit chaque
entrée de registre, puis installe sans la tâche et désinstalle — en vérifiant à chaque fois
que les bonnes clés existent, et uniquement elles. `scripts-verify/verify-machine.ps1` fait
de même pour une installation machine, qui demande un shell élevé. `scripts-verify/verify-finish-page.ps1`
conduit le vrai assistant jusqu'à sa dernière page et confirme que les trois options sont là
et que Finish ouvre l'app, le dossier d'installation et la page du projet.
`scripts-verify/verify-signing.ps1` signe une **copie** jetable de l'exécutable publié avec
un certificat qu'il crée puis supprime : la chaîne de signature est donc prouvée même sur une
machine qui n'a aucun certificat.

---

## 📦 Versionnage et releases

La version vit à un seul endroit, [`Directory.Build.props`](Directory.Build.props)
(`VersionPrefix`), et s'affiche dans l'UI comme `v1.0.0`.

Les releases suivent [Semantic Versioning](https://semver.org/). La publication est
automatisée : pousser une balise `v*` construit l'exécutable et crée une GitHub Release.
L'application lit déjà `releases/latest` et compare les versions avec un parseur SemVer
normalisé — ajouter un téléchargement automatique de mise à jour est donc une petite étape
suivante.

### Signature de code

Les releases sont publiées **sans signature par défaut** — c'est pourquoi SmartScreen de
Windows avertit tant qu'aucun certificat n'est configuré. Quand il l'est, le workflow signe
deux fois : l'exécutable `RandoFile.exe` publié avant qu'Inno Setup ne l'emballage, puis
l'installateur ensuite — un programme signé dans un installateur non signé ne rend pas le
téléchargement fiable à lui seul. Chaque signature est horodatée, donc reste valide après
l'expiration du certificat.

Deux secrets de dépôt activent le tout (*Settings → Secrets and variables → Actions*) :

| Secret | Valeur |
|---|---|
| `WINDOWS_CERT_PFX_BASE64` | votre certificat en `.pfx`, encodé en base64 |
| `WINDOWS_CERT_PASSWORD` | le mot de passe de ce `.pfx` |

Pour produire le premier depuis un certificat local :

```powershell
[Convert]::ToBase64String([IO.File]::ReadAllBytes('certificates\code-signing.pfx')) | Set-Clipboard
```

Pour une release construite sur votre propre machine, placez le certificat à
`certificates\code-signing.pfx` — c'est exactement le chemin que
[`tools/Sign-Release.ps1`](tools/Sign-Release.ps1) cherche, et `certificates/` est ignoré par
git pour qu'une clé privée ne puisse jamais être commitée — puis lancez :

```powershell
tools\Sign-Release.ps1 -Files dist\installer\RandoFile-Setup-v1.0.0-win-x64.exe
```

Si le fichier est protégé par mot de passe, ajoutez `-Password <password>` (ou définissez
`RANDOFILE_CERT_PASSWORD`). Quand le certificat est installé dans le magasin de certificats
Windows — c'est là qu'un jeton matériel ou le client d'un service de signature le place —
signez par empreinte :

```powershell
tools\Sign-Release.ps1 -Files dist\win-x64\RandoFile.exe -Thumbprint <thumbprint>
```

Depuis juin 2023, une autorité de certification publiquement de confiance n'a pas le droit
d'émettre une clé privée exportable : elle doit vivre sur un jeton matériel, un HSM ou un
service de signature cloud. Un certificat public passe donc généralement par `-Thumbprint`,
tandis que la voie `.pfx` convient aux certificats auto-signés et internes. Les services
cloud (Azure Trusted Signing, DigiCert KeyLocker, SSL.com eSigner) signent avec leur propre
outil, qui remplace les deux étapes de signature du workflow.

`scripts-verify/verify-signing.ps1` prouve toute la chaîne sur une machine sans aucun
certificat : il crée un certificat auto-signé jetable, signe une **copie** de l'exécutable
publié, vérifie que la signature porte un horodatage, puis supprime le certificat, le `.pfx`
et la copie.

Voir [CHANGELOG.md](CHANGELOG.md) pour l'historique des releases.

---

## 👤 À propos du créateur

**Hossein Moradi** — HM
Technicien en génie de la puissance électrique
Génie électrique de puissance · Développeur d'applications logicielles et web · Créateur de sites web · Gestion et planification de projets

---

## 📄 Licence

MIT © 2026 Hossein Moradi — voir [LICENSE](LICENSE).

<br/>

<div align="center">

<img src="src/RandoFile.App/Assets/Brand/logo-mark.png" width="64" alt="RandoFile" />

<p>Si RandoFile vous a rendu service, offrez-lui une étoile ⭐ sur ce dépôt — ça compte vraiment.</p>

<p>Créé avec 💛 par <b>Hossein Moradi</b></p>

</div>
