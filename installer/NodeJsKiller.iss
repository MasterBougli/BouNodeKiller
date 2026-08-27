#define MyAppName "BOU NodeJS Killer"
#define MyAppVersion GetEnv("APP_VERSION")
#define MyAppPublisher "MasterBougli"
#define MyAppExeName "NodeJsKiller.exe"
#define MyAppId "{{8B4E5F2A-7B3B-4D2D-A143-4A94E6E22B70}}"
#define PublishDir GetEnv("PUBLISH_DIR")

[Setup]
AppId={#MyAppId}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\BOU NodeJS Killer
DefaultGroupName=BOU NodeJS Killer
DisableProgramGroupPage=yes
OutputDir={#GetEnv("INSTALLER_OUTPUT_DIR")}
OutputBaseFilename=BOU-NodeJSKiller-Setup-x64
Compression=lzma2
SolidCompression=yes
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
PrivilegesRequired=lowest
UninstallDisplayIcon={app}\{#MyAppExeName}

[Files]
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\BOU NodeJS Killer"; Filename: "{app}\{#MyAppExeName}"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Lancer BOU NodeJS Killer"; Flags: nowait postinstall skipifsilent
