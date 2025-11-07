using Serilog;
using System;
using System.IO;

namespace DeskLink.Core.Logging
{
	/// <summary>
	/// Serilog 전역 설정 도우미
	/// </summary>
	public static class LogConfig
	{
		public static ILogger Configure()
		{
			// 로그 폴더 결정 (로컬 AppData)
			var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
			"DeskLink", "logs");
			Directory.CreateDirectory(dir);
			var path = Path.Combine(dir, "desklink-.log");
			Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Information()
			.Enrich.FromLogContext()
			.WriteTo.Async(c => c.File(
			path,
			rollingInterval: RollingInterval.Day,
			retainedFileCountLimit: 10,
			shared: true))
			.CreateLogger();
			return Log.Logger;
		}
	}
}
