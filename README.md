<div align="center">

# 🔗 DeskLink — Windows 링크 관리 & 빠른 실행 런처

**URL · 파일 · 폴더 · 실행파일 · 네트워크 경로를 한 곳에서 관리하고, 어디서나 `Alt + Space` 로 즉시 실행하는 Windows 데스크톱 런처**

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Platform](https://img.shields.io/badge/platform-Windows%2010%2F11-0078D6?logo=windows)](https://www.microsoft.com/windows)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE.txt)
[![UI](https://img.shields.io/badge/UI-DevExpress%20WPF-orange)](https://www.devexpress.com/)
[![Release](https://img.shields.io/github/v/release/BaeTab/DeskLink?include_prereleases&label=download)](https://github.com/BaeTab/DeskLink/releases/latest)
[![Stars](https://img.shields.io/github/stars/BaeTab/DeskLink?style=social)](https://github.com/BaeTab/DeskLink)

[⬇️ 설치 파일 다운로드](https://github.com/BaeTab/DeskLink/releases/latest) · [한국어](#-한국어) · [English](#-english)

</div>

---

> **DeskLink** 는 즐겨 쓰는 웹사이트, 사내 시스템, 공유 폴더(NAS), 자주 쓰는 프로그램을
> 하나의 깔끔한 대시보드에서 관리하는 무료 오픈소스 **Windows 링크 관리자 / 바로가기 런처 / 북마크 매니저** 입니다.
> 카테고리 분류, 즐겨찾기, 실시간 검색, 링크 상태 점검(Health Check), 전역 단축키 빠른 실행을 지원합니다.

---

<a name="-한국어"></a>
## 🇰🇷 한국어

### 📑 목차
- [주요 기능](#-주요-기능)
- [이런 분께 추천](#-이런-분께-추천)
- [설치 방법](#-설치-방법)
- [사용 방법](#-사용-방법)
- [기술 스택](#-기술-스택)
- [데이터 저장 위치](#-데이터-저장-위치)
- [자주 묻는 질문 (FAQ)](#-자주-묻는-질문-faq)
- [로드맵](#-로드맵)
- [기여하기](#-기여하기)

### ✨ 주요 기능

#### 🔗 통합 링크 관리
- **URL 북마크** — 자주 방문하는 웹사이트를 한곳에 저장
- **파일 / 폴더** — 드래그 앤 드롭으로 손쉽게 추가
- **네트워크 경로** — NAS, 공유 폴더(UNC) 바로가기
- **실행 파일 / RDP / SSH** — 자주 쓰는 프로그램·원격 접속을 빠르게 실행

#### ⚡ 생산성 향상
- **전역 빠른 실행** (`Alt` + `Space`) — 어떤 화면에서든 즉시 검색 후 실행
- **카테고리 분류 & 즐겨찾기(★)** — 업무·개발·자료를 깔끔하게 정리, 즐겨찾기 우선 정렬
- **실시간 검색** — 입력과 동시에 디바운스 필터링
- **정렬 옵션** — 이름 · 최근 사용 · 사용 빈도
- **단축키** — `Ctrl + F` 검색 포커스, `Esc` 검색어 지우기

#### 💚 링크 상태 점검 (Health Check)
- **URL 접속 확인** — HEAD/GET 요청으로 웹사이트 도달 여부 점검 (느린 사이트는 경고로 구분)
- **파일·폴더 존재 확인** — 깨진 링크 자동 탐지
- **일괄 점검** — 모든 링크를 한 번에 검사하고 진행률 표시
- **상태 표시** — 🟢 정상 · 🟠 경고 · 🔴 오류 · ⚪ 알 수 없음

#### 🎨 직관적인 UI
- **타일 뷰 / 그리드 뷰** — 카드형 또는 표 형태로 전환
- **다크 테마** — 눈이 편안한 인터페이스 (DevExpress WPF)
- **타입별 아이콘 · 사용 횟수 배지 · 상세 툴팁**
- **링크별 색상 커스터마이징** — 내장 컬러 피커

#### 💾 데이터 관리
- **JSON / CSV 가져오기·내보내기** — 백업과 이관이 간편
- **로컬 SQLite DB** — 빠르고 가벼운 저장
- **컨텍스트 메뉴** — 대상 복사, 파일 위치 열기, 편집, 삭제

### 👍 이런 분께 추천
- 북마크가 브라우저·바탕화면·탐색기에 흩어져 있는 분
- 사내 ERP·위키·메일·NAS 등 **여러 시스템을 자주 오가는 직장인/개발자**
- 자주 쓰는 폴더와 프로그램을 **키보드만으로 빠르게 열고 싶은** 분

### 📥 설치 방법

#### 시스템 요구사항
- Windows 10 / 11 (x64)
- [.NET 8.0 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) (설치 시 자동 안내)

#### 방법 1 — 설치 프로그램 (권장)
1. [최신 릴리즈](https://github.com/BaeTab/DeskLink/releases/latest) 에서 `DeskLink-Setup-1.0.0.exe` 다운로드
2. 실행 후 설치 마법사 진행
3. 바탕화면 아이콘 / Windows 시작 시 자동 실행을 선택적으로 설정

#### 방법 2 — 소스에서 빌드
```bash
git clone https://github.com/BaeTab/DeskLink.git
cd DeskLink/DeskLink
dotnet build -c Release
dotnet run -c Release
```

### 🚀 사용 방법
1. **링크 추가** — `➕ New Link` 클릭 또는 파일/URL 을 창에 끌어다 놓기 → 이름·대상·카테고리·색상 지정 후 `💾 저장`
2. **실행** — 타일 더블클릭(또는 그리드 더블클릭), 단일클릭은 선택
3. **빠른 실행** — 어디서나 `Alt + Space` → 이름 검색 → `Enter`
4. **상태 점검** — `💚 Health Check` 로 모든 링크의 유효성 확인

### 🛠️ 기술 스택
| 분류 | 기술 |
|------|------|
| UI 프레임워크 | DevExpress WPF 25.1 |
| 런타임 | .NET 8.0 (Windows Desktop) |
| 아키텍처 | MVVM (DevExpress POCO ViewModel) |
| 데이터베이스 | SQLite + Entity Framework Core 8 |
| 로깅 | Serilog |
| 설치 관리자 | Inno Setup 6 |

### 🗂️ 데이터 저장 위치
| 항목 | 경로 |
|------|------|
| 데이터베이스 | `%LOCALAPPDATA%\DeskLink\desklink.db` |
| 로컬 상태(즐겨찾기/빈도) | `%LOCALAPPDATA%\DeskLink\state.json` |
| 설정 | `%LOCALAPPDATA%\DeskLink\settings.json` |
| 로그 | `%LOCALAPPDATA%\DeskLink\logs\` |

### ❓ 자주 묻는 질문 (FAQ)
**Q. 무료인가요?** 네, MIT 라이선스의 완전한 무료 오픈소스입니다.

**Q. 인터넷 연결이 필요한가요?** 아니요. 모든 데이터는 로컬에 저장되며, 외부 서버로 전송되지 않습니다. (URL Health Check 시에만 해당 사이트로 요청)

**Q. 설치 시 .NET 런타임 오류가 나요.** [.NET 8.0 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) 을 설치한 뒤 다시 실행하세요.

**Q. 데이터를 다른 PC로 옮기려면?** `📤 JSON` 또는 `📤 CSV` 로 내보낸 뒤 새 PC에서 `📥` 로 가져오세요.

### 🗺️ 로드맵
- [ ] v1.1 — 클라우드 동기화 (OneDrive / Dropbox)
- [ ] v1.2 — 브라우저 확장 프로그램
- [ ] v1.3 — 시스템 트레이 상주 & 모바일 동반 앱
- [ ] v2.0 — AI 기반 링크 추천

### 🤝 기여하기
이슈와 PR 환영합니다! [Issues](https://github.com/BaeTab/DeskLink/issues) 에 버그·기능 제안을 남겨주세요.
1. Fork → 2. `git checkout -b feature/foo` → 3. 커밋 → 4. Push → 5. Pull Request

### 📄 라이선스
[MIT License](LICENSE.txt)

---

<a name="-english"></a>
## 🇬🇧 English

**DeskLink** is a free, open-source **link manager and quick-launch app for Windows**. Save and organize your
URLs, files, folders, network shares (NAS), and programs in one clean dashboard, then launch anything instantly
with a global `Alt + Space` hotkey.

### ✨ Features
- **Unified link management** — URLs, files, folders, executables, RDP/SSH, UNC network paths
- **Global quick launch** (`Alt + Space`) — instant fuzzy search and run from anywhere
- **Categories & favorites (★)** with favorite-first sorting and real-time debounced search
- **Health Check** — verify URL reachability (HEAD/GET) and file/folder existence, batch scan with progress
- **Tile & grid views**, dark theme, per-link colors, type icons, usage counters, rich tooltips
- **Import/Export** in JSON and CSV, fast local SQLite storage
- **Context menu** — copy target, open file location, edit, delete
- **Keyboard shortcuts** — `Ctrl + F` focus search, `Esc` clear

### 📥 Installation
**Requirements:** Windows 10/11 (x64) and [.NET 8.0 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0).

**Installer (recommended):** download `DeskLink-Setup-1.0.0.exe` from the [latest release](https://github.com/BaeTab/DeskLink/releases/latest).

**Build from source:**
```bash
git clone https://github.com/BaeTab/DeskLink.git
cd DeskLink/DeskLink
dotnet build -c Release
dotnet run -c Release
```

### 🛠️ Tech Stack
DevExpress WPF 25.1 · .NET 8.0 · MVVM (POCO) · SQLite + EF Core 8 · Serilog · Inno Setup 6

### 📄 License
[MIT](LICENSE.txt) — contributions welcome via [Issues](https://github.com/BaeTab/DeskLink/issues) and Pull Requests.

---

<div align="center">

**Keywords:** Windows 링크 관리, 바로가기 런처, 북마크 관리자, quick launcher, link manager, bookmark manager,
shortcut launcher, NAS shortcut, URL organizer, Alt+Space launcher, .NET 8 WPF, DevExpress, SQLite

⭐ 이 프로젝트가 마음에 드신다면 Star 를 눌러주세요! · If you find DeskLink useful, please give it a ⭐

Made with ❤️ using .NET 8 & DevExpress WPF

</div>
