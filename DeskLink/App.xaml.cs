using DevExpress.Xpf.Core;
using Serilog;
using System;
using System.Windows;
using DeskLink.Core.Logging;

namespace DeskLink
{
	/// <summary>
	/// 앱 진입점. 로깅 및 테마 초기화
	/// </summary>
	public partial class App : System.Windows.Application
	{
		static App()
		{
			// 경량 테마 사용
			CompatibilitySettings.UseLightweightThemes = true;
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			// Serilog 구성
			LogConfig.Configure();
			Log.Information("DeskLink 시작");
			// 기본 테마 적용
			ThemeManager.ApplicationThemeName = Theme.Office2019BlackName;
		}

		protected override void OnExit(ExitEventArgs e)
		{
			Log.Information("DeskLink 종료");
			Log.CloseAndFlush();
			base.OnExit(e);
		}
	}
}
