using System;
using System.Runtime.InteropServices;
using System.Windows;
using DeskLink.Core.Abstractions;

namespace DeskLink.Infrastructure.Services
{
	/// <summary>
	/// 전역 단축키(Alt+Space 등) 등록/해제. RegisterHotKey Win32 API 사용.
	/// </summary>
	public class ShortcutService : IShortcutService
	{
		private const int WM_HOTKEY = 0x0312;
		private int _id = 1;
		public event Action? QuickOpenRequested;

		[DllImport("user32.dll")] private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
		[DllImport("user32.dll")] private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		private static (uint mod, uint vk) ParseGesture(string gesture)
		{
			// 간이 파서: Alt+Space 만 우선 지원
			return (0x0001, 0x20); // MOD_ALT, VK_SPACE
		}

		public void RegisterQuickOpen(Window window, string gesture)
		{
			var (mod, vk) = ParseGesture(gesture);
			var source = (System.Windows.Interop.HwndSource)System.Windows.PresentationSource.FromVisual(window)!;
			var handle = source.Handle;
			RegisterHotKey(handle, _id, mod, vk);
			source.AddHook(HookProc);
		}
		public void UnregisterQuickOpen(Window window)
		{
			var source = (System.Windows.Interop.HwndSource)System.Windows.PresentationSource.FromVisual(window)!;
			UnregisterHotKey(source.Handle, _id);
			source.RemoveHook(HookProc);
		}

		private IntPtr HookProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == WM_HOTKEY && wParam.ToInt32() == _id)
			{
				handled = true;
				QuickOpenRequested?.Invoke();
			}
			return IntPtr.Zero;
		}
	}
}
