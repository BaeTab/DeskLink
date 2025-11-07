using System.Windows;
using SWC = System.Windows.Controls; // WPF Controls 별칭
using DeskLink.Core.ViewModels;

namespace DeskLink.Views
{
	/// <summary>
	/// 간단한 코드비하인드: 검색 버튼 이벤트만 VM 메서드 호출로 연결
	/// </summary>
	public partial class MainView : SWC.UserControl
	{
		public MainView()
		{
			InitializeComponent();
		}

		private async void OnSearchClick(object sender, RoutedEventArgs e)
		{
			if (DataContext is MainVm vm)
			{
				vm.Query = ((SWC.TextBox)SearchBox).Text;
				await vm.RefreshAsync();
			}
		}
	}
}
