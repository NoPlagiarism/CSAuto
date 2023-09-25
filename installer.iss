; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define NAME "CSAuto"
#define EXE_NAME "CSAuto.exe"
#define InstallPath "C:\Program Files (x86)\"
#define AppWebsite "https://csauto.netlify.app"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{F5E4D811-7A92-47F9-BEEC-B6751463D69B}
AppName={#NAME}
AppVersion={#VERSION_NAME}
AppPublisherURL={#AppWebsite}
AppSupportURL={#AppWebsite}
AppUpdatesURL={#AppWebsite}
AppPublisher=Murky
Compression=lzma
LicenseFile=LICENSE
DisableProgramGroupPage=yes
DefaultDirName={#InstallPath}{#NAME}
OutputBaseFilename={#NAME}_Installer
PrivilegesRequired=admin
SolidCompression=yes
SetupIconFile=src\CSAuto\Icons\main.ico
UninstallDisplayIcon={app}\{#EXE_NAME}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "src\CSAuto\bin\Release\*.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "src\CSAuto\bin\Release\*.dll"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{autoprograms}\{#NAME}"; Filename: "{app}\{#EXE_NAME}"
Name: "{autodesktop}\{#NAME}"; Filename: "{app}\{#EXE_NAME}"; Comment: "Software to automate usual CS in game tasks, such as accepting match, buying items and much more!"; Tasks: desktopicon

[Run]
Filename: "{app}\{#EXE_NAME}"; Description: "{cm:LaunchProgram,{#StringChange(NAME, '&', '&&')}}"; Flags: nowait postinstall skipifsilent unchecked
