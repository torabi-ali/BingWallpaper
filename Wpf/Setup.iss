[Setup]
AppId={{F1B1B91D-CFA3-498C-A858-2F6585F46CB7}
AppName=BingWallpaper
AppVersion=2.1.0
AppVerName=BingWallpaper 2.1.0
AppPublisher=Ali Torabi
AppPublisherURL=https://github.com/torabi-ali
AppSupportURL=https://github.com/torabi-ali/BingWallpaper
AppUpdatesURL=https://github.com/torabi-ali/BingWallpaper
DefaultDirName={commonpf}\BingWallpaper
ArchitecturesInstallIn64BitMode=x64
DisableDirPage=yes
DisableProgramGroupPage=yes
OutputDir=Setup
OutputBaseFilename=BingWallpaper-Setup
Compression=lzma
SolidCompression=yes

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
Name: "{commonprograms}\BingWallpaper"; Filename: "{app}\BingWallpaper.exe"
Name: "{commondesktop}\BingWallpaper"; Filename: "{app}\BingWallpaper.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\BingWallpaper.exe"; Description: "{cm:LaunchProgram,BingWallpaper}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: files; Name: "{app}\BingWallpaper.db"
Type: filesandordirs; Name: "{app}\Logs"

