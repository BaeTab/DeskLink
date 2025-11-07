namespace DeskLink.Core.Settings
{
	/// <summary>
	/// ¾Û ¼³Á¤ ¸ðµ¨ (¿ä¾à)
	/// </summary>
	public class AppSettings
	{
		public string Theme { get; set; } = "Office2019Black";
		public Hotkeys Hotkeys { get; set; } = new Hotkeys();
		public HealthCheckOptions HealthCheck { get; set; } = new HealthCheckOptions();
		public SyncOptions Sync { get; set; } = new SyncOptions();
		public string CurrentRole { get; set; } = "Viewer"; // ¿ªÇÒ
	}

	public class Hotkeys { public string QuickOpen { get; set; } = "Alt+Space"; }
	public class HealthCheckOptions { public int IntervalMinutes { get; set; } = 10; public int TimeoutMs { get; set; } = 3000; }
	public class SyncOptions { public string Mode { get; set; } = "Pull"; public string SharedPath { get; set; } = @"\\\\NAS\\DeskLink\\repo.json"; }
}
