using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace DeskLink.Core.Models
{
	/// <summary>
	/// 링크 항목 도메인 모델 (핵심 속성만 우선 정의)
	/// 표시에 사용되는 가변 속성은 INotifyPropertyChanged 로 UI 와 즉시 동기화한다.
	/// </summary>
	public class LinkItem : INotifyPropertyChanged
	{
		// 고유 식별자
		public Guid Id { get; set; }

		private string _name = string.Empty;
		// 표시 이름
		public string Name { get => _name; set => SetField(ref _name, value); }

		private string? _description;
		// 설명
		public string? Description { get => _description; set => SetField(ref _description, value); }

		private LinkType _type;
		// 링크 종류
		public LinkType Type { get => _type; set { if (SetField(ref _type, value)) OnPropertyChanged(nameof(TypeGlyph)); } }

		private string _target = string.Empty;
		// 대상 (URL, 경로, 실행파일 등)
		public string Target { get => _target; set => SetField(ref _target, value); }

		// 실행 인자
		public string? Arguments { get; set; }
		// 작업 디렉터리
		public string? WorkingDir { get; set; }
		// 아이콘 키
		public string? IconKey { get; set; }

		private string? _colorHex;
		// 색상(hex)
		public string? ColorHex { get => _colorHex; set => SetField(ref _colorHex, value); }

		private string? _tags;
		// 태그(세미콜론 구분 문자열 등을 가정)
		public string? Tags { get => _tags; set => SetField(ref _tags, value); }

		private string? _category;
		// 카테고리
		public string? Category { get => _category; set => SetField(ref _category, value); }

		// 소유자
		public string? Owner { get; set; }
		// 가시성 규칙 표현식
		public string? VisibilityRule { get; set; }
		// 생성/수정/건강검사 시각
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }

		private DateTime? _lastCheckedAt;
		public DateTime? LastCheckedAt { get => _lastCheckedAt; set => SetField(ref _lastCheckedAt, value); }

		private LinkHealthStatus _health;
		// 건강 상태
		public LinkHealthStatus Health { get => _health; set => SetField(ref _health, value); }

		// --- 영속화하지 않는 런타임 표시 전용 속성 (EF 매핑 제외) ---

		private bool _isFavorite;
		/// <summary>즐겨찾기 여부. LocalStateService 에서 로드되어 UI 에 반영.</summary>
		[NotMapped, JsonIgnore]
		public bool IsFavorite { get => _isFavorite; set => SetField(ref _isFavorite, value); }

		private int _useCount;
		/// <summary>실행 빈도. LocalStateService 에서 로드.</summary>
		[NotMapped, JsonIgnore]
		public int UseCount { get => _useCount; set => SetField(ref _useCount, value); }

		/// <summary>타입별 이모지 글리프 (타일 표시용).</summary>
		[NotMapped, JsonIgnore]
		public string TypeGlyph => Type switch
		{
			LinkType.Url => "🌐",
			LinkType.File => "📄",
			LinkType.Folder => "📁",
			LinkType.Exe => "⚙️",
			LinkType.Rdp => "🖥️",
			LinkType.Ssh => "🔑",
			_ => "🔗"
		};

		public event PropertyChangedEventHandler? PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string? name = null)
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

		protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? name = null)
		{
			if (Equals(field, value)) return false;
			field = value;
			OnPropertyChanged(name);
			return true;
		}
	}

	/// <summary>
	/// 링크 종류 열거형
	/// </summary>
	public enum LinkType { Url, File, Folder, Exe, Rdp, Ssh, Custom }

	/// <summary>
	/// 링크 건강 상태 열거형
	/// </summary>
	public enum LinkHealthStatus { Unknown = 0, Ok = 1, Warning = 2, Error = 3 }
}
