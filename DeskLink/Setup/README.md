# DeskLink 설치 프로그램 빌드 가이드

## 필요 사항

### 1. Inno Setup 설치
- [Inno Setup 6.0 이상 다운로드](https://jrsoftware.org/isdl.php)
- 설치 후 시스템 PATH에 추가 권장

### 2. .NET 8.0 SDK
- 애플리케이션 빌드를 위해 필요
- [.NET 8.0 SDK 다운로드](https://dotnet.microsoft.com/download/dotnet/8.0)

## 빌드 절차

### 자동 빌드 (PowerShell 스크립트 사용)

```powershell
# 1. 빌드 스크립트 실행
.\Setup\Build-Installer.ps1

# 2. 버전 지정 빌드
.\Setup\Build-Installer.ps1 -Version "1.0.1"
```

### 수동 빌드

#### 1단계: Release 빌드
```bash
cd D:\mySource\DeskLink\DeskLink
dotnet build -c Release
```

#### 2단계: 출력 확인
빌드 결과물이 다음 경로에 생성되었는지 확인:
```
DeskLink\bin\Release\net8.0-windows\
```

필요한 파일:
- ? DeskLink.exe
- ? *.dll (모든 의존성)
- ? runtimes\ 폴더
- ? share-link_1388978.ico

#### 3단계: Inno Setup 컴파일
```bash
# GUI 사용
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" Setup\DeskLink.iss

# 또는 Inno Setup GUI에서 DeskLink.iss 열고 Build > Compile
```

#### 4단계: 설치 파일 확인
```
Setup\Output\DeskLink-Setup-1.0.0.exe
```

## 출력 결과

### 설치 프로그램 파일
- **위치**: `Setup\Output\DeskLink-Setup-{version}.exe`
- **크기**: 약 50-80MB (압축됨)
- **형식**: Windows 실행 파일 (.exe)

### 설치 옵션
1. **기본 설치 경로**: `C:\Program Files\DeskLink`
2. **바탕화면 아이콘**: 선택 사항
3. **시작 프로그램 등록**: 선택 사항
4. **빠른 실행 아이콘**: Windows 10 이하만

## 배포 체크리스트

### 설치 프로그램 테스트
- [ ] 깨끗한 시스템에서 설치 테스트
- [ ] .NET Runtime 미설치 시 경고 확인
- [ ] 프로그램 실행 확인
- [ ] 바탕화면 아이콘 생성 확인
- [ ] 시작 메뉴 등록 확인

### 제거 테스트
- [ ] 프로그램 제거
- [ ] 사용자 데이터 보존/삭제 옵션 확인
- [ ] 레지스트리 정리 확인
- [ ] 남은 파일 확인

### 업그레이드 테스트
- [ ] 이전 버전 위에 설치
- [ ] 설정 유지 확인
- [ ] 데이터베이스 마이그레이션 확인

## 버전 관리

### 버전 번호 수정
`DeskLink.iss` 파일에서:
```ini
#define MyAppVersion "1.0.0"
```

### 변경 이력
- **1.0.0** (2024-01-XX)
  - 초기 릴리스
  - 링크 관리 기본 기능
  - Health Check
  - Import/Export

## 문제 해결

### 오류: "Cannot find file"
**원인**: Release 빌드가 없음
**해결**: `dotnet build -c Release` 실행

### 오류: ".NET Runtime not found"
**원인**: 사용자 시스템에 .NET 8.0 Runtime 미설치
**해결**: 설치 프로그램이 자동으로 안내

### 설치 프로그램 크기가 너무 큼
**원인**: 모든 DLL 포함
**해결**: 
- Self-contained 대신 Framework-dependent 배포 고려
- 또는 .NET Runtime 번들 배포

## 고급 설정

### Self-Contained 배포 (Runtime 포함)
```bash
dotnet publish -c Release -r win-x64 --self-contained true
```

DeskLink.iss 수정:
```ini
Source: "..\bin\Release\net8.0-windows\publish\**"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
```

### 코드 서명 (선택)
```ini
[Setup]
SignTool=signtool
SignedUninstaller=yes
```

## 지원

문제가 발생하면 다음을 확인하세요:
1. Inno Setup 컴파일 로그
2. Release 빌드 출력
3. 설치 로그: `%TEMP%\Setup Log YYYY-MM-DD #XXX.txt`

---
**생성 일자**: 2024-01-XX  
**Inno Setup 버전**: 6.0+  
**.NET 버전**: 8.0
