using System.Threading.Tasks;
using System.Windows.Input;

namespace Async.Mvvm;

public interface IAsyncCommand : ICommand
{
    Task ExecuteAsync(object? parameter);
}