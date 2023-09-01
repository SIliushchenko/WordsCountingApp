using Shell;

namespace WordsCountingApp.ViewModels;

public class ShellViewModel : BindableBase, IShell
{
    private object? _content;
    private string? _statusText;

    public object? Content
    {
        get => _content;
        private set
        {
            _content = value;
        }
    }

    public string? StatusText
    {
        get => _statusText;
        set
        {
            _statusText = value;
            OnPropertyChanged();
        }
    }

    public void SetContent(BindableBase content)
    {
        Content = content;
    }
}