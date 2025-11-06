using DevExpress.Xpf.Core;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DeskLink.Core.Models;

namespace DeskLink.Views
{
 public partial class QuickOpenWindow : ThemedWindow
 {
 public IReadOnlyList<LinkItem> Items { get; set; } = new List<LinkItem>();
 public LinkItem? Selected { get; private set; }
 public QuickOpenWindow()
 {
 InitializeComponent();
 }
 public void LoadItems(IEnumerable<LinkItem> items)
 {
 Items = items.ToList();
 Grid.ItemsSource = Items;
 if (Items.Any()) Grid.View.FocusedRowHandle =0;
 }
 private void OnTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
 {
 var text = Search.Text?.ToLowerInvariant() ?? string.Empty;
 var filtered = Items.Where(i => (i.Name?.ToLowerInvariant().Contains(text) ?? false) || (i.Target?.ToLowerInvariant().Contains(text) ?? false)).ToList();
 Grid.ItemsSource = filtered;
 }
 private void OnDoubleClick(object sender, MouseButtonEventArgs e)
 {
 Selected = (LinkItem?)Grid.SelectedItem;
 DialogResult = true;
 }
 }
}
