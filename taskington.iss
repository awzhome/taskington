#define APP_NAME "Taskington"
#define APP_VERSION "0.1.0.99"
#define APP_FULL_VERSION "0.1-preview.99"
#define APP_PUBLISHER "Andreas Weizel"
#define APP_URL "https://github.com/awzhome/taskington"
#define APP_EXE_FILE "taskington-gui.exe"

[Setup]
AppId={{D32AB7B1-334D-43A1-9A7C-EE692FCA7A8A}
AppName={#APP_NAME}
AppVersion={#APP_VERSION}
AppVerName={#APP_NAME} {#APP_FULL_VERSION}
AppPublisher={#APP_PUBLISHER}
AppPublisherURL={#APP_URL}
AppSupportURL={#APP_URL}
AppUpdatesURL={#APP_URL}
DefaultDirName={userpf}\{#APP_NAME}
DisableProgramGroupPage=yes
DisableWelcomePage=false
PrivilegesRequired=lowest
OutputBaseFilename=taskington-setup-win64-{#APP_FULL_VERSION}
OutputDir=output
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; Flags: unchecked
Name: "autostart"; Description: "{cm:AutoStartProgram,{#APP_NAME}}"

[Files]
Source: "output\publish-win64\*"; DestDir: "{app}\{#APP_FULL_VERSION}"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\{#APP_NAME}"; Filename: "{app}\{#APP_FULL_VERSION}\{#APP_EXE_FILE}"
Name: "{autodesktop}\{#APP_NAME}"; Filename: "{app}\{#APP_FULL_VERSION}\{#APP_EXE_FILE}"; Tasks: desktopicon
Name: "{userstartup}\{#APP_NAME}"; Filename: "{app}\{#APP_FULL_VERSION}\{#APP_EXE_FILE}"; Tasks: autostart

[Run]
Filename: "{app}\{#APP_FULL_VERSION}\{#APP_EXE_FILE}"; Description: "{cm:LaunchProgram,{#StringChange(APP_NAME, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

