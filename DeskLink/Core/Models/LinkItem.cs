using System;

namespace DeskLink.Core.Models
{
 /// <summary>
 /// 링크 항목 도메인 모델 (핵심 속성만 우선 정의)
 /// </summary>
 public class LinkItem
 {
 // 고유 식별자
 public Guid Id { get; set; }
 // 표시 이름
 public string Name { get; set; } = string.Empty;
 // 설명
 public string? Description { get; set; }
 // 링크 유형
 public LinkType Type { get; set; }
 // 대상 (URL, 경로, 실행파일 등)
 public string Target { get; set; } = string.Empty;
 // 실행 인자
 public string? Arguments { get; set; }
 // 작업 디렉터리
 public string? WorkingDir { get; set; }
 // 아이콘 키
 public string? IconKey { get; set; }
 // 색상(hex)
 public string? ColorHex { get; set; }
 // 태그(세미콜론 연결 문자열 저장 예정)
 public string? Tags { get; set; }
 // 카테고리
 public string? Category { get; set; }
 // 소유자
 public string? Owner { get; set; }
 // 가시성 규칙 표현식
 public string? VisibilityRule { get; set; }
 // 생성/수정/건강검사 시간
 public DateTime CreatedAt { get; set; }
 public DateTime UpdatedAt { get; set; }
 public DateTime? LastCheckedAt { get; set; }
 // 건강 상태
 public LinkHealthStatus Health { get; set; }
 }

 /// <summary>
 /// 링크 유형 열거형
 /// </summary>
 public enum LinkType { Url, File, Folder, Exe, Rdp, Ssh, Custom }

 /// <summary>
 /// 링크 건강 상태 열거형
 /// </summary>
 public enum LinkHealthStatus { Unknown =0, Ok =1, Warning =2, Error =3 }
}
