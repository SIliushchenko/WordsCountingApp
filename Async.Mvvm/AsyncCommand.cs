using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Async.Mvvm;

public class AsyncCommand<TResult> : AsyncCommandBase, INotifyPropertyChanged
{
    private readonly Func<CancellationToken, Task<TResult>> _command;
    private readonly CancelAsyncCommand _cancelCommand;
    private NotifyTaskCompletion<TResult>? _execution;

    public AsyncCommand(Func<CancellationToken, Task<TResult>> command)
    {
        _command = command;
        _cancelCommand = new CancelAsyncCommand();
    }

    public override bool CanExecute(object? parameter)
    {
        return Execution == null || Execution.IsCompleted;
    }

    public override async Task ExecuteAsync(object? parameter)
    {
        _cancelCommand.NotifyCommandStarting();
        Execution = new NotifyTaskCompletion<TResult>(_command(_cancelCommand.Token));
        RaiseCanExecuteChanged();
        await Execution.TaskCompletion;
        _cancelCommand.NotifyCommandFinished();
        RaiseCanExecuteChanged();
    }

    public ICommand CancelCommand => _cancelCommand;

    public NotifyTaskCompletion<TResult>? Execution
    {
        get => _execution;
        private set
        {
            _execution = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        var handler = PropertyChanged;
        handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private sealed class CancelAsyncCommand : ICommand
    {
        private CancellationTokenSource _cts = new();
        private bool _commandExecuting;

        public CancellationToken Token => _cts.Token;

        public void NotifyCommandStarting()
        {
            _commandExecuting = true;
            if (!_cts.IsCancellationRequested)
                return;
            _cts = new CancellationTokenSource();
            RaiseCanExecuteChanged();
        }

        public void NotifyCommandFinished()
        {
            _commandExecuting = false;
            RaiseCanExecuteChanged();
        }

        bool ICommand.CanExecute(object? parameter)
        {
            return _commandExecuting && !_cts.IsCancellationRequested;
        }

        void ICommand.Execute(object? parameter)
        {
            _cts.Cancel();
            RaiseCanExecuteChanged();
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        private void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}