#define MyAppVersion "2.4.1"
#define MyAppName "BingWallpaper"

[Setup]
AppId={{F1B1B91D-CFA3-498C-A858-2F6585F46CB7}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName="{#MyAppName} {#MyAppVersion}"
AppPublisher=Ali Torabi
AppPublisherURL=https://github.com/torabi-ali
AppSupportURL=https://github.com/torabi-ali/BingWallpaper
AppUpdatesURL=https://github.com/torabi-ali/BingWallpaper
DefaultDirName={commonpf}\{#MyAppName}
ArchitecturesInstallIn64BitMode=x64
DisableDirPage=auto
DisableProgramGroupPage=auto
OutputDir=Setup
OutputBaseFilename=Setup-{#MyAppName}-{#MyAppVersion}
SolidCompression=yes
Compression=lzma2/ultra64

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "bin\Release\net7.0-windows\win-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Dirs]
Name: "{app}"; Permissions: users-full
Name: "{app}\Logs"; Permissions: users-full

[Icons]
Name: "{commonprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppName}.exe"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppName}.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppName}.exe"; Description: "{cm:LaunchProgram,{#MyAppName}}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: files; Name: "{app}\{#MyAppName}.db"
Type: filesandordirs; Name: "{app}\Logs"

