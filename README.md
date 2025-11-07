# ?? DeskLink

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE.txt)
[![Platform](https://img.shields.io/badge/platform-Windows-blue)](https://www.microsoft.com/windows)
[![DevExpress](https://img.shields.io/badge/UI-DevExpress%20WPF-orange)](https://www.devexpress.com/)

> **Windows용 통합형 링크 관리 데스크톱 애플리케이션**  
> URL, 파일, 폴더를 한 곳에서 관리하고 빠르게 실행하세요!

[English](#english) | [한국어](#korean)

---

<a name="korean"></a>
## ???? 한국어

### ? 주요 기능

#### ?? 통합 링크 관리
- **URL 바로가기** - 자주 방문하는 웹사이트 저장
- **파일/폴더** - 끌어다 놓기로 손쉽게 추가
- **네트워크 경로** - NAS, 공유 폴더 바로가기
- **실행 파일** - 자주 쓰는 프로그램 빠른 실행

#### ?? 생산성 향상
- **빠른 실행** (`Alt` + `Space`) - 어디서나 즉시 검색 & 실행
- **카테고리 분류** - 업무, 개발, 파일 등으로 정리
- **정렬 옵션** - 이름, 최근 사용, 빈도 순서
- **검색 필터** - 실시간 필터링

#### ?? Health Check
- **URL 상태 확인** - 웹사이트 접속 가능 여부 체크
- **파일 존재 확인** - 깨진 링크 자동 탐지
- **일괄 체크** - 모든 링크 한번에 검사
- **상태 아이콘** - ? 정상 / ? 오류 / ? 알수없음

#### ?? 직관적 친화적 UI
- **타일 뷰** - 시각적으로 보기 좋은 카드 레이아웃
- **그리드 뷰** - 상세 정보 표시
- **다크 테마** - 눈에 편한 인터페이스
- **색상 커스터마이제이션** - 링크별 색상 지정 (Color Picker 내장)

#### ?? 데이터 관리
- **Import/Export** - JSON 파일 백업
- **로컬 DB** - SQLite 기반 빠른 저장
- **설정 동기화** - 사용자 데이터 보존
- **자동 시작** - Windows 부팅 시 자동 실행 (선택)

---

### ?? 스크린샷

#### 메인 화면 - 타일 뷰
```
┌────────────────────────────────────────────────────────────┐
│ ?? 검색...    ? Grid  ▦ Tiles  ? New │
├────────────────────────────────────────────────────────────┤
│ ?? Categories │        타일 영역    │
│   All     │  ┌────────┐  ┌────────┐      │
│ ? 업무    │  │ ERP │  │Mail │      │
│   개발   │  └────────┘  └────────┘    │
│   파일        │  ┌────────┐  ┌────────┐      │
│    등  │  │Wiki │  │NAS  │      │
│  │  └────────┘  └────────┘      │
└────────────────────────────────────────────────────────────┘
```

#### 속성 패널
- 이름, 타입, 경로, 카테고리
- 색상 피커 (?? Pick)
- 태그, Health 상태
- 저장/삭제 버튼

---

### ?? 빠른 시작

#### 시스템 요구사항
- Windows 10 이상
- .NET 8.0 Desktop Runtime ([다운로드](https://dotnet.microsoft.com/download/dotnet/8.0))

#### 설치 방법

**옵션 1: 설치 프로그램 (권장)**
1. [최신 릴리스](https://github.com/yourusername/desklink/releases) 다운로드
2. `DeskLink-Setup-x.x.x.exe` 실행
3. 설치 마법사 따라하기

**옵션 2: 소스 빌드**
```bash
git clone https://github.com/yourusername/desklink.git
cd desklink/DeskLink
dotnet build -c Release
dotnet run
```

---

### ?? 사용 가이드

#### 1. 링크 추가
- **New Link** 버튼 클릭
- 이름, URL/경로 입력
- 카테고리, 색상 선택
- **Save** 클릭

#### 2. 빠른 실행
- `Alt` + `Space` 단축키
- 링크 이름 검색
- `Enter`로 실행

#### 3. Health Check
- ?? **Health Check** 버튼
- 모든 링크 상태 확인
- ? 깨진 링크 수정

---

### ??? 기술 스택

| 분류 | 기술 |
|------|------|
| **UI Framework** | DevExpress WPF 25.1 |
| **Runtime** | .NET 8.0 |
| **Architecture** | MVVM (POCO ViewModel) |
| **Database** | SQLite + EF Core 8.0 |
| **Logging** | Serilog |
| **Installer** | Inno Setup 6.0 |

---

### ?? 프로젝트 구조

```
DeskLink/
├── DeskLink/     # 메인 WPF 프로젝트
│   ├── Core/         # 핵심 로직
│   │   ├── Models/    # 데이터 모델
│ │   ├── ViewModels/      # MVVM ViewModel
│   │   ├── Abstractions/      # 인터페이스
│   │   └── Converters/   # XAML 컨버터
│   ├── Infrastructure/    # 구현 레이어
│   │   ├── Repositories/    # 데이터 접근
│   │   ├── Services/          # 비즈니스 로직
│   │   └── Data/      # EF Core DbContext
│   ├── Views/              # XAML 뷰
│   └── Setup/          # 설치 스크립트
├── LICENSE.txt
└── README.md
```

---

### ?? 기여하기

기여를 환영합니다! 다음 단계로 진행하세요:

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

### ?? 로드맵

- [ ] **v1.1.0** - 클라우드 동기화 (OneDrive, Dropbox)
- [ ] **v1.2.0** - 브라우저 확장 프로그램
- [ ] **v1.3.0** - 모바일 앱 (Android/iOS)
- [ ] **v2.0.0** - AI 기반 링크 추천

---

### ?? 파일 저장 위치

| 항목 | 경로 |
|------|------|
| **데이터베이스** | `%LOCALAPPDATA%\DeskLink\desklink.db` |
| **설정 파일** | `%APPDATA%\DeskLink\settings.json` |
| **로그 파일** | `%LOCALAPPDATA%\DeskLink\Logs\` |

---

### ?? 알려진 이슈

- Windows 7은 지원하지 않습니다 (.NET 8 제약)
- 네트워크 드라이브는 Health Check가 느릴 수 있습니다

---

### ?? 라이선스

본 프로젝트는 MIT 라이선스로 배포됩니다. [LICENSE.txt](LICENSE.txt) 참조

---

### ?? 연락처

- **이슈**: [GitHub Issues](https://github.com/yourusername/desklink/issues)
- **이메일**: your.email@example.com
- **블로그**: https://yourblog.com

---

### ? 후원하기

본 프로젝트가 유용하다면 ? Star를 눌러주세요!

[![Star History Chart](https://api.star-history.com/svg?repos=yourusername/desklink&type=Date)](https://star-history.com/#yourusername/desklink&Date)

---

<a name="english"></a>
## ???? English

### ? Key Features

#### ?? Unified Link Management
- **URL Bookmarks** - Save frequently visited websites
- **Files/Folders** - Easy drag & drop addition
- **Network Paths** - NAS, shared folder shortcuts
- **Executables** - Quick launch frequently used programs

#### ?? Productivity Boost
- **Quick Launch** (`Alt` + `Space`) - Instant search & launch anywhere
- **Category Organization** - Work, Development, Files, etc.
- **Sort Options** - Name, Recent, Frequency
- **Search** - Real-time filtering

#### ?? Health Check
- **URL Status** - Check website accessibility
- **File Existence** - Auto-detect broken links
- **Batch Check** - Inspect all links at once
- **Status Icons** - ? OK / ? Error / ? Unknown

#### ?? User-Friendly UI
- **Tile View** - Beautiful card layout
- **Grid View** - Detailed information
- **Dark Theme** - Eye-friendly interface
- **Color Customization** - Custom colors per link (Built-in Color Picker)

#### ?? Data Management
- **Import/Export** - JSON format backup
- **Local DB** - Fast SQLite-based storage
- **Settings Sync** - User data preservation
- **Auto Start** - Launch on Windows boot (optional)

---

### ?? Quick Start

#### Prerequisites
- Windows 10 or later
- .NET 8.0 Desktop Runtime ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))

#### Installation

**Option 1: Installer (Recommended)**
1. Download [Latest Release](https://github.com/yourusername/desklink/releases)
2. Run `DeskLink-Setup-x.x.x.exe`
3. Follow installation wizard

**Option 2: Build from Source**
```bash
git clone https://github.com/yourusername/desklink.git
cd desklink/DeskLink
dotnet build -c Release
dotnet run
```

---

### ??? Tech Stack

| Category | Technology |
|----------|------------|
| **UI Framework** | DevExpress WPF 25.1 |
| **Runtime** | .NET 8.0 |
| **Architecture** | MVVM (POCO ViewModel) |
| **Database** | SQLite + EF Core 8.0 |
| **Logging** | Serilog |
| **Installer** | Inno Setup 6.0 |

---

### ?? Roadmap

- [ ] **v1.1.0** - Cloud Sync (OneDrive, Dropbox)
- [ ] **v1.2.0** - Browser Extension
- [ ] **v1.3.0** - Mobile App (Android/iOS)
- [ ] **v2.0.0** - AI-based Link Recommendations

---

### ?? License

This project is licensed under the MIT License - see [LICENSE.txt](LICENSE.txt)

---

### ? Support

If you find this project useful, please consider giving it a ? Star!

---

**Made with ?? using .NET 8 & DevExpress WPF**
