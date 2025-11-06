using System;
using System.Windows;

namespace DeskLink.Core.Abstractions
{
 public interface IShortcutService
 {
 event Action QuickOpenRequested;
 void RegisterQuickOpen(Window window, string gesture); // e.g. "Alt+Space"
 void UnregisterQuickOpen(Window window);
 }
}
