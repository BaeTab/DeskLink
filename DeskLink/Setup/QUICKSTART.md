# DeskLink 설치 프로그램 빠른 시작

## ?? 가장 빠른 방법

```powershell
# PowerShell에서 실행
cd D:\mySource\DeskLink
.\Setup\Build-Installer.ps1
```

완료! ??

설치 프로그램 위치: `Setup\Output\DeskLink-Setup-1.0.0.exe`

---

## ?? 사전 요구사항

### 필수 설치
1. **Inno Setup 6.0+**
   - 다운로드: https://jrsoftware.org/isdl.php
   - 기본 경로에 설치 권장

2. **.NET 8.0 SDK**
   - 이미 설치되어 있음 (개발 환경)

### 선택 설치
- **코드 서명 도구** (배포용)

---

## ??? 빌드 명령어

### 기본 빌드
```powershell
.\Setup\Build-Installer.ps1
```

### 버전 지정
```powershell
.\Setup\Build-Installer.ps1 -Version "1.0.1"
```

### 빌드 스킵 (이미 빌드된 경우)
```powershell
.\Setup\Build-Installer.ps1 -SkipBuild
```

### 정리 스킵
```powershell
.\Setup\Build-Installer.ps1 -SkipClean
```

### 완료 후 폴더 열기
```powershell
.\Setup\Build-Installer.ps1 -OpenOutput
```

### 조합 예제
```powershell
.\Setup\Build-Installer.ps1 -Version "1.0.2" -SkipClean -OpenOutput
```

---

## ?? 폴더 구조

```
DeskLink\
├── Setup\
│   ├── DeskLink.iss          # Inno Setup 스크립트
│   ├── Build-Installer.ps1   # 자동 빌드 스크립트
│   ├── README.md # 상세 가이드
│   ├── QUICKSTART.md         # 이 파일
│   └── Output\               # 생성된 설치 파일
│       └── DeskLink-Setup-*.exe
├── DeskLink\
│   ├── bin\Release\      # 빌드 출력
│   └── share-link_*.ico   # 아이콘
└── README.md
```

---

## ? 체크리스트

### 첫 빌드
- [ ] Inno Setup 설치 확인
- [ ] .NET SDK 설치 확인
- [ ] 스크립트 실행
- [ ] Output 폴더에 .exe 생성 확인

### 테스트
- [ ] 가상 머신에서 설치 테스트
- [ ] 프로그램 실행 확인
- [ ] 제거 테스트

### 배포
- [ ] 버전 번호 업데이트
- [ ] README 업데이트
- [ ] 릴리스 노트 작성
- [ ] 설치 파일 업로드

---

## ?? 문제 해결

### "ISCC.exe를 찾을 수 없습니다"
**해결**: Inno Setup 설치 후 다시 시도

### "DeskLink.exe를 찾을 수 없습니다"
**해결**: `-SkipBuild` 플래그 제거

### 빌드는 성공했지만 설치 파일이 없음
**해결**: `Setup\Output` 폴더 확인, Inno Setup 로그 확인

---

## ?? 지원

문제가 계속되면:
1. `Setup\README.md` 상세 가이드 참조
2. Inno Setup 로그 확인
3. PowerShell 오류 메시지 복사

---

**마지막 업데이트**: 2024-01-XX  
**버전**: 1.0.0
