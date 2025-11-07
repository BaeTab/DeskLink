using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DeskLink.Core.Abstractions;
using DeskLink.Core.Models;
using Serilog;

namespace DeskLink.Infrastructure.Services
{
	/// <summary>
	/// 링크 실행 서비스 구현 - URL/파일/폴더/EXE/RDP 등 기본 실행 규칙 처리
	/// - 실행 전 존재 여부/접근성 검사로 사용자 친화적 오류 처리
	/// - 폴더는 explorer.exe로 열어 호환성 확보(UNC 포함)
	/// </summary>
	public class LinkLauncher : ILinkLauncher
	{
		public Task LaunchAsync(LinkItem item, CancellationToken ct = default)
		{
			try
			{
				switch (item.Type)
				{
					case LinkType.Url:
						return LaunchUrlAsync(item.Target);
					case LinkType.File:
						return LaunchFileAsync(item.Target);
					case LinkType.Folder:
						return LaunchFolderAsync(item.Target);
					case LinkType.Exe:
						return LaunchExeAsync(item);
					case LinkType.Rdp:
						return LaunchRdpAsync(item.Target);
					case LinkType.Ssh:
					case LinkType.Custom:
						return LaunchCustomAsync(item);
					default:
						return Task.CompletedTask;
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex, "링크 실행 실패: {Name} ({Target})", item.Name, item.Target);
				System.Windows.MessageBox.Show($"실행 중 오류가 발생했습니다.\n{ex.Message}", "실행 오류", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
				return Task.CompletedTask;
			}
		}

		// URL 실행 (기본 브라우저)
		private Task LaunchUrlAsync(string target)
		{
			if (!Uri.TryCreate(target, UriKind.Absolute, out var _))
			{
				System.Windows.MessageBox.Show("잘못된 URL 입니다.", "실행 오류", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
				return Task.CompletedTask;
			}
			Process.Start(new ProcessStartInfo
			{
				FileName = target,
				UseShellExecute = true,
				Verb = "open"
			});
			return Task.CompletedTask;
		}

		// 파일 실행 (연결 프로그램)
		private Task LaunchFileAsync(string path)
		{
			if (!File.Exists(path))
			{
				System.Windows.MessageBox.Show($"파일을 찾을 수 없습니다.\n{path}", "실행 오류", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
				return Task.CompletedTask;
			}
			Process.Start(new ProcessStartInfo
			{
				FileName = path,
				UseShellExecute = true,
				Verb = "open"
			});
			return Task.CompletedTask;
		}

		// 폴더 열기 (Explorer)
		private Task LaunchFolderAsync(string path)
		{
			if (!Directory.Exists(path))
			{
				System.Windows.MessageBox.Show($"폴더에 접근할 수 없습니다.\n{path}", "실행 오류", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
				return Task.CompletedTask;
			}
			// explorer.exe로 명시적으로 열기 (UNC 포함 안정적)
			Process.Start(new ProcessStartInfo
			{
				FileName = "explorer.exe",
				Arguments = $"\"{path}\"",
				UseShellExecute = true
			});
			return Task.CompletedTask;
		}

		// 실행 파일 실행
		private Task LaunchExeAsync(LinkItem item)
		{
			if (!File.Exists(item.Target))
			{
				System.Windows.MessageBox.Show($"실행 파일을 찾을 수 없습니다.\n{item.Target}", "실행 오류", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
				return Task.CompletedTask;
			}
			var psi = new ProcessStartInfo
			{
				FileName = item.Target,
				Arguments = item.Arguments ?? string.Empty,
				UseShellExecute = true
			};
			if (!string.IsNullOrWhiteSpace(item.WorkingDir))
				psi.WorkingDirectory = item.WorkingDir!;
			Process.Start(psi);
			return Task.CompletedTask;
		}

		// RDP 파일 열기
		private Task LaunchRdpAsync(string path)
		{
			if (!File.Exists(path))
			{
				System.Windows.MessageBox.Show($"RDP 파일을 찾을 수 없습니다.\n{path}", "실행 오류", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
				return Task.CompletedTask;
			}
			Process.Start(new ProcessStartInfo
			{
				FileName = path,
				UseShellExecute = true,
				Verb = "open"
			});
			return Task.CompletedTask;
		}

		// 사용자 지정/SSH 등: 프로토콜/외부 클라이언트
		private Task LaunchCustomAsync(LinkItem item)
		{
			// 프로토콜 형식이면 그대로 open, 아니면 파일/폴더 존재 시도
			if (Uri.TryCreate(item.Target, UriKind.Absolute, out var _))
			{
				Process.Start(new ProcessStartInfo { FileName = item.Target, UseShellExecute = true, Verb = "open" });
				return Task.CompletedTask;
			}
			if (File.Exists(item.Target)) return LaunchFileAsync(item.Target);
			if (Directory.Exists(item.Target)) return LaunchFolderAsync(item.Target);
			System.Windows.MessageBox.Show($"대상을 찾을 수 없습니다.\n{item.Target}", "실행 오류", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
			return Task.CompletedTask;
		}
	}
}
