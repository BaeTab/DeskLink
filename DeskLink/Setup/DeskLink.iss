; DeskLink - 링크 관리 데스크톱 애플리케이션 설치 스크립트
; Inno Setup 6.0 이상 필요

#define MyAppName "DeskLink"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "DeskLink Team"
#define MyAppURL "https://github.com/yourusername/desklink"
#define MyAppExeName "DeskLink.exe"
#define MyAppIcon "share-link_1388978.ico"

[Setup]
; 기본 정보
AppId={{A7B8C9D0-E1F2-4A5B-9C8D-7E6F5A4B3C2D}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
LicenseFile=..\..\LICENSE.txt
InfoBeforeFile=..\..\README.md
OutputDir=Output
OutputBaseFilename=DeskLink-Setup-{#MyAppVersion}
SetupIconFile=..\{#MyAppIcon}
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

; 언어
[Languages]
Name: "korean"; MessagesFile: "compiler:Languages\Korean.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

; 작업
[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 6.1; Check: not IsAdminInstallMode
Name: "startup"; Description: "Windows 시작 시 자동 실행"; GroupDescription: "추가 옵션:"; Flags: unchecked

; 파일
[Files]
; 메인 실행 파일
Source: "..\bin\Release\net8.0-windows\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\Release\net8.0-windows\*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\Release\net8.0-windows\*.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\Release\net8.0-windows\*.config"; DestDir: "{app}"; Flags: ignoreversion skipifsourcedoesntexist
Source: "..\bin\Release\net8.0-windows\runtimes\*"; DestDir: "{app}\runtimes"; Flags: ignoreversion recursesubdirs createallsubdirs

; 아이콘 및 리소스
Source: "..\{#MyAppIcon}"; DestDir: "{app}"; Flags: ignoreversion

; 문서
Source: "..\..\README.md"; DestDir: "{app}"; Flags: ignoreversion isreadme
Source: "..\..\LICENSE.txt"; DestDir: "{app}"; Flags: ignoreversion skipifsourcedoesntexist

; .NET 런타임 체크 (선택사항)
; NOTE: .NET 8.0 Desktop Runtime이 필요합니다
; Source: "dotnet-runtime-8.0-win-x64.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall; Check: not IsDotNetInstalled

; 아이콘
[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\{#MyAppIcon}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\{#MyAppIcon}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

; 시작 프로그램 등록
[Registry]
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "{#MyAppName}"; ValueData: """{app}\{#MyAppExeName}"""; Flags: uninsdeletevalue; Tasks: startup

; 애플리케이션 설정 저장 위치 (사용자별)
Root: HKCU; Subkey: "Software\{#MyAppPublisher}\{#MyAppName}"; Flags: uninsdeletekeyifempty
Root: HKCU; Subkey: "Software\{#MyAppPublisher}\{#MyAppName}\Settings"; Flags: uninsdeletekey

; 실행
[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

; 삭제
[UninstallDelete]
Type: filesandordirs; Name: "{localappdata}\{#MyAppName}"
Type: files; Name: "{userappdata}\{#MyAppName}\*"
Type: dirifempty; Name: "{userappdata}\{#MyAppName}"

; 코드 섹션
[Code]
// .NET 8.0 Desktop Runtime 설치 확인
function IsDotNetInstalled: Boolean;
var
  Success: Boolean;
  ResultCode: Integer;
begin
  // dotnet --list-runtimes 명령으로 확인
  Success := Exec('dotnet', '--list-runtimes', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  Result := Success and (ResultCode = 0);
end;

// 설치 전 확인
function InitializeSetup(): Boolean;
var
  ErrorMessage: String;
begin
  Result := True;
  
  // .NET 8.0 Runtime 체크
  if not IsDotNetInstalled then
  begin
    ErrorMessage := '이 애플리케이션을 실행하려면 .NET 8.0 Desktop Runtime이 필요합니다.' + #13#10 + #13#10 +
       '다음 링크에서 다운로드하여 설치해주세요:' + #13#10 +
       'https://dotnet.microsoft.com/download/dotnet/8.0' + #13#10 + #13#10 +
      '계속 설치하시겠습니까?';
    
    if MsgBox(ErrorMessage, mbConfirmation, MB_YESNO) = IDNO then
      Result := False;
  end;
end;

// 설치 완료 후
procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
  begin
    // 설치 후 추가 작업
    // 예: 기본 설정 파일 생성, 데이터베이스 초기화 등
  end;
end;

// 삭제 전 확인
function InitializeUninstall(): Boolean;
var
  Response: Integer;
begin
  Response := MsgBox('DeskLink을 제거하시겠습니까?' + #13#10 + #13#10 +
       '사용자 데이터(링크, 설정)도 함께 삭제하시겠습니까?' + #13#10 + #13#10 +
          'Yes - 모든 데이터 삭제' + #13#10 +
 'No - 프로그램만 삭제' + #13#10 +
         'Cancel - 제거 취소', 
  mbConfirmation, MB_YESNOCANCEL);
  
  if Response = IDCANCEL then
    Result := False
  else if Response = IDNO then
  begin
    // 프로그램만 삭제 (사용자 데이터 보존)
    Result := True;
  end
  else
  begin
    // 모든 데이터 삭제
  Result := True;
  end;
end;
