using System.Windows.Media;
using System.Threading.Tasks;

namespace DeskLink.Core.Abstractions
{
 public interface IIconService
 {
 ImageSource? GetIcon(string? key);
 }
}
