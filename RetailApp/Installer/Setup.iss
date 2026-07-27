#define MyAppName "الرافدين ERP"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "الرافدين"
#define MyAppExeName "RetailApp.exe"

[Setup]
AppId={{D3F9A1B2-5E4C-4D2A-9F8C-1234567890AB}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL=https://www.retailapp.example.com
AppSupportURL=https://www.retailapp.example.com/support
AppUpdatesURL=https://www.retailapp.example.com/updates
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputBaseFilename=الرافدين_Setup_v1.0.0
Compression=lzma2/ultra64
SolidCompression=yes
SetupIconFile=..\icon.ico
UninstallDisplayIcon={app}\{#MyAppExeName}
PrivilegesRequired=admin
ArchitecturesInstallIn64BitMode=x64

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "arabic"; MessagesFile: "compiler:Languages\Arabic.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
; IMPORTANT: Run `dotnet publish -c Release -r win-x64 --self-contained true` before compiling this script.
Source: "..\bin\Release\net8.0-windows\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; Exclude local database from being packaged and overwriting client data if you published with one
; Source: "..\bin\Release\net8.0-windows\win-x64\publish\app.db"; DestDir: "{app}"; Flags: dontcopy

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#MyAppName}}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
; Clean up app directory but preserve LocalAppData settings!
Type: filesandordirs; Name: "{app}"

[Code]
// Custom Code to handle dependency checks (e.g. .NET runtime) can be placed here
function InitializeSetup(): Boolean;
begin
  Result := True;
end;
