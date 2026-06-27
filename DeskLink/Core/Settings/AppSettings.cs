namespace DeskLink.Core.Settings
{
	/// <summary>
	/// 앱 설정 모델 (요약)
	/// </summary>
	public class AppSettings
	{
		public string Theme { get; set; } = "Office2019Black";
		public Hotkeys Hotkeys { get; set; } = new Hotkeys();
		public HealthCheckOptions HealthCheck { get; set; } = new HealthCheckOptions();
		public SyncOptions Sync { get; set; } = new SyncOptions();
		public string CurrentRole { get; set; } = "Viewer"; // 역할
	}

	public class Hotkeys { public string QuickOpen { get; set; } = "Alt+Space"; }
	public class HealthCheckOptions { public int IntervalMinutes { get; set; } = 10; public int TimeoutMs { get; set; } = 3000; }
	public class SyncOptions { public string Mode { get; set; } = "Pull"; public string SharedPath { get; set; } = @"\\NAS\DeskLink\repo.json"; }
}
