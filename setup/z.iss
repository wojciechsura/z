; Script generated by the Inno Script Studio Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Z"
#define MyAppVersion "1.0"
#define MyAppPublisher "Spooksoft"
#define MyAppURL "http://www.spooksoft.pl"
#define MyAppExeName "Z.exe"

; #define PGS
#define WSCAD

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{AC4CD13C-B814-4D1F-B366-54E8D61396A7}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\Spooksoft\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
OutputDir=.
OutputBaseFilename=setupZ
SetupIconFile=Setup.ico
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "autostart"; Description: "Start Z automatically with Windows"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "..\Z\bin\Release\Z.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Z\bin\Release\ControlPanelModule.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Z\bin\Release\CustomCommandsModule.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Z\bin\Release\DesktopModule.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Z\bin\Release\Filesystem.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Z\bin\Release\FavoritesModule.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Z\bin\Release\Hardcodet.Wpf.TaskbarNotification.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Z\bin\Release\HashModule.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Z\bin\Release\Microsoft.Practices.ServiceLocation.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Z\bin\Release\Microsoft.Practices.Unity.Configuration.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Z\bin\Release\Microsoft.Practices.Unity.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Z\bin\Release\Microsoft.Practices.Unity.RegistrationByConvention.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Z\bin\Release\Microsoft.WindowsAPICodePack.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Z\bin\Release\Microsoft.WindowsAPICodePack.Shell.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Z\bin\Release\PowerModule.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Z\bin\Release\ProCalc.NET.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Z\bin\Release\ProCalcModule.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Z\bin\Release\ProcessModule.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Z\bin\Release\ProjectsModule.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Z\bin\Release\RunModule.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Z\bin\Release\ShellFoldersModule.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Z\bin\Release\ShortcutModule.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Z\bin\Release\StartMenuModule.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Z\bin\Release\WebSearchModule.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Z\bin\Release\Z.Api.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Z\bin\Release\Z.BusinessLogic.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Z\bin\Release\Z.Common.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Z\bin\Release\Z.Dependencies.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Z\bin\Release\Z.Wpf.dll"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files
Source: "..\SampleModule\Module.cs"; DestDir: "{app}\samples\SampleModule\"; Flags: ignoreversion; Components: CustomModuleSample
Source: "..\SampleModule\SampleModule.csproj"; DestDir: "{app}\samples\SampleModule\"; Flags: ignoreversion; Components: CustomModuleSample
Source: "..\SampleModule\Properties\AssemblyInfo.cs"; DestDir: "{app}\samples\SampleModule\Properties\"; Flags: ignoreversion; Components: CustomModuleSample
Source: "..\SampleModule\Properties\Resources.Designer.cs"; DestDir: "{app}\samples\SampleModule\Properties\"; Flags: ignoreversion; Components: CustomModuleSample
Source: "..\SampleModule\Properties\Resources.resx"; DestDir: "{app}\samples\SampleModule\Properties\"; Flags: ignoreversion; Components: CustomModuleSample
Source: "..\SampleModule\Properties\Settings.Designer.cs"; DestDir: "{app}\samples\SampleModule\Properties\"; Flags: ignoreversion; Components: CustomModuleSample
Source: "..\SampleModule\Properties\Settings.settings"; DestDir: "{app}\samples\SampleModule\Properties\"; Flags: ignoreversion; Components: CustomModuleSample
Source: "..\SampleModule\Resources\sample.png"; DestDir: "{app}\samples\SampleModule\Resources\"; Flags: ignoreversion; Components: CustomModuleSample
#ifdef PGS
Source: "..\PgsModule\bin\Release\PgsModule.dll"; DestDir: "{app}\plugins\"; Flags: ignoreversion
#endif
#ifdef WSCAD
Source: "..\WsCADModule\bin\Release\WsCADModule.dll"; DestDir: "{app}\plugins\"; Flags: ignoreversion
#endif

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{commonstartup}\"; Filename: "{app}\{#MyAppExeName}"; Tasks: autostart

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Components]
Name: "CustomModuleSample"; Description: "Install sample custom module source code"; Types: full

[Dirs]
Name: "{app}\samples\SampleModule\SampleModule"; Components: CustomModuleSample
Name: "{app}\samples\SampleModule\Properties\Properties"; Components: CustomModuleSample
Name: "{app}\samples\SampleModule\Resources\Resources"; Components: CustomModuleSample
Name: "{app}\plugins"
