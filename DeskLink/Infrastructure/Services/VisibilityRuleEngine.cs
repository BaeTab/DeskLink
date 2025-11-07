using System;
using System.Linq;
using DeskLink.Core.Settings;

namespace DeskLink.Infrastructure.Services
{
	/// <summary>
	/// 가시성 규칙 평가 엔진. 간단한 문자열 기반 규칙.
	/// 예: "Role:Admin" or "Team:Dev"
	/// </summary>
	public class VisibilityRuleEngine
	{
		public bool IsVisible(string? rule, AppSettings settings)
		{
			if (string.IsNullOrWhiteSpace(rule)) return true; // 규칙 없으면 모두 표시
			var parts = rule.Split(new[] { ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
			return parts.Any(p => EvaluateSingle(p.Trim(), settings));
		}

		private bool EvaluateSingle(string rule, AppSettings settings)
		{
			if (rule.StartsWith("Role:", StringComparison.OrdinalIgnoreCase))
			{
				var role = rule.Substring(5);
				return settings.CurrentRole.Equals(role, StringComparison.OrdinalIgnoreCase);
			}
			// 추가 규칙: Team:, OU: 등 확장 가능
			return true;
		}
	}
}
